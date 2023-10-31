using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Services.Helper.Interfaces
{
    public interface IDownloadExcel
    {

        Task<IActionResult> GetDailyCafeteriaReport(List<GetDailyCafeteriaReportViewModel> dt);
        Task<IActionResult> MonthlyCafeteriaReport(List<GetDailyCafeteriaReportViewModel> dt);


    }



}
