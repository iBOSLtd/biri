using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Controllers.SAAS.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAllLandingController : ControllerBase
    {
        private readonly IEmployeeAllLanding _employeeAllLanding;
        public EmployeeAllLandingController(IEmployeeAllLanding employeeAllLanding)
        {
            _employeeAllLanding = employeeAllLanding;
        }

        [HttpGet]
        [Route("EmployeeBasicForConfirmation")]
        public async Task<IActionResult> EmployeeBasicForConfirmation(long? BusinessUnitId, long? WorkplaceGroupId, DateTime? FromDate, DateTime? ToDate, string SearchTxt, int PageNo, int PageSize)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)BusinessUnitId, workplaceGroupId = (long)WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.isAuthorize)
                {
                    ConfirmationEmployeeLandingPaginationViewModelWithHeader confirmEmp = await _employeeAllLanding.EmployeeBasicForConfirmation(tokenData, BusinessUnitId, WorkplaceGroupId, FromDate, ToDate, SearchTxt, PageNo, PageSize);

                    return Ok(confirmEmp);
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
