using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.MasterData;

namespace PeopleDesk.Services.Auth
{
    public interface IAuthService
    {
        Task<User> SaveUser(User user);

        Task<List<FeatureCommontDDL>> GetUserList(BaseVM tokenData, long businessUnitId, long workplaceGroupId, string? Search);

        Task<List<User>> GetAllUserList(long accountId);

        Task<User> GetUserByUserId(long userId);

        Task<User> GetUserIsExists(long loginUrlId, long accountId, string loginId);

        Task<List<UserViewModel>> UserLanding(long accountId);

        Task<EmpEmployeeBasicInfo> GetEmployeeBasicInfoByLoginUserId(string loginUserId);

        Task<MessageHelper> CreateRoleGroup(RoleGroupCommongViewModel obj);

        Task<List<RoleGroupHeader>> GetRoleGroupLanding(long AccountId, long BusinessUnitId);

        Task<RoleGroupCommongViewModel> GetRoleGroupById(long RoleGroupId);

        string EnCoding(string password);

        string DeCoding(string password);

        //string GetOTP();

        #region === M E N U   &   F E A T U R E S ===

        public Task<Menu> CRUDMenus(Menu menu);

        Task<List<MenuViewModel>> GetMenuListPermissionWise(long EmployeeId);

        Task<List<MenuViewModel>> GetMenuListPermissionWiseApps(long EmployeeId);

        Task<List<FeatureCommontDDL>> GetFirstLevelMenuList(long employeeId);

        Task<List<FeatureCommontWithExtraDDL>> GetMenuFeatureList(long firstLevelMenuId, long employeeId);

        Task<MessageHelper> CreateMenuUserPermissionForUserFeature(MenuUserPermissionForUserFeatureViewModel menuUserPermission, string isFor);

        Task<List<MenuUserPermissionViewModel>> GetMenuUserPermission(long EmployeeId, string isFor);

        Task<List<ForActivityCheckViewModel>> GetMenuUserPermissionForActivityCheck(long employeeId);

        Task<List<RoleValuLabelVM>> GetAllAssignedRoleByEmployeeId(long employeeId);

        #endregion === M E N U   &   F E A T U R E S ===
    }
}