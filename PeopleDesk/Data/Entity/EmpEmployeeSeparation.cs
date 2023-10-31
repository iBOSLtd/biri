using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeSeparation
    {
        public long IntSeparationId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrEmployeeCode { get; set; }
        public long? IntSeparationTypeId { get; set; }
        public string StrSeparationTypeName { get; set; }
        public DateTime? DteSeparationDate { get; set; }
        public DateTime? DteLastWorkingDate { get; set; }
        public string StrDocumentId { get; set; }
        public string StrReason { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsReleased { get; set; }
        public bool? IsRejoin { get; set; }
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
    }
}
