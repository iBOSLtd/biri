using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class UserGroupRow
    {
        public long IntUserGroupRowId { get; set; }
        public long IntUserGroupHeaderId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
