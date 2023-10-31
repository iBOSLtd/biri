using Microsoft.Data.SqlClient;
using PeopleDesk.Data;
using PeopleDesk.Helper;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Services.SAAS
{
    public class PeopleDeskDDLService : IPeopleDeskDDLService
    {
        private readonly PeopleDeskContext _context;
        DataTable dt = new DataTable();
        public PeopleDeskDDLService(PeopleDeskContext _context)
        {
            this._context = _context;
        }
        public async Task<DataTable> PeopleDeskAllDDL(string? DDLType, long? AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? intId, string? AutoEmployeeCode, bool? IsView, int? IntMonth, long? IntYear, long? IntWorkplaceId, long? IntPayrollGroupId, long? ParentTerritoryId, string? SearchTxt)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPeopleDeskAllDDL";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strDDLType", DDLType);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceId", IntWorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", IntPayrollGroupId);
                        sqlCmd.Parameters.AddWithValue("@strEmployeeCode", AutoEmployeeCode);
                        sqlCmd.Parameters.AddWithValue("@intId", intId);
                        sqlCmd.Parameters.AddWithValue("@isView", IsView);
                        sqlCmd.Parameters.AddWithValue("@intMonthId", IntMonth);
                        sqlCmd.Parameters.AddWithValue("@intYearId", IntYear);
                        sqlCmd.Parameters.AddWithValue("@ParentTerritoryId", ParentTerritoryId);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", SearchTxt);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                ////long s = dt.Rows.Count;

                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public async Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId,
            DateTime? FromDate, DateTime? ToDate, long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount,
            long? MovementTypeId, DateTime? ApplicationDate, int? StatusId, long? LoggedEmployeeId)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprAllLandingPageData";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strTableName", TableName);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intId", intId);
                        sqlCmd.Parameters.AddWithValue("@intStatusId", intStatusId);
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", "");
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@LoanTypeId", LoanTypeId);
                        sqlCmd.Parameters.AddWithValue("@DeptId", DeptId);
                        sqlCmd.Parameters.AddWithValue("@DesigId", DesigId);
                        sqlCmd.Parameters.AddWithValue("@EmpId", EmpId);
                        sqlCmd.Parameters.AddWithValue("@MinimumAmount", MinimumAmount);
                        sqlCmd.Parameters.AddWithValue("@MaximumAmount", MaximumAmount);
                        sqlCmd.Parameters.AddWithValue("@MovementTypeId", MovementTypeId);
                        sqlCmd.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@StatusId", StatusId);
                        sqlCmd.Parameters.AddWithValue("@intLoggedEmployeeId", LoggedEmployeeId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
    }
}


