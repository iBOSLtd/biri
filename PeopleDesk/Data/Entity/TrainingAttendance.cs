using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingAttendance
    {
        public long IntAttendanceId { get; set; }
        public long IntRequisitionId { get; set; }
        public long IntEmployeeId { get; set; }
        public DateTime DteAttendanceDate { get; set; }
        public string StrAttendanceStatus { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime? DteActionDate { get; set; }
        public long? IntActionBy { get; set; }
    }
}
