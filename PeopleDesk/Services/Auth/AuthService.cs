using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.MasterData;
using System.Text;
using User = PeopleDesk.Data.Entity.User;

namespace PeopleDesk.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly PeopleDeskContext _context;

        public AuthService(PeopleDeskContext _context)
        {
            this._context = _context;
        }

        public async Task<User> SaveUser(User obj)
        {
            try
            {
                if (obj.IntUserId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.Users.AddRangeAsync(obj);
                    await _context.SaveChangesAsync();
                }

                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FeatureCommontDDL>> GetUserList(BaseVM tokenData, long businessUnitId, long workplaceGroupId, string? Search)
        {
            try
            {
                Search = Search == null? Search : Search.ToLower();
                var res = await (from usr in _context.Users
                                 join emp in _context.EmpEmployeeBasicInfos on usr.IntRefferenceId equals emp.IntEmployeeBasicInfoId
                                 join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                 from empD in empD2.DefaultIfEmpty()
                                 join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into dess
                                 from desInfo in dess.DefaultIfEmpty()

                                 where usr.IntAccountId == tokenData.accountId
                                 && (!string.IsNullOrEmpty(Search) && emp != null ? (emp.StrEmployeeName.ToLower().Contains(Search) || emp.StrEmployeeCode.ToLower().Contains(Search)
                                 || desInfo.StrDesignation.ToLower().Contains(Search)) : true)

                                 && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                                 && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                                && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                                : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                                : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                                : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                                : tokenData.territoryList.Contains(empD.IntTerritoryId))
                                || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                                 select new FeatureCommontDDL()
                                 {
                                     Value = emp.IntEmployeeBasicInfoId,
                                     Label = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "] " + (desInfo != null ? ", " + desInfo.StrDesignation : ""),
                                     Name = usr.StrDisplayName
                                 }).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<User>> GetAllUserList(long accountId)
        {
            try
            {
                return await _context.Users.Where(x => x.IsActive == true && x.IntAccountId == accountId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserByUserId(long userId)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.IntUserId == userId);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserIsExists(long loginUrlId, long accountId, string loginId)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.IntUrlId == loginUrlId && x.IntAccountId == accountId && x.StrLoginId == loginId);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UserViewModel>> UserLanding(long accountId)
        {
            try
            {
                List<UserViewModel> userList = await _context.Users.Where(x => (x.IntAccountId == accountId || accountId == 0) && x.IsActive == true)
                                .Select(y => new UserViewModel
                                {
                                    IntUserId = y.IntUserId,
                                    StrLoginId = y.StrLoginId,
                                    StrDisplayName = y.StrDisplayName,
                                    IntUserTypeId = y.IntUserTypeId,
                                    StrUserType = _context.UserTypes.FirstOrDefault(u => u.IntUserTypeId == y.IntUserTypeId).StrUserType,
                                    IntRefferenceId = y.IntRefferenceId,
                                    IsOfficeAdmin = y.IsOfficeAdmin,
                                    IsSuperuser = y.IsSuperuser,
                                    DteLastLogin = y.DteLastLogin,
                                    IntUrlId = y.IntUrlId,
                                    StrUrl = _context.Urls.FirstOrDefault(u => u.IntUrlId == y.IntUrlId).StrUrl,
                                    IntAccountId = accountId
                                }).ToListAsync();
                return userList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmpEmployeeBasicInfo> GetEmployeeBasicInfoByLoginUserId(string loginUserId)
        {
            var data = await _context.Users.Where(x => x.StrLoginId == loginUserId).FirstOrDefaultAsync();
            EmpEmployeeBasicInfo obj = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == (long)data.IntRefferenceId).FirstOrDefaultAsync();

            return obj;
        }

        public async Task<MessageHelper> CreateRoleGroup(RoleGroupCommongViewModel obj)
        {
            try
            {
                if (obj.Header.IntRoleGroupId > 0) // update
                {
                    var existingItemList = new List<RoleGroupRow>();
                    var newItemList = new List<RoleGroupRow>();
                    var inactiveItemList = new List<RoleGroupRow>();

                    foreach (var item in obj.Rows)
                    {
                        if (item.IntRowId > 0)
                        {
                            existingItemList.Add(item);
                        }
                        else
                        {
                            item.IsActive = true;
                            item.DteCreatedAt = DateTime.Now;
                            newItemList.Add(item);
                        }
                    }
                    inactiveItemList = await (from rg in _context.RoleGroupRows
                                              where rg.IntRoleGroupId == obj.Header.IntRoleGroupId && rg.IsActive == true
                                              && !existingItemList.Select(x => x.IntRowId).ToList().Contains(rg.IntRowId)
                                              select rg).ToListAsync();
                    inactiveItemList.ForEach(x =>
                    {
                        x.IsActive = false;
                    });

                    if (newItemList.Count() > 0)
                    {
                        await _context.RoleGroupRows.AddRangeAsync(newItemList);
                        await _context.SaveChangesAsync();
                    }
                    if (inactiveItemList.Count > 0)
                    {
                        _context.RoleGroupRows.UpdateRange(inactiveItemList);
                        await _context.SaveChangesAsync();
                    }
                }
                else // create
                {
                    var existdata = await (_context.RoleGroupHeaders
                                                                  .Where(x => x.IntBusinessUnitId == obj.Header.IntBusinessUnitId && x.IsActive == true &&
                                                                            (x.StrRoleGroupName == obj.Header.StrRoleGroupName || x.StrRoleGroupCode == obj.Header.StrRoleGroupCode)).FirstOrDefaultAsync());
                    if (existdata != null)
                    {
                        throw new Exception("Group Name or Code Already exists");
                    }

                    var head = new RoleGroupHeader();
                    head = obj.Header;
                    head.IsActive = true;
                    head.DteCreatedAt = DateTime.Now;

                    await _context.RoleGroupHeaders.AddAsync(head);
                    await _context.SaveChangesAsync();

                    var row = new List<RoleGroupRow>();
                    row.AddRange(obj.Rows);

                    row.ForEach(x =>
                    {
                        x.IntRoleGroupId = head.IntRoleGroupId;
                        x.IsActive = true;
                        x.DteCreatedAt = DateTime.Now;
                    });

                    await _context.RoleGroupRows.AddRangeAsync(row);
                    await _context.SaveChangesAsync();
                }
                return new MessageHelper()
                {
                    Message = "Operation Successful",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RoleGroupHeader>> GetRoleGroupLanding(long AccountId, long BusinessUnitId)
        {
            try
            {
                var res = await (_context.RoleGroupHeaders.Where(x => x.IntAccountId == AccountId && x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true).ToListAsync());
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RoleGroupCommongViewModel> GetRoleGroupById(long RoleGroupId)
        {
            try
            {
                var head = await _context.RoleGroupHeaders.Where(x => x.IntRoleGroupId == RoleGroupId && x.IsActive == true).FirstOrDefaultAsync();
                var row = await _context.RoleGroupRows.Where(x => x.IntRoleGroupId == RoleGroupId && x.IsActive == true).ToListAsync();

                var res = new RoleGroupCommongViewModel()
                {
                    Header = head,
                    Rows = row,
                };

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string EnCoding(string password)
        {
            var Password = Encoding.UTF8.GetBytes(password);
            string encryptedPassword = Convert.ToBase64String(Password);
            return encryptedPassword;
        }

        public string DeCoding(string password)
        {
            var Password = System.Convert.FromBase64String(password);
            string encryptedPassword = Encoding.UTF8.GetString(Password);
            return encryptedPassword;
        }

        #region === M E N U   &   F E A T U R E S ===

        ///For Controll Panel
        public async Task<Menu> CRUDMenus(Menu menu)
        {
            if (menu.IntMenuId > 0)
            {
                _context.Menus.Update(menu);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.AddAsync(menu);
                await _context.SaveChangesAsync();
            }
            return menu;
        }

        ///For Menu UI

        public async Task<List<MenuViewModel>> GetMenuListPermissionWise(long EmployeeId)
        {
            try
            {
                List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(EmployeeId);

                List<Menu> allMenus = await (from parent in _context.Menus
                                             where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForWeb == true
                                             join child in _context.Menus on parent.IntMenuId equals child.IntParentMenuId
                                             where child.IsActive == true
                                             join item in _context.Menus on child.IntMenuId equals item.IntParentMenuId
                                             where item.IsActive == true
                                             join per in _context.MenuPermissions on item.IntMenuId equals per.IntMenuId
                                             where per.IsActive == true && per.IsForWeb == true
                                             && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                   || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                             select parent).Distinct().ToListAsync();

                List<Menu> child_list_one = await (from parent in _context.Menus
                                                   where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForWeb == true
                                                   join child in _context.Menus on parent.IntMenuId equals child.IntParentMenuId
                                                   where child.IsActive == true
                                                   join item in _context.Menus on child.IntMenuId equals item.IntParentMenuId
                                                   where item.IsActive == true
                                                   join per in _context.MenuPermissions on item.IntMenuId equals per.IntMenuId
                                                   where per.IsActive == true && per.IsForWeb == true
                                                   && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                   select child).Distinct().ToListAsync();

                List<Menu> child_list_two = await (from parent in _context.Menus
                                                   where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForWeb == true
                                                   join child in _context.Menus on parent.IntMenuId equals child.IntParentMenuId
                                                   where child.IsActive == true
                                                   join item in _context.Menus on child.IntMenuId equals item.IntParentMenuId
                                                   where item.IsActive == true
                                                   join per in _context.MenuPermissions on item.IntMenuId equals per.IntMenuId
                                                   where per.IsActive == true && per.IsForWeb == true
                                                   && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                   select item).Distinct().ToListAsync();

                List<Menu> child_list_three = await (from parent in _context.Menus
                                                     where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForWeb == true
                                                     join child in _context.Menus on parent.IntMenuId equals child.IntParentMenuId
                                                     where child.IsActive == true
                                                     join per in _context.MenuPermissions on child.IntMenuId equals per.IntMenuId
                                                     where per.IsActive == true && per.IsForWeb == true
                                                     && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                     select parent).Distinct().ToListAsync();

                List<Menu> child_list_four = await (from parent in _context.Menus
                                                    where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForWeb == true
                                                    join child in _context.Menus on parent.IntMenuId equals child.IntParentMenuId
                                                    where child.IsActive == true
                                                    join per in _context.MenuPermissions on child.IntMenuId equals per.IntMenuId
                                                    where per.IsActive == true && per.IsForWeb == true
                                                    && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                    select child).Distinct().ToListAsync();

                allMenus.AddRange(child_list_one);
                allMenus.AddRange(child_list_two);
                allMenus.AddRange(child_list_three);
                allMenus.AddRange(child_list_four);

                allMenus = allMenus.Distinct().OrderBy(x => x.IntMenuSerial).ToList();

                List<MenuViewModel> filteredData = new List<MenuViewModel>();

                foreach (Menu parent in allMenus.Where(x => x.IntMenuLabelId == 1 && x.IsExpandable).ToList())
                {
                    MenuViewModel menuView = new MenuViewModel
                    {
                        Id = parent.IntMenuId,
                        Icon = parent.StrIcon,
                        Label = parent.StrMenuNameWeb,
                        To = parent.StrTo,
                        IsFirstLabel = parent.IsExpandable,
                        ParentId = parent.IntParentMenuId,
                        ThirdLabelSl = parent.IntMenuSerial,
                        ChildList = new List<MenuViewModel>()
                    };

                    if (allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == true && x.IntParentMenuId == parent.IntMenuId).Count() > 0)
                    {
                        foreach (Menu child in allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == true && x.IntParentMenuId == parent.IntMenuId).ToList())
                        {
                            MenuViewModel childView = new MenuViewModel
                            {
                                Id = child.IntMenuId,
                                Icon = child.StrIcon,
                                Label = child.StrMenuNameWeb,
                                To = child.StrTo,
                                IsSecondLabel = child.IsExpandable,
                                ParentId = child.IntParentMenuId,
                                ThirdLabelSl = child.IntMenuSerial,
                                ChildList = new List<MenuViewModel>()
                            };
                            if (allMenus.Where(x => x.IntMenuLabelId == 3 && x.IsExpandable == false && x.IntParentMenuId == child.IntMenuId).Count() > 0)
                            {
                                childView.ChildList = allMenus.Where(x => x.IntMenuLabelId == 3 && x.IsExpandable == false && x.IntParentMenuId == child.IntMenuId)
                                                    .Select(y => new MenuViewModel
                                                    {
                                                        Id = y.IntMenuId,
                                                        Icon = y.StrIcon,
                                                        Label = y.StrMenuNameWeb,
                                                        To = y.StrTo,
                                                        IsFirstLabel = y.IsExpandable,
                                                        ParentId = y.IntParentMenuId,
                                                        ThirdLabelSl = y.IntMenuSerial,
                                                    }).OrderBy(y => y.ThirdLabelSl).Distinct().ToList();

                                menuView.ChildList.Add(childView);
                            }
                        }
                    }

                    if (allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == false && x.IntParentMenuId == parent.IntMenuId).Count() > 0)
                    {
                        List<MenuViewModel> chilsAsLastNode = allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == false && x.IntParentMenuId == parent.IntMenuId)
                                             .Select(y => new MenuViewModel
                                             {
                                                 Id = y.IntMenuId,
                                                 Icon = y.StrIcon,
                                                 Label = y.StrMenuNameWeb,
                                                 To = y.StrTo,
                                                 IsFirstLabel = y.IsExpandable,
                                                 ParentId = y.IntParentMenuId,
                                                 ThirdLabelSl = y.IntMenuSerial,
                                             }).OrderBy(y => y.ThirdLabelSl).Distinct().ToList();

                        menuView.ChildList.AddRange(chilsAsLastNode);
                    }

                    if (allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == true && x.IntParentMenuId == parent.IntMenuId).Count() > 0 ||
                       allMenus.Where(x => x.IntMenuLabelId == 2 && x.IsExpandable == false && x.IntParentMenuId == parent.IntMenuId).Count() > 0)
                    {
                        menuView.ChildList = (menuView.ChildList.OrderBy(x => x.ThirdLabelSl).ToList());
                        filteredData.Add(menuView);
                    }
                }

                return filteredData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MenuViewModel>> GetMenuListPermissionWiseApps(long EmployeeId)
        {
            try
            {
                List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(EmployeeId);

                List<Menu> allMenus = await (from parent in _context.Menus
                                             where parent.IsActive == true && parent.IntMenuLabelId == 1 && parent.IsForApps == true
                                             join per in _context.MenuPermissions on parent.IntMenuId equals per.IntMenuId
                                             where per.IsActive == true && per.IsForApps == true
                                             && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                   || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                             select parent).Distinct().ToListAsync();

                List<Menu> secondLabelMenus = await (from child in _context.Menus
                                                     where child.IsActive == true && child.IntMenuLabelId == 2 && child.IsForApps == true
                                                     join per in _context.MenuPermissions on child.IntMenuId equals per.IntMenuId
                                                     where per.IsActive == true && per.IsForApps == true
                                                     && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                     select child).Distinct().ToListAsync();

                List<Menu> thirdLabelMenus = await (from item in _context.Menus
                                                    where item.IsActive == true && item.IntMenuLabelId == 3 && item.IsForApps == true
                                                    join per in _context.MenuPermissions on item.IntMenuId equals per.IntMenuId
                                                    where per.IsActive == true && per.IsForApps == true
                                                    && ((per.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(per.IntEmployeeOrRoleId))
                                                        || (per.StrIsFor.ToLower() == "Employee".ToLower() && per.IntEmployeeOrRoleId == EmployeeId))
                                                    select item).Distinct().ToListAsync();

                allMenus.AddRange(secondLabelMenus);
                allMenus.AddRange(thirdLabelMenus);

                if (allMenus.Where(x => x.IsHasApproval == true).Count() > 0 && allMenus.Where(x => x.IntMenuId == 4).Count() <= 0)
                {
                    Menu menu = await _context.Menus.FirstOrDefaultAsync(x => x.IntMenuId == 4);
                    if (menu != null)
                    {
                        allMenus.Add(menu);
                    }
                }

                allMenus = allMenus.Distinct().OrderBy(x => x.IntMenuSerial).ToList();

                List<MenuViewModel> filteredData = new List<MenuViewModel>();

                foreach (Menu parent in allMenus.Where(x => x.IntMenuLabelId == 1).ToList())
                {
                    MenuViewModel menuView = new MenuViewModel
                    {
                        Id = parent.IntMenuId,
                        Icon = parent.StrIconForApps,
                        Label = parent.StrMenuNameApps,
                        To = parent.StrToForApps,
                        IsFirstLabel = parent.IsExpandable,
                        ParentId = parent.IntParentMenuId,
                        ThirdLabelSl = parent.IntMenuSerialForApps,
                        ChildList = allMenus.Where(x => x.IntParentMenuId == parent.IntMenuId).Select(child => new MenuViewModel
                        {
                            Id = child.IntMenuId,
                            Icon = child.StrIconForApps,
                            Label = child.StrMenuNameApps,
                            To = child.StrToForApps,
                            IsFirstLabel = child.IsExpandable,
                            ParentId = child.IntParentMenuId,
                            ThirdLabelSl = child.IntMenuSerialForApps,
                            ChildList = allMenus.Where(x => x.IntParentMenuId == child.IntMenuId).Select(item => new MenuViewModel
                            {
                                Id = item.IntMenuId,
                                Icon = item.StrIconForApps,
                                Label = item.StrMenuNameApps,
                                To = item.StrToForApps,
                                IsFirstLabel = item.IsExpandable,
                                ParentId = item.IntParentMenuId,
                                ThirdLabelSl = item.IntMenuSerialForApps
                            }).OrderBy(item => item.ThirdLabelSl).ToList()
                        }).OrderBy(item => item.ThirdLabelSl).ToList()
                    };
                    filteredData.Add(menuView);
                }

                return filteredData.OrderBy(x => x.ThirdLabelSl).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class parentIdClass
        {
            public long parentId { get; set; }
        }

        public async Task<List<FeatureCommontDDL>> GetFirstLevelMenuList(long employeeId)
        {
            try
            {
                List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(employeeId);

                List<Menu> list_one = await (from f in _context.Menus
                                             join sec in _context.Menus on f.IntMenuId equals sec.IntParentMenuId
                                             join p in _context.MenuPermissions on sec.IntMenuId equals p.IntMenuId
                                             where p.IsActive == true && f.IsActive == true && sec.IsActive == true
                                             && sec.IsExpandable == false && (sec.IsForWeb == true || sec.IsForApps == true)
                                             && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                   || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == employeeId))
                                             select f).Distinct().ToListAsync();

                List<Menu> list_two = await (from f in _context.Menus
                                             join sec in _context.Menus on f.IntMenuId equals sec.IntParentMenuId
                                             join th in _context.Menus on sec.IntMenuId equals th.IntParentMenuId
                                             join p in _context.MenuPermissions on th.IntMenuId equals p.IntMenuId
                                             where p.IsActive == true && f.IsActive == true && sec.IsActive == true && th.IsActive == true
                                             && th.IsExpandable == false && (sec.IsForWeb == true || sec.IsForApps == true)
                                             && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                   || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == employeeId))
                                             select f).Distinct().ToListAsync();

                list_two.AddRange(list_one);

                List<FeatureCommontDDL> result = list_two.Distinct().Where(x => x.IntParentMenuId == 0).GroupBy(x => new
                {
                    x.IntMenuId,
                    x.StrMenuName,
                    x.IsForWeb,
                    x.IsForApps
                }).Select(s => new FeatureCommontDDL
                {
                    Value = s.Key.IntMenuId,
                    Label = s.Key.StrMenuName + (s.Key.IsForWeb == false && s.Key.IsForApps == true ? " (Apps)" : "")
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FeatureCommontWithExtraDDL>> GetMenuFeatureList(long firstLevelMenuId, long employeeId)
        {
            try
            {
                List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(employeeId);

                List<FeatureCommontWithExtraDDL> activityList = await (from f in _context.Menus
                                                                       where f.IntMenuId == firstLevelMenuId
                                                                       join sec in _context.Menus on f.IntMenuId equals sec.IntParentMenuId
                                                                       join p in _context.MenuPermissions on sec.IntMenuId equals p.IntMenuId
                                                                       where p.IsActive == true && f.IsActive == true && sec.IsActive == true && sec.IsExpandable == false
                                                                       && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                                             || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == employeeId))
                                                                       select new FeatureCommontWithExtraDDL
                                                                       {
                                                                           Value = p.IntMenuId,
                                                                           Label = p.IsForWeb == true ? sec.StrMenuNameWeb : p.IsForApps == true ? sec.StrMenuNameApps : sec.StrMenuName,
                                                                           IsForWeb = p.IsForWeb,
                                                                           IsForApps = p.IsForApps
                                                                       }).Distinct().ToListAsync();

                List<FeatureCommontWithExtraDDL> activityList2 = await (from f in _context.Menus
                                                                        where f.IntMenuId == firstLevelMenuId
                                                                        join sec in _context.Menus on f.IntMenuId equals sec.IntParentMenuId
                                                                        join th in _context.Menus on sec.IntMenuId equals th.IntParentMenuId
                                                                        join p in _context.MenuPermissions on th.IntMenuId equals p.IntMenuId
                                                                        where p.IsActive == true && f.IsActive == true && sec.IsActive == true && th.IsActive == true && th.IsExpandable == false
                                                                        && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                                             || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == employeeId))
                                                                        select new FeatureCommontWithExtraDDL
                                                                        {
                                                                            Value = p.IntMenuId,
                                                                            Label = p.IsForWeb == true ? th.StrMenuNameWeb : p.IsForApps == true ? th.StrMenuNameApps : sec.StrMenuName,
                                                                            IsForWeb = p.IsForWeb,
                                                                            IsForApps = p.IsForApps
                                                                        }).Distinct().ToListAsync();

                activityList2.AddRange(activityList);
                activityList = activityList2.Distinct().OrderBy(x => x.Value).ToList();

                return activityList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> ProcessMenuUserPermission(List<UserForPermissionViewModel> UserList, List<MenuForPermissionViewModel> MenuList, InfoForPermissionViewModel infoForPermission, string isFor)
        {
            try
            {
                foreach (UserForPermissionViewModel user in UserList)
                {
                    List<MenuPermission> updateItemList = new List<MenuPermission>();
                    List<MenuPermission> newItemList = new List<MenuPermission>();
                    List<MenuPermission> employeeExistsByRoleList = new List<MenuPermission>();

                    if (isFor.ToLower() == "Employee".ToLower())
                    {
                        List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(user.IntEmployeeId);

                        employeeExistsByRoleList = await _context.MenuPermissions.Where(x => x.StrIsFor.ToLower() == "Role".ToLower()
                                                    && userRoleList.Select(x => x.Value).Contains(x.IntEmployeeOrRoleId)).ToListAsync();

                        MenuList = MenuList.Where(x => !employeeExistsByRoleList.Select(ex => ex.IntMenuId).Contains(x.IntMenuId)).ToList();
                    }

                    foreach (MenuForPermissionViewModel menu in MenuList)
                    {
                        MenuPermission role = await _context.MenuPermissions.FirstOrDefaultAsync(x => x.IntMenuId == menu.IntMenuId
                                                && x.IntEmployeeOrRoleId == user.IntEmployeeId && x.StrIsFor.ToLower() == isFor.ToLower());

                        if (role != null)
                        {
                            role.StrIsFor = isFor;
                            role.IntEmployeeOrRoleId = user.IntEmployeeId;
                            role.StrEmployeeName = user.StrEmployeeName;
                            role.IntModuleId = menu.ModuleId;
                            role.StrModuleName = menu.ModuleName;
                            role.IntMenuId = menu.IntMenuId;
                            role.StrMenuName = menu.StrMenuName;
                            role.IsCreate = menu.IsCreate;
                            role.IsEdit = menu.IsEdit;
                            role.IsView = menu.IsView;
                            role.IsDelete = menu.IsDelete;
                            role.IntCreatedBy = infoForPermission.IntCreatedBy;
                            role.DteCreatedAt = DateTime.Now;
                            role.IsActive = (menu.IsCreate == false && menu.IsEdit == false && menu.IsView == false) ? false : true;
                            role.IsForWeb = menu.IsForWeb;
                            role.IsForApps = menu.IsForApps;

                            updateItemList.Add(role);
                        }
                        else
                        {
                            MenuPermission result = new MenuPermission
                            {
                                StrIsFor = isFor,
                                IntEmployeeOrRoleId = user.IntEmployeeId,
                                StrEmployeeName = user.StrEmployeeName,
                                IntModuleId = menu.ModuleId,
                                StrModuleName = menu.ModuleName,
                                IntMenuId = menu.IntMenuId,
                                StrMenuName = menu.StrMenuName,
                                IsCreate = menu.IsCreate,
                                IsEdit = menu.IsEdit,
                                IsView = menu.IsView,
                                IsDelete = menu.IsDelete,
                                IntCreatedBy = infoForPermission.IntCreatedBy,
                                DteCreatedAt = DateTime.Now,
                                IsActive = (menu.IsCreate == false && menu.IsEdit == false && menu.IsView == false) ? false : true,
                                IsForWeb = menu.IsForWeb,
                                IsForApps = menu.IsForApps,
                            };

                            newItemList.Add(result);
                        }
                    }

                    if (updateItemList.Count > 0)
                    {
                        _context.MenuPermissions.UpdateRange(updateItemList);
                        await _context.SaveChangesAsync();
                    }
                    if (newItemList.Count > 0)
                    {
                        await _context.MenuPermissions.AddRangeAsync(newItemList);
                        await _context.SaveChangesAsync();
                    }
                }

                var msg = new MessageHelper();
                msg.Message = "Successfully Processed";
                msg.StatusCode = 200;

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CreateMenuUserPermissionForUserFeature(MenuUserPermissionForUserFeatureViewModel menuUserPermission, string isFor)
        {
            try
            {
                List<UserForPermissionViewModel> userList = new List<UserForPermissionViewModel>();
                userList.Add(menuUserPermission.userInfo);

                InfoForPermissionViewModel info = new InfoForPermissionViewModel()
                {
                    IntCreatedBy = menuUserPermission.IntCreatedBy,
                    IntMenuPermissionId = 1,
                };

                MessageHelper res = await ProcessMenuUserPermission(userList, menuUserPermission.menuList, info, isFor);

                MessageHelper msg = new MessageHelper();
                msg.Message = res.Message;
                msg.StatusCode = res.StatusCode;

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MenuUserPermissionViewModel>> GetMenuUserPermission(long EmployeeId, string isFor)
        {
            try
            {
                if (isFor.ToLower() == "Employee".ToLower())
                {
                    List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(EmployeeId);

                    List<MenuUserPermissionViewModel> data = await (from p in _context.MenuPermissions
                                                                    join m in _context.Menus on p.IntMenuId equals m.IntMenuId
                                                                    where p.IsActive == true && m.IsActive == true
                                                                    && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                                       || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == EmployeeId))
                                                                    select new MenuUserPermissionViewModel()
                                                                    {
                                                                        UserPermissionId = p.IntMenuPermissionId,
                                                                        IntEmployeeId = p.IntEmployeeOrRoleId,
                                                                        StrEmployeeName = p.StrEmployeeName,
                                                                        ModuleId = p.IntModuleId,
                                                                        ModuleName = p.StrModuleName,
                                                                        IntMenuId = p.IntMenuId,
                                                                        StrMenuName = p.StrMenuName,
                                                                        IsCreate = p.IsCreate,
                                                                        IsEdit = p.IsEdit,
                                                                        IsView = p.IsView,
                                                                        IsDelete = p.IsDelete,
                                                                        IsForWeb = p.IsForWeb,
                                                                        IsForApps = p.IsForApps
                                                                    }).Distinct().ToListAsync();

                    List<MenuUserPermissionViewModel> filteredData = new List<MenuUserPermissionViewModel>();

                    data.ForEach(data =>
                    {
                        if (!filteredData.Select(x => x.IntMenuId).Contains(data.IntMenuId))
                        {
                            filteredData.Add(data);
                        }
                    });

                    return filteredData;
                }
                else if (isFor.ToLower() == "Role".ToLower())
                {
                    List<MenuUserPermissionViewModel> data = await (from p in _context.MenuPermissions
                                                                    join m in _context.Menus on p.IntMenuId equals m.IntMenuId
                                                                    where p.IsActive == true && m.IsActive == true
                                                                    && p.StrIsFor.ToLower() == "Role".ToLower() && p.IntEmployeeOrRoleId == EmployeeId
                                                                    select new MenuUserPermissionViewModel()
                                                                    {
                                                                        UserPermissionId = p.IntMenuPermissionId,
                                                                        IntEmployeeId = p.IntEmployeeOrRoleId,
                                                                        StrEmployeeName = p.StrEmployeeName,
                                                                        ModuleId = p.IntModuleId,
                                                                        ModuleName = p.StrModuleName,
                                                                        IntMenuId = p.IntMenuId,
                                                                        StrMenuName = p.StrMenuName,
                                                                        IsCreate = p.IsCreate,
                                                                        IsEdit = p.IsEdit,
                                                                        IsView = p.IsView,
                                                                        IsDelete = p.IsDelete,
                                                                        IsForWeb = p.IsForWeb,
                                                                        IsForApps = p.IsForApps
                                                                    }).Distinct().ToListAsync();
                    return data;
                }
                else
                {
                    return new List<MenuUserPermissionViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ForActivityCheckViewModel>> GetMenuUserPermissionForActivityCheck(long employeeId)
        {
            try
            {
                List<RoleValuLabelVM> userRoleList = await GetAllAssignedRoleByEmployeeId(employeeId);

                List<MenuPermission> permissionList = await (from p in _context.MenuPermissions.AsNoTracking().AsQueryable()
                                                             join m in _context.Menus on p.IntMenuId equals m.IntMenuId
                                                             where m.IsForWeb == true && p.IsActive == true && m.IsActive == true
                                                             && ((p.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(p.IntEmployeeOrRoleId))
                                                                   || (p.StrIsFor.ToLower() == "Employee".ToLower() && p.IntEmployeeOrRoleId == employeeId))
                                                             select p).AsNoTracking().AsQueryable().ToListAsync();

                List<ForActivityCheckViewModel> activityList = await _context.Menus
                                                .Where(x => !_context.Menus.Where(i => i.IsActive == true).Select(a => a.IntParentMenuId).Contains(x.IntMenuId) && x.IsForWeb == true && x.IsForCommon == false && x.IsActive == true)
                                                .OrderBy(x => x.IntMenuLabelId)
                                                .Select(x => new ForActivityCheckViewModel()
                                                {
                                                    ModuleId = 0,
                                                    ModuleName = "",
                                                    MenuReferenceId = x.IntMenuId,
                                                    MenuReferenceName = x.StrMenuName,
                                                    IsCreate = false,
                                                    IsEdit = false,
                                                    IsView = false,
                                                    IsClose = false
                                                }).AsNoTracking().AsQueryable().ToListAsync();

                permissionList.ForEach(permission =>
                {
                    activityList.ForEach(activity =>
                    {
                        if (activity.MenuReferenceId == permission.IntMenuId)
                        {
                            activity.IsCreate = (bool)permission.IsCreate;
                            activity.IsView = (bool)permission.IsView;
                            activity.IsEdit = (bool)permission.IsEdit;
                            activity.IsClose = (bool)permission.IsDelete;
                            activity.ModuleId = permission.IntModuleId;
                            activity.ModuleName = permission.StrModuleName;
                            activity.MenuReferenceId = permission.IntMenuId;
                            activity.MenuReferenceName = permission.StrMenuName;
                        }
                    });
                });
                return activityList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion === M E N U   &   F E A T U R E S ===

        #region Others

        public async Task<List<RoleValuLabelVM>> GetAllAssignedRoleByEmployeeId(long employeeId)
        {
            EmpEmployeeBasicInfo empEmployeeBasicInfo = await _context.EmpEmployeeBasicInfos.AsNoTracking().AsNoTracking().FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == employeeId);

            if (empEmployeeBasicInfo != null)
            {
                List<RoleValuLabelVM> userRoleList = await (from item in _context.RoleBridgeWithDesignations
                                                            where ((item.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntDesignationId && item.StrIsFor.ToLower() == "Designation")
                                                            || (item.IntDesignationOrEmployeeId == empEmployeeBasicInfo.IntEmployeeBasicInfoId && item.StrIsFor.ToLower() == "Employee"))
                                                            && item.IsActive == true
                                                            select new RoleValuLabelVM
                                                            {
                                                                Value = (long)item.IntRoleId,
                                                                Label = "Role"
                                                            }).AsNoTracking().AsQueryable().ToListAsync();

                bool isOwner = await _context.Users.AsNoTracking().AsQueryable().Where(x => x.IntRefferenceId == employeeId).Select(x => x.IsOwner).FirstOrDefaultAsync();

                if (!isOwner)
                {
                    RoleValuLabelVM defaultRole = await _context.UserRoles.AsNoTracking().AsQueryable()
                    .Where(x => x.IntAccountId == empEmployeeBasicInfo.IntAccountId && x.IsDefault == true && x.IsActive == true)
                    .Select(x => new RoleValuLabelVM { Value = x.IntRoleId, Label = "Role" }).FirstOrDefaultAsync();

                    userRoleList.Add(defaultRole);
                }

                return userRoleList.Distinct().ToList();
            }
            else
            {
                return new List<RoleValuLabelVM>();
            }
        }

        #endregion Others
    }
}