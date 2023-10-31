using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Services.SignalR.Interfaces;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly PeopleDeskContext _context;

        public NotificationController(INotificationService _notificationService, PeopleDeskContext _context)
        {
            this._context = _context;
            this._notificationService = _notificationService;
        }

        [Route("GetAllNotificationByUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllNotificationByUser(int pageNo, int pageSize, long employeeId, long accountId)
        {
            try
            {
                var data = await _notificationService.GetAllSeenNUnSeenNotificationByUser(pageNo, pageSize, employeeId, accountId);
                return Ok(data);
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }

        [Route("GetAllUnseenNotificationByUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllUnseenNotificationByUser(string username)
        {
            try
            {
                var data = await _notificationService.GetAllUnSeenNotificationByUser(username);
                return Ok(data);
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }


        [Route("GetNotificationCount")]
        [HttpGet]
        public async Task<IActionResult> GetNotificationCount(long employeeId, long accountId)
        {
            try
            {
                var data = await _notificationService.GetNotificationCount(employeeId, accountId);
                return Ok(data.Count());
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }

        [Route("RealTimeNotificationTestToAllUser")]
        [HttpGet]
        public async Task<IActionResult> RealTimeNotificationTestToAllUser(long? orgId)
        {
            try
            {
                var data = await _notificationService.SendToGlobalAll();
                return Ok("success");
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }

        [Route("RealTimeNotificationTestSendToOrdBased")]
        [HttpGet]
        public async Task<IActionResult> RealTimeNotificationTestSendToOrdBased(long? orgId)
        {
            try
            {
                var data = await _notificationService.SendToAllUserByOrgId(orgId);
                return Ok("success");
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }

        [Route("RealTimeNotificationTestSendToSingleUser")]
        [HttpGet]
        public async Task<IActionResult> RealTimeNotificationTestSendToSingleUser(string accountId, string username)
        {
            try
            {
                var data = await _notificationService.SendToSingleUserByUsername(accountId, username);
                return Ok("success");
            }
            catch (Exception)
            {
                throw new Exception("internal server error occur");
            }
        }


        #region =============================== PUSH NOTIFICATION =================================
        //=========================================================================================



        #endregion
    }
}
