using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterDepartment
    {
        public long IntDepartmentId { get; set; }
        public string StrDepartment { get; set; }
        public string StrDepartmentCode { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long? IntRankingId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? IntParentDepId { get; set; }
        public string StrParentDepName { get; set; }
    }
}
