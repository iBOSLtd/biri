using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TimeAttendanceProcessRequestHeader
    {
        public long IntHeaderRequestId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteProcessDate { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime DteToDate { get; set; }
        public long IntTotalEmployee { get; set; }
        public bool? Status { get; set; }
        public bool IsAll { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreateDate { get; set; }
        public int IntCreateBy { get; set; }
        public DateTime? DteUpdateDate { get; set; }
        public int? IntUpdateBy { get; set; }
    }
}
