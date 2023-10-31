using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using PeopleDesk.Services.Cache;
using System.IdentityModel.Tokens.Jwt;

namespace PeopleDesk.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private ILogger<RateLimitMiddleware> _logger;
        private ICacheService _iCacheService;

        public RateLimitMiddleware(RequestDelegate next, ICacheService iCacheService, IConfiguration configuration, ILogger<RateLimitMiddleware> logger)
        {
            _configuration = configuration;
            _next = next;
            _logger = logger;
            _iCacheService = iCacheService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string path = httpContext.Request.Path;

            if (path == "/api/Auth/Login"
                || path == "/api/Auth/GetLoginOTP"
                || path == "/api/Auth/GenerateRefreshToken"
                || path == "/api/AuthApps/Login"
                || path == "/api/AuthApps/GenerateRefreshToken"
                || path == "/api/Document/DownloadFile")
            {
                await _next(httpContext);
            }
            else
            {
                var isPostOrGet = httpContext.Request.Method;
                JObject payload = new JObject
                {
                    ["error"] = "invalid request",
                    ["status"] = 429,
                    ["error_description"] = "we are extremely sorry to let you know that you cant perform this operation; Until your before request finished.",
                };

                if (isPostOrGet != HttpMethod.Get.ToString())
                {
                    var clientId = httpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                    var handler = new JwtSecurityTokenHandler();
                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        var jsonToken = handler.ReadToken(clientId);
                        var tokenS = jsonToken as JwtSecurityToken;
                        if (tokenS != null)
                        {
                            string query = httpContext.Request.Path + "-" + tokenS.Claims.First(claim => claim.Type == "loginId").Value;
                            if (_iCacheService.hasCache(query))
                            {
                                var data = _iCacheService.DataFromCache<CallType>(query);
                                _logger.LogInformation("Rate_Limit_Expired_User_Name:" + tokenS.Claims.First(claim => claim.Type == "loginId").Value + " Host:" + httpContext.Request.Host + " Body:"
                              + httpContext.Request.Path + " QueryString:" + httpContext.Request.QueryString, DateTime.UtcNow);

                                httpContext.Response.StatusCode = 429; // i.e. 400 works
                                httpContext.Response.ContentType = "application/json";
                                await httpContext.Response.WriteAsync(payload.ToString());
                            }
                            else
                            {
                                var data = new List<CallType>();
                                data.Add(new CallType() { url = httpContext.Request.Path, User = tokenS.Claims.First(claim => claim.Type == "loginId").Value });
                                var SetAbsoluteExpiration = int.Parse(_configuration.GetSection("RateLimitCacheExpiration")["SetAbsoluteExpiration"]);
                                var SetSlidingExpiration = int.Parse(_configuration.GetSection("RateLimitCacheExpiration")["SetSlidingExpiration"]);
                                _iCacheService.AddDataToCache<CallType>(query, data, SetAbsoluteExpiration, SetSlidingExpiration);
                            }
                        }
                    }
                }

                await _next(httpContext);
            }
        }
    }

    public class CallType
    {
        public string url { get; set; }
        public string User { get; set; }
    }
}