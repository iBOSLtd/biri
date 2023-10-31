using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalPipelineHeader
    {
        public long IntPipelineHeaderId { get; set; }
        public string StrPipelineName { get; set; }
        public string StrApplicationType { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntWingId { get; set; }
        public long IntSoleDepoId { get; set; }
        public long IntRegionId { get; set; }
        public long IntAreaId { get; set; }
        public long IntTerritoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
