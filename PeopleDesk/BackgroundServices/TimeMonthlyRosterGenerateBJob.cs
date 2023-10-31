using Dapper;
using NCrontab;
using System.Data;
using PeopleDesk.Helper;
using System.Data.SqlClient;

namespace PeopleDesk.BackgroundServices
{
    public class TimeMonthlyRosterGenerateBJob : BackgroundService
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private DateTime firstDayOfNextMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 12, 1, 0).AddMonths(1);
        private string Schedule => "* * * */1 * *"; //First Day of the per month

        public TimeMonthlyRosterGenerateBJob()
        {
            _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(firstDayOfNextMonth);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    if (DateTime.Now > _nextRun)
                    {
                        await Process();
                        firstDayOfNextMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 12, 1, 0).AddMonths(1);
                        _nextRun = _schedule.GetNextOccurrence(firstDayOfNextMonth);
                    }
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken); //2 hours delay
                }
                catch (Exception ex)
                {
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task Process()
        {
            try
            {
                using var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprTimeMonthlyRosterGenerateAllForJob";
                await connection.QueryAsync(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
            catch (Exception ex)
            {
            }
        }
    }
}