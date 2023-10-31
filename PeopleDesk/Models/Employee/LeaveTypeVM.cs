namespace PeopleDesk.Models.Employee
{
    public class LeaveTypeVM
    {
        public long IntLeaveTypeId { get; set; }
        public long? IntParentId { get; set; }
        public string? StrParentName { get; set; }
        public string StrLeaveType { get; set; } = null!;
        public string StrLeaveTypeCode { get; set; } = null!;
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
