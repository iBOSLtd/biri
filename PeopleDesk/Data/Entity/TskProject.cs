using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TskProject
    {
        public long IntProjectId { get; set; }
        public string StrProjectName { get; set; }
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntFileUrlId { get; set; }
        public bool IsActivate { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public string StrStatus { get; set; }
        public string StrDescription { get; set; }
    }
}
