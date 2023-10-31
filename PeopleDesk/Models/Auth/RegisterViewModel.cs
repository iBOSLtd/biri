namespace PeopleDesk.Models.Auth
{
    public class RegisterViewModel
    {
        public long? EmployeeId { get; set; }
        public string? LoginId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ContactNo { get; set; }
        public int? UserTypeId { get; set; }
        public string? UserTypeName { get; set; }
        public long? AccountId { get; set; }
        public long? CountryId { get; set; }
        public long? InsertUserId { get; set; }
        public bool IsEdit { get; set; }
        public bool IsActive { get; set; }

        // For Identity Server

        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public long? OrgId { get; set; }
        public string? Scope { get; set; }
        public string? Response { get; set; }
    }
}
