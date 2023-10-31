using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Uom
    {
        public long IntUomId { get; set; }
        public string StrUomName { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
