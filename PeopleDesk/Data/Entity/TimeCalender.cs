using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeCalender
    {
        public long IntCalenderId { get; set; }
        public string StrCalenderCode { get; set; }
        public string StrCalenderName { get; set; }
        public TimeSpan DteStartTime { get; set; }
        public TimeSpan DteExtendedStartTime { get; set; }
        public TimeSpan DteLastStartTime { get; set; }
        public TimeSpan DteEndTime { get; set; }
        public decimal NumMinWorkHour { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public TimeSpan? DteBreakStartTime { get; set; }
        public TimeSpan? DteBreakEndTime { get; set; }
        public TimeSpan DteOfficeStartTime { get; set; }
        public TimeSpan DteOfficeCloseTime { get; set; }
        public bool? IsNightShift { get; set; }
    }
}
