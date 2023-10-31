using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class NotificationPermissionMaster
    {
        public long IntPermissionId { get; set; }
        public long IntAccountId { get; set; }
        public long? IntNcategoryId { get; set; }
        public string StrNcategoryName { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
