using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayrollSalaryGenerateRequest
    {
        public long IntSalaryGenerateRequestId { get; set; }
        public string StrSalaryCode { get; set; }
        public string StrSalaryType { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public int? IntWorkplaceGroupId { get; set; }
        public string StrWorkplaceGroupName { get; set; }
        public long? IntWingId { get; set; }
        public long? IntSoleDepoId { get; set; }
        public long? IntRegionId { get; set; }
        public long? IntAreaId { get; set; }
        public string StrTerritoryIdList { get; set; }
        public string StrTerritoryNameList { get; set; }
        public long IntMonth { get; set; }
        public long IntYear { get; set; }
        public DateTime DteSalaryGenerateFrom { get; set; }
        public DateTime DteSalaryGenerateTo { get; set; }
        public bool IsGenerated { get; set; }
        public decimal NumNetPayableSalary { get; set; }
        public string StrDescription { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long IntCurrentStage { get; set; }
        public long IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public bool? IsProcessing { get; set; }
        public bool? IsHasRankingOrder { get; set; }
        public bool? IsApprovedBySoleDepo { get; set; }
        public bool? IsApprovedByHeadOffice { get; set; }
        public bool? IsRejectedBySoleDepo { get; set; }
        public bool? IsRejectedByHeadOffice { get; set; }
    }
}
