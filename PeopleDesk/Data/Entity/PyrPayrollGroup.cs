using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayrollGroup
    {
        public int IntPayrollGroupId { get; set; }
        public string StrPayrollGroupName { get; set; }
        public string StrPayrollGroupCode { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrStartDateOfMonth { get; set; }
        public string StrEndDateOfMonth { get; set; }
        public string StrPayFrequencyName { get; set; }
        public int? IntPayFrequencyId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
