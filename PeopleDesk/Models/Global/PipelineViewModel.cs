using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Models.Global
{
    public class PipelineViewModel
    {
    }

    public class PipelineStageInfoVM
    {
        public long HeaderId { get; set; }
        public long CurrentStageId { get; set; }
        public long NextStageId { get; set; }
        public MessageHelper? messageHelper { get; set; } = new();
    }

    public class EmpIsSupNLMORUGMemberViewModel
    {
        public bool IsSupervisor { get; set; }
        public bool IsLineManager { get; set; }
        public bool IsUserGroup { get; set; }
        public List<UserGroupRow> UserGroupRows { get; set; }
    }

    public class PipelineApprovalViewModel
    {
        public long? ApplicationId { get; set; }

        //public long? EmployeeId { get; set; }
        public long? ApproverEmployeeId { get; set; } // from back end

        public bool IsReject { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long AccountId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    #region Leave Application

    public class LeaveApprovalResponseVM
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<LeaveApplicationLandingResponseVM> ListData { get; set; }
    }

    public class LeaveApplicationLandingRequestVM
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class LeaveApplicationLandingResponseVM
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string LeaveType { get; set; }
        public string DateRange { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public LveLeaveApplication LeaveApplication { get; set; }
    }

    #endregion Leave Application

    #region Movement Application
    public class MovementApplicationLandingRequestVM
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }
    public class MovementApplicationLandingResponseVM
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string MovementType { get; set; }
        public string DateRange { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public LveMovementApplication MovementApplication { get; set; }
    }
    public class MovementApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<MovementApplicationLandingReturnViewModel> ListData { get; set; }
    }

    public class MovementApplicationLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }

        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long? LeaveTypeId { get; set; } = 0;
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public long AccountId { get; set; }
        public long? IntId { get; set; } = 0;
        public long MovementTypeId { get; set; }
    }

    public class MovementApplicationLandingReturnViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string LeaveType { get; set; }
        public string DateRange { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public string MovementType { get; set; }
        public LveMovementApplication MovementApplication { get; set; }
    }

    #endregion Movement Application

    #region Salary Addition and Deduction

    public class SalaryAdditionNDeductionApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<SalaryAdditionNDeductionLandingReturnViewModel> ListData { get; set; }
    }

    public class SalaryAdditionNDeductionLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class SalaryAdditionNDeductionLandingReturnViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string DateRange { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public DateTime? date { get; set; }
        public PyrEmpSalaryAdditionNdeduction Application { get; set; }
    }

    #endregion Salary Addition and Deduction

    #region Remote Attendance

    public class RemoteAttendanceApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<RemoteAttendanceLandingReturnViewModel> ListData { get; set; }
    }

    public class RemoteAttendanceLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }

        public long? BusinessUnitId { get; set; } = 0;
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntId { get; set; } = 0;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class RemoteAttendanceLandingReturnViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public TimeEmployeeAttendance Application { get; set; }
    }

    #endregion Remote Attendance

    #region Remote Attendance Location & Landing

    public class RemoteAttendanceLocationNDeviceApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<RemoteAttendanceLocationNDeviceLandingReturnViewModel> ListData { get; set; }
    }

    public class RemoteAttendanceLocationNDeviceLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class RemoteAttendanceLocationNDeviceLandingReturnViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string LeaveType { get; set; }
        public string DateRange { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public TimeRemoteAttendanceRegistration Application { get; set; }
    }

    #endregion Remote Attendance Location & Landing

    #region IOU

    public class IOUApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<IOULandingReturnViewModel> ListData { get; set; }
    }

    public class IOULandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class IOULandingReturnViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string LeaveType { get; set; }
        public string DateRange { get; set; }
        public string ApplicationDate { get; set; }
        public decimal? AdjustedAmount { get; set; }
        public decimal? PayableAmount { get; set; }
        public decimal? ReceivableAmount { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public PyrIouapplication Application { get; set; }
    }

    #endregion IOU

    #region IOU Adjustment

    public class IOUAdjustmentApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<IOUAdjustmentLandingReturnViewModel> ListData { get; set; }
    }

    public class IOUAdjustmentLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class IOUAdjustmentLandingReturnViewModel
    {
        public long IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public decimal? NumPayableAmount { get; set; }
        public decimal? NumReceivableAmount { get; set; }
        public decimal? NumAdjustmentAmount { get; set; }
        public decimal? AccountsAdjustmentAmount { get; set; }
        public bool? IsAcknowledgement { get; set; }
        public string? Status { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        //public DateTime? DteFromDate { get; set; }
        //public DateTime? DteToDate { get; set; }
        public string? DateRange { get; set; }
        public decimal? IOUAmount { get; set; }
        public string? Description { get; set; }
        public long? ImgUrlId { get; set; }
        public bool? IsActive { get; set; }
        public string CurrentStage { get; set; }
        public PyrIouadjustmentHistory Application { get; set; }
    }

    #endregion IOU Adjustment

    #region Loan

    public class LoanApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<LoanApplicationLandingReturnViewModel> ListData { get; set; }
    }

    public class LoanApplicationLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class LoanApplicationLandingReturnViewModel
    {
        public long IntLoanApplicationId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string StrDepartment { get; set; }
        public long? IntDesignationId { get; set; }
        public string StrDesignation { get; set; }
        public string LoanApplicationDate { get; set; }
        public long IntLoanTypeId { get; set; }
        public string StrLoanType { get; set; }
        public long IntLoanAmount { get; set; }
        public long IntNumberOfInstallment { get; set; }
        public long IntNumberOfInstallmentAmount { get; set; }
        public string CurrentStage { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public EmpLoanApplication Application { get; set; }
    }

    #endregion Loan

    #region Salary Generate

    public class SalaryGenerateRequestApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<SalaryGenerateRequestLandingReturnViewModel> ListData { get; set; }
    }
    public class SalaryGenerateRequestLandingRequestVM
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class SalaryGenerateRequestLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? WorkplaceId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntId { get; set; } = 0;
    }

    public class SalaryGenerateRequestLandingReturnViewModel
    {
        public long SalaryGenerateRequestId { get; set; }
        public string SalaryCode { get; set; } = null!;
        public long IntAccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string BusinessUnit { get; set; } = null!;

        public string? SalaryType { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long? SoleDepoId { get; set; }
        public string? SoleDepoName { get; set; }
        public long? AreaId { get; set; }
        public string? AreaName { get; set; }
        public string? TerritoryIdList { get; set; }
        public string? TerritoryNameList { get; set; }
        public decimal? NumNetPayableSalary { get; set; }
        public string? DateRange { get; set; }
        public long? IntMonth { get; set; }
        public long? IntYear { get; set; }
        public string? StrDescription { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public PyrPayrollSalaryGenerateRequest Application { get; set; }
    }

    #endregion Salary Generate

    #region Over Time

    public class OverTimeApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<OverTimeLandingReturnViewModel> ListData { get; set; }
    }

    public class OverTimeLandingReturnViewModel
    {
        public long IntOverTimeId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntDepartmentId { get; set; }
        public string StrDepartment { get; set; }
        public long? IntDesignationId { get; set; }
        public string StrDesignation { get; set; }
        public string? OverTimeDate { get; set; }
        public TimeSpan? TmeStartTime { get; set; }
        public TimeSpan? TmeEndTime { get; set; }
        public decimal? NumOverTimeHour { get; set; }
        public string? StrReason { get; set; }
        public bool? IsActive { get; set; }
        public string CurrentStage { get; set; }
        public string Status { get; set; }
        public TimeEmpOverTime Application { get; set; }
    }

    public class OverTimeLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Over Time

    #region Manual Attendance Summary

    public class ManualAttendanceSummaryApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<ManualAttendanceSummaryLandingReturnViewModel> ListData { get; set; }
    }

    public class ManualAttendanceSummaryLandingVM
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class ManualAttendanceLandingResponseVM
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public string RequestStatus { get; set; }
        public string CurrentStatus { get; set; }
        public EmpManualAttendanceSummary application { get; set; }
    }
    public class ManualAttendanceSummaryLandingReturnViewModel
    {
        public long IntId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntAttendanceSummaryId { get; set; }
        public DateTime? DteAttendanceDate { get; set; }
        public string? TimeInTime { get; set; }
        public string? TimeOutTime { get; set; }
        public string? StrCurrentStatus { get; set; }
        public string? StrRequestStatus { get; set; }
        public string? StrRemarks { get; set; }
        public string CurrentStage { get; set; }
        public EmpManualAttendanceSummary Application { get; set; }
    }

    public class ManualAttendanceSummaryLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? WorkplaceId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntId { get; set; } = 0;
    }

    #endregion Manual Attendance Summary

    #region Employee Separation

    public class EmployeeSeparationApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<EmployeeSeparationLandingReturnViewModel> ListData { get; set; }
    }

    public class EmployeeSeparationLandingReturnViewModel
    {
        public long IntSeparationId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? EmployeeCode { get; set; } = null!;
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntSeparationTypeId { get; set; }
        public string? StrSeparationTypeName { get; set; }
        public string? DteSeparationDate { get; set; }
        public string? DteLastWorkingDate { get; set; }
        public string? StrDocumentId { get; set; }
        public string? StrReason { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public EmpEmployeeSeparation Application { get; set; }
    }

    public class EmployeeSeparationLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Employee Separation

    #region Transfer & Promotion

    public class TransferNPromotionApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<TransferNPromotionLandingReturnViewModel> ListData { get; set; }
    }

    public class TransferNPromotionLandingReturnViewModel
    {
        public long IntTransferNpromotionId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string? StrTransferNpromotionType { get; set; }
        public long? IntTransferOrpromotedFrom { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string? BusinessUnitName { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public string? WorkplaceGroupName { get; set; }
        public long IntDepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long IntDesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long IntSupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public long IntLineManagerId { get; set; }
        public string? LineManagerName { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public string DteEffectiveDate { get; set; }
        public string? DteReleaseDate { get; set; }
        public long? IntAttachementId { get; set; }
        public string? StrRemarks { get; set; }
        public string StrStatus { get; set; }
        public long IntBusinessUnitIdFrom { get; set; }
        public string? BusinessUnitNameFrom { get; set; }
        public long IntWorkplaceGroupIdFrom { get; set; }
        public string? WorkplaceGroupNameFrom { get; set; }
        public long IntDepartmentIdFrom { get; set; }
        public string? DepartmentNameFrom { get; set; }
        public long IntDesignationIdFrom { get; set; }
        public string? DesignationNameFrom { get; set; }
        public long? IntSupervisorIdFrom { get; set; }
        public string? SupervisorNameFrom { get; set; }
        public long? IntLineManagerIdFrom { get; set; }
        public string? LineManagerNameFrom { get; set; }
        public long? IntDottedSupervisorIdFrom { get; set; }
        public string? DottedSupervisorNameFrom { get; set; }
        public string CurrentStage { get; set; }

        //public List<EmpTransferNpromotionUserRoleVM>? EmpTransferNpromotionUserRoleVMList { get; set; }
        //public List<EmpTransferNpromotionRoleExtensionVM>? EmpTransferNpromotionRoleExtensionVMList { get; set; }
    }

    public class TransferNPromotionLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Transfer & Promotion

    #region Employee Increment

    public class EmployeeIncrementApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<EmployeeIncrementLandingReturnViewModel> ListData { get; set; }
    }

    public class EmployeeIncrementLandingReturnViewModel
    {
        public long IntId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDesignationTypeId { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrIncrementDependOn { get; set; }
        public decimal? NumIncrementDependOn { get; set; }
        public decimal? NumIncrementPercentage { get; set; }
        public decimal? NumIncrementAmount { get; set; }
        public DateTime? DteEffectiveDate { get; set; }
        public long? IntTransferNpromotionReferenceId { get; set; }
        public bool? IsActive { get; set; }
        public string CurrentStage { get; set; }
        public EmpEmployeeIncrement Application { get; set; }
    }

    public class EmployeeIncrementLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Employee Increment

    #region Bonus Generate Header

    public class BonusGenerateHeaderApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<BonusGenerateHeaderLandingReturnViewModel> ListData { get; set; }
    }

    public class BonusGenerateHeaderLandingReturnViewModel
    {
        public long IntBonusHeaderId { get; set; }
        //public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntBonusId { get; set; }
        public string StrBonusName { get; set; } = null!;
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntWorkplaceId { get; set; }
        public string? StrWorkplaceName { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string? StrPayrollGroupName { get; set; }
        public string? DteEffectedDateTime { get; set; }
        public decimal? NumBonusAmount { get; set; }
        public bool? IsArrearBonus { get; set; }
        public long? IntArrearBonusReferenceId { get; set; }
        public int? IntBonusYearCal { get; set; }
        public int? IntBonusMonthCal { get; set; }
        public long? SoleDepoId { get; set; }
        public string? SoleDepoName { get; set; }
        public long? AreaId { get; set; }
        public string? AreaName { get; set; }
        public string? Status { get; set; }
        public string CurrentStage { get; set; }
        public PyrBonusGenerateHeader Application { get; set; }
    }

    public class BonusGenerateHeaderLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Bonus Generate Header

    #region PF Withdraw

    public class PFWithdrawApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<PFWithdrawLandingReturnViewModel> ListData { get; set; }
    }

    public class PFWithdrawLandingReturnViewModel
    {
        public long IntId { get; set; }
        public long IntEmployeeId { get; set; }
        public string? StrEmployee { get; set; }
        public string? EmployeeCode { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntAccountId { get; set; }
        public DateTime DteApplicationDate { get; set; }
        public decimal NumWithdrawAmount { get; set; }
        public string? StrReason { get; set; }
        public bool IsActive { get; set; }
        public string CurrentStage { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public EmpPfwithdraw Application { get; set; }
    }

    public class PFWithdrawLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion PF Withdraw

    #region Arear Salary Generate Request

    public class ArearSalaryGenerateRequestApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<ArearSalaryGenerateRequestLandingReturnViewModel> ListData { get; set; }
    }

    public class ArearSalaryGenerateRequestLandingReturnViewModel
    {
        public long ArearSalaryGenerateRequestId { get; set; }
        public string ArearSalaryCode { get; set; } = null!;
        //public long IntAccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string BusinessUnit { get; set; } = null!;
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public decimal? numPercentOfGross { get; set; }
        public string? SalaryPolicyName { get; set; }
        public string? DateRange { get; set; }
        public DateTime DteEffectiveFrom { get; set; }
        public DateTime DteEffectiveTo { get; set; }
        public bool IsGenerated { get; set; }
        public decimal NumNetPayableSalary { get; set; }
        public string? StrDescription { get; set; }
        public bool IsActive { get; set; }
        public string CurrentStage { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public PyrArearSalaryGenerateRequest Application { get; set; }
    }

    public class ArearSalaryGenerateRequestLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Arear Salary Generate Request

    #region Expense Application

    public class ExpenseApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<ExpenseApplicationLandingReturnViewModel> ListData { get; set; }
    }

    public class ExpenseApplicationLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    public class ExpenseApplicationLandingReturnViewModel
    {
        public long IntExpenseId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public long IntEmployeeId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public long IntExpenseTypeId { get; set; }
        public string StrExpenseType { get; set; }
        //public DateTime DteExpenseFromDate { get; set; }
        //public DateTime DteExpenseToDate { get; set; }
        public string DateRange { get; set; }
        public string? StrDiscription { get; set; }
        public decimal? NumExpenseAmount { get; set; }
        public long? IntDocumentId { get; set; }
        public bool? IsActive { get; set; }
        public string CurrentStage { get; set; }
        public EmpExpenseApplication Application { get; set; }
    }

    #endregion Expense Application

    #region Salary Certificate Request

    public class SalaryCertificateRequestApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<SalaryCertificateRequestLandingReturnViewModel> ListData { get; set; }
    }

    public class SalaryCertificateRequestLandingReturnViewModel
    {
        public long IntId { get; set; }
        public long IntEmployeeId { get; set; }
        public string? StrEmployee { get; set; }
        public string? EmployeeCode { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntPayRollMonth { get; set; }
        public long? IntPayRollYear { get; set; }
        public bool IsActive { get; set; }
        public string CurrentStage { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public EmpSalaryCertificateRequest Application { get; set; }
    }

    public class SalaryCertificateRequestLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Salary Certificate Request

    #region Asset Requisition
    public class AssetRequisitionLandingRequestVM
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }
    public class AssetRequisitionLandingResponseVM
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? ProfileUrlId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string DateRange { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public AssetRequisition application { get; set; }
    }
    public class AssetRequisitionApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<AssetRequisitionLandingReturnViewModel> ListData { get; set; }
    }

    public class AssetRequisitionLandingReturnViewModel
    {
        public long AssetRequisitionId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public long ReqisitionQuantity { get; set; }
        public DateTime ReqisitionDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PipelineHeaderId { get; set; }
        public string CurrentStage { get; set; }
        public long NextStage { get; set; }
        public string Status { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public long? RejectedBy { get; set; }
        public bool? IsDenied { get; set; }
        public AssetRequisition Application { get; set; }
    }

    public class AssetRequisitionLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? WorkplaceId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntAssetRequisitionId { get; set; } = 0;
    }

    #endregion Asset Requisition

    #region Asset Transfer

    public class AssetTransferApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<AssetTransferLandingReturnViewModel> ListData { get; set; }
    }

    public class AssetTransferLandingReturnViewModel
    {
        public long AssetTransferId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long? FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long? TransferQuantity { get; set; }
        public long? ToEmployeeId { get; set; }
        public string ToEmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string ItemCategory { get; set; }
        public DateTime? TransferDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PipelineHeaderId { get; set; }
        public string CurrentStage { get; set; }
        public long NextStage { get; set; }
        public string Status { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public long? RejectedBy { get; set; }
        public bool? IsDenied { get; set; }
        public AssetTransfer Application { get; set; }
    }

    public class AssetTransferLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? WorkplaceId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? AssetTransferId { get; set; } = 0;
    }

    #endregion Asset Transfer

    #region Training Schedule
    public class TrainingScheduleApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<TrainingScheduleLandingReturnViewModel> ListData { get; set; }
    }
    public class TrainingScheduleLandingReturnViewModel
    {
        public long TrainingScheduleId { get; set; }
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string TrainingCode { get; set; }
        public DateTime DteDate { get; set; }
        public decimal TotalDuration { get; set; }
        public string Venue { get; set; }
        public string ResourcePersonName { get; set; }
        public long BatchSize { get; set; }
        public string BatchNo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PipelineHeaderId { get; set; }
        public string CurrentStage { get; set; }
        public long? NextStage { get; set; }
        public string Status { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public long? RejectedBy { get; set; }
        public bool? IsDenied { get; set; }
        public TrainingSchedule Application { get; set; }
    }

    public class TrainingScheduleLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? WorkplaceId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; } = 0;
        public long? DepartmentId { get; set; } = 0;
        public long? DesignationId { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntTrainingScheduleId { get; set; } = 0;
    }
    #endregion

    #region Training Requisition

    public class TrainingRequisitionApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<TrainingRequisitionLandingReturnViewModel> ListData { get; set; }
    }

    public class TrainingRequisitionLandingReturnViewModel
    {
        public long TrainingRequisitionId { get; set; }
        public long? ScheduleId { get; set; }       
        public long BusinessUnitId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string TrainingName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string? Gender { get; set; }        
        public string CurrentStage { get; set; }       
        public string Status { get; set; }        
        public TrainingRequisition Application { get; set; }
    }

    public class TrainingRequisitionLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public long BusinessUnitId { get; set; } = -1;
        public long WorkplaceGroupId { get; set; } = -1;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PageSize { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
    }

    #endregion Training Requisition

    #region Master Location
    public class MasterLocationApprovalResponse
    {
        public string ResponseStatus { get; set; }
        public string ApplicationStatus { get; set; }
        public long CurrentSatageId { get; set; }
        public long NextSatageId { get; set; }
        public bool IsComplete { get; set; }
        public List<MasterLocationLandingReturnViewModel> ListData { get; set; }
    }
    public class MasterLocationLandingViewModel
    {
        public string ApplicationStatus { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public long? IsSupOrLineManager { get; set; } = 0;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsLineManager { get; set; } = false;
        public bool? IsUserGroup { get; set; } = false;
        public long ApproverId { get; set; }
        public long? BusineessUnit { get; set; } = 0;
        public long? ApplicantId { get; set; } = 0;
        public long AccountId { get; set; }
        public long? IntId { get; set; } = 0;
    }

    public class MasterLocationLandingReturnViewModel
    {
        public long IntMasterLocationId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessId { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public bool? IsActive { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public string Status { get; set; }
        public string CurrentStage { get; set; }
        public MasterLocationRegister Application { get; set; }
    }
    #endregion
    public class ApplicationLandingVM
    {
        public dynamic Data { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int TotalCount { get; set; }
    }

}