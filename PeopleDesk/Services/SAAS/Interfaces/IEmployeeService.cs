using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Attendance;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.Employee;
using System.Data;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IEmployeeService
    {
        #region ================ Employee Department ===================

        public Task<long> SaveEmpDepartment(MasterDepartment obj);

        public Task<List<DepartmentViewModel>> GetAllEmpDepartment(long accountId, long businessUnitId);

        public Task<MasterDepartment> GetEmpDepartmentById(long Id);

        public Task<bool> DeleteEmpDepartment(long id);

        #endregion ================ Employee Department ===================

        #region ============ Employee Bulk Upload ===============

        public Task<MessageHelperBulkUpload> SaveEmployeeBulkUpload(List<EmployeeBulkUploadViewModel> obj);

        #endregion ============ Employee Bulk Upload ===============

        #region ================ Employee Basic Info ===================

        public Task<long> SaveEmpBasicInfo(EmployeeBasicViewModel obj);

        public Task<IEnumerable<EmpEmployeeBasicInfo>> GetAllEmpBasicInfo(long accountId);

        public Task<dynamic> GetEmpBasicInfoById(long id);

        public Task<bool> DeleteEmpBasicInfo(long id);

        //public Task<MessageHelper> CRUDTblEmployeeBasicInfo(EmployeeBasicViewModel obj);

        public Task<bool> LeaveNMovementMenuPermission(long IntEmployeeId, long IntCreatedBy);

        public Task<EmploymentTypeVM> GetEmploymentParentType(long IntAccountId, long? subEmploymentTypeId);
        public Task<MessageHelper> CreateNUpdateEmployeeBasicInfo(EmployeeBasicCreateNUpdateVM model, long AccountId, long CreateBy);
        public Task<MessageHelper> ConfirmationEmployee(ConfirmationEmployeeVM model, long AccountId, long EmployeeId);
        public Task<MessageHelper> CalendarAssignWhenEmployeeCreation(CalendarAssignViewModel model, long AccountId, long CreatedBy);
        public Task<MessageHelper> LeaveBalanceGenerate(long? EmployeeId, long CreateBy, long? OldEmploymentType);
        public Task<MessageHelper> UserCreation(CreateUserViewModel userModel, long AccountId, long CreateBy);

        #endregion ================ Employee Basic Info ===================

        #region ============== Employee Address ==============

        public Task<MessageHelper> SaveEmployeeAddress(EmployeeAddressViewModel obj);

        public Task<List<EmployeeAddressViewModel>> GetAllEmployeeAddress(long employeeId);

        public Task<EmployeeAddressViewModel> GetEmployeeAddressById(long id);

        public Task<MessageHelper> DeleteEmployeeAddress(long id);

        #endregion ============== Employee Address ==============

        #region ============ Employee Bank Details ===============

        public Task<long> SaveEmpBankDetails(EmployeeBankDetailsViewModel obj);

        public Task<MessageHelper> CreateBankBranch(GlobalBankBranchViewModel obj);

        public Task<List<FeatureCommontDDL>> BankBranchDDL(long BankId, long AccountID, long DistrictId);

        public Task<EmployeeBankDetailsViewModel> GetEmpBankDetailsById(long id);

        public Task<bool> DeleteEmpBankDetails(long id);

        #endregion ============ Employee Bank Details ===============

        #region ============== Employee Education ===================

        public Task<long> SaveEmpEducation(EmployeeEducationViewModel obj);

        public Task<List<EmployeeEducationViewModel>> GetEmpEducationById(long id);

        public Task<bool> DeleteEmpEducation(long id);

        #endregion ============== Employee Education ===================

        #region ==================== Employee Relatives ===================

        public Task<long> SaveEmpRelativesContact(EmployeeRelativesContactViewModel obj);

        public Task<List<EmployeeRelativesContactViewModel>> GetEmpRelativesContactById(long id);

        public Task<bool> DeleteEmpRelativesContact(long id);

        #endregion ==================== Employee Relatives ===================

        #region ==================== Employee Job History ===================

        public Task<long> SaveEmpJobHistory(EmployeeJobHistoryViewModel obj);

        public Task<List<EmployeeJobHistoryViewModel>> GetEmpJobHistorytById(long id);

        public Task<bool> DeleteEmpJobHistory(long id);

        #endregion ==================== Employee Job History ===================

        #region ================== Employee Training ==================

        public Task<long> SaveEmpTraining(EmployeeTrainingViewModel obj);

        public Task<List<EmployeeTrainingViewModel>> GetEmpTrainingById(long id);

        public Task<bool> DeleteEmpTraining(long id);

        #endregion ================== Employee Training ==================

        #region ================== Employee File ===============

        public Task<long> SaveEmpFile(EmployeeFileViewModel obj);

        public Task<List<EmployeeFileViewModel>> GetEmpFileById(long id);

        public Task<bool> DeleteEmpFile(long id);

        #endregion ================== Employee File ===============

        #region ============= Employee Photo Identity =================

        public Task<long> SaveEmpPhotoIdentity(EmployeePhotoIdentityViewModel obj);

        public Task<EmployeePhotoIdentityViewModel> GetEmpPhotoIdentityById(long id);

        public Task<bool> DeleteEmpPhotoIdentity(long id);

        #endregion ============= Employee Photo Identity =================

        #region ====================== Employee Information ==================

        Task<MessageHelper> UpdateEmployeeProfile(EmployeeProfileUpdateViewModel obj);

        Task<MessageHelper> CRUDEmployeeSeparation(BaseVM tokenData, EmployeeSeparationViewModel obj);

        #endregion ====================== Employee Information ==================

        #region ============== Employee Document Management ===============

        public Task<MessageHelper> SaveEmployeeDocumentManagement(EmployeeDocumentManagementViewModel obj);

        public Task<List<EmpEmployeeDocumentManagement>> GetAllEmployeeDocumentManagement(long IntAccountId, long IntEmployeeId);

        public Task<EmpEmployeeDocumentManagement> GetEmployeeDocumentById(long id);

        public Task<bool> DeleteEmpDocumentManagement(long id);

        #endregion ============== Employee Document Management ===============

        #region ============= Elezible for job config =============

        //Task<MessageHelper> SaveElezibleJobConfig(List<ElezableForJobConfigViewModel> obj);
        //Task<List<ElezableForJobConfigViewModel>> GetAllJobConfig(long IntAccountId);
        //Task<ElezableForJobConfigViewModel> GetJobConfigById(long id);

        #endregion ============= Elezible for job config =============

        #region =========== Time Sheet ==============

        Task<DataTable> TimeSheetAllLanding(string? PartType, long? BuninessUnitId, long? intId, long? intYear, long? intMonth);

        Task<DataTable> AttendanceAdjustmentFilter(AttendanceAdjustmentFilterViewModel obj);

        Task<MessageHelper> CreateExtraSideDuty(ExtraSideDutyViewModel obj);

        Task<IEnumerable<dynamic>> OverTimeFilter(OverTimeFilterViewModel obj, dynamic tokenData);

        Task<List<EmpOverTimeUploadDTO>> CreateOvertimeUpload(List<EmpOverTimeUploadDTO> objList);
        Task<MessageHelperBulkUpload> CreateOvertime(List<TimeEmpOverTimeVM> obj);

        Task<CustomMessageHelper> SubmitOvertimeUpload(List<EmpOverTimeUploadDTO> objList);

        #endregion =========== Time Sheet ==============

        #region =========== Employee Landing ==============

        Task<EmployeeProfileLandingPaginationViewModel> EmployeeProfileLandingPagination(long accountId, long businessUnitId, long WorkplaceGroupId, string? searchTxt, long? EmployeeId, int PageNo, int PageSize, bool IsForXl);
        Task<dynamic> EmployeeProfileLandingPaginationWithMasterFilter(BaseVM tokenData, long businessUnitId, long WorkplaceGroupId, string? searchTxt, int PageNo, int PageSize, bool IsPaginated, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors, List<long> employeementType, List<long> LineManagers, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList,
            List<long> TerritoryList);
        Task<EmployeeProfileLandingView> EmployeeProfileViewData(long employeeId);

        Task<EmployeeProfileLandingPaginationViewModel> EmployeeListForUserLandingPagination(long accountId, long businessUnitId, long workplaceGroupId, string? searchTxt, int? isUser, int PageNo, int PageSize, bool IsForXl);

        Task<EmployeeProfileView> EmployeeProfileView(long employeeId);

        Task<List<EmployeeListDDLViewModel>> EmployeeListBySupervisorORLineManagerNOfficeadmin(long EmployeeId, long? WorkplaceGroupId);

        #endregion =========== Employee Landing ==============

        #region ====================== Employee Bank Info ==================

        Task<MessageHelper> CRUDEmployeeBankDetails(EmployeeBankDetailsViewModel obj);

        #endregion ====================== Employee Bank Info ==================

        #region =========== Bulk Tax Assign ===========

        public Task<MessageHelperBulkUpload> SaveTaxBulkUpload(List<TaxBulkUploadViewModel> model);

        #endregion =========== Bulk Tax Assign ===========

        #region =========== Tax Assign ===========

        Task<MessageHelper> EmployeeTaxAssign(List<EmployeeTaxAssignViewModel> model);

        Task<ActiveEmployeeTaxAssignLanding> GetAllActiveEmployeeForTaxAssign(long IntAccountId, long? IntBusinessUnitId,
            long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntEmployeeId, string? searchTxt, int PageNo, int PageSize, int? intAssignStatus);

        #endregion =========== Tax Assign ===========

        #region ========== Time Sheet CRUD ==============

        Task<CustomMessageHelper> TimeSheetCRUD(TimeSheetViewModel obj);

        Task<HolidayAssignLandingPaginationViewModelWithHeader> HolidayNExceptionFilter(BaseVM tokenData, long businessUnitId, long WorkplaceGroupId, string? searchTxt, int PageNo, int PageSize, bool? IsNotAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList);

        Task<MessageHelper> HolidayAndExceptionOffdayAssign(HolidayAssignVm obj);

        Task<CalendarAssignLandingPaginationViewModelWithHeader> CalendarAssignFilter(BaseVM tokenData, long BusinessUnitId, long WorkplaceGroupId, string? SearchTxt, int PageNo, int PageSize, bool? IsNotAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> Departments, List<long> Designations, List<long> Supervisors, List<long> EmploymentTypeList, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList);

        Task<dynamic> TimeAttendanceSummaryForRoasterAsync(long IntEmployeeId, DateTime FromDate, DateTime ToDate);

        Task<MessageHelper> RosterGenerateList(RosterGenerateViewModel obj);

        Task<OffdayAssignLandingPaginationViewModelWithHeader> OffdayLandingFilter(BaseVM tokenData, long BusinessUnitId, long WorkplaceGroupId, string? SearchTxt, int PageNo, int PageSize, bool? IsAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> Departments, List<long> Designations, List<long> Supervisors, List<long> EmploymentTypeList, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList);

        Task<MessageHelper> OffdayAssign(OffdayAssignViewModel obj);

        #endregion ========== Time Sheet CRUD ==============

        #region ======= Report =======

        Task<List<AttendanceDailySummaryViewModel>> GetAttendanceSummaryCalenderViewReport(long EmployeeId, long Month, long Year);

        Task<DailyAttendanceReportVM> DailyAttendanceReport(long IntAccountId, DateTime AttendanceDate, long IntBusinessUnitId, long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntDepartmentId, string? EmployeeIdList, string? SearchTxt, int? PageNo, int? PageSize, bool IsPaginated);

        Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId, DateTime? FromDate, DateTime? ToDate,
            long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount, long? MovementTypeId, DateTime? ApplicationDate, long? LoggedEmployeeId,
            int? MonthId, long? YearId, long? WorkplaceGroupId, string? SearchTxt, int? PageNo, int? PageSize);

        //Task<EmployeeQryProfileAllViewModel> EmployeeQryProfileAllViewModel(long? intEmployeeId);
        Task<MessageHelper> LoanCRUD(LoanViewModel obj);

        Task<LoanLandingPagination> GetLoanApplicationByAdvanceFilter(LoanApplicationByAdvanceFilterViewModel obj);

        Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllList(long AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? EmployeeId);

        //Task<DataTable> AllEmployeeListWithFilter(AllEmployeeListWithFilterViewModel objFilter);
        Task<dynamic> EmployeeReportWithFilter(BaseVM tokenData, long businessUnitId, long workplaceGroupId, bool IsPaginated, string? searchTxt, int pageSize, int pageNo, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors, List<long> employeementType, List<long> LineManagers, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList);

        Task<EmployeeSeaprationLanding> EmployeeSeparationListFilter(long AccountId, long BusinessUnitId, long WorkplaceGroupId, long? EmployeeId, DateTime? FromDate, DateTime? ToDate, bool IsForXl, int PageNo, int PageSize, string? searchTxt,BaseVM tokenData);

        //Task<DataTable> EmployeeSeparationListFilter(EmployeeSeparationListFilterViewModel obj);
        Task<EmployeeSeparationLandingVM> EmployeeSeparationById(long SeparationId);
        Task<MessageHelper> EmployeeRejoinFromSeparation(EmployeeRejoinFromSeparationVM model, long EmployeeId);
        Task<EmployeeSeparationReportLanding> EmployeeSeparationReportFilter(BaseVM tokenData, long businessUnitId, long workplaceGroupId, DateTime? FromDate, DateTime? ToDate, int pageNo, int pageSize, string? searchTxt, bool IsPaginated, 
            bool IsHeaderNeed, List<long> departments, List<long> designations, List<long> supervisors, List<long> LineManagers, List<long> employeementType, List<long> WingList, List<long> SoledepoList, List<long> RegionList,
            List<long> AreaList, List<long> TerritoryList);


        Task<EmployeeDetailsViewModel> GetEmployeeDetailsByEmployeeId(long? EmployeeId);

        Task<List<LeaveApplicationReportViewModel>> GetAllApprovedLeaveApplicationListByEmployee(long EmployeeId, string? fromDate, string? toDate);

        Task<EmployeeSeparationReportViewModel> SeperationReportByEmployeeId(long? EmployeeId);

        Task<SalaryTaxCertificateViewModel> SalaryTaxCertificate(long FiscalYearId, string FiscalYear, long EmployeeId);
        Task<EmployeeDaylyAttendanceReportLanding> GetDateWiseAttendanceReport(long IntAccountId, long IntBusinessUnitId, long? IntWorkplaceGroupId, DateTime attendanceDate, bool IsXls, int PageNo, int PageSize, string? searchTxt);

        #endregion ======= Report =======

        #region ======= BonusManagement =======

        Task<MessageHelper> CRUDBonusSetup(CRUDBonusSetupViewModel obj);

        Task<DataTable> EligbleEmployeeForBonusGenerateLanding(string StrPartName, long IntAccountId, long IntBusinessUnitId, long WorkplaceGroupId, long? WingId, long? SoleDepoId, long? RegionId, long? AreaId, long? TerritoryId, long? IntBonusHeaderId, long? IntBonusId, DateTime? DteEffectedDate, long? IntCreatedBy, long? IntPageNo, long? IntPageSize, string? searchText);

        Task<DataTable> BonusAllLanding(BonusAllLandingViewModel obj);

        Task<MessageHelper> CRUDBonusGenerate(CRUDBonusGenerateViewModel obj);

        #endregion ======= BonusManagement =======

        #region ======= SalaryManagement =======

        Task<DataTable> EmployeeSalaryManagement(EmployeeSalaryManagementDTO obj);

        Task<MessageHelper> SalaryGenerateRequest(SalaryGenerateRequestDTO obj);

        Task<DataTable> GetSalaryGenerateRequestReport(long BusinessUnitId);
        Task<SalaryCertificateLanding> SalaryCertificateApplication(long? intSalaryCertificateRequestId, long AccountId, long BusinessUnitId, long WorkplaceGroupId, long? IntEmployeeId, long? MonthId, long? YearId, string? searchTxt, int PageNo, int PageSize);


        #endregion ======= SalaryManagement =======

        #region == Remote Attendance ==

        Task<MessageHelper> RemoteAttendanceRegistration(RemoteAttendanceRegistrationViewModel obj);

        Task<MessageHelper> RemoteAttendance(RemoteAttendanceViewModel obj);

        #endregion == Remote Attendance ==

        #region ===============  Loan  ==============

        // Already Exist in the same Interface

        //Task<List<LoanReportByAdvanceFilterViewModel>> GetLoanApplicationByAdvanceFilter(LoanApplicationByAdvanceFilterViewModel obj);

        #endregion ===============  Loan  ==============

        #region ================ Promotion Transfer Increment ===================

        public Task<MessageHelperCreate> CRUDEmpTransferNpromotion(TransferNpromotionVM obj);

        public Task<long> SaveEmpTransferNpromotion(EmpTransferNpromotion obj);

        public Task<TransferNPromotionPaginationVM> GetAllEmpTransferNpromotion(BaseVM tokenData, long businessUnitId, long workplaceGroupId, string landingType, DateTime dteFromDate, DateTime dteToDate, string? SearchTxt, int PageNo, int PageSize);

        public Task<List<TransferNpromotionVM>> GetEmpTransferNpromotionHistoryByEmployeeId(long accountId, long employeeId);

        public Task<TransferNpromotionVM> GetEmpTransferNpromotionById(long Id);

        public Task<bool> DeleteEmpTransferNpromotion(long id, long actionBy);

        public Task<bool> IsPromotionEligibleThroughIncrement(CRUDIncrementPromotionTransferVM objCrud);

        public Task<MessageHelperCreate> CreateEmployeeIncrement(CRUDIncrementPromotionTransferVM objCrud);

        public Task<GetIncrementPaginationVM> GetEmployeeIncrementLanding(long accountId, long businessUnitId, long? workplaceGroupId, DateTime? dteFromDate, DateTime? dteToDate, int PageNo, int PageSize, string? searchTxt);

        public Task<CRUDIncrementPromotionTransferVM> GetEmployeeIncrementById(long autoId);

        public Task UpdateSalaryBreakDownDueToIncrement(long employeeId, decimal incrementedAmount);

        public Task<List<CRUDEmployeeIncrementVM>> GetEmployeeIncrementByEmoloyeeId(long IntAccountId, long IntEmployeeId);

        #endregion ================ Promotion Transfer Increment ===================

        #region PF Gratuity

        public Task<MessageHelperCreate> CRUDEmpPfngratuity(CRUDEmpPfngratuityVM obj);

        public Task<CRUDEmpPfngratuityVM> GetEmpPfngratuity(long AccountId);

        public Task<PFInvestmentPagination> GetPFInvestmentLanding(long accountId, long businessUnitId, string? searchTxt, int? pageNo, int? pageSize, DateTime dteFromDate, DateTime dteToDate);

        public Task<List<PFInvestmentRowVM>> GetPFInvestmentById(long HeaderId);

        public Task<PFInvestmentHeaderVM> GetValidPFInvestmentPeriod(long AccountId, long BusinessUnitId);

        public Task<List<PFInvestmentRowVM>> GetEmployeeDataForPFInvestment(long accountId, long businessUnitId, DateTime fromMonthYear, DateTime toMonthYear);

        public Task<MessageHelperCreate> CreatePFInvestment(CRUDPFInvestmentVM obj);

        public Task<PFNGratuityViewModel> PfNGratuityLanding(long IntAccountId, long IntEmployeeId);

        public Task<PfAccountViewModel> PfNGratuityCount(EmpEmployeeBasicInfo employeeBasicInfo);

        public Task<List<PFWithdrawViewModel>> PfWithdrawLanding(long IntAccountId, long IntEmployeeId);

        public Task<MessageHelper> CRUDPFWithdraw(PFWithdrawViewModel obj);

        #endregion PF Gratuity

        long? EmployeeLeaveTaken(long intEmployeeId, DateTime FromDate, DateTime ToDate, long intLeaveTypeId);

        #region Promotion

        Task<PromotionReportViewModel> PromotionReportForPdf(long IntTransferNPromotionId, long AccountId);

        Task<IncrementLetterViewModel> IncrementLetterForPdf(long intIncrementId, long AccountId);

        Task<TransferAndPromotionReportPDFViewModel> TransferAndPromotionReportForPdf(long IntTransferAndPromotionId, long AccountId);

        Task<TransferReportViewModel> TransferReport(long IntTransferId, long AccountId);

        Task<BankAdviceReportForIBBLViewModel> BankAdviceReportForIBBL(long MonthId, long SalaryGenerateRequestId, long YearId, long AccountId, long intBusinessUnitId, long WorkplaceGroupId);

        Task<BankAdviceReportForBEFTNViewModel> BankAdviceReportForBEFTN(long MonthId, long SalaryGenerateRequestId, long YearId, long AccountId, long? intBusinessUnitId, long WorkplaceGroupId);

        #endregion Promotion

        #region===== master location register============

        Task<MessageHelper> MasterLocationRegistrationAsync(MasterLocationRegisterDTO model);

        Task<List<GetMasterLocationRegisterDTO>> GetMasterLocationByAccountIdAsync(long AcccountId, long BusinessUnitId);

        #endregion

        #region Location assaign

        Task<List<MasterLoctionRegistrationDdl>> MasterLocationRegistrationDDL(long AccountId, long BussinessUnit);

        Task<dynamic> EmployeeWiseLocationAsync(long AccountId, long EmployeeId);

        Task<MessageHelper> CreateNUpdateEmployeeWiseLocationAssaignAsync(CreateNUpdateMasterLocationEployeeWise model);

        Task<dynamic> GetLocationWiseEmployeeAsync(long LocationMasterId, long AccountId, long BusineesUint);

        Task<dynamic> CreateNUpdateLocationWiseEmployeeAsync(CreateNUpdateMasterLocationWise model);

        Task<dynamic> GetLocationDashBoardByAccountId(long IntAccountId, long IntBusinessUnitId);

        Task<dynamic> GetEmployeeListLocationBased(long IntAccountId, long IntBusinessUnitId);

        #endregion

        #region ==========Shift Management ===========
        Task<dynamic> GetEmployeeShiftInfoAsync(long intEmployeeId, long intYear, long intMonth);
        Task<dynamic> GetCalenderDdlAsync(long intAccountId, long intBusinessUnitId);
        Task<dynamic> PostCalendarAssignAsync(CalendarAssignList model);
        Task<dynamic> LogAttendanceOfChangedCalendarAsync(long EmployeeId, DateTime FromDate, DateTime Todate);
        #endregion

        #region====Off day reassign====
        Task<dynamic> EmployeeOffDayReassignAsync(EmployeeOffDayReassignList model);
        Task<dynamic> GetEmployeeOffDayReassignLandingAsync(int IntMonthId, int IntYearId, long EmployeeId);

        #endregion

        #region=== Master Fixed Roaster===
        Task<dynamic> GetFixedRoasterMasterByIdAsync(long intAccountId, long intBusinessId);
        Task<dynamic> GetFixedRoasterDetaisByIdAsync(long intFixedMasterId);
        Task<dynamic> CreateNUpdateFixedRoasterMasterAsync(FixedMasterRoaster model);
        Task<dynamic> CreateNUpdateFixedRoasterDeatailsAsync(List<FixedRoasterVm> model);
        #endregion

        #region === Active current inactive Employee===
        Task<MessageHelper> EmployeeInactive(long EmployeeId);
        Task<dynamic> PostAcitveCurrentInactiveEmployeeAsync(long InEmployeeId);
        Task<InactiveEmployeeListLanding> GetInactiveEmployeeList(BaseVM tokenData, long businessUnitId, long workplaceGroupId, DateTime? FromDate, DateTime? ToDate,  int pageNo, int pageSize, string? searchTxt, bool IsPaginated, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors,  List<long> LineManagers, List<long> employeementType, List<long> WingList, List<long> SoledepoList, List<long> RegionList, 
            List<long> AreaList, List<long> TerritoryList);
        #endregion

        #region ==== Get Employee All Role And Extensions====
        Task<RoleExtensionsList> GetEmployeeRoleExtensions(long IntEmployeeId, long IntAccountId);
        #endregion

        #region ===== Attendance Process ====
        Task<MessageHelper> TimeAttendanceProcess(TimeAttendanceProcessViewModel obj);
        Task<List<TimeAttendanceProcessResponseVM>> GetTimeAttendanceProcess(DateTime? FromDate, DateTime? ToDate);
        #endregion

        Task<MessageHelperWithValidation> BulkSalaryAdditionNDeduction(BulkSalaryAdditionNDeductionViewModel model);
        Task<EmpSalaryAdditionNDeductionLanding> SalaryAdditionDeductionLanding(long AccountId, long IntMonth, long IntYear, long BusinessUnitId, long WorkplaceGroupId, int PageNo, int PageSize, string? searchTxt, bool IsHeaderNeed,
             List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList);
        Task<IEnumerable<CommonEmployeeDDL>> GetCommonEmployeeDDL(BaseVM tokenData, long businessUnitId, long workplaceGroupId, long? employeeId, string? searchText);
        Task<IEnumerable<CommonEmployeeDDL>> PermissionCheckFromEmployeeListByEnvetFireEmployee(BaseVM tokenData, long businessUnitId, long workplaceGroupId, List<long> employeeIdList, string? searchText);
        Task<EmpJobConfirmationLanding> EmpJobConfirmation(long AccountId, long BusinessUnitId, long WorkplaceGroupId, long Month, long Year, bool IsXls, int PageNo, int PageSize, string? searchTxt);
        Task<EmployeeMovementPaginationVM> EmployeeMovementReportAll(long BusinessUnitId, long WorkplaceGroupId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, string SearchText, bool IsPaginated, BaseVM tokenData);
    }
}