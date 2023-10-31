namespace PeopleDesk.Models.Auth
{
    public class CreateUserViewModel
    {
        public long? IntUserId { get; set; }
        public string? StrLoginId { get; set; }
        public string? StrPassword { get; set; }
        public string? StrOldPassword { get; set; }
        public string? StrDisplayName { get; set; }
        public long? IntUserTypeId { get; set; }
        public long? IntRefferenceId { get; set; }
        public bool? IsOfficeAdmin { get; set; }
        public bool? IsSuperuser { get; set; }
        public DateTime? DteLastLogin { get; set; }
        public bool? IsOwner { get; set; }
        public bool? IsActive { get; set; }
        public long? IntUrlId { get; set; }
        //public long? IntAccountId { get; set; }
        public long? IntCountryId { get; set; }
        public string? IntOfficeMail { get; set; }
        public string? StrContactNo { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        //public long? IntCreatedBy { get; set; }
        //public DateTime? DteUpdatedAt { get; set; }
        //public long? IntUpdatedBy { get; set; }
    }
}
