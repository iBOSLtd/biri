using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Helper;
using PeopleDesk.Services.SAAS.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PeopleDesk.Controllers.SAAS.Cafeteria
{
    [Route("api/[controller]")]
    [ApiController]
    public class CafeteriaController : ControllerBase
    {
        DataTable dt = new DataTable();
        private readonly PeopleDeskContext _context;
        private readonly ICafeteriaService _cafeteriaService;
        public CafeteriaController(ICafeteriaService _cafeteriaService)
        {
            this._cafeteriaService = _cafeteriaService;
        }

        #region ========= Food Corner =============

        [HttpGet]
        [Route("GetCafeteriaMenuListReport")]
        public async Task<IActionResult> GetCafeteriaMenuListReport([Required] long LoginBy)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(await _cafeteriaService.GetCafeteriaMenuListReport(LoginBy));
                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("CafeteriaEntry")]
        public async Task<IActionResult> CafeteriaEntry([Required] long PartId, [Required] DateTime ToDate, [Required] long EnrollId, [Required] long TypeId, [Required] long MealOption, [Required] long MealFor
           , [Required] long CountMeal, [Required] long isOwnGuest, [Required] long isPayable, string? Narration, [Required] long ActionBy)
        {
            try
            {
                var res = await _cafeteriaService.CafeteriaEntry(PartId, ToDate, EnrollId, TypeId, MealOption, MealFor, CountMeal, isOwnGuest, isPayable, Narration, ActionBy);

                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetPendingAndConsumeMealReport")]
        public async Task<IActionResult> GetPendingAndConsumeMealReport([Required] long PartId, [Required] long EnrollId)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(await _cafeteriaService.GetPendingAndConsumeMealReport(PartId, EnrollId));
                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [Route("GetDailyCafeteriaReport")]
        public async Task<IActionResult> GetDailyCafeteriaReport([Required] long businessUnitId, [Required] DateTime mealDate, bool isDownload)
        {
            try
            {
                var result = await _cafeteriaService.GetDailyCafeteriaReport(businessUnitId, mealDate);

                if (isDownload == true)
                {
                    return await DownloadExcel.GetDailyCafeteriaReport(result);

                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("MonthlyCafeteriaReport")]
        public async Task<IActionResult> MonthlyCafeteriaReport([Required] long businessUnitId, long workPlaceId, DateTime fromDate, DateTime toDate, bool isDownload)
        {
            try
            {
                var result = await _cafeteriaService.MonthlyCafeteriaReport(businessUnitId, workPlaceId, fromDate, toDate);

                if (isDownload == true)
                {
                    return await DownloadExcel.MonthlyCafeteriaReport(result);

                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("EditCafeteriaMenuList")]
        public async Task<IActionResult> EditCafeteriaMenuList(MenuListOfFoodCornerEditCommonViewModel obj)
        {
            try
            {
                var res = await _cafeteriaService.EditCafeteriaMenuList(obj);

                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
