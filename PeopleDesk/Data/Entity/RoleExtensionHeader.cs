using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class RoleExtensionHeader
    {
        public long IntRoleExtensionHeaderId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteCreatedDateTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
