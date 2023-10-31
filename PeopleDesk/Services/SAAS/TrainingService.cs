using DocumentFormat.OpenXml.Wordprocessing;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.Training;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Services.SAAS
{
    public class TrainingService : ITrainingService
    {
        private PeopleDeskContext _context;
        private readonly IApprovalPipelineService _approvalPipelineService;

        private MessageHelper msg = new MessageHelper();

        public TrainingService(PeopleDeskContext context, IApprovalPipelineService approvalPipelineService)
        {
            _context = context;
            _approvalPipelineService = approvalPipelineService;
        }

        #region -- Training Name --

        public async Task<MessageHelper> CreateTrainingName(List<TrainingNameDTO> obj)
        {
            try
            {   /////
                //var existItem = _context.training.Where(x => x.IsActive == true &&
                //                                           obj.Select(a => a.StrTrainingName.ToLower()).ToList().Contains(x.StrTrainingName.ToLower()))
                //                                .Select(x => x.StrTrainingName).ToList();
                //if (existItem.Count() > 0)
                //{
                //    //throw new Exception($"{string.Join(", ", existItem)} - Already Exists");
                //    msg.Message = $"{existItem} - already exists";
                //    return msg;
                //}
                //////

                List<training> createTraining = new List<training>();
                string exp = "";
                foreach (var item in obj)
                {
                    var exist = await _context.training.Where(x => x.IsActive == true && x.StrTrainingName.ToLower() == item.StrTrainingName.ToLower()).FirstOrDefaultAsync();
                    long maxValue = _context.training.Max(x => x.IntTrainingId as long?) ?? 0;
                    maxValue++;
                    if (exist == null)
                    {
                        training data = new training
                        {
                            StrTrainingName = item.StrTrainingName,
                            StrTrainingCode = String.Concat("TRA-", maxValue.ToString("0000")),
                            IntAccountId = item.IntAccountId,
                            IntBusinessUnitId = item.IntBusinessUnitId,
                            IsActive = true,
                            IntActionBy = item.IntActionBy,
                            DteActionDate = item.DteActionDate
                        };
                        createTraining.Add(data);
                    }
                    else
                    {
                        exp = item.StrTrainingName + " " + exp;
                    }
                }

                if (exp.Length > 0)
                {
                    //throw new Exception($"{exp} - already exists");
                    msg.Message = $"{exp} - already exists";
                    return msg;
                }
                else
                {
                    await _context.training.AddRangeAsync(createTraining);
                    await _context.SaveChangesAsync();

                    msg.Message = "Created Successfully";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingDDL>> TrainingNameDDL()
        {
            try
            {
                List<TrainingDDL> data = await (from training in _context.training
                                                where training.IsActive == true
                                                orderby training.IntTrainingId
                                                select new TrainingDDL
                                                {
                                                    Value = training.IntTrainingId,
                                                    Label = training.StrTrainingName
                                                }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion -- Training Name --

        #region -- Training Schedule --

        public async Task<MessageHelper> CreateTrainingSchedule(List<TrainingScheduleDTO> obj)
        {
            try
            {
                List<TrainingSchedule> schedule = new List<TrainingSchedule>();
                foreach (var item in obj)
                {
                    PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId((long)item.IntAccountId, "trainingScheduleApproval");

                    TrainingSchedule data = new TrainingSchedule
                    {
                        IntTrainingId = item.IntTrainingId,
                        StrTrainingName = item.StrTrainingName,
                        //IntMonth = item.IntMonth,
                        //IntYear = item.IntYear,
                        DteDate = item.DteDate,
                        NumTotalDuration = item.NumTotalDuration,
                        StrVenue = item.StrVenue,
                        StrResourcePersonName = item.StrResourcePersonName,
                        IntBatchSize = item.IntBatchSize,
                        StrBatchNo = item.StrBatchNo,
                        DteFromDate = item.DteFromDate,
                        DteToDate = item.DteToDate,
                        StrRemarks = item.StrRemarks,
                        StrApprovealStatus = "Pending",
                        IsActive = true,
                        IntAccountId = item.IntAccountId,
                        IntBusinessUnitId = item.IntBusinessUnitId,
                        IntActionBy = item.IntActionBy,
                        DteActionDate = DateTime.Now,
                        IsRequestedSchedule = item.IsRequestedSchedule,
                        IntRequestedByEmp = item.IntRequestedByEmp,
                        IntPipelineHeaderId = stage.HeaderId,
                        IntCurrentStage = stage.CurrentStageId,
                        IntNextStage = stage.NextStageId,
                        StrStatus = "Pending",
                        IsPipelineClosed = false,
                        IsReject = false
                    };
                    schedule.Add(data);
                }
                await _context.TrainingSchedules.AddRangeAsync(schedule);
                await _context.SaveChangesAsync();
                msg.Message = "Created Successfully";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> EditTrainingSchedule(TrainingScheduleDTO obj)
        {
            try
            {
                var data = _context.TrainingSchedules.FirstOrDefault(x => x.IntScheduleId == obj.IntScheduleId && x.IsActive == true);
                if (data != null)
                {
                    data.IntTrainingId = obj.IntTrainingId;
                    data.NumTotalDuration = obj.NumTotalDuration;
                    data.StrVenue = obj.StrVenue;
                    data.DteDate = obj.DteDate;
                    data.StrResourcePersonName = obj.StrResourcePersonName;
                    data.IntBatchSize = obj.IntBatchSize;
                    data.StrBatchNo = obj.StrBatchNo;
                    data.DteFromDate = obj.DteFromDate;
                    data.DteToDate = obj.DteToDate;
                    data.StrRemarks = obj.StrRemarks;
                    data.StrStatus = obj.StrStatus;
                    data.IsActive = obj.IsActive;
                    data.IntUpdatedBy = obj.IntUpdatedBy;
                    data.DteActionDate = DateTime.Now;
                    data.IsRequestedSchedule = obj.IsRequestedSchedule;
                    data.IntRequestedByEmp = obj.IntRequestedByEmp;
                    data.DteCourseCompletionDate = obj.DteCourseCompletionDate;
                    data.DteExtentedDate = obj.DteExtentedDate;
                    data.DteLastAssesmentSubmissionDate = obj.DteLastAssesmentSubmissionDate;

                    _context.TrainingSchedules.Update(data);
                    await _context.SaveChangesAsync();
                    msg.Message = "Edited Successfully";
                }
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingScheduleDTO>> GetTrainingScheduleLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId)
        {
            try
            {

                List<TrainingScheduleDTO> data = await (from schedule in _context.TrainingSchedules
                                                        join training in _context.training on schedule.IntTrainingId equals training.IntTrainingId into train
                                                        from training in train.DefaultIfEmpty()
                                                        where schedule.IsActive == true
                                                        && (schedule.IntScheduleId == intTrainingId || intTrainingId == 0 || intTrainingId == null)
                                                        && schedule.IntAccountId == intAccountId && schedule.IntBusinessUnitId == intBusinessUnitId
                                                        //&& schedule.IsPipelineClosed == true
                                                        select new TrainingScheduleDTO
                                                        {
                                                            IntScheduleId = schedule.IntScheduleId,
                                                            IntTrainingId = schedule.IntTrainingId,
                                                            StrTrainingName = schedule.StrTrainingName,
                                                            StrTrainingCode = training.StrTrainingCode,
                                                            //IntMonth = schedule.IntMonth,
                                                            //IntYear = schedule.IntYear,
                                                            //MonthYear = schedule.MonthYear,//(schedule.DteFromDate.ToString("dd MMM, yy") + "-" + schedule.DteToDate.ToString("dd MMM, yy")).ToString(),
                                                            NumTotalDuration = schedule.NumTotalDuration,
                                                            StrVenue = schedule.StrVenue,
                                                            StrResourcePersonName = schedule.StrResourcePersonName,
                                                            IntBatchSize = schedule.IntBatchSize,
                                                            StrBatchNo = schedule.StrBatchNo,
                                                            DteFromDate = schedule.DteFromDate,
                                                            DteDate = schedule.DteDate,
                                                            DteToDate = schedule.DteToDate,
                                                            StrRemarks = schedule.StrRemarks,
                                                            StrStatus = (schedule.IsPipelineClosed == true && schedule.IsActive == true && schedule.IsReject == false) ? "Approved"
                                                            : (schedule.IsPipelineClosed == false && schedule.IsActive == true && schedule.IsReject == false) ? "Pending"
                                                            : (schedule.IsReject == true) ? "Rejected" : "-",
                                                            IsActive = schedule.IsActive,
                                                            IntActionBy = schedule.IntActionBy,
                                                            DteActionDate = schedule.DteActionDate,
                                                            IntUpdatedBy = schedule.IntUpdatedBy,
                                                            DteUpdatedDate = schedule.DteUpdatedDate,
                                                            IsRequestedSchedule = schedule.IsRequestedSchedule,
                                                            IntRequestedByEmp = schedule.IntRequestedByEmp,
                                                            DteCourseCompletionDate = schedule.DteCourseCompletionDate,
                                                            DteExtentedDate = schedule.DteExtentedDate,
                                                            DteLastAssesmentSubmissionDate = schedule.DteLastAssesmentSubmissionDate,
                                                            StrEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == schedule.IntRequestedByEmp).Select(a => a.StrEmployeeName).FirstOrDefault(),
                                                            TotalRequisition = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.StrApprovalStatus == "Approved").Count()
                                                        }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingDDL>> TrainingScheduleDDL()
        {
            try
            {
                List<TrainingDDL> data = await (from schedule in _context.TrainingSchedules
                                                where schedule.IsActive == true
                                                orderby schedule.DteActionDate descending, schedule.IntScheduleId descending
                                                select new TrainingDDL
                                                {
                                                    Value = schedule.IntScheduleId,
                                                    Label = schedule.StrTrainingName + " [" + schedule.DteFromDate.ToString("dd MMM, yy") + " - " + schedule.DteToDate.ToString("dd MMM, yy") + "]",
                                                    Name = schedule.StrTrainingName,
                                                    FromDate = schedule.DteFromDate,
                                                    ToDate = schedule.DteToDate,
                                                    ResourcePerson = schedule.StrResourcePersonName,
                                                    RequisitionId = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId).Select(x => x.IntRequisitionId).FirstOrDefault()
                                                }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteTrainingSchedule(long id)
        {
            try
            {
                TrainingSchedule obj = await _context.TrainingSchedules.FirstAsync(x => x.IntScheduleId == id);
                obj.IsActive = false;
                _context.TrainingSchedules.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion -- Training Schedule --

        #region -- Trining Requisition --
        public async Task<MessageHelperBulkUpload> CreateTrainingRequisition(List<TrainingRequisitionDTO> obj)
        {
            try
            {
                MessageHelperBulkUpload message = new MessageHelperBulkUpload();

                List<TrainingRequisition> data = new List<TrainingRequisition>();
                List<ErrorList> list = new List<ErrorList>();
                foreach (var item in obj)
                {
                    var existData = _context.TrainingRequisitions.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntScheduleId == item.IntScheduleId && x.IsActive == true).FirstOrDefault();
                    if (existData != null)
                    {
                        var errorList = new ErrorList
                        {
                            Body = item.StrEmployeeName,
                            Title = "UnUploadList"
                        };
                        list.Add(errorList);
                    }
                    else
                    {
                        PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId((long)item.IntAccountId, "trainingRequisitionApproval");

                        TrainingRequisition requisition = new TrainingRequisition
                        {
                            IntScheduleId = (long)item.IntScheduleId,
                            StrTrainingName = item.StrTrainingName,
                            IntEmployeeId = item.IntEmployeeId,
                            StrEmployeeName = item.StrEmployeeName,
                            IntAccountId = item.IntAccountId,
                            IntBusinessUnitId = item.IntBusinessUnitId,
                            IntDesignationId = item.IntDesignationId,
                            StrDesignationName = item.StrDesignationName,
                            IntDepartmentId = item.IntDepartmentId,
                            StrEmail = item.StrEmail,
                            StrPhoneNo = item.StrPhoneNo,
                            StrGender = item.StrGender,
                            IntSupervisorId = item.IntSupervisorId,
                            IntEmploymentTypeId = item.IntEmploymentTypeId,
                            StrEmploymentType = item.StrEmploymentType,
                            DteActionDate = DateTime.Now,
                            IntActionBy = (long)item.IntActionBy,
                            IsActive = true,
                            StrApprovalStatus = "Pending",
                            StrRejectionComments = "",
                            IsFromRequisition = true,
                            IntPipelineHeaderId = stage.HeaderId,
                            IntCurrentStage = stage.CurrentStageId,
                            IntNextStage = stage.NextStageId,
                            StrStatus = "Pending",
                            IsPipelineClosed = false,
                            IsReject = false
                        };
                        data.Add(requisition);
                    }
                }
                if(data.Count > 0)
                {
                    await _context.TrainingRequisitions.AddRangeAsync(data);
                    await _context.SaveChangesAsync();
                    message.Message = "Created Successfully";
                    message.ListData = list;
                }
                return message;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingScheduleDTO>> GetApprovedTrainingRequisitionLanding(long intAccountId, long intBusinessUnitId)
        {
            try
            {
                List<TrainingScheduleDTO> data = await (from schedule in _context.TrainingSchedules
                                                        join training in _context.training on schedule.IntTrainingId equals training.IntTrainingId into train
                                                        from training in train.DefaultIfEmpty()
                                                        where schedule.IsActive == true
                                                        && schedule.IntAccountId == intAccountId && schedule.IntBusinessUnitId == intBusinessUnitId
                                                        && schedule.IsPipelineClosed == true
                                                        select new TrainingScheduleDTO
                                                        {
                                                            IntScheduleId = schedule.IntScheduleId,
                                                            IntTrainingId = schedule.IntTrainingId,
                                                            StrTrainingName = schedule.StrTrainingName,
                                                            StrTrainingCode = training.StrTrainingCode,
                                                            NumTotalDuration = schedule.NumTotalDuration,
                                                            StrVenue = schedule.StrVenue,
                                                            StrResourcePersonName = schedule.StrResourcePersonName,
                                                            IntBatchSize = schedule.IntBatchSize,
                                                            StrBatchNo = schedule.StrBatchNo,
                                                            DteFromDate = schedule.DteFromDate,
                                                            DteDate = schedule.DteDate,
                                                            DteToDate = schedule.DteToDate,
                                                            StrRemarks = schedule.StrRemarks,
                                                            StrStatus = schedule.StrStatus,
                                                            IsActive = schedule.IsActive,
                                                            IntActionBy = schedule.IntActionBy,
                                                            DteActionDate = schedule.DteActionDate,
                                                            IntUpdatedBy = schedule.IntUpdatedBy,
                                                            DteUpdatedDate = schedule.DteUpdatedDate,
                                                            IsRequestedSchedule = schedule.IsRequestedSchedule,
                                                            IntRequestedByEmp = schedule.IntRequestedByEmp,
                                                            DteCourseCompletionDate = schedule.DteCourseCompletionDate,
                                                            DteExtentedDate = schedule.DteExtentedDate,
                                                            DteLastAssesmentSubmissionDate = schedule.DteLastAssesmentSubmissionDate,
                                                            StrEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == schedule.IntRequestedByEmp).Select(a => a.StrEmployeeName).FirstOrDefault(),
                                                            TotalRequisition = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true).Count()
                                                        }).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TrainingRequisitionLandingDTO> GetTrainingRequisitionLanding(long? intScheduleId, long IntAccountId, long intBusinessUnitId)
        {
            try
            {
                TrainingRequisitionLandingDTO requisitionLandingDTO = new TrainingRequisitionLandingDTO();

                var trainingRequisitionList = await(_context.TrainingRequisitions
                        .Where(x => x.IntScheduleId == intScheduleId && x.IsActive == true && (long)x.IntAccountId == IntAccountId
                        && !(x.IsPipelineClosed == true && x.IsReject == true))).ToListAsync();

                var employeeList = await (from emp in _context.EmpEmployeeBasicInfos
                                    join empDetails in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empDetails.IntEmployeeId
                                    join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                    from dept in dept2.DefaultIfEmpty()
                                    join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                    from desig in desig2.DefaultIfEmpty()
                                    where emp.IsActive == true && emp.IntAccountId == IntAccountId
                                    select new  
                                    {
                                        IntEmployeeId = (long)emp.IntEmployeeBasicInfoId,
                                        StrEmployeeName = emp.StrEmployeeName,
                                        StrEmployeeCode = emp.StrEmployeeCode,
                                        IntBusinessUnitId = emp.IntBusinessUnitId,
                                        IntDesignationId = (long)emp.IntDesignationId,
                                        StrDesignationName = desig == null ? "" : (string)desig.StrDesignation,
                                        IntDepartmentId = (long)emp.IntDepartmentId,
                                        StrDepartmentName = dept == null ? "" : dept.StrDepartment,
                                        StrEmail = empDetails.StrOfficeMail,
                                        StrPhoneNo = (string)empDetails.StrPersonalMobile
                                    }).ToListAsync();


                List<TrainingRequisitionDTO> allReqData = (from emp in employeeList
                                                           join requisition in trainingRequisitionList on emp.IntEmployeeId equals requisition.IntEmployeeId into req
                                                           from requisitions in req.DefaultIfEmpty()
                                                           select new TrainingRequisitionDTO
                                                           {
                                                               IntRequisitionId = requisitions != null ? (long)requisitions.IntRequisitionId : null,
                                                               IntScheduleId = requisitions != null ? (long)requisitions.IntScheduleId : null,
                                                               StrTrainingName = requisitions != null ? (string)requisitions.StrTrainingName : null,
                                                               IntEmployeeId = (long)emp.IntEmployeeId,
                                                               StrEmployeeName = emp.StrEmployeeName,
                                                               StrEmployeeCode = emp.StrEmployeeCode,
                                                               IntBusinessUnitId = emp.IntBusinessUnitId,
                                                               IntDesignationId = (long)emp.IntDesignationId,
                                                               StrDesignationName =  (string)emp.StrDesignationName,
                                                               IntDepartmentId = (long)emp.IntDepartmentId,
                                                               StrDepartmentName = emp.StrDepartmentName,
                                                               StrEmail = emp.StrEmail,
                                                               StrPhoneNo = requisitions != null ? (string)requisitions.StrPhoneNo : (string)emp.StrPhoneNo,
                                                               DteActionDate = requisitions == null ? null : requisitions.DteActionDate,
                                                               IntActionBy = requisitions == null ? 0 : (long)requisitions.IntActionBy,
                                                               IsActive = requisitions == null ? false : (bool)requisitions.IsActive,
                                                               StrStatus = requisitions == null ? "NotAssigned" : (requisitions.IsPipelineClosed == true && requisitions.IsReject == false) ? "Assigned" : "Pending",
                                                               IsFromRequisition = requisitions == null ? false : requisitions.IsFromRequisition,
                                                           }).ToList();

                requisitionLandingDTO.data = allReqData;

                return requisitionLandingDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region === Submission Landing ===

        public async Task<TrainingRequisitionLandingDTO> GetTrainingSubmissionLanding(long scheduleId, long IntAccountId, long intBusinessUnitId)
        {
            try
            {

                List<TrainingRequisitionDTO> data = await (from requisition in _context.TrainingRequisitions
                                                           join emp1 in _context.EmpEmployeeBasicInfos on requisition.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                                                           from emp in emp2.DefaultIfEmpty()
                                                           join sch in _context.TrainingSchedules on requisition.IntScheduleId equals sch.IntScheduleId
                                                           join dept1 in _context.MasterDepartments on requisition.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                                           from dept in dept2.DefaultIfEmpty()
                                                           where requisition.IntScheduleId == scheduleId && requisition.IsActive == true && sch.IsActive == true && requisition.IsPipelineClosed == true
                                                           select new TrainingRequisitionDTO
                                                           {
                                                               IntRequisitionId = requisition.IntRequisitionId,
                                                               IntScheduleId = requisition.IntScheduleId,
                                                               StrTrainingName = requisition.StrTrainingName,
                                                               IntEmployeeId = requisition.IntEmployeeId,
                                                               StrEmployeeName = requisition.StrEmployeeName,
                                                               StrEmployeeCode = emp.StrEmployeeCode,
                                                               IntBusinessUnitId = requisition.IntBusinessUnitId,
                                                               IntDesignationId = requisition.IntDesignationId,
                                                               StrDesignationName = requisition.StrDesignationName,
                                                               IntDepartmentId = requisition.IntDepartmentId,
                                                               StrDepartmentName = dept.StrDepartment,
                                                               StrEmail = requisition.StrEmail,
                                                               StrPhoneNo = requisition.StrPhoneNo,
                                                               DteActionDate = requisition.DteActionDate,
                                                               IntActionBy = requisition.IntActionBy,
                                                               IsActive = requisition.IsActive,
                                                               IsFromRequisition = requisition.IsFromRequisition,
                                                               Attendance = _context.TrainingAttendances.Where(x => x.IntRequisitionId == requisition.IntRequisitionId && x.IntEmployeeId == requisition.IntEmployeeId).Count(),
                                                               PreAssessmentMarks = (from taq in _context.TrainingAssesmentQuestions
                                                                                     join taa1 in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa1.IntQuestionId into taa2
                                                                                     from taa in taa2.DefaultIfEmpty()
                                                                                     where taa.IntRequisitionId == requisition.IntRequisitionId && taq.IsPreAssesment == true && taq.IsActive == true
                                                                                     select taa.IntAnswerId).Count() > 0 ?
                                                                                    (from taq in _context.TrainingAssesmentQuestions
                                                                                     join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId into taa2
                                                                                     from taa in taa2.DefaultIfEmpty()
                                                                                     where taa.IntRequisitionId == requisition.IntRequisitionId && taq.IsPreAssesment == true && taq.IsActive == true
                                                                                     select taa.NumMarks).Sum().ToString() : "Not Submitted",
                                                               PostAssessmentMarks = (from taq in _context.TrainingAssesmentQuestions
                                                                                      join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId into taa2
                                                                                      from taa in taa2.DefaultIfEmpty()
                                                                                      where taa.IntRequisitionId == requisition.IntRequisitionId && taq.IsPreAssesment == false && taq.IsActive == true
                                                                                      select taa.IntAnswerId).Count() > 0 ?
                                                                                    (from taq in _context.TrainingAssesmentQuestions
                                                                                     join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId into taa2
                                                                                     from taa in taa2.DefaultIfEmpty()
                                                                                     where taa.IntRequisitionId == requisition.IntRequisitionId && taq.IsPreAssesment == false && taq.IsActive == true
                                                                                     select taa.NumMarks).Sum().ToString() : "Not Submitted",

                                                               //_context.TrainingAssesmentAnswares.Where(x => x.IntRequisitionId == requisition.IntRequisitionId).Select(x => x.NumMarks).Sum() >= 0 ? _context.TrainingAssesmentAnswares.Where(x => x.IntRequisitionId == requisition.IntRequisitionId).Select(x => x.NumMarks).Sum().ToString() : "Not Submitted"
                                                           }).ToListAsync();

                TrainingRequisitionLandingDTO requisitionLandingDTO = new TrainingRequisitionLandingDTO()
                {
                    data = data
                };

                return requisitionLandingDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingAssesmentQuestionDTO>> GetTrainingAssesmentQuestionAnswerById(long intScheduleId, long EmployeeId, long RequsitionId, bool isPreAssessment)
        {
            try
            {
                List<TrainingAssesmentQuestionDTO> data = await (from ques in _context.TrainingAssesmentQuestions
                                                                 join req in _context.TrainingRequisitions on ques.IntScheduleId equals req.IntScheduleId
                                                                 join ans in _context.TrainingAssesmentAnswares on req.IntRequisitionId equals ans.IntRequisitionId
                                                                 where ques.IntScheduleId == intScheduleId && ques.IsPreAssesment == isPreAssessment && ques.IsActive == true
                                                                 && req.IntRequisitionId == RequsitionId && req.IntEmployeeId == EmployeeId && req.IsActive == true && ques.IntQuestionId == ans.IntQuestionId
                                                                 && ans.IntRequisitionId == RequsitionId
                                                                 select new TrainingAssesmentQuestionDTO()
                                                                 {
                                                                     IntQuestionId = ques.IntQuestionId,
                                                                     StrQuestion = ques.StrQuestion,
                                                                     Options = (from option in _context.TrainingAssesmentQuestionOptions
                                                                                where option.IntQuestionId == ques.IntQuestionId && option.IsActive == true
                                                                                select new TrainingAssenmentQuestionOptionDTO()
                                                                                {
                                                                                    IntOptionId = option.IntOptionId,
                                                                                    StrOption = option.StrOption,
                                                                                    Marks = option.NumPoints
                                                                                }).ToList(),
                                                                     strAnswer = ans.StrOption,
                                                                     numMarks = ans.NumMarks,
                                                                     //PreAssessmentMarks = (from taq in _context.TrainingAssesmentQuestions
                                                                     //                      join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId
                                                                     //                      where taa.IntRequisitionId == req.IntRequisitionId && taq.IntScheduleId == req.IntScheduleId && taq.IsPreAssesment == true
                                                                     //                      select taa.NumMarks).Sum() >= 0 ?
                                                                     //               (from taq in _context.TrainingAssesmentQuestions
                                                                     //                join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId
                                                                     //                where taa.IntRequisitionId == req.IntRequisitionId && taq.IntScheduleId == req.IntScheduleId && taq.IsPreAssesment == true
                                                                     //                select taa.NumMarks).Sum().ToString() : "Not Submitted",
                                                                     //PostAssessmentMarks = (from taq in _context.TrainingAssesmentQuestions
                                                                     //                       join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId
                                                                     //                       where taa.IntRequisitionId == req.IntRequisitionId && taq.IntScheduleId == req.IntScheduleId && taq.IsPreAssesment == false
                                                                     //                       select taa.NumMarks).Sum() >= 0 ?
                                                                     //               (from taq in _context.TrainingAssesmentQuestions
                                                                     //                join taa in _context.TrainingAssesmentAnswares on taq.IntQuestionId equals taa.IntQuestionId
                                                                     //                where taa.IntRequisitionId == req.IntRequisitionId && taq.IntScheduleId == req.IntScheduleId && taq.IsPreAssesment == false
                                                                     //                select taa.NumMarks).Sum().ToString() : "Not Submitted",
                                                                     //PostProssessmentMark = _context.TrainingAssesmentAnswares.Where(x => x.IntQuestionId == ques.IntQuestionId).Select(x => x.NumMarks).FirstOrDefault(),
                                                                 }).ToListAsync();
                long count = data.Count();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion === Submission Landing ===

        public async Task<MessageHelper> TrainingRequisitionApproval(TrainingRequisitionApprovalDTO obj)
        {
            try
            {
                if (obj.intRequisitionId.Count() == 0)
                    throw new Exception("Invalid Data");

                List<TrainingRequisition> requisitionIdList = (from a in _context.TrainingRequisitions
                                                               where a.IsActive == true
                                                               && obj.intRequisitionId.ToList().Contains(a.IntRequisitionId)
                                                               select a).ToList();

                string? errorMessage = null;
                foreach (var item in requisitionIdList)
                {
                    var existData = (from sch in _context.TrainingSchedules
                                     join req in _context.TrainingRequisitions on sch.IntScheduleId equals req.IntScheduleId
                                     join att in _context.TrainingAttendances on req.IntScheduleId equals att.IntRequisitionId
                                     where sch.IntScheduleId == item.IntScheduleId
                                     && (att.StrAttendanceStatus.ToLower() == "present" || att.StrAttendanceStatus.ToLower() == "absent")
                                     && sch.IsActive == true && req.IsActive == true
                                     select sch).FirstOrDefault();
                    if (existData != null)
                    {
                        errorMessage = string.Concat(existData.StrTrainingName, "[", existData.DteFromDate.ToString("dd MMM, yy"), "-", existData.DteToDate.ToString("dd MMM, yy"), "] ");
                    }
                }
                if (errorMessage != null)
                {
                    throw new Exception($"Training(s) are already completed > {errorMessage}");
                }

                if (obj.isApproved == true)
                {
                    requisitionIdList.ForEach(x =>
                    {
                        x.StrApprovalStatus = "Approved";
                        x.IntApprovedBy = obj.intApprovedBy;
                        x.DteApprovedDate = DateTime.Now;
                        x.StrRejectionComments = "";
                    });

                    /// insert into Attendance Table
                    //await CreateTrainingAttendance(requisitionIdList);
                }
                else
                {
                    requisitionIdList.ForEach(x =>
                    {
                        x.StrApprovalStatus = "Rejected";
                        x.IntApprovedBy = obj.intRejectedBy;
                        x.DteApprovedDate = DateTime.Now;
                        x.StrRejectionComments = obj.Comments;
                    });

                    await DeleteFromTrainingAttendance(requisitionIdList);
                }

                _context.TrainingRequisitions.UpdateRange(requisitionIdList);
                await _context.SaveChangesAsync();

                return new MessageHelper { Message = obj.isApproved ? "Approved Successfully" : "Rejected Successfully" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CreateTrainingAttendance(List<TrainingRequisitionDTO> insertList)
        {
            MessageHelper msg = new MessageHelper();

            List<TrainingAttendance> attendance = new List<TrainingAttendance>();
            foreach (var item in insertList)
            {
                var attendanceCount = _context.TrainingAttendances.Where(x => x.IntRequisitionId == item.IntRequisitionId && x.DteAttendanceDate == item.dteAttendanceDate && x.IntEmployeeId == item.IntEmployeeId).Count();
                if (attendanceCount == 0)
                {
                    DateTime fromDate, toDate;
                    var scheduleData = _context.TrainingSchedules.Where(a => a.IntScheduleId == item.IntScheduleId).Select(a => a).FirstOrDefault();
                    fromDate = scheduleData.DteFromDate.Date;
                    toDate = scheduleData.DteToDate.Date;
                    if (fromDate <= item.dteAttendanceDate && toDate >= item.dteAttendanceDate)
                    {
                        TrainingAttendance data = new TrainingAttendance
                        {
                            IntRequisitionId = (long)item.IntRequisitionId,
                            IntEmployeeId = item.IntEmployeeId,
                            StrAttendanceStatus = item.StrAttendanceStatus,
                            DteAttendanceDate = (DateTime)item.dteAttendanceDate,
                            IntAccountId = (long)item.IntAccountId,
                            IntBusinessUnitId = item.IntBusinessUnitId,
                            DteActionDate = item.DteActionDate,
                            IntActionBy = item.IntActionBy
                        };
                        attendance.Add(data);
                    }
                    else
                    {
                        msg.StatusCode = 500;
                        msg.Message = "Please correct attendance date!";
                    }

                }
            }
            if (attendance.Count > 0)
            {
                await _context.TrainingAttendances.AddRangeAsync(attendance);
                await _context.SaveChangesAsync();

                msg.StatusCode = 200;
                msg.Message = "Create Successfully";
            }

            return msg;
        }

        public async Task DeleteFromTrainingAttendance(List<TrainingRequisition> deleteList)
        {
            var att = new List<TrainingAttendance>();

            var attendance = await _context.TrainingAttendances
            .Where(x => x.IntRequisitionId == deleteList.Select(x => x.IntScheduleId).FirstOrDefault() && x.IntEmployeeId == deleteList.Select(x => x.IntEmployeeId).FirstOrDefault()).ToListAsync();

            _context.TrainingAttendances.RemoveRange(attendance);
            await _context.SaveChangesAsync();
        }

        #endregion -- Trining Requisition --

        #region -- Training Attendance --

        public async Task<List<TrainingScheduleDTO>> GetTrainingAssesmentLanding(long? intTrainingId, long intAccountId, long intBusinessUnitId)
        {
            try
            {

                List<TrainingScheduleDTO> data = await (from schedule in _context.TrainingSchedules
                                                        join training in _context.training on schedule.IntTrainingId equals training.IntTrainingId into train
                                                        from training in train.DefaultIfEmpty()
                                                        where schedule.IsActive == true
                                                        && (schedule.IntScheduleId == intTrainingId || intTrainingId == 0 || intTrainingId == null)
                                                        && schedule.IntAccountId == intAccountId && schedule.IntBusinessUnitId == intBusinessUnitId
                                                        && schedule.IsPipelineClosed == true
                                                        && (_context.TrainingRequisitions.Where(x=> x.IntScheduleId == schedule.IntScheduleId && x.IsPipelineClosed == true && x.IsReject == false).Count() > 0)
                                                        select new TrainingScheduleDTO
                                                        {
                                                            IntScheduleId = schedule.IntScheduleId,
                                                            IntTrainingId = schedule.IntTrainingId,
                                                            StrTrainingName = schedule.StrTrainingName,
                                                            StrTrainingCode = training.StrTrainingCode,
                                                            NumTotalDuration = schedule.NumTotalDuration,
                                                            StrVenue = schedule.StrVenue,
                                                            StrResourcePersonName = schedule.StrResourcePersonName,
                                                            IntBatchSize = schedule.IntBatchSize,
                                                            StrBatchNo = schedule.StrBatchNo,
                                                            DteFromDate = schedule.DteFromDate,
                                                            DteDate = schedule.DteDate,
                                                            DteToDate = schedule.DteToDate,
                                                            StrRemarks = schedule.StrRemarks,
                                                            StrStatus = schedule.StrStatus,
                                                            IsActive = schedule.IsActive,
                                                            IntActionBy = schedule.IntActionBy,
                                                            DteActionDate = schedule.DteActionDate,
                                                            IntUpdatedBy = schedule.IntUpdatedBy,
                                                            DteUpdatedDate = schedule.DteUpdatedDate,
                                                            IsRequestedSchedule = schedule.IsRequestedSchedule,
                                                            IntRequestedByEmp = schedule.IntRequestedByEmp,
                                                            DteCourseCompletionDate = schedule.DteCourseCompletionDate,
                                                            DteExtentedDate = schedule.DteExtentedDate,
                                                            DteLastAssesmentSubmissionDate = schedule.DteLastAssesmentSubmissionDate,
                                                            IntRequisitionId = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.StrStatus == "Approved By Admin" && x.IsPipelineClosed == true).Select(a=> a.IntRequisitionId).FirstOrDefault(),
                                                            StrEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == schedule.IntRequestedByEmp).Select(a => a.StrEmployeeName).FirstOrDefault(),
                                                            TotalRequisition = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true).Count()
                                                        }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingScheduleDTO>> GetTrainingSelfAssesmentLanding(long? intTrainingId, long intAccountId, long intEmployeeId, long intBusinessUnitId)
        {
            try
            {

                List<TrainingScheduleDTO> data = await (from schedule in _context.TrainingSchedules
                                                        join req1 in _context.TrainingRequisitions on schedule.IntScheduleId equals req1.IntScheduleId into req2
                                                        from req in req2.DefaultIfEmpty()
                                                        join training in _context.training on schedule.IntTrainingId equals training.IntTrainingId into train
                                                        from training in train.DefaultIfEmpty()
                                                        where schedule.IsActive == true
                                                        && (schedule.IntScheduleId == intTrainingId || intTrainingId == 0 || intTrainingId == null)
                                                        && schedule.IntAccountId == intAccountId && schedule.IntBusinessUnitId == intBusinessUnitId
                                                        && schedule.IsPipelineClosed == true
                                                        && (_context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId).Count() > 0)
                                                        && req.IntEmployeeId == intEmployeeId
                                                        select new TrainingScheduleDTO
                                                        {
                                                            IntScheduleId = schedule.IntScheduleId,
                                                            IntTrainingId = schedule.IntTrainingId,
                                                            StrTrainingName = schedule.StrTrainingName,
                                                            StrTrainingCode = training.StrTrainingCode,
                                                            NumTotalDuration = schedule.NumTotalDuration,
                                                            StrVenue = schedule.StrVenue,
                                                            StrResourcePersonName = schedule.StrResourcePersonName,
                                                            IntBatchSize = schedule.IntBatchSize,
                                                            StrBatchNo = schedule.StrBatchNo,
                                                            DteFromDate = schedule.DteFromDate,
                                                            DteDate = schedule.DteDate,
                                                            DteToDate = schedule.DteToDate,
                                                            StrRemarks = schedule.StrRemarks,
                                                            StrStatus = schedule.StrStatus,
                                                            IsActive = schedule.IsActive,
                                                            IntActionBy = schedule.IntActionBy,
                                                            DteActionDate = schedule.DteActionDate,
                                                            IntUpdatedBy = schedule.IntUpdatedBy,
                                                            DteUpdatedDate = schedule.DteUpdatedDate,
                                                            IsRequestedSchedule = schedule.IsRequestedSchedule,
                                                            IntRequestedByEmp = schedule.IntRequestedByEmp,
                                                            DteCourseCompletionDate = schedule.DteCourseCompletionDate,
                                                            DteExtentedDate = schedule.DteExtentedDate,
                                                            DteLastAssesmentSubmissionDate = schedule.DteLastAssesmentSubmissionDate,
                                                            IntRequisitionId = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true).Select(a => a.IntRequisitionId).FirstOrDefault(),
                                                            StrEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == schedule.IntRequestedByEmp).Select(a => a.StrEmployeeName).FirstOrDefault(),
                                                            TotalRequisition = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true).Count()
                                                        }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TrainingAttendanceLandingDTO> GetScheduleEmployeeListForTrainingAttendance(long scheduleId, DateTime? attendanceDate, long IntAccountId, long intBusinessUnitId)
        {
            try
            {

                var requisitionList = await (from requisition in _context.TrainingRequisitions
                                             join dept1 in _context.MasterDepartments on requisition.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                             from dept in dept2.DefaultIfEmpty()
                                             where requisition.IntScheduleId == scheduleId && requisition.IsPipelineClosed == true && requisition.IsReject == false
                                             select new
                                             {
                                                 IntRequisitionId = requisition.IntRequisitionId,
                                                 IntEmployeeId = requisition.IntEmployeeId,
                                                 StrEmployeeName = requisition.StrEmployeeName,
                                                 StrDesignationName = requisition.StrDesignationName,
                                                 IntDesignationId = requisition.IntDesignationId,
                                                 IntDepartmentId = requisition.IntDepartmentId,
                                                 StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                 StrEmail = requisition.StrEmail,
                                                 StrPhoneNo = requisition.StrPhoneNo,
                                                 IsActive = requisition.IsActive,
                                                 IntScheduleId = requisition.IntScheduleId,
                                                 IsPipelineClosed = requisition.IsPipelineClosed,
                                             }).ToListAsync();

                var attendanceList = await (from requisition in _context.TrainingRequisitions
                                             join attendance in _context.TrainingAttendances on requisition.IntRequisitionId equals attendance.IntRequisitionId
                                             where requisition.IntScheduleId == scheduleId && requisition.IsPipelineClosed == true && requisition.IsReject == false
                                             && attendance.DteAttendanceDate.Date == attendanceDate.Value.Date
                                            select new
                                             {
                                                 IntRequisitionId = requisition.IntRequisitionId,
                                                 IntAttendanceId = attendance.IntAttendanceId,    
                                                 AttendanceDate = attendance.DteAttendanceDate,
                                                 StrAttendanceStatus = attendance.StrAttendanceStatus == "Present" ? true : false
                                             }).ToListAsync();




                List<ScheduleEmployeeListForTrainingAttendance> resultList = (from req in requisitionList
                                join att1 in attendanceList on req.IntRequisitionId equals att1.IntRequisitionId into att2
                                from att in att2.DefaultIfEmpty()
                                select new ScheduleEmployeeListForTrainingAttendance
                                {
                                    IntRequisitionId = req.IntRequisitionId,
                                    IntEmployeeId = req.IntEmployeeId,
                                    StrEmployeeName = req.StrEmployeeName,
                                    Designation = req.StrDesignationName,
                                    DesignationId = req.IntDesignationId,
                                    DepartmentId = req.IntDepartmentId,
                                    DepartmentName = req.StrDepartment,
                                    Email = req.StrEmail,
                                    Phone = req.StrPhoneNo,
                                    IntScheduleId = req.IntScheduleId,
                                    IntAttendanceId = att == null ? 0 : att.IntAttendanceId,
                                    AttendanceDate = att == null ? DateTime.Now.Date : att.AttendanceDate,
                                    StrAttendanceStatus = att == null ? false : att.StrAttendanceStatus
                                }).ToList();


             
                TrainingAttendanceLandingDTO trainingAttendance = new TrainingAttendanceLandingDTO()
                {
                    data = resultList
                };
                return trainingAttendance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> PresentAbsentProcessOfAttendance(List<PresentAbsentProcessOfAttendanceDTO> obj)
        {
            try
            {
                var updateDateList = new List<TrainingAttendance>();

                foreach (var item in obj)
                {
                    var attendanceId = (from att in _context.TrainingAttendances
                                        where item.AttendanceId == att.IntAttendanceId
                                        select att).FirstOrDefault();

                    if (item.isPresent == true)
                    {
                        attendanceId.StrAttendanceStatus = "Present";
                        attendanceId.DteActionDate = DateTime.Now;
                        attendanceId.IntActionBy = item.ActionBy;
                        updateDateList.Add(attendanceId);
                    }
                    else
                    {
                        attendanceId.StrAttendanceStatus = "Absent";
                        attendanceId.DteActionDate = DateTime.Now;
                        attendanceId.IntActionBy = item.ActionBy;
                        updateDateList.Add(attendanceId);
                    }
                }
                _context.TrainingAttendances.UpdateRange(updateDateList);
                await _context.SaveChangesAsync();

                return new MessageHelper { Message = "Successful" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion -- Training Attendance --

        #region -- Training Assesment --

        public async Task<MessageHelper> CreateTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    var isQuesExist = _context.TrainingAssesmentQuestions.Where(x => x.IsActive == true && x.IntScheduleId == item.IntScheduleId && x.StrQuestion.Trim().ToLower() == item.StrQuestion.Trim().ToLower()).FirstOrDefault();
                    if (isQuesExist != null)
                        throw new Exception($"This question > {item.StrQuestion} already exists in this schedule");
                }

                foreach (var item in obj)
                {
                    TrainingAssesmentQuestion ques = new TrainingAssesmentQuestion
                    {
                        StrQuestion = item.StrQuestion,
                        IntScheduleId = item.IntScheduleId,
                        IsPreAssesment = item.IsPreAssesment,
                        IsActive = true,
                        DteLastActionDate = DateTime.Now,
                        IntActionBy = item.IntActionBy,
                        IsRequired = item.IsRequired,
                        StrInputType = item.StrInputType,
                        IntOrder = item.IntOrder
                    };
                    await _context.TrainingAssesmentQuestions.AddAsync(ques);
                    await _context.SaveChangesAsync();

                    List<TrainingAssesmentQuestionOption> opt = new List<TrainingAssesmentQuestionOption>();
                    foreach (var data in item.Options)
                    {
                        TrainingAssesmentQuestionOption option = new TrainingAssesmentQuestionOption
                        {
                            IntQuestionId = ques.IntQuestionId,
                            StrOption = data.StrOption,
                            NumPoints = (long)data.NumPoints,
                            IntActionBy = data.IntActionBy,
                            DteLastAction = DateTime.Now,
                            IntOrder = data.IntOrder,
                            IsActive = true
                        };
                        opt.Add(option);
                    }
                    await _context.TrainingAssesmentQuestionOptions.AddRangeAsync(opt);
                    await _context.SaveChangesAsync();
                }
                msg.Message = "Created Successfully";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TrainingAssesmentQuestionDTO>> GetTrainingAssesmentQuestionByScheduleId(long? employeeId ,long intScheduleId, bool isPreAssessment)
        {
            try
            {
                //var requisition = _context.TrainingRequisitions
                //    .Where(x => x.IntScheduleId == intScheduleId && x.IntEmployeeId == employeeId).ToList();

                List<TrainingAssesmentQuestionDTO> data = new List<TrainingAssesmentQuestionDTO>();

                if(employeeId == null || employeeId == 0)
                {
                        data = await (from ques in _context.TrainingAssesmentQuestions
                        where ques.IsActive == true
                        && ques.IntScheduleId == intScheduleId && ques.IsPreAssesment == isPreAssessment
                        select new TrainingAssesmentQuestionDTO
                        {
                            IntQuestionId = ques.IntQuestionId,
                            StrQuestion = ques.StrQuestion,
                            IntScheduleId = ques.IntScheduleId,
                            IsPreAssesment = ques.IsPreAssesment,
                            IsActive = ques.IsActive,
                            IsRequired = ques.IsRequired,
                            StrInputType = ques.StrInputType,
                            IntOrder = ques.IntOrder,
                            Options = (from opt in _context.TrainingAssesmentQuestionOptions
                                        join ans1 in _context.TrainingAssesmentAnswares on opt.IntOptionId equals ans1.IntOptionId into ans2
                                        from ans in ans2.DefaultIfEmpty()
                                        where opt.IntQuestionId == ques.IntQuestionId && opt.IsActive == true
                                        select new TrainingAssenmentQuestionOptionDTO
                                        {
                                            IntOptionId = opt.IntOptionId,
                                            StrOption = opt.StrOption,
                                            IntQuestionId = opt.IntQuestionId,
                                            NumPoints = opt.NumPoints,
                                            IntOrder = opt.IntOrder,
                                            isAnswer = (ans != null && ans.IntOptionId == opt.IntOptionId) ? true : false
                                        }).ToList()
                        }).ToListAsync();
                    
                }
                else
                {


                        data = await (from ques in _context.TrainingAssesmentQuestions
                        join req1 in _context.TrainingRequisitions on ques.IntScheduleId equals req1.IntScheduleId into req2
                        from req in req2.DefaultIfEmpty()
                        where ques.IsActive == true
                        && ques.IntScheduleId == intScheduleId && ques.IsPreAssesment == isPreAssessment
                        && (req.IntEmployeeId == employeeId || employeeId == 0 || employeeId == null)
                        select new TrainingAssesmentQuestionDTO
                        {
                            IntQuestionId = ques.IntQuestionId,
                            StrQuestion = ques.StrQuestion,
                            IntScheduleId = ques.IntScheduleId,
                            IsPreAssesment = ques.IsPreAssesment,
                            IsActive = ques.IsActive,
                            IsRequired = ques.IsRequired,
                            StrInputType = ques.StrInputType,
                            IntOrder = ques.IntOrder,
                            IntRequisitionId = _context.TrainingRequisitions.Where(x => x.IntScheduleId == intScheduleId && x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true).Select(a => a.IntRequisitionId).FirstOrDefault(),
                            Options = (from opt in _context.TrainingAssesmentQuestionOptions
                                        join ans1 in _context.TrainingAssesmentAnswares on opt.IntOptionId equals ans1.IntOptionId into ans2
                                        from ans in ans2.DefaultIfEmpty()
                                        where
                                        opt.IntQuestionId == ques.IntQuestionId &&
                                        opt.IsActive == true
                                        //&& ans.IntActionBy == employeeId
                                       select new TrainingAssenmentQuestionOptionDTO
                                        {
                                            IntOptionId = opt.IntOptionId,
                                            StrOption = opt.StrOption,
                                            IntQuestionId = opt.IntQuestionId,
                                            NumPoints = opt.NumPoints,
                                            IntOrder = opt.IntOrder,
                                            isAnswer = (ans != null && ans.IntOptionId == opt.IntOptionId && ans.IntActionBy == employeeId) ? true : false
                                       }).ToList()
                        }).ToListAsync();
                }

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Create Assessment Question and Also Edit Assessment Question.
        public async Task<MessageHelper> EditTrainingAssesmentQuestion(List<TrainingAssesmentQuestionDTO> obj)
        {
            try
            {
                bool isPreAssesment = obj.Select(a => a.IsPreAssesment).FirstOrDefault();
                long scheduleId = obj.Select(a => a.IntScheduleId).FirstOrDefault();

                var schedule = _context.TrainingSchedules.Where(x => x.IsActive == true && x.IntScheduleId == scheduleId).FirstOrDefault();
                var questionId = (from ques in obj
                                  where ques.IntQuestionId > 0
                                  select ques.IntQuestionId);

                var inActiveQues = (from question in _context.TrainingAssesmentQuestions
                                    where question.IntScheduleId == schedule.IntScheduleId && question.IsPreAssesment == isPreAssesment && question.IsActive == true
                                    && !questionId.Contains(question.IntQuestionId)
                                    select question).ToList();
                inActiveQues.ForEach(x =>
                {
                    x.IsActive = false;
                });
                _context.TrainingAssesmentQuestions.UpdateRange(inActiveQues);
                await _context.SaveChangesAsync();

                foreach (var item in obj)
                {
                    if (item.IntQuestionId == 0)
                    {
                        TrainingAssesmentQuestion question = new TrainingAssesmentQuestion
                        {
                            StrQuestion = item.StrQuestion,
                            IsPreAssesment = item.IsPreAssesment,
                            IntScheduleId = item.IntScheduleId,
                            IsActive = true,
                            IntActionBy = item.IntActionBy,
                            DteLastActionDate = DateTime.Now,
                            IsRequired = item.IsRequired,
                            StrInputType = item.StrInputType,
                            IntOrder = item.IntOrder
                        };
                        await _context.TrainingAssesmentQuestions.AddAsync(question);
                        await _context.SaveChangesAsync();

                        List<TrainingAssesmentQuestionOption> option = new List<TrainingAssesmentQuestionOption>();
                        foreach (var data in item.Options)
                        {
                            TrainingAssesmentQuestionOption opt = new TrainingAssesmentQuestionOption
                            {
                                StrOption = data.StrOption,
                                IntQuestionId = question.IntQuestionId,
                                NumPoints = (long)data.NumPoints,
                                IntActionBy = data.IntActionBy,
                                DteLastAction = DateTime.Now,
                                IntOrder = data.IntOrder,
                                IsActive = true
                            };
                            option.Add(opt);
                        }
                        await _context.TrainingAssesmentQuestionOptions.AddRangeAsync(option);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var question = _context.TrainingAssesmentQuestions.Where(x => x.IntQuestionId == item.IntQuestionId).FirstOrDefault();
                        question.StrQuestion = item.StrQuestion;
                        question.IntActionBy = item.IntActionBy;
                        question.DteLastActionDate = DateTime.Now;
                        question.StrInputType = item.StrInputType;
                        question.IntOrder = item.IntOrder;
                        question.IsRequired = item.IsRequired;

                        _context.TrainingAssesmentQuestions.Update(question);
                        await _context.SaveChangesAsync();

                        List<TrainingAssesmentQuestionOption> newRowList = new List<TrainingAssesmentQuestionOption>();
                        List<TrainingAssesmentQuestionOption> existingRowList = new List<TrainingAssesmentQuestionOption>();
                        foreach (var data in item.Options)
                        {
                            if (data.IntOptionId == 0)
                            {
                                TrainingAssesmentQuestionOption option = new TrainingAssesmentQuestionOption
                                {
                                    StrOption = data.StrOption,
                                    IntQuestionId = question.IntQuestionId,
                                    NumPoints = (long)data.NumPoints,
                                    IntActionBy = data.IntActionBy,
                                    DteLastAction = DateTime.Now,
                                    IntOrder = data.IntOrder,
                                    IsActive = true
                                };
                                newRowList.Add(option);
                            }
                            else
                            {
                                TrainingAssesmentQuestionOption opt = _context.TrainingAssesmentQuestionOptions.Where(x => x.IntOptionId == data.IntOptionId).FirstOrDefault();
                                opt.StrOption = data.StrOption;
                                opt.NumPoints = (long)data.NumPoints;
                                opt.DteLastAction = DateTime.Now;
                                opt.IntActionBy = data.IntActionBy;
                                opt.IntOrder = data.IntOrder;

                                existingRowList.Add(opt);
                            }
                        }

                        var optionId = (from opt in item.Options
                                        where opt.IntOptionId > 0
                                        select opt.IntOptionId);

                        var inActiveOptionId = (from opt in _context.TrainingAssesmentQuestionOptions
                                                where opt.IntQuestionId == question.IntQuestionId && opt.IsActive == true
                                                && !optionId.Contains(opt.IntOptionId)
                                                select opt).ToList();

                        if (inActiveOptionId.Count() > 0)
                        {
                            inActiveOptionId.ForEach(x =>
                            {
                                x.IsActive = false;
                            });
                            _context.TrainingAssesmentQuestionOptions.UpdateRange(inActiveOptionId);
                            await _context.SaveChangesAsync();
                        }
                        if (newRowList.Count() > 0)
                        {
                            await _context.TrainingAssesmentQuestionOptions.AddRangeAsync(newRowList);
                            await _context.SaveChangesAsync();
                        }
                        else if (existingRowList.Count() > 0)
                        {
                            _context.TrainingAssesmentQuestionOptions.UpdateRange(existingRowList);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                msg.Message = "Edited Successfully";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<MessageHelper> AssessmentQuestionDelete(bool isPreassesment, long ScheduleId)
        {
            try
            {

                TrainingAssesmentQuestion questionStatus = await (from question in _context.TrainingAssesmentQuestions
                                      where question.IsPreAssesment == isPreassesment && question.IntScheduleId == ScheduleId && question.IsActive == true
                                      select question).FirstOrDefaultAsync();

                questionStatus.IsActive = false;

                _context.TrainingAssesmentQuestions.Update(questionStatus);
                await _context.SaveChangesAsync();

                var questionOptionStatus = (from option in _context.TrainingAssesmentQuestionOptions
                                            where option.IntQuestionId == questionStatus.IntQuestionId && option.IsActive == true
                                            select option).ToList();


                questionOptionStatus.ForEach(x =>
                {
                    x.IsActive = false;
                });
                _context.TrainingAssesmentQuestionOptions.UpdateRange(questionOptionStatus);
                await _context.SaveChangesAsync();

                var answerStatus = (from option in _context.TrainingAssesmentAnswares
                                            where option.IntQuestionId == questionStatus.IntQuestionId && option.IsActive == true
                                            select option).ToList();


                answerStatus.ForEach(x =>
                {
                    x.IsActive = false;
                });
                _context.TrainingAssesmentAnswares.UpdateRange(answerStatus);
                await _context.SaveChangesAsync();

                msg.Message = "Delete Successfully";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CreateTrainingAssesmentAnswer(List<TrainingAssesmentAnswerDTO> obj)
        {
            try
            {
                var data = _context.TrainingAssesmentAnswares
                                    .Where(x => x.IntRequisitionId == obj.Select(a => a.IntRequisitionId).FirstOrDefault() && x.IntActionBy == obj.Select(a => a.IntActionBy).FirstOrDefault() && x.IntQuestionId == obj.Select(a => a.IntQuestionId).FirstOrDefault()).Count();

                if (data > 0) throw new Exception("Assesment already submitted");

                List<TrainingAssesmentAnsware> ansList = new List<TrainingAssesmentAnsware>();
                foreach (var item in obj)
                {
                    if (item.IntOptionId > 0)
                    {
                        var marks = _context.TrainingAssesmentQuestionOptions.Where(x => x.IntQuestionId == item.IntQuestionId && x.IntOptionId == item.IntOptionId).Select(a => a.NumPoints).FirstOrDefault();
                        TrainingAssesmentAnsware answer = new TrainingAssesmentAnsware();
                        answer.IntQuestionId = item.IntQuestionId;
                        answer.IntOptionId = item.IntOptionId;
                        answer.StrOption = item.StrOption;
                        answer.NumMarks = marks;
                        answer.DteLastAction = DateTime.Now;
                        answer.IntActionBy = item.IntActionBy;
                        answer.IntRequisitionId = (long)item.IntRequisitionId;
                        answer.IsActive = true;

                        ansList.Add(answer);
                    }
                }
                if (ansList.Count() > 0)
                {
                    await _context.TrainingAssesmentAnswares.AddRangeAsync(ansList);
                    await _context.SaveChangesAsync();
                }

                msg.Message = "Submitted Successfully";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get assesment details
        public async Task<TrainingAssesmentQuestionCommonDTO> GetTrainingAssesmentQuestionAnswerByRequisitionId(long intScheduleId,long intEmployeeId, long intRequisitionId, bool isPreAssessment, long IntAccountId, long intBusinessUnitId)
        {
            try
            {
                TrainingRequisitionForAssesmentDTO requisition = await (from req in _context.TrainingRequisitions
                                                           join dept1 in _context.MasterDepartments on req.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                                           from dept in dept2.DefaultIfEmpty()
                                                           join desig1 in _context.MasterDesignations on req.IntDesignationId equals desig1.IntDesignationId into desig2
                                                           from desig in desig2.DefaultIfEmpty()
                                                           where req.IsActive == true 
                                                           && req.IntAccountId == IntAccountId 
                                                           && req.IntBusinessUnitId == intBusinessUnitId
                                                           && req.IntRequisitionId == intRequisitionId 
                                                           && req.IntScheduleId == intScheduleId
                                                           && req.IntEmployeeId == intEmployeeId
                                                           //&& (trainingSchedule.IntScheduleId != null ? trainingSchedule.IntScheduleId : 0)
                                                           select new TrainingRequisitionForAssesmentDTO
                                                           {
                                                               IntRequisitionId = (long)req.IntRequisitionId,
                                                               IntScheduleId = (long)req.IntScheduleId,
                                                               StrTrainingName = (string)req.StrTrainingName,
                                                               IntEmployeeId = (long)req.IntEmployeeId,
                                                               StrEmployeeName = req.StrEmployeeName,
                                                               IntBusinessUnitId = req.IntBusinessUnitId,
                                                               IntDesignationId = (long)req.IntDesignationId,
                                                               StrDesignationName = (string)desig.StrDesignation,
                                                               IntDepartmentId = (long)req.IntDepartmentId,
                                                               StrDepartmentName = dept == null ? "" : dept.StrDepartment,
                                                               StrEmail = req.StrEmail,
                                                               StrPhoneNo = req.StrPhoneNo == null ? "" : req.StrPhoneNo,
                                                               Attendance = _context.TrainingAttendances.Where(x=> x.IntEmployeeId == req.IntEmployeeId && x.StrAttendanceStatus == "Present").Count(),
                                                               TotalMark = _context.TrainingAssesmentAnswares.Where(x=> x.IntRequisitionId == intRequisitionId).Select(a=> a.NumMarks).Sum(),
                                                               TotalQuestion = _context.TrainingAssesmentQuestions.Where(x => x.IntScheduleId == intScheduleId).Count(),
                                                               TotalQuestionAnswered = _context.TrainingAssesmentAnswares.Where(x => x.IntRequisitionId == intRequisitionId).Count()
                                                           }).FirstOrDefaultAsync();
                List<TrainingAssesmentQuestionDTO> data = await (from ques in _context.TrainingAssesmentQuestions
                                                                 join req1 in _context.TrainingRequisitions on ques.IntScheduleId equals req1.IntScheduleId into req2
                                                                 from req in req2.DefaultIfEmpty()
                                                                 where //ques.IsActive == true && 
                                                                 ques.IntScheduleId == intScheduleId && ques.IsPreAssesment == isPreAssessment
                                                                 && req.IntEmployeeId == intEmployeeId
                                                                 select new TrainingAssesmentQuestionDTO
                                                                 {
                                                                     IntQuestionId = ques.IntQuestionId,
                                                                     StrQuestion = ques.StrQuestion,
                                                                     IntScheduleId = ques.IntScheduleId,
                                                                     IsPreAssesment = ques.IsPreAssesment,
                                                                     IsActive = ques.IsActive,
                                                                     IsRequired = ques.IsRequired,
                                                                     StrInputType = ques.StrInputType,
                                                                     IntOrder = ques.IntOrder,
                                                                     Options = (from opt in _context.TrainingAssesmentQuestionOptions
                                                                                join ans1 in _context.TrainingAssesmentAnswares on opt.IntOptionId equals ans1.IntOptionId into ans2
                                                                                from ans in ans2.DefaultIfEmpty()
                                                                                where
                                                                                opt.IntQuestionId == ques.IntQuestionId &&
                                                                                opt.IsActive == true
                                                                                //&& ans.IntActionBy == employeeId
                                                                                select new TrainingAssenmentQuestionOptionDTO
                                                                                {
                                                                                    IntOptionId = opt.IntOptionId,
                                                                                    StrOption = opt.StrOption,
                                                                                    IntQuestionId = opt.IntQuestionId,
                                                                                    NumPoints = opt.NumPoints,
                                                                                    IntOrder = opt.IntOrder,
                                                                                    isAnswer = (ans != null && ans.IntOptionId == opt.IntOptionId && ans.IntActionBy == intEmployeeId) ? true : false
                                                                                }).ToList()
                                                                 }).ToListAsync();

                TrainingAssesmentQuestionCommonDTO trainingAssesment = new TrainingAssesmentQuestionCommonDTO()
                {
                    requisitionForAssesment = requisition,
                    question = data
                };
                return trainingAssesment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion -- Training Assesment --

        #region ==== Employee Assessment ====

        public async Task<List<TrainingDDL>> EmployeeTrainingScheduleLanding(long employeeId)
        {
            try
            {
                List<TrainingDDL> data = await (from schedule in _context.TrainingSchedules
                                                join requisition in _context.TrainingRequisitions on schedule.IntScheduleId equals requisition.IntScheduleId
                                                where requisition.IntEmployeeId == employeeId && schedule.IsActive == true && requisition.IsActive == true && requisition.StrApprovalStatus == "Approved"
                                                orderby schedule.DteFromDate.Date descending
                                                select new TrainingDDL
                                                {
                                                    Value = schedule.IntScheduleId,
                                                    Label = schedule.StrTrainingName + " [" + schedule.DteFromDate.ToString("dd MMM, yy") + " - " + schedule.DteToDate.ToString("dd MMM, yy") + "]",
                                                    Name = schedule.StrTrainingName,
                                                    FromDate = schedule.DteFromDate,
                                                    ToDate = schedule.DteToDate,
                                                    ResourcePerson = schedule.StrResourcePersonName,
                                                    Status = requisition.StrApprovalStatus,
                                                    RequisitionId = _context.TrainingRequisitions.Where(x => x.IntScheduleId == schedule.IntScheduleId && x.IntEmployeeId == employeeId).Select(x => x.IntRequisitionId).FirstOrDefault(),
                                                    isPreSubmitted = (from q in _context.TrainingAssesmentQuestions
                                                                      join a in _context.TrainingAssesmentAnswares on q.IntQuestionId equals a.IntQuestionId
                                                                      where q.IntScheduleId == schedule.IntScheduleId && q.IsPreAssesment == true && q.IsActive == true
                                                                      && a.IntRequisitionId == requisition.IntRequisitionId
                                                                      select a).Count() > 0 ? true : false,
                                                    isPostSubmitted = (from q in _context.TrainingAssesmentQuestions
                                                                       join a in _context.TrainingAssesmentAnswares on q.IntQuestionId equals a.IntQuestionId
                                                                       where q.IntScheduleId == schedule.IntScheduleId && q.IsPreAssesment == false && q.IsActive == true
                                                                       && a.IntRequisitionId == requisition.IntRequisitionId
                                                                       select a).Count() > 0 ? true : false,
                                                }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ==== Employee Assessment ====


        public async Task<MessageHelper> ExternalTraining(ExternalTrainingDTO obj)
        {
            try
            {
                MessageHelper msg = new MessageHelper();
                var training = _context.ExternalTrainings.Where(x => x.IntExternalTrainingId == obj.IntExternalTrainingId).FirstOrDefault();


                if (training != null)
                {
                    training.IntExternalTrainingId = obj.IntExternalTrainingId;
                    training.StrTrainingName = obj.StrTrainingName;
                    training.StrResourcePersonName = obj.StrResourcePersonName;
                    training.DteDate = obj.DteDate;
                    training.IntDepartmentId = obj.IntDepartmentId;
                    training.StrDepartmentName = obj.StrDepartmentName;
                    training.IntBatchSize = obj.IntBatchSize;
                    training.IntPresentParticipant = obj.IntPresentParticipant;
                    training.StrOrganizationCategory = obj.StrOrganizationCategory;
                    training.StrRemarks = obj.StrRemarks;
                    training.IntAccountId = obj.IntAccountId;
                    training.IntBusinessUnitId = obj.IntBusinessUnitId;
                    training.IsActive = obj.IsActive;
                    training.IntUpdatedBy = obj.IntUpdatedBy;
                    training.DteUpdatedDate = obj.DteUpdatedDate;

                    _context.ExternalTrainings.Update(training);
                    await _context.SaveChangesAsync();
                    msg.Message = "Update Successfully";
                    return msg;
                }
                else
                {
                    ExternalTraining externalTraining = new ExternalTraining
                    {
                        IntExternalTrainingId = obj.IntExternalTrainingId,
                        StrTrainingName = obj.StrTrainingName,
                        StrResourcePersonName = obj.StrResourcePersonName,
                        DteDate = obj.DteDate,
                        IntDepartmentId = obj.IntDepartmentId,
                        StrDepartmentName = obj.StrDepartmentName,
                        IntBatchSize = obj.IntBatchSize,
                        IntPresentParticipant = obj.IntPresentParticipant,
                        StrOrganizationCategory = obj.StrOrganizationCategory,
                        StrRemarks = obj.StrRemarks,
                        IntAccountId = obj.IntAccountId,
                        IntBusinessUnitId = obj.IntBusinessUnitId,
                        IsActive = true,
                        IntActionBy = obj.IntActionBy,
                        DteActionDate = obj.DteActionDate,
                        IntUpdatedBy = obj.IntUpdatedBy,
                        DteUpdatedDate = obj.DteUpdatedDate,
                    };
                    await _context.ExternalTrainings.AddAsync(externalTraining);
                    await _context.SaveChangesAsync();
                    msg.Message = "Created Successfully";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> UpdateExternalTraining(long ExternalTrainingId)
        {
            try
            {
                MessageHelper msg = new MessageHelper();
                var training = _context.ExternalTrainings.Where(x => x.IntExternalTrainingId == ExternalTrainingId).FirstOrDefault();
                if (training != null)
                {
                    training.IsActive = false;

                    _context.ExternalTrainings.Update(training);
                    await _context.SaveChangesAsync();
                }
                msg.StatusCode = 200;
                msg.Message = "Delete Successfully";
                return msg;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<List<ExternalTrainingDTO>> GetExternalTrainingLanding(long? intExternalTrainingId, long intAccountId, long intBusinessUnitId)
        {
            try
            {
                List<ExternalTrainingDTO> data = await (from training in _context.ExternalTrainings
                                                        where training.IsActive == true
                                                        && (training.IntExternalTrainingId == intExternalTrainingId || intExternalTrainingId == 0 || intExternalTrainingId == null)
                                                        && training.IntAccountId == intAccountId && training.IntBusinessUnitId == intBusinessUnitId
                                                        select new ExternalTrainingDTO
                                                        {
                                                            IntExternalTrainingId = training.IntExternalTrainingId,
                                                            StrTrainingName = training.StrTrainingName,
                                                            StrResourcePersonName = training.StrResourcePersonName,
                                                            DteDate = training.DteDate,
                                                            IntDepartmentId = training.IntDepartmentId,
                                                            StrDepartmentName = training.StrDepartmentName,
                                                            IntBatchSize = training.IntBatchSize,
                                                            IntPresentParticipant = training.IntPresentParticipant,
                                                            StrOrganizationCategory = training.StrOrganizationCategory,
                                                            StrRemarks = training.StrRemarks,
                                                            IntAccountId = training.IntAccountId,
                                                            IntBusinessUnitId = training.IntBusinessUnitId,
                                                            IsActive = training.IsActive,
                                                            IntActionBy = training.IntActionBy,
                                                            DteActionDate = training.DteActionDate,
                                                            IntUpdatedBy = training.IntUpdatedBy,
                                                            DteUpdatedDate = training.DteUpdatedDate,
                                                        }).AsNoTracking().ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}