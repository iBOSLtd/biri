using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterTerritoryType
    {
        public long IntTerritoryTypeId { get; set; }
        public long? IntHrPositionId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string StrTerritoryType { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}
