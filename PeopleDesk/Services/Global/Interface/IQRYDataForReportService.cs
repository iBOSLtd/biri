using PeopleDesk.Models.Employee;

namespace PeopleDesk.Services.Global.Interface
{
    public interface IQRYDataForReportService
    {
        Task<EmployeeQryProfileAllViewModel> EmployeeQryProfileAll(long intEmployeeId);
        Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllList(long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? EmployeeId);
        Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllListBySupervisorORLineManagerId(long? SupervisorORLineManagerEmployeeId);
    }
}
