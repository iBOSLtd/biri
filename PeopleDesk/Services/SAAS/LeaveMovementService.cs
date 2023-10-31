using Microsoft.Data.SqlClient;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
using System.Text.Json;

namespace PeopleDesk.Services.SAAS
{
    public class LeaveMovementService : ILeaveMovementService
    {
        DataTable dt = new DataTable();
        private readonly PeopleDeskContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        public LeaveMovementService(PeopleDeskContext _context, IEmployeeService employeeService, IApprovalPipelineService approvalPipelineService)
        {
            this._context = _context;
            this._employeeService = employeeService;
            this._approvalPipelineService = approvalPipelineService;
        }

        #region ===> LEAVE <===
        public async Task<DataTable> GetEmployeeLeaveBalanceAndHistory(long? EmployeeId, string? ViewType, long? LeaveTypeId, DateTime? ApplicationDate, long? IntYear, int? StatusId)
        {
            try
            {

                //ViewType=>LeaveBalance
                //ViewType=>LeaveHistory
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeLeaveBalanceAndHistory";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@strViewType", ViewType);
                        sqlCmd.Parameters.AddWithValue("@LeaveTypeId", LeaveTypeId);
                        sqlCmd.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@inYerar", IntYear);
                        //sqlCmd.Parameters.AddWithValue("@FromDate", FromDate);
                        //sqlCmd.Parameters.AddWithValue("@ToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@StatusId", StatusId);
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

        public async Task<List<LeaveDataSetDTO>> GetAllLeaveApplicatonListForApprove(LeaveDTO model)
        {
            try
            {
                //ViewType = 1 for pending 2 for approved 3 for rejected
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLeaveApplicationListForApprove";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intViewTypeId", model.ViewType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", model.EmployeeId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<LeaveDataSetDTO> list = new List<LeaveDataSetDTO>();

                foreach (DataRow row in dt.Rows)
                {
                    long applicationId = Convert.ToInt32(row["intApplicationId"].ToString());
                    LeaveDataSetDTO isExists = list.Where(x => x.intApplicationId == applicationId).FirstOrDefault();

                    if (isExists != null)
                    {
                        list.Remove(isExists);
                    }

                    LeaveDataSetDTO obj = new LeaveDataSetDTO
                    {
                        intApplicationId = Convert.ToInt32(row["intApplicationId"].ToString()),
                        intAccountId = Convert.ToInt32(row["intAccountId"].ToString()),
                        intBusinessUnitId = Convert.ToInt32(row["intBusinessUnitId"].ToString()),
                        intWorkplaceGroupId = Convert.ToInt32(row["intWorkplaceGroupId"].ToString()),
                        intEmployeeId = Convert.ToInt32(row["intEmployeeId"].ToString()),
                        intApproverId = Convert.ToInt32(row["intApproverId"].ToString()),
                        strEmployeeCode = row["strEmployeeCode"].ToString(),
                        strEmployeeName = row["strEmployeeName"].ToString(),
                        strLeaveReason = row["strLeaveReason"].ToString(),
                        dteAppliedFromDate = Convert.ToDateTime(row["dteAppliedFromDate"].ToString()),
                        dteAppliedToDate = Convert.ToDateTime(row["dteAppliedToDate"].ToString()),
                        strDocumentFile = row["strDocumentFile"].ToString(),
                        intLeaveTypeId = Convert.ToInt32(row["intLeaveTypeId"].ToString()),
                        strLeaveType = row["strLeaveType"].ToString(),
                        intTotalConsumeLeave = Convert.ToInt32(row["intTotalConsumeLeave"].ToString()),
                        intRemainingDays = Convert.ToInt32(row["intRemainingDays"].ToString()),
                        intAllTypeLeaveConsume = Convert.ToInt32(row["intAllTypeLeaveConsume"].ToString()),
                        strDepartmentName = row["strDepartmentName"].ToString(),
                        strDesignationName = row["strDesignationName"].ToString(),
                        intDesignationId = Convert.ToInt64(row["intDepartmentId"].ToString()),
                        intDepartmentId = Convert.ToInt64(row["intDesignationId"].ToString()),
                        strEmploymentTypeName = row["strEmploymentTypeName"].ToString(),
                        strAddressDuetoLeave = row["strAddressDuetoLeave"].ToString(),
                        dteApplicationDate = Convert.ToDateTime(row["dteApplicationDate"].ToString()),
                        totalDays = Convert.ToInt32(row["totalDays"].ToString()),
                        strStatus = row["strStatus"].ToString(),
                        strViewAs = row["strViewAs"].ToString(),
                    };
                    list.Add(obj);
                }

                // SEARCHING 

                DateTime? fromDate = !string.IsNullOrEmpty(model.FromDate) ? Convert.ToDateTime(model.FromDate) : DateTime.Now;
                DateTime? toDate = !string.IsNullOrEmpty(model.ToDate) ? Convert.ToDateTime(model.ToDate) : DateTime.Now;

                list = (from app in list
                        where (model.WorkplaceGroupId > 0 ? app.intWorkplaceGroupId == model.WorkplaceGroupId : true
                        && model.DepartmentId > 0 ? app.intDepartmentId == model.DepartmentId : true
                        && model.DesignationId > 0 ? app.intDesignationId == model.DesignationId : true
                        && model.ApplicantId > 0 ? app.intEmployeeId == model.ApplicantId : true
                        && model.LeaveTypeId > 0 ? app.intLeaveTypeId == model.LeaveTypeId : true
                        && (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                        ? (fromDate.Value.Date <= app.dteAppliedFromDate.Value.Date && app.dteAppliedToDate.Value.Date <= toDate.Value.Date) : true)
                        select app).OrderByDescending(x => x.dteApplicationDate).ToList();

                return list;

            }
            catch (Exception ex)
            {
                return new List<LeaveDataSetDTO>();
            }
        }

        public async Task<LeaveDataSetDTO> GetSingleLeaveApplicatonListForApprove(long? ViewType, long? EmployeeId, long? ApplicationId)
        {
            try
            {
                //ViewType = 1 for pending 2 for approved 3 for rejected
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLeaveApplicationListForApprove";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intViewTypeId", ViewType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intApplicationId", ApplicationId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<LeaveDataSetDTO> list = new List<LeaveDataSetDTO>();
                LeaveDataSetDTO returnObj = new LeaveDataSetDTO();

                foreach (DataRow row in dt.Rows)
                {
                    long applicationId = Convert.ToInt32(row["intApplicationId"].ToString());
                    LeaveDataSetDTO isExists = list.Where(x => x.intApplicationId == applicationId).FirstOrDefault();

                    if (isExists != null)
                    {
                        list.Remove(isExists);
                    }

                    LeaveDataSetDTO obj = new LeaveDataSetDTO
                    {
                        intApplicationId = Convert.ToInt32(row["intApplicationId"].ToString()),
                        intAccountId = Convert.ToInt32(row["intAccountId"].ToString()),
                        intBusinessUnitId = Convert.ToInt32(row["intBusinessUnitId"].ToString()),
                        intWorkplaceGroupId = Convert.ToInt32(row["intWorkplaceGroupId"].ToString()),
                        intEmployeeId = Convert.ToInt32(row["intEmployeeId"].ToString()),
                        intApproverId = Convert.ToInt32(row["intApproverId"].ToString()),
                        strEmployeeCode = row["strEmployeeCode"].ToString(),
                        strEmployeeName = row["strEmployeeName"].ToString(),
                        strLeaveReason = row["strLeaveReason"].ToString(),
                        dteAppliedFromDate = Convert.ToDateTime(row["dteAppliedFromDate"].ToString()),
                        dteAppliedToDate = Convert.ToDateTime(row["dteAppliedToDate"].ToString()),
                        strDocumentFile = row["strDocumentFile"].ToString(),
                        intLeaveTypeId = Convert.ToInt32(row["intLeaveTypeId"].ToString()),
                        strLeaveType = row["strLeaveType"].ToString(),
                        intTotalConsumeLeave = Convert.ToInt32(row["intTotalConsumeLeave"].ToString()),
                        intAllTypeLeaveConsume = Convert.ToInt32(row["intAllTypeLeaveConsume"].ToString()),
                        strDepartmentName = row["strDepartmentName"].ToString(),
                        strDesignationName = row["strDesignationName"].ToString(),
                        intDesignationId = Convert.ToInt64(row["intDepartmentId"].ToString()),
                        intDepartmentId = Convert.ToInt64(row["intDesignationId"].ToString()),
                        strEmploymentTypeName = row["strEmploymentTypeName"].ToString(),
                        strAddressDuetoLeave = row["strAddressDuetoLeave"].ToString(),
                        dteApplicationDate = Convert.ToDateTime(row["dteApplicationDate"].ToString()),
                        totalDays = Convert.ToInt32(row["totalDays"].ToString()),
                        strStatus = row["strStatus"].ToString(),
                        strViewAs = row["strViewAs"].ToString(),
                    };
                    list.Add(obj);
                }

                returnObj = list.FirstOrDefault();
                return returnObj;
            }
            catch (Exception ex)
            {
                return new LeaveDataSetDTO();
            }
        }

        public async Task<MessageHelper> CRUDLeaveApplication(LeaveApplicationDTO obj)
        {
            try
            {
                var pipe = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(obj.EmployeeId, "Leave");

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLeaveApplication";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@intLeaveApplicationId", obj.LeaveApplicationId);
                        sqlCmd.Parameters.AddWithValue("@intLeaveTypeId", obj.LeaveTypeId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@dteApplicationDate", obj.ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@dteAppliedFromDate", obj.AppliedFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteAppliedToDate", obj.AppliedToDate);
                        sqlCmd.Parameters.AddWithValue("@intDocumentFile", obj.DocumentFile);
                        sqlCmd.Parameters.AddWithValue("@strLeaveReason", obj.LeaveReason);
                        sqlCmd.Parameters.AddWithValue("@strAddressDuetoLeave", obj.AddressDuetoLeave);
                        sqlCmd.Parameters.AddWithValue("@intInsertBy", obj.InsertBy);

                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipe.HeaderId);
                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipe.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", pipe.NextStageId);


                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        msg.AutoId = Convert.ToInt32(dt.Rows[0]["autoId"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<MessageHelper> LeaveApprove(List<LeaveAndMovementApprovedDTO> obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string jsonString = JsonSerializer.Serialize(obj);

                    string sql = "saas.sprLeaveApprovalProcedure";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region ===< MOVEMENT >===
        public async Task<MessageHelper> CRUDMovementApplication(MovementApplicationDTO obj)
        {
            try
            {
                if (obj.IntEmployeeId <= 0)
                {
                    throw new Exception("Employee Id is Not Provided");
                }

                PipelineStageInfoVM res = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(obj.IntEmployeeId, "Movement");

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprMovementApplication";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@intMovementAutoId", obj.MovementId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.IntEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intMovementTypeId", obj.MovementTypeId);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", obj.FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", obj.ToDate);
                        sqlCmd.Parameters.AddWithValue("@tmFromTime", obj.FromTime);
                        sqlCmd.Parameters.AddWithValue("@tmToTime", obj.ToTime);
                        sqlCmd.Parameters.AddWithValue("@strLocation", obj.Location);
                        sqlCmd.Parameters.AddWithValue("@strReason", obj.Reason);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@intInsertBy", obj.InsertBy);
                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", res.HeaderId);
                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", res.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", res.NextStageId);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        msg.AutoId = Convert.ToInt32(dt.Rows[0]["autoId"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MovementDataSetDTO>> GetAllMovementApplicatonListForApprove(LeaveDTO model)
        {
            try
            {
                //ViewType = 1 for pending 2 for approved 3 for rejected
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprMovementApplicationListForApprove";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intViewTypeId", model.ViewType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", model.EmployeeId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<MovementDataSetDTO> list = new List<MovementDataSetDTO>();

                foreach (DataRow row in dt.Rows)
                {
                    long applicationId = Convert.ToInt32(row["intMovementId"].ToString());
                    MovementDataSetDTO isExists = list.Where(x => x.intMovementId == applicationId).FirstOrDefault();

                    if (isExists != null)
                    {
                        list.Remove(isExists);
                    }

                    MovementDataSetDTO obj = new MovementDataSetDTO
                    {
                        intMovementId = Convert.ToInt32(row["intMovementId"].ToString()),
                        intBusinessUnitId = Convert.ToInt32(row["intBusinessUnitId"].ToString()),
                        intWorkplaceGroupId = Convert.ToInt32(row["intWorkplaceGroupId"].ToString()),
                        intEmployeeId = Convert.ToInt32(row["intEmployeeId"].ToString()),
                        intApproverId = Convert.ToInt32(row["intApproverId"].ToString()),
                        strEmployeeCode = row["strEmployeeCode"].ToString(),
                        strEmployeeName = row["strEmployeeName"].ToString(),
                        strReason = row["strReason"].ToString(),
                        dteFromDate = Convert.ToDateTime(row["dteFromDate"].ToString()),
                        dteToDate = Convert.ToDateTime(row["dteToDate"].ToString()),
                        dteFromTime = row["dteFromTime"].ToString(),
                        dteTimeTo = row["dteTimeTo"].ToString(),
                        strDepartmentName = row["strDepartmentName"].ToString(),
                        strDesignationName = row["strDesignationName"].ToString(),
                        intDesignationId = Convert.ToInt64(row["intDepartmentId"].ToString()),
                        intDepartmentId = Convert.ToInt64(row["intDesignationId"].ToString()),
                        strEmploymentTypeName = row["strEmploymentTypeName"].ToString(),
                        strMovementType = row["strMovementType"].ToString(),
                        dteApplicationDate = Convert.ToDateTime(row["dteApplicationDate"].ToString()),
                        strLocation = row["strLocation"].ToString(),
                        strStatus = row["strStatus"].ToString(),
                        strViewAs = row["strViewAs"].ToString(),
                    };
                    list.Add(obj);
                }

                list = (from app in list
                        where (model.WorkplaceGroupId > 0 ? app.intWorkplaceGroupId == model.WorkplaceGroupId : true
                        && model.DepartmentId > 0 ? app.intDepartmentId == model.DepartmentId : true
                        && model.DesignationId > 0 ? app.intDesignationId == model.DesignationId : true
                        && model.ApplicantId > 0 ? app.intEmployeeId == model.ApplicantId : true
                        && model.LeaveTypeId > 0 ? app.intMovementId == model.LeaveTypeId : true
                        && (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                        ? (app.dteApplicationDate.Value.Date >= Convert.ToDateTime(model.FromDate).Date && Convert.ToDateTime(model.ToDate).Date <= app.dteApplicationDate.Value.Date) : true)
                        select app).OrderByDescending(x => x.dteApplicationDate).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<MovementDataSetDTO>();
            }
        }
        public async Task<MovementDataSetDTO> GetSingleMovementApplicatonListForApprove(long? ViewType, long? EmployeeId, long? MovementId)
        {
            try
            {
                //ViewType = 1 for pending 2 for approved 3 for rejected
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprMovementApplicationListForApprove";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intViewTypeId", ViewType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intMovementId", MovementId);
                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                List<MovementDataSetDTO> list = new List<MovementDataSetDTO>();
                MovementDataSetDTO returnObj = new MovementDataSetDTO();

                foreach (DataRow row in dt.Rows)
                {
                    long applicationId = Convert.ToInt32(row["intMovementId"].ToString());
                    MovementDataSetDTO isExists = list.Where(x => x.intMovementId == applicationId).FirstOrDefault();

                    if (isExists != null)
                    {
                        list.Remove(isExists);
                    }

                    MovementDataSetDTO obj = new MovementDataSetDTO
                    {
                        intMovementId = Convert.ToInt32(row["intMovementId"].ToString()),
                        intBusinessUnitId = Convert.ToInt32(row["intBusinessUnitId"].ToString()),
                        intWorkplaceGroupId = Convert.ToInt32(row["intWorkplaceGroupId"].ToString()),
                        intEmployeeId = Convert.ToInt32(row["intEmployeeId"].ToString()),
                        intApproverId = Convert.ToInt32(row["intApproverId"].ToString()),
                        strEmployeeCode = row["strEmployeeCode"].ToString(),
                        strEmployeeName = row["strEmployeeName"].ToString(),
                        strReason = row["strReason"].ToString(),
                        dteFromDate = Convert.ToDateTime(row["dteFromDate"].ToString()),
                        dteToDate = Convert.ToDateTime(row["dteToDate"].ToString()),
                        dteFromTime = row["dteFromTime"].ToString(),
                        dteTimeTo = row["dteTimeTo"].ToString(),
                        strDepartmentName = row["strDepartmentName"].ToString(),
                        strDesignationName = row["strDesignationName"].ToString(),
                        intDesignationId = Convert.ToInt64(row["intDepartmentId"].ToString()),
                        intDepartmentId = Convert.ToInt64(row["intDesignationId"].ToString()),
                        strEmploymentTypeName = row["strEmploymentTypeName"].ToString(),
                        strMovementType = row["strMovementType"].ToString(),
                        dteApplicationDate = Convert.ToDateTime(row["dteApplicationDate"].ToString()),
                        strLocation = row["strLocation"].ToString(),
                        strStatus = row["strStatus"].ToString(),
                        strViewAs = row["strViewAs"].ToString(),
                    };
                    list.Add(obj);
                }

                returnObj = list.FirstOrDefault();

                return returnObj;
            }
            catch (Exception ex)
            {
                return new MovementDataSetDTO();
            }
        }
        public async Task<MessageHelper> MovementApprove(List<LeaveAndMovementApprovedDTO> obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string jsonString = JsonSerializer.Serialize(obj);

                    string sql = "saas.sprMovementApprovalProcedure";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region ===> LEAVE MOVEMENT TYPE <===
        public async Task<MessageHelper> CRUDLeaveMovementType(LeaveMovementTypeDTO obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLeaveMovementType";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@isLeave", obj.IsLeave);
                        sqlCmd.Parameters.AddWithValue("@intLeaveMovementAutoId", obj.LeaveMovementAutoId);
                        sqlCmd.Parameters.AddWithValue("@strLeaveTypeCode", obj.LeaveTypeCode);
                        sqlCmd.Parameters.AddWithValue("@strLeaveType", obj.LeaveType);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@isPayable", obj.IsPayable);
                        sqlCmd.Parameters.AddWithValue("@numPercentPayable", obj.PercentPayable);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@strInsertUserId", obj.InsertUser);
                        sqlCmd.Parameters.AddWithValue("@strMovementType", obj.MovementType);
                        sqlCmd.Parameters.AddWithValue("@intMovementMonthlyAllowedHour", obj.MovementMonthlyAllowedHour);


                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();


                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region ===> Employment type wise leave type <===
        public async Task<MessageHelper> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeWiseLeaveBalanceDTO obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmploymentTypeWiseLeaveBalance";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@intAutoId", obj.AutoId);
                        sqlCmd.Parameters.AddWithValue("@intEmploymentTypeId", obj.EmploymentTypeId);
                        sqlCmd.Parameters.AddWithValue("@intAllocatedLeave", obj.AllocatedLeave);
                        sqlCmd.Parameters.AddWithValue("@intYearId", obj.YearId);
                        sqlCmd.Parameters.AddWithValue("@intLeaveTypeId", obj.LeaveTypeId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.isActive);
                        sqlCmd.Parameters.AddWithValue("@strInsertUserId", obj.InsertUserId);



                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();


                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
