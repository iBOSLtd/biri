using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Training;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Controllers.SAAS.Training
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingService _trainingService;
        private readonly PeopleDeskContext _context;
        public TrainingController(PeopleDeskContext context, ITrainingService trainingService)
        {
            _context = context;
            _trainingService = trainingService;
        }
        #region -- Training Name
        [HttpPost]
        [Route("CreateTrainingName")]
        public async Task<IActionResult> CreateTrainingName(List<TrainingNameDTO> obj)
        {
            var dt = await _trainingService.CreateTrainingName(obj);
            return Ok(dt);
        }
        [HttpGet]
        [Route("TrainingNameDDL")]
        public async Task<IActionResult> TrainingNameDDL()
        {
            var dt = await _trainingService.TrainingNameDDL();
            return Ok(dt);
        }
        #endregion

        #region -- Training Schedule --
        [HttpPost]
        [Route("CreateTrainingSchedule")]
        public async Task<IActionResult> CreateTrainingSchedule(List<TrainingScheduleDTO> obj)
        {
            try
            {
                var dt = await _trainingService.CreateTrainingSchedule(obj);
                return Ok(dt);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.CatchProcess());
            }

        }
        [HttpPost]
        [Route("EditTrainingSchedule")]
        public async Task<IActionResult> EditTrainingSchedule(TrainingScheduleDTO obj)
        {
            var dt = await _trainingService.EditTrainingSchedule(obj);
            return Ok(dt);
        }
        [HttpPut]
        [Route("UpdateTrainingSchedule")]
        public async Task<IActionResult> UpdateTrainingSchedule(TrainingScheduleDTO model)
        {
            try
            {
                MessageHelper res = new MessageHelper();

                TrainingSchedule obj = await _context.TrainingSchedules.FirstAsync(x => x.IntScheduleId == model.IntScheduleId);
                obj.DteToDate = model.DteToDate;
                obj.DteLastAssesmentSubmissionDate = model.DteLastAssesmentSubmissionDate;
                obj.DteExtentedDate = model.DteExtentedDate;
                obj.DteCourseCompletionDate = model.DteCourseCompletionDate;

                _context.TrainingSchedules.Update(obj);
                await _context.SaveChangesAsync();

                res.StatusCode = 200;
                res.Message = "Successfully Update";
                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("GetTrainingScheduleLanding")]
        public async Task<IActionResult> GetTrainingScheduleLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingScheduleLanding(intTrainingId, intAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("TrainingScheduleDDL")]
        public async Task<IActionResult> TrainingScheduleDDL()
        {
            var dt = await _trainingService.TrainingScheduleDDL();
            return Ok(dt);
        }
        [HttpPut]
        [Route("DeleteTrainingSchedule")]
        public async Task<IActionResult> DeleteTrainingSchedule(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _trainingService.DeleteTrainingSchedule(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region -- Training Requisition
        [HttpPost]
        [Route("CreateTrainingRequisition")]
        public async Task<IActionResult> CreateTrainingRequisition(List<TrainingRequisitionDTO> obj)
        {
            try
            {
                var dt = await _trainingService.CreateTrainingRequisition(obj);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }

        }
        [HttpPost]
        [Route("EditTrainingRequisition")]
        public async Task<IActionResult> EditTrainingRequisition(TrainingRequisitionDTO obj)
        {
            MessageHelper res = new MessageHelper();
            try
            {
                if (obj.IntRequisitionId > 0)
                {
                    TrainingRequisition req = await _context.TrainingRequisitions.Where(x => x.IntRequisitionId == obj.IntRequisitionId).FirstOrDefaultAsync();

                    req.StrPhoneNo = obj.StrPhoneNo;

                    _context.TrainingRequisitions.Update(req);
                    await _context.SaveChangesAsync();

                    res.StatusCode = 200;
                    res.Message = "UPDATE SUCCESSFULLY";
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                res.StatusCode = 500;
                res.Message = e.Message;

                return BadRequest(res);
            }
        }
        [HttpGet]
        [Route("GetApprovedTrainingRequisitionLanding")]
        public async Task<IActionResult> GetApprovedTrainingRequisitionLanding(long intAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetApprovedTrainingRequisitionLanding(intAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingRequisitionLanding")]
        public async Task<IActionResult> GetTrainingRequisitionLanding(long? intScheduleId, long IntAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingRequisitionLanding(intScheduleId, IntAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingSubmissionLanding")]
        public async Task<IActionResult> GetTrainingSubmissionLanding(long scheduleId, long IntAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingSubmissionLanding(scheduleId, IntAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingAssesmentQuestionAnswerById")]
        public async Task<IActionResult> GetTrainingAssesmentQuestionAnswerById(long intScheduleId, long EmployeeId, long RequsitionId, bool isPreAssessment)
        {
            var dt = await _trainingService.GetTrainingAssesmentQuestionAnswerById(intScheduleId, EmployeeId, RequsitionId, isPreAssessment);
            return Ok(dt);
        }
        [HttpPost]
        [Route("TrainingRequisitionApproval")]
        public async Task<IActionResult> TrainingRequisitionApproval(TrainingRequisitionApprovalDTO obj)
        {
            var dt = await _trainingService.TrainingRequisitionApproval(obj);
            return Ok(dt);
        }
        #endregion

        #region -- Training Attendance

        [HttpGet]
        [Route("GetTrainingAssessmentLanding")]
        public async Task<IActionResult> GetTrainingAssesmentLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingAssesmentLanding(intTrainingId, intAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingSelfAssesmentLanding")]
        public async Task<IActionResult> GetTrainingSelfAssesmentLanding(long? intTrainingId, long intAccountId, long intEmployeeId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingSelfAssesmentLanding(intTrainingId, intAccountId, intEmployeeId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetScheduleEmployeeListForTrainingAttendance")]
        public async Task<IActionResult> GetScheduleEmployeeListForTrainingAttendance(long scheduleId, DateTime? attendanceDate, long IntAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetScheduleEmployeeListForTrainingAttendance(scheduleId, attendanceDate, IntAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        [HttpPost]
        [Route("PresentAbsentProcessOfAttendance")]
        public async Task<IActionResult> PresentAbsentProcessOfAttendance(List<PresentAbsentProcessOfAttendanceDTO> obj)
        {
            var dt = await _trainingService.PresentAbsentProcessOfAttendance(obj);
            return Ok(dt);
        }
        [HttpPost]
        [Route("CreateTrainingAttendance")]
        public async Task<IActionResult> CreateTrainingAttendance(List<TrainingRequisitionDTO> obj)
        {
            var dt = await _trainingService.CreateTrainingAttendance(obj);
            return Ok(dt);
        }
        #endregion

        #region -- Training Assesment --
        [HttpPost]
        [Route("CreateTrainingAssesmentQuestion")]
        public async Task<IActionResult> CreateTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj)
        {
            var dt = await _trainingService.CreateTrainingAssesmentQuestion(obj);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingAssesmentQuestionByScheduleId")]
        public async Task<IActionResult> GetTrainingAssesmentQuestionByScheduleId(long? employeeId, long intScheduleId, bool isPreAssessment)
        {
            var dt = await _trainingService.GetTrainingAssesmentQuestionByScheduleId(employeeId, intScheduleId, isPreAssessment);
            return Ok(dt);
        }
        [HttpPost]
        [Route("EditTrainingAssesmentQuestion")]
        public async Task<IActionResult> EditTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj)
        {
            var dt = await _trainingService.EditTrainingAssesmentQuestion(obj);
            return Ok(dt);
        }
        [HttpPut]
        [Route("AssessmentQuestionDelete")]
        public async Task<IActionResult> AssessmentQuestionDelete(bool isPreassesment, long ScheduleId)
        {
            var dt = await _trainingService.AssessmentQuestionDelete(isPreassesment, ScheduleId);
            return Ok(dt);
        }
        [HttpPost]
        [Route("CreateTrainingAssesmentAnswer")]
        public async Task<IActionResult> CreateTrainingAssesmentAnswer(List<TrainingAssesmentAnswerDTO> obj)
        {
            var dt = await _trainingService.CreateTrainingAssesmentAnswer(obj);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetTrainingAssesmentQuestionAnswerByRequisitionId")]
        public async Task<IActionResult> GetTrainingAssesmentQuestionAnswerByRequisitionId(long intScheduleId, long intEmployeeId, long intRequisitionId, bool isPreAssessment, long IntAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetTrainingAssesmentQuestionAnswerByRequisitionId(intScheduleId, intEmployeeId, intRequisitionId, isPreAssessment, IntAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        #endregion

        #region ==== Employee Assessment ====
        [HttpGet]
        [Route("EmployeeTrainingScheduleLanding")]
        public async Task<IActionResult> EmployeeTrainingScheduleLanding(long employeeId)
        {
            var dt = await _trainingService.EmployeeTrainingScheduleLanding(employeeId);
            return Ok(dt);
        }
        #endregion
        #region ---- External Training ----
        [HttpPost]
        [Route("ExternalTraining")]
        public async Task<IActionResult> ExternalTraining(ExternalTrainingDTO obj)
        {
            var dt = await _trainingService.ExternalTraining(obj);
            return Ok(dt);
        }
        [HttpPost]
        [Route("DeleteExternalTraining")]
        public async Task<IActionResult> UpdateExternalTraining(long ExternalTrainingId)
        {
            var dt = await _trainingService.UpdateExternalTraining(ExternalTrainingId);
            return Ok(dt);
        }
        [HttpGet]
        [Route("GetExternalTrainingLanding")]
        public async Task<IActionResult> GetExternalTrainingLanding(long? intExternalTrainingId, long intAccountId, long intBusinessUnitId)
        {
            var dt = await _trainingService.GetExternalTrainingLanding(intExternalTrainingId, intAccountId, intBusinessUnitId);
            return Ok(dt);
        }
        #endregion
    }
}
