using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeEmpOverTimeUpload
    {
        public long IntAutoId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrEmployeeCode { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long? IntDesignationId { get; set; }
        public string StrDesignationName { get; set; }
        public long IntYear { get; set; }
        public long IntMonth { get; set; }
        public long? IntDay { get; set; }
        public DateTime? DteOvertimeDate { get; set; }
        public long? IntFromHour { get; set; }
        public long? IntFromMinute { get; set; }
        public string StrFromAmPm { get; set; }
        public string StrFromTime { get; set; }
        public long? IntToHour { get; set; }
        public long? IntToMinute { get; set; }
        public string StrToAmPm { get; set; }
        public string StrToTime { get; set; }
        public bool IsMonthly { get; set; }
        public int? IntTotalHoursInMonth { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsValid { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public string StrInsertBy { get; set; }
        public string StrRemarks { get; set; }
    }
}
