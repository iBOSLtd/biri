using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Services.Cache;
using System.IdentityModel.Tokens.Jwt;

namespace PeopleDesk.Middlewares
{
    public class UserValidityCheckedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public UserValidityCheckedMiddleware(RequestDelegate next, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string path = httpContext.Request.Path;
            JObject payload = new JObject
            {
                ["error"] = "invalid user",
                ["status"] = 406,
                ["error_description"] = "User has been In-Actived",
            };

            if (path == "/api/Auth/Login"
                || path == "/api/Auth/GetLoginOTP"
                || path == "/api/Auth/GenerateRefreshToken"
                || path == "/api/AuthApps/Login"
                || path == "/api/AuthApps/GenerateRefreshToken"
                || path == "/api/AuthApps/GetUrlByUser"
                || path == "/api/Document/DownloadFile")
            {
                await _next(httpContext);
            }
            else
            {
                var clientId = httpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    JwtSecurityTokenHandler handler = new();
                    JwtSecurityToken jsonToken = (JwtSecurityToken)handler.ReadToken(clientId);

                    if (jsonToken != null)
                    {
                        string query = jsonToken.Claims.First(claim => claim.Type == "loginId").Value;

                        using (IServiceScope scope = _serviceProvider.CreateScope())
                        {
                            PeopleDeskContext _context = scope.ServiceProvider.GetService<PeopleDeskContext>();
                            User user = await _context.Users.FirstOrDefaultAsync(x => x.StrLoginId == query);

                            if (user == null || user.IsActive == false)
                            {
                                httpContext.Response.StatusCode = 406;
                                httpContext.Response.ContentType = "application/json";

                                await httpContext.Response.WriteAsync(payload.ToString());
                            }
                            else
                            {
                                await _next(httpContext);
                            }
                        }
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 406;
                        httpContext.Response.ContentType = "application/json";
                        await httpContext.Response.WriteAsync(payload.ToString());
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
        }
    }
}