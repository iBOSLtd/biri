using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.SignalR.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PeopleDesk.Services.Master
{
    public class SaasMasterService : ISaasMasterService
    {
        private readonly PeopleDeskContext _context;
        private readonly INotificationService _notificationService;

        private DataTable dt = new DataTable();

        public SaasMasterService(INotificationService _notificationService, PeopleDeskContext context)
        {
            _context = context;
            this._notificationService = _notificationService;
        }

        #region ================= Employee BusinessUnit ================

        public async Task<MasterBusinessUnit> SaveBusinessUnit(MasterBusinessUnit obj)
        {
            if (obj.IntBusinessUnitId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterBusinessUnits.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterBusinessUnits.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<MasterBusinessUnit>> GetAllBusinessUnit(long accountId)
        {
            return await _context.MasterBusinessUnits.Where(x => x.IsActive == true && x.IntAccountId == accountId).OrderByDescending(x => x.IntBusinessUnitId).AsNoTracking().AsQueryable().ToListAsync();
        }

        public async Task<MasterBusinessUnit> GetBusinessUnitById(long Id)
        {
            return await _context.MasterBusinessUnits.FirstAsync(x => x.IntBusinessUnitId == Id);
        }

        public async Task<MasterBusinessUnitDWithAccount> GetBusinessDetailsByBusinessUnitIdAsync(long Id)
        {
            MasterBusinessUnitDWithAccount businessUnitDetails = await (from masterBusinessUnit in _context.MasterBusinessUnits
                                                                        where masterBusinessUnit.IntBusinessUnitId == Id && masterBusinessUnit.IsActive == true
                                                                        join account in _context.Accounts on
                                                                        masterBusinessUnit.IntAccountId equals account.IntAccountId
                                                                        select new MasterBusinessUnitDWithAccount
                                                                        {
                                                                            IntBusinessUnitId = masterBusinessUnit.IntBusinessUnitId,
                                                                            StrBusinessUnit = masterBusinessUnit.StrBusinessUnit,
                                                                            StrShortCode = masterBusinessUnit.StrShortCode,
                                                                            StrBusinessUnitAddress = masterBusinessUnit.StrAddress,
                                                                            StrBusinessUnitLogoUrlId = masterBusinessUnit.StrLogoUrlId,
                                                                            IntDistrictId = masterBusinessUnit.IntDistrictId,
                                                                            StrEmail = masterBusinessUnit.StrEmail,
                                                                            StrWebsiteUrl = masterBusinessUnit.StrWebsiteUrl,
                                                                            StrCurrency = masterBusinessUnit.StrCurrency,
                                                                            IsActive = masterBusinessUnit.IsActive,
                                                                            IntAccountId = account.IntAccountId,
                                                                            StrAccountName = account.StrAccountName,
                                                                            StrAccountAddress = account.StrAddress,
                                                                            IntAccountLogoUrlId = account.IntLogoUrlId,
                                                                            DteCreatedAt = masterBusinessUnit.DteCreatedAt,
                                                                            IntCreatedBy = masterBusinessUnit.IntCreatedBy,
                                                                            DteUpdatedAt = masterBusinessUnit.DteUpdatedAt,
                                                                            IntUpdatedBy = masterBusinessUnit.IntUpdatedBy
                                                                        }).AsQueryable().AsNoTracking().FirstOrDefaultAsync();

            return businessUnitDetails;
        }

        public async Task<bool> DeleteBusinessUnit(long id)
        {
            try
            {
                MasterBusinessUnit obj = await _context.MasterBusinessUnits.FirstAsync(x => x.IntBusinessUnitId == id);
                obj.IsActive = false;

                _context.MasterBusinessUnits.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================= Employee BusinessUnit ================

        #region ==================== Employee Designation ===================

        public async Task<MasterDesignation> SaveDesignation(MasterDesignation obj)
        {
            if (obj.IntDesignationId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterDesignations.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterDesignations.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<DesignationViewModel>> GetAllDesignation(long accountId, long businessUnitId)
        {
            IEnumerable<DesignationViewModel> designationList = await (from d in _context.MasterDesignations
                                                                       join bu in _context.MasterBusinessUnits on d.IntBusinessUnitId equals bu.IntBusinessUnitId into bu2
                                                                       from bus in bu2.DefaultIfEmpty()
                                                                       where d.IsActive == true && d.IsDeleted == false && d.IntAccountId == accountId
                                                                       select new DesignationViewModel
                                                                       {
                                                                           IntDesignationId = d.IntDesignationId,
                                                                           StrDesignation = d.StrDesignation,
                                                                           StrDesignationCode = d.StrDesignationCode,
                                                                           IsActive = d.IsActive,
                                                                           IsDeleted = d.IsDeleted,
                                                                           IntBusinessUnitId = d.IntBusinessUnitId,
                                                                           StrBusinessUnit = bus != null ? bus.StrBusinessUnit : "",
                                                                           IntAccountId = d.IntAccountId,
                                                                           IntCreatedBy = d.IntCreatedBy,
                                                                           DteCreatedAt = d.DteCreatedAt,
                                                                           IntUpdatedBy = d.IntUpdatedBy,
                                                                           DteUpdatedAt = d.DteUpdatedAt,
                                                                           StrBusinessUnitCode = bus.StrShortCode
                                                                       }).OrderByDescending(x => x.IntDesignationId).AsNoTracking().AsQueryable().ToListAsync();
            return designationList;
        }

        public async Task<DesignationVM> GetDesignationById(long id)
        {
            MasterDesignation designation = await _context.MasterDesignations.FirstOrDefaultAsync(x => x.IntDesignationId == id);

            if (designation != null)
            {
                DesignationVM obj = new DesignationVM
                {
                    IntDesignationId = designation.IntDesignationId,
                    StrDesignation = designation.StrDesignation,
                    StrDesignationCode = designation.StrDesignationCode,
                    IsActive = designation.IsActive,
                    IsDeleted = designation.IsDeleted,
                    IntAccountId = designation.IntAccountId,
                    IntCreatedBy = designation.IntCreatedBy,
                    DteCreatedAt = designation.DteCreatedAt,
                    IntUpdatedBy = designation.IntUpdatedBy,
                    DteUpdatedAt = designation.DteUpdatedAt,
                    IntPayscaleGradeId = designation.IntPayscaleGradeId,
                    StrPayscaleGradeName = await _context.PyrPayscaleGrades.Where(x => x.IntPayscaleGradeId == designation.IntPayscaleGradeId).Select(x => x.StrPayscaleGradeName).FirstOrDefaultAsync(),
                    BusinessUnitValuLabelVMList = new List<BusinessUnitValuLabelVM>(),
                    RoleValuLabelVMList = new List<RoleValuLabelVM>()
                };

                if (designation.IntBusinessUnitId == 0)
                {
                    obj.BusinessUnitValuLabelVMList.Add(new BusinessUnitValuLabelVM
                    {
                        Value = 0,
                        Label = "ALL"
                    });
                }
                else
                {
                    obj.BusinessUnitValuLabelVMList = await (from des in _context.MasterDesignations
                                                             where des.StrDesignationCode.ToLower() == designation.StrDesignationCode.ToLower()
                                                             && des.StrDesignation.ToLower() == designation.StrDesignation.ToLower()
                                                             && des.IntAccountId == designation.IntAccountId
                                                             join bus in _context.MasterBusinessUnits on des.IntBusinessUnitId equals bus.IntBusinessUnitId
                                                             select new BusinessUnitValuLabelVM
                                                             {
                                                                 Value = bus.IntBusinessUnitId,
                                                                 Label = bus.StrBusinessUnit
                                                             }).ToListAsync();
                }

                obj.RoleValuLabelVMList = await (from bri in _context.RoleBridgeWithDesignations
                                                 where bri.IntAccountId == designation.IntAccountId && bri.IntDesignationOrEmployeeId == designation.IntDesignationId
                                                 && bri.IsActive == true
                                                 join role in _context.UserRoles on bri.IntRoleId equals role.IntRoleId
                                                 select new RoleValuLabelVM
                                                 {
                                                     Value = role.IntRoleId,
                                                     Label = role.StrRoleName
                                                 }).ToListAsync();

                return obj;
            }
            return new DesignationVM();
        }

        public async Task<bool> DeleteDesignation(long id)
        {
            try
            {
                MasterDesignation obj = await _context.MasterDesignations.FirstAsync(x => x.IntDesignationId == id);
                obj.IsActive = false;
                _context.MasterDesignations.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ==================== Employee Designation ===================

        #region =========== Separation Type =================

        public async Task<bool> SaveSeparationType(EmpSeparationType obj)
        {
            try
            {
                if (obj.IntSeparationTypeId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.EmpSeparationTypes.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.EmpSeparationTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<EmpSeparationTypeVM>> GetAllSeparationType(long accountId)
        {
            IEnumerable<EmpSeparationTypeVM> dataList = await (from item in _context.EmpSeparationTypes
                                                               where item.IntAccountId == accountId
                                                               select new EmpSeparationTypeVM
                                                               {
                                                                   IntSeparationTypeId = item.IntSeparationTypeId,
                                                                   StrSeparationType = item.StrSeparationType,
                                                                   IsEmployeeView = item.IsEmployeeView,
                                                                   IsActive = item.IsActive,
                                                                   IntAccountId = item.IntAccountId,
                                                                   DteCreatedAt = item.DteCreatedAt,
                                                                   IntCreatedBy = item.IntCreatedBy
                                                               }).AsNoTracking().AsQueryable().ToListAsync();
            return dataList;
        }

        public async Task<EmpSeparationTypeVM> GetSeparationTypeById(long id)
        {
            EmpSeparationTypeVM dataList = await (from item in _context.EmpSeparationTypes
                                                  where item.IntSeparationTypeId == id
                                                  select new EmpSeparationTypeVM
                                                  {
                                                      IntSeparationTypeId = item.IntSeparationTypeId,
                                                      StrSeparationType = item.StrSeparationType,
                                                      IsEmployeeView = item.IsEmployeeView,
                                                      IsActive = item.IsActive,
                                                      IntAccountId = item.IntAccountId,
                                                      DteCreatedAt = item.DteCreatedAt,
                                                      IntCreatedBy = item.IntCreatedBy
                                                  }).FirstOrDefaultAsync();
            return dataList;
        }

        #endregion =========== Separation Type =================

        #region =========== User Role =================

        public async Task<bool> SaveUserRole(UserRole obj)
        {
            try
            {
                if (obj.IntRoleId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.UserRoles.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.UserRoles.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserRoleVM>> GetAllUserRole(long accountId)
        {
            IEnumerable<UserRoleVM> dataList = await (from item in _context.UserRoles
                                                      where item.IntAccountId == accountId
                                                      select new UserRoleVM
                                                      {
                                                          IntRoleId = item.IntRoleId,
                                                          StrRoleName = item.StrRoleName,
                                                          IsActive = item.IsActive,
                                                          IntAccountId = item.IntAccountId,
                                                          DteCreatedAt = item.DteCreatedAt,
                                                          IntCreatedBy = item.IntCreatedBy
                                                      }).AsNoTracking().AsQueryable().ToListAsync();
            return dataList;
        }

        public async Task<UserRoleVM> GetUserRoleById(long id)
        {
            UserRoleVM dataList = await (from item in _context.UserRoles
                                         where item.IntRoleId == id
                                         select new UserRoleVM
                                         {
                                             IntRoleId = item.IntRoleId,
                                             StrRoleName = item.StrRoleName,
                                             IsActive = item.IsActive,
                                             IntAccountId = item.IntAccountId,
                                             DteCreatedAt = item.DteCreatedAt,
                                             IntCreatedBy = item.IntCreatedBy
                                         }).FirstOrDefaultAsync();
            return dataList;
        }

        #endregion =========== User Role =================

        #region =================== Employee Employment Type =======================

        public async Task<bool> SaveEmploymentType(MasterEmploymentType obj)
        {
            try
            {
                if (obj.IntEmploymentTypeId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.MasterEmploymentTypes.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.MasterEmploymentTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<EmploymentTypeVM>> GetAllEmploymentType(long accountId)
        {
            IEnumerable<EmploymentTypeVM> dataList = await (from item in _context.MasterEmploymentTypes
                                                            join parent1 in _context.MasterEmploymentTypes on item.IntParentId equals parent1.IntEmploymentTypeId into parent2
                                                            from parent in parent2.DefaultIfEmpty()
                                                            where item.IntAccountId == accountId
                                                            select new EmploymentTypeVM
                                                            {
                                                                IntEmploymentTypeId = item.IntEmploymentTypeId,
                                                                IntParentId = parent != null ? parent.IntEmploymentTypeId : 0,
                                                                StrParentName = parent != null ? parent.StrEmploymentType : "",
                                                                StrEmploymentType = item.StrEmploymentType,
                                                                IsActive = item.IsActive,
                                                                IntAccountId = item.IntAccountId,
                                                                DteCreatedAt = item.DteCreatedAt,
                                                                IntCreatedBy = item.IntCreatedBy
                                                            }).AsNoTracking().AsQueryable().ToListAsync();
            return dataList;
        }

        public async Task<EmploymentTypeVM> GetEmploymentTypeById(long id)
        {
            return await (from item in _context.MasterEmploymentTypes
                          join parent1 in _context.MasterEmploymentTypes on item.IntParentId equals parent1.IntEmploymentTypeId into parent2
                          from parent in parent2.DefaultIfEmpty()
                          where item.IntEmploymentTypeId == id
                          select new EmploymentTypeVM
                          {
                              IntEmploymentTypeId = item.IntEmploymentTypeId,
                              IntParentId = parent != null ? parent.IntEmploymentTypeId : 0,
                              StrParentName = parent != null ? parent.StrEmploymentType : "",
                              StrEmploymentType = item.StrEmploymentType,
                              IsActive = item.IsActive,
                              IntAccountId = item.IntAccountId,
                              DteCreatedAt = item.DteCreatedAt,
                              IntCreatedBy = item.IntCreatedBy
                          }).FirstOrDefaultAsync();
        }

        public async Task<MessageHelperCreate> DeleteEmploymentType(long id)
        {
            MessageHelperCreate msg = new MessageHelperCreate();
            try
            {
                MasterEmploymentType obj = await _context.MasterEmploymentTypes.FirstAsync(x => x.IntEmploymentTypeId == id);
                if (_context.EmpEmployeeBasicInfos.Where(x => x.IntEmploymentTypeId == obj.IntEmploymentTypeId).Count() > 0)
                {
                    msg.StatusCode = 500;
                    msg.Message = "Already exists this type of Employment Type.";
                }
                else
                {
                    obj.IsActive = false;
                    _context.MasterEmploymentTypes.Update(obj);
                    await _context.SaveChangesAsync();

                    msg.StatusCode = 200;
                    msg.Message = "Deleted Successfully";
                }
                return msg;
            }
            catch (Exception e)
            {
                msg.StatusCode = 500;
                msg.Message = e.Message;
                return msg;
            }
        }

        #endregion =================== Employee Employment Type =======================

        #region ========= Loan Type ==============

        public async Task<bool> SaveLoanType(EmpLoanType obj)
        {
            try
            {
                if (obj.IntLoanTypeId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.EmpLoanTypes.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.EmpLoanTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<EmpLoanType>> GetAllLoanType()
        {
            return await _context.EmpLoanTypes.OrderByDescending(x => x.IntLoanTypeId).AsNoTracking().AsQueryable().ToListAsync();
        }

        public async Task<EmpLoanType> GetLoanTypeById(long id)
        {
            return await _context.EmpLoanTypes.FirstOrDefaultAsync(x => x.IntLoanTypeId == id);
        }

        public async Task<bool> DeleteLoanType(long id)
        {
            try
            {
                EmpLoanType obj = await _context.EmpLoanTypes.FirstAsync(x => x.IntLoanTypeId == id);
                obj.IsActive = false;
                _context.EmpLoanTypes.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ========= Loan Type ==============

        #region====== Emp Expense ============

        public async Task<bool> SaveExpenseType(EmpExpenseType obj)
        {
            try
            {
                if (obj.IntExpenseTypeId > 0)
                {
                    //_context.Entry(obj).State = EntityState.Modified;
                    //_context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    EmpExpenseType empExpenseType = _context.EmpExpenseTypes.Where(x => x.IntExpenseTypeId == obj.IntExpenseTypeId).FirstOrDefault();

                    empExpenseType.IntExpenseTypeId = obj.IntExpenseTypeId;
                    empExpenseType.StrExpenseType = obj.StrExpenseType;
                    empExpenseType.IntAccountId = obj.IntAccountId;
                    empExpenseType.IsActive = obj.IsActive;

                    _context.EmpExpenseTypes.Update(empExpenseType);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.EmpExpenseTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<EmpExpenseType>> GetAllEmpExpenseType(long accountId)
        {
            return await _context.EmpExpenseTypes.Where(x => x.IntAccountId == accountId).OrderBy(x => x.StrExpenseType).AsNoTracking().AsQueryable().ToListAsync();
        }

        public async Task<EmpExpenseType> GetEmpExpenseTypeById(long id)
        {
            return await _context.EmpExpenseTypes.FirstOrDefaultAsync(x => x.IntExpenseTypeId == id);
        }

        public async Task<bool> DeleteEmpExpense(long id)
        {
            try
            {
                EmpExpenseType obj = await _context.EmpExpenseTypes.FirstOrDefaultAsync(x => x.IntExpenseTypeId == id);
                obj.IsActive = false;
                _context.EmpExpenseTypes.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region ================= Employee Position ================

        public async Task<MasterPosition> SavePosition(MasterPosition obj)
        {
            if (obj.IntPositionId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterPositions.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterPositions.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<MasterPosition>> GetAllPosition(long accountId, long businessUnitId)
        {
            return await _context.MasterPositions.Where(x => x.IsActive == true && x.IntAccountId == accountId && x.IntBusinessUnitId == businessUnitId).OrderByDescending(x => x.IntPositionId).ToListAsync();
        }

        public async Task<MasterPosition> GetPositionById(long Id)
        {
            return await _context.MasterPositions.FirstAsync(x => x.IntPositionId == Id);
        }

        public async Task<bool> DeletePosition(long id)
        {
            try
            {
                MasterPosition obj = await _context.MasterPositions.FirstAsync(x => x.IntPositionId == id);
                obj.IsActive = false;

                _context.MasterPositions.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region ================= Employee Workplace ================

        public async Task<MasterWorkplace> SaveWorkplace(MasterWorkplace obj)
        {
            if (obj.IntWorkplaceId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterWorkplaces.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterWorkplaces.AddAsync(obj);
                await _context.SaveChangesAsync();

                List<long?> user = await _context.Users.Where(x => x.IntAccountId == obj.IntAccountId && x.IsOfficeAdmin == true
                && x.IsActive == true).Select(x => x.IntRefferenceId).ToListAsync();

                foreach (long empId in user)
                {
                    RoleExtensionHeader roleExtensionHeader = _context.RoleExtensionHeaders.Where(x => x.IsActive == true
                    && x.IntEmployeeId == empId).FirstOrDefault();

                    if (roleExtensionHeader == null)
                    {
                        RoleExtensionHeader roleExtn = new RoleExtensionHeader
                        {
                            IntEmployeeId = empId,
                            IntCreatedBy = (long)obj.IntCreatedBy,
                            DteCreatedDateTime = DateTime.Now,
                            IsActive = true
                        };

                        _context.RoleExtensionHeaders.Add(roleExtn);
                        await _context.AddRangeAsync();
                        roleExtensionHeader = roleExtn;
                    }

                    RoleExtensionRow extRow = await _context.RoleExtensionRows.Where(x => x.IntRoleExtensionHeaderId == roleExtensionHeader.IntRoleExtensionHeaderId
                    && x.IntOrganizationTypeId == 3 && x.IsActive == true).FirstOrDefaultAsync();

                    if (extRow == null || (extRow != null && extRow.IntOrganizationReffId != 0))
                    {
                        RoleExtensionRow extensionRow = new RoleExtensionRow
                        {
                            IntRoleExtensionHeaderId = roleExtensionHeader.IntRoleExtensionHeaderId,
                            IntEmployeeId = empId,
                            IntOrganizationTypeId = 3,
                            StrOrganizationTypeName = "Workplace",
                            IntOrganizationReffId = obj.IntWorkplaceId,
                            StrOrganizationReffName = obj.StrWorkplace,
                            IntCreatedBy = (long)obj.IntCreatedBy,
                            DteCreatedDateTime = DateTime.Now,
                            IsActive = true
                        };

                        await _context.RoleExtensionRows.AddAsync(extensionRow);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return obj;
        }

        public async Task<IEnumerable<WorkPlaceViewModel>> GetAllWorkplace(long accountId, long businessUnitId)
        {
            IEnumerable<WorkPlaceViewModel> workPlaceList = await (from w in _context.MasterWorkplaces
                                                                   join bu in _context.MasterBusinessUnits on w.IntBusinessUnitId equals bu.IntBusinessUnitId
                                                                   where w.IntAccountId == accountId
                                                                   && w.IntBusinessUnitId == businessUnitId
                                                                   select new WorkPlaceViewModel
                                                                   {
                                                                       IntWorkplaceId = w.IntWorkplaceId,
                                                                       StrWorkplace = w.StrWorkplace,
                                                                       StrWorkplaceCode = w.StrWorkplaceCode,
                                                                       IntDistrictId = w.IntDistrictId,
                                                                       StrDistrict = w.StrDistrict,
                                                                       IntWorkplaceGroupId = w.IntWorkplaceGroupId,
                                                                       StrWorkplaceGroup = w.StrWorkplaceGroup,
                                                                       IsActive = w.IsActive,
                                                                       IntBusinessUnitId = w.IntBusinessUnitId,
                                                                       StrBusinessUnit = bu.StrBusinessUnit,
                                                                       IntAccountId = w.IntAccountId,
                                                                       IntCreatedBy = w.IntCreatedBy,
                                                                       DteCreatedAt = w.DteCreatedAt,
                                                                       IntUpdatedBy = w.IntUpdatedBy,
                                                                       DteUpdatedAt = w.DteUpdatedAt
                                                                   }).OrderByDescending(x => x.IntWorkplaceId).AsNoTracking().AsQueryable().ToListAsync();
            return workPlaceList;
        }

        public async Task<WorkPlaceViewModel> GetWorkplaceById(long Id)
        {
            WorkPlaceViewModel obj = await (from w in _context.MasterWorkplaces
                                            join bu in _context.MasterBusinessUnits on w.IntBusinessUnitId equals bu.IntBusinessUnitId
                                            where w.IntWorkplaceId == Id
                                            select new WorkPlaceViewModel
                                            {
                                                IntWorkplaceId = w.IntWorkplaceId,
                                                StrWorkplace = w.StrWorkplace,
                                                StrWorkplaceCode = w.StrWorkplaceCode,
                                                IntDistrictId = w.IntDistrictId,
                                                StrDistrict = w.StrDistrict,
                                                IntWorkplaceGroupId = w.IntWorkplaceGroupId,
                                                StrWorkplaceGroup = w.StrWorkplaceGroup,
                                                IsActive = w.IsActive,
                                                IntBusinessUnitId = w.IntBusinessUnitId,
                                                StrBusinessUnit = bu.StrBusinessUnit,
                                                IntAccountId = w.IntAccountId,
                                                IntCreatedBy = w.IntCreatedBy,
                                                DteCreatedAt = w.DteCreatedAt,
                                                IntUpdatedBy = w.IntUpdatedBy,
                                                DteUpdatedAt = w.DteUpdatedAt
                                            }).FirstOrDefaultAsync();
            if (obj == null)
            {
                throw new Exception("Data Not Found");
            }
            return obj;
        }

        public async Task<bool> DeleteWorkplace(long id)
        {
            try
            {
                MasterWorkplace obj = await _context.MasterWorkplaces.FirstAsync(x => x.IntWorkplaceId == id);
                obj.IsActive = false;

                _context.MasterWorkplaces.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region ================= Employee WorkplaceGroup ================

        public async Task<MasterWorkplaceGroup> SaveWorkplaceGroup(MasterWorkplaceGroup obj)
        {
            if (obj.IntWorkplaceGroupId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;

                _context.MasterWorkplaceGroups.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterWorkplaceGroups.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<MasterWorkplaceGroup>> GetAllWorkplaceGroup(long accountId, long businessUnitId)
        {
            return await _context.MasterWorkplaceGroups.Where(x => x.IntAccountId == accountId && x.IsActive == true).OrderByDescending(x => x.IntWorkplaceGroupId).ToListAsync();
        }

        public async Task<MasterWorkplaceGroup> GetWorkplaceGroupById(long Id)
        {
            return await _context.MasterWorkplaceGroups.FirstAsync(x => x.IntWorkplaceGroupId == Id);
        }

        public async Task<bool> DeleteWorkplaceGroup(long id)
        {
            try
            {
                MasterWorkplaceGroup obj = await _context.MasterWorkplaceGroups.FirstAsync(x => x.IntWorkplaceGroupId == id);
                obj.IsActive = false;

                _context.MasterWorkplaceGroups.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region ========== BankWallet ==================

        public async Task<BankWallet> SaveBankWallet(BankWallet obj)
        {
            if (obj.IntBankWalletId > 0)
            {
                _context.BankWallets.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.BankWallets.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<BankWallet>> GetAllBankWallet()
        {
            return await _context.BankWallets.Where(x => x.IsActive == true).OrderByDescending(x => x.IntBankWalletId).ToListAsync();
        }

        public async Task<BankWallet> GetBankWalletById(long id)
        {
            return await _context.BankWallets.FirstAsync(x => x.IsActive == true && x.IntBankWalletId == id);
        }

        public async Task<MessageHelper> DeleteBankWalletById(long id)
        {
            try
            {
                BankWallet obj = await _context.BankWallets.FirstAsync(x => x.IntBankWalletId == id);
                obj.IsActive = false;

                _context.BankWallets.Update(obj);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully !!!"
                };
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Data Not Found");
            }
        }

        #endregion

        #region ========== GlobalDocumentType ==================

        public async Task<long> SaveGlobalDocumentType(GlobalDocumentType obj)
        {
            if (obj.IntDocumentTypeId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.GlobalDocumentTypes.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.GlobalDocumentTypes.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntDocumentTypeId;
        }

        public async Task<IEnumerable<GlobalDocumentType>> GetAllGlobalDocumentType(long IntAccountId)
        {
            IEnumerable<GlobalDocumentType> globalDocumentTypes = await _context.GlobalDocumentTypes.Where(x => x.IntAccountId == IntAccountId).OrderByDescending(x => x.IntAccountId).AsNoTracking().AsQueryable().ToListAsync();
            return globalDocumentTypes;
        }

        public async Task<GlobalDocumentType> GetGlobalDocumentTypeById(long id)
        {
            return await _context.GlobalDocumentTypes.FirstAsync(x => x.IsActive == true && x.IntDocumentTypeId == id);
        }

        public async Task<MessageHelper> DeleteGlobalDocumentTypeById(long id)
        {
            try
            {
                GlobalDocumentType obj = await _context.GlobalDocumentTypes.FirstAsync(x => x.IntDocumentTypeId == id);
                obj.IsActive = false;

                _context.GlobalDocumentTypes.Update(obj);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully !!!"
                };
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Data Not Found");
            }
        }

        #endregion

        #region ========== GlobalFileUrl ==================

        public async Task<long> SaveGlobalFileUrl(GlobalFileUrl obj)
        {
            if (obj.IntDocumentTypeId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.GlobalFileUrls.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.GlobalFileUrls.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntDocumentId;
        }

        public async Task<IEnumerable<GlobalFileUrl>> GetAllGlobalFileUrl()
        {
            return await _context.GlobalFileUrls.OrderByDescending(x => x.IntDocumentId).ToListAsync();
        }

        public async Task<GlobalFileUrl> GetGlobalFileUrlById(long id)
        {
            return await _context.GlobalFileUrls.FirstAsync(x => x.IntDocumentId == id);
        }

        #endregion

        #region ========== GlobalInstitute ==================

        public async Task<long> SaveGlobalInstitute(GlobalInstitute obj)
        {
            if (obj.IntInstituteId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;

                _context.GlobalInstitutes.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.GlobalInstitutes.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntInstituteId;
        }

        public async Task<IEnumerable<GlobalInstitute>> GetAllGlobalInstitute()
        {
            return await _context.GlobalInstitutes.Where(x => x.IsActive == true).OrderByDescending(x => x.IntInstituteId).ToListAsync();
        }

        public async Task<GlobalInstitute> GetGlobalInstituteById(long id)
        {
            return await _context.GlobalInstitutes.FirstAsync(x => x.IsActive == true && x.IntInstituteId == id);
        }

        public async Task<MessageHelper> DeleteGlobalInstituteById(long id)
        {
            try
            {
                GlobalInstitute obj = await _context.GlobalInstitutes.FirstAsync(x => x.IntInstituteId == id);
                obj.IsActive = false;

                _context.GlobalInstitutes.Update(obj);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully !!!"
                };
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Data Not Found");
            }
        }

        #endregion

        #region ========= Workline Config ========

        public async Task<MessageHelper> CRUDWorklineConfig(EmpWorklineConfigViewModel obj)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (obj.IntWorklineId > 0)
                {
                    if (await _context.EmpWorklineConfigs.Where(x => x.IntEmploymentTypeId == obj.IntEmploymentTypeId && x.StrEmploymentType.Trim().ToLower() == obj.StrEmploymentType.Trim().ToLower() && x.IntAccountId == obj.IntAccountId && x.IntWorklineId != obj.IntWorklineId).CountAsync() > 0)
                    {
                        message.StatusCode = 500;
                        message.Message = "This Workline Config is already exists.";
                        return message;
                    }
                    else
                    {
                        EmpWorklineConfig empWorkline = await _context.EmpWorklineConfigs.Where(x => x.IntWorklineId == obj.IntWorklineId && x.IntAccountId == obj.IntAccountId).FirstOrDefaultAsync();

                        empWorkline.IntEmploymentTypeId = obj.IntEmploymentTypeId;
                        empWorkline.StrEmploymentType = obj.StrEmploymentType;
                        empWorkline.IntServiceLengthInDays = obj.IntServiceLengthInDays;
                        empWorkline.IntNotifyInDays = obj.IntNotifyInDays;
                        empWorkline.IntAccountId = obj.IntAccountId;
                        empWorkline.IntUpdateBy = obj.IntCreatedBy;
                        empWorkline.DteUpdateAt = DateTime.UtcNow;
                        empWorkline.IsActive = obj.IsActive;

                        _context.EmpWorklineConfigs.Update(empWorkline);
                        await _context.SaveChangesAsync();

                        message.StatusCode = 200;
                        message.Message = "UPDATED SUCCESSFULLY.";
                        return message;
                    }
                }
                else
                {
                    if (await _context.EmpWorklineConfigs.Where(x => x.IntEmploymentTypeId == obj.IntEmploymentTypeId && x.StrEmploymentType.Trim().ToLower() == obj.StrEmploymentType.Trim().ToLower() && x.IntAccountId == obj.IntAccountId).CountAsync() > 0)
                    {
                        message.StatusCode = 500;
                        message.Message = "This Workline Config is already exists.";
                        return message;
                    }
                    else
                    {
                        EmpWorklineConfig empWorkline = new EmpWorklineConfig
                        {
                            IntEmploymentTypeId = obj.IntEmploymentTypeId,
                            StrEmploymentType = obj.StrEmploymentType,
                            IntServiceLengthInDays = obj.IntServiceLengthInDays,
                            IntNotifyInDays = obj.IntNotifyInDays,
                            IntAccountId = obj.IntAccountId,
                            IsActive = obj.IsActive,
                            IntCreatedBy = obj.IntCreatedBy,
                            DteCreatedAt = DateTime.UtcNow
                        };

                        await _context.EmpWorklineConfigs.AddAsync(empWorkline);
                        await _context.SaveChangesAsync();

                        message.StatusCode = 200;
                        message.Message = "CREATED SUCCESSFULLY.";
                        return message;
                    }
                }
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return message;
            }
        }

        public async Task<IEnumerable<EmpWorklineConfigViewModel>> GetAllWorklineConfig(long AccoountId)
        {
            IEnumerable<EmpWorklineConfigViewModel> empWorklineConfigs = await (from wl in _context.EmpWorklineConfigs
                                                                                where wl.IntAccountId == AccoountId
                                                                                select new EmpWorklineConfigViewModel
                                                                                {
                                                                                    IntWorklineId = wl.IntWorklineId,
                                                                                    IntEmploymentTypeId = wl.IntEmploymentTypeId,
                                                                                    StrEmploymentType = wl.StrEmploymentType,
                                                                                    IntServiceLengthInDays = wl.IntServiceLengthInDays,
                                                                                    IntNotifyInDays = wl.IntNotifyInDays,
                                                                                    IntAccountId = wl.IntAccountId,
                                                                                    IntCreatedBy = wl.IntCreatedBy,
                                                                                    DteCreatedAt = wl.DteCreatedAt,
                                                                                    IsActive = wl.IsActive
                                                                                }).AsNoTracking().AsQueryable().ToListAsync();

            return empWorklineConfigs;
        }

        public async Task<EmpWorklineConfigViewModel> GetWorklineConfigById(long IntWorklineId)
        {
            EmpWorklineConfigViewModel empWorklineConfigs = await (from wl in _context.EmpWorklineConfigs
                                                                   where wl.IntWorklineId == IntWorklineId
                                                                   select new EmpWorklineConfigViewModel
                                                                   {
                                                                       IntWorklineId = wl.IntWorklineId,
                                                                       IntEmploymentTypeId = wl.IntEmploymentTypeId,
                                                                       StrEmploymentType = wl.StrEmploymentType,
                                                                       IntServiceLengthInDays = wl.IntServiceLengthInDays,
                                                                       IntNotifyInDays = wl.IntNotifyInDays,
                                                                       IntAccountId = wl.IntAccountId,
                                                                       IntCreatedBy = wl.IntCreatedBy,
                                                                       DteCreatedAt = wl.DteCreatedAt,
                                                                       IsActive = wl.IsActive
                                                                   }).FirstOrDefaultAsync();

            return empWorklineConfigs;
        }

        #endregion

        #region ========== LveLeaveType ==================

        public async Task<bool> SaveLveLeaveType(LveLeaveType obj)
        {
            try
            {
                if (obj.IntLeaveTypeId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.LveLeaveTypes.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.LveLeaveTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<LeaveTypeVM>> GetAllLveLeaveType(long accountId)
        {
            List<LeaveTypeVM> dataList = await (from item in _context.LveLeaveTypes
                                                join parent1 in _context.LveLeaveTypes on item.IntParentId equals parent1.IntLeaveTypeId into parent2
                                                from parent in parent2.DefaultIfEmpty()
                                                where item.IntAccountId == accountId
                                                select new LeaveTypeVM
                                                {
                                                    IntLeaveTypeId = item.IntLeaveTypeId,
                                                    IntParentId = parent != null ? parent.IntLeaveTypeId : 0,
                                                    StrParentName = parent != null ? parent.StrLeaveType : "",
                                                    StrLeaveType = item.StrLeaveType,
                                                    StrLeaveTypeCode = item.StrLeaveTypeCode,
                                                    IsActive = item.IsActive,
                                                    IntAccountId = item.IntAccountId,
                                                    DteCreatedAt = item.DteCreatedAt,
                                                    IntCreatedBy = item.IntCreatedBy
                                                }).AsNoTracking().AsQueryable().ToListAsync();

            return dataList;
        }

        public async Task<LeaveTypeVM> GetLveLeaveTypeById(long id)
        {
            return await (from item in _context.LveLeaveTypes
                          join parent1 in _context.LveLeaveTypes on item.IntParentId equals parent1.IntLeaveTypeId into parent2
                          from parent in parent2.DefaultIfEmpty()
                          where item.IntLeaveTypeId == id
                          select new LeaveTypeVM
                          {
                              IntLeaveTypeId = item.IntLeaveTypeId,
                              IntParentId = parent != null ? parent.IntLeaveTypeId : 0,
                              StrParentName = parent != null ? parent.StrLeaveType : "",
                              StrLeaveType = item.StrLeaveType,
                              StrLeaveTypeCode = item.StrLeaveTypeCode,
                              IsActive = item.IsActive,
                              IntAccountId = item.IntAccountId,
                              DteCreatedAt = item.DteCreatedAt,
                              IntCreatedBy = item.IntCreatedBy
                          }).FirstOrDefaultAsync();
        }

        public async Task<MessageHelper> DeleteLveLeaveTypeById(long id)
        {
            try
            {
                LveLeaveType obj = await _context.LveLeaveTypes.FirstAsync(x => x.IntLeaveTypeId == id);
                obj.IsActive = false;

                _context.LveLeaveTypes.Update(obj);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully !!!"
                };
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Data Not Found");
            }
        }

        #endregion

        #region ========== LveMovementType ==================

        public async Task<long> SaveLveMovementType(LveMovementType obj)
        {
            if (obj.IntMovementTypeId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.LveMovementTypes.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.LveMovementTypes.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntMovementTypeId;
        }

        public async Task<IEnumerable<LveMovementType>> GetAllLveMovementType(long accountId)
        {
            return await _context.LveMovementTypes.Where(x => x.IsActive == true && x.IntAccountId == accountId).OrderByDescending(x => x.IntMovementTypeId).AsNoTracking().AsQueryable().ToListAsync();
        }

        public async Task<LveMovementType> GetLveMovementTypeById(long id)
        {
            return await _context.LveMovementTypes.FirstAsync(x => x.IsActive == true && x.IntMovementTypeId == id);
        }

        public async Task<MessageHelper> DeleteLveMovementTypeById(long id)
        {
            try
            {
                LveMovementType obj = await _context.LveMovementTypes.FirstAsync(x => x.IntMovementTypeId == id);
                obj.IsActive = false;

                _context.LveMovementTypes.Update(obj);
                await _context.SaveChangesAsync();

                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully !!!"
                };
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Data Not Found");
            }
        }

        #endregion

        #region ===> Employment type wise leave type <===

        public async Task<MessageHelper> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeWiseLeaveBalanceViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmploymentTypeWiseLeaveBalance";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@intAutoId", obj.AutoId);
                        sqlCmd.Parameters.AddWithValue("@intEmploymentTypeId", obj.EmploymentTypeId);
                        sqlCmd.Parameters.AddWithValue("@intAllocatedLeave", obj.AllocatedLeave);
                        sqlCmd.Parameters.AddWithValue("@intYearId", obj.YearId);
                        sqlCmd.Parameters.AddWithValue("@intLeaveTypeId", obj.LeaveTypeId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.isActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", obj.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@intGenderId", obj.IntGenderId);
                        sqlCmd.Parameters.AddWithValue("@strGender", obj.StrGender);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel(CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel obj, BaseVM tokenData)
        {
            try
            {
                MessageHelper msg = new();

                List<EmpEmploymentTypeWiseLeaveBalance> typeWiseLeaveBalances = new();
               
                if(obj.WorkPlaceGroupList.Count > 0 )
                {
                    var businessUnitId = obj.BusinessUnitList.FirstOrDefault();
                    
                    foreach(var workPlaceGroupId in obj.WorkPlaceGroupList)
                    {
                        EmpEmploymentTypeWiseLeaveBalance empTypeBalance = new()
                        {
                            IntId = obj.AutoId,
                            IntEmploymentTypeId = obj.EmploymentTypeId,
                            IntGenderId = (int)obj.IntGenderId,
                            StrGender = obj.StrGender,
                            IntAllocatedLeave = obj.AllocatedLeave,
                            IntYearId = obj.YearId,
                            IntLeaveTypeId = obj.LeaveTypeId,
                            IntAccountId = obj.AccountId,
                            IsActive = obj.isActive,
                            IntCreatedBy = obj.IntCreatedBy,
                            DteCreatedAt = DateTime.Now,
                            IntBusinessUnitId = businessUnitId,
                            IntWorkplaceGroupId = workPlaceGroupId,
                            IntWorkplaceId = null
                        };
                        typeWiseLeaveBalances.Add(empTypeBalance);

                    }
                }
                else if(obj.BusinessUnitList.Count>0 && obj.WorkPlaceGroupList.Count <= 0 )
                {
                    foreach(var bu in obj.BusinessUnitList)
                    {
                        var listWorkplaceGroup = await _context.MasterWorkplaceGroups
                                                               .Where(x => x.IntBusinessUnitId == bu
                                                                && x.IsActive == true
                                                                && (tokenData.workplaceGroupList.Contains(x.IntWorkplaceGroupId) || tokenData.workplaceGroupList.Contains(0)))
                                                               .Select(x => x.IntWorkplaceGroupId).ToListAsync();
                        foreach(var item in listWorkplaceGroup)
                        {
                            EmpEmploymentTypeWiseLeaveBalance empTypeBalance = new()
                            {
                                IntId = obj.AutoId,
                                IntEmploymentTypeId = obj.EmploymentTypeId,
                                IntGenderId = (int)obj.IntGenderId,
                                StrGender = obj.StrGender,
                                IntAllocatedLeave = obj.AllocatedLeave,
                                IntYearId = obj.YearId,
                                IntLeaveTypeId = obj.LeaveTypeId,
                                IntAccountId = obj.AccountId,
                                IsActive = obj.isActive,
                                IntCreatedBy = obj.IntCreatedBy,
                                DteCreatedAt = DateTime.Now,
                                IntBusinessUnitId = bu,
                                IntWorkplaceGroupId = item,
                                IntWorkplaceId = null
                            };
                            typeWiseLeaveBalances.Add(empTypeBalance);

                        }

                    }
                }

                List<string> existslists = new();
                foreach (var item in typeWiseLeaveBalances)
                {
                    if(item.IntId>0)
                    {
                        var exists = await _context.EmpEmploymentTypeWiseLeaveBalances.
                                                    Where(x => x.IntId == item.IntId).FirstOrDefaultAsync();
                        if(exists != null)
                        {
                            exists.IntGenderId = item.IntGenderId;
                            exists.StrGender = item.StrGender;
                            exists.IntEmploymentTypeId = item.IntEmploymentTypeId;
                            exists.IntYearId = item.IntYearId;
                            exists.IntLeaveTypeId = item.IntLeaveTypeId;
                            exists.IntAllocatedLeave = item.IntAllocatedLeave;

                            var isAlreadyExists = await _context.EmpEmploymentTypeWiseLeaveBalances.
                                                   Where(x => x.IntEmploymentTypeId == exists.IntEmploymentTypeId
                                                   && x.IntYearId == exists.IntYearId
                                                   && x.IntLeaveTypeId == exists.IntLeaveTypeId
                                                   && x.IsActive == true
                                                   && x.IntGenderId == exists.IntGenderId
                                                   && x.IntAccountId == exists.IntAccountId
                                                   && x.IntBusinessUnitId == exists.IntBusinessUnitId
                                                   && x.IntWorkplaceGroupId == exists.IntWorkplaceGroupId
                                                   && x.IntAllocatedLeave == exists.IntAllocatedLeave
                                                   && x.IntGenderId == exists.IntGenderId).FirstOrDefaultAsync();

                            if (isAlreadyExists != null)
                            {
                                throw new  Exception ("Already Exists");
                            }
                            else
                            {
                                _context.EmpEmploymentTypeWiseLeaveBalances.Update(exists);
                            }
                        }

                    }
                    else
                    {
                        var exists = await _context.EmpEmploymentTypeWiseLeaveBalances.
                                                    Where(x => x.IntEmploymentTypeId == item.IntEmploymentTypeId
                                                    && x.IntYearId == item.IntYearId
                                                    && x.IntLeaveTypeId == item.IntLeaveTypeId 
                                                    && x.IsActive == true

                                                    && (item.IntGenderId==0   ?    x.IntGenderId == 0 || x.IntGenderId == 1 || x.IntGenderId == 2    :  x.IntGenderId == 0 || x.IntGenderId == item.IntGenderId)
                                                 

                                                   // && x.IntGenderId == item.IntGenderId
                                                    && x.IntAccountId == item.IntAccountId
                                                    && x.IntBusinessUnitId == item.IntBusinessUnitId
                                                    && x.IntWorkplaceGroupId == item.IntWorkplaceGroupId).FirstOrDefaultAsync();
                                                 
                        if (exists == null)
                        {
                            await _context.EmpEmploymentTypeWiseLeaveBalances.AddAsync(item);
                        }
                        else
                        {
                            var existsWp = _context.MasterWorkplaceGroups.Where(x => x.IntWorkplaceGroupId == exists.IntWorkplaceGroupId).Select(x => x.StrWorkplaceGroup).FirstOrDefault();
                            if (existsWp != null)
                            {
                                existslists.Add(existsWp);
                            }

                        }

                    }

                }
                if (existslists.Count > 0)
                {
                    throw new Exception($"Aready exists for {string.Join(",", existslists)}");
                }

                int res = await _context.SaveChangesAsync();
                msg.StatusCode = res > 0 ? 200 : 500;
                msg.Message = res > 0 ? "Update/Save Successful" : "Failed to save";

                return msg;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ========== Poliocy ==========

        public async Task<MessageHelper> CRUDPolicyCategory(CRUDPolicyCategoryViewModel obj)
        {
            try
            {
                if (obj.PolicyCategoryId > 0)
                {
                    var updateData = await Task.FromResult(_context.PolicyCategories.
                        Where(c => c.IntPolicyCategoryId == obj.PolicyCategoryId).FirstOrDefault());

                    if (obj.IsActive == false)
                    {
                        updateData.IsActive = false;

                        _context.PolicyCategories.Update(updateData);
                        await _context.SaveChangesAsync();

                        return new MessageHelper()
                        {
                            Message = "Deleted Successfully",
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        var existsData = await _context.PolicyCategories
                                    .Where(x => x.IntAccountId == obj.AccountId && x.IsActive == true && x.IntPolicyCategoryId != obj.PolicyCategoryId
                                    && x.StrPolicyCategoryName.ToLower() == obj.PolicyCategoryName.ToLower()).FirstOrDefaultAsync();
                        if (existsData != null)
                        {
                            throw new Exception("Category Name Already Exists");
                        }

                        updateData.StrPolicyCategoryName = obj.PolicyCategoryName;
                        updateData.DteCreatedAt = DateTime.Now;
                        updateData.IntCreatedBy = obj.IntCreatedBy;

                        _context.PolicyCategories.Update(updateData);
                        await _context.SaveChangesAsync();

                        return new MessageHelper()
                        {
                            Message = "Updated Successfully",
                            StatusCode = 200
                        };
                    }
                }
                else
                {
                    var existsData = await _context.PolicyCategories
                                    .Where(x => x.IntAccountId == obj.AccountId && x.IsActive == true
                                    && x.StrPolicyCategoryName.ToLower() == obj.PolicyCategoryName.ToLower()).FirstOrDefaultAsync();
                    if (existsData != null)
                    {
                        throw new Exception("Category Name Already Exists");
                    }

                    var createData = new PolicyCategory()
                    {
                        StrPolicyCategoryName = obj.PolicyCategoryName,
                        DteCreatedAt = DateTime.Now,
                        IntAccountId = obj.AccountId,
                        IsActive = true,
                        IntCreatedBy = obj.IntCreatedBy
                    };
                    await _context.PolicyCategories.AddAsync(createData);
                    await _context.SaveChangesAsync();

                    return new MessageHelper()
                    {
                        Message = "Created Successfully",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<GetCommonDDLViewModel>> GetPolicyCategoryDDL(long AccountId)
        {
            return await _context.PolicyCategories.Where(x => x.IntAccountId == AccountId && x.IsActive == true)
                                                     .Select(a => new GetCommonDDLViewModel()
                                                     {
                                                         Value = a.IntPolicyCategoryId,
                                                         Label = a.StrPolicyCategoryName
                                                     }).ToListAsync();
        }

        public async Task<List<GetCommonDDLViewModel>> GetPolicyAreaTypes()
        {
            return Enum.GetValues(typeof(PolicyAreaTypeEnum))
                    .Cast<PolicyAreaTypeEnum>()
                    .Select(item => new GetCommonDDLViewModel()
                    {
                        Value = (long)item,
                        Label = item.GetType()
                                .GetMember(item.ToString()).First()
                                .GetCustomAttribute<DisplayAttribute>()
                                .Name
                    }).ToList();
        }

        public async Task<MessageHelper> CreatePolicy(PolicyCommonViewModel obj)
        {
            try
            {
                PolicyHeader head = new PolicyHeader()
                {
                    DteCreatedAt = DateTime.Now,
                    IntAccountId = obj.objHeader.AccountId,
                    IntBusinessUnitId = obj.objHeader.BusinessUnitId,
                    IntPolicyCategoryId = obj.objHeader.PolicyCategoryId,
                    IntPolicyId = obj.objHeader.PolicyId,
                    IsActive = true,
                    IntCreatedBy = obj.objHeader.IntCreatedBy,
                    StrPolicyCategoryName = obj.objHeader.PolicyCategoryName,
                    StrPolicyFileName = obj.objHeader.PolicyFileName,
                    IntPolicyFileUrlId = obj.objHeader.PolicyFileUrlId,
                    StrPolicyTitle = obj.objHeader.PolicyTitle
                };
                await _context.PolicyHeaders.AddAsync(head);
                await _context.SaveChangesAsync();

                var rowList = new List<PolicyRow>();

                foreach (var item in obj.objRow)
                {
                    var row = new PolicyRow
                    {
                        DteCreatedAt = DateTime.Now,
                        IntAreaAutoId = item.AreaAutoId,
                        IntPolicyId = head.IntPolicyId,
                        IntRowId = item.RowId,
                        IsActive = true,
                        StrAreaName = item.AreaName,
                        StrAreaType = item.AreaType,
                        IntCreatedBy = item.IntCreatedBy
                    };
                    rowList.Add(row);
                }
                await _context.PolicyRows.AddRangeAsync(rowList);
                await _context.SaveChangesAsync();

                await _notificationService.PolicyUploadNotify(head);

                return new MessageHelper()
                {
                    Message = "Created Successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<GetPolicyLandingViewModel>> GetPolicyLanding(long AccountId, long BusinessUnitId, long CategoryId, string? Search)
        {
            try
            {
                IEnumerable<GetPolicyLandingViewModel> res = await (from ph in _context.PolicyHeaders
                                                                    where ph.IntAccountId == AccountId && ph.IntBusinessUnitId == BusinessUnitId
                                                                    && (ph.IntPolicyCategoryId == CategoryId || CategoryId == 0)
                                                                    && (string.IsNullOrEmpty(Search) ? true : ph.StrPolicyTitle.ToLower().Contains(Search.ToLower()))
                                                                    && ph.IsActive == true
                                                                    select new GetPolicyLandingViewModel()
                                                                    {
                                                                        PolicyId = ph.IntPolicyId,
                                                                        BusinessUnitId = ph.IntBusinessUnitId,
                                                                        PolicyTitle = ph.StrPolicyTitle,
                                                                        PolicyCategoryId = ph.IntPolicyCategoryId,
                                                                        PolicyCategoryName = ph.StrPolicyCategoryName,
                                                                        PolicyFileUrlId = ph.IntPolicyFileUrlId,
                                                                        PolicyFileName = ph.StrPolicyFileName,
                                                                        BusinessUnitList = CommonHelper.ConcateStringWithComma(_context.PolicyRows.Where(a => a.IntPolicyId == ph.IntPolicyId && a.StrAreaType == PolicyAreaTypeEnum.BusinessUnit.ToString()).Select(x => x.StrAreaName).ToList()),
                                                                        DepartmentList = CommonHelper.ConcateStringWithComma(_context.PolicyRows.Where(a => a.IntPolicyId == ph.IntPolicyId && a.StrAreaType == PolicyAreaTypeEnum.Department.ToString()).Select(x => x.StrAreaName).ToList()),
                                                                        AcknowledgeCount = _context.PolicyAcknowledges.Where(x => x.IntPolicyId == ph.IntPolicyId).Count(),
                                                                        PolicyCreateDate = ph.DteCreatedAt
                                                                    }).AsNoTracking().AsQueryable().ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> DeletePolicy(long PolicyId)
        {
            try
            {
                var res = await _context.PolicyHeaders.Where(x => x.IntPolicyId == PolicyId).FirstOrDefaultAsync();
                if (res == null)
                    throw new Exception("Policy not found");

                res.IsActive = false;
                _context.PolicyHeaders.Update(res);
                await _context.SaveChangesAsync();

                return new MessageHelper()
                {
                    StatusCode = 200,
                    Message = "Deleted Successfully"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<GetPolicyLandingViewModel>> GetPolicyOnEmployeeInbox(long EmployeeId)
        {
            try
            {
                var policyIdList = await GetPolicyListByEmployee(EmployeeId);

                var res = await (from ph in _context.PolicyHeaders
                                 where policyIdList.Contains(ph.IntPolicyId)
                                 select new GetPolicyLandingViewModel()
                                 {
                                     PolicyId = ph.IntPolicyId,
                                     BusinessUnitId = ph.IntBusinessUnitId,
                                     PolicyTitle = ph.StrPolicyTitle,
                                     PolicyCategoryId = ph.IntPolicyCategoryId,
                                     PolicyCategoryName = ph.StrPolicyCategoryName,
                                     PolicyFileUrlId = ph.IntPolicyFileUrlId,
                                     PolicyFileName = ph.StrPolicyFileName,
                                     BusinessUnitList = "",
                                     DepartmentList = "",
                                     AcknowledgeCount = _context.PolicyAcknowledges.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeId == EmployeeId && x.IntPolicyId == ph.IntPolicyId).FirstOrDefault() == null ? 0 : 1,
                                     PolicyCreateDate = ph.DteCreatedAt
                                 }).AsNoTracking().AsQueryable().ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> PolicyAcknowledge(long PolicyId, long EmployeeId)
        {
            try
            {
                var createObj = new PolicyAcknowledge()
                {
                    IntEmployeeId = EmployeeId,
                    IntPolicyId = PolicyId,
                    DteAcknowledgeDate = DateTime.Now
                };

                await _context.PolicyAcknowledges.AddAsync(createObj);
                await _context.SaveChangesAsync();

                return new MessageHelper()
                {
                    StatusCode = 200,
                    Message = "Submitted Successfully"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<long>> GetPolicyListByEmployee(long EmployeeId)
        {
            var empInfo = await _context.EmpEmployeeBasicInfos.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == EmployeeId && x.IsActive == true).FirstOrDefaultAsync();
            if (empInfo == null)
                throw new Exception("Employee not found");

            var policyRowList = await (from head in _context.PolicyHeaders
                                       join row in _context.PolicyRows on head.IntPolicyId equals (long)row.IntPolicyId
                                       where head.IsActive == true && row.IsActive == true
                                       select row).AsNoTracking().AsQueryable().ToListAsync();
            var policyGroup = policyRowList.GroupBy(p => p.IntPolicyId);

            var policyIdList = new List<long>();
            foreach (var item in policyGroup)
            {
                bool isUnit = false;
                bool isDept = false;
                foreach (var row in item)
                {
                    if (row.StrAreaType == PolicyAreaTypeEnum.BusinessUnit.ToString() && (row.IntAreaAutoId == empInfo.IntBusinessUnitId || row.IntAreaAutoId == 0))
                    {
                        isUnit = true;
                    }
                    else if (row.StrAreaType == PolicyAreaTypeEnum.Department.ToString() && (row.IntAreaAutoId == empInfo.IntDepartmentId || row.IntAreaAutoId == 0))
                    {
                        isDept = true;
                    }

                    if (isUnit == true && isDept == true)
                    {
                        policyIdList.Add((long)row.IntPolicyId);
                        break;
                    }
                }
            }

            return policyIdList;
        }

        public async Task<List<long>> GetEmployeeListByPolicy(long PolicyId)
        {
            var policyInfo = await _context.PolicyRows.Where(x => x.IntPolicyId == PolicyId && x.IsActive == true).ToListAsync();
            if (policyInfo.Count() == 0)
                return null;

            var businessUnitList = policyInfo.Where(x => x.StrAreaType == PolicyAreaTypeEnum.BusinessUnit.ToString()).Select(x => x.IntAreaAutoId).ToList();
            var departmentList = policyInfo.Where(x => x.StrAreaType == PolicyAreaTypeEnum.Department.ToString()).Select(x => x.IntAreaAutoId).ToList();

            bool isAllBusinessUnit = businessUnitList.Where(x => x.Value == 0).Count() > 0 ? true : false;
            bool isAllDepartment = departmentList.Where(x => x.Value == 0).Count() > 0 ? true : false;

            var empIdList = await (from emp in _context.EmpEmployeeBasicInfos
                                   where emp.IsActive == true
                                       && (businessUnitList.Contains(emp.IntBusinessUnitId) || isAllBusinessUnit == true)
                                       && (departmentList.Contains(emp.IntDepartmentId) || isAllDepartment == true)
                                   select emp.IntEmployeeBasicInfoId).Distinct().ToListAsync();
            return empIdList;
        }

        #endregion

        #region ========== Payroll   Management ==========

        public async Task<MessageHelper> CRUDPayrollManagement(CRUDPayrolManagementViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPyrPayrollCRUD";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        // ================== payroll element ============================
                        sqlCmd.Parameters.AddWithValue("@strPartType", obj.PartType);
                        sqlCmd.Parameters.AddWithValue("@intAutoId", obj.AutoId);
                        sqlCmd.Parameters.AddWithValue("@strPayrollElementName", obj.PayrollElementName);
                        sqlCmd.Parameters.AddWithValue("@strPayrollElementCode", obj.PayrollElementCode);
                        sqlCmd.Parameters.AddWithValue("@intPayrollElementTypeId", obj.PayrollElementTypeId);
                        sqlCmd.Parameters.AddWithValue("@strPayrollElementTypeCode", obj.PayrollElementTypeCode);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.isActive);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.IntAccountId);
                        // ================== payroll grade ============================
                        sqlCmd.Parameters.AddWithValue("@strPayrollGradeName", obj.PayrollGradeName);
                        sqlCmd.Parameters.AddWithValue("@numLowerLimit", obj.LowerLimit);
                        sqlCmd.Parameters.AddWithValue("@numUpperLimit", obj.UpperLimit);

                        // ================= payroll group ============================
                        sqlCmd.Parameters.AddWithValue("@strPayrollGroupName", obj.PayrollGradeName);
                        sqlCmd.Parameters.AddWithValue("@strPayrollGroupCode", obj.PayrollGroupCode);
                        sqlCmd.Parameters.AddWithValue("@strStartDateOfMonth", obj.StartDateOfMonth);
                        sqlCmd.Parameters.AddWithValue("@strEndDateOfMonth", obj.EndDateOfMonth);
                        sqlCmd.Parameters.AddWithValue("@strPayFrequencyName", obj.PayFrequencyName);
                        sqlCmd.Parameters.AddWithValue("@intPayFrequencyId", obj.PayFrequencyId);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ========== Bank Branch ==========

        public async Task<BankBranchLandingViewModel> BankBranchLanding(long? bankId, long? bankBranchId, long? accountId, string? search)
        {
            IQueryable<BankBranchViewModel> data = (from a in _context.GlobalBankBranches
                                                    where (a.IntBankId == bankId || bankId == null || bankId == 0)
                                                    && (a.IntBankBranchId == bankBranchId || bankBranchId == null || bankBranchId == 0)
                                                    && (a.IntAccountId == null || a.IntAccountId == 0 || a.IntAccountId == accountId)
                                                    join d in _context.Districts on a.IntDistrictId equals d.IntDistrictId into d1
                                                    from dis in d1.DefaultIfEmpty()
                                                    select new BankBranchViewModel
                                                    {
                                                        IntBankBranchId = a.IntBankBranchId,
                                                        StrBankBranchCode = a.StrBankBranchCode,
                                                        StrBankBranchName = a.StrBankBranchName,
                                                        StrBankBranchAddress = a.StrBankBranchAddress,
                                                        IntAccountId = a.IntAccountId,
                                                        IntDistrictId = a.IntDistrictId,
                                                        StrDistrict = dis == null ? "" : dis.StrDistrict,
                                                        IntCountryId = a.IntCountryId,
                                                        IntBankId = a.IntBankId,
                                                        StrBankName = a.StrBankName,
                                                        StrBankShortName = a.StrBankShortName,
                                                        StrBankCode = a.StrBankCode,
                                                        StrRoutingNo = a.StrRoutingNo,
                                                        IsActive = a.IsActive,
                                                        DteCreatedAt = a.DteCreatedAt,
                                                        DteUpdatedAt = a.DteUpdatedAt,
                                                        IntCreatedBy = a.IntCreatedBy,
                                                        IntUpdatedBy = a.IntUpdatedBy,
                                                    }).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                data = data.Where(x => x.StrBankName.ToLower().Contains(search) || x.StrBankCode.ToLower().Contains(search) || x.StrBankBranchName.ToLower().Contains(search) ||
                  x.StrBankBranchCode.ToLower().Contains(search) || x.StrDistrict.ToLower().Contains(search) || x.StrBankBranchAddress.ToLower().Contains(search) ||
                  x.StrRoutingNo.ToLower().Contains(search));
            }

            BankBranchLandingViewModel res = new();

            res.data = await data.ToListAsync();

            return res;
        }

        public async Task<BankBranchViewModel> BankBranchLandingById(long IntBankBranchId)
        {
            try
            {
                BankBranchViewModel bankBranch = await _context.GlobalBankBranches.Where(x => x.IntBankBranchId == IntBankBranchId)
                    .Select(x => new BankBranchViewModel
                    {
                        IntBankBranchId = x.IntBankBranchId,
                        StrBankBranchCode = x.StrBankBranchCode,
                        StrBankBranchName = x.StrBankBranchName,
                        StrBankBranchAddress = x.StrBankBranchAddress,
                        IntAccountId = x.IntAccountId,
                        IntDistrictId = x.IntDistrictId,
                        StrDistrict = _context.Districts.Where(s => s.IntDistrictId == x.IntDistrictId).Select(s => s.StrDistrict).FirstOrDefault(),
                        IntCountryId = x.IntCountryId,
                        IntBankId = x.IntBankId,
                        StrBankName = x.StrBankName,
                        StrBankShortName = x.StrBankShortName,
                        StrBankCode = x.StrBankCode,
                        StrRoutingNo = x.StrRoutingNo,
                        IsActive = x.IsActive,
                        DteCreatedAt = x.DteCreatedAt,
                        DteUpdatedAt = x.DteUpdatedAt,
                        IntCreatedBy = x.IntCreatedBy,
                        IntUpdatedBy = x.IntUpdatedBy,
                    }).FirstOrDefaultAsync();

                return bankBranch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ===== Master dashboard component =====

        public async Task<MessageHelperCreate> SaveDashboardComponent(MasterDashboardComponent obj)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            if (obj.IntId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterDashboardComponents.Update(obj);
                res.Message = "Update Successfully";
            }
            else
            {
                await _context.MasterDashboardComponents.AddAsync(obj);
            }
            await _context.SaveChangesAsync();

            res.AutoId = obj.IntId;

            return res;
        }

        public async Task<MasterDashboardComponent> GetDashboardComponentById(long id)
        {
            return await _context.MasterDashboardComponents.Where(x => x.IntId == id).FirstOrDefaultAsync();
        }

        public async Task<List<MasterDashboardComponent>> GetAllDashboardComponent()
        {
            return await _context.MasterDashboardComponents.Where(x => x.IsActive == true).OrderByDescending(x => x.IntId).ToListAsync();
        }

        public async Task<MessageHelperDelete> DeleteDashboardComponent(long id)
        {
            MessageHelperDelete smg = new MessageHelperDelete();
            MasterDashboardComponent res = await _context.MasterDashboardComponents.Where(x => x.IntId == id).FirstOrDefaultAsync();
            if (res != null)
            {
                _context.MasterDashboardComponents.Remove(res);
                await _context.SaveChangesAsync();
            }
            return smg;
        }

        #endregion

        #region ===== Master dashboard component permission =====

        public async Task<MessageHelperCreate> SaveDashboardComponentPermission(MasterDashboardComponentPermission obj)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            if (obj.IntId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterDashboardComponentPermissions.Update(obj);
                res.Message = "Update Successfully";
            }
            else
            {
                await _context.MasterDashboardComponentPermissions.AddAsync(obj);
            }
            await _context.SaveChangesAsync();

            res.AutoId = obj.IntId;

            return res;
        }

        public async Task<MasterDashboardComponentPermission> GetDashboardComponentPermissionById(long id)
        {
            MasterDashboardComponentPermission res = await _context.MasterDashboardComponentPermissions.Where(x => x.IntId == id).FirstOrDefaultAsync();
            return res;
        }

        public async Task<List<MasterDashboardComponentPermission>> GetAllDashboardComponentPermission()
        {
            return await _context.MasterDashboardComponentPermissions.Where(x => x.IsActive == true).OrderBy(x => x.IntId).ToListAsync();
        }

        public async Task<MessageHelperDelete> DeleteDashboardComponentPermission(long id)
        {
            MessageHelperDelete smg = new MessageHelperDelete();
            MasterDashboardComponentPermission res = await _context.MasterDashboardComponentPermissions.Where(x => x.IntId == id).FirstOrDefaultAsync();
            if (res != null)
            {
                _context.MasterDashboardComponentPermissions.Remove(res);
                await _context.SaveChangesAsync();
            }
            return smg;
        }

        #endregion

        #region ================= Payroll Element Type ================

        public async Task<PyrPayrollElementType> SavePayrollElementType(PyrPayrollElementType obj)
        {
            try
            {
                if (obj.IntPayrollElementTypeId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.StrPayrollElementName).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.PyrPayrollElementTypes.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.PyrPayrollElementTypes.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PyrPayrollElementTypeViewModel>> GetAllPayrollElementType(long accountId)
        {
            List<PyrPayrollElementTypeViewModel> listData = await (from obj in _context.PyrPayrollElementTypes
                                                                   where obj.IsActive == true && obj.IntAccountId == accountId
                                                                   select new PyrPayrollElementTypeViewModel
                                                                   {
                                                                       IntPayrollElementTypeId = obj.IntPayrollElementTypeId,
                                                                       IntAccountId = obj.IntAccountId,
                                                                       StrPayrollElementName = obj.StrPayrollElementName,
                                                                       StrCode = obj.StrCode,
                                                                       IsBasicSalary = obj.IsBasicSalary,
                                                                       IsPrimarySalary = obj.IsPrimarySalary,
                                                                       IsAddition = obj.IsAddition,
                                                                       IsDeduction = obj.IsDeduction,
                                                                       IsTaxable = obj.IsTaxable,
                                                                       IsActive = obj.IsActive,
                                                                       DteCreatedAt = obj.DteCreatedAt,
                                                                       IntCreatedBy = obj.IntCreatedBy,
                                                                       DteUpdatedAt = obj.DteUpdatedAt,
                                                                       IntUpdatedBy = obj.IntUpdatedBy
                                                                   }).OrderByDescending(x => x.DteCreatedAt).ToListAsync();
            return listData;
        }

        public async Task<PyrPayrollElementTypeViewModel> GetPayrollElementTypeById(long Id)
        {
            PyrPayrollElementTypeViewModel data = await (from obj in _context.PyrPayrollElementTypes
                                                         select new PyrPayrollElementTypeViewModel
                                                         {
                                                             IntPayrollElementTypeId = obj.IntPayrollElementTypeId,
                                                             IntAccountId = obj.IntAccountId,
                                                             StrPayrollElementName = obj.StrPayrollElementName,
                                                             StrCode = obj.StrCode,
                                                             IsBasicSalary = obj.IsBasicSalary,
                                                             IsPrimarySalary = obj.IsPrimarySalary,
                                                             IsAddition = obj.IsAddition,
                                                             IsDeduction = obj.IsDeduction,
                                                             IsTaxable = obj.IsTaxable,
                                                             IsActive = obj.IsActive,
                                                             DteCreatedAt = obj.DteCreatedAt,
                                                             IntCreatedBy = obj.IntCreatedBy,
                                                             DteUpdatedAt = obj.DteUpdatedAt,
                                                             IntUpdatedBy = obj.IntUpdatedBy
                                                         }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> DeletePayrollElementTypeById(long Id)
        {
            try
            {
                PyrPayrollElementType obj = await _context.PyrPayrollElementTypes.FirstAsync(x => x.IntPayrollElementTypeId == Id);
                obj.IsActive = false;

                _context.PyrPayrollElementTypes.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region ================= Salary Breakdow ================

        public async Task<PyrSalaryBreakdownHeader> SaveSalaryBreakdownHeader(PyrSalaryBreakdownHeader obj)
        {
            try
            {
                if (obj.IntSalaryBreakdownHeaderId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.PyrSalaryBreakdownHeaders.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.PyrSalaryBreakdownHeaders.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PyrSalaryBreakdownHeader>> GetAllSalaryBreakdownHeader(long accountId)
        {
            return await _context.PyrSalaryBreakdownHeaders.Where(x => x.IsActive == true && x.IntAccountId == accountId).OrderByDescending(x => x.IntSalaryBreakdownHeaderId).ToListAsync();
        }

        public async Task<PyrSalaryBreakdownHeader> GetSalaryBreakdownById(long Id)
        {
            return await _context.PyrSalaryBreakdownHeaders.FirstAsync(x => x.IntSalaryBreakdownHeaderId == Id);
        }

        #endregion

        #region ================= Salary Policy  ================

        public async Task<PyrSalaryPolicy> SaveSalaryPolicy(PyrSalaryPolicy obj)
        {
            try
            {
                if (obj.IntPolicyId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.PyrSalaryPolicies.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.PyrSalaryPolicies.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PyrSalaryPolicyViewModel>> GetAllSalaryPolicy(long accountId)
        {
            List<PyrSalaryPolicyViewModel> listData = await (from obj in _context.PyrSalaryPolicies
                                                             where obj.IsActive == true && obj.IntAccountId == accountId
                                                             select new PyrSalaryPolicyViewModel
                                                             {
                                                                 IntPolicyId = obj.IntPolicyId,
                                                                 IntAccountId = obj.IntAccountId,
                                                                 StrPolicyName = obj.StrPolicyName,
                                                                 IsSalaryCalculationShouldBeActual = obj.IsSalaryCalculationShouldBeActual,
                                                                 IntGrossSalaryDevidedByDays = obj.IntGrossSalaryDevidedByDays,
                                                                 IntGrossSalaryRoundDigits = obj.IntGrossSalaryRoundDigits,
                                                                 IsGrossSalaryRoundUp = obj.IsGrossSalaryRoundUp,
                                                                 IsGrossSalaryRoundDown = obj.IsGrossSalaryRoundDown,
                                                                 IntNetPayableSalaryRoundDigits = obj.IntNetPayableSalaryRoundDigits,
                                                                 IsNetPayableSalaryRoundUp = obj.IsNetPayableSalaryRoundUp,
                                                                 IsNetPayableSalaryRoundDown = obj.IsNetPayableSalaryRoundDown,
                                                                 IsSalaryShouldBeFullMonth = obj.IsSalaryShouldBeFullMonth,
                                                                 IntPreviousMonthStartDay = obj.IntPreviousMonthStartDay,
                                                                 IntNextMonthEndDay = obj.IntNextMonthEndDay,
                                                                 IsActive = obj.IsActive,
                                                                 DteCreatedAt = obj.DteCreatedAt,
                                                                 IntCreatedBy = obj.IntCreatedBy,
                                                                 DteUpdatedAt = obj.DteUpdatedAt,
                                                                 IntUpdatedBy = obj.IntUpdatedBy
                                                             }).OrderByDescending(x => x.DteCreatedAt).AsNoTracking().ToListAsync();
            return listData;
        }

        public async Task<PyrSalaryPolicyViewModel> GetSalaryPolicyById(long Id)
        {
            PyrSalaryPolicyViewModel data = await (from obj in _context.PyrSalaryPolicies
                                                   where obj.IsActive == true && obj.IntPolicyId == Id
                                                   select new PyrSalaryPolicyViewModel
                                                   {
                                                       IntPolicyId = obj.IntPolicyId,
                                                       IntAccountId = obj.IntAccountId,
                                                       StrPolicyName = obj.StrPolicyName,
                                                       IsSalaryCalculationShouldBeActual = obj.IsSalaryCalculationShouldBeActual,
                                                       IntGrossSalaryDevidedByDays = obj.IntGrossSalaryDevidedByDays,
                                                       IntGrossSalaryRoundDigits = obj.IntGrossSalaryRoundDigits,
                                                       IsGrossSalaryRoundUp = obj.IsGrossSalaryRoundUp,
                                                       IsGrossSalaryRoundDown = obj.IsGrossSalaryRoundDown,
                                                       IntNetPayableSalaryRoundDigits = obj.IntNetPayableSalaryRoundDigits,
                                                       IsNetPayableSalaryRoundUp = obj.IsNetPayableSalaryRoundUp,
                                                       IsNetPayableSalaryRoundDown = obj.IsNetPayableSalaryRoundDown,
                                                       IsSalaryShouldBeFullMonth = obj.IsSalaryShouldBeFullMonth,
                                                       IntPreviousMonthStartDay = obj.IntPreviousMonthStartDay,
                                                       IntNextMonthEndDay = obj.IntNextMonthEndDay,
                                                       IsActive = obj.IsActive,
                                                       DteCreatedAt = obj.DteCreatedAt,
                                                       IntCreatedBy = obj.IntCreatedBy,
                                                       DteUpdatedAt = obj.DteUpdatedAt,
                                                       IntUpdatedBy = obj.IntUpdatedBy
                                                   }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> DeleteSalaryPolicyById(long Id)
        {
            try
            {
                PyrSalaryPolicy obj = await _context.PyrSalaryPolicies.FirstAsync(x => x.IntPolicyId == Id);
                obj.IsActive = false;

                _context.PyrSalaryPolicies.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region ========== OverTime Configuration =============

        public async Task<GetOverTimeConfigurationVM> GetOverTimeConfiguration(int accountId)
        {
            try
            {


                GetOverTimeConfigurationVM data = await (from config in _context.OverTimeConfigurations
                                                         where config.IntAccountId == accountId && config.IsActive == true
                                                         select new GetOverTimeConfigurationVM
                                                         {
                                                            IntOtconfigId = config.IntOtconfigId,
                                                            IntAccountId = config.IntAccountId,
                                                            IsOvertimeAutoCalculate = _context.Accounts.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.IsOvertimeAutoCalculated).FirstOrDefault(),
                                                            IntOtdependOn = config.IntOtdependOn,
                                                            NumFixedAmount = config.NumFixedAmount,
                                                            IntOverTimeCountFrom = config.IntOverTimeCountFrom,
                                                            IntOtbenefitsHour = config.IntOtbenefitsHour,
                                                            IntMaxOverTimeDaily = (config.IntMaxOverTimeDaily / 60),
                                                            IntMaxOverTimeMonthly = (config.IntMaxOverTimeMonthly / 60),
                                                            IntOtAmountShouldBe = config.IntOtAmountShouldBe,
                                                            IntOtcalculationShouldBe = config.IntOtcalculationShouldBe,
                                                            IntCreatedBy = config.IntCreatedBy,
                                                            IntUpdatedBy = config.IntUpdatedBy,
                                                            IsActive = config.IsActive,
                                                            DteCreateAt = config.DteCreateAt,
                                                            DteUpdatedDate = config.DteUpdatedDate

                                                         }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GetOverTimeConfigurationVM> SaveOTConfiguration(GetOverTimeConfigurationVM obj)
        {
            try
            {
                obj.IntMaxOverTimeDaily = obj.IntMaxOverTimeDaily * 60;
                obj.IntMaxOverTimeMonthly = obj.IntMaxOverTimeMonthly * 60;
                if (obj.IntOtconfigId > 0)
                {
                    //_context.Entry(obj).State = EntityState.Modified;
                    //_context.Entry(obj).Property(x => x.DteCreateAt).IsModified = false;
                    //_context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    OverTimeConfiguration overTimeConfiguration = await _context.OverTimeConfigurations.FirstOrDefaultAsync(x => x.IntOtconfigId == obj.IntOtconfigId && x.IsActive == true);

                    overTimeConfiguration.IntAccountId = obj.IntAccountId;
                    overTimeConfiguration.IntOtdependOn = obj.IntOtdependOn;
                    overTimeConfiguration.NumFixedAmount = obj.NumFixedAmount;
                    overTimeConfiguration.IntOverTimeCountFrom = obj.IntOverTimeCountFrom;
                    overTimeConfiguration.IntOtbenefitsHour = (long)obj.IntOtbenefitsHour;
                    overTimeConfiguration.IntMaxOverTimeDaily = (long)obj.IntMaxOverTimeDaily;
                    overTimeConfiguration.IntMaxOverTimeMonthly = (long)obj.IntMaxOverTimeMonthly;
                    overTimeConfiguration.IntOtAmountShouldBe = (long)obj.IntOtAmountShouldBe;
                    overTimeConfiguration.IntOtcalculationShouldBe = (long)obj.IntOtcalculationShouldBe;
                    overTimeConfiguration.IntUpdatedBy = obj.IntUpdatedBy;
                    overTimeConfiguration.IsActive = obj.IsActive;
                    overTimeConfiguration.DteUpdatedDate = DateTime.Now;

                    _context.OverTimeConfigurations.Update(overTimeConfiguration);
                }
                else
                {
                    OverTimeConfiguration overTimeConfiguration = new OverTimeConfiguration
                    {
                        IntOtconfigId = obj.IntOtconfigId,
                        IntAccountId = obj.IntAccountId,
                        IntOtdependOn = obj.IntOtdependOn,
                        NumFixedAmount = obj.NumFixedAmount,
                        IntOverTimeCountFrom = obj.IntOverTimeCountFrom,
                        IntOtbenefitsHour = (long)obj.IntOtbenefitsHour,
                        IntMaxOverTimeDaily = (long)obj.IntMaxOverTimeDaily,
                        IntMaxOverTimeMonthly = (long)obj.IntMaxOverTimeMonthly,
                        IntOtAmountShouldBe = (long)obj.IntOtAmountShouldBe,
                        IntOtcalculationShouldBe = (long)obj.IntOtcalculationShouldBe,
                        IntCreatedBy = obj.IntCreatedBy,
                        IntUpdatedBy = obj.IntUpdatedBy,
                        IsActive = obj.IsActive,
                        DteCreateAt = DateTime.Now
                    };
                    await _context.OverTimeConfigurations.AddAsync(overTimeConfiguration);
                }
                await _context.SaveChangesAsync();

                Account account = await _context.Accounts.Where(x => x.IntAccountId == obj.IntAccountId && x.IsActive == true).FirstOrDefaultAsync();

                if (account != null)
                {
                    account.IsOvertimeAutoCalculated = obj.IsOvertimeAutoCalculate;
                    _context.Accounts.Update(account);
                    await _context.SaveChangesAsync();
                }

                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<OverTimeConfigurationVM> GetOverTimeConfigById(long intAccountId)
        {
            try
            {
                if (intAccountId > 0)
                {
                    OverTimeConfigurationVM otConfig = await (from config in _context.OverTimeConfigurations
                                                              where config.IntAccountId == intAccountId
                                                              select new OverTimeConfigurationVM
                                                              {
                                                                  IntOtconfigId = config.IntOtconfigId,
                                                                  IntAccountId = config.IntAccountId,
                                                                  IsOvertimeAutoCalculate = _context.Accounts.Where(x => x.IntAccountId == intAccountId && x.IsActive == true).Select(x => x.IsOvertimeAutoCalculated).FirstOrDefault(),
                                                                  IntOtdependOn = config.IntOtdependOn,
                                                                  NumFixedAmount = config.NumFixedAmount,
                                                                  IntOverTimeCountFrom = config.IntOverTimeCountFrom,
                                                                  IntOtbenefitsHour = config.IntOtbenefitsHour,
                                                                  intMaxOverTimeDaily = config.IntMaxOverTimeDaily,
                                                                  intMaxOverTimeMonthly = config.IntMaxOverTimeMonthly,
                                                                  IntOtcalculationShouldBe = config.IntOtcalculationShouldBe,
                                                                  IntOtAmountShouldBe = config.IntOtAmountShouldBe,
                                                                  IsActive = config.IsActive,
                                                                  DteCreateAt = config.DteCreateAt,
                                                                  IntCreatedBy = config.IntCreatedBy,
                                                                  DteUpdatedDate = config.DteUpdatedDate,
                                                                  IntUpdatedBy = config.IntUpdatedBy
                                                              }).FirstOrDefaultAsync();

                    return otConfig;

                }
                return null;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateTimeAttenSummery(List<TimeAttendanceDailySummeryVM> objDetails)
        {
            try
            {
                foreach (TimeAttendanceDailySummeryVM item in objDetails)
                {
                    TimeAttendanceDailySummary summary = await _context.TimeAttendanceDailySummaries.Where(x => x.IntAutoId == item.intAutoId).AsNoTracking().FirstOrDefaultAsync();

                    summary.NumModifiedOverTime = item.Minutes;
                    summary.IntUpdatedBy = item.ActionBy;
                    summary.DteUpdatedAt = DateTime.Now;
                    _context.TimeAttendanceDailySummaries.Update(summary);
                }

                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TimeAttendanceDailySummeryVM>> GetAllTimeAttenSummeryData()
        {
            try
            {
                List<TimeAttendanceDailySummeryVM> allSummeryData = await (from attendance in _context.TimeAttendanceDailySummaries
                                                                           select new TimeAttendanceDailySummeryVM
                                                                           {
                                                                               intAutoId = attendance.IntAutoId,
                                                                               Minutes = attendance.NumModifiedOverTime
                                                                           }).AsNoTracking().ToListAsync();
                return allSummeryData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OverTimeConfigurationDetail> SaveOTConfigDetails(OverTimeConfigurationDetail objDetails)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (objDetails.IntOtautoId > 0)
                {
                    _context.Entry(objDetails).State = EntityState.Modified;
                    _context.Entry(objDetails).Property(x => x.DteCreateAt).IsModified = false;
                    _context.Entry(objDetails).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.OverTimeConfigurationDetails.Update(objDetails);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.OverTimeConfigurationDetails.AddAsync(objDetails);
                    await _context.SaveChangesAsync();
                }
                return objDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region ========== Tax Challan Configuration =============

        public async Task<MasterTaxChallanConfig> SaveMasterTaxChallanConfig(MasterTaxChallanConfig obj)
        {
            try
            {
                if (obj.IntTaxChallanConfigId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                    _context.MasterTaxChallanConfigs.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    obj.DteCreatedAt = DateTime.Now;

                    await _context.MasterTaxChallanConfigs.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MasterTaxchallanConfigVM> GetTaxChallanConfigById(long id)
        {
            try
            {
                MasterTaxchallanConfigVM taxData = await (from taxChallan in _context.MasterTaxChallanConfigs
                                                          where taxChallan.IntTaxChallanConfigId == id
                                                          join fy in _context.FiscalYears on taxChallan.IntFiscalYearId equals fy.IntAutoId into fyscal
                                                          from detail in fyscal.DefaultIfEmpty()
                                                          select new MasterTaxchallanConfigVM
                                                          {
                                                              IntTaxChallanConfigId = taxChallan.IntTaxChallanConfigId,
                                                              IntYear = taxChallan.IntYear,
                                                              strFiscalYearDateRange = detail.StrFiscalYear,
                                                              DteFiscalFromDate = taxChallan.DteFiscalFromDate,
                                                              DteFiscalToDate = taxChallan.DteFiscalToDate,
                                                              IntFiscalYearId = taxChallan.IntFiscalYearId,
                                                              StrCircle = taxChallan.StrCircle,
                                                              StrZone = taxChallan.StrZone,
                                                              StrChallanNo = taxChallan.StrChallanNo,
                                                              DteChallanDate = taxChallan.DteChallanDate,
                                                              StrBankName = taxChallan.StrBankName,
                                                              IntBankId = taxChallan.IntBankId,
                                                              IntAccountId = taxChallan.IntAccountId,
                                                              IntActionBy = taxChallan.IntActionBy,
                                                              DteCreatedAt = taxChallan.DteCreatedAt,
                                                              IntCreatedBy = taxChallan.IntCreatedBy,
                                                              DteUpdatedAt = taxChallan.DteUpdatedAt,
                                                              IntUpdatedBy = taxChallan.IntUpdatedBy
                                                          }).FirstOrDefaultAsync();

                return taxData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MasterTaxchallanConfigVM>> GetAllMasterTaxchallanConfig(long intAccountId)
        {
            try
            {
                IEnumerable<MasterTaxchallanConfigVM> taxData = await (from taxChallan in _context.MasterTaxChallanConfigs
                                                                       where taxChallan.IntAccountId == intAccountId
                                                                       join fy in _context.FiscalYears on taxChallan.IntFiscalYearId equals fy.IntAutoId into fyscal
                                                                       from detail in fyscal.DefaultIfEmpty()
                                                                       select new MasterTaxchallanConfigVM
                                                                       {
                                                                           IntTaxChallanConfigId = taxChallan.IntTaxChallanConfigId,
                                                                           IntYear = taxChallan.IntYear,
                                                                           strFiscalYearDateRange = detail.StrFiscalYear,
                                                                           DteFiscalFromDate = taxChallan.DteFiscalFromDate,
                                                                           DteFiscalToDate = taxChallan.DteFiscalToDate,
                                                                           IntFiscalYearId = taxChallan.IntFiscalYearId,
                                                                           StrCircle = taxChallan.StrCircle,
                                                                           StrZone = taxChallan.StrZone,
                                                                           StrChallanNo = taxChallan.StrChallanNo,
                                                                           DteChallanDate = taxChallan.DteChallanDate,
                                                                           StrBankName = taxChallan.StrBankName,
                                                                           IntBankId = taxChallan.IntBankId,
                                                                           IntAccountId = taxChallan.IntAccountId,
                                                                           IntActionBy = taxChallan.IntActionBy,
                                                                           DteCreatedAt = taxChallan.DteCreatedAt,
                                                                           IntCreatedBy = taxChallan.IntCreatedBy,
                                                                           DteUpdatedAt = taxChallan.DteUpdatedAt,
                                                                           IntUpdatedBy = taxChallan.IntUpdatedBy
                                                                       }).AsNoTracking().AsQueryable().ToListAsync();

                return taxData;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion


        #region ================= Territory Type ================

        public async Task<MasterTerritoryType> SaveTerritoryType(MasterTerritoryType obj)
        {
            if (obj.IntTerritoryTypeId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                _context.MasterTerritoryTypes.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterTerritoryTypes.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<MasterTerritoryTypeVM>> GetAllTerritoryType(long accountId, long businessUnitId)
        {
            List<MasterTerritoryTypeVM> territoryType = await _context.OrganizationTypes.Where(x => x.IsActive == true && x.IntAccountId == accountId && x.IntOrganizationTypeId >= 4).OrderByDescending(x => x.IntOrganizationTypeId)
                .Select(x => new MasterTerritoryTypeVM
                {
                    IntTerritoryTypeId = x.IntOrganizationTypeId,
                    //IntHrPositionId = x.IntHrPositionId,
                    //HrPosition = _context.MasterPositions.Where(y => y.IntPositionId == x.IntHrPositionId && y.IsActive == true).Select(y => y.StrPosition).FirstOrDefault(),
                    //IntWorkplaceGroupId = x.IntWorkplaceGroupId,
                    //WorkplaceGroup = _context.MasterWorkplaceGroups.Where(v => v.IntWorkplaceGroupId == x.IntWorkplaceGroupId && v.IsActive == true).Select(y => y.StrWorkplaceGroup).FirstOrDefault(),
                    StrTerritoryType = x.StrOrganizationTypeName,
                    IntAccountId = x.IntAccountId,
                    //IntBusinessUnitId = x.IntBusinessUnitId,
                    //IsActive = x.IsActive,
                    //IntCreatedBy = x.IntCreatedBy,
                    //DteCreatedAt = x.DteCreatedAt
                }).AsNoTracking().AsQueryable().OrderByDescending(x => x.IntTerritoryTypeId).ToListAsync();

            return territoryType;
        }

        //public async Task<MasterTerritoryTypeVM> GetTerritoryTypeById(long id)
        //{
        //    MasterTerritoryTypeVM territoryType = await _context.MasterTerritoryTypes.Where(x => x.IntTerritoryTypeId == id)
        //        .Select(x => new MasterTerritoryTypeVM {
        //            IntTerritoryTypeId = x.IntTerritoryTypeId,
        //            IntHrPositionId = x.IntHrPositionId,
        //            HrPosition = _context.MasterPositions.Where(y => y.IntPositionId == x.IntHrPositionId && y.IsActive == true).Select(y => y.StrPosition).FirstOrDefault(),
        //            IntWorkplaceGroupId = x.IntWorkplaceGroupId,
        //            WorkplaceGroup = _context.MasterWorkplaceGroups.Where(v => v.IntWorkplaceGroupId == x.IntWorkplaceGroupId && v.IsActive == true).Select(y => y.StrWorkplaceGroup).FirstOrDefault(),
        //            StrTerritoryType = x.StrTerritoryType,
        //            IntAccountId = x.IntAccountId,
        //            IntBusinessUnitId = x.IntBusinessUnitId,
        //            IsActive = x.IsActive,
        //            IntCreatedBy = x.IntCreatedBy,
        //            DteCreatedAt = x.DteCreatedAt
        //        }).FirstOrDefaultAsync();

        //    return territoryType;
        //}

        #endregion
    }
}