using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Global;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IApprovalPipelineService
    {
        Task<EmpIsSupNLMORUGMemberViewModel> EmployeeIsSupervisorNLineManagerORUserGroupMember(long accountId, long employeeId);
        Task<EmpIsSupNLMORUGMemberViewModel> PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(long AccountId, string ApplicationType, long EmployeeId);
        Task<PipelineStageInfoVM> GetCurrentNextStageAndPipelineHeaderId(long accountId, string pipelineType);
        Task<bool> LeaveBalanceAndAttendanceUpdateAfterLeaveApproved(LveLeaveApplication application);
        Task<bool> AttendanceSummaryUpdateAfterMovementApproved(LveMovementApplication application);
        Task<bool> AttendanceSummaryUpdateAfterRemoteAttendanceApproved(TimeEmployeeAttendance application);
        Task<bool> LoanBalanceRollBackAfterGeneratedSalaryRejectd(PyrPayrollSalaryGenerateRequest application);

        Task<LeaveApprovalResponseVM> LeaveApprovalEngine(LveLeaveApplication application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> LeaveLandingEngine(LeaveApplicationLandingRequestVM model, BaseVM tokenData);

        Task<MovementApprovalResponse> MovementApprovalEngine(LveMovementApplication application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> MovementLandingEngine(MovementApplicationLandingRequestVM model, BaseVM tokenData);

        Task<RemoteAttendanceLocationNDeviceApprovalResponse> RemoteAttendanceLocationNDeviceApprovalEngine(TimeRemoteAttendanceRegistration application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> RemoteAttendanceLocationNDeviceLandingEngine(RemoteAttendanceLocationNDeviceLandingViewModel model, BaseVM tokenData);

        Task<RemoteAttendanceApprovalResponse> RemoteAttendanceApprovalEngine(TimeEmployeeAttendance application, bool isReject, long approverId, long accountId, bool IsMarket);
        Task<ApplicationLandingVM> RemoteAttendanceLandingEngine(RemoteAttendanceLandingViewModel model, BaseVM tokenData, bool IsMarket);

        //SalaryAdditionNDeduction
        Task<SalaryAdditionNDeductionApprovalResponse> SalaryAdditionNDeductionApprovalEngine(PyrEmpSalaryAdditionNdeduction application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> SalaryAdditionNDeductionLandingEngine(SalaryAdditionNDeductionLandingViewModel model, BaseVM tokenData);

        //IOU
        Task<IOUApprovalResponse> IOUApplicationApprovalEngine(PyrIouapplication application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> IOUApplicationLandingEngine(IOULandingViewModel model, BaseVM tokenData);

        //IOU Adjustment
        Task<IOUAdjustmentApprovalResponse> IOUAdjustmentApprovalEngine(PyrIouadjustmentHistory application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> IOUAdjustmentLandingEngine(IOUAdjustmentLandingViewModel model, BaseVM tokenData);

        //SalaryGeneratRequest
        Task<SalaryGenerateRequestApprovalResponse> SalaryGenerateRequestApprovalEngine(PyrPayrollSalaryGenerateRequest application, bool isReject, long approverId, long accountId);
        //Task<SalaryGenerateRequestApprovalResponse> SalaryGenerateRequestLandingEngine(SalaryGenerateRequestLandingViewModel model);
        Task<ApplicationLandingVM> SalaryGenerateRequestLandingEngine(SalaryGenerateRequestLandingRequestVM model, BaseVM tokenData);

        //Loan 
        Task<LoanApprovalResponse> LoanApprovalEngine(EmpLoanApplication application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> LoanLandingEngine(LoanApplicationLandingViewModel model, BaseVM tokenData);

        //OverTime 
        Task<OverTimeApprovalResponse> OverTimeApprovalEngine(TimeEmpOverTime application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> OverTimeLandingEngine(OverTimeLandingViewModel model, BaseVM tokenData);

        //ManualAttendance
        Task<ManualAttendanceSummaryApprovalResponse> ManualAttendanceApprovalEngine(EmpManualAttendanceSummary application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> ManualAttendanceLandingEngine(ManualAttendanceSummaryLandingVM model, BaseVM tokenData);

        //Employee Separation
        Task<EmployeeSeparationApprovalResponse> EmployeeSeparationApprovalEngine(EmpEmployeeSeparation application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> EmployeeSeparationLandingEngine(EmployeeSeparationLandingViewModel model, BaseVM tokenData);

        //Promotion & Increment
        Task<TransferNPromotionApprovalResponse> TransferNPromotionApprovalEngine(EmpTransferNpromotion application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> TransferNPromotionLandingEngine(TransferNPromotionLandingViewModel model, BaseVM tokenData);

        //Employee Increment
        Task<EmployeeIncrementApprovalResponse> EmployeeIncrementApprovalEngine(EmpEmployeeIncrement application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> EmployeeIncrementLandingEngine(EmployeeIncrementLandingViewModel model, BaseVM tokenData);

        //Bonus Generate Header
        Task<BonusGenerateHeaderApprovalResponse> BonusGenerateHeaderApprovalEngine(PyrBonusGenerateHeader application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> BonusGenerateHeaderLandingEngine(BonusGenerateHeaderLandingViewModel model, BaseVM tokenData);

        // PF Withdraw
        Task<PFWithdrawApprovalResponse> PFWithdrawApprovalEngine(EmpPfwithdraw application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model, BaseVM tokenData);

        //Arrear Salary Generate Request
        Task<ArearSalaryGenerateRequestApprovalResponse> ArearSalaryGenerateRequestApprovalEngine(PyrArearSalaryGenerateRequest application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> ArearSalaryGenerateRequestLandingEngine(ArearSalaryGenerateRequestLandingViewModel model, BaseVM tokenData);

        //Expense Application
        Task<ExpenseApprovalResponse> ExpenseApprovalEngine(EmpExpenseApplication application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> ExpenseLandingEngine(ExpenseApplicationLandingViewModel model, BaseVM tokenData);

        //Salary Certificate Requisition
        Task<SalaryCertificateRequestApprovalResponse> SalaryCertificateRequestApprovalEngine(EmpSalaryCertificateRequest application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> SalaryCertificateRequestLandingEngine(SalaryCertificateRequestLandingViewModel model, BaseVM tokenData);

        //Asset Requisition
        Task<AssetRequisitionApprovalResponse> AssetRequisitionApprovalEngine(AssetRequisition application, bool isReject, long approverId, long accountId);
        Task<ApplicationLandingVM> AssetRequisitionLandingEngine(AssetRequisitionLandingRequestVM model, BaseVM tokenData);

        //Asset Transfer
        Task<AssetTransferApprovalResponse> AssetTransferApprovalEngine(AssetTransfer application, bool isReject, long approverId, long accountId);
        Task<AssetTransferApprovalResponse> AssetTransferLandingEngine(AssetTransferLandingViewModel model);

        //Training Schedule
        Task<TrainingScheduleApprovalResponse> TrainingScheduleLandingEngine(TrainingScheduleLandingViewModel model);
        Task<TrainingScheduleApprovalResponse> TrainingScheduleApprovalEngine(TrainingSchedule application, bool isReject, long approverId, long accountId);

        //Training Requisition
        Task<ApplicationLandingVM> TrainingRequisitionLandingEngine(TrainingRequisitionLandingViewModel model, BaseVM tokenData);
        Task<TrainingRequisitionApprovalResponse> TrainingRequisitionApprovalEngine(TrainingRequisition application, bool isReject, long approverId, long accountId);

        //MasterLocationEmployeeWise Approval
        Task<MasterLocationApprovalResponse> MastrerLocaLandingEngine(MasterLocationLandingViewModel model);
        Task<MasterLocationApprovalResponse> MasterLocationApprovalEngine(MasterLocationRegister application, bool isReject, long approverId, long accountId);

        #region ======= Get pipeline current stage next stage====
        Task<PipelineStageInfoVM> GetPipelineDetailsByEmloyeeIdNType(long intEmployeeId, string ApplicationType);
        Task<PipelineStageInfoVM> GetPipelineDetailsByEmloyeeIdNType(long AccountId, long BusinessUnitId, long WorkPlaceGroupId, long soleDepoId, long AreaId, string ApplicationType);
        #endregion
    }
}
