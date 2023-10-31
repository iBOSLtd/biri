using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpSeparationType
    {
        public long IntSeparationTypeId { get; set; }
        public string StrSeparationType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsEmployeeView { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}
