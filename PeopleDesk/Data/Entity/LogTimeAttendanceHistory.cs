using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LogTimeAttendanceHistory
    {
        public long IntAutoId { get; set; }
        public long? IntDayId { get; set; }
        public long? IntMonthId { get; set; }
        public long? IntYear { get; set; }
        public DateTime? DteAttendanceDate { get; set; }
        public long IntEmployeeId { get; set; }
        public long? IntCalendarTypeId { get; set; }
        public string StrCalendarType { get; set; }
        public long? IntCalendarId { get; set; }
        public string StrCalendarName { get; set; }
        public DateTime? DteNextChangeDate { get; set; }
        public TimeSpan? DteStartTime { get; set; }
        public TimeSpan? DteExtendedStartTime { get; set; }
        public TimeSpan? DteLastStartTime { get; set; }
        public TimeSpan? DteEndTime { get; set; }
        public decimal? NumMinWorkHour { get; set; }
        public bool IsNightShift { get; set; }
        public TimeSpan? DteOfficeOpeningTime { get; set; }
        public TimeSpan? DteOfficeClosingTime { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
