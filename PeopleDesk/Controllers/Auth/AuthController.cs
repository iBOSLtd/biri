using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.Cache;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
using System.Text;
using User = PeopleDesk.Data.Entity.User;

namespace PeopleDesk.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PeopleDeskContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IMasterService _masterService;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenService _tokenService;
        private readonly iEmailService _iEmailService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private ReturnVM responseData = new ReturnVM();

        public AuthController(IApprovalPipelineService _approvalPipelineService, iEmailService _iEmailService, ITokenService _tokenService,
            IAuthService _authService, PeopleDeskContext _context, IConfiguration _configuration, IMasterService _masterService, IEmployeeService _employeeService)
        {
            this._configuration = _configuration;
            this._context = _context;
            this._authService = _authService;
            this._masterService = _masterService;
            this._employeeService = _employeeService;
            this._tokenService = _tokenService;
            this._iEmailService = _iEmailService;
            this._approvalPipelineService = _approvalPipelineService;

        }

        [Route("EnCode")]
        [HttpGet]
        public ActionResult<ReturnVM> EnCoding(string password)
        {
            try
            {
                byte[] Password = Encoding.UTF8.GetBytes(password);
                string encryptedPassword = Convert.ToBase64String(Password);

                responseData.StatusCode = 200;
                responseData.Message = "success";
                responseData.Data = encryptedPassword;
            }
            catch (Exception ex)
            {
                responseData.StatusCode = 500;
                responseData.Message = ex.Message;
            }

            return responseData;
        }

        [Route("DeCode")]
        [HttpGet]
        public IActionResult DeCoding(string password)
        {
            var Password = System.Convert.FromBase64String(password);
            string encryptedPassword = Encoding.UTF8.GetString(Password);

            return Ok(encryptedPassword);
        }

        [HttpPost]
        [Route("AccountCreateUpdate")]
        public async Task<IActionResult> AccountCreateUpdate(AccountCreateViewModel model)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();

                if (model.IntUrlId > 0 && model.StrAccountName is not null && model.StrOwnerName is not null && model.StrMobileNumber is not null
                    && model.StrCurrency is not null && model.StrAccountPackageName is not null && model.IntMinEmployee > 0 && model.IntMaxEmployee > 0
                    && model.NumPrice > 0 && model.StrShortCode is not null)
                {
                    Account Account = new Account
                    {
                        IntAccountId = model.IntAccountId,
                        StrAccountName = model.StrAccountName,
                        StrShortCode = model.StrShortCode,
                        StrOwnerName = model.StrOwnerName,
                        StrAddress = model.StrAddress,
                        StrMobileNumber = model.StrMobileNumber,
                        StrNid = model.StrNid,
                        StrBin = model.StrBin,
                        StrEmail = model.StrEmail,
                        StrWebsite = model.StrWebsite,
                        IntLogoUrlId = model.IntLogoUrlId,
                        IntUrlId = model.IntUrlId,
                        IntCountryId = 18,
                        StrCurrency = model.StrCurrency,
                        IsBlock = false,
                        IsProvidentFund = true,
                        IsTax = true,
                        IsLoan = true,
                        IsActive = true,
                        //DteExpireDate = DateTime.Now.AddYears(1),
                        IntAccountPackageId = model.IntAccountPackageId,
                        StrAccountPackageName = model.StrAccountPackageName,
                        IntMinEmployee = model.IntMinEmployee,
                        IntMaxEmployee = model.IntMaxEmployee,
                        NumPrice = model.NumPrice,
                        NumPackageFileStorageQuaota = model.NumPackageFileStorageQuaota,
                        IsFree = model.IsFree,
                        DteDateOfOnboard = DateTime.Now,
                        NumFileStorageQuaota = model.NumFileStorageQuaota,
                        IsLoggedInWithOtp = model.IsLoggedInWithOtp
                        //IntCreatedBy = 1,
                        //DteCreatedAt = DateTime.Now,
                        //IntUpdatedBy = 1,
                        //DteUpdatedAt = DateTime.Now,
                    };

                    if (model.IntAccountId > 0)
                    {
                        Account.DteUpdatedAt = DateTime.Now;
                        Account.IntUpdatedBy = model.IntUpdatedBy;

                        await _masterService.SaveAccount(Account);

                        res.Message = "Update Successfully !!!";
                        return Ok(res);
                    }
                    else
                    {
                        long accountNameIsExists = await _context.Accounts.Where(x => x.StrAccountName.ToLower() == model.StrAccountName.ToLower()).CountAsync();
                        long accountCodeIsExists = await _context.Accounts.Where(x => x.StrShortCode.ToLower() == model.StrShortCode.ToLower()).CountAsync();
                        long accountNumberIsExists = await _context.Accounts.Where(x => x.StrMobileNumber.ToLower() == model.StrMobileNumber.ToLower()).CountAsync();

                        if (accountNameIsExists > 0 && accountCodeIsExists > 0 && accountNumberIsExists > 0)
                        {
                            res.StatusCode = 401;
                            res.Message = "Account Already Exists with this Account Name, Code and Mobile Number !!!";
                            return BadRequest(res);
                        }
                        else if (accountNameIsExists > 0 && accountCodeIsExists > 0)
                        {
                            res.StatusCode = 401;
                            res.Message = "Account Already Exists with this Account Name, Code !!!";
                            return BadRequest(res);
                        }
                        else if (accountNumberIsExists > 0)
                        {
                            res.StatusCode = 401;
                            res.Message = "Account Already Exists with this Mobile Number !!!";
                            return BadRequest(res);
                        }
                        else if (accountNameIsExists > 0)
                        {
                            res.StatusCode = 401;
                            res.Message = "Account Already Exists with this Account Name !!!";
                            return BadRequest(res);
                        }
                        else if (accountCodeIsExists > 0)
                        {
                            res.StatusCode = 401;
                            res.Message = "Account Already Exists with this Account Code !!!";
                            return BadRequest(res);
                        }
                        else
                        {
                            Account.DteCreatedAt = DateTime.Now;
                            Account.IntCreatedBy = model.IntCreatedBy;
                            Account obj = await _masterService.SaveAccount(Account);

                            if (obj == null)
                            {
                                res.Message = "Creation Failed !!!";
                                return BadRequest(res);
                            }
                            else
                            {
                                List<UserRole> userRole = new List<UserRole>();

                                userRole.Add(new UserRole
                                {
                                    IntRoleId = 0,
                                    StrRoleName = "Default",
                                    IntAccountId = obj.IntAccountId,
                                    IsDefault = true,
                                    IsActive = true,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = obj.IntCreatedBy
                                });
                                userRole.Add(new UserRole
                                {
                                    IntRoleId = 0,
                                    StrRoleName = "Administration",
                                    IntAccountId = obj.IntAccountId,
                                    IsDefault = false,
                                    IsActive = true,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = obj.IntCreatedBy
                                });
                                userRole.Add(new UserRole
                                {
                                    IntRoleId = 0,
                                    StrRoleName = "Owner",
                                    IntAccountId = obj.IntAccountId,
                                    IsDefault = false,
                                    IsActive = true,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = obj.IntCreatedBy
                                });

                                await _context.UserRoles.AddRangeAsync(userRole);
                                await _context.SaveChangesAsync();

                                #region BusinessUnit

                                MasterBusinessUnit BusinessUnit = new MasterBusinessUnit
                                {
                                    StrBusinessUnit = model.StrAccountName,
                                    StrShortCode = model.StrShortCode.Length > 4 ? model.StrShortCode.Substring(0, 4) : model.StrShortCode,
                                    StrAddress = model.StrAddress,
                                    StrLogoUrlId = model.IntLogoUrlId,
                                    IntDistrictId = 18,
                                    IsActive = true,
                                    IntAccountId = obj.IntAccountId,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy
                                };

                                await _context.MasterBusinessUnits.AddRangeAsync(BusinessUnit);
                                await _context.SaveChangesAsync();

                                #endregion BusinessUnit

                                #region MasterWorkplace

                                MasterWorkplaceGroup WorkplaceGroup = new MasterWorkplaceGroup
                                {
                                    StrWorkplaceGroup = model.StrAccountName,
                                    StrWorkplaceGroupCode = model.StrShortCode.Length > 3 ? model.StrShortCode.Substring(0, 3) : model.StrShortCode,
                                    IsActive = true,
                                    IntAccountId = obj.IntAccountId
                                };

                                await _context.MasterWorkplaceGroups.AddRangeAsync(WorkplaceGroup);
                                await _context.SaveChangesAsync();

                                MasterWorkplace Workplace = new MasterWorkplace
                                {
                                    StrWorkplace = model.StrAccountName,
                                    StrWorkplaceCode = model.StrShortCode.Length > 3 ? model.StrShortCode.Substring(0, 3) : model.StrShortCode,
                                    StrAddress = model.StrAddress,
                                    IntDistrictId = 18,
                                    IntWorkplaceGroupId = WorkplaceGroup.IntWorkplaceGroupId,
                                    StrWorkplaceGroup = WorkplaceGroup.StrWorkplaceGroup,
                                    IntBusinessUnitId = BusinessUnit.IntBusinessUnitId,
                                    IsActive = true,
                                    IntAccountId = obj.IntAccountId,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy
                                };

                                await _context.MasterWorkplaces.AddRangeAsync(Workplace);
                                await _context.SaveChangesAsync();

                                #endregion MasterWorkplace

                                List<MasterEmploymentType> masterEmploymentTypeList = await _context.MasterEmploymentTypes.AsNoTracking().Where(x => x.IntAccountId == 0).OrderBy(x => x.IntEmploymentTypeId).AsQueryable().ToListAsync();
                                List<MasterEmploymentType> employmentTypeList = new List<MasterEmploymentType>();

                                if (masterEmploymentTypeList == null || masterEmploymentTypeList.Count() <= 0)
                                {
                                    employmentTypeList.Add(new MasterEmploymentType
                                    {
                                        StrEmploymentType = "Permanent",
                                        IsActive = true,
                                        IntAccountId = 0,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = (long)model.IntCreatedBy
                                    });
                                    employmentTypeList.Add(new MasterEmploymentType
                                    {
                                        StrEmploymentType = "Probationary",
                                        IsActive = true,
                                        IntAccountId = 0,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = (long)model.IntCreatedBy
                                    });
                                    employmentTypeList.Add(new MasterEmploymentType
                                    {
                                        StrEmploymentType = "Intern",
                                        IsActive = true,
                                        IntAccountId = 0,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = (long)model.IntCreatedBy
                                    });
                                    employmentTypeList.Add(new MasterEmploymentType
                                    {
                                        StrEmploymentType = "Contractual",
                                        IsActive = true,
                                        IntAccountId = 0,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = (long)model.IntCreatedBy
                                    });

                                    await _context.MasterEmploymentTypes.AddRangeAsync(employmentTypeList);
                                    await _context.SaveChangesAsync();

                                    employmentTypeList = new List<MasterEmploymentType>();
                                    masterEmploymentTypeList = await _context.MasterEmploymentTypes.AsNoTracking().Where(x => x.IntAccountId == 0).OrderBy(x => x.IntEmploymentTypeId).AsQueryable().ToListAsync();
                                }

                                masterEmploymentTypeList.ForEach(type =>
                                {
                                    employmentTypeList.Add(new MasterEmploymentType
                                    {
                                        IntParentId = type.IntEmploymentTypeId,
                                        StrEmploymentType = type.StrEmploymentType,
                                        IsActive = true,
                                        IntAccountId = Account.IntAccountId,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = (long)model.IntCreatedBy
                                    });
                                });

                                await _context.MasterEmploymentTypes.AddRangeAsync(employmentTypeList);
                                await _context.SaveChangesAsync();

                                List<LveLeaveType> masterLeaveTypeList = await _context.LveLeaveTypes.Where(x => x.IntAccountId == 0).OrderBy(x => x.IntLeaveTypeId).AsNoTracking().ToListAsync();
                                List<LveLeaveType> leaveTypeList = new List<LveLeaveType>();

                                if (masterEmploymentTypeList == null || masterLeaveTypeList.Count <= 0)
                                {
                                    leaveTypeList.Add(new LveLeaveType
                                    {
                                        IntParentId = 0,
                                        StrLeaveType = "Casual Leave",
                                        StrLeaveTypeCode = "CL",
                                        IntAccountId = 0,
                                        IsActive = true,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = model.IntCreatedBy
                                    });
                                    leaveTypeList.Add(new LveLeaveType
                                    {
                                        IntParentId = 0,
                                        StrLeaveType = "Sick Leave",
                                        StrLeaveTypeCode = "SL",
                                        IntAccountId = 0,
                                        IsActive = true,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = model.IntCreatedBy
                                    });
                                    leaveTypeList.Add(new LveLeaveType
                                    {
                                        IntParentId = 0,
                                        StrLeaveType = "Maternity Leave",
                                        StrLeaveTypeCode = "MTL",
                                        IntAccountId = 0,
                                        IsActive = true,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = model.IntCreatedBy
                                    });
                                    leaveTypeList.Add(new LveLeaveType
                                    {
                                        IntParentId = 0,
                                        StrLeaveType = "Paternity Leave",
                                        StrLeaveTypeCode = "PTL",
                                        IntAccountId = 0,
                                        IsActive = true,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = model.IntCreatedBy
                                    });

                                    await _context.LveLeaveTypes.AddRangeAsync(leaveTypeList);
                                    await _context.SaveChangesAsync();

                                    masterLeaveTypeList = await _context.LveLeaveTypes.Where(x => x.IntAccountId == 0).OrderBy(x => x.IntLeaveTypeId).AsNoTracking().ToListAsync();
                                }

                                masterLeaveTypeList.ForEach(type =>
                                {
                                    leaveTypeList.Add(new LveLeaveType
                                    {
                                        IntParentId = type.IntLeaveTypeId,
                                        StrLeaveType = type.StrLeaveType,
                                        StrLeaveTypeCode = type.StrLeaveTypeCode,
                                        IntAccountId = Account.IntAccountId,
                                        IsActive = true,
                                        DteCreatedAt = DateTime.Now,
                                        IntCreatedBy = model.IntCreatedBy
                                    });
                                });

                                #region Employee

                                MasterEmploymentType employmentType = employmentTypeList.FirstOrDefault(x => x.StrEmploymentType.ToLower() == "permanent");
                                EmpEmployeeBasicInfo Employee = new EmpEmployeeBasicInfo
                                {
                                    StrEmployeeCode = (await _context.EmpEmployeeBasicInfos.CountAsync() + 1).ToString(),
                                    StrEmployeeName = model.StrOwnerName,
                                    IntGenderId = 1,
                                    StrGender = "Male",
                                    IsActive = true,
                                    IsUserInactive = false,
                                    IsRemoteAttendance = false,
                                    IntAccountId = obj.IntAccountId,
                                    IntEmploymentTypeId = employmentType != null ? employmentType.IntEmploymentTypeId : 0,
                                    StrEmploymentType = employmentType != null ? employmentType.StrEmploymentType : "",
                                    IntBusinessUnitId = BusinessUnit.IntBusinessUnitId,
                                    IntWorkplaceGroupId = WorkplaceGroup.IntWorkplaceGroupId,
                                    IntWorkplaceId = Workplace.IntWorkplaceId,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy,
                                    IsSalaryHold = true,
                                    DteJoiningDate = DateTime.Now
                                };
                                await _context.EmpEmployeeBasicInfos.AddRangeAsync(Employee);
                                await _context.SaveChangesAsync();

                                EmpEmployeeBasicInfoDetail EmployeeBasicInfoDetail = new EmpEmployeeBasicInfoDetail
                                {
                                    IntEmployeeId = Employee.IntEmployeeBasicInfoId,
                                    StrPersonalMail = model.StrEmail,
                                    StrPersonalMobile = model.StrMobileNumber,
                                    StrEmployeeStatus = "Active",
                                    IntEmployeeStatusId = 1,
                                    IsActive = true,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy
                                };
                                _context.EmpEmployeeBasicInfoDetails.Add(EmployeeBasicInfoDetail);
                                await _context.SaveChangesAsync();

                                #endregion Employee

                                #region User

                                User isExists = await _authService.GetUserIsExists(model.IntUrlId, model.IntAccountId, model.StrMobileNumber);

                                if (isExists != null)
                                {
                                    int count = await _context.Users.Where(x => x.IntUrlId == model.IntUrlId && x.IntAccountId == model.IntAccountId && x.StrLoginId == model.StrMobileNumber).CountAsync();
                                    model.StrMobileNumber = count.ToString() + model.StrMobileNumber;
                                }
                                var Password = Encoding.UTF8.GetBytes(model.StrMobileNumber);
                                string encryptedPassword = Convert.ToBase64String(Password);
                                User user = new User
                                {
                                    StrLoginId = model.StrMobileNumber,
                                    StrPassword = encryptedPassword,
                                    StrDisplayName = model.StrOwnerName,
                                    IntUserTypeId = 1,
                                    IntRefferenceId = Employee.IntEmployeeBasicInfoId,
                                    IsOfficeAdmin = true,
                                    IsActive = true,
                                    IntUrlId = model.IntUrlId,
                                    IntAccountId = obj.IntAccountId,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy
                                };

                                User CreatedUser = await _authService.SaveUser(user);

                                Employee.StrReferenceId = Employee.IntEmployeeBasicInfoId.ToString();
                                Employee.IntSupervisorId = Employee.IntEmployeeBasicInfoId;
                                Employee.IntDottedSupervisorId = Employee.IntDottedSupervisorId;
                                Employee.IntLineManagerId = Employee.IntLineManagerId;
                                _context.EmpEmployeeBasicInfos.Update(Employee);
                                await _context.SaveChangesAsync();

                                obj.IntOwnerEmployeeId = Employee.IntEmployeeBasicInfoId;
                                _context.Accounts.Update(obj);
                                await _context.SaveChangesAsync();

                                #region ROLE EXTENSION

                                RoleExtensionHeader roleHeader = new RoleExtensionHeader
                                {
                                    IntEmployeeId = Employee.IntEmployeeBasicInfoId,
                                    IntCreatedBy = Employee.IntEmployeeBasicInfoId,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                };
                                await _context.RoleExtensionHeaders.AddAsync(roleHeader);
                                await _context.SaveChangesAsync();

                                List<RoleExtensionRow> roleExtensionRows = new List<RoleExtensionRow>();
                                roleExtensionRows.Add(new RoleExtensionRow
                                {
                                    IntRoleExtensionHeaderId = roleHeader.IntRoleExtensionHeaderId,
                                    IntEmployeeId = roleHeader.IntEmployeeId,
                                    IntOrganizationTypeId = 1,
                                    StrOrganizationTypeName = "Business Unit",
                                    IntOrganizationReffId = 0,
                                    StrOrganizationReffName = "ALL",
                                    IntCreatedBy = roleHeader.IntCreatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });
                                roleExtensionRows.Add(new RoleExtensionRow
                                {
                                    IntRoleExtensionHeaderId = roleHeader.IntRoleExtensionHeaderId,
                                    IntEmployeeId = roleHeader.IntEmployeeId,
                                    IntOrganizationTypeId = 2,
                                    StrOrganizationTypeName = "Workplace Group",
                                    IntOrganizationReffId = 0,
                                    StrOrganizationReffName = "ALL",
                                    IntCreatedBy = roleHeader.IntCreatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });
                                roleExtensionRows.Add(new RoleExtensionRow
                                {
                                    IntRoleExtensionHeaderId = roleHeader.IntRoleExtensionHeaderId,
                                    IntEmployeeId = roleHeader.IntEmployeeId,
                                    IntOrganizationTypeId = 3,
                                    StrOrganizationTypeName = "Workplace",
                                    IntOrganizationReffId = 0,
                                    StrOrganizationReffName = "ALL",
                                    IntCreatedBy = roleHeader.IntCreatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });

                                await _context.RoleExtensionRows.AddRangeAsync(roleExtensionRows);
                                await _context.SaveChangesAsync();

                                #endregion ROLE EXTENSION

                                //await _authService.ProvideMenuPermission(Employee.IntEmployeeBasicInfoId, (long)model.IntCreatedBy);

                                #endregion User

                                #region Role Permission

                                UserRole administrationRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.IntAccountId == obj.IntAccountId && x.StrRoleName.ToLower() == "Administration".ToLower());
                                if (administrationRole != null)
                                {
                                    RoleBridgeWithDesignation roleBridgeWith = new RoleBridgeWithDesignation
                                    {
                                        IntId = 0,
                                        IntAccountId = obj.IntAccountId,
                                        StrIsFor = "Employee",
                                        IntDesignationOrEmployeeId = Employee.IntEmployeeBasicInfoId,
                                        IntRoleId = administrationRole.IntRoleId,
                                        IntCreatedBy = (long)model.IntCreatedBy,
                                        DteCreatedDateTime = DateTime.Now,
                                        IsActive = true
                                    };
                                    await _context.RoleBridgeWithDesignations.AddAsync(roleBridgeWith);
                                    await _context.SaveChangesAsync();
                                }

                                #endregion Role Permission

                                #region ================ Menu Permission ======================

                                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                                {
                                    string sql = "auth.sprAccountBaseMenuPermission";
                                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                                    {
                                        sqlCmd.CommandType = CommandType.StoredProcedure;

                                        sqlCmd.Parameters.AddWithValue("@intAccountId", Account.IntAccountId);

                                        connection.Open();
                                        sqlCmd.ExecuteNonQuery();
                                        connection.Close();
                                    }
                                }

                                #endregion ================ Menu Permission ======================

                                return Ok(CreatedUser);
                            }
                        }
                    }
                }
                else
                {
                    res.Message = "Invalid Data !!!";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("UserCreation")]
        public async Task<IActionResult> UserCreation(CreateUserViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                MessageHelperCreate res = new MessageHelperCreate();

                if (model.IntUrlId > 0 && tokenData.accountId > 0 && model.StrLoginId is not null)
                {
                    var Password = Encoding.UTF8.GetBytes(model.StrPassword);
                    string encryptedPassword = Convert.ToBase64String(Password);

                    long isExists = await _context.Users.Where(x => x.IntAccountId == tokenData.accountId && x.StrLoginId == model.StrLoginId && x.IntUrlId == model.IntUrlId && x.IntUserId != model.IntUserId).CountAsync();

                    if (isExists > 0)
                    {
                        MessageHelper res2 = new MessageHelper
                        {
                            StatusCode = 401,
                            Message = "User Already Exists With this credentials !!!"
                        };
                        return BadRequest(res2);
                    }

                    User user = new User
                    {
                        IntUserId = (long)model.IntUserId,
                        StrLoginId = model.StrLoginId,
                        StrPassword = encryptedPassword,
                        //StrOldPassword = model.StrOldPassword,
                        StrDisplayName = model.StrDisplayName,
                        IntUserTypeId = (long)model.IntUserTypeId,
                        IntRefferenceId = model.IntRefferenceId,
                        IsOfficeAdmin = model.IsOfficeAdmin,
                        IsSuperuser = model.IsSuperuser,
                        //DteLastLogin = model.DteLastLogin
                        IsActive = model.IsActive,
                        IsOwner = (bool)model.IsOwner,
                        IntUrlId = (long)model.IntUrlId,
                        IntAccountId = (long)tokenData.accountId,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = tokenData.employeeId
                    };
                    long globalUserExists = await _context.GlobalUserUrls.Where(x => x.StrLoginId == model.StrLoginId && x.IsActive == true && x.IntUrlId == (long)model.IntUrlId).CountAsync();
                    if (globalUserExists <= 0)
                    {
                        //save into global url
                        GlobalUserUrl globalUserUrl = new()
                        {
                            StrLoginId = model.StrLoginId,
                            IntUrlId = (long)model.IntUrlId,
                            IsActive = true
                        };
                        await _context.GlobalUserUrls.AddAsync(globalUserUrl);
                    }
                    User CreatedUser = await _authService.SaveUser(user);
                    EmpEmployeeBasicInfoDetail employeeBasicInfoDetail = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == model.IntRefferenceId);
                    if (employeeBasicInfoDetail != null)
                    {
                        employeeBasicInfoDetail.StrOfficeMail = model.IntOfficeMail;
                        employeeBasicInfoDetail.StrPersonalMobile = model.StrContactNo;

                        _context.EmpEmployeeBasicInfoDetails.Update(employeeBasicInfoDetail);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        EmpEmployeeBasicInfoDetail obj = new EmpEmployeeBasicInfoDetail
                        {
                            IntEmployeeId = (long)model.IntRefferenceId,
                            StrOfficeMail = model.IntOfficeMail,
                            StrPersonalMobile = model.StrContactNo,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = tokenData.employeeId
                        };
                        await _context.EmpEmployeeBasicInfoDetails.AddAsync(obj);
                        await _context.SaveChangesAsync();
                    }
                    return Ok(CreatedUser);
                }
                else
                {

                    res.Message = "Invalid Data !!!";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UserIsExistsRemoteValidation")]
        public async Task<IActionResult> UserIsExistsRemoteValidation(CreateUserViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                MessageHelperCreate res = new MessageHelperCreate();

                if (model.IntUrlId > 0 && tokenData.accountId > 0 && model.StrLoginId is not null)
                {
                    long isExists = await _context.Users.Where(x => x.IntAccountId == tokenData.accountId && x.StrLoginId == model.StrLoginId && x.IntUrlId == model.IntUrlId).CountAsync();
                    long globalUserExists = await _context.GlobalUserUrls.Where(x => x.StrLoginId == model.StrLoginId && x.IsActive == true).CountAsync();

                    if (isExists > 0 || globalUserExists > 0)
                    {
                        res.StatusCode = 401;
                        res.Message = "Invalid";

                        return BadRequest(res);
                    }
                    else
                    {
                        res.StatusCode = 200;
                        res.Message = "Valid";
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Invalid Data !!!";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("JWTData")]
        public async Task<IActionResult> JWTData()
        {

            var data = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);


            return Ok(data);
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

                    Data.Entity.Url url = await _context.Urls.FirstOrDefaultAsync(x => x.StrUrl.ToLower() == model.StrUrl.ToLower());

                    if (url != null)
                    {
                        Data.Entity.User user = await _context.Users.Where(x => x.StrLoginId == model.StrLoginId && x.IntUrlId == url.IntUrlId
                                    && x.StrPassword == encryptedPassword && x.IsActive == true).FirstOrDefaultAsync();

                        if (user == null)
                        {
                            res.Message = "invalid user please try again";
                            return BadRequest(res);
                        }
                        else
                        {

                            EmpEmployeeBasicInfo employeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == user.IntRefferenceId);
                            EmpEmployeeBasicInfoDetail employeeBasicInfoDetails = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == user.IntRefferenceId);
                            MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.FirstOrDefaultAsync(x => x.IntBusinessUnitId == employeeBasicInfo.IntBusinessUnitId);
                            MasterWorkplaceGroup workplaceGroup = await _context.MasterWorkplaceGroups.FirstOrDefaultAsync(x => x.IntWorkplaceGroupId == employeeBasicInfo.IntWorkplaceGroupId);
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
                                IntWorkplaceGroupId = Convert.ToInt64(employeeBasicInfo.IntWorkplaceGroupId),
                                StrWorkplaceGroup = workplaceGroup != null ? workplaceGroup.StrWorkplaceGroup : "",
                                IntEmployeeId = employeeBasicInfo.IntEmployeeBasicInfoId,
                                StrDisplayName = user.StrDisplayName,
                                IntProfileImageUrl = employeePhotoIdentity != null ? employeePhotoIdentity.IntProfilePicFileUrlId : 0,
                                IntLogoUrlId = Convert.ToInt64(businessUnit.StrLogoUrlId),
                                IntDefaultDashboardId = 0,
                                IntDepartmentId = Convert.ToInt64(employeeBasicInfo.IntDepartmentId),
                                StrDepartment = department != null ? department.StrDepartment : "",
                                IntDesignationId = Convert.ToInt64(employeeBasicInfo.IntDesignationId),
                                StrDesignation = designation != null ? designation.StrDesignation : "",
                                IntUserTypeId = user.IntUserTypeId,
                                //IntUserType =
                                IntRefferenceId = Convert.ToInt64(user.IntRefferenceId),
                                IsOfficeAdmin = user.IsOfficeAdmin,
                                IsSuperuser = user.IsSuperuser,
                                IsOwner = (user.IsOwner == null || user.IsOwner == false) ? false : true,
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
                        return BadRequest(JsonConvert.SerializeObject(res));
                    }
                }
                else
                {
                    res.Message = "invalid Data";
                    return BadRequest(JsonConvert.SerializeObject(res));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [Route("GetLoginOTP")]
        [HttpGet]
        public async Task<IActionResult> GetLoginOTP(string mailAddress)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (!string.IsNullOrEmpty(mailAddress))
                {
                    Random generator = new Random();
                    message.Message = generator.Next(0, 1000000).ToString("D6");
                    message.StatusCode = 200;

                    string mailBody = "<!DOCTYPE html>" +
                                        "<html>" +
                                        "<body>" +
                                            "<div style=" + '"' + "font-size:12px" + '"' +
                                                "<p>Hi <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + mailAddress + "</a> </p>" +
                                                "<p>We received your request for a single-use code to use with your PeopleDesk account.</p>" +
                                                "<p>Your single-use code is: " + message.Message + "</p>" +
                                                "<p>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>" +
                                                "<p>" +
                                                    "Thanks, <Br /> " +
                                                    "The PeopleDesk team" +
                                                "</p>" +
                                            "</div>" +
                                        "</body>" +
                                        "</html>";

                    string res = await _iEmailService.SendEmail("iBOS", mailAddress, "", "", "PeopleDesk Login OTP", mailBody, "HTML");
                    if (res != "success")
                    {
                        message.StatusCode = 500;
                        message.Message = res;
                    }
                    return Ok(message);
                }
                else
                {
                    message.Message = "Mail Address was not passed";
                    message.StatusCode = 401;

                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.StatusCode = 500;
                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("UserLanding")]
        public async Task<IActionResult> UserLanding(long AccountId)
        {
            try
            {
                List<UserViewModel> datas = await _authService.UserLanding(AccountId);
                return Ok(datas);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetRoleGroupById")]
        public async Task<IActionResult> GetRoleGroupById(long RoleGroupId)
        {
            try
            {
                var res = await _authService.GetRoleGroupById(RoleGroupId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(long accountId, string loginId, string oldPassword, string newPassword, long updatedBy)
        {
            try
            {
                MessageHelper messageHelper = new MessageHelper();

                string encodedOLDPass = _authService.EnCoding(oldPassword);
                User user = await _context.Users.FirstOrDefaultAsync(x => x.IntAccountId == accountId && x.StrLoginId == loginId && x.StrPassword == encodedOLDPass);
                if (user != null)
                {
                    user.StrOldPassword = user.StrPassword;
                    user.StrPassword = _authService.EnCoding(newPassword);
                    user.DteUpdatedAt = DateTime.Now;
                    user.IntUpdatedBy = updatedBy;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    messageHelper.Message = "Password Updated Successfully";
                    messageHelper.StatusCode = 200;

                    return Ok(messageHelper);
                }
                else
                {
                    messageHelper.Message = "Invalid User !!!";
                    messageHelper.StatusCode = 401;

                    return BadRequest(messageHelper);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region === M E N U   &   F E A T U R E S ===

        [HttpPost]
        [Route("CRUDMenus")]
        public async Task<IActionResult> CRUDMenus(Menu Model)
        {
            try
            {
                return Ok(await _authService.CRUDMenus(Model));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetFirstLevelMenuList")]
        public async Task<IActionResult> GetFirstLevelMenuList(long employeeId)
        {
            try
            {
                var res = await _authService.GetFirstLevelMenuList(employeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetMenuFeatureList")]
        public async Task<IActionResult> GetMenuFeatureList(long firstLevelMenuId, long employeeId)
        {
            try
            {
                var res = await _authService.GetMenuFeatureList(firstLevelMenuId, employeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetMenuListPermissionWise")]
        public async Task<IActionResult> GetMenuListPermissionWise(long EmployeeId)
        {
            try
            {
                var res = await _authService.GetMenuListPermissionWise(EmployeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetMenuListPermissionWiseApps")]
        public async Task<IActionResult> GetMenuListPermissionWiseApps(long EmployeeId)
        {
            try
            {
                var res = await _authService.GetMenuListPermissionWiseApps(EmployeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("MenuPermissionAssignToRole")]
        public async Task<IActionResult> MenuPermissionAssignToRole(MenuUserPermissionForUserFeatureViewModel menuUserPermission)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = menuUserPermission.IntBusinessunitId, workplaceGroupId = menuUserPermission.IntWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _authService.CreateMenuUserPermissionForUserFeature(menuUserPermission, "Role");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MenuPermissionAssignToUser")]
        public async Task<IActionResult> MenuPermissionAssignToUser(MenuUserPermissionForUserFeatureViewModel menuUserPermission)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = menuUserPermission.IntBusinessunitId, workplaceGroupId = menuUserPermission.IntWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _authService.CreateMenuUserPermissionForUserFeature(menuUserPermission, "Employee");

                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetMenuUserPermission")]
        public async Task<IActionResult> GetMenuUserPermission(long businessUnitId, long workplaceGroupId,long EmployeeId, string isFor)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if(tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                
                var res = await _authService.GetMenuUserPermission(EmployeeId, isFor);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message});
            }
        }

        [HttpGet]
        [Route("GetMenuUserPermissionForActivityCheck")]
        public async Task<IActionResult> GetMenuUserPermissionForActivityCheck(long employeeId)
        {
            try
            {
                var res = await _authService.GetMenuUserPermissionForActivityCheck(employeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion === M E N U   &   F E A T U R E S ===

        #region U S E R - - - G R O U P

        [HttpPost]
        [Route("UserGroupCreateNUpdate")]
        public async Task<IActionResult> UserGroupCreateNUpdate(UserGroupHeaderViewModel model)
        {
            try
            {
                MessageHelper messageHelper = new MessageHelper();
                if (model.IntUserGroupHeaderId > 0 && !string.IsNullOrEmpty(model.StrUserGroup))
                {
                    UserGroupHeader groupHeader = await _context.UserGroupHeaders.FirstOrDefaultAsync(x => x.IntUserGroupHeaderId == model.IntUserGroupHeaderId);
                    if (groupHeader == null)
                    {
                        messageHelper.StatusCode = 500;
                        messageHelper.Message = "User Group Not Found !!!";
                        return BadRequest(messageHelper);
                    }
                    else
                    {
                        List<UserGroupRow> removeUserGroupRows = model.UserGroupRowViewModelList.Where(x => x.IsDelete == true)
                                                                                                .Select(y => new UserGroupRow
                                                                                                {
                                                                                                    IntUserGroupRowId = y.IntUserGroupRowId,
                                                                                                    IsActive = false
                                                                                                }).ToList();

                        List<UserGroupRow> createUserGroupRows = model.UserGroupRowViewModelList.Where(x => x.IsCreate == true)
                                                                            .Select(y => new UserGroupRow
                                                                            {
                                                                                IntUserGroupHeaderId = groupHeader.IntUserGroupHeaderId,
                                                                                IntEmployeeId = y.IntEmployeeId,
                                                                                StrEmployeeName = y.StrEmployeeName,
                                                                                IsActive = true,
                                                                                IntCreatedBy = model.IntCreatedBy,
                                                                                DteCreatedAt = DateTime.Now
                                                                            }).ToList();
                        groupHeader.StrUserGroup = model.StrUserGroup;
                        groupHeader.StrRemarks = model.StrRemarks;
                        groupHeader.StrCode = model.StrCode;
                        groupHeader.DteUpdatedAt = DateTime.Now;
                        groupHeader.IntUpdatedBy = model.IntUpdatedBy;

                        _context.UserGroupHeaders.Update(groupHeader);
                        _context.UserGroupRows.RemoveRange(removeUserGroupRows);
                        await _context.UserGroupRows.AddRangeAsync(createUserGroupRows);

                        await _context.SaveChangesAsync();

                        messageHelper.StatusCode = 200;
                        messageHelper.Message = "Update Successfully";

                        return Ok(messageHelper);
                    }
                }
                else if (!string.IsNullOrEmpty(model.StrUserGroup))
                {
                    UserGroupHeader groupHeader = new UserGroupHeader
                    {
                        IntAccountId = model.IntAccountId,
                        StrUserGroup = model.StrUserGroup,
                        StrCode = model.StrCode,
                        StrRemarks = model.StrRemarks,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = model.IntCreatedBy
                    };
                    await _context.UserGroupHeaders.AddAsync(groupHeader);
                    await _context.SaveChangesAsync();

                    List<UserGroupRow> createUserGroupRows = model.UserGroupRowViewModelList.Where(x => x.IsCreate == true)
                                                                           .Select(y => new UserGroupRow
                                                                           {
                                                                               IntUserGroupHeaderId = groupHeader.IntUserGroupHeaderId,
                                                                               IntEmployeeId = y.IntEmployeeId,
                                                                               StrEmployeeName = y.StrEmployeeName,
                                                                               IsActive = true,
                                                                               IntCreatedBy = model.IntCreatedBy,
                                                                               DteCreatedAt = DateTime.Now
                                                                           }).ToList();
                    await _context.UserGroupRows.AddRangeAsync(createUserGroupRows);
                    await _context.SaveChangesAsync();

                    messageHelper.StatusCode = 200;
                    messageHelper.Message = "Create Successfully";

                    return Ok(messageHelper);
                }
                else
                {
                    messageHelper.StatusCode = 401;
                    messageHelper.Message = "User Group Is Required !!!";
                    return BadRequest(messageHelper);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("UserGroupById")]
        public async Task<IActionResult> UserGroupById(long userGroupHeaderId)
        {
            try
            {
                UserGroupViewModel data = new UserGroupViewModel
                {
                    UserGroupHeader = await _context.UserGroupHeaders.FirstOrDefaultAsync(x => x.IntUserGroupHeaderId == userGroupHeaderId),
                    UserGroupRows = await _context.UserGroupRows.Where(x => x.IntUserGroupHeaderId == userGroupHeaderId).ToListAsync()
                };

                return Ok(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllUserGroupByAccountId")]
        public async Task<IActionResult> GetAllUserGroupByAccountId(int PageSize, int PageNo, string? searchTxt)
        {
            try
            {

                var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);


                IQueryable<UserGroupHeader> userGroupHeaders = _context.UserGroupHeaders.Where(x => (x.IntAccountId == tD.accountId || x.IntAccountId == 0) && x.IsActive == true).OrderByDescending(X => X.IntUserGroupHeaderId).AsNoTracking().AsQueryable();

                int maxSize = 1000;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                UserGroupPaginationViewModel userGroup = new();

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                    userGroupHeaders = userGroupHeaders.Where(x => x.StrUserGroup.ToLower().Contains(searchTxt) || x.StrCode.ToLower().Contains(searchTxt));
                }

                userGroup.TotalCount = await userGroupHeaders.CountAsync();
                userGroup.Data = await userGroupHeaders.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                userGroup.PageSize = PageSize;
                userGroup.CurrentPage = PageNo;

                return Ok(userGroup);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetUserList")]
        public async Task<IActionResult> GetUserList(long businessUnitId, long workplaceGroupId, string? Search)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId}, PermissionLebelCheck.WorkplaceGroup);

                if(tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _authService.GetUserList(tokenData, businessUnitId, workplaceGroupId, Search);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError());
            }
        }

        [HttpPost]
        [Route("CreateRoleGroup")]
        public async Task<IActionResult> CreateRoleGroup(RoleGroupCommongViewModel obj)
        {
            try
            {
                var res = await _authService.CreateRoleGroup(obj);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetRoleGroupLanding")]
        public async Task<IActionResult> GetRoleGroupLanding(long AccountId, long BusinessUnitId)
        {
            try
            {
                var res = await _authService.GetRoleGroupLanding(AccountId, BusinessUnitId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("RoleAssignToUser")]
        public async Task<IActionResult> RoleAssignToUser(RoleAssignToUserVM model)
        {
            MessageHelper helper = new MessageHelper { StatusCode = 401, Message = "Something went wrong" };
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, (long)model.BusinessUnitId, (long)model.WorkplaceGroupId, model.EmployeeId, null);

                if(isValid.Count() > 0)
                {
                    EmpEmployeeBasicInfo empEmployeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == model.EmployeeId);
                    if (empEmployeeBasicInfo != null)
                    {
                        List<RoleBridgeWithDesignation> Roles = await _context.RoleBridgeWithDesignations
                            .Where(x => (x.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntDesignationId && x.StrIsFor.ToLower() == "Designation")
                            || (x.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntEmployeeBasicInfoId && x.StrIsFor.ToLower() == "Employee")
                            && x.IsActive == true).ToListAsync();

                        List<long?> roleBridgeWithDesignationIdList = Roles.Select(x => x.IntRoleId).ToList();

                        List<RoleBridgeWithDesignation> empRoleBridgeWithDesignationIdList = await _context.RoleBridgeWithDesignations
                                    .Where(x => x.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntEmployeeBasicInfoId && x.StrIsFor.ToLower() == "Employee"
                                    && x.IsActive == true).ToListAsync();

                        List<long?> empIdList = empRoleBridgeWithDesignationIdList.Select(x => x.IntRoleId).ToList();

                        List<RoleBridgeWithDesignation> roleBridgeWithDesignations = new List<RoleBridgeWithDesignation>();

                        foreach (var item in model.RoleIdList)
                        {
                            Roles.RemoveAll(r => r.IntRoleId == item);
                            if (!roleBridgeWithDesignationIdList.Contains(item))
                            {
                                roleBridgeWithDesignations.Add(new RoleBridgeWithDesignation
                                {
                                    IntId = 0,
                                    IntAccountId = (long)model.AccountId,
                                    StrIsFor = "Employee",
                                    IntDesignationOrEmployeeId = model.EmployeeId,
                                    IntRoleId = item,
                                    IntCreatedBy = (long)model.UpdatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });
                            }
                            if (empIdList.Contains(item))
                            {
                                empRoleBridgeWithDesignationIdList.Remove(empRoleBridgeWithDesignationIdList.FirstOrDefault(x => x.IntRoleId == item));
                            }
                        }
                        if (roleBridgeWithDesignations.Count() > 0)
                        {
                            await _context.RoleBridgeWithDesignations.AddRangeAsync(roleBridgeWithDesignations);
                            await _context.SaveChangesAsync();
                        }
                        else if (empRoleBridgeWithDesignationIdList.Count() > 0)
                        {
                            empRoleBridgeWithDesignationIdList.ForEach(item =>
                            {
                                item.IsActive = false;
                                item.IntUpdatedBy = model.UpdatedBy;
                                item.DteUpdateDateTime = DateTime.Now;
                            });
                            _context.RoleBridgeWithDesignations.UpdateRange(empRoleBridgeWithDesignationIdList);
                            await _context.SaveChangesAsync();
                        }

                        _context.RoleBridgeWithDesignations.RemoveRange(Roles);
                        await _context.SaveChangesAsync();

                        helper.StatusCode = 200;
                        helper.Message = "Assigned Successfull";
                    }

                    return Ok(helper);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message});
            }
        }

        [HttpPost]
        [Route("RoleAssignToUserById")]
        public async Task<IActionResult> RoleAssignToUserById(long businessUnitId, long workplaceGroupId,  long employeeId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, employeeId, null);

                if(isValid.Count() > 0)
                {
                    EmpEmployeeBasicInfo empEmployeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == employeeId);
                    if (empEmployeeBasicInfo != null)
                    {
                        List<RoleValuLabelVM> data = await (from bridge in _context.RoleBridgeWithDesignations
                                                            where ((bridge.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntDesignationId && bridge.StrIsFor.ToLower() == "Designation")
                                                             || (bridge.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntEmployeeBasicInfoId && bridge.StrIsFor.ToLower() == "Employee"))
                                                             && bridge.IsActive == true
                                                            join role in _context.UserRoles on bridge.IntRoleId equals role.IntRoleId
                                                            select new RoleValuLabelVM
                                                            {
                                                                Value = role.IntRoleId,
                                                                Label = role.StrRoleName
                                                            }).ToListAsync();

                        return Ok(data);
                    }
                    else
                    {
                        return Ok(new List<RoleValuLabelVM>());
                    }
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        #endregion U S E R - - - G R O U P

        #region ===== Role Extension ==============

        [HttpPost]
        [Route("RoleExtensionCreateOrUpdate")]
        public async Task<IActionResult> RoleExtensionCreateOrUpdate(RollExtensionViewModel rollExtensionViewModel)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRoleExtensionCreateUpdate";
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(rollExtensionViewModel.RoleExtensionRowViewModelList);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", rollExtensionViewModel.intEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", rollExtensionViewModel.intCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                var dataTbl = JsonConvert.SerializeObject(dt);
                return Ok(dataTbl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllRoleExtensionLanding")]
        public async Task<IActionResult> GetAllRoleExtensionLanding(string strReportType, long? intEmployeeId, long? intAccountId, long? intWorkplaceGroupId, int? PageNo, int? PageSize, string? SearchText)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRoleExtensionAllSelectQuery";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strReportType", strReportType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", intAccountId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
                        sqlCmd.Parameters.AddWithValue("@strSearchText", SearchText);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                var dataTbl = JsonConvert.SerializeObject(dt);
                return Ok(dataTbl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllRoleExtensionAssignedEmployeeLanding")]
        public async Task<IActionResult> GetAllRoleExtensionAssignedEmployeeLanding(long businessUnitId, long intWorkplaceGroupId, int? PageNo, int? PageSize, string? SearchText)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var data = (from roleEx in _context.RoleExtensionHeaders
                            join emp1 in _context.EmpEmployeeBasicInfos on roleEx.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                            from emp in emp2.DefaultIfEmpty()
                            join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                            from empD in empD2.DefaultIfEmpty()
                            join des1 in _context.MasterDesignations on emp.IntDesignationId equals des1.IntDesignationId into des2
                            from des in des2.DefaultIfEmpty()
                            join dep1 in _context.MasterDepartments on emp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                            from dep in dep2.DefaultIfEmpty()
                            join wg1 in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                            from wg in wg2.DefaultIfEmpty()
                            join wp1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wp1.IntWorkplaceId into wp2
                            from wp in wp2.DefaultIfEmpty()
                            where emp.IntAccountId == tokenData.accountId && roleEx.IsActive == true
                            && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                            && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(intWorkplaceGroupId)) && emp.IntWorkplaceGroupId == intWorkplaceGroupId

                            && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                            : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                            : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                            : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                            : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                            : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                            : tokenData.territoryList.Contains(empD.IntTerritoryId))
                            || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                            select new
                            {
                                intRoleExtensionHeaderId = roleEx.IntRoleExtensionHeaderId,
                                intEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                strEmployeeName = emp.StrEmployeeName,
                                employeeCode = emp.StrEmployeeCode,
                                strDesignation = des.StrDesignation,
                                strDepartment = dep.StrDepartment,
                                workplaceGroupId = wg.IntWorkplaceGroupId,
                                strWorkplaceGroup = wg.StrWorkplaceGroup,
                                businessUnitId = emp.IntBusinessUnitId,

                            }).AsTracking().AsQueryable();

                if (!string.IsNullOrEmpty(SearchText))
                {
                    SearchText = SearchText.ToLower();

                    data = data.Where(x => x.strEmployeeName.ToLower().Contains(SearchText) ||
                                            x.employeeCode.ToLower().Contains(SearchText) ||
                                            x.strDesignation.ToLower().Contains(SearchText) ||
                                            x.strDepartment.ToLower().Contains(SearchText) ||
                                            x.strWorkplaceGroup.ToLower().Contains(SearchText));
                }




                return Ok(await data.ApplyPagination((int)PageNo, (int)PageSize));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("GetAllRoleExtensionListForCreatePage")]
        public async Task<IActionResult> GetAllRoleExtensionListForCreatePage(long businessUnitId, long intWorkplaceGroupId, long intEmployeeId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, intWorkplaceGroupId, intEmployeeId, null);

                if(isValid.Count() > 0)
                {
                    var data = await (from roleEx in _context.RoleExtensionHeaders
                                      join row in _context.RoleExtensionRows on new { a = roleEx.IntRoleExtensionHeaderId, b = true } equals new { a = row.IntRoleExtensionHeaderId, b = (bool)row.IsActive }
                                      join emp1 in _context.EmpEmployeeBasicInfos on roleEx.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                                      from emp in emp2.DefaultIfEmpty()
                                      join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                      from empD in empD2.DefaultIfEmpty()
                                      where emp.IntAccountId == tokenData.accountId && roleEx.IsActive == true && row.IntEmployeeId == intEmployeeId
                                      select new
                                      {
                                          intRoleExtensionHeaderId = roleEx.IntRoleExtensionHeaderId,
                                          intEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                          strEmployeeName = emp.StrEmployeeName,
                                          IntRoleExtensionRowId = row.IntRoleExtensionRowId,
                                          intOrganizationTypeId = row.IntOrganizationTypeId,
                                          strOrganizationTypeName = row.StrOrganizationTypeName,
                                          intOrganizationReffId = row.IntOrganizationReffId,
                                          StrOrganizationReffName = row.StrOrganizationReffName
                                      }).AsTracking().AsQueryable().ToListAsync();


                    return Ok(data);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion ===== Role Extension ==============

        #region ========= Bulk upload employee password encording =========

        //[HttpPost]
        //[Route("EmployeeBulkUploadPasswordEncording")]
        //public async Task<IActionResult> EmployeeBulkUploadPasswordEncording()
        //{
        //    try
        //    {
        //        List<User> usersList = await _context.Users.Where(x => x.IntAccountId == 10025 && (x.IntUserId != 10341 && x.IntUserId != 10367 && x.IntUserId != 10368)).ToListAsync();

        //        if (usersList != null)
        //        {
        //            usersList.ForEach(u =>
        //            {
        //                var Password = Encoding.UTF8.GetBytes(u.StrPassword);
        //                string encryptedPassword = Convert.ToBase64String(Password);
        //                u.StrPassword = encryptedPassword;
        //            });

        //            _context.Users.UpdateRange(usersList);
        //            await _context.SaveChangesAsync();
        //        }

        //        return Ok(true);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion ========= Bulk upload employee password encording =========

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

        [AllowAnonymous]
        [Route("Unauthorized")]
        [HttpGet]
        public IActionResult Unauthorized()
        {
            return Unauthorized("You are not authorized to access this resource.");
        }
    }
}