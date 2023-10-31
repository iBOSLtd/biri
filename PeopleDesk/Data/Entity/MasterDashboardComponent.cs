using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterDashboardComponent
    {
        public long IntId { get; set; }
        public string StrName { get; set; }
        public string StrDisplayName { get; set; }
        public string StrHashCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
