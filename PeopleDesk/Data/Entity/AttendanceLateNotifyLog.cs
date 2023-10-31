using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AttendanceLateNotifyLog
    {
        public long IntAttendanceNotifyId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteAttendenceDate { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
