namespace PeopleDesk.Models.Auth
{
    public class RollExtensionViewModel
    {
        public long intEmployeeId { set; get; }
        public long intCreatedBy { set; get; }
        public List<RoleExtensionRowViewModel> RoleExtensionRowViewModelList { set; get; }
    }
    public class RoleExtensionRowViewModel
    {
        public long intRoleExtensionRowId { set; get; }
        public long intOrganizationTypeId { set; get; }
        public string strOrganizationTypeName { set; get; }
        public long intOrganizationReffId { set; get; }
        public string strOrganizationReffName { set; get; }
    }
}
