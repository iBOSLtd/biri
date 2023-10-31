using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterDashboardComponentPermission
    {
        public long IntId { get; set; }
        public long IntAccountId { get; set; }
        public string StrAccountName { get; set; }
        public long? IntDashboardComponentId { get; set; }
        public string StrHashCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
