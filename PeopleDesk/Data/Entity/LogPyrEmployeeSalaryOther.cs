using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LogPyrEmployeeSalaryOther
    {
        public long IntLogEmployeeSalaryOtherId { get; set; }
        public long IntEmployeeId { get; set; }
        public long? IntPayrollElementId { get; set; }
        public string StrPayrollElementTypeCode { get; set; }
        public decimal? NumAmount { get; set; }
        public decimal? ReqNumAmount { get; set; }
        public long? IntEffectiveMonth { get; set; }
        public long? IntEffectiveYear { get; set; }
        public long? IntLogCreatedByUserId { get; set; }
        public DateTime? DtelogCreatedByDateTime { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
