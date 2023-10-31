using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeRemoteAttendanceRegistration
    {
        public long IntAttendanceRegId { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsLocationRegister { get; set; }
        public long? IntMasterLocationId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public bool? IsHomeOffice { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public string StrDeviceId { get; set; }
        public string StrDeviceName { get; set; }
        public DateTime DteInsertDate { get; set; }
        public long IntInsertBy { get; set; }
        public bool? IsActive { get; set; }
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
