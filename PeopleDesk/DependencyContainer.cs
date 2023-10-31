using AspNetCoreRateLimit;
using EmployeeMgmtAPI.Services.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using PeopleDesk.BackgroundServices;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.Cache;
using PeopleDesk.Services.Global;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.Master;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.MasterData;
using PeopleDesk.Services.MasterData.Interfaces;
using PeopleDesk.Services.PushNotify;
using PeopleDesk.Services.PushNotify.Interfaces;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;
using PeopleDesk.Services.SignalR.Interfaces;

namespace PeopleDesk
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services, WebApplicationBuilder builder, ConfigurationManager Configuration)
        {
            services.AddTransient<IQRYDataForReportService, QRYDataForReportService>();
            services.AddTransient<IMasterService, MasterService>();
            services.AddTransient<ISaasMasterService, SaasMasterService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IPayrollService, PayrollService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IPeopleDeskDDLService, PeopleDeskDDLService>();
            services.AddTransient<ITaskManagementService, TaskManagementService>();
            services.AddTransient<ICafeteriaService, CafeteriaService>();
            services.AddTransient<ILeaveMovementService, LeaveMovementService>();
            services.AddTransient<IApprovalPipelineService, ApprovalPipelineService>();
            services.AddTransient<IOrganogramService, OrganogramService>();
            services.AddTransient<iEmailService, EmailService>();
            services.AddTransient<IAssetManagement, AssetManagement>();

            services.AddTransient<IPdfAndExcelService, PdfAndExcelService>();
            services.AddTransient<IPdfTemplateGeneratorService, PdfTemplateGeneratorService>();
            //services.AddTransient<ILeaveMovementService, LeaveMovementService>();
            //services.AddTransient<IHCMService, HCMService>();
            //services.AddTransient<IReportService, ReportService>();
            //services.AddTransient<IPayrollManagement, PayrollManagement>();
            services.AddTransient<IQRYDataForReportService, QRYDataForReportService>();
            //services.AddTransient<IBonusManagement, BonusManagement>();
            services.AddTransient<IDashboardService, DashboardService>();
            //services.AddTransient<IOrganogramService, OrganogramService>();
            //services.AddTransient<IMenuAndFeatureService, MenuAndFeatureService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IPushNotifyService, PushNotifyService>();
            //services.AddTransient<IEmployeeDocumentService, EmployeeDocumentService>();
            //services.AddTransient<IFileSaveService, FileSaveService>();
            //services.AddTransient<IAssetService, AssetService>();
            //services.AddTransient<IShippingMgmt, ShippingMgmt>();
            //services.AddTransient<ICRAllCRUD, CRAllCRUD>();
            //services.AddTransient<IBankBranch, BankBranch>();
            //services.AddTransient<IVesselCertificate, VesselCertificate>();
            //services.AddTransient<IMedicalApplicationService, MedicalApplicationService>();
            //services.AddTransient<ISignOffService, SignOffService>();
            services.AddTransient<ITrainingService, TrainingService>();
            services.AddTransient<IEmployeeAllLanding, EmployeeAllLanding>();
            //services.AddTransient<IDistributedCacheService, DistributedCacheService>();
            services.AddTransient<IReportrdlc, Reportrdlc>();

            //services.AddHostedService<MultiBackgroundJobService>();

            #region=========Background Service=============
            if (builder.Environment.IsProduction())
            {
                services.AddHostedService<PunchMechineRawDataProcessBJob>();
                services.AddHostedService<AttendanceProcessToSummaryBJob>();
                services.AddHostedService<InOutTimeUpdateInAttendanceSummaryForBJob>();
                
                services.AddHostedService<ArearSalaryProcessBJob>();
                //services.AddHostedService<EarnLeaveProcessBJob>();
                //services.AddHostedService<TimeMonthlyRosterGenerateBJob>();
                services.AddHostedService<IncrementSalaryAssignBJob>();
                services.AddHostedService<PromotionDataProcessBJob>();

                //services.AddHostedService<EmpWorkYearAnniversaryBackgroundService>();
                //services.AddHostedService<AttendanceLateNotifyBackgroundService>();
                //services.AddHostedService<BirthdayNotificationBackgroundService>();
                services.AddHostedService<SchedulePunchGrabJob>();
            }
            services.AddHostedService<SalaryProcessBJob>();
            #endregion

            //services.AddTransient<IPunchGraphManager, PunchGraphManager>();
            #region ==== Rate limit ======
            //services.AddMemoryCache();

            ////load general configuration from appsettings.json
            //services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            ////load ip rules from appsettings.json
            //services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            //// inject counter and rules stores
            //services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            //// Add framework services.
            //services.AddMvc();

            //// configuration (resolvers, counter key builders)
            //services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            //services.AddTransient<CacheExpiration>();
            //services.AddTransient<ICacheService, CacheService>();
            #endregion === Close ========
        }
    }
}