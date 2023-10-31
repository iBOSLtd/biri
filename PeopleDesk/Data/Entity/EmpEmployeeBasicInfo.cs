using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeBasicInfo
    {
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrEmployeeCode { get; set; }
        public string StrCardNumber { get; set; }
        public string StrEmployeeName { get; set; }
        public long IntGenderId { get; set; }
        public string StrGender { get; set; }
        public long? IntReligionId { get; set; }
        public string StrReligion { get; set; }
        public string StrMaritalStatus { get; set; }
        public string StrBloodGroup { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntDesignationId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteConfirmationDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntLineManagerId { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public bool IsSalaryHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUserInactive { get; set; }
        public bool IsRemoteAttendance { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public string StrReferenceId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public DateTime? DteContactFromDate { get; set; }
        public DateTime? DteContactToDate { get; set; }
    }
}
