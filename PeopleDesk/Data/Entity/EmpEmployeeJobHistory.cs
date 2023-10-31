using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeJobHistory
    {
        public long IntJobExperienceId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrCompanyName { get; set; }
        public string StrJobTitle { get; set; }
        public string StrLocation { get; set; }
        public DateTime? DteFromDate { get; set; }
        public DateTime? DteToDate { get; set; }
        public string StrRemarks { get; set; }
        public long? IntNocfileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
