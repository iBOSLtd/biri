using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Announcement
    {
        public long IntAnnouncementId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string StrTitle { get; set; }
        public string StrDetails { get; set; }
        public long? IntTypeId { get; set; }
        public string StrTypeName { get; set; }
        public DateTime? DteExpiredDate { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
