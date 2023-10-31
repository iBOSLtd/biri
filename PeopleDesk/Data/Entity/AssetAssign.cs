using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AssetAssign
    {
        public long IntId { get; set; }
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; }
        public long IntAssetRegisterId { get; set; }
        public string StrRegisterCode { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public DateTime DteAssignDate { get; set; }
        public string StrRemarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DteWithdrawDate { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
