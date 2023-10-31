using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeExceptionOffdayGroup
    {
        public long IntExceptionOffdayGroupId { get; set; }
        public string StrExceptionOffdayName { get; set; }
        public bool IsAlternativeDay { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
