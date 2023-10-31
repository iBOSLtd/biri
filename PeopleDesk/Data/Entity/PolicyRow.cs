using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PolicyRow
    {
        public long IntRowId { get; set; }
        public long? IntPolicyId { get; set; }
        public string StrAreaType { get; set; }
        public long? IntAreaAutoId { get; set; }
        public string StrAreaName { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}
