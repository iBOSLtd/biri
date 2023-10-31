using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class BloodGroup
    {
        public long IntBloodGroupId { get; set; }
        public string StrBloodGroup { get; set; }
        public string StrBloodGroupCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
