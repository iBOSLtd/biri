using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Elastic.Apm.Api;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using User = PeopleDesk.Data.Entity.User;

namespace PeopleDesk.Controllers.SAAS.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private System.Data.DataTable dt = new System.Data.DataTable();
        private readonly PeopleDeskContext _context;
        private readonly IWebHostEnvironment env;

        public EmployeeController(IApprovalPipelineService _approvalPipelineService, IEmployeeService employeeService, PeopleDeskContext _context, IWebHostEnvironment env)
        {
            this._employeeService = employeeService;
            this._context = _context;
            this.env = env;
            this._approvalPipelineService = _approvalPipelineService;
        }

        #region ================ Employee Department ===================

        [HttpPost]
        [Route("SaveEmpDepartment")]
        public async Task<IActionResult> SaveEmpDepartment(MasterDepartment model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            MessageHelperUpdate resUpdate = new MessageHelperUpdate();

            try
            {
                if (await _context.MasterDepartments.Where(x => x.StrDepartment.ToLower() == model.StrDepartment.ToLower()
                && x.StrDepartmentCode.ToLower() == model.StrDepartmentCode.ToLower() && x.IntDepartmentId != model.IntDepartmentId
                && x.IntAccountId == model.IntAccountId && x.IntBusinessUnitId == model.IntBusinessUnitId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    long id = await _employeeService.SaveEmpDepartment(model);
                    if (model.IntDepartmentId > 0)
                    {
                        res.AutoId = id;
                        return Ok(resUpdate);
                    }
                    else
                    {
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllEmpDepartment")]
        public async Task<IActionResult> GetAllEmpDepartment(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _employeeService.GetAllEmpDepartment(accountId, businessUnitId));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetEmpDepartmentById")]
        public async Task<IActionResult> GetEmpDepartmentById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpDepartmentById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpDepartment")]
        public async Task<IActionResult> DeleteEmpDepartment(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _employeeService.DeleteEmpDepartment(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        #endregion ================ Employee Department ===================

        #region =========== Employee Bulk Upload ===============

        [HttpPost]
        [Route("SaveEmployeeBulkUpload")]
        public async Task<IActionResult> SaveEmployeeBulkUpload(List<EmployeeBulkUploadViewModel> obj)
        {
            MessageHelperBulkUpload message = new MessageHelperBulkUpload();
            try
            {
                message = await _employeeService.SaveEmployeeBulkUpload(obj);

                if (message.StatusCode == 500)
                {
                    return BadRequest(message);
                }
                else
                {
                    return Ok(message);
                }
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return BadRequest(message);
            }
        }

        #endregion =========== Employee Bulk Upload ===============

        #region ===================== Employee Basic Info ========================

        [HttpPost]
        [Route("SaveEmpBasicInfo")]
        public async Task<IActionResult> SaveEmpBasicInfo(EmployeeBasicViewModel model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _employeeService.SaveEmpBasicInfo(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpPost]
        [Route("UpdateEmpBasicInfoByEmployeeId")]
        public async Task<IActionResult> UpdateEmpBasicInfoByEmployeeId(ContractualFromNToDateViewModel fromNToDateViewModel)
        {
            MessageHelper res = new MessageHelper();
            try
            {
                if (fromNToDateViewModel.EmployeeId > 0)
                {
                    EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == fromNToDateViewModel.EmployeeId).FirstOrDefaultAsync();

                    emp.DteContactFromDate = fromNToDateViewModel.ContractFromDate;
                    emp.DteContactToDate = fromNToDateViewModel.ContractToDate;

                    _context.EmpEmployeeBasicInfos.Update(emp);
                    await _context.SaveChangesAsync();

                    res.StatusCode = 200;
                    res.Message = "UPDATE SUCCESSFULLY";
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;

                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteRegisteredDeviceByEmployeeIDNDeviceId")]
        public async Task<IActionResult> DeleteRegisteredDeviceByEmployeeIDNDeviceId(long EmployeeId, string strDeviceId)
        {
            MessageHelper res = new MessageHelper();
            try
            {
                if (EmployeeId > 0 && !string.IsNullOrEmpty(strDeviceId))
                {
                    TimeRemoteAttendanceRegistration emp = await _context.TimeRemoteAttendanceRegistrations
                        .Where(x => x.IntEmployeeId == EmployeeId && x.StrDeviceId == strDeviceId && x.IsActive == true)
                        .FirstOrDefaultAsync();

                    if (emp is not null && emp.IsLocationRegister == false)
                    {
                        emp.IsActive = false;

                        _context.TimeRemoteAttendanceRegistrations.Update(emp);
                        await _context.SaveChangesAsync();

                        res.StatusCode = 200;
                        res.Message = "UPDATE SUCCESSFULLY";
                        return Ok(res);
                    }
                }

                res.StatusCode = 500;
                res.Message = "Data was not found !!!";

                return BadRequest(res);
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;

                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteRegisteredLocationByEmployeeIDAttendanceRegisterId")]
        public async Task<IActionResult> DeleteRegisteredLocationByEmployeeIDAttendanceRegisterId(long EmployeeId, long attendanceRegisterId)
        {
            MessageHelper res = new MessageHelper();
            try
            {
                if (EmployeeId > 0 && attendanceRegisterId > 0)
                {
                    TimeRemoteAttendanceRegistration emp = await _context.TimeRemoteAttendanceRegistrations
                        .Where(x => x.IntEmployeeId == EmployeeId && x.IntAttendanceRegId == attendanceRegisterId
                        && x.IsActive == true).FirstOrDefaultAsync();

                    if (emp is not null && emp?.IsLocationRegister == true)
                    {
                        emp.IsActive = false;

                        _context.TimeRemoteAttendanceRegistrations.Update(emp);
                        await _context.SaveChangesAsync();

                        res.StatusCode = 200;
                        res.Message = "UPDATE SUCCESSFULLY";

                        return Ok(res);
                    }
                }

                res.StatusCode = 500;
                res.Message = "Data was not found !!!";

                return BadRequest(res);
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;

                return BadRequest(res);
            }
        }

        [HttpPost]
        [Route("ActiveORInactiveEmployeeBasicInfo")]
        public async Task<IActionResult> ActiveORInactiveEmployeeBasicInfo(long AccountId, long EmployeeId)
        {
            MessageHelper res = new MessageHelper();
            try
            {
                if (EmployeeId > 0)
                {
                    //EmpEmployeeBasicInfoDetail emp = await _context.EmpEmployeeBasicInfoDetails.
                    //Where(x => x.IntEmployeeId == activeNInactiveViewModel.EmployeeId).FirstOrDefaultAsync();

                    EmpEmployeeBasicInfoDetail employeeDetails = await (from details in _context.EmpEmployeeBasicInfoDetails
                                                                        join emp in _context.EmpEmployeeBasicInfos on details.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                        where details.IntEmployeeId == EmployeeId && emp.IntAccountId == AccountId
                                                                        select details).FirstOrDefaultAsync();
                    if (employeeDetails != null)
                    {
                        employeeDetails.IntEmployeeStatusId = 1;
                        employeeDetails.StrEmployeeStatus = "Active";

                        _context.EmpEmployeeBasicInfoDetails.Update(employeeDetails);
                        await _context.SaveChangesAsync();

                        res.StatusCode = 200;
                        res.Message = "Activated SUCCESSFULLY";
                    }
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;

                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllEmpBasicInfo")]
        public async Task<IActionResult> GetAllEmpBasicInfo(long accountId)
        {
            try
            {
                return Ok(await _employeeService.GetAllEmpBasicInfo(accountId));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetEmpBasicInfoById")]
        public async Task<IActionResult> GetEmpBasicInfoById(long businessUnitId, long workplaceGroupId, long id)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.employeeId == id)
                {
                    return Ok(await _employeeService.GetEmpBasicInfoById(id));
                }
                else
                {
                    var data = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, id, null);
                    if (data.Count() > 0)
                    {
                        return Ok(await _employeeService.GetEmpBasicInfoById(id));
                    }
                    else
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpBasicInfo")]
        public async Task<IActionResult> DeleteEmpBasicInfo(long id)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();
                await _employeeService.DeleteEmpBasicInfo(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        //[HttpPost]
        //[Route("CRUDEmployeeBasicInfo")]
        //public async Task<IActionResult> CRUDEmployeeBasicInfo(EmployeeBasicViewModel obj)
        //{
        //    try
        //    {
        //        MessageHelper msg = await _employeeService.CRUDTblEmployeeBasicInfo(obj);

        //        return msg.StatusCode == 200 ? StatusCode(200, msg) : StatusCode(500, msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("CreateNUpdateEmployeeBasicInfo")]
        public async Task<IActionResult> CreateNUpdateEmployeeBasicInfo(EmployeeBasicCreateNUpdateVM model)
        {
            try
            {
                PermissionLebelCheck permissionLebel = model.TerritoryId > 0 ? PermissionLebelCheck.Territory : model.AreaId > 0 ? PermissionLebelCheck.Area : model.RegionId > 0 ? PermissionLebelCheck.Region : model.SoleDepoId > 0 ? PermissionLebelCheck.SoleDepo : model.WingId > 0 ? PermissionLebelCheck.Wing : model.IntWorkplaceGroupId > 0 ? PermissionLebelCheck.WorkplaceGroup : PermissionLebelCheck.BusinessUnit;

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.IntBusinessUnitId, workplaceGroupId = model.IntWorkplaceGroupId, wingId = (long)model.WingId, soleDepoId = (long)model.SoleDepoId, regionId = (long)model.RegionId, areaId = (long)model.AreaId, territoryId = (long)model.TerritoryId }, permissionLebel);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                MessageHelper msg = new();
                msg = await _employeeService.CreateNUpdateEmployeeBasicInfo(model, tokenData.accountId, tokenData.employeeId);

                return msg.StatusCode == 200 ? Ok(msg) : BadRequest(msg);

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ConfirmationEmployee")]
        public async Task<IActionResult> ConfirmationEmployee(ConfirmationEmployeeVM model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData != null)
                {
                    MessageHelper msg = await _employeeService.ConfirmationEmployee(model, tokenData.accountId, tokenData.employeeId);

                    if (msg.StatusCode == 200)
                    {
                        return Ok(msg);
                    }
                }

                return BadRequest(new MessageHelper { StatusCode = StatusCodes.Status401Unauthorized, Message = "UnAuthorized" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion ===================== Employee Basic Info ========================

        #region ================ Employee Address ===============

        [HttpPost]
        [Route("SaveEmployeeAddress")]
        public async Task<IActionResult> SaveEmployeeAddress(EmployeeAddressViewModel obj)
        {
            return Ok(await _employeeService.SaveEmployeeAddress(obj));
        }

        [HttpGet]
        [Route("GetAllEmployeeAddress")]
        public async Task<IActionResult> GetAllEmployeeAddress(long employeeId)
        {
            return Ok(await _employeeService.GetAllEmployeeAddress(employeeId));
        }

        [HttpGet]
        [Route("GetEmployeeAddressById")]
        public async Task<IActionResult> GetEmployeeAddressById(long id)
        {
            return Ok(await _employeeService.GetEmployeeAddressById(id));
        }

        [HttpPut]
        [Route("DeleteEmployeeAddress")]
        public async Task<IActionResult> DeleteEmployeeAddress(long id)
        {
            return Ok(await _employeeService.DeleteEmployeeAddress(id));
        }

        #endregion ================ Employee Address ===============

        #region ============== Employee Bank Details ==================

        [HttpPost]
        [Route("SaveEmpBankDetails")]
        public async Task<IActionResult> SaveEmpBankDetails(EmployeeBankDetailsViewModel model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _employeeService.SaveEmpBankDetails(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpPost]
        [Route("CreateBankBranch")]
        public async Task<IActionResult> CreateBankBranch(GlobalBankBranchViewModel model)
        {
            try
            {
                var res = await _employeeService.CreateBankBranch(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("BankBranchDDL")]
        public async Task<IActionResult> BankBranchDDL(long BankId, long AccountID, long DistrictId)
        {
            try
            {
                var res = await _employeeService.BankBranchDDL(BankId, AccountID, DistrictId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetEmpBankDetailsById")]
        public async Task<IActionResult> GetEmpBankDetailsById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpBankDetailsById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpBankDetails")]
        public async Task<IActionResult> DeleteEmpBankDetails(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpBankDetails(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ============== Employee Bank Details ==================

        #region =============== Employee Education ===============

        [HttpPost]
        [Route("SaveEmpEducation")]
        public async Task<IActionResult> SaveEmpEducation(EmployeeEducationViewModel model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _employeeService.SaveEmpEducation(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpEducationById")]
        public async Task<IActionResult> GetEmpEducationById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpEducationById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper()
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpEducation")]
        public async Task<IActionResult> DeleteEmpEducation(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpEducation(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 200
                };
                return BadRequest(res);
            }
        }

        #endregion =============== Employee Education ===============

        #region ================ Employee Relative ==============

        [HttpPost]
        [Route("SaveEmpRelativesContact")]
        public async Task<IActionResult> SaveEmpRelativesContact(EmployeeRelativesContactViewModel model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _employeeService.SaveEmpRelativesContact(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpRelativesContactById")]
        public async Task<IActionResult> GetEmpRelativesContactById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpRelativesContactById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 200
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpRelativesContact")]
        public async Task<IActionResult> DeleteEmpRelativesContact(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpRelativesContact(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ================ Employee Relative ==============

        #region ============ Employee Job History ==============

        [HttpPost]
        [Route("SaveEmpJobHistory")]
        public async Task<IActionResult> SaveEmpJobHistory(EmployeeJobHistoryViewModel model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _employeeService.SaveEmpJobHistory(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpJobHistorytById")]
        public async Task<IActionResult> GetEmpJobHistorytById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpJobHistorytById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpJobHistory")]
        public async Task<IActionResult> DeleteEmpJobHistory(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpJobHistory(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ============ Employee Job History ==============

        #region ================ Employee Training ====================

        [HttpPost]
        [Route("SaveEmpTraining")]
        public async Task<IActionResult> SaveEmpTraining(EmployeeTrainingViewModel model)
        {
            MessageHelper res = new MessageHelper
            {
                Message = "Created Successfully",
                StatusCode = 200
            };

            try
            {
                await _employeeService.SaveEmpTraining(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpTrainingById")]
        public async Task<IActionResult> GetEmpTrainingById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpTrainingById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpTraining")]
        public async Task<IActionResult> DeleteEmpTraining(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpTraining(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ================ Employee Training ====================

        #region ================== Employee File =====================

        [HttpPost]
        [Route("SaveEmpFile")]
        public async Task<IActionResult> SaveEmpFile(EmployeeFileViewModel model)
        {
            MessageHelper res = new MessageHelper
            {
                Message = "Created Successfully",
                StatusCode = 200
            };

            try
            {
                await _employeeService.SaveEmpFile(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpFileById")]
        public async Task<IActionResult> GetEmpFileById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpFileById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpFile")]
        public async Task<IActionResult> DeleteEmpFile(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpFile(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ================== Employee File =====================

        #region =============== Employee Photo Identity =================

        [HttpPost]
        [Route("SaveEmpPhotoIdentity")]
        public async Task<IActionResult> SaveEmpFile(EmployeePhotoIdentityViewModel model)
        {
            MessageHelper res = new MessageHelper
            {
                Message = "Created Successfully",
                StatusCode = 200
            };

            try
            {
                await _employeeService.SaveEmpPhotoIdentity(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return BadRequest(res);
        }

        [HttpGet]
        [Route("GetEmpPhotoIdentityById")]
        public async Task<IActionResult> GetEmpPhotoIdentityById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpPhotoIdentityById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpPhotoIdentity")]
        public async Task<IActionResult> DeleteEmpPhotoIdentity(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpPhotoIdentity(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion =============== Employee Photo Identity =================

        #region ========== Employee Document Management ============

        [HttpPost]
        [Route("SaveEmployeeDocumentManagement")]
        public async Task<IActionResult> SaveEmployeeDocumentManagement(EmployeeDocumentManagementViewModel obj)
        {
            try
            {
                MessageHelper msg = await _employeeService.SaveEmployeeDocumentManagement(obj);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAllEmployeeDocumentManagement")]
        public async Task<IActionResult> GetAllEmployeeDocumentManagement(long EmployeeId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                List<EmpEmployeeDocumentManagement> EmpDocument = await _employeeService.GetAllEmployeeDocumentManagement(tokenData.accountId, EmployeeId);

                return Ok(EmpDocument);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeDocumentById")]
        public async Task<IActionResult> GetEmployeeDocumentById(int id)
        {
            try
            {
                EmpEmployeeDocumentManagement EmpDocument = await _employeeService.GetEmployeeDocumentById(id);

                return Ok(EmpDocument);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("DeleteEmpDocumentManagement")]
        public async Task<IActionResult> DeleteEmpDocumentManagement(long id)
        {
            try
            {
                return Ok(await _employeeService.DeleteEmpDocumentManagement(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    Message = ex.Message,
                    StatusCode = 500
                };
                return BadRequest(res);
            }
        }

        #endregion ========== Employee Document Management ============

        #region ========== Elezable For Job Config ==========

        //[HttpPost]
        //[Route("SaveElezibleJobConfig")]
        //public async Task<IActionResult> SaveElezibleJobConfig(List<ElezableForJobConfigViewModel> obj)
        //{
        //    try
        //    {
        //        MessageHelper msg = await _employeeService.SaveElezibleJobConfig(obj);

        //        if (msg.StatusCode == 200)
        //        {
        //            return Ok(msg);
        //        }
        //        else
        //        {
        //            return BadRequest(msg);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }

        //}
        //[HttpGet]
        //[Route("GetAllJobConfig")]
        //public async Task<IActionResult> GetAllJobConfig(long IntAccountId)
        //{
        //    try
        //    {
        //        List<ElezableForJobConfigViewModel> EmpDocument = await _employeeService.GetAllJobConfig(IntAccountId);

        //        return Ok(EmpDocument);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //    }
        //}
        //[HttpGet]
        //[Route("GetJobConfigById")]
        //public async Task<IActionResult> GetJobConfigById(long id)
        //{
        //    try
        //    {
        //        ElezableForJobConfigViewModel EmpDocument = await _employeeService.GetJobConfigById(id);

        //        return Ok(EmpDocument);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        #endregion ========== Elezable For Job Config ==========

        #region ========== Time Sheet ============

        [HttpGet]
        [Route("TimeSheetAllLanding")]
        public async Task<IActionResult> TimeSheetAllLanding(string? PartType, long? BuninessUnitId, long? intId, long? intYear, long? intMonth)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BuninessUnitId }, PermissionLebelCheck.BusinessUnit);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(JsonConvert.SerializeObject(await (_employeeService.TimeSheetAllLanding(PartType, BuninessUnitId, intId, intYear, intMonth))));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("ManualAttendance")]
        public async Task<IActionResult> ManualAttendance(List<EmployeeManualAttendanceViewModel> model)
        {
            var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
            if (tokenData.accountId == -1)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }

            long employeeId = model.Select(x => (long)x.EmployeeId).FirstOrDefault();

            if (tokenData.employeeId != employeeId)
            {
                var check = await _employeeService.GetCommonEmployeeDDL(tokenData, model.Select(x => x.BusinessUnitId).FirstOrDefault(), model.Select(x => x.WorkPlaceGroup).FirstOrDefault(), employeeId, null);
                if (check.Count() <= 0)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
            }

            List<EmpManualAttendanceSummary> datas = new List<EmpManualAttendanceSummary>();
            List<EmpManualAttendanceSummary> isExistsDatas = new List<EmpManualAttendanceSummary>();

            long accountId = tokenData.accountId;
            // model.Select(x => x.AccountId).FirstOrDefault().Value;
            var pipeline = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(model.Select(x => (long)x.EmployeeId).First(), "manualAttendance");

            if (pipeline.messageHelper != null && pipeline.messageHelper.StatusCode == 400)
            {
                return BadRequest(new MessageHelper() { Message = pipeline.messageHelper.Message, StatusCode = pipeline.messageHelper.StatusCode });
            }

            //GlobalPipelineHeader pipelineHeader = await _context.GlobalPipelineHeaders.FirstOrDefaultAsync(x => x.IntAccountId == accountId && x.IsActive == true && x.StrApplicationType.ToLower() == "manualAttendance");
            MessageHelper messageHelper = new MessageHelper();

            if (pipeline != null)
            {
                // List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows.Where(x => x.IsActive == true && x.IntPipelineHeaderId == pipelineHeader.IntPipelineHeaderId).OrderBy(x => x.IntShortOrder).ToListAsync();

                foreach (EmployeeManualAttendanceViewModel item in model)
                {
                    //long PipelineRowId = pipelineRows.OrderBy(x => x.IntShortOrder).FirstOrDefault().IntPipelineRowId;

                    EmpManualAttendanceSummary obj = await _context.EmpManualAttendanceSummaries
                        .Where(x => x.IntEmployeeId == item.EmployeeId && x.DteAttendanceDate.Value.Date == item.AttendanceDate.Value.Date && x.IsActive == true).FirstOrDefaultAsync();

                    if (obj is null)
                    {
                        EmpManualAttendanceSummary newObj = new EmpManualAttendanceSummary
                        {
                            IntAttendanceSummaryId = (long)item.AttendanceSummaryId,
                            IntEmployeeId = item.EmployeeId,
                            DteAttendanceDate = item.AttendanceDate,
                            StrCurrentStatus = item.CurrentStatus,
                            StrRequestStatus = item.RequestStatus,
                            StrRemarks = item.Remarks,
                            IsActive = true,
                            IntCreatedBy = item.IntCreatedBy,
                            DteCreatedAt = item.DteCreatedAt,
                            IntPipelineHeaderId = pipeline.HeaderId,
                            IntCurrentStage = pipeline.CurrentStageId, /*pipelineRows.OrderBy(x => x.IntShortOrder).First().IntPipelineRowId,*/
                            IntNextStage = pipeline.NextStageId, /*pipelineRows.Count() > 1 ? pipelineRows.OrderBy(x => x.IntShortOrder).Skip(1).First().IntPipelineRowId : pipelineRows.OrderBy(x => x.IntShortOrder).First().IntPipelineRowId,*/
                            StrStatus = "Pending",
                            IsPipelineClosed = false
                        };
                        if (!string.IsNullOrEmpty(item.InTime))
                        {
                            newObj.TimeInTime = TimeSpan.Parse(item.InTime);
                        }
                        if (!string.IsNullOrEmpty(item.OutTime))
                        {
                            newObj.TimeOutTime = TimeSpan.Parse(item.OutTime);
                        }
                        datas.Add(newObj);
                    }
                    else if ((obj.IsPipelineClosed == false && obj.StrStatus.ToLower() == "Pending".ToLower()) || item.isManagement == true)
                    {
                        obj.IntAttendanceSummaryId = (long)item.AttendanceSummaryId;
                        obj.StrCurrentStatus = item.CurrentStatus;
                        obj.StrRequestStatus = item.RequestStatus;
                        obj.StrRemarks = item.Remarks;
                        obj.IntPipelineHeaderId = pipeline.HeaderId;
                        obj.IntCurrentStage = pipeline.CurrentStageId;
                        obj.IntNextStage = pipeline.NextStageId;
                        obj.StrStatus = "Pending";
                        obj.IsReject = false;
                        obj.DteRejectDateTime = null;
                        obj.IntRejectedBy = null;
                        obj.IsPipelineClosed = false;

                        isExistsDatas.Add(obj);
                    }
                    else
                    {
                        messageHelper.StatusCode = 500;
                        messageHelper.Message = "Multiple Request Was Not Allowed For Rejected OR Approved Adjustment";
                    }
                }
                await _context.EmpManualAttendanceSummaries.AddRangeAsync(datas);
                _context.EmpManualAttendanceSummaries.UpdateRange(isExistsDatas);
                await _context.SaveChangesAsync();

                messageHelper.StatusCode = 200;
                messageHelper.Message = "Successfull";

                return Ok(messageHelper);
            }
            else
            {
                messageHelper.Message = "Pipeline was not setup";
                return StatusCode(400, messageHelper);
            }
        }

        [HttpPost]
        [Route("HolidayNExceptionFilter")]
        public async Task<IActionResult> HolidayNExceptionFilter(HolidayNExceptionFilterViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.businessUnitId, workplaceGroupId = model.workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                HolidayAssignLandingPaginationViewModelWithHeader holidayAssign = await _employeeService.HolidayNExceptionFilter(tokenData, model.businessUnitId, model.workplaceGroupId, model.searchTxt,
                        model.PageNo, model.PageSize, model.IsNotAssign, model.IsPaginated, model.IsHeaderNeed, model.DepartmentList, model.DesignationList, model.SupervisorNameList,
                        model.WingNameList, model.SoleDepoNameList, model.RegionNameList, model.AreaNameList, model.TerritoryNameList);

                return Ok(holidayAssign);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("HolidayAndExceptionOffdayAssign")]
        public async Task<IActionResult> HolidayAndExceptionOffdayAssign(HolidayAssignVm model)
        {
            try
            {
                return Ok(await _employeeService.HolidayAndExceptionOffdayAssign(model));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("CalendarAssignFilter")]
        public async Task<IActionResult> CalendarAssignFilter(CalendarAssignFilterViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                CalendarAssignLandingPaginationViewModelWithHeader calendarAssign = await _employeeService.CalendarAssignFilter(tokenData, model.BusinessUnitId, model.WorkplaceGroupId, model.SearchTxt,
                        model.PageNo, model.PageSize, model.IsNotAssign, model.IsPaginated, model.IsHeaderNeed, model.DepartmentList, model.DesignationList, model.SupervisorNameList, model.EmploymentTypeList,
                        model.WingNameList, model.SoleDepoNameList, model.RegionNameList, model.AreaNameList, model.TerritoryNameList);


                return Ok(calendarAssign);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost("TimeAttendanceSummaryForRoaster")]
        public async Task<IActionResult> TimeAttendanceSummaryForRoasterAsync(long IntEmployeeId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                MessageHelper msg = await _employeeService.TimeAttendanceSummaryForRoasterAsync(IntEmployeeId, FromDate, ToDate);
                return msg.StatusCode == 200 ? Ok(msg) : BadRequest(msg);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpPost]
        [Route("RosterGenerateList")]
        public async Task<IActionResult> RosterGenerateList(RosterGenerateViewModel obj)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(await _employeeService.RosterGenerateList(obj)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("OffdayLandingFilter")]
        public async Task<IActionResult> OffdayLandingFilter(OffdayLandingFilterViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                OffdayAssignLandingPaginationViewModelWithHeader offdayAssign = await _employeeService.OffdayLandingFilter(tokenData, model.BusinessUnitId, model.WorkplaceGroupId, model.SearchTxt, model.PageNo, model.PageSize, model.IsAssign,
                    model.IsPaginated, model.IsHeaderNeed, model.DepartmentList, model.DesignationList, model.SupervisorNameList, model.EmploymentTypeList, model.WingNameList, model.SoleDepoNameList, model.RegionNameList, model.AreaNameList, model.TerritoryNameList);

                return Ok(offdayAssign);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("OffdayAssign")]
        public async Task<IActionResult> OffdayAssign(OffdayAssignViewModel model)
        {
            try
            {
                return Ok(await _employeeService.OffdayAssign(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("AttendanceAdjustmentFilter")]
        public async Task<IActionResult> AttendanceAdjustmentFilter(AttendanceAdjustmentFilterViewModel obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = (long)obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.BusinessUnitId, (long)obj.WorkplaceGroupId, obj.EmployeeId, null);

                if (isValid.Count() > 0)
                {
                    return Ok(JsonConvert.SerializeObject(await (_employeeService.AttendanceAdjustmentFilter(obj))));
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

        [HttpGet]
        [Route("GetAttendanceProcessData")]
        public async Task<IActionResult> GetAttendanceProcessData(long? accountId, long? employeeId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string res = "error";

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    res = (await connection.QueryAsync<string>("EXEC saas.sprAttendanceProcess @accountId, @employeeId, @fromDate, @toDate", new { accountId, employeeId, fromDate, toDate }, commandTimeout: 600)).ToString();
                    connection.Close();
                }

                //return Ok(res);

                return Ok(new MessageHelperUpdate { StatusCode = 200, Message = "success" });

            }
            catch (Exception ex)
            {
                return BadRequest("internal server error");
            }
        }

        [HttpPost]
        [Route("CreateExtraSideDuty")]
        public async Task<IActionResult> CreateExtraSideDuty(ExtraSideDutyViewModel obj)
        {
            try
            {
                var msg = await _employeeService.CreateExtraSideDuty(obj);

                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("OverTimeFilter")]
        public async Task<IActionResult> OverTimeFilter(OverTimeFilterViewModel obj)
        {
            try
            {
                BaseVM checkedAuthorize = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.Authorization, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = (long)obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!checkedAuthorize.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var TokenData = AuthExtension.GetDataFromJwtToken();

                //var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.BusinessUnitId, (long)obj.WorkplaceGroupId, obj.EmployeeId, null);

                //if (isValid.Count() > 0)
                //{
                //    return Ok(await _employeeService.OverTimeFilter(obj));
                //}
                //else
                //{
                //    return BadRequest(new MessageHelperAccessDenied());
                //}
                return Ok(await _employeeService.OverTimeFilter(obj, TokenData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateOvertimeUpload")]
        public async Task<IActionResult> CreateOvertimeUpload(List<EmpOverTimeUploadDTO> objList)
        {
            try
            {
                var res = await _employeeService.CreateOvertimeUpload(objList);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [Route("InsertAllOvertime")]
        public async Task<IActionResult> CreateOvertime(List<TimeEmpOverTimeVM> objList)
        {
            try
            {
                var buNWg = objList.Select(n => new { BusinessUnitId = n.IntBusinessUnitId, WorkplaceGroupId = n.IntWorkplaceGroupId }).FirstOrDefault();
                List<long> empList = objList.Select(n => (long)n.IntEmployeeId).ToList();

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                var employeeDDL = await _employeeService.PermissionCheckFromEmployeeListByEnvetFireEmployee(tokenData, (long)buNWg.BusinessUnitId, (long)buNWg.WorkplaceGroupId, empList, null);

                if (employeeDDL.Count() != empList.Count())
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _employeeService.CreateOvertime(objList);

                if (res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                MessageHelperThrow msg = new()
                {
                    Message = ex.Message,
                };
                return BadRequest(msg);
            }
        }

        [HttpPost]
        [Route("SubmitOvertimeUpload")]
        public async Task<IActionResult> SubmitOvertimeUpload(List<EmpOverTimeUploadDTO> objList)
        {
            try
            {
                var res = await _employeeService.SubmitOvertimeUpload(objList);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion ========== Time Sheet ============

        #region ====================== Employee Information ==================

        [HttpPost]
        [Route("UpdateEmployeeProfile")]
        public async Task<IActionResult> UpdateEmployeeProfile(EmployeeProfileUpdateViewModel obj)
        {
            try
            {
                return Ok(await _employeeService.UpdateEmployeeProfile(obj));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("CRUDEmployeeSeparation")]
        public async Task<IActionResult> CRUDEmployeeSeparation(EmployeeSeparationViewModel obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = (long)obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                //obj.IntAccountId = tokenData.accountId;

                MessageHelper msg = await _employeeService.CRUDEmployeeSeparation(tokenData, obj);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("EmployeeSeparationDocumentDelete")]
        public async Task<IActionResult> EmployeeSeparationDocumentDelete(long intSeparationid, string DocumentId)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (intSeparationid > 0)
                {
                    EmpEmployeeSeparation separation = _context.EmpEmployeeSeparations.FirstOrDefault(x => x.IntSeparationId == intSeparationid);

                    string[] documentArray = separation.StrDocumentId.Split(',');

                    List<string> list = new List<string>(documentArray);
                    list.Remove(DocumentId);
                    documentArray = list.ToArray();
                    string strDocumentId = String.Join(",", documentArray);

                    separation.StrDocumentId = strDocumentId;
                    _context.EmpEmployeeSeparations.Update(separation);
                    await _context.SaveChangesAsync();
                }
                message.StatusCode = 200;
                message.Message = "Delete Successfully";
                return Ok(message);
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return BadRequest(message);
            }
        }

        [HttpPost]
        [Route("ReleasedEmployeeSeparation")]
        public async Task<IActionResult> ReleasedEmployeeSeparation(EmployeeSeparationReleaseViewModel obj)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (obj.IntSeparationId > 0 && obj.IsReleased == true)
                {
                    BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                    EmpEmployeeSeparation separation = _context.EmpEmployeeSeparations.Where(x => x.IntSeparationId == obj.IntSeparationId
                    && x.IntAccountId == tokenData.accountId).FirstOrDefault();

                    if (separation.IsPipelineClosed == true && separation.IsReject == false)
                    {
                        separation.IsReleased = true;
                        separation.DteUpdatedAt = DateTime.Now;
                        separation.IntUpdatedBy = tokenData.employeeId;

                        _context.EmpEmployeeSeparations.Update(separation);
                        await _context.SaveChangesAsync();

                        EmpEmployeeBasicInfo empEmployee = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == separation.IntEmployeeId && x.IntAccountId == separation.IntAccountId && x.IsActive == true).FirstOrDefaultAsync();
                        if (empEmployee != null)
                        {
                            empEmployee.IsActive = false;
                            empEmployee.DteUpdatedAt = DateTime.Now;
                            _context.EmpEmployeeBasicInfos.Update(empEmployee);
                        }

                        EmpEmployeeBasicInfoDetail empDetail = await _context.EmpEmployeeBasicInfoDetails.Where(x => x.IntEmployeeId == separation.IntEmployeeId && x.IsActive == true).FirstOrDefaultAsync();
                        if (empDetail != null)
                        {
                            empDetail.IsActive = false;
                            empDetail.IntEmployeeStatusId = 2;
                            empDetail.StrEmployeeStatus = "Inactive";
                            empDetail.IsActive = false;
                            
                            _context.EmpEmployeeBasicInfoDetails.Update(empDetail);
                        }

                        User user = await _context.Users.FirstOrDefaultAsync(x => (long)x.IntRefferenceId == separation.IntEmployeeId && x.IsActive == true);
                        if (user != null)
                        {
                            user.IsActive = false;

                            _context.Users.Update(user);
                        }
                        await _context.SaveChangesAsync();


                        message.StatusCode = 200;
                        message.Message = "Released Successfully";
                    }
                    else
                    {
                        message.StatusCode = 500;
                        message.Message = "You need to approve before release.";
                    }
                }
                else
                {
                    message.StatusCode = 400;
                    message.Message = "Not Released";
                }

                if (message.StatusCode == 200)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return BadRequest(message);
            }
        }

        #endregion ====================== Employee Information ==================

        #region ====================== Employee Landing ==================

        [HttpGet]
        [Route("EmployeeProfileLandingPagination")]
        public async Task<IActionResult> EmployeeProfileLandingPagination(long businessUnitId, long WorkplaceGroupId, string? searchTxt, int PageNo, int PageSize, bool IsForXl)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, null, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var data = await _employeeService.EmployeeProfileLandingPagination(tokenData.accountId, businessUnitId, WorkplaceGroupId, searchTxt, tokenData.employeeId, PageNo, PageSize, IsForXl);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }
        [HttpPost]
        [Route("EmployeeProfileLandingPaginationWithMasterFilter")]
        public async Task<IActionResult> EmployeeProfileLandingPaginationWithMasterFilter(profileLandingFilterVM filter)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = filter.businessUnitId, workplaceGroupId = filter.workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.isAuthorize)
                {
                    dynamic employees = await _employeeService.EmployeeProfileLandingPaginationWithMasterFilter(tokenData, filter.businessUnitId, filter.workplaceGroupId, filter.searchTxt,
                        filter.PageNo, filter.PageSize, filter.IsPaginated, filter.IsHeaderNeed, filter.StrDepartmentList, filter.StrDesignationList, filter.StrSupervisorNameList,
                        filter.StrEmploymentTypeList, filter.StrLinemanagerList, filter.WingNameList, filter.SoleDepoNameList, filter.RegionNameList, filter.AreaNameList, filter.TerritoryNameList);

                    return Ok(employees);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError());
            }
        }
        ////Employee Profile Landing Pagination new
        //[HttpPost]
        //[Route("EmployeeProfileLandingPaginationRequiredFieldOnly")]
        //public async Task<IActionResult> EmployeeProfileLandingPaginationRequiredFieldOnly(profileLandingFilterVM filterVM)
        //{
        //    try
        //    {
        //        dynamic employees = await _employeeService.EmployeeProfileLandingPagination(filterVM, false);
        //        return Ok(employees);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}
        //[HttpGet]
        //[Route("EmployeeProfileLandingPaginationHeadersOnly")]
        //public async Task<IActionResult> EmployeeProfileLandingPaginationHeadersOnly(long accountId, long businessUnitId, long? workplaceGroupId, long? workplaceId)
        //{
        //    try
        //    {
        //        profileLandingFilterVM filterVM = new()
        //        {
        //            accountId = accountId,
        //            businessUnitId = businessUnitId,
        //            workplaceGroupId = workplaceGroupId,
        //            workplaceId = workplaceId
        //        };
        //        return Ok(await _employeeService.EmployeeProfileLandingPagination(filterVM, true));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}

        [HttpGet]
        [Route("EmployeeProfileViewData")]
        public async Task<IActionResult> EmployeeProfileViewData(long employeeId)
        {
            try
            {
                return Ok(await _employeeService.EmployeeProfileViewData(employeeId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("EmployeeListForUserLandingPagination")]
        public async Task<IActionResult> EmployeeListForUserLandingPagination(long businessUnitId, long workplaceGroupId, string? searchTxt, int? isUser, int PageNo, int PageSize, bool IsForXl)
        {
            try
            {
                var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tD.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                EmployeeProfileLandingPaginationViewModel data = await _employeeService.EmployeeListForUserLandingPagination(tD.accountId, businessUnitId, workplaceGroupId, searchTxt, isUser, PageNo, PageSize, IsForXl);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("EmployeeListBySupervisorORLineManagerNOfficeadmin")]
        public async Task<IActionResult> EmployeeListBySupervisorORLineManagerNOfficeadmin(long BusinessUnitId, long? WorkplaceGroupId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                List<EmployeeListDDLViewModel> data = await _employeeService.EmployeeListBySupervisorORLineManagerNOfficeadmin(tokenData.employeeId, WorkplaceGroupId);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }


        [HttpGet]
        [Route("EmployeeProfileView")]
        public async Task<IActionResult> EmployeeProfileView(long businessUnitId, long workplaceGroupId, long employeeId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                bool isValid = tokenData.employeeId == employeeId ? true : false;

                if (!isValid)
                {
                    var pCount = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, employeeId, null);
                    isValid = pCount.Count() > 0 ? true : false;
                }

                if (isValid)
                {
                    EmployeeProfileView data = await _employeeService.EmployeeProfileView(employeeId);                   

                    return Ok(data);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("EmployeeDetailsList")]
        public async Task<IActionResult> EmployeeDetailsList(long AccountId, long BusinessUnitId, long? WorkplaceGroupId)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeDetails";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                //long count = dt.Rows.Count;
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion ====================== Employee Landing ==================

        #region ====================== Employee Bank Info ==================

        [HttpPost]
        [Route("CRUDEmployeeBankDetails")]
        public async Task<IActionResult> CRUDEmployeeBankDetails(EmployeeBankDetailsViewModel obj)
        {
            try
            {
                return Ok(await _employeeService.CRUDEmployeeBankDetails(obj));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion ====================== Employee Bank Info ==================

        #region ======== Report ========

        [HttpGet]
        [Route("GetAttendanceDetailsReport")]
        public async Task<IActionResult> GetAttendanceDetailsReport(long TypeId, long EmployeeId, DateTime FromDate, DateTime ToDate)
        {
            // @type int, @empid int, @fdate date, @tdate date
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeAttendanceDetailsReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@type", TypeId);
                        sqlCmd.Parameters.AddWithValue("@empid", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@fdate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@tdate", ToDate);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                var res = JsonConvert.SerializeObject(dt);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("DailyAttendanceReport")]
        public async Task<IActionResult> DailyAttendanceReport(long IntAccountId, DateTime AttendanceDate, long IntBusinessUnitId, long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntDepartmentId, string? EmployeeIdList, string? SearchTxt, int? PageNo, int? PageSize, bool IsPaginated)
        {
            try
            {
                DailyAttendanceReportVM dailyAttendanceReport = await _employeeService.DailyAttendanceReport(IntAccountId, AttendanceDate, IntBusinessUnitId, IntWorkplaceGroupId, IntWorkplaceId, IntDepartmentId, EmployeeIdList, SearchTxt, PageNo, PageSize, IsPaginated);

                return Ok(dailyAttendanceReport);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetDateWiseAttendanceReport")]
        public async Task<IActionResult> GetDateWiseAttendanceReport(long IntBusinessUnitId, long IntWorkplaceGroupId, DateTime attendanceDate, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = IntBusinessUnitId, workplaceGroupId = IntWorkplaceGroupId }, PermissionLebelCheck.BusinessUnit);

                if (tokenData.accountId == -1 && !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                EmployeeDaylyAttendanceReportLanding dailyAttendanceReport = await _employeeService.GetDateWiseAttendanceReport(tokenData.accountId, IntBusinessUnitId, IntWorkplaceGroupId, attendanceDate, IsXls, PageNo, PageSize, searchTxt);

                return Ok(dailyAttendanceReport);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("PeopleDeskAllLanding")]
        public async Task<IActionResult> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId, DateTime? FromDate, DateTime? ToDate,
           long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount, long? MovementTypeId, DateTime? ApplicationDate, long? LoggedEmployeeId,
           int? MonthId, long? YearId, long? WorkplaceGroupId, string? SearchTxt, int? PageNo, int? PageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId }, PermissionLebelCheck.BusinessUnit);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                if (!string.IsNullOrEmpty(TableName))
                {
                    if (TableName == "EmploymentTypeWiseLeaveBalance" || TableName == "EmploymentTypeWiseLeaveBalance" || TableName == "HolidayByHolidayGroupId"
                        || TableName == "HolidayGroupById" || TableName == "Calender" || TableName == "CalenderById" || TableName == "RosterGroup" || TableName == "RosterByRosterGroupId"
                        || TableName == "EmployeeBasicShortInfoByEmployeeId" || TableName == "EmployeeBasicById" || TableName == "ContractualClosing"
                        )
                    {
                        AccountId = tokenData.accountId;
                    }

                    return Ok(JsonConvert.SerializeObject(await _employeeService.PeopleDeskAllLanding(TableName, AccountId, BusinessUnitId, intId, intStatusId, FromDate, ToDate,
                        LoanTypeId, DeptId, DesigId, EmpId, MinimumAmount, MaximumAmount, MovementTypeId, ApplicationDate, LoggedEmployeeId, MonthId, YearId, WorkplaceGroupId, SearchTxt, PageNo, PageSize)));
                }
                else
                {
                    throw new Exception("invalid data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("EmployeeContactInfo")]
        public async Task<IActionResult> EmployeeContactInfo(long businessUnitId, int pageSize, int pageNo, bool isForEXL, string? searchText)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId <= 0 || businessUnitId != tokenData.businessUnitId)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var query = from em in _context.EmpEmployeeBasicInfos
                            join lem in _context.MasterDepartments on em.IntDepartmentId equals lem.IntDepartmentId into lemGroup
                            from lem in lemGroup.DefaultIfEmpty()
                            join led in _context.MasterDesignations on em.IntDesignationId equals led.IntDesignationId into ledGroup
                            from led in ledGroup.DefaultIfEmpty()
                            join det in _context.EmpEmployeeBasicInfoDetails on em.IntEmployeeBasicInfoId equals det.IntEmployeeId into detGroup
                            from det in detGroup.DefaultIfEmpty()
                            join photo in _context.EmpEmployeePhotoIdentities on em.IntEmployeeBasicInfoId equals photo.IntEmployeeBasicInfoId into photoGroup
                            from photo in photoGroup.DefaultIfEmpty()
                            where em.IntAccountId == tokenData.accountId && em.IntBusinessUnitId == tokenData.businessUnitId && em.IsActive == true
                            && (det.IntEmployeeStatusId == 1 || det.IntEmployeeStatusId == 4)
                            && (string.IsNullOrEmpty(searchText) ? true
                            : (em.StrEmployeeName.Contains(searchText) || em.StrEmployeeCode.Contains(searchText) || det.StrPersonalMobile.Contains(searchText)
                            || det.StrPersonalMail.Contains(searchText) || det.StrOfficeMail.Contains(searchText)))
                            orderby em.StrEmployeeName ascending
                            select new
                            {
                                EmployeeId = em.IntEmployeeBasicInfoId,
                                EmployeeName = em.StrEmployeeName,
                                EmployeeCode = em.StrEmployeeCode,
                                DepartmentName = lem.StrDepartment,
                                strReferenceId = em.StrReferenceId,
                                DesignationName = led.StrDesignation,
                                Phone = det.StrPersonalMobile,
                                Email = det.StrPersonalMail,
                                photo.IntProfilePicFileUrlId,
                                em.IntBusinessUnitId
                            };

                if (!isForEXL)
                {
                    pageSize = pageSize > 1000 ? 1000 : pageSize;
                    pageNo = pageNo < 1 ? 1 : pageNo;


                    var result = new
                    {
                        TotalCount = await query.CountAsync(),
                        Data = await query.Skip((int)pageSize * ((int)pageNo - 1)).Take((int)pageSize).ToListAsync(),
                        PageSize = pageSize,
                        CurrentPage = pageNo
                    };

                    return Ok(result);
                }
                else
                {
                    var data = await query.ToListAsync();
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("LoanCRUD")]
        public async Task<IActionResult> LoanCRUD(LoanViewModel obj)
        {
            try
            {
                if (obj.EffectiveDate.Value.Year < DateTime.Now.Year && obj.EffectiveDate.Value.Month < DateTime.Now.Month)
                {
                    return NotFound("Invalid Date");
                }
                else
                {
                    var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                    if (tokenData.accountId == -1)
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }

                    if (tokenData.employeeId != obj.EmployeeId)
                    {
                        var check = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.BusinessUnitId, obj.WorkPlaceGrop, obj.EmployeeId, "");
                        if (check.Count() <= 0)
                        {
                            return BadRequest(new MessageHelperAccessDenied());

                        }
                    }
                    obj.IntAccountId = tokenData.accountId;
                    MessageHelper msg = await _employeeService.LoanCRUD(obj);

                    if (msg.StatusCode == 200)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        return BadRequest(msg);
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("LoanReSchedule")]
        public async Task<IActionResult> LoanReSchedule(List<LoanApproveLandingViewModel> model)
        {
            try
            {
                List<EmpLoanApplication> applicationList = new List<EmpLoanApplication>();
                foreach (LoanApproveLandingViewModel app in model)
                {
                    EmpLoanApplication obj = await _context.EmpLoanApplications.Where(x => x.IntLoanApplicationId == app.LoanApplicationId).FirstOrDefaultAsync();
                    if (obj.IsPipelineClosed == true && obj.IsReject == false && obj.IsActive == true)
                    {
                        obj.IntReScheduleNumberOfInstallment = app.ApproveNumberOfInstallment;//
                        obj.IntReScheduleNumberOfInstallmentAmount = app.ApproveNumberOfInstallmentAmount;//
                        obj.StrReScheduleRemarks = app.ReScheduleRemarks;
                        obj.IntApproveNumberOfInstallment = app.ApproveNumberOfInstallment;
                        obj.IntApproveNumberOfInstallmentAmount = app.ApproveNumberOfInstallmentAmount;
                        obj.IntUpdatedBy = app.UpdateByUserId;
                        obj.DteEffectiveDate = app.ApproveDate;
                        obj.DteUpdatedAt = DateTime.Now;
                        obj.IntReScheduleCount = obj.IntReScheduleCount + 1;
                        obj.DteReScheduleDateTime = obj.DteUpdatedAt;
                        applicationList.Add(obj);
                    }
                }
                _context.EmpLoanApplications.UpdateRange(applicationList);
                await _context.SaveChangesAsync();

                return Ok("successfull");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetLoanApplicationByAdvanceFilter")]
        public async Task<IActionResult> GetLoanApplicationByAdvanceFilter(LoanApplicationByAdvanceFilterViewModel obj)
        {
            try
            {

                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)obj.BusinessUnitId, workplaceGroupId = (long)obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                obj.AccountId = tokenData.accountId;
                var data = await _employeeService.GetLoanApplicationByAdvanceFilter(obj);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("CombineDataSetByWorkplaceGDeptDesigSupEmpType")]
        public async Task<IActionResult> CombineDataSetByWorkplaceGDeptDesigSupEmpType(long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? SupervisorId, long? EmpType, long? WorkplaceId)
        {
            try
            {
                IEnumerable<CustomViewModel> data = await (from E in _context.EmpEmployeeBasicInfos
                                                           join ed in _context.EmpEmployeeBasicInfoDetails on E.IntEmployeeBasicInfoId equals ed.IntEmployeeId
                                                           join B in _context.MasterBusinessUnits on E.IntBusinessUnitId equals B.IntBusinessUnitId into BB
                                                           from b in BB.DefaultIfEmpty()
                                                           join W in _context.MasterWorkplaces on E.IntWorkplaceId equals W.IntWorkplaceGroupId into WW
                                                           from w in WW.DefaultIfEmpty()
                                                           join D in _context.MasterDepartments on E.IntDepartmentId equals D.IntDepartmentId into DD
                                                           from d in DD.DefaultIfEmpty()
                                                           join DES in _context.MasterDesignations on E.IntDesignationId equals DES.IntDesignationId into DESS
                                                           from des in DESS.DefaultIfEmpty()
                                                           join S in _context.EmpEmployeeBasicInfos on E.IntSupervisorId equals S.IntEmployeeBasicInfoId into SS
                                                           from s in SS.DefaultIfEmpty()
                                                           join ET in _context.MasterEmploymentTypes on E.IntEmploymentTypeId equals ET.IntEmploymentTypeId into ETT
                                                           from et in ETT.DefaultIfEmpty()
                                                           where (ed.IntEmployeeStatusId == 1 || ed.IntEmployeeStatusId == 4) && E.IsActive == true
                                                               && (WorkplaceGroupId == 0 || WorkplaceGroupId == null || E.IntWorkplaceGroupId == WorkplaceGroupId)
                                                               && (BusinessUnitId == 0 || BusinessUnitId == null || BusinessUnitId == E.IntBusinessUnitId)
                                                               && (DeptId > 0 ? DeptId == E.IntDepartmentId : true
                                                               && DesigId > 0 ? DesigId == E.IntDesignationId : true
                                                               && SupervisorId > 0 ? SupervisorId == E.IntSupervisorId : true
                                                               && EmpType > 0 ? EmpType == E.IntEmploymentTypeId : true)

                                                           select new CustomViewModel
                                                           {
                                                               IntEmployeeId = E.IntEmployeeBasicInfoId,
                                                               IntEmploymentTypeId = E.IntEmploymentTypeId,
                                                               EmploymentTypeName = et.StrEmploymentType,
                                                               StrEmployeeName = E.StrEmployeeName,
                                                               StrEmployeeCode = E.StrEmployeeCode,
                                                               IntBusinessUnitId = b.IntBusinessUnitId,
                                                               StrBusinessUnitName = b.StrBusinessUnit,
                                                               IntWorkplaceGroupId = w.IntWorkplaceGroupId,
                                                               StrWorkplaceGroupName = w.StrWorkplaceGroup,
                                                               IntDepartmentId = d.IntDepartmentId,
                                                               StrDepartmentName = d.StrDepartment,
                                                               IntDesignationId = des.IntDesignationId,
                                                               StrDesignationName = des.StrDesignation,
                                                               IntSupervisorId = E.IntSupervisorId,
                                                               SupervisorName = s.StrEmployeeName
                                                           }).AsNoTracking().AsQueryable().ToListAsync();

                dynamic groupResult = new
                {
                    WorkplaceGroupList = data.GroupBy(x => x.IntWorkplaceGroupId).Select(x => new { Id = x.Key, Name = x?.FirstOrDefault()?.StrWorkplaceGroupName }),
                    DepartmentList = data.GroupBy(x => x.IntDepartmentId).Select(x => new { Id = x.Key, Name = x?.FirstOrDefault()?.StrDepartmentName }),
                    DesignationList = data.GroupBy(x => x.IntDesignationId).Select(x => new { Id = x.Key, Name = x?.FirstOrDefault()?.StrDesignationName }),
                    SupervisorList = data.GroupBy(x => x.IntSupervisorId).Select(x => new { Id = x.Key, Name = x?.FirstOrDefault()?.SupervisorName }),
                    EmploymentTypeList = data.Where(x => x.IntEmploymentTypeId == 2 || x.IntEmploymentTypeId == 3).GroupBy(x => x.IntEmploymentTypeId).Select(x => new { Id = x.Key, Name = x?.FirstOrDefault()?.EmploymentTypeName }),
                    EmployeeList = data.GroupBy(x => x.IntEmployeeId).Select(x => new { Id = x.Key, Name = (x?.FirstOrDefault()?.StrEmployeeName + " (" + x?.FirstOrDefault()?.StrEmployeeCode + ")") })
                };
                return Ok(groupResult);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("CommonEmployeeDDL")]
        public async Task<IActionResult> CommonEmployeeDDL(long businessUnitId, long workplaceGroupId, string? searchText)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                IEnumerable<CommonEmployeeDDL> data = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, 0, searchText);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }



        [HttpGet]
        [Route("MonthlySalaryReportView")]
        public async Task<IActionResult> MonthlySalaryReportView(long BusinessUnitId, long Year, long Month, long? WorkplaceGroupId, long? DepartmentId, long? DesignationId, long? EmployeeId)
        {
            try
            {
                var SalaryGenerateHeaderList = await (from sl in _context.PyrSalaryGenerateHeaders
                                                      where sl.IntYearId == Year && sl.IntMonthId == Month && sl.IntBusinessUnitId == BusinessUnitId && sl.IsApprove == true
                                                      && (sl.IntWorkplaceGroupId == WorkplaceGroupId || WorkplaceGroupId == 0)
                                                      && (sl.IntDepartmentId == DepartmentId || DepartmentId == 0)
                                                      && (sl.IntDesignationId == DesignationId || DesignationId == 0)
                                                      && (sl.IntEmployeeId == EmployeeId || EmployeeId == 0)
                                                      join empp in _context.EmpEmployeeBasicInfos on sl.IntEmployeeId equals empp.IntEmployeeBasicInfoId into empp2
                                                      from emp in empp2.DefaultIfEmpty()
                                                      join deptt in _context.MasterDepartments on emp.IntDepartmentId equals deptt.IntDepartmentId into deptt2
                                                      from dept in deptt2.DefaultIfEmpty()
                                                      join dess in _context.MasterDesignations on emp.IntDesignationId equals dess.IntDesignationId into dess2
                                                      from des in dess2.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          SalaryGenerateHeaderObj = sl,
                                                          EmployeeBasicInfoObj = emp,
                                                          EmpDepartmentObj = dept,
                                                          EmpDesignationObj = des,
                                                      }).AsNoTracking().ToListAsync();

                return Ok(SalaryGenerateHeaderList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("MonthlySalaryDepartmentWiseReportView")]
        public async Task<IActionResult> MonthlySalaryDepartmentWiseReportView(long BusinessUnitId, long Year, long Month, long? WorkplaceGroupId, long? DepartmentId, long? DesignationId)
        {
            try
            {
                var SalaryData = await (from sl in _context.PyrSalaryGenerateHeaders
                                        where sl.IntYearId == Year && sl.IntMonthId == Month && sl.IntBusinessUnitId == BusinessUnitId && sl.IsApprove == true
                                        join empp in _context.EmpEmployeeBasicInfos on sl.IntEmployeeId equals empp.IntEmployeeBasicInfoId into empp2
                                        from emp in empp2.DefaultIfEmpty()
                                        join empb in _context.EmpEmployeeBasicInfoDetails on sl.IntEmployeeId equals empb.IntEmployeeId into empb2
                                        from emb in empb2.DefaultIfEmpty()
                                        join deptt in _context.MasterDepartments on emp.IntDepartmentId equals deptt.IntDepartmentId into deptt2
                                        from dept in deptt2.DefaultIfEmpty()
                                        join dess in _context.MasterDesignations on emp.IntDesignationId equals dess.IntDesignationId into dess2
                                        from des in dess2.DefaultIfEmpty()
                                        join paygg in _context.PyrPayscaleGrades on emb.IntPayscaleGradeId equals paygg.IntPayscaleGradeId into paygg2
                                        from payg in paygg2.DefaultIfEmpty()
                                        where (sl.IntWorkplaceGroupId == WorkplaceGroupId || WorkplaceGroupId == 0)
                                              && (sl.IntDepartmentId == DepartmentId || DepartmentId == 0)
                                              && (sl.IntDesignationId == DesignationId || DesignationId == 0)
                                        group sl by sl.IntDepartmentId into salg
                                        select new MonthlySalaryDepartmentWiseReportViewModel
                                        {
                                            DepartmentId = salg.Key,
                                            SalaryGenerateHeaderList = salg.ToList()
                                        }).AsNoTracking().ToListAsync();

                List<MonthlySalaryDepartmentWiseReportViewModel> ResultSet = new List<MonthlySalaryDepartmentWiseReportViewModel>();
                string monthName = Month == 1 ? "January" : Month == 2 ? "February" : Month == 3 ? "March" : Month == 4 ? "April"
                    : Month == 5 ? "May" : Month == 6 ? "June" : Month == 7 ? "July" : Month == 8 ? "August" : Month == 9 ? "September"
                    : Month == 10 ? "October" : Month == 11 ? "November" : Month == 11 ? "December" : "";
                monthName = monthName + " " + Year;

                MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == BusinessUnitId).AsNoTracking().FirstOrDefaultAsync();

                foreach (var item in SalaryData)
                {
                    MonthlySalaryDepartmentWiseReportViewModel obj = new MonthlySalaryDepartmentWiseReportViewModel
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.SalaryGenerateHeaderList.FirstOrDefault().StrDepartment,
                        ManPowerCount = item.SalaryGenerateHeaderList.Select(x => x.IntEmployeeId).ToList().Distinct().Count(),
                        //OverTimeAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumOverTimeAmount),
                        //ExtraSideDutyAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumExtraSideDutyAllowance),
                        //NightDutyAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumNightAllowance),
                        //AttendanceBonusAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumAttendanceBonusAllowance),
                        //Salary = item.SalaryGenerateHeaderList.Sum(a => a.NumBasicSalary),
                        GrossSalary = item.SalaryGenerateHeaderList.Sum(a => a.NumGrossSalary),
                        //CbaAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumCba),
                        //DeductAmount = item.SalaryGenerateHeaderList.Sum(a => a.NumTotalDeductionCal),
                        //NetPayable = item.SalaryGenerateHeaderList.Sum(a => a.NumNetPayableAmountCal)
                    };
                    ResultSet.Add(obj);
                }

                return Ok(ResultSet);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("LeaveBalanceHistoryForAllEmployee")]
        public async Task<IActionResult> LeaveBalanceHistoryForAllEmployee(long BusinessUnitId, long yearId, long? WorkplaceGroupId, string? SearchText, bool IsPaginated, int? PageNo = 1, int? PageSize = 10)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (BusinessUnitId <= 0 || yearId <= 0)
                {
                    return NotFound("Invalid Business Unit OR Year");
                }

                List<LveLeaveBalance> leaveBalanceList = await (from item in _context.LveLeaveBalances
                                                                where item.IsActive == true
                                                                && (yearId > 0 ? item.IntYear == yearId : true)
                                                                select item).AsNoTracking().ToListAsync();
                EmployeeProfileLandingPaginationViewModel returnObj = new();

                if (leaveBalanceList.Count <= 0)
                {
                    return Ok(returnObj);
                }
                List<EmployeeQryProfileAllViewModel> employeeList = await _employeeService.EmployeeQryProfileAllList(tokenData.accountId, BusinessUnitId, WorkplaceGroupId, null, null, null);


                List<LveLeaveType> leaveTypeList = await (from item in _context.LveLeaveTypes
                                                          where item.IsActive == true && item.IntAccountId == tokenData.accountId
                                                          select item).AsNoTracking().ToListAsync();

                var data = (from emp in employeeList
                            select new Models.Employee.LeaveBalanceHistoryForAllEmployeeViewModel
                            {
                                EmployeeId = emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId,
                                Employee = emp?.EmployeeBasicInfo?.StrEmployeeName,
                                EmployeeCode = emp?.EmployeeBasicInfo?.StrEmployeeCode,
                                Department = emp.DepartmentName + ", " + emp.EmploymentTypeName,
                                Designation = emp.DesignationName,
                                CLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "CL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                CLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "CL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                SLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "SL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                SLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "SL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                ELBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "EL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                ELTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "EL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                LWPBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "LWP")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                LWPTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "LWP")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                MLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "ML")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                MLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeBasicInfoId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "ML")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                            }).AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = SearchText.ToLower();
                    data = data.Where(x =>
                                        x.Employee.ToLower().Contains(SearchText) ||
                                        x.EmployeeCode.ToLower().Contains(SearchText) ||
                                        x.Department.ToLower().Contains(SearchText) ||
                                        x.Designation.ToLower().Contains(SearchText));

                }
                returnObj.TotalCount = data.Count();
                returnObj.CurrentPage = (long)PageNo;
                returnObj.PageSize = (long)PageSize;
                returnObj.Data = IsPaginated == true ? data.Skip((int)PageNo - 1).Take((int)PageSize).ToList() : data.ToList();
                return Ok(returnObj);
            }
            catch (Exception ex)
            {
                return NotFound("Something went wrong");
            }
        }

        [HttpGet]
        [Route("AllEmployeeMovementReport")]
        public async Task<IActionResult> AllEmployeeMovementReport(long AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? MovementTypeId, long? EmployeeId, DateTime FromDate, DateTime ToDate, string? applicationStatus)
        {
            try
            {
                List<MovementReportViewModel> data = new List<MovementReportViewModel>();
                MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == BusinessUnitId).AsNoTracking().FirstOrDefaultAsync();

                string date = FromDate.ToString("dd-MMMM-yyyy") + " To " + ToDate.ToString("dd-MMMM-yyyy");

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprMovementDetailsReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@dteUserFromDate", FromDate.Date);
                        sqlCmd.Parameters.AddWithValue("@dteUserToDate", ToDate.Date);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intDeptId", DeptId);
                        sqlCmd.Parameters.AddWithValue("@intDesigId", DesigId);
                        sqlCmd.Parameters.AddWithValue("@intMovementTypeId", MovementTypeId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@applicationStatus ", applicationStatus);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                var res = JsonConvert.SerializeObject(dt);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }
        //[HttpGet]
        //[Route("GetOverTimeLanding")]
        //public async Task<IActionResult> GetOverTimeLanding(long BusinessUnitId, long WorkplaceGroupId, DateTime FromDate, DateTime ToDate)
        //{
        //    try
        //    {
        //        var result = 
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        [HttpGet]
        [Route("EmployeeMovementReportAll")]
        public async Task<IActionResult> EmployeeMovementReportAll(long BusinessUnitId, long WorkplaceGroupId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, string SearchText, bool IsPaginated)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var retObj = await _employeeService.EmployeeMovementReportAll(BusinessUnitId, WorkplaceGroupId, FromDate, ToDate, PageNo, PageSize, SearchText, IsPaginated, tokenData);


                return Ok(retObj);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("OvertimeReportLanding")]
        public async Task<IActionResult> OvertimeReportLanding(string? PartType, long BusinessUnitId, long? WorkplaceGroupId, DateTime FromDate, DateTime ToDate, string SearchText = "", bool IsPaginated = false, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprOvertimeReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@partType", PartType);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceId", null);
                        sqlCmd.Parameters.AddWithValue("@intDepartmentId", null);
                        sqlCmd.Parameters.AddWithValue("@intDesignationId", null);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate.Date);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate.Date);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", null);

                        sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
                        sqlCmd.Parameters.AddWithValue("@strSearchText", SearchText);
                        sqlCmd.Parameters.AddWithValue("@IsPaginated", IsPaginated);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<OvertimeReport> dataList = new List<OvertimeReport>();
                foreach (DataRow row in dt.Rows)
                {
                    OvertimeReport overtime = new OvertimeReport();

                    overtime.intEmployeeId = Convert.ToInt64(row["intEmployeeId"]);
                    overtime.strEmployeeCode = row["strEmployeeCode"].ToString();
                    overtime.strEmployeeName = row["strEmployeeName"].ToString();
                    overtime.strDesignationName = row["strDesignation"].ToString();
                    overtime.strDepartmentName = row["strDepartment"].ToString();
                    overtime.EmployementType = row["EmployementType"].ToString();
                    overtime.numGrossAmount = Convert.ToDecimal(row["numGrossAmount"]);
                    overtime.numBasicSalary = Convert.ToDecimal(row["numBasicSalary"]);
                    overtime.dteOverTimeDate = (DateTime)(row["dteOverTimeDate"]);
                    overtime.timeStartTime = (TimeSpan)(row["tmeStartTime"]);
                    overtime.timeEndTime = (TimeSpan)(row["tmeEndTime"]);
                    overtime.numHours = Convert.ToDecimal(row["numHours"]);
                    overtime.numPerMinunitRate = Convert.ToDecimal(row["numPerHourRate"]);
                    overtime.numTotalAmount = Convert.ToDecimal(row["numTotalAmount"]);
                    overtime.strReason = row["strReason"].ToString();
                    overtime.TotalCount = Convert.ToInt64(row["totalCount"]);

                    dataList.Add(overtime);
                }

                // process data
                var groupDataSet = dataList.GroupBy(x => x.intEmployeeId).Select(x => new OvertimeReportViewModel
                {
                    EmployeeId = x.FirstOrDefault()?.intEmployeeId,
                    Employee = x.FirstOrDefault()?.strEmployeeName + " [" + x.FirstOrDefault()?.strEmployeeCode + "]",
                    EmployeeCode = x.FirstOrDefault()?.strEmployeeCode,
                    OverTimeDate = x.FirstOrDefault()?.dteOverTimeDate,
                    Designation = x?.FirstOrDefault()?.strDesignationName,
                    Department = x?.FirstOrDefault()?.strDepartmentName,
                    EmployementType = x?.FirstOrDefault()?.EmployementType,
                    Salary = x?.FirstOrDefault()?.numBasicSalary,
                    BasicSalary = x?.FirstOrDefault()?.numBasicSalary,
                    Hours = x?.ToList()?.Sum(x => x.numHours),
                    PerHourRate = x?.FirstOrDefault()?.numPerMinunitRate,
                    PayAmount = x?.ToList()?.Sum(x => x.numHours) * x?.FirstOrDefault()?.numPerMinunitRate,
                    totalCount = (long)x.FirstOrDefault()?.TotalCount
                }).ToList();

                return Ok(groupDataSet);
            }
            catch (Exception ex)
            {
                return NotFound("Invalid Data");
            }
        }

        private static List<T> ConvertDataTableToList<T>(System.Data.DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        [HttpPost]
        [Route("EmployeeReportWithFilter")]
        public async Task<IActionResult> EmployeeReportWithFilter(profileLandingFilterVM filter)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = filter.businessUnitId, workplaceGroupId = filter.workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var employee = await _employeeService.EmployeeReportWithFilter(tokenData, filter.businessUnitId, filter.workplaceGroupId, filter.IsPaginated, filter.searchTxt,
                         filter.PageSize, filter.PageNo, filter.IsHeaderNeed, filter.StrDepartmentList, filter.StrDesignationList, filter.StrSupervisorNameList,
                        filter.StrEmploymentTypeList, filter.StrLinemanagerList, filter.WingNameList, filter.SoleDepoNameList, filter.RegionNameList, filter.AreaNameList, filter.TerritoryNameList);

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #region ========= Separation =============
        [HttpGet]
        [Route("EmployeeSeparationListFilter")]
        public async Task<IActionResult> EmployeeSeparationListFilter(long BusinessUnitId, long WorkplaceGroupId, long? EmployeeId, DateTime? FromDate, DateTime? ToDate, bool IsForXl, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (EmployeeId > 0)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, BusinessUnitId, WorkplaceGroupId, EmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }
                }

                var separationList = await _employeeService.EmployeeSeparationListFilter(tokenData.accountId, BusinessUnitId, WorkplaceGroupId, EmployeeId, FromDate, ToDate, IsForXl, PageNo, PageSize, searchTxt, tokenData);
                return Ok(separationList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpGet]
        [Route("EmployeeSeparationById")]
        public async Task<IActionResult> EmployeeSeparationById(long SeparationId)
        {
            try
            {
                var separationList = await _employeeService.EmployeeSeparationById(SeparationId);

                return Ok(separationList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("EmployeeRejoinFromSeparation")]
        public async Task<IActionResult> EmployeeRejoinFromSeparation(EmployeeRejoinFromSeparationVM model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                MessageHelper message = new MessageHelper();

                if (model.IsRejoin)
                {
                    message = await _employeeService.EmployeeRejoinFromSeparation(model, tokenData.employeeId);
                }

                if (message.StatusCode == 200)
                {
                    return Ok(message);
                }

                return BadRequest(message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("EmployeeSeparationReportFilter")]
        public async Task<IActionResult> EmployeeSeparationReportFilter(EmployeeSeparationReportVM model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                EmployeeSeparationReportLanding separatedList = await _employeeService.EmployeeSeparationReportFilter(tokenData, model.BusinessUnitId, model.WorkplaceGroupId, model.FromDate, model.ToDate, model.PageNo, model.PageSize, model.SearchTxt,
                    model.IsPaginated, model.IsHeaderNeed, model.StrDepartmentList, model.StrDesignationList, model.StrSupervisorNameList, model.StrLinemanagerList, model.StrEmploymentTypeList, model.WingNameList, model.SoleDepoNameList, model.RegionNameList,
                    model.AreaNameList, model.TerritoryNameList);

                return Ok(separatedList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #endregion

        //[HttpPost]
        //[Route("EmployeeSeparationListFilter")]
        //public async Task<IActionResult> EmployeeSeparationListFilter(EmployeeSeparationListFilterViewModel obj)
        //{
        //    try
        //    {
        //        var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
        //        if (tokenData.accountId == -1)
        //        {
        //            return BadRequest(new MessageHelperAccessDenied());
        //        }

        //        if (tokenData.employeeId != obj.EmployeeId)
        //        {
        //            var check = await _employeeService.GetCommonEmployeeDDL(tokenData, (long)obj.BusinessUnitId, (long)obj.WorkplaceGroupId, obj.EmployeeId, "");
        //            if (check.Count() <= 0)
        //            {
        //                return BadRequest(new MessageHelperAccessDenied());

        //            }
        //        }

        //        obj.AccountId = tokenData.accountId;

        //        string separationList = JsonConvert.SerializeObject(await (_employeeService.EmployeeSeparationListFilter(obj)));
        //        return Ok(separationList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}

        [HttpGet]
        [Route("SalaryTaxCertificate")]
        public async Task<IActionResult> SalaryTaxCertificate(long businessUnitId, long workplaceGroupId, long FiscalYearId, string FiscalYear, long EmployeeId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != EmployeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, EmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }
                }

                SalaryTaxCertificateViewModel salaryTaxCertificate = await _employeeService.SalaryTaxCertificate(FiscalYearId, FiscalYear, EmployeeId);

                return Ok(salaryTaxCertificate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("EmpJobConfirmation")]
        public async Task<IActionResult> EmpJobConfirmation(long BusinessUnitId, long WorkplaceGroupId, long Month, long Year, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var result = await _employeeService.EmpJobConfirmation(tokenData.accountId, BusinessUnitId, WorkplaceGroupId, Month, Year, IsXls, PageNo, PageSize, searchTxt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }
        #endregion ======== Report ========

        #region ======= BonusManagement =======

        [HttpPost]
        [Route("CRUDBonusSetup")]
        public async Task<IActionResult> CRUDBonusSetup(CRUDBonusSetupViewModel obj)
        {
            try
            {
                var res = await _employeeService.CRUDBonusSetup(obj);

                if (res.StatusCode == 500)
                {
                    return BadRequest(res);
                }
                else
                {
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("BonusAllLanding")]
        public async Task<IActionResult> BonusAllLanding(BonusAllLandingViewModel obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.IntBusinessUnitId, workplaceGroupId = obj.IntWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                obj.IntAccountId = tokenData.accountId;

                var dt = await _employeeService.BonusAllLanding(obj);

                var res = JsonConvert.SerializeObject(dt);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("EligbleEmployeeForBonusGenerateLanding")]
        public async Task<IActionResult> EligbleEmployeeForBonusGenerateLanding(string StrPartName, long IntAccountId, long IntBusinessUnitId, long WorkplaceGroupId, long? WingId,
            long? SoleDepoId, long? RegionId, long? AreaId, long? TerritoryId, long? IntBonusHeaderId, long? IntBonusId, DateTime? DteEffectedDate, long? IntCreatedBy, long? IntPageNo, long? IntPageSize, string? searchText)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = IntBusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var dt = await _employeeService.EligbleEmployeeForBonusGenerateLanding(StrPartName, tokenData.accountId, IntBusinessUnitId, WorkplaceGroupId, WingId, SoleDepoId, RegionId, AreaId, TerritoryId, IntBonusHeaderId, IntBonusId, DteEffectedDate, IntCreatedBy, IntPageNo, IntPageSize, searchText);

                var res = JsonConvert.SerializeObject(dt);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CRUDBonusGenerate")]
        public async Task<IActionResult> CRUDBonusGenerate(CRUDBonusGenerateViewModel obj)
        {
            try
            {
                var res = await _employeeService.CRUDBonusGenerate(obj);

                if (res.StatusCode == 500)
                {
                    return BadRequest(res);
                }
                else
                {
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion ======= BonusManagement =======

        #region Salary Additon & Deduction

        [HttpPost]
        [Route("SalaryAdditonNDeduction")]
        public async Task<IActionResult> SalaryAdditonNDeduction(List<EmpSalaryAdditionNDeductionViewModel> modelList)
        {
            try
            {
                MessageHelper msg = new MessageHelper();
                string jsonString = "";

                long empId = modelList.Select(a => (long)a.intEmployeeId).First();

                PipelineStageInfoVM pipeline = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(empId, "salaryAdditionNDeduction");

                if (pipeline.HeaderId <= 0 || empId <= 0)
                {
                    return BadRequest(new MessageHelper { StatusCode = 404, Message = "Pipeline was not setup" });
                }

                var SalaryType = modelList.Select(x => x.strEntryType).First();
                BaseVM tokenData;

                if (SalaryType == "BulkUpload")
                {
                    List<long> empIdList = modelList.Select(a => a.EmployeeIdList).First();
                    var buID = modelList.Select(x => x.intBusinessUnitId).First();
                    var wgID = modelList.Select(x => x.intWorkplaceGroupId).First();
                    tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)buID, workplaceGroupId = (long)wgID }, PermissionLebelCheck.WorkplaceGroup);

                    IEnumerable<CommonEmployeeDDL> empIdCheck = await _employeeService.PermissionCheckFromEmployeeListByEnvetFireEmployee(tokenData, (long)buID, (long)wgID, empIdList, "");

                    if (empIdCheck.Count() != empIdList.Count())
                    {
                        msg.StatusCode = 401;
                        msg.Message = "Employee Id Not Authenticate!";
                        return BadRequest(msg);
                    }
                }
                else if (SalaryType == "ENTRY" || SalaryType == "EDIT")
                {
                    
                    var buID = modelList.Select(x => x.intBusinessUnitId).First();
                    var wgID = modelList.Select(x => x.intWorkplaceGroupId).First();
                    tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)buID, workplaceGroupId = (long)wgID }, PermissionLebelCheck.WorkplaceGroup);

                    IEnumerable<CommonEmployeeDDL> empIdCheck = await _employeeService.GetCommonEmployeeDDL(tokenData, (long)buID, (long)wgID, empId, "");

                    if (empIdCheck.Count() <= 0)
                    {
                        msg.StatusCode = 401;
                        msg.Message = "Employee Id Not Authenticate!";
                        return BadRequest(msg);
                    }
                }
                else
                {
                    var buID = modelList.Select(x => x.intBusinessUnitId).First();
                    var wgID = modelList.Select(x => x.intWorkplaceGroupId).First();
                    tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)buID, workplaceGroupId = (long)wgID }, PermissionLebelCheck.WorkplaceGroup);

                    if (!tokenData.isAuthorize)
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }
                }


                foreach (EmpSalaryAdditionNDeductionViewModel model in modelList)
                {
                    var connection = new SqlConnection(Connection.iPEOPLE_HCM);
                    string sql = "saas.sprPyrEmpSalaryAdditionNDeduction";
                    SqlCommand sqlCmd = new SqlCommand(sql, connection);

                    if (model.strEntryType == "BulkUpload")
                    {
                        if (model.EmployeeIdList is null)
                        {
                            msg.Message = "Invalid Employee List For Bulk Upload";
                            msg.StatusCode = 401;
                            return BadRequest(msg);
                        }
                        else if (model.EmployeeIdList.Count() > 0)
                        {
                            jsonString = System.Text.Json.JsonSerializer.Serialize(model.EmployeeIdList);
                        }
                        else
                        {
                            msg.Message = "Invalid Employee List For Bulk Upload";
                            msg.StatusCode = 401;
                            return BadRequest(msg);
                        }
                    }

                    

                    #region === SP Property ===
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@strEntryType", model.strEntryType);
                    sqlCmd.Parameters.AddWithValue("@intSalaryAdditionAndDeductionId", model.intSalaryAdditionAndDeductionId);
                    sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                    sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", model.intBusinessUnitId);
                    sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", model.intWorkplaceGroupId);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", model.intEmployeeId);
                    sqlCmd.Parameters.AddWithValue("@isAutoRenew", model.isAutoRenew);
                    sqlCmd.Parameters.AddWithValue("@intYear", model.intYear);
                    sqlCmd.Parameters.AddWithValue("@intMonth", model.intMonth);
                    sqlCmd.Parameters.AddWithValue("@strMonth", model.strMonth);
                    sqlCmd.Parameters.AddWithValue("@isAddition", model.isAddition);
                    sqlCmd.Parameters.AddWithValue("@strAdditionNDeduction", model.strAdditionNDeduction);
                    sqlCmd.Parameters.AddWithValue("@intAdditionNDeductionTypeId", model.intAdditionNDeductionTypeId);
                    sqlCmd.Parameters.AddWithValue("@intAmountWillBeId", model.intAmountWillBeId);
                    sqlCmd.Parameters.AddWithValue("@strAmountWillBe", model.strAmountWillBe);
                    sqlCmd.Parameters.AddWithValue("@numAmount", model.numAmount);
                    sqlCmd.Parameters.AddWithValue("@isActive", model.isActive);
                    sqlCmd.Parameters.AddWithValue("@isReject", model.isReject);
                    sqlCmd.Parameters.AddWithValue("@intActionBy", model.intActionBy);
                    sqlCmd.Parameters.AddWithValue("@intToYear", model.intToYear);
                    sqlCmd.Parameters.AddWithValue("@intToMonth", model.intToMonth);
                    sqlCmd.Parameters.AddWithValue("@strToMonth", model.strToMonth);
                    sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipeline.HeaderId);
                    sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipeline.CurrentStageId);
                    sqlCmd.Parameters.AddWithValue("@NextStageId", pipeline.NextStageId);
                    sqlCmd.Parameters.AddWithValue("@strJsonString", jsonString);

                    connection.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(dt);
                    }
                    connection.Close();
                    #endregion
                    if (model.strEntryType.ToLower() == "GetEmpSalaryAdditionNDeductionLanding".ToLower()
                        || model.strEntryType.ToLower() == "GetEmpSalaryAdditionNDeductionByEmployeeId".ToLower())
                    {
                        return Ok(JsonConvert.SerializeObject(dt));
                    }
                    else
                    {
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        dt = new System.Data.DataTable();
                        jsonString = "";
                    }
                }

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpPost]
        [Route("SalaryAdditionDeductionLanding")]
        public async Task<IActionResult> SalaryAdditionDeductionLanding(EmpAdditionDeductionFiltering filter)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)filter.BusinessUnitId, workplaceGroupId = (long)filter.workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var result = await _employeeService.SalaryAdditionDeductionLanding(tokenData.accountId, filter.IntMonth, filter.IntYear, filter.BusinessUnitId, filter.workplaceGroupId, filter.PageNo, filter.PageSize, filter.searchTxt, filter.IsHeaderNeed,
                                    filter.WingNameList, filter.SoleDepoNameList, filter.RegionNameList, filter.AreaNameList, filter.TerritoryNameList);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("BulkSalaryAdditionNDeduction")]
        public async Task<IActionResult> BulkSalaryAdditionNDeduction(BulkSalaryAdditionNDeductionViewModel model)
        {
            try
            {
                MessageHelperWithValidation msg = await _employeeService.BulkSalaryAdditionNDeduction(model);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperWithValidation() { StatusCode = 500, Message = ex.Message});
            }
        }
        #endregion Salary Additon & Deduction

        #region ======= IOU =======

        [HttpPost]
        [Route("IOUApplicationCreateEdit")]
        public async Task<IActionResult> IOUApplicationCreateEdit(IOUApplicationViewModel iOUApplicationViewModel)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.employeeId != iOUApplicationViewModel.intEmployeeId)
                {
                    var data = await _employeeService.GetCommonEmployeeDDL(tokenData, (long)iOUApplicationViewModel.intBusinessUnitId, (long)iOUApplicationViewModel.intWorkplaceGroupId, iOUApplicationViewModel.intEmployeeId, null);

                    if (data.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());
                    }
                }

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    var pipe = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(iOUApplicationViewModel.intEmployeeId, "iou");

                    string sql = "saas.sprIOUApplicationCreateEdit";

                    string jsonString = System.Text.Json.JsonSerializer.Serialize(iOUApplicationViewModel.UrlIdViewModelList);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strEntryType", iOUApplicationViewModel.strEntryType);
                        sqlCmd.Parameters.AddWithValue("@intIOUId", iOUApplicationViewModel.intIOUId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", iOUApplicationViewModel.intEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", iOUApplicationViewModel.dteFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", iOUApplicationViewModel.dteToDate);
                        sqlCmd.Parameters.AddWithValue("@numIOUAmount", iOUApplicationViewModel.numIOUAmount);
                        sqlCmd.Parameters.AddWithValue("@numAdjustedAmount", iOUApplicationViewModel.numAdjustedAmount);
                        sqlCmd.Parameters.AddWithValue("@numPayableAmount", iOUApplicationViewModel.numPayableAmount);
                        sqlCmd.Parameters.AddWithValue("@numReceivableAmount", iOUApplicationViewModel.numReceivableAmount);
                        sqlCmd.Parameters.AddWithValue("@strDiscription", iOUApplicationViewModel.strDiscription);
                        sqlCmd.Parameters.AddWithValue("@isActive", iOUApplicationViewModel.isActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", tokenData.employeeId);
                        sqlCmd.Parameters.AddWithValue("@intUpdatedBy", tokenData.employeeId);
                        sqlCmd.Parameters.AddWithValue("@isAdjustment", iOUApplicationViewModel.isAdjustment);
                        sqlCmd.Parameters.AddWithValue("@intIOUAdjustmentId", iOUApplicationViewModel.intIOUAdjustmentId);
                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipe.HeaderId);
                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipe.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", pipe.NextStageId);


                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetAllIOULanding")]
        public async Task<IActionResult> GetAllIOULanding(string strReportType, long? intBusinessUnitId, long? WorkplaceGroupId, long? intIOUId, DateTime? applicationDate,
            DateTime? fromDate, DateTime? toDate, string? status, string? searchTxt, string? strDocFor, int? pageNo, int? pageSize)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)intBusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.isAuthorize)
                {
                    using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                    {
                        string sql = "saas.sprIOUAllSelectQuery";
                        using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@strReportType", strReportType);
                            sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                            sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                            sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                            sqlCmd.Parameters.AddWithValue("@intEmployeeId", tokenData.employeeId);
                            sqlCmd.Parameters.AddWithValue("@intIOUId", intIOUId);
                            sqlCmd.Parameters.AddWithValue("@dteApplicationDate", applicationDate);
                            sqlCmd.Parameters.AddWithValue("@dteFromDate", fromDate);
                            sqlCmd.Parameters.AddWithValue("@dteToDate", toDate);
                            sqlCmd.Parameters.AddWithValue("@strStatus", status);
                            sqlCmd.Parameters.AddWithValue("@strDocFor", strDocFor);
                            sqlCmd.Parameters.AddWithValue("@strSearchTxt", searchTxt);
                            sqlCmd.Parameters.AddWithValue("@intPageNo", pageNo);
                            sqlCmd.Parameters.AddWithValue("@intPageSize", pageSize);

                            connection.Open();
                            using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                            {
                                sqlAdapter.Fill(dt);
                            }
                            connection.Close();
                        }
                    }
                    string dataTbl = JsonConvert.SerializeObject(dt);
                    return Ok(dataTbl);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetIOULanding")]
        public async Task<IActionResult> GetIOULanding(long businessUnitId, long workplaceGroupId, long? employeeId, DateTime? fromDate, DateTime? toDate, string? searchTxt, int pageNo, int pageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)businessUnitId, workplaceGroupId = (long)workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                searchTxt = string.IsNullOrEmpty(searchTxt) ? searchTxt : searchTxt.ToLower();

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                IQueryable<IOUApplicationLandingVM> iouDala = (from iou in _context.PyrIouapplications
                                                               join emp in _context.EmpEmployeeBasicInfos on iou.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                               join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                                               from empD in empD2.DefaultIfEmpty()
                                                               join iouH in _context.PyrIouadjustmentHistories on iou.IntIouid equals iouH.IntIouid into iouHJoin
                                                               from iouH in iouHJoin.DefaultIfEmpty()
                                                               where iou.IsActive == true && emp.IntAccountId == tokenData.accountId
                                                                 //&& (employeeId == null || employeeId == 0 || data.Count() > 0 ? iou.IntEmployeeId == employeeId: iou.IntEmployeeId == 0)
                                                                 && (employeeId > 0 ? employeeId == tokenData.employeeId : true)
                                                                 && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                                                                 && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                                                                 && ((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                 : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                                 : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                                                                 : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                                                                 : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                                                                 : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                                                                 : tokenData.territoryList.Contains(empD.IntTerritoryId))
                                                                  //&& (strStatus == null || iou.StrStatus == strStatus || strStatus == "")
                                                                  //&& (iou.DteApplicationDate.Date == dteApplicationDate.Date || dteApplicationDate == null)
                                                                  && (fromDate == null || toDate == null || (iou.DteApplicationDate.Date >= fromDate.Value.Date && iou.DteApplicationDate.Date <= toDate.Value.Date))
                                                                  && (searchTxt == null || searchTxt == "" || iou.StrIoucode.Contains(searchTxt) || emp.StrEmployeeName.Contains(searchTxt) || emp.StrEmployeeCode.Contains(searchTxt))
                                                               orderby iou.IntIouid descending
                                                               select new IOUApplicationLandingVM
                                                               {
                                                                   IOUId = iou.IntIouid,
                                                                   IOUCode = iou.StrIoucode,
                                                                   EmployeeId = iou.IntEmployeeId,
                                                                   EmployeeName = emp.StrEmployeeName,
                                                                   EmployeeCode = emp.StrEmployeeCode,
                                                                   businessUnitId = emp.IntBusinessUnitId,
                                                                   workplaceGroupId = emp.IntWorkplaceGroupId,
                                                                   ApplicationDate = iou.DteApplicationDate,
                                                                   dteFromDate = iou.DteFromDate,
                                                                   dteToDate = iou.DteToDate,
                                                                   numIOUAmount = iou.NumIouamount,
                                                                   numAdjustedAmount = iou.NumAdjustedAmount,
                                                                   numPayableAmount = iou.NumPayableAmount,
                                                                   numReceivableAmount = iou.NumReceivableAmount,
                                                                   Status = (iou.IsReject == false && iou.IsPipelineClosed == true) ? "Approved" :
                                                                            (iou.IsReject == false && iou.IsPipelineClosed == false && iou.StrStatus == "Pending") ? "Pending" :
                                                                            (iou.IsReject == false && iou.IsPipelineClosed == false) ? "Process" :
                                                                            (iou.IsReject == true) ? "Rejected" : null,
                                                                   AdjustmentStatus = (iouH.IsAcknowledgement == true) ? "Completed" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == true) ? "Adjusted" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == false && iouH.StrStatus == "Pending") ? "Pending" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == false) ? "Process" :
                                                                                      (iouH.IsReject == true) ? "Rejected" : null,
                                                                   Discription = iou.StrDiscription,
                                                                   intIOUAdjustmentId = iouH.IntIouadjustmentId,
                                                                   PendingAdjAmount = iou.NumPendingAdjAmount
                                                               }).AsNoTracking().AsQueryable();

                IOUApplicationLandingPaginetionVM iouApplication = new();

                int maxSize = 1000;
                pageSize = pageSize > maxSize ? maxSize : pageSize;
                pageNo = pageNo < 1 ? 1 : pageNo;

                iouApplication.TotalCount = await iouDala.CountAsync();
                iouApplication.iouApplicationLandings = await iouDala.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToListAsync();
                iouApplication.PageSize = (int)pageSize;
                iouApplication.CurrentPage = (int)pageNo;

                return Ok(iouApplication);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError());
            }
        }

        [HttpGet]
        [Route("IOULandingById")]
        public async Task<IActionResult> IOULandingById(long intIOUId)
        {
            try
            {
                var query = (from iou in _context.PyrIouapplications
                             join emp in _context.EmpEmployeeBasicInfos on iou.IntEmployeeId equals emp.IntEmployeeBasicInfoId into empJoin
                             from emp in empJoin.DefaultIfEmpty()
                             join iouH in _context.PyrIouadjustmentHistories on iou.IntIouid equals iouH.IntIouid into iouHJoin
                             from iouH in iouHJoin.DefaultIfEmpty()
                             where (iou.IntIouid == intIOUId || intIOUId == 0)
                             select new IOUApplicationLandingVM
                             {
                                 IOUId = iou.IntIouid,
                                 IOUCode = iou.StrIoucode,
                                 EmployeeId = iou.IntEmployeeId,
                                 EmployeeName = emp.StrEmployeeName,
                                 EmployeeCode = emp.StrEmployeeCode,
                                 businessUnitId = emp.IntBusinessUnitId,
                                 workplaceGroupId = emp.IntWorkplaceGroupId,
                                 ApplicationDate = iou.DteApplicationDate,
                                 dteFromDate = iou.DteFromDate,
                                 dteToDate = iou.DteToDate,
                                 numIOUAmount = iou.NumIouamount,
                                 numAdjustedAmount = iou.NumAdjustedAmount,
                                 numPayableAmount = iou.NumPayableAmount,
                                 numReceivableAmount = iou.NumReceivableAmount,
                                 Status = (iou.IsReject == false && iou.IsPipelineClosed == true) ? "Approved" :
                                                                            (iou.IsReject == false && iou.IsPipelineClosed == false && iou.StrStatus == "Pending") ? "Pending" :
                                                                            (iou.IsReject == false && iou.IsPipelineClosed == false) ? "Process" :
                                                                            (iou.IsReject == true) ? "Rejected" : null,
                                 AdjustmentStatus = (iouH.IsAcknowledgement == true) ? "Completed" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == true) ? "Adjusted" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == false && iouH.StrStatus == "Pending") ? "Pending" :
                                                                                      (iouH.IsReject == false && iouH.IsPipelineClosed == false) ? "Process" :
                                                                                      (iouH.IsReject == true) ? "Rejected" : null,
                                 Discription = iou.StrDiscription,
                                 intIOUAdjustmentId = iouH.IntIouadjustmentId,
                                 PendingAdjAmount = iou.NumPendingAdjAmount

                             }).FirstOrDefault();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("IouDocList")]
        public async Task<IActionResult> IouDocList(long intIOUId, string strDocFor)
        {
            try
            {
                var query = (from D in _context.PyrIoudocuments
                             join F in _context.GlobalFileUrls on D.IntDocUrlid equals F.IntDocumentId
                             where F.IsActive == true && D.IntIouid == intIOUId && D.StrDocFor == strDocFor
                             orderby D.IntDocUrlid descending
                             select new
                             {
                                 IntIoudocId = D.IntIoudocId,
                                 IntDocUrlid = D.IntDocUrlid,
                                 StrDocFor = D.StrDocFor
                             }).ToList();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }
        #endregion ======= IOU =======

        #region ======= Bulk Tax Assign =======

        [HttpPost]
        [Route("SaveTaxBulkUpload")]
        public async Task<IActionResult> SaveTaxBulkUpload(List<TaxBulkUploadViewModel> model)
        {
            MessageHelperBulkUpload message = new MessageHelperBulkUpload();
            try
            {
                message = await _employeeService.SaveTaxBulkUpload(model);

                if (message.StatusCode == 500)
                {
                    return BadRequest(message);
                }
                else
                {
                    return Ok(message);
                }
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return BadRequest(message);
            }
        }

        #endregion ======= Bulk Tax Assign =======

        #region ======= Tax Assign =======

        [HttpPost]
        [Route("EmployeeTaxAssign")]
        public async Task<IActionResult> EmployeeTaxAssign(List<EmployeeTaxAssignViewModel> model)
        {
            try
            {
                MessageHelper msg = await _employeeService.EmployeeTaxAssign(model);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetAllEmployeeForTaxAssign")]
        public async Task<IActionResult> GetAllEmployeeForTaxAssign(long? IntBusinessUnitId, long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntEmployeeId, string? searchTxt, int PageNo, int PageSize, int? intAssignStatus)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)IntBusinessUnitId, workplaceGroupId = (long)IntWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var taxAssignList = await _employeeService.GetAllActiveEmployeeForTaxAssign(tokenData.accountId, IntBusinessUnitId, IntWorkplaceGroupId, IntWorkplaceId, IntEmployeeId, searchTxt, PageNo, PageSize, intAssignStatus);

                return Ok(taxAssignList);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        #endregion ======= Tax Assign =======

        #region ====== Excel sheet Generate ==========

        [HttpGet]
        [Route("GenerateExcelSheet")]
        public async Task<IActionResult> GenerateExcelSheet()
        {
            try
            {
                //List<EmpEmployeeBasicInfo> empInfo = await _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true).ToListAsync();

                //DataTable dataTable = new DataTable(typeof(EmpEmployeeBasicInfo).Name);
                ////Get all the properties
                //PropertyInfo[] Props = typeof(EmpEmployeeBasicInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                //foreach (PropertyInfo prop in Props)
                //{
                //    //Setting column names as Property names
                //    dataTable.Columns.Add(prop.Name);
                //}
                //foreach (EmpEmployeeBasicInfo item in empInfo)
                //{
                //    var values = new object[Props.Length];
                //    for (int i = 0; i < Props.Length; i++)
                //    {
                //        //inserting property values to datatable rows
                //        values[i] = Props[i].GetValue(item, null);
                //    }
                //    dataTable.Rows.Add(values);
                //}

                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Clear();
                dt.TableName = "Employee Info";
                dt.Columns.Add("Name");
                dt.Columns.Add("Address");
                dt.Columns.Add("Designation");
                dt.Columns.Add("Department");

                DataColumn colDate = new DataColumn("ReportDate");
                colDate.DataType = System.Type.GetType("System.DateTime");
                dt.Columns.Add(colDate);

                DataColumn colReportDate = new DataColumn("ReportDateTime");
                colReportDate.DataType = System.Type.GetType("System.DateTime");
                dt.Columns.Add(colReportDate);

                DataColumn colDecimal = new DataColumn("Salary");
                colDecimal.DataType = System.Type.GetType("System.Decimal");
                dt.Columns.Add(colDecimal);

                DataColumn colTransport = new DataColumn("Transport Bill");
                colTransport.DataType = System.Type.GetType("System.Int32");
                dt.Columns.Add(colTransport);

                DataColumn colHouseRent = new DataColumn("House Rent");
                colHouseRent.DataType = System.Type.GetType("System.Decimal");
                dt.Columns.Add(colHouseRent);

                DataColumn colMedical = new DataColumn("Medical awareness");
                colMedical.DataType = System.Type.GetType("System.Double");
                dt.Columns.Add(colMedical);

                DataColumn collateP = new DataColumn("Late Present");
                collateP.DataType = System.Type.GetType("System.Decimal");
                dt.Columns.Add(collateP);

                DataColumn colNetSalary = new DataColumn("Net Salary");
                colNetSalary.DataType = System.Type.GetType("System.Decimal");
                dt.Columns.Add(colNetSalary);

                DataColumn colTotalValue = new DataColumn("TotalValue");
                colTotalValue.DataType = System.Type.GetType("System.Double");
                dt.Columns.Add(colTotalValue);

                DataRow Emp1 = dt.NewRow();
                Emp1["Name"] = "Devid Loren";
                Emp1["Address"] = "House-4, Road-4, Block-B, New York";
                Emp1["Designation"] = "Software Engineer";
                Emp1["Department"] = "Development";
                Emp1["ReportDate"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                Emp1["ReportDateTime"] = DateTime.Now;
                Emp1["Salary"] = 30000454544440000;
                Emp1["Transport Bill"] = 5000;
                Emp1["House Rent"] = 9223372036854775801;
                Emp1["Medical awareness"] = 10000;
                Emp1["Late Present"] = 1000;
                Emp1["Net Salary"] = 62000;
                Emp1["TotalValue"] = 23948579999900000;
                dt.Rows.Add(Emp1);

                for (int i = 1; i <= 10; i++)
                {
                    DataRow Emp2 = dt.NewRow();

                    Emp2["Name"] = "Jhone Sharp";
                    Emp2["Address"] = "House-5/A, Road-03, Block-B, New York";
                    Emp2["Designation"] = "Software Engineer";
                    Emp2["Department"] = "Development";
                    Emp2["ReportDate"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                    Emp2["ReportDateTime"] = DateTime.Now;
                    Emp2["Salary"] = 30000454544440000;
                    Emp2["Transport Bill"] = 8000;
                    Emp2["House Rent"] = 9223372036854775801;
                    Emp2["Medical awareness"] = 15000;
                    Emp2["Late Present"] = 1000;
                    Emp2["Net Salary"] = 80000;
                    Emp2["TotalValue"] = 23948579999900000;

                    dt.Rows.Add(Emp2);
                }

                XLWorkbook wb = new XLWorkbook();
                int cr = 5;
                int cc = 1;

                IXLWorksheet excelsh = wb.Worksheets.Add(dt);
                //var n = excelsh.SetShowRowColHeaders();

                int columnLength = dt.Columns.Count;
                int rowLength = dt.Rows.Count;

                for (int i = 0; i < columnLength; i++)
                {
                    string colType = dt.Columns[i].DataType.Name.ToString();
                    string colName = dt.Columns[i].ColumnName.ToString();
                    //string colType = dt.Columns[0].DataType.Name.ToString();

                    int colN = i + 1;

                    if (colType == "Decimal" || colType == "Int32" || colType == "Int64" || colType == "Double")
                    {
                        excelsh.Column(colN).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        if (colType == "Double" || colType == "Decimal" || colType == "Int64")
                        {
                            string cellName1 = excelsh.Column(colN).ColumnLetter() + 2.ToString();
                            string cellName2 = excelsh.Column(colN).ColumnLetter() + (1 + rowLength).ToString();
                            excelsh.Range(cellName1 + ":" + cellName2).Style.NumberFormat.NumberFormatId = 4;
                        }
                    }
                    else
                    {
                        excelsh.Column(colN).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    //Stringt,DateTimey,Decimal,Int32,Int64,Double
                }

                IXLRow rowOverTable = excelsh.Row(1);
                rowOverTable.InsertRowsAbove(cr);

                IXLColumn firstColumn = excelsh.Column(1);
                firstColumn.InsertColumnsBefore(cc);

                //excelsh.Column(1).AdjustToContents();
                excelsh.Columns().AdjustToContents();

                // ====================== COLUMN TEXT ALIGNMENT =========================
                //var collenght = dt.Columns.Count;
                //for (int i = 1; i <= collenght; i++)
                //{
                //    int c = 1 + i;
                //    excelsh.Column(c).AdjustToContents();
                //    excelsh.Column(c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //}
                // ====================== END =========================

                // ================= ROW TEXT ALIGNMENT ================
                //long rowCount = dt.Rows.Count;
                //for (int i = 1; i <= rowCount; i++)
                //{
                //    int c = 6 + i;
                //    excelsh.Row(c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //    //excelsh.Row(c).Style.Font.Bold = true;
                //}
                // ====================== END =========================

                string ReportTitle = "iBOS Limited";
                string subTitle = "Employee Salary Report";
                int titlePosition = (int)(columnLength + 1) / 2;

                // ReportTitle
                excelsh.Cell(2, titlePosition).Value = ReportTitle;
                string cR1 = excelsh.Cell(2, titlePosition).WorksheetColumn().ColumnLetter() + 2.ToString();
                string cR2 = excelsh.Cell(2, titlePosition + 2).WorksheetColumn().ColumnLetter() + 2.ToString();
                excelsh.Range(cR1 + ":" + cR2).Merge();
                excelsh.Range(cR1).Style.Font.FontSize = 20;
                excelsh.Range(cR1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Subtitle
                excelsh.Cell(3, titlePosition).Value = subTitle;
                string st1 = excelsh.Cell(3, titlePosition).WorksheetColumn().ColumnLetter() + 3.ToString();
                string st2 = excelsh.Cell(3, titlePosition + 2).WorksheetColumn().ColumnLetter() + 3.ToString();
                excelsh.Range(st1 + ":" + st2).Merge();
                excelsh.Range(st1).Style.Font.FontSize = 16;
                excelsh.Range(st1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Border Style
                excelsh.Range(cR1 + ":" + cR2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelsh.Range(cR1 + ":" + cR2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                excelsh.Range(cR1 + ":" + cR2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelsh.Range(cR1 + ":" + cR2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                //excelsh.Range(st1 + ":" + st2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelsh.Range(st1 + ":" + st2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                excelsh.Range(st1 + ":" + st2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelsh.Range(st1 + ":" + st2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                //Company Logo style
                string imagefolder = Path.Combine(env.WebRootPath, "uttara_group_logo.png");  //var imagePath = @"c:\path\to\your\image.jpg";
                var image = excelsh.AddPicture(imagefolder)
                    .MoveTo(excelsh.Cell("B2"))
                    .WithSize(200, 65); // optional: resize picture
                                        //.Scale(.5);

                //Column Header style
                int columnC = columnLength + cc; // cc for mirgen column
                string headRow1 = excelsh.Cell(cr + 1, cc + 1).WorksheetColumn().ColumnLetter() + (cr + 1).ToString();
                string headRow2 = excelsh.Cell(cr + 1, columnC).WorksheetColumn().ColumnLetter() + (cr + 1).ToString();
                excelsh.Range(headRow1 + ":" + headRow2).Style.Fill.BackgroundColor = XLColor.BlueGray;
                //excelsh.Range(headRow1 + ":" + headRow2).Style.Fill.BackgroundColor = XLColor.FromArgb(0x7F3F7F);

                excelsh.Range(headRow1 + ":" + headRow2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excelsh.Range(headRow1 + ":" + headRow2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelsh.Range(headRow1 + ":" + headRow2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelsh.Range(headRow1 + ":" + headRow2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelsh.Range(headRow1 + ":" + headRow2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                //Select All Cell and Style
                int rowC = rowLength + cr + 1; // 1 for header row
                string dataRow1 = excelsh.Cell(cr + 2, cc + 1).WorksheetColumn().ColumnLetter() + (cr + 2).ToString();
                string dataRow2 = excelsh.Cell(rowC, columnC).WorksheetColumn().ColumnLetter() + rowC.ToString();
                excelsh.Range(dataRow1 + ":" + dataRow2).Style.Fill.BackgroundColor = XLColor.White;

                excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                // ======================== RETUN EXCEL FILE =======================

                MemoryStream stream = new MemoryStream();
                wb.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                // ======================== END =======================
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion ====== Excel sheet Generate ==========

        #region ================ Transfer & promotionVM ===================

        [HttpPost]
        [Route("SaveEmpTransferNpromotion")]
        public async Task<IActionResult> SaveEmpTransferNpromotion(TransferNpromotionVM model)
        {
            try
            {
                PermissionLebelCheck permissionLebel = model.IntTerritoryId > 0 ? PermissionLebelCheck.Territory : model.IntAreaId > 0 ? PermissionLebelCheck.Area : model.IntRegionId > 0 ? PermissionLebelCheck.Region : model.IntSoldDepoId > 0 ? PermissionLebelCheck.SoleDepo : model.IntWingId > 0 ? PermissionLebelCheck.Wing : model.IntWorkplaceGroupId > 0 ? PermissionLebelCheck.WorkplaceGroup : PermissionLebelCheck.BusinessUnit;

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.IntBusinessUnitId, workplaceGroupId = model.IntWorkplaceGroupId, wingId = (long)model.IntWingId, soleDepoId = (long)model.IntSoldDepoId, regionId = (long)model.IntRegionId, areaId = (long)model.IntAreaId, territoryId = (long)model.IntTerritoryId }, permissionLebel);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                MessageHelperCreate res = await _employeeService.CRUDEmpTransferNpromotion(model);

                if (res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());

            }
        }

        [HttpGet]
        [Route("GetAllEmpTransferNpromotion")]
        public async Task<IActionResult> GetAllEmpTransferNpromotion(long businessUnitId, long workplaceGroupId, string landingType, DateTime dteFromDate, DateTime dteToDate, string? SearchTxt, int PageNo, int PageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _employeeService.GetAllEmpTransferNpromotion(tokenData, businessUnitId, workplaceGroupId, landingType, dteFromDate, dteToDate, SearchTxt, PageNo, PageSize));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetEmpTransferNpromotionHistoryByEmployeeId")]
        public async Task<IActionResult> GetEmpTransferNpromotionHistoryByEmployeeId(long businessUnitId, long workplaceGroupId, long employeeId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, employeeId, null);

                if (isValid.Count() > 0)
                {
                    return Ok(await _employeeService.GetEmpTransferNpromotionHistoryByEmployeeId(tokenData.accountId, employeeId));
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());

                }

            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetEmpTransferNpromotionById")]
        public async Task<IActionResult> GetEmpTransferNpromotionById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmpTransferNpromotionById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("DeleteEmpTransferNpromotion")]
        public async Task<IActionResult> DeleteEmpTransferNpromotion(long id, long actionBy)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _employeeService.DeleteEmpTransferNpromotion(id, actionBy);

                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("ReleaseEmpTransferNpromotion")]
        public async Task<IActionResult> ReleaseEmpTransferNpromotion(long accountId, long employeeId, long substitutionEmployeeId, long transferNPromotionId, DateTime ReleaseDate, long actionBy)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Release Successfully !!!"
                };

                EmpTransferNpromotion transferNpromotion = await _context.EmpTransferNpromotions.FirstOrDefaultAsync(x => x.IntAccountId == accountId
                                        && x.IntEmployeeId == employeeId && x.IntTransferNpromotionId == transferNPromotionId && x.IsActive == true);

                if (transferNpromotion != null)
                {
                    transferNpromotion.DteReleaseDate = ReleaseDate;
                    transferNpromotion.IntSubstitutionEmployeeId = substitutionEmployeeId;
                    transferNpromotion.IntUpdatedBy = actionBy;
                    transferNpromotion.DteUpdatedAt = DateTime.Now;

                    _context.EmpTransferNpromotions.Update(transferNpromotion);
                    await _context.SaveChangesAsync();

                    return Ok(res);
                }
                else
                {
                    res.StatusCode = 401;
                    res.Message = "Data not found";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpPut]
        [Route("JoiningAcknowledgeEmpTransferNpromotion")]
        public async Task<IActionResult> JoiningAcknowledgeEmpTransferNpromotion(long accountId, long employeeId, long transferNPromotionId, bool isJoined, long actionBy)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Joined Successfully !!!"
                };

                EmpTransferNpromotion transferNpromotion = await _context.EmpTransferNpromotions.FirstOrDefaultAsync(x => x.IntAccountId == accountId
                                        && x.IntEmployeeId == employeeId && x.IntTransferNpromotionId == transferNPromotionId && x.IsActive == true);

                if (transferNpromotion != null && transferNpromotion.DteReleaseDate.Value.Date <= DateTime.Now)
                {
                    transferNpromotion.IsJoined = isJoined;
                    transferNpromotion.IntUpdatedBy = actionBy;
                    transferNpromotion.DteUpdatedAt = DateTime.Now;

                    _context.EmpTransferNpromotions.Update(transferNpromotion);
                    await _context.SaveChangesAsync();

                    EmpEmployeeBasicInfo empEmployeeBasic = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == transferNpromotion.IntEmployeeId);
                    EmpEmployeeBasicInfoDetail empEmployeeBasicInfoDetail = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == transferNpromotion.IntEmployeeId);

                    if (empEmployeeBasic != null)
                    {
                        empEmployeeBasic.IntBusinessUnitId = transferNpromotion.IntBusinessUnitId;
                        empEmployeeBasic.IntWorkplaceGroupId = transferNpromotion.IntWorkplaceGroupId;
                        empEmployeeBasic.IntWorkplaceId = transferNpromotion.IntWorkplaceId;
                        empEmployeeBasic.IntSupervisorId = transferNpromotion.IntSupervisorId;
                        empEmployeeBasic.IntLineManagerId = transferNpromotion.IntLineManagerId;
                        empEmployeeBasic.IntDottedSupervisorId = transferNpromotion.IntDottedSupervisorId;
                        empEmployeeBasic.IntDepartmentId = transferNpromotion.IntDepartmentId;
                        empEmployeeBasic.IntDesignationId = transferNpromotion.IntDesignationId;

                        empEmployeeBasicInfoDetail.IntWingId = transferNpromotion.IntWingId;
                        empEmployeeBasicInfoDetail.IntSoleDepo = transferNpromotion.IntSoldDepoId;
                        empEmployeeBasicInfoDetail.IntRegionId = transferNpromotion.IntRegionId;
                        empEmployeeBasicInfoDetail.IntAreaId = transferNpromotion.IntAreaId;
                        empEmployeeBasicInfoDetail.IntTerritoryId = transferNpromotion.IntTerritoryId;

                        _context.EmpEmployeeBasicInfos.Update(empEmployeeBasic);
                        _context.EmpEmployeeBasicInfoDetails.Update(empEmployeeBasicInfoDetail);
                        await _context.SaveChangesAsync();

                        List<RoleBridgeWithDesignation> deleteExistingRoleList = await _context.RoleBridgeWithDesignations
                            .Where(x => x.StrIsFor.ToLower() == "Employee".ToLower() && x.IntAccountId == transferNpromotion.IntAccountId && x.IntDesignationOrEmployeeId == transferNpromotion.IntEmployeeId && x.IsActive == true).ToListAsync();

                        List<RoleBridgeWithDesignation> roleExistWithNewDesignationList = await _context.RoleBridgeWithDesignations
                            .Where(x => (x.StrIsFor.ToLower() == "Designation".ToLower() && x.IntAccountId == transferNpromotion.IntAccountId && x.IntDesignationOrEmployeeId == transferNpromotion.IntDesignationId && x.IsActive == true)).ToListAsync();

                        List<EmpTransferNpromotionUserRole> transferNpromotionUserRoleList = await _context.EmpTransferNpromotionUserRoles.Where(x => x.IntTransferNpromotionId == transferNpromotion.IntTransferNpromotionId && x.IsActive == true).ToListAsync();
                        List<RoleBridgeWithDesignation> newRoleAssignToUser = new List<RoleBridgeWithDesignation>();

                        foreach (EmpTransferNpromotionUserRole item in transferNpromotionUserRoleList)
                        {
                            bool isDesignationExists = false;

                            if (roleExistWithNewDesignationList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId))
                            {
                                isDesignationExists = true;
                            }

                            if (deleteExistingRoleList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId) && isDesignationExists == false)
                            {
                                deleteExistingRoleList = deleteExistingRoleList.Where(x => x.IntRoleId != item.IntUserRoleId).ToList();
                            }
                            else if (!deleteExistingRoleList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId) && isDesignationExists == false)
                            {
                                newRoleAssignToUser.Add(new RoleBridgeWithDesignation
                                {
                                    IntAccountId = transferNpromotion.IntAccountId,
                                    StrIsFor = "Employee",
                                    IntDesignationOrEmployeeId = transferNpromotion.IntEmployeeId,
                                    IntRoleId = item.IntUserRoleId,
                                    IntCreatedBy = (long)transferNpromotion.IntCreatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });
                            }
                        }

                        List<RoleExtensionRow> roleExtensionList = await _context.RoleExtensionRows
                            .Where(x => x.IntEmployeeId == transferNpromotion.IntEmployeeId && x.IsActive == true).ToListAsync();

                        List<EmpTransferNpromotionRoleExtension> transferNpromotionRoleExtensionList = await _context.EmpTransferNpromotionRoleExtensions.Where(x => x.IntTransferNpromotionId == transferNpromotion.IntTransferNpromotionId && x.IsActive == true).ToListAsync();
                        List<RoleExtensionRow> newRoleExtensionList = new List<RoleExtensionRow>();

                        RoleExtensionHeader header = await _context.RoleExtensionHeaders.FirstOrDefaultAsync(x => x.IntEmployeeId == transferNpromotion.IntEmployeeId && x.IsActive == true);

                        if (header == null)
                        {
                            header = new RoleExtensionHeader();
                            header.IntEmployeeId = transferNpromotion.IntEmployeeId;
                            header.IntCreatedBy = (long)transferNpromotion.IntCreatedBy;
                            header.DteCreatedDateTime = DateTime.Now;
                            header.IsActive = true;

                            await _context.RoleExtensionHeaders.AddAsync(header);
                            await _context.SaveChangesAsync();
                        }

                        foreach (EmpTransferNpromotionRoleExtension item in transferNpromotionRoleExtensionList)
                        {
                            if (roleExtensionList.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntOrganizationTypeId == item.IntOrganizationTypeId && x.IntOrganizationReffId == item.IntOrganizationReffId).Count() <= 0)
                            {
                                newRoleExtensionList.Add(new RoleExtensionRow
                                {
                                    IntRoleExtensionHeaderId = header.IntRoleExtensionHeaderId,
                                    IntEmployeeId = transferNpromotion.IntEmployeeId,
                                    IntOrganizationTypeId = item.IntOrganizationTypeId,
                                    StrOrganizationTypeName = item.StrOrganizationTypeName,
                                    IntOrganizationReffId = item.IntOrganizationReffId,
                                    StrOrganizationReffName = item.StrOrganizationReffName,
                                    IntCreatedBy = (long)transferNpromotion.IntCreatedBy,
                                    DteCreatedDateTime = DateTime.Now,
                                    IsActive = true
                                });
                            }
                            else
                            {
                                roleExtensionList = roleExtensionList.Where(x => x.IntRoleExtensionRowId != roleExtensionList.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntOrganizationTypeId == item.IntOrganizationTypeId && x.IntOrganizationReffId == item.IntOrganizationReffId).FirstOrDefault().IntRoleExtensionRowId).ToList();
                            }
                        }

                        if (deleteExistingRoleList.Count() > 0)
                        {
                            deleteExistingRoleList.ForEach(d =>
                            {
                                d.IsActive = false;
                                d.IntUpdatedBy = transferNpromotion.IntEmployeeId;
                                d.DteUpdateDateTime = DateTime.Now;
                            });
                            _context.RoleBridgeWithDesignations.UpdateRange(deleteExistingRoleList);
                            await _context.SaveChangesAsync();
                        }
                        if (newRoleAssignToUser.Count() > 0)
                        {
                            await _context.RoleBridgeWithDesignations.AddRangeAsync(newRoleAssignToUser);
                            await _context.SaveChangesAsync();
                        }

                        if (roleExtensionList.Count() > 0)
                        {
                            roleExtensionList.ForEach(d =>
                            {
                                d.IsActive = false;
                            });
                            _context.RoleExtensionRows.UpdateRange(roleExtensionList);
                            await _context.SaveChangesAsync();
                        }
                        if (newRoleExtensionList.Count() > 0)
                        {
                            await _context.RoleExtensionRows.AddRangeAsync(newRoleExtensionList);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return Ok(res);
                }
                else
                {
                    res.StatusCode = 401;
                    res.Message = "Not Eligble Before Relased Date";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        #endregion ================ Transfer & promotionVM ===================

        #region Increment

        [HttpPost]
        [Route("IsPromotionEligibleThroughIncrement")]
        public async Task<IActionResult> IsPromotionEligibleThroughIncrement(CRUDIncrementPromotionTransferVM objCrud)
        {
            bool res = await _employeeService.IsPromotionEligibleThroughIncrement(objCrud);
            return Ok(res);
        }

        [HttpPost]
        [Route("CreateEmployeeIncrement")]
        public async Task<IActionResult> CreateEmployeeIncrement(CRUDIncrementPromotionTransferVM objCrud)
        {
            try
            {
                MessageHelperCreate res = await _employeeService.CreateEmployeeIncrement(objCrud);
                if (res.StatusCode == 200 || res.StatusCode == 201)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception e)
            {
                MessageHelperCreate res = new MessageHelperCreate();
                res.StatusCode = 500;
                res.Message = e.Message;
                return StatusCode(500, res);
            }
        }

        [HttpGet]
        [Route("GetEmployeeIncrementLanding")]
        public async Task<IActionResult> GetEmployeeIncrementLanding(long businessUnitId, long? workplaceGroupId, DateTime? dteFromDate, DateTime? dteToDate, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)businessUnitId, workplaceGroupId = (long)workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.isAuthorize)
                {
                    GetIncrementPaginationVM res = await _employeeService.GetEmployeeIncrementLanding(tokenData.accountId, businessUnitId, workplaceGroupId, dteFromDate, dteToDate, PageNo, PageSize, searchTxt);
                    return Ok(res);
                }
                else
                    return NotFound(new MessageHelperAccessDenied());
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetEmployeeIncrementById")]
        public async Task<IActionResult> GetEmployeeIncrementById(long businessUnitId, long workplaceGroupId, long employeeId, long autoId)
        {
            BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

            if (tokenData.accountId == -1)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }

            var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, employeeId, null);

            if (isValid.Count() > 0)
            {
                var res = await _employeeService.GetEmployeeIncrementById(autoId);
                return Ok(res);
            }
            else
            {
                return BadRequest(new MessageHelperAccessDenied());
            }
        }

        [HttpGet]
        [Route("GetEmployeeIncrementByEmoloyeeId")]
        public async Task<IActionResult> GetEmployeeIncrementByEmoloyeeId(long businessUnitId, long workplaceGroupId, long EmployeeId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, EmployeeId, null);

                if (isValid.Count() > 0)
                {

                    List<CRUDEmployeeIncrementVM> IncrementList = await _employeeService.GetEmployeeIncrementByEmoloyeeId(tokenData.accountId, EmployeeId);
                    return Ok(IncrementList);
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        #endregion Increment

        #region PF Gratuity

        [HttpPost]
        [Route("CRUDEmpPfngratuity")]
        public async Task<IActionResult> CRUDEmpPfngratuity(CRUDEmpPfngratuityVM obj)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                res = await _employeeService.CRUDEmpPfngratuity(obj);
                if (res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetEmpPfngratuity")]
        public async Task<IActionResult> GetEmpPfngratuity(long AccountId)
        {
            return Ok(await _employeeService.GetEmpPfngratuity(AccountId));
        }

        [HttpGet]
        [Route("GetPFInvestmentLanding")]
        public async Task<IActionResult> GetPFInvestmentLanding(long accountId, long businessUnitId, string? searchTxt, int? pageNo, int? pageSize, DateTime dteFromDate, DateTime dteToDate)
        {
            try
            {
                return Ok(await _employeeService.GetPFInvestmentLanding(accountId, businessUnitId, searchTxt, pageNo, pageSize, dteFromDate, dteToDate));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPFInvestmentById")]
        public async Task<IActionResult> GetPFInvestmentById(long HeaderId)
        {
            return Ok(await _employeeService.GetPFInvestmentById(HeaderId));
        }

        [HttpGet]
        [Route("GetValidPFInvestmentPeriod")]
        public async Task<IActionResult> GetValidPFInvestmentPeriod(long AccountId, long BusinessUnitId)
        {
            return Ok(await _employeeService.GetValidPFInvestmentPeriod(AccountId, BusinessUnitId));
        }

        [HttpGet]
        [Route("GetEmployeeDataForPFInvestment")]
        public async Task<IActionResult> GetEmployeeDataForPFInvestment(long accountId, long businessUnitId, DateTime fromMonthYear, DateTime toMonthYear)
        {
            List<PFInvestmentRowVM> pFInvestments = await _employeeService.GetEmployeeDataForPFInvestment(accountId, businessUnitId, fromMonthYear, toMonthYear);
            return Ok(pFInvestments);
        }

        [HttpPost]
        [Route("CreatePFInvestment")]
        public async Task<IActionResult> CreatePFInvestment(CRUDPFInvestmentVM obj)
        {
            return Ok(await _employeeService.CreatePFInvestment(obj));
        }

        [HttpGet]
        [Route("PfNGratuityLanding")]
        public async Task<IActionResult> PfNGratuityLanding(long IntAccountId, long IntEmployeeId)
        {
            try
            {
                PFNGratuityViewModel pFNGratuityView = await _employeeService.PfNGratuityLanding(IntAccountId, IntEmployeeId);
                return Ok(pFNGratuityView);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("CRUDPfWithdraw")]
        public async Task<IActionResult> CRUDPfWithdraw(PFWithdrawViewModel obj)
        {
            MessageHelper msg = new MessageHelper();
            try
            {
                msg = await _employeeService.CRUDPFWithdraw(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception e)
            {
                msg.StatusCode = 500;
                msg.Message = e.Message;
                return BadRequest(msg);
            }
        }

        #endregion PF Gratuity

        #region test api

        [HttpGet]
        [Route("PipelineRowDetailsByApplicationTypeForIsSupNLMORUG")]
        public async Task<IActionResult> PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(long IntAccountId, string ApplicationType, long IntEmployeeId)
        {
            try
            {
                EmpIsSupNLMORUGMemberViewModel EmpIsSupNLMORUGMember = await _approvalPipelineService.PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(IntAccountId, ApplicationType, IntEmployeeId);
                return Ok(EmpIsSupNLMORUGMember);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion test api

        #region ============== Expense Application ====================

        [HttpPost]
        [Route("ExpenseApplicationCreateEdit")]
        public async Task<IActionResult> ExpenseApplicationCreateEdit(ExpenseApplicationViewModel expenseApplicationViewModel)
        {
            System.Data.DataTable dt1 = new System.Data.DataTable();
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.employeeId != expenseApplicationViewModel.intEmployeeId)
                {
                    var data = await _employeeService.GetCommonEmployeeDDL(tokenData, (long)expenseApplicationViewModel.intBusinessUnitId, (long)expenseApplicationViewModel.intWorkplaceGroupId, expenseApplicationViewModel.intEmployeeId, null);

                    if (data.Count() <= 0)
                    {
                        return Ok(new MessageHelperAccessDenied());
                    }
                }

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    var pipeInfo = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType((long)expenseApplicationViewModel.intEmployeeId, "expense");

                    string sql = "saas.sprEmExpenseApplicationCreateEdit";

                    string jsonString = System.Text.Json.JsonSerializer.Serialize(expenseApplicationViewModel.UrlIdViewModelList);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intExpenseId", expenseApplicationViewModel.intExpenseId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", expenseApplicationViewModel.intAccontId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", expenseApplicationViewModel.intEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@strEntryType", expenseApplicationViewModel.strEntryType);
                        sqlCmd.Parameters.AddWithValue("@intExpenseTypeId", expenseApplicationViewModel.intExpenseTypeId);
                        sqlCmd.Parameters.AddWithValue("@dteExpenseFromDate", expenseApplicationViewModel.dteExpenseFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteExpenseToDate", expenseApplicationViewModel.dteExpenseToDate);
                        sqlCmd.Parameters.AddWithValue("@numExpenseAmount", expenseApplicationViewModel.numExpenseAmount);
                        sqlCmd.Parameters.AddWithValue("@strDiscription", expenseApplicationViewModel.strDiscription);
                        sqlCmd.Parameters.AddWithValue("@isActive ", expenseApplicationViewModel.isActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", expenseApplicationViewModel.intCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@dteCreatedAt", expenseApplicationViewModel.dteCreatedAt);
                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipeInfo.HeaderId);
                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipeInfo.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", pipeInfo.NextStageId);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt1);
                        }
                        connection.Close();
                    }
                }

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt1.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt1.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("ExpenseApplicationLanding")]
        public async Task<IActionResult> ExpenseApplicationLanding(string strPartName, long? intBusinessUnitId, long? intWorkplaceGroupId, long? intEmployeeId, DateTime? dteApplicationDate,
           DateTime? dteFromDate, DateTime? dteToDate, string? strStatus, string? strSearchTxt, long? intExpenseId, string? strDocFor, int? pageNo, int? pageSize)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)intBusinessUnitId, workplaceGroupId = (long)intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprExpenseAllSelectQuery";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPartName", strPartName);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@dteApplicationDate", dteApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", dteFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", dteToDate);
                        sqlCmd.Parameters.AddWithValue("@strStatus", strStatus);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", strSearchTxt);
                        sqlCmd.Parameters.AddWithValue("@intExpenseId ", intExpenseId);
                        sqlCmd.Parameters.AddWithValue("@strDocFor", strDocFor);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", pageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", pageSize);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                string dataTbl = JsonConvert.SerializeObject(dt);
                return Ok(dataTbl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("ExpenseApplicationLandingDataPaginetion")]
        public async Task<IActionResult> ExpenseApplicationLandingDataPaginetion(long? intBusinessUnitId, long? intWorkplaceGroupId, DateTime? dteApplicationDate, long? intEmployeeId,
           DateTime? dteFromDate, DateTime? dteToDate, string? strSearchTxt, int? pageNo, int? pageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)intBusinessUnitId, workplaceGroupId = (long)intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                strSearchTxt = string.IsNullOrEmpty(strSearchTxt) ? strSearchTxt : strSearchTxt.ToLower();

                IQueryable<ExpenseApplicationLandingVM> data = (from ex in _context.EmpExpenseApplications
                                                                join emp in _context.EmpEmployeeBasicInfos on ex.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                                                from empD in empD2.DefaultIfEmpty()
                                                                join dsg in _context.MasterDesignations on emp.IntDesignationId equals dsg.IntDesignationId into dsg2
                                                                from desi in dsg2.DefaultIfEmpty()
                                                                join dep in _context.MasterDepartments on emp.IntDepartmentId equals dep.IntDepartmentId into dep2
                                                                from dept in dep2.DefaultIfEmpty()
                                                                join ext in _context.EmpExpenseTypes on ex.IntExpenseTypeId equals ext.IntExpenseTypeId into exty
                                                                from extyp in exty.DefaultIfEmpty()

                                                                where ex.IsActive == true && emp.IntAccountId == tokenData.accountId
                                                                 && (intEmployeeId > 0 ? intEmployeeId == tokenData.employeeId : true)
                                                                 && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(intBusinessUnitId)) && emp.IntBusinessUnitId == intBusinessUnitId
                                                                 && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(intWorkplaceGroupId)) && emp.IntWorkplaceGroupId == intWorkplaceGroupId
                                                          
                                                                 && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                 : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                                 : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                                                                 : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                                                                 : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                                                                 : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                                                                 : tokenData.territoryList.Contains(empD.IntTerritoryId))
                                                                 || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                                                                && (dteApplicationDate == null || ex.DteExpenseFromDate.Date == dteApplicationDate.Value.Date)
                                                                && (dteFromDate == null || dteToDate == null || ex.DteExpenseFromDate.Date >= dteFromDate.Value.Date && ex.DteExpenseFromDate <= dteToDate.Value.Date)
                                                                && (!string.IsNullOrEmpty(strSearchTxt) ? (emp.StrEmployeeName.ToLower().Contains(strSearchTxt) || emp.StrEmployeeCode.ToLower().Contains(strSearchTxt)
                                                                || extyp.StrExpenseType.ToLower().Contains(strSearchTxt)) : true)

                                                                select new ExpenseApplicationLandingVM
                                                                {
                                                                    ExpenseId = ex.IntExpenseId,
                                                                    intEmployeeId = ex.IntEmployeeId,
                                                                    EmployeeName = emp.StrEmployeeName,
                                                                    employeeCode = emp.StrEmployeeCode,
                                                                    strDesignation = desi.StrDesignation,
                                                                    strDepartment = dept.StrDepartment,
                                                                    intExpenseTypeId = ex.IntExpenseTypeId,
                                                                    strExpenseType = extyp.StrExpenseType,
                                                                    dteExpenseFromDate = ex.DteExpenseFromDate,
                                                                    dteExpenseToDate = ex.DteExpenseToDate,
                                                                    strDiscription = ex.StrDiscription,
                                                                    numExpenseAmount = ex.NumExpenseAmount,
                                                                    isActive = ex.IsActive,
                                                                    intCreatedBy = (long)ex.IntCreatedBy,
                                                                    dteCreatedAt = ex.DteCreatedAt,
                                                                    Status = (ex.IsReject == false && ex.IsPipelineClosed == true && ex.IsActive == true) ? "Approved"
                                                                    : (ex.IsReject == false && ex.IsPipelineClosed == false && ex.IsActive == true) ? "Pending"
                                                                    : ex.IsReject == true ? "Rejected" : ""

                                                                }).OrderByDescending(x => x.ExpenseId).AsNoTracking().AsQueryable();

                ExpenseApplicationLandingPaginetionVM expenseApplication = new();




                int maxSize = 1000;
                pageSize = pageSize > maxSize ? maxSize : pageSize;
                pageNo = pageNo < 1 ? 1 : pageNo;

                expenseApplication.TotalCount = await data.CountAsync();
                expenseApplication.expenseApplicationLandings = await data.Skip((int)pageSize * ((int)pageNo - 1)).Take((int)pageSize).ToListAsync();
                expenseApplication.PageSize = (int)pageSize;
                expenseApplication.CurrentPage = (int)pageNo;


                return Ok(expenseApplication);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetExpenseDocList")]
        public async Task<IActionResult> GetExpenseDocList(long intExpenseId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                var expenseDocList = (from D in _context.EmpExpenseDocuments
                                      join F in _context.GlobalFileUrls on D.IntDocUrlid equals F.IntDocumentId
                                      where F.IsActive == true && D.IsActive == true && D.IntExpenseId == intExpenseId && F.IntAccountId == tokenData.accountId && D.StrDocFor == "Expense"
                                      orderby D.IntDocUrlid descending
                                      select new
                                      {
                                          D.IntExpenseDocId,
                                          D.IntDocUrlid,
                                          D.StrDocFor
                                      }).ToList();

                return Ok(expenseDocList);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetExpenseById")]
        public async Task<IActionResult> GetExpenseById(long intExpenseId, long? businessUnitId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)businessUnitId }, PermissionLebelCheck.BusinessUnit);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                ExpenseApplicationLandingVM expesne = (from ex in _context.EmpExpenseApplications
                                                       join emp in _context.EmpEmployeeBasicInfos on ex.IntEmployeeId equals emp.IntEmployeeBasicInfoId into empJoin
                                                       from emp in empJoin.DefaultIfEmpty()
                                                       join dsg in _context.MasterDesignations on emp.IntDesignationId equals dsg.IntDesignationId into dsgJoin
                                                       from dsg in dsgJoin.DefaultIfEmpty()
                                                       join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into deptJoin
                                                       from dept in deptJoin.DefaultIfEmpty()
                                                       join emExp in _context.EmpExpenseTypes on ex.IntExpenseTypeId equals emExp.IntExpenseTypeId into emExpJoin
                                                       from emExp in emExpJoin.DefaultIfEmpty()
                                                       where ex.IsActive == true && emp.IntAccountId == tokenData.accountId
                                                          && ex.IntExpenseId == intExpenseId
                                                          && (tokenData.businessUnitId == null || tokenData.businessUnitId == 0 || emp.IntBusinessUnitId == tokenData.businessUnitId)
                                                       orderby ex.IntExpenseId descending
                                                       select new ExpenseApplicationLandingVM
                                                       {
                                                           ExpenseId = ex.IntExpenseId,
                                                           intEmployeeId = ex.IntEmployeeId,
                                                           EmployeeName = emp.StrEmployeeName,
                                                           employeeCode = emp.StrEmployeeCode,
                                                           strDesignation = dsg.StrDesignation,
                                                           strDepartment = dept.StrDepartment,
                                                           intExpenseTypeId = ex.IntExpenseTypeId,
                                                           strExpenseType = emExp.StrExpenseType,
                                                           dteExpenseFromDate = ex.DteExpenseFromDate,
                                                           dteExpenseToDate = ex.DteExpenseToDate,
                                                           strDiscription = ex.StrDiscription,
                                                           numExpenseAmount = ex.NumExpenseAmount,
                                                           isActive = ex.IsActive,
                                                           intCreatedBy = (long)ex.IntCreatedBy,
                                                           dteCreatedAt = ex.DteCreatedAt,
                                                           Status = ex.IsReject == false && ex.IsPipelineClosed == true && ex.IsActive == true ? "Approved" :
                                                                    ex.IsReject == false && ex.IsPipelineClosed == false && ex.IsActive == true ? "Pending" :
                                                                    ex.IsReject == true ? "Rejected" : null
                                                       }).FirstOrDefault();

                return Ok(expesne);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        #endregion ============== Expense Application ====================

        #region ============== SalaryCertificate Request ====================

        [HttpPost]
        [Route("SalaryCertificateRequestCreateEdit")]
        public async Task<IActionResult> SalaryCertificateRequestCreateEdit(EmpSalaryCertificateRequestViewModel requestViewModel)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var isValid = await _employeeService.GetCommonEmployeeDDL(tokenData, requestViewModel.BusinessUnitId, requestViewModel.WorkplaceGroupId, requestViewModel.IntEmployeeId, null);

                if (isValid.Count() <= 0)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                PipelineStageInfoVM res = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(requestViewModel.IntEmployeeId, "salaryCertificateRequsition");


                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmpSalaryCertificateRequestCreateEdit";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strEntryType", requestViewModel.strEntryType);
                        sqlCmd.Parameters.AddWithValue("@IntSalaryCertificateRequestId", requestViewModel.IntSalaryCertificateRequestId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", requestViewModel.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", requestViewModel.IntEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@StrEmployeName", requestViewModel.StrEmployeName);
                        sqlCmd.Parameters.AddWithValue("@IntPayRollMonth", requestViewModel.IntPayRollMonth);
                        sqlCmd.Parameters.AddWithValue("@IntPayRollYear", requestViewModel.IntPayRollYear);
                        sqlCmd.Parameters.AddWithValue("@isActive ", requestViewModel.IsActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", requestViewModel.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@dteCreatedAt", requestViewModel.DteCreatedAt);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpGet]
        [Route("SalaryCertificateApplication")]
        public async Task<IActionResult> SalaryCertificateApplication(long? intSalaryCertificateRequestId, long BusinessUnitId, long WorkplaceGroupId, long? IntEmployeeId, long? MonthId, long? YearId, string? searchTxt, int PageNo, int PageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize || tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var result = await _employeeService.SalaryCertificateApplication(intSalaryCertificateRequestId, tokenData.accountId, BusinessUnitId, WorkplaceGroupId, IntEmployeeId, MonthId, YearId, searchTxt, PageNo, PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        //[HttpGet]
        //[Route("SalaryCertificateApplication")]
        //public async Task<IActionResult> SalaryCertificateApplication(string strPartName, long intAccountId, long intEmployeeId, long? intSalaryCertificateRequestId,
        //  string? strStatus, string? strSearchTxt, long? MonthId, long? YearId)
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
        //        {
        //            string sql = "saas.spSalaryCertificateRequestSelectQuery";

        //            using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
        //            {
        //                sqlCmd.CommandType = CommandType.StoredProcedure;
        //                sqlCmd.Parameters.AddWithValue("@strPartName", strPartName);
        //                sqlCmd.Parameters.AddWithValue("@intAccountId", intAccountId);
        //                sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
        //                sqlCmd.Parameters.AddWithValue("@strStatus", strStatus);
        //                sqlCmd.Parameters.AddWithValue("@strSearchTxt", strSearchTxt);
        //                sqlCmd.Parameters.AddWithValue("@intSalaryCertificateRequestId", intSalaryCertificateRequestId);
        //                sqlCmd.Parameters.AddWithValue("@MonthId", MonthId);
        //                sqlCmd.Parameters.AddWithValue("@YearId", YearId);

        //                connection.Open();

        //                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
        //                {
        //                    sqlAdapter.Fill(dt);
        //                }
        //                connection.Close();
        //            }
        //        }
        //        string dataTbl = JsonConvert.SerializeObject(dt);
        //        return Ok(dataTbl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}

        [HttpGet]
        [Route("LastMonthOfSalaryGenerate")]
        public async Task<IActionResult> LastMonthOfSalaryGenerate(long IntAccountId, long IntEmployeeId)
        {
            try
            {
                var month = (from sr in _context.PyrPayrollSalaryGenerateRequests
                             join sh in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals sh.IntSalaryGenerateRequestId
                             where sr.IsActive == true && sh.IsActive == true && sr.IntAccountId == IntAccountId && sh.IntEmployeeId == IntEmployeeId
                             select new
                             {
                                 Month = sh.IntMonthId,
                                 Year = sh.IntYearId
                             }).FirstOrDefault();
                return Ok(month);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion ============== SalaryCertificate Request ====================

        #region ==========Shift Management ===========
        //sagor -- 07 march 2023
        [HttpGet("GetEmployeeShiftInfo")]
        public async Task<IActionResult> GetEmployeeShiftInfoAsync(long intEmployeeId, long intYear, long intMonth)
        {
            try
            {
                return Ok(await _employeeService.GetEmployeeShiftInfoAsync(intEmployeeId, intYear, intMonth));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet("GetCalenderDdl")]
        public async Task<IActionResult> GetCalenderDdlAsync(long intAccountId, long intBusinessUnitId)
        {
            try
            {
                return Ok(await _employeeService.GetCalenderDdlAsync(intAccountId, intBusinessUnitId));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost("PostCalendarAssign")]
        public async Task<IActionResult> PostCalendarAssignAsync(CalendarAssignList model)
        {
            try
            {
                return Ok(await _employeeService.PostCalendarAssignAsync(model));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet("LogAttendanceOfChangedCalendar")]
        public async Task<IActionResult> LogAttendanceOfChangedCalendarAsync(long EmployeeId, DateTime FromDate, DateTime Todate)
        {
            try
            {
                return Ok(await _employeeService.LogAttendanceOfChangedCalendarAsync(EmployeeId, FromDate, Todate));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion

        #region====Off day reassign====
        [HttpPost("EmployeeOffDayReassign")]
        public async Task<IActionResult> EmployeeOffDayReassignAsync(EmployeeOffDayReassignList model)
        {
            try
            {
                MessageHelper msg = await _employeeService.EmployeeOffDayReassignAsync(model);
                return msg.StatusCode == 200 ? Ok(msg) : BadRequest(msg);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetEmployeeOffDayReassignLanding")]
        public async Task<IActionResult> GetEmployeeOffDayReassignLandingAsync(int IntMonthId, int IntYearId, long EmployeeId)
        {
            try
            {
                var res = await _employeeService.GetEmployeeOffDayReassignLandingAsync(IntMonthId, IntYearId, EmployeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion

        #region ===== Address related data write from json ======
        [HttpPost]
        [Route("AddressDataWriteFromJson")]
        public async Task<IActionResult> AddressDataWriteFromJson(DivisionDistrictThanaPostOfficeViewModel model)
        {
            try
            {
                MessageHelper msg = new();
                List<Division> divisionsList = new List<Division>();

                foreach (var item in model.DivisionVMs)
                {
                    Division division = new Division
                    {
                        IntDivisionId = (long)item.Id,
                        StrDivision = item.Name,
                        StrDivisionBn = item.Bn_name,
                        StrLatitude = item.Lat,
                        StrLongitude = item.Long,
                        IntCountryId = 18,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now
                    };
                    divisionsList.Add(division);
                }

                await _context.Divisions.AddRangeAsync(divisionsList);
                await _context.SaveChangesAsync();

                List<District> districtsList = new List<District>();
                foreach (var item in model.DistrictVMs)
                {
                    District district = new District
                    {
                        IntDistrictId = (long)item.Id,
                        StrDistrict = item.Name,
                        StrDistrictBn = item.Bn_name,
                        StrLatitude = item.Lat,
                        StrLongitude = item.Long,
                        IntDivisionId = (long)item.Division_id,
                        IsActive = true,
                        IntCountryId = 18,
                        DteCreatedAt = DateTime.Now
                    };
                    districtsList.Add(district);
                }
                await _context.Districts.AddRangeAsync(districtsList);
                await _context.SaveChangesAsync();

                List<Thana> thanasList = new List<Thana>();
                foreach (var item in model.ThanaVMs)
                {
                    Thana thana = new Thana
                    {
                        IntThanaId = (long)item.Id,
                        IntDistrictId = (long)item.District_id,
                        StrThanaName = item.Name,
                        StrThanaBn = item.Bn_name,
                        IntCountryId = 18,
                        IntDivisionId = 0,
                        IsActive = true,
                        DteInsertDateTime = DateTime.Now
                    };
                    thanasList.Add(thana);
                }
                await _context.Thanas.AddRangeAsync(thanasList);
                await _context.SaveChangesAsync();

                List<PostOffice> postOfficesList = new List<PostOffice>();
                foreach (var item in model.PostOfficeVMs)
                {
                    PostOffice postOffice = new PostOffice
                    {
                        StrPostOffice = item.PostOffice,
                        StrPostCode = item.PostCode,
                        StrThanaName = item.Upazila,
                        IntDivisionId = item.Division_id,
                        IntDistrictId = item.District_id,
                        IntCountryId = 18,
                        DteCreateAt = DateTime.Now
                    };
                    postOfficesList.Add(postOffice);
                }

                await _context.PostOffices.AddRangeAsync(postOfficesList);
                await _context.SaveChangesAsync();


                return Ok(msg);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion

        #region === Active current inactive Employee===
        [Route("PostAcitveCurrentInactiveEmployee")]
        [HttpPost]
        public async Task<IActionResult> PostAcitveCurrentInactiveEmployeeAsync(Emp emp)
        {
            try
            {

                return Ok(await _employeeService.PostAcitveCurrentInactiveEmployeeAsync(emp.IntEmployeeId));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("GetInactiveEmployeeList")]
        public async Task<IActionResult> GetInactiveEmployeeList(InactiveEmployeeLandingFilterVM obj)
        {
            //profileLandingFilterVM
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.businessUnitId, workplaceGroupId = obj.workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize && tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                InactiveEmployeeListLanding inactiveEmp = await _employeeService.GetInactiveEmployeeList(tokenData, obj.businessUnitId, obj.workplaceGroupId, obj.FromDate, obj.ToDate, obj.PageNo, obj.PageSize, obj.searchTxt, obj.IsPaginated, obj.IsHeaderNeed,
                    obj.StrDepartmentList, obj.StrDesignationList,  obj.StrSupervisorNameList, obj.StrLinemanagerList, obj.StrEmploymentTypeList, obj.WingNameList, obj.SoleDepoNameList, obj.RegionNameList, obj.AreaNameList,
                    obj.TerritoryNameList);

                return Ok(inactiveEmp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion

        #region ==== Get Employee All Role And Extensions====
        [HttpGet("GetEmployeeRoleExtensions")]
        public async Task<IActionResult> GetEmployeeRoleExtensions(long IntEmployeeId, long IntAccountId)
        {
            try
            {

                return Ok(await _employeeService.GetEmployeeRoleExtensions(IntEmployeeId, IntAccountId));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion


        [HttpGet]
        [Route("ContractualClosing")]
        public async Task<IActionResult> ContractualClosing(long businessUnitId, long workplaceGroupId, bool IsPaginated, string? searchTxt, int pageSize, int pageNo)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var query = (from eb in _context.EmpEmployeeBasicInfos
                             join empD in _context.EmpEmployeeBasicInfoDetails on eb.IntEmployeeBasicInfoId equals empD.IntEmployeeId into empD2
                             from empD in empD2.DefaultIfEmpty()
                             join desig in _context.MasterDesignations on eb.IntDesignationId equals desig.IntDesignationId into desigJoin
                             from desig in desigJoin.DefaultIfEmpty()
                             join dept in _context.MasterDepartments on eb.IntDepartmentId equals dept.IntDepartmentId into deptJoin
                             from dept in deptJoin.DefaultIfEmpty()
                             join supervisor in _context.EmpEmployeeBasicInfos on eb.IntSupervisorId equals supervisor.IntEmployeeBasicInfoId into supervisorJoin
                             from supervisor in supervisorJoin.DefaultIfEmpty()
                             join details in _context.EmpEmployeeBasicInfoDetails on eb.IntEmployeeBasicInfoId equals details.IntEmployeeId into detailsJoin
                             from details in detailsJoin.DefaultIfEmpty()
                             join pho in _context.EmpEmployeePhotoIdentities on eb.IntEmployeeBasicInfoId equals pho.IntEmployeeBasicInfoId into phoJoin
                             from pho in phoJoin.DefaultIfEmpty()

                             where eb.IntAccountId == tokenData.accountId && eb.IsActive == true && eb.DteContactToDate.Value.Date >= DateTime.Now.Date.AddDays(-30)
                             && eb.DteConfirmationDate == null
                             && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && eb.IntBusinessUnitId == businessUnitId
                             && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && eb.IntWorkplaceGroupId == workplaceGroupId

                            && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                            : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId)
                            : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                            : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                            : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                            : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                            : tokenData.territoryList.Contains(empD.IntTerritoryId))
                            || eb.IntDottedSupervisorId == tokenData.employeeId || eb.IntSupervisorId == tokenData.employeeId || eb.IntLineManagerId == tokenData.employeeId)

                             && (IsPaginated == true ?

                             (!string.IsNullOrEmpty(searchTxt) ? (eb.StrEmployeeName.ToLower().Contains(searchTxt) || eb.StrEmployeeCode.ToLower().Contains(searchTxt)
                             || eb.StrReferenceId.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                             || empD.StrPinNo.ToLower().Contains(searchTxt) || empD.StrPersonalMobile.ToLower().Contains(searchTxt)) : true) : true)

                             orderby eb.IntEmployeeBasicInfoId descending
                             select new
                             {
                                 EmployeeId = eb.IntEmployeeBasicInfoId,
                                 EmployeeName = eb.StrEmployeeName,
                                 EmployeeCode = eb.StrEmployeeCode,
                                 DepartmentId = eb.IntDepartmentId,
                                 DepartmentName = dept.StrDepartment,
                                 eb.DteJoiningDate,
                                 eb.DteContactFromDate,
                                 eb.DteContactToDate,
                                 eb.IntEmploymentTypeId,
                                 EmploymentType = eb.StrEmploymentType,
                                 DesignationId = eb.IntDesignationId,
                                 DesignationName = desig.StrDesignation,
                                 SupervisorId = supervisor.IntEmployeeBasicInfoId,
                                 SupervisorName = supervisor.StrEmployeeName,
                                 pho.IntProfilePicFileUrlId,
                                 Phone = details.StrOfficeMobile,
                                 Email = details.StrOfficeMail
                             });

                var result = query.ToList();


                if (IsPaginated == false)
                {
                    return Ok(await query.ToListAsync());
                }
                else
                {
                    int maxSize = 1000;
                    pageSize = pageSize > maxSize ? maxSize : pageSize;
                    pageNo = pageNo < 1 ? 1 : pageNo;

                    dynamic data = new
                    {
                        TotalCount = await query.CountAsync(),
                        Data = await query.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToListAsync(),
                        pageSize = pageSize,
                        CurrentPage = pageNo,
                    };
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message, StatusCode = 500 });
            }
        }


    }
}