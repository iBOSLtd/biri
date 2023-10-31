using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrEmployeeSalaryElementAssignRow
    {
        public long IntEmpSalaryElementAssignRowId { get; set; }
        public long IntEmpSalaryElementAssignHeaderId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntSalaryElementId { get; set; }
        public string StrSalaryElement { get; set; }
        public string StrDependOn { get; set; }
        public decimal? NumNumberOfPercent { get; set; }
        public decimal NumAmount { get; set; }
        public bool? IsActive { get; set; }
        public long IntCreateBy { get; set; }
        public DateTime DteCreateDateTime { get; set; }
    }
}
