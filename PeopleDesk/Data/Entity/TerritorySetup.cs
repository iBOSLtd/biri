using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TerritorySetup
    {
        public long IntTerritoryId { get; set; }
        public long IntTerritoryTypeId { get; set; }
        public long? IntHrPositionId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string StrTerritoryName { get; set; }
        public string StrTerritoryCode { get; set; }
        public string StrTerritoryAddress { get; set; }
        public long? IntRankingId { get; set; }
        public long IntParentTerritoryId { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdateBy { get; set; }
        public DateTime? DteUpdateBy { get; set; }
    }
}
