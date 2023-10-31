using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.MasterData;
using System.Data;

namespace PeopleDesk.Services.Master.Interfaces
{
    public interface IMasterService
    {
        #region ========== User Type ==================
        Task<long> SaveUserType(UserType obj);
        Task<IEnumerable<UserType>> GetAllUserType();
        Task<UserType> GetUserTypeById(long id);
        Task<bool> DeleteUserTypeById(long id);
        #endregion

        #region ========== Account ==================
        Task<Account> SaveAccount(Account obj);
        Task<IEnumerable<Account>> GetAllAccount();
        Task<Account> GetAccountById(long id);
        Task<bool> DeleteAccountById(long id);
        #endregion

        #region ========== Account Package ==================
        Task<long> SaveAccountPackage(AccountPackage obj);
        Task<IEnumerable<AccountPackage>> GetAllAccountPackage();
        Task<AccountPackage> GetAccountPackageById(long id);
        Task<bool> DeleteAccountPackageById(long id);
        #endregion

        #region ============== District ==============
        Task<long> SaveDistrict(District obj);
        Task<IEnumerable<District>> GetAllDistrict();
        Task<District> GetDistrictById(long id);
        Task<bool> DeleteDistrictById(long id);
        #endregion

        #region ============== Division ==============
        Task<long> SaveDivision(Division obj);
        Task<IEnumerable<Division>> GetAllDivision();
        Task<Division> GetDivisionById(long id);
        Task<bool> DeleteDivisionById(long id);
        #endregion

        #region ============== Gender ==============
        Task<long> SaveGender(Gender obj);
        Task<IEnumerable<Gender>> GetAllGender();
        Task<Gender> GetGenderById(long id);
        Task<bool> DeleteGenderById(long id);
        #endregion

        #region ============== Religion ==============
        Task<long> SaveReligion(Religion obj);
        Task<IEnumerable<Religion>> GetAllReligion();
        Task<Religion> GetReligionById(long id);
        Task<bool> DeleteReligionById(long id);
        #endregion

        #region ============== ONLY GET ALL ==============
        Task<IEnumerable<BankWallet>> GetAllBankWallet();
        Task<IEnumerable<BloodGroup>> GetAllBloodGroup();
        Task<IEnumerable<Country>> GetAllCountry();
        Task<IEnumerable<Currency>> GetAllCurrency();
        Task<IEnumerable<EducationDegree>> GetAllEducationDegree();
        Task<IEnumerable<EducationFieldOfStudy>> GetAllEducationFieldOfStudy();
        Task<IEnumerable<Url>> GetAllURLs();
        #endregion

        #region ========== Announcement  ============
        Task<MessageHelper> CRUDTblAnnouncement(Announcement obj);
        Task<MessageHelper> CreateEditAnnouncement(AnnouncementCommonDTO model);
        Task<IEnumerable<Announcement>> GetAllAnnouncement(int BusinessUnitId, int year);
        Task<Announcement> GetAnnouncementById(int intId);

        public Task<AnnouncementCommonDTO> GetAnnouncementListById(long id);
        public Task<List<AnnouncementLanding>> GetAnnouncement(long employeeId, long accountId, long buId, int YearId);

        public Task<MessageHelper> DeleteAnnouncement(AnnouncementCommonDTO obj);
        #endregion

        #region ============= All Landing ===============
        Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId, DateTime? FromDate, DateTime? ToDate,
            long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount, long? MovementTypeId, DateTime? ApplicationDate, int? StatusId, long? LoggedEmployeeId);
        #endregion


    }
}
