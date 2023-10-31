using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.PushNotify;
using PeopleDesk.Models.SignalR;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.PushNotify.Interfaces;
using PeopleDesk.Services.SignalR.Interfaces;
using System.Net.Mail;
using User = PeopleDesk.Data.Entity.User;

namespace EmployeeMgmtAPI.Services.SignalR
{
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly PeopleDeskContext _context;
        private readonly IPushNotifyService _pushNotifyServices;
        private readonly iEmailService _iEmailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IHttpContextAccessor _httpContextAccessor, iEmailService _iEmailService, IPushNotifyService _pushNotifyServices, IConfiguration _configuration, PeopleDeskContext _context)
        {
            this._configuration = _configuration;
            this._context = _context;
            this._pushNotifyServices = _pushNotifyServices;
            this._iEmailService = _iEmailService;
            this._httpContextAccessor = _httpContextAccessor;
        }

        #region Notification Master

        public async Task<long> SaveNotificationMaster(NotificationMaster obj)
        {
            try
            {
                if (obj.IntId > 0)
                {
                    _context.NotificationMasters.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.NotificationMasters.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }

                return obj.IntId;
            }
            catch (Exception)
            {
                return -999;
            }
        }

        public async Task<NotificationMaster> GetNotificationMasterById(long id)
        {
            return await _context.NotificationMasters.Where(x => x.IntId == id).FirstOrDefaultAsync();
        }

        public async Task<long> DeleteNotificationMasterId(long id)
        {
            try
            {
                NotificationMaster obj = await _context.NotificationMasters.Where(x => x.IntId == id).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.IsActive = false;

                    _context.NotificationMasters.Update(obj);
                    await _context.SaveChangesAsync();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -999;
            }
        }

        public async Task<IEnumerable<NotificationMaster>> GetAllNotificationMasterByOrgId(long orgId)
        {
            return await _context.NotificationMasters.Where(x => x.IntOrgId == orgId && x.IsActive == true).ToListAsync();
        }

        public async Task<IEnumerable<NotificationMaster>> GetAllNotificationMasterByUser(string username)
        {
            return await _context.NotificationMasters.Where(x => x.StrReceiver == username && x.IsActive == true).ToListAsync();
        }

        public async Task<IEnumerable<NotificationMaster>> GetAllUnSeenNotificationMasterByUser(string username)
        {
            return await _context.NotificationMasters.Where(x => x.StrReceiver == username && x.IsActive == true && x.IsSeen == false).ToListAsync();
        }

        public async Task<IEnumerable<NotificationMaster>> GetAllSeenNotificationMasterByUser(string username)
        {
            return await _context.NotificationMasters.Where(x => x.StrReceiver == username && x.IsActive == true && x.IsSeen == true).ToListAsync();
        }

        public async Task<IEnumerable<NotificationViewModel>> GetAllUnSeenNotificationByUser(string username)
        {
            try
            {
                User user = await _context.Users.Where(x => x.StrLoginId == username).FirstOrDefaultAsync();
                List<NotificationViewModel> data = await (from m in _context.NotificationMasters
                                                          where m.IsActive == true && m.IntOrgId == user.IntAccountId
                                                          && (m.IsCommon == true && m != null ? _context.NotificationDetails.Where(x => x.IntMasterId == m.IntId && x.StrReceiver == username).FirstOrDefault() == null : false) ? true
                                                          : (m.StrReceiver == username && m.IsSeen == false) ? true : false
                                                          select new NotificationViewModel
                                                          {
                                                              Id = m.IntId,
                                                              NotifyTitle = m.StrNotifyTitle,
                                                              NotifyDetails = m.StrNotifyDetails,
                                                              Receiver = m.StrReceiver,
                                                              IsSeen = m.IsSeen,
                                                              ModuleId = m.IntModuleId,
                                                              Module = m.StrModule,
                                                              CreatedBy = m.StrCreatedBy,
                                                              CreatedAt = m.DteCreatedAt,
                                                              NotificationMaster = m
                                                          }).OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();

                List<NotificationMaster> isSeenList = new List<NotificationMaster>();
                data.ForEach(x =>
                {
                    LocalDateTime start = new LocalDateTime(x.CreatedAt.Value.Year, x.CreatedAt.Value.Month, x.CreatedAt.Value.Day, x.CreatedAt.Value.Hour, x.CreatedAt.Value.Minute, x.CreatedAt.Value.Second);
                    LocalDateTime end = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    Period period = Period.Between(start, end);

                    x.TimeDifference = period.Years > 0 ? period.Years + "y"
                        : period.Months > 0 ? period.Months + "mo"
                        : period.Days > 0 ? period.Days + "d"
                        : period.Hours > 0 ? period.Hours + "h"
                        : period.Minutes > 0 ? period.Minutes + "m"
                        : period.Seconds > 0 ? period.Seconds + "s"
                        : "now";

                    x.NotificationMaster.IsSeen = true;
                    isSeenList.Add(x.NotificationMaster);
                    x.NotificationMaster = x.NotificationMaster;
                });

                _context.NotificationMasters.UpdateRange(isSeenList);
                await _context.SaveChangesAsync();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception("invalid data");
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetAllSeenNUnSeenNotificationByUser(int pageNo, int pageSize, long employeeId, long accountId)
        {
            try
            {
                int skipCount = ((pageNo == 1 || pageNo == 0) ? 0 : (pageNo - 1)) * pageSize;

                List<NotificationViewModel> data = await (from m in _context.NotificationMasters
                                                          where m.IsActive == true && m.IntOrgId == accountId
                                                          && (true == (
                                                          (m.IsCommon == true && m != null) ? _context.NotificationDetails.Where(x => x.IntMasterId == m.IntId && x.StrReceiver == employeeId.ToString()).Count() > 0 ? true : false : false)
                                                          || true == (m.StrReceiver == employeeId.ToString()) ? true : false)
                                                          //&& ((m.IsCommon == true && m != null ? _context.NotificationDetails.Where(x => x.IntMasterId == m.IntId && x.StrReceiver == username).FirstOrDefault() == null : false) ? true
                                                          //: (m.StrReceiver == username && m.IsSeen == false) ? true : false
                                                          //|| (m.IsCommon == true || m.StrReceiver == username) ? true : false)
                                                          join emp1 in _context.EmpEmployeeBasicInfos on m.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                                                          from emp in emp2.DefaultIfEmpty()
                                                          select new NotificationViewModel
                                                          {
                                                              Id = m.IntId,
                                                              BusinessUnitId = emp.IntBusinessUnitId,
                                                              NotifyTitle = m.StrNotifyTitle,
                                                              NotifyDetails = m.StrNotifyDetails,
                                                              Feature = m.StrFeature,
                                                              Receiver = m.StrReceiver,
                                                              IsSeen = m.IsSeen,
                                                              ModuleId = m.IntModuleId,
                                                              Module = m.StrModule,
                                                              CreatedBy = m.StrCreatedBy,
                                                              CreatedAt = m.DteCreatedAt,
                                                              NotificationMaster = m
                                                          }).OrderByDescending(x => x.CreatedAt).Skip(skipCount).Take(pageSize).AsNoTracking().ToListAsync();

                List<NotificationMaster> isSeenList = new List<NotificationMaster>();
                data.ForEach(x =>
                {
                    LocalDateTime start = new LocalDateTime(x.CreatedAt.Value.Year, x.CreatedAt.Value.Month, x.CreatedAt.Value.Day, x.CreatedAt.Value.Hour, x.CreatedAt.Value.Minute, x.CreatedAt.Value.Second);
                    LocalDateTime end = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    Period period = Period.Between(start, end);

                    x.TimeDifference = period.Years > 0 ? period.Years + "y"
                        : period.Months > 0 ? period.Months + "mo"
                        : period.Days > 0 ? period.Days + "d"
                        : period.Hours > 0 ? period.Hours + "h"
                        : period.Minutes > 0 ? period.Minutes + "m"
                        : period.Seconds > 0 ? period.Seconds + "s"
                        : "now";

                    x.NotificationMaster.IsSeen = true;
                    isSeenList.Add(x.NotificationMaster);
                    x.NotificationMaster = x.NotificationMaster;
                });

                _context.NotificationMasters.UpdateRange(isSeenList);
                await _context.SaveChangesAsync();

                return data;
            }
            catch (Exception e)
            {
                throw new Exception("invalid data");
            }
        }

        public async Task<IEnumerable<NotificationMaster>> GetNotificationCount(long employeeId, long accountId)
        {
            try
            {
                var data = await (from m in _context.NotificationMasters
                                  where m.IsActive == true && m.IntOrgId == accountId
                                  && (m.IsCommon == false && !string.IsNullOrEmpty(m.StrReceiver) ? m.StrReceiver == employeeId.ToString() && m.IsSeen == false
                                     : m.IsCommon == true ? _context.NotificationDetails.Where(x => x.IntMasterId == m.IntId && x.StrReceiver == employeeId.ToString()).Count() <= 0
                                     : false)
                                  select m).Distinct().AsQueryable().AsNoTracking().ToListAsync();
                //&& (true == (
                //(m.IsCommon == true && m != null) ? _context.NotificationDetails.Where(x => x.IntMasterId == m.IntId && x.StrReceiver == employeeId.ToString()).Count() > 0 ? true : false : false)
                //|| true == (m.StrReceiver == employeeId.ToString() && m.IsSeen == false) ? true : false)

                return data;
            }
            catch (Exception e)
            {
                throw new Exception("invalid data");
            }
        }

        #endregion Notification Master

        #region Notification Detail

        public async Task<long> SaveNotificationDetail(NotificationDetail obj)
        {
            try
            {
                if (obj.IntId > 0)
                {
                    _context.NotificationDetails.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.NotificationDetails.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }

                return obj.IntId;
            }
            catch (Exception)
            {
                return -999;
            }
        }

        public async Task<NotificationDetail> GetNotificationDetailById(long id)
        {
            return await _context.NotificationDetails.Where(x => x.IntId == id).FirstOrDefaultAsync();
        }

        public async Task<long> DeleteNotificationDetailId(long id)
        {
            try
            {
                NotificationDetail obj = await _context.NotificationDetails.Where(x => x.IntId == id).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.IsActive = false;

                    _context.NotificationDetails.Update(obj);
                    await _context.SaveChangesAsync();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -999;
            }
        }

        public async Task<IEnumerable<NotificationDetail>> GetAllNotificationDetailByMasterId(long masterId)
        {
            return await _context.NotificationDetails.Where(x => x.IntMasterId == masterId && x.IsActive == true).ToListAsync();
        }

        #endregion Notification Detail

        #region Notification

        public async Task<bool> SendToGlobalAll()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var url = _configuration["SignalR:Url"] + ("/Notification/SendToGlobalAll");
                var res = await httpClient.GetAsync(url);
                var stringContent = await res.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<string>(stringContent);
                if (response == "success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SendToAllUserByAppBased(string? appName)
        {
            try
            {
                if (!string.IsNullOrEmpty(appName))
                {
                    HttpClient httpClient = new HttpClient();
                    var url = _configuration["SignalR:Url"] + ("/Notification/SendToAllUserByAppBased?appName=" + appName);
                    var res = await httpClient.GetAsync(url);
                    var stringContent = await res.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<string>(stringContent);
                    if (response == "success")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SendToAllUserByOrgId(long? orgId)
        {
            try
            {
                if (orgId > 0)
                {
                    HttpClient httpClient = new HttpClient();
                    var url = _configuration["SignalR:Url"] + ("/Notification/SendToAllUserByOrgId?orgId=" + orgId);
                    var res = await httpClient.GetAsync(url);
                    var stringContent = await res.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<string>(stringContent);
                    if (response == "success")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SendToSingleUserByUsername(string? appName, string? username)
        {
            try
            {
                string baseUrl = _httpContextAccessor.HttpContext.Request.Host.Value;

                if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(username))
                {
                    HttpClient httpClient = new HttpClient();
                    var url = _configuration["SignalR:Url"] + ("/Notification/SendToSingleUserByUsername?appName=people_desk_saas_" + baseUrl + "_" + appName + "&username=" + username);
                    var res = await httpClient.GetAsync(url);
                    var stringContent = await res.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<string>(stringContent);
                    if (response == "success")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion Notification

        #region ALL Notification M E T H O D

        public async Task<bool> SendToCompanyPolicyNotification(SendToGroupUserViewModel obj, long AccountId, bool isAll, string notificationTitle, string notificationDescription)
        {
            try
            {
                List<long> EmployeeIdList = obj.EmployeeIdList.GroupBy(x => x).Select(x => x.Key).ToList();
                List<User> ApplicationUsersList = await _context.Users.Where(x => x.IntAccountId == AccountId).ToListAsync();
                List<NotificationMaster> NotificationMasterList = new List<NotificationMaster>();

                EmployeeIdList.ForEach(x =>
                {
                    NotificationMaster notificationObj = new NotificationMaster
                    {
                        IntId = 0,
                        IntOrgId = AccountId,
                        StrNotifyTitle = notificationTitle,
                        StrNotifyDetails = notificationDescription,
                        IntModuleId = 1,
                        StrModule = "HCM",
                        StrFeature = "Company Policy",
                        //IntFeatureTableAutoId = msg.AutoId,
                        IntEmployeeId = x,
                        StrLoginId = ApplicationUsersList.Where(y => y.IntRefferenceId == x).FirstOrDefault().StrLoginId,
                        IsCommon = false,
                        StrReceiver = null,
                        IsSeen = false,
                        IsActive = true,
                        StrCreatedBy = "System",
                        DteCreatedAt = DateTime.Now
                    };

                    NotificationMasterList.Add(notificationObj);
                });

                if (NotificationMasterList.Count() > 0)
                {
                    await _context.NotificationMasters.AddRangeAsync(NotificationMasterList);
                    await _context.SaveChangesAsync();

                    // send to group based notification
                    SendToGroupUserViewModel sendObj = new SendToGroupUserViewModel
                    {
                        AccountId = AccountId,
                        EmployeeIdList = EmployeeIdList,
                    };
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion ALL Notification M E T H O D

        #region ====================== EVENT WISE NOTIFICATION SEND ==========================

        //====================================================================================
        public async Task<bool> LeaveApplicationNotify(LeaveApplicationDTO model)
        {
            try
            {
                NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters.Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && x.StrNcategoryName.ToLower() == "Leave Application".ToLower()).FirstOrDefaultAsync();
                if (NotificationPermissionMaster != null)
                {
                    List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails.Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && NotificationPermissionMaster.IntPermissionId == x.IntPermissionId).ToListAsync();

                    if (NotificationPermissionDetailsList.Count() > 0)
                    {
                        LveLeaveType leaveType = await _context.LveLeaveTypes.Where(x => x.IntLeaveTypeId == model.LeaveTypeId).AsNoTracking().FirstOrDefaultAsync();

                        GlobalPipelineRow pipelineRow = await (from master in _context.GlobalPipelineHeaders
                                                               join row in _context.GlobalPipelineRows on master.IntPipelineHeaderId equals row.IntPipelineHeaderId
                                                               where master.StrApplicationType.ToLower() == "leave" && master.IsActive == true && row.IsActive == true
                                                               select row).OrderBy(x => x.IntShortOrder).FirstOrDefaultAsync();

                        List<long> approverEmployeeId = pipelineRow.IsSupervisor ? await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == model.EmployeeId).Select(x => (long)x.IntSupervisorId).Take(1).ToListAsync()
                                                        : pipelineRow.IsLineManager ? await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == model.EmployeeId).Select(x => (long)x.IntLineManagerId).Take(1).ToListAsync()
                                                        : await (from p in _context.GlobalPipelineRows
                                                                 join ugr in _context.UserGroupRows on p.IntUserGroupHeaderId equals ugr.IntUserGroupHeaderId
                                                                 select (long)ugr.IntEmployeeId).ToListAsync();

                        EmpEmployeeBasicInfo employeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == model.EmployeeId);

                        //string str = employeeBasicInfo.StrGender.ToLower() == "male" ? " his " : " her ";
                        string mrNms = employeeBasicInfo.StrGender.ToLower() == "male" ? "Mr. " : "Ms. ";

                        LocalDate joiningDate = new LocalDate(model.AppliedFromDate.Year, model.AppliedFromDate.Month, model.AppliedFromDate.Day);
                        LocalDate currentDate = new LocalDate(model.AppliedToDate.Year, model.AppliedToDate.Month, model.AppliedToDate.Day);
                        Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                        string leaveDays = (period.Years > 0 && period.Months > 0 && period.Days > 0) ? (period.Years.ToString() + (period.Years > 1 ? " Years" : " Year") + period.Months.ToString() + (period.Months > 1 ? " Months" : " Month") + period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : (period.Months > 0 && period.Days > 0) ? (period.Months.ToString() + (period.Months > 1 ? " Months" : " Month") + period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : (period.Days > 1) ? (period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : "1";

                        NotificationMaster notificationObj = new NotificationMaster
                        {
                            IntId = 0,
                            IntOrgId = model.AccountId,
                            StrNotifyTitle = "Leave Application",
                            StrNotifyDetails = mrNms + employeeBasicInfo?.StrEmployeeName + " " + "Apply For " + leaveDays + " " + leaveType.StrLeaveType + ".",
                            IntModuleId = 1,
                            StrModule = "Employee Management",
                            StrFeature = "leave_application",
                            IntFeatureTableAutoId = model.LeaveApplicationId,
                            IntEmployeeId = model.EmployeeId,
                            // Login ID Rakhar karon hocce Age User dore kaj kortam tar jonno Query korte jeno easy hoy tar jonno rakha chilo
                            //StrLoginId = _context.Users.Where(x => x.IntRefferenceId == model.EmployeeId).FirstOrDefault().StrLoginId,
                            IsCommon = false,
                            StrReceiver = null,
                            IsSeen = false,
                            IsActive = true,
                            StrCreatedBy = "System",
                            DteCreatedAt = DateTime.Now
                        };

                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Real Time".ToLower()).Count() > 0)
                        {
                            foreach (long receiverId in approverEmployeeId)
                            {
                                notificationObj.StrReceiver = receiverId.ToString();

                                await SaveNotificationMaster(notificationObj);
                                await SendToSingleUserByUsername(employeeBasicInfo?.IntAccountId.ToString(), receiverId.ToString());

                                notificationObj.IntId = 0;
                                notificationObj.DteCreatedAt = DateTime.Now;
                            }
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Push".ToLower()).Count() > 0)
                        {
                            foreach (long receiverId in approverEmployeeId)
                            {
                                List<PushNotifyDeviceRegistration> deviceList = await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId(receiverId);
                                PushNotificationViewModel notificationModel = new PushNotificationViewModel
                                {
                                    DeviceId = "",
                                    Title = notificationObj.StrNotifyTitle,
                                    Body = notificationObj.StrNotifyDetails
                                };

                                foreach (PushNotifyDeviceRegistration pushNotifyDevice in deviceList)
                                {
                                    notificationModel.DeviceId = pushNotifyDevice.StrDeviceId;
                                    await _pushNotifyServices.SendNotification(notificationModel);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        public async Task<bool> MovementApplicationNotify(MovementApplicationDTO model)
        {
            try
            {
                NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters.Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && x.StrNcategoryName.ToLower() == "Movement Application".ToLower()).FirstOrDefaultAsync();
                if (NotificationPermissionMaster != null)
                {
                    List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails.Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && NotificationPermissionMaster.IntPermissionId == x.IntPermissionId).ToListAsync();

                    if (NotificationPermissionDetailsList.Count() > 0)
                    {
                        LveMovementType leaveType = await _context.LveMovementTypes.Where(x => x.IntMovementTypeId == model.MovementTypeId).AsNoTracking().FirstOrDefaultAsync();

                        GlobalPipelineRow pipelineRow = await (from master in _context.GlobalPipelineHeaders
                                                               join row in _context.GlobalPipelineRows on master.IntPipelineHeaderId equals row.IntPipelineHeaderId
                                                               where master.StrApplicationType.ToLower() == "movement" && master.IsActive == true && row.IsActive == true
                                                               select row).OrderBy(x => x.IntShortOrder).FirstOrDefaultAsync();

                        List<long> approverEmployeeId = pipelineRow.IsSupervisor ? await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == model.IntEmployeeId).Select(x => (long)x.IntSupervisorId).Take(1).ToListAsync()
                                                        : pipelineRow.IsLineManager ? await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == model.IntEmployeeId).Select(x => (long)x.IntLineManagerId).Take(1).ToListAsync()
                                                        : await (from p in _context.GlobalPipelineRows
                                                                 join ugr in _context.UserGroupRows on p.IntUserGroupHeaderId equals ugr.IntUserGroupHeaderId
                                                                 select (long)ugr.IntEmployeeId).ToListAsync();

                        EmpEmployeeBasicInfo employeeBasicInfo = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == model.IntEmployeeId);

                        //string str = employeeBasicInfo.StrGender.ToLower() == "male" ? " his " : " her ";
                        string mrNms = employeeBasicInfo.StrGender.ToLower() == "male" ? "Mr. " : "Ms. ";

                        LocalDate joiningDate = new LocalDate(model.FromDate.Year, model.FromDate.Month, model.FromDate.Day);
                        LocalDate currentDate = new LocalDate(model.ToDate.Year, model.ToDate.Month, model.ToDate.Day);
                        Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                        string leaveDays = (period.Years > 0 && period.Months > 0 && period.Days > 0) ? (period.Years.ToString() + (period.Years > 1 ? " Years" : " Year") + period.Months.ToString() + (period.Months > 1 ? " Months" : " Month") + period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : (period.Months > 0 && period.Days > 0) ? (period.Months.ToString() + (period.Months > 1 ? " Months" : " Month") + period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : (period.Days > 1) ? (period.Days.ToString() + (period.Days > 1 ? " Days" : "Day"))
                                           : "1";

                        NotificationMaster notificationObj = new NotificationMaster
                        {
                            IntId = 0,
                            IntOrgId = model.AccountId,
                            StrNotifyTitle = "Movement Application",
                            StrNotifyDetails = mrNms + employeeBasicInfo?.StrEmployeeName + " " + "Apply For " + leaveDays + " " + leaveType.StrMovementType + " pass.",
                            IntModuleId = 1,
                            StrModule = "Employee Management",
                            StrFeature = "movement_application",
                            IntFeatureTableAutoId = model.MovementId,
                            IntEmployeeId = model.IntEmployeeId,
                            // Login ID Rakhar karon hocce Age User dore kaj kortam tar jonno Query korte jeno easy hoy tar jonno rakha chilo
                            //StrLoginId = _context.Users.Where(x => x.IntRefferenceId == model.IntEmployeeId).FirstOrDefault().StrLoginId,
                            IsCommon = false,
                            StrReceiver = null,
                            IsSeen = false,
                            IsActive = true,
                            StrCreatedBy = "System",
                            DteCreatedAt = DateTime.Now
                        };

                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Real Time".ToLower()).Count() > 0)
                        {
                            foreach (long receiverId in approverEmployeeId)
                            {
                                notificationObj.StrReceiver = receiverId.ToString();

                                await SaveNotificationMaster(notificationObj);
                                await SendToSingleUserByUsername(employeeBasicInfo?.IntAccountId.ToString(), receiverId.ToString());

                                notificationObj.IntId = 0;
                                notificationObj.DteCreatedAt = DateTime.Now;
                            }
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Push".ToLower()).Count() > 0)
                        {
                            foreach (long receiverId in approverEmployeeId)
                            {
                                List<PushNotifyDeviceRegistration> deviceList = await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId(receiverId);
                                PushNotificationViewModel notificationModel = new PushNotificationViewModel
                                {
                                    DeviceId = "",
                                    Title = notificationObj.StrNotifyTitle,
                                    Body = notificationObj.StrNotifyDetails
                                };

                                foreach (PushNotifyDeviceRegistration pushNotifyDevice in deviceList)
                                {
                                    notificationModel.DeviceId = pushNotifyDevice.StrDeviceId;
                                    await _pushNotifyServices.SendNotification(notificationModel);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        public async Task<bool> PolicyUploadNotify(PolicyHeader model)
        {
            try
            {
                NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters.Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId && x.StrNcategoryName.ToLower() == "Policy".ToLower()).FirstOrDefaultAsync();
                if (NotificationPermissionMaster != null)
                {
                    List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails.Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId && NotificationPermissionMaster.IntPermissionId == x.IntPermissionId).ToListAsync();

                    if (NotificationPermissionDetailsList.Count() > 0)
                    {
                        List<EmpEmployeeBasicInfo> employeeList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == model.IntAccountId
                        && (x.IntBusinessUnitId == 0 || x.IntBusinessUnitId == model.IntBusinessUnitId) && x.IsActive == true).ToListAsync();

                        NotificationMaster notificationObj = new NotificationMaster
                        {
                            IntId = 0,
                            IntOrgId = model.IntAccountId,
                            StrNotifyTitle = "Organization Policy",
                            StrNotifyDetails = model.StrPolicyTitle,
                            IntModuleId = 1,
                            StrModule = "Policy",
                            StrFeature = "policy",
                            IntFeatureTableAutoId = model.IntPolicyId,
                            IntEmployeeId = model.IntCreatedBy,
                            // Login ID Rakhar karon hocce Previously User dore kaj kortam tar jonno Query korte jeno easy hoy tar jonno rakha chilo
                            //StrLoginId = _context.Users.Where(x => x.IntRefferenceId == model.IntCreatedBy).FirstOrDefault().StrLoginId,
                            IsCommon = false,
                            StrReceiver = null,
                            IsSeen = false,
                            IsActive = true,
                            StrCreatedBy = "System",
                            DteCreatedAt = DateTime.Now
                        };

                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Real Time".ToLower()).Count() > 0)
                        {
                            foreach (EmpEmployeeBasicInfo employee in employeeList)
                            {
                                notificationObj.StrReceiver = employee.IntEmployeeBasicInfoId.ToString();

                                await SaveNotificationMaster(notificationObj);
                                await SendToSingleUserByUsername(employee?.IntAccountId.ToString(), notificationObj.StrReceiver);

                                notificationObj.IntId = 0;
                                notificationObj.DteCreatedAt = DateTime.Now;
                            }
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Push".ToLower()).Count() > 0)
                        {
                            foreach (EmpEmployeeBasicInfo employee in employeeList)
                            {
                                List<PushNotifyDeviceRegistration> deviceList = await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId(employee.IntEmployeeBasicInfoId);
                                PushNotificationViewModel notificationModel = new PushNotificationViewModel
                                {
                                    DeviceId = "",
                                    Title = notificationObj.StrNotifyTitle,
                                    Body = notificationObj.StrNotifyDetails
                                };

                                foreach (PushNotifyDeviceRegistration pushNotifyDevice in deviceList)
                                {
                                    notificationModel.DeviceId = pushNotifyDevice.StrDeviceId;
                                    await _pushNotifyServices.SendNotification(notificationModel);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        public async Task<bool> LateAttendanceNotify(BackgrounNotifyVM model)
        {
            try
            {
                NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters
                    .Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && x.StrNcategoryName.ToLower() == "Late Attendance".ToLower())
                    .AsQueryable().AsNoTracking().FirstOrDefaultAsync();

                if (NotificationPermissionMaster != null)
                {
                    List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails
                        .Where(x => x.IsActive == true && x.IntAccountId == model.AccountId && NotificationPermissionMaster.IntPermissionId == x.IntPermissionId).ToListAsync();

                    if (NotificationPermissionDetailsList.Count() > 0)
                    {
                        NotificationMaster notificationObj = new NotificationMaster
                        {
                            IntId = 0,
                            IntOrgId = model.AccountId,
                            StrNotifyTitle = model.NotifyTitle,
                            StrNotifyDetails = model.NotifyDetails,
                            IntModuleId = 1,
                            StrModule = "Employee Management",
                            StrFeature = "late_attendance_notify",
                            IntFeatureTableAutoId = model.IntFeatureTableAutoId,
                            IntEmployeeId = model.EmployeeId,
                            // Login ID Rakhar karon hocce Age User dore kaj kortam tar jonno Query korte jeno easy hoy tar jonno rakha chilo
                            //StrLoginId = _context.Users.Where(x => x.IntRefferenceId == model.IntReciverId).FirstOrDefault().StrLoginId,
                            IsCommon = false,
                            StrReceiver = model.IntReciverId.ToString(),
                            IsSeen = false,
                            IsActive = true,
                            StrCreatedBy = "System",
                            DteCreatedAt = DateTime.Now
                        };

                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Real Time".ToLower()).Count() > 0)
                        {
                            await SaveNotificationMaster(notificationObj);
                            await SendToSingleUserByUsername(model.AccountId.ToString(), model.IntReciverId.ToString());
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Push".ToLower()).Count() > 0)
                        {
                            List<PushNotifyDeviceRegistration> deviceList = await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId((long)model.IntReciverId);
                            PushNotificationViewModel notificationModel = new PushNotificationViewModel
                            {
                                DeviceId = "",
                                Title = notificationObj.StrNotifyTitle,
                                Body = notificationObj.StrNotifyDetails
                            };

                            foreach (PushNotifyDeviceRegistration pushNotifyDevice in deviceList)
                            {
                                notificationModel.DeviceId = pushNotifyDevice.StrDeviceId;
                                await _pushNotifyServices.SendNotification(notificationModel);
                            }
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Mail".ToLower()).Count() > 0)
                        {
                            string mailBody = "<!DOCTYPE html>" +
                                                "<html>" +
                                                "<body>" +
                                                    "<div style=" + '"' + "font-size:12px" + '"' +
                                                        "<p>Hi <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + model.StrReceiverEmail + "</a> </p>" +
                                                        "<p>SORRY Sir, you have late 3 days.</p>" +
                                                        "<p>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>" +
                                                        "<p>" +
                                                            "Thanks, <Br /> " +
                                                            "The PeopleDesk team" +
                                                        "</p>" +
                                                    "</div>" +
                                                "</body>" +
                            "</html>";

                            //string res = await _iEmailService.SendEmail("iBOS", model.Email, "", "", "PeopleDesk Login OTP", mailBody, "HTML");
                            SendMail sendMail = new SendMail()
                            {
                                SendTo = model.StrReceiverEmail,
                                MailSubject = "PeopleDesk Leave notify",
                                MailBody = mailBody,
                                IsHtmlFormat = true

                            };

                            bool res = await _iEmailService.SendMail(sendMail);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }


        public async Task<bool> BackgroundNotifyCommon(BackgroundNotifyCommonVM model)
        {
            try
            {
                NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters
                   .Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId && x.StrNcategoryName.ToLower() == model.StrCategoryName.ToLower())
                   .AsQueryable().AsNoTracking().FirstOrDefaultAsync();

                if (NotificationPermissionMaster != null)
                {
                    List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails
                        .Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId && NotificationPermissionMaster.IntPermissionId == x.IntPermissionId).ToListAsync();

                    if (NotificationPermissionDetailsList.Count() > 0)
                    {
                        NotificationMaster notificationObj = new NotificationMaster
                        {
                            IntId = 0,
                            IntOrgId = model.IntAccountId,
                            StrNotifyTitle = model.NotifyTitle,
                            StrNotifyDetails = model.NotifyDetails,
                            IntModuleId = model.IntModuleId,
                            StrModule = model.StrModule,
                            StrFeature = model.StrFeature,
                            IntFeatureTableAutoId = model.IntFeatureTableAutoId,
                            IntEmployeeId = model.IntEmployeeId,
                            // Login ID Rakhar karon hocce Age User dore kaj kortam tar jonno Query korte jeno easy hoy tar jonno rakha chilo
                            //StrLoginId = _context.Users.Where(x => x.IntRefferenceId == model.IntReciverId).FirstOrDefault().StrLoginId,
                            IsCommon = model.IsCommon,
                            StrReceiver = model.IntReciverId.ToString(),
                            IsSeen = false,
                            IsActive = true,
                            StrCreatedBy = "System",
                            DteCreatedAt = DateTime.Now
                        };

                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Real Time".ToLower()).Count() > 0)
                        {
                            await SaveNotificationMaster(notificationObj);
                            await SendToSingleUserByUsername(model.IntAccountId.ToString(), model.IntReciverId.ToString());
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Push".ToLower()).Count() > 0)
                        {
                            List<PushNotifyDeviceRegistration> deviceList = await _pushNotifyServices.GetAllPushNotifyDeviceRegistrationByEmployeeId((long)model.IntReciverId);
                            PushNotificationViewModel notificationModel = new PushNotificationViewModel
                            {
                                DeviceId = "",
                                Title = notificationObj.StrNotifyTitle,
                                Body = notificationObj.StrNotifyDetails
                            };

                            foreach (PushNotifyDeviceRegistration pushNotifyDevice in deviceList)
                            {
                                notificationModel.DeviceId = pushNotifyDevice.StrDeviceId;
                                await _pushNotifyServices.SendNotification(notificationModel);
                            }
                        }
                        if (NotificationPermissionDetailsList.Where(x => x.SteNcategoryTypeName.ToLower() == "Mail".ToLower()).Count() > 0)
                        {

                            //string res = await _iEmailService.SendEmail("iBOS", model.Email, "", "", "PeopleDesk Login OTP", mailBody, "HTML");
                            if (model.StrReceiverEmail != null && !string.IsNullOrEmpty(model.StrReceiverEmail))
                            {
                                bool isValid;
                                try
                                {
                                    new MailAddress(model.StrReceiverEmail);
                                    isValid = true;
                                }
                                catch (Exception)
                                {

                                    isValid = false;
                                }
                                if (isValid)
                                {
                                    SendMail sendMail = new SendMail()
                                    {
                                        SendTo = model.StrReceiverEmail,
                                        MailSubject = model.MailSubject,
                                        MailBody = model.MailBody,
                                        IsHtmlFormat = true
                                    };
                                    bool res = await _iEmailService.SendMail(sendMail);
                                }

                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        #endregion ====================== EVENT WISE NOTIFICATION SEND ==========================
    }
}

#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore CS8603 // Possible null reference return.