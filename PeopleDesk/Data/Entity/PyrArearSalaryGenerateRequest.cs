using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrArearSalaryGenerateRequest
    {
        public long IntArearSalaryGenerateRequestId { get; set; }
        public string StrArearSalaryCode { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; }
        public DateTime DteEffectiveFrom { get; set; }
        public DateTime DteEffectiveTo { get; set; }
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
        public long? IntSalaryPolicyId { get; set; }
        public string StrSalaryPolicyName { get; set; }
        public decimal NumPercentOfGross { get; set; }
    }
}
