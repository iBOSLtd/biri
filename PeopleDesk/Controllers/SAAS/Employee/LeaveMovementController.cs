using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.SAAS.Interfaces;
using PeopleDesk.Services.SignalR.Interfaces;
using System.ComponentModel;
using System.Data;

namespace PeopleDesk.Controllers.SAAS.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveMovementController : ControllerBase
    {
        private readonly ILeaveMovementService _leaveMovement;
        private readonly PeopleDeskContext _context;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private readonly IEmployeeService _employeeService;
        private readonly INotificationService _notificationService;

        public LeaveMovementController(INotificationService _notificationService, PeopleDeskContext _context, ILeaveMovementService _leaveMovement, IApprovalPipelineService approvalPipelineService, IEmployeeService _employeeService)
        {
            this._leaveMovement = _leaveMovement;
            this._context = _context;
            _approvalPipelineService = approvalPipelineService;
            this._employeeService = _employeeService;
            this._notificationService = _notificationService;
        }


        #region ===> LEAVE <===
        [HttpPost]
        [Route("CRUDLeaveApplication")]
        public async Task<IActionResult> CRUDLeaveApplication(LeaveApplicationDTO obj)
        {
            try
            {
                //PartId = 1 -> Create
                //PartId = 2 -> Edit
                //PartId = 3 -> Delete
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                obj.AccountId = tokenData.accountId;
                MessageHelper msg = await _leaveMovement.CRUDLeaveApplication(obj);

                if (msg.StatusCode == 200)
                {
                    if (obj.PartId == 1)
                    {
                        obj.LeaveApplicationId = (long)msg.AutoId;

                        bool notify = await _notificationService.LeaveApplicationNotify(obj);
                        if (!notify)
                        {
                            NotifySendFailedLog notifySend = new NotifySendFailedLog
                            {
                                IntEmployeeId = obj.EmployeeId,
                                IntFeatureTableAutoId = (long)msg.AutoId,
                                StrFeature = "leave_application",
                                DteCreatedAt = DateTime.Now,
                                IntCreatedBy = obj.EmployeeId
                            };
                            await _context.NotifySendFailedLogs.AddAsync(notifySend);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetEmployeeLeaveBalanceAndHistory")]
        public async Task<IActionResult> GetEmployeeLeaveBalanceAndHistory(long EmployeeId, long WorkPlaceGroup, long BusinessUnit, string ViewType, long? LeaveTypeId, DateTime? ApplicationDate, long IntYear, int? StatusId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != EmployeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, BusinessUnit, WorkPlaceGroup, EmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }

                return Ok(JsonConvert.SerializeObject(await _leaveMovement.GetEmployeeLeaveBalanceAndHistory(EmployeeId, ViewType, LeaveTypeId, ApplicationDate, IntYear, StatusId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllLeaveApplicatonListForApprove")]
        public async Task<IActionResult> GetAllLeaveApplicatonListForApprove(LeaveDTO model)
        {
            try
            {
                return Ok(await _leaveMovement.GetAllLeaveApplicatonListForApprove(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSingleLeaveApplicatonListForApprove")]
        public async Task<IActionResult> GetSingleLeaveApplicatonListForApprove(long? ViewType, long? EmployeeId, long? ApplicationId)
        {
            try
            {
                return Ok(await _leaveMovement.GetSingleLeaveApplicatonListForApprove(ViewType, EmployeeId, ApplicationId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion


        #region ===< MOVEMENT >===
        [HttpPost]
        [Route("CRUDMovementApplication")]
        public async Task<IActionResult> CRUDMovementApplication(MovementApplicationDTO obj)
        {
            try
            {
                //PartId = 1 -> Create
                //PartId = 2 -> Edit
                //PartId = 3 -> Delete

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != obj.IntEmployeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.BusinessUnitId, obj.WorkplaceGroupId, obj.IntEmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }


                var msg = await _leaveMovement.CRUDMovementApplication(obj);
                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    if (obj.PartId == 1)
                    {
                        obj.MovementId = (long)msg.AutoId;

                        bool notify = await _notificationService.MovementApplicationNotify(obj);
                        if (!notify)
                        {
                            NotifySendFailedLog notifySend = new NotifySendFailedLog
                            {
                                IntEmployeeId = obj.IntEmployeeId,
                                IntFeatureTableAutoId = (long)msg.AutoId,
                                StrFeature = "movement_application",
                                DteCreatedAt = DateTime.Now,
                                IntCreatedBy = obj.IntEmployeeId
                            };
                            await _context.NotifySendFailedLogs.AddAsync(notifySend);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("GetAllMovementApplicatonListForApprove")]
        public async Task<IActionResult> GetAllMovementApplicatonListForApprove(LeaveDTO model)
        {
            try
            {
                return Ok(await _leaveMovement.GetAllMovementApplicatonListForApprove(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSingleMovementApplicatonListForApprove")]
        public async Task<IActionResult> GetSingleMovementApplicatonListForApprove(long? ViewType, long? EmployeeId, long? MovementId)
        {
            try
            {
                return Ok(await _leaveMovement.GetSingleMovementApplicatonListForApprove(ViewType, EmployeeId, MovementId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("MovementApprove")]
        public async Task<IActionResult> MovementApprove(List<LeaveAndMovementApprovedDTO> model)
        {
            try
            {
                return Ok(await _leaveMovement.MovementApprove(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion



        #region ===> LEAVE MOVEMENT TYPE <===
        [HttpPost]
        [Route("CRUDLeaveMovementType")]
        public async Task<IActionResult> CRUDLeaveMovementType(LeaveMovementTypeDTO obj)
        {
            try
            {
                //PartId = 1 -> Create
                //PartId = 2 -> Edit
                //PartId = 3 -> Delete

                var response = await _leaveMovement.CRUDLeaveMovementType(obj);

                if (response.Message == "500")
                {
                    response.Message = "Already exist";
                    response.StatusCode = 500;
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region ===> Employment Typewise Leave Balance <===
        [HttpPost]
        [Route("CRUDEmploymentTypeWiseLeaveBalance")]
        public async Task<IActionResult> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeWiseLeaveBalanceDTO obj)
        {
            try
            {
                //PartId = 1 -> Create
                //PartId = 2 -> Edit
                //PartId = 3 -> Delete

                var response = await _leaveMovement.CRUDEmploymentTypeWiseLeaveBalance(obj);

                if (response.Message == "500")
                {
                    response.Message = "Already exist";
                    response.StatusCode = 500;
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Landing

        [HttpGet]
        [Route("LeaveBalanceHistoryForAllEmployee")]
        public async Task<IActionResult> LeaveBalanceHistoryForAllEmployee(long BusinessUnitId, long yearId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? EmployeeId)
        {
            try
            {   /*
                if (BusinessUnitId <= 0 || yearId <= 0)
                {
                    return NotFound("Invalid Business Unit OR Year");
                }

                List<EmployeeQryProfileAllViewModel> employeeList = await _iQRYDataForReportService.EmployeeQryProfileAllList(BusinessUnitId, WorkplaceGroupId, DeptId, DesigId, EmployeeId);

                List<LveLeaveBalance> leaveBalanceList = await (from item in _context.LveLeaveBalances
                                                                where item.IsActive == true &&
                                                                (yearId > 0 ? item.IntYear == yearId : true)
                                                                select item).AsNoTracking().ToListAsync();

                List<LveLeaveBalance> leaveTypeList = await (from item in _context.LveLeaveBalances
                                                             where item.IsActive == true
                                                          select item).AsNoTracking().ToListAsync();

                List<LeaveBalanceHistoryForAllEmployeeDTO> data = (from emp in employeeList
                                                                   select new LeaveBalanceHistoryForAllEmployeeDTO
                                                                   {
                                                                       EmployeeId = emp?.EmployeeBasicInfo?.IntEmployeeId,
                                                                       Employee = emp?.EmployeeBasicInfo?.StrEmployeeName + " [" + emp?.EmployeeBasicInfo?.StrEmployeeCode + "]",
                                                                       Department = emp.DepartmentName + ", " + emp.EmploymentTypeName,
                                                                       Designation = emp.DesignationName,
                                                                       CLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "CL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                                                       CLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "CL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                                                       SLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "SL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                                                       SLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "SL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                                                       ELBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "EL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                                                       ELTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "EL")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                                                       LWPBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "LWP")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                                                       LWPTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "LWP")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,
                                                                       MLBalance = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "ML")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntBalanceDays,
                                                                       MLTaken = leaveBalanceList.Where(x => x.IntYear == yearId && x.IntEmployeeId == emp?.EmployeeBasicInfo?.IntEmployeeId && x.IntLeaveTypeId == leaveTypeList.Where(x => x.StrLeaveTypeCode == "ML")?.FirstOrDefault()?.IntLeaveTypeId)?.FirstOrDefault()?.IntLeaveTakenDays,

                                                                   }).ToList();

                return Ok(data);
                */
                throw new NotImplementedException();

            }
            catch (Exception ex)
            {
                return NotFound("Something went wrong");
            }
        }

        [HttpGet]
        [Route("AllEmployeeMovementReport")]
        public async Task<IActionResult> AllEmployeeMovementReport(long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? MovementTypeId, long? EmployeeId, DateTime FromDate, DateTime ToDate, string? applicationStatus)
        {
            try
            {
                List<MovementReportViewModel> data = new List<MovementReportViewModel>();
                MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == BusinessUnitId).AsNoTracking().FirstOrDefaultAsync();
                DataTable dt = new DataTable();
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


        #endregion




        public DataTable ListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }


    }
}
