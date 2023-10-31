using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrSalaryBreakdownRow
    {
        public long IntSalaryBreakdownRowId { get; set; }
        public long IntSalaryBreakdownHeaderId { get; set; }
        public long IntPayrollElementTypeId { get; set; }
        public string StrPayrollElementName { get; set; }
        public string StrBasedOn { get; set; }
        public string StrDependOn { get; set; }
        public decimal? NumNumberOfPercent { get; set; }
        public decimal? NumAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
