using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeFixedRoasterSetupMaster
    {
        public long IntId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnit { get; set; }
        public string StrRoasterName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
