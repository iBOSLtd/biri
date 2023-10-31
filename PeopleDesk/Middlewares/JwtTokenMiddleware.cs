using LanguageExt;
using PeopleDesk.Data;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.Cache;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PeopleDesk.Middlewares
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCacheService _distributedCacheService;

        public JwtTokenMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IDistributedCacheService distributedCacheService)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _distributedCacheService = distributedCacheService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string token = null;

            //var serviceProvider = context.RequestServices;
            //var _distributedCacheService = serviceProvider.GetRequiredService<>();

            if (context.Request.Headers.ContainsKey("Authorization"))
            {

                string authorizationHeader = context.Request.Headers["Authorization"];
                if (authorizationHeader.StartsWith("Bearer "))
                {
                    token = authorizationHeader.Substring("Bearer ".Length).Trim();
                }

                if(!string.IsNullOrEmpty(token))
                {

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);

                    string businessUnit = jwt.Claims.FirstOrDefault(x => x.Type == "businessUnitList").Value;
                    string workplaceGroup = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceGroupList").Value;
                    string workplace = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceList").Value;

                    List<long> businessUnitList = !string.IsNullOrEmpty(businessUnit) ? businessUnit.Split(',').Select(x => long.Parse(x)).ToList() : null;
                    List<long> workplaceGroupList = !string.IsNullOrEmpty(workplaceGroup) ? workplaceGroup.Split(',').Select(x => long.Parse(x)).ToList() : null;
                    List<long> workplaceList = !string.IsNullOrEmpty(workplace) ? workplace.Split(',').Select(x => long.Parse(x)).ToList() : null;

                    BaseVM baseVM = new BaseVM
                    {
                       accountId =  Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "accountId")?.Value),
                       loginId = jwt.Claims.FirstOrDefault(x => x.Type == "loginId")?.Value,
                       employeeId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "employeeId")?.Value),
                       isOfficeAdmin = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOfficeAdmin")?.Value),
                       isOwner = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOwner")?.Value),
                       businessUnitList = businessUnitList != null ? businessUnitList.ConvertAll(x => (long?)x) : null,
                       workplaceGroupList = workplaceGroupList != null ? businessUnitList.ConvertAll(x => (long?)x) : null,
                       workplaceList = workplaceList != null ? workplaceList.ConvertAll(x => (long?)x) : null
                    };

                    List<BaseVM> listData = new List<BaseVM>();
                    listData.Add(baseVM);
                    string baseUrl = _httpContextAccessor.HttpContext.Request.Host.Value;

                    string key = baseUrl + "_" + baseVM.accountId.ToString() + "_" + baseVM.loginId.ToString() + "_" + baseVM.employeeId.ToString();

                    if (!await _distributedCacheService.HasCache(key))
                    {
                        await _distributedCacheService.AddDataToCache(key, listData, 2, 2, CasheExperiationTypeEnum.minute.ToString());
                    }
                }

            }
            

            await _next(context);
        }
    }
}
