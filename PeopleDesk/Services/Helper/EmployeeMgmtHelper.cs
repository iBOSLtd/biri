namespace PeopleDesk.Services.Helper
{
    public class EmployeeMgmtHelper
    {
        //public static async Task<string> GetDashboardUserRole(EmpEmployeeBasicInfo? EmployeeBasicInfo)
        //{
        //    string role = "";
        //    if (EmployeeBasicInfo != null)
        //    {
        //        role = ((EmployeeBasicInfo?.IntIsSupOrmanager == 1 || EmployeeBasicInfo?.IntIsSupOrmanager == 2) ? "Supervisor"
        //            : (string.IsNullOrEmpty(Convert.ToString(EmployeeBasicInfo?.IntIsSupOrmanager)) || EmployeeBasicInfo?.IntIsSupOrmanager == 0) ? "Employee"
        //            : EmployeeBasicInfo.IntIsSupOrmanager == 3 ? "Management" : "");
        //    }

        //    return role;
        //}
        //public static async Task<List<DashboardRole>> GetDashboardRole(EmpEmployeeBasicInfo? EmployeeBasicInfo)
        //{
        //    var dRole = new List<DashboardRole>();

        //    if (EmployeeBasicInfo != null)
        //    {
        //        dRole.Add(new DashboardRole() { Label = "Employee", Value = 1 });

        //        if (EmployeeBasicInfo?.IntIsSupOrmanager == 1 || EmployeeBasicInfo?.IntIsSupOrmanager == 2)
        //        {
        //            dRole.Add(new DashboardRole() { Label = "Supervisor", Value = 2 });
        //        }
        //        if (EmployeeBasicInfo?.IntIsSupOrmanager == 3)
        //        {
        //            dRole.Add(new DashboardRole() { Label = "Management ", Value = 3 });
        //        }
        //        if (EmployeeBasicInfo?.IntIsSupOrmanager == 999)
        //        {
        //            dRole.Add(new DashboardRole() { Label = "Supervisor", Value = 2 });
        //            dRole.Add(new DashboardRole() { Label = "Management ", Value = 3 });
        //        }
        //    }

        //    return dRole;
        //}
    }
}
