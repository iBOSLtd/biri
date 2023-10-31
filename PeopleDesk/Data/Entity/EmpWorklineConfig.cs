using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpWorklineConfig
    {
        public long IntWorklineId { get; set; }
        public long IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long? IntServiceLengthInDays { get; set; }
        public long? IntNotifyInDays { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdateBy { get; set; }
        public DateTime? DteUpdateAt { get; set; }
    }
}
