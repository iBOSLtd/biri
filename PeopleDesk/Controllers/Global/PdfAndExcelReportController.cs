using ClosedXML.Excel;
using Dapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Global;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.Helper;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;
using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Text.Json;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfAndExcelReportController : Controller
    {
        private readonly IQRYDataForReportService _QRYDataForReportService;
        private readonly IPdfAndExcelService _pdfAndExcelService;
        private readonly ISaasMasterService _saasMasterService;
        private readonly IWebHostEnvironment env;
        private readonly IPdfTemplateGeneratorService _pdfTemplateGeneratorService;
        private readonly IReportrdlc _iReportrdlc;

        //private readonly IShippingMgmt _shippingMgmt;
        private IConverter _converter;

        private readonly PeopleDeskContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly IReportService _reportService;
        //private readonly IHCMService _hcmService;
        private readonly IEmployeeService _employeeService;

        //private readonly IMasterDataService _masterDataService;
        //private readonly IQRYDataForReportService _iQRYDataForReportService;
        //private readonly IVesselCertificate _iVesselCertificate;
        private DataTable dt = new DataTable();

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILeaveMovementService _leaveMovement;

        //private readonly IMedicalApplicationService _medicalService;
        //private readonly ISignOffService _signOffService;
        private string documentUrl;

        public PdfAndExcelReportController(/*IHCMService _hcmService,*/
            ISaasMasterService saasMasterService, ILeaveMovementService _leaveMovement,
            IWebHostEnvironment _hostEnvironment, IQRYDataForReportService _QRYDataForReportService,
            IEmployeeService _employeeService, PeopleDeskContext _context, IConverter _converter,
            IPdfAndExcelService _pdfAndExcelService, IQRYDataForReportService QRYDataForReportService,
            IHttpContextAccessor _httpContextAccessor, IWebHostEnvironment env,
            IReportrdlc _iReportrdlc
            )
        {
            this._QRYDataForReportService = QRYDataForReportService;
            this._pdfAndExcelService = _pdfAndExcelService;
            this._saasMasterService = saasMasterService;
            this._converter = _converter;
            this._context = _context;
            //this._reportService = _reportService;
            //this._hcmService = _hcmService;
            this._employeeService = _employeeService;
            //this._masterDataService = _masterDataService;
            //this._iQRYDataForReportService = _iQRYDataForReportService;
            this._hostEnvironment = _hostEnvironment;
            this._leaveMovement = _leaveMovement;
            this.env = env;
            this._httpContextAccessor = _httpContextAccessor;
            this.documentUrl = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/api/Document/DownloadFile?id=";
            //this._shippingMgmt = _shippingMgmt;
            //this._iVesselCertificate = _iVesselCertificate;
            //this._medicalService = _medicalService;
            //this._signOffService = _signOffService;
            this._iReportrdlc = _iReportrdlc;
        }

        //#region =======================  PDF REPORT ALL =================================

        //#region  H U M A I N - - -  R E S O U R C E - - - M A N A G E M E N T

        [HttpGet]
        [Route("EmployeeListExportAsExcel")]
        public async Task<IActionResult> EmployeeListExportAsExcel(int AccountId, int BusinessUnitId)
        {
            try
            {
                DataTable dt = await _employeeService.PeopleDeskAllLanding("EmployeeBasicInfoListForExcel", AccountId, BusinessUnitId, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

                Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.IntAccountId == AccountId);
                MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.FirstOrDefaultAsync(x => x.IntBusinessUnitId == BusinessUnitId);

                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + account.IntLogoUrlId}");
                Stream OrgLogo = new MemoryStream(img);

                //string date = DateTime.Now.ToString("dd-MM-yyy") + " to " + DateTime.Now.ToString("dd-MMM-yyyy");
                string date = String.Empty;

                EmployeeListExportAsExcelViewModel empListExport = new EmployeeListExportAsExcelViewModel
                {
                    dataTable = dt,
                    leftSidePeddingCell = 1,
                    topPeddingCell = 5,
                    SheetTitle = "All Employee Report List",
                    OrgLogo = OrgLogo,
                    TitleLine_One = account.StrAccountName,
                    TitleLine_Two = businessUnit.StrBusinessUnit,
                    TitleLine_Three = businessUnit.StrAddress,
                    date = date
                };

                MemoryStream stream = await _pdfAndExcelService.ExcelFileGenerate(empListExport);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{empListExport.SheetTitle}.xlsx");
            }
            catch (Exception e)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("DailyAttendanceReportByEmployee")]
        public async Task<IActionResult> PdfDailyAttendanceReportByEmployee(long TypeId, long EmployeeId, DateTime FromDate, DateTime ToDate)
        {
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
                        sqlCmd.Parameters.AddWithValue("@fdate", FromDate.Date);
                        sqlCmd.Parameters.AddWithValue("@tdate", ToDate.Date);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                PdfDailyAttendanceReportViewModel model = new PdfDailyAttendanceReportViewModel();
                List<DailyAttendanceReportViewModel> list = new List<DailyAttendanceReportViewModel>();

                foreach (DataRow row in dt.Rows)
                {
                    DailyAttendanceReportViewModel obj = new DailyAttendanceReportViewModel
                    {
                        RowId = row["RowId"].ToString(),
                        Attendance = row["Attendance"].ToString(),
                        InTime = row["InTime"].ToString(),
                        OutTime = row["OutTime"].ToString(),
                        AttStatus = row["AttStatus"].ToString(),
                        MAddress = row["MAddress"].ToString(),
                        MReason = row["MReason"].ToString(),
                        Remarks = row["Remarks"].ToString()
                    };
                    list.Add(obj);
                }

                model.DailyAttendanceReportViewModelList = list;
                model.EmployeeQryProfileAll = await _QRYDataForReportService.EmployeeQryProfileAll(EmployeeId);
                model.FromDate = FromDate.ToString("dd-MMM-yyyy");
                model.ToDate = ToDate.ToString("dd-MMM-yyyy");

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfDailyAttendanceReportByEmployeeId(model);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
                //throw ex;
            }
        }

        [HttpGet]
        [Route("AttendanceReportExportAsExcel")]
        public async Task<IActionResult> AttendanceReportExportAsExcel(DateTime FromDate, DateTime ToDate, long AccountId, long BusinessUnitId)
        {
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

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(EmployeeAttendanceReportViewModel));
                DataTable dataTable = new DataTable();
                foreach (PropertyDescriptor prop in props)
                {
                    if (prop.Name.ToLower() != "LeaveTypeWiseList".ToLower())
                    {
                        //Setting column names as Property names
                        dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                }

                //foreach (EmployeeAttendanceReportViewModel item in attendanceReportList)
                //{
                //    DataRow row = dataTable.NewRow();
                //    foreach (PropertyDescriptor prop in properties)
                //        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                //    table.Rows.Add(row);
                //}

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Attendance Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("RosterDetailsReportExportAsExcel")]
        public async Task<IActionResult> RosterDetailsReportExportAsExcel(long monthId, long yearId, long businessUnitId, string? departmentList, long? designationId, long? workplaceGroupId, long? employmentTypeId, string? srcText)
        {
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
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", workplaceGroupId);
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
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Roster Details Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Roster Details Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("RosterReportExportAsExcel")]
        public async Task<IActionResult> RosterReportExportAsExcel(long? AccountId, long BusinessUnitId, long? WorkplaceGroupId, long? WorkplaceId, long? CalendarId, DateTime? UserDate, long? CalendarTypeId)
        {
            string msg = string.Empty;

            try
            {
                msg += BusinessUnitId <= 0 ? " Invalid BusinessUnit" : " ";

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRosterReportExcel";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkPlaceId", WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@intWorkPlaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intCalendarId", CalendarId);
                        sqlCmd.Parameters.AddWithValue("@dteUserDate", UserDate);
                        sqlCmd.Parameters.AddWithValue("@intCalenderTypeId", CalendarTypeId);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Roster Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Roster Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound(msg);
            }
        }

        [HttpGet]
        [Route("LoanReportAll")]
        public async Task<IActionResult> PdfLoanReportAll(long BusinessUnitId, long WorkplaceGroupId, long? DepartmentId, long? DesignationId, long? EmployeeId, long? LoanTypeId, string? FromDate, string? ToDate, decimal? MinimumAmount, decimal? MaximumAmount, string? ApplicationStatus, string? InstallmentStatus)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                LoanApplicationByAdvanceFilterViewModel obj = new LoanApplicationByAdvanceFilterViewModel
                {
                    AccountId = tokenData.accountId,
                    BusinessUnitId = BusinessUnitId,
                    WorkplaceGroupId = WorkplaceGroupId,
                    LoanTypeId = LoanTypeId,
                    DepartmentId = DepartmentId,
                    DesignationId = DesignationId,
                    EmployeeId = EmployeeId,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    MinimumAmount = MinimumAmount,
                    MaximumAmount = MaximumAmount,
                    ApplicationStatus = ApplicationStatus,
                    InstallmentStatus = InstallmentStatus,
                    Ispaginated = false
                };

                var data = await _employeeService.GetLoanApplicationByAdvanceFilter(obj);
                MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == BusinessUnitId).FirstOrDefaultAsync();

                // ----------------- pdf ----------------------

                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfLoanReportAll(data.Data, BusinessUnit, FromDate, ToDate);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("OvertimeReportExportAsExcel")]
        public async Task<IActionResult> OvertimeReportExportAsExcel(string? PartType, long AccountId, long BusinessUnitId, long? WorkplaceGroupId, long? WorkplaceId, long? DepartmentId, long? DesignationId, long? EmployeeId, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprOvertimeReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@partType", PartType);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceId", WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@intDepartmentId", DepartmentId);
                        sqlCmd.Parameters.AddWithValue("@intDesignationId", DesignationId);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                // process data
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

                    dataList.Add(overtime);
                }

                var groupDataSet = dataList.GroupBy(x => x.intEmployeeId).Select(x => new OvertimeReport
                {
                    intEmployeeId = x.FirstOrDefault()?.intEmployeeId,
                    strEmployeeCode = x.FirstOrDefault()?.strEmployeeCode,
                    strEmployeeName = x.FirstOrDefault()?.strEmployeeName + " [" + x.FirstOrDefault()?.strEmployeeCode + "]",
                    strDesignationName = x?.FirstOrDefault()?.strDesignationName,
                    strDepartmentName = x?.FirstOrDefault()?.strDepartmentName,
                    EmployementType = x?.FirstOrDefault()?.EmployementType,
                    dteOverTimeDate = x?.FirstOrDefault()?.dteOverTimeDate,
                    timeStartTime = x?.FirstOrDefault()?.timeStartTime,
                    timeEndTime = x?.FirstOrDefault()?.timeEndTime,
                    numGrossAmount = x?.FirstOrDefault()?.numBasicSalary,
                    numBasicSalary = x?.FirstOrDefault()?.numBasicSalary,
                    numHours = x?.ToList()?.Sum(x => x.numHours),
                    numPerMinunitRate = x?.FirstOrDefault()?.numPerMinunitRate,
                    numTotalAmount = x?.ToList()?.Sum(x => x.numHours) * x?.FirstOrDefault()?.numPerMinunitRate,
                    strReason = x?.FirstOrDefault()?.strReason
                }).ToList();

                // ----------------- excel ----------------------

                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable d = ConvertListToDataTable.Convert(groupDataSet);

                    wb.Worksheets.Add(d, "Overtime Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Overtime Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Invalid Data");
            }
        }

        [HttpGet]
        [Route("MovementReportExportAsExcel")]
        public async Task<IActionResult> MovementReportExportAsExcel(long? AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? MovementTypeId, long? EmployeeId, DateTime FromDate, DateTime ToDate, string? applicationStatus)
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
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
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

                    foreach (DataRow row in dt.Rows)
                    {
                        data.Add(new MovementReportViewModel()
                        {
                            AutoId = row["AutoId"].ToString(),
                            EmployeeId = row["EmployeeId"].ToString(),
                            EmployeeName = row["EmployeeName"].ToString(),
                            DepartmentId = row["DepartmentId"].ToString(),
                            DepartmentName = row["DepartmentName"].ToString(),
                            DesignationId = row["DesignationId"].ToString(),
                            DesignationName = row["DesignationName"].ToString(),
                            EmployemntTypeId = row["EmployemntTypeId"].ToString(),
                            EmploymentTypeName = row["EmploymentTypeName"].ToString(),
                            Duration = row["Duration"].ToString()
                        });
                    }
                }

                // ----------------- excel ----------------------

                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable d = ConvertListToDataTable.Convert(data);

                    wb.Worksheets.Add(d, "All Employee Movement Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "All Employee Movement Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("MovementReport")]
        public async Task<IActionResult> PdfMovementReport(long? BusinessUnitId, long? WorkplaceGroupId, DateTime FromDate, DateTime ToDate, string? SearchText)
        {
            try
            {
                List<MovementReportViewModel> data = new List<MovementReportViewModel>();
                MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == BusinessUnitId).AsNoTracking().FirstOrDefaultAsync();

                string date = FromDate.ToString("dd-MMMM-yyyy") + " To " + ToDate.ToString("dd-MMMM-yyyy");

                //using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                //{
                //    string sql = "saas.sprMovementDetailsReport";
                //    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                //    {
                //        sqlCmd.CommandType = CommandType.StoredProcedure;

                //        sqlCmd.Parameters.AddWithValue("@dteUserFromDate", FromDate.Date);
                //        sqlCmd.Parameters.AddWithValue("@dteUserToDate", ToDate.Date);
                //        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                //        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                //        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                //        sqlCmd.Parameters.AddWithValue("@intDeptId", DeptId);
                //        sqlCmd.Parameters.AddWithValue("@intDesigId", DesigId);
                //        sqlCmd.Parameters.AddWithValue("@intMovementTypeId", MovementTypeId);
                //        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                //        sqlCmd.Parameters.AddWithValue("@applicationStatus ", applicationStatus);

                //        connection.Open();
                //        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                //        {
                //            sqlAdapter.Fill(dt);
                //        }
                //        connection.Close();
                //    }

                //    foreach (DataRow row in dt.Rows)
                //    {
                //        data.Add(new MovementReportViewModel()
                //        {
                //            AutoId = row["AutoId"].ToString(),
                //            EmployeeId = row["EmployeeId"].ToString(),
                //            EmployeeName = row["EmployeeName"].ToString(),
                //            DepartmentId = row["DepartmentId"].ToString(),
                //            DepartmentName = row["DepartmentName"].ToString(),
                //            DesignationId = row["DesignationId"].ToString(),
                //            DesignationName = row["DesignationName"].ToString(),
                //            EmployemntTypeId = row["EmployemntTypeId"].ToString(),
                //            EmploymentTypeName = row["EmploymentTypeName"].ToString(),
                //            Duration = row["Duration"].ToString()
                //        });
                //    }
                //}
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var retObj = await _employeeService.EmployeeMovementReportAll((long)BusinessUnitId, (long)WorkplaceGroupId, FromDate, ToDate, 0, 0, SearchText, false, tokenData);
                //-----------------pdf----------------------
                long id = 1;
                foreach (var item in retObj.Data)
                {
                    data.Add(new MovementReportViewModel()
                    {
                        AutoId = id.ToString(),
                        EmployeeId = item.EmployeeBasicInfoId.ToString(),
                        EmployeeName = item.EmployeeName,
                        DepartmentId = item.DepartmentId.ToString(),
                        DepartmentName = item.DepartmentName,
                        DesignationId = item.DesignationId.ToString(),
                        DesignationName = item.DesignationName,
                        EmployemntTypeId = item.EmploymentTypeId.ToString(),
                        EmploymentTypeName = item.EmploymentType,
                        Duration = item.RawDuration.ToString(),

                    });
                    id++;
                }
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfMovementReport(data, BusinessUnit, date);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("LoanReportDetails")]
        public async Task<IActionResult> PdfLoanReportDetails(long LoanApplicationId, long BusinessUintId, long WorkplaceGroupId, long EmployeeId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUintId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != EmployeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, BusinessUintId, WorkplaceGroupId, EmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }

                LoanDetailsReportViewModel model = await _pdfAndExcelService.LoanDetailsByLoanId(LoanApplicationId);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfLoanDetailsReportByEmployee(model);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("LeaveHistoryReport")]
        public async Task<IActionResult> PdfLeaveHistoryReportByEmployee(long EmployeeId, string? fromDate, string? toDate)
        {
            try
            {
                LeaveHistoryViewModel model = new LeaveHistoryViewModel();
                model.EmployeeDetailsViewModel = await _employeeService.GetEmployeeDetailsByEmployeeId(EmployeeId);
                model.BusinessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == model.EmployeeDetailsViewModel.EmployeeInfoObj.IntBusinessUnitId).FirstOrDefaultAsync();
                model.LeaveApplicationList = await _employeeService.GetAllApprovedLeaveApplicationListByEmployee(EmployeeId, fromDate, toDate);

                model.FromDate = !string.IsNullOrEmpty(fromDate) ? Convert.ToDateTime(fromDate)
                    : model.LeaveApplicationList.Count() <= 0 ? DateTime.Now
                    : model.LeaveApplicationList.OrderBy(x => x.LeaveApplication?.DteApplicationDate)?.FirstOrDefault()?.LeaveApplication?.DteApplicationDate;

                model.ToDate = !string.IsNullOrEmpty(toDate) ? Convert.ToDateTime(toDate)
                    : model.LeaveApplicationList.Count() <= 0 ? DateTime.Now
                    : model.LeaveApplicationList.OrderByDescending(x => x.LeaveApplication?.DteApplicationDate)?.FirstOrDefault()?.LeaveApplication?.DteApplicationDate;

                // ----------------- pdf ----------------------

                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfLeaveHistoryByEmployee(model);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Wrong");
            }
        }

        [HttpGet]
        [Route("EmployeePaySlipReport")]
        public async Task<IActionResult> EmployeePaySlipReport(string partName, long intEmployeeId, long intMonthId, long intYearId, long intSalaryGenerateRequestId)
        {
            try
            {
                EmployeeSalaryPaySlipViewModel model = await _pdfAndExcelService.PdfPaySlipReportByEmployeeId(partName, intEmployeeId, intMonthId, intYearId, intSalaryGenerateRequestId);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfSalaryPaySlipReportByEmployee(model, "paySlip");
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("EmployeeSalaryCertificate")]
        public async Task<IActionResult> EmployeeSalaryCertificate(string partName, long intEmployeeId, long intMonthId, long intYearId, long intSalaryGenerateRequestId)
        {
            try
            {
                EmployeeSalaryPaySlipViewModel model = await _pdfAndExcelService.PdfPaySlipReportByEmployeeId(partName, intEmployeeId, intMonthId, intYearId, intSalaryGenerateRequestId);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfSalaryPaySlipReportByEmployee(model, "salaryCertificate");
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("SeparationReportByEmployee")]
        public async Task<IActionResult> PdfSeparationReportByEmployee(long EmployeeId)
        {
            try
            {
                EmployeeSeparationReportViewModel SeperationObj = await _employeeService.SeperationReportByEmployeeId(EmployeeId);

                MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == SeperationObj.EmployeeInfoObj.IntBusinessUnitId && x.IsActive == true).FirstOrDefaultAsync();
                EmpEmployeeBasicInfo employeeBasicInfo = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == EmployeeId).FirstOrDefault();
                SeperationObj.pfAccountViewModel = await _employeeService.PfNGratuityCount(employeeBasicInfo);

                // ---------------- pdf -------------------------

                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfSeparationReportByEmployee(businessUnit, SeperationObj);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("EmployeeSeparationExportAsExcel")]
        public async Task<IActionResult> EmployeeSeparationExportAsExcel(string strPartName, string? Status, long AccountId, long BusinessUnitId, long? WorkplaceGroupId, long? DepartmentId, long? DesignationId, long? SupervisorId, long? EmployeeId, long? SeparationTypeId, DateTime? ApplicationFromDate, DateTime? ApplicationToDate)
        {
            try
            {
                EmployeeSeparationListFilterViewModel obj = new EmployeeSeparationListFilterViewModel
                {
                    TableName = strPartName,
                    Status = Status,
                    AccountId = AccountId,
                    BusinessUnitId = BusinessUnitId,
                    WorkplaceGroupId = WorkplaceGroupId,
                    DepartmentId = DepartmentId,
                    DesignationId = DesignationId,
                    SupervisorId = SupervisorId,
                    EmployeeId = EmployeeId,
                    SeparationTypeId = SeparationTypeId,
                    ApplicationFromDate = ApplicationFromDate,
                    ApplicationToDate = ApplicationToDate,
                };

                var jsonString = JsonSerializer.Serialize(obj);
                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprAllLandingPageData";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@strTableName", obj.TableName);
                sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                sqlCmd.Parameters.AddWithValue("@intId", 0);
                sqlCmd.Parameters.AddWithValue("@intStatusId", 0);
                sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
                connection.Open();

                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                {
                    sqlAdapter.Fill(dt);
                }
                connection.Close();

                // ----------------- excel ----------------------

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Employee Separation Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Separation Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Invalid Data");
            }
        }

        [HttpGet]
        [Route("AllTypeOfSalaryReport")]
        public async Task<IActionResult> AllTypeOfSalaryReport(string StrPartName, long IntAccountId, long IntBusinessUnitId, long IntMonthId, long IntYearId, long IntSalaryGenerateRequestId, int IntBankOrWalletType)
        {
            try
            {
                SalaryAllReportViewModel model = await _pdfAndExcelService.PdfSalaryReportAllType(StrPartName, IntAccountId, IntBusinessUnitId, IntMonthId, IntYearId, IntSalaryGenerateRequestId, IntBankOrWalletType);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfAllTypeOfSalaryReport(model, IntBankOrWalletType);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("BankAdviceReportPDF")]
        public async Task<IActionResult> BankAdviceReportPDF(string partName, long intAccountId, long? intBusinessUnitId, long intMonthId, long intYearId,
            long? intWorkPlaceId, long? intWorkplaceGroupId, long? intPayrollGroupId, long? intSalaryGenerateRequestId, string? BankAccountNo, int? intBankOrWalletType, long? intEmployeeId, string? strAdviceType)
        {
            try
            {
                BankAdviceViewModel model = await _pdfAndExcelService.PdfBankAdviceReport(partName, intAccountId, intBusinessUnitId, intMonthId, intYearId, intWorkPlaceId, intWorkplaceGroupId, intPayrollGroupId,
                    intSalaryGenerateRequestId, BankAccountNo, intBankOrWalletType, intEmployeeId, strAdviceType);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfBankAdviceReport(model);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        [HttpGet]
        [Route("IncrementLetterReportPDF")]
        public async Task<IActionResult> IncrementLetterReportPDF(long intIncrementId, long IntAccountId, long? IntEmployeeId)
        {
            try
            {
                IncrementLetterViewModel incrementLetterView = await _employeeService.IncrementLetterForPdf(intIncrementId, IntAccountId);
                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfIncrementLetterReport(incrementLetterView);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("TransferAndPromotionReportPDF")]
        public async Task<IActionResult> TransferAndPromotionReportPDF(long IntTransferAndPromotionId, long IntAccountId, long? IntEmployeeId)
        {
            try
            {
                TransferAndPromotionReportPDFViewModel transferAndPromotionReportPDFView = await _employeeService.TransferAndPromotionReportForPdf(IntTransferAndPromotionId, IntAccountId);
                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfTransferAndPromotionReport(transferAndPromotionReportPDFView);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("TransferReportPDF")]
        public async Task<IActionResult> TransferReportPDF(long intTransferId, long IntAccountId, long? IntEmployeeId)
        {
            try
            {
                TransferReportViewModel transferReportViewModel = await _employeeService.TransferReport(intTransferId, IntAccountId);
                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfTransferReport(transferReportViewModel);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("PromotionReportPDF")]
        public async Task<IActionResult> PromotionReportPDF(long IntTransferNPromotionId, long AccountId, long? IntEmployeeId)
        {
            try
            {
                PromotionReportViewModel model = await _employeeService.PromotionReportForPdf(IntTransferNPromotionId, AccountId);
                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfPromotionReport(model);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SalaryDetailsReport")]
        public async Task<IActionResult> PdfSalaryDetailsReport(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long intMonthId, long intYearId, string? strSalaryCode, bool isDownload)
        {
            try
            {
                SalaryMasyterVM result = await _pdfAndExcelService.PdfSalaryDetailsReport(intAccountId, intBusinessUnitId, intWorkplaceGroupId, intMonthId, intYearId, strSalaryCode);
                MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == intBusinessUnitId && x.IsActive == true).FirstOrDefaultAsync();

                if (isDownload == true)
                {
                    //return await DownloadExcel.SalaryDetailsExcellReport(result, businessUnit);

                    XLWorkbook wb = await DownloadExcel.SalaryDetailsExcellReport(result, businessUnit);

                    //wb.Worksheets.Add(dt, $"MonthlySalary_{result.Month}.xlsx");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MonthlySalary_{result.Month}.xlsx");
                    }

                    //using (XLWorkbook wb = new XLWorkbook())
                    //{
                    //    wb.Worksheets.Add(dt, "Employee Separation Report");
                    //    using (MemoryStream stream = new MemoryStream())
                    //    {
                    //        wb.SaveAs(stream);
                    //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Separation Report.xlsx");
                    //    }
                    //}
                }
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfSalaryDetailsReport(result, businessUnit, (long)intWorkplaceGroupId, intMonthId, intYearId);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetSalaryLandingData")]
        public async Task<IActionResult> GetSalaryLandingData(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long intMonthId, long intYearId, string? strSalaryCode)
        {
            using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
            var obj = (await connection.QueryAsync<dynamic>("EXEC saas.sprPyrSalaryDetailsReport_RDLC @intAccountId, @intBusinessUnitId,@intWorkplaceGroupId, @intMonthId, @intYearId, @strSalaryCode", new { intAccountId, intBusinessUnitId, intWorkplaceGroupId, intMonthId, intYearId, strSalaryCode })).ToList();

            var data = _iReportrdlc.GetHTML("PeopleDesk.Reports.Rdlc.SalaryDetailsLanding.rdlc", obj, "SalaryData", "HTML5");
            return File(data, "text/html", "report.html");
        }

        [HttpGet]
        [Route("EmpBonusReportPdf")]
        public async Task<IActionResult> EmpBonusReportPdf(string PartType, long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long BonusHeaderId, long BonusId)
        {
            try
            {
                BonusReportVM result = await _pdfAndExcelService.PdfBonusReport(PartType, intAccountId, intBusinessUnitId, intWorkplaceGroupId, BonusHeaderId, BonusId);

                MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == intBusinessUnitId && x.IsActive == true).FirstOrDefaultAsync();



                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.PdfBonusReport(result, businessUnit);

                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #region ================== BankAdviceReportForIBBL =======================
        [HttpGet]
        [Route("BankAdviceReportForIBBL")]
        public async Task<IActionResult> BankAdviceReportForIBBL(long MonthId, long SalaryGenerateRequestId, long YearId, long IntAccountId, long IntBusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                BankAdviceReportForIBBLViewModel model = await _employeeService.BankAdviceReportForIBBL(MonthId, SalaryGenerateRequestId, YearId, IntAccountId, IntBusinessUnitId, WorkplaceGroupId);


                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.BankAdviceReportForIBBL(model);
                //HtmlToPdfDocument htmlPdf = new HtmlToPdfDocument();
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");

            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }

        #endregion ================ BankAdviceReportForIBBL ====================

        #region ================== BankAdviceReportForBEFTN =======================
        [HttpGet]
        [Route("BankAdviceReportForBEFTN")]
        public async Task<IActionResult> BankAdviceReportForBEFTN(long MonthId, long SalaryGenerateRequestId, long YearId, long IntAccountId, long IntBusinessUnitId, long WorkplaceGroupId)
        {
            try
            {

                BankAdviceReportForBEFTNViewModel model = await _employeeService.BankAdviceReportForBEFTN(MonthId, SalaryGenerateRequestId, YearId, IntAccountId, IntBusinessUnitId, WorkplaceGroupId);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.BankAdviceReportForBEFTN(model);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");

            }
            catch (Exception ex)
            {
                return NotFound("Something Went Worng");
            }
        }
        #endregion ================ BankAdviceReportForBEFTN ====================

        #region ============= Daily Attendance Report PDF ==============
        [HttpGet]
        [Route("DailyAttendanceReportPDF")]
        public async Task<IActionResult> DailyAttendanceReportPDF(long IntAccountId, long IntBusinessUnitId, long IntWorkplaceGroupId, DateTime attendanceDate)
        {
            //long IntAccountId, DateTime AttendanceDate, long IntBusinessUnitId, long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntDepartmentId, string? EmployeeIdList
            try
            {
                //new
                bool IsXls = false; int PageNo = 0; int PageSize = 0; string? searchTxt = "";
                EmployeeDaylyAttendanceReportLanding dailyAttendanceReport = await _employeeService.GetDateWiseAttendanceReport(IntAccountId, IntBusinessUnitId, IntWorkplaceGroupId, attendanceDate, IsXls, PageNo, PageSize, searchTxt);

                //sp call
                // DailyAttendanceReportVM dailyAttendanceReport = await _employeeService.DailyAttendanceReport(IntAccountId, AttendanceDate, IntBusinessUnitId, IntWorkplaceGroupId, IntWorkplaceId, IntDepartmentId, EmployeeIdList, null, null, null, false);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.DailyAttendanceReportPDF(dailyAttendanceReport);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf", "Daily Attendance Report.pdf");
            }
            catch (Exception ex)
            {

                return NotFound("Something Went Worng");
            }
        }
        #endregion

        #region ====== Monthly Attendance Report ======
        [HttpGet]
        [Route("GetMonthlyAttenReportData")]
        public async Task<IActionResult> GetMonthlyAttenReportData(long intAccountId, long businessUnitId, DateTime fromDate, DateTime toDate, long WorkPlaceGroup)
        {
            using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
            var obj = (await connection.QueryAsync<dynamic>("EXEC saas.sprMonthlyAttendanceReportRDLC @intAccountId,@businessUnitId, @fromDate, @toDate, @WorkPlaceGroup", new { intAccountId, businessUnitId, fromDate, toDate, WorkPlaceGroup })).ToList();

            var data = _iReportrdlc.GetPDF("PeopleDesk.Reports.Rdlc.MonthlyAttendanceReport.rdlc", obj, "MonthlyAttendanceReport", "PDF", 0);
            return File(data, "application/pdf", "report.pdf");
        }
        [HttpGet]
        [Route("GetMonthlyAttenReportExcel")]
        public async Task<IActionResult> GetMonthlyAttenReportExcel(long accountId, long businessUnitId, DateTime fromDate, DateTime toDate, long WorkPlaceGroup)
        {
            using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
            var obj = (await connection.QueryAsync<dynamic>("EXEC saas.sprMonthlyAttendanceReportRDLC @accountId,@businessUnitId, @fromDate, @toDate, @WorkPlaceGroup", new { accountId, businessUnitId, fromDate, toDate, WorkPlaceGroup })).ToList();

            var data = _iReportrdlc.GetXLSX("PeopleDesk.Reports.Rdlc.MonthlyAttendanceReport.rdlc", obj, "MonthlyAttendanceReport", "EXCELOPENXML");
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MonthlyAttendanceReport_"+ fromDate.ToString("dd-MMM-yyyy") + "_TO_" + toDate.ToString("dd-MMM-yyyy") + ".xlsx");
        }

        [HttpGet]
        [Route("GetMonthlyAttenReportPDF")]
        public async Task<IActionResult> GetMonthlyAttenReportPDF(long accountId, long businessUnitId, DateTime fromDate, DateTime toDate, long WorkPlaceGroup)
        {
            using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
            var obj = (await connection.QueryAsync<dynamic>("EXEC saas.sprMonthlyAttendanceReportRDLC @accountId,@businessUnitId, @fromDate, @toDate, @WorkPlaceGroup", new { accountId, businessUnitId, fromDate, toDate, WorkPlaceGroup })).ToList();

            var data = _iReportrdlc.GetPDF("PeopleDesk.Reports.Rdlc.MonthlyAttendanceReport.rdlc", obj, "MonthlyAttendanceReport", "PDF",0);
            return File(data, "application/pdf", "MonthlyAttendanceReport_" + fromDate.ToString("dd-MM-yyyy") + "_TO_" + toDate.ToString("dd-MM-yyyy") + ".pdf");
        }

        #endregion

        #region ============= Salary Assign Report ===================
        [HttpGet]
        [Route("GetSalaryAssignReportRDLC")]
        public async Task<IActionResult> GetSalaryAssignReportRDLC(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, string ReportType)
        {
            byte[] data = null;

            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                var obj = (await connection.QueryAsync<dynamic>("EXEC saas.sprPyrSalaryAssignReport_RDLC @intAccountId, @intBusinessUnitId, @intWorkplaceGroupId", new { intAccountId, intBusinessUnitId, intWorkplaceGroupId })).ToList();

                if (intWorkplaceGroupId == 2)
                {
                    if (ReportType.ToLower() == "excel")
                    {
                        data = _iReportrdlc.GetXLSX("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForHOExcel.rdlc", obj, "SalaryAssignReportRDLC", "EXCELOPENXML");
                    }
                    else if (ReportType.ToLower() == "pdf")
                    {
                        data = _iReportrdlc.GetPDF("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForHO.rdlc", obj, "SalaryAssignReportRDLC", "PDF", 0);
                    }
                    else if (ReportType.ToLower() == "html")
                    {
                        data = _iReportrdlc.GetHTML("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForHO.rdlc", obj, "SalaryAssignReportRDLC", "HTML5");
                    }
                }
                else if (intWorkplaceGroupId == 3)
                {
                    if (ReportType.ToLower() == "excel")
                    {
                        data = _iReportrdlc.GetXLSX("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForMExcel.rdlc", obj, "SalaryAssignReportRDLC", "EXCELOPENXML");
                    }
                    else if (ReportType.ToLower() == "pdf")
                    {
                        data = _iReportrdlc.GetPDF("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForM.rdlc", obj, "SalaryAssignReportRDLC", "PDF", 0);
                    }
                    else if (ReportType.ToLower() == "html")
                    {
                        data = _iReportrdlc.GetHTML("PeopleDesk.Reports.Rdlc.SalaryAssignLandingForM.rdlc", obj, "SalaryAssignReportRDLC", "HTML5");
                    }
                }

                if (ReportType.ToLower() == "excel")
                {
                    return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalaryAssignReport.xlsx");
                }
                else if (ReportType.ToLower() == "pdf")
                {
                    return File(data, "application/pdf", "report.pdf");
                }

                return File(data, "text/html", "report.html");
            }
        }

        #endregion

        #region ============= Salary Tax Certificate Report PDF ==============
        [HttpGet]
        [Route("SalaryTaxCertificateReportPDF")]
        public async Task<IActionResult> SalaryTaxCertificateReportPDF(long FiscalYearId, string FiscalYear, long EmployeeId)
        {
            try
            {
                SalaryTaxCertificateViewModel salaryTaxCertificate = await _employeeService.SalaryTaxCertificate(FiscalYearId, FiscalYear, EmployeeId);

                // ---------------- pdf -------------------------
                HtmlToPdfDocument htmlPdf = await _pdfAndExcelService.SalaryTaxCertificate(salaryTaxCertificate);
                var file = _converter.Convert(htmlPdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {

                return NotFound("Something Went Worng");
            }
        }
        #endregion
    }


}