namespace PeopleDesk.Models.Employee
{
    public class EmploymentTypeVM
    {
        public long IntEmploymentTypeId { get; set; }
        public long? IntParentId { get; set; }
        public string? StrParentName { get; set; }
        public string StrEmploymentType { get; set; } = null!;
        public bool? IsActive { get; set; }
        public int? IsManual { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
    }

    public class EmpSeparationTypeVM
    {
        public long IntSeparationTypeId { get; set; }
        public string StrSeparationType { get; set; } = null!;
        public bool? IsActive { get; set; }
        public bool? IsEmployeeView { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }

    public class UserRoleVM
    {
        public long IntRoleId { get; set; }
        public string StrRoleName { get; set; } = null!;
        public long? IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
