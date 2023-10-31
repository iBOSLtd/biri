using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.MasterData;

namespace PeopleDesk.Services.Master.Interfaces
{
    public interface ISaasMasterService
    {
        #region ================ Master BusinessUnit =====================

        Task<MasterBusinessUnit> SaveBusinessUnit(MasterBusinessUnit obj);

        Task<IEnumerable<MasterBusinessUnit>> GetAllBusinessUnit(long accountId);

        Task<MasterBusinessUnit> GetBusinessUnitById(long Id);

        Task<MasterBusinessUnitDWithAccount> GetBusinessDetailsByBusinessUnitIdAsync(long Id);

        Task<bool> DeleteBusinessUnit(long id);

        #endregion ================ Master BusinessUnit =====================

        #region =============== Employee Designation ====================

        Task<MasterDesignation> SaveDesignation(MasterDesignation obj);

        Task<IEnumerable<DesignationViewModel>> GetAllDesignation(long accountId, long businessUnitId);

        Task<DesignationVM> GetDesignationById(long id);

        Task<bool> DeleteDesignation(long id);

        #endregion =============== Employee Designation ====================

        #region =============== Separation Type ============

        Task<bool> SaveSeparationType(EmpSeparationType obj);

        Task<IEnumerable<EmpSeparationTypeVM>> GetAllSeparationType(long accountId);

        Task<EmpSeparationTypeVM> GetSeparationTypeById(long id);

        #endregion =============== Separation Type ============

        #region =============== User Role ============

        Task<bool> SaveUserRole(UserRole obj);

        Task<IEnumerable<UserRoleVM>> GetAllUserRole(long accountId);

        Task<UserRoleVM> GetUserRoleById(long id);

        #endregion =============== User Role ============

        #region ================== Employee Employment Type ===================

        Task<bool> SaveEmploymentType(MasterEmploymentType obj);

        Task<IEnumerable<EmploymentTypeVM>> GetAllEmploymentType(long accountId);

        Task<EmploymentTypeVM> GetEmploymentTypeById(long id);

        Task<MessageHelperCreate> DeleteEmploymentType(long id);

        #endregion ================== Employee Employment Type ===================

        #region ================== Loan Type ===================

        Task<bool> SaveLoanType(EmpLoanType obj);

        Task<IEnumerable<EmpLoanType>> GetAllLoanType();

        Task<EmpLoanType> GetLoanTypeById(long id);

        Task<bool> DeleteLoanType(long id);

        #endregion ================== Loan Type ===================

        #region ================== EmpExpense Type ===================

        Task<bool> SaveExpenseType(EmpExpenseType obj);

        Task<IEnumerable<EmpExpenseType>> GetAllEmpExpenseType(long accountId);

        Task<EmpExpenseType> GetEmpExpenseTypeById(long id);

        Task<bool> DeleteEmpExpense(long id);

        #endregion ================== EmpExpense Type ===================

        #region ================ Employee Position =====================

        Task<MasterPosition> SavePosition(MasterPosition obj);

        Task<IEnumerable<MasterPosition>> GetAllPosition(long accountId, long businessUnitId);

        Task<MasterPosition> GetPositionById(long Id);

        Task<bool> DeletePosition(long id);

        #endregion ================ Employee Position =====================

        #region ================ Master Workplace =====================

        Task<MasterWorkplace> SaveWorkplace(MasterWorkplace obj);

        Task<IEnumerable<WorkPlaceViewModel>> GetAllWorkplace(long accountId, long businessUnitId);

        Task<WorkPlaceViewModel> GetWorkplaceById(long Id);

        Task<bool> DeleteWorkplace(long id);

        #endregion ================ Master Workplace =====================

        #region ================ Master Workplace Group =====================

        Task<MasterWorkplaceGroup> SaveWorkplaceGroup(MasterWorkplaceGroup obj);

        Task<IEnumerable<MasterWorkplaceGroup>> GetAllWorkplaceGroup(long accountId, long businessUnitId);

        Task<MasterWorkplaceGroup> GetWorkplaceGroupById(long Id);

        Task<bool> DeleteWorkplaceGroup(long id);

        #endregion ================ Master Workplace Group =====================

        #region ========== BankWallet ==================

        Task<BankWallet> SaveBankWallet(BankWallet obj);

        Task<IEnumerable<BankWallet>> GetAllBankWallet();

        Task<BankWallet> GetBankWalletById(long id);

        Task<MessageHelper> DeleteBankWalletById(long id);

        #endregion ========== BankWallet ==================

        #region ========== GlobalDocumentType ==================

        Task<long> SaveGlobalDocumentType(GlobalDocumentType obj);

        Task<IEnumerable<GlobalDocumentType>> GetAllGlobalDocumentType(long IntAccountId);

        Task<GlobalDocumentType> GetGlobalDocumentTypeById(long id);

        Task<MessageHelper> DeleteGlobalDocumentTypeById(long id);

        #endregion ========== GlobalDocumentType ==================

        #region ========== GlobalFileUrl ==================

        Task<long> SaveGlobalFileUrl(GlobalFileUrl obj);

        Task<IEnumerable<GlobalFileUrl>> GetAllGlobalFileUrl();

        Task<GlobalFileUrl> GetGlobalFileUrlById(long id);

        #endregion ========== GlobalFileUrl ==================

        #region ========== GlobalInstitute ==================

        Task<long> SaveGlobalInstitute(GlobalInstitute obj);

        Task<IEnumerable<GlobalInstitute>> GetAllGlobalInstitute();

        Task<GlobalInstitute> GetGlobalInstituteById(long id);

        Task<MessageHelper> DeleteGlobalInstituteById(long id);

        #endregion ========== GlobalInstitute ==================

        #region ========== Workline Config ==============

        Task<MessageHelper> CRUDWorklineConfig(EmpWorklineConfigViewModel obj);

        Task<IEnumerable<EmpWorklineConfigViewModel>> GetAllWorklineConfig(long AccoountId);

        Task<EmpWorklineConfigViewModel> GetWorklineConfigById(long IntWorklineId);

        #endregion ========== Workline Config ==============

        #region ========== LveLeaveType ==================

        Task<bool> SaveLveLeaveType(LveLeaveType obj);

        Task<List<LeaveTypeVM>> GetAllLveLeaveType(long accountId);

        Task<LeaveTypeVM> GetLveLeaveTypeById(long id);

        Task<MessageHelper> DeleteLveLeaveTypeById(long id);

        #endregion ========== LveLeaveType ==================

        #region ========== LveMovementType ==================

        Task<long> SaveLveMovementType(LveMovementType obj);

        Task<IEnumerable<LveMovementType>> GetAllLveMovementType(long accountId);

        Task<LveMovementType> GetLveMovementTypeById(long id);

        Task<MessageHelper> DeleteLveMovementTypeById(long id);

        #endregion ========== LveMovementType ==================

        #region ============ leave policy =============

        Task<MessageHelper> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeWiseLeaveBalanceViewModel obj);
        Task<MessageHelper> CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel(CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel obj, BaseVM tokenData);

        #endregion ============ leave policy =============

        #region ========== Poliocy ==========

        Task<MessageHelper> CRUDPolicyCategory(CRUDPolicyCategoryViewModel obj);

        Task<List<GetCommonDDLViewModel>> GetPolicyCategoryDDL(long AccountId);

        Task<List<GetCommonDDLViewModel>> GetPolicyAreaTypes();

        Task<MessageHelper> CreatePolicy(PolicyCommonViewModel obj);

        Task<IEnumerable<GetPolicyLandingViewModel>> GetPolicyLanding(long AccountId, long BusinessUnitId, long CategoryId, string? Search);

        Task<MessageHelper> DeletePolicy(long PolicyId);

        Task<List<GetPolicyLandingViewModel>> GetPolicyOnEmployeeInbox(long EmployeeId);

        Task<MessageHelper> PolicyAcknowledge(long PolicyId, long EmployeeId);

        #endregion ========== Poliocy ==========

        #region ========== Payroll   Management ==========

        Task<MessageHelper> CRUDPayrollManagement(CRUDPayrolManagementViewModel obj);

        #endregion ========== Payroll   Management ==========

        #region ========== Bank Branch ==========

        Task<BankBranchLandingViewModel> BankBranchLanding(long? bankId, long? bankBranchId, long? accountId, string? search);

        Task<BankBranchViewModel> BankBranchLandingById(long IntBankBranchId);

        #endregion ========== Bank Branch ==========

        #region ===== Master dashboard component =====

        Task<MessageHelperCreate> SaveDashboardComponent(MasterDashboardComponent obj);

        Task<MasterDashboardComponent> GetDashboardComponentById(long id);

        Task<List<MasterDashboardComponent>> GetAllDashboardComponent();

        Task<MessageHelperDelete> DeleteDashboardComponent(long id);

        #endregion ===== Master dashboard component =====

        #region ===== Master dashboard component permission =====

        Task<MessageHelperCreate> SaveDashboardComponentPermission(MasterDashboardComponentPermission obj);

        Task<MasterDashboardComponentPermission> GetDashboardComponentPermissionById(long id);

        Task<List<MasterDashboardComponentPermission>> GetAllDashboardComponentPermission();

        Task<MessageHelperDelete> DeleteDashboardComponentPermission(long id);

        #endregion ===== Master dashboard component permission =====

        #region ================ Payroll Element Type =====================

        Task<PyrPayrollElementType> SavePayrollElementType(PyrPayrollElementType obj);

        Task<List<PyrPayrollElementTypeViewModel>> GetAllPayrollElementType(long accountId);

        Task<PyrPayrollElementTypeViewModel> GetPayrollElementTypeById(long Id);

        Task<bool> DeletePayrollElementTypeById(long Id);

        #endregion ================ Payroll Element Type =====================

        #region ================ Salary Breakdow =====================

        Task<PyrSalaryBreakdownHeader> SaveSalaryBreakdownHeader(PyrSalaryBreakdownHeader obj);

        Task<List<PyrSalaryBreakdownHeader>> GetAllSalaryBreakdownHeader(long accountId);

        Task<PyrSalaryBreakdownHeader> GetSalaryBreakdownById(long Id);

        #endregion ================ Salary Breakdow =====================

        #region ================ Salary Policy =====================

        Task<PyrSalaryPolicy> SaveSalaryPolicy(PyrSalaryPolicy obj);

        Task<List<PyrSalaryPolicyViewModel>> GetAllSalaryPolicy(long accountId);

        Task<PyrSalaryPolicyViewModel> GetSalaryPolicyById(long Id);

        Task<bool> DeleteSalaryPolicyById(long Id);

        #endregion ================ Salary Policy =====================

        #region ================ OverTime Configuration =====================
        Task<GetOverTimeConfigurationVM> GetOverTimeConfiguration(int accountId);
        Task<GetOverTimeConfigurationVM> SaveOTConfiguration(GetOverTimeConfigurationVM obj);
        Task<OverTimeConfigurationVM> GetOverTimeConfigById(long intAccountId);
        Task<bool> UpdateTimeAttenSummery(List<TimeAttendanceDailySummeryVM> obj);
        Task<OverTimeConfigurationDetail> SaveOTConfigDetails(OverTimeConfigurationDetail objDetails);
        #endregion

        #region =============== Tax Challan Config =================
        Task<MasterTaxChallanConfig> SaveMasterTaxChallanConfig(MasterTaxChallanConfig obj);
        Task<MasterTaxchallanConfigVM> GetTaxChallanConfigById(long Id);
        Task<IEnumerable<MasterTaxchallanConfigVM>> GetAllMasterTaxchallanConfig(long intAccountId);
        #endregion

        #region ================ Master Territory Type ================

        Task<MasterTerritoryType> SaveTerritoryType(MasterTerritoryType obj);

        Task<IEnumerable<MasterTerritoryTypeVM>> GetAllTerritoryType(long accountId, long businessUnitId);

        ////Task<MasterTerritoryTypeVM> GetTerritoryTypeById(long id);

        #endregion
    }
}