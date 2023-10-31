using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class RoleBridgeWithDesignation
    {
        public long IntId { get; set; }
        public long IntAccountId { get; set; }
        public string StrIsFor { get; set; }
        public long? IntDesignationOrEmployeeId { get; set; }
        public long? IntRoleId { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteCreatedDateTime { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdateDateTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
