using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class RoleGroupRow
    {
        public long IntRowId { get; set; }
        public long IntRoleGroupId { get; set; }
        public long IntUserId { get; set; }
        public string StrUserName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
