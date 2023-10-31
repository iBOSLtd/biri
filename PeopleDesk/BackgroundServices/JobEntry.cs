using Dapper;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using System.Data;
using System.Data.SqlClient;

namespace PeopleDesk.BackgroundServices
{
    public class JobEntry
    {
        public static void PrintMessage1()
        {
            Console.WriteLine($"Print from function 1 at  {DateTimeExtend.BD}");
        }

        public static void PrintMessage2()
        {
            Console.WriteLine($"Print from function 2  at  {DateTimeExtend.BD}");
        }

        public static void sptest()
        {
            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.test_sp";
                connection.Query(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }


        public static async void linqtest()
        {
            using (var context = new PeopleDeskContext())
            {
                var successHistory = new BackgroundJobSuccessHistory
                {
                    DteLastExcute = DateTimeExtend.BD,
                    StrModuleName = "test",
                    StrStatus = "from linq"

                };

                await context.BackgroundJobSuccessHistories.AddAsync(successHistory);
                await context.SaveChangesAsync();
            }

        }
        public static async void linqtest5()
        {
            using (var context = new PeopleDeskContext())
            {
                var successHistory = new BackgroundJobSuccessHistory
                {
                    DteLastExcute = DateTimeExtend.BD,
                    StrModuleName = "linq every 5 minutes",
                    StrStatus = "from linq"

                };

                await context.BackgroundJobSuccessHistories.AddAsync(successHistory);
                await context.SaveChangesAsync();
            }

        }
        public static void sptest3()
        {
            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.test_sp";
                connection.Query(sql, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
