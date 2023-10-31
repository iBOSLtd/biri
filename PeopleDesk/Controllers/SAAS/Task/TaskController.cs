using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Task;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Controllers.SAAS.Task
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskManagementService _taskManagement;

        public TaskController(ITaskManagementService _taskManagement)
        {
            this._taskManagement = _taskManagement;
        }

        #region =====================  Task Project  =====================

        [HttpPost]
        [Route("ProjectCreateAndUpdate")]
        public async Task<IActionResult> ProjectCreateAndUpdate(TskProject model)
        {
            return Ok(await _taskManagement.TskProjectCreateAndUpdate(model));
        }

        [HttpGet]
        [Route("ProjectLanding")]
        public async Task<IActionResult> ProjectLanding(long accountId, long businessId)
        {
            return Ok(await _taskManagement.TskProjectLanding(accountId, businessId));
        }

        [HttpGet]
        [Route("ProjectById")]
        public async Task<IActionResult> ProjectById(long Id)
        {
            return Ok(await _taskManagement.TskProjectById(Id));
        }

        [HttpDelete]
        [Route("DeleteProjectById")]
        public async Task<IActionResult> DeleteProjectById(long Id)
        {
            return Ok(await _taskManagement.TskProjectDeleteById(Id));
        }

        #endregion =====================  Task Project  =====================

        #region ======================  Task Group Member  ======================

        [HttpPost]
        [Route("GroupMemberCreateAndUpdate")]
        public async Task<IActionResult> GroupMemberCreateAndUpdate(TskGroupMember obj)
        {
            return Ok(await _taskManagement.TskGroupMemberCreateAndUpdate(obj));
        }

        [HttpGet]
        [Route("GroupMemberLanding")]
        public async Task<IActionResult> GroupMemberLanding(long Id)
        {
            return Ok(await _taskManagement.TskGroupMemberLandingByProjectId(Id));
        }

        [HttpGet]
        [Route("GroupMemberById")]
        public async Task<IActionResult> GroupMemberById(long Id)
        {
            return Ok(await _taskManagement.TskGroupMemberById(Id));
        }

        [HttpDelete]
        [Route("DeleteGroupMemberById")]
        public async Task<IActionResult> DeleteGroupMemberById(long Id)
        {
            return Ok(await _taskManagement.TskGroupMemberDelete(Id));
        }

        #endregion ======================  Task Group Member  ======================

        #region ====================== Task Board ========================

        [HttpPost]
        [Route("BoardCreateAndUpdate")]
        public async Task<IActionResult> BoardCreateAndUpdate(TskBoard obj)
        {
            return Ok(await _taskManagement.TskBoardCreateAndUpdate(obj));
        }

        [HttpGet]
        [Route("BoardLanding")]
        public async Task<IActionResult> BoardLanding()
        {
            return Ok(await _taskManagement.TskBoardLandingByProjectId());
        }

        [HttpGet]
        [Route("BoardById")]
        public async Task<IActionResult> BoardById(long Id)
        {
            return Ok(await _taskManagement.TskBoardById(Id));
        }

        [HttpDelete]
        [Route("BoardDeleteById")]
        public async Task<IActionResult> BoardDeleteById(long Id)
        {
            return Ok(await _taskManagement.TskBoardDeleteById(Id));
        }

        #endregion ====================== Task Board ========================

        #region ======================= Task Details =======================

        [HttpPost]
        [Route("TaskDetailsCreateAndUpdate")]
        public async Task<IActionResult> TaskDetailsCreateAndUpdate(TskTaskDetail obj)
        {
            return Ok(await _taskManagement.TskTaskDetailsCreateAndUpdate(obj));
        }

        [HttpGet]
        [Route("TaskDetailsLanding")]
        public async Task<IActionResult> TaskDetailsLanding()
        {
            return Ok(await _taskManagement.TskTaskDetailsLandingByBoardId());
        }

        [HttpGet]
        [Route("TaskDetailsById")]
        public async Task<IActionResult> TaskDetailsById(long Id)
        {
            return Ok(await _taskManagement.TskTaskDetailsById(Id));
        }

        [HttpDelete]
        [Route("TaskDetailsDeleteById")]
        public async Task<IActionResult> TaskDetailsDeleteById(long Id)
        {
            return Ok(await _taskManagement.TskTaskDetailsById(Id));
        }

        #endregion ======================= Task Details =======================

        #region ===================== Query Data =====================

        [HttpGet]
        [Route("GetProjectBoardAndList")]
        public async Task<IActionResult> GetProjectBoardAndList(long Id)
        {
            return Ok(await _taskManagement.ProjectDetailsInformation(Id));
        }

        #endregion ===================== Query Data =====================

        #region ================  Project Creation  =================

        [HttpPost]
        [Route("Project")]
        public async Task<IActionResult> Project(TskProjectCreateViewModel obj)
        {
            return Ok(await _taskManagement.ProjectCreaete(obj));
        }

        #endregion ================  Project Creation  =================

        #region ================  Board Creation  =================

        [HttpPost]
        [Route("Board")]
        public async Task<IActionResult> Board(TskBoardCreateViewModel obj)
        {
            return Ok(await _taskManagement.BoardCreate(obj));
        }

        #endregion ================  Board Creation  =================

        #region ================  Task Creation  =================

        [HttpPost]
        [Route("Task")]
        public async Task<IActionResult> Task(TskTaskCreateViewModel obj)
        {
            return Ok(await _taskManagement.TaskCreate(obj));
        }

        #endregion ================  Task Creation  =================

        #region ====================  Project Create SP  ====================

        //[HttpPost]
        //[Route("ProjectCreateSp")]

        #endregion ====================  Project Create SP  ====================

        #region ====================  Board Create SP  =====================

        //[HttpPost]
        //[Route("BoardCreateSp")]

        #endregion ====================  Board Create SP  =====================

        #region ====================  Task Create SP  ====================

        //[HttpPost]
        //[Route("TaskCreateSp")]

        #endregion ====================  Task Create SP  ====================
    }
}