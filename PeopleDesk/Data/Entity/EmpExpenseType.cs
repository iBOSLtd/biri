using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpExpenseType
    {
        public long IntExpenseTypeId { get; set; }
        public string StrExpenseType { get; set; }
        public bool? IsActive { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}
