using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpTransferNpromotionUserRole
    {
        public long IntTransferNpromotionUserRoleId { get; set; }
        public long? IntTransferNpromotionId { get; set; }
        public long? IntUserRoleId { get; set; }
        public string StrUserRoleName { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
