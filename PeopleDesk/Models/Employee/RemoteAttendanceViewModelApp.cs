namespace PeopleDesk.Models.Employee
{
    public class RemoteAttendanceViewModelApp
    {
    }

    public class RemoteAttendanceViewModel
    {
        public long PartId { get; set; }
        public long AccountId { get; set; }
        public long EmployeeId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public long? RealTimeImageId { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public bool? isMarket { get; set; }
        public string? VisitingCompany { get; set; }
        public string? VisitingLocation { get; set; }
        public string? Remarks { get; set; }
    }

    public class RemoteAttendanceRegistrationViewModel
    {
        public long PartId { get; set; }
        public long AttendanceRegId { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsLocationRegister { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public long? InsertBy { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public bool? IsHomeOffice { get; set; }
    }
    public class TimeAttendanceProcessViewModel
    {
        public long IntHeaderRequestId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
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
        public List<TimeAttendanceProcessRequestRowVM> processRequestRowVMs { get; set; }
    }
    public partial class TimeAttendanceProcessRequestRowVM
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
    public class TimeAttendanceProcessResponseVM
    {
        public long IntHeaderRequestId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime DteToDate { get; set; }
        public long IntTotalEmployee { get; set; }
        public bool? Status { get; set; }
        public bool IsAll { get; set; }
        public bool? IsActive { get; set; }
        public List<ProcessEmployee> ProcessEmployees { get; set; }
    }
    public class ProcessEmployee
    {
        public long IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrDesignation { get; set; }
        public string? StrDepartment { get; set; }

    }
}