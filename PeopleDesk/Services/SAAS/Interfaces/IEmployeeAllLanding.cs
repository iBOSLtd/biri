using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Models.Employee;
using System.Data;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IEmployeeAllLanding
    {
        Task<ConfirmationEmployeeLandingPaginationViewModelWithHeader> EmployeeBasicForConfirmation(BaseVM tokenData, long? BusinessUnitId, long? WorkplaceGroupId, DateTime? FromDate, DateTime? ToDate, string? SearchTxt, int PageNo, int PageSize);
        Task<TimeSheetEmpAttenReportLanding> GetEmpAttendanceReport(long AccountId, long IntBusinessUnitId, long IntWorkplaceGroupId, DateTime FromDate, DateTime ToDate, bool IsXls, int PageNo, int PageSize, string? searchTxt);
    }
}
