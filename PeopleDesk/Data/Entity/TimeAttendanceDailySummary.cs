using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeAttendanceDailySummary
    {
        public long IntAutoId { get; set; }
        public long? IntDayId { get; set; }
        public long? IntMonthId { get; set; }
        public long? IntYear { get; set; }
        public DateTime? DteAttendanceDate { get; set; }
        public long IntEmployeeId { get; set; }
        public bool? IsPresent { get; set; }
        public bool? IsAbsent { get; set; }
        public bool? IsLeave { get; set; }
        public bool? IsLeaveWithPay { get; set; }
        public bool? IsMovement { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsOffday { get; set; }
        public bool? IsLate { get; set; }
        public TimeSpan? TmeLateHour { get; set; }
        public bool? IsEarlyLeave { get; set; }
        public TimeSpan? TmeEarlyLeaveHour { get; set; }
        public TimeSpan? TmeAttendanceHour { get; set; }
        public TimeSpan? TmeExtraHour { get; set; }
        public TimeSpan? TmeShiftOverTime { get; set; }
        public long? IntPunchCount { get; set; }
        public long? IntCalendarTypeId { get; set; }
        public string StrCalendarType { get; set; }
        public long? IntCalendarId { get; set; }
        public string StrCalendarName { get; set; }
        public DateTime? DteNextChangeDate { get; set; }
        public TimeSpan? DteStartTime { get; set; }
        public TimeSpan? DteExtendedStartTime { get; set; }
        public TimeSpan? DteLastStartTime { get; set; }
        public TimeSpan? DteEndTime { get; set; }
        public TimeSpan? DteBreakStartTime { get; set; }
        public TimeSpan? DteBreakEndTime { get; set; }
        public TimeSpan? TmeInTime { get; set; }
        public TimeSpan? TmeLastOutTime { get; set; }
        public decimal? NumMinWorkHour { get; set; }
        public bool? IsProcess { get; set; }
        public DateTime? DteProcessDateTime { get; set; }
        public string StrWorkingHours { get; set; }
        public long? IntWorkingHourInMinute { get; set; }
        public long? IntMissingWorkingHoursInMinutes { get; set; }
        public int IsWorkingDayCal { get; set; }
        public long? IntRosterGroupId { get; set; }
        public string StrRosterGroupName { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public bool? IsAutoGenerate { get; set; }
        public bool? IsManual { get; set; }
        public DateTime? DteGenerateDate { get; set; }
        public decimal? NumOverTime { get; set; }
        public decimal? NumModifiedOverTime { get; set; }
        public TimeSpan? TmeOtStartTime { get; set; }
        public TimeSpan? TmeOtOutTime { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsManualPresent { get; set; }
        public bool? IsManualAbsent { get; set; }
        public bool? IsManualLeave { get; set; }
        public bool? IsManualLate { get; set; }
        public long? IntManualAttendanceBy { get; set; }
        public DateTime? DteManualAttendanceDate { get; set; }
        public bool? IsNightShift { get; set; }
        public TimeSpan? DteOfficeOpeningTime { get; set; }
        public TimeSpan? DteOfficeClosingTime { get; set; }
    }
}
