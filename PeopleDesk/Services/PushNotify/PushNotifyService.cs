using CorePush.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.PushNotify;
using PeopleDesk.Services.PushNotify.Interfaces;
using System.Net.Http.Headers;
using static PeopleDesk.Models.PushNotify.GoogleNotification;

namespace PeopleDesk.Services.PushNotify
{
    public class PushNotifyService : IPushNotifyService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private readonly PeopleDeskContext _context;
        public PushNotifyService(IOptions<FcmNotificationSetting> settings, PeopleDeskContext _context)
        {
            _fcmNotificationSetting = settings.Value;
            this._context = _context;
        }

        public async Task<ResponseViewModel> SendNotification(PushNotificationViewModel notificationModel)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                FcmSettings settings = new FcmSettings()
                {
                    SenderId = _fcmNotificationSetting.SenderId,
                    ServerKey = _fcmNotificationSetting.ServerKey
                };
                HttpClient httpClient = new HttpClient();

                string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                string deviceToken = notificationModel.DeviceId;

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                httpClient.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                DataPayload dataPayload = new DataPayload();
                dataPayload.Title = notificationModel.Title;
                dataPayload.Body = notificationModel.Body;


                GoogleNotification notification = new GoogleNotification();
                notification.Data = dataPayload;
                notification.Notification = dataPayload;

                var fcm = new FcmSender(settings, httpClient);
                var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                if (fcmSendResponse.IsSuccess())
                {
                    response.IsSuccess = true;
                    response.Message = "Notification sent successfully";
                    return response;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = fcmSendResponse.Results[0].Error;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
                return response;
            }
        }


        #region ====================== Push Notify Device Registration =========================
        public async Task<long> SavePushNotifyDeviceRegistration(PushNotifyDeviceRegistration obj)
        {
            if (obj.IntId > 0)
            {
                _context.PushNotifyDeviceRegistrations.Update(obj);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.PushNotifyDeviceRegistrations.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntId;
        }
        public async Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistration()
        {
            return await _context.PushNotifyDeviceRegistrations.Where(x => x.IsActive == true).OrderBy(x => x.IntId).ToListAsync();
        }
        public async Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistrationByEmployeeId(long employeeId)
        {
            return await _context.PushNotifyDeviceRegistrations.Where(x => x.IsActive == true && x.IntEmployeeId == employeeId && !string.IsNullOrEmpty(x.StrDeviceId)).OrderBy(x => x.IntId).ToListAsync();
        }
        public async Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistrationByDeviceId(string deviceId)
        {
            return await _context.PushNotifyDeviceRegistrations.Where(x => x.IsActive == true && x.StrDeviceId == deviceId).OrderBy(x => x.IntId).ToListAsync();
        }
        public async Task<PushNotifyDeviceRegistration> GetPushNotifyDeviceRegistrationById(long id)
        {
            return await _context.PushNotifyDeviceRegistrations.FirstAsync(x => x.IsActive == true && x.IntId == id);
        }
        public async Task<bool> DeletePushNotifyDeviceRegistrationById(string deviceId)
        {
            try
            {
                List<PushNotifyDeviceRegistration> objList = await _context.PushNotifyDeviceRegistrations.Where(x => x.StrDeviceId == deviceId && x.IsActive == true).ToListAsync();

                objList.ForEach(x =>
                {
                    x.IsActive = false;
                });

                _context.PushNotifyDeviceRegistrations.UpdateRange(objList);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeletePushNotifyDeviceRegistrationByDeviceIdNEmpId(string deviceId, long empId)
        {
            try
            {
                List<PushNotifyDeviceRegistration> objList = await _context.PushNotifyDeviceRegistrations.Where(x => x.StrDeviceId == deviceId && x.IntEmployeeId != empId && x.IsActive == true).ToListAsync();

                objList.ForEach(x =>
                {
                    x.IsActive = false;
                });

                _context.PushNotifyDeviceRegistrations.UpdateRange(objList);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

    }
}
