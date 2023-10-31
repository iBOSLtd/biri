using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    /// <summary>
    /// PipelineTypeDDL
    /// </summary>
    public partial class PipelineTypeDdl
    {
        public long IntId { get; set; }
        public string StrHashCode { get; set; }
        public string StrDisplayName { get; set; }
        public string StrApplicationType { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
    }
}
