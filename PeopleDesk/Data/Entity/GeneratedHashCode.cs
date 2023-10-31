using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GeneratedHashCode
    {
        public long IntHashId { get; set; }
        public string StrHashCode { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool IsLocked { get; set; }
    }
}
