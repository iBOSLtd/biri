using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class NotificationPermissionDetail
    {
        public long IntPermissionDetailsId { get; set; }
        public long? IntPermissionId { get; set; }
        public long? IntNcategoryTypeId { get; set; }
        public string SteNcategoryTypeName { get; set; }
        public long? IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
