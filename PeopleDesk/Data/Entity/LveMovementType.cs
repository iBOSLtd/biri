using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LveMovementType
    {
        public long IntMovementTypeId { get; set; }
        public string StrMovementType { get; set; }
        public string StrMovementTypeCode { get; set; }
        public long? IntQuotaHour { get; set; }
        public long? IntQuotaFrequency { get; set; }
        public bool IsActive { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
