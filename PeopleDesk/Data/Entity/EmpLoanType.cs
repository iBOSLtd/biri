using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpLoanType
    {
        public long IntLoanTypeId { get; set; }
        public string StrLoanType { get; set; }
        public bool? IsActive { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}
