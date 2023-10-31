using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeEmployeeHoliday
    {
        public long IntEmployeeHolidayAssignId { get; set; }
        public long? IntEmployeeId { get; set; }
        public long? IntHolidayGroupId { get; set; }
        public string StrHolidayGroupName { get; set; }
        public DateTime? DteEffectiveDate { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
