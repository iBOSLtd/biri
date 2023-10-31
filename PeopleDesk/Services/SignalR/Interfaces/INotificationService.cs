using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.SignalR;

namespace PeopleDesk.Services.SignalR.Interfaces
{
    public interface INotificationService
    {
        #region Notification Master

        Task<long> SaveNotificationMaster(NotificationMaster obj);

        Task<NotificationMaster> GetNotificationMasterById(long id);

        Task<long> DeleteNotificationMasterId(long id);

        Task<IEnumerable<NotificationMaster>> GetAllNotificationMasterByOrgId(long orgId);

        Task<IEnumerable<NotificationMaster>> GetAllNotificationMasterByUser(string username);

        Task<IEnumerable<NotificationMaster>> GetAllUnSeenNotificationMasterByUser(string username);

        Task<IEnumerable<NotificationMaster>> GetAllSeenNotificationMasterByUser(string username);

        Task<IEnumerable<NotificationViewModel>> GetAllUnSeenNotificationByUser(string username);

        Task<IEnumerable<NotificationViewModel>> GetAllSeenNUnSeenNotificationByUser(int pageNo, int pageSize, long employeeId, long accountId);

        Task<IEnumerable<NotificationMaster>> GetNotificationCount(long employeeId, long accountId);

        #endregion Notification Master

        #region Notification Details

        Task<long> SaveNotificationDetail(NotificationDetail obj);

        Task<NotificationDetail> GetNotificationDetailById(long id);

        Task<long> DeleteNotificationDetailId(long id);

        Task<IEnumerable<NotificationDetail>> GetAllNotificationDetailByMasterId(long masterId);

        #endregion Notification Details

        #region Notification Send

        Task<bool> SendToGlobalAll();

        Task<bool> SendToAllUserByAppBased(string? appName);

        Task<bool> SendToAllUserByOrgId(long? orgId);

        Task<bool> SendToSingleUserByUsername(string? appName, string? username);

        Task<bool> SendToCompanyPolicyNotification(SendToGroupUserViewModel obj, long AccountId, bool isAll, string notificationTitle, string notificationDescription);

        #endregion Notification Send

        #region ====================== Event wise Leave Application ==========================

        //====================================================================================
        Task<bool> LeaveApplicationNotify(LeaveApplicationDTO model);

        Task<bool> MovementApplicationNotify(MovementApplicationDTO model);

        Task<bool> PolicyUploadNotify(PolicyHeader model);

        Task<bool> LateAttendanceNotify(BackgrounNotifyVM model);

        Task<bool> BackgroundNotifyCommon(BackgroundNotifyCommonVM model);

        #endregion ====================== Event wise Leave Application ==========================
    }
}