using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterLocationRegister
    {
        public long IntMasterLocationId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessId { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrLocationCode { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public bool? IsActive { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
