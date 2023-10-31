using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeRemoteAttendance
    {
        public long IntRemoteAttendanceId { get; set; }
        public long? IntAttendanceRegId { get; set; }
        public long IntEmployeeId { get; set; }
        public DateTime DteAttendanceDate { get; set; }
        public TimeSpan TmAttendanceTime { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public long? IntRealTimeImage { get; set; }
        public string StrDeviceId { get; set; }
        public string StrDeviceName { get; set; }
        public string StrInOutStatus { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsMarket { get; set; }
        public string StrVisitingCompany { get; set; }
        public string StrVisitingLocation { get; set; }
        public string StrRemarks { get; set; }
        public bool? IsInvalidCalendar { get; set; }
    }
}
