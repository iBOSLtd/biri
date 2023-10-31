using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using System.Data;

namespace PeopleDesk.Services.Global.Interface
{
    public interface IDashboardService
    {
        Task<EmployeeDashboardViewModel> EmployeeDashboard(long EmployeeId, long? BusinessUnitId);
        Task<MidLevelDashboardViewModel> MidLevelDashboard(long EmployeeId, long AccountId);
        Task<TopLevelDashboardViewModel> TopLevelDashboard(long? EmployeeId, long AccountId, long BusinessUnitId, long WorkplaceGroupId);
        //Task<List<DashboardComponent>> DashboardComponentViewlanding(long? IntAccountId, long? IntEmployeeId);
        //Task<MessageHelper> DashboardComponentShowHide(DashboardComponent component);
        Task<DataTable> ConvertTimeToHours(DateTime? date, long IntAccountId);
        Task<List<ApplicationPendingViewModel>> EmployeeApplicationPendingList(long IntEmployeeId);

        Task<List<EmployeeAttandanceListViewModel>> EmployeeTodayAttandanceList(long IntAccountId, long EmployeeId);
        Task<EmployeeStatusViewModel> EmployeeStatusGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<AttendanceSummaryViewModel> GetAttendanceSummaryCalenderReport(long EmployeeId, long Month, long Year);
        Task<List<DepartmentWiseEmployeeSalaryCountViewModel>> DepartmentWiseEmployeeSalary(long IntAccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<List<MonthWiseLeaveTakenViewModel>> MonthWiseLeaveTakenGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<List<MonthWiseIOUViewModel>> MonthWiseIOUGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<DepartmentWiseEmployeeTurnoverRatioViewModel> EmployeeTurnOverRatio(long IntAccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<List<LastFiveYearsTurnoverRatioViewModel>> LastFiveYearsTurnoverRatio(long AccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<(decimal, decimal, decimal)> LastYearEmployeeTrunOverRatio(long AccountId, long deptId, long BusinessUnitId, long WorkplaceGroupId);
        Task<SalaryRangeViewModel> EmployeeCountBySalaryRange(long AccountId, long? MinSalary, long? MaxSalary, long BusinessUnitId, long WorkplaceGroupId);
        Task<List<UpcomingBirthdayEmployeeViewModel>> UpcomingBirthday(long AccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<TodayAttendanceViewModel> AttendanceGraphData(long AccountId, int intDay, long WorkplaceGroupId);
        Task<LeaveStatusViewModel> LeaveGraphData(long AccountId, long BusinessUnitId, long WorkplaceGroupId);
        Task<MovementStatusViewModel> MovementGraphData(long AccountId, long BusinessUnitId, long WorkplaceGroupId);

        #region ================== GRAPH DATA ALL ============================
        Task<DepartmentWiseSalaryGraphViewModel> DepartmentWiseSalaryGraph(long? BusinessUnitId);
        Task<DepartmentWiseAgeGraphViewModel> DepartmentWiseAgeGraph(long? BusinessUnitId);
        Task<MonthOfYearWiseSeparationGraphViewModel> MonthOfYearWiseSeparationGraph(long? BusinessUnitId, int? Year);
        Task<InternNProbationPeriodViewModel> InternNProbationPeriodGraphData(long AccountId, int? Year, long BusinessUnitId, long WorkplaceGroupId);
        #endregion

        #region  ========= Management Dashboard Permission ========
        Task<List<ManagementDashboardViewModel>> ManagementDashboardPermission(long EmployeeId);
        Task<List<EmployeeManagementPermission>> ManagementDashboardPermissionByAccount(long IntAccountId, long IntBusinessUnitId);
        Task<MessageHelper> ManagementDashboardPermissionCRUD(ManagementDashboardViewModel dashboard);
        #endregion
    }
}
