using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Services.Helper.Interfaces
{
    public interface IConvertEntityToViewModelService
    {
        Task<EmployeeBasicInfoViewModel> ConvertEmployeeBasicInfo(EmpEmployeeBasicInfo obj);
    }
}
