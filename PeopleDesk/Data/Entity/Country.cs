using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Country
    {
        public long IntCountryId { get; set; }
        public string StrCountry { get; set; }
        public string StrCountryCode { get; set; }
        public string StrDialingCode { get; set; }
        public string StrNationality { get; set; }
        public bool? IsActive { get; set; }
    }
}
