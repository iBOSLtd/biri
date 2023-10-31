using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeEmployeeAttendance
    {
        public long IntAutoIncrement { get; set; }
        public string StrAttendanceType { get; set; }
        public long? IntRemoteAttendanceId { get; set; }
        public long IntEmployeeId { get; set; }
        public DateTime? DteAttendanceDate { get; set; }
        public TimeSpan? DteAttendanceTime { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrInOutStatus { get; set; }
        public bool? IsProcess { get; set; }
        public string StrRemark { get; set; }
        public DateTime? DteSyncDatetime { get; set; }
        public string StrCloudId { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public bool IsMarket { get; set; }
        public DateTime? DtePunchDate { get; set; }
    }
}
