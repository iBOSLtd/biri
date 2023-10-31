using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AssetTransaction
    {
        public long IntId { get; set; }
        public long IntAssetId { get; set; }
        public long IntRegisterId { get; set; }
        public long IntTransactionTypeId { get; set; }
        public DateTime DteTransactionDate { get; set; }
        public string StrRemarks { get; set; }
        public bool IsApprovedOrRejected { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
