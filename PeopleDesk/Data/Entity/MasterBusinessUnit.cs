using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterBusinessUnit
    {
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public string StrShortCode { get; set; }
        public string StrAddress { get; set; }
        public long? StrLogoUrlId { get; set; }
        public long? IntDistrictId { get; set; }
        public string StrDistrict { get; set; }
        public string StrEmail { get; set; }
        public string StrWebsiteUrl { get; set; }
        public string StrCurrency { get; set; }
        public bool? IsActive { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
