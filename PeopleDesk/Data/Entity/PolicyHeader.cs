using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PolicyHeader
    {
        public long IntPolicyId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrPolicyTitle { get; set; }
        public long? IntPolicyCategoryId { get; set; }
        public string StrPolicyCategoryName { get; set; }
        public long IntPolicyFileUrlId { get; set; }
        public string StrPolicyFileName { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
