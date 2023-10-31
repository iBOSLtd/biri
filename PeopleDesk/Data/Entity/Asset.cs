using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Asset
    {
        public long IntAssetId { get; set; }
        public string StrAssetCode { get; set; }
        public long IntItemId { get; set; }
        public string StrItemName { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrSupplierName { get; set; }
        public string StrSupplierMobileNo { get; set; }
        public DateTime DteAcquisitionDate { get; set; }
        public long? IntAcquisitionValue { get; set; }
        public long? IntInvoiceValue { get; set; }
        public long? IntDepreciationValue { get; set; }
        public DateTime? DteDepreciationDate { get; set; }
        public DateTime? DteWarrantyDate { get; set; }
        public string StrDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
