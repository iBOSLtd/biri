using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeOffdayGroup
    {
        public long IntOffdayGroupId { get; set; }
        public string StrOffdayGroupName { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
