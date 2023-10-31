namespace PeopleDesk.Models.Employee
{
    public class BonusManagementViewModel
    {
    }

    public class CRUDBonusSetupViewModel
    {
        //For Bonus Setup
        public string strPartName { get; set; }

        public long IntBonusSetupId { get; set; }
        public int? IntBonusId { get; set; }
        public string? StrBonusName { get; set; }
        public string? StrBonusDescription { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public int? IntReligion { get; set; }
        public string? StrReligionName { get; set; }
        public int? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public int? IntMinimumServiceLengthMonth { get; set; }
        public long? IntMaximumServiceLengthMonth { get; set; }
        public string? StrBonusPercentageOn { get; set; }
        public decimal? NumBonusPercentage { get; set; }
        public long? IntCreatedBy { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BonusAllLandingViewModel
    {
        public string StrPartName { get; set; }
        public long IntBonusHeaderId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntBonusId { get; set; }
        public long IntPayrollGroupId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntReligionId { get; set; }
        public DateTime DteEffectedDate { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? dteFromDate { get; set; }
        public DateTime? dteToDate { get; set; }
    }

    public class CRUDBonusGenerateViewModel
    {
        public string? StrPartName { get; set; }
        public long IntBonusHeaderId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? WingId { get; set; }
        public long? SoleDepoId { get; set; }
        public long? RegionId { get; set; }
        public long? AreaId { get; set; }
        public long? TerritoryId { get; set; }
        public long? IntBonusId { get; set; }
        public string? StrBonusName { get; set; } = null!;
        public DateTime? DteEffectedDateTime { get; set; }
        public decimal? NumBonusAmount { get; set; }
        public bool? IsArrearBonus { get; set; }
        public long? IntArrearBonusReferenceId { get; set; }
        public long? IntCreatedBy { get; set; }
        public List<BonusGenerateRowViewModel>? BonusGenerateRowVM { get; set; }
    }
    public class BonusReportVM
    {
        public string? strBonusName { get; set; }
        public string? strWorkplaceGroupName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? strSoleDepoName { get; set; }
        public string? strWingName { get; set; }
        public List<BonusGenerateRowViewModel> data { get; set; }
    }
    public class BonusGenerateRowViewModel
    {
        public string SL { get; set; }
        public long IntRowId { get; set; }
        public long? IntBonusHeaderId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrEmployeeName { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentTypeName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignationName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartmentName { get; set; }
        public string? DeptName { get; set; }
        public long? IntReligionId { get; set; }
        public string? StrReligionName { get; set; }
        public long? IntWorkPlaceGroupId { get; set; }
        public string? StrWorkPlaceGroupName { get; set; }
        public string? BonusName { get; set; }
        public long? IntWorkPlaceId { get; set; }
        public string? StrWorkPlaceName { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string? StrPayrollGroupName { get; set; }
        public string? DteJoiningDate { get; set; }
        public string? StrServiceLength { get; set; }
        public decimal? NumSalary { get; set; }
        public decimal? NumBasic { get; set; }
        public decimal? NumBonusAmount { get; set; }
        public decimal? NumBonusPercentage { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class JsonStringViewModel
    {
        public long AutoId { get; set; }
        public long ApproveStatusId { get; set; }
        public string ApproveBy { get; set; }
    }
}