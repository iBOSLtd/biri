using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrEmpSalaryAdditionNdeduction
    {
        public long IntSalaryAdditionAndDeductionId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntEmployeeId { get; set; }
        public bool? IsAutoRenew { get; set; }
        public long? IntYear { get; set; }
        public long? IntMonth { get; set; }
        public string StrMonth { get; set; }
        public long? IntToYear { get; set; }
        public long? IntToMonth { get; set; }
        public string StrToMonth { get; set; }
        public bool? IsAddition { get; set; }
        public bool? IsOther { get; set; }
        public string StrAdditionNdeduction { get; set; }
        public long? IntAdditionNdeductionTypeId { get; set; }
        public int? IntAmountWillBeId { get; set; }
        public string StrAmountWillBe { get; set; }
        public decimal? NumAmount { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsProcessed { get; set; }
        public string StrCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
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
    }
}
