using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayrollElementAndRulesTest
    {
        public int IntPayrollElementId { get; set; }
        public string StrPayrollElementName { get; set; }
        public string StrPayrollElementCode { get; set; }
        public int? IntPayrollElementTypeId { get; set; }
        public string StrPayrollElementTypeName { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool? IsDefaultSalary { get; set; }
        public int? IntEmploymentTypeId { get; set; }
        public string StrEmploymentTypeName { get; set; }
        public string StrCalculationOn { get; set; }
        public string StrPercentageOfPayrollElement { get; set; }
        public int? IntPercentageOfPayrollElementId { get; set; }
        public decimal? NumAmount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
