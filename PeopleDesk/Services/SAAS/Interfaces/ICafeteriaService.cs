using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface ICafeteriaService
    {
        #region ============= Food Corner ===============
        Task<DataTable> GetCafeteriaMenuListReport(long LoginBy);
        Task<MessageHelper> CafeteriaEntry(long PartId, DateTime ToDate, long EnrollId, long TypeId, long MealOption, long MealFor
            , long CountMeal, long isOwnGuest, long isPayable, string? Narration, long ActionBy);
        Task<DataTable> GetPendingAndConsumeMealReport(long PartId, long EnrollId);

        Task<List<GetDailyCafeteriaReportViewModel>> GetDailyCafeteriaReport([Required] long businessUnitId, [Required] DateTime mealDate);
        Task<List<GetDailyCafeteriaReportViewModel>> MonthlyCafeteriaReport([Required] long businessUnitId, long workPlaceId, DateTime fromDate, DateTime toDate);
        Task<MessageHelperUpdate> EditCafeteriaMenuList(MenuListOfFoodCornerEditCommonViewModel obj);
        #endregion
    }
}
