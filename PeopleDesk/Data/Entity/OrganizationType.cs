using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class OrganizationType
    {
        public long IntOrganizationTypeId { get; set; }
        public string StrOrganizationTypeName { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsActive { get; set; }
    }
}
