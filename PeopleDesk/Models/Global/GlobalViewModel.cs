using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Models.Global
{
    public class GlobalViewModel
    {
    }
    public class EmployeeDashboardViewModel
    {
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? EmploymentType { get; set; }
        public long? EmployeeProfileUrlId { get; set; }
        public string? AttendanceStatus { get; set; }
        public string? WorkingPeriod { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public long? MyPendingApplicationCount { get; set; }
        public string? ServiceLength { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string? CalendarName { get; set; }
        public TimeSpan? CalendarStartTime { get; set; }
        public TimeSpan? CalendarEndTime { get; set; }

        //As Log in
        public int? DefaultDashboardId { get; set; }
        public string userRole { get; set; }
        public List<DashboardRole> dashboardRoles { get; set; }
        //

        public string? Supervisor { get; set; }
        public long? IntSupervisorImageUrlId { set; get; }
        public string? DottedSupervisor { get; set; }
        public long? IntDottedSupervisorImageUrlId { set; get; }
        public string? LineManager { get; set; }
        public long? IntLineManagerImageUrlId { get; set; }

        // Attendance info

        public string? MonthName { get; set; }
        public long? WorkingDays { get; set; }
        public long? PresentDays { get; set; }
        public long? LateDays { get; set; }
        public long? AbsentDays { get; set; }
        public long? MovementDays { get; set; }
        public long? LeaveDays { get; set; }

        // Graph info
        public int? BalanceMaxValue { get; set; }
        public int? BalanceMinValue { get; set; }
        public int? TakenMaxValue { get; set; }
        public int? TakenMinValue { get; set; }

        public List<LeaveBalanceHistory> LeaveBalanceHistoryList { get; set; }
        public AttendanceSummaryViewModel AttendanceSummaryViewModel { get; set; }
        public List<ApplicationPendingViewModel> applicationPendingViewModels { get; set; }

    }
    public class DashboardComponent
    {
        public long? IntPermissionId { get; set; }
        public long? IntDashboardId { set; get; }
        public string StrHashCode { set; get; }
        public string StrDisplayName { set; get; }
        public long? IntForAccountOREmp { get; set; }
        public long? IntAccountId { set; get; }
        public long? IntEmployeeId { set; get; }
        public bool IsActive { set; get; }
        public DateTime? DteCreateAt { set; get; }
    }

    public class ApplicationPendingViewModel
    {
        public string? ApplicationType { set; get; }
        public DateTime? ApplicationDate { set; get; }
        public string ApprovalStatus { set; get; }
    }

    public class HourNMinutes
    {
        public long? Hours { set; get; }
        public long? Minutes { set; get; }
    }

    public class EmployeeStatusViewModel
    {
        public long? TotalEmployee { set; get; }
        public long? TotalMale { set; get; }
        public decimal? MalePercentage { set; get; }
        public long? TotalFemale { set; get; }
        public decimal? FemalePercentage { set; get; }
        public List<EmployeeStatusGraphViewModel> employeeStatusGraphs { set; get; }
    }
    public class EmployeeStatusGraphViewModel
    {
        public string GraphText { set; get; }
        public long? GraphValue { set; get; }
    }

    public class DashboardRole
    {
        public long? Value { get; set; }
        public string? Label { get; set; }
    }
    public class LeaveBalanceHistory
    {
        public int? LeaveBalanceId { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? LeaveTypeCode { get; set; }
        public string? LeaveType { get; set; }
        public int? RemainingDays { get; set; }
        public int? LeaveTakenDays { get; set; }
        public int? BalanceDays { get; set; }

    }
    public class MidLevelDashboardViewModel
    {
        public long? TodayPresent { set; get; }
        public long? TodayLate { set; get; }
        public long? TodayAbsent { set; get; }
        public long? TodayMovement { set; get; }
        public long? YesterdayMovement { set; get; }
        public long? TommorrowMovement { set; get; }
        public long? TodayLeave { set; get; }
        public long? YesterdayLeave { set; get; }
        public long? TommorrowLeave { set; get; }

        //public List<UpcomingBirthdayEmployeeViewModel> upcomingBirthdayEmployeeList { get; set; }
        public List<EmployeeAttandanceListViewModel> employeeAttandanceListViewModels { get; set; }

        public List<EmployeeQryProfileAllViewModel> EmployeeQryProfileAllList { get; set; }

    }

    public class EmployeeAttandanceListViewModel
    {
        public long EmployeeId { set; get; }
        public string EmployeeName { set; get; }
        public string EmployeeCode { set; get; }
        public long? EmployeeProfileUrlId { set; get; }
        public long? DepartmentId { set; get; }
        public string Departmant { set; get; }
        public long? DesignationId { set; get; }
        public string Designation { set; get; }
        public string? StrWorkingHours { set; get; }
        public TimeSpan? InTime { set; get; }
        public TimeSpan? OutTime { set; get; }
        public string Status { set; get; }
    }
    public class DepartmentWiseEmployeeSalaryCountViewModel
    {
        public long? DepartmentId { set; get; }
        public string? Department { set; get; }
        public long? EmployeeCount { set; get; }
        public decimal? Salary { set; get; }
    }
    public class EmployeeWithSalaryList
    {
        public EmpEmployeeBasicInfo Emp { get; set; }
        public PyrEmployeeSalaryElementAssignHeader Salary { get; set; }
    }
    public class DepartmentWiseEmployeeTurnoverRatioViewModel
    {
        public long TotalEmployee { set; get; }
        public long TotalLeft { set; get; }
        public decimal? TurnoverRate { set; get; }
        public List<DepartmentWiseTurnoverRateViewModel> departmentWiseTurnoverRateViewModel { set; get; }
    }
    public class DepartmentWiseTurnoverRateViewModel
    {
        public long DepartmentId { set; get; }
        public string DepartmentName { set; get; }
        public long? LastYearEmployee { set; get; }
        public long? CurrentYearEmployee { set; get; }
        public decimal? TurnoverRatio { set; get; }
    }
    public class MonthWiseLeaveTakenViewModel
    {
        public long? MonthId { set; get; }
        public long? LeaveCount { set; get; }
    }
    public class MonthWiseIOUViewModel
    {
        public long? MonthId { set; get; }
        public decimal? IOU { set; get; }
    }
    public class TopLevelDashboardViewModel
    {
        public TodayAttendanceViewModel todayAttendanceViewModel { get; set; }
        public LeaveStatusViewModel leaveStatusViewModel { get; set; }
        public MovementStatusViewModel movementStatusViewModel { get; set; }
        public List<UpcomingBirthdayEmployeeViewModel> upcomingBirthdayEmployeeList { get; set; }
        public List<DepartmentWiseEmployeeSalaryCountViewModel> departmentWiseEmployeeSalaryCount { set; get; }
        public List<LastFiveYearsTurnoverRatioViewModel> lastFiveYearsTurnoversViewModel { get; set; }
    }
    public class LeaveStatusViewModel
    {
        public long? TodayLeave { set; get; }
        public long? TommorrowLeave { set; get; }
        public long? YesterdayLeave { set; get; }

        //Leave
        public decimal? TodayLeavePercentage { set; get; }
        public decimal? TommorrowLeavePercentage { set; get; }
        public decimal? YesterdayLeavePercentage { set; get; }
    }
    public class MovementStatusViewModel
    {
        public long? TodayMovement { set; get; }
        public long? TommorrowMovement { set; get; }
        public long? YesterdayMovement { set; get; }

        //Movement
        public decimal? TodayMovementPercentage { set; get; }
        public decimal? TommorrowMovementPercentage { set; get; }
        public decimal? YesterdayMovementPercentage { set; get; }

    }
    public class InternNProbationPeriodViewModel
    {
        public long InternBellowThreeMonth { set; get; }
        public long InternAboveThreeMonth { set; get; }
        public long ProbationBellowSixMonth { set; get; }
        public long ProbationAboveSixMonth { set; get; }
    }
    public class TodayAttendanceViewModel
    {
        public decimal? TodayPresentPercentage { set; get; }
        public decimal? TodayLatePercentage { set; get; }
        public decimal? TodayAbsentPercentage { set; get; }

        public List<AttendanceDonutChart> AttendanceDonutChartData { get; set; }
        public long? TotalEmployeeCount { set; get; }
    }
    public class AttendanceDonutChart
    {
        public string name { get; set; }
        public long? value { get; set; }
    }
    public class DepartmentWiseSalaryGraphViewModel
    {
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public List<DepartmentWiseSalaryGraphDataViewModel> DepartmentWiseSalaryGraphDataList { get; set; }
        public List<MasterDepartment> DepartmentList { get; set; }
        public List<CustomGraphDataSet> CustomGraphDataSetList { get; set; }
        public List<CatagoriesViewModel> catagoriesViewModels { get; set; }
        public List<decimal?> datas { get; set; }
    }
    public class DepartmentWiseAgeGraphViewModel
    {
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public List<DepartmentWiseAgeGraphDataViewModel> DepartmentWiseAgeGraphDataList { get; set; }
        public List<MasterDepartment> DepartmentList { get; set; }
        public List<CustomGraphDataSet> CustomGraphDataSetList { get; set; }
        public List<CatagoriesViewModel> catagoriesViewModels { get; set; }
        public List<decimal?> datas { get; set; }

    }
    public class LastFiveYearsTurnoverRatioViewModel
    {
        public long Years { get; set; }
        public decimal? YearlyTurnover { get; set; }
    }
    public class SalaryRangeViewModel
    {
        public decimal FixedMinimumSalary { get; set; }
        public decimal FixedMaximumSalary { get; set; }
        public decimal MinimumSalary { get; set; }
        public decimal MaximumSalary { get; set; }
        public long NumberOfEmployee { get; set; }
    }
    public class MonthOfYearWiseSeparationGraphViewModel
    {
        public decimal? MaximumSeparation { get; set; }
        public decimal? MinimumSeparation { get; set; }
        public List<MonthOfYearWiseSeparationGraphDataViewModel> MonthOfYearWiseSeparationGraphDataList { get; set; }
        public List<MasterDepartment> DepartmentList { get; set; }
        public List<CustomGraphDataSet> CustomGraphDataSetList { get; set; }

    }
    public class SalaryAndOvertimeGraphViewModel
    {
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public List<CatagoriesViewModel> catagoriesViewModels { get; set; }
        public List<decimal?> datas { get; set; }
        public List<decimal?> datas2 { get; set; }

    }
    public class EmploymentTypeWiseEmployeeListViewModel
    {
        public string? EmploymentTypeName { get; set; }
        public long? EmployeeCount { get; set; }

    }
    public class UpcomingBirthdayEmployeeViewModel
    {
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Supervisor { get; set; }
        public string? LineManager { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long? ProfileUrl { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }

    }
    public class DepartmentWiseSalaryGraphDataViewModel
    {
        public string? DepartmentName { get; set; }
        public decimal? DepartmentBaseMaxSalary { get; set; }
        public decimal? DepartmentBaseMinSalary { get; set; }
        public List<decimal?> SalaryRangeList { get; set; }
        public decimal? DeptBasedAvarageSalary { get; set; }
        public long? DeptBasedNumberOfEmployeeCount { get; set; }
    }
    public class CustomGraphDataSet
    {
        public string? title { get; set; }
        public List<decimal?> datas { get; set; }
    }
    public class CatagoriesViewModel
    {
        public long? index { get; set; }
        public string? name { get; set; }
    }

    public class DatasViewModel
    {
        public long? index { get; set; }
        public decimal? value { get; set; }
    }
    public class MonthOfYearWiseSeparationGraphDataViewModel
    {
        public string? MonthName { get; set; }
        public int? MonthId { get; set; }
        public long? NumberOfEmployee { get; set; }

    }
    public class DepartmentWiseAgeGraphDataViewModel
    {
        public string? DepartmentName { get; set; }
        public int DepartmentBaseMaxAge { get; set; }
        public int DepartmentBaseMinAge { get; set; }
        public List<int> AgeRangeList { get; set; }
        public int DeptBasedNumberOfEmployeeCount { get; set; }
        public decimal DepartmentBaseAvgAge { get; set; }

    }
    public class DashboardViewModel
    {
        public EmployeeDashboardViewModel EmployeeDashboardViewModel { get; set; }
        public MidLevelDashboardViewModel MidLevelDashboardViewModel { get; set; }
        public TopLevelDashboardViewModel TopLevelDashboardViewModel { get; set; }

    }
    public class MonthViewModel
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; }
    }

    public class ManagementDashboardViewModel
    {
        public long IntAccountId { get; set; }
        public string? AccountName { get; set; }
        public long IntBusinessUnitId { get; set; }
        public string? BusinessUnitName { get; set; }
        public long? IntCreateBy { get; set; }
        public List<EmployeeManagementPermission> EmployeeManagementPermissionList { get; set; }
    }
    public class EmployeeManagementPermission
    {
        public long? intEmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeType { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public bool? IsChecked { get; set; }
    }

    public class SendMail
    {
        public string SendTo { get; set; }
        public string SendBy { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public bool? IsHtmlFormat { get; set; }
    }
}
