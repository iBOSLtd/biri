using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AnnouncementRow
    {
        public long IntAnnouncementRowId { get; set; }
        public long IntAnnoucementId { get; set; }
        public long? IntAnnouncementReferenceId { get; set; }
        public string StrAnnounceCode { get; set; }
        public string StrAnnouncementFor { get; set; }
        public bool IsActive { get; set; }
    }
}
