using PeopleDesk.Data.Entity;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Auth;

namespace PeopleDesk.Models.Employee
{
    public class EmployeeViewModel
    {
    }

    public class DepartmentViewModel
    {
        public long IntDepartmentId { get; set; }
        public string StrDepartment { get; set; } = null!;
        public string StrDepartmentCode { get; set; } = null!;
        public long? IntParentDepId { get; set; }
        public string? StrParentDepName { get; set; } = null!;
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class WorkPlaceViewModel
    {
        public long IntWorkplaceId { get; set; }
        public string StrWorkplace { get; set; } = null!;
        public string? StrWorkplaceCode { get; set; }
        public string? StrAddress { get; set; }
        public long? IntDistrictId { get; set; }
        public string? StrDistrict { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public bool? IsActive { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class DesignationViewModel
    {
        public long IntDesignationId { get; set; }
        public string StrDesignation { get; set; } = null!;
        public string? StrDesignationCode { get; set; }
        public long? IntPositionId { get; set; }
        public string? StrPosition { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public string StrBusinessUnitCode { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeCodeDuplicateViewModel
    {
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public long? DuplicateCount { get; set; }
    }

    public class EmployeeBulkUploadViewModel
    {
        public long IntEmpBulkUploadId { get; set; }
        public long IntAccountId { get; set; }
        public long? IntUrlId { get; set; }
        public long? IntIdentitySlid { get; set; }
        public string? StrBusinessUnit { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public string? StrWorkplace { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrDesignation { get; set; }
        public string? StrEmploymentType { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrCardNumber { get; set; }
        public string? StrGender { get; set; }
        public bool? IsSalaryHold { get; set; }
        public string? StrReligionName { get; set; }
        public string? strHrPosition { get; set; }
        public string StrWingName { get; set; }
        public string StrSoleDepoName { get; set; }
        public string StrRegionName { get; set; }
        public string StrAreaName { get; set; }
        public string StrTerritoryName { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteContactFromDate { get; set; }
        public DateTime? DteContactToDate { get; set; }
        public string? strSupervisorCode { get; set; }
        public string? strDottedSupervisorCode { get; set; }
        public string? strLineManagerCode { get; set; }
        public string? StrLoginId { get; set; }
        public string? StrPassword { get; set; }
        public string? StrEmailAddress { get; set; }
        public string? StrPhoneNumber { get; set; }
        public string? StrDisplayName { get; set; }
        public string? StrUserType { get; set; }
        public bool IsProcess { get; set; }
        public bool IsActive { get; set; }
        public long IntCreateBy { get; set; }
        public DateTime DteCreateAt { get; set; }
    }

    public class EmployeeBasicViewModel
    {
        public string StrPart { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeCode { get; set; } = null!;
        public string? StrCardNumber { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public long IntGenderId { get; set; }
        public string StrGender { get; set; } = null!;
        public long? IntReligionId { get; set; }
        public string? StrReligion { get; set; }
        public string? StrMaritalStatus { get; set; }
        public string? StrBloodGroup { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntDesignationId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteConfirmationDate { get; set; }
        public DateTime? DteContactFromDate { get; set; }
        public DateTime? DteContactToDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntLineManagerId { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public bool IsSalaryHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUserInactive { get; set; }
        public bool IsRemoteAttendance { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string? StrPayrollGroupName { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public string? StrPayscaleGradeName { get; set; }
        public long? IntCalenderId { get; set; }
        public string? StrCalenderName { get; set; }
        public long? IntHrpositionId { get; set; }
        public string? StrHrpostionName { get; set; }
        public long? WingId { get; set; }
        public long? SoleDepoId { get; set; }
        public long? RegionId { get; set; }
        public long? AreaId { get; set; }
        public long? TerritoryId { get; set; }
        public string? PinNo { get; set; }
        public long? IntEmployeeStatusId { get; set; }
        public string? StrEmployeeStatus { get; set; }
        public string? StrOfficeMail { get; set; }
        public string? StrPersonalMail { get; set; }
        public string? StrPersonalMobile { get; set; }
        public string? StrOfficeMobile { get; set; }
        public bool? IsTakeHomePay { get; set; }

        public bool? IsCreateUser { get; set; }
        public CreateUserViewModel? UserViewModel { get; set; }
        public CalendarAssignViewModel? calendarAssignViewModel { get; set; }
    }
    public class EmployeeBasicInfoViewModel
    {
        public long IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeCode { get; set; } = null!;
        public string? StrCardNumber { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public long IntGenderId { get; set; }
        public string StrGender { get; set; } = null!;
        public long? IntReligionId { get; set; }
        public string? StrReligion { get; set; }
        public string? StrMaritalStatus { get; set; }
        public string? StrBloodGroup { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntDesignationId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteConfirmationDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntLineManagerId { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public bool IsSalaryHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUserInactive { get; set; }
        public bool IsRemoteAttendance { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class EmployeeBasicCreateNUpdateVM
    {
        public long IntEmployeeBasicInfoId { get; set; }
        public string? StrEmployeeCode { get; set; } = null!;
        public string? StrCardNumber { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public long IntGenderId { get; set; }
        public string StrGender { get; set; } = null!;
        public long? IntReligionId { get; set; }
        public string? StrReligion { get; set; }
        public string? StrMaritalStatus { get; set; }
        public string? StrBloodGroup { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntDesignationId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteConfirmationDate { get; set; }
        public DateTime? DteContactFromDate { get; set; }
        public DateTime? DteContactToDate { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntLineManagerId { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public bool IsSalaryHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUserInactive { get; set; }
        public bool IsRemoteAttendance { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long? IntCalenderId { get; set; }
        public string? StrCalenderName { get; set; }
        public long? IntHrpositionId { get; set; }
        public string? StrHrpostionName { get; set; }
        public long? WingId { get; set; }
        public long? SoleDepoId { get; set; }
        public long? RegionId { get; set; }
        public long? AreaId { get; set; }
        public long? TerritoryId { get; set; }
        public long? IntEmployeeStatusId { get; set; }
        public string? StrEmployeeStatus { get; set; }
        public string? StrOfficeMail { get; set; }
        public string? StrPersonalMail { get; set; }
        public string? StrPersonalMobile { get; set; }
        public string? StrOfficeMobile { get; set; }
        public bool? IsTakeHomePay { get; set; }
        public bool? IsCreateUser { get; set; }
        public CreateUserViewModel? UserViewModel { get; set; }
        public CalendarAssignViewModel? calendarAssignViewModel { get; set; }
    }
    public class ConfirmationEmployeeVM
    {
        public long EmployeeId { get; set; }
        public long? DesignationId { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string? PinNo { get; set; }
        //public long ConfirmById { get; set; }
        //public DateTime ConfirmDateTime { get; set; }
    }
    public class CalendarAssignViewModel
    {
        public long EmployeeId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? GenerateStartDate { get; set; }
        public long RunningCalendarId { get; set; }
        public string CalendarType { get; set; }
        public DateTime? NextChangeDate { get; set; }
        public long RosterGroupId { get; set; }
        public DateTime? GenerateEndDate { get; set; }
        public bool isAutoGenerate { get; set; }
    }

    public class EmployeeAddressViewModel
    {
        public long IntEmployeeAddressId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long IntAddressTypeId { get; set; }
        public string StrAddressType { get; set; } = null!;
        public long? IntCountryId { get; set; }
        public string? StrCountry { get; set; }
        public string? StrDivision { get; set; }
        public string? StrDistrictOrState { get; set; }
        public string? StrAddressDetails { get; set; }
        public string? StrZipOrPostCode { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeBankDetailsViewModel
    {
        public long PartId { get; set; }
        public long IntEmployeeBankDetailsId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long? IntBankOrWalletType { get; set; }
        public long IntBankWalletId { get; set; }
        public string StrBankWalletName { get; set; } = null!;
        public string? StrDistrict { get; set; }
        public string? StrBranchName { get; set; }
        public string? StrRoutingNo { get; set; }
        public string? StrSwiftCode { get; set; }
        public string StrAccountName { get; set; } = null!;
        public string StrAccountNo { get; set; } = null!;
        public bool IsPrimarySalaryAccount { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class GlobalBankBranchViewModel
    {
        public long IntBankBranchId { get; set; }
        public string StrBankBranchCode { get; set; } = null!;
        public string StrBankBranchName { get; set; } = null!;
        public string StrBankBranchAddress { get; set; } = null!;
        public long? IntAccountId { get; set; }
        public long? IntDistrictId { get; set; }
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

    public class EmployeeEducationViewModel
    {
        public long IntEmployeeEducationId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long IntInstituteId { get; set; }
        public string StrInstituteName { get; set; } = null!;
        public bool IsForeign { get; set; }
        public long? IntCountryId { get; set; }
        public string StrCountry { get; set; } = null!;
        public long? IntEducationDegreeId { get; set; }
        public string? StrEducationDegree { get; set; }
        public long? IntEducationFieldOfStudyId { get; set; }
        public string? StrEducationFieldOfStudy { get; set; }
        public string? StrCgpa { get; set; }
        public string? StrOutOf { get; set; }
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntCertificateFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeRelativesContactViewModel
    {
        public long IntEmployeeRelativesContactId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrRelativesName { get; set; } = null!;
        public string StrRelationship { get; set; } = null!;
        public string? StrPhone { get; set; }
        public string? StrEmail { get; set; }
        public string? StrAddress { get; set; }
        public bool IsEmergencyContact { get; set; }
        public string StrGrantorNomineeType { get; set; }
        public string? StrNid { get; set; }
        public string? StrBirthId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public string? StrRemarks { get; set; }
        public long? IntPictureFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeJobHistoryViewModel
    {
        public long IntJobExperienceId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrCompanyName { get; set; } = null!;
        public string StrJobTitle { get; set; } = null!;
        public string? StrLocation { get; set; }
        public DateTime? DteFromDate { get; set; }
        public DateTime? DteToDate { get; set; }
        public string? StrRemarks { get; set; }
        public long? IntNocfileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeTrainingViewModel
    {
        public long IntTrainingId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrTrainingTitle { get; set; } = null!;
        public long? IntInstituteId { get; set; }
        public string? StrInstituteName { get; set; }
        public bool IsForeign { get; set; }
        public long IntCountryId { get; set; }
        public string StrCountry { get; set; } = null!;
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public DateTime? DteExpiryDate { get; set; }
        public long? IntTrainingFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeFileViewModel
    {
        public long IntEmployeeFileId { get; set; }
        public long IntDocumentTypeId { get; set; }
        public string StrFileTitle { get; set; } = null!;
        public long IntEmployeeFileUrlId { get; set; }
        public string? StrTags { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeePhotoIdentityViewModel
    {
        public long IntEmployeePhotoIdentityId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long? IntProfilePicFileUrlId { get; set; }
        public long? IntProfilePicFormalFileUrlId { get; set; }
        public long? IntSignatureFileUrlId { get; set; }
        public string? StrNid { get; set; }
        public long? IntNidfileUrlId { get; set; }
        public string? StrBirthId { get; set; }
        public long? IntBirthIdfileUrlId { get; set; }
        public string? StrPassport { get; set; }
        public long? IntPassportFileUrlId { get; set; }
        public string? StrNationality { get; set; }
        public string? StrBiography { get; set; }
        public string? StrHobbies { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class EmployeeManualAttendanceViewModel
    {
        public long? Id { get; set; }
        public long? AttendanceSummaryId { get; set; }
        public long? EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? CurrentStatus { get; set; }
        public string? RequestStatus { get; set; }
        public string? Remarks { get; set; }
        public bool? isApproved { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public bool? isManagement { get; set; }
        public long? AccountId { get; set; }
        public long WorkPlaceGroup { get; set; }
        public long BusinessUnitId { get; set; }
    }

    public class EmployeeProfileUpdateViewModel
    {
        public string? PartType { get; set; }
        public long? EmployeeId { get; set; }
        public long? AutoId { get; set; }
        public string? Value { get; set; }
        public long? InsertByEmpId { get; set; }
        public bool IsActive { get; set; }

        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? RoutingNo { get; set; }
        public string? SwiftCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNo { get; set; }

        public string? PaymentGateway { get; set; }
        public string? DigitalBankingName { get; set; }
        public string? DigitalBankingNo { get; set; }

        public int? AddressTypeId { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public int? DivisionId { get; set; }
        public string? DivisionName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public int? ThanaId { get; set; }
        public string? ThanaName { get; set; }
        public int? PostOfficeId { get; set; }
        public string? PostOfficeName { get; set; }
        public string? PostCode { get; set; }
        public string? AddressDetails { get; set; }

        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? Location { get; set; }
        public string? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? Description { get; set; }

        public bool? isForeign { get; set; }
        public string? InstituteName { get; set; }
        public string? Degree { get; set; }
        public long? DegreeId { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? CGPA { get; set; }
        public string? OutOf { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? FileUrlId { get; set; }

        public string? Name { get; set; }
        public int? RelationId { get; set; }
        public string? RelationName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? NID { get; set; }
        public string? BirthId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Remarks { get; set; }
        public bool? IsEmergencyContact { get; set; }
        public long? SpecialContactTypeId { get; set; }
        public string? SpecialContactTypeName { get; set; }

        public string? TrainingName { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class EmployeeSeparationViewModel
    {
        public long PartId { get; set; }
        public long? IntSeparationId { get; set; }
        public long IntEmployeeId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public string? StrEmployeeName { get; set; } = null!;
        public string? StrEmployeeCode { get; set; } = null!;
        public long? IntSeparationTypeId { get; set; }
        public string? StrSeparationTypeName { get; set; }
        public DateTime? DteSeparationDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public List<long>? StrDocumentId { get; set; }
        public string? StrReason { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeSeparationReleaseViewModel
    {
        public long? IntSeparationId { get; set; }
        public bool? IsReleased { get; set; }
        //public long IntAccountId { get; set; }
        //public long IntCreatedBy { get; set; }
    }

    public class EmployeeTaxAssignViewModel
    {
        public long? IntTaxId { get; set; }
        public long? IntEmployeeId { set; get; }
        public decimal? NumTaxAmount { set; get; }
        public long? IntCreatedBy { set; get; }
        public long? IntAccountId { set; get; }
        public long? IntBusinessUnitId { set; get; }
        public long? IntWorkplaceId { set; get; }
        public long? IntWorkplaceGroupId { set; get; }
    }

    public class TaxBulkUploadViewModel
    {
        public string StrEmployeeCode { get; set; }
        public decimal NumTaxAmount { get; set; }
    }
    public class ActiveEmployeeTaxAssignLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public List<ActiveEmployeeForTaxAssignViewModel> Data { get; set; }
    }

    public class ActiveEmployeeForTaxAssignViewModel
    {
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntEmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public long? intDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public bool? IsTakeHomePay { get; set; }
        public long? IntTaxId { get; set; }
        public decimal? NumTaxAmount { get; set; }
        public decimal? NumGrossSalary { get; set; }
        public string? Status { get; set; }
    }

    public class TimeSheetViewModel
    {
        public string? PartType { get; set; }
        public long? EmployeeId { get; set; }
        public long? AutoId { get; set; }
        public string? Value { get; set; }
        public long? IntCreatedBy { get; set; }
        public bool? IsActive { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? AccountId { get; set; }

        public string? HolidayGroupName { get; set; }
        public long? Year { get; set; }

        public long? HolidayGroupId { get; set; }
        public string? HolidayName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? TotalDays { get; set; }

        public string? CalenderCode { get; set; }
        public string? CalendarName { get; set; }
        public string? OfficeStartTime { get; set; }
        public string? StartTime { get; set; }
        public string? ExtendedStartTime { get; set; }
        public string? LastStartTime { get; set; }
        public string? OfficeCloseTime { get; set; }
        public string? EndTime { get; set; }
        public decimal? MinWorkHour { get; set; }
        public bool? isNightShift { get; set; }
        public bool? IsConfirm { get; set; }

        public string? ExceptionOffdayName { get; set; }
        public bool? IsAlternativeDay { get; set; }

        public long? ExceptionOffdayGroupId { get; set; }
        public string? WeekOfMonth { get; set; }
        public long? WeekOfMonthId { get; set; }
        public string? DaysOfWeek { get; set; }
        public long? DaysOfWeekId { get; set; }
        public string? Remarks { get; set; }

        public string? RosterGroupName { get; set; }
        public string? OffdayGroupName { get; set; }

        public long? WorkplaceId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public DateTime? OvertimeDate { get; set; }
        public decimal? OvertimeHour { get; set; }
        public string? Reason { get; set; }
        public string? BreakStartTime { get; set; }
        public string? BreakEndTime { get; set; }
        public List<TimeSheetRosterJson>? timeSheetRosterJsons { get; set; }
        public List<EmpOverTimeUploadDTO>? objOvertimeBulkUpload { get; set; }
        public List<TimeSheetOffdayJson>? timeSheetOffdayJsons { get; set; }
    }

    public class TimeSheetRosterJson
    {
        public string? CalenderName { get; set; }
        public long? RosterGroupId { get; set; }
        public long? CalendarId { get; set; }
        public long? NoOfDaysChange { get; set; }
        public long? NextCalenderId { get; set; }
        public string? NextCalendarName { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class TimeSheetOffdayJson
    {
        public long? OffdayGroupId { get; set; }
        public long? WeekdayId { get; set; }
        public string? WeekdayName { get; set; }
        public long? NoOfDaysChange { get; set; }
        public long? NextWeekdayId { get; set; }
        public string? NextWeekdayName { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class EmpOverTimeUploadDTO
    {
        public long? AutoId { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public long? EmployeeDesignationId { get; set; }
        public string? EmployeeDesignationName { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? Year { get; set; }
        public long? Month { get; set; }
        public long? Day { get; set; }
        public DateTime? OnlyDate { get; set; }
        public long? FromHour { get; set; }
        public long? FromMinute { get; set; }
        public string? FromAmPm { get; set; }
        public string? FromTime { get; set; }
        public long? ToHour { get; set; }
        public long? ToMinute { get; set; }
        public string? ToAmPm { get; set; }
        public string? ToTime { get; set; }
        public decimal? OvertimeHour { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsValid { get; set; }
        public DateTime? InsertDateTime { get; set; }
        public string? InsertBy { get; set; }
        public string? Remarks { get; set; }
    }

    public class TimeEmpOverTimeVM
    {
        public long IntOverTimeId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        //public long? IntWorkplaceId { get; set; }
        public long? IntYear { get; set; }
        public long? IntMonth { get; set; }
        public DateTime? DteOverTimeDate { get; set; }
        public TimeSpan? TmeStartTime { get; set; }
        public TimeSpan? TmeEndTime { get; set; }
        public decimal? NumOverTimeHour { get; set; }
        public string StrReason { get; set; }
        public string StrDailyOrMonthly { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public long? IntUpdatedBy { get; set; }
        //public long? IntPipelineHeaderId { get; set; }
        //public long? IntCurrentStage { get; set; }
        //public long? IntNextStage { get; set; }
        //public string? StrStatus { get; set; }
        //public bool? IsPipelineClosed { get; set; }
        //public bool? IsReject { get; set; }
        //public long? IntRejectedBy { get; set; }
        public bool? IsUpdate { get; set; } = false;
    }
    public class AttendanceSummaryViewModel
    {
        public long? WorkingDays { set; get; }
        public long? PresentDays { set; get; }
        public long? LateDays { set; get; }
        public long? AbsentDays { set; get; }
        public long? MovementDays { set; get; }
        public long? LeaveDays { set; get; }
        public List<AttendanceDailySummaryViewModel> attendanceDailySummaryViewModel { set; get; }
        public List<TimeAttendanceDailySummary> timeAttendanceDailySummaries { set; get; }
    }

    public class AttendanceDailySummaryViewModel
    {
        public string DayName { get; set; }
        public int? DayNumber { get; set; }
        public string presentStatus { get; set; }
        public DateTime? dteDate { get; set; }
    }

    public class CheckInCheckOutViewModel
    {
        public long? IntAutoId { get; set; }
        public long? IntDayId { get; set; }
        public long? IntMonthId { get; set; }
        public long? IntYear { get; set; }
        public DateTime? DteAttendanceDate { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? DutyHoursNMinute { get; set; }
    }
    public class EmployeeDaylyAttendanceReportLanding
    {
        public decimal? TotalEmployee { get; set; }
        public decimal? PresentCount { get; set; }
        public decimal? ManualPresentCount { get; set; }
        public decimal? AbsentCount { get; set; }
        public decimal? LateCount { get; set; }
        public decimal? LeaveCount { get; set; }
        public decimal? MovementCount { get; set; }
        public decimal? WeekendCount { get; set; }
        public decimal? HolidayCount { get; set; }
        public string? AttendanceDate { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }

        public string? CompanyAddress { get; set; }
        public long? CompanyLogoUrlId { get; set; }
        public string? BusinessUnitName { get; set; }

        public long BusinessUnitId { get; set; }

        public string? WorkplaceGroup { get; set; }
        public string? Workplace { get; set; }

        public IList<DepartmentVM> departmentVM { get; set; }
        public IEnumerable<EmpAttendanceSummaryListVM> Data { get; set; }
    }
    public class EmpAttendanceSummaryListVM
    {
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public long? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public bool? Present { get; set; }
        public bool? Absent { get; set; }
        public bool? Late { get; set; }
        public bool? Leave { get; set; }
        public bool? Movement { get; set; }
        public bool? Weekend { get; set; }
        public bool? Holiday { get; set; }
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public string? DutyHours { get; set; }
        public string? ActualStatus { get; set; }
        public string? ManualStatus { get; set; }
        public string? Location { get; set; }
        public string? CalendarName { get; set; }
        public string? Remarks { get; set; }
        public long? TotalCount { get; set; }
    }
    public class DailyAttendanceReportVM
    {
        public string? BusinessUnit { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? CompanyLogoUrlId { get; set; }
        public string? CompanyAddress { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? WorkplaceGroup { get; set; }
        public string? Workplace { get; set; }
        public string? Department { get; set; }
        public decimal? TotalEmployee { get; set; }
        public decimal? PresentCount { get; set; }
        public decimal? ManualPresentCount { get; set; }
        public decimal? AbsentCount { get; set; }
        public decimal? LateCount { get; set; }
        public decimal? LeaveCount { get; set; }
        public decimal? MovementCount { get; set; }
        public decimal? WeekendCount { get; set; }
        public decimal? HolidayCount { get; set; }
        public List<EmployeeAttendanceSummaryViewModel> EmployeeAttendanceSummaryVM { get; set; }
    }

    public class EmployeeAttendanceSummaryViewModel
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public bool? Present { get; set; }
        public bool? Absent { get; set; }
        public bool? Late { get; set; }
        public bool? Leave { get; set; }
        public bool? Movement { get; set; }
        public bool? Weekend { get; set; }
        public bool? Holiday { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? DutyHours { get; set; }
        public string? ActualStatus { get; set; }
        public string? ManualStatus { get; set; }
        public string? Location { get; set; }
        public string? CalendarName { get; set; }
        public string? Remarks { get; set; }
        public long? TotalCount { get; set; }
    }
    public class TimeSheetEmpAttenReportLanding
    {
        public decimal? PresentCount { get; set; }
        public decimal? AbsentCount { get; set; }
        public decimal? LateCount { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public IEnumerable<TimeSheetEmpAttenListVM> Data { get; set; }
    }
    public class TimeSheetEmpAttenListVM
    {
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public decimal? WorkingDays { get; set; }
        public decimal? Present { get; set; }
        public decimal? Absent { get; set; }
        public decimal? Late { get; set; }
        public decimal? Leave { get; set; }
        public decimal? Movement { get; set; }
        public decimal? Weekend { get; set; }
        public decimal? Holiday { get; set; }
        public decimal? CasualLeave { get; set; }
        public decimal? MedicalLeave { get; set; }
        public decimal? LeaveWithoutPay { get; set; }
        public long? TotalCount { get; set; }
        public string SalaryStatus { get; set; }
    }
    public class SalaryTaxCertificateViewModel
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmploymentType { get; set; }
        public string? BusinessUnit { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public decimal? TotalSalary { get; set; }
        public string? FiscalyearName { get; set; }
        public decimal? TaxAmount { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string? Circle { get; set; }
        public string? ZoneName { get; set; }
        public string? ChallanNo { get; set; }
        public long? BankId { get; set; }
        public string? BankName { get; set; }
        public DateTime? TaxPaidDate { get; set; }
        public string? Fiscalyear { get; set; }
        public List<PayrollElementVM>? payrollElementVMs { get; set; }
    }

    public class PayrollElementVM
    {
        public long? PayrollElementId { get; set; }
        public string? PayrollElement { get; set; }
        public decimal? NumAmount { get; set; }
    }

    public class LoanViewModel
    {
        public string? PartType { get; set; }
        public long? LoanApplicationId { get; set; }
        public long? EmployeeId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkPlaceGrop { get; set; }
        public long? LoanTypeId { get; set; }
        public long? LoanAmount { get; set; }
        public long? IntAccountId { get; set; }
        public long? NumberOfInstallment { get; set; }
        public long? NumberOfInstallmentAmount { get; set; }

        public long? IntApproveLoanAmount { get; set; }
        public long? IntApproveNumberOfInstallment { get; set; }
        public long? IntApproveNumberOfInstallmentAmount { get; set; }

        public DateTime? ApplicationDate { get; set; }
        public bool? IsApprove { get; set; }
        public decimal? RemainingBalance { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Description { get; set; }
        public long? FileUrl { get; set; }
        public string? ReferenceNo { get; set; }
        public bool? IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? InsertDateTime { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdateDateTime { get; set; }
    }

    public class LoanApprovelViewModel
    {
        public long? ApplicationId { get; set; }

        //public long? EmployeeId { get; set; }
        public long? ApproverEmployeeId { get; set; } // from back end

        public bool IsReject { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long AccountId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class LoanApproveLandingViewModel
    {
        public long? LoanApplicationId { get; set; }
        public bool? IsApprove { get; set; }
        public string? ApproveBy { get; set; }
        public long? ApproveLoanAmount { get; set; }
        public long? ApproveNumberOfInstallment { get; set; }
        public long? ApproveNumberOfInstallmentAmount { get; set; }
        public DateTime? ApproveDate { get; set; }
        public bool? IsReject { get; set; }
        public string? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public long? UpdateByUserId { get; set; }
        public long? ReScheduleNumberOfInstallment { get; set; }
        public long? ReScheduleNumberOfInstallmentAmount { get; set; }
        public string? ReScheduleRemarks { get; set; }
        public DateTime? ReScheduleDateTime { get; set; }
    }

    public class LoanApplicationByAdvanceFilterViewModel
    {
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? LoanTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? EmployeeId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumAmount { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? InstallmentStatus { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public bool Ispaginated { get; set; }
        public string? SearchText { get; set; }
    }

    public class LoanReportByAdvanceFilterViewModel
    {
        public long? LoanApplicationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? LoanTypeId { get; set; }
        public long? LoanAmount { get; set; }
        public long? NumberOfInstallment { get; set; }
        public long? NumberOfInstallmentAmount { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public bool? IsApprove { get; set; }
        public string? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public long? ApproveLoanAmount { get; set; }
        public long? ApproveNumberOfInstallment { get; set; }
        public decimal? RemainingBalance { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Description { get; set; }
        public long? FileUrl { get; set; }
        public string? StrReferenceId { get; set; }
        public bool? isActive { get; set; }
        public bool? isReject { get; set; }
        public string? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public long? CreatedByUserId { get; set; }
        public DateTime? CreatedatDateTime { get; set; }
        public long? UpdateByUserId { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrDesignation { get; set; }
        public string? PositonGroupName { get; set; }
        public string? PositionName { get; set; }
        public string? LoanType { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? InstallmentStatus { get; set; }
    }

    public class CustomViewModel
    {
        public long? IntEmployeeId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartmentName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignationName { get; set; }
        public long? IntSupervisorId { get; set; }
        public string? SupervisorName { get; set; }
    }

    public class CommonEmployeeDDL
    {
        public long EmployeeId { get; set; } = 0;
        public string EmployeeName { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public string EmployeeNameWithCode { get; set; } = "";
        public long EmploymentTypeId { get; set; } = 0;
        public string EmploymentType { get; set; } = "";
        public string DesignationName { get; set; } = "";
        public long Designation { get; set; } = 0;

    }
    public class EmployeeQryProfileAllViewModel
    {
        public EmpEmployeeBasicInfo EmployeeBasicInfo { get; set; }
        public MasterBusinessUnit BusinessUnit { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long? IntSupervisorImageUrlId { set; get; }
        public long? DottedSupervisorId { get; set; }
        public string? DottedSupervisorName { get; set; }
        public long? IntDottedSupervisorImageUrlId { set; get; }
        public long? LineManagerId { get; set; }
        public string? LineManagerName { get; set; }
        public long? IntLineManagerImageUrlId { get; set; }

        public long? EmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
    }

    public class MonthlySalaryDepartmentWiseReportViewModel
    {
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long ManPowerCount { get; set; }
        public decimal? OverTimeAmount { get; set; }
        public decimal? ExtraSideDutyAmount { get; set; }
        public decimal? NightDutyAmount { get; set; }
        public decimal? AttendanceBonusAmount { get; set; }
        public decimal? Salary { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? CbaAmount { get; set; }
        public decimal? DeductAmount { get; set; }
        public decimal? NetPayable { get; set; }
        public List<PyrSalaryGenerateHeader> SalaryGenerateHeaderList { get; set; }
    }

    public class LeaveBalanceHistoryForAllEmployeeViewModel
    {
        public long? EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public long? CLBalance { get; set; }
        public long? CLTaken { get; set; }
        public long? SLBalance { get; set; }
        public long? SLTaken { get; set; }
        public long? ELBalance { get; set; }
        public long? ELTaken { get; set; }
        public long? LWPBalance { get; set; }
        public long? LWPTaken { get; set; }
        public long? MLBalance { get; set; }
        public long? MLTaken { get; set; }
    }

    public class MovementReportViewModel
    {
        public string? AutoId { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public string? EmployemntTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
        public string? Duration { get; set; }
    }

    public class OvertimeDetails
    {
        public long intMaxOverTimeDaily { get; set; }
        public long intMaxOverTimeMonthly { get; set; }
        public long intOtbenefitsHour { get; set; }
        public long intOtAmountShouldBe { get; set; }
        public List<OvertimeReport> overtimeReports { get; set; }
    }

    public class OvertimeReport
    {
        public long? intAutoId { get; set; }
        public long? intEmployeeId { get; set; }
        public string? strEmployeeCode { get; set; }
        public string? strEmployeeName { get; set; }
        public string? strDesignationName { get; set; }
        public string? strDepartmentName { get; set; }
        public string? EmployementType { get; set; }
        public decimal? numGrossAmount { get; set; }
        public decimal? numBasicSalary { get; set; }
        public DateTime? dteOverTimeDate { get; set; }
        public DateTime? dteAttendanceDate { get; set; }
        public TimeSpan? timeStartTime { get; set; }
        public TimeSpan? timeEndTime { get; set; }
        public decimal? numHours { get; set; }
        public decimal? numPerMinunitRate { get; set; }
        public decimal? numTotalAmount { get; set; }
        public string? strReason { get; set; }
        public long TotalCount { get; set; }
    }

    public class OvertimeReportViewModel
    {
        public long? EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? EmployeeCode { get; set; }
        public DateTime? OverTimeDate { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? EmployementType { get; set; }
        public decimal? Salary { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? Hours { get; set; }
        public decimal? PerHourRate { get; set; }
        public decimal? PayAmount { get; set; }
        public long totalCount { get; set; }
    }

    public class AllEmployeeListWithFilterViewModel
    {
        public string PartName { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long EmployeeId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long PayrollGroupId { get; set; }
        public long DepartmentId { get; set; }
        public long DesignationId { get; set; }
        public long SupervisorId { get; set; }
        public long RosterGroupId { get; set; }
        public long CalendarId { get; set; }
        public long GenderId { get; set; }
        public long ReligionId { get; set; }
        public long EmploymentTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public long IsGivenNid { get; set; }
        public long IsGivenBirthCertificate { get; set; }
        public string? SearchTxt { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
    }

    public class EmployeeRejoinFromSeparationVM
    {
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long SeparationId { get; set; }
        public bool IsRejoin { get; set; }
    }
    public class EmployeeSeaprationLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public List<EmployeeSeparationLandingVM> Data { get; set; }
    }
    public class EmployeeSeparationLandingVM
    {
        public long? SeparationId { get; set; }
        public long? intEmployeeId { get; set; }
        public string? strEmployeeName { get; set; }
        public string? strEmployeeCode { get; set; }
        public long? intSeparationTypeId { get; set; }
        public string? strSeparationTypeName { get; set; }
        public DateTime? dteSeparationDate { get; set; }
        public DateTime? dteLastWorkingDate { get; set; }
        public string? strReason { get; set; }
        public bool? IsReject { get; set; }
        public bool? isActive { get; set; }
        public DateTime? dteCreatedAt { get; set; }
        public DateTime? dteUpdatedAt { get; set; }
        public long? intCreatedBy { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? strEmploymentType { get; set; }
        public string? strDesignation { get; set; }
        public string? strDepartment { get; set; }
        public string? dteJoiningDate { get; set; }
        public string? ServiceLength { get; set; }
        public string? NoticePeriod { get; set; }
        public string? StrDocumentId { get; set; }
        public bool? IsReleased { get; set; }
        public bool? IsRejoin { get; set; }
    }

    public class EmployeeSeparationListFilterViewModel
    {
        public string? TableName { get; set; }
        public long AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public string? Status { get; set; }
        public long? IntSeparationId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? SupervisorId { get; set; }
        public long? EmployeeId { get; set; }
        public long? SeparationTypeId { get; set; }
        public DateTime? ApplicationFromDate { get; set; }
        public DateTime? ApplicationToDate { get; set; }
        public bool? IsForXl { get; set; }
        public string? SearchTxt { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
    }

    public class HolidayNExceptionFilterViewModel
    {
        //public long accountId { get; set; }
        public long businessUnitId { get; set; }
        public long workplaceGroupId { get; set; }
        public long? workplaceId { get; set; }
        public bool? IsNotAssign { get; set; }
        //public long EmployeeId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? searchTxt { get; set; }

        public List<long>? DesignationList { get; set; }
        public List<long>? DepartmentList { get; set; }
        public List<long>? SupervisorNameList { get; set; }
        //public List<long>? LinemanagerList { get; set; }
        //public List<long>? EmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class HolidayAndExceptionOffdayViewModel
    {
        public List<HolidayAssignViewModel> HolidayAssignDTOList { get; set; }
        public List<ExceptionOffdayAssignViewModel>? ExceptionOffdayAssignDTOList { get; set; }
    }

    public class HolidayAssignVm
    {
        public string EmployeeList { get; set; }
        public long? HolidayGroupId { get; set; }
        public string? HolidayGroupName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public bool? IsActive { get; set; }
        public long ActionBy { get; set; }
    }
    public class HolidayAssignViewModel
    {
        public long? EmployeeHolidayAssignId { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? PinNo { get; set; }
        public long? DepartmentId { get; set; }
        public string? Department { get; set; }
        public long? DesignationId { get; set; }
        public string? Designation { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long? HolidayGroupId { get; set; }
        public string? HolidayGroupName { get; set; }
        public long? EmploymentStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long? ProfileImageUrl { get; set; }
        public long? ExceptionOffdayGroupId { get; set; }
        public string? ExceptionOffdayGroupName { get; set; }
        public DateTime? ExceptionEffectiveDate { get; set; }
        public DateTime? HolidayEffectiveDate { get; set; }
        public bool? IsAlternativeDay { get; set; }        
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
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
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class HolidayAssignLandingPaginationViewModelWithHeader : PaginationBaseVM
    {
        public string employeeList { get; set; }
        public List<HolidayAssignViewModel> Data { get; set; }
        public HolidayAssignHeader holidayAssignHeader { get; set; }
    }
    public class HolidayAssignHeader
    {
        public IList<CommonDDLVM> DesignationList { get; set; }
        public IList<CommonDDLVM> DepartmentList { get; set; }
        public IList<CommonDDLVM> SupervisorNameList { get; set; }
        //public IList<CommonDDLVM> StrLinemanagerList { get; set; }
        //public IList<CommonDDLVM> StrEmploymentTypeList { get; set; }
        public IList<CommonDDLVM> WingNameList { get; set; }
        public IList<CommonDDLVM> SoleDepoNameList { get; set; }
        public IList<CommonDDLVM> RegionNameList { get; set; }
        public IList<CommonDDLVM> AreaNameList { get; set; }
        public IList<CommonDDLVM> TerritoryNameList { get; set; }
    }
    #region ======== Calendar Assign =======
    public class CalendarAssignFilterViewModel
    {
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long? WorkplaceId { get; set; }
        public bool? IsNotAssign { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? SearchTxt { get; set; }
        public List<long>? DesignationList { get; set; }
        public List<long>? DepartmentList { get; set; }
        public List<long>? SupervisorNameList { get; set; }
        public List<long>? EmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }
    public class CalendarAssignLandingViewModel
    {
        public long? CalendarAssignId { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? PinNo { get; set; }
        public long? DepartmentId { get; set; }
        public string? Department { get; set; }
        public long? DesignationId { get; set; }
        public string? Designation { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long? RosterGroupId { get; set; }
        public string? RosterGroupName { get; set; }
        public long? EmploymentTypeId { get; set; }
        public string? EmploymentType { get; set; }
        public long? EmploymentStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long? ProfileImageUrl { get; set; }
        public string? CalendarType { get; set; }
        public string? CalendarName { get; set; }
        public DateTime? GenerateDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
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
    public class CalendarAssignLandingPaginationViewModelWithHeader : PaginationBaseVM
    {
        public List<CalendarAssignLandingViewModel> Data { get; set; }
        public CalendarAssignHeader calendarAssignHeader { get; set; }
        public string? employeeIdList { get; set; }
    }
    public class CalendarAssignHeader
    {
        public IList<CommonDDLVM> DesignationList { get; set; }
        public IList<CommonDDLVM> DepartmentList { get; set; }
        public IList<CommonDDLVM> SupervisorNameList { get; set; }
        public IList<CommonDDLVM> EmploymentTypeList { get; set; }
        public IList<CommonDDLVM> WingNameList { get; set; }
        public IList<CommonDDLVM> SoleDepoNameList { get; set; }
        public IList<CommonDDLVM> RegionNameList { get; set; }
        public IList<CommonDDLVM> AreaNameList { get; set; }
        public IList<CommonDDLVM> TerritoryNameList { get; set; }
    }
    #endregion

    #region ======== Offday Assign =========
    public class OffdayLandingFilterViewModel
    {
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long? WorkplaceId { get; set; }
        public bool? IsAssign { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? SearchTxt { get; set; }
        public List<long>? DesignationList { get; set; }
        public List<long>? DepartmentList { get; set; }
        public List<long>? SupervisorNameList { get; set; }
        public List<long>? EmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class OffdayAssignLandingViewModel
    {
        public long? EmployeeOffdayAssignId { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? PinNo { get; set; }
        public long? DepartmentId { get; set; }
        public string? Department { get; set; }
        public long? DesignationId { get; set; }
        public string? Designation { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long? EmploymentTypeId { get; set; }
        public string? EmploymentType { get; set; }
        public long? EmploymentStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long? ProfileImageUrl { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsSaturday { get; set; }
        public bool? IsSunday { get; set; }
        public bool? IsMonday { get; set; }
        public bool? IsTuesday { get; set; }
        public bool? IsWednesday { get; set; }
        public bool? IsThursday { get; set; }
        public bool? IsFriday { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
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
    public class OffdayAssignLandingPaginationViewModelWithHeader : PaginationBaseVM
    {
        public string employeeList { get; set; }
        public List<OffdayAssignLandingViewModel> Data { get; set; }
        public OffdayAssignHeader offdayAssignHeader { get; set; }
    }
    public class OffdayAssignHeader
    {
        public IList<CommonDDLVM> DesignationList { get; set; }
        public IList<CommonDDLVM> DepartmentList { get; set; }
        public IList<CommonDDLVM> SupervisorNameList { get; set; }
        public IList<CommonDDLVM> EmploymentTypeList { get; set; }
        public IList<CommonDDLVM> WingNameList { get; set; }
        public IList<CommonDDLVM> SoleDepoNameList { get; set; }
        public IList<CommonDDLVM> RegionNameList { get; set; }
        public IList<CommonDDLVM> AreaNameList { get; set; }
        public IList<CommonDDLVM> TerritoryNameList { get; set; }
    }
    #endregion

    public class ExceptionOffdayAssignViewModel
    {
        public long? EmployeeOffdayAssignId { get; set; }
        public long? EmployeeId { get; set; }
        public long? ExceptionOffdayGroupId { get; set; }
        public string? ExceptionOffdayGroupName { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }



    public class RosterGenerateViewModel
    {
        public string EmployeeList { get; set; }
        public DateTime GenerateStartDate { get; set; }
        public DateTime? GenerateEndDate { get; set; }
        public long IntCreatedBy { get; set; }
        public long RunningCalendarId { get; set; }
        public string CalendarType { get; set; }
        public long? RosterGroupId { get; set; }
        public DateTime? NextChangeDate { get; set; }
        public bool? IsAutoGenerate { get; set; }
    }



    public class OffdayAssignViewModel
    {
        //public long? EmployeeOffdayAssignId { get; set; }
        public string EmployeeList { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsSaturday { get; set; }
        public bool? IsSunday { get; set; }
        public bool? IsMonday { get; set; }
        public bool? IsTuesday { get; set; }
        public bool? IsWednesday { get; set; }
        public bool? IsThursday { get; set; }
        public bool? IsFriday { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public bool? IsActive { get; set; }
        public long ActionBy { get; set; }
    }

    public class AttendanceAdjustmentFilterViewModel
    {
        public long? EmployeeId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? DepartmentId { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string? AttendanceStatus { get; set; }
        public string? PunchStatus { get; set; }
        public long? JobTypeId { get; set; }
        public long BusinessUnitId { get; set; }
        public long? YearId { get; set; }
        public long? MonthId { get; set; }
    }

    public class ExtraSideDutyViewModel
    {
        public long PartId { get; set; }
        public long ExtraSideDutyId { get; set; }
        public long BusinessUnitId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long DutyCount { get; set; }
        public DateTime DutyDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public long InsertBy { get; set; }
        public long? AccountId { get; set; }
    }

    public class OverTimeFilterViewModel
    {
        public string? StrPartName { get; set; }
        public long? IntOverTimeId { get; set; }
        public string? Status { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? SupervisorId { get; set; }
        public long? EmployeeId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long BusinessUnitId { get; set; }
        public long? LoggedEmployeeId { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class TimeAttendanceDailySummeryVM
    {
        public long intAutoId { get; set; }
        public decimal? Minutes { get; set; }
        public long? ActionBy { get; set; }
    }

    #region LEAVE MOVEMENT

    public class LeaveDTO
    {
        public long? ViewType { get; set; }
        public long? EmployeeId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? ApplicantId { get; set; }
        public long? LeaveTypeId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public long? ApplicationId { get; set; }
    }

    public class LeaveApplicationDTO
    {
        public long PartId { get; set; }
        public long LeaveApplicationId { get; set; }
        public long LeaveTypeId { get; set; }
        public long EmployeeId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime AppliedFromDate { get; set; }
        public DateTime AppliedToDate { get; set; }
        public long DocumentFile { get; set; }
        public string LeaveReason { get; set; }
        public string AddressDuetoLeave { get; set; }
        public long InsertBy { get; set; }
    }

    public class LeaveDataSetDTO
    {
        public long? intApplicationId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { get; set; }
        public long? intApproverId { get; set; }
        public string? strEmployeeCode { get; set; }
        public string? strEmployeeName { get; set; }
        public string? strLeaveReason { get; set; }
        public DateTime? dteAppliedFromDate { get; set; }
        public DateTime? dteAppliedToDate { get; set; }
        public string? strDocumentFile { get; set; }
        public long? intLeaveTypeId { get; set; }
        public string? strLeaveType { get; set; }
        public long? intTotalConsumeLeave { get; set; }
        public long? intRemainingDays { get; set; }
        public long? intAllTypeLeaveConsume { get; set; }

        public string? strDepartmentName { get; set; }
        public string? strDesignationName { get; set; }
        public string? strEmploymentTypeName { get; set; }
        public long? intDepartmentId { get; set; }
        public long? intDesignationId { get; set; }
        public string? strAddressDuetoLeave { get; set; }
        public DateTime? dteApplicationDate { get; set; }
        public long? totalDays { get; set; }
        public string? strStatus { get; set; }
        public string? strViewAs { get; set; }
    }

    public class MovementApplicationDTO
    {
        public long PartId { get; set; }
        public long MovementId { get; set; }
        public long IntEmployeeId { get; set; }
        public long MovementTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Reason { get; set; }
        public string Location { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public bool IsActive { get; set; }
        public long InsertBy { get; set; }
    }

    public class MovementDataSetDTO
    {
        public long? intMovementId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { get; set; }
        public long? intApproverId { get; set; }
        public string? strEmployeeCode { get; set; }
        public string? strEmployeeName { get; set; }
        public string? strReason { get; set; }
        public DateTime? dteFromDate { get; set; }
        public DateTime? dteToDate { get; set; }
        public string? dteFromTime { get; set; }
        public string? dteTimeTo { get; set; }
        public string? strDepartmentName { get; set; }
        public string? strDesignationName { get; set; }
        public long? intDesignationId { get; set; }
        public long? intDepartmentId { get; set; }
        public string? strEmploymentTypeName { get; set; }
        public string? strMovementType { get; set; }
        public DateTime? dteApplicationDate { get; set; }
        public string? strLocation { get; set; }
        public string? strStatus { get; set; }
        public string? strViewAs { get; set; }
    }

    public class LeaveAndMovementApprovedDTO
    {
        public long? ApplicationId { get; set; }

        //public long? EmployeeId { get; set; }
        public long? ApproverEmployeeId { get; set; } // from back end

        public bool IsReject { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long AccountId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class LeaveMovementTypeDTO
    {
        public long PartId { get; set; }
        public bool IsLeave { get; set; }
        public long LeaveMovementAutoId { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveType { get; set; }
        public long AccountId { get; set; }
        public bool IsPayable { get; set; }
        public decimal PercentPayable { get; set; }
        public bool IsActive { get; set; }
        public string InsertUser { get; set; }
        public string MovementType { get; set; }
        public decimal MovementMonthlyAllowedHour { get; set; }
    }

    public class CRUDEmploymentTypeWiseLeaveBalanceDTO
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
        public string InsertUserId { get; set; }
    }

    //public class MovementReportDTO
    //{
    //    public string? AutoId { get; set; }
    //    public string? EmployeeId { get; set; }
    //    public string? EmployeeName { get; set; }
    //    public string? DepartmentId { get; set; }
    //    public string? DepartmentName { get; set; }
    //    public string? DesignationId { get; set; }
    //    public string? DesignationName { get; set; }
    //    public string? EmployemntTypeId { get; set; }
    //    public string? EmploymentTypeName { get; set; }
    //    public string? Duration { get; set; }

    //}

    #endregion LEAVE MOVEMENT

    public class GetDailyCafeteriaReportViewModel
    {
        //public long MyProperty { get; set; }
        public long EmployeeId { get; set; }

        public string? Referenceid { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeFullName { get; set; }
        public long? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string MealDate { get; set; }
        public long? MealCount { get; set; }
    }

    #region ======= SalaryManagement =======

    public class EmployeeSalaryManagementDTO
    {
        public string? PartType { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? SupervisorId { get; set; }
        public long? EmployeeId { get; set; }
        public string? StrStatus { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? StrSearchTxt { get; set; }
        public long? IntBreakdownHeaderId { get; set; }
        public bool IsPaginated { get; set; }
    }

    public class SalaryAssignDTO
    {
        public long? EmployeeSalaryDefaultId { get; set; }
        public long? EmployeeId { get; set; }
        public long? EmploymentTypeId { get; set; }
        public long? PayrollGroupId { get; set; }
        public long? PayscaleGradeId { get; set; }
        public long? PayscaleGradeAdditionId { get; set; }
        public decimal? Basic { get; set; }
        public decimal? HouseAllowance { get; set; }
        public decimal? MedicalAllowance { get; set; }
        public decimal? ConveyanceAllowance { get; set; }
        public decimal? WashingAllowance { get; set; }
        public decimal? CBADeduction { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? TotalSalary { get; set; }
        public long? EffectiveMonth { get; set; }
        public long? EffectiveYear { get; set; }
        public decimal? reqBasic { get; set; }
        public decimal? reqHouseAllowance { get; set; }
        public decimal? reqMedicalAllowance { get; set; }
        public decimal? reqConveyanceAllowance { get; set; }
        public decimal? reqWashingAllowance { get; set; }
        public decimal? reqCBADeduction { get; set; }
        public decimal? reqSpecialAllowance { get; set; }
        public decimal? reqGrossSalary { get; set; }
        public decimal? reqTotalSalary { get; set; }
        public bool isActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public bool IsHold { get; set; }
        public long? IntLogCreatedByUserId { get; set; }
        public DateTime? DteLogInsertCreatedAt { get; set; }
        public List<OthersSalaryDTO> OthersSalaryList { get; set; }
    }

    public class OthersSalaryDTO
    {
        public long? EmployeeSalaryOtherId { get; set; }
        public long? EmployeeId { get; set; }
        public long? PayrollElementId { get; set; }
        public string? PayrollElementTypeCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? reqAmount { get; set; }
        public int? ActionTypeId { get; set; } // 1 for create ======== 2 for eidt ========= 3 for delete
    }

    public class SalaryGenerateRequestDTO
    {
        public string ReportType { get; set; }
        public long BusinessUnitId { get; set; }
        public string BusinessUnit { get; set; }
        public long WorkplaceGroupId { get; set; }
        public string WorkplaceGroup { get; set; }
        public long PayrollGroupId { get; set; }
        public string PayrollGroup { get; set; }
        public long MonthId { get; set; }
        public long YearId { get; set; }
        public string GenerateByUserId { get; set; }
    }

    public class EmpSalaryCertificateRequestViewModel
    {
        public long IntSalaryCertificateRequestId { get; set; }
        public string strEntryType { get; set; }
        public long IntEmployeeId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public string StrEmployeName { get; set; } = null!;
        public long? IntPayRollMonth { get; set; }
        public long? IntPayRollYear { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
    #region ========= Expense VM =========
    public class ExpenseApplicationViewModel
    {
        public long? intExpenseId { set; get; }
        public long? intAccontId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { set; get; }
        public long? intExpenseTypeId { set; get; }
        public string? strEntryType { get; set; }
        public DateTime dteExpenseFromDate { get; set; }
        public DateTime dteExpenseToDate { get; set; }
        public string? strDiscription { get; set; }
        public decimal? numExpenseAmount { set; get; }
        public bool? isActive { set; get; }
        public long? intCreatedBy { set; get; }
        public DateTime? dteCreatedAt { get; set; }
        public List<UrlIdViewModel>? UrlIdViewModelList { set; get; }
    }

    public class ExpenseApplicationLandingPaginetionVM : PaginationBaseVM
    {
        public List<ExpenseApplicationLandingVM> expenseApplicationLandings { get; set; }
    }
    public class ExpenseApplicationLandingVM
    {
        public long ExpenseId { set; get; }
        public long intEmployeeId { set; get; }
        public string EmployeeName { set; get; }
        public string employeeCode { set; get; }
        public string strDesignation { set; get; }
        public string strDepartment { set; get; }
        public long intExpenseTypeId { set; get; }
        public string strExpenseType { set; get; }
        public DateTime? dteExpenseFromDate { set; get; }
        public DateTime? dteExpenseToDate { set; get; }
        public string strDiscription { set; get; }
        public decimal? numExpenseAmount { set; get; }
        public bool? isActive { set; get; }
        public string? Status { set; get; }
        public long? RejectedBy { set; get; }
        public long intCreatedBy { get; set; }
        public DateTime? dteCreatedAt { set; get; }
    }
    #endregion

    #region ======= IOU VM =======
    public class IOUApplicationLandingPaginetionVM : PaginationBaseVM
    {
        public List<IOUApplicationLandingVM> iouApplicationLandings { get; set; }
    }
    public class IOUApplicationLandingVM
    {
        public long? IOUId { set; get; }
        public string? IOUCode { set; get; }
        public long EmployeeId { set; get; }
        public string EmployeeName { set; get; }
        public string EmployeeCode { set; get; }
        public long? businessUnitId { set; get; }
        public long? workplaceGroupId { set; get; }
        public DateTime? ApplicationDate { set; get; }
        public DateTime? dteFromDate { set; get; }
        public DateTime? dteToDate { set; get; }
        public decimal? numIOUAmount { set; get; }
        public decimal? numAdjustedAmount { set; get; }
        public decimal? numPayableAmount { set; get; }
        public decimal? numReceivableAmount { set; get; }
        public string? Status { set; get; }
        public string? AdjustmentStatus { set; get; }
        public string? Discription { set; get; }
        public decimal? PendingAdjAmount { set; get; }
        public bool? isActive { set; get; }
        public long? intCreatedBy { set; get; }
        public long? intUpdatedBy { set; get; }
        public bool? isAdjustment { get; set; }
        public long? intIOUAdjustmentId { get; set; }
    }
    public class IOUApplicationViewModel
    {
        public string strEntryType { set; get; }
        public long? intIOUId { set; get; }
        public long intEmployeeId { set; get; }
        public long intBusinessUnitId { set; get; }
        public long intWorkplaceGroupId { set; get; }
        public DateTime? dteFromDate { set; get; }
        public DateTime? dteToDate { set; get; }
        public decimal? numIOUAmount { set; get; }
        public decimal? numAdjustedAmount { set; get; }
        public decimal? numPayableAmount { set; get; }
        public decimal? numReceivableAmount { set; get; }
        public string? strDiscription { set; get; }
        public bool? isActive { set; get; }
        public bool? isAdjustment { get; set; }
        public long? intIOUAdjustmentId { get; set; }
        public List<UrlIdViewModel>? UrlIdViewModelList { set; get; }
    }

    public class UrlIdViewModel
    {
        public long? intDocURLId { set; get; }
    }
    #endregion
    public class EmployeeSalaryAssignViewModel
    {
        public List<EmployeeId> IntEmployeeIdList { set; get; }
        public long IntSalaryBreakdownHeaderId { set; get; }
        public string StrSalaryBreakdownHeaderTitle { set; get; }
        public long? IntPayrollGroupId { set; get; }
        public string? StrPayrollGroupName { set; get; }
        public decimal NumBasicORGross { set; get; }
        public decimal NumGrossAmount { set; get; }
        public decimal NumNetGrossSalary { set; get; }
        public long EffectiveYear { set; get; }
        public long EffectiveMonth { set; get; }
        public long IntCreateBy { set; get; }
        public bool IsPerdaySalary { set; get; }
        public List<BreakdownElementType>? BreakdownElements { set; get; }
    }
    public class EmployeeId
    {
        public long? IntEmployeeId { get; set; }
    }
    public class BreakdownElementType
    {
        public long? intSalaryBreakdownRowId { set; get; }
        public long? intPayrollElementTypeId { set; get; }
        public string? DependOn { set; get; }
        public decimal? numberOfPercent { set; get; }
        public decimal? numAmount { set; get; }
    }

    #endregion ======= SalaryManagement =======

    public class ContractualFromNToDateViewModel
    {
        public long? EmployeeId { set; get; }
        public DateTime? ContractFromDate { set; get; }
        public DateTime? ContractToDate { set; get; }
    }

    public class AllowanceNDeductionForSalaryGenerate
    {
        public long EmployeeId { get; set; }
        public long SalaryGenerateHeaderId { get; set; }
        public decimal? NumManualSalaryAddition { get; set; }
        public decimal? NumManualSalaryDeduction { get; set; }
    }

    public class EmployeeNotAssignForTaxViewModel
    {
        public string PartName { set; get; }
        public long IntAccountId { set; get; }
        public long IntBusinessUnitId { set; get; }
        //public List<ListOfEmployeeId> listOfEmployeeId { set; get; }
        public string listOfEmployeeId { set; get; }
    }

    public class ListOfEmployeeId
    {
        public long IntEmployeeId { set; get; }
    }

    public class SalaryGenerateViewModel
    {
        public string strPartName { set; get; }
        public long? intSalaryGenerateRequestId { set; get; }
        public string strSalaryCode { set; get; }
        //public long intAccountId { set; get; }
        public long intBusinessUnitId { set; get; }
        public string? strBusinessUnit { set; get; }
        public long? intWorkplaceGroupId { set; get; }
        public string? strWorkplaceGroup { set; get; }
        public long? intWingId { set; get; }
        public long? intSoleDepoId { set; get; }
        public long? intRegionId { set; get; }
        public long? intAreaId { set; get; }
        public string? territoryIdList { set; get; }
        public string? territoryNameList { set; get; }
        public long intMonthId { set; get; }
        public long intYearId { set; get; }
        public string? strDescription { set; get; }
        public long intCreatedBy { set; get; }
        public string? strSalryType { set; get; }
        public DateTime? dteFromDate { set; get; }
        public DateTime? dteToDate { set; get; }
        public string? strEmpIdList { get; set; }
        //public List<SalaryGenerateRequestRow>? generateRequestRows { set; get; }
    }

    //public class SalaryGenerateRequestRow
    //{
    //    public long? intEmployeeId { set; get; }
    //    public string? strEmployeeName { set; get; }
    //    public long? intPayrollGroupId { set; get; }
    //    public string? strPayrollGroup { set; get; }
    //    public long? intWingId { set; get; }
    //    public long? intSoleDepoId { set; get; }
    //    public long? intRegionId { set; get; }
    //    public long? intAreaId { set; get; }
    //    public long? intTerritoryId { set; get; }
    //}

    public class ArearSalaryGenerateVM
    {
        public string? strPartName { set; get; }
        public long? intArearSalaryGenerateRequestId { get; set; }
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public string? strBusinessUnit { get; set; } = null!;
        public DateTime? dteEffectiveFrom { get; set; }
        public DateTime? dteEffectiveTo { get; set; }
        public string? strDescription { get; set; }
        public long? intCreatedBy { get; set; }
        public long? intSalaryPolicyId { get; set; }
        public decimal? numPercentOfGross { get; set; }

        public List<EmpIdList>? EmployeeIdList { get; set; }
    }

    public class EmpIdList
    {
        public long? intEmployeeId { set; get; }
        public string? strEmployeeName { set; get; }
    }

    public class PayScaleGradeViewModel
    {
        public long IntPayscaleGradeId { get; set; }
        public string StrPayscaleGradeName { get; set; } = null!;
        public string? StrPayscaleGradeCode { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long IntDesignationId { get; set; }
        public string? StrDesignationName { get; set; }
        public long IntShortOrder { get; set; }
        public decimal? NumMinSalary { get; set; }
        public decimal? NumMaxSalary { get; set; }
        public string? StrDepentOn { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class EmployeeDocumentManagementViewModel
    {
        public long IntDocumentManagementId { get; set; }
        public long IntAccountId { get; set; }
        public long IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public long? IntDocumentTypeId { get; set; }
        public string? StrDocumentType { get; set; }
        public long? IntFileUrlId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class ElezableForJobConfigViewModel
    {
        public long IntJobConfigId { get; set; }
        public long IntAccountId { get; set; }
        public long IntFromEmploymentTypeId { get; set; }
        public string? FromEmploymentType { get; set; }
        public long? IntToEmploymentTypeId { get; set; }
        public string? ToEmploymentType { get; set; }
        public long IntServiceLengthDays { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }

    public class EmployeeAttendanceReportViewModel
    {
        public long EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? EmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
        public string? Email { get; set; }
        public string? BusinessUnitName { get; set; }
        public string? Phone { get; set; }
        public string? OfficialPhone { get; set; }
        public long? WorkingDays { get; set; }
        public long? Present { get; set; }
        public long? Absent { get; set; }
        public long? Movement { get; set; }
        public long? Holiday { get; set; }
        public long? offDay { get; set; }
        public long? Late { get; set; }
        public string? SalaryStatus { get; set; }
        public long TotalCount { get; set; }
        public List<LeaveTypeWiseCount> LeaveTypeWiseList { get; set; }
    }

    public class LeaveTypeWiseCount
    {
        public long? LeaveTypeId { get; set; }
        public long? EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public long? TotalLeave { get; set; }
    }

    public class MasterLocationAssaignApprovedDTO
    {
        public long? LocationId { get; set; }

        //public long? EmployeeId { get; set; }
        public long? ApproverEmployeeId { get; set; } // from back end

        public bool IsReject { get; set; }
        public long AccountId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    #region ==========Shift Management ===========
    public class CalendarAssignList
    {
        public List<long> IntEmployeeId { get; set; }
        public List<Shift> Shifts { get; set; }
        public long IntActionBy { get; set; }
    }
    public class Shift
    {
        public long IntCalenderId { get; set; }
        public string StrCalenderName { get; set; }
        public TimeSpan DteStartTime { get; set; }
        public TimeSpan DteExtendedStartTime { get; set; }
        public TimeSpan DteLastStartTime { get; set; }
        public TimeSpan DteEndTime { get; set; }
        public decimal NumMinWorkHour { get; set; }
        public TimeSpan? DteBreakStartTime { get; set; }
        public TimeSpan? DteBreakEndTime { get; set; }
        public TimeSpan DteOfficeStartTime { get; set; }
        public TimeSpan DteOfficeCloseTime { get; set; }
        public bool? IsNightShift { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
    }
    #endregion

    #region====Off day reassign====
    public class EmployeeOffDayReassignList
    {
        public List<long> IntEmployeeId { get; set; }
        public List<EmployeeOffDayReassign> Offdays { get; set; }
        public long IntActionBy { get; set; }
    }

    public class EmployeeOffDayReassign
    {
        public DateTime Date { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsActive { get; set; }
    }

    public class OffdayStatus
    {
        public DateTime DteDate { get; set; }
        public bool IsOffday { get; set; }
        public int DayId { get; set; }
        public string DayName { get; set; }
    }
    #endregion

    #region=== Master Fixed Roaster===
    public class FixedMasterRoaster
    {
        public long IntId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnit { get; set; }
        public string StrRoasterName { get; set; }
        public bool? IsActive { get; set; }
        public long IntActionBy { get; set; }

        public List<FixedRoasterVm> FixedRoasterDetails { get; set; }

    }
    public class FixedRoasterVm
    {
        public long IntId { get; set; }
        public long IntDay { get; set; }
        public long MasterId { get; set; }
        public long? IntCalendarId { get; set; }
        public string? StrCalendarName { get; set; }
        public bool? IsOffDay { get; set; }
        public bool? IsHoliday { get; set; }
        public bool IsActive { get; set; }
        public long IntActionBy { get; set; }
    }
    #endregion

    #region ==== Gross Wise Basic ====
    public class GrossWiseBasicViewModel
    {
        public long? IntGrossWiseBasicId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? BusinessUnit { get; set; }
        public decimal? NumMinGross { get; set; }
        public decimal? NumMaxGross { get; set; }
        public decimal? NumPercentageOfBasic { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreateBy { get; set; }
        public DateTime? DteCreateDate { get; set; }
    }

    public class GrossWiseBasicPercentageViewModel
    {
        public decimal? BasicSalary { set; get; }
        //public decimal? HouseAllowance { set; get; }
        public decimal? NumPercentageOfGross { set; get; }
    }

    #endregion

    #region ===== DivisionDistrictThanaPostOfficeViewModel ====
    public class DivisionDistrictThanaPostOfficeViewModel
    {
        public List<DivisionVM> DivisionVMs { get; set; }
        public List<DistrictVM> DistrictVMs { get; set; }
        public List<ThanaVM> ThanaVMs { get; set; }
        public List<PostOfficeVM> PostOfficeVMs { get; set; }
    }

    public class DivisionVM
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Bn_name { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
    }
    public class DistrictVM
    {
        public long? Id { set; get; }
        public long? Division_id { set; get; }
        public string? Name { set; get; }
        public string? Bn_name { set; get; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
    }
    public class ThanaVM
    {
        public long? Id { set; get; }
        public long? District_id { set; get; }
        public string? Name { set; get; }
        public string? Bn_name { set; get; }
    }
    public class PostOfficeVM
    {
        public long? Id { set; get; }
        public long? District_id { set; get; }
        public long? Division_id { set; get; }
        public string? Upazila { get; set; }
        public string? PostOffice { get; set; }
        public string? PostCode { get; set; }
    }
    #endregion
    public class Emp
    {
        public long IntEmployeeId { get; set; }
    }

    #region ========= Employee Landing for pagination =======
    public class profileLandingFilterVM
    {
        //public long accountId { get; set; }
        public long businessUnitId { get; set; }
        public long workplaceGroupId { get; set; }
        public long? workplaceId { get; set; }
        //public long EmployeeId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? searchTxt { get; set; }
        public List<long>? StrDesignationList { get; set; }
        public List<long>? StrDepartmentList { get; set; }
        public List<long>? StrSupervisorNameList { get; set; }
        public List<long>? StrLinemanagerList { get; set; }
        public List<long>? StrEmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class InactiveEmployeeLandingFilterVM
    {        
        public long businessUnitId { get; set; }
        public long workplaceGroupId { get; set; }
        public long? workplaceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? searchTxt { get; set; }
        public List<long>? StrDesignationList { get; set; }
        public List<long>? StrDepartmentList { get; set; }
        public List<long>? StrSupervisorNameList { get; set; }
        public List<long>? StrLinemanagerList { get; set; }
        public List<long>? StrEmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class EmployeeSeparationReportVM
    {
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long? WorkplaceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? SearchTxt { get; set; }
        public List<long>? StrDesignationList { get; set; }
        public List<long>? StrDepartmentList { get; set; }
        public List<long>? StrSupervisorNameList { get; set; }
        public List<long>? StrLinemanagerList { get; set; }
        public List<long>? StrEmploymentTypeList { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class EmployeeProfileLandingPaginationViewModelWithHeader : PaginationBaseVM
    {
        public dynamic Data { get; set; }
        public EmployeeHeader EmployeeHeader { get; set; }
    }
    public class EmployeeSeparationReportLanding : PaginationBaseVM
    {
        public dynamic Data { get; set; }
        public EmployeeHeader EmployeeHeader { get; set; }
    }
    public class EmployeeHeader
    {
        public IList<CommonDDLVM> StrDesignationList { get; set; }
        public IList<CommonDDLVM> StrDepartmentList { get; set; }
        public IList<CommonDDLVM> StrSupervisorNameList { get; set; }
        public IList<CommonDDLVM> StrLinemanagerList { get; set; }
        public IList<CommonDDLVM> StrEmploymentTypeList { get; set; }
        public IList<CommonDDLVM> WingNameList { get; set; }
        public IList<CommonDDLVM> SoleDepoNameList { get; set; }
        public IList<CommonDDLVM> RegionNameList { get; set; }
        public IList<CommonDDLVM> AreaNameList { get; set; }
        public IList<CommonDDLVM> TerritoryNameList { get; set; }
    }

    public class WorkplaceGroupVM
    {
        public long? Sl { get; set; }
        public long? workplaceGroupId { get; set; }
        public string? workplaceGroup { get; set; }
    }

    #endregion

    #region ==== Get Employee All Role And Extensions====
    public class RoleExtensionsList
    {
        public List<CommonDDLVM> BusinessUnitList { get; set; }
        public List<CommonDDLVM> WorkGroupList { get; set; }
        public List<CommonDDLVM> WorkPlaceList { get; set; }
        public List<CommonDDLVM> WingList { get; set; }
        public List<CommonDDLVM> SoleDepoList { get; set; }
        public List<CommonDDLVM> RegiontList { get; set; }
        public List<CommonDDLVM> AreaList { get; set; }
        public List<CommonDDLVM> TeritoryList { get; set; }
    }
    #endregion

    public class EmpJobConfirmationLanding
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public List<EmpJobConfirmationLandingVM> Data { get; set; }
    }
    public class EmpJobConfirmationLandingVM
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? ConfirmationDateRaw { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long? WorkplaceId { get; set; }
        public string? WorkplaceName { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? ProbationaryCloseDate { get; set; }
        public DateTime? InternCloseDate { get; set; }
        public string? ServiceLength { get; set; }
    }

    public class MonthlyRosterReportForSingleEmployee : PaginationBaseVM
    {
        public dynamic Data { get; set; }
    }

    public class EmployeeData
    {
        public int intEmployeeBasicInfoId { get; set; }
        public string EmployeeCode { get; set; }
        public string strEmployeeName { get; set; }
        public string strDepartment { get; set; }
        public string strDesignation { get; set; }
        public string strCalendarName { get; set; }
        public DateTime dteAttendanceDate { get; set; }
        public Dictionary<string, string> AttendanceDates { get; set; }
    }

    public class MovementApplicationViewModel
    {
        public long EmployeeBasicInfoId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; }
        public DateTime? RawFromDate { get; set; }
        public DateTime? RawToDate { get; set; }
        public long? RawDuration { get; set; }
    }
    public class AreaPermissionVM
    {
        public long AreaId { get; set; }
        public string AreaName { get; set; }
    }
}