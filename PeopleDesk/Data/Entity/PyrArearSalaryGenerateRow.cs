using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrArearSalaryGenerateRow
    {
        public long IntArearSalaryGenerateRow { get; set; }
        public long IntArearSalaryGenerateHeaderId { get; set; }
        public long IntArearSalaryGenerateRequestId { get; set; }
        public long IntEmployeeId { get; set; }
        public int IntPayrollElementId { get; set; }
        public string StrPayrollElement { get; set; }
        public decimal NumAmount { get; set; }
        public string StrPayrollElementCode { get; set; }
        public int? IntPayrollElementTypeId { get; set; }
        public int IntMonthId { get; set; }
        public int IntYearId { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
