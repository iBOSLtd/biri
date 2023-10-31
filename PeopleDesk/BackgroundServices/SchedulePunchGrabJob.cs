using Dapper;
using Microsoft.Data.SqlClient;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using System.Data;

namespace PeopleDesk.BackgroundServices
{
    public class SchedulePunchGrabJob : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            int delayMilliseconds = (1000 * 60) * 10; //10 minutes
            string response = string.Empty;
            string sql = "saas.sprSchedulePunchGrabJob";

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
                    await connection.QueryAsync(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }

                await Task.Delay(delayMilliseconds, stoppingToken);
            }
        }
    }
}
