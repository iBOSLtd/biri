using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Religion
    {
        public long IntReligionId { get; set; }
        public string StrReligion { get; set; }
        public string StrReligionCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
