using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LveLeaveApplication
    {
        public long IntApplicationId { get; set; }
        public long IntLeaveTypeId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteApplicationDate { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime DteToDate { get; set; }
        public long? IntDocumentFileId { get; set; }
        public string StrReason { get; set; }
        public string StrAddressDuetoLeave { get; set; }
        public bool? IsPayable { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsActive { get; set; }
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
