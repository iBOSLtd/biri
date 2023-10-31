using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AssetsRegister
    {
        public long IntRegisterId { get; set; }
        public string StrRegisterCode { get; set; }
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long? IntUomId { get; set; }
        public string StrUomName { get; set; }
        public long? IntAssetTypeId { get; set; }
        public string StrAssetTypeName { get; set; }
        public string StrSpecification { get; set; }
        public DateTime DteAcquisitionDate { get; set; }
        public DateTime DteWarrantyDate { get; set; }
        public bool IsActive { get; set; }
        public long? IntAssignEmployeeId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
