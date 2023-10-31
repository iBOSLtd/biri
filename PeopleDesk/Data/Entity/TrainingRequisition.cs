using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingRequisition
    {
        public long IntRequisitionId { get; set; }
        public long IntScheduleId { get; set; }
        public string StrTrainingName { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntDesignationId { get; set; }
        public string StrDesignationName { get; set; }
        public string StrEmail { get; set; }
        public string StrPhoneNo { get; set; }
        public string StrGender { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public DateTime DteActionDate { get; set; }
        public long IntActionBy { get; set; }
        public bool? IsFromRequisition { get; set; }
        public bool IsActive { get; set; }
        public string StrApprovalStatus { get; set; }
        public long? IntApprovedBy { get; set; }
        public DateTime? DteApprovedDate { get; set; }
        public string StrRejectionComments { get; set; }
        public long IntDepartmentId { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
    }
}
