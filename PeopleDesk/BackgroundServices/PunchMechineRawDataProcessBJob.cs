using Dapper;
using Microsoft.Data.SqlClient;
using PeopleDesk.Helper;
using System.Data;

namespace PeopleDesk.BackgroundServices
{
    public class PunchMechineRawDataProcessBJob : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                    string sql = "saas.sprTimePunchMechineRawDataProcessForJob";
                    var values = new
                    {
                        IsProcessAll = 0
                    };
                    await connection.QueryAsync(sql, values, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
                catch (Exception ex)
                {
                }
                await Task.Delay(1000 * 60 * 2, stoppingToken);
            }
        }
    }
}