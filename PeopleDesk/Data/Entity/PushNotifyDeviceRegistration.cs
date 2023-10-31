using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PushNotifyDeviceRegistration
    {
        public long IntId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrDeviceId { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
