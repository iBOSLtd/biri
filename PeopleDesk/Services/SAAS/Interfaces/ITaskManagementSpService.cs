using PeopleDesk.Models.Task;
using System.Data;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface ITaskManagementSpService
    {
        Task<DataTable> TskProjectCreate(ProjectCreateSPViewModel proj);
        Task<DataTable> TskBoardCreate(BoardCreateSPViewModel board);
        Task<DataTable> TskTaskCreate(TaskCreateSPViewModel task);

    }
}
