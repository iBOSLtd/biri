using PeopleDesk.Models;
using PeopleDesk.Models.Training;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface ITrainingService
    {
        #region -- Training Name --
        Task<MessageHelper> CreateTrainingName(List<TrainingNameDTO> obj);
        Task<List<TrainingDDL>> TrainingNameDDL();
        #endregion

        #region -- Training Schedule --
        Task<MessageHelper> CreateTrainingSchedule(List<TrainingScheduleDTO> obj);
        Task<MessageHelper> EditTrainingSchedule(TrainingScheduleDTO obj);
        Task<List<TrainingScheduleDTO>> GetTrainingScheduleLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId);
        Task<List<TrainingDDL>> TrainingScheduleDDL();
        public Task<bool> DeleteTrainingSchedule(long id);
        #endregion

        #region -- Training Requisition
        Task<MessageHelperBulkUpload> CreateTrainingRequisition(List<TrainingRequisitionDTO> obj);
        Task<List<TrainingScheduleDTO>> GetApprovedTrainingRequisitionLanding(long intAccountId, long intBusinessUnitId);
        Task<TrainingRequisitionLandingDTO> GetTrainingRequisitionLanding(long? intScheduleId, long IntAccountId, long intBusinessUnitId);
        Task<TrainingRequisitionLandingDTO> GetTrainingSubmissionLanding(long scheduleId, long IntAccountId, long intBusinessUnitId);
        Task<List<TrainingAssesmentQuestionDTO>> GetTrainingAssesmentQuestionAnswerById(long intScheduleId, long EmployeeId, long RequsitionId, bool isPreAssessment);
        Task<MessageHelper> TrainingRequisitionApproval(TrainingRequisitionApprovalDTO obj);
        #endregion

        #region -- Training Attendance --
        Task<List<TrainingScheduleDTO>> GetTrainingAssesmentLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId);
        Task<List<TrainingScheduleDTO>> GetTrainingSelfAssesmentLanding(long? intTrainingId, long intAccountId,long intEmployeeId, long intBusinessUnitId);
        Task<TrainingAttendanceLandingDTO> GetScheduleEmployeeListForTrainingAttendance(long scheduleId, DateTime? attendanceDate, long IntAccountId, long intBusinessUnitId);
        Task<MessageHelper> PresentAbsentProcessOfAttendance(List<PresentAbsentProcessOfAttendanceDTO> obj);
        Task<MessageHelper> CreateTrainingAttendance(List<TrainingRequisitionDTO> obj);
        #endregion

        #region -- Training Assesment --
        Task<MessageHelper> CreateTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj);
        Task<List<TrainingAssesmentQuestionDTO>> GetTrainingAssesmentQuestionByScheduleId(long? employeeId,long intScheduleId, bool isPreAssessment);
        Task<MessageHelper> EditTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj);
        Task<MessageHelper> AssessmentQuestionDelete(bool isPreassesment, long ScheduleId);
        Task<MessageHelper> CreateTrainingAssesmentAnswer(List<TrainingAssesmentAnswerDTO> obj);
        Task<TrainingAssesmentQuestionCommonDTO> GetTrainingAssesmentQuestionAnswerByRequisitionId(long intScheduleId, long intEmployeeId, long intRequisitionId, bool isPreAssessment, long IntAccountId, long intBusinessUnitId);
        #endregion

        #region ==== Employee Assessment ====
        Task<List<TrainingDDL>> EmployeeTrainingScheduleLanding(long employeeId);
        #endregion

        #region -- External Training --
        Task<MessageHelper> ExternalTraining(ExternalTrainingDTO obj);
        Task<MessageHelper> UpdateExternalTraining(long ExternalTrainingId);
        Task<List<ExternalTrainingDTO>> GetExternalTrainingLanding(long? intExternalTrainingId, long intAccountId, long intBusinessUnitId);
        #endregion
    }
}
