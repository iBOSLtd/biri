using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class ItemCategory
    {
        public long IntItemCategoryId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrItemCategory { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
    }
}
