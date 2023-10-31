using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AssetRequisition
    {
        public long IntAssetRequisitionId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntItemId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntReqisitionQuantity { get; set; }
        public DateTime DteReqisitionDate { get; set; }
        public string StrRemarks { get; set; }
        public bool IsActive { get; set; }
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
        public bool? IsDenied { get; set; }
        public bool? IsAcknowledged { get; set; }
    }
}
