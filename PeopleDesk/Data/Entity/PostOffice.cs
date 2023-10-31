using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PostOffice
    {
        public long IntPostOfficeId { get; set; }
        public string StrPostOffice { get; set; }
        public string StrPostOfficeBn { get; set; }
        public string StrPostCode { get; set; }
        public long? IntThanaId { get; set; }
        public string StrThanaName { get; set; }
        public long? IntDistrictId { get; set; }
        public long? IntDivisionId { get; set; }
        public long? IntCountryId { get; set; }
        public DateTime DteCreateAt { get; set; }
    }
}
