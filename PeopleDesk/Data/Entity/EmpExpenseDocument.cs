using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpExpenseDocument
    {
        public long IntExpenseDocId { get; set; }
        public long? IntExpenseId { get; set; }
        public string StrDocFor { get; set; }
        public long IntDocUrlid { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
