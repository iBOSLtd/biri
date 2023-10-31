namespace PeopleDesk.Models.Training
{
    public class TrainingViewModel
    {

    }
    public class TrainingDDL
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string ResourcePerson { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
        public long? RequisitionId { get; set; }
        public bool? isPreSubmitted { get; set; }
        public bool? isPostSubmitted { get; set; }
    }
    public class TrainingNameDTO
    {
        public long IntTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public string StrTrainingCode { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long IntActionBy { get; set; }
        public DateTime DteActionDate { get; set; }
    }
    public class TrainingScheduleDTO
    {
        public long IntScheduleId { get; set; }
        public long IntTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public string StrTrainingCode { get; set; }
        public DateTime DteDate { get; set; }
        public decimal NumTotalDuration { get; set; }
        public string StrVenue { get; set; }
        public string StrResourcePersonName { get; set; }
        public long IntBatchSize { get; set; }
        public string StrBatchNo { get; set; }
        public DateTime DteFromDate { get; set; }
        public DateTime DteToDate { get; set; }
        public string StrRemarks { get; set; }
        public bool IsActive { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntRequisitionId { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime? DteActionDate { get; set; }
        public bool? IsRequestedSchedule { get; set; }
        public long? IntRequestedByEmp { get; set; }
        public DateTime? DteCourseCompletionDate { get; set; }
        public DateTime? DteExtentedDate { get; set; }
        public DateTime? DteLastAssesmentSubmissionDate { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public string? MonthYear { get; set; }
        public string? StrEmployeeName { get; set; }
        public long? TotalRequisition { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }

    }
    public class TrainingScheduleLandingDTO
    {
        public List<TrainingScheduleDTO> data { get; set; }
        public long CurrentPage { get; set; }
        public long PageSize { get; set; }
        public long TotalCount { get; set; }
    }
    public class TrainingRequisitionDTO
    {
        public long? IntRequisitionId { get; set; }
        public long? IntScheduleId { get; set; }
        public string? StrTrainingName { get; set; }
        public string? StrTrainingCode { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrEmployeeCode { get; set; }
        public long? IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntDepartmentId { get; set; }
        public string StrDepartmentName { get; set; }
        public long IntDesignationId { get; set; }
        public string StrDesignationName { get; set; }
        public string StrEmail { get; set; }
        public string? StrPhoneNo { get; set; }
        public string? StrGender { get; set; }
        public DateTime? dteAttendanceDate { get; set; }
        public string? StrAttendanceStatus { get; set; }
        public long? IntSupervisorId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? StrEmploymentType { get; set; }
        public DateTime? DteActionDate { get; set; }
        public long? IntActionBy { get; set; }
        public bool? IsFromRequisition { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public long? IntApprovedBy { get; set; }
        public DateTime? DteApprovedDate { get; set; }
        public string StrRejectionComments { get; set; }
        public string? StrApprovalStatus { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteCourseCompletionDate { get; set; }
        public DateTime? DteExtentedDate { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public long? Attendance { get; set; }
        public string PreAssessmentMarks { get; set; }
        public string PostAssessmentMarks { get; set; }
    }
    public class TrainingRequisitionLandingDTO
    {
        public List<TrainingRequisitionDTO> data { get; set; }
        public TrainingScheduleDTO TrainingSchedule { get; set; }
        //public long CurrentPage { get; set; }
        //public long PageSize { get; set; }
        //public long TotalCount { get; set; }
    }
    public class TrainingAttendanceDTO
    {
        public long IntAttendanceId { get; set; }
        public long IntScheduleId { get; set; }
        public long IntEmployeeId { get; set; }
        public DateTime DteDate { get; set; }
        public string StrAttendanceStatus { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime? DteActionDate { get; set; }
        public long? IntActionBy { get; set; }
    }
    public class TrainingAttendanceLandingDTO
    {
        //public List<TrainingAttendanceDTO> data { get; set; }
        public List<ScheduleEmployeeListForTrainingAttendance> data { get; set; }
        public TrainingScheduleDTO TrainingSchedule { get; set; }
        //public long CurrentPage { get; set; }
        //public long PageSize { get; set; }
        //public long TotalCount { get; set; }
    }
    public class TrainingRequisitionApprovalDTO
    {
        public long[] intRequisitionId { get; set; }
        public bool isApproved { get; set; }
        public long? intApprovedBy { get; set; }
        public long? intRejectedBy { get; set; }
        public string Comments { get; set; }
    }
    public class ScheduleEmployeeListForTrainingAttendance
    {
        public long IntScheduleId { get; set; }
        public long? IntRequisitionId { get; set; }
        public long? IntAttendanceId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string? StrEmployeeName { get; set; }
        public long? DesignationId { get; set; }
        public string? Designation { get; set; }
        public long? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public bool? StrAttendanceStatus { get; set; }
    }
    public class PresentAbsentProcessOfAttendanceDTO
    {
        public long AttendanceId { get; set; }
        public bool isPresent { get; set; }
        public long ActionBy { get; set; }
    }
    public class TrainingAssesmentQuestionDTO
    {
        public long IntQuestionId { get; set; }
        public string StrQuestion { get; set; }
        public long IntScheduleId { get; set; }
        public bool IsPreAssesment { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DteLastActionDate { get; set; }
        public long? IntActionBy { get; set; }
        public bool IsRequired { get; set; }
        public string StrInputType { get; set; }
        public long? IntOrder { get; set; }
        public string strAnswer { get; set; }
        public long? numMarks { get; set; }
        public string PreAssessmentMarks { get; set; }
        public string PostAssessmentMarks { get; set; }
        public long? IntRequisitionId { get; set; }
        public List<TrainingAssenmentQuestionOptionDTO> Options { get; set; }

    }
    public class TrainingAssenmentQuestionOptionDTO
    {
        public long IntOptionId { get; set; }
        public string StrOption { get; set; }
        public long IntQuestionId { get; set; }
        public long? NumPoints { get; set; }
        public DateTime? DteLastAction { get; set; }
        public long? IntActionBy { get; set; }
        public long? IntOrder { get; set; }
        public decimal Marks { get; set; }
        public bool isAnswer { get; set; }

    }
    public class TrainingRequisitionForAssesmentDTO
    {
        public long? IntRequisitionId { get; set; }
        public long? IntScheduleId { get; set; }
        public string? StrTrainingName { get; set; }
        public string? StrTrainingCode { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntDepartmentId { get; set; }
        public string StrDepartmentName { get; set; }
        public long IntDesignationId { get; set; }
        public string StrDesignationName { get; set; }
        public string StrEmail { get; set; }
        public string? StrPhoneNo { get; set; }
        public long Attendance { get; set; }
        public long? TotalMark { get; set; }
        public decimal TotalQuestion { get; set; }
        public decimal TotalQuestionAnswered { get; set; }
    }
    public class TrainingAssesmentQuestionCommonDTO
    {
        public TrainingRequisitionForAssesmentDTO requisitionForAssesment { get; set; }
        public List<TrainingAssesmentQuestionDTO> question { get; set; }
        public List<TrainingAssenmentQuestionOptionDTO> questionOption { get; set; }
    }
    public class TrainingAssesmentAnswerDTO
    {
        public long IntAnswerId { get; set; }
        public long IntQuestionId { get; set; }
        public long IntOptionId { get; set; }
        public string StrOption { get; set; }
        public decimal NumMarks { get; set; }
        public DateTime? DteLastAction { get; set; }
        public long? IntActionBy { get; set; }
        public long? IntRequisitionId { get; set; }
    }
    public class ExternalTrainingDTO
    {
        public long IntExternalTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public string StrResourcePersonName { get; set; }
        public DateTime DteDate { get; set; }
        public long IntDepartmentId { get; set; }
        public string StrDepartmentName { get; set; }
        public string StrOrganizationCategory { get; set; }
        public long IntBatchSize { get; set; }
        public long? IntPresentParticipant { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime? DteActionDate { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
    }
}
