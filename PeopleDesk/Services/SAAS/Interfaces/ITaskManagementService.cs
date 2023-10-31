using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Task;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface ITaskManagementService

    {


        #region ===============  Task Project  ================

        Task<MessageHelper> TskProjectCreateAndUpdate(TskProject obj);
        Task<List<TskProject>> TskProjectLanding(long accountId, long businessId);
        Task<TskProject> TskProjectById(long Id);
        Task<MessageHelper> TskProjectDeleteById(long Id);

        #endregion

        #region  =================  Task Group Members  =================

        Task<MessageHelper> TskGroupMemberCreateAndUpdate(TskGroupMember obj);
        Task<List<TskGroupMember>> TskGroupMemberLandingByProjectId(long Id);
        Task<TskGroupMember> TskGroupMemberById(long Id);
        Task<MessageHelper> TskGroupMemberDelete(long Id);

        #endregion

        #region ===================  Task Board =================
        Task<MessageHelper> TskBoardCreateAndUpdate(TskBoard obj);
        Task<List<TskBoard>> TskBoardLandingByProjectId();
        Task<TskBoard> TskBoardById(long Id);
        Task<MessageHelper> TskBoardDeleteById(long Id);

        #endregion

        #region ===================  Task Details  =====================
        Task<MessageHelper> TskTaskDetailsCreateAndUpdate(TskTaskDetail obj);
        Task<List<TskTaskDetail>> TskTaskDetailsLandingByBoardId();
        Task<TskTaskDetail> TskTaskDetailsById(long Id);
        Task<MessageHelper> TskTaskDetailsDeleteById(long Id);

        #endregion 


        #region  ================  Query Data  ==================
        Task<TskProjectViewModel> ProjectDetailsInformation(long Id);

        //Task<TskProjectViewModel> ProjectDetailsInformationJoin(long Id);

        #endregion


        #region  =================  Project Creation  ==================
        Task<MessageHelper> ProjectCreaete(TskProjectCreateViewModel obj);

        #endregion

        #region  =================  Board Creation  =================
        Task<MessageHelper> BoardCreate(TskBoardCreateViewModel obj);

        #endregion

        #region  =================  Task Creation  ==================
        Task<MessageHelper> TaskCreate(TskTaskCreateViewModel obj);

        #endregion

    }
}
