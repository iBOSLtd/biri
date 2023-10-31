using Dapper;
using PeopleDesk.Helper;
using System.Data;
using System.Data.SqlClient;

namespace PeopleDesk.BackgroundServices
{
    public class SalaryProcessBJob : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                    string sql = "saas.sprPyrSalaryGenereateProcess";
                    await connection.QueryAsync(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
                catch (Exception ex)
                {
                }
                await Task.Delay(1000 * 60 * 2, stoppingToken);
            }
        }
    }
}