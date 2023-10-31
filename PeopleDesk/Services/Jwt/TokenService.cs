using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SAAS.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PeopleDesk.Services.Jwt
{
    public class TokenService : ITokenService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PeopleDeskContext _context;
        public TokenService(IServiceScopeFactory scopeFactory, IHttpContextAccessor _httpContextAccessor, PeopleDeskContext context)
        {
            this.scopeFactory = scopeFactory;
            this._httpContextAccessor = _httpContextAccessor;
            _context = context;
        }

        public string BuildToken(string key, string issuer, User user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, user.StrLoginId),
                //new Claim(ClaimTypes.Role, user.nam),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddMonths(1), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<Token> GenerateJSONWebToken(string key, string issuer, User user, long? empId, bool? isOfficeAdmin, bool? isOwner, int? isSupNLMORManagement, string? businessUnitList, string? workplaceGroupList, string? workplaceList,
            string? wings, string? soleDepos, String? regions, string? areas, string? territories, long? businessUnitId, long? workplaceGroupId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("accountId", user.IntAccountId.ToString()),
                new Claim("businessUnitId", businessUnitId.ToString()),
                new Claim("workplaceGroupId", workplaceGroupId.ToString()),
                new Claim("loginId", user.StrLoginId),
                new Claim("employeeId", empId.ToString()),
                new Claim("isOfficeAdmin",isOfficeAdmin.ToString()),
                new Claim("isOwner", isOwner.ToString()),
                new Claim("businessUnitList", businessUnitList),
                new Claim("workplaceGroupList", workplaceGroupList),
                new Claim("workplaceList", workplaceList),
                new Claim("wingList", wings),
                new Claim("soleDepoList", soleDepos),
                new Claim("regionList", regions),
                new Claim("areaList", areas),
                new Claim("territoryList", territories),
            };

            var token = new JwtSecurityToken(issuer, issuer, claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new Token()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };
        }

        public bool IsTokenValid(string key, string issuer, string token)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext;
                var headers = _httpContextAccessor.HttpContext.Request.Headers;

                var mySecret = Encoding.UTF8.GetBytes(key);
                var mySecurityKey = new SymmetricSecurityKey(mySecret);

                var deSerializeToken = new JwtSecurityToken(token);
                var claims = deSerializeToken.Claims;

                if (claims != null && claims.Any())
                {
                    User user = new User();
                    string username = claims.First().Value;
                    //DateTime expiredDateTime = Convert.ToDateTime(deSerializeToken.ValidTo.Date.Date + " " + deSerializeToken.ValidTo.TimeOfDay);

                    if (!string.IsNullOrEmpty(user.StrLoginId))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        try
                        {
                            tokenHandler.ValidateToken(token, new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidIssuer = issuer,
                                ValidAudience = issuer,
                                IssuerSigningKey = mySecurityKey,
                                LifetimeValidator = LifetimeValidator
                            }, out SecurityToken validatedToken);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            if (expires != null)
            {
                if (expires > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //GetPrincipalFromExpiredToken
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string key)
        {
            var Key = Encoding.UTF8.GetBytes(key);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async Task<Token> GenerateNewToken(Token token, string key, string issuer)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token.AccessToken, key);
                User user = new();
                user.StrLoginId = principal.Identity?.Name;

                RefreshTokenHistory history = await _context.RefreshTokenHistories.FirstOrDefaultAsync(x => x.StrRefreshToken == token.RefreshToken && x.StrLogInId == user.StrLoginId && x.IsActive == true);

                if (history != null)
                {
                    if (history.StrRefreshToken != token.RefreshToken)
                    {
                        return new Token();
                    }
                    _context.RefreshTokenHistories.RemoveRange(history);
                }

                BaseVM baseVM = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                var data = await GenerateJSONWebToken(
                    key, issuer, user, baseVM.employeeId, baseVM.isOfficeAdmin, baseVM.isOwner, baseVM.isSupNLMORManagement,
                    (baseVM.businessUnitList.Count > 0 ? string.Join(",", baseVM.businessUnitList) : string.Empty),
                    (baseVM.workplaceGroupList.Count > 0 ? string.Join(",", baseVM.workplaceGroupList) : string.Empty),
                    (baseVM.workplaceList.Count > 0 ? string.Join(",", baseVM.workplaceList) : string.Empty),
                    (baseVM.wingList.Count > 0 ? string.Join(",", baseVM.wingList) : string.Empty),
                    (baseVM.soleDepoList.Count > 0 ? string.Join(",", baseVM.soleDepoList) : string.Empty),
                    (baseVM.regionList.Count > 0 ? string.Join(",", baseVM.regionList) : string.Empty),
                    (baseVM.areaList.Count > 0 ? string.Join(",", baseVM.areaList) : string.Empty),
                    (baseVM.territoryList.Count > 0 ? string.Join(",", baseVM.territoryList) : string.Empty),
                    baseVM.businessUnitId,
                    baseVM.workplaceGroupId
                    );

                if (!string.IsNullOrEmpty(data.RefreshToken))
                {
                    RefreshTokenHistory newHistory = new()
                    {
                        IntAutoId = 0,
                        StrLogInId = user.StrLoginId,
                        StrRefreshToken = data.RefreshToken,
                        DteCreatedAt = DateTime.UtcNow,
                        IsActive = true,
                    };

                    await _context.RefreshTokenHistories.AddAsync(newHistory);

                    int res = await _context.SaveChangesAsync();
                }

                return new Token()
                {
                    AccessToken = data.AccessToken,
                    RefreshToken = data.RefreshToken
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GenerateRefreshToken
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //updateOrCreateRefreshToken
        public async Task<bool> UpdateRefreshToken(string strLoginId, string strRefreshToken)
        {
            try
            {
                List<RefreshTokenHistory> histories = await _context.RefreshTokenHistories.Where(x => x.StrRefreshToken == strRefreshToken && x.StrLogInId == strLoginId && x.IsActive == true).ToListAsync();

                if (histories != null)
                {
                    _context.RefreshTokenHistories.RemoveRange(histories);
                }
                RefreshTokenHistory newHistory = new()
                {
                    IntAutoId = 0,
                    StrLogInId = strLoginId,
                    StrRefreshToken = strRefreshToken,
                    DteCreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };

                await _context.RefreshTokenHistories.AddAsync(newHistory);

                int res = await _context.SaveChangesAsync();

                return res > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}