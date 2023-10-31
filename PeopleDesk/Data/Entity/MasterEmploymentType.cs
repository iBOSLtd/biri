using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterEmploymentType
    {
        public long IntEmploymentTypeId { get; set; }
        public long? IntParentId { get; set; }
        public string StrEmploymentType { get; set; }
        public bool? IsActive { get; set; }
        public long IntAccountId { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
