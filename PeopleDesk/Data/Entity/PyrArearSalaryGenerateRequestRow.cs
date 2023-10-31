using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrArearSalaryGenerateRequestRow
    {
        public long IntArearSalaryGenerateRequestRowId { get; set; }
        public long IntArearSalaryGenerateRequestId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
