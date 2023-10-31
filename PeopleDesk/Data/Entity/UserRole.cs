using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class UserRole
    {
        public long IntRoleId { get; set; }
        public string StrRoleName { get; set; }
        public long? IntAccountId { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
