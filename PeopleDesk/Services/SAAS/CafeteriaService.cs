using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.SAAS.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PeopleDesk.Services.SAAS
{
    public class CafeteriaService : ICafeteriaService
    {
        private readonly PeopleDeskContext _context;
        DataTable dt = new DataTable();
        public CafeteriaService(PeopleDeskContext _context)
        {
            this._context = _context;
        }

        #region ============== Food Corner ===============

        public async Task<DataTable> GetCafeteriaMenuListReport(long LoginBy)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprMenuList";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intLoginBy", LoginBy);
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


        public async Task<MessageHelperUpdate> EditCafeteriaMenuList(MenuListOfFoodCornerEditCommonViewModel obj)
        {
            try
            {
                List<CafMenuListOfFoodCorner> menuEditList = new List<CafMenuListOfFoodCorner>();
                foreach (var item in obj.menuListViewModelObj)
                {
                    CafMenuListOfFoodCorner menuData = await _context.CafMenuListOfFoodCorners.FirstOrDefaultAsync(x => x.IntAutoId == item.id);
                    menuData.StrMenu = item.menu;
                    menuData.DteUpdatedAt = DateTime.Now;
                    menuData.IntUpdatedBy = obj.updateBy;

                    menuEditList.Add(menuData);

                }
                _context.CafMenuListOfFoodCorners.UpdateRange(menuEditList);
                await _context.SaveChangesAsync();

                return new MessageHelperUpdate();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CafeteriaEntry(long PartId, DateTime ToDate, long EnrollId, long TypeId, long MealOption, long MealFor
            , long CountMeal, long isOwnGuest, long isPayable, string? Narration, long ActionBy)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                if (ToDate.ToString("MM-yyyy") != currentDate.ToString("MM-yyyy"))
                {
                    throw new Exception("You have to select current month.");
                }

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprCafeteria";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intPart", PartId);
                        sqlCmd.Parameters.AddWithValue("@tdate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@intEnroll", EnrollId);
                        sqlCmd.Parameters.AddWithValue("@intType", TypeId);
                        sqlCmd.Parameters.AddWithValue("@intMealOption", MealOption);
                        sqlCmd.Parameters.AddWithValue("@intMealFor", MealFor);
                        sqlCmd.Parameters.AddWithValue("@intCountMeal", CountMeal);
                        sqlCmd.Parameters.AddWithValue("@isOwnGuest", isOwnGuest);
                        sqlCmd.Parameters.AddWithValue("@isPayable", isPayable);
                        sqlCmd.Parameters.AddWithValue("@strNarration", Narration);
                        sqlCmd.Parameters.AddWithValue("@intActionBy", ActionBy);
                        sqlCmd.Parameters.Add("@strmsgvcd", SqlDbType.VarChar, 100);
                        sqlCmd.Parameters["@strmsgvcd"].Direction = ParameterDirection.Output;

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                        if (msg.StatusCode == 500)
                        {
                            throw new Exception(msg.Message);
                        }
                        else
                        {
                            return msg;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> GetPendingAndConsumeMealReport(long PartId, long EnrollId)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprCafeteriaR";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intPart", PartId);
                        sqlCmd.Parameters.AddWithValue("@intEnroll", EnrollId);
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
        public async Task<List<GetDailyCafeteriaReportViewModel>> GetDailyCafeteriaReport([Required] long businessUnitId, [Required] DateTime mealDate)
        {
            var result = await Task.FromResult((from cd in _context.CafCafeteriaDetails
                                                join emp in _context.EmpEmployeeBasicInfos on (long)cd.IntEnroll equals emp.IntEmployeeBasicInfoId
                                                where emp.IntBusinessUnitId == businessUnitId
                                                && emp.IsActive == true
                                                && cd.DteMeal.Value.Date == mealDate.Date
                                                && cd.IntCountMeal > 0
                                                select new
                                                {
                                                    emp.IntEmployeeBasicInfoId,
                                                    //emp.StrReferenceid,
                                                    emp.StrEmployeeCode,
                                                    emp.StrEmployeeName,
                                                    emp.IntDesignationId,
                                                    emp.IntDepartmentId,
                                                    DteMeal = cd.DteMeal.Value.Date,
                                                    cd.IntCountMeal
                                                }).GroupBy(x => new
                                                {
                                                    x.IntEmployeeBasicInfoId,
                                                    // x.StrReferenceid,
                                                    x.StrEmployeeCode,
                                                    x.StrEmployeeName,
                                                    x.IntDesignationId,
                                                    x.IntDepartmentId,
                                                    x.DteMeal
                                                }).Select(x => new GetDailyCafeteriaReportViewModel
                                                {
                                                    EmployeeId = x.Key.IntEmployeeBasicInfoId,
                                                    //Referenceid = x.Key.StrReferenceid,
                                                    EmployeeCode = x.Key.StrEmployeeCode,
                                                    EmployeeFullName = x.Key.StrEmployeeName,
                                                    DesignationId = x.Key.IntDesignationId,
                                                    DesignationName = _context.MasterDesignations.Where(a => a.IntDesignationId == x.Key.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                                    DepartmentId = x.Key.IntDepartmentId,
                                                    DepartmentName = _context.MasterDepartments.Where(a => a.IntDepartmentId == x.Key.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                                    MealDate = x.Key.DteMeal.Date.ToString("dd MMM, yyyy"),
                                                    MealCount = x.Sum(a => a.IntCountMeal)
                                                }).ToList());
            return result;
        }

        public async Task<List<GetDailyCafeteriaReportViewModel>> MonthlyCafeteriaReport([Required] long businessUnitId, long workPlaceId, DateTime fromDate, DateTime toDate)
        {
            List<GetDailyCafeteriaReportViewModel> result = await (from cd in _context.CafCafeteriaDetails
                                                                   join emp in _context.EmpEmployeeBasicInfos on (long)cd.IntEnroll equals emp.IntEmployeeBasicInfoId
                                                                   where emp.IntBusinessUnitId == businessUnitId
                                                                   && (workPlaceId == 0 ? true : emp.IntWorkplaceId == workPlaceId)
                                                                   && emp.IsActive == true
                                                                   && cd.DteMeal.Value.Date >= fromDate.Date && cd.DteMeal.Value.Date <= toDate.Date
                                                                   && cd.IntCountMeal > 0
                                                                   select new
                                                                   {
                                                                       emp.IntEmployeeBasicInfoId,
                                                                       emp.StrEmployeeCode,
                                                                       emp.StrEmployeeName,
                                                                       emp.IntDesignationId,
                                                                       emp.IntDepartmentId,
                                                                       DteMeal = cd.DteMeal.Value.Date,
                                                                       cd.IntCountMeal
                                                                   }).GroupBy(x => new
                                                                   {
                                                                       x.IntEmployeeBasicInfoId,
                                                                       x.StrEmployeeCode,
                                                                       x.StrEmployeeName,
                                                                       x.IntDesignationId,
                                                                       x.IntDepartmentId
                                                                   }).Select(x => new GetDailyCafeteriaReportViewModel
                                                                   {
                                                                       EmployeeId = x.Key.IntEmployeeBasicInfoId,
                                                                       EmployeeCode = x.Key.StrEmployeeCode,
                                                                       EmployeeFullName = x.Key.StrEmployeeName,
                                                                       DesignationId = x.Key.IntDesignationId,
                                                                       DesignationName = _context.MasterDesignations.Where(a => a.IntDesignationId == x.Key.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                                                       DepartmentId = x.Key.IntDepartmentId,
                                                                       DepartmentName = _context.MasterDepartments.Where(a => a.IntDepartmentId == x.Key.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                                                       MealCount = x.Sum(x => x.IntCountMeal)
                                                                   }).ToListAsync();

            return result;
        }
        #endregion
    }
}
