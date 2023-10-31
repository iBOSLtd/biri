using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmploymentTypeWiseLeaveBalance
    {
        public long IntId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public int? IntGenderId { get; set; }
        public string StrGender { get; set; }
        public long? IntAllocatedLeave { get; set; }
        public long? IntYearId { get; set; }
        public long? IntLeaveTypeId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public long? IntWorkplaceId { get; set; }
    }
}
