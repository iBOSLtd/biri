using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LveLeaveBalance
    {
        public long IntLeaveBalanceId { get; set; }
        public long IntLeaveTypeId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntLeaveTakenDays { get; set; }
        public long IntRemainingDays { get; set; }
        public long IntBalanceDays { get; set; }
        public string StrRemarks { get; set; }
        public long IntYear { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public bool? IsAutoGenerate { get; set; }
    }
}
