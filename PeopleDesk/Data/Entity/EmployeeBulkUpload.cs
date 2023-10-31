using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmployeeBulkUpload
    {
        public long IntEmpBulkUploadId { get; set; }
        public long IntAccountId { get; set; }
        public long? IntUrlId { get; set; }
        public long? IntSlid { get; set; }
        public long? IntEmployeeId { get; set; }
        public string StrBusinessUnit { get; set; }
        public string StrWorkplaceGroup { get; set; }
        public string StrWorkplace { get; set; }
        public string StrDepartment { get; set; }
        public string StrDesignation { get; set; }
        public string StrHrPosition { get; set; }
        public string StrWingName { get; set; }
        public string StrSoleDepoName { get; set; }
        public string StrRegionName { get; set; }
        public string StrAreaName { get; set; }
        public string StrTerritoryName { get; set; }
        public string StrEmploymentType { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrEmployeeCode { get; set; }
        public string StrCardNumber { get; set; }
        public string StrGender { get; set; }
        public bool? IsSalaryHold { get; set; }
        public string StrReligionName { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public DateTime? DteInternCloseDate { get; set; }
        public DateTime? DteProbationaryCloseDate { get; set; }
        public DateTime? DteContactFromDate { get; set; }
        public DateTime? DteContactToDate { get; set; }
        public string StrSupervisorCode { get; set; }
        public string StrDottedSupervisorCode { get; set; }
        public string StrLineManagerCode { get; set; }
        public string StrLoginId { get; set; }
        public string StrPassword { get; set; }
        public string StrEmailAddress { get; set; }
        public string StrPhoneNumber { get; set; }
        public string StrDisplayName { get; set; }
        public string StrUserType { get; set; }
        public bool IsProcess { get; set; }
        public bool IsActive { get; set; }
        public long IntCreateBy { get; set; }
        public DateTime DteCreateAt { get; set; }
    }
}
