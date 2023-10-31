using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PeopleDesk.Models.Employee
{
    public class TransferNpromotionVM
    {
        public long IntTransferNpromotionId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string? StrTransferNpromotionType { get; set; }
        public long? IntTransferOrpromotedFrom { get; set; }
        public long IntAccountId { get; set; }
        public string? AccountName { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string? BusinessUnitName { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long IntWorkplaceId { get; set; }
        public string? WorkplaceName { get; set; }
        public long IntDepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long IntDesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long IntSupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long IntLineManagerId { get; set; }
        public string? LineManagerName { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public string? DottedSupervisorName { get; set; }
        public long? IntWingId { get; set; }
        public string? WingName { get; set; }
        public long? IntSoldDepoId { get; set; }
        public string? SoldDepoName { get; set; }
        public long? IntRegionId { get; set; }
        public string? RegionName { get; set; }
        public long? IntAreaId { get; set; }
        public string? AreaName { get; set; }
        public long? IntTerritoryId { get; set; }
        public string? TerritoryName { get; set; }

        public DateTime DteEffectiveDate { get; set; }
        public DateTime? DteReleaseDate { get; set; }
        public long? IntAttachementId { get; set; }
        public string? StrRemarks { get; set; }
        public string StrStatus { get; set; }
        public bool? IsJoined { get; set; }
        public bool IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public long IntBusinessUnitIdFrom { get; set; }
        public string? BusinessUnitNameFrom { get; set; }
        public long IntWorkplaceGroupIdFrom { get; set; }
        public string? WorkplaceGroupNameFrom { get; set; }
        public long IntWorkplaceIdFrom { get; set; }
        public string? WorkplaceNameFrom { get; set; }
        public long IntDepartmentIdFrom { get; set; }
        public string? DepartmentNameFrom { get; set; }
        public long IntDesignationIdFrom { get; set; }
        public string? DesignationNameFrom { get; set; }
        public long? IntSupervisorIdFrom { get; set; }
        public string? SupervisorNameFrom { get; set; }
        public long? IntLineManagerIdFrom { get; set; }
        public string? LineManagerNameFrom { get; set; }
        public long? IntDottedSupervisorIdFrom { get; set; }
        public string? DottedSupervisorNameFrom { get; set; }
        public long? IntWingIdFrom { get; set; }
        public string? WingNameFrom { get; set; }
        public long? IntSoldDepoIdFrom { get; set; }
        public string? SoldDepoNameFrom { get; set; }
        public long? IntRegionIdFrom { get; set; }
        public string? RegionNameFrom { get; set; }
        public long? IntAreaIdFrom { get; set; }
        public string? AreaNameFrom { get; set; }
        public long? IntTerritoryIdFrom { get; set; }
        public string? TerritoryNameFrom { get; set; }

        public List<EmpTransferNpromotionUserRoleVM>? EmpTransferNpromotionUserRoleVMList { get; set; }
        public List<EmpTransferNpromotionRoleExtensionVM>? EmpTransferNpromotionRoleExtensionVMList { get; set; }
    }
    public class TransferNPromotionPaginationVM : PaginationBaseVM
    {
        public List<TransferNpromotionVM> Data { get; set; }
    }
    public class EmpTransferNpromotionUserRoleVM
    {
        public long? IntTransferNpromotionUserRoleId { get; set; }
        public long? IntTransferNpromotionId { get; set; }
        public long? IntUserRoleId { get; set; }
        public string? StrUserRoleName { get; set; }
    }

    public class EmpTransferNpromotionRoleExtensionVM
    {
        public long? IntRoleExtensionRowId { get; set; }
        public long? IntTransferNpromotionId { get; set; }
        public long? IntEmployeeId { get; set; }
        public long? IntOrganizationTypeId { get; set; }
        public string? StrOrganizationTypeName { get; set; }
        public long? IntOrganizationReffId { get; set; }
        public string? StrOrganizationReffName { get; set; }
    }

    public class CRUDEmployeeIncrementVM
    {
        public long IntIncrementId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public decimal? NumOldGrossAmount { get; set; }
        public decimal? NumCurrentGrossAmount { get; set; }
        public string? StrIncrementDependOn { get; set; }
        public decimal? NumIncrementPercentageOrAmount { get; set; }
        public DateTime? DteEffectiveDate { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }

        ///return view
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrStatus { get; set; }
        public bool? IsPromotion { get; set; }
        public long? IntTransferNpromotionReferenceId { get; set; }
        public decimal? NumIncrementAmount { get; set; }
    }

    public class CRUDIncrementPromotionTransferVM
    {
        public bool isPromotion { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public List<CRUDEmployeeIncrementVM> incrementList { get; set; }
        public TransferNpromotionVM? transferPromotionObj { get; set; }
    }

    public class GetIncrementPaginationVM
    {
        public IEnumerable<CRUDEmployeeIncrementVM> Data { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
    }

    public class IncrementLetterViewModel
    {
        public long? IncrementLetterId { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmploymentType { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? BusinessUnit { get; set; }
        public DateTime? IncrementSalaryDate { get; set; }
        public List<ExistingSalaryViewModel> existingSalaryViewModels { get; set; }
        public List<IncrementSalaryViewModel> incrementSalaryViewModels { get; set; }
        public string AmountInWords { get; set; }
        public string ApprovarName { get; set; }
        public string ApprovarDesignation { get; set; }
        public string ApprovarDepartment { get; set; }
        public string ApprovarBusinessUnit { get; set; }
    }
    public class ExistingSalaryViewModel
    {
        public long? IntEmpSalaryElementAssignHeaderId { get; set; }
        public long? SalaryElementId { get; set; }
        public string? SalaryElement { get; set; }
        public double? numAmount { get; set; }
    }
    public class IncrementSalaryViewModel
    {
        public long? IntEmpSalaryElementAssignHeaderId { get; set; }
        public long? SalaryElementId { get; set; }
        public string? SalaryElement { get; set; }
        public double? numAmount { get; set; }
    }
    public class TransferAndPromotionReportPDFViewModel
    {
        public long? TransferAndPromotionId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? BusinessUnit { get; set; }
        public string? CompanyName { get; set; }
        public string? PreviousDesignation { get; set; }
        public string? PromotedDesignation { get; set; }
        public string? PromotedBusinessUnit { get; set; }
        public DateTime? PromotedDate { get; set; }
        public string ApprovarName { get; set; }
        public string ApprovarDesignation { get; set; }
        public string ApprovarDepartment { get; set; }
        public string ApprovarBusinessUnit { get; set; }
    }
    public class TransferReportViewModel
    {
        public long? TransferReportId { get; set; }
        public DateTime? dteTranserReportDate { get; set; }
        public string? EmployeeName { get; set; }
        public string? FromDesignation { get; set; }
        public string? ToDesignation { get; set; }
        public string FromDepartment { get; set; }
        public string ToDepartment { get; set; }
        public decimal? PresenetSalary { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? FromBusinessUnit { get; set; }
        public string? ToBusinessUnit { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Approvarname { get; set; }
        public string? ApprovarDesignation { get; set; }
        public string? ApprovarDepartment { get; set; }
        public string? ApprovarBusinessUnit { get; set; }

        #region Promotion Report
        public class PromotionReportViewModel
        {
            public long EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string? FromBusinessUnit { get; set; }
            public string? FromDepartment { get; set; }
            public string? FromDesignation { get; set; }
            public string? ToDepartment { get; set; }
            public string? ToDesignation { get; set; }
            public string? ToBusinessUnit { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public string? ApproverName { get; set; }
            public string? ApproverBusinessUnit { get; set; }
            public string? ApproverDepartment { get; set; }
            public string? ApproverDesignation { get; set; }
        }
    }
    public class BankAdviceReportForIBBLViewModel
    {
        public long? intLogoUrlId { get; set; }
        public string? BusinessUnit { get; set; }
        public string? CompanyAddress { get; set; }
        public DateTime? ReportGenerateDate { get; set; }
        public string? CompanyBankName { get; set; }
        public string? CompanyBranchName { get; set; }
        public string? CompanyAccountNo { get; set; }
        //public List<BankAdviceHeaderVM> BankAdviceVM { get; set; }
        //public List<BankAdvice>? Data { get; set; }
        public List<BankAdviceHeaderVM> BankAdviceVM { get; set; } = new List<BankAdviceHeaderVM>();
        public List<BankAdvice>? Data { get; set; } = new List<BankAdvice>();

        //public List<EmployeePaymentInfoViewModel>? employeePaymentInfoViewModels { get; set; }
    }
    public class EmployeePaymentInfoViewModel
    {
        public string? BankAccountNo { get; set; }
        public string? AccountName { get; set; }
        public Decimal? NetAmount { get; set; }
        public string? EmployeCode { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }

    }
    public class BankAdviceReportForBEFTNViewModel
    {
        public long? intLogoUrlId { get; set; }
        public string? BusinessUnit { get; set; }
        public string? CompanyAddress { get; set; }
        public DateTime? ReportGenerateDate { get; set; }

        public string? CompanyBankName { get; set; }
        public string? CompanyBranchName { get; set; }
        public string? CompanyAccountNo { get; set; }
        public List<BankAdviceHeaderVM> BankAdviceVM { get; set; } = new List<BankAdviceHeaderVM>();
        public List<BankAdvice>? Data { get; set; } = new List<BankAdvice>();

        //public List<EmployeePaymentInfoBEFTNViewModel>? employeePaymentInfoBEFTNViewModels { get; set; }
    }
    public class EmployeePaymentInfoBEFTNViewModel
    {

        public string? AccountName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNo { get; set; }
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentInfo { get; set; }
        public string? Comments { get; set; }
        public string? RoutingNo { get; set; }
        public string? InstrumentNo { get; set; }
    }
    #endregion
}



