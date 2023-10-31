using Azure.Storage.Blobs.Models;
using ClosedXML.Excel;
using Dapper;
using DinkToPdf;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Global.Interface;
using System.Data;
using System.Globalization;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Services.Global
{
    public class PdfAndExcelService : IPdfAndExcelService
    {
        private readonly IPdfTemplateGeneratorService _pdfTemplateGeneratorService;

        //private readonly IShippingMgmt _ShippingMgmt;
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly string DailyAttendanceReportCSSUrl;
        private readonly string SeparationReportByEmployeeCSSUrl;
        private readonly string LoanDetailsReportCSSUrl;
        private readonly string LoanReportAllCSSUrl;
        private readonly string LeaveHistoryCSSUrl;
        private readonly string MonthlySalaryReportCSSUrl;
        private readonly string MonthlySalaryReportDeptWiseCSSUrl;
        private readonly string EmployeeBasedSalaryReportCSSUrl;
        private readonly string RosterReportCSSUrl;
        private readonly string MovementReportCSSUrl;
        private readonly string ShippingResumeCSSUrl;
        private readonly string ShippingPortageCSSUrl;
        private readonly string ShippingEmployeeRandCSSUrl;
        private readonly string ShippingEmployeeSignOffCSSUrl;
        private readonly string ShippingEmployeeMedicalAppCSSUrl;
        private readonly string ShippingPortageBillCSSUrl;
        private readonly string ShippingHomeAttotmentLocalCSSUrl;
        private readonly string ShippingHomeAttotmentForeginCSSUrl;
        private readonly string BondInventoryStatementCSSUrl;
        private readonly string VsetCallingReportCSSUrl;
        private readonly string BondReportCSSUrl;
        private readonly string BondAccReportCSSUrl;
        private readonly string CashAccReportCSSUrl;
        private readonly string MedicalApplicationCSSUrl;
        private readonly string SignOffApplicationCSSUrl;
        private readonly string CrewReportCSSUrl;
        private readonly string SalaryDetailsReportCSSUrl;
        private readonly string BonusReportCSSUrl;
        private readonly PeopleDeskContext _contex;

        private DataTable dt = new DataTable();

        public PdfAndExcelService(PeopleDeskContext _contex, IWebHostEnvironment hostingEnvironment, IPdfTemplateGeneratorService _pdfTemplateGeneratorService/*, IShippingMgmt _ShippingMgmt*/)
        {
            this.hostingEnvironment = hostingEnvironment;
            //this._ShippingMgmt = _ShippingMgmt;
            this._pdfTemplateGeneratorService = _pdfTemplateGeneratorService;
            this.DailyAttendanceReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/attendance_report.css";
            this.SeparationReportByEmployeeCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/seperation_report.css";
            this.LoanDetailsReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/loan_details_report.css";
            this.LoanReportAllCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/loan_report_all.css";
            this.LeaveHistoryCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/leave_history.css";
            this.MonthlySalaryReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/monthly_salary.css";
            this.MonthlySalaryReportDeptWiseCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/dept_wise_salary_report.css";
            this.EmployeeBasedSalaryReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/single_emp_salary_report.css";
            this.RosterReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/roster_report.css";
            this.MovementReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/movement_report.css";
            this.ShippingResumeCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/shipping_resume.css";
            this.ShippingPortageCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/shipping_portage.css";
            this.ShippingEmployeeRandCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/employee_rank.css";
            this.ShippingEmployeeSignOffCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/shipping_signoff.css";
            this.ShippingEmployeeMedicalAppCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/shipping_medical.css";
            this.ShippingPortageBillCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/portage_bill.css";
            this.ShippingHomeAttotmentLocalCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/home_attotment_local.css";
            this.ShippingHomeAttotmentForeginCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/home_attotment_foregin.css";
            this.BondInventoryStatementCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/Bond_Inventory_Statement.css";
            this.VsetCallingReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/Tel_Account.css";
            this.BondReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/Bond_Report.css";
            this.BondAccReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/Bond_Account.css";
            this.CashAccReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/cash_account_report.css";
            this.MedicalApplicationCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/medical_application.css";
            this.SignOffApplicationCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/sign_off_application.css";
            this.CrewReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/crew_report.css";
            this.SalaryDetailsReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/SalaryReport.css";
            this.BonusReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/bonusReport.css";
            this._contex = _contex;
        }

        public async Task<HtmlToPdfDocument> PdfDailyAttendanceReportByEmployeeId(PdfDailyAttendanceReportViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Attendance Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.DailyAttendanceReportByEmployeeId(obj),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = DailyAttendanceReportCSSUrl },
                    //HeaderSettings = new HeaderSettings { Spacing = 0},
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfSeparationReportByEmployee(MasterBusinessUnit businessUnit, EmployeeSeparationReportViewModel SeperationObj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Separation Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.SeparationReportByEmployee(businessUnit, SeperationObj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = SeparationReportByEmployeeCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfLoanDetailsReportByEmployee(LoanDetailsReportViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Loan Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.LoanDetailsReportByEmployee(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfLoanReportAll(List<LoanReportByAdvanceFilterViewModel> obj, MasterBusinessUnit BusinessUnit, string? FromDate, string? ToDate)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Loan Report ALL",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.LoanReportAll(obj, BusinessUnit, FromDate, ToDate),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanReportAllCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfLeaveHistoryByEmployee(LeaveHistoryViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Leave History Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.LeaveReportByEmployee(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LeaveHistoryCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfMonthlySalaryReport(MonthlySalaryReportViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Monthly Salary Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.MonthlySalaryReport(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = MonthlySalaryReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfEmployeeBasedSalaryReport(EmployeePayslipViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Employee Based Salary Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.EmployeeBasedSalaryReport(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = EmployeeBasedSalaryReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfMonthlySalaryDepartmentWiseReport(List<MonthlySalaryDepartmentWiseReportViewModel> obj, MasterBusinessUnit BusinessUnit, string monthName)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Monthly Department Wise Salary Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.MonthlySalaryDepartmentWiseReport(obj, BusinessUnit, monthName),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = MonthlySalaryReportDeptWiseCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfRosterReport(List<RosterReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Roster Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.RosterReport(obj, BusinessUnit, date),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = RosterReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfMovementReport(List<MovementReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Movement Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.MovementReport(obj, BusinessUnit, date),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = MovementReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> PdfGetAllLeaveApplicatonListForApprove(List<LeaveDataSetViewModel> model)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Leave Approval Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.LeaveApplicationApprovalReport(model),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = MovementReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> BanglaPdfGenerate()
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Leave Approval Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.BanglaPdfGenerate(),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = MovementReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<LoanDetailsReportViewModel> LoanDetailsByLoanId(long? loanId)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprAllLandingPageData";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strTableName", "LoanApplicationById");
                        sqlCmd.Parameters.AddWithValue("@intAccountId", 0);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", 0);
                        sqlCmd.Parameters.AddWithValue("@intId", loanId);
                        sqlCmd.Parameters.AddWithValue("@intStatusId", 0);
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", "");
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", null);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", null);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        LoanDetailsReportViewModel model = new LoanDetailsReportViewModel();

                        model.loanApplicationId = dt.Rows[0]["loanApplicationId"].ToString();
                        model.employeeId = dt.Rows[0]["employeeId"].ToString();
                        model.loanTypeId = dt.Rows[0]["loanTypeId"].ToString();
                        model.loanAmount = dt.Rows[0]["loanAmount"].ToString();
                        model.numberOfInstallment = dt.Rows[0]["numberOfInstallment"].ToString();
                        model.numberOfInstallmentAmount = dt.Rows[0]["numberOfInstallmentAmount"].ToString();
                        model.applicationDate = dt.Rows[0]["applicationDate"].ToString();
                        model.isApprove = dt.Rows[0]["isApprove"].ToString();
                        model.approveBy = dt.Rows[0]["approveBy"].ToString();
                        model.approveDate = dt.Rows[0]["approveDate"].ToString();
                        model.approveLoanAmount = dt.Rows[0]["approveLoanAmount"].ToString();
                        model.approveNumberOfInstallment = dt.Rows[0]["approveNumberOfInstallment"].ToString();
                        model.approveNumberOfInstallmentAmount = dt.Rows[0]["approveNumberOfInstallmentAmount"].ToString();
                        model.remainingBalance = dt.Rows[0]["remainingBalance"].ToString();
                        model.effectiveDate = dt.Rows[0]["effectiveDate"].ToString();
                        model.description = dt.Rows[0]["description"].ToString();
                        model.fileUrl = dt.Rows[0]["fileUrl"].ToString();
                        model.referenceNo = dt.Rows[0]["referenceNo"].ToString();
                        model.isActive = dt.Rows[0]["isActive"].ToString();
                        model.isReject = dt.Rows[0]["isReject"].ToString();
                        model.rejectBy = dt.Rows[0]["rejectBy"].ToString();
                        model.rejectDate = dt.Rows[0]["rejectDate"].ToString();
                        model.insertByUserId = dt.Rows[0]["insertByUserId"].ToString();
                        model.insertDateTime = dt.Rows[0]["insertDateTime"].ToString();
                        model.updateByUserId = dt.Rows[0]["updateByUserId"].ToString();
                        model.updateDateTime = dt.Rows[0]["updateDateTime"].ToString();
                        model.employeeName = dt.Rows[0]["employeeName"].ToString();
                        model.reScheduleCount = dt.Rows[0]["reScheduleCount"].ToString();
                        model.reScheduleNumberOfInstallment = dt.Rows[0]["reScheduleNumberOfInstallment"].ToString();
                        model.reScheduleNumberOfInstallmentAmount = dt.Rows[0]["reScheduleNumberOfInstallmentAmount"].ToString();
                        model.reScheduleDateTime = dt.Rows[0]["reScheduleDateTime"].ToString();
                        model.employeeCode = dt.Rows[0]["employeeCode"].ToString();
                        model.departmentName = dt.Rows[0]["departmentName"].ToString();
                        model.designationName = dt.Rows[0]["designationName"].ToString();
                        model.positonGroupName = dt.Rows[0]["positonGroupName"].ToString();
                        model.positionName = dt.Rows[0]["positionName"].ToString();
                        model.loanType = dt.Rows[0]["loanType"].ToString();
                        model.paidAmount = dt.Rows[0]["paidAmount"].ToString();
                        model.dueInstallment = dt.Rows[0]["dueInstallment"].ToString();
                        model.paidInstallment = dt.Rows[0]["paidInstallment"].ToString();
                        model.applicationStatus = dt.Rows[0]["applicationStatus"].ToString();
                        model.installmentStatus = dt.Rows[0]["installmentStatus"].ToString();
                        model.businessUnitId = dt.Rows[0]["businessUnitId"].ToString();
                        model.employmentTypeName = dt.Rows[0]["employmentTypeName"].ToString();
                        model.joiningDate = dt.Rows[0]["joiningDate"].ToString();

                        model.BusinessUnit = await _contex.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == Convert.ToInt32(dt.Rows[0]["businessUnitId"].ToString())).AsNoTracking().FirstOrDefaultAsync();
                        model.EmployeeSalaryDefault = await _contex.PyrEmployeeSalaryDefaults.Where(x => x.IntEmployeeId == Convert.ToInt32(dt.Rows[0]["employeeId"].ToString())).AsNoTracking().FirstOrDefaultAsync();
                        model.LoanReScheduleList = await _contex.EmpLoanSchedules.Where(x => x.IntLoanApplicationId == loanId && x.IsActive == true).ToListAsync();

                        return model;
                    }
                    else
                    {
                        throw new Exception("inalid data");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<EmployeeSalaryPaySlipViewModel> PdfPaySlipReportByEmployeeId(string partName, long intEmployeeId, long intMonthId, long intYearId, long intSalaryGenerateRequestId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.sprPyrSalarySelectQueryAll";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@strPartName", partName);
                sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
                sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", intSalaryGenerateRequestId);

                connection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                sqlAdapter.Fill(dt);
                connection.Close();
            }

            DataTable dt2 = new DataTable();
            using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.sprPyrSalarySelectQueryAll";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@strPartName", "SalaryPaySlipByEmployeeId");
                sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
                sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", intSalaryGenerateRequestId);
                connection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                sqlAdapter.Fill(dt2);
                connection.Close();
            }

            try
            {
                List<PaySlipViewModel> paySlipList = new List<PaySlipViewModel>();
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow row in dt2.Rows)
                    {
                        PaySlipViewModel paySlip = new PaySlipViewModel();
                        paySlip.PayrollElementId = row["intPayrollElementId"].ToString();
                        paySlip.PayrollElement = row["strPayrollElement"].ToString();
                        paySlip.PayrollElementTypeId = row["intPayrollElementTypeId"].ToString();
                        paySlip.NumAmount = Convert.ToDecimal(row["numAmount"]);
                        paySlip.Arrear = Convert.ToDecimal(row["numArrear"]);
                        paySlip.Total = Convert.ToDecimal(row["numTotal"]);

                        paySlipList.Add(paySlip);
                    }
                }
                else
                {
                    throw new Exception("inalid data");
                }

                EmployeeSalaryPaySlipViewModel model = new EmployeeSalaryPaySlipViewModel();
                if (dt.Rows.Count > 0)
                {
                    model.GeneratDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");
                    model.EmployeeId = dt.Rows[0]["intEmployeeId"].ToString();
                    model.Employee = dt.Rows[0]["strEmployeeName"].ToString();
                    model.EmployeeCode = dt.Rows[0]["strEmployeeCode"].ToString();
                    model.EmploymentType = dt.Rows[0]["strEmploymentType"].ToString();
                    model.JoiningDate = dt.Rows[0]["dteJoiningDate"].ToString();
                    model.DepartmentId = dt.Rows[0]["intDepartmentId"].ToString();
                    model.Department = dt.Rows[0]["strDepartment"].ToString();
                    model.DesignationId = dt.Rows[0]["intDesignationId"].ToString();
                    model.Designation = dt.Rows[0]["strDesignation"].ToString();
                    model.ModeOfPayment = dt.Rows[0]["strPaymentBankType"].ToString();
                    model.strFinancialInstitution = dt.Rows[0]["strFinancialInstitution"].ToString();
                    model.strBankBranchName = dt.Rows[0]["strBankBranchName"].ToString();
                    model.strAccountName = dt.Rows[0]["strAccountName"].ToString();
                    model.strAccountNo = dt.Rows[0]["strAccountNo"].ToString();
                    model.DateOfPayment = Convert.ToDateTime(dt.Rows[0]["dteSalaryGenerateFor"].ToString()).ToString("dd-MMM-yyyy");
                    model.numTaxAmount = Convert.ToDecimal(dt.Rows[0]["numTaxAmount"]);
                    model.numLoanAmount = Convert.ToDecimal(dt.Rows[0]["numLoanAmount"]);
                    model.numPFAmount = Convert.ToDecimal(dt.Rows[0]["numPFAmount"]);
                    model.numPFCompanyAmount = Convert.ToDecimal(dt.Rows[0]["numPFCompany"]);
                    model.AccountId = Convert.ToInt64(dt.Rows[0]["intAccountId"]);
                    model.BusinessUnitName = dt.Rows[0]["strBusinessUnitName"].ToString();
                    model.HRPostionName = _contex.EmpEmployeeBasicInfoDetails.Where(x => x.IsActive == true && x.IntEmployeeId == Convert.ToInt64(model.EmployeeId)).Select(x => x.StrHrpostionName).FirstOrDefault();
                    model.paySlipViewModels = paySlipList;



                    return model;
                }
                else
                {
                    throw new Exception("inalid data");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HtmlToPdfDocument> PdfSalaryPaySlipReportByEmployee(EmployeeSalaryPaySlipViewModel obj, string employeeSalary)
        {
            try
            {
                string docType = employeeSalary == "paySlip" ? "Pay Slip Report" : employeeSalary == "salaryCertificate" ? "Salary Certificate Report" : "";

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = docType,
                };

                string htmlContent = "";
                if (employeeSalary == "paySlip")
                {
                    htmlContent = await _pdfTemplateGeneratorService.SalaryPaySlipReportByEmployee(obj);
                }
                else
                {
                    htmlContent = await _pdfTemplateGeneratorService.SalaryCertificateReportByEmployeeId(obj);
                }

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<SalaryMasyterVM> PdfSalaryDetailsReport(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long intMonthId, long intYearId, string? strSalaryCode)
        {
            DataTable dt = new DataTable();
            try
            {
                using var connection = new SqlConnection(Connection.iPEOPLE_HCM);              

                List<SalaryDetailsReportVM> salaryReportList = (await connection.QueryAsync<SalaryDetailsReportVM>("EXEC [saas].[sprGeneratedSalaryLandingForDetailsReport] @intAccountId, @intBusinessUnitId, @intWorkplaceGroupId, @strSalaryCode", new { intAccountId, intBusinessUnitId, intWorkplaceGroupId, strSalaryCode })).ToList();

                IList<DepartmentVM> deptList = new List<DepartmentVM>();
                IList<AreaVM> areaList = new List<AreaVM>();
                IList<SoleDepoVM> soleDepoList = new List<SoleDepoVM>();
                var SoleDipo = "";
                var wing = "";

                if (intWorkplaceGroupId == 2)
                {
                    deptList = salaryReportList
                    .GroupBy(x => new { x.intDepartmentId, x.strDepartment, x.depRankId })
                    .Select(d => new DepartmentVM { deptId = d.Key.intDepartmentId, deptName = d.Key.strDepartment, depRankId = d.Key.depRankId }).OrderBy(a => a.depRankId).ToList();

                }
                else if(intWorkplaceGroupId == 3)
                {
                    soleDepoList = salaryReportList
                    .Where(x=>x.intSoleDepoId > 0 && Convert.ToInt64(x.intAreaId) <= 0)
                    .GroupBy(x => new { x.intSoleDepoId, intAreaId = Convert.ToInt64(x.intAreaId), x.SoleDipoName, x.intRankingId })
                    .Select(d => new SoleDepoVM { SoleDepoId = d.Key.intSoleDepoId, SoleDepoName = d.Key.SoleDipoName, AreaId = Convert.ToInt64(d.Key.intAreaId), intRankingId = Convert.ToInt64(d.Key.intRankingId) }).OrderBy(a => a.intRankingId).ToList();
                    SoleDipo = salaryReportList.Select(a => a.SoleDipoName).First();
                    

                    areaList = salaryReportList
                    .Where(x => x.intSoleDepoId > 0 && x.intAreaId > 0)
                    .GroupBy(x => new { x.intSoleDepoId, x.intAreaId, x.AreaName, x.intRankingId })
                    .Select(d => new AreaVM { SoleDepoId = d.Key.intSoleDepoId, AreaId = d.Key.intAreaId, AreaName = d.Key.AreaName, intRankingId = d.Key.intRankingId }).OrderBy(a => a.intRankingId).ToList();
                    SoleDipo = salaryReportList.Select(a=> a.SoleDipoName).First();

                    wing = salaryReportList.Select(a=> a.WingName).First();
                }

                string Month = CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName((int)intMonthId);
                var WG = await _contex.MasterWorkplaceGroups.Where(x => x.IntWorkplaceGroupId == intWorkplaceGroupId && x.IsActive == true).Select(a=> a.StrWorkplaceGroup).FirstOrDefaultAsync();
                

                SalaryMasyterVM salaryMasyter = new SalaryMasyterVM
                {
                    Month = Month,
                    Year = intYearId,
                    WorkplaceGroupName = WG,
                    SoleDipoName = SoleDipo,
                    WingName = wing,
                    salaryDataRows = null,
                    salaryDetailsReportVM = salaryReportList,
                    DepartmentVM = deptList,
                    SoleDepoVM = soleDepoList,
                    AreaVM = areaList,
                };



                return salaryMasyter;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        public async Task<HtmlToPdfDocument> PdfSalaryDetailsReport(SalaryMasyterVM obj, MasterBusinessUnit businessUnit,long intWorkplaceGroupId, long intMonthId, long intYearId)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.CSheet,                    
                    DocumentTitle = "Monthly Salary Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.PdfSalaryDetailsReport(obj, businessUnit, intWorkplaceGroupId, intMonthId, intYearId),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = SalaryDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTimeExtend.BD.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<SalaryAllReportViewModel> PdfSalaryReportAllType(string StrPartName, long IntAccountId, long IntBusinessUnitId, long IntMonthId, long IntYearId, long IntSalaryGenerateRequestId, int IntBankOrWalletType)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

            string sql = "saas.sprPyrSalarySelectQueryAll";
            SqlCommand sqlCmd = new SqlCommand(sql, connection);

            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@strPartName", StrPartName);
            sqlCmd.Parameters.AddWithValue("@intAccountId", IntAccountId);
            sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", IntBusinessUnitId);
            sqlCmd.Parameters.AddWithValue("@intMonthId", IntMonthId);
            sqlCmd.Parameters.AddWithValue("@intYearId", IntYearId);
            sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", IntSalaryGenerateRequestId);
            sqlCmd.Parameters.AddWithValue("@intBankOrWalletType", IntBankOrWalletType);

            connection.Open();
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(dt);
            connection.Close();

            try
            {
                List<SalaryReportHeaderViewModel> salaryReportHeaders = new List<SalaryReportHeaderViewModel>();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        SalaryReportHeaderViewModel model = new SalaryReportHeaderViewModel();

                        model.EmployeeId = Convert.ToInt64(row["intEmployeeId"]);
                        model.EmployeeName = row["strEmployeeName"].ToString();
                        model.EmployeeCode = row["strEmployeeCode"].ToString();
                        model.AccountName = row["strAccountName"].ToString();
                        model.BankName = row["strFinancialInstitution"].ToString();
                        model.Branch = row["strBankBranchName"].ToString();
                        model.AccountNo = row["strAccountNo"].ToString();
                        model.RoutingNo = row["strRoutingNumber"].ToString();
                        model.Salary = Convert.ToDecimal(row["numGrossSalary"]);
                        model.TotalAllowance = Convert.ToDecimal(row["numTotalAllowance"]);
                        model.TotalDecuction = Convert.ToDecimal(row["numTotalDeduction"]);
                        model.Netpay = Convert.ToDecimal(row["netPay"]);
                        model.BankPay = Convert.ToDecimal(row["bankPay"]);
                        model.DigitalBank = Convert.ToDecimal(row["degitalBankPay"]);
                        model.CashPay = Convert.ToDecimal(row["cashPay"]);
                        model.WorkplaceGroupe = row["strWorkplaceGroupName"].ToString();
                        model.Workplace = row["strWorkplaceName"].ToString();
                        model.PayrollGroupe = row["strPayrollGroupName"].ToString();
                        model.BusinessUnitId = Convert.ToInt64(row["intBusinessUnitId"]);
                        model.BusinessUnit = row["strBusinessUnitName"].ToString();
                        model.Address = row["strAddress"].ToString();
                        model.IntBankOrWalletType = Convert.ToInt64(row["intBankOrWalletType"]);

                        salaryReportHeaders.Add(model);
                    }

                    SalaryAllReportViewModel SalaryAllTypeReport = new SalaryAllReportViewModel
                    {
                        salaryReportHeaderViewModels = salaryReportHeaders,
                        BusinessUnitId = salaryReportHeaders.Select(x => x.BusinessUnitId).FirstOrDefault(),
                        BusinessUnit = salaryReportHeaders.Select(x => x.BusinessUnit).FirstOrDefault(),
                        Address = salaryReportHeaders.Select(x => x.Address).FirstOrDefault(),
                        IntBankOrWalletType = salaryReportHeaders.Select(x => x.IntBankOrWalletType).FirstOrDefault(),
                        GenerateDate = DateTime.Now.Date
                    };

                    return SalaryAllTypeReport;
                }
                else
                {
                    throw new Exception("inalid data");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HtmlToPdfDocument> PdfAllTypeOfSalaryReport(SalaryAllReportViewModel obj, int IntBankOrWalletType)
        {
            try
            {
                string docType = IntBankOrWalletType == 0 ? "Salary Report" :
                    IntBankOrWalletType == 1 ? "Salary Report(Bank Pay)" :
                    IntBankOrWalletType == 2 ? "Salary Report(degital Bank Pay)" :
                    IntBankOrWalletType == 3 ? "Salary Report(Cash)" : "";

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = docType,
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.AllTypeOfSalaryReport(obj, IntBankOrWalletType),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<BankAdviceViewModel> PdfBankAdviceReport(string partName, long intAccountId, long? intBusinessUnitId, long intMonthId, long intYearId,
            long? intWorkPlaceId, long? intWorkplaceGroupId, long? intPayrollGroupId, long? intSalaryGenerateRequestId, string? BankAccountNo, int? intBankOrWalletType, long? intEmployeeId, string? strAdviceType)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprPyrSalarySelectQueryAll";

                using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@strPartName", partName);
                    sqlCmd.Parameters.AddWithValue("@intAccountId", intAccountId);
                    sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                    sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                    sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                    sqlCmd.Parameters.AddWithValue("@intWorkplaceId", intWorkPlaceId);
                    sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                    sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", intPayrollGroupId);
                    sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", intSalaryGenerateRequestId);
                    sqlCmd.Parameters.AddWithValue("@BankAccountNumber", BankAccountNo);
                    sqlCmd.Parameters.AddWithValue("@intBankOrWalletType", intBankOrWalletType);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
                    sqlCmd.Parameters.AddWithValue("@strAdviceType", strAdviceType);

                    connection.Open();
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                    sqlAdapter.Fill(dt);
                    connection.Close();
                }
                decimal? creditAmount = 0;
                long? transaction = 0, attachment = 0;

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        creditAmount += Convert.ToDecimal(row["numNetPayable"]);
                        transaction += 1;
                    }
                }
                BankAdviceViewModel bankAdvice = _contex.AccountBankDetails.Where(x => x.IsActive == true && x.StrAccountNo == BankAccountNo)
                    .Select(x => new BankAdviceViewModel
                    {
                        CompanyBank = x.StrBankWalletName,
                        BankBranch = x.StrBranchName,
                        BankAddress = _contex.GlobalBankBranches.Where(s => s.IsActive == true && s.IntBankBranchId == x.IntBankBranchId).Select(s => s.StrBankBranchAddress).FirstOrDefault(),
                        BankAccountNo = x.StrAccountNo,
                        BusinessUnit = _contex.MasterBusinessUnits.Where(n => n.IsActive == true && n.IntBusinessUnitId == x.IntBusinessUnitId).Select(n => n.StrBusinessUnit).FirstOrDefault()
                    }).FirstOrDefault();

                bankAdvice.GeneratDate = DateTime.Now.Date;
                bankAdvice.TotalCreditAmount = creditAmount;
                bankAdvice.TotalTransactions = transaction;
                bankAdvice.TotalAttachment = attachment;
                bankAdvice.AdviceName = strAdviceType;
                //bankAdvice.AdviceTo = strAdviceTo;

                return bankAdvice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HtmlToPdfDocument> PdfBankAdviceReport(BankAdviceViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Bank Advice",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.BankAdviceReport(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task<BonusReportVM> PdfBonusReport(string PartType, long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long BonusHeaderId, long BonusId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBonusAllLanding";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@StrPartName", PartType);
                        sqlCmd.Parameters.AddWithValue("@IntBonusHeaderId", BonusHeaderId);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", intAccountId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", intBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntBonusId", BonusId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", intWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceId", null);
                        sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", null);
                        sqlCmd.Parameters.AddWithValue("@IntReligionId", null);
                        sqlCmd.Parameters.AddWithValue("@DteEffectedDate", null);
                        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", null);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", null);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", null);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                    }
                }
                List<BonusGenerateRowViewModel> dataList = new List<BonusGenerateRowViewModel>();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BonusGenerateRowViewModel model = new BonusGenerateRowViewModel();

                        model.DeptName = row["DeptName"].ToString();
                        model.SL = row["SL"].ToString();
                        model.StrEmployeeName = row["strEmployeeName"].ToString();
                        model.StrEmployeeCode = row["strEmployeeCode"].ToString();
                        model.StrEmploymentTypeName = row["strEmploymentTypeName"].ToString();
                        model.StrDepartmentName = row["DepartmentName"].ToString();
                        model.StrDesignationName = row["strDesignationName"].ToString();
                        model.StrWorkPlaceGroupName = row["strWorkPlaceGroupName"].ToString();
                        model.BonusName = row["strBonusName"].ToString();
                        model.DteJoiningDate = row["dteJoiningDate"] != DBNull.Value ? Convert.ToDateTime(row["dteJoiningDate"]).ToString() : "";
                        model.StrServiceLength = row["strServiceLength"].ToString();
                        model.NumSalary = row["numSalary"] != DBNull.Value ? Convert.ToDecimal(row["numSalary"]) : 0;
                        model.NumBasic = row["numBasic"] != DBNull.Value ? Convert.ToDecimal(row["numBasic"]) : 0;
                        model.NumBonusAmount = row["numBonusAmount"] != DBNull.Value ? Convert.ToDecimal(row["numBonusAmount"]) : 0;
                        model.NumBonusPercentage = row["numBonusPercentage"] != DBNull.Value ? Convert.ToDecimal(row["numBonusPercentage"]) : 0;

                        dataList.Add(model);
                    }
                }

                BonusReportVM obj = new BonusReportVM();
                obj.data= dataList;
                obj.strWorkplaceGroupName = _contex.MasterWorkplaceGroups.Where(x=> x.IntWorkplaceGroupId == intWorkplaceGroupId && x.IsActive == true).Select(a=> a.StrWorkplaceGroup).FirstOrDefault();
                obj.strBonusName = _contex.PyrBonusSetups.Where(x=> x.IntBonusId == BonusId && x.IsActive == true).Select(a=> a.StrBonusName).FirstOrDefault();
                obj.EffectiveDate = _contex.PyrBonusGenerateHeaders.Where(x => x.IntBonusHeaderId == BonusHeaderId && x.IsActive == true).Select(a=> a.DteEffectedDateTime).FirstOrDefault();

                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<HtmlToPdfDocument> PdfBonusReport(BonusReportVM obj, MasterBusinessUnit businessUnit)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Bonus Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.BonusReport(obj, businessUnit),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = BonusReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTimeExtend.BD.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #region =================== Increment Letter ====================

        public async Task<HtmlToPdfDocument> PdfIncrementLetterReport(IncrementLetterViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Increment Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.IncrementLetterReport(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion =================== Increment Letter ====================

        #region ======================= Transfter and Promotion ===============================

        public async Task<HtmlToPdfDocument> PdfTransferAndPromotionReport(TransferAndPromotionReportPDFViewModel model)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Increment Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.TransferAndPromotionReport(model),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion ======================= Transfter and Promotion ===============================

        #region =================Transfer Report ========================

        public async Task<HtmlToPdfDocument> PdfTransferReport(TransferReportViewModel model)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Increment Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.TransferReport(model),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion =================Transfer Report ========================

        public async Task<HtmlToPdfDocument> PdfPromotionReport(PromotionReportViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Promotion Letter",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.PromotionLetterReport(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = LoanDetailsReportCSSUrl },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Excel Generate

        public async Task<MemoryStream> ExcelFileGenerate(EmployeeListExportAsExcelViewModel empListExportExcel)
        {
            XLWorkbook wb = new XLWorkbook();

            IXLWorksheet excelsh = wb.Worksheets.Add(empListExportExcel.dataTable, empListExportExcel.SheetTitle);

            int columnLength = empListExportExcel.dataTable.Columns.Count;
            int rowLength = empListExportExcel.dataTable.Rows.Count;
            int titlePosition = empListExportExcel.leftSidePeddingCell + 1;

            for (int i = 0; i < columnLength; i++)
            {
                string colType = empListExportExcel.dataTable.Columns[i].DataType.Name.ToString();
                //string colName = dt.Columns[i].ColumnName.ToString();

                int colN = i + 1;

                if (colType == "Decimal" || colType == "Int32" || colType == "Int64" || colType == "Double")
                {
                    excelsh.Column(colN).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    if (colType == "Double" || colType == "Decimal")
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
            }

            IXLRow rowOverTable = excelsh.Row(1);
            rowOverTable.InsertRowsAbove(empListExportExcel.topPeddingCell);

            IXLColumn firstColumn = excelsh.Column(1);
            firstColumn.InsertColumnsBefore(empListExportExcel.leftSidePeddingCell);

            excelsh.Columns().AdjustToContents();

            // TitleLine_One
            excelsh.Cell(2, titlePosition).Value = empListExportExcel.TitleLine_One;
            string tlo1 = excelsh.Cell(2, titlePosition).WorksheetColumn().ColumnLetter() + 2.ToString();
            string tlo2 = excelsh.Cell(2, columnLength + empListExportExcel.leftSidePeddingCell).WorksheetColumn().ColumnLetter() + 2.ToString();
            excelsh.Range(tlo1 + ":" + tlo2).Merge();
            excelsh.Range(tlo1).Style.Font.FontSize = 20;
            excelsh.Range(tlo1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // TitleLine_Two
            excelsh.Cell(3, titlePosition).Value = empListExportExcel.TitleLine_Two;
            string tltw1 = excelsh.Cell(3, titlePosition).WorksheetColumn().ColumnLetter() + 3.ToString();
            string tltw2 = excelsh.Cell(3, columnLength + empListExportExcel.leftSidePeddingCell).WorksheetColumn().ColumnLetter() + 3.ToString();
            excelsh.Range(tltw1 + ":" + tltw2).Merge();
            excelsh.Range(tltw1).Style.Font.FontSize = 16;
            excelsh.Range(tltw1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (empListExportExcel.TitleLine_Three != null || empListExportExcel.TitleLine_Three != "")
            {
                // TitleLine_Three
                excelsh.Cell(4, titlePosition).Value = empListExportExcel.TitleLine_Three;
                string tlt1 = excelsh.Cell(4, titlePosition).WorksheetColumn().ColumnLetter() + 4.ToString();
                string tlt2 = excelsh.Cell(4, columnLength + empListExportExcel.leftSidePeddingCell).WorksheetColumn().ColumnLetter() + 4.ToString();
                excelsh.Range(tlt1 + ":" + tlt2).Merge();
                excelsh.Range(tlt1).Style.Font.FontSize = 11;
                excelsh.Range(tlt1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            if (!string.IsNullOrEmpty(empListExportExcel.date))
            {
                excelsh.Cell(empListExportExcel.topPeddingCell, titlePosition).Value = "Date: " + empListExportExcel.date;
            }

            if (empListExportExcel.OrgLogo != null)
            {
                excelsh.AddPicture(empListExportExcel.OrgLogo).MoveTo(excelsh.Cell(2, titlePosition)).WithSize(200, 75);
            }

            //Column Header style
            int columnC = columnLength + empListExportExcel.leftSidePeddingCell; // 1 for mirgen column
            string headRow1 = excelsh.Cell(empListExportExcel.topPeddingCell + 1, empListExportExcel.leftSidePeddingCell + 1).WorksheetColumn().ColumnLetter() + (empListExportExcel.topPeddingCell + 1).ToString();
            string headRow2 = excelsh.Cell(empListExportExcel.topPeddingCell + 1, columnC).WorksheetColumn().ColumnLetter() + (empListExportExcel.topPeddingCell + 1).ToString();
            excelsh.Range(headRow1 + ":" + headRow2).Style.Fill.BackgroundColor = XLColor.BlueGray;
            //excelsh.Range(headRow1 + ":" + headRow2).Style.Fill.BackgroundColor = XLColor.FromArgb(0x7F3F7F);

            excelsh.Range(headRow1 + ":" + headRow2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            excelsh.Range(headRow1 + ":" + headRow2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            excelsh.Range(headRow1 + ":" + headRow2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            excelsh.Range(headRow1 + ":" + headRow2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //Select All Cell and Style
            int rowC = rowLength + empListExportExcel.topPeddingCell + 1; // 1 for header row
            string dataRow1 = excelsh.Cell(empListExportExcel.topPeddingCell + 2, empListExportExcel.leftSidePeddingCell + 1).WorksheetColumn().ColumnLetter() + (empListExportExcel.topPeddingCell + 2).ToString();
            string dataRow2 = excelsh.Cell(rowC, columnC).WorksheetColumn().ColumnLetter() + rowC.ToString();
            excelsh.Range(dataRow1 + ":" + dataRow2).Style.Fill.BackgroundColor = XLColor.White;

            excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            excelsh.Range(dataRow1 + ":" + dataRow2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            MemoryStream stream = new MemoryStream();
            wb.SaveAs(stream);

            return stream;
        }

        #endregion Excel Generate

        #region ===============IBBL AND BEFTN =================

        public async Task<HtmlToPdfDocument> BankAdviceReportForIBBL(BankAdviceReportForIBBLViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Bank Advice",
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.BankAdviceReportForIBBL(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HtmlToPdfDocument> BankAdviceReportForBEFTN(BankAdviceReportForBEFTNViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Bank Advice",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.BankAdviceReportForBEFTN(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion ===============IBBL AND BEFTN =================

        #region ========= Daily Attendance Report PDF =============
        public async Task<HtmlToPdfDocument> DailyAttendanceReportPDF(EmployeeDaylyAttendanceReportLanding obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Daily Attendance Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.DailyAttendanceReportPDF(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region ========= Salary Tax Certificate Report PDF =============
        public async Task<HtmlToPdfDocument> SalaryTaxCertificate(SalaryTaxCertificateViewModel obj)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "Salary Tax Certificate Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _pdfTemplateGeneratorService.SalaryTaxCertificateReportPDF(obj),
                    HeaderSettings = { Line = false },
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" },
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Right = "Page [page] of [toPage]", Center = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return pdf;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}