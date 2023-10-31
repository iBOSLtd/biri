using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalPipelineRow
    {
        public long IntPipelineRowId { get; set; }
        public long IntPipelineHeaderId { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsLineManager { get; set; }
        public long? IntUserGroupHeaderId { get; set; }
        public long IntShortOrder { get; set; }
        public string StrStatusTitle { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
