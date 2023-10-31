using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Thana
    {
        public long IntThanaId { get; set; }
        public string StrThanaName { get; set; }
        public string StrThanaBn { get; set; }
        public long IntDistrictId { get; set; }
        public long IntDivisionId { get; set; }
        public long IntCountryId { get; set; }
        public bool IsActive { get; set; }
        public string StrInsertUserId { get; set; }
        public DateTime DteInsertDateTime { get; set; }
    }
}
