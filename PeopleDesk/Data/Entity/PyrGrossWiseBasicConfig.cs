using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrGrossWiseBasicConfig
    {
        public long IntGrossWiseBasicId { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public decimal NumMinGross { get; set; }
        public decimal NumMaxGross { get; set; }
        public decimal NumPercentageOfBasic { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreateBy { get; set; }
        public DateTime? DteCreateDate { get; set; }
    }
}
