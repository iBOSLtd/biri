using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
namespace PeopleDesk.Controllers.SAAS
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleDeskDDLController : Controller
    {
        DataTable dt = new DataTable();
        private IPeopleDeskDDLService _peopledeskddlservice;
        private readonly PeopleDeskContext _context;
        public PeopleDeskDDLController(IPeopleDeskDDLService _peopledeskddlservice)
        {
            this._peopledeskddlservice = _peopledeskddlservice;
        }

        [HttpGet]
        [Route("PeopleDeskAllDDL")]
        public async Task<IActionResult> PeopleDeskAllDDL(string? DDLType, long? AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? intId, string? AutoEmployeeCode, bool? IsView, int? IntMonth, long? IntYear, long? IntWorkplaceId, long? IntPayrollGroupId, long? ParentTerritoryId, string? SearchTxt)
        {
            try
            {
                /*
                 * BusinessUnit (BU) 
                 * * SBU 
                 * * * Currency 
                 * * * * Position 
                 * * * * * PositionGroup (PG) 
                 * * * * * * Workplace 
                 * * * * * * * WorkplaceGroup (WG) 
                 * * * * * * * * DocumentType (DT)
                 * * * * * * * * * Country
                 * * * * * * * * * * Religion
                 * * * * * * * * * * * AutoEmployeeCode
                 */

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
              
                if (DDLType == "WingDDL" || DDLType == "SoleDepoDDL" || DDLType == "RegionDDL" || DDLType == "AreaDDL" || DDLType == "TerritoryDDL" || DDLType == "DegreeLevel"
                     || DDLType == "BloodGroupName" || DDLType == "EmploymentType" || DDLType == "LeaveType" || DDLType == "Calender" || DDLType == "HolidayGroup"
                     || DDLType == "ExceptionOffdayGroup" || DDLType == "RosterGroup" || DDLType == "CalenderByRosterGroup" || DDLType == "Workplace" || DDLType == "SeparationType")
                {
                    if (tokenData.isAuthorize)
                    {
                        intId = tokenData.employeeId;
                        AccountId = tokenData.accountId;
                    }
                }
                else
                {
                    tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId}, PermissionLebelCheck.BusinessUnit);
                }

                if (!string.IsNullOrEmpty(DDLType))
                {
                    return Ok(JsonConvert.SerializeObject(await _peopledeskddlservice.PeopleDeskAllDDL(DDLType, tokenData.accountId, BusinessUnitId, WorkplaceGroupId, intId, AutoEmployeeCode, IsView, IntMonth, IntYear, IntWorkplaceId, IntPayrollGroupId, ParentTerritoryId, SearchTxt)));
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }
    }
}
