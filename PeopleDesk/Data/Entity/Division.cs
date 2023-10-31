using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Division
    {
        public long IntDivisionId { get; set; }
        public string StrDivision { get; set; }
        public string StrDivisionBn { get; set; }
        public string StrDivisionCode { get; set; }
        public string StrLatitude { get; set; }
        public string StrLongitude { get; set; }
        public long? IntCountryId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
