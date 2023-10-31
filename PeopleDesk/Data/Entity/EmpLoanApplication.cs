using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpLoanApplication
    {
        public long IntLoanApplicationId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntLoanTypeId { get; set; }
        public long IntLoanAmount { get; set; }
        public long IntNumberOfInstallment { get; set; }
        public long IntNumberOfInstallmentAmount { get; set; }
        public DateTime DteApplicationDate { get; set; }
        public long? IntApproveLoanAmount { get; set; }
        public long? IntApproveNumberOfInstallment { get; set; }
        public long? IntApproveNumberOfInstallmentAmount { get; set; }
        public long? NumRemainingBalance { get; set; }
        public DateTime? DteEffectiveDate { get; set; }
        public string StrDescription { get; set; }
        public long? IntFileUrlId { get; set; }
        public string StrReferenceNo { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsHold { get; set; }
        public long? IntReScheduleCount { get; set; }
        public long? IntReScheduleNumberOfInstallment { get; set; }
        public long? IntReScheduleNumberOfInstallmentAmount { get; set; }
        public string StrReScheduleRemarks { get; set; }
        public DateTime? DteReScheduleDateTime { get; set; }
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
    }
}
