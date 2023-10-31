using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingSchedule
    {
        public long IntScheduleId { get; set; }
        public long IntTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public DateTime DteDate { get; set; }
        public decimal NumTotalDuration { get; set; }
        public string StrVenue { get; set; }
        public string StrResourcePersonName { get; set; }
        public long IntBatchSize { get; set; }
        public string StrBatchNo { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime DteToDate { get; set; }
        public string StrRemarks { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string StrApprovealStatus { get; set; }
        public bool IsActive { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime? DteActionDate { get; set; }
        public bool? IsRequestedSchedule { get; set; }
        public long? IntRequestedByEmp { get; set; }
        public DateTime? DteCourseCompletionDate { get; set; }
        public DateTime? DteExtentedDate { get; set; }
        public DateTime? DteLastAssesmentSubmissionDate { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
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
