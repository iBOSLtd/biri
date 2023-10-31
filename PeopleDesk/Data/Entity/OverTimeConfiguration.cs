using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class OverTimeConfiguration
    {
        public long IntOtconfigId { get; set; }
        public long IntAccountId { get; set; }
        public long IntOtdependOn { get; set; }
        public decimal? NumFixedAmount { get; set; }
        public long IntOverTimeCountFrom { get; set; }
        public long IntOtbenefitsHour { get; set; }
        public long IntMaxOverTimeDaily { get; set; }
        public long IntMaxOverTimeMonthly { get; set; }
        public long IntOtcalculationShouldBe { get; set; }
        public long IntOtAmountShouldBe { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
