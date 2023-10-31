using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class RoleGroupHeader
    {
        public long IntRoleGroupId { get; set; }
        public string StrRoleGroupName { get; set; }
        public string StrRoleGroupCode { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
