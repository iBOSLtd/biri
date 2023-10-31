using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayrollElementType
    {
        public long IntPayrollElementTypeId { get; set; }
        public long IntAccountId { get; set; }
        public string StrPayrollElementName { get; set; }
        public string StrCode { get; set; }
        public bool IsBasicSalary { get; set; }
        public bool IsPrimarySalary { get; set; }
        public bool IsAddition { get; set; }
        public bool IsDeduction { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
