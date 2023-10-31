using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PeopleDesk.Models.Global
{
    public class PdfViewModel
    {

        public class DailyAttendanceReportViewModel
        {
            public string? RowId { get; set; }
            public string? Attendance { get; set; }
            public string? InTime { get; set; }
            public string? OutTime { get; set; }
            public string? AttStatus { get; set; }
            public string? MAddress { get; set; }
            public string? MReason { get; set; }
            public string? Remarks { get; set; }

        }
        public class PdfDailyAttendanceReportViewModel
        {
            public EmployeeQryProfileAllViewModel EmployeeQryProfileAll { get; set; }
            public List<DailyAttendanceReportViewModel> DailyAttendanceReportViewModelList { get; set; }
            public string? FromDate { get; set; }
            public string? ToDate { get; set; }

        }
        //public class EmployeeQryProfileAllViewModel
        //{
        //    public EmpEmployeeBasicInfo EmployeeBasicInfo { get; set; }
        //    public MasterBusinessUnit BusinessUnit { get; set; }
        //    public long? DepartmentId { get; set; }
        //    public string? DepartmentName { get; set; }
        //    public long? DesignationId { get; set; }
        //    public string? DesignationName { get; set; }
        //    public long? SupervisorId { get; set; }
        //    public string? SupervisorName { get; set; }
        //    public long? DottedSupervisorId { get; set; }
        //    public string? DottedSupervisorName { get; set; }
        //    public long? LineManagerId { get; set; }
        //    public string? LineManagerName { get; set; }
        //    public long? EmploymentTypeId { get; set; }
        //    public string? EmploymentTypeName { get; set; }
        //    public long? WorkplaceGroupId { get; set; }
        //    public string? WorkplaceGroupName { get; set; }
        //}
        public class EmployeeSeparationReportViewModel
        {
            public EmpEmployeeBasicInfo EmployeeInfoObj { get; set; }
            public MasterEmploymentType EmployeeTypeObj { get; set; }
            public EmpEmployeeBasicInfo SupervisorObj { get; set; }
            public EmpEmployeeBasicInfo LineManagerObj { get; set; }
            public MasterDepartment DepartmentObj { get; set; }
            public MasterDesignation DesignationObj { get; set; }
            public EmpSeparationType SeparationTypeObj { get; set; }
            public EmpEmployeeSeparation SeperationObj { get; set; }
            public PfAccountViewModel pfAccountViewModel { get; set; }
        }

        public class LoanDetailsReportViewModel
        {

            public string? loanApplicationId { get; set; }
            public string? employeeId { get; set; }
            public string? loanTypeId { get; set; }
            public string? loanAmount { get; set; }
            public string? numberOfInstallment { get; set; }
            public string? numberOfInstallmentAmount { get; set; }
            public string? applicationDate { get; set; }
            public string? isApprove { get; set; }
            public string? approveBy { get; set; }
            public string? approveDate { get; set; }
            public string? approveLoanAmount { get; set; }
            public string? approveNumberOfInstallment { get; set; }
            public string? approveNumberOfInstallmentAmount { get; set; }
            public string? remainingBalance { get; set; }
            public string? effectiveDate { get; set; }
            public string? description { get; set; }
            public string? fileUrl { get; set; }
            public string? referenceNo { get; set; }
            public string? isActive { get; set; }
            public string? isReject { get; set; }
            public string? rejectBy { get; set; }
            public string? rejectDate { get; set; }
            public string? insertByUserId { get; set; }
            public string? insertDateTime { get; set; }
            public string? updateByUserId { get; set; }
            public string? updateDateTime { get; set; }
            public string? employeeName { get; set; }
            public string? reScheduleCount { get; set; }
            public string? reScheduleNumberOfInstallment { get; set; }
            public string? reScheduleNumberOfInstallmentAmount { get; set; }
            public string? reScheduleDateTime { get; set; }
            public string? employeeCode { get; set; }
            public string? departmentName { get; set; }
            public string? designationName { get; set; }
            public string? positonGroupName { get; set; }
            public string? positionName { get; set; }
            public string? loanType { get; set; }
            public string? paidAmount { get; set; }
            public string? dueInstallment { get; set; }
            public string? paidInstallment { get; set; }
            public string? applicationStatus { get; set; }
            public string? installmentStatus { get; set; }
            public string? businessUnitId { get; set; }
            public string? employmentTypeName { get; set; }
            public string? joiningDate { get; set; }

            public MasterBusinessUnit BusinessUnit { get; set; }
            public PyrEmployeeSalaryDefault EmployeeSalaryDefault { get; set; }
            public List<EmpLoanSchedule> LoanReScheduleList { get; set; }

        }
        public class LeaveHistoryViewModel
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public EmployeeDetailsViewModel EmployeeDetailsViewModel { get; set; }
            public MasterBusinessUnit BusinessUnit { get; set; }
            public List<LeaveApplicationReportViewModel> LeaveApplicationList { get; set; }
        }
        public class LeaveApplicationReportViewModel
        {
            public LveLeaveApplication LeaveApplication { get; set; }
            public LveLeaveType LeaveType { get; set; }
        }
        public class MonthlySalaryReportViewModel
        {
            public string MonthName { get; set; }
            public MasterBusinessUnit BusinessUnit { get; set; }
            public List<SalaryGenerateHeaderReprotViewModel> SalaryGenerateHeaderList { get; set; }
        }
        public class SalaryGenerateHeaderReprotViewModel
        {

            public PyrSalaryGenerateHeader SalaryGenerateHeaderObj { get; set; }
            public EmpEmployeeBasicInfo EmployeeBasicInfoObj { get; set; }
            public MasterDepartment EmpDepartmentObj { get; set; }
            public MasterDesignation EmpDesignationObj { get; set; }
            public PyrPayscaleGrade PayscaleGradeObj { get; set; }
            public MasterEmploymentType EmploymentTypeObj { get; set; }
            public MasterWorkplaceGroup WorkplaceGroupObj { get; set; }

        }
        public class EmployeePayslipViewModel
        {
            public List<EmployeePayslipAddDedViewModel> AdditionList { get; set; }
            public decimal TotalAllowance { get; set; }
            public decimal GrossEarnings { get; set; }
            public List<EmployeePayslipAddDedViewModel> DeductionList { get; set; }
            public decimal TotalDeduction { get; set; }
            public decimal NetPayable { get; set; }
            public decimal Gratuity { get; set; }
            public decimal ProvidentFund { get; set; }
            public EmployeeDetailsViewModel EmployeeDetails { get; set; }
            public MasterBusinessUnit BusinessUnit { get; set; }
            public string MonthName { get; set; }
            public PyrSalaryGenerateHeader SalaryGenerateHeader { get; set; }

        }
        public class EmployeePayslipAddDedViewModel
        {
            public long SequenceId { get; set; }
            public string SalaryPortionName { get; set; }
            public decimal PortionAmount { get; set; }
        }


        public class RosterReportViewModel
        {
            public string? EmployeeId { get; set; }
            public string? strEmployeeCode { get; set; }
            public string? strEmployeeName { get; set; }
            public string? strDepartmentName { get; set; }
            public string? strDesignationName { get; set; }
            public string? intCalendarId { get; set; }
            public string? strCalendarName { get; set; }
            public string? strCalendarType { get; set; }
            public string? strRosterGroupName { get; set; }

        }

        public class LeaveDataSetViewModel
        {
            public long? intApplicationId { get; set; }
            public long? intAccountId { get; set; }
            public long? intBusinessUnitId { get; set; }
            public long? intWorkplaceGroupId { get; set; }
            public long? intEmployeeId { get; set; }
            public long? intApproverId { get; set; }
            public string? strEmployeeCode { get; set; }
            public string? strEmployeeName { get; set; }
            public string? strLeaveReason { get; set; }
            public DateTime? dteAppliedFromDate { get; set; }
            public DateTime? dteAppliedToDate { get; set; }
            public string? strDocumentFile { get; set; }
            public long? intLeaveTypeId { get; set; }
            public string? strLeaveType { get; set; }
            public long? intTotalConsumeLeave { get; set; }
            public long? intRemainingDays { get; set; }
            public long? intAllTypeLeaveConsume { get; set; }

            public string? strDepartmentName { get; set; }
            public string? strDesignationName { get; set; }
            public string? strEmploymentTypeName { get; set; }
            public long? intDepartmentId { get; set; }
            public long? intDesignationId { get; set; }
            public string? strAddressDuetoLeave { get; set; }
            public DateTime? dteApplicationDate { get; set; }
            public long? totalDays { get; set; }
            public string? strStatus { get; set; }
            public string? strViewAs { get; set; }
        }

        //public class EmployeeResumeViewModel
        //{
        //    public MasterBusinessUnit TblBusinessUnit { get; set; }
        //    public TblVessel TblVessel { get; set; }
        //    public EmployeeProfileViewDTO? EmployeeProfileViewDTO { get; set; }
        //    public List<TblEmployeeEducationShipDTO>? TblEmployeeEducationShipList { get; set; }
        //    public List<SeaServiceRecordPdfDTO> SeaServiceRecordList { get; set; }
        //    public List<EmployeeDocumentsChecklistDTO> EmployeeDocumentsChecklistDTOList { get; set; }
        //    public TblBankingInfoShip TblBankingInfoShip { get; set; }

        //}

        //public class EmployeeProfileViewViewModel
        //{
        //    public TblEmployeeBasicInfo? TblEmployeeBasicInfo { get; set; }
        //    public TblShippingEmployeeBasicInfoShip? TblShippingEmployeeBasicInfoShip { get; set; }
        //    public long? CurrentRankId { get; set; }
        //    public string? CurrentRankName { get; set; }
        //    public long? AppliedRankId { get; set; }
        //    public string? AppliedRankName { get; set; }
        //    public long? DeptId { get; set; }
        //    public string? DeptName { get; set; }
        //    public long? VesselId { get; set; }
        //    public string? VesselName { get; set; }

        //}
        public class TblEmployeeEducationShipViewModel
        {
            public long DegreeId { get; set; }
            public string DegreeName { get; set; }
            public string Institute { get; set; }
            public long PassingYear { get; set; }
            public string Result { get; set; }
            public string StrInsertUserId { get; set; }
            public string StrMarineAcaBatch { get; set; }
            public string StrNmibatch { get; set; }
            public DateTime DteInsertDateTime { get; set; }

        }

        public class EmployeeDocumentsChecklistViewModel
        {
            public int? IntEmployeeDocumentsId { get; set; }
            public int? IntEmployeeId { get; set; }
            public int? IntDocumentsTypeId { get; set; }
            public string? StrDocumentsType { get; set; }
            public string? StrIssuedBy { get; set; } = null!;
            public string? StrIdentityNumber { get; set; }
            public DateTime? DteIssueDate { get; set; }
            public DateTime? DteExpireDate { get; set; }
            public bool? IsAgentCheck { get; set; }
            public bool? IsEmployerCheck { get; set; }
            public int? IntBusinessUnitId { get; set; }
            public int? IntAccountId { get; set; }
            public bool? IsActive { get; set; }
            public string? StrInsertUserId { get; set; }
            public DateTime? DteInsertDateTime { get; set; }
            public string? StrUpdateUserId { get; set; }
            public DateTime? DteUpdateDateTime { get; set; }
            public string? StrAttachment { get; set; }
        }

        public class SeaServiceRecordPdfViewModel
        {
            public string RankName { get; set; }
            public string OwnerNme { get; set; }
            public string VesselName { get; set; }
            public string Flag { get; set; }
            public string VesselType { get; set; }
            public string GRT { get; set; }
            public string EngineDetail { get; set; }
            public DateTime? SigninDate { get; set; }
            public DateTime? SignoffDate { get; set; }
            public string ServiceLength { get; set; }
            public string ReasonOfLeaving { get; set; }
            public decimal? Salary { get; set; }
        }

        public class PortageBillViewModel
        {
            public long? intSalaryGenerateRowId { get; set; }
            public long? intSalaryGenerateHeaderId { get; set; }
            public long? intEmployeeId { get; set; }
            public string? strEmployeeName { get; set; }
            public string? strRank { get; set; }
            public string? strPPNo { get; set; }
            public decimal? WagesPerMonth { get; set; }
            public int? NoOfDays { get; set; }
            public decimal? EarningOfTheMonth { get; set; }
            public decimal? PreviousBalance { get; set; }
            public decimal? AdditionalEarning { get; set; }
            public decimal TotalEarning { get; set; }
            public decimal? AdvanceOnBoard { get; set; }
            public decimal? VsatCallingCard { get; set; }
            public decimal? BondedItem { get; set; }
            public decimal? JoiningAdvance { get; set; }
            public decimal? TotalDeduction { get; set; }
            public decimal? PayableAmount { get; set; }
            public decimal? USDConversionRate { get; set; }
            public long? BusinessUnitId { get; set; }
            public long? AccountId { get; set; }

        }


        //public class CRMatrixChartViewModel
        //{
        //    public TblCremployeeHeaderShip? CREmployeeHeaderShip { get; set; }
        //    public ReportData? ReportData { get; set; }
        //    public List<CRMatrixChartHeaderDTO> CRMatrixChartHeaderDTOList { get; set; }
        //    public List<CRMatrixChartRowDTO> CRMatrixChartRowDTOList { get; set; }

        //}

        public class CRMatrixChartHeaderViewModel
        {
            public long? CRMatrixChartHeaderId { get; set; }
            public string? CRMatrixChartHeaderName { get; set; }
            public string? Value { get; set; }
            public bool IsChecked { get; set; }
            public int? ShortOrder { get; set; }

        }

        public class CRMatrixChartRowViewModel
        {
            public long? CRMatrixChartRowId { get; set; }
            public string? CRMatrixChartRowName { get; set; }
            public int? ShortOrder { get; set; }

            public List<CRMatrixChartHeaderViewModel> CRMatrixChartHeaderViewModelList { get; set; }
        }

        public class BondInventoryReportViewModel
        {
            public BondInventoryReportData? IssueData { get; set; }
            public BondInventoryReportData? ReceivedData { get; set; }
        }
        public class BondInventoryReportData
        {
            public long? ItemId { get; set; }
            public string? strItemName { get; set; }
            public string? strType { get; set; }
            public decimal? numOpenQty { get; set; }
            public decimal? numOpenRate { get; set; }
            public decimal? numOpenValue { get; set; }
            public decimal? numInQty { get; set; }
            public decimal? numInRate { get; set; }
            public decimal? numInValue { get; set; }
            public decimal? numOutQty { get; set; }
            public decimal? numOutRate { get; set; }
            public decimal? numOutValue { get; set; }
            public decimal? numBalanceQty { get; set; }
            public decimal? numBalanceRate { get; set; }
            public decimal? numBalanceValue { get; set; }

        }

        public class BondReportViewModel
        {
            public string? VesselName { get; set; }
            public string? Master { get; set; }
            public string? FromDate { get; set; }
            public string? ToDate { get; set; }
            public List<TotalReceivedInThisMonthGroupByPortSupplierNameViewModel>? TotalReceivedInThisMonthGroupByPortSupplierNameViewModelList { get; set; }
            public BondReportDataViewModel BondReportDataViewModel { get; set; }

        }

        public class TotalReceivedInThisMonthGroupByPortSupplierNameViewModel
        {
            public DateTime? Date { get; set; }
            public string? PortSupplierName { get; set; }
            public decimal? TotalAmount { get; set; }
        }

        public class BondReportDataViewModel
        {
            public decimal? numCarryForwardBalance { get; set; }
            public decimal? numTotalReceiveInThisMonth { get; set; }
            public decimal? numTotalBondedStoreOnThismonth { get; set; }
            public decimal? numTotalIssueFromOwnersAccount { get; set; }
            public decimal? numTotalIssueFromCrewNOfficer { get; set; }
            public decimal? numTotalIssueFromChartererAccount { get; set; }
            public decimal? numTotalIssueInThisMonth { get; set; }
            public decimal? numCarryForwardBalanceForNextMonth { get; set; }
        }


        public class CashAccountReportViewModel
        {
            public string? VesselName { get; set; }
            public string? Master { get; set; }
            public string? FromDate { get; set; }
            public string? ToDate { get; set; }

            public List<CashAccountReportDataViewModel>? CashAccountReportDataListViewModel { get; set; }

        }

        public class CashAccountReportDataViewModel
        {
            public DateTime Date { get; set; }
            public string? PortName { get; set; }
            public string? Rate { get; set; }
            public decimal? TotalAmount { get; set; }
            public bool? IsOpening { get; set; }
            public bool? IsCashReceived { get; set; }
            public bool? IsCashReturn { get; set; }
            public bool? IsOthers { get; set; }
        }

        public class MedicalApplicationViewModel
        {
            public long MedicalApplicationId { get; set; }
            public long? IntVesselId { get; set; }
            public string VesselName { get; set; }
            public long? PortId { get; set; }
            public string PortName { get; set; }
            public DateTime ApplicationDate { get; set; }
            public long EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Rank { get; set; }
            public string Symptoms { get; set; }
            public string MedicationGiven { get; set; }
            public string DoctorName { get; set; }
            public string Diagnosis { get; set; }
            public bool IsFitForService { get; set; }
            public bool IsMedicationGiven { get; set; }
            public bool IsRepatriation { get; set; }
            public bool IsHospitalization { get; set; }
            public bool IsAttachedByDoctor { get; set; }
            public string CheckedBy { get; set; }
            public DateTime? CheckedDate { get; set; }
            public long? MsgNo { get; set; }
            public bool IsActive { get; set; }
            public bool IsApprovedByCheifEng { get; set; }
            public bool? IsApproveByMaster { get; set; }
            public string InsertUserId { get; set; }
            public DateTime InsertDateTime { get; set; }
            public string UpdateUserId { get; set; }
            public DateTime? UpdateDateTime { get; set; }
            public string? Status { get; set; }
        }

        public class SignOffViewModel
        {
            public long SignOffApplicationId { get; set; }
            public long VesselId { get; set; }
            public string VesselName { get; set; }
            public DateTime ApplicationDate { get; set; }
            public long EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public long? RankId { get; set; }
            public string RankName { get; set; }
            public string ApplicationTo { get; set; }
            public string Description { get; set; }
            public DateTime? FromDate { get; set; }
            public long? PortId { get; set; }
            public string PortName { get; set; }
            public DateTime? TenureDate { get; set; }
            public DateTime? CompletionDate { get; set; }
            public DateTime? NoticeGivenDate { get; set; }
            public string ExtensionRequest { get; set; }
            public DateTime? RejoiningDate { get; set; }
            public string RemarksFromMaster { get; set; }
            public string RemarksFromEmployee { get; set; }
            public long MasterId { get; set; }
            public string MasterName { get; set; }
            public long CheifEngineerId { get; set; }
            public string CheifEngineerName { get; set; }
            public bool IsApprovedByCheifEng { get; set; }
            public bool IsApprovedByMaster { get; set; }
            public bool IsActive { get; set; }
            public string InsertUserId { get; set; }
            public DateTime InsertDateTime { get; set; }
            public string UpdateUserId { get; set; }
            public DateTime? UpdateDateTime { get; set; }
            public string? Status { get; set; }
        }

        public class ImoCrewReporViewModel
        {
            public long VesselId { get; set; }
            public string VesselName { get; set; }
            public long MasterId { get; set; }
            public string MasterName { get; set; }
            public string Nationality { get; set; }
            public DateTime? Date { get; set; }

            public List<ImoCrewEmployeeListViewModel> ImoCrewEmployeeList { get; set; }
        }


        public class ImoCrewEmployeeListViewModel
        {
            public long? EmployeeId { get; set; }
            public string? EmployeeName { get; set; }
            public string? Gender { get; set; }
            public string? Rank { get; set; }
            public string? Nationality { get; set; }
            public string? PasportNo { get; set; }
            public DateTime? DateOfExpiryPass { get; set; }
            public string? SeamanBookNo { get; set; }
            public DateTime? DateOfExpirySeamanBookNo { get; set; }
            public string? PlaceOfBirth { get; set; }
            public string? PlaceofJoining { get; set; }
            public DateTime? DateOfJoining { get; set; }
            public DateTime? DateOfBirth { get; set; }



        }

        //public class PaySlipReportViewModel
        //{
        //    public string Employee { get; set; }
        //    public string Designation { get; set; }
        //    public string Department { get; set; }
        //    [DisplayName("Employment Type")]
        //    public string EmploymentType { get; set; }
        //    [DisplayName("Date of Joining")]
        //    public string DateOfJoining { get; set; }
        //    [DisplayName("Bank Name")]
        //    public string BankName { get; set; }
        //    [DisplayName("Branch Name")]
        //    public string BranchName { get; set; }
        //    [DisplayName("Bank Account Name")]
        //    public string BankAccountName { get; set; }
        //    [DisplayName("Account Number")]
        //    public string AccountNumber { get; set; }
        //    [DisplayName("Routing Number")]
        //    public string RoutingNumber { get; set; }
        //    [DisplayName("Working Days")]
        //    public string WorkingDays { get; set; }
        //    public string Present { get; set; }
        //    public string Absent { get; set; }
        //    public string Late { get; set; }
        //    public string Movement { get; set; }
        //    [DisplayName("Sick Leave")]
        //    public string SickLeave { get; set; }
        //    [DisplayName("Casual Leave")]
        //    public string CasualLeave { get; set; }
        //    [DisplayName("Earned Leave")]
        //    public string EarnedLeave { get; set; }
        //    [DisplayName("Leave Without Pay")]
        //    public string LeaveWithoutPay { get; set; }
        //    [DisplayName("Off Day")]
        //    public string OffDay { get; set; }
        //    public string Holiday { get; set; }
        //    public string Basic { get; set; }
        //    [DisplayName("House Allowance")]
        //    public string HouseAllowance { get; set; }
        //    [DisplayName("Medical Allowance")]
        //    public string MedicalAllowance { get; set; }
        //    [DisplayName("Transport Allowance")]
        //    public string TransportAllowance { get; set; }
        //    [DisplayName("Provident Fund")]
        //    public string ProvidentFund { get; set; }
        //    [DisplayName("Gratuity Fund")]
        //    public string GratuityFund { get; set; }
        //    [DisplayName("Net Payable Salary")]
        //    public string NetPayableSalary { get; set; }
        //}

        //public class LoanApplicationByAdvanceFilterViewModel
        //{
        //    public long? BusinessUnitId { get; set; }
        //    public long? LoanTypeId { get; set; }
        //    public long? DepartmentId { get; set; }
        //    public long? DesignationId { get; set; }
        //    public long? EmployeeId { get; set; }
        //    public string? FromDate { get; set; }
        //    public string? ToDate { get; set; }
        //    public decimal? MinimumAmount { get; set; }
        //    public decimal? MaximumAmount { get; set; }
        //    public string? ApplicationStatus { get; set; }
        //    public string? InstallmentStatus { get; set; }
        //}

        //public class LoanReportByAdvanceFilterViewModel
        //{
        //    public long? LoanApplicationId { get; set; }
        //    public long? EmployeeId { get; set; }
        //    public long? LoanTypeId { get; set; }
        //    public long? LoanAmount { get; set; }
        //    public long? NumberOfInstallment { get; set; }
        //    public long? NumberOfInstallmentAmount { get; set; }
        //    public DateTime? ApplicationDate { get; set; }
        //    public bool? IsApprove { get; set; }
        //    public string? ApproveBy { get; set; }
        //    public DateTime? ApproveDate { get; set; }
        //    public long? ApproveLoanAmount { get; set; }
        //    public long? ApproveNumberOfInstallment { get; set; }
        //    public decimal? RemainingBalance { get; set; }
        //    public DateTime? EffectiveDate { get; set; }
        //    public string? Description { get; set; }
        //    public string? FileUrl { get; set; }
        //    public string? ReferenceNo { get; set; }
        //    public bool? isActive { get; set; }
        //    public bool? isReject { get; set; }
        //    public string? RejectBy { get; set; }
        //    public DateTime? RejectDate { get; set; }
        //    public string? InsertByUserId { get; set; }
        //    public DateTime? InsertDateTime { get; set; }
        //    public string? UpdateByUserId { get; set; }
        //    public DateTime? UpdateDateTime { get; set; }
        //    public string? EmployeeName { get; set; }
        //    public string? EmployeeCode { get; set; }
        //    public string? DepartmentName { get; set; }
        //    public string? DesignationName { get; set; }
        //    public string? PositonGroupName { get; set; }
        //    public string? PositionName { get; set; }
        //    public string? LoanType { get; set; }
        //    public string? ApplicationStatus { get; set; }
        //    public string? InstallmentStatus { get; set; }

        //}

        public class LeaveBalanceHistoryForAllEmployeeViewModel
        {

            public long? EmployeeId { get; set; }
            public string? Employee { get; set; }
            public string? Department { get; set; }
            public string? Designation { get; set; }
            public long? CLBalance { get; set; }
            public long? CLTaken { get; set; }
            public long? SLBalance { get; set; }
            public long? SLTaken { get; set; }
            public long? ELBalance { get; set; }
            public long? ELTaken { get; set; }
            public long? LWPBalance { get; set; }
            public long? LWPTaken { get; set; }
            public long? MLBalance { get; set; }
            public long? MLTaken { get; set; }

        }

        public class EmployeeListExportAsExcelViewModel
        {

            public DataTable dataTable { set; get; }
            [Required]
            [Range(1, 10)]
            public int leftSidePeddingCell { set; get; } = 1;
            [Required]
            [Range(5, 10)]
            public int topPeddingCell { set; get; } = 5;
            public string? SheetTitle { set; get; }
            public Stream? OrgLogo { set; get; }
            public string? TitleLine_One { set; get; }
            public string? TitleLine_Two { set; get; }
            public string? TitleLine_Three { set; get; }
            public string? date { set; get; }
        }

        public class EmployeeSalaryPaySlipViewModel
        {
            public string? EmployeeId { get; set; }
            public string? GeneratDate { set; get; }
            public string? Employee { get; set; }
            public string? EmployeeCode { get; set; }
            public string? EmploymentType { get; set; }
            public string? HRPostionName { set; get; }
            public string? DepartmentId { get; set; }
            public string? Department { get; set; }
            public string? DesignationId { get; set; }
            public string? Designation { get; set; }
            public string? JoiningDate { get; set; }
            public string? ModeOfPayment { get; set; }
            public string? DateOfPayment { get; set; }
            public string? BusinessUnitName { get; set; }
            public decimal? numTaxAmount { get; set; }
            public decimal? numLoanAmount { get; set; }
            public decimal? numPFAmount { get; set; }
            public decimal? numPFCompanyAmount { get; set; }
            public long? AccountId { get; set; }
            public string? strFinancialInstitution { get; set; }
            public string? strBankBranchName { get; set; }
            public string? strAccountName { get; set; }
            public string? strAccountNo { get; set; }
            public List<PaySlipViewModel> paySlipViewModels { get; set; }

        }
        public class PaySlipViewModel
        {
            public string? PayrollElementId { get; set; }
            public string? PayrollElement { get; set; }
            public string? PayrollElementTypeId { get; set; }
            public decimal? NumAmount { get; set; }
            public decimal? Arrear { get; set; }
            public decimal? Total { get; set; }
        }
        public class SalaryAllReportViewModel
        {
            public long? BusinessUnitId { get; set; }
            public string? BusinessUnit { get; set; }
            public string? Address { get; set; }
            public DateTime? GenerateDate { get; set; }
            public long? IntBankOrWalletType { get; set; }
            public List<SalaryReportHeaderViewModel> salaryReportHeaderViewModels { get; set; }
        }
        public class SalaryReportHeaderViewModel
        {
            public long? EmployeeId { get; set; }
            public string? EmployeeName { get; set; }
            public string? EmployeeCode { get; set; }
            public decimal? Salary { get; set; }
            public decimal? TotalAllowance { get; set; }
            public decimal? TotalDecuction { get; set; }
            public string? AccountName { get; set; }
            public string? BankName { get; set; }
            public string? Branch { get; set; }
            public string? AccountNo { get; set; }
            public string? RoutingNo { get; set; }
            public decimal? Netpay { get; set; }
            public decimal? BankPay { get; set; }
            public decimal? DigitalBank { get; set; }
            public decimal? CashPay { get; set; }
            public string WorkplaceGroupe { get; set; }
            public string Workplace { get; set; }
            public string PayrollGroupe { get; set; }
            public long? BusinessUnitId { get; set; }
            public string? BusinessUnit { get; set; }
            public string? Address { get; set; }
            public long? IntBankOrWalletType { get; set; }
        }

        public class BankAdviceViewModel
        {
            public DateTime? GeneratDate { get; set; }
            public string? CompanyBank { get; set; }
            public string? BankBranch { get; set; }
            public string? BankAddress { get; set; }
            public string? AdviceName { get; set; }
            public string? AdviceTo { get; set; }
            public string? BankAccountNo { get; set; }
            public decimal? TotalCreditAmount { get; set; }
            public long? TotalTransactions { get; set; }
            public long? TotalAttachment { get; set; }
            public string? BusinessUnit { get; set; }
        }
    }
}
