namespace PeopleDesk.Models.Auth
{
    public class LoginViewModel
    {
        public string StrLoginId { get; set; } = null!;
        public string StrPassword { get; set; } = null!;
        public long IntUrlId { get; set; }
        public string StrUrl { get; set; }
        public long IntAccountId { get; set; }
    }

    public class LoginReturnViewModel
    {
        public string? StrLoginId { get; set; } = null!;
        public long? IntAccountId { get; set; }
        public long? IntUrlId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnit { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrDisplayName { get; set; } = null!;
        public long? IntProfileImageUrl { get; set; }
        public long? IntLogoUrlId { get; set; }
        public long? IntDefaultDashboardId { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }

        public long? IntUserTypeId { get; set; }
        public string? IntUserType { get; set; }
        public long? IntRefferenceId { get; set; }
        public bool? IsOfficeAdmin { get; set; }
        public bool? IsSuperuser { get; set; }
        public bool? IsOwner { get; set; }
        public long? IsSupNLMORManagement { get; set; }
        public DateTime? DteLastLogin { get; set; }
        public bool? IsLoggedIn { get; set; }

        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsLoggedInWithOtp { get; set; } = false;
        public string? StrOfficeMail { get; set; }
        public string? StrPersonalMail { get; set; }
    }
}