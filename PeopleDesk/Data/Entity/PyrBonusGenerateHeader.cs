using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrBonusGenerateHeader
    {
        public long IntBonusHeaderId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public long? IntWingId { get; set; }
        public long? IntSoleDepoId { get; set; }
        public long? IntRegionId { get; set; }
        public long? IntAreaId { get; set; }
        public long? IntTerritoryId { get; set; }
        public long? IntBonusId { get; set; }
        public string StrBonusName { get; set; }
        public DateTime? DteEffectedDateTime { get; set; }
        public decimal? NumBonusAmount { get; set; }
        public bool? IsArrearBonus { get; set; }
        public long? IntArrearBonusReferenceId { get; set; }
        public int? IntBonusYearCal { get; set; }
        public int? IntBonusMonthCal { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsSendForApproval { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long IntCurrentStage { get; set; }
        public long IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
    }
}
