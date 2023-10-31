using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Text;

namespace PeopleDesk.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAppsController : ControllerBase
    {
        private readonly PeopleDeskContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenService _tokenService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private readonly iEmailService _iEmailService;

        public AuthAppsController(IApprovalPipelineService _approvalPipelineService, PeopleDeskContext _context, iEmailService _iEmailService, ITokenService _tokenService, IConfiguration _configuration, IEmployeeService _employeeService)
        {
            this._configuration = _configuration;
            _employeeService = _employeeService;
            this._context = _context;
            this._tokenService = _tokenService;
            this._iEmailService = _iEmailService;
            this._approvalPipelineService = _approvalPipelineService;
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                MessageHelper res = new MessageHelper();

                if (!string.IsNullOrEmpty(model.StrLoginId) && !string.IsNullOrEmpty(model.StrPassword))
                {
                    var Password = Encoding.UTF8.GetBytes(model.StrPassword);
                    string encryptedPassword = Convert.ToBase64String(Password);

                    Url url = await _context.Urls.FirstOrDefaultAsync(x => x.StrUrl.ToLower() == model.StrUrl.ToLower());

                    if (url != null)
                    {
                        User user = await _context.Users.Where(x => x.StrLoginId == model.StrLoginId && x.IntUrlId == url.IntUrlId
                                    && x.StrPassword == encryptedPassword && x.IsActive == true).FirstOrDefaultAsync();

                        if (user == null)
                        {
                            res.Message = "invalid user please try again";
                            res.StatusCode = 0;
                            return Ok(res);
                        }
                        else
                        {

                            EmpEmployeeBasicInfo employeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == user.IntRefferenceId);
                            EmpEmployeeBasicInfoDetail employeeBasicInfoDetails = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == user.IntRefferenceId);
                            MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.FirstOrDefaultAsync(x => x.IntBusinessUnitId == employeeBasicInfo.IntBusinessUnitId);
                            EmpEmployeePhotoIdentity employeePhotoIdentity = await _context.EmpEmployeePhotoIdentities.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == employeeBasicInfo.IntEmployeeBasicInfoId);
                            MasterDepartment department = await _context.MasterDepartments.FirstOrDefaultAsync(x => x.IntDepartmentId == employeeBasicInfo.IntDepartmentId);
                            MasterDesignation designation = await _context.MasterDesignations.FirstOrDefaultAsync(x => x.IntDesignationId == employeeBasicInfo.IntDesignationId);
                            Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.IntAccountId == user.IntAccountId);
                            EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await _approvalPipelineService.EmployeeIsSupervisorNLineManagerORUserGroupMember(employeeBasicInfo.IntAccountId, employeeBasicInfo.IntEmployeeBasicInfoId);


                            #region =================ROLE EXTENSION=============
                          
                            var RoleEx = await _employeeService.GetEmployeeRoleExtensions(employeeBasicInfo.IntEmployeeBasicInfoId, employeeBasicInfo.IntAccountId);


                            string? businessUnitList = RoleEx.BusinessUnitList.Count > 0 ? string.Join(",", RoleEx.BusinessUnitList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? workplaceGroupList = RoleEx.WorkGroupList.Count > 0 ? string.Join(",", RoleEx.WorkGroupList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? workplaceList = RoleEx.WorkPlaceList.Count > 0 ? string.Join(",", RoleEx.WorkPlaceList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? WingList = RoleEx.WingList.Count > 0 ? string.Join(",", RoleEx.WingList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? soleDepoList = RoleEx.SoleDepoList.Count > 0 ? string.Join(",", RoleEx.SoleDepoList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? regionList = RoleEx.RegiontList.Count > 0 ? string.Join(",", RoleEx.RegiontList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? areaList = RoleEx.AreaList.Count > 0 ? string.Join(",", RoleEx.AreaList.Select(x => x.Value.ToString()).ToList()) : string.Empty;
                            string? territoryList = RoleEx.TeritoryList.Count > 0 ? string.Join(",", RoleEx.TeritoryList.Select(x => x.Value.ToString()).ToList()) : string.Empty;

                            #endregion =======================END==============================


                            var data = await _tokenService.GenerateJSONWebToken(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString(), user, employeeBasicInfo.IntEmployeeBasicInfoId,
                                user.IsOfficeAdmin, user.IsOwner, (user.IsOfficeAdmin == true ? 3 : isSupNLMORUGMemberViewModel.IsSupervisor ? 1 : isSupNLMORUGMemberViewModel.IsLineManager ? 2 : 0),
                                businessUnitList, workplaceGroupList, workplaceList, WingList, soleDepoList, regionList, areaList, territoryList, employeeBasicInfo.IntBusinessUnitId, employeeBasicInfo.IntWorkplaceGroupId);


                            LoginReturnViewModel returnObj = new LoginReturnViewModel
                            {
                                StrLoginId = user.StrLoginId,
                                IntAccountId = user.IntAccountId,
                                IntUrlId = user.IntUrlId,
                                IntBusinessUnitId = employeeBasicInfo.IntBusinessUnitId,
                                StrBusinessUnit = businessUnit.StrBusinessUnit,
                                IntEmployeeId = employeeBasicInfo.IntEmployeeBasicInfoId,
                                StrDisplayName = user.StrDisplayName,
                                IntProfileImageUrl = employeePhotoIdentity != null ? employeePhotoIdentity.IntProfilePicFileUrlId : 0,
                                IntLogoUrlId = businessUnit.StrLogoUrlId,
                                IntDefaultDashboardId = 0,
                                IntDepartmentId = employeeBasicInfo.IntDepartmentId,
                                StrDepartment = department != null ? department.StrDepartment : "",
                                IntDesignationId = employeeBasicInfo.IntDesignationId,
                                StrDesignation = designation != null ? designation.StrDesignation : "",
                                IntUserTypeId = user.IntUserTypeId,
                                //IntUserType =
                                IntRefferenceId = user.IntRefferenceId,
                                IsOfficeAdmin = user.IsOfficeAdmin,
                                IsSuperuser = user.IsSuperuser,
                                DteLastLogin = DateTime.Now,
                                IsLoggedIn = true,
                                Token = data.AccessToken,
                                RefreshToken = data.RefreshToken,
                                IsLoggedInWithOtp = account.IsLoggedInWithOtp,
                                StrOfficeMail = employeeBasicInfoDetails == null ? "" : employeeBasicInfoDetails.StrOfficeMail,
                                StrPersonalMail = employeeBasicInfoDetails == null ? "" : employeeBasicInfoDetails.StrPersonalMail,
                                IsSupNLMORManagement = user.IsOfficeAdmin == true ? 3 : isSupNLMORUGMemberViewModel.IsSupervisor ? 1 : isSupNLMORUGMemberViewModel.IsLineManager ? 2 : 0
                            };

                            if (!string.IsNullOrEmpty(returnObj.RefreshToken))
                            {
                                RefreshTokenHistory newHistory = new()
                                {
                                    IntAutoId = 0,
                                    StrLogInId = user.StrLoginId,
                                    StrRefreshToken = data.RefreshToken,
                                    DteCreatedAt = DateTime.UtcNow,
                                    IsActive = true
                                };

                                await _context.RefreshTokenHistories.AddAsync(newHistory);
                                await _context.SaveChangesAsync();
                            }

                            return Ok(returnObj);
                        }
                    }
                    else
                    {
                        res.Message = "Invalid Url";
                        res.StatusCode = 0;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "invalid Data";
                    res.StatusCode = 0;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //GeneRate Refresh Token
        [AllowAnonymous]
        [Route("GenerateRefreshToken")]
        [HttpPost]
        public async Task<IActionResult> GenerateRefreshToken(Token token)
        {
            try
            {
                return Ok(await _tokenService.GenerateNewToken(token, _configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //CheckUserDatatBase
        [AllowAnonymous]
        [HttpGet("GetUrlByUser")]
        public async Task<IActionResult> GetUrlByUser(string StrLoginId, string StrPassword)
        {
            try
            {
                var domain = await (from gu in _context.GlobalUserUrls
                                    join url in _context.Urls on gu.IntUrlId equals url.IntUrlId
                                    where gu.IsActive == true && url.IsActive == true && gu.StrLoginId.ToLower() == StrLoginId.ToLower()
                                    select url).FirstOrDefaultAsync();

                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = "Not Found"
                };

                return domain != null ? Ok(domain) : BadRequest(msg);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
    }
}