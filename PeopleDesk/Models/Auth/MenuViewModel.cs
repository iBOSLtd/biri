namespace PeopleDesk.Models.Auth
{
    public class MenuViewModel
    {
        public long Id { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string To { get; set; }
        public bool? IsFirstLabel { get; set; }
        public bool? IsSecondLabel { get; set; }
        public bool? IsThirdLabel { get; set; }
        public long? ThirdLabelSl { get; set; }
        public long ParentId { get; set; }
        public List<MenuViewModel> ChildList { get; set; }
    }
    public class GetFirstLabelMenuDTO
    {
        public long Id { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string To { get; set; }
        public bool? IsFirstLabel { get; set; }
        public long ParentId { get; set; }
        public List<GetSecondLabelMenuDTO> Subs { get; set; }
    }
    public class GetSecondLabelMenuDTO
    {
        public long Id { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string To { get; set; }
        public bool? IsSecondLabel { get; set; }
        public long ParentId { get; set; }
        public List<GetThirdLabelMenuDTO> NestedSubs { get; set; }
    }
    public class GetThirdLabelMenuDTO
    {
        public long Id { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string To { get; set; }
        public bool? IsThirdLabel { get; set; }
        public long? ThirdLabelSl { get; set; }
        public long ParentId { get; set; }
    }

    public class InfoForPermissionViewModel
    {
        public long IntMenuPermissionId { get; set; }
        public long IntCreatedBy { get; set; }
    }
    public class UserForPermissionViewModel
    {
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
    }
    public class MenuForPermissionViewModel
    {
        public long? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public long IntMenuId { get; set; }
        public string StrMenuName { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsDelete { get; set; }
        public bool IsForWeb { get; set; }
        public bool IsForApps { get; set; }
    }

    public class MenuUserPermissionForUserFeatureViewModel
    {
        public long IntCreatedBy { get; set; } = 0;
        public long IntBusinessunitId { get; set; } = 0;
        public long IntWorkplaceGroupId { get; set; } = 0;
        public UserForPermissionViewModel userInfo { get; set; }
        public List<MenuForPermissionViewModel> menuList { get; set; }
    }
    public class MenuUserPermissionForUserGroupFeatureViewModel
    {
        public long IntCreatedBy { get; set; }
        public long UserGroupId { get; set; }
        public List<MenuForPermissionViewModel> menuList { get; set; }
    }
    public class MenuUserPermissionViewModel
    {
        public long UserPermissionId { get; set; }
        public long PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public long IntMenuId { get; set; }
        public string StrMenuName { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsView { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsCommon { get; set; }
        public bool IsForWeb { get; set; }
        public bool IsForApps { get; set; }

    }

    public class ForActivityCheckViewModel
    {
        public long? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public long MenuReferenceId { get; set; }
        public string MenuReferenceName { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsClose { get; set; }

    }
    public class AppActivityCheckViewModel
    {
        public bool IsSupervisorDashboard { get; set; }
        public bool IsLeaveApproval { get; set; }
        public bool IsMovementApproval { get; set; }
        public bool IsAttendanceApproval { get; set; }
        public bool IsManagement { get; set; }

    }

}
