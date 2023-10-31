using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SignalR.Interfaces;

namespace PeopleDesk.BackgroundServices
{
    public class BirthdayNotificationBJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BirthdayNotificationBJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    PeopleDeskContext _context = scope.ServiceProvider.GetService<PeopleDeskContext>();

                    NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters
                        .Where(x => x.IsActive == true && x.StrNcategoryName.ToLower() == "Birthday".ToLower())
                        .AsQueryable().AsNoTracking().FirstOrDefaultAsync();
                    if (NotificationPermissionMaster != null)
                    {
                        var empDetails = await (from empb in _context.EmpEmployeeBasicInfos
                                                where empb.DteDateOfBirth.Value.Month == DateTime.Now.Month
                                                && empb.DteDateOfBirth.Value.Day == DateTime.Now.Day && empb.IsActive == true
                                                join empd in _context.EmpEmployeeBasicInfoDetails on empb.IntEmployeeBasicInfoId equals empd.IntEmployeeId
                                                join acc in _context.Accounts on empb.IntAccountId equals acc.IntAccountId
                                                where acc.IsActive == true
                                                select new
                                                {
                                                    IntAccountId = empb.IntAccountId,
                                                    IntEmployeeId = empb.IntEmployeeBasicInfoId,
                                                    StrEmployeeName = empb.StrEmployeeName,
                                                    StrOfficeMail = empd.StrOfficeMail != null ? empd.StrOfficeMail : empd.StrPersonalMail,
                                                }).AsQueryable().AsNoTracking().ToListAsync();

                        BackgroundNotifyCommonVM notifyVm = new()
                        {
                            StrCategoryName = "Birthday",
                            IntModuleId = 1,
                            StrModule = "Happy Birthday",
                            StrFeature = "Happy Birthday",
                            IntFeatureTableAutoId = 0,
                            NotifyTitle = "Happy Birthday",
                            IsCommon = false,
                            MailSubject = "Happy Birthday ",
                        };
                        var notifyService = scope.ServiceProvider.GetService<INotificationService>();
                        foreach (var empInfo in empDetails)
                        {
                            notifyVm.IntAccountId = empInfo.IntAccountId;
                            notifyVm.IntEmployeeId = empInfo.IntEmployeeId;
                            notifyVm.IntReciverId = empInfo.IntEmployeeId;
                            notifyVm.StrReceiverEmail = empInfo.StrOfficeMail;
                            notifyVm.NotifyDetails = $"Dear {empInfo.StrEmployeeName}, wishing you the best on your birthday and everything good in the year ahead. Happy Birthday from iBOS Limited.";
                            notifyVm.MailBody = "<!DOCTYPE html>" +
                                                    "<html>" +
                                                    "<body>" +
                                                        "<div style=" + '"' + "font-size:12px" + '"' +
                                                            "<p>Dear  <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + empInfo.StrEmployeeName + "</a> </p>" +
                                                            "<p>wishing you the best on your birthday and everything good in the year ahead.</p>" +
                                                            "<p>Happy Birthday from iBOS Limited.</p>" +
                                                        "</div>" +
                                                    "</body>" +
                                                 "</html>";

                            bool res = await notifyService.BackgroundNotifyCommon(notifyVm);
                        }
                    }
                }
                await Task.Delay(1000 * 60 * 60 * 24);
            }
        }
    }
}