using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeFixedRoasterSetupDetail
    {
        public long IntId { get; set; }
        public long IntDay { get; set; }
        public long IntFixedRoasterMasterId { get; set; }
        public long? IntCalendarId { get; set; }
        public string StrCalendarName { get; set; }
        public bool? IsOffDay { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
