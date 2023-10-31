using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class UserGroupHeader
    {
        public long IntUserGroupHeaderId { get; set; }
        public string StrUserGroup { get; set; }
        public string StrCode { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
