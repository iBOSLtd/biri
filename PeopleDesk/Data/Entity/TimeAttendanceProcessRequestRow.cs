using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeAttendanceProcessRequestRow
    {
        public long IntRowRequestId { get; set; }
        public long IntHeaderRequestId { get; set; }
        public long IntEmployeeId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreateDate { get; set; }
        public int IntCreateBy { get; set; }
        public DateTime? DteUpdateDate { get; set; }
        public int? IntUpdateBy { get; set; }
    }
}
