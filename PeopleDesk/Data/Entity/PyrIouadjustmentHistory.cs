using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrIouadjustmentHistory
    {
        public long IntIouadjustmentId { get; set; }
        public long? IntIouid { get; set; }
        public long IntEmployeeId { get; set; }
        public decimal? NumPayableAmount { get; set; }
        public decimal? NumReceivableAmount { get; set; }
        public bool? IsAcknowledgement { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
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
