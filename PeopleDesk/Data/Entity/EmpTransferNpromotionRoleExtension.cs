using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpTransferNpromotionRoleExtension
    {
        public long IntRoleExtensionRowId { get; set; }
        public long IntTransferNpromotionId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntOrganizationTypeId { get; set; }
        public string StrOrganizationTypeName { get; set; }
        public long IntOrganizationReffId { get; set; }
        public string StrOrganizationReffName { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteCreatedDateTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
