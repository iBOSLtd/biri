using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PolicyAcknowledge
    {
        public long IntAcknowledgeId { get; set; }
        public long IntPolicyId { get; set; }
        public long IntEmployeeId { get; set; }
        public DateTime DteAcknowledgeDate { get; set; }
    }
}
