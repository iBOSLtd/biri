using PeopleDesk.Data;
using PeopleDesk.Models.Global;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;

namespace PeopleDesk.Extensions
{
    public static class AuthExtension
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static async Task<BaseVM> GetDataFromJwtToken(GetDataFromJwtTokenRequestType reqType, PayloadIsAuthorizeVM? isAuthorizeVM, PermissionLebelCheck? permissionLebel)
        {
            BaseVM baseVM = new BaseVM();
            string token = null;

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {

                string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (authorizationHeader.StartsWith("Bearer "))
                {
                    token = authorizationHeader.Substring("Bearer ".Length).Trim();
                }

                if (!string.IsNullOrEmpty(token))
                {

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);

                    string businessUnit = jwt.Claims.FirstOrDefault(x => x.Type == "businessUnitList").Value;
                    string workplaceGroup = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceGroupList").Value;
                    string workplace = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceList").Value;
                    string wings = jwt.Claims.FirstOrDefault(x => x.Type == "wingList").Value;
                    string spoleDepos = jwt.Claims.FirstOrDefault(x => x.Type == "soleDepoList").Value;
                    string regions = jwt.Claims.FirstOrDefault(x => x.Type == "regionList").Value;
                    string areas = jwt.Claims.FirstOrDefault(x => x.Type == "areaList").Value;
                    string territories = jwt.Claims.FirstOrDefault(x => x.Type == "territoryList").Value;

                    var emptyList = new List<long>();

                    List<long> businessUnitList = !string.IsNullOrEmpty(businessUnit) ? businessUnit.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> workplaceGroupList = !string.IsNullOrEmpty(workplaceGroup) ? workplaceGroup.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> workplaceList = !string.IsNullOrEmpty(workplace) ? workplace.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> wingList = !string.IsNullOrEmpty(wings) ? wings.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> soleDepoList = !string.IsNullOrEmpty(spoleDepos) ? spoleDepos.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> regionList = !string.IsNullOrEmpty(regions) ? regions.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> areaList = !string.IsNullOrEmpty(areas) ? areas.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;
                    List<long> territoryList = !string.IsNullOrEmpty(territories) ? territories.Split(',').Select(x => long.Parse(x)).ToList() : emptyList;


                    if (reqType == GetDataFromJwtTokenRequestType.TokenData)
                    {
                        baseVM.accountId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "accountId")?.Value);
                        baseVM.businessUnitId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "businessUnitId")?.Value);
                        baseVM.workplaceGroupId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "workplaceGroupId")?.Value);
                        baseVM.loginId = jwt.Claims.FirstOrDefault(x => x.Type == "loginId")?.Value;
                        baseVM.employeeId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "employeeId")?.Value);
                        baseVM.isOfficeAdmin = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOfficeAdmin")?.Value);
                        baseVM.isOwner = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOwner")?.Value);
                        baseVM.isSupNLMORManagement = Convert.ToInt32(jwt.Claims.FirstOrDefault(x => x.Type == "isSupNLMORManagement")?.Value);
                        baseVM.businessUnitList = businessUnitList.ConvertAll(x => (long?)x);
                        baseVM.workplaceGroupList = workplaceGroupList.ConvertAll(x => (long?)x);
                        baseVM.workplaceList = workplaceList.ConvertAll(x => (long?)x);
                        baseVM.wingList = wingList.ConvertAll(x => (long?)x);
                        baseVM.soleDepoList = soleDepoList.ConvertAll(x => (long?)x);
                        baseVM.regionList = regionList.ConvertAll(x => (long?)x);
                        baseVM.areaList = areaList.ConvertAll(x => (long?)x);
                        baseVM.territoryList = territoryList.ConvertAll(x => (long?)x);
                    }
                    else if (reqType == GetDataFromJwtTokenRequestType.Authorization)
                    {
                        dynamic dataList = new { businessUnitList, workplaceGroupList, wingList, soleDepoList, regionList, areaList, territoryList };

                        baseVM.isAuthorize = isAuthorizeVM != null && dataList != null && permissionLebel != null ? await IsAuthorize(isAuthorizeVM, dataList, permissionLebel) : false;
                    }
                    else if (reqType == GetDataFromJwtTokenRequestType.TokenDataIfAuthorize)
                    {
                        dynamic dataList = new { businessUnitList, workplaceGroupList, wingList, soleDepoList, regionList, areaList, territoryList };

                        baseVM.isAuthorize = isAuthorizeVM != null && dataList != null && permissionLebel != null ? await IsAuthorize(isAuthorizeVM, dataList, permissionLebel) : false;

                        if (baseVM.isAuthorize)
                        {
                            baseVM.accountId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "accountId")?.Value);
                            baseVM.loginId = jwt.Claims.FirstOrDefault(x => x.Type == "loginId")?.Value;
                            baseVM.employeeId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "employeeId")?.Value);
                            baseVM.businessUnitId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "businessUnitId")?.Value);
                            baseVM.isOfficeAdmin = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOfficeAdmin")?.Value);
                            baseVM.isOwner = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOwner")?.Value);
                            baseVM.isSupNLMORManagement = Convert.ToInt32(jwt.Claims.FirstOrDefault(x => x.Type == "isSupNLMORManagement")?.Value);
                            baseVM.businessUnitList = businessUnitList.ConvertAll(x => (long?)x);
                            baseVM.workplaceGroupList = workplaceGroupList.ConvertAll(x => (long?)x);
                            baseVM.workplaceList = workplaceList.ConvertAll(x => (long?)x);
                            baseVM.wingList = wingList.ConvertAll(x => (long?)x);
                            baseVM.soleDepoList = soleDepoList.ConvertAll(x => (long?)x);
                            baseVM.regionList = regionList.ConvertAll(x => (long?)x);
                            baseVM.areaList = areaList.ConvertAll(x => (long?)x);
                            baseVM.territoryList = territoryList.ConvertAll(x => (long?)x);
                        }
                    }

                }

            }

            return baseVM;
        }

        public static async Task<bool> IsAuthorize(PayloadIsAuthorizeVM? isAuthorizeVM, dynamic dataList, PermissionLebelCheck? permissionLebel)
        {
            bool isAuthorize = false;

            if (isAuthorizeVM.isOfficeAdmin)
            {
                isAuthorize = true;
            }
            else if (dataList?.businessUnitList?.Contains(0))
            {
                isAuthorize = true;
            }
            else if (dataList?.businessUnitList?.Contains(isAuthorizeVM.businessUnitId))
            {
                if (dataList?.workplaceGroupList.Contains(0) || permissionLebel == PermissionLebelCheck.BusinessUnit)
                {
                    isAuthorize = true;
                }
                else if (dataList?.workplaceGroupList?.Contains(isAuthorizeVM.workplaceGroupId))
                {
                    if (dataList?.wingList.Contains(0) || permissionLebel == PermissionLebelCheck.WorkplaceGroup)
                    {
                        isAuthorize = true;
                    }
                    else if (dataList?.wingList.Contains(isAuthorizeVM.wingId))
                    {
                        if (dataList?.soleDepoList.Contains(0) || permissionLebel == PermissionLebelCheck.Wing)
                        {
                            isAuthorize = true;
                        }
                        else if (dataList?.soleDepoList.Contains(isAuthorizeVM.soleDepoId))
                        {
                            if (dataList?.regionList.Contains(0) || permissionLebel == PermissionLebelCheck.SoleDepo)
                            {
                                isAuthorize = true;
                            }
                            else if (dataList?.regionList.Contains(isAuthorizeVM.regionId))
                            {
                                if (dataList?.areaList.Contains(0) || permissionLebel == PermissionLebelCheck.Region)
                                {
                                    isAuthorize = true;
                                }
                                else if (dataList?.areaList.Contains(isAuthorizeVM.areaId))
                                {
                                    if (dataList?.territoryList.Contains(0) || permissionLebel == PermissionLebelCheck.Area)
                                    {
                                        isAuthorize = true;
                                    }
                                    else if (dataList?.territoryList.Contains(isAuthorizeVM.territoryId))
                                    {
                                        isAuthorize = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isAuthorize;
        }

        public static async Task<dynamic> GetDataFromJwtToken()
        {
            string token = null;

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {

                string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (authorizationHeader.StartsWith("Bearer "))
                {
                    token = authorizationHeader.Substring("Bearer ".Length).Trim();
                }

                if (!string.IsNullOrEmpty(token))
                {

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);

                    
                    return new
                    {
                        loginId = jwt.Claims.FirstOrDefault(x => x.Type == "loginId")?.Value,
                        employeeId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "employeeId")?.Value),
                        accountId = Convert.ToInt64(jwt.Claims.FirstOrDefault(x => x.Type == "accountId")?.Value),
                        isOfficeAdmin = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOfficeAdmin")?.Value),
                        isOwner = Convert.ToBoolean(jwt.Claims.FirstOrDefault(x => x.Type == "isOwner")?.Value),
                        isSupNLMORManagement = Convert.ToInt32(jwt.Claims.FirstOrDefault(x => x.Type == "isSupNLMORManagement")?.Value),
                        businessUnit = jwt.Claims.FirstOrDefault(x => x.Type == "businessUnitList").Value,
                        workplaceGroup = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceGroupList").Value,
                        workplace = jwt.Claims.FirstOrDefault(x => x.Type == "workplaceList").Value,
                        wings = jwt.Claims.FirstOrDefault(x => x.Type == "wingList").Value,
                        soleDepos = jwt.Claims.FirstOrDefault(x => x.Type == "soleDepoList").Value,
                        regions = jwt.Claims.FirstOrDefault(x => x.Type == "regionList").Value,
                        areas = jwt.Claims.FirstOrDefault(x => x.Type == "areaList").Value,
                        territories = jwt.Claims.FirstOrDefault(x => x.Type == "territoryList").Value
                    };

                }

            }

            return null;
        }

    }
}
