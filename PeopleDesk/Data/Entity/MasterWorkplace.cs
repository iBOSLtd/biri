using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterWorkplace
    {
        public long IntWorkplaceId { get; set; }
        public string StrWorkplace { get; set; }
        public string StrWorkplaceCode { get; set; }
        public string StrAddress { get; set; }
        public long? IntDistrictId { get; set; }
        public string StrDistrict { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string StrWorkplaceGroup { get; set; }
        public bool? IsActive { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
