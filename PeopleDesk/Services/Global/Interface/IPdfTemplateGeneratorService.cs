using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Services.Global.Interface
{
    public interface IPdfTemplateGeneratorService
    {
        Task<string> DailyAttendanceReportByEmployeeId(PdfDailyAttendanceReportViewModel obj);

        Task<string> SeparationReportByEmployee(MasterBusinessUnit businessUnit, EmployeeSeparationReportViewModel SeperationObj);

        Task<string> LoanDetailsReportByEmployee(LoanDetailsReportViewModel obj);

        Task<string> LoanReportAll(List<LoanReportByAdvanceFilterViewModel> obj, MasterBusinessUnit BusinessUnit, string? FromDate, string? ToDate);

        Task<string> LeaveReportByEmployee(LeaveHistoryViewModel obj);

        Task<string> MonthlySalaryReport(MonthlySalaryReportViewModel obj);
        Task<string> PdfSalaryDetailsReport(SalaryMasyterVM obj, MasterBusinessUnit businessUnit, long intWorkplaceGroupId, long intMonthId, long intYearId);

        Task<string> EmployeeBasedSalaryReport(EmployeePayslipViewModel obj);

        Task<string> MonthlySalaryDepartmentWiseReport(List<MonthlySalaryDepartmentWiseReportViewModel> obj, MasterBusinessUnit BusinessUnit, string monthName);

        Task<string> RosterReport(List<RosterReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date);

        Task<string> MovementReport(List<MovementReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date);

        Task<string> LeaveApplicationApprovalReport(List<LeaveDataSetViewModel> obj);

        Task<string> BanglaPdfGenerate();

        Task<string> SalaryPaySlipReportByEmployee(EmployeeSalaryPaySlipViewModel obj);

        Task<string> SalaryCertificateReportByEmployeeId(EmployeeSalaryPaySlipViewModel obj);

        Task<string> AllTypeOfSalaryReport(SalaryAllReportViewModel obj, int IntBankOrWalletType);

        Task<string> BankAdviceReport(BankAdviceViewModel obj);
        Task<string> BonusReport(BonusReportVM obj, MasterBusinessUnit businessUnit);

        #region============ IncrementLetterViewModel ========

        Task<string> IncrementLetterReport(IncrementLetterViewModel obj);

        #endregion================= IncrementLetterViewModel ======================
        Task<string> TransferAndPromotionReport(TransferAndPromotionReportPDFViewModel obj);
        Task<string> TransferReport(TransferReportViewModel obj);
        Task<string> PromotionLetterReport(PromotionReportViewModel obj);
        Task<string> BankAdviceReportForIBBL(BankAdviceReportForIBBLViewModel obj);
        Task<string> BankAdviceReportForBEFTN(BankAdviceReportForBEFTNViewModel obj);
        Task<string> DailyAttendanceReportPDF(EmployeeDaylyAttendanceReportLanding obj);
        Task<string> SalaryTaxCertificateReportPDF(SalaryTaxCertificateViewModel obj);

    }
}

