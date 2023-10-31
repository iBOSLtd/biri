namespace PeopleDesk.Models.Auth
{
    public class UserViewModel : Base
    {
        public long IntUserId { get; set; }
        public string StrLoginId { get; set; }
        public string StrPassword { get; set; }
        public string? StrOldPassword { get; set; }
        public string StrDisplayName { get; set; }
        public long IntUserTypeId { get; set; }
        public string? StrUserType { get; set; }
        public long? IntRefferenceId { get; set; }
        public bool? IsOfficeAdmin { get; set; }
        public bool? IsSuperuser { get; set; }
        public DateTime? DteLastLogin { get; set; }
        public long IntUrlId { get; set; }
        public string StrUrl { get; set; }
        public long IntAccountId { get; set; }

    }
}
