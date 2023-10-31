using Microsoft.Data.SqlClient;
using PeopleDesk.Models.Task;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Services.SAAS
{
    public class TaskManagementSpService : ITaskManagementSpService
    {
        DataTable dt = new DataTable();
        public async Task<DataTable> TskProjectCreate(ProjectCreateSPViewModel proj)
        {
            try
            {
                using (var connection = new SqlConnection())
                {
                    string sql = "saas.sprProjectCRUD";
                    using (SqlCommand sqlcmd = new SqlCommand(sql, connection))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@strPartType", proj.StrPartType);
                        sqlcmd.Parameters.AddWithValue("@strProjectName", proj.StrProjectName);
                        sqlcmd.Parameters.AddWithValue("@dteStartDate", proj.DteStartDate);
                        sqlcmd.Parameters.AddWithValue("@dteEndDate", proj.DteEndDate);
                        sqlcmd.Parameters.AddWithValue("@intFileUrl", proj.IntFileUrlId);
                        sqlcmd.Parameters.AddWithValue("@isActive", proj.IsActivate);
                        sqlcmd.Parameters.AddWithValue("@dteCreatedAt", proj.DteCreatedAt);
                        sqlcmd.Parameters.AddWithValue("@intCreaetedBy", proj.IntCreatedBy);
                        sqlcmd.Parameters.AddWithValue("@dteUpdatedAt", proj.DteUpdatedAt);
                        sqlcmd.Parameters.AddWithValue("@intUpdatedBy", proj.IntUpdatedBy);
                        sqlcmd.Parameters.AddWithValue("@strStatus", proj.StrStatus);
                        sqlcmd.Parameters.AddWithValue("@strDescription", proj.StrDescription);

                    }

                }
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> TskBoardCreate(BoardCreateSPViewModel board)
        {
            try
            {
                using (var connection = new SqlConnection())
                {
                    string sql = "saas.sprBoardCRUD";
                    using (SqlCommand sqlcmd = new SqlCommand(sql, connection))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@strPartType", board.StrPartType);
                        sqlcmd.Parameters.AddWithValue("@intProjectId", board.IntProjectId);
                        sqlcmd.Parameters.AddWithValue("@strBoardName", board.StrBoardName);
                        sqlcmd.Parameters.AddWithValue("@strDescription", board.StrDescription);
                        sqlcmd.Parameters.AddWithValue("@dteStartDate", board.DteStartDate);
                        sqlcmd.Parameters.AddWithValue("@dteEndDate", board.DteEndDate);
                        sqlcmd.Parameters.AddWithValue("@intReporterId", board.IntReporterId);
                        sqlcmd.Parameters.AddWithValue("@intFileUrlId", board.IntFileUrlId);
                        sqlcmd.Parameters.AddWithValue("@strPriority", board.StrPriority);
                        sqlcmd.Parameters.AddWithValue("@strBackgroundColor", board.StrBackgroundColor);
                        sqlcmd.Parameters.AddWithValue("@strHtmlColorCode", board.StrHtmlColorCode);
                        sqlcmd.Parameters.AddWithValue("@strStatus", board.StrStatus);
                        sqlcmd.Parameters.AddWithValue("@isActive", board.IsActive);
                        sqlcmd.Parameters.AddWithValue("@dteCreatedAt", board.DteCreatedAt);
                        sqlcmd.Parameters.AddWithValue("@intCreaetedBy", board.IntCreatedBy);
                        sqlcmd.Parameters.AddWithValue("@dteUpdatedAt", board.DteUpdatedAt);
                        sqlcmd.Parameters.AddWithValue("@intUpdatedBy", board.IntUpdatedBy);

                    }

                }
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> TskTaskCreate(TaskCreateSPViewModel task)
        {
            try
            {
                using (var connection = new SqlConnection())
                {
                    string sql = "saas.sprTaskCRUD";
                    using (SqlCommand sqlcmd = new SqlCommand(sql, connection))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@strPartType", task.StrPartType);
                        sqlcmd.Parameters.AddWithValue("@intProjectId", task.IntProjectId);
                        sqlcmd.Parameters.AddWithValue("@intBoardId", task.IntBoardId);
                        sqlcmd.Parameters.AddWithValue("@strTaskTitle", task.StrTaskTitle);
                        sqlcmd.Parameters.AddWithValue("@strDescription", task.StrTaskDescription);
                        sqlcmd.Parameters.AddWithValue("@strStatus", task.StrStatus);
                        sqlcmd.Parameters.AddWithValue("@isActive", task.IsActive);
                        sqlcmd.Parameters.AddWithValue("@dteCreatedAt", task.DteCreatedAt);
                        sqlcmd.Parameters.AddWithValue("@intCreaetedBy", task.IntCreatedBy);
                        sqlcmd.Parameters.AddWithValue("@dteUpdatedAt", task.DteUpdatedAt);
                        sqlcmd.Parameters.AddWithValue("@intUpdatedBy", task.IntUpdatedBy);

                    }

                }
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
