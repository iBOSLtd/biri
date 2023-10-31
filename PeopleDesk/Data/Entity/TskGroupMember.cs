using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TskGroupMember
    {
        public long IntGroupMemberId { get; set; }
        public long IntAutoId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? IntGroupMemberTypeId { get; set; }
    }
}
