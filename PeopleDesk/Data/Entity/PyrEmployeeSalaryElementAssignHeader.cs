using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrEmployeeSalaryElementAssignHeader
    {
        public long IntEmpSalaryElementAssignHeaderId { get; set; }
        public long? IntSalaryBreakdownHeaderId { get; set; }
        public string StrSalaryBreakdownHeaderTitle { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public decimal NumBasicOrgross { get; set; }
        public decimal NumGrossSalary { get; set; }
        public decimal NumNetGrossSalary { get; set; }
        public byte[] NumBasicOrgrossEncrypted { get; set; }
        public byte[] NumGrossSalaryEncrypted { get; set; }
        public byte[] NumNetGrossSalaryEncrypted { get; set; }
        public bool IsPerdaySalary { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreateBy { get; set; }
        public DateTime? DteCreateDateTime { get; set; }
        public long? IntUpdateBy { get; set; }
        public DateTime? DteUpdateDateTime { get; set; }
    }
}
