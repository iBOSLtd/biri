using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpJobExperience
    {
        public long IntJobExperienceId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrCompanyName { get; set; }
        public string StrJobTitle { get; set; }
        public string StrLocation { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime? DteToDate { get; set; }
        public long? IntNocUrlId { get; set; }
        public string StrDescription { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
