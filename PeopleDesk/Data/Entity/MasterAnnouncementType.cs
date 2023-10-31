using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterAnnouncementType
    {
        public long IntAnnouncementTypeId { get; set; }
        public string StrAnnouncementTypeName { get; set; }
        public long? IsActive { get; set; }
    }
}
