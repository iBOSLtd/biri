using DinkToPdf;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Services.Global.Interface
{
    public interface IPdfAndExcelService
    {
        #region ================ pdf =============================

        Task<HtmlToPdfDocument> PdfDailyAttendanceReportByEmployeeId(PdfDailyAttendanceReportViewModel obj);

        Task<HtmlToPdfDocument> PdfSeparationReportByEmployee(MasterBusinessUnit businessUnit, EmployeeSeparationReportViewModel SeperationObj);

        Task<HtmlToPdfDocument> PdfLoanDetailsReportByEmployee(LoanDetailsReportViewModel obj);

        Task<HtmlToPdfDocument> PdfLoanReportAll(List<LoanReportByAdvanceFilterViewModel> obj, MasterBusinessUnit BusinessUnit, string? FromDate, string? ToDate);

        Task<HtmlToPdfDocument> PdfLeaveHistoryByEmployee(LeaveHistoryViewModel obj);

        Task<HtmlToPdfDocument> PdfMonthlySalaryReport(MonthlySalaryReportViewModel obj);

        Task<HtmlToPdfDocument> PdfEmployeeBasedSalaryReport(EmployeePayslipViewModel obj);

        Task<HtmlToPdfDocument> PdfMonthlySalaryDepartmentWiseReport(List<MonthlySalaryDepartmentWiseReportViewModel> obj, MasterBusinessUnit BusinessUnit, string monthName);

        Task<HtmlToPdfDocument> PdfRosterReport(List<RosterReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date);

        Task<HtmlToPdfDocument> PdfMovementReport(List<MovementReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date);

        Task<HtmlToPdfDocument> PdfGetAllLeaveApplicatonListForApprove(List<LeaveDataSetViewModel> model);

        Task<HtmlToPdfDocument> BanglaPdfGenerate();

        Task<LoanDetailsReportViewModel> LoanDetailsByLoanId(long? loanId);

        Task<EmployeeSalaryPaySlipViewModel> PdfPaySlipReportByEmployeeId(string partName, long intEmployeeId, long intMonthId, long intYearId, long intSalaryGenerateRequestId);

        Task<HtmlToPdfDocument> PdfSalaryPaySlipReportByEmployee(EmployeeSalaryPaySlipViewModel obj, string employeeSalary);
        Task<SalaryMasyterVM> PdfSalaryDetailsReport(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long intMonthId, long intYearId, string? strSalaryCode);
        Task<HtmlToPdfDocument> PdfSalaryDetailsReport(SalaryMasyterVM obj, MasterBusinessUnit businessUnit,long intWorkplaceGroupId, long intMonthId, long intYearId);
        Task<SalaryAllReportViewModel> PdfSalaryReportAllType(string StrPartName, long IntAccountId, long IntBusinessUnitId, long IntMonthId, long IntYearId, long IntSalaryGenerateRequestId, int IntBankOrWalletType);

        Task<HtmlToPdfDocument> PdfAllTypeOfSalaryReport(SalaryAllReportViewModel obj, int IntBankOrWalletType);

        Task<BankAdviceViewModel> PdfBankAdviceReport(string partName, long intAccountId, long? intBusinessUnitId, long intMonthId, long intYearId,
            long? intWorkPlaceId, long? intWorkplaceGroupId, long? intPayrollGroupId, long? intSalaryGenerateRequestId, string? BankAccountNo, int? intBankOrWalletType, long? intEmployeeId, string? strAdviceType);
        Task<HtmlToPdfDocument> PdfBankAdviceReport(BankAdviceViewModel obj);
        Task<BonusReportVM> PdfBonusReport(string PartType, long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long BonusHeaderId, long BonusId);
        Task<HtmlToPdfDocument> PdfBonusReport(BonusReportVM obj, MasterBusinessUnit businessUnit);


        #region ============= Increment, Transfer and Promotion ================
        Task<HtmlToPdfDocument> PdfIncrementLetterReport(IncrementLetterViewModel model);
        Task<HtmlToPdfDocument> PdfTransferAndPromotionReport(TransferAndPromotionReportPDFViewModel model);
        Task<HtmlToPdfDocument> PdfTransferReport(TransferReportViewModel model);
        Task<HtmlToPdfDocument> PdfPromotionReport(PromotionReportViewModel obj);
        #endregion ================ Increment, Transfer and Promotion ====================

        Task<HtmlToPdfDocument> BankAdviceReportForIBBL(BankAdviceReportForIBBLViewModel obj);
        Task<HtmlToPdfDocument> BankAdviceReportForBEFTN(BankAdviceReportForBEFTNViewModel obj);
        Task<HtmlToPdfDocument> DailyAttendanceReportPDF(EmployeeDaylyAttendanceReportLanding obj);
        Task<HtmlToPdfDocument> SalaryTaxCertificate(SalaryTaxCertificateViewModel obj);

        #endregion ================ pdf =============================

        #region ============= Excel ==============

        Task<MemoryStream> ExcelFileGenerate(EmployeeListExportAsExcelViewModel employeeListExportAsExcelView);

        #endregion ============= Excel ==============
    }
}