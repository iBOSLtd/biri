using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeBasicInfoDetail
    {
        public long IntDetailsId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrOfficeMail { get; set; }
        public string StrPersonalMail { get; set; }
        public string StrPersonalMobile { get; set; }
        public string StrOfficeMobile { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string StrPayrollGroupName { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public string StrPayscaleGradeName { get; set; }
        public long? IntCalenderId { get; set; }
        public string StrCalenderName { get; set; }
        public long? IntHrpositionId { get; set; }
        public string StrHrpostionName { get; set; }
        public long? IntWingId { get; set; }
        public long? IntSoleDepo { get; set; }
        public long? IntRegionId { get; set; }
        public long? IntAreaId { get; set; }
        public long? IntTerritoryId { get; set; }
        public long? IntEmployeeStatusId { get; set; }
        public string StrEmployeeStatus { get; set; }
        public string StrVehicleNo { get; set; }
        public string StrDrivingLicenseNo { get; set; }
        public string StrPinNo { get; set; }
        public bool? IsActive { get; set; }
        public string StrRemarks { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsTakeHomePay { get; set; }
    }
}
