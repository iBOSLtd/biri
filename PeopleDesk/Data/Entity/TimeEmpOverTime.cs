using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeEmpOverTime
    {
        public long IntOverTimeId { get; set; }
        public long? IntEmployeeId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public long? IntWorkplaceId { get; set; }
        public DateTime? DteOverTimeDate { get; set; }
        public TimeSpan? TmeStartTime { get; set; }
        public TimeSpan? TmeEndTime { get; set; }
        public decimal? NumOverTimeHour { get; set; }
        public string StrReason { get; set; }
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
        public long? IntMonth { get; set; }
        public long? IntYear { get; set; }
        public string StrDailyOrMonthly { get; set; }
    }
}
