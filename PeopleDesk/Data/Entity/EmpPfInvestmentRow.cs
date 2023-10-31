using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpPfInvestmentRow
    {
        public long IntRowId { get; set; }
        public long IntInvenstmentHeaderId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntDesignationId { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string StrServiceLength { get; set; }
        public decimal NumEmployeeContribution { get; set; }
        public decimal NumEmployerContribution { get; set; }
        public decimal NumTotalAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
