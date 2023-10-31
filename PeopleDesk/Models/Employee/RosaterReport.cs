namespace PeopleDesk.Models.Employee
{
    public class RosaterReport
    {
    }
    public class RoasterReportFilter
    {
        public long AccountId { get; set; }
        public long BusinessUnit { get; set; }
        public long WorkGroupId { get; set; }
        public long WorkPlaceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }

    public class GetRoasterReportVM
    {
        public long IntId { get; set; }
        public string EmployeeCode { get; set; }
        public string EployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        public List<RoasterCalenderName> RoasterCalenderNames { get; set; }

    }
    public class RoasterCalenderName
    {
        public DateTime? DteAttendenceDate { get; set; }
        public int Serial { get; set; }
        public string CalenderName { get; set; }
    }
}
