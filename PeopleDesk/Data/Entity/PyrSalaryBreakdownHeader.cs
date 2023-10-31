using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrSalaryBreakdownHeader
    {
        public long IntSalaryBreakdownHeaderId { get; set; }
        public string StrSalaryBreakdownTitle { get; set; }
        public long IntAccountId { get; set; }
        public long? IntHrPositionId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public long? IntSalaryPolicyId { get; set; }
        public string StrDependOn { get; set; }
        public bool IsPerday { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
