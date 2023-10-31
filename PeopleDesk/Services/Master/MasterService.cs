using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Master.Interfaces;
using System.Data;

namespace PeopleDesk.Services.Master
{
    public class MasterService : IMasterService
    {
        private readonly PeopleDeskContext _context;
        private DataTable dt = new DataTable();

        public MasterService(PeopleDeskContext context)
        {
            _context = context;
        }

        #region ========== User Type ==================

        public async Task<long> SaveUserType(UserType obj)
        {
            if (obj.IntUserTypeId > 0)
            {
                _context.UserTypes.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.UserTypes.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntUserTypeId;
        }

        public async Task<IEnumerable<UserType>> GetAllUserType()
        {
            return await _context.UserTypes.Where(x => x.IsActive == true).OrderBy(x => x.StrUserType).ToListAsync();
        }

        public async Task<UserType> GetUserTypeById(long id)
        {
            return await _context.UserTypes.FirstAsync(x => x.IsActive == true && x.IntUserTypeId == id);
        }

        public async Task<bool> DeleteUserTypeById(long id)
        {
            try
            {
                UserType obj = await _context.UserTypes.FirstAsync(x => x.IntUserTypeId == id);
                obj.IsActive = false;

                _context.UserTypes.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ========== User Type ==================

        #region ========== Account ==================

        public async Task<Account> SaveAccount(Account obj)
        {
            if (obj.IntAccountId > 0)
            {
                obj.DteCreatedAt = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.IntAccountId == obj.IntAccountId).DteCreatedAt;

                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                _context.Accounts.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Accounts.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj;
        }

        public async Task<IEnumerable<Account>> GetAllAccount()
        {
            return await _context.Accounts.Where(x => x.IsActive == true).OrderBy(x => x.IntAccountId).ToListAsync();
        }

        public async Task<Account> GetAccountById(long id)
        {
            return await _context.Accounts.FirstAsync(x => x.IsActive == true && x.IntAccountId == id);
        }

        public async Task<bool> DeleteAccountById(long id)
        {
            try
            {
                Account obj = await _context.Accounts.FirstAsync(x => x.IntAccountId == id);
                obj.IsActive = false;

                _context.Accounts.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ========== Account ==================

        #region ========== Account Package ==================

        public async Task<long> SaveAccountPackage(AccountPackage obj)
        {
            if (obj.IntAccountPackageId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();

                _context.AccountPackages.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.AccountPackages.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntAccountPackageId;
        }

        public async Task<IEnumerable<AccountPackage>> GetAllAccountPackage()
        {
            return await _context.AccountPackages.Where(x => x.IsActive == true).OrderBy(x => x.StrAccountPackageName).ToListAsync();
        }

        public async Task<AccountPackage> GetAccountPackageById(long id)
        {
            return await _context.AccountPackages.FirstAsync(x => x.IsActive == true && x.IntAccountPackageId == id);
        }

        public async Task<bool> DeleteAccountPackageById(long id)
        {
            try
            {
                AccountPackage obj = await _context.AccountPackages.FirstAsync(x => x.IntAccountPackageId == id);
                obj.IsActive = false;

                _context.AccountPackages.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ========== Account Package ==================

        #region ============== District ==============

        public async Task<long> SaveDistrict(District obj)
        {
            if (obj.IntDistrictId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                //_context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();

                _context.Districts.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Districts.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntDistrictId;
        }

        public async Task<IEnumerable<District>> GetAllDistrict()
        {
            return await _context.Districts.Where(x => x.IsActive == true).OrderBy(x => x.StrDistrict).ToListAsync();
        }

        public async Task<District> GetDistrictById(long id)
        {
            return await _context.Districts.FirstAsync(x => x.IsActive == true && x.IntDistrictId == id);
        }

        public async Task<bool> DeleteDistrictById(long id)
        {
            try
            {
                District obj = await _context.Districts.FirstAsync(x => x.IntDistrictId == id);
                obj.IsActive = false;
                _context.Districts.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== District ==============

        #region ============== Division ==============

        public async Task<long> SaveDivision(Division obj)
        {
            if (obj.IntDivisionId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                //_context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
                _context.Divisions.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Divisions.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntDivisionId;
        }

        public async Task<IEnumerable<Division>> GetAllDivision()
        {
            return await _context.Divisions.Where(x => x.IsActive == true).OrderBy(x => x.StrDivision).ToListAsync();
        }

        public async Task<Division> GetDivisionById(long id)
        {
            return await _context.Divisions.FirstAsync(x => x.IsActive == true && x.IntDivisionId == id);
        }

        public async Task<bool> DeleteDivisionById(long id)
        {
            try
            {
                Division obj = await _context.Divisions.FirstAsync(x => x.IntDivisionId == id);
                obj.IsActive = false;
                _context.Divisions.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== Division ==============

        #region ============== Gender ==============

        public async Task<long> SaveGender(Gender obj)
        {
            if (obj.IntGenderId > 0)
            {
                _context.Genders.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Genders.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntGenderId;
        }

        public async Task<IEnumerable<Gender>> GetAllGender()
        {
            return await _context.Genders.Where(x => x.IsActive == true).OrderBy(x => x.StrGender).ToListAsync();
        }

        public async Task<Gender> GetGenderById(long id)
        {
            return await _context.Genders.FirstAsync(x => x.IsActive == true && x.IntGenderId == id);
        }

        public async Task<bool> DeleteGenderById(long id)
        {
            try
            {
                Gender obj = await _context.Genders.FirstAsync(x => x.IntGenderId == id);
                obj.IsActive = false;
                _context.Genders.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== Gender ==============

        #region ============== Religion ==============

        public async Task<long> SaveReligion(Religion obj)
        {
            if (obj.IntReligionId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
                _context.Religions.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Religions.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntReligionId;
        }

        public async Task<IEnumerable<Religion>> GetAllReligion()
        {
            return await _context.Religions.Where(x => x.IsActive == true).OrderBy(x => x.StrReligion).AsNoTracking().AsQueryable().ToListAsync();
        }

        public async Task<Religion> GetReligionById(long id)
        {
            return await _context.Religions.FirstAsync(x => x.IsActive == true && x.IntReligionId == id);
        }

        public async Task<bool> DeleteReligionById(long id)
        {
            try
            {
                Religion obj = await _context.Religions.FirstAsync(x => x.IntReligionId == id);
                obj.IsActive = false;
                _context.Religions.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== Religion ==============

        #region ============== ONLY GET ALL ==============

        public async Task<IEnumerable<BankWallet>> GetAllBankWallet()
        {
            return await _context.BankWallets.Where(x => x.IsActive == true).OrderBy(x => x.StrBankWalletName).ToListAsync();
        }

        public async Task<IEnumerable<BloodGroup>> GetAllBloodGroup()
        {
            return await _context.BloodGroups.Where(x => x.IsActive == true).OrderBy(x => x.StrBloodGroup).ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetAllCountry()
        {
            return await _context.Countries.Where(x => x.IsActive == true).OrderBy(x => x.StrCountry).ToListAsync();
        }

        public async Task<IEnumerable<Currency>> GetAllCurrency()
        {
            return await _context.Currencies.Where(x => x.IsActive == true).OrderBy(x => x.StrCurrency).ToListAsync();
        }

        public async Task<IEnumerable<EducationDegree>> GetAllEducationDegree()
        {
            return await _context.EducationDegrees.Where(x => x.IsActive == true).OrderBy(x => x.StrEducationDegree).ToListAsync();
        }

        public async Task<IEnumerable<EducationFieldOfStudy>> GetAllEducationFieldOfStudy()
        {
            return await _context.EducationFieldOfStudies.Where(x => x.IsActive == true).OrderBy(x => x.StrEducationFieldOfStudy).ToListAsync();
        }

        public async Task<IEnumerable<Url>> GetAllURLs()
        {
            return await _context.Urls.Where(x => x.IsActive == true).OrderBy(x => x.IntUrlId).ToListAsync();
        }

        #endregion ============== ONLY GET ALL ==============

        #region ============= Announcement ==============

        public async Task<MessageHelper> CRUDTblAnnouncement(Announcement obj)
        {
            try
            {
                if (obj.IntAnnouncementId > 0)
                {
                    if (obj.IsActive == false)
                    {
                        var inactiveItem = await _context.Announcements.Where(x => x.IntAnnouncementId == obj.IntAnnouncementId).FirstOrDefaultAsync();
                        if (inactiveItem != null)
                        {
                            inactiveItem.IsActive = false;
                        }
                        _context.Announcements.Update(inactiveItem);
                        await _context.SaveChangesAsync();

                        return new MessageHelper()
                        {
                            Message = "Deleted Successfully",
                            StatusCode = 200
                        };
                    }

                    obj.IsActive = true;
                    _context.Announcements.Update(obj);
                    await _context.SaveChangesAsync();

                    return new MessageHelper()
                    {
                        Message = "Updated Successfully",
                        StatusCode = 200
                    };
                }
                else
                {
                    obj.IsActive = true;
                    await _context.Announcements.AddAsync(obj);
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

        public async Task<IEnumerable<Announcement>> GetAllAnnouncement(int BusinessUnitId, int year)
        {
            return await _context.Announcements.Where(x => x.IntBusinessUnitId == BusinessUnitId && x.DteCreatedAt.Year == year && x.IsActive == true)
                .OrderByDescending(x => x.DteCreatedAt).ToListAsync();
        }

        public async Task<Announcement> GetAnnouncementById(int intId)
        {
            return await _context.Announcements.Where(x => x.IntAnnouncementId == intId && x.IsActive == true)
                .OrderByDescending(x => x.DteCreatedAt).FirstOrDefaultAsync();
        }


        public async Task<MessageHelper> CreateEditAnnouncement(AnnouncementCommonDTO model)
        {
            try
            {
                MessageHelper msg = new MessageHelper();
                long HeaderId = 0;

                if (model.Announcement.IntAnnouncementId > 0) //edit
                {
                    Announcement announcement = _context.Announcements.Where(x => x.IntAnnouncementId == model.Announcement.IntAnnouncementId).FirstOrDefault();

                    HeaderId = model.Announcement.IntAnnouncementId;
                    announcement.StrTitle = model.Announcement.StrTitle;
                    announcement.StrDetails = model.Announcement.StrDetails;
                    announcement.IntTypeId = model.Announcement.IntTypeId;
                    announcement.StrTypeName = model.Announcement.StrTypeName;
                    announcement.DteExpiredDate = model.Announcement.DteExpiredDate;

                    msg.Message = "Updated Successfully";
                    msg.StatusCode = 200;
                }
                else //create
                {
                    Announcement createHeader = new Announcement
                    {
                        IntAccountId = model.Announcement.IntAccountId,
                        IntBusinessUnitId = model.Announcement.IntBusinessUnitId,
                        StrTitle = model.Announcement.StrTitle,
                        StrDetails = model.Announcement.StrDetails,
                        IntTypeId = model.Announcement.IntTypeId,
                        StrTypeName = model.Announcement.StrTypeName,
                        DteExpiredDate = model.Announcement.DteExpiredDate,
                        IntCreatedBy = model.Announcement.IntCreatedBy,
                        DteCreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    await _context.Announcements.AddAsync(createHeader);
                    await _context.SaveChangesAsync();

                    HeaderId = createHeader.IntAnnouncementId;

                    msg.Message = "Created Successfully";
                    msg.StatusCode = 200;
                }

                var newList = new List<AnnouncementRow>(model.AnnouncementRow.Count);
                var editList = new List<AnnouncementRow>(model.AnnouncementRow.Count);

                foreach (var item in model.AnnouncementRow)
                {
                    AnnouncementRow check = await _context.AnnouncementRows.Where(x =>
                                        x.IntAnnouncementRowId == item.IntAnnouncementRowId && x.IsActive == true).FirstOrDefaultAsync();

                    if (check == null)
                    {
                        var detailsRow = new AnnouncementRow();

                        detailsRow.IntAnnoucementId = HeaderId;
                        detailsRow.IntAnnouncementReferenceId = item.IntAnnouncementReferenceId;
                        detailsRow.StrAnnounceCode = item.StrAnnounceCode;
                        detailsRow.StrAnnouncementFor = item.StrAnnouncementFor;
                        detailsRow.IsActive = true;

                        newList.Add(detailsRow);
                    }
                    else
                    {
                        check.IntAnnouncementReferenceId = item.IntAnnouncementReferenceId;
                        check.StrAnnounceCode = item.StrAnnounceCode;
                        check.StrAnnouncementFor = item.StrAnnouncementFor;
                        check.IsActive = true;

                        editList.Add(check);
                    }
                }
                var innerquery = from a in model.AnnouncementRow
                                 where a.IntAnnouncementRowId > 0
                                 select a.IntAnnouncementRowId;

                var inactiveitems = (from b in _context.AnnouncementRows
                                     where b.IntAnnoucementId == HeaderId && !innerquery.Contains(b.IntAnnouncementRowId)
                                     select b).ToList();

                if (inactiveitems.Count > 0)
                {
                    inactiveitems.ForEach(itms => { itms.IsActive = false; });

                    _context.AnnouncementRows.UpdateRange(inactiveitems);
                    await _context.SaveChangesAsync();
                }

                if (newList.Count > 0)
                {
                    _context.AnnouncementRows.AddRange(newList);
                    await _context.SaveChangesAsync();
                }

                if (editList.Count > 0)
                {
                    _context.AnnouncementRows.UpdateRange(editList);
                    await _context.SaveChangesAsync();
                }

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AnnouncementLanding>> GetAnnouncement(long employeeId, long accountId, long buId, int YearId)
        {
            List<UserGroupHeader> userGroup = await (from ug in _context.UserGroupHeaders
                                                     join ugr in _context.UserGroupRows on ug.IntUserGroupHeaderId equals ugr.IntUserGroupHeaderId
                                                     where ug.IntAccountId == accountId && ugr.IntEmployeeId == employeeId && ug.IsActive == true && ugr.IsActive == true
                                                     select ug).AsNoTracking().AsQueryable().ToListAsync();

            List<AnnouncementLanding> data = await (from a in _context.Announcements
                                                    join b in _context.AnnouncementRows on a.IntAnnouncementId equals b.IntAnnoucementId
                                                    join emp in _context.EmpEmployeeBasicInfos on employeeId equals emp.IntEmployeeBasicInfoId
                                                    where a.IntAccountId == accountId && a.IntBusinessUnitId == buId && a.DteCreatedAt.Year == YearId
                                                    && ((emp.IntWorkplaceId == b.IntAnnouncementReferenceId && b.StrAnnounceCode.Contains("W"))
                                                          || (emp.IntWorkplaceGroupId == b.IntAnnouncementReferenceId && b.StrAnnounceCode.Contains("Wg"))
                                                          || (b.IntAnnouncementReferenceId > 0 ? emp.IntDepartmentId == b.IntAnnouncementReferenceId && b.StrAnnounceCode.Contains("Dept") : b.IntAnnouncementReferenceId == 0 && b.StrAnnounceCode.Contains("Dept"))
                                                          || (b.IntAnnouncementReferenceId > 0 ? emp.IntDesignationId == b.IntAnnouncementReferenceId && b.StrAnnounceCode.Contains("Desig") : b.IntAnnouncementReferenceId == 0 && b.StrAnnounceCode.Contains("Desig"))
                                                          || (b.IntAnnouncementReferenceId > 0 ? userGroup.Select(x => x.IntUserGroupHeaderId).Contains((long)b.IntAnnouncementReferenceId) && b.StrAnnounceCode.Contains("Ug") : b.IntAnnouncementReferenceId == 0 && b.StrAnnounceCode.Contains("Ug")))
                                                          && a.IsActive == true && b.IsActive == true && emp.IsActive == true
                                                    select new AnnouncementLanding
                                                    {
                                                        IntAnnouncementId = a.IntAnnouncementId,
                                                        IntAccountId = a.IntAccountId,
                                                        IntBusinessUnitId = a.IntBusinessUnitId,
                                                        StrTitle = a.StrTitle,
                                                        StrDetails = a.StrDetails,
                                                        IntTypeId = a.IntTypeId,
                                                        StrTypeName = a.StrTypeName,
                                                        DteExpiredDate = a.DteExpiredDate,
                                                        DteCreatedAt = a.DteCreatedAt,
                                                        IntCreatedBy = a.IntCreatedBy,
                                                        IsActive = a.IsActive
                                                    }).AsNoTracking().AsQueryable().Distinct().ToListAsync();
            return data;
        }

        public async Task<AnnouncementCommonDTO> GetAnnouncementListById(long id)
        {
            var head = await (from a in _context.Announcements
                              where a.IntAnnouncementId == id && a.IsActive == true
                              select new AnnouncementViewModel()
                              {
                                  IntAnnouncementId = a.IntAnnouncementId,
                                  IntAccountId = a.IntAccountId,
                                  IntBusinessUnitId = a.IntBusinessUnitId,
                                  StrTitle = a.StrTitle,
                                  StrDetails = a.StrDetails,
                                  IntTypeId = a.IntTypeId,
                                  StrTypeName = a.StrTypeName,
                                  DteExpiredDate = a.DteExpiredDate,
                                  DteCreatedAt = a.DteCreatedAt,
                                  IntCreatedBy = a.IntCreatedBy,
                                  IsActive = a.IsActive,
                              }).FirstOrDefaultAsync();

            var row = await (from b in _context.AnnouncementRows
                             where b.IntAnnoucementId == id && b.IsActive == true
                             select new AnnouncementRowViewModel()
                             {
                                 IntAnnouncementRowId = b.IntAnnouncementRowId,
                                 IntAnnoucementId = b.IntAnnoucementId,
                                 IntAnnouncementReferenceId = b.IntAnnouncementReferenceId,
                                 StrAnnounceCode = b.StrAnnounceCode,
                                 StrAnnouncementFor = b.StrAnnouncementFor,
                                 IsActive = b.IsActive,
                             }).ToListAsync();

            return new AnnouncementCommonDTO()
            {
                Announcement = head,
                AnnouncementRow = row
            };
        }

        public async Task<MessageHelper> DeleteAnnouncement(AnnouncementCommonDTO obj)
        {
            try
            {
                if (obj.Announcement.IntAnnouncementId > 0)
                {
                    if (obj.Announcement.IsActive == false)
                    {
                        var inactiveItem = await _context.Announcements.Where(x => x.IntAnnouncementId == obj.Announcement.IntAnnouncementId && x.IsActive == true).FirstOrDefaultAsync();
                        if (inactiveItem != null)
                        {
                            inactiveItem.IsActive = false;

                            List<AnnouncementRow> inactiveRow = await _context.AnnouncementRows.Where(x => x.IntAnnoucementId == inactiveItem.IntAnnouncementId && x.IsActive == true).ToListAsync();
                            foreach (var item in inactiveRow)
                            {
                                if (item != null)
                                {
                                    item.IsActive = false;
                                }
                                _context.AnnouncementRows.Update(item);
                                await _context.SaveChangesAsync();
                            }
                            _context.Announcements.Update(inactiveItem);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return new MessageHelper()
                {
                    Message = "Deleted Successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ============= Announcement ==============

        #region ============ All Landing =============

        public async Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId,
            DateTime? FromDate, DateTime? ToDate, long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount,
            long? MovementTypeId, DateTime? ApplicationDate, int? StatusId, long? LoggedEmployeeId)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprAllLandingPageData";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strTableName", TableName);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intId", intId);
                        sqlCmd.Parameters.AddWithValue("@intStatusId", intStatusId);
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", "");
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@LoanTypeId", LoanTypeId);
                        sqlCmd.Parameters.AddWithValue("@DeptId", DeptId);
                        sqlCmd.Parameters.AddWithValue("@DesigId", DesigId);
                        sqlCmd.Parameters.AddWithValue("@EmpId", EmpId);
                        sqlCmd.Parameters.AddWithValue("@MinimumAmount", MinimumAmount);
                        sqlCmd.Parameters.AddWithValue("@MaximumAmount", MaximumAmount);
                        sqlCmd.Parameters.AddWithValue("@MovementTypeId", MovementTypeId);
                        sqlCmd.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@StatusId", StatusId);
                        sqlCmd.Parameters.AddWithValue("@intLoggedEmployeeId", LoggedEmployeeId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        #endregion ============ All Landing =============
    }
}