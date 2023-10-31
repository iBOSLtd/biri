using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeEmployeeOffdayReassign
    {
        public long IntAutoId { get; set; }
        public DateTime DteDate { get; set; }
        public long IntEmployeeId { get; set; }
        public bool IsOffday { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
