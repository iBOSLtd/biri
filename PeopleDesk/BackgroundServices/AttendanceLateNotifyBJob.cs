using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SignalR.Interfaces;

namespace PeopleDesk.BackgroundServices
{
    public class AttendanceLateNotifyBJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AttendanceLateNotifyBJob(IServiceProvider serviceProvider, ILogger<AttendanceLateNotifyBJob> logger)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<PeopleDeskContext>();

                        NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters
                        .Where(x => x.IsActive == true && x.StrNcategoryName.ToLower() == "Late Attendance".ToLower())
                        .AsQueryable().AsNoTracking().FirstOrDefaultAsync();

                        if (NotificationPermissionMaster != null)
                        {
                            var lateEmployees = await (from emp in _context.EmpEmployeeBasicInfos
                                                       join empDet in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empDet.IntEmployeeId
                                                       join att in _context.TimeAttendanceDailySummaries on emp.IntEmployeeBasicInfoId equals att.IntEmployeeId
                                                       join sup in _context.EmpEmployeeBasicInfoDetails on emp.IntSupervisorId equals sup.IntEmployeeId
                                                       join dsup1 in _context.EmpEmployeeBasicInfoDetails on emp.IntDottedSupervisorId equals dsup1.IntEmployeeId into dsup2
                                                       from dsup in dsup2.DefaultIfEmpty()
                                                       join acc in _context.Accounts on emp.IntAccountId equals acc.IntAccountId
                                                       where att.IsLate == true && att.IntMonthId == DateTime.Now.Month && att.IntYear == DateTime.Now.Year
                                                       && emp.IsActive == true && acc.IsActive == true
                                                       && (_context.AttendanceLateNotifyLogs.Where(x => x.IntAccountId == emp.IntAccountId
                                                            && x.IntEmployeeId == emp.IntEmployeeBasicInfoId && x.DteAttendenceDate.Date == att.DteAttendanceDate.Value.Date).AsNoTracking().AsQueryable().Count() <= 0)
                                                       select new
                                                       {
                                                           IntEmployeeId = emp.IntEmployeeBasicInfoId,
                                                           IntAccountId = emp.IntAccountId,
                                                           strEmployeeName = emp.StrEmployeeName,
                                                           strOfficeEmail = empDet.StrOfficeMail,
                                                           IntSupervisorId = emp.IntSupervisorId,
                                                           strSupervisorEmail = sup.StrOfficeMail,
                                                           intDottedSupervisorId = emp.IntDottedSupervisorId,
                                                           strDottedSupervisorEmail = dsup != null ? dsup.StrOfficeMail : "",
                                                           DteAttendenceDate = (DateTime)att.DteAttendanceDate,
                                                       }).AsNoTracking().AsQueryable().ToListAsync();

                            var empIds = lateEmployees.GroupBy(g => g.IntEmployeeId).Where(g => g.Count() >= 3).Select(g => g.Key).ToList();

                            var notifyscopped = scope.ServiceProvider.GetRequiredService<INotificationService>();


                            BackgroundNotifyCommonVM notifyVm = new()
                            {
                                StrCategoryName = "Late Attendance",
                                IntModuleId = 1,
                                StrModule = "Late Attendance",
                                StrFeature = "Late Attendance",
                                IntFeatureTableAutoId = 0,
                                NotifyTitle = "Late Info",
                                IsCommon = false,
                                MailSubject = "Late Info",
                            };
                            foreach (long empId in empIds)
                            {
                                List<AttendanceLateNotifyLog> logList = lateEmployees.Where(x => x.IntEmployeeId == empId).OrderBy(x => x.DteAttendenceDate).Take(3).Select(x => new AttendanceLateNotifyLog
                                {
                                    IntEmployeeId = x.IntEmployeeId,
                                    IntAccountId = x.IntAccountId,
                                    DteAttendenceDate = x.DteAttendenceDate,
                                    DteCreatedAt = DateTime.Now
                                }).ToList();

                                await _context.AttendanceLateNotifyLogs.AddRangeAsync(logList);
                                int savedCount = await _context.SaveChangesAsync();

                                // notification service will be called from here
                                if (savedCount >= 3)
                                {
                                    var notifyReveiverList = lateEmployees.Where(a => a.IntEmployeeId == empId)
                                    .Select(b => new
                                    {
                                        IntEmployeeId = b.IntEmployeeId,
                                        StrEmployeeName = b.strEmployeeName,
                                        IntAccountId = b.IntAccountId,
                                        IntSuperVisorId = b.intDottedSupervisorId,
                                        strSupervisorEmail = b.strSupervisorEmail,
                                        IntDottedSupervisor = b.intDottedSupervisorId != null ? b.intDottedSupervisorId : 0,
                                        strDottedSupervisorEmail = b.strDottedSupervisorEmail
                                    }).FirstOrDefault();

                                    List<BackgroundNotifyCommonVM> notifyLogs = new();

                                    if (notifyReveiverList.IntSuperVisorId == notifyReveiverList.IntDottedSupervisor)
                                    {
                                        notifyVm.IntAccountId = notifyReveiverList.IntAccountId;
                                        notifyVm.IntEmployeeId = notifyReveiverList.IntEmployeeId;
                                        notifyVm.IntReciverId = notifyReveiverList.IntSuperVisorId;
                                        notifyVm.StrReceiverEmail = notifyReveiverList.strSupervisorEmail;
                                        notifyVm.NotifyDetails = $"You have a reminder, {notifyReveiverList.StrEmployeeName} has 3 days late attendance.";
                                        notifyVm.StrReceiverEmail = "<!DOCTYPE html>" +
                                                                    "<html>" +
                                                                    "<body>" +
                                                                        "<div style=" + '"' + "font-size:12px" + '"' +
                                                                            "<p> You have a reminder, <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + notifyReveiverList.StrEmployeeName + "</a> " +
                                                                            "has 3 days late attendance. </p>" +
                                                                        "</div>" +
                                                                    "</body>" +
                                                                    "</html>";
                                        notifyLogs.Add(notifyVm);
                                    }
                                    else
                                    {
                                        notifyVm.IntAccountId = notifyReveiverList.IntAccountId;
                                        notifyVm.IntEmployeeId = notifyReveiverList.IntEmployeeId;
                                        notifyVm.IntReciverId = notifyReveiverList.IntSuperVisorId;
                                        notifyVm.StrReceiverEmail = notifyReveiverList.strSupervisorEmail;
                                        notifyVm.NotifyDetails = $"You have a reminder, {notifyReveiverList.StrEmployeeName} has 3 days late attendance.";
                                        notifyVm.StrReceiverEmail = "<!DOCTYPE html>" +
                                                                    "<html>" +
                                                                    "<body>" +
                                                                        "<div style=" + '"' + "font-size:12px" + '"' +
                                                                            "<p> You have a reminder, <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + notifyReveiverList.StrEmployeeName + "</a> " +
                                                                            "has 3 days late attendance. </p>" +
                                                                        "</div>" +
                                                                    "</body>" +
                                                                    "</html>";
                                        notifyLogs.Add(notifyVm);

                                        if (notifyReveiverList.IntDottedSupervisor > 0)
                                        {
                                            notifyVm.IntAccountId = notifyReveiverList.IntAccountId;
                                            notifyVm.IntEmployeeId = notifyReveiverList.IntEmployeeId;
                                            notifyVm.IntReciverId = notifyReveiverList.IntDottedSupervisor;
                                            notifyVm.StrReceiverEmail = notifyReveiverList.strDottedSupervisorEmail;
                                            notifyVm.NotifyDetails = $"You have a reminder, {notifyReveiverList.StrEmployeeName} has 3 days late attendance.";
                                            notifyVm.StrReceiverEmail = "<!DOCTYPE html>" +
                                                                        "<html>" +
                                                                        "<body>" +
                                                                            "<div style=" + '"' + "font-size:12px" + '"' +
                                                                                "<p> You have a reminder, <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + notifyReveiverList.StrEmployeeName + "</a> " +
                                                                                "has 3 days late attendance. </p>" +
                                                                            "</div>" +
                                                                        "</body>" +
                                                                        "</html>";
                                            notifyLogs.Add(notifyVm);
                                        }
                                    }

                                    foreach (BackgroundNotifyCommonVM notifyVM in notifyLogs)
                                    {
                                        bool notify = await notifyscopped.BackgroundNotifyCommon(notifyVM);
                                        if (!notify)
                                        {
                                            NotifySendFailedLog notifySend = new NotifySendFailedLog
                                            {
                                                IntEmployeeId = notifyVM.IntEmployeeId,
                                                IntFeatureTableAutoId = notifyVM.IntFeatureTableAutoId,
                                                StrFeature = notifyVM.NotifyTitle,
                                                DteCreatedAt = DateTime.Now,
                                                IntCreatedBy = 1
                                            };
                                            await _context.NotifySendFailedLogs.AddAsync(notifySend);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await Task.Delay(1000 * 60 * 60, stoppingToken);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}