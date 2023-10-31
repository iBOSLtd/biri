using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class District
    {
        public long IntDistrictId { get; set; }
        public string StrDistrict { get; set; }
        public string StrDistrictBn { get; set; }
        public string StrDistrictCode { get; set; }
        public string StrLatitude { get; set; }
        public string StrLongitude { get; set; }
        public long IntDivisionId { get; set; }
        public long? IntCountryId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
