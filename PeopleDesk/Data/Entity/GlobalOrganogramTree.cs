using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalOrganogramTree
    {
        public long IntAutoId { get; set; }
        public long? IntPositionId { get; set; }
        public string StrPositionName { get; set; }
        public long? IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long IntParentId { get; set; }
        public long IntSequence { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
