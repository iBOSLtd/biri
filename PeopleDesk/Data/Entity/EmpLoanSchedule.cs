using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpLoanSchedule
    {
        public long IntScheduleId { get; set; }
        public long IntLoanApplicationId { get; set; }
        public long IntEmployeeId { get; set; }
        public string IntMonth { get; set; }
        public long IntYear { get; set; }
        public long? IntInstallmentAmount { get; set; }
        public string DteInstallmentDate { get; set; }
        public string YsnInstallmentStatus { get; set; }
        public string YsnEnable { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
