using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class OverTimeConfigurationDetail
    {
        public long IntOtautoId { get; set; }
        public long IntMasterId { get; set; }
        public decimal NumFromMinute { get; set; }
        public decimal NumToMinute { get; set; }
        public long IntAmountPercentage { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public string IntUpdatedBy { get; set; }
    }
}
