using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeOffday
    {
        public long IntOffdayId { get; set; }
        public long? IntOffdayGroupId { get; set; }
        public long? IntWeekdayId { get; set; }
        public string StrWeekdayName { get; set; }
        public long? IntNoOfDaysChange { get; set; }
        public long? IntNextWeekdayId { get; set; }
        public string StrNextWeekdayName { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
