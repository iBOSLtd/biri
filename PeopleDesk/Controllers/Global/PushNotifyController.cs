using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.PushNotify;
using PeopleDesk.Services.PushNotify.Interfaces;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotifyController : ControllerBase
    {
        private readonly IPushNotifyService _pushNotifyServices;
        private readonly PeopleDeskContext _context;
        public PushNotifyController(PeopleDeskContext _context, IPushNotifyService _pushNotifyServices)
        {
            this._pushNotifyServices = _pushNotifyServices;
            this._context = _context;
        }

        [Route("SendPushNotification")]
        [HttpPost]
        public async Task<IActionResult> SendNotification(PushNotificationViewModel notificationModel)
        {
            var result = await _pushNotifyServices.SendNotification(notificationModel);
            return Ok(result);
        }

        [Route("PushNotifyDeviceRegistration")]
        [HttpPost]
        public async Task<IActionResult> PushNotifyDeviceRegistration(PushNotifyDeviceRegistration deviceRegistration)
        {
            MessageHelperCreate res = new MessageHelperCreate
            {
                StatusCode = 500,
                Message = "error",
                AutoId = 0
            };

            try
            {
                if (deviceRegistration.IntEmployeeId > 0)
                {
                    List<PushNotifyDeviceRegistration> pushNotifyDevice = await _context.PushNotifyDeviceRegistrations.Where(x => x.IntEmployeeId == deviceRegistration.IntEmployeeId && x.StrDeviceId == deviceRegistration.StrDeviceId && x.IsActive == true).ToListAsync();

                    if (pushNotifyDevice.Count() <= 0)
                    {
                        res.AutoId = await _pushNotifyServices.SavePushNotifyDeviceRegistration(deviceRegistration);
                        res.Message = "Device Registration Successfully";
                        res.StatusCode = 200;
                    }
                    else if (pushNotifyDevice.Count() > 1)
                    {
                        pushNotifyDevice = pushNotifyDevice.OrderBy(x => x.IntId).SkipLast(1).ToList();
                        pushNotifyDevice.ForEach(x =>
                        {
                            x.IsActive = false;
                        });

                        _context.PushNotifyDeviceRegistrations.UpdateRange(pushNotifyDevice);
                        await _context.SaveChangesAsync();

                        res.AutoId = 0;
                        res.Message = "Device Registration Successfully";
                        res.StatusCode = 200;
                    }
                    else
                    {
                        res.AutoId = pushNotifyDevice.FirstOrDefault().IntId;
                        res.Message = "Device Registration Successfully";
                        res.StatusCode = 200;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }

            return Ok(res);
        }

        [Route("GetAllPushNotifyDeviceRegistrationByEmployeeId")]
        [HttpGet]
        public async Task<IActionResult> GetAllPushNotifyDeviceRegistrationByEmployeeId(long employeeId)
        {
            try
            {
                return Ok(await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId(employeeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetAllPushNotifyDeviceRegistrationByDeviceId")]
        [HttpGet]
        public async Task<IActionResult> GetAllPushNotifyDeviceRegistrationByDeviceId(string deviceId)
        {
            try
            {
                return Ok(await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByDeviceId(deviceId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("DeletePushNotifyDeviceRegistration")]
        [HttpGet]
        public async Task<IActionResult> DeletePushNotifyDeviceRegistration(string id)
        {
            MessageHelperCreate res = new MessageHelperCreate
            {
                StatusCode = 500,
                Message = "error",
                AutoId = 0
            };

            try
            {
                bool response = await _pushNotifyServices.DeletePushNotifyDeviceRegistrationById(id);
                if (response)
                {
                    res.Message = "Deleted Successfully";
                    res.StatusCode = 200;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }

            return Ok(res);
        }

        [Route("DeletePushNotifyDeviceRegistrationByDeviceIdNEmpId")]
        [HttpGet]
        public async Task<IActionResult> DeletePushNotifyDeviceRegistrationByDeviceIdNEmpId(string deviceId, long empId)
        {
            MessageHelperCreate res = new MessageHelperCreate
            {
                StatusCode = 500,
                Message = "error",
                AutoId = 0
            };

            try
            {
                bool response = await _pushNotifyServices.DeletePushNotifyDeviceRegistrationByDeviceIdNEmpId(deviceId, empId);
                if (response)
                {
                    res.Message = "Deleted Successfully";
                    res.StatusCode = 200;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }

            return Ok(res);
        }

    }
}
