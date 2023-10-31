using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeRemoteAttendanceSetup
    {
        public long IntAutoId { get; set; }
        public long IntAccountId { get; set; }
        public bool IsCheckInApprovalNeed { get; set; }
        public bool IsDeviceRegNeed { get; set; }
        public bool IsLocationRegNeed { get; set; }
        public bool IsRealTimeImageNeed { get; set; }
        public decimal NumMinimumValidDistance { get; set; }
        public bool? IsFlexible { get; set; }
        public bool? IsInSideApprovalNeed { get; set; }
        public bool? IsOutSideApprovalNeed { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public string IntUpdatedBy { get; set; }
    }
}
