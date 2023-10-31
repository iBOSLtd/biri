using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TskTaskDetail
    {
        public long IntTaskDetailsId { get; set; }
        public long IntProjectId { get; set; }
        public long IntBoardId { get; set; }
        public string StrTaskTitle { get; set; }
        public string StrTaskDescription { get; set; }
        public string StrStatus { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
