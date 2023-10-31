using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.Attendance;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Controllers.SAAS.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private DataTable dt = new DataTable();
        private readonly PeopleDeskContext _context;
        private readonly IApprovalPipelineService _approvalPipelineService;

        public TimeSheetController(PeopleDeskContext _context, IApprovalPipelineService _approvalPipelineService, IEmployeeService _employeeService)
        {
            this._employeeService = _employeeService;
            this._approvalPipelineService = _approvalPipelineService;
            this._context = _context;
        }

        [HttpPost]
        [Route("TimeSheetCRUD")]
        public async Task<IActionResult> TimeSheetCRUD(TimeSheetViewModel obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)obj.BusinessUnitId }, PermissionLebelCheck.BusinessUnit);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                obj.AccountId = tokenData.accountId;

                var msg = await _employeeService.TimeSheetCRUD(obj);

                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("RemoteAttendanceRegistration")]
        public async Task<IActionResult> RemoteAttendanceRegistration(RemoteAttendanceRegistrationViewModel obj)
        {
            try
            {
                var msg = await _employeeService.RemoteAttendanceRegistration(obj);

                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("RemoteAttendance")]
        public async Task<IActionResult> RemoteAttendance(RemoteAttendanceViewModel obj)
        {
            try
            {
                var msg = await _employeeService.RemoteAttendance(obj);

                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #region===== master location register============

        [HttpPost("MasterLocationRegistration")]
        public async Task<IActionResult> MasterLocationRegistrationAsync(MasterLocationRegisterDTO model)
        {
            try
            {
                MessageHelper msg = await _employeeService.MasterLocationRegistrationAsync(model);
                return msg.StatusCode == 200 ? Ok(msg) : BadRequest(msg);
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpGet("GetMasterLocationByAccountId")]
        public async Task<IActionResult> GetMasterLocationByAccountIdAsync(long AcccountId, long BusinessUnitId)
        {
            try
            {
                return Ok(await _employeeService.GetMasterLocationByAccountIdAsync(AcccountId, BusinessUnitId));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        #endregion

        #region Location assaign

        [HttpGet("GetMasterLocationRegistrationDdlById")]
        public async Task<IActionResult> MasterLocationRegistrationDDL(long AccountId, long BussinessUnit)
        {
            try
            {
                return Ok(await _employeeService.MasterLocationRegistrationDDL(AccountId, BussinessUnit));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpGet("EmployeeWiseLocation")]
        public async Task<IActionResult> EmployeeWiseLocationAsync(long AccountId, long EmployeeId)
        {
            try
            {
                return Ok(await _employeeService.EmployeeWiseLocationAsync(AccountId, EmployeeId));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpPost("CreateNUpdateEmployeeWiseLocationAssaign")]
        public async Task<IActionResult> CreateNUpdateEmployeeWiseLocationAssaignAsync(CreateNUpdateMasterLocationEployeeWise model)
        {
            try
            {
                MessageHelper msg = await _employeeService.CreateNUpdateEmployeeWiseLocationAssaignAsync(model);
                return msg.StatusCode == 200 ? Ok(msg) : BadRequest(msg);
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpGet("GetLocationWiseEmployee")]
        public async Task<IActionResult> GetLocationWiseEmployeeAsync(long LocationMasterId, long AccountId, long BusineesUint)
        {
            try
            {
                return Ok(await _employeeService.GetLocationWiseEmployeeAsync(LocationMasterId, AccountId, BusineesUint));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpPost("CreateNUpdateLocationWiseEmployee")]
        public async Task<IActionResult> CreateNUpdateLocationWiseEmployeeAsync(CreateNUpdateMasterLocationWise model)
        {
            try
            {
                return Ok(await _employeeService.CreateNUpdateLocationWiseEmployeeAsync(model));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpGet("GetLocationDashBoardByAccountId")]
        public async Task<IActionResult> GetLocationDashBoardByAccountId(long IntAccountId, long IntBusinessUnitId)
        {
            try
            {
                return Ok(await _employeeService.GetLocationDashBoardByAccountId(IntAccountId, IntBusinessUnitId));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }

        [HttpGet("GetEmployeeListLocationBasedByAccountId")]
        public async Task<IActionResult> GetEmployeeListLocationBased(long IntAccountId, long IntBusinessUnitId)
        {
            try
            {
                return Ok(await _employeeService.GetEmployeeListLocationBased(IntAccountId, IntBusinessUnitId));
            }
            catch (Exception ex)
            {
                MessageHelper msg = new()
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    AutoId = 0
                };
                return BadRequest(msg);
            }
        }
        #endregion

        #region=== Master Fixed Roaster===

        [HttpGet("GetFixedRoasterMasterById")]
        public async Task<IActionResult> GetFixedRoasterMasterByIdAsync(long intAccountId, long intBusinessId)
        {
            try
            {
                return Ok(await _employeeService.GetFixedRoasterMasterByIdAsync(intAccountId, intBusinessId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet("GetFixedRoasterDetaisById")]
        public async Task<IActionResult> GetFixedRoasterDetaisByIdAsync(long intFixedMasterId)
        {
            try
            {
                return Ok(await _employeeService.GetFixedRoasterDetaisByIdAsync(intFixedMasterId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }


        [HttpPost("CreateNUpdateFixedRoaster")]
        public async Task<IActionResult> CreateNUpdateFixedRoasterAsync(FixedMasterRoaster model)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                MessageHelper msg = new();
                msg.StatusCode = 400;
                msg.Message = "failed";
                long res = await _employeeService.CreateNUpdateFixedRoasterMasterAsync(model);
                if (res > 0 && model.FixedRoasterDetails.Count > 0)
                {
                    model.FixedRoasterDetails.ForEach(x => x.MasterId = res);
                    model.FixedRoasterDetails.ForEach(x => x.IntActionBy = model.IntActionBy);
                    msg = await _employeeService.CreateNUpdateFixedRoasterDeatailsAsync(model.FixedRoasterDetails);
                    if (msg.StatusCode == 200)
                    {
                        await transaction.CommitAsync();
                        return Ok(msg);
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(msg);
                    }

                }
                else if (res > 0 && model.FixedRoasterDetails.Count <= 0)
                {
                    msg.StatusCode = 200;
                    msg.Message = "Success";

                    await transaction.CommitAsync();
                    return Ok(msg);
                }
                await transaction.RollbackAsync();
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion

        #region ===== Attendance Process ====
        [HttpPost]
        [Route("TimeAttendanceProcess")]
        public async Task<IActionResult> TimeAttendanceProcess(TimeAttendanceProcessViewModel obj)
        {
            try
            {
                var result = await _employeeService.TimeAttendanceProcess(obj);
                if (result.StatusCode == 200)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetTimeAttendanceProcess")]
        public async Task<IActionResult> GetTimeAttendanceProcess(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var result = await _employeeService.GetTimeAttendanceProcess(FromDate, ToDate);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }
        }
        #endregion
    }
}