using Dapper;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using Quartz;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PeopleDesk.Extensions
{   //for cron https://www.freeformatter.com/cron-expression-generator-quartz.html
    public static class QuartzExtension
    {
        public static void StartQuartzJob(this IServiceCollection services)
        {
            TimeZoneInfo bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");

            /*==================> Step 1 Entry job key start <=====================*/

            var yearlyLeaveBalanceKey = new JobKey("yearlyLeaveBalance");
            var monthlyRosterGenerateKey = new JobKey("monthlyRosterGenerate");
            var inTimeOutTimeUpdateInAttendanceSummaryForJobKey = new JobKey("inTimeOutTimeUpdateInAttendanceSummaryForJob");


            /* Job key entry end */
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                /* =================> Step 2 Entry trigger start <===============*/

                q.AddJob<MonthlyRoasterGenerate>(o => o.WithIdentity(monthlyRosterGenerateKey));
                q.AddJob<YearlyLeaveBalanceGenerate>(o => o.WithIdentity(yearlyLeaveBalanceKey));
                q.AddJob<InTimeOutTimeUpdateInAttendanceSummaryForJob>(o => o.WithIdentity(inTimeOutTimeUpdateInAttendanceSummaryForJobKey));

                /* trigger end */

                /* ==================> Step 3 start trigger <====================== */

                q.AddTrigger(opts => opts.ForJob(monthlyRosterGenerateKey).WithIdentity("monthlyRosterGenerate-trigger")
                                            .WithSchedule(CronScheduleBuilder.CronSchedule("2 2 0 1 * ? *").InTimeZone(bdTimeZone)));

                q.AddTrigger(opts => opts.ForJob(yearlyLeaveBalanceKey).WithIdentity("yearlyLeaveBalance-trigger")
                                            .WithSchedule(CronScheduleBuilder.CronSchedule("3 3 3 1 JAN ? *").InTimeZone(bdTimeZone)));  
                
                q.AddTrigger(opts => opts.ForJob(inTimeOutTimeUpdateInAttendanceSummaryForJobKey).WithIdentity("inTimeOutTimeUpdateInAttendanceSummaryForJob-trigger")
                                            .WithSchedule(CronScheduleBuilder.CronSchedule("0 50 23 * * ?").InTimeZone(bdTimeZone)));


                /* End trigger */

            });

            services.AddQuartzHostedService(p => p.WaitForJobsToComplete = true);
        }
    }

   

    [DisallowConcurrentExecution]
    public class MonthlyRoasterGenerate : IJob
    {
        private readonly PeopleDeskContext _linqContext;
        public MonthlyRoasterGenerate(PeopleDeskContext _linqContext)
        {
            this._linqContext = _linqContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //calling sp 
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprTimeMonthlyRosterGenerateAllForJob";
                    await connection.QueryAsync(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }

                //saving record
                var sucess = new BackgroundJobSuccessHistory()
                {
                    StrStatus = $"Success at bd time {DateTimeExtend.BD};",
                    StrModuleName = "Monthly RosterGenerate All ForJob",
                    DteLastExcute = DateTimeExtend.BD
                };

                await _linqContext.BackgroundJobSuccessHistories.AddAsync(sucess);
                await _linqContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                var fail = new BackgroundJobFailureHistory()
                {
                    StrStatus = $"{DateTimeExtend.BD} - {ex.Message};",
                    StrModuleName = "Monthly RosterGenerate All ForJob",
                    DteLastExcute = DateTimeExtend.BD
                };

                await _linqContext.BackgroundJobFailureHistories.AddAsync(fail);
                await _linqContext.SaveChangesAsync();
            }
        }
    }

    //Yearly leave generate
    [DisallowConcurrentExecution]
    public class YearlyLeaveBalanceGenerate : IJob
    {
        private readonly PeopleDeskContext _linqContext;
        public YearlyLeaveBalanceGenerate(PeopleDeskContext _linqContext)
        {
            this._linqContext = _linqContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //calling sp 
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLeaveBalanceGenerateForSchedule";
                    await connection.QueryAsync(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }

                //saving record
                var sucess = new BackgroundJobSuccessHistory()
                {
                    StrStatus = $"Success at bd time {DateTimeExtend.BD};",
                    StrModuleName = "Yearly Leave Balance Generate",
                    DteLastExcute = DateTimeExtend.BD
                };

                await _linqContext.BackgroundJobSuccessHistories.AddAsync(sucess);
                await _linqContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                var fail = new BackgroundJobFailureHistory()
                {
                    StrStatus = $"{DateTimeExtend.BD} - {ex.Message};",
                    StrModuleName = "Yearly Leave Balance Generate",
                    DteLastExcute = DateTimeExtend.BD
                };

                await _linqContext.BackgroundJobFailureHistories.AddAsync(fail);
                await _linqContext.SaveChangesAsync();
            }
        }
    }

    //Yearly leave generate
    [DisallowConcurrentExecution]
    public class InTimeOutTimeUpdateInAttendanceSummaryForJob : IJob
    {
        private readonly PeopleDeskContext _linqContext;
        public InTimeOutTimeUpdateInAttendanceSummaryForJob(PeopleDeskContext _linqContext)
        {
            this._linqContext = _linqContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                bool isManual = true;
                DateTime FromDate = DateTime.Now.AddDays(-10);
                DateTime ToDate = DateTime.Now;
                int AccountId = 0;
                int EmployeeId = 0;

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprInTimeOutTimeUpdateInAttendanceSummaryForJob";
                    await connection.QueryAsync(sql, new { isManual, FromDate, ToDate, AccountId, EmployeeId }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
