using PeopleDesk.Data.Entity;

namespace PeopleDesk.Models.Employee
{
    public class EmployeeProfileView
    {
        public EmployeeProfileLandingView EmployeeProfileLandingView { get; set; }
        public UserVM userVM { get; set; }
        public List<EmpEmployeeAddress> EmpEmployeeAddress { get; set; }
        public EmpEmployeeBankDetail EmpEmployeeBankDetail { get; set; }
        public List<EmpEmployeeEducation> EmpEmployeeEducation { get; set; }
        public List<EmpEmployeeRelativesContact> EmpEmployeeRelativesContact { get; set; }
        public List<EmpEmployeeJobHistory> EmpEmployeeJobHistory { get; set; }
        public List<EmpEmployeeTraining> EmpEmployeeTraining { get; set; }
        public EmpEmployeePhotoIdentity EmpEmployeePhotoIdentity { get; set; }
        public List<EmpJobExperience> EmpJobExperience { get; set; }
        public List<EmpSocialMedium> EmpSocialMedia { get; set; }
    }
    public class UserVM
    {
        public string? LoginId { get; set; }
        public string? StrPassword { get; set; }
        public string? OfficeMail { get; set; }
        public string? StrPersonalMobile { get; set; }
        public long? UserTypeId { get; set; }
        public string? StrUserType { get; set; }
        public bool? UserStatus { get; set; }
    }

    public class EmployeeProfileLandingView
    {
        public long? IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrReferenceId { get; set; }
        public long? IntEmployeeImageUrlId { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public long? IntSupervisorId { get; set; }
        public string? StrSupervisorName { get; set; }
        public long? IntSupervisorImageUrlId { get; set; }
        public long? IntLineManagerId { get; set; }
        public string? StrLinemanager { get; set; }
        public long? IntLinemanagerImageUrlId { get; set; }
        public string? StrCardNumber { get; set; }
        public long? IntGenderId { get; set; }
        public string? StrGender { get; set; }
        public long? IntReligionId { get; set; }
        public string? StrReligion { get; set; }
        public string? StrMaritalStatus { get; set; }
        public string? StrBloodGroup { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteContractFromDate { get; set; }
        public DateTime? DteContractToDate { get; set; }
        public DateTime? DteConfirmationDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public string? StrDottedSupervisorName { get; set; }
        public string? StrServiceLength { get; set; }
        public bool? IsSalaryHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUserInactive { get; set; }
        public bool? IsRemoteAttendance { get; set; }
        public long? IntWorkplaceId { get; set; }
        public string? StrWorkplaceName { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntAccountId { get; set; }
        public string? StrAccountName { get; set; }

        //bank Info
        public string? StrBankAccountName { get; set; }
        public string? StrBankAccountNo { get; set; }
        public string? StrRoutingNo { get; set; }
        public string? StrBranchName { get; set; }
        public string? StrBankWalletName { get; set; }
        //End

        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public EmploymentTypeVM employmentType { get; set; }

        //Basic Info Details
        public long? IntDetailsId { get; set; }

        public string? StrOfficeMail { get; set; }
        public string? StrPersonalMail { get; set; }
        public string? StrPersonalMobile { get; set; }
        public string? StrOfficeMobile { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string? StrPayrollGroupName { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public string? StrPayscaleGradeName { get; set; }
        public long? IntCalenderTypeId { get; set; }
        public string? StrCalenderType { get; set; }
        public long? IntCalenderId { get; set; }
        public string? StrCalenderName { get; set; }
        public long? IntHrpositionId { get; set; }
        public string? StrHrpostionName { get; set; }
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
        public long? IntEmployeeStatusId { get; set; }
        public string? StrEmployeeStatus { get; set; }
        public long? IntParentId { get; set; }
        public int? IsManual { get; set; }
        public string? StrLoginId { get; set; }
        public long? IntCountryId { get; set; }
        public string? StrCountry { get; set; }
        public long? IntUserTypeId { get; set; }
        public string? StrPassword { get; set; }
        public string? StrPersonalEmail { get; set; }
        public long? IntUserId { get; set; }
        public bool? IsCreateUser { get; set; }
        public string? StrUserType { get; set; }
        public string? VehicleNo { get; set; }
        public string? PinNo { get; set; }
        public string? Remarks { get; set; }
        public string? DrivingLicenseNo { get; set; }
        public bool? UserStatus { get; set; }
        public bool? IsTakeHomePay { get; set; }
    }

    public class EmployeeProfileLandingVM
    {
        public long? IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrReferenceId { get; set; }
        public long? IntEmployeeImageUrlId { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public long? IntSupervisorId { get; set; }
        public string? StrSupervisorName { get; set; }
        public long? IntLineManagerId { get; set; }
        public string? StrLinemanager { get; set; }
        public string? StrCardNumber { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public string? StrDottedSupervisorName { get; set; }
        public string? StrServiceLength { get; set; } 
        public long? IntWorkplaceId { get; set; }
        public string? StrWorkplaceName { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
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
        public long? IntEmployeeStatusId { get; set; }
        public string? StrEmployeeStatus { get; set; }
        public string? PinNo { get; set; }
        public string? ContactNo { get; set; }
    }
    public class EmployeeProfileLandingPaginationViewModel
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public IEnumerable<dynamic> Data { get; set; }
    }

    public class EmployeeListDDLViewModel
    {
        public long? IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrReferenceId { get; set; }
        public long? IntEmployeeImageUrlId { get; set; }
        public long? IntAccountId { get; set; }

        //Basic Info Detail
        public string? StrOfficeMail { get; set; }

        public string? StrPersonalMail { get; set; }
        public string? StrPersonalMobile { get; set; }
        public string? StrOfficeMobile { get; set; }
    }

    public class EmployeeDetailsViewModel
    {
        public EmpEmployeeBasicInfo EmployeeInfoObj { get; set; }
        public MasterEmploymentType EmployeeTypeObj { get; set; }
        public EmpEmployeeBasicInfo SupervisorObj { get; set; }
        public EmpEmployeeBasicInfo LineManagerObj { get; set; }
        public MasterDepartment DepartmentObj { get; set; }
        public MasterDesignation DesignationObj { get; set; }
        public MasterWorkplaceGroup WorkplaceGroupObj { get; set; }
        public PyrPayscaleGrade PayscaleGradeObj { get; set; }
    }
    public class EmployeeMovementPaginationVM
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public IEnumerable<MovementApplicationViewModel> Data { get; set; }
    }

    public class LoanLandingPagination
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public List<LoanReportByAdvanceFilterViewModel> Data { get; set; }
    }
}