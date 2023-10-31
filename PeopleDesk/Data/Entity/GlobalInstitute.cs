using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalInstitute
    {
        public long IntInstituteId { get; set; }
        public string StrInstituteName { get; set; }
        public long IntInstituteTypeId { get; set; }
        public bool? IsActive { get; set; }
    }
}
