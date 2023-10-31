namespace PeopleDesk.Models.Employee
{
    public class PayrollManagementViewModel
    {

    }
    public class EmpAdditionDeductionFiltering
    {
        public long IntMonth { get; set; }
        public long IntYear { get; set; }
        public long BusinessUnitId { get; set; }
        public long workplaceGroupId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? searchTxt { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class EmpSalaryAdditionNDeductionLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public EmployeeHeader EmployeeHeader { get; set; }
        public List<EmpSalaryAdditionNDeductionLandingVM> Data { get; set; }
    }
    public class EmpSalaryAdditionNDeductionLandingVM
    {
        public string? strEntryType { get; set; }
        public long? intSalaryAdditionAndDeductionId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public string? BusinessUnit { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrEmployeeCode { get; set; }
        public string? StrDesignation { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public string? StrWorkplace { get; set; }
        public long? Year { get; set; }
        public long? Month { get; set; }        
        public string? MonthName { get; set; }
        public long? ToYear { get; set; }
        public long? ToMonth { get; set; }
        public bool? isActive { get; set; }
        public bool? isAddition { get; set; }
        public bool? isAutoRenew { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? WingId { get; set; }
        public string? WingName { get; set; }
        public long? SoleDepoId { get; set; }
        public string? SoleDepoName { get; set; }
        public long? RegionId { get; set; }
        public string? RegionName { get; set; }
        public long? AreaId { get; set; }
        public string? AreaName { get; set; }
        public long? TerritoryId { get; set; }
        public string? TerritoryName { get; set; }
    }
    public class EmpSalaryAdditionNDeductionViewModel
    {
        public string? strEntryType { get; set; }
        public long? intSalaryAdditionAndDeductionId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { get; set; }
        public bool? isAutoRenew { get; set; }
        public long? intYear { get; set; }
        public long? intMonth { get; set; }
        public string? strMonth { get; set; }
        public bool? isAddition { get; set; }
        public string? strAdditionNDeduction { get; set; }
        public long? intAdditionNDeductionTypeId { get; set; }
        public int? intAmountWillBeId { get; set; }
        public string? strAmountWillBe { get; set; }
        public decimal? numAmount { get; set; }
        public bool? isActive { get; set; }
        public bool? isReject { get; set; }
        public long? intActionBy { get; set; }
        public long? intToYear { get; set; }
        public long? intToMonth { get; set; }
        public string? strToMonth { get; set; }
        public List<long>? EmployeeIdList { get; set; }
    }
    public class EmployeeIdList
    {
        public long? intEmployeeId { get; set; }
    }

    public class BulkSalaryAdditionNDeductionViewModel
    {
        public bool? IsForceAssign { get; set; }
        public bool? IsSkipNAssign { get; set; }
        public List<BulkSalaryAdditionNDeductionVM> bulkSalaryAdditionNDeductions { get; set; }
    }
    public class BulkSalaryAdditionNDeductionVM
    {
        public long? intSalaryAdditionAndDeductionId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public bool? isAutoRenew { get; set; }
        public long? intYear { get; set; }
        public long? intMonth { get; set; }
        public string? strMonth { get; set; }
        public bool? isAddition { get; set; }
        public string? strAdditionNDeduction { get; set; }
        public int? intAmountWillBeId { get; set; }
        public string? strAmountWillBe { get; set; }
        public decimal? numAmount { get; set; }
        public bool? isActive { get; set; }
        public bool? isReject { get; set; }
        public long? intActionBy { get; set; }
        public long? intToYear { get; set; }
        public long? intToMonth { get; set; }
        public string? strToMonth { get; set; }
    }
    public class PolicyAssignViewModel
    {
        public string strEntryType { get; set; }
        public long? intPolicyAssignId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intActionBy { get; set; }
        public List<EmpPolicyIdViewModel> EmpPolicyIdViewModelList { get; set; }
    }

    public class EmpPolicyIdViewModel
    {
        public long intEmployeeId { get; set; }
        public long? intPolicyId { get; set; }
        public string? strPolicyName { get; set; }
    }

    public class SalaryBreakdownPolicyViewModel
    {
        public string strEntryType { get; set; }
        public long? intSalaryBreakdownAssignId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intActionBy { get; set; }
        public List<BreakdownPolicyAssignViewModel> breakdownPolicieList { get; set; }
    }

    public class BreakdownPolicyAssignViewModel
    {
        public long intEmployeeId { get; set; }
        public long? intSalaryBreakdownId { get; set; }
        public string? strSalaryBreakdownName { get; set; }
        public decimal? numNumberOfPercent { get; set; }

    }
    public class SalaryDataRows
    {
        public string StrPayrollElement { get; set; }
        public decimal NumAmount { get; set; }
        public long? IntPayrollElementId { get; set; }
    }
    public class SalaryMasyterVM
    {
        public string? Month { get; set; }
        public long? Year { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public string? SoleDipoName { get; set; }
        public string? WingName { get; set; }
        public IList<DepartmentVM> DepartmentVM { get; set; }
        public IList<SoleDepoVM> SoleDepoVM { get; set; }
        public IList<AreaVM> AreaVM { get; set; }
        public SalaryDataRows salaryDataRows { get; set; }
        public IList<SalaryDetailsReportVM> salaryDetailsReportVM { get; set; }
    }
    public class DepartmentVM
    {
        public long? deptId { get; set; }
        public string? deptName { get; set; }
        public long? depRankId { get; set; }
    }
    public class SoleDepoVM
    {
        public long? SoleDepoId { get; set; }
        public string? SoleDepoName { get; set; }
        public long? intRankingId { get; set; }
        public long? AreaId { get; set; }
    }
    public class AreaVM
    {
        public long? SoleDepoId { get; set; }
        public long? AreaId { get; set; }
        public string? AreaName { get; set; }
        public long? intRankingId { get; set; }
    }
    public class SalaryDetailsReportVM
    {
        public long SL { get; set; }
        public long intSalaryGenerateHeaderId { get; set; }
        public long intSalaryGenerateRequestId { get; set; }
        public long intEmployeeId { get; set; }
        public long intTotalWorkingDays { get; set; }
        public long? intPayableDays { get; set; }
        public long? intActualPayableDays { get; set; }
        public long? intPresent { get; set; }
        public long? intAbsent { get; set; }
        public long? intLate { get; set; }
        public long? intOffDay { get; set; }
        public long? intHoliday { get; set; }
        public long? intMovement { get; set; }
        public long? intCasualLeave { get; set; }
        public long? intEarnLeave { get; set; }
        public long? intSickLeave { get; set; }
        public long? intMaternityLeave { get; set; }
        public long? intSpecialLeave { get; set; }
        public long? intAnnualLeave { get; set; }
        public long? intLWP { get; set; }
        public long? intPrivilegeLeave { get; set; }
        public long? intOthersLeave { get; set; }
        public string? strSalaryPolicyName { get; set; }
        public string? strEmployeeCode { get; set; }
        public string? strEmployeeName { get; set; }
        public string? strEmploymentType { get; set; }
        public string? strEmployeeStatus { get; set; }
        public long? intDepartmentId { get; set; }
        public string? strDepartment { get; set; }
        public string? strDesignation { get; set; }
        public string? strServiceLength { get; set; }
        public string? strAccountName { get; set; }
        public string? strPaymentBankType { get; set; }
        public string? strFinancialInstitution { get; set; }
        public string? strBankBranchName { get; set; }
        public string? strRoutingNumber { get; set; }
        public string? strAccountNo { get; set; }
        public string? strWorkplaceName { get; set; }
        public string? strWorkplaceGroupName { get; set; }
        public string? strBusinessUnitName { get; set; }
        public string? strPayrollGroupName { get; set; }
        public string? DeptName { get; set; }
        public DateTime? dteJoiningDate { get; set; }
        
        public DateTime? dtePayrollGenerateFrom { get; set; }
        public DateTime? dtePayrollGenerateTo { get; set; }
        public decimal? numPerDaySalary { get; set; }
        public decimal? numGrossSalary { get; set; }
        public decimal? numPayableSalaryCal { get; set; }
        public decimal? numOverTimeHour { get; set; }
        public decimal? numOverTimeRate { get; set; }
        public decimal? numOverTimeAmount { get; set; }
        public decimal? numLoanAmount { get; set; }
        public decimal? numBasic { get; set; }
        public decimal? numHouseRent { get; set; }
        public decimal? numMedical { get; set; }
        public decimal? numConvence { get; set; }
        public decimal? numSpecialAllowence { get; set; }
        public decimal? numAbsent { get; set; }
        public decimal? AbsentAmount { get; set; }
        public decimal? numExtraRouseRent { get; set; }
        public decimal? numExtraAllowance { get; set; }
        public decimal? numFine { get; set; }
        public decimal? numAdjustment { get; set; }
        public decimal? numCityAllowence { get; set; }
        public decimal? numPfCompany { get; set; }
        public decimal? numGratuity { get; set; }
        public decimal? TotalPfNGratuity { get; set; }
        public decimal? numArearSalary { get; set; }
        public decimal? numBikeLoan { get; set; }
        public int? intPIN { get; set; }
        public int? ApprovedLeave { get; set; }
        public long? intPayrollElementTypeId { get; set; }
        public long? intPayrollElementId { get; set; }
        public long? strPayrollElement { get; set; }
        public decimal? numAmount { get; set; }
        public bool? isPrimarySalaryElement { get; set; }
        public decimal? numTotalAllowance { get; set; }
        public decimal? numTotalDeduction { get; set; }
        public decimal? numNetPayableSalary { get; set; }
        public decimal? numBankPay { get; set; }
        public decimal? numDigitalBankPay { get; set; }
        public decimal? numCashPay { get; set; }
        public decimal? numTaxAmount { get; set; }
        public decimal? numPFAmount { get; set; }
        public long? intAreaId { get; set; }
        public string? AreaName { get; set; }
        public long? intSoleDepoId { get; set; }
        public string? SoleDipoName { get; set; }
        public long? intWingId { get; set; }
        public string? WingName { get; set; }
        public long? intRankingId { get; set; }
        public long? RankId { get; set; }
        public long? depRankId { get; set; }
       
    }
    public class BankAdviceLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public string MonthName { get; set; }
        public List<BankAdviceHeaderVM> BankAdviceVM { get; set; }=new List<BankAdviceHeaderVM>();
        public List<BankAdvice>? Data { get; set; } = new List<BankAdvice>();

    }

    public class BankAdviceHeaderVM
    { 
        public string SoleDepoName { get; set; }
        public string WingName { get; set; }
       
    }
    public class BankAdvice
    {
        //New
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation { get; set; }

        public long? DesignationRank { get; set; }
        public string? Territory { get; set; }

        public string? Area { get; set; }

        public string? Region { get; set; }

        public string? MobileNumber { get; set; }

        public string? AccountNo { get; set; }

        public decimal? NetSalary { get; set; }
        public string? Remarks { get; set; }

        public string? Wing { get; set; }
        public string? SoleDepo { get; set; }

        //Old
        //public string? Reason { get; set; }
        //public string? BankAccountNumber { get; set; }
        //public string? AdviceType { get; set; }
        //public string? RoutingNumber { get; set; }
        //public long? EmployeeId { get; set; }
        //public string? EmployeeName { get; set; }
        //public string? AccountNo { get; set; }
        //public string? AccountName { get; set; }
        //public decimal? numNetPayable { get; set; }
        //public string? AccType { get; set; }
        //public string? FinancialInstitution { get; set; }
        //public string? BranchName { get; set; }
        //public string? EmployeeCode { get; set; }
    }
    public class BankAdviceHeader
    {
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long intMonthId { get; set; }
        public long intYearId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intSalaryGenerateRequestId { get; set; }
        public string? BankAccountNo { get; set; }
        public int? intBankOrWalletType { get; set; }
        public string? strAdviceType { get; set; }
        public bool IsForXl { get; set; }
        public string? searchTxt { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public class SalaryCertificateLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public List<SalaryCertificateVM> Data { get; set; }
    }
    public class SalaryCertificateVM
    {
        public long intSalaryCertificateRequestId { get; set; }
        public long intEmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? employeeCode { get; set; }
        public string? strDesignation { get; set; }
        public string? strDepartment { get; set; }
        public long? intEmploymentTypeId { get; set; }
        public string? strEmploymentType { get; set; }
        public long? intPayRollMonth { get; set; }
        public long? intPayRollYear { get; set; }
        public long? intAccountId { get; set; }
        public bool? isActive { get; set; }
        public long? intCreatedBy { get; set; }
        public DateTime? dteCreatedAt { get; set; }
        public long? intUpdatedBy { get; set; }
        public DateTime? dteUpdatedAt { get; set; }
        public long? intPipelineHeaderId { get; set; }
        public string? strStatus { get; set; }
        public bool? isPipelineClosed { get; set; }
        public bool? isReject { get; set; }
        public DateTime? dteRejectDateTime { get; set; }
        public string? Status { get; set; }
        public string? RejectedBy { get; set; }
    }
}
