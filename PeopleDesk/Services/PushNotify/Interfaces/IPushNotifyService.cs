using PeopleDesk.Data.Entity;
using PeopleDesk.Models.PushNotify;

namespace PeopleDesk.Services.PushNotify.Interfaces
{
    public interface IPushNotifyService
    {
        Task<ResponseViewModel> SendNotification(PushNotificationViewModel notificationModel);

        #region ========== Push Notify Device Registration ==================
        Task<long> SavePushNotifyDeviceRegistration(PushNotifyDeviceRegistration obj);
        Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistration();
        Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistrationByEmployeeId(long employeeId);
        Task<List<PushNotifyDeviceRegistration>> GetAllPushNotifyDeviceRegistrationByDeviceId(string deviceId);
        Task<PushNotifyDeviceRegistration> GetPushNotifyDeviceRegistrationById(long id);
        Task<bool> DeletePushNotifyDeviceRegistrationById(string deviceId);
        Task<bool> DeletePushNotifyDeviceRegistrationByDeviceIdNEmpId(string deviceId, long empId);
        #endregion
    }
}
