using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PolicyCategory
    {
        public long IntPolicyCategoryId { get; set; }
        public string StrPolicyCategoryName { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
