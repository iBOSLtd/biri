using PeopleDesk.Data.Entity;

namespace PeopleDesk.Models.Auth
{
    public class UserGroupViewModel
    {
        public UserGroupHeader UserGroupHeader { get; set; }
        public List<UserGroupRow> UserGroupRows { get; set; }
    }
    public class UserGroupHeaderViewModel : Base
    {
        public long IntUserGroupHeaderId { get; set; }
        public string StrUserGroup { get; set; } = null!;
        public string StrCode { get; set; } = null!;
        public string? StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public List<UserGroupRowViewModel> UserGroupRowViewModelList { get; set; }
    }
    public class UserGroupRowViewModel
    {
        public long IntUserGroupRowId { get; set; }
        public long IntUserGroupHeaderId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public bool IsCreate { get; set; }
        public bool IsDelete { get; set; }
    }
}
