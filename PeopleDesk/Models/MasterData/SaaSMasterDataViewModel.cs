using PeopleDesk.Data.Entity;

namespace PeopleDesk.Models.MasterData
{
    public class SaaSMasterDataViewModel
    {
    }
    public class CRUDEmploymentTypeWiseLeaveBalanceViewModel
    {
        public long PartId { get; set; }
        public long AutoId { get; set; }
        public long EmploymentTypeId { get; set; }
        public long AllocatedLeave { get; set; }
        public long YearId { get; set; }
        public long LeaveTypeId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public bool isActive { get; set; }
        public long IntCreatedBy { get; set; }
        public long IntGenderId { get; set; }
        public string StrGender { get; set; }
    }

    public class CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel
    {
        public long AutoId { get; set; }
        public long EmploymentTypeId { get; set; }
        public long AllocatedLeave { get; set; }
        public long YearId { get; set; }
        public long LeaveTypeId { get; set; }
        public bool isActive { get; set; }
        public long IntGenderId { get; set; }
        public string StrGender { get; set; }
        public long IntCreatedBy { get; set; }
        public long AccountId { get; set; }
        public List<long> BusinessUnitList { get; set; }
        public List<long> WorkPlaceGroupList { get; set; }
    }

    

    public class AnnouncementViewModel
    {
        public long IntAnnouncementId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrTitle { get; set; } = null!;
        public string StrDetails { get; set; } = null!;
        public long? IntTypeId { get; set; }
        public string? StrTypeName { get; set; }

        public DateTime? DteExpiredDate { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class AnnouncementRowViewModel
    {
        public long IntAnnouncementRowId { get; set; }
        public long IntAnnoucementId { get; set; }
        public long? IntAnnouncementReferenceId { get; set; }
        public string? StrAnnounceCode { get; set; }
        public string? StrAnnouncementFor { get; set; }
        public bool IsActive { get; set; }
    }

    public class AnnouncementCommonDTO
    {
        public AnnouncementViewModel Announcement { get; set; }
        public List<AnnouncementRowViewModel> AnnouncementRow { get; set; }
    }

    public class AnnouncementLanding
    {
        public long IntAnnouncementId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrTitle { get; set; } = null!;
        public string StrDetails { get; set; } = null!;
        public long? IntTypeId { get; set; }
        public string? StrTypeName { get; set; }
        public DateTime? DteExpiredDate { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    #region ========== Policy ==========
    public class CRUDPolicyCategoryViewModel
    {
        public long PolicyCategoryId { get; set; }
        public string PolicyCategoryName { get; set; }
        public long AccountId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
    }

    public class PolicyHeaderViewModel
    {
        public long PolicyId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string PolicyTitle { get; set; }
        public long? PolicyCategoryId { get; set; }
        public string? PolicyCategoryName { get; set; }
        public long PolicyFileUrlId { get; set; }
        public string PolicyFileName { get; set; }
        public long IntCreatedBy { get; set; }
        public bool IsActive { get; set; }

    }
    public class PolicyRowViewModel
    {
        public long RowId { get; set; }
        public long? PolicyId { get; set; }
        public string? AreaType { get; set; }
        public long? AreaAutoId { get; set; }
        public string? AreaName { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
    }

    public class PolicyCommonViewModel
    {
        public PolicyHeaderViewModel objHeader { get; set; }
        public List<PolicyRowViewModel> objRow { get; set; }
    }
    public class GetPolicyLandingViewModel
    {
        public long PolicyId { get; set; }
        public long BusinessUnitId { get; set; }
        public string PolicyTitle { get; set; }
        public long? PolicyCategoryId { get; set; }
        public string? PolicyCategoryName { get; set; }
        public long PolicyFileUrlId { get; set; }
        public string PolicyFileName { get; set; }
        public string BusinessUnitList { get; set; }
        public string DepartmentList { get; set; }
        public long AcknowledgeCount { get; set; }
        public DateTime PolicyCreateDate { get; set; }
    }
    #endregion

    #region ========== Bank Branch ==========
    public class BankBranchViewModel
    {
        public long Sl { get; set; }
        public long IntBankBranchId { get; set; }
        public string StrBankBranchCode { get; set; } = null!;
        public string StrBankBranchName { get; set; } = null!;
        public string StrBankBranchAddress { get; set; } = null!;
        public long? IntAccountId { get; set; }
        public long? IntDistrictId { get; set; }
        public string? StrDistrict { get; set; }
        public long IntCountryId { get; set; }
        public long IntBankId { get; set; }
        public string StrBankName { get; set; } = null!;
        public string? StrBankShortName { get; set; }
        public string? StrBankCode { get; set; }
        public string StrRoutingNo { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class BankBranchLandingViewModel
    {
        public long CurrentPage { get; set; }
        public long PageSize { get; set; }
        public long TotalCount { get; set; }
        public IEnumerable<BankBranchViewModel> data { get; set; }
    }
    #endregion

    #region
    public class MasterComponentVM
    {
        public MasterDashboardComponent Obj { get; set; }
        public long IntPermissionId { get; set; }
        public bool IsPermission { get; set; }
    }
    public class DashboardComponentPermissionVM
    {
        public long IntDashboardId { get; set; }
        public long IntAccountId { get; set; }
        public long IntPermissionId { get; set; }
        public string StrHashCode { get; set; }
        public bool IsCreate { get; set; }
        public bool IsDelete { get; set; }
        public long ActionById { get; set; }
    }

    #endregion

    public class PyrPayrollElementTypeViewModel
    {
        public long IntPayrollElementTypeId { get; set; }
        public long IntAccountId { get; set; }
        public string StrPayrollElementName { get; set; } = null!;
        public string? StrCode { get; set; }
        public bool IsPrimarySalary { get; set; }
        public bool IsBasicSalary { get; set; }
        public bool IsAddition { get; set; }
        public bool IsDeduction { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class SalaryBreakdownElementDDLViewModel
    {
        public string Label { get; set; }
        public long Value { get; set; }
        public bool IsBasic { get; set; }
    }
    public class PyrSalaryBreakdowViewModel
    {
        public long IntSalaryBreakdownHeaderId { get; set; }
        public string StrSalaryBreakdownTitle { get; set; } = null!;
        public long IntAccountId { get; set; }
        public long? IntHrPositionId { get; set; }
        public string? HrPosition { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? WorkplaceGroup { get; set; }
        public long? IntSalaryPolicyId { get; set; }
        public string? StrSalaryPolicy { get; set; }
        public string? StrDependOn { get; set; }
        public bool IsPerday { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class PyrSalaryBreakdownHeaderViewModel
    {
        public long IntSalaryBreakdownHeaderId { get; set; }
        public string StrSalaryBreakdownTitle { get; set; } = null!;
        public long IntAccountId { get; set; }
        public long? IntHrPositonId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public long? IntSalaryPolicyId { get; set; }
        public string? StrDependOn { get; set; }
        public bool IsPerday { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public List<PyrSalaryBreakdownRowViewModel> pyrSalaryBreakdowRowList { get; set; }

    }
    public class PyrSalaryBreakdownRowViewModel
    {
        public long IntSalaryBreakdownRowId { get; set; }
        public long IntSalaryBreakdownHeaderId { get; set; }
        public long IntPayrollElementTypeId { get; set; }
        public string StrPayrollElementName { get; set; } = null!;
        public string? StrBasedOn { get; set; }
        public string? StrDependOn { get; set; }
        public decimal? NumNumberOfPercent { get; set; }
        public decimal? NumAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class SalaryBreakdownRowViewModel
    {
        public long IntSalaryBreakdownRowId { get; set; }
        public long IntSalaryBreakdownHeaderId { get; set; }
        public long IntPayrollElementTypeId { get; set; }
        public string StrPayrollElementName { get; set; } = null!;
        public string? StrBasedOn { get; set; }
        public string? StrDependOn { get; set; }
        public bool? IsBasic { get; set; }
        public decimal? NumNumberOfPercent { get; set; }
        public decimal? NumAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class PyrSalaryPolicyViewModel
    {
        public long IntPolicyId { get; set; }
        public long IntAccountId { get; set; }
        public string StrPolicyName { get; set; } = null!;
        public bool IsSalaryCalculationShouldBeActual { get; set; }
        public long IntGrossSalaryDevidedByDays { get; set; }
        public long IntGrossSalaryRoundDigits { get; set; }
        public bool IsGrossSalaryRoundUp { get; set; }
        public bool IsGrossSalaryRoundDown { get; set; }
        public long IntNetPayableSalaryRoundDigits { get; set; }
        public bool IsNetPayableSalaryRoundUp { get; set; }
        public bool IsNetPayableSalaryRoundDown { get; set; }
        public bool IsSalaryShouldBeFullMonth { get; set; }
        public long IntPreviousMonthStartDay { get; set; }
        public long IntNextMonthEndDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class PyrSalaryPolicyApplyViewModel
    {
        public long IntPolicyAssignId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntDepartmentId { get; set; }
        public long IntDesignationId { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public long IntSalaryPolicyId { get; set; }
        public string StrSalaryPolicyName { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }

    }

    public class TimeCalenderViewModel
    {
        public long? CalenderId { get; set; }
        public string? CalenderCode { get; set; }
        public string? CalenderName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? LastStartTime { get; set; }
        public TimeSpan? ExtendedStartTime { get; set; }
        public decimal? MinWorkHour { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? AccountId { get; set; }
        public long? InsertUserId { get; set; }
        public DateTime? InsertDateTime { get; set; }
        public long? UpdateUserId { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public bool? IsActive { get; set; }
        public TimeSpan? DteBreakStartTime { get; set; }
        public TimeSpan? DteBreakEndTime { get; set; }

    }

    public class EmpWorklineConfigViewModel
    {
        public long IntWorklineId { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntServiceLengthInDays { get; set; }
        public long? IntNotifyInDays { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }

    #region Notification Permission
    public class AccountNotificationViewModel
    {
        public long CompanyId { get; set; }
        public string companyName { get; set; }
        public List<NotificationCategoryViewModel> notificationCategoryViewModel { get; set; }
    }

    public class NotificationCategoryViewModel
    {
        public long CategoryId { get; set; }
        public long? PermissionId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsChecked { get; set; }
        public List<NotificationCategoryTypeViewModel> NotificationCategoryTypes { get; set; }
    }

    public class NotificationCategoryTypeViewModel
    {
        public long? PermissionId { get; set; }
        public long CategoryTypeId { get; set; }
        public string TypeName { get; set; }
        public bool? IsChecked { get; set; }
    }

    public class NotificationPermissionViewModel
    {
        public long IntAccountId { get; set; }
        public long intCreateBy { get; set; }
        public List<NotificationCategoriesViewModel> notificationCategoriesViewModel { get; set; }
    }
    public class NotificationCategoriesViewModel
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<NotificationCategoryTypesViewModel> notificationCategoriesViewModel { get; set; }
    }
    public class NotificationCategoryTypesViewModel
    {
        public long CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
    }

    #endregion

    #region Management Dashboard Permission
    public class ManagementDashboardPermissionViewModel
    {
        public long IntAccountid { get; set; }
        public string AccountName { get; set; }
        public long? IntCreateBy { get; set; }
        public List<DashboardComponentViewModel> dashboardComponentViewModels { get; set; }
    }
    public class DashboardComponentViewModel
    {
        public long? intDashboardComponentId { get; set; }
        public string DashboardComponentName { get; set; }
        public string strHashCode { get; set; }
        public bool? IsChecked { get; set; }
    }

    #endregion

    #region Account Bank Detail
    public class AccountBankDetailViewModel
    {
        public long IntAccountBankDetailsId { get; set; }
        public long IntAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long IntBankOrWalletType { get; set; }
        public long IntBankWalletId { get; set; }
        public string? StrBankWalletName { get; set; }
        public string? StrDistrict { get; set; }
        public long? IntBankBranchId { get; set; }
        public string? StrBranchName { get; set; }
        public string? StrRoutingNo { get; set; }
        public string? StrSwiftCode { get; set; }
        public string? StrAccountName { get; set; }
        public string? StrAccountNo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    #endregion

    #region OverTime Configuration
    public class OverTimeConfigurationVM
    {
        public long IntOtconfigId { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsOvertimeAutoCalculate { get; set; }
        public long IntOtdependOn { get; set; }
        public decimal? NumFixedAmount { get; set; }
        public long IntOverTimeCountFrom { get; set; }
        public long IntOtbenefitsHour { get; set; }
        public long intMaxOverTimeDaily { get; set; }
        public long intMaxOverTimeMonthly { get; set; }
        public long IntOtcalculationShouldBe { get; set; }
        public long IntOtAmountShouldBe { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class OverTimeConfigurationDetailsVM
    {
        public long IntOtautoId { get; set; }
        public long IntMasterId { get; set; }
        public decimal NumFromMinute { get; set; }
        public decimal NumToMinute { get; set; }
        public long IntAmountPercentage { get; set; }
    }
    #endregion

    #region Tax Challan Config
    public class MasterTaxchallanConfigVM
    {
        public long IntTaxChallanConfigId { get; set; }
        public int IntYear { get; set; }
        public DateTime DteFiscalFromDate { get; set; }
        public DateTime DteFiscalToDate { get; set; }
        public string strFiscalYearDateRange { get; set; }
        public string? StrCircle { get; set; }
        public string? StrZone { get; set; }
        public string? StrChallanNo { get; set; }
        public DateTime DteChallanDate { get; set; }
        public string? StrBankName { get; set; }
        public long? IntBankId { get; set; }
        public long IntAccountId { get; set; }
        public long IntFiscalYearId { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    #endregion

    #region

    public partial class MasterTerritoryTypeVM
    {
        public long IntTerritoryTypeId { get; set; }
        public long? IntHrPositionId { get; set; }
        public string? HrPosition { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? WorkplaceGroup { get; set; }
        public string StrTerritoryType { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
    public class TerritorySetupViewModel
    {
        public long TerritoryId { set; get; }
        public string TerritoryName { get; set; }
        public long ParentTerritoryId { get; set; }
        public long WorkplaceGroupId { set; get; }
        public long? HrPositionId { set; get; }
        public string TerritoryCode { set; get; }
        public string TerritoryAddress { set; get;}
        public long TerritoryTypeId { set; get; }
        public long AccountId { set; get; }
        public long BusinessUnitId { set; get; }
        public string Remarks { set; get; }
        public bool IsActive { set; get; }
        public long CreatedBy { set; get; }
    }


    public class TerritorySetupTableViewModel
    {
        public long TerritoryId { set; get; }
        public string TerritoryName { get; set; }
        public long AccountId { set; get; }
        public long BusinessUnitId { set; get; }
        public long? WorkplaceGroupId { set; get; }
        public long? HrPositionId { set; get; }
        public string TerritoryCode { set; get; }
        public string TerritoryAddress { set; get; }
        public long? TerritoryTypeId { set; get; }
        public string TerritoryType { set; get; }
        public long? ParentTerritoryId { get; set; }
        public string ParentTerritory { get; set; }        
        public long? ParentTerritoryTypeId { set; get; }
        public string ParentTerritoryType { set; get; }
        public string Remarks { set; get; }
    }
    #endregion
}

