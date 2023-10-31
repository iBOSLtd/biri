using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SignalR.Interfaces;

namespace PeopleDesk.BackgroundServices
{
    public class EmpWorkYearAnniversaryBJob : BackgroundService
    {
        private readonly ILogger<EmpWorkYearAnniversaryBJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EmpWorkYearAnniversaryBJob(ILogger<EmpWorkYearAnniversaryBJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetService<PeopleDeskContext>();

                    NotificationPermissionMaster NotificationPermissionMaster = await _context.NotificationPermissionMasters
                           .Where(x => x.IsActive == true && x.StrNcategoryName.ToLower() == "Employment Anniversary".ToLower())
                           .AsQueryable().AsNoTracking().FirstOrDefaultAsync();

                    if (NotificationPermissionMaster != null)
                    {
                        var empEmployeeBasicInfoDetails = await (from empBasic in _context.EmpEmployeeBasicInfos
                                                                 where empBasic.IsActive == true
                                                                 && empBasic.DteJoiningDate.Value.Day == DateTime.Now.Day
                                                                 && empBasic.DteJoiningDate.Value.Month == DateTime.Now.Month
                                                                 && empBasic.DteJoiningDate < DateTime.Now
                                                                 && empBasic.StrEmploymentType.ToLower() == "permanent"
                                                                 join empBasicDetail in _context.EmpEmployeeBasicInfoDetails
                                                                 on empBasic.IntEmployeeBasicInfoId equals empBasicDetail.IntEmployeeId
                                                                 join acc in _context.Accounts on empBasic.IntAccountId equals acc.IntAccountId
                                                                 where empBasicDetail.IsActive == true && acc.IsActive == true &&
                                                                 (empBasicDetail.IntEmployeeStatusId == 1 || empBasicDetail.IntEmployeeStatusId == 4)
                                                                 select new
                                                                 {
                                                                     StrEmployeeName = empBasic.StrEmployeeName,
                                                                     IntAccountId = empBasic.IntAccountId,
                                                                     IntEmployeeId = empBasic.IntEmployeeBasicInfoId,
                                                                     StrOfficeMail = empBasicDetail.StrOfficeMail,
                                                                     dtej = empBasic.DteJoiningDate,
                                                                     TotalYear = (DateTime.Now.Year - empBasic.DteJoiningDate.Value.Year)
                                                                 }).AsQueryable().AsNoTracking().ToListAsync();
                        // _logger.LogInformation("hello   dfjkjfkj");
                        BackgroundNotifyCommonVM notifyVm = new()
                        {
                            StrCategoryName = "Employment Anniversary",
                            IntModuleId = 1,
                            StrModule = "Anniversary Module",
                            StrFeature = "Anniversary Message",
                            IntFeatureTableAutoId = 0,
                            NotifyTitle = "Workiversary!!!",
                            IsCommon = false,
                            MailSubject = "Workiversary!!!",
                        };
                        var notifyService = scope.ServiceProvider.GetService<INotificationService>();
                        foreach (var empInfo in empEmployeeBasicInfoDetails)
                        {
                            notifyVm.IntAccountId = empInfo.IntAccountId;
                            notifyVm.IntEmployeeId = empInfo.IntEmployeeId;
                            notifyVm.IntReciverId = empInfo.IntEmployeeId;
                            notifyVm.StrReceiverEmail = empInfo.StrOfficeMail;
                            notifyVm.NotifyDetails = $"Today is {empInfo.TotalYear} years of joining in iBOS Limited, which is worth celebrating! Happy work anniversary. We are so glad you chose to join us and that you chose to stay with us.";
                            notifyVm.MailBody = "<!DOCTYPE html>" +
                                                "<html>" +
                                                "<body>" +
                                                    "<div style=" + '"' + "font-size:12px" + '"' +
                                                        "<p>Today is <a href=" + '"' + '"' + "style=" + '"' + "color:blue" + '"' + ">" + empInfo.TotalYear + "</a> years of joining in iBOS Limited,</p>" +
                                                        "<p>which is worth celebrating!</p>" +
                                                        "<p>Happy work anniversary.</p>" +
                                                        "<p>" +
                                                            "We are so glad you chose to join us and that you chose to stay with us." +
                                                        "</p>" +
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