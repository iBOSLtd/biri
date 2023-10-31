using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Item
    {
        public long IntItemId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrItemCode { get; set; }
        public bool IsAutoCode { get; set; }
        public string StrItemName { get; set; }
        public long IntItemCategoryId { get; set; }
        public long IntItemUomId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
