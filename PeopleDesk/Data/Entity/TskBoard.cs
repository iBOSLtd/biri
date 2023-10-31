using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TskBoard
    {
        public long IntBoardId { get; set; }
        public long IntProjectId { get; set; }
        public string StrBoardName { get; set; }
        public string StrDescription { get; set; }
        public DateTime DteStartDate { get; set; }
        public DateTime DteEndDate { get; set; }
        public long IntReporterId { get; set; }
        public long IntFileUrlId { get; set; }
        public string StrPriority { get; set; }
        public string StrBackgroundColor { get; set; }
        public string StrHtmlColorCode { get; set; }
        public string StrStatus { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
