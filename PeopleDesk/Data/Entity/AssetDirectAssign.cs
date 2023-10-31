using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AssetDirectAssign
    {
        public long IntAssetDirectAssignId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntItemId { get; set; }
        public long IntItemQuantity { get; set; }
        public DateTime DteAssignDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsAcknowledged { get; set; }
        public string StrStatus { get; set; }
    }
}
