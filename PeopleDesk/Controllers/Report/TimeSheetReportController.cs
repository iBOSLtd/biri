using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
using System.Linq.Dynamic.Core;

namespace PeopleDesk.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetReportController : ControllerBase
    {
        private readonly PeopleDeskContext _context;
        private readonly ISaasMasterService _saasMasterService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeAllLanding _employeeAllService;
        private DataTable dt = new DataTable();

        public TimeSheetReportController(ISaasMasterService saasMasterService, IEmployeeService employeeService, PeopleDeskContext _context, IEmployeeAllLanding employeeAllService)
        {
            this._context = _context;
            _saasMasterService = saasMasterService;
            _employeeService = employeeService;
            _employeeAllService = employeeAllService;
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
                                                      join emppd in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals emppd.IntEmployeeId into emppdd
                                                      from emppd2 in emppdd.DefaultIfEmpty()
                                                      join deptt in _context.MasterDepartments on emp.IntDepartmentId equals deptt.IntDepartmentId into deptt2
                                                      from dept in deptt2.DefaultIfEmpty()
                                                      join dess in _context.MasterDesignations on emp.IntDesignationId equals dess.IntDesignationId into dess2
                                                      from des in dess2.DefaultIfEmpty()
                                                      join paygg in _context.PyrPayscaleGrades on emppd2.IntPayscaleGradeId equals paygg.IntPayscaleGradeId into paygg2
                                                      from payg in paygg2.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          SalaryGenerateHeaderObj = sl,
                                                          EmployeeBasicInfoObj = emp,
                                                          EmpDepartmentObj = dept,
                                                          EmpDesignationObj = des,
                                                          PayscaleGradeObj = payg
                                                      }).AsNoTracking().ToListAsync();

                return Ok(SalaryGenerateHeaderList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetEmpAttendanceReport")]
        public async Task<IActionResult> GetEmpAttendanceReport(long IntBusinessUnitId, long IntWorkplaceGroupId, DateTime FromDate, DateTime ToDate, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = IntBusinessUnitId, workplaceGroupId = IntWorkplaceGroupId }, PermissionLebelCheck.BusinessUnit);

                if (tokenData.accountId == -1 && !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                TimeSheetEmpAttenReportLanding result = await _employeeAllService.GetEmpAttendanceReport(tokenData.accountId, IntBusinessUnitId, IntWorkplaceGroupId, FromDate, ToDate, IsXls, PageNo, PageSize, searchTxt);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetAttendanceReport")]
        public async Task<IActionResult> GetAttendanceReport(DateTime FromDate, DateTime ToDate, long AccountId, long BusinessUnitId, long? IntWorkplaceGroupId, string? SearchTxt, int? PageNo, int? PageSize, bool IsPaginated)
        {
            //[timesheet].[sprEmployeeAttendanceReport]
            //@dteFromDate DATE, @dteToDate DATE, @intBusinessUnitId BIGINT, @intWorkPlaceId BIGINT
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeAttendanceReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", IntWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", SearchTxt);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
                        sqlCmd.Parameters.AddWithValue("@isPaginated", IsPaginated);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<EmployeeAttendanceReportViewModel> attendanceReportList = new List<EmployeeAttendanceReportViewModel>();

                foreach (DataRow row in dt.Rows)
                {
                    EmployeeAttendanceReportViewModel attendanceReport = new EmployeeAttendanceReportViewModel();
                    attendanceReport.EmployeeId = Convert.ToInt64(row["employeeId"]);
                    attendanceReport.EmployeeCode = row["employeeCode"].ToString();
                    attendanceReport.EmployeeName = row["employeeName"].ToString();
                    attendanceReport.DepartmentId = Convert.ToInt64(row["departmentId"]);
                    attendanceReport.DepartmentName = row["departmentName"].ToString();
                    attendanceReport.DesignationId = Convert.ToInt64(row["designationId"]);
                    attendanceReport.DesignationName = row["designationName"].ToString();
                    attendanceReport.EmploymentTypeId = Convert.ToInt64(row["employmentTypeId"]);
                    attendanceReport.EmploymentTypeName = row["employmentTypeName"].ToString();
                    attendanceReport.Email = row["email"].ToString();
                    attendanceReport.BusinessUnitName = row["businessUnitName"].ToString();
                    attendanceReport.Phone = row["Phone"].ToString();
                    attendanceReport.OfficialPhone = row["OfficialPhone"].ToString();
                    attendanceReport.WorkingDays = Convert.ToInt64(row["workingDays"]);
                    attendanceReport.Present = Convert.ToInt64(row["present"]);
                    attendanceReport.Absent = Convert.ToInt64(row["absent"]);
                    attendanceReport.Movement = Convert.ToInt64(row["movement"]);
                    attendanceReport.Holiday = Convert.ToInt64(row["holiday"]);
                    attendanceReport.offDay = Convert.ToInt64(row["offDay"]);
                    attendanceReport.Late = Convert.ToInt64(row["late"]);
                    attendanceReport.SalaryStatus = row["salaryStatus"].ToString();
                    attendanceReport.TotalCount = Convert.ToInt64(row["TotalCount"]);

                    attendanceReportList.Add(attendanceReport);
                }

                List<LeaveTypeVM> type = await _saasMasterService.GetAllLveLeaveType(AccountId);

                List<LeaveTypeWiseCount> typeWiseCounts = new List<LeaveTypeWiseCount>();
                LeaveTypeWiseCount leaveCount = new LeaveTypeWiseCount();

                attendanceReportList.ForEach(x =>
                {
                    type.ForEach(t =>
                    {
                        leaveCount.EmployeeId = x.EmployeeId;
                        leaveCount.LeaveTypeId = t.IntLeaveTypeId;
                        leaveCount.LeaveType = t.StrLeaveType;
                        leaveCount.TotalLeave = (long)_employeeService.EmployeeLeaveTaken(x.EmployeeId, FromDate, ToDate, t.IntLeaveTypeId);
                        typeWiseCounts.Add(leaveCount);

                        leaveCount = new LeaveTypeWiseCount();
                    });

                    x.LeaveTypeWiseList = typeWiseCounts;
                    typeWiseCounts = new List<LeaveTypeWiseCount>();
                });

                return Ok(attendanceReportList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //get report in dapper
        [HttpGet]
        [Route("GetAttendanceReportDapper")]
        public async Task<IActionResult> GetAttendanceReportDapper(DateTime FromDate, DateTime ToDate, long AccountId, long BusinessUnitId)
        {
            try
            {
                using var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprEmployeeAttendanceReport";

                var values = new
                {
                    dteFromDate = FromDate,
                    dteToDate = ToDate,
                    intAccountId = AccountId,
                    intBusinessUnitId = BusinessUnitId,
                };

                List<EmployeeAttendanceReportViewModel> attendanceReportList = (await connection.QueryAsync<EmployeeAttendanceReportViewModel>(sql, values, commandType: CommandType.StoredProcedure)).ToList();

                List<LeaveTypeVM> type = await _saasMasterService.GetAllLveLeaveType(AccountId);

                List<LeaveTypeWiseCount> typeWiseCounts = new List<LeaveTypeWiseCount>();
                LeaveTypeWiseCount leaveCount = new LeaveTypeWiseCount();

                attendanceReportList.ForEach(x =>
                {
                    type.ForEach(t =>
                    {
                        leaveCount.EmployeeId = x.EmployeeId;
                        leaveCount.LeaveTypeId = t.IntLeaveTypeId;
                        leaveCount.LeaveType = t.StrLeaveType;
                        //this subquery is taking time
                        leaveCount.TotalLeave = (long)_employeeService.EmployeeLeaveTaken(x.EmployeeId, FromDate, ToDate, t.IntLeaveTypeId);
                        typeWiseCounts.Add(leaveCount);

                        leaveCount = new LeaveTypeWiseCount();
                    });

                    x.LeaveTypeWiseList = typeWiseCounts;
                    typeWiseCounts = new List<LeaveTypeWiseCount>();
                });


                return Ok(attendanceReportList);

            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        [Route("GetAttendanceByRosterReport")]
        public async Task<IActionResult> GetAttendanceByRosterReport(long monthId, long yearId, long businessUnitId, string departmentList, long workplaceGroupId, string? srcText)
        {
            //@intMonthId INT, @intYear INT, @intBusinessunitId INT, @deptList NVARCHAR(MAX), @intWorkplaceGroupId BIGINT
            string msg = string.Empty;

            try
            {
                msg = monthId <= 0 ? " Invalid monthId" : " ";
                msg += yearId <= 0 ? " Invalid yearId" : " ";
                msg += businessUnitId <= 0 ? " Invalid businessUnitId" : " ";
                msg += string.IsNullOrEmpty(departmentList) ? " Invalid departmentList" : " ";
                msg += workplaceGroupId <= 0 ? " Invalid workplaceGroupId" : " ";

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprAttendanceByRosterReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intMonthId", monthId);
                        sqlCmd.Parameters.AddWithValue("@intYear", yearId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessunitId", businessUnitId);
                        sqlCmd.Parameters.AddWithValue("@deptList", departmentList);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", workplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@srcText", srcText);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                var retData = await CommonHelper.GetDynamicDataTable(dt);

                return Ok(retData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetRosterReport")]
        public async Task<IActionResult> GetRosterReport(long? AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? WorkplaceId, long? CalendarTypeId, long? CalendarId, long? DepartmentId, long? DesignationId, DateTime? UserDate, long? RosterGroupId, DateTime DteFromDate, DateTime DteToDate)
        {
            string msg = string.Empty;

            try
            {
                msg += CalendarTypeId <= 0 ? " Invalid Calendar Type " : " ";

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRosterReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkPlaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intWorkPlaceId", WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@intCalendarId", CalendarId);
                        sqlCmd.Parameters.AddWithValue("@dteUserDate", UserDate);
                        sqlCmd.Parameters.AddWithValue("@intCalenderTypeId", CalendarTypeId);
                        sqlCmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                        sqlCmd.Parameters.AddWithValue("@DesignationId", DesignationId);
                        sqlCmd.Parameters.AddWithValue("@RosterGroupId", RosterGroupId);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", DteFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", DteToDate);


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
                return NotFound(msg);
            }
        }


        [HttpGet]
        [Route("GetRosterDetailsReport")]
        public async Task<IActionResult> GetRosterDetailsReport(long monthId, long yearId, long businessUnitId, string? departmentList, long? designationId, long? workplaceGroupId, long? employmentTypeId, string? srcText)
        {
            //@intMonthId INT, @intYear INT, @intBusinessunitId INT, @deptList NVARCHAR(MAX), @intWorkplaceGroupId BIGINT, @intEmploymentTypeId BIGINT
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRosterDetailsReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intMonthId", monthId);
                        sqlCmd.Parameters.AddWithValue("@intYear", yearId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessunitId", businessUnitId);
                        sqlCmd.Parameters.AddWithValue("@deptList", departmentList);
                        sqlCmd.Parameters.AddWithValue("@designationId", designationId);
                        //sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", workplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intEmploymentTypeId", employmentTypeId);
                        sqlCmd.Parameters.AddWithValue("@srcText", string.IsNullOrEmpty(srcText) ? "" : srcText);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                var retData = await CommonHelper.GetDynamicDataTable(dt);

                return Ok(retData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("TimeManagementDynamicPIVOTReport")]
        public async Task<IActionResult> TimeManagementDynamicPIVOTReport(string? ReportType, long? AccountId, DateTime DteFromDate, DateTime DteToDate, long? EmployeeId, long? WorkplaceGroupId, long? WorkplaceId, string? SearchTxt, int? PageNo, int? PageSize, bool IsPaginated)
        {
            string msg = string.Empty;

            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprTimeManagementDynamicPIVOTReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@reportType", ReportType);
                        sqlCmd.Parameters.AddWithValue("@accountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@fromDate", DteFromDate);
                        sqlCmd.Parameters.AddWithValue("@toDate", DteToDate);
                        sqlCmd.Parameters.AddWithValue("@employeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@WorkPlaceGroup", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@WorkPlace", WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", SearchTxt);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
                        sqlCmd.Parameters.AddWithValue("@isPaginated", IsPaginated);


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
                return NotFound(msg);
            }
        }

        [HttpGet]
        [Route("MonthlyRosterReportForSingleEmployee")]
        public async Task<IActionResult> MonthlyRosterReportForSingleEmployee(long BusinessUnitId, long WorkplaceGroupId, long EmployeeId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize)
        {
            BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM() { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId}, PermissionLebelCheck.WorkplaceGroup);

            if (!tokenData.isAuthorize)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }
            else
            {
                var data = await _employeeService.GetCommonEmployeeDDL(tokenData, BusinessUnitId, WorkplaceGroupId, EmployeeId, null);

                if (data.Count() <= 0)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
            }

            var result = (from ads in _context.TimeAttendanceDailySummaries
                          join ebi in _context.EmpEmployeeBasicInfos on ads.IntEmployeeId equals ebi.IntEmployeeBasicInfoId
                          join ebid in _context.EmpEmployeeBasicInfoDetails on ads.IntEmployeeId equals ebid.IntEmployeeId
                          join d in _context.MasterDepartments on ebi.IntDepartmentId equals d.IntDepartmentId
                          join d1 in _context.MasterDesignations on ebi.IntDesignationId equals d1.IntDesignationId
                          where ebi.IntAccountId == tokenData.accountId && ebi.IsActive == true 
                          && (ebid.IntEmployeeStatusId == 1 || ebid.IntEmployeeStatusId == 4) 
                          && ads.DteAttendanceDate >= FromDate && ads.DteAttendanceDate <= ToDate 
                          && ads.IntEmployeeId == EmployeeId                          
                          orderby ads.IntEmployeeId ascending
                          select new
                          {
                              ebi.IntEmployeeBasicInfoId,
                              ads.DteAttendanceDate,
                              ebi.StrEmployeeName,
                              d.StrDepartment,
                              d1.StrDesignation,
                              ads.StrCalendarName,
                              ads.DteStartTime,
                              ads.DteEndTime,
                              ads.DteLastStartTime,
                              ads.TmeInTime,
                              ads.TmeLastOutTime
                          }).AsNoTracking().AsQueryable();

            int maxSize = 1000;
            PageSize = PageSize > maxSize ? maxSize : PageSize;
            PageNo = PageNo < 1 ? 1 : PageNo;

            MonthlyRosterReportForSingleEmployee rosterReport = new();

            rosterReport.TotalCount = await result.CountAsync();
            rosterReport.Data = await result.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
            rosterReport.PageSize = PageSize;
            rosterReport.CurrentPage = PageNo;

            return Ok(rosterReport);
        }

        //[HttpGet]
        //[Route("MonthlyRosterReportForAllEmployee")]
        //public async Task<IActionResult> MonthlyRosterReportForAllEmployee(long accountId, long? workplaceGroupId, DateTime FromDate, DateTime ToDate, int pageNo, int pageSize)
        //{
        //    var primarySalaryHead = string.Join(",", _context.TimeAttendanceDailySummaries
        //    .Join(_context.EmpEmployeeBasicInfos, ads => ads.IntEmployeeId, ebi => ebi.IntEmployeeBasicInfoId, (ads, ebi) => new { ads, ebi })
        //    .Where(x => x.ebi.IntAccountId == accountId && x.ads.DteAttendanceDate >= FromDate && x.ads.DteAttendanceDate <= ToDate
        //        && (workplaceGroupId == null || x.ebi.IntWorkplaceGroupId == workplaceGroupId))
        //    .GroupBy(x => x.ads.DteAttendanceDate)
        //    .Select(x => x.Key)
        //    .ToArray());

        //    var rosterReport = from ads in _context.TimeAttendanceDailySummaries
        //                       join emp in _context.EmpEmployeeBasicInfos on ads.IntEmployeeId equals emp.IntEmployeeBasicInfoId
        //                       join empDetails in _context.EmpEmployeeBasicInfoDetails on ads.IntEmployeeId equals empDetails.IntEmployeeId into empDetails2
        //                       from empDetails in empDetails2.DefaultIfEmpty()
        //                       join dep in _context.MasterDepartments on emp.IntDepartmentId equals dep.IntDepartmentId into departM
        //                       from dep in departM.DefaultIfEmpty()
        //                       join desi in _context.MasterDesignations on emp.IntDesignationId equals desi.IntDesignationId into desig
        //                       from desi in desig.DefaultIfEmpty()
        //                       where emp.IntAccountId == accountId && emp.IsActive == true 
        //                       && ads.DteAttendanceDate.Value.Date >= FromDate 


        //    var tempTable = _context.TimeAttendanceDailySummaries
        //        .Join(_context.EmpEmployeeBasicInfos, ads => ads.IntEmployeeId, ebi => ebi.IntEmployeeBasicInfoId, (ads, ebi) => new { ads, ebi })
        //        .Join(_context.EmpEmployeeBasicInfoDetails, x => x.ads.IntEmployeeId, ebid => ebid.IntEmployeeId, (x, ebid) => new { x, ebid })
        //        .Join(_context.MasterDepartments, x => x.x.ebi.IntDepartmentId, d => d.IntDepartmentId, (x, d) => new { x, d })
        //        .Join(_context.MasterDesignations, x => x.x.x.ads.intde, d1 => d1.IntDesignationId, (x, d1) => new { x, d1 })
        //        .Where(x => x.x.ebi.intAccountId == accountId && x.x.ebi.isActive == 1 && (new[] { 1, 4 }).Contains(x.x.ebid.intEmployeeStatusId)
        //            && x.x.ads.dteAttendanceDate >= fromDate && x.x.ads.dteAttendanceDate <= toDate
        //            && (WorkPlaceGroup == null || x.x.ebi.intWorkplaceGroupId == WorkPlaceGroup)
        //        .Select(x => new
        //        {
        //            x.x.ebi.intEmployeeBasicInfoId,
        //            EmployeeCode = x.x.ebi.strEmployeeCode,
        //            x.x.ebi.strEmployeeName,
        //            x.d.strDepartment,
        //            x.d1.strDesignation,
        //            strCalendarName = x.x.ads.strCalendarName ?? "N/A",
        //            x.x.ads.dteAttendanceDate
        //        })
        //        .ToList();

        //    var pivotQuery = tempTable
        //        .GroupBy(x => new { x.intEmployeeBasicInfoId, x.EmployeeCode, x.strEmployeeName, x.strDepartment, x.strDesignation })
        //        .Select(g => new
        //        {
        //            g.Key.intEmployeeBasicInfoId,
        //            g.Key.EmployeeCode,
        //            g.Key.strEmployeeName,
        //            g.Key.strDepartment,
        //            g.Key.strDesignation,
        //            AttendanceDates = g.Select(x => x.dteAttendanceDate).ToList()
        //        })
        //        .ToList()
        //        .ToPivotTable(x => new { x.intEmployeeBasicInfoId, x.EmployeeCode, x.strEmployeeName, x.strDepartment, x.strDesignation },
        //            x => x.AttendanceDates,
        //            x => x == null ? "N/A" : x.strCalendarName)
        //        .ToList();


        //    return Ok();
        //}


    }
}