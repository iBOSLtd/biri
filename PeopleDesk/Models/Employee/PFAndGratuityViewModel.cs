namespace PeopleDesk.Models.Employee
{
    public class PFAndGratuityViewModel
    {
    }

    public class CRUDEmpPfngratuityVM
    {
        public long IntPfngratuityId { get; set; }
        public long IntAccountId { get; set; }
        public bool IsHasPfpolicy { get; set; }
        public long IntNumOfEligibleYearForBenifit { get; set; }
        public decimal NumEmployeeContributionOfBasic { get; set; }
        public decimal NumEmployerContributionOfBasic { get; set; }
        public long IntNumOfEligibleMonthForPfinvestment { get; set; }
        public bool IsHasGratuityPolicy { get; set; }
        public long IntNumOfEligibleYearForGratuity { get; set; }
        public long IntCreatedBy { get; set; }
    }

    public class PFInvestmentHeaderVM
    {
        public long IntInvenstmentHeaderId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DtePfPeriodFromMonthYear { get; set; }
        public DateTime DtePfPeriodToMonthYear { get; set; }
        public string? StrInvestmentCode { get; set; }
        public string? StrInvestmentReffNo { get; set; }
        public DateTime DteInvestmentDate { get; set; }
        public DateTime? DteMatureDate { get; set; }
        public decimal NumInterestRate { get; set; }
        public long? IntBankId { get; set; }
        public string? StrBankName { get; set; }
        public long? IntBankBranchId { get; set; }
        public string? StrBankBranchName { get; set; }
        public string? StrRoutingNo { get; set; }
        public string? StrAccountName { get; set; }
        public string? StrAccountNumber { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }

        /// extra info
        public decimal NumInvestmentAmount { get; set; }
        public decimal NumInterestAmount { get; set; }
        public string StrStatus { get; set; }
    }
    public partial class PFInvestmentRowVM
    {
        public long IntRowId { get; set; }
        public long IntInvenstmentHeaderId { get; set; }
        public long IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public string? StrServiceLength { get; set; }
        public decimal NumEmployeeContribution { get; set; }
        public decimal NumEmployerContribution { get; set; }
        public decimal NumTotalAmount { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }

        /// extra info
        public string StrEmployeeCode { get; set; }
        public decimal NumInterestRate { get; set; }
        public decimal NumInterestAmount { get; set; }
        public decimal NumGrandTotalAmount { get; set; }
        public DateTime salaryDate { get; set; }

    }
    public class CRUDPFInvestmentVM
    {
        public PFInvestmentHeaderVM header { get; set; }
        public List<PFInvestmentRowVM> rowList { get; set; }
    }
    public class PFInvestmentPagination
    {
        public List<PFInvestmentHeaderVM> Data { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
    }

    public class PFNGratuityViewModel
    {
        public PfAccountViewModel PfAccountViewModel { get; set; }
        public List<FiscalYearWisePFInfoViewModel> FiscalYearWisePFInfoViewModel { get; set; }
        public List<PFWithdrawViewModel> PFWithdrawViewModel { get; set; }
    }
    public class PfAccountViewModel
    {
        public decimal? Gratuity { get; set; }
        public decimal? EmployeePFContribution { get; set; }
        public decimal? EmployerPFContribution { get; set; }
        public decimal? TotalPFNGratuity { get; set; }
        public decimal? TotalPFWithdraw { get; set; }
        public decimal? TotalAvailablePFNGratuity { get; set; }
    }
    public class FiscalYearWisePFInfoViewModel
    {
        public long? intYearId { get; set; }
        public string? strFiscalYear { get; set; }
        public DateTime? dteFiscalFromDate { get; set; }
        public DateTime? dteFiscalToDate { get; set; }
        public List<PFInformationViewModel> PFInformationViewModel { get; set; }
    }
    public class PFInformationViewModel
    {
        public string? Month { get; set; }
        public decimal? EmployeeContribution { get; set; }
        public decimal? OrgContribution { get; set; }
        public decimal? TotalPfAmount { get; set; }
    }
    public class PFWithdrawViewModel
    {
        public long intPFWithdrawId { get; set; }
        public long intEmployeeId { get; set; }
        public string? strEmployee { get; set; }
        public string? strDepartment { get; set; }
        public string? strDesignation { get; set; }
        public long? intAccountId { get; set; }
        public DateTime? dteApplicationDate { get; set; }
        public decimal numWithdrawAmount { get; set; }
        public string? strReason { get; set; }
        public bool? isActive { get; set; }
        public long? intCreatedBy { get; set; }
        public DateTime? dteCreatedAt { get; set; }
        public string? strStatus { get; set; }
    }
}
