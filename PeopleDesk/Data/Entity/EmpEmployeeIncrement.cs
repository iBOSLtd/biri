using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeIncrement
    {
        public long IntIncrementId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public long? IntDesignationId { get; set; }
        public long? IntDepartmentId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public decimal? NumOldGrossAmount { get; set; }
        public string StrIncrementDependOn { get; set; }
        public decimal? NumIncrementDependOn { get; set; }
        public decimal? NumIncrementPercentage { get; set; }
        public decimal? NumIncrementAmount { get; set; }
        public DateTime? DteEffectiveDate { get; set; }
        public long? IntTransferNpromotionReferenceId { get; set; }
        public long? IntOldSalaryElementAssignHeaderId { get; set; }
        public long? IntNewSalaryElementAssignHeaderId { get; set; }
        public bool? IsProcess { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
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
