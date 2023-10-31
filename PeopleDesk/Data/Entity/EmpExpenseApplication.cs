using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpExpenseApplication
    {
        public long IntExpenseId { get; set; }
        public long IntAccountId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntExpenseTypeId { get; set; }
        public DateTime DteExpenseFromDate { get; set; }
        public DateTime DteExpenseToDate { get; set; }
        public string StrDiscription { get; set; }
        public decimal? NumExpenseAmount { get; set; }
        public long? IntDocumentId { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
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
