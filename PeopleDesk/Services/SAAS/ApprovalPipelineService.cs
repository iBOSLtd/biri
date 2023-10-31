using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Runtime.Intrinsics.Arm;

namespace PeopleDesk.Services.SAAS
{
    public class ApprovalPipelineService : IApprovalPipelineService
    {
        private readonly PeopleDeskContext _context;
        //private readonly IEmployeeService _employeeService;

        public ApprovalPipelineService(PeopleDeskContext context)
        {
            _context = context;
            //_employeeService = employeeService;
        }

        public async Task<EmpIsSupNLMORUGMemberViewModel> EmployeeIsSupervisorNLineManagerORUserGroupMember(long accountId, long employeeId)
        {
            EmpIsSupNLMORUGMemberViewModel model = new EmpIsSupNLMORUGMemberViewModel { IsSupervisor = false, IsLineManager = false, IsUserGroup = false, UserGroupRows = new List<UserGroupRow>() };

            model.IsSupervisor = _context.EmpEmployeeBasicInfos.AsNoTracking().Where(x => (x.IntSupervisorId == employeeId || x.IntDottedSupervisorId == employeeId) && x.IntAccountId == accountId).Count() > 0 ? true : false;
            model.IsLineManager = _context.EmpEmployeeBasicInfos.AsNoTracking().Where(x => x.IntLineManagerId == employeeId && x.IntAccountId == accountId).Count() > 0 ? true : false;


            List<UserGroupRow> UserGroupRows = await (from ug in _context.UserGroupHeaders
                                                      where ug.IntAccountId == accountId && ug.IsActive == true
                                                      join ugr in _context.UserGroupRows on ug.IntUserGroupHeaderId equals ugr.IntUserGroupHeaderId
                                                      where ugr.IsActive == true && ugr.IntEmployeeId == employeeId
                                                      select ugr).AsNoTracking().Distinct().ToListAsync();


            model.UserGroupRows = UserGroupRows;
            model.IsUserGroup = UserGroupRows.Count() > 0 ? true : false;

            return model;
        }

        public async Task<EmpIsSupNLMORUGMemberViewModel> PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(long AccountId, string ApplicationType, long EmployeeId)
        {
            EmpIsSupNLMORUGMemberViewModel model = new EmpIsSupNLMORUGMemberViewModel { IsSupervisor = false, IsLineManager = false, IsUserGroup = false, UserGroupRows = new List<UserGroupRow>() };

            List<GlobalPipelineRow> leaveOrMovement = (from lm in _context.GlobalPipelineHeaders
                                                       join pr1 in _context.GlobalPipelineRows on lm.IntPipelineHeaderId equals pr1.IntPipelineHeaderId into plr
                                                       from pr in plr.DefaultIfEmpty()
                                                       where lm.IntAccountId == AccountId && lm.IsActive == true && pr.IsActive == true
                                                       && lm.StrApplicationType.ToLower() == ApplicationType.ToLower()
                                                       select pr).ToList();

            List<long> groupRows = (from gh in _context.UserGroupHeaders
                                    join gr1 in _context.UserGroupRows on gh.IntUserGroupHeaderId equals gr1.IntUserGroupHeaderId
                                    where gh.IsActive == true && gr1.IsActive == true
                                    && gh.IntAccountId == AccountId && gr1.IntEmployeeId == EmployeeId
                                    select gr1.IntUserGroupHeaderId).ToList();

            model.IsLineManager = leaveOrMovement.Where(x => x.IsLineManager == true).Count() > 0 ? true : false;
            model.IsSupervisor = leaveOrMovement.Where(x => x.IsSupervisor == true).Count() > 0 ? true : false;
            model.IsUserGroup = leaveOrMovement.Where(x => groupRows.Contains((long)x.IntUserGroupHeaderId)).Count() > 0 ? true : false;

            return model;
        }

        public async Task<PipelineStageInfoVM> GetCurrentNextStageAndPipelineHeaderId(long accountId, string pipelineType)
        {
            PipelineStageInfoVM res = new PipelineStageInfoVM();

            GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.FirstOrDefaultAsync(x => x.IsActive == true && x.IntAccountId == accountId && x.StrApplicationType.ToLower() == pipelineType.ToLower());
            if (header != null)
            {
                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                res.HeaderId = header.IntPipelineHeaderId;
                res.CurrentStageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                res.NextStageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
            }
            if (res.HeaderId <= 0 || res.CurrentStageId <= 0 || res.NextStageId <= 0)
            {
                throw new Exception("Pipeline was not set correctly");
            }
            return res;
        }

        #region Leave Application

        public async Task<ApplicationLandingVM> LeaveLandingEngine(LeaveApplicationLandingRequestVM model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();


                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "leave" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var leaveApplicationQuery = from obj in _context.LveLeaveApplications
                                            join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                            join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                            join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                            from dept in dept2.DefaultIfEmpty()
                                            join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                            from desig in desig2.DefaultIfEmpty()
                                            join lvt in _context.LveLeaveTypes on obj.IntLeaveTypeId equals lvt.IntLeaveTypeId into lvt2
                                            from lvt in lvt2.DefaultIfEmpty()
                                            where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                            (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                             || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true) &&
                                            ((model.FromDate.Date <= obj.DteFromDate && obj.DteToDate <= model.ToDate) || (model.FromDate.Year == obj.DteFromDate.Year && model.FromDate.Month == obj.DteFromDate.Month) || (model.ToDate.Year == obj.DteToDate.Year && model.ToDate.Month == obj.DteToDate.Month))
                                            && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                            select new
                                            {
                                                EmployeeName = emp.StrEmployeeName,
                                                EmployeeCode = emp.StrEmployeeCode,
                                                EmploymentType = emp.StrEmploymentType,
                                                BusinessUnitId = emp.IntBusinessUnitId,
                                                Department = dept != null ? dept.StrDepartment : "",
                                                Designation = desig != null ? desig.StrDesignation : "",
                                                LeaveType = lvt != null ? lvt.StrLeaveType : "",
                                                Status = model.ApplicationStatus,
                                                CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                SupervisorId = emp.IntSupervisorId,
                                                DottedSupervisorId = emp.IntDottedSupervisorId,
                                                LineManagerId = emp.IntLineManagerId,
                                                LeaveApplication = obj,
                                                CurrentStageObj = currentStage
                                            };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var leaveApplicationList1 = await leaveApplicationQuery.Select(obj => new LeaveApplicationLandingResponseVM
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        BusinessUnitId = obj.BusinessUnitId,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        LeaveType = obj.LeaveType,
                        DateRange = obj.LeaveApplication.DteFromDate.ToString("dd-MMM-yyyy") + " to " + obj.LeaveApplication.DteToDate.ToString("dd-MMM-yyyy"),
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        LeaveApplication = obj.LeaveApplication
                    }).ToListAsync();


                    retObj.TotalCount = leaveApplicationList1.Count();
                    retObj.Data = leaveApplicationList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var leaveApplicationList = await leaveApplicationQuery.ToListAsync();


                var listData = from obj in leaveApplicationList
                               join currentStage in globalPipelineRowList on obj.LeaveApplication.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.LeaveApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.LeaveApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.LeaveApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.LeaveApplication.IsPipelineClosed == true && obj.LeaveApplication.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.LeaveApplication.IntCurrentStage == obj.LeaveApplication.IntNextStage))
                               : false)
                               select new LeaveApplicationLandingResponseVM
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   LeaveType = obj.LeaveType,
                                   DateRange = obj.LeaveApplication.DteFromDate.ToString("dd-MMM-yyyy") + " to " + obj.LeaveApplication.DteToDate.ToString("dd-MMM-yyyy"),
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   LeaveApplication = obj.LeaveApplication
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LeaveApprovalResponseVM> LeaveApprovalEngine(LveLeaveApplication application, bool isReject, long approverId, long accountId)
        {
            try
            {
                LeaveApprovalResponseVM response = new LeaveApprovalResponseVM();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await LeaveApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await LeaveApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await LeaveApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await LeaveApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Leave Application

        #region Movement Application

        public async Task<ApplicationLandingVM> MovementLandingEngine(MovementApplicationLandingRequestVM model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "movement" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var MovementApplicationQuery = from obj in _context.LveMovementApplications
                                               join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                               join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                               join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                               from dept in dept2.DefaultIfEmpty()
                                               join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                               from desig in desig2.DefaultIfEmpty()
                                               join lvt in _context.LveMovementTypes on obj.IntMovementTypeId equals lvt.IntMovementTypeId into lvt2
                                               from lvt in lvt2.DefaultIfEmpty()
                                               where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                               (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                                || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true)
                                               && ((model.FromDate.Date <= obj.DteFromDate && obj.DteToDate <= model.ToDate) || (model.FromDate.Year == obj.DteFromDate.Year && model.FromDate.Month == obj.DteFromDate.Month) || (model.ToDate.Year == obj.DteToDate.Year && model.ToDate.Month == obj.DteToDate.Month))
                                               && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                               : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                               : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                               : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                               select new
                                               {
                                                   EmployeeName = emp.StrEmployeeName,
                                                   EmployeeCode = emp.StrEmployeeCode,
                                                   EmploymentType = emp.StrEmploymentType,
                                                   BusinessUnitId = emp.IntBusinessUnitId,
                                                   Department = dept != null ? dept.StrDepartment : "",
                                                   Designation = desig != null ? desig.StrDesignation : "",
                                                   MovementType = lvt != null ? lvt.StrMovementType : "",
                                                   Status = model.ApplicationStatus,
                                                   CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                   SupervisorId = emp.IntSupervisorId,
                                                   DottedSupervisorId = emp.IntDottedSupervisorId,
                                                   LineManagerId = emp.IntLineManagerId,
                                                   MovementApplication = obj,
                                                   CurrentStageObj = currentStage
                                               };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var leaveApplicationList1 = await MovementApplicationQuery.Select(obj => new MovementApplicationLandingResponseVM
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        BusinessUnitId = obj.BusinessUnitId,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        MovementType = obj.MovementType,
                        DateRange = obj.MovementApplication.DteFromDate.ToString("dd-MMM-yyyy") + " to " + obj.MovementApplication.DteToDate.ToString("dd-MMM-yyyy"),
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        MovementApplication = obj.MovementApplication
                    }).ToListAsync();

                    retObj.TotalCount = leaveApplicationList1.Count();
                    retObj.Data = leaveApplicationList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var MovementApplicationList = await MovementApplicationQuery.ToListAsync();


                var listData = from obj in MovementApplicationList
                               join currentStage in globalPipelineRowList on obj.MovementApplication.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.MovementApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.MovementApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.MovementApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.MovementApplication.IsPipelineClosed == true && obj.MovementApplication.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.MovementApplication.IntCurrentStage == obj.MovementApplication.IntNextStage))
                               : false)
                               select new MovementApplicationLandingResponseVM
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   MovementType = obj.MovementType,
                                   DateRange = obj.MovementApplication.DteFromDate.ToString("dd-MMM-yyyy") + " to " + obj.MovementApplication.DteToDate.ToString("dd-MMM-yyyy"),
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   MovementApplication = obj.MovementApplication
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MovementApprovalResponse> MovementApprovalEngine(LveMovementApplication application, bool isReject, long approverId, long accountId)
        {
            try
            {
                MovementApprovalResponse response = new MovementApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await MovementApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await MovementApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await MovementApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await MovementApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Movement Application

        #region Remote Attendance Location & Device Registration Application

        public async Task<ApplicationLandingVM> RemoteAttendanceLocationNDeviceLandingEngine(RemoteAttendanceLocationNDeviceLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var list = (from obj in _context.TimeRemoteAttendanceRegistrations
                            where obj.IsActive == true
                            //join msl in _context.MasterLocationRegisters on obj.IntMasterLocationId equals msl.IntMasterLocationId into msl2
                            //from mslocation in msl2.DefaultIfEmpty()
                            join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                            join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                            join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                            from dept in dept2.DefaultIfEmpty()
                            join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                            from desig in desig2.DefaultIfEmpty()
                            where emp.IntAccountId == tokenData.accountId && obj.IsActive == true
                            && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                              || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true)
                                && (model.FromDate.Date <= obj.DteInsertDate && obj.DteInsertDate <= model.ToDate)
                                && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                            select new
                            {
                                EmployeeName = emp.StrEmployeeName,
                                EmployeeCode = emp.StrEmployeeCode,
                                EmploymentType = emp.StrEmploymentType,
                                Department = dept != null ? dept.StrDepartment : "",
                                Designation = desig != null ? desig.StrDesignation : "",
                                Status = model.ApplicationStatus,
                                CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                Application = obj,
                                SupervisorId = emp.IntSupervisorId,
                                DottedSupervisorId = emp.IntDottedSupervisorId,
                                LineManagerId = emp.IntLineManagerId,
                            }).AsNoTrackingWithIdentityResolution();
                //RemoteAttendanceLocationNDeviceLandingReturnViewModel vm = new();
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    retObj.TotalCount = list.Count();
                    retObj.Data = list.Select(obj => new RemoteAttendanceLocationNDeviceLandingReturnViewModel
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();

                    return retObj;
                }
                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "locationNDevice" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();


                var QueryList = await list.ToListAsync();

                var listData = from obj in QueryList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new RemoteAttendanceLocationNDeviceLandingReturnViewModel
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };
                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();

                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RemoteAttendanceLocationNDeviceApprovalResponse> RemoteAttendanceLocationNDeviceApprovalEngine(TimeRemoteAttendanceRegistration application, bool isReject, long approverId, long accountId)
        {
            try
            {
                RemoteAttendanceLocationNDeviceApprovalResponse response = new RemoteAttendanceLocationNDeviceApprovalResponse();

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await RemoteAttendanceLocationNDeviceApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await RemoteAttendanceLocationNDeviceApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await RemoteAttendanceLocationNDeviceApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await RemoteAttendanceLocationNDeviceApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Remote Attendance Location & Device Registration Application

        #region Remote Attendance Application

        public async Task<ApplicationLandingVM> RemoteAttendanceLandingEngine(RemoteAttendanceLandingViewModel model, BaseVM tokenData, bool IsMarket)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();
                string applicationType = IsMarket == true ? "marketAttendance" : "remoteAttendance";

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "remoteAttendance" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var remoteAttenQuery = from obj in _context.TimeEmployeeAttendances
                                       join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                       join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                       join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                       from dept in dept2.DefaultIfEmpty()
                                       join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                       from desig in desig2.DefaultIfEmpty()
                                       where emp.IntAccountId == tokenData.accountId && obj.IsMarket == IsMarket &&
                                       (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                        || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true) &&
                                       (model.FromDate.Date <= obj.DteAttendanceDate.Value.Date && obj.DteAttendanceDate.Value.Date <= model.ToDate.Date)
                                       && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                       : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                       : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                       : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                       select new
                                       {
                                           EmployeeName = emp.StrEmployeeName,
                                           EmployeeCode = emp.StrEmployeeCode,
                                           EmploymentType = emp.StrEmploymentType,
                                           BusinessUnitId = emp.IntBusinessUnitId,
                                           Department = dept != null ? dept.StrDepartment : "",
                                           Designation = desig != null ? desig.StrDesignation : "",
                                           Status = model.ApplicationStatus,
                                           ApplicationDate = obj.DteAttendanceDate,
                                           CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                           SupervisorId = emp.IntSupervisorId,
                                           DottedSupervisorId = emp.IntDottedSupervisorId,
                                           LineManagerId = emp.IntLineManagerId,
                                           RemoteAttendance = obj,
                                           CurrentStageObj = currentStage
                                       };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var remoteAttenList1 = await remoteAttenQuery.Select(obj => new RemoteAttendanceLandingReturnViewModel
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        ApplicationDate = obj.ApplicationDate.Value.Date,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.RemoteAttendance
                    }).ToListAsync();


                    retObj.TotalCount = remoteAttenList1.Count();
                    retObj.Data = remoteAttenList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var remoteAttenList = await remoteAttenQuery.ToListAsync();


                var listData = from obj in remoteAttenList
                               join currentStage in globalPipelineRowList on obj.RemoteAttendance.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.RemoteAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.RemoteAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.RemoteAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.RemoteAttendance.IsPipelineClosed == true && obj.RemoteAttendance.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.RemoteAttendance.IntCurrentStage == obj.RemoteAttendance.IntNextStage))
                               : false)
                               select new RemoteAttendanceLandingReturnViewModel
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   ApplicationDate = obj.ApplicationDate.Value.Date,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.RemoteAttendance
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<RemoteAttendanceApprovalResponse> RemoteAttendanceLandingEngine(RemoteAttendanceLandingViewModel model, bool IsMarket)
        //{
        //    try
        //    {
        //        RemoteAttendanceApprovalResponse response = new RemoteAttendanceApprovalResponse
        //        {
        //            ResponseStatus = "",
        //            ApplicationStatus = "",
        //            CurrentSatageId = 0,
        //            NextSatageId = 0,
        //            IsComplete = true,
        //            ListData = new List<RemoteAttendanceLandingReturnViewModel>()
        //        };

        //        EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
        //        model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
        //        model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
        //        model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

        //        if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
        //            && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
        //        {
        //            response.ResponseStatus = "Invalid EmployeeId";
        //        }

        //        string applicationType = IsMarket == true ? "marketAttendance" : "remoteAttendance";

        //        GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == applicationType.ToLower());

        //        if (header == null)
        //        {
        //            response.ResponseStatus = "Invalid approval pipeline";
        //        }
        //        else
        //        {
        //            List<RemoteAttendanceLandingReturnViewModel> applicationList = new List<RemoteAttendanceLandingReturnViewModel>();

        //            if ((bool)model.IsAdmin)
        //            {
        //                model.IsSupOrLineManager = -1;
        //                applicationList = await RemoteAttendanceLandingEngine(model, header, null, IsMarket);
        //                if (applicationList.Count() > 0)
        //                {
        //                    response.ListData.AddRange(applicationList);
        //                    applicationList = new List<RemoteAttendanceLandingReturnViewModel>();
        //                }
        //            }
        //            else
        //            {
        //                if (model.IsSupervisor == true)
        //                {
        //                    model.IsSupOrLineManager = 1;
        //                    applicationList = await RemoteAttendanceLandingEngine(model, header, null, IsMarket);
        //                    if (applicationList.Count() > 0)
        //                    {
        //                        response.ListData.AddRange(applicationList);
        //                        applicationList = new List<RemoteAttendanceLandingReturnViewModel>();
        //                    }
        //                }
        //                if (model.IsLineManager == true)
        //                {
        //                    model.IsSupOrLineManager = 2;
        //                    applicationList = await RemoteAttendanceLandingEngine(model, header, null, IsMarket);
        //                    if (applicationList.Count() > 0)
        //                    {
        //                        response.ListData.AddRange(applicationList);
        //                        applicationList = new List<RemoteAttendanceLandingReturnViewModel>();
        //                    }
        //                }
        //                if (model.IsUserGroup == true)
        //                {
        //                    model.IsSupOrLineManager = 3;
        //                    foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
        //                    {
        //                        applicationList = await RemoteAttendanceLandingEngine(model, header, userGroup, IsMarket);
        //                        if (applicationList.Count() > 0)
        //                        {
        //                            response.ListData.AddRange(applicationList);
        //                            applicationList = new List<RemoteAttendanceLandingReturnViewModel>();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        response.ListData = response.ListData.OrderByDescending(x => x.Application.DteAttendanceDate).ToList();
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<RemoteAttendanceApprovalResponse> RemoteAttendanceApprovalEngine(TimeEmployeeAttendance application, bool isReject, long approverId, long accountId, bool IsMarket)
        {
            try
            {
                RemoteAttendanceApprovalResponse response = new RemoteAttendanceApprovalResponse();

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);
                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await RemoteAttendanceApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await RemoteAttendanceApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await RemoteAttendanceApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await RemoteAttendanceApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Remote Attendance Application

        #region Salary Addition and Deduction Application

        public async Task<SalaryAdditionNDeductionApprovalResponse> SalaryAdditionNDeductionApprovalEngine(PyrEmpSalaryAdditionNdeduction application, bool isReject, long approverId, long accountId)
        {
            try
            {
                SalaryAdditionNDeductionApprovalResponse response = new SalaryAdditionNDeductionApprovalResponse();

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await SalaryAdditionNDeductionApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await SalaryAdditionNDeductionApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await SalaryAdditionNDeductionApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await SalaryAdditionNDeductionApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> SalaryAdditionNDeductionLandingEngine(SalaryAdditionNDeductionLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();


                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "salaryAdditionNDeduction" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var salaryAdditionNDeductionList = await (from obj in _context.PyrEmpSalaryAdditionNdeductions
                                                          where obj.IsActive == true
                                                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                                                          join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                                          from dept in dept2.DefaultIfEmpty()
                                                          join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig2
                                                          from des in desig2.DefaultIfEmpty()
                                                          where emp.IntAccountId == tokenData.accountId && emp.IsActive == true
                                                          && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText)
                                                                  || des.StrDesignation.ToLower().Contains(model.SearchText)
                                                                  || dept.StrDepartment.ToLower().Contains(model.SearchText)
                                                                  || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true)
                                                          && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                                  : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                                  : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                                  : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                                          select new
                                                          {
                                                              EmployeeName = emp.StrEmployeeName,
                                                              EmployeeCode = emp.StrEmployeeCode,
                                                              EmploymentType = emp.StrEmploymentType,
                                                              Department = dept != null ? dept.StrDepartment : "",
                                                              Designation = des != null ? des.StrDesignation : "",
                                                              Status = model.ApplicationStatus,
                                                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                              Application = obj,
                                                              SupervisorId = emp.IntSupervisorId,
                                                              DottedSupervisorId = emp.IntDottedSupervisorId,
                                                              LineManagerId = emp.IntLineManagerId,
                                                              date = new DateTime(obj.IntYear > 0 ? (int)obj.IntYear : DateTime.Now.Year, obj.IntMonth > 0 ? (int)obj.IntMonth : DateTime.Now.Month, 5)
                                                          }).AsNoTracking().ToListAsync();

                var salaryAdditionNDeduction = salaryAdditionNDeductionList.Where(n => n.date.Date >= model.FromDate.Date && n.date.Date <= model.ToDate.Date).ToList();

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    retObj.TotalCount = salaryAdditionNDeduction.Count();
                    retObj.Data = salaryAdditionNDeduction.Select(obj => new SalaryAdditionNDeductionLandingReturnViewModel
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application,
                        date = obj.date
                    }).Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                //var salaryAdditionNDeductionList = await salaryAdditionNDeduction.ToListAsync();

                var listData = from obj in salaryAdditionNDeduction
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new SalaryAdditionNDeductionLandingReturnViewModel
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application,
                               };
                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Salary Addition and Deduction Application

        #region IOU

        public async Task<IOUApprovalResponse> IOUApplicationApprovalEngine(PyrIouapplication application, bool isReject, long approverId, long accountId)
        {
            try
            {
                IOUApprovalResponse response = new IOUApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await IOUApplicationApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await IOUApplicationApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await IOUApplicationApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await IOUApplicationApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> IOUApplicationLandingEngine(IOULandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var iouQuery = from obj in _context.PyrIouapplications
                               where obj.IsActive == true
                               join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                               join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                               join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                               from dept in dept2.DefaultIfEmpty()
                               join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                               from desig in desig2.DefaultIfEmpty()
                               where emp.IntAccountId == tokenData.accountId && obj.IsActive == true
                               //&& (model.FromDate.Date <= obj.DteApplicationDate.Date && obj.DteApplicationDate.Date <= model.ToDate.Date)
                               && ((model.FromDate.Date <= obj.DteFromDate.Value.Date && obj.DteToDate.Value.Date <= model.ToDate) || (model.FromDate.Year == obj.DteFromDate.Value.Year && model.FromDate.Month == obj.DteFromDate.Value.Month) || (model.ToDate.Year == obj.DteToDate.Value.Year && model.ToDate.Month == obj.DteToDate.Value.Month))
                                    && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)

                                    && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                                        || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText) : true)
                               select new
                               {
                                   EmployeeName = emp.StrEmployeeName,
                                   EmployeeCode = emp.StrEmployeeCode,
                                   EmploymentType = emp.StrEmploymentType,
                                   Department = dept != null ? dept.StrDepartment : "",
                                   Designation = desig != null ? desig.StrDesignation : "",
                                   ApplicationDate = obj.DteApplicationDate.Date.ToString("dd MMM, yyyy"),
                                   DateRange = obj.DteFromDate.Value.Date.ToString("dd MMM, yyyy") + "-" + obj.DteToDate.Value.Date.ToString("dd MMM, yyyy"),
                                   IouAmount = obj.NumIouamount,
                                   AdjustedAmount = obj.NumAdjustedAmount,
                                   PayableAmount = obj.NumPayableAmount,
                                   ReceivableAmount = obj.NumReceivableAmount,
                                   Status = model.ApplicationStatus,
                                   CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                   SupervisorId = emp.IntSupervisorId,
                                   LineManagerId = emp.IntLineManagerId,
                                   DottedSupervisorId = emp.IntDottedSupervisorId,
                                   Application = obj,
                               };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var iouListForAdmin = await iouQuery.Select(obj => new IOULandingReturnViewModel
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        ApplicationDate = obj.ApplicationDate,
                        DateRange = obj.DateRange,
                        AdjustedAmount = obj.AdjustedAmount,
                        PayableAmount = obj.PayableAmount,
                        ReceivableAmount = obj.ReceivableAmount,
                        CurrentStage = obj.CurrentStage,
                        Status = obj.Status,
                        Application = obj.Application,
                    }).ToListAsync();

                    retObj.TotalCount = iouListForAdmin.Count();
                    retObj.Data = iouListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "iou" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var iouList = await iouQuery.ToListAsync();

                var listData = from obj in iouList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new IOULandingReturnViewModel
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   ApplicationDate = obj.ApplicationDate,
                                   DateRange = obj.DateRange,
                                   AdjustedAmount = obj.AdjustedAmount,
                                   PayableAmount = obj.PayableAmount,
                                   ReceivableAmount = obj.ReceivableAmount,
                                   CurrentStage = obj.CurrentStage,
                                   Status = obj.Status,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion IOU

        #region IOU Adjustment

        public async Task<IOUAdjustmentApprovalResponse> IOUAdjustmentApprovalEngine(PyrIouadjustmentHistory application, bool isReject, long approverId, long accountId)
        {
            try
            {
                IOUAdjustmentApprovalResponse response = new IOUAdjustmentApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await IOUAdjustmentApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await IOUAdjustmentApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await IOUAdjustmentApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await IOUAdjustmentApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> IOUAdjustmentLandingEngine(IOUAdjustmentLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var iouAdjustmentQuery = from obj in _context.PyrIouadjustmentHistories
                                         where obj.IsActive == true
                                         join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                         join iou in _context.PyrIouapplications on obj.IntIouid equals iou.IntIouid
                                         join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                                         join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                         from dept in dept2.DefaultIfEmpty()
                                         join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                         from desig in desig2.DefaultIfEmpty()
                                         let imgUrlId = _context.EmpEmployeePhotoIdentities.Where(x => x.IntEmployeeBasicInfoId == emp.IntEmployeeBasicInfoId).FirstOrDefault().IntProfilePicFileUrlId

                                         where emp.IntAccountId == tokenData.accountId && obj.IsActive == true
                                         && ((model.FromDate.Date <= iou.DteFromDate.Value.Date && iou.DteToDate.Value.Date <= model.ToDate) || (model.FromDate.Year == iou.DteFromDate.Value.Year && model.FromDate.Month == iou.DteFromDate.Value.Month) || (model.ToDate.Year == iou.DteToDate.Value.Year && model.ToDate.Month == iou.DteToDate.Value.Month))
                                         && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                  : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                  : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                  : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)

                                         && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                                              || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText) : true)
                                         select new
                                         {
                                             IntEmployeeId = emp.IntEmployeeBasicInfoId,
                                             StrEmployeeName = emp.StrEmployeeName,
                                             EmployeeCode = emp.StrEmployeeCode,
                                             NumPayableAmount = obj.NumPayableAmount,
                                             NumReceivableAmount = obj.NumReceivableAmount,
                                             NumAdjustmentAmount = iou.NumAdjustedAmount,
                                             AccountsAdjustmentAmount = iou.NumReceivableAmount,
                                             IsAcknowledgement = obj.IsAcknowledgement,
                                             Status = model.ApplicationStatus,
                                             EmploymentType = emp.StrEmploymentType,
                                             Department = dept != null ? dept.StrDepartment : "",
                                             Designation = desig != null ? desig.StrDesignation : "",
                                             DteFromDate = iou.DteFromDate,
                                             DteToDate = iou.DteToDate,
                                             DateRange = iou.DteFromDate.Value.Date.ToString("dd MMM, yyyy") + "-" + iou.DteToDate.Value.Date.ToString("dd MMM, yyyy"),
                                             IOUAmount = iou.NumIouamount,
                                             Description = iou.StrDiscription,
                                             ImgUrlId = imgUrlId,
                                             SupervisorId = emp.IntSupervisorId,
                                             LineManagerId = emp.IntLineManagerId,
                                             DottedSupervisorId = emp.IntDottedSupervisorId,
                                             CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                             Application = obj
                                         };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here
                    var iouAdjustmentListForAdmin = await iouAdjustmentQuery.Select(obj => new IOUAdjustmentLandingReturnViewModel
                    {
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        NumPayableAmount = obj.NumPayableAmount,
                        NumReceivableAmount = obj.NumReceivableAmount,
                        NumAdjustmentAmount = obj.NumAdjustmentAmount,
                        AccountsAdjustmentAmount = obj.NumReceivableAmount,
                        IsAcknowledgement = obj.IsAcknowledgement,
                        Status = obj.Status,
                        EmploymentType = obj.EmploymentType,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        DateRange = obj.DateRange,
                        IOUAmount = obj.IOUAmount,
                        Description = obj.Description,
                        ImgUrlId = obj.ImgUrlId,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).ToListAsync();

                    retObj.TotalCount = iouAdjustmentListForAdmin.Count();
                    retObj.Data = iouAdjustmentListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "iOUAdj" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var iouList = await iouAdjustmentQuery.ToListAsync();

                var listData = from obj in iouList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new IOUAdjustmentLandingReturnViewModel
                               {
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   NumPayableAmount = obj.NumPayableAmount,
                                   NumReceivableAmount = obj.NumReceivableAmount,
                                   NumAdjustmentAmount = obj.NumAdjustmentAmount,
                                   AccountsAdjustmentAmount = obj.NumReceivableAmount,
                                   IsAcknowledgement = obj.IsAcknowledgement,
                                   Status = obj.Status,
                                   EmploymentType = obj.EmploymentType,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   DateRange = obj.DateRange,
                                   IOUAmount = obj.IOUAmount,
                                   Description = obj.Description,
                                   ImgUrlId = obj.ImgUrlId,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion IOU Adjustment

        #region Salary Generate Request

        public async Task<SalaryGenerateRequestApprovalResponse> SalaryGenerateRequestApprovalEngine(PyrPayrollSalaryGenerateRequest application, bool isReject, long approverId, long accountId)
        {
            try
            {
                SalaryGenerateRequestApprovalResponse response = new SalaryGenerateRequestApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);
                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await SalaryGenerateRequestApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await PayrollSalaryGenerateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await SalaryGenerateRequestApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await PayrollSalaryGenerateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApplicationLandingVM> SalaryGenerateRequestLandingEngine(SalaryGenerateRequestLandingRequestVM model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "salaryGenerateRequest" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                //var roleExtension = await (from reh in _context.RoleExtensionHeaders
                //                           join rer in _context.RoleExtensionRows on reh.IntRoleExtensionHeaderId equals rer.IntRoleExtensionHeaderId
                //                           where reh.IsActive == true && rer.IsActive == true && rer.IntEmployeeId == tokenData.employeeId
                //                           select rer).AsNoTracking().ToListAsync();

                var SalaryGenerateApplicationQuery = from obj in _context.PyrPayrollSalaryGenerateRequests
                                                     join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                                     join soleDp in _context.TerritorySetups on obj.IntSoleDepoId equals soleDp.IntTerritoryId into soledp2
                                                     from soleDp in soledp2.DefaultIfEmpty()
                                                     join area in _context.TerritorySetups on obj.IntAreaId equals area.IntTerritoryId into area2
                                                     from area in area2.DefaultIfEmpty()
                                                     where obj.IntAccountId == tokenData.accountId && obj.IsActive == true && ((model.FromDate.Date <= obj.DteSalaryGenerateFrom.Date && obj.DteSalaryGenerateTo.Date <= model.ToDate) || (model.FromDate.Year == obj.DteSalaryGenerateFrom.Date.Year && model.FromDate.Month == obj.DteSalaryGenerateFrom.Month) || (model.ToDate.Year == obj.DteSalaryGenerateTo.Year && model.ToDate.Month == obj.DteSalaryGenerateTo.Month))
                                                     && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                         : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                         : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                         : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                                     && (!string.IsNullOrEmpty(model.SearchText) ? obj.StrSalaryCode.ToLower().Contains(model.SearchText) || obj.StrSalaryType.ToLower().Contains(model.SearchText)
                                                        || obj.StrWorkplaceGroupName.ToLower().Contains(model.SearchText) || soleDp.StrTerritoryName.ToLower().Contains(model.SearchText) || area.StrTerritoryName.ToLower().Contains(model.SearchText) : true)

                                                     //&& (tokenData.isOfficeAdmin == true || ((tokenData.areaList.Contains(obj.IntAreaId) && obj.IsApprovedBySoleDepo == false)
                                                     //                                    || (tokenData.soleDepoList.Contains(obj.IntSoleDepoId) && obj.IsPipelineClosed == true && obj.IsApprovedBySoleDepo == false)))

                                                     select new
                                                     {
                                                         SalaryGenerateRequestId = obj.IntSalaryGenerateRequestId,
                                                         StrSalaryCode = obj.StrSalaryCode,
                                                         StrSalaryType = obj.StrSalaryType,
                                                         //IntAccountId = obj.IntAccountId,
                                                         BusinessUnitId = obj.IntBusinessUnitId,
                                                         BusinessUnit = obj.StrBusinessUnit,
                                                         WorkplaceGroupId = obj.IntWorkplaceGroupId,
                                                         WorkplaceGroupName = obj.StrWorkplaceGroupName,
                                                         SoleDepoId = obj.IntSoleDepoId,
                                                         SoleDepoName = soleDp.StrTerritoryName,
                                                         AreaId = obj.IntAreaId,
                                                         AreaName = area.StrTerritoryName,
                                                         TerritoryIdList = obj.StrTerritoryIdList,
                                                         TerritoryNameList = obj.StrTerritoryNameList,
                                                         NumNetPayableSalary = obj.NumNetPayableSalary +
                                                                               (obj.IntAreaId > 0 && (obj.StrTerritoryIdList == "" || obj.StrTerritoryIdList == "0" || obj.StrTerritoryIdList == null) ?
                                                                                _context.PyrPayrollSalaryGenerateRequests.Where(n => n.IntAreaId == obj.IntAreaId && (n.StrTerritoryIdList != null || n.StrTerritoryIdList != "") && n.IsPipelineClosed == true && n.IsReject == false
                                                                                    && n.IsActive == true).Sum(n => n.NumNetPayableSalary)
                                                                               : obj.IntSoleDepoId > 0 && (obj.IntAreaId == null || obj.IntAreaId <= 0) ?
                                                                                _context.PyrPayrollSalaryGenerateRequests.Where(x => x.IntSoleDepoId == obj.IntSoleDepoId && x.IntAreaId > 0 && x.IsPipelineClosed == true && x.IsReject == false && x.IsActive == true).Sum(x => x.NumNetPayableSalary) : 0),
                                                         Status = model.ApplicationStatus,
                                                         CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                         SalaryGenerateRequestApplication = obj,
                                                         CurrentStageObj = currentStage
                                                     };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var SalaryGenerateApplicationQueryList1 = await SalaryGenerateApplicationQuery.Select(obj => new SalaryGenerateRequestLandingReturnViewModel
                    {
                        SalaryGenerateRequestId = obj.SalaryGenerateRequestId,
                        SalaryCode = obj.StrSalaryCode,
                        SalaryType = obj.StrSalaryType,
                        BusinessUnitId = obj.BusinessUnitId,
                        BusinessUnit = obj.BusinessUnit,
                        WorkplaceGroupId = obj.WorkplaceGroupId,
                        WorkplaceGroupName = obj.WorkplaceGroupName,
                        SoleDepoId = obj.SoleDepoId,
                        SoleDepoName = obj.SoleDepoName,
                        AreaId = obj.AreaId,
                        AreaName = obj.AreaName,
                        TerritoryIdList = obj.TerritoryIdList,
                        TerritoryNameList = obj.TerritoryNameList,
                        NumNetPayableSalary = obj.NumNetPayableSalary,
                        DateRange = obj.SalaryGenerateRequestApplication.DteSalaryGenerateFrom.ToString("dd-MMM-yyyy") + " to " + obj.SalaryGenerateRequestApplication.DteSalaryGenerateFrom.ToString("dd-MMM-yyyy"),
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.SalaryGenerateRequestApplication
                    }).ToListAsync();

                    retObj.TotalCount = SalaryGenerateApplicationQueryList1.Count();
                    retObj.Data = SalaryGenerateApplicationQueryList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;

                }

                var SalaryGenerateApplicationQueryList = await SalaryGenerateApplicationQuery.ToListAsync();


                var listData = from obj in SalaryGenerateApplicationQueryList
                               join currentStage in globalPipelineRowList on obj.SalaryGenerateRequestApplication.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsUserGroup == true && obj.SalaryGenerateRequestApplication.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.SalaryGenerateRequestApplication.IsPipelineClosed == true && obj.SalaryGenerateRequestApplication.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.SalaryGenerateRequestApplication.IntCurrentStage == obj.SalaryGenerateRequestApplication.IntNextStage))
                               : false)
                               select new SalaryGenerateRequestLandingReturnViewModel
                               {
                                   SalaryGenerateRequestId = obj.SalaryGenerateRequestId,
                                   SalaryCode = obj.StrSalaryCode,
                                   SalaryType = obj.StrSalaryType,
                                   //IntAccountId = obj.IntAccountId,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   BusinessUnit = obj.BusinessUnit,
                                   WorkplaceGroupId = obj.WorkplaceGroupId,
                                   WorkplaceGroupName = obj.WorkplaceGroupName,
                                   SoleDepoId = obj.SoleDepoId,
                                   SoleDepoName = obj.SoleDepoName,
                                   AreaId = obj.AreaId,
                                   AreaName = obj.AreaName,
                                   TerritoryIdList = obj.TerritoryIdList,
                                   TerritoryNameList = obj.TerritoryNameList,
                                   NumNetPayableSalary = obj.NumNetPayableSalary,
                                   DateRange = obj.SalaryGenerateRequestApplication.DteSalaryGenerateFrom.ToString("dd-MMM-yyyy") + " to " + obj.SalaryGenerateRequestApplication.DteSalaryGenerateTo.ToString("dd-MMM-yyyy"),
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.SalaryGenerateRequestApplication
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Salary Generate Request

        #region Loan

        public async Task<LoanApprovalResponse> LoanApprovalEngine(EmpLoanApplication application, bool isReject, long approverId, long accountId)
        {
            try
            {
                LoanApprovalResponse response = new LoanApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await LoanApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await EmpLoanApplicationApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await LoanApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await EmpLoanApplicationApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> LoanLandingEngine(LoanApplicationLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "loan" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var loanQuery = from obj in _context.EmpLoanApplications
                                where obj.IsActive == true
                                join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                join emp1 in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into Employee
                                from emp in Employee.DefaultIfEmpty()
                                join type in _context.EmpLoanTypes on obj.IntLoanTypeId equals type.IntLoanTypeId into LType
                                from LoanType in LType.DefaultIfEmpty()
                                join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                from depart in dp.DefaultIfEmpty()
                                join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
                                from designat in desig.DefaultIfEmpty()
                                where emp.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteApplicationDate.Date && obj.DteApplicationDate.Date <= model.ToDate.Date)
                                    && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                    && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || designat.StrDesignation.ToLower().Contains(model.SearchText)
                                                        || depart.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || LoanType.StrLoanType.ToLower().Contains(model.SearchText) : true)
                                select new
                                {
                                    IntLoanApplicationId = obj.IntLoanApplicationId,
                                    IntEmployeeId = obj.IntEmployeeId,
                                    StrEmployeeName = emp != null ? emp.StrEmployeeName : "",
                                    IntDepartmentId = emp.IntDepartmentId,
                                    StrDepartment = depart != null ? depart.StrDepartment : "",
                                    IntDesignationId = emp.IntDesignationId,
                                    StrDesignation = designat != null ? designat.StrDesignation : "",
                                    dteApplicationDate = obj.DteApplicationDate.Date,
                                    IntLoanTypeId = obj.IntLoanTypeId,
                                    StrLoanType = LoanType != null ? LoanType.StrLoanType : "",
                                    IntLoanAmount = obj.IntLoanAmount,
                                    IntNumberOfInstallment = obj.IntNumberOfInstallment,
                                    IntNumberOfInstallmentAmount = obj.IntNumberOfInstallmentAmount,
                                    CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                    StrDescription = obj.StrDescription,
                                    Status = model.ApplicationStatus,
                                    SupervisorId = emp.IntSupervisorId,
                                    LineManagerId = emp.IntLineManagerId,
                                    DottedSupervisorId = emp.IntDottedSupervisorId,
                                    Application = obj,
                                };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var loanListForAdmin = await loanQuery.Select(obj => new LoanApplicationLandingReturnViewModel
                    {
                        IntLoanApplicationId = obj.IntLoanApplicationId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        IntDepartmentId = obj.IntDepartmentId,
                        StrDepartment = obj.StrDepartment,
                        IntDesignationId = obj.IntDesignationId,
                        StrDesignation = obj.StrDesignation,
                        LoanApplicationDate = obj.dteApplicationDate.Date.ToString("dd-MMM-yyyy"),
                        IntLoanTypeId = obj.IntLoanTypeId,
                        StrLoanType = obj.StrLoanType,
                        IntLoanAmount = obj.IntLoanAmount,
                        IntNumberOfInstallment = obj.IntNumberOfInstallment,
                        IntNumberOfInstallmentAmount = obj.IntNumberOfInstallmentAmount,
                        CurrentStage = obj.CurrentStage,
                        Description = obj.StrDescription,
                        Status = obj.Status,
                        //SupervisorId = obj.IntSupervisorId,
                        //LineManagerId = obj.IntLineManagerId,
                        //DottedSupervisorId = obj.IntDottedSupervisorId,
                        Application = obj.Application,
                    }).ToListAsync();

                    retObj.TotalCount = loanListForAdmin.Count();
                    retObj.Data = loanListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var loanList = await loanQuery.ToListAsync();

                var listData = from obj in loanList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new LoanApplicationLandingReturnViewModel
                               {
                                   IntLoanApplicationId = obj.IntLoanApplicationId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   IntDepartmentId = obj.IntDepartmentId,
                                   StrDepartment = obj.StrDepartment,
                                   IntDesignationId = obj.IntDesignationId,
                                   StrDesignation = obj.StrDesignation,
                                   LoanApplicationDate = obj.dteApplicationDate.Date.ToString("dd-MMM-yyyy"),
                                   IntLoanTypeId = obj.IntLoanTypeId,
                                   StrLoanType = obj.StrLoanType,
                                   IntLoanAmount = obj.IntLoanAmount,
                                   IntNumberOfInstallment = obj.IntNumberOfInstallment,
                                   IntNumberOfInstallmentAmount = obj.IntNumberOfInstallmentAmount,
                                   CurrentStage = obj.CurrentStage,
                                   Description = obj.StrDescription,
                                   Status = obj.Status,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Loan

        #region Over Time

        public async Task<OverTimeApprovalResponse> OverTimeApprovalEngine(TimeEmpOverTime application, bool isReject, long approverId, long accountId)
        {
            try
            {
                OverTimeApprovalResponse response = new OverTimeApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await OverTimeApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await OverTimeApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await OverTimeApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await OverTimeApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> OverTimeLandingEngine(OverTimeLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "overTime" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var overTimeQuery = from obj in _context.TimeEmpOverTimes
                                    where obj.IsActive == true
                                    join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                    join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                    join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                    from depart in dp.DefaultIfEmpty()
                                    join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
                                    from designat in desig.DefaultIfEmpty()
                                    where emp.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteOverTimeDate.Value.Date && obj.DteOverTimeDate.Value.Date <= model.ToDate.Date)
                                    && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                    && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || designat.StrDesignation.ToLower().Contains(model.SearchText)
                                                || depart.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText) : true)

                                    select new
                                    {
                                        IntOverTimeId = obj.IntOverTimeId,
                                        IntEmployeeId = obj.IntEmployeeId,
                                        StrEmployeeName = emp != null ? emp.StrEmployeeName : "",
                                        StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
                                        IntDepartmentId = emp.IntDepartmentId,
                                        StrDepartment = depart != null ? depart.StrDepartment : "",
                                        IntDesignationId = emp.IntDesignationId,
                                        StrDesignation = designat != null ? designat.StrDesignation : "",
                                        DteOverTimeDate = obj.DteOverTimeDate,
                                        TmeStartTime = obj.TmeStartTime,
                                        TmeEndTime = obj.TmeEndTime,
                                        NumOverTimeHour = obj.NumOverTimeHour,
                                        StrReason = obj.StrReason,
                                        Status = model.ApplicationStatus,
                                        CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                        SupervisorId = emp.IntSupervisorId,
                                        LineManagerId = emp.IntLineManagerId,
                                        DottedSupervisorId = emp.IntDottedSupervisorId,
                                        Application = obj
                                    };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var overTimeListForAdmin = await overTimeQuery.Select(obj => new OverTimeLandingReturnViewModel
                    {
                        IntOverTimeId = obj.IntOverTimeId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        StrEmploymentType = obj.StrEmploymentType,
                        IntDepartmentId = obj.IntDepartmentId,
                        StrDepartment = obj.StrDepartment,
                        IntDesignationId = obj.IntDesignationId,
                        StrDesignation = obj.StrDesignation,
                        OverTimeDate = obj.DteOverTimeDate.Value.Date.ToString("dd-MMM-yyyy"),
                        TmeStartTime = obj.TmeStartTime,
                        TmeEndTime = obj.TmeEndTime,
                        NumOverTimeHour = obj.NumOverTimeHour,
                        StrReason = obj.StrReason,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application,
                        Status = obj.Status,
                    }).ToListAsync();


                    retObj.TotalCount = overTimeListForAdmin.Count();
                    retObj.Data = overTimeListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var overTimeList = await overTimeQuery.ToListAsync();


                var listData = from obj in overTimeList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new OverTimeLandingReturnViewModel
                               {
                                   IntOverTimeId = obj.IntOverTimeId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   StrEmploymentType = obj.StrEmploymentType,
                                   IntDepartmentId = obj.IntDepartmentId,
                                   StrDepartment = obj.StrDepartment,
                                   IntDesignationId = obj.IntDesignationId,
                                   StrDesignation = obj.StrDesignation,
                                   OverTimeDate = obj.DteOverTimeDate.Value.Date.ToString("dd-MMM-yyyy"),
                                   TmeStartTime = obj.TmeStartTime,
                                   TmeEndTime = obj.TmeEndTime,
                                   NumOverTimeHour = obj.NumOverTimeHour,
                                   StrReason = obj.StrReason,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application,
                                   Status = obj.Status
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Over Time

        #region Manual Attendance Summary

        public async Task<ManualAttendanceSummaryApprovalResponse> ManualAttendanceApprovalEngine(EmpManualAttendanceSummary application, bool isReject, long approverId, long accountId)
        {
            try
            {
                ManualAttendanceSummaryApprovalResponse response = new ManualAttendanceSummaryApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await ManualAttendanceApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await ManualAttendanceApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await ManualAttendanceApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await ManualAttendanceApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> ManualAttendanceLandingEngine(ManualAttendanceSummaryLandingVM model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "manualAttendance" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var ManualAttendance = from obj in _context.EmpManualAttendanceSummaries
                                       join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                       join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                       join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                       from dept in dept2.DefaultIfEmpty()
                                       join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                       from desig in desig2.DefaultIfEmpty()
                                       where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                       (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                       || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true) &&
                                       (obj.DteAttendanceDate >= model.FromDate && obj.DteAttendanceDate <= model.ToDate)
                                       && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                       : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                       : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                       : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                       select new
                                       {
                                           EmployeeName = emp.StrEmployeeName,
                                           EmployeeCode = emp.StrEmployeeCode,
                                           EmploymentType = emp.StrEmploymentType,
                                           BusinessUnitId = emp.IntBusinessUnitId,
                                           Department = dept != null ? dept.StrDepartment : "",
                                           Designation = desig != null ? desig.StrDesignation : "",
                                           Status = model.ApplicationStatus,
                                           CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                           SupervisorId = emp.IntSupervisorId,
                                           DottedSupervisorId = emp.IntDottedSupervisorId,
                                           LineManagerId = emp.IntLineManagerId,
                                           ManualAttendance = obj,
                                           CurrentStageObj = currentStage,
                                           CurrentStatus = obj.StrCurrentStatus,
                                           RequestStatus = obj.StrRequestStatus,
                                           AttendanceDate = obj.DteAttendanceDate
                                       };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var leaveApplicationList1 = await ManualAttendance.Select(obj => new ManualAttendanceLandingResponseVM
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        BusinessUnitId = obj.BusinessUnitId,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        Date = obj.AttendanceDate,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        application = obj.ManualAttendance,
                        CurrentStatus = obj.CurrentStatus,
                        RequestStatus = obj.RequestStatus

                    }).ToListAsync();


                    retObj.TotalCount = leaveApplicationList1.Count();
                    retObj.Data = leaveApplicationList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var ManualAttendanceApplicationList = await ManualAttendance.ToListAsync();


                var listData = from obj in ManualAttendanceApplicationList
                               join currentStage in globalPipelineRowList on obj.ManualAttendance.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.ManualAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.ManualAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.ManualAttendance.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.ManualAttendance.IsPipelineClosed == true && obj.ManualAttendance.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.ManualAttendance.IntCurrentStage == obj.ManualAttendance.IntNextStage))
                               : false)
                               select new ManualAttendanceLandingResponseVM
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   Date = obj.AttendanceDate,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   application = obj.ManualAttendance,
                                   CurrentStatus = obj.CurrentStatus,
                                   RequestStatus = obj.RequestStatus
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Manual Attendance Summary

        #region Employee Separation

        public async Task<EmployeeSeparationApprovalResponse> EmployeeSeparationApprovalEngine(EmpEmployeeSeparation application, bool isReject, long approverId, long accountId)
        {
            try
            {
                EmployeeSeparationApprovalResponse response = new EmployeeSeparationApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await EmployeeSeparationApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await EmployeeSeparationApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await EmployeeSeparationApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await EmployeeSeparationApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> EmployeeSeparationLandingEngine(EmployeeSeparationLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "separation" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var separationQuery = from obj in _context.EmpEmployeeSeparations
                                      where obj.IsActive == true
                                      join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                      join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                      join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                      from depart in dp.DefaultIfEmpty()
                                      join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
                                      from designat in desig.DefaultIfEmpty()
                                      where emp.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteSeparationDate.Value.Date && obj.DteSeparationDate.Value.Date <= model.ToDate.Date)
                                            && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false

                                                    : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                    : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                    : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                            && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || designat.StrDesignation.ToLower().Contains(model.SearchText)
                                                        || depart.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText)
                                                        || obj.StrSeparationTypeName.ToLower().Contains(model.SearchText) : true)
                                      select new
                                      {
                                          IntSeparationId = obj.IntSeparationId,
                                          IntEmployeeId = obj.IntEmployeeId,
                                          StrEmployeeName = emp != null ? emp.StrEmployeeName : "",
                                          EmployeeCode = emp.StrEmployeeCode,
                                          StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
                                          IntDepartmentId = emp.IntDepartmentId,
                                          StrDepartment = depart != null ? depart.StrDepartment : "",
                                          IntDesignationId = emp.IntDesignationId,
                                          StrDesignation = designat != null ? designat.StrDesignation : "",
                                          IntSeparationTypeId = obj.IntSeparationTypeId,
                                          StrSeparationTypeName = obj.StrSeparationTypeName,
                                          DteSeparationDate = obj.DteSeparationDate,
                                          DteLastWorkingDate = obj.DteLastWorkingDate,
                                          StrDocumentId = obj.StrDocumentId,
                                          StrReason = obj.StrReason,
                                          CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                          Status = model.ApplicationStatus,
                                          SupervisorId = emp.IntSupervisorId,
                                          LineManagerId = emp.IntLineManagerId,
                                          DottedSupervisorId = emp.IntDottedSupervisorId,
                                          Application = obj
                                      };



                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var separationListForAdmin = await separationQuery.Select(obj => new EmployeeSeparationLandingReturnViewModel
                    {
                        IntSeparationId = obj.IntSeparationId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        StrEmploymentType = obj.StrEmploymentType,
                        IntDepartmentId = obj.IntDepartmentId,
                        StrDepartment = obj.StrDepartment,
                        IntDesignationId = obj.IntDesignationId,
                        StrDesignation = obj.StrDesignation,
                        IntSeparationTypeId = obj.IntSeparationTypeId,
                        StrSeparationTypeName = obj.StrSeparationTypeName,
                        DteSeparationDate = obj.DteSeparationDate.Value.Date.ToString("dd-MMM-yyyy"),
                        DteLastWorkingDate = obj.DteLastWorkingDate.Value.Date.ToString("dd-MMM-yyyy"),
                        StrReason = obj.StrReason,
                        StrDocumentId = obj.StrDocumentId,
                        CurrentStage = obj.CurrentStage,
                        Status = obj.Status,
                        Application = obj.Application,
                    }).ToListAsync();

                    retObj.TotalCount = separationListForAdmin.Count();
                    retObj.Data = separationListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var separationList = await separationQuery.ToListAsync();

                var listData = from obj in separationList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new EmployeeSeparationLandingReturnViewModel
                               {
                                   IntSeparationId = obj.IntSeparationId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   StrEmploymentType = obj.StrEmploymentType,
                                   IntDepartmentId = obj.IntDepartmentId,
                                   StrDepartment = obj.StrDepartment,
                                   IntDesignationId = obj.IntDesignationId,
                                   StrDesignation = obj.StrDesignation,
                                   IntSeparationTypeId = obj.IntSeparationTypeId,
                                   StrSeparationTypeName = obj.StrSeparationTypeName,
                                   DteSeparationDate = obj.DteSeparationDate.Value.Date.ToString("dd-MMM-yyyy"),
                                   DteLastWorkingDate = obj.DteLastWorkingDate.Value.Date.ToString("dd-MMM-yyyy"),
                                   StrReason = obj.StrReason,
                                   StrDocumentId = obj.StrDocumentId,
                                   CurrentStage = obj.CurrentStage,
                                   Status = obj.Status,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Employee Separation

        #region Transfer & Promotion

        public async Task<TransferNPromotionApprovalResponse> TransferNPromotionApprovalEngine(EmpTransferNpromotion application, bool isReject, long approverId, long accountId)
        {
            try
            {
                TransferNPromotionApprovalResponse response = new TransferNPromotionApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await TransferNPromotionApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await TransferNPromotionApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await TransferNPromotionApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await TransferNPromotionApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> TransferNPromotionLandingEngine(TransferNPromotionLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "transferNPromotion" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var transferNPromotionQuery = from obj in _context.EmpTransferNpromotions
                                              join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                              join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId

                                              join account in _context.Accounts on obj.IntAccountId equals account.IntAccountId
                                              join bus1 in _context.MasterBusinessUnits on obj.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                              from bus in bus2.DefaultIfEmpty()
                                              join wg1 in _context.MasterWorkplaceGroups on obj.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                              from wg in wg2.DefaultIfEmpty()
                                              join dept1 in _context.MasterDepartments on obj.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                              from dept in dept2.DefaultIfEmpty()
                                              join desig1 in _context.MasterDesignations on obj.IntDesignationId equals desig1.IntDesignationId into desig2
                                              from desig in desig2.DefaultIfEmpty()
                                              join sup1 in _context.EmpEmployeeBasicInfos on obj.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                              from sup in sup2.DefaultIfEmpty()
                                              join lin1 in _context.EmpEmployeeBasicInfos on obj.IntLineManagerId equals lin1.IntEmployeeBasicInfoId into lin2
                                              from lin in lin2.DefaultIfEmpty()

                                              join busFrom1 in _context.MasterBusinessUnits on obj.IntBusinessUnitIdFrom equals busFrom1.IntBusinessUnitId into busFrom2
                                              from busFrom in busFrom2.DefaultIfEmpty()
                                              join wgFrom1 in _context.MasterWorkplaceGroups on obj.IntWorkplaceGroupIdFrom equals wgFrom1.IntWorkplaceGroupId into wgFrom2
                                              from wgFrom in wgFrom2.DefaultIfEmpty()
                                              join wFrom1 in _context.MasterWorkplaces on obj.IntWorkplaceIdFrom equals wFrom1.IntWorkplaceId into wFrom2
                                              from wFrom in wFrom2.DefaultIfEmpty()
                                              join deptFrom1 in _context.MasterDepartments on obj.IntDepartmentIdFrom equals deptFrom1.IntDepartmentId into deptFrom2
                                              from deptFrom in deptFrom2.DefaultIfEmpty()
                                              join desigFrom1 in _context.MasterDesignations on obj.IntDesignationIdFrom equals desigFrom1.IntDesignationId into desigFrom2
                                              from desigFrom in desigFrom2.DefaultIfEmpty()
                                              join supFrom1 in _context.EmpEmployeeBasicInfos on obj.IntDottedSupervisorIdFrom equals supFrom1.IntEmployeeBasicInfoId into supFrom2
                                              from supFrom in supFrom2.DefaultIfEmpty()
                                              join linFrom1 in _context.EmpEmployeeBasicInfos on obj.IntLineManagerIdFrom equals linFrom1.IntEmployeeBasicInfoId into linFrom2
                                              from linFrom in linFrom2.DefaultIfEmpty()
                                              where emp.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteEffectiveDate.Date && obj.DteEffectiveDate.Date <= model.ToDate.Date)
                                                    && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                          : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                          : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                          : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                                    && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                                        || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || wg.StrWorkplaceGroup.ToLower().Contains(model.SearchText) : true)
                                              select new
                                              {
                                                  Application = obj,
                                                  IntTransferNpromotionId = obj.IntTransferNpromotionId,
                                                  IntEmployeeId = obj.IntEmployeeId,
                                                  StrEmployeeName = obj.StrEmployeeName,
                                                  EmployeeCode = emp.StrEmployeeCode,
                                                  StrTransferNpromotionType = obj.StrTransferNpromotionType,
                                                  IntBusinessUnitId = obj.IntBusinessUnitId,
                                                  BusinessUnitName = bus.StrBusinessUnit,
                                                  IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                                  WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                  IntDepartmentId = obj.IntDepartmentId,
                                                  DepartmentName = dept.StrDepartment,
                                                  IntDesignationId = obj.IntDesignationId,
                                                  DesignationName = desig.StrDesignation,
                                                  IntSupervisorId = obj.IntSupervisorId,
                                                  SupervisorName = sup.StrEmployeeName,
                                                  IntLineManagerId = obj.IntLineManagerId,
                                                  LineManagerName = lin.StrEmployeeName,
                                                  IntDottedSupervisorId = obj.IntDottedSupervisorId,
                                                  DteEffectiveDate = obj.DteEffectiveDate,
                                                  DteReleaseDate = obj.DteReleaseDate,
                                                  IntAttachementId = obj.IntAttachementId,
                                                  StrRemarks = obj.StrRemarks,
                                                  StrStatus = obj.IsJoined == true ? "Joined" : obj.DteReleaseDate != null ? "Released" : (obj.IsPipelineClosed == true && obj.IsReject == false) ? "Approved" : (obj.IsPipelineClosed == true && obj.IsReject == true) ? "Rejected" : "Pending",
                                                  CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,

                                                  IntBusinessUnitIdFrom = busFrom != null ? busFrom.IntBusinessUnitId : 0,
                                                  BusinessUnitNameFrom = busFrom != null ? busFrom.StrBusinessUnit : "",
                                                  IntWorkplaceGroupIdFrom = wgFrom != null ? wgFrom.IntWorkplaceGroupId : 0,
                                                  WorkplaceGroupNameFrom = wgFrom != null ? wgFrom.StrWorkplaceGroup : "",
                                                  IntWorkplaceIdFrom = wFrom != null ? wFrom.IntWorkplaceId : 0,
                                                  WorkplaceNameFrom = wFrom != null ? wFrom.StrWorkplaceGroup : "",
                                                  IntDepartmentIdFrom = deptFrom != null ? deptFrom.IntDepartmentId : 0,
                                                  DepartmentNameFrom = deptFrom != null ? deptFrom.StrDepartment : "",
                                                  IntDesignationIdFrom = desigFrom != null ? desigFrom.IntDesignationId : 0,
                                                  DesignationNameFrom = desigFrom != null ? desigFrom.StrDesignation : "",
                                                  IntSupervisorIdFrom = supFrom != null ? supFrom.IntEmployeeBasicInfoId : 0,
                                                  SupervisorNameFrom = supFrom != null ? supFrom.StrEmployeeName : "",
                                                  IntLineManagerIdFrom = linFrom != null ? linFrom.IntEmployeeBasicInfoId : 0,
                                                  LineManagerNameFrom = linFrom != null ? linFrom.StrEmployeeName : ""
                                              };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var transferNPromotioForAdmin = await transferNPromotionQuery.Select(obj => new TransferNPromotionLandingReturnViewModel
                    {
                        //Application = obj.Application,
                        IntTransferNpromotionId = obj.IntTransferNpromotionId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        StrTransferNpromotionType = obj.StrTransferNpromotionType,
                        IntBusinessUnitId = obj.IntBusinessUnitId,
                        BusinessUnitName = obj.BusinessUnitName,
                        IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                        WorkplaceGroupName = obj.WorkplaceGroupName,
                        IntDepartmentId = obj.IntDepartmentId,
                        DepartmentName = obj.DepartmentName,
                        IntDesignationId = obj.IntDesignationId,
                        DesignationName = obj.DesignationName,
                        IntSupervisorId = obj.IntSupervisorId,
                        SupervisorName = obj.SupervisorName,
                        IntLineManagerId = obj.IntLineManagerId,
                        LineManagerName = obj.LineManagerName,
                        IntDottedSupervisorId = obj.IntDottedSupervisorId,
                        DteEffectiveDate = obj.DteEffectiveDate.Date.ToString("dd-MMM-yyyy"),
                        DteReleaseDate = obj.DteReleaseDate.Value.Date.ToString("dd-MMM-yyyy"),
                        IntAttachementId = obj.IntAttachementId,
                        StrRemarks = obj.StrRemarks,
                        StrStatus = obj.StrStatus,
                        CurrentStage = obj.CurrentStage,

                        IntBusinessUnitIdFrom = obj.IntBusinessUnitIdFrom,
                        BusinessUnitNameFrom = obj.BusinessUnitNameFrom,
                        IntWorkplaceGroupIdFrom = obj.IntWorkplaceGroupIdFrom,
                        WorkplaceGroupNameFrom = obj.WorkplaceGroupNameFrom,
                        IntDepartmentIdFrom = obj.IntDepartmentIdFrom,
                        DepartmentNameFrom = obj.DepartmentNameFrom,
                        IntDesignationIdFrom = obj.IntDesignationIdFrom,
                        DesignationNameFrom = obj.DesignationNameFrom,
                        IntSupervisorIdFrom = obj.IntSupervisorIdFrom,
                        SupervisorNameFrom = obj.SupervisorNameFrom,
                        IntLineManagerIdFrom = obj.IntLineManagerIdFrom,
                        LineManagerNameFrom = obj.LineManagerNameFrom
                    }).ToListAsync();

                    retObj.TotalCount = transferNPromotioForAdmin.Count();
                    retObj.Data = transferNPromotioForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var transferNPromotionList = await transferNPromotionQuery.ToListAsync();

                var listData = from obj in transferNPromotionList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.IntSupervisorId == tokenData.employeeId || obj.IntDottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.IntLineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new TransferNPromotionLandingReturnViewModel
                               {
                                   IntTransferNpromotionId = obj.IntTransferNpromotionId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   StrTransferNpromotionType = obj.StrTransferNpromotionType,
                                   IntBusinessUnitId = obj.IntBusinessUnitId,
                                   BusinessUnitName = obj.BusinessUnitName,
                                   IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                   WorkplaceGroupName = obj.WorkplaceGroupName,
                                   IntDepartmentId = obj.IntDepartmentId,
                                   DepartmentName = obj.DepartmentName,
                                   IntDesignationId = obj.IntDesignationId,
                                   DesignationName = obj.DesignationName,
                                   IntSupervisorId = obj.IntSupervisorId,
                                   SupervisorName = obj.SupervisorName,
                                   IntLineManagerId = obj.IntLineManagerId,
                                   LineManagerName = obj.LineManagerName,
                                   IntDottedSupervisorId = obj.IntDottedSupervisorId,
                                   DteEffectiveDate = obj.DteEffectiveDate.Date.ToString("dd-MMM-yyyy"),
                                   DteReleaseDate = obj.DteReleaseDate.Value.Date.ToString("dd-MMM-yyyy"),
                                   IntAttachementId = obj.IntAttachementId,
                                   StrRemarks = obj.StrRemarks,
                                   StrStatus = obj.StrStatus,
                                   CurrentStage = obj.CurrentStage,

                                   IntBusinessUnitIdFrom = obj.IntBusinessUnitIdFrom,
                                   BusinessUnitNameFrom = obj.BusinessUnitNameFrom,
                                   IntWorkplaceGroupIdFrom = obj.IntWorkplaceGroupIdFrom,
                                   WorkplaceGroupNameFrom = obj.WorkplaceGroupNameFrom,
                                   IntDepartmentIdFrom = obj.IntDepartmentIdFrom,
                                   DepartmentNameFrom = obj.DepartmentNameFrom,
                                   IntDesignationIdFrom = obj.IntDesignationIdFrom,
                                   DesignationNameFrom = obj.DesignationNameFrom,
                                   IntSupervisorIdFrom = obj.IntSupervisorIdFrom,
                                   SupervisorNameFrom = obj.SupervisorNameFrom,
                                   IntLineManagerIdFrom = obj.IntLineManagerIdFrom,
                                   LineManagerNameFrom = obj.LineManagerNameFrom
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Transfer & Promotion

        #region Employee Increment

        public async Task<EmployeeIncrementApprovalResponse> EmployeeIncrementApprovalEngine(EmpEmployeeIncrement application, bool isReject, long approverId, long accountId)
        {
            try
            {
                EmployeeIncrementApprovalResponse response = new EmployeeIncrementApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await EmployeeIncrementApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await EmployeeIncrementApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await EmployeeIncrementApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await EmployeeIncrementApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> EmployeeIncrementLandingEngine(EmployeeIncrementLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var EmpIncrements = from obj in _context.EmpEmployeeIncrements
                                    where obj.IsActive == true
                                    join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                    join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                    join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                    from dept in dp.DefaultIfEmpty()
                                    join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig2
                                    from desig in desig2.DefaultIfEmpty()
                                    where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                           (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                            || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true)
                                           && (model.FromDate.Date <= obj.DteEffectiveDate && obj.DteEffectiveDate <= model.ToDate)
                                           && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                           : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                           : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                           : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                    select new
                                    {

                                        IntId = obj.IntIncrementId,
                                        IntEmployeeId = obj.IntEmployeeId,
                                        StrEmployeeName = emp.StrEmployeeName,
                                        EmployeeCode = emp.StrEmployeeCode,
                                        StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
                                        IntDepartmentId = emp.IntDepartmentId,
                                        StrDepartment = dept != null ? dept.StrDepartment : "",
                                        IntDesignationId = emp.IntDesignationId,
                                        StrDesignation = desig != null ? desig.StrDesignation : "",
                                        IntDesignationTypeId = obj.IntDesignationId,
                                        IntAccountId = obj.IntAccountId,
                                        IntBusinessUnitId = obj.IntBusinessUnitId,
                                        StrIncrementDependOn = obj.StrIncrementDependOn,
                                        NumIncrementDependOn = obj.NumIncrementDependOn,
                                        NumIncrementPercentage = obj.NumIncrementPercentage,
                                        NumIncrementAmount = obj.NumIncrementAmount,
                                        DteEffectiveDate = obj.DteEffectiveDate,
                                        IntTransferNpromotionReferenceId = obj.IntTransferNpromotionReferenceId,
                                        IsActive = obj.IsActive,
                                        CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                        Application = obj,
                                        SupervisorId = emp.IntSupervisorId,
                                        DottedSupervisorId = emp.IntDottedSupervisorId,
                                        LineManagerId = emp.IntLineManagerId,
                                    };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    retObj.TotalCount = await EmpIncrements.CountAsync();
                    retObj.Data = await EmpIncrements.Select(obj => new EmployeeIncrementLandingReturnViewModel
                    {
                        IntId = obj.IntId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        StrEmploymentType = obj.StrEmploymentType,
                        IntDepartmentId = obj.IntDepartmentId,
                        StrDepartment = obj.StrDepartment,
                        IntDesignationId = obj.IntDesignationId,
                        StrDesignation = obj.StrDesignation,
                        IntDesignationTypeId = obj.IntDesignationTypeId,
                        IntAccountId = obj.IntAccountId,
                        IntBusinessUnitId = obj.IntBusinessUnitId,
                        StrIncrementDependOn = obj.StrIncrementDependOn,
                        NumIncrementDependOn = obj.NumIncrementDependOn,
                        NumIncrementPercentage = obj.NumIncrementPercentage,
                        NumIncrementAmount = obj.NumIncrementAmount,
                        DteEffectiveDate = obj.DteEffectiveDate,
                        IntTransferNpromotionReferenceId = obj.IntTransferNpromotionReferenceId,
                        IsActive = obj.IsActive,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).AsNoTrackingWithIdentityResolution().Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
                    return retObj;
                }

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "increment" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var EmpIncrementsList = await EmpIncrements.AsNoTrackingWithIdentityResolution().ToListAsync();

                var listData = from obj in EmpIncrementsList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new EmployeeIncrementLandingReturnViewModel
                               {
                                   IntId = obj.IntId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployeeName = obj.StrEmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   StrEmploymentType = obj.StrEmploymentType,
                                   IntDepartmentId = obj.IntDepartmentId,
                                   StrDepartment = obj.StrDepartment,
                                   IntDesignationId = obj.IntDesignationId,
                                   StrDesignation = obj.StrDesignation,
                                   IntDesignationTypeId = obj.IntDesignationTypeId,
                                   IntAccountId = obj.IntAccountId,
                                   IntBusinessUnitId = obj.IntBusinessUnitId,
                                   StrIncrementDependOn = obj.StrIncrementDependOn,
                                   NumIncrementDependOn = obj.NumIncrementDependOn,
                                   NumIncrementPercentage = obj.NumIncrementPercentage,
                                   NumIncrementAmount = obj.NumIncrementAmount,
                                   DteEffectiveDate = obj.DteEffectiveDate,
                                   IntTransferNpromotionReferenceId = obj.IntTransferNpromotionReferenceId,
                                   IsActive = obj.IsActive,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };
                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;


                //EmployeeIncrementApprovalResponse response = new EmployeeIncrementApprovalResponse
                //{
                //    ResponseStatus = "",
                //    ApplicationStatus = "",
                //    CurrentSatageId = 0,
                //    NextSatageId = 0,
                //    IsComplete = true,
                //    ListData = new List<EmployeeIncrementLandingReturnViewModel>()
                //};

                //EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
                //model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
                //model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
                //model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

                //if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
                //    && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
                //{
                //    response.ResponseStatus = "Invalid EmployeeId";
                //}

                //GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.AsNoTracking().AsQueryable().Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == "increment".ToLower());

                //if (header == null)
                //{
                //    response.ResponseStatus = "Invalid approval pipeline";
                //}
                //else
                //{
                //    List<EmployeeIncrementLandingReturnViewModel> applicationList = new List<EmployeeIncrementLandingReturnViewModel>();

                //    if ((bool)model.IsAdmin)
                //    {
                //        model.IsSupOrLineManager = -1;
                //        applicationList = await EmployeeIncrementLandingEngine(model, header, null);
                //        if (applicationList.Count() > 0)
                //        {
                //            response.ListData.AddRange(applicationList);
                //            applicationList = new List<EmployeeIncrementLandingReturnViewModel>();
                //        }
                //    }
                //    else
                //    {
                //        if (model.IsSupervisor == true)
                //        {
                //            model.IsSupOrLineManager = 1;
                //            applicationList = await EmployeeIncrementLandingEngine(model, header, null);
                //            if (applicationList.Count() > 0)
                //            {
                //                response.ListData.AddRange(applicationList);
                //                applicationList = new List<EmployeeIncrementLandingReturnViewModel>();
                //            }
                //        }
                //        if (model.IsLineManager == true)
                //        {
                //            model.IsSupOrLineManager = 2;
                //            applicationList = await EmployeeIncrementLandingEngine(model, header, null);
                //            if (applicationList.Count() > 0)
                //            {
                //                response.ListData.AddRange(applicationList);
                //                applicationList = new List<EmployeeIncrementLandingReturnViewModel>();
                //            }
                //        }
                //        if (model.IsUserGroup == true)
                //        {
                //            model.IsSupOrLineManager = 3;
                //            foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
                //            {
                //                applicationList = await EmployeeIncrementLandingEngine(model, header, userGroup);
                //                if (applicationList.Count() > 0)
                //                {
                //                    response.ListData.AddRange(applicationList);
                //                    applicationList = new List<EmployeeIncrementLandingReturnViewModel>();
                //                }
                //            }
                //        }
                //    }
                //}
                //response.ListData = response.ListData.OrderByDescending(x => x.Application.DteCreatedAt).ToList();
                //return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Employee Increment

        #region Bonus Generate

        public async Task<BonusGenerateHeaderApprovalResponse> BonusGenerateHeaderApprovalEngine(PyrBonusGenerateHeader application, bool isReject, long approverId, long accountId)
        {
            try
            {
                BonusGenerateHeaderApprovalResponse response = new BonusGenerateHeaderApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await BonusGenerateHeaderApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await BonusGenerateHeaderApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await BonusGenerateHeaderApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await BonusGenerateHeaderApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> BonusGenerateHeaderLandingEngine(BonusGenerateHeaderLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "bonusGenerate" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var bonusGeneratQuery = from obj in _context.PyrBonusGenerateHeaders
                                        where obj.IsActive == true
                                        join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                        join bu in _context.MasterBusinessUnits on obj.IntBusinessUnitId equals bu.IntBusinessUnitId into bu2
                                        from BusinessU in bu2.DefaultIfEmpty()
                                        join soleDp in _context.TerritorySetups on obj.IntSoleDepoId equals soleDp.IntTerritoryId into soledp2
                                        from soleDp in soledp2.DefaultIfEmpty()
                                        join area in _context.TerritorySetups on obj.IntAreaId equals area.IntTerritoryId into area2
                                        from area in area2.DefaultIfEmpty()
                                        join wg in _context.MasterWorkplaceGroups on obj.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wg2
                                        from wg in wg2.DefaultIfEmpty()

                                        where obj.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteEffectedDateTime.Value.Date && obj.DteEffectedDateTime.Value.Date <= model.ToDate)
                                                && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                     : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                     : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                     : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                                && (!string.IsNullOrEmpty(model.SearchText) ? obj.StrBonusName.ToLower().Contains(model.SearchText) || wg.StrWorkplaceGroup.ToLower().Contains(model.SearchText)
                                                        || soleDp.StrTerritoryName.ToLower().Contains(model.SearchText) || area.StrTerritoryName.ToLower().Contains(model.SearchText) : true)
                                        select new
                                        {
                                            IntBonusHeaderId = obj.IntBonusHeaderId,
                                            IntBusinessUnitId = obj.IntBusinessUnitId,
                                            StrBusinessUnitName = BusinessU.StrBusinessUnit,
                                            IntBonusId = obj.IntBonusId,
                                            StrBonusName = obj.StrBonusName,
                                            IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                            StrWorkplaceGroupName = wg != null ? wg.StrWorkplaceGroup : "",
                                            DteEffectedDateTime = obj.DteEffectedDateTime.Value.Date,
                                            NumBonusAmount = obj.NumBonusAmount,
                                            IsArrearBonus = obj.IsArrearBonus,
                                            IntArrearBonusReferenceId = obj.IntArrearBonusReferenceId,
                                            IntBonusYearCal = obj.IntBonusYearCal,
                                            IntBonusMonthCal = obj.IntBonusMonthCal,
                                            CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                            Status = model.ApplicationStatus,
                                            Application = obj,
                                            SoleDepoId = obj.IntSoleDepoId,
                                            SoleDepoName = soleDp.StrTerritoryName,
                                            AreaId = obj.IntAreaId,
                                            AreaName = area.StrTerritoryName,
                                            //TerritoryIdList = obj.StrTerritoryIdList,
                                            //TerritoryNameList = obj.StrTerritoryNameList,
                                        };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var bonusGeneratListForAdmin = await bonusGeneratQuery.Select(obj => new BonusGenerateHeaderLandingReturnViewModel
                    {
                        IntBonusHeaderId = obj.IntBonusHeaderId,
                        IntBusinessUnitId = obj.IntBusinessUnitId,
                        StrBusinessUnitName = obj.StrBusinessUnitName,
                        IntBonusId = obj.IntBonusId,
                        StrBonusName = obj.StrBonusName,
                        IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                        StrWorkplaceGroupName = obj.StrWorkplaceGroupName,
                        DteEffectedDateTime = obj.DteEffectedDateTime.Date.ToString("dd-MMM-yyyy"),
                        NumBonusAmount = obj.NumBonusAmount,
                        IsArrearBonus = obj.IsArrearBonus,
                        IntArrearBonusReferenceId = obj.IntArrearBonusReferenceId,
                        IntBonusYearCal = obj.IntBonusYearCal,
                        IntBonusMonthCal = obj.IntBonusMonthCal,
                        SoleDepoId = obj.SoleDepoId,
                        SoleDepoName = obj.SoleDepoName,
                        AreaId = obj.AreaId,
                        AreaName = obj.AreaName,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).ToListAsync();

                    retObj.TotalCount = bonusGeneratListForAdmin.Count();
                    retObj.Data = bonusGeneratListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var bonusGeneratList = await bonusGeneratQuery.ToListAsync();

                var listData = from obj in bonusGeneratList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new BonusGenerateHeaderLandingReturnViewModel
                               {
                                   IntBonusHeaderId = obj.IntBonusHeaderId,
                                   IntBusinessUnitId = obj.IntBusinessUnitId,
                                   StrBusinessUnitName = obj.StrBusinessUnitName,
                                   IntBonusId = obj.IntBonusId,
                                   StrBonusName = obj.StrBonusName,
                                   IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                   StrWorkplaceGroupName = obj.StrWorkplaceGroupName,
                                   DteEffectedDateTime = obj.DteEffectedDateTime.Date.ToString("dd-MMM-yyyy"),
                                   NumBonusAmount = obj.NumBonusAmount,
                                   IsArrearBonus = obj.IsArrearBonus,
                                   IntArrearBonusReferenceId = obj.IntArrearBonusReferenceId,
                                   IntBonusYearCal = obj.IntBonusYearCal,
                                   IntBonusMonthCal = obj.IntBonusMonthCal,
                                   SoleDepoId = obj.SoleDepoId,
                                   SoleDepoName = obj.SoleDepoName,
                                   AreaId = obj.AreaId,
                                   AreaName = obj.AreaName,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Bonus Generate

        #region PF Withdraw

        public async Task<PFWithdrawApprovalResponse> PFWithdrawApprovalEngine(EmpPfwithdraw application, bool isReject, long approverId, long accountId)
        {
            try
            {
                PFWithdrawApprovalResponse response = new PFWithdrawApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await PFWithdrawApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await PFWithdrawApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await PFWithdrawApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await PFWithdrawApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "pfWithdraw" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var pfWithdrawQuery = from obj in _context.EmpPfwithdraws
                                      where obj.IsActive == true
                                      join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                      join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                      join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                      from depart in dp.DefaultIfEmpty()
                                      join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
                                      from designat in desig.DefaultIfEmpty()
                                      where emp.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteApplicationDate.Date && obj.DteApplicationDate.Date <= model.ToDate.Date)
                                            && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                      select new
                                      {
                                          IntPfwithdrawId = obj.IntPfwithdrawId,
                                          IntEmployeeId = obj.IntEmployeeId,
                                          StrEmployeeName = emp != null ? emp.StrEmployeeName : "",
                                          EmployeeCode = emp.StrEmployeeCode,
                                          StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
                                          IntDepartmentId = emp.IntDepartmentId,
                                          StrDepartment = depart != null ? depart.StrDepartment : "",
                                          IntDesignationId = emp.IntDesignationId,
                                          StrDesignation = designat != null ? designat.StrDesignation : "",
                                          DteApplicationDate = obj.DteApplicationDate,
                                          NumWithdrawAmount = obj.NumWithdrawAmount,
                                          StrReason = obj.StrReason,
                                          CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                          Status = model.ApplicationStatus,
                                          SupervisorId = emp.IntSupervisorId,
                                          LineManagerId = emp.IntLineManagerId,
                                          DottedSupervisorId = emp.IntDottedSupervisorId,
                                          Application = obj
                                      };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    //pagination here

                    var pfWithdrawQueryListForAdmin = await pfWithdrawQuery.Select(obj => new PFWithdrawLandingReturnViewModel
                    {
                        IntId = obj.IntPfwithdrawId,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployee = obj.StrEmployeeName,
                        ////IntDepartmentId = obj.IntDepartmentId,
                        StrDepartment = obj.StrDepartment,
                        ////IntDesignationId = obj.IntDesignationId,
                        StrDesignation = obj.StrDesignation,
                        NumWithdrawAmount = obj.NumWithdrawAmount,
                        DteApplicationDate = obj.DteApplicationDate.Date,
                        StrReason = obj.StrReason,
                        CurrentStage = obj.CurrentStage,
                        //Status = obj.Status,
                        Application = obj.Application,
                    }).ToListAsync();

                    retObj.TotalCount = pfWithdrawQueryListForAdmin.Count();
                    retObj.Data = pfWithdrawQueryListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var pfWithdrawList = await pfWithdrawQuery.ToListAsync();

                var listData = from obj in pfWithdrawList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new PFWithdrawLandingReturnViewModel
                               {
                                   IntId = obj.IntPfwithdrawId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   StrEmployee = obj.StrEmployeeName,
                                   //IntDepartmentId = obj.IntDepartmentId,
                                   StrDepartment = obj.StrDepartment,
                                   //IntDesignationId = obj.IntDesignationId,
                                   StrDesignation = obj.StrDesignation,
                                   NumWithdrawAmount = obj.NumWithdrawAmount,
                                   DteApplicationDate = obj.DteApplicationDate.Date,
                                   StrReason = obj.StrReason,
                                   CurrentStage = obj.CurrentStage,
                                   //Status = obj.Status,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<PFWithdrawApprovalResponse> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model)
        //{
        //    try
        //    {
        //        PFWithdrawApprovalResponse response = new PFWithdrawApprovalResponse
        //        {
        //            ResponseStatus = "",
        //            ApplicationStatus = "",
        //            CurrentSatageId = 0,
        //            NextSatageId = 0,
        //            IsComplete = true,
        //            ListData = new List<PFWithdrawLandingReturnViewModel>()
        //        };

        //        EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
        //        model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
        //        model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
        //        model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

        //        if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
        //            && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
        //        {
        //            response.ResponseStatus = "Invalid EmployeeId";
        //        }

        //        GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == "pfWithdraw".ToLower());

        //        if (header == null)
        //        {
        //            response.ResponseStatus = "Invalid approval pipeline";
        //        }
        //        else
        //        {
        //            List<PFWithdrawLandingReturnViewModel> applicationList = new List<PFWithdrawLandingReturnViewModel>();

        //            if ((bool)model.IsAdmin)
        //            {
        //                model.IsSupOrLineManager = -1;
        //                applicationList = await PFWithdrawLandingEngine(model, header, null);
        //                if (applicationList.Count() > 0)
        //                {
        //                    response.ListData.AddRange(applicationList);
        //                    applicationList = new List<PFWithdrawLandingReturnViewModel>();
        //                }
        //            }
        //            else
        //            {
        //                if (model.IsSupervisor == true)
        //                {
        //                    model.IsSupOrLineManager = 1;
        //                    applicationList = await PFWithdrawLandingEngine(model, header, null);
        //                    if (applicationList.Count() > 0)
        //                    {
        //                        response.ListData.AddRange(applicationList);
        //                        applicationList = new List<PFWithdrawLandingReturnViewModel>();
        //                    }
        //                }
        //                if (model.IsLineManager == true)
        //                {
        //                    model.IsSupOrLineManager = 2;
        //                    applicationList = await PFWithdrawLandingEngine(model, header, null);
        //                    if (applicationList.Count() > 0)
        //                    {
        //                        response.ListData.AddRange(applicationList);
        //                        applicationList = new List<PFWithdrawLandingReturnViewModel>();
        //                    }
        //                }
        //                if (model.IsUserGroup == true)
        //                {
        //                    model.IsSupOrLineManager = 3;
        //                    foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
        //                    {
        //                        applicationList = await PFWithdrawLandingEngine(model, header, userGroup);
        //                        if (applicationList.Count() > 0)
        //                        {
        //                            response.ListData.AddRange(applicationList);
        //                            applicationList = new List<PFWithdrawLandingReturnViewModel>();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        response.ListData = response.ListData.OrderByDescending(x => x.Application.DteCreatedAt).ToList();
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion PF Withdraw

        #region Arrear Salary Generate Request

        public async Task<ArearSalaryGenerateRequestApprovalResponse> ArearSalaryGenerateRequestApprovalEngine(PyrArearSalaryGenerateRequest application, bool isReject, long approverId, long accountId)
        {
            try
            {
                ArearSalaryGenerateRequestApprovalResponse response = new ArearSalaryGenerateRequestApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await ArearSalaryGenerateRequestApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await ArearSalaryGenerateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await ArearSalaryGenerateRequestApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await ArearSalaryGenerateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> ArearSalaryGenerateRequestLandingEngine(ArearSalaryGenerateRequestLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "arearSalaryGenerateRequest" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var areaSalaryApplicationQuery = from obj in _context.PyrArearSalaryGenerateRequests
                                                 where obj.IsActive == true
                                                 join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                                 join bu in _context.MasterBusinessUnits on obj.IntBusinessUnitId equals bu.IntBusinessUnitId into bu2
                                                 from BusinessU in bu2.DefaultIfEmpty()
                                                 where obj.IntAccountId == tokenData.accountId && obj.IsActive == true && ((model.FromDate.Date <= obj.DteEffectiveFrom.Date && obj.DteEffectiveTo.Date <= model.ToDate) || (model.FromDate.Year == obj.DteEffectiveFrom.Year && model.FromDate.Month == obj.DteEffectiveFrom.Month) || (model.ToDate.Year == obj.DteEffectiveTo.Year && model.ToDate.Month == obj.DteEffectiveTo.Month))
                                                 && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                     : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                     : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                     : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                                 && (!string.IsNullOrEmpty(model.SearchText) ? obj.StrArearSalaryCode.ToLower().Contains(model.SearchText) || obj.StrSalaryPolicyName.ToLower().Contains(model.SearchText) : true)
                                                 select new
                                                 {
                                                     ArearSalaryGenerateRequestId = obj.IntArearSalaryGenerateRequestId,
                                                     ArearSalaryCode = obj.StrArearSalaryCode,
                                                     BusinessUnitId = obj.IntBusinessUnitId,
                                                     BusinessUnit = BusinessU != null ? BusinessU.StrBusinessUnit : "",
                                                     //IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                                     //StrWorkplaceGroup = wkpg != null ? wkpg.StrWorkplaceGroup : "",
                                                     numPercentOfGross = obj.NumPercentOfGross,
                                                     SalaryPolicyName = obj.StrSalaryPolicyName,
                                                     DateRange = obj.DteEffectiveFrom.Date.ToString("dd MMM, yyyy") + "-" + obj.DteEffectiveTo.Date.ToString("dd MMM, yyyy"),
                                                     DteEffectiveFrom = obj.DteEffectiveFrom,
                                                     DteEffectiveTo = obj.DteEffectiveTo,
                                                     IsGenerated = obj.IsGenerated,
                                                     StrDescription = obj.StrDescription,
                                                     NumNetPayableSalary = obj.NumNetPayableSalary,
                                                     CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                     Application = obj
                                                 };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var areaSalaryApplicationList = await areaSalaryApplicationQuery.Select(obj => new ArearSalaryGenerateRequestLandingReturnViewModel
                    {
                        ArearSalaryGenerateRequestId = obj.ArearSalaryGenerateRequestId,
                        ArearSalaryCode = obj.ArearSalaryCode,
                        BusinessUnitId = obj.BusinessUnitId,
                        BusinessUnit = obj.BusinessUnit,
                        //IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                        //StrWorkplaceGroup = wkpg != null ? wkpg.StrWorkplaceGroup : "",
                        numPercentOfGross = obj.numPercentOfGross,
                        SalaryPolicyName = obj.SalaryPolicyName,
                        DateRange = obj.DateRange,
                        //DteEffectiveFrom = obj.DteEffectiveFrom,
                        //DteEffectiveTo = obj.DteEffectiveTo,
                        IsGenerated = obj.IsGenerated,
                        StrDescription = obj.StrDescription,
                        NumNetPayableSalary = obj.NumNetPayableSalary,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).ToListAsync();

                    retObj.TotalCount = areaSalaryApplicationList.Count();
                    retObj.Data = areaSalaryApplicationList.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var areaSalaryApplicationQueryList = await areaSalaryApplicationQuery.ToListAsync();


                var listData = from obj in areaSalaryApplicationQueryList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new ArearSalaryGenerateRequestLandingReturnViewModel
                               {
                                   ArearSalaryGenerateRequestId = obj.ArearSalaryGenerateRequestId,
                                   ArearSalaryCode = obj.ArearSalaryCode,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   BusinessUnit = obj.BusinessUnit,
                                   //IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                   //StrWorkplaceGroup = wkpg != null ? wkpg.StrWorkplaceGroup : "",
                                   numPercentOfGross = obj.numPercentOfGross,
                                   SalaryPolicyName = obj.SalaryPolicyName,
                                   DateRange = obj.DateRange,
                                   //DteEffectiveFrom = obj.DteEffectiveFrom,
                                   //DteEffectiveTo = obj.DteEffectiveTo,
                                   IsGenerated = obj.IsGenerated,
                                   StrDescription = obj.StrDescription,
                                   NumNetPayableSalary = obj.NumNetPayableSalary,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Arrear Salary Generate Request

        #region Expense Application

        public async Task<ApplicationLandingVM> ExpenseLandingEngine(ExpenseApplicationLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var expenseQuery = from obj in _context.EmpExpenseApplications
                                   where obj.IsActive == true
                                   join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                   join expT in _context.EmpExpenseTypes on obj.IntExpenseTypeId equals expT.IntExpenseTypeId into expT1
                                   from expType in expT1.DefaultIfEmpty()
                                   join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                                   from emp in emp1.DefaultIfEmpty()
                                   join dep1 in _context.MasterDepartments on emp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                   from dep in dep2.DefaultIfEmpty()
                                   join deg1 in _context.MasterDesignations on emp.IntDesignationId equals deg1.IntDesignationId into deg2
                                   from desg in deg2.DefaultIfEmpty()
                                   where obj.IntAccountId == tokenData.accountId && obj.IsActive == true
                                   //&& (model.FromDate.Date <= obj.DteApplicationDate.Date && obj.DteApplicationDate.Date <= model.ToDate.Date)
                                   && ((model.FromDate.Date <= obj.DteExpenseFromDate.Date && obj.DteExpenseToDate.Date <= model.ToDate) || (model.FromDate.Year == obj.DteExpenseFromDate.Year && model.FromDate.Month == obj.DteExpenseFromDate.Month) || (model.ToDate.Year == obj.DteExpenseToDate.Year && model.ToDate.Month == obj.DteExpenseToDate.Month))
                                        && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                                : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                                : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                                : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)

                                        && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desg.StrDesignation.ToLower().Contains(model.SearchText)
                                                            || dep.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText)
                                                            || expType.StrExpenseType.ToLower().Contains(model.SearchText) : true)
                                   select new
                                   {
                                       EmployeeName = emp.StrEmployeeName,
                                       EmployeeCode = emp.StrEmployeeCode,
                                       EmploymentType = emp.StrEmploymentType,
                                       BusinessUnitId = emp.IntBusinessUnitId,
                                       ProfileUrlId = _context.EmpEmployeePhotoIdentities.Where(x => x.IsActive == true && x.IntEmployeeBasicInfoId == emp.IntEmployeeBasicInfoId).Select(x => x.IntProfilePicFileUrlId).FirstOrDefault(),
                                       Department = dep != null ? dep.StrDepartment : "",
                                       Designation = desg != null ? desg.StrDesignation : "",
                                       IntExpenseTypeId = obj.IntExpenseTypeId,
                                       StrExpenseType = expType.StrExpenseType,
                                       IntExpenseId = obj.IntExpenseId,
                                       //DteExpenseFromDate = obj.DteExpenseFromDate,
                                       //DteExpenseToDate = obj.DteExpenseToDate,
                                       DateRange = obj.DteExpenseFromDate.Date.ToString("dd MMM, yyyy") + "-" + obj.DteExpenseToDate.Date.ToString("dd MMM, yyyy"),
                                       IntEmployeeId = obj.IntEmployeeId,
                                       NumExpenseAmount = obj.NumExpenseAmount,
                                       StrDiscription = obj.StrDiscription,
                                       //IntAccountId = obj.IntAccountId,
                                       CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                       SupervisorId = emp.IntSupervisorId,
                                       DottedSupervisorId = emp.IntDottedSupervisorId,
                                       LineManagerId = emp.IntLineManagerId,
                                       Application = obj
                                   };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var expenseListForAdmin = await expenseQuery.Select(obj => new ExpenseApplicationLandingReturnViewModel
                    {
                        IntExpenseId = obj.IntExpenseId,
                        IntEmployeeId = obj.IntEmployeeId,
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        BusinessUnitId = obj.BusinessUnitId,
                        ProfileUrlId = obj.ProfileUrlId,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        IntExpenseTypeId = obj.IntExpenseTypeId,
                        StrExpenseType = obj.StrExpenseType,
                        DateRange = obj.DateRange,
                        NumExpenseAmount = obj.NumExpenseAmount,
                        StrDiscription = obj.StrDiscription,
                        CurrentStage = obj.CurrentStage,
                        Application = obj.Application
                    }).ToListAsync();

                    retObj.TotalCount = expenseListForAdmin.Count();
                    retObj.Data = expenseListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "expense" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var expenseList = await expenseQuery.ToListAsync();

                var listData = from obj in expenseList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new ExpenseApplicationLandingReturnViewModel
                               {
                                   IntExpenseId = obj.IntExpenseId,
                                   IntEmployeeId = obj.IntEmployeeId,
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   ProfileUrlId = obj.ProfileUrlId,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   IntExpenseTypeId = obj.IntExpenseTypeId,
                                   StrExpenseType = obj.StrExpenseType,
                                   DateRange = obj.DateRange,
                                   NumExpenseAmount = obj.NumExpenseAmount,
                                   StrDiscription = obj.StrDiscription,
                                   CurrentStage = obj.CurrentStage,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ExpenseApprovalResponse> ExpenseApprovalEngine(EmpExpenseApplication application, bool isReject, long approverId, long accountId)
        {
            try
            {
                ExpenseApprovalResponse response = new ExpenseApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await ExpenseApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await ExpenseApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await ExpenseApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await ExpenseApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Expense Application

        #region =========== Salary Certificate Requisition ===========

        public async Task<SalaryCertificateRequestApprovalResponse> SalaryCertificateRequestApprovalEngine(EmpSalaryCertificateRequest application, bool isReject, long approverId, long accountId)
        {
            try
            {
                SalaryCertificateRequestApprovalResponse response = new SalaryCertificateRequestApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await SalaryCertificateRequestApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await SalaryCertificateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await SalaryCertificateRequestApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await SalaryCertificateRequestApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> SalaryCertificateRequestLandingEngine(SalaryCertificateRequestLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var list = (from obj in _context.EmpSalaryCertificateRequests
                            where obj.IsActive == true
                            join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                            join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                            from emp in emp1.DefaultIfEmpty()
                            join dep1 in _context.MasterDepartments on emp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                            from dep in dep2.DefaultIfEmpty()
                            join deg1 in _context.MasterDesignations on emp.IntDesignationId equals deg1.IntDesignationId into deg2
                            from desg in deg2.DefaultIfEmpty()
                            where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                     (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desg.StrDesignation.ToLower().Contains(model.SearchText)
                                      || dep.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true)
                                     && (model.FromDate.Date <= obj.DteCreatedAt && obj.DteCreatedAt <= model.ToDate)
                                     && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                     : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                     : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                     : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)

                            select new
                            {
                                IntId = obj.IntSalaryCertificateRequestId,
                                IntAccountId = obj.IntAccountId,
                                IntEmployeeId = obj.IntEmployeeId,
                                StrEmployee = emp.StrEmployeeName,
                                EmployeeCode = emp.StrEmployeeCode,
                                StrDepartment = dep.StrDepartment,
                                StrDesignation = desg.StrDesignation,
                                IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                StrEmploymentType = emp.StrEmploymentType,
                                IntPayRollMonth = obj.IntPayRollMonth,
                                IntPayRollYear = obj.IntPayRollYear,
                                IntCreatedBy = obj.IntCreatedBy,
                                IsActive = obj.IsActive,
                                CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                Application = obj,
                                SupervisorId = emp.IntSupervisorId,
                                DottedSupervisorId = emp.IntDottedSupervisorId,
                                LineManagerId = emp.IntLineManagerId
                            }).AsNoTrackingWithIdentityResolution().AsQueryable();

                SalaryCertificateRequestLandingReturnViewModel vm = new();
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    retObj.TotalCount = list.Count();
                    retObj.Data = list.Select(x => new SalaryCertificateRequestLandingReturnViewModel
                    {
                        IntId = x.IntId,
                        IntAccountId = x.IntAccountId,
                        IntEmployeeId = x.IntEmployeeId,
                        StrEmployee = x.StrEmployee,
                        EmployeeCode = x.EmployeeCode,
                        StrDepartment = x.StrDepartment,
                        StrDesignation = x.StrDesignation,
                        IntEmploymentTypeId = x.IntEmploymentTypeId,
                        StrEmploymentType = x.StrEmploymentType,
                        IntPayRollMonth = x.IntPayRollMonth,
                        IntPayRollYear = x.IntPayRollYear,
                        IntCreatedBy = x.IntCreatedBy,
                        IsActive = x.IsActive,
                        CurrentStage = x.CurrentStage,
                        Application = x.Application
                    }).Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();

                    return retObj;
                }

                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "leave" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var listData = await list.ToListAsync();

                var listDataFilter = from obj in listData
                                     join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                                     let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                                     (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                                     || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                                     || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                                     where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                                     : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                                     : false)
                                     select new SalaryCertificateRequestLandingReturnViewModel
                                     {
                                         IntId = obj.IntId,
                                         IntAccountId = obj.IntAccountId,
                                         IntEmployeeId = obj.IntEmployeeId,
                                         StrEmployee = obj.StrEmployee,
                                         EmployeeCode = obj.EmployeeCode,
                                         StrDepartment = obj.StrDepartment,
                                         StrDesignation = obj.StrDesignation,
                                         IntEmploymentTypeId = obj.IntEmploymentTypeId,
                                         StrEmploymentType = obj.StrEmploymentType,
                                         IntPayRollMonth = obj.IntPayRollMonth,
                                         IntPayRollYear = obj.IntPayRollYear,
                                         IntCreatedBy = obj.IntCreatedBy,
                                         IsActive = obj.IsActive,
                                         CurrentStage = obj.CurrentStage,
                                         Application = obj.Application
                                     };
                retObj.TotalCount = listDataFilter.Count();
                retObj.Data = listDataFilter.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();

                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion =========== Salary Certificate Requisition ===========

        #region =========== Asset Requisition ===========

        public async Task<AssetRequisitionApprovalResponse> AssetRequisitionApprovalEngine(AssetRequisition application, bool isReject, long approverId, long accountId)
        {
            try
            {
                AssetRequisitionApprovalResponse response = new AssetRequisitionApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Asset Requisition";
                }
                else
                {
                    var res = await AssetRequisitionApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await AssetRequisitionApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await AssetRequisitionApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await AssetRequisitionApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationLandingVM> AssetRequisitionLandingEngine(AssetRequisitionLandingRequestVM model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();


                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "assetApproval" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var assetApplicationQuery = from obj in _context.AssetRequisitions
                                            join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                            join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                            join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                            from dept in dept2.DefaultIfEmpty()
                                            join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                            from desig in desig2.DefaultIfEmpty()
                                            where emp.IntAccountId == tokenData.accountId && obj.IsActive == true &&
                                            (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desig.StrDesignation.ToLower().Contains(model.SearchText)
                                             || dept.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) : true) &&
                                            (obj.DteReqisitionDate >= model.FromDate && obj.DteReqisitionDate <= model.ToDate)
                                            && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                            : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                            : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                            : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)
                                            select new
                                            {
                                                EmployeeName = emp.StrEmployeeName,
                                                EmployeeCode = emp.StrEmployeeCode,
                                                EmploymentType = emp.StrEmploymentType,
                                                BusinessUnitId = emp.IntBusinessUnitId,
                                                Department = dept != null ? dept.StrDepartment : "",
                                                Designation = desig != null ? desig.StrDesignation : "",
                                                Status = model.ApplicationStatus,
                                                CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                                SupervisorId = emp.IntSupervisorId,
                                                DottedSupervisorId = emp.IntDottedSupervisorId,
                                                LineManagerId = emp.IntLineManagerId,
                                                AssetRequisition = obj,
                                                CurrentStageObj = currentStage
                                            };
                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var leaveApplicationList1 = await assetApplicationQuery.Select(obj => new AssetRequisitionLandingResponseVM
                    {
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        EmploymentType = obj.EmploymentType,
                        BusinessUnitId = obj.BusinessUnitId,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        Status = obj.Status,
                        CurrentStage = obj.CurrentStage,
                        application = obj.AssetRequisition
                    }).ToListAsync();


                    retObj.TotalCount = leaveApplicationList1.Count();
                    retObj.Data = leaveApplicationList1.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }

                var assetApplicationList = await assetApplicationQuery.ToListAsync();


                var listData = from obj in assetApplicationList
                               join currentStage in globalPipelineRowList on obj.AssetRequisition.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.AssetRequisition.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.AssetRequisition.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.AssetRequisition.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.AssetRequisition.IsPipelineClosed == true && obj.AssetRequisition.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.AssetRequisition.IntCurrentStage == obj.AssetRequisition.IntNextStage))
                               : false)
                               select new AssetRequisitionLandingResponseVM
                               {
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   EmploymentType = obj.EmploymentType,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   Status = obj.Status,
                                   CurrentStage = obj.CurrentStage,
                                   application = obj.AssetRequisition
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion =========== Asset Requisition ===========

        #region =========== Asset Transfer ===========

        public async Task<AssetTransferApprovalResponse> AssetTransferApprovalEngine(AssetTransfer application, bool isReject, long approverId, long accountId)
        {
            try
            {
                AssetTransferApprovalResponse response = new AssetTransferApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Asset Requisition";
                }
                else
                {
                    var res = await AssetTransferApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await AssetTransferApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await AssetTransferApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await AssetTransferApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AssetTransferApprovalResponse> AssetTransferLandingEngine(AssetTransferLandingViewModel model)
        {
            try
            {
                AssetTransferApprovalResponse response = new AssetTransferApprovalResponse
                {
                    ResponseStatus = "",
                    ApplicationStatus = "",
                    CurrentSatageId = 0,
                    NextSatageId = 0,
                    IsComplete = true,
                    ListData = new List<AssetTransferLandingReturnViewModel>()
                };

                EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
                model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
                model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
                model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

                if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
                    && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
                {
                    response.ResponseStatus = "Invalid EmployeeId";
                }

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.AsNoTracking().AsQueryable().Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == "assetTransferApproval".ToLower());

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else
                {
                    List<AssetTransferLandingReturnViewModel> applicationList = new List<AssetTransferLandingReturnViewModel>();

                    if ((bool)model.IsAdmin)
                    {
                        model.IsSupOrLineManager = -1;
                        applicationList = await AssetTransferLandingEngine(model, header, null);
                        if (applicationList.Count() > 0)
                        {
                            response.ListData.AddRange(applicationList);
                            applicationList = new List<AssetTransferLandingReturnViewModel>();
                        }
                    }
                    else
                    {
                        if (model.IsSupervisor == true)
                        {
                            model.IsSupOrLineManager = 1;
                            applicationList = await AssetTransferLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<AssetTransferLandingReturnViewModel>();
                            }
                        }
                        if (model.IsLineManager == true)
                        {
                            model.IsSupOrLineManager = 2;
                            applicationList = await AssetTransferLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<AssetTransferLandingReturnViewModel>();
                            }
                        }
                        if (model.IsUserGroup == true)
                        {
                            model.IsSupOrLineManager = 3;
                            foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
                            {
                                applicationList = await AssetTransferLandingEngine(model, header, userGroup);
                                if (applicationList.Count() > 0)
                                {
                                    response.ListData.AddRange(applicationList);
                                    applicationList = new List<AssetTransferLandingReturnViewModel>();
                                }
                            }
                        }
                    }
                }
                response.ListData = response.ListData.OrderByDescending(x => x.Application.DteCreatedAt).ToList();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion =========== Asset Transfer ===========

        #region ===================== APPROVAL ENGINE ALL METHOD ========================

        #region -------------- Leave --------------------
        private async Task<(LeaveApprovalResponseVM, long)> LeaveApprovalEngine(LveLeaveApplication application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            LeaveApprovalResponseVM response = new LeaveApprovalResponseVM();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.LveLeaveApplications.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                _context.LveLeaveApplications.Update(application);
                await _context.SaveChangesAsync();

                if (application.IsPipelineClosed == true && application.IsReject == false)
                {
                    await LeaveBalanceAndAttendanceUpdateAfterLeaveApproved(application);
                }
            }

            return (response, application.IntApplicationId);
        }

        private async Task<bool> LeaveApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            LveLeaveApplication updatedApplication = await _context.LveLeaveApplications.FirstOrDefaultAsync(x => x.IntApplicationId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Leave --------------------

        #region -------------- Movement --------------------

        private async Task<List<MovementApplicationLandingReturnViewModel>> MovementLandingEngine(MovementApplicationLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<MovementApplicationLandingReturnViewModel> ListData = new List<MovementApplicationLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true)
                .AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.LveMovementApplications
                                  where obj.IsActive == true
                                  join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                  join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                                  join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                  from dept in dept2.DefaultIfEmpty()
                                  join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                  from desig in desig2.DefaultIfEmpty()
                                  join lvt1 in _context.LveMovementTypes on obj.IntMovementTypeId equals lvt1.IntMovementTypeId into lvt2
                                  from lvt in lvt2.DefaultIfEmpty()

                                  where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
                                  && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

                                  && (model.WorkplaceGroupId > 0 ? emp.IntWorkplaceId == model.WorkplaceGroupId : true)
                                  && (model.DepartmentId > 0 ? emp.IntDepartmentId == model.DepartmentId : true)
                                  && (model.DesignationId > 0 ? emp.IntDesignationId == model.DesignationId : true)
                                  && (model.ApplicantId > 0 ? emp.IntEmployeeBasicInfoId == model.DesignationId : true)
                                  && (model.MovementTypeId > 0 ? obj.IntMovementTypeId == model.MovementTypeId : true)
                                  where emp.IntAccountId == model.AccountId
                                  select new MovementApplicationLandingReturnViewModel
                                  {
                                      EmployeeName = emp.StrEmployeeName,
                                      EmployeeCode = emp.StrEmployeeCode,
                                      EmploymentType = emp.StrEmploymentType,
                                      BusinessUnitId = (long)emp.IntBusinessUnitId,
                                      ProfileUrlId = _context.EmpEmployeePhotoIdentities.Where(x => x.IsActive == true && x.IntEmployeeBasicInfoId == emp.IntEmployeeBasicInfoId).Select(x => x.IntProfilePicFileUrlId).FirstOrDefault(),
                                      Department = dept != null ? dept.StrDepartment : "",
                                      Designation = desig != null ? desig.StrDesignation : "",
                                      MovementType = lvt == null ? "" : lvt.StrMovementType,
                                      DateRange = obj.DteFromDate.ToString("dd-MMM-yyyy") + " to " + obj.DteToDate.ToString("dd-MMM-yyyy"),
                                      Status = model.ApplicationStatus,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                      MovementApplication = obj
                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }

            return ListData;
        }

        private async Task<(MovementApprovalResponse, long)> MovementApprovalEngine(LveMovementApplication application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            MovementApprovalResponse response = new MovementApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.LveMovementApplications.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;

                    TimeAttendanceDailySummary attendance = await _context.TimeAttendanceDailySummaries.FirstOrDefaultAsync(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == application.DteFromDate.Date);
                    if (attendance != null)
                    {
                        attendance.IsMovement = true;
                        _context.TimeAttendanceDailySummaries.Update(attendance);
                        await _context.SaveChangesAsync();
                    }
                }
                _context.LveMovementApplications.Update(application);
                await _context.SaveChangesAsync();

                if (application.IsPipelineClosed == true && application.IsReject == false)
                {
                    await AttendanceSummaryUpdateAfterMovementApproved(application);
                }
            }

            return (response, application.IntApplicationId);
        }

        private async Task<bool> MovementApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            LveMovementApplication updatedApplication = await _context.LveMovementApplications.FirstOrDefaultAsync(x => x.IntApplicationId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Movement --------------------

        #region -------------- Remote Attendance Location & Device Registration --------------------

        //private async Task<List<RemoteAttendanceLocationNDeviceLandingReturnViewModel>> RemoteAttendanceLocationNDeviceLandingEngine(RemoteAttendanceLocationNDeviceLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        //{
        //    GlobalPipelineRow approverStage = new GlobalPipelineRow();
        //    List<RemoteAttendanceLocationNDeviceLandingReturnViewModel> ListData = new List<RemoteAttendanceLocationNDeviceLandingReturnViewModel>();

        //    if (!(bool)model.IsAdmin)
        //    {
        //        approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
        //        .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : x.IntPipelineRowId == -0);
        //    }

        //    if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
        //    {
        //        ListData = await (from obj in _context.TimeRemoteAttendanceRegistrations
        //                          where obj.IsActive == true
        //                          //join msl in _context.MasterLocationRegisters on obj.IntMasterLocationId equals msl.IntMasterLocationId into msl2
        //                          //from mslocation in msl2.DefaultIfEmpty()
        //                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
        //                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

        //                          join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
        //                          from dept in dept2.DefaultIfEmpty()
        //                          join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
        //                          from desig in desig2.DefaultIfEmpty()

        //                          where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
        //                          && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
        //                          && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

        //                          && (model.WorkplaceGroupId > 0 ? emp.IntWorkplaceId == model.WorkplaceGroupId : true)
        //                          && (model.DepartmentId > 0 ? emp.IntDepartmentId == model.DepartmentId : true)
        //                          && (model.DesignationId > 0 ? emp.IntDesignationId == model.DesignationId : true)
        //                          && (model.ApplicantId > 0 ? emp.IntEmployeeBasicInfoId == model.DesignationId : true)
        //                          where emp.IntAccountId == model.AccountId
        //                          select new RemoteAttendanceLocationNDeviceLandingReturnViewModel
        //                          {
        //                              EmployeeName = emp.StrEmployeeName,
        //                              EmployeeCode = emp.StrEmployeeCode,
        //                              EmploymentType = emp.StrEmploymentType,
        //                              Department = dept != null ? dept.StrDepartment : "",
        //                              Designation = desig != null ? desig.StrDesignation : "",
        //                              Status = model.ApplicationStatus,
        //                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
        //                              Application = obj
        //                          }).AsNoTracking().AsQueryable().ToListAsync();
        //    }

        //    return ListData;
        //}

        private async Task<(RemoteAttendanceLocationNDeviceApprovalResponse, long)> RemoteAttendanceLocationNDeviceApprovalEngine(TimeRemoteAttendanceRegistration application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            RemoteAttendanceLocationNDeviceApprovalResponse response = new RemoteAttendanceLocationNDeviceApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.TimeRemoteAttendanceRegistrations.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.TimeRemoteAttendanceRegistrations.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntAttendanceRegId);
        }

        private async Task<bool> RemoteAttendanceLocationNDeviceApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            TimeRemoteAttendanceRegistration updatedApplication = await _context.TimeRemoteAttendanceRegistrations.FirstOrDefaultAsync(x => x.IntAttendanceRegId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Remote Attendance Location & Device Registration --------------------

        #region -------------- Remote Attendance --------------------

        private async Task<List<RemoteAttendanceLandingReturnViewModel>> RemoteAttendanceLandingEngine(RemoteAttendanceLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow, bool IsMarket)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<RemoteAttendanceLandingReturnViewModel> ListData = new List<RemoteAttendanceLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true)
                .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = (from obj in _context.TimeEmployeeAttendances
                            join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                            join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                            join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
                            from dept in dept2.DefaultIfEmpty()
                            join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                            from desig in desig2.DefaultIfEmpty()

                            where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
                            && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                            && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

                            && (model.WorkplaceGroupId > 0 ? emp.IntWorkplaceId == model.WorkplaceGroupId : true)
                            && (model.DepartmentId > 0 ? emp.IntDepartmentId == model.DepartmentId : true)
                            && (model.DesignationId > 0 ? emp.IntDesignationId == model.DesignationId : true)
                            && (model.ApplicantId > 0 ? emp.IntEmployeeBasicInfoId == model.DesignationId : true)
                            where emp.IntAccountId == model.AccountId && obj.IsMarket == IsMarket
                            select new RemoteAttendanceLandingReturnViewModel
                            {
                                EmployeeName = emp.StrEmployeeName,
                                EmployeeCode = emp.StrEmployeeCode,
                                EmploymentType = emp.StrEmploymentType,
                                Department = dept != null ? dept.StrDepartment : "",
                                Designation = desig != null ? desig.StrDesignation : "",
                                Status = model.ApplicationStatus,
                                CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                Application = obj
                            }).ToList();
            }

            return ListData;
        }

        private async Task<(RemoteAttendanceApprovalResponse, long)> RemoteAttendanceApprovalEngine(TimeEmployeeAttendance application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            RemoteAttendanceApprovalResponse response = new RemoteAttendanceApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.TimeEmployeeAttendances.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.TimeEmployeeAttendances.Update(application);
                await _context.SaveChangesAsync();

                if (application.IsPipelineClosed == true && application.IsReject == false)
                {
                    await AttendanceSummaryUpdateAfterRemoteAttendanceApproved(application);
                }
            }

            return (response, application.IntAutoIncrement);
        }

        private async Task<bool> RemoteAttendanceApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            TimeEmployeeAttendance updatedApplication = await _context.TimeEmployeeAttendances.FirstOrDefaultAsync(x => x.IntAutoIncrement == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Remote Attendance --------------------

        #region -------------- Salary Addition and Deduction --------------------

        //private async Task<List<SalaryAdditionNDeductionLandingReturnViewModel>> SalaryAdditionNDeductionLandingEngine(SalaryAdditionNDeductionLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        //{
        //    GlobalPipelineRow approverStage = new GlobalPipelineRow();
        //    List<SalaryAdditionNDeductionLandingReturnViewModel> ListData = new List<SalaryAdditionNDeductionLandingReturnViewModel>();

        //    if (!(bool)model.IsAdmin)
        //    {
        //        approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
        //        .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : x.IntPipelineRowId == -0);
        //    }

        //    if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
        //    {
        //        ListData = await (from obj in _context.PyrEmpSalaryAdditionNdeductions
        //                          where obj.IsActive == true
        //                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
        //                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

        //                          join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dept2
        //                          from dept in dept2.DefaultIfEmpty()
        //                          join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
        //                          from desig in desig2.DefaultIfEmpty()

        //                          where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
        //                          && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
        //                          && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

        //                          && (model.WorkplaceGroupId > 0 ? emp.IntWorkplaceId == model.WorkplaceGroupId : true)
        //                          && (model.DepartmentId > 0 ? emp.IntDepartmentId == model.DepartmentId : true)
        //                          && (model.DesignationId > 0 ? emp.IntDesignationId == model.DesignationId : true)
        //                          && (model.ApplicantId > 0 ? emp.IntEmployeeBasicInfoId == model.DesignationId : true)
        //                          where emp.IntAccountId == model.AccountId
        //                          select new SalaryAdditionNDeductionLandingReturnViewModel
        //                          {
        //                              EmployeeName = emp.StrEmployeeName,
        //                              EmployeeCode = emp.StrEmployeeCode,
        //                              EmploymentType = emp.StrEmploymentType,
        //                              Department = dept != null ? dept.StrDepartment : "",
        //                              Designation = desig != null ? desig.StrDesignation : "",
        //                              Status = model.ApplicationStatus,
        //                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
        //                              Application = obj
        //                          }).AsNoTracking().AsQueryable().ToListAsync();
        //    }

        //    return ListData;
        //}

        private async Task<(SalaryAdditionNDeductionApprovalResponse, long)> SalaryAdditionNDeductionApprovalEngine(PyrEmpSalaryAdditionNdeduction application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            SalaryAdditionNDeductionApprovalResponse response = new SalaryAdditionNDeductionApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrEmpSalaryAdditionNdeductions.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.PyrEmpSalaryAdditionNdeductions.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntSalaryAdditionAndDeductionId);
        }

        private async Task<bool> SalaryAdditionNDeductionApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrEmpSalaryAdditionNdeduction updatedApplication = await _context.PyrEmpSalaryAdditionNdeductions.FirstOrDefaultAsync(x => x.IntSalaryAdditionAndDeductionId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Salary Addition and Deduction --------------------

        #region ------------- IOU -------------------

        private async Task<(IOUApprovalResponse, long)> IOUApplicationApprovalEngine(PyrIouapplication application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            IOUApprovalResponse response = new IOUApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrIouapplications.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.PyrIouapplications.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntIouid);
        }

        private async Task<bool> IOUApplicationApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrIouapplication updatedApplication = await _context.PyrIouapplications.FirstOrDefaultAsync(x => x.IntIouid == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ------------- IOU -------------------

        #region ------------- IOU Adjustment -------------------

        private async Task<(IOUAdjustmentApprovalResponse, long)> IOUAdjustmentApprovalEngine(PyrIouadjustmentHistory application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            IOUAdjustmentApprovalResponse response = new IOUAdjustmentApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrIouadjustmentHistories.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.PyrIouadjustmentHistories.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntIouadjustmentId);
        }

        private async Task<bool> IOUAdjustmentApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrIouadjustmentHistory updatedApplication = await _context.PyrIouadjustmentHistories.FirstOrDefaultAsync(x => x.IntIouadjustmentId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ------------- IOU Adjustment -------------------

        #region ------------- Salary Generate Request --------

        private async Task<(SalaryGenerateRequestApprovalResponse, long)> SalaryGenerateRequestApprovalEngine(PyrPayrollSalaryGenerateRequest application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            SalaryGenerateRequestApprovalResponse response = new SalaryGenerateRequestApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrPayrollSalaryGenerateRequests.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.PyrPayrollSalaryGenerateRequests.Update(application);
                await _context.SaveChangesAsync();


                if (application.IntWorkplaceGroupId == 3 && application.IntSoleDepoId > 0 && Convert.ToInt64(application.IntAreaId) <= 0
                    && application.IsPipelineClosed == true && (application.IsReject == false || application.IsReject == true))
                {
                    List<PyrPayrollSalaryGenerateRequest> reqList = await _context.PyrPayrollSalaryGenerateRequests
                        .Where(x => x.IsPipelineClosed == true && x.IsReject == false && x.IntSoleDepoId == application.IntSoleDepoId
                        && (long)x.IntAreaId > 0 && application.IntYear == x.IntYear && application.IntMonth == x.IntMonth
                        && x.IsActive == true).ToListAsync();

                    reqList.ForEach(app =>
                    {
                        if (application.IsReject == false)
                        {
                            app.IsApprovedByHeadOffice = true;
                            app.IsRejectedByHeadOffice = false;
                        }
                        else
                        {
                            app.IsApprovedByHeadOffice = false;
                            app.IsRejectedByHeadOffice = true;
                        }
                    });

                    _context.PyrPayrollSalaryGenerateRequests.UpdateRange(reqList);
                    await _context.SaveChangesAsync();
                }
            }

            return (response, application.IntSalaryGenerateRequestId);
        }

        private async Task<bool> PayrollSalaryGenerateRequestApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrPayrollSalaryGenerateRequest updatedApplication = await _context.PyrPayrollSalaryGenerateRequests.FirstOrDefaultAsync(x => x.IntSalaryGenerateRequestId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ------------- Salary Generate Request --------

        #region ----------- Loan --------------

        private async Task<(LoanApprovalResponse, long)> LoanApprovalEngine(EmpLoanApplication application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            LoanApprovalResponse response = new LoanApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpLoanApplications.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                if ((application.IntApproveLoanAmount == null || application.IntApproveLoanAmount <= 0)
                    || (application.IntApproveNumberOfInstallment == null || application.IntApproveNumberOfInstallment <= 0)
                    || (application.IntApproveNumberOfInstallmentAmount == null || application.IntApproveNumberOfInstallmentAmount <= 0))
                {
                    application.IntApproveLoanAmount = application.IntLoanAmount;
                    application.IntApproveNumberOfInstallment = application.IntNumberOfInstallment;
                    application.IntApproveNumberOfInstallmentAmount = application.IntNumberOfInstallmentAmount;
                    application.NumRemainingBalance = application.IntLoanAmount;
                }

                _context.EmpLoanApplications.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntLoanApplicationId);
        }

        private async Task<bool> EmpLoanApplicationApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpLoanApplication updatedApplication = await _context.EmpLoanApplications.FirstOrDefaultAsync(x => x.IntLoanApplicationId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Loan --------------

        #region ----------- Over Time -----------

        private async Task<(OverTimeApprovalResponse, long)> OverTimeApprovalEngine(TimeEmpOverTime application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            OverTimeApprovalResponse response = new OverTimeApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.TimeEmpOverTimes.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.TimeEmpOverTimes.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntOverTimeId);
        }

        private async Task<bool> OverTimeApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            TimeEmpOverTime updatedApplication = await _context.TimeEmpOverTimes.FirstOrDefaultAsync(x => x.IntOverTimeId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Over Time -----------

        #region ----------- Manual Attendance Summary -----------

        private async Task<List<ManualAttendanceSummaryLandingReturnViewModel>> ManualAttendanceLandingEngine(ManualAttendanceSummaryLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<ManualAttendanceSummaryLandingReturnViewModel> ListData = new List<ManualAttendanceSummaryLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true)
                .AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.EmpManualAttendanceSummaries
                                  where obj.IsActive == true
                                  join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                  join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                  join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
                                  from depart in dp.DefaultIfEmpty()
                                  join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
                                  from designat in desig.DefaultIfEmpty()
                                  join timeAtt2 in _context.TimeAttendanceDailySummaries on obj.IntEmployeeId equals timeAtt2.IntEmployeeId into timeAtt1
                                  from timeAtt in timeAtt1.DefaultIfEmpty()
                                  where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
                                  && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)
                                  where emp.IntAccountId == model.AccountId && timeAtt.DteAttendanceDate.Value.Date == obj.DteAttendanceDate.Value.Date//&& emp.IntBusinessUnitId == model.BusinessUnitId
                                  select new ManualAttendanceSummaryLandingReturnViewModel
                                  {
                                      IntId = obj.IntId,
                                      IntEmployeeId = obj.IntEmployeeId,
                                      StrEmployeeName = emp.StrEmployeeName,
                                      EmployeeCode = emp.StrEmployeeCode,
                                      StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
                                      IntDepartmentId = emp.IntDepartmentId,
                                      StrDepartment = depart != null ? depart.StrDepartment : "",
                                      IntDesignationId = emp.IntDesignationId,
                                      StrDesignation = designat != null ? designat.StrDesignation : "",
                                      IntAttendanceSummaryId = obj.IntAttendanceSummaryId,
                                      DteAttendanceDate = obj.DteAttendanceDate,
                                      TimeInTime = timeAtt.TmeInTime != null ? new DateTime().Add(timeAtt.TmeInTime.Value).ToString("hh:mm tt") : null,
                                      TimeOutTime = timeAtt.TmeLastOutTime != null ? new DateTime().Add(timeAtt.TmeLastOutTime.Value).ToString("hh:mm tt") : null,
                                      StrCurrentStatus = obj.StrCurrentStatus,
                                      StrRequestStatus = obj.StrRequestStatus,
                                      StrRemarks = obj.StrRemarks,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,

                                      Application = obj,

                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }

            return ListData;
        }

        private async Task<(ManualAttendanceSummaryApprovalResponse, long)> ManualAttendanceApprovalEngine(EmpManualAttendanceSummary application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            ManualAttendanceSummaryApprovalResponse response = new ManualAttendanceSummaryApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpManualAttendanceSummaries.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpManualAttendanceSummaries.Update(application);
                await _context.SaveChangesAsync();

                if (application.IsPipelineClosed == true && application.IsReject == false)
                {
                    TimeAttendanceDailySummary attendance = await _context.TimeAttendanceDailySummaries.Where(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == application.DteAttendanceDate.Value.Date).FirstOrDefaultAsync();

                    attendance.IsManual = true;
                    attendance.IsManualPresent = application.StrRequestStatus.ToLower() == "Present".ToLower() ? true : false;
                    attendance.IsManualLate = application.StrRequestStatus.ToLower() == "Late".ToLower() ? true : false;
                    attendance.IsManualAbsent = application.StrRequestStatus.ToLower() == "Absent".ToLower() ? true : false;
                    attendance.IntManualAttendanceBy = approverId;
                    attendance.DteManualAttendanceDate = DateTime.Now;

                    _context.TimeAttendanceDailySummaries.Update(attendance);
                    await _context.SaveChangesAsync();
                }
            }

            return (response, application.IntId);
        }

        private async Task<bool> ManualAttendanceApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpManualAttendanceSummary updatedApplication = await _context.EmpManualAttendanceSummaries.FirstOrDefaultAsync(x => x.IntId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Manual Attendance Summary -----------

        #region ----------- Employee Separation -----------

        private async Task<(EmployeeSeparationApprovalResponse, long)> EmployeeSeparationApprovalEngine(EmpEmployeeSeparation application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            EmployeeSeparationApprovalResponse response = new EmployeeSeparationApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpEmployeeSeparations.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpEmployeeSeparations.Update(application);
                await _context.SaveChangesAsync();

                //if (application.IsPipelineClosed == true && application.IsReject == false)
                //{
                    //MessageHelper message = await _employeeService.EmployeeInactive(application.IntEmployeeId);

                    //EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == application.IntEmployeeId && x.IsActive == true);
                    //EmpEmployeeBasicInfoDetail details = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == application.IntEmployeeId && x.IsActive == true);
                    //User user = await _context.Users.FirstOrDefaultAsync(x => (long)x.IntRefferenceId == application.IntEmployeeId && x.IsActive == true);

                    //if(emp is not null)
                    //{
                    //    emp.IsActive = false;
                    //    _context.EmpEmployeeBasicInfos.Update(emp);
                    //}

                    //if(details is not null)
                    //{
                    //    details.IntEmployeeStatusId = 2;
                    //    details.StrEmployeeStatus = "Inactive";

                    //    _context.EmpEmployeeBasicInfoDetails.Update(details);
                    //}

                    //if(user is not null)
                    //{
                    //    user.IsActive = false;
                    //    _context.Users.Update(user);
                    //}

                    //if(user is not null || emp is not null || details is not null)
                    //{
                    //    await _context.SaveChangesAsync();
                    //}
                //}
            }

            return (response, application.IntSeparationId);
        }

        private async Task<bool> EmployeeSeparationApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpEmployeeSeparation updatedApplication = await _context.EmpEmployeeSeparations.FirstOrDefaultAsync(x => x.IntSeparationId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Employee Separation -----------

        #region ----------- Transfer & Promotion -----------

        private async Task<(TransferNPromotionApprovalResponse, long)> TransferNPromotionApprovalEngine(EmpTransferNpromotion application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            TransferNPromotionApprovalResponse response = new TransferNPromotionApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpTransferNpromotions.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpTransferNpromotions.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntTransferNpromotionId);
        }

        private async Task<bool> TransferNPromotionApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpTransferNpromotion updatedApplication = await _context.EmpTransferNpromotions.FirstOrDefaultAsync(x => x.IntTransferNpromotionId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Transfer & Promotion -----------

        #region ----------- Employee Increment -----------

        //private async Task<List<EmployeeIncrementLandingReturnViewModel>> EmployeeIncrementLandingEngine(EmployeeIncrementLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        //{
        //    GlobalPipelineRow approverStage = new GlobalPipelineRow();
        //    List<EmployeeIncrementLandingReturnViewModel> ListData = new List<EmployeeIncrementLandingReturnViewModel>();

        //    if (!(bool)model.IsAdmin)
        //    {
        //        approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
        //        .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : x.IntPipelineRowId == -0);
        //    }

        //    if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
        //    {
        //        ListData = await (from obj in _context.EmpEmployeeIncrements
        //                          where obj.IsActive == true
        //                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
        //                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId
        //                          join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dp
        //                          from depart in dp.DefaultIfEmpty()
        //                          join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desig
        //                          from designat in desig.DefaultIfEmpty()
        //                          where (model.IsSupOrLineManager == 1 ? emp.IntSupervisorId == model.ApproverId || emp.IntDottedSupervisorId == model.ApproverId : model.IsSupOrLineManager == 2 ? emp.IntLineManagerId == model.ApproverId : true)
        //                          && ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
        //                          && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

        //                          where emp.IntAccountId == model.AccountId
        //                          select new EmployeeIncrementLandingReturnViewModel
        //                          {
        //                              IntId = obj.IntIncrementId,
        //                              IntEmployeeId = obj.IntEmployeeId,
        //                              StrEmployeeName = emp.StrEmployeeName,
        //                              EmployeeCode = emp.StrEmployeeCode,
        //                              StrEmploymentType = emp != null ? emp.StrEmploymentType : "",
        //                              IntDepartmentId = emp.IntDepartmentId,
        //                              StrDepartment = depart != null ? depart.StrDepartment : "",
        //                              IntDesignationId = emp.IntDesignationId,
        //                              StrDesignation = designat != null ? designat.StrDesignation : "",
        //                              IntDesignationTypeId = obj.IntDesignationId,
        //                              IntAccountId = obj.IntAccountId,
        //                              IntBusinessUnitId = obj.IntBusinessUnitId,
        //                              StrIncrementDependOn = obj.StrIncrementDependOn,
        //                              NumIncrementDependOn = obj.NumIncrementDependOn,
        //                              NumIncrementPercentage = obj.NumIncrementPercentage,
        //                              NumIncrementAmount = obj.NumIncrementAmount,
        //                              DteEffectiveDate = obj.DteEffectiveDate,
        //                              IntTransferNpromotionReferenceId = obj.IntTransferNpromotionReferenceId,
        //                              IsActive = obj.IsActive,
        //                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
        //                              Application = obj
        //                          }).AsNoTracking().AsQueryable().ToListAsync();
        //    }
        //    return ListData;
        //}

        private async Task<(EmployeeIncrementApprovalResponse, long)> EmployeeIncrementApprovalEngine(EmpEmployeeIncrement application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            EmployeeIncrementApprovalResponse response = new EmployeeIncrementApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpEmployeeIncrements.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpEmployeeIncrements.Update(application);
                await _context.SaveChangesAsync();

                //if (application.IsPipelineClosed == true && application.IsReject == false)
                //{
                //    EmpEmployeeSeparation attendance = await _context.EmpEmployeeSeparations.Where(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == application.DteAttendanceDate.Value.Date).FirstOrDefaultAsync();
                //    attendance.IsPresent = true;
                //    attendance.IsLate = false;

                //    _context.TimeAttendanceDailySummaries.Update(attendance);
                //    await _context.SaveChangesAsync();
                //}
            }

            return (response, application.IntIncrementId);
        }

        private async Task<bool> EmployeeIncrementApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpEmployeeIncrement updatedApplication = await _context.EmpEmployeeIncrements.FirstOrDefaultAsync(x => x.IntIncrementId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Employee Increment -----------

        #region ----------- Bonus Generate -----------

        private async Task<(BonusGenerateHeaderApprovalResponse, long)> BonusGenerateHeaderApprovalEngine(PyrBonusGenerateHeader application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            BonusGenerateHeaderApprovalResponse response = new BonusGenerateHeaderApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrBonusGenerateHeaders.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.PyrBonusGenerateHeaders.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntBonusHeaderId);
        }

        private async Task<bool> BonusGenerateHeaderApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrBonusGenerateHeader updatedApplication = await _context.PyrBonusGenerateHeaders.FirstOrDefaultAsync(x => x.IntBonusHeaderId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- Bonus Generate -----------

        #region ----------- PF Withdraw -----------

        //private async Task<List<PFWithdrawLandingReturnViewModel>> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        //{
        //    GlobalPipelineRow approverStage = new GlobalPipelineRow();
        //    List<PFWithdrawLandingReturnViewModel> ListData = new List<PFWithdrawLandingReturnViewModel>();

        //    if (!(bool)model.IsAdmin)
        //    {
        //        approverStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true)
        //        .AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : x.IntPipelineRowId == -0);
        //    }

        //    if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
        //    {
        //        ListData = await (from obj in _context.EmpPfwithdraws
        //                          where obj.IsActive == true
        //                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
        //                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
        //                          from empi in emp1.DefaultIfEmpty()
        //                          join dep1 in _context.MasterDepartments on empi.IntDepartmentId equals dep1.IntDepartmentId into dep2
        //                          from dep in dep2.DefaultIfEmpty()
        //                          join deg1 in _context.MasterDesignations on empi.IntDesignationId equals deg1.IntDesignationId into deg2
        //                          from desg in deg2.DefaultIfEmpty()

        //                          where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
        //                          && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

        //                          //&& (model.BusinessUnitId > 0 ? obj.IntBusinessUnitId == model.BusinessUnitId : true)
        //                          //&& (model.WorkplaceId > 0 ? obj.IntWorkplaceId == model.WorkplaceId : true)
        //                          //&& (model.WorkplaceGroupId > 0 ? obj.IntWorkplaceGroupId == model.WorkplaceGroupId : true)
        //                          where obj.IsActive == true && obj.IntAccountId == model.AccountId
        //                          && dep.IsActive == true && desg.IsActive == true
        //                          select new PFWithdrawLandingReturnViewModel
        //                          {
        //                              IntId = obj.IntPfwithdrawId,
        //                              IntAccountId = obj.IntAccountId,
        //                              IntEmployeeId = obj.IntEmployeeId,
        //                              StrEmployee = obj.StrEmployee,
        //                              EmployeeCode = empi.StrEmployeeCode,
        //                              StrDepartment = dep.StrDepartment,
        //                              StrDesignation = desg.StrDesignation,
        //                              DteApplicationDate = obj.DteApplicationDate,
        //                              NumWithdrawAmount = obj.NumWithdrawAmount,
        //                              StrReason = obj.StrReason,
        //                              IntCreatedBy = obj.IntCreatedBy,
        //                              IsActive = obj.IsActive,
        //                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
        //                              Application = obj
        //                          }).AsNoTracking().AsQueryable().ToListAsync();
        //    }
        //    return ListData;
        //}

        private async Task<(PFWithdrawApprovalResponse, long)> PFWithdrawApprovalEngine(EmpPfwithdraw application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            PFWithdrawApprovalResponse response = new PFWithdrawApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpPfwithdraws.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpPfwithdraws.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntPfwithdrawId);
        }

        private async Task<bool> PFWithdrawApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpPfwithdraw updatedApplication = await _context.EmpPfwithdraws.FirstOrDefaultAsync(x => x.IntPfwithdrawId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ----------- PF Withdraw -----------

        #region ------------- Arear Salary Generate Request --------

        private async Task<(ArearSalaryGenerateRequestApprovalResponse, long)> ArearSalaryGenerateRequestApprovalEngine(PyrArearSalaryGenerateRequest application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            ArearSalaryGenerateRequestApprovalResponse response = new ArearSalaryGenerateRequestApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.PyrArearSalaryGenerateRequests.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                _context.PyrArearSalaryGenerateRequests.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntArearSalaryGenerateRequestId);
        }

        private async Task<bool> ArearSalaryGenerateRequestApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            PyrArearSalaryGenerateRequest updatedApplication = await _context.PyrArearSalaryGenerateRequests.FirstOrDefaultAsync(x => x.IntArearSalaryGenerateRequestId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion ------------- Salary Generate Request --------

        #region -------------- Expense --------------------

        private async Task<(ExpenseApprovalResponse, long)> ExpenseApprovalEngine(EmpExpenseApplication application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            ExpenseApprovalResponse response = new ExpenseApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpExpenseApplications.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                _context.EmpExpenseApplications.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntExpenseId);
        }

        private async Task<bool> ExpenseApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpExpenseApplication updatedApplication = await _context.EmpExpenseApplications.FirstOrDefaultAsync(x => x.IntExpenseId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Expense --------------------

        #region -------------- Salary Certificate Approval --------------------

        //private async Task<List<SalaryCertificateRequestLandingReturnViewModel>> SalaryCertificateRequestLandingEngine(SalaryCertificateRequestLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        //{
        //    GlobalPipelineRow approverStage = new GlobalPipelineRow();
        //    List<SalaryCertificateRequestLandingReturnViewModel> ListData = new List<SalaryCertificateRequestLandingReturnViewModel>();

        //    if (!(bool)model.IsAdmin)
        //    {
        //        approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
        //        .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
        //        : x.IntPipelineRowId == -0);
        //    }

        //    if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
        //    {
        //        ListData = await (from obj in _context.EmpSalaryCertificateRequests
        //                          where obj.IsActive == true
        //                          join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
        //                          join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
        //                          from empi in emp1.DefaultIfEmpty()
        //                          join dep1 in _context.MasterDepartments on empi.IntDepartmentId equals dep1.IntDepartmentId into dep2
        //                          from dep in dep2.DefaultIfEmpty()
        //                          join deg1 in _context.MasterDesignations on empi.IntDesignationId equals deg1.IntDesignationId into deg2
        //                          from desg in deg2.DefaultIfEmpty()

        //                          where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
        //                          && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)

        //                          //&& (model.BusinessUnitId > 0 ? obj.IntBusinessUnitId == model.BusinessUnitId : true)
        //                          //&& (model.WorkplaceId > 0 ? obj.IntWorkplaceId == model.WorkplaceId : true)
        //                          //&& (model.WorkplaceGroupId > 0 ? obj.IntWorkplaceGroupId == model.WorkplaceGroupId : true)
        //                          where obj.IsActive == true && obj.IntAccountId == model.AccountId
        //                          && dep.IsActive == true && desg.IsActive == true
        //                          select new SalaryCertificateRequestLandingReturnViewModel
        //                          {
        //                              IntId = obj.IntSalaryCertificateRequestId,
        //                              IntAccountId = obj.IntAccountId,
        //                              IntEmployeeId = obj.IntEmployeeId,
        //                              StrEmployee = empi.StrEmployeeName,
        //                              EmployeeCode = empi.StrEmployeeCode,
        //                              StrDepartment = dep.StrDepartment,
        //                              StrDesignation = desg.StrDesignation,
        //                              IntEmploymentTypeId = empi.IntEmploymentTypeId,
        //                              StrEmploymentType = empi.StrEmploymentType,
        //                              IntPayRollMonth = obj.IntPayRollMonth,
        //                              IntPayRollYear = obj.IntPayRollYear,
        //                              IntCreatedBy = obj.IntCreatedBy,
        //                              IsActive = obj.IsActive,
        //                              CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
        //                              Application = obj
        //                          }).AsNoTracking().AsQueryable().ToListAsync();
        //    }
        //    return ListData;
        //}

        private async Task<(SalaryCertificateRequestApprovalResponse, long)> SalaryCertificateRequestApprovalEngine(EmpSalaryCertificateRequest application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            SalaryCertificateRequestApprovalResponse response = new SalaryCertificateRequestApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.EmpSalaryCertificateRequests.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.EmpSalaryCertificateRequests.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntSalaryCertificateRequestId);
        }

        private async Task<bool> SalaryCertificateRequestApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            EmpSalaryCertificateRequest updatedApplication = await _context.EmpSalaryCertificateRequests.FirstOrDefaultAsync(x => x.IntSalaryCertificateRequestId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Salary Certificate Approval --------------------

        #region -------------- Asset Requisition Approval --------------------

        private async Task<List<AssetRequisitionLandingReturnViewModel>> AssetRequisitionLandingEngine(AssetRequisitionLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<AssetRequisitionLandingReturnViewModel> ListData = new List<AssetRequisitionLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
                .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.AssetRequisitions
                                  where obj.IsActive == true
                                  join i in _context.Items on obj.IntItemId equals i.IntItemId
                                  join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                  join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                                  from empi in emp1.DefaultIfEmpty()
                                  join dep1 in _context.MasterDepartments on empi.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                  from dep in dep2.DefaultIfEmpty()
                                  join deg1 in _context.MasterDesignations on empi.IntDesignationId equals deg1.IntDesignationId into deg2
                                  from desg in deg2.DefaultIfEmpty()

                                  where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)
                                  where obj.IsActive == true && obj.IntAccountId == model.AccountId
                                  && dep.IsActive == true && desg.IsActive == true && obj.IsDenied == false
                                  select new AssetRequisitionLandingReturnViewModel
                                  {
                                      AssetRequisitionId = obj.IntAssetRequisitionId,
                                      AccountId = obj.IntAccountId,
                                      BusinessUnitId = obj.IntBusinessUnitId,
                                      ItemId = obj.IntItemId,
                                      ItemName = i.StrItemName,
                                      ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                      EmployeeId = obj.IntEmployeeId,
                                      EmployeeName = empi.StrEmployeeName,
                                      EmployeeCode = empi.StrEmployeeCode,
                                      Department = dep.StrDepartment,
                                      Designation = desg.StrDesignation,
                                      EmploymentType = empi.StrEmploymentType,
                                      ReqisitionQuantity = obj.IntReqisitionQuantity,
                                      ReqisitionDate = obj.DteReqisitionDate,
                                      Remarks = obj.StrRemarks,
                                      IsActive = obj.IsActive,
                                      CreatedAt = obj.DteCreatedAt,
                                      CreatedBy = obj.IntCreatedBy,
                                      UpdatedAt = obj.DteUpdatedAt,
                                      UpdatedBy = obj.IntUpdatedBy,
                                      PipelineHeaderId = obj.IntPipelineHeaderId,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                      NextStage = obj.IntNextStage,
                                      Status = obj.StrStatus,
                                      IsPipelineClosed = obj.IsPipelineClosed,
                                      IsReject = obj.IsReject,
                                      RejectDateTime = obj.DteRejectDateTime,
                                      RejectedBy = obj.IntRejectedBy,
                                      IsDenied = obj.IsDenied,
                                      Application = obj
                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }
            return ListData;
        }

        private async Task<(AssetRequisitionApprovalResponse, long)> AssetRequisitionApprovalEngine(AssetRequisition application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            AssetRequisitionApprovalResponse response = new AssetRequisitionApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.AssetRequisitions.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.AssetRequisitions.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntAssetRequisitionId);
        }

        private async Task<bool> AssetRequisitionApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            AssetRequisition updatedApplication = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Asset Requisition Approval --------------------

        #region -------------- Asset Transfer Approval --------------------

        private async Task<List<AssetTransferLandingReturnViewModel>> AssetTransferLandingEngine(AssetTransferLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<AssetTransferLandingReturnViewModel> ListData = new List<AssetTransferLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
                .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.AssetTransfers
                                  where obj.IsActive == true
                                  join i in _context.Items on obj.IntItemId equals i.IntItemId
                                  join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                  join emp in _context.EmpEmployeeBasicInfos on obj.IntFromEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                                  from empi in emp1.DefaultIfEmpty()
                                  join dep1 in _context.MasterDepartments on empi.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                  from dep in dep2.DefaultIfEmpty()
                                  join deg1 in _context.MasterDesignations on empi.IntDesignationId equals deg1.IntDesignationId into deg2
                                  from desg in deg2.DefaultIfEmpty()

                                  where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)
                                  where obj.IsActive == true && obj.IntAccountId == model.AccountId
                                  && dep.IsActive == true && desg.IsActive == true
                                  select new AssetTransferLandingReturnViewModel
                                  {
                                      AssetTransferId = obj.IntAssetTransferId,
                                      AccountId = obj.IntAccountId,
                                      BusinessUnitId = obj.IntBusinessUnitId,
                                      ItemId = obj.IntItemId,
                                      ItemName = i.StrItemName,
                                      ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                      FromEmployeeId = obj.IntFromEmployeeId,
                                      FromEmployeeName = empi.StrEmployeeName,
                                      EmployeeCode = empi.StrEmployeeCode,
                                      ToEmployeeId = obj.IntToEmployeeId,
                                      ToEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == obj.IntToEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                      Department = dep.StrDepartment,
                                      Designation = desg.StrDesignation,
                                      EmploymentType = empi.StrEmploymentType,
                                      TransferQuantity = obj.IntTransferQuantity,
                                      TransferDate = obj.DteTransferDate,
                                      Remarks = obj.StrRemarks,
                                      IsActive = obj.IsActive,
                                      CreatedAt = obj.DteCreatedAt,
                                      CreatedBy = obj.IntCreatedBy,
                                      UpdatedAt = obj.DteUpdatedAt,
                                      UpdatedBy = obj.IntUpdatedBy,
                                      PipelineHeaderId = obj.IntPipelineHeaderId,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                      NextStage = obj.IntNextStage,
                                      Status = obj.StrStatus,
                                      IsPipelineClosed = obj.IsPipelineClosed,
                                      IsReject = obj.IsReject,
                                      RejectDateTime = obj.DteRejectDateTime,
                                      RejectedBy = obj.IntRejectedBy,
                                      Application = obj
                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }
            return ListData;
        }

        private async Task<(AssetTransferApprovalResponse, long)> AssetTransferApprovalEngine(AssetTransfer application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            AssetTransferApprovalResponse response = new AssetTransferApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.AssetTransfers.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.AssetTransfers.Update(application);
                await _context.SaveChangesAsync();

                AssetTransfer assetTransfer = await _context.AssetTransfers.Where(x => x.IntAssetTransferId == application.IntAssetTransferId && x.IsPipelineClosed == true
                                                                                                    && x.IsReject == false && x.IsActive == true).FirstOrDefaultAsync();

                if (assetTransfer != null)
                {
                    List<AssetRequisition> assetRequisition = await _context.AssetRequisitions.Where(a => a.IsActive == true
                                                                                       && a.IntEmployeeId == assetTransfer.IntFromEmployeeId
                                                                                       && a.IsPipelineClosed == true && a.IsReject == false
                                                                                       && a.IntItemId == assetTransfer.IntItemId
                                                                                       && a.IntReqisitionQuantity > 0).OrderBy(a => a.IntReqisitionQuantity).ToListAsync();

                    long remainingTransferQuantity = (long)assetTransfer.IntTransferQuantity;

                    foreach (var itemAsset in assetRequisition)
                    {
                        long decrementQuantity = Math.Min(remainingTransferQuantity, itemAsset.IntReqisitionQuantity);
                        itemAsset.IntReqisitionQuantity -= decrementQuantity;
                        remainingTransferQuantity -= decrementQuantity;

                        _context.AssetRequisitions.Update(itemAsset);
                        await _context.SaveChangesAsync();

                        if (remainingTransferQuantity == 0)
                        {
                            break;
                        }
                    }

                    if (remainingTransferQuantity > 0)
                    {
                        List<AssetDirectAssign> assetDirectAssign = await _context.AssetDirectAssigns.Where(a => a.IsActive == true
                                                                                           && a.IntEmployeeId == assetTransfer.IntFromEmployeeId
                                                                                           && a.IntItemId == assetTransfer.IntItemId
                                                                                           && a.IntItemQuantity > 0).OrderBy(a => a.IntItemQuantity).ToListAsync();

                        if (assetDirectAssign != null)
                        {
                            foreach (var itemDirect in assetDirectAssign)
                            {

                                long decrementQuantity = Math.Min(remainingTransferQuantity, itemDirect.IntItemQuantity);
                                itemDirect.IntItemQuantity -= decrementQuantity;
                                remainingTransferQuantity -= decrementQuantity;

                                _context.AssetDirectAssigns.Update(itemDirect);
                                await _context.SaveChangesAsync();

                                if (remainingTransferQuantity == 0)
                                {
                                    break;
                                }
                            }

                            if (remainingTransferQuantity > 0)
                            {
                                List<AssetTransfer> assetTrnsfr = await _context.AssetTransfers.Where(a => a.IsActive == true
                                                                                                   && a.IntToEmployeeId == assetTransfer.IntFromEmployeeId
                                                                                                   && a.IntItemId == assetTransfer.IntItemId
                                                                                                   && a.IsPipelineClosed == true && a.IsReject == false
                                                                                                   && a.IntTransferQuantity > 0).OrderBy(a => a.IntTransferQuantity).ToListAsync();

                                if (assetTrnsfr != null)
                                {
                                    foreach (var itemTransfer in assetTrnsfr)
                                    {

                                        long decrementQuantity = Math.Min((long)remainingTransferQuantity, (long)itemTransfer.IntTransferQuantity);
                                        itemTransfer.IntTransferQuantity -= decrementQuantity;
                                        remainingTransferQuantity -= decrementQuantity;

                                        _context.AssetTransfers.Update(itemTransfer);
                                        await _context.SaveChangesAsync();

                                        if (remainingTransferQuantity == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return (response, application.IntAssetTransferId);
        }

        private async Task<bool> AssetTransferApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            AssetTransfer updatedApplication = await _context.AssetTransfers.FirstOrDefaultAsync(x => x.IntAssetTransferId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #endregion -------------- Asset Transfer Approval --------------------

        public async Task<bool> LeaveBalanceAndAttendanceUpdateAfterLeaveApproved(LveLeaveApplication application)
        {
            try
            {
                LveLeaveBalance lveLeaveBalance = await _context.LveLeaveBalances.FirstOrDefaultAsync(x => x.IntEmployeeId == application.IntEmployeeId && x.IntLeaveTypeId == application.IntLeaveTypeId && x.IsActive == true && x.IntYear == application.DteFromDate.Year);
                if (lveLeaveBalance != null)
                {
                    int totalDays = YearMonthDayCalculate.CalculateDaysBetweenTwoDate(application.DteFromDate.Date, application.DteToDate.Date);
                    lveLeaveBalance.IntRemainingDays = lveLeaveBalance.IntRemainingDays - totalDays;
                    lveLeaveBalance.IntLeaveTakenDays = lveLeaveBalance.IntLeaveTakenDays + totalDays;
                    _context.LveLeaveBalances.Update(lveLeaveBalance);
                    await _context.SaveChangesAsync();

                    List<TimeAttendanceDailySummary> attendanceDailySummariesUpdateList = new List<TimeAttendanceDailySummary>();
                    List<TimeAttendanceDailySummary> attendanceDailySummariesInsertList = new List<TimeAttendanceDailySummary>();

                    for (DateTime date = application.DteFromDate; application.DteToDate >= date; date = date.AddDays(1))
                    {
                        TimeAttendanceDailySummary attendance = await _context.TimeAttendanceDailySummaries.FirstOrDefaultAsync(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == date.Date);
                        if (attendance != null)
                        {
                            attendance.IsLeave = true;
                            attendance.IsPresent = false;
                            attendance.IsAbsent = false;
                            attendance.IsMovement = false;
                            attendance.IsHoliday = false;
                            attendance.IsOffday = false;
                            attendance.IsLate = false;
                            attendanceDailySummariesUpdateList.Add(attendance);
                        }
                        else
                        {
                            TimeAttendanceDailySummary dailySummary = new TimeAttendanceDailySummary()
                            {
                                IntDayId = date.Day,
                                IntMonthId = date.Month,
                                IntYear = date.Year,
                                IntEmployeeId = application.IntEmployeeId,
                                DteAttendanceDate = date.Date,
                                IsProcess = false,
                                IsAbsent = false,
                                IsHoliday = false,
                                IsLate = false,
                                IsLeave = true,
                                IsMovement = false,
                                IsOffday = false,
                                IsPresent = false,
                                IntCreatedBy = 0,
                                DteCreatedAt = DateTime.Now
                            };
                            attendanceDailySummariesInsertList.Add(dailySummary);
                        }
                    }
                    if (attendanceDailySummariesUpdateList.Count() > 0)
                    {
                        _context.TimeAttendanceDailySummaries.UpdateRange(attendanceDailySummariesUpdateList);
                        await _context.SaveChangesAsync();
                    }

                    if (attendanceDailySummariesInsertList.Count() > 0)
                    {
                        _context.TimeAttendanceDailySummaries.AddRange(attendanceDailySummariesInsertList);
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AttendanceSummaryUpdateAfterMovementApproved(LveMovementApplication application)
        {
            try
            {
                List<TimeAttendanceDailySummary> attendanceDailySummariesUpdateList = new List<TimeAttendanceDailySummary>();

                for (DateTime date = application.DteFromDate; application.DteToDate >= date; date = date.AddDays(1))
                {
                    TimeAttendanceDailySummary attendance = await _context.TimeAttendanceDailySummaries.FirstOrDefaultAsync(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == date.Date);
                    if (attendance != null)
                    {
                        attendance.IsMovement = true;
                        attendance.IsLeave = false;
                        attendance.IsPresent = false;
                        attendance.IsAbsent = false;
                        attendance.IsHoliday = false;
                        attendance.IsOffday = false;
                        attendance.IsLate = false;
                        attendanceDailySummariesUpdateList.Add(attendance);
                    }
                }
                if (attendanceDailySummariesUpdateList.Count() > 0)
                {
                    _context.TimeAttendanceDailySummaries.UpdateRange(attendanceDailySummariesUpdateList);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AttendanceSummaryUpdateAfterRemoteAttendanceApproved(TimeEmployeeAttendance application)
        {
            try
            {
                if (application != null)
                {

                    if (await _context.TimeAttendanceDailySummaries.Where(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == application.DteAttendanceDate.Value.Date).CountAsync() > 0)
                    {
                        DataTable dt = new DataTable();
                        DateTime dateTime = new DateTime(application.DteAttendanceDate.Value.Year, application.DteAttendanceDate.Value.Month, application.DteAttendanceDate.Value.Day,
                            application.DteAttendanceTime.Value.Hours, application.DteAttendanceTime.Value.Minutes, application.DteAttendanceTime.Value.Seconds);

                        using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                        {
                            string sql = "saas.sprTimeAttendanceProcessToSummaryForJob";
                            using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                            {
                                sqlCmd.CommandType = CommandType.StoredProcedure;
                                sqlCmd.Parameters.AddWithValue("@isManual", 1);
                                sqlCmd.Parameters.AddWithValue("@FromDate", dateTime);
                                sqlCmd.Parameters.AddWithValue("@ToDate", dateTime);
                                sqlCmd.Parameters.AddWithValue("@EmployeeId", application.IntEmployeeId);

                                connection.Open();
                                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                                {
                                    sqlAdapter.Fill(dt);
                                }
                                connection.Close();

                                return true;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> LoanBalanceRollBackAfterGeneratedSalaryRejectd(PyrPayrollSalaryGenerateRequest application)
        {
            try
            {
                if (application != null)
                {
                    List<EmpLoanSchedule> loanSchedules = (from sH in _context.PyrSalaryGenerateHeaders
                                                           join l in _context.EmpLoanSchedules on sH.IntEmployeeId equals l.IntEmployeeId
                                                           where (long)sH.IntSalaryGenerateRequestId == application.IntSalaryGenerateRequestId && sH.IsActive == true
                                                           && l.IntYear == application.IntYear && Convert.ToInt64(l.IntMonth) == application.IntMonth && l.IsActive == true
                                                           select l).ToList();

                    loanSchedules.ForEach(x =>
                    {
                        x.IsActive = false;
                    });


                    List<EmpLoanApplication> loanApplications = (from l in loanSchedules
                                                                 join app in _context.EmpLoanApplications on l.IntLoanApplicationId equals app.IntLoanApplicationId
                                                                 select app).ToList();

                    loanApplications.ForEach(x =>
                    {
                        x.NumRemainingBalance = x.NumRemainingBalance + loanSchedules.Where(ls => ls.IntLoanApplicationId == x.IntLoanApplicationId).Sum(s => s.IntInstallmentAmount);
                    });

                    _context.EmpLoanApplications.UpdateRange(loanApplications);
                    _context.EmpLoanSchedules.UpdateRange(loanSchedules);

                    await _context.SaveChangesAsync();

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Training Schedule
        public async Task<TrainingScheduleApprovalResponse> TrainingScheduleLandingEngine(TrainingScheduleLandingViewModel model)
        {
            try
            {
                TrainingScheduleApprovalResponse response = new TrainingScheduleApprovalResponse
                {
                    ResponseStatus = "",
                    ApplicationStatus = "",
                    CurrentSatageId = 0,
                    NextSatageId = 0,
                    IsComplete = true,
                    ListData = new List<TrainingScheduleLandingReturnViewModel>()
                };

                EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
                model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
                model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
                model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

                if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
                    && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
                {
                    response.ResponseStatus = "Invalid EmployeeId";
                }

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.AsNoTracking().AsQueryable().Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == "trainingScheduleApproval".ToLower());

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else
                {
                    List<TrainingScheduleLandingReturnViewModel> applicationList = new List<TrainingScheduleLandingReturnViewModel>();

                    if ((bool)model.IsAdmin)
                    {
                        model.IsSupOrLineManager = -1;
                        applicationList = await TrainingScheduleLandingEngine(model, header, null);
                        if (applicationList.Count() > 0)
                        {
                            response.ListData.AddRange(applicationList);
                            applicationList = new List<TrainingScheduleLandingReturnViewModel>();
                        }
                    }
                    else
                    {
                        if (model.IsSupervisor == true)
                        {
                            model.IsSupOrLineManager = 1;
                            applicationList = await TrainingScheduleLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<TrainingScheduleLandingReturnViewModel>();
                            }
                        }
                        if (model.IsLineManager == true)
                        {
                            model.IsSupOrLineManager = 2;
                            applicationList = await TrainingScheduleLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<TrainingScheduleLandingReturnViewModel>();
                            }
                        }
                        if (model.IsUserGroup == true)
                        {
                            model.IsSupOrLineManager = 3;
                            foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
                            {
                                applicationList = await TrainingScheduleLandingEngine(model, header, userGroup);
                                if (applicationList.Count() > 0)
                                {
                                    response.ListData.AddRange(applicationList);
                                    applicationList = new List<TrainingScheduleLandingReturnViewModel>();
                                }
                            }
                        }
                    }
                }
                response.ListData = response.ListData.OrderByDescending(x => x.Application.DteActionDate).ToList();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<TrainingScheduleLandingReturnViewModel>> TrainingScheduleLandingEngine(TrainingScheduleLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<TrainingScheduleLandingReturnViewModel> ListData = new List<TrainingScheduleLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.AsNoTracking().AsQueryable().Where(r => r.IsActive == true)
                .FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.TrainingSchedules
                                  where obj.IsActive == true
                                  join t in _context.training on obj.IntTrainingId equals t.IntTrainingId
                                  join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId

                                  where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)
                                  where obj.IsActive == true && obj.IntAccountId == model.AccountId && obj.IsReject == false
                                  select new TrainingScheduleLandingReturnViewModel
                                  {
                                      TrainingScheduleId = obj.IntScheduleId,
                                      AccountId = obj.IntAccountId,
                                      BusinessUnitId = obj.IntBusinessUnitId,
                                      TrainingId = obj.IntTrainingId,
                                      TrainingName = t.StrTrainingName,
                                      TrainingCode = t.StrTrainingCode,
                                      Venue = obj.StrVenue,
                                      ResourcePersonName = obj.StrResourcePersonName,
                                      BatchSize = obj.IntBatchSize,
                                      BatchNo = obj.StrBatchNo,
                                      DteDate = obj.DteDate,
                                      FromDate = obj.DteFromDate,
                                      ToDate = obj.DteToDate,
                                      TotalDuration = obj.NumTotalDuration,
                                      Remarks = obj.StrRemarks,
                                      CreatedAt = obj.DteActionDate,
                                      CreatedBy = obj.IntActionBy,
                                      PipelineHeaderId = obj.IntPipelineHeaderId,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                      NextStage = (long)obj.IntNextStage,
                                      Status = obj.StrStatus,
                                      IsPipelineClosed = (bool)obj.IsPipelineClosed,
                                      IsReject = (bool)obj.IsReject,
                                      RejectDateTime = obj.DteRejectDateTime,
                                      RejectedBy = obj.IntRejectedBy,
                                      IsActive = obj.IsActive,
                                      Application = obj
                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }
            return ListData;
        }

        private async Task<(TrainingScheduleApprovalResponse, long)> TrainingScheduleApprovalEngine(TrainingSchedule application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            TrainingScheduleApprovalResponse response = new TrainingScheduleApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.TrainingSchedules.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.TrainingSchedules.Update(application);
                await _context.SaveChangesAsync();


            }

            return (response, application.IntScheduleId);
        }

        public async Task<TrainingScheduleApprovalResponse> TrainingScheduleApprovalEngine(TrainingSchedule application, bool isReject, long approverId, long accountId)
        {
            try
            {
                TrainingScheduleApprovalResponse response = new TrainingScheduleApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid 'Training Schedule";
                }
                else
                {
                    var res = await TrainingScheduleApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await TrainingScheduleApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await TrainingScheduleApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await TrainingScheduleApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<bool> TrainingScheduleApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            TrainingSchedule updatedApplication = await _context.TrainingSchedules.FirstOrDefaultAsync(x => x.IntScheduleId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }

        #region Training Requisition
        public async Task<ApplicationLandingVM> TrainingRequisitionLandingEngine(TrainingRequisitionLandingViewModel model, BaseVM tokenData)
        {
            try
            {
                model.ApplicationStatus = model.ApplicationStatus.ToLower();
                model.SearchText = model.SearchText.ToLower();

                var TrainingReqQuery = from obj in _context.TrainingRequisitions
                                       where obj.IsActive == true
                                       join currentStage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currentStage.IntPipelineRowId
                                       join emp in _context.EmpEmployeeBasicInfos on obj.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                                       from emp in emp1.DefaultIfEmpty()
                                       join dep1 in _context.MasterDepartments on emp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                       from dep in dep2.DefaultIfEmpty()
                                       join deg1 in _context.MasterDesignations on emp.IntDesignationId equals deg1.IntDesignationId into deg2
                                       from desg in deg2.DefaultIfEmpty()
                                       where obj.IntAccountId == tokenData.accountId && obj.IsActive == true && (model.FromDate.Date <= obj.DteActionDate.Date && obj.DteActionDate.Date <= model.ToDate.Date)
                                       && (model.ApplicationStatus == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false
                                               : model.ApplicationStatus == "approved" && !tokenData.isOfficeAdmin ? true
                                               : model.ApplicationStatus == "reject" ? obj.IsReject == true
                                               : model.ApplicationStatus == "approved" && tokenData.isOfficeAdmin ? obj.IsPipelineClosed == true && obj.IsReject == false : false)

                                       && (!string.IsNullOrEmpty(model.SearchText) ? emp.StrEmployeeName.ToLower().Contains(model.SearchText) || desg.StrDesignation.ToLower().Contains(model.SearchText)
                                                   || dep.StrDepartment.ToLower().Contains(model.SearchText) || emp.StrEmployeeCode.ToLower().Contains(model.SearchText) || emp.StrEmploymentType.ToLower().Contains(model.SearchText) : true)

                                       select new
                                       {
                                           TrainingRequisitionId = obj.IntRequisitionId,
                                           BusinessUnitId = obj.IntBusinessUnitId,
                                           EmployeeId = obj.IntEmployeeId,
                                           EmployeeName = emp.StrEmployeeName,
                                           EmployeeCode = emp.StrEmployeeCode,
                                           Department = dep.StrDepartment,
                                           Designation = desg.StrDesignation,
                                           EmploymentType = emp.StrEmploymentType,
                                           ScheduleId = obj.IntScheduleId,
                                           TrainingName = obj.StrTrainingName,
                                           Email = obj.StrEmail,
                                           PhoneNo = obj.StrPhoneNo,
                                           Gender = obj.StrGender,
                                           CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currentStage.StrStatusTitle,
                                           Status = obj.StrStatus,
                                           SupervisorId = emp.IntSupervisorId,
                                           LineManagerId = emp.IntLineManagerId,
                                           DottedSupervisorId = emp.IntDottedSupervisorId,
                                           Application = obj
                                       };

                ApplicationLandingVM retObj = new();
                retObj.PageSize = model.PageSize;
                retObj.PageNo = model.PageNo;

                if (tokenData.isOfficeAdmin)
                {
                    // pagination here

                    var TrainingReqListForAdmin = await TrainingReqQuery.Select(obj => new TrainingRequisitionLandingReturnViewModel
                    {
                        TrainingRequisitionId = obj.TrainingRequisitionId,
                        BusinessUnitId = obj.BusinessUnitId,
                        EmployeeId = obj.EmployeeId,
                        EmployeeName = obj.EmployeeName,
                        EmployeeCode = obj.EmployeeCode,
                        Department = obj.Department,
                        Designation = obj.Designation,
                        EmploymentType = obj.EmploymentType,
                        ScheduleId = obj.ScheduleId,
                        TrainingName = obj.TrainingName,
                        Email = obj.Email,
                        PhoneNo = obj.PhoneNo,
                        Gender = obj.Gender,
                        CurrentStage = obj.CurrentStage,
                        Status = obj.Status,
                        Application = obj.Application
                    }).ToListAsync();

                    retObj.TotalCount = TrainingReqListForAdmin.Count();
                    retObj.Data = TrainingReqListForAdmin.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                    return retObj;
                }


                EmpIsSupNLMORUGMemberViewModel isSupNLmVM = await EmployeeIsSupervisorNLineManagerORUserGroupMember(tokenData.accountId, tokenData.employeeId);

                List<long> userGroupIdList = new List<long>();

                if (isSupNLmVM.IsUserGroup)
                {
                    userGroupIdList = isSupNLmVM.UserGroupRows.Select(x => x.IntUserGroupHeaderId).ToList();
                }

                var globalPipelineRowList = await (from h in _context.GlobalPipelineHeaders
                                                   join r in _context.GlobalPipelineRows on h.IntPipelineHeaderId equals r.IntPipelineHeaderId
                                                   where h.StrApplicationType == "trainingRequisitionApproval" && h.IsActive == true && r.IsActive == true
                                                   select r).AsNoTracking().ToListAsync();

                var TrainingReqList = await TrainingReqQuery.ToListAsync();

                var listData = from obj in TrainingReqList
                               join currentStage in globalPipelineRowList on obj.Application.IntCurrentStage equals currentStage.IntPipelineRowId

                               let approverStage = globalPipelineRowList.FirstOrDefault(x => x.IsActive == true &&
                               (isSupNLmVM.IsSupervisor == true && x.IsSupervisor == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && (obj.SupervisorId == tokenData.employeeId || obj.DottedSupervisorId == tokenData.employeeId))
                               || (isSupNLmVM.IsLineManager == true && x.IsLineManager == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && obj.LineManagerId == tokenData.employeeId)
                               || (isSupNLmVM.IsUserGroup == true && obj.Application.IntPipelineHeaderId == x.IntPipelineHeaderId && userGroupIdList.Contains((long)x.IntUserGroupHeaderId)))

                               where ((model.ApplicationStatus == "pending" || model.ApplicationStatus == "reject") && approverStage != null ? currentStage.IntShortOrder == approverStage.IntShortOrder
                               : model.ApplicationStatus == "approved" && approverStage != null && obj.Application.IsPipelineClosed == true && obj.Application.IsReject == false ? (approverStage.IntShortOrder < currentStage.IntShortOrder || (obj.Application.IntCurrentStage == obj.Application.IntNextStage))
                               : false)
                               select new TrainingRequisitionLandingReturnViewModel
                               {
                                   TrainingRequisitionId = obj.TrainingRequisitionId,
                                   BusinessUnitId = obj.BusinessUnitId,
                                   EmployeeId = obj.EmployeeId,
                                   EmployeeName = obj.EmployeeName,
                                   EmployeeCode = obj.EmployeeCode,
                                   Department = obj.Department,
                                   Designation = obj.Designation,
                                   EmploymentType = obj.EmploymentType,
                                   ScheduleId = obj.ScheduleId,
                                   TrainingName = obj.TrainingName,
                                   Email = obj.Email,
                                   PhoneNo = obj.PhoneNo,
                                   Gender = obj.Gender,
                                   CurrentStage = obj.CurrentStage,
                                   Status = obj.Status,
                                   Application = obj.Application
                               };

                retObj.TotalCount = listData.Count();
                retObj.Data = listData.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize).ToList();
                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<TrainingRequisitionApprovalResponse> TrainingRequisitionApprovalEngine(TrainingRequisition application, bool isReject, long approverId, long accountId)
        {
            try
            {
                TrainingRequisitionApprovalResponse response = new TrainingRequisitionApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid 'Training Requisition";
                }
                else
                {
                    var res = await TrainingRequisitionApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await TrainingRequisitionApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await TrainingRequisitionApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await TrainingRequisitionApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<(TrainingRequisitionApprovalResponse, long)> TrainingRequisitionApprovalEngine(TrainingRequisition application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            TrainingRequisitionApprovalResponse response = new TrainingRequisitionApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.TrainingRequisitions.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    //application.up = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    //application.DteUpdatedAt = DateTime.Now;
                    //application.IntUpdatedBy = approverId;
                }

                _context.TrainingRequisitions.Update(application);
                await _context.SaveChangesAsync();


            }

            return (response, application.IntScheduleId);
        }
        private async Task<bool> TrainingRequisitionApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            TrainingRequisition updatedApplication = await _context.TrainingRequisitions.FirstOrDefaultAsync(x => x.IntRequisitionId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }
        #endregion

        #region Master Location Registration
        public async Task<MasterLocationApprovalResponse> MastrerLocaLandingEngine(MasterLocationLandingViewModel model)
        {
            try
            {
                MasterLocationApprovalResponse response = new MasterLocationApprovalResponse
                {
                    ResponseStatus = "",
                    ApplicationStatus = "",
                    CurrentSatageId = 0,
                    NextSatageId = 0,
                    IsComplete = true,
                    ListData = new List<MasterLocationLandingReturnViewModel>()
                };

                EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await EmployeeIsSupervisorNLineManagerORUserGroupMember(model.AccountId, model.ApproverId);
                model.IsSupervisor = isSupNLMORUGMemberViewModel.IsSupervisor;
                model.IsLineManager = isSupNLMORUGMemberViewModel.IsLineManager;
                model.IsUserGroup = isSupNLMORUGMemberViewModel.IsUserGroup;

                if (!(bool)model.IsSupervisor && !(bool)model.IsLineManager && !(bool)model.IsUserGroup && !(bool)model.IsAdmin
                    && isSupNLMORUGMemberViewModel.UserGroupRows.Count() <= 0)
                {
                    response.ResponseStatus = "Invalid EmployeeId";
                }

                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == model.AccountId).AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.StrApplicationType.ToLower() == "masterLocationRegistration");

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else
                {
                    List<MasterLocationLandingReturnViewModel> applicationList = new List<MasterLocationLandingReturnViewModel>();

                    if ((bool)model.IsAdmin)
                    {
                        model.IsSupOrLineManager = -1;
                        applicationList = await MastrerLocaLandingEngine(model, header, null);
                        if (applicationList.Count() > 0)
                        {
                            response.ListData.AddRange(applicationList);
                            applicationList = new List<MasterLocationLandingReturnViewModel>();
                        }
                    }
                    else
                    {
                        if (model.IsSupervisor == true)
                        {
                            model.IsSupOrLineManager = 1;
                            applicationList = await MastrerLocaLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<MasterLocationLandingReturnViewModel>();
                            }
                        }
                        if (model.IsLineManager == true)
                        {
                            model.IsSupOrLineManager = 2;
                            applicationList = await MastrerLocaLandingEngine(model, header, null);
                            if (applicationList.Count() > 0)
                            {
                                response.ListData.AddRange(applicationList);
                                applicationList = new List<MasterLocationLandingReturnViewModel>();
                            }
                        }
                        if (model.IsUserGroup == true)
                        {
                            model.IsSupOrLineManager = 3;
                            foreach (UserGroupRow userGroup in isSupNLMORUGMemberViewModel.UserGroupRows)
                            {
                                applicationList = await MastrerLocaLandingEngine(model, header, userGroup);
                                if (applicationList.Count() > 0)
                                {
                                    response.ListData.AddRange(applicationList);
                                    applicationList = new List<MasterLocationLandingReturnViewModel>();
                                }
                            }
                        }
                    }
                }
                response.ListData = response.ListData.OrderByDescending(x => x.Application.DteCreatedAt).ToList();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<MasterLocationLandingReturnViewModel>> MastrerLocaLandingEngine(MasterLocationLandingViewModel model, GlobalPipelineHeader header, UserGroupRow? userGroupRow)
        {
            GlobalPipelineRow approverStage = new GlobalPipelineRow();
            List<MasterLocationLandingReturnViewModel> ListData = new List<MasterLocationLandingReturnViewModel>();

            if (!(bool)model.IsAdmin)
            {
                approverStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true)
                .AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => model.IsSupOrLineManager == 1 ? x.IsSupervisor && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : model.IsSupOrLineManager == 2 ? x.IsLineManager && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : userGroupRow != null ? userGroupRow.IntUserGroupHeaderId == x.IntUserGroupHeaderId && header.IntPipelineHeaderId == x.IntPipelineHeaderId
                : x.IntPipelineRowId == -0);
            }

            if ((bool)model.IsAdmin || (approverStage == null ? 0 : approverStage.IntPipelineRowId) > 0)
            {
                ListData = await (from obj in _context.MasterLocationRegisters
                                  where obj.IsActive == true && obj.IntAccountId == model.AccountId
                                  join currrentstage in _context.GlobalPipelineRows on obj.IntCurrentStage equals currrentstage.IntPipelineRowId
                                  where ((bool)model.IsAdmin ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? obj.IsPipelineClosed == true && obj.IsReject == false : model.ApplicationStatus.ToLower() == "reject" ? obj.IsReject == true : false) : true)
                                  && ((model.ApproverId > 0 && model.IsAdmin == false && approverStage != null) ? (model.ApplicationStatus.ToLower() == "pending" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsPipelineClosed == false && obj.IsReject == false : model.ApplicationStatus.ToLower() == "approved" ? (approverStage.IntShortOrder < currrentstage.IntShortOrder || (obj.IntCurrentStage == obj.IntNextStage && approverStage.IntPipelineRowId == obj.IntNextStage)) : model.ApplicationStatus.ToLower() == "reject" ? obj.IntCurrentStage == approverStage.IntPipelineRowId && obj.IsReject == true : false) : true)
                                  select new MasterLocationLandingReturnViewModel
                                  {
                                      IntMasterLocationId = obj.IntMasterLocationId,
                                      IntAccountId = obj.IntAccountId,
                                      IntBusinessId = obj.IntBusinessId,
                                      StrLongitude = obj.StrLongitude,
                                      StrLatitude = obj.StrLatitude,
                                      StrPlaceName = obj.StrPlaceName,
                                      StrAddress = obj.StrAddress,
                                      IsActive = obj.IsActive,
                                      IntPipelineHeaderId = obj.IntPipelineHeaderId,
                                      IntCurrentStage = obj.IntCurrentStage,
                                      IntNextStage = obj.IntNextStage,
                                      StrStatus = obj.StrStatus,
                                      Status = model.ApplicationStatus,
                                      CurrentStage = model.ApplicationStatus.ToLower() == "approved" ? "" : currrentstage.StrStatusTitle,
                                      Application = obj
                                  }).AsNoTracking().AsQueryable().ToListAsync();
            }

            return ListData;
        }

        public async Task<MasterLocationApprovalResponse> MasterLocationApprovalEngine(MasterLocationRegister application, bool isReject, long approverId, long accountId)
        {
            try
            {
                MasterLocationApprovalResponse response = new MasterLocationApprovalResponse();
                GlobalPipelineHeader header = await _context.GlobalPipelineHeaders.Where(r => r.IsActive == true && r.IntAccountId == accountId).FirstOrDefaultAsync(x => x.IntPipelineHeaderId == application.IntPipelineHeaderId);

                if (header == null)
                {
                    response.ResponseStatus = "Invalid approval pipeline";
                }
                else if (application == null)
                {
                    response.ResponseStatus = "Invalid Leave Application";
                }
                else
                {
                    var res = await MasterLocationApprovalEngine(application, header, isReject, approverId);
                    bool isDo = res.Item1.IsComplete ? false : true;
                    response = res.Item1;

                    if (isDo)
                    {
                        isDo = await MasterLocationApprovalWhile(res.Item2, approverId, accountId, header);
                        while (isDo)
                        {
                            res = await MasterLocationApprovalEngine(application, header, isReject, approverId);
                            bool isApplicableForNext = await MasterLocationApprovalWhile(res.Item2, approverId, accountId, header);
                            isDo = (isApplicableForNext && !res.Item1.IsComplete) ? true : false;
                            response = res.Item1;
                        }
                    }
                }

                // here

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<(MasterLocationApprovalResponse, long)> MasterLocationApprovalEngine(MasterLocationRegister application, GlobalPipelineHeader header, bool isReject, long approverId)
        {
            MasterLocationApprovalResponse response = new MasterLocationApprovalResponse();

            if (isReject)
            {
                response.ResponseStatus = "success";
                response.ApplicationStatus = "Rejected";
                response.IsComplete = true;

                application.StrStatus = response.ApplicationStatus;
                application.IsPipelineClosed = true;
                application.IsReject = true;
                application.IntRejectedBy = approverId;
                application.DteRejectDateTime = DateTime.Now;

                _context.MasterLocationRegisters.Update(application);
                await _context.SaveChangesAsync();
            }
            else
            {
                GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == application.IntCurrentStage);

                List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder < x.IntShortOrder)
                .OrderBy(x => x.IntShortOrder).ToListAsync();

                if (pipelineRows.Count > 0)
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    response.NextSatageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                    response.IsComplete = false;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = false;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                else
                {
                    response.ResponseStatus = "success";
                    response.ApplicationStatus = currentStage.StrStatusTitle;
                    response.CurrentSatageId = currentStage.IntPipelineRowId;
                    response.NextSatageId = currentStage.IntPipelineRowId;
                    response.IsComplete = true;

                    application.IntCurrentStage = response.CurrentSatageId;
                    application.IntNextStage = response.NextSatageId;
                    application.StrStatus = response.ApplicationStatus;
                    application.IsPipelineClosed = true;
                    application.DteUpdatedAt = DateTime.Now;
                    application.IntUpdatedBy = approverId;
                }
                _context.MasterLocationRegisters.Update(application);
                await _context.SaveChangesAsync();
            }

            return (response, application.IntMasterLocationId);
        }

        private async Task<bool> MasterLocationApprovalWhile(long applicationId, long approverId, long accountId, GlobalPipelineHeader header)
        {
            bool res = false;

            MasterLocationRegister updatedApplication = await _context.MasterLocationRegisters.FirstOrDefaultAsync(x => x.IntMasterLocationId == applicationId);

            GlobalPipelineRow currentStage = await _context.GlobalPipelineRows.Where(r => r.IsActive == true).FirstOrDefaultAsync(x => x.IntPipelineRowId == updatedApplication.IntCurrentStage);

            List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
            .Where(x => x.IntPipelineHeaderId == header.IntPipelineHeaderId && x.IsActive == true && currentStage.IntShortOrder <= x.IntShortOrder)
            .OrderBy(x => x.IntShortOrder).ToListAsync();

            if (pipelineRows.Count > 0)
            {
                EmpIsSupNLMORUGMemberViewModel model = await EmployeeIsSupervisorNLineManagerORUserGroupMember(accountId, approverId);
                if (pipelineRows.FirstOrDefault().IsSupervisor && model.IsSupervisor)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntSupervisorId == approverId || x.IntDottedSupervisorId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IsLineManager && model.IsLineManager)
                {
                    res = await _context.EmpEmployeeBasicInfos.Where(x => x.IntLineManagerId == approverId).CountAsync() > 0 ? true : false;
                }
                else if (pipelineRows.FirstOrDefault().IntUserGroupHeaderId > 0 && model.IsUserGroup)
                {
                    res = await _context.UserGroupRows.Where(x => x.IsActive == true
                    && x.IntUserGroupHeaderId == pipelineRows.FirstOrDefault().IntUserGroupHeaderId && x.IntEmployeeId == approverId).CountAsync() > 0 ? true : false;
                }
            }

            return res;
        }
        #endregion

        #endregion ===================== APPROVAL ENGINE ALL METHOD ========================

        #region ======================== OTERS METHOD ===============================

        #endregion ===================== OTERS METHOD ===============================


        #region ======= Get pipeline current stage next stage====
        public async Task<PipelineStageInfoVM> GetPipelineDetailsByEmloyeeIdNType(long intEmployeeId, string ApplicationType)
        {
            try
            {
                var eD = await (from emp in _context.EmpEmployeeBasicInfos
                                where emp.IntEmployeeBasicInfoId == intEmployeeId
                                join empD1 in _context.EmpEmployeeBasicInfoDetails on
                                new { a = emp.IntEmployeeBasicInfoId, b = (bool)emp.IsActive, c = true } equals
                                new { a = empD1.IntEmployeeId, b = true, c = (bool)empD1.IsActive } into empD2
                                from empD in empD2.DefaultIfEmpty()
                                join usr in _context.Users on emp.IntEmployeeBasicInfoId equals usr.IntRefferenceId into usr2
                                from usr in usr2.DefaultIfEmpty()
                                select new
                                {
                                    IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                    IntAccountId = emp.IntAccountId,
                                    IntBusinessUnitId = emp.IntBusinessUnitId,
                                    IntWorkplaceGroupId = emp.IntWorkplaceGroupId,
                                    IntWorkplaceId = emp.IntWorkplaceId,
                                    IsOfficeAdmin = usr.IsOfficeAdmin,
                                    IntWingId = empD == null ? -1 : (empD.IntWingId == null || empD.IntWingId == 0 ? -1 : empD.IntWingId),
                                    IntSoleDepo = empD == null ? -1 : (empD.IntSoleDepo == null || empD.IntSoleDepo == 0 ? -1 : empD.IntSoleDepo),
                                    IntRegionId = empD == null ? -1 : (empD.IntRegionId == null || empD.IntRegionId == 0 ? -1 : empD.IntRegionId),
                                    IntAreaId = empD == null ? -1 : (empD.IntAreaId == null || empD.IntAreaId == 0 ? -1 : empD.IntAreaId),
                                    IntTerritoryId = empD == null ? -1 : (empD.IntTerritoryId == null || empD.IntTerritoryId == 0 ? -1 : empD.IntTerritoryId)
                                }).AsNoTracking().FirstOrDefaultAsync();

                GlobalPipelineHeader pipeHeader = await _context.GlobalPipelineHeaders.Where(
                                                                            x => x.IntAccountId == eD.IntAccountId
                                                                            && x.IsActive == true
                                                                            && x.IntBusinessUnitId == eD.IntBusinessUnitId
                                                                            && x.IntWorkplaceGroupId == eD.IntWorkplaceGroupId
                                                                            && x.StrApplicationType.ToLower() == ApplicationType.ToLower()
                                                                            && (x.IntWingId == 0 ? true : (x.IntWingId == eD.IntWingId && (x.IntSoleDepoId == 0 ? true :
                                                                            (x.IntSoleDepoId == eD.IntSoleDepo && (x.IntRegionId == 0 ? true : (x.IntRegionId == eD.IntRegionId &&
                                                                            (x.IntAreaId == 0 ? true : (x.IntAreaId == eD.IntAreaId
                                                                            && (x.IntTerritoryId == 0 ? true : x.IntTerritoryId == eD.IntTerritoryId)))))))))).FirstOrDefaultAsync();
                PipelineStageInfoVM res = new();

                if (pipeHeader != null)
                {
                    List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                                                           .Where(x => x.IntPipelineHeaderId == pipeHeader.IntPipelineHeaderId && x.IsActive == true)
                                                           .OrderBy(x => x.IntShortOrder).ToListAsync();

                    res.HeaderId = pipeHeader.IntPipelineHeaderId;
                    res.CurrentStageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    res.NextStageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;
                }

                if (res.HeaderId <= 0)
                {
                    //throw new Exception("Pipeline was not set correctly");
                    res.messageHelper.StatusCode = 400;
                    res.messageHelper.Message = "Pipeline was not set correctly";
                    return res;
                }

                else if (res.CurrentStageId <= 0 || res.NextStageId <= 0)
                {
                    //throw new Exception("pipe line was set but no aprrover found");
                    res.messageHelper.StatusCode = 400;
                    res.messageHelper.Message = "pipe line was set but no aprrover found";
                    return res;
                }
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<PipelineStageInfoVM> GetPipelineDetailsByEmloyeeIdNType(long AccountId, long BusinessUnitId, long WorkPlaceGroupId, long soleDepoId, long AreaId, string ApplicationType)
        {
            try
            {
                soleDepoId = soleDepoId == 0 ? -1 : soleDepoId;
                AreaId = AreaId == 0 ? -1 : AreaId;

                var pipeHeader = await _context.GlobalPipelineHeaders.Where(x => x.IntAccountId == AccountId && x.IntBusinessUnitId == BusinessUnitId && x.IntWorkplaceGroupId == WorkPlaceGroupId
                                                                        && x.IntSoleDepoId == soleDepoId && x.IntAreaId == AreaId && x.StrApplicationType.ToLower() == ApplicationType.ToLower()
                                                                         ).FirstOrDefaultAsync();
                PipelineStageInfoVM res = new();

                if (pipeHeader != null)
                {
                    List<GlobalPipelineRow> pipelineRows = await _context.GlobalPipelineRows
                                                           .Where(x => x.IntPipelineHeaderId == pipeHeader.IntPipelineHeaderId && x.IsActive == true)
                                                           .OrderBy(x => x.IntShortOrder).ToListAsync();
                    res.HeaderId = pipeHeader.IntPipelineHeaderId;
                    res.CurrentStageId = pipelineRows.FirstOrDefault().IntPipelineRowId;
                    res.NextStageId = pipelineRows.Count() == 1 ? pipelineRows.LastOrDefault().IntPipelineRowId : pipelineRows.Skip(1).FirstOrDefault().IntPipelineRowId;

                }

                if (res.HeaderId <= 0)
                {
                    throw new Exception("Pipeline was not set correctly");
                }

                else if (res.CurrentStageId <= 0 || res.NextStageId <= 0)
                {
                    throw new Exception("pipe line was set but no aprrover found");
                }
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public bool DateRangeValidation(int MonthId, int YearId, DateTime fromDate, DateTime toDate)
        {
            bool res = false;

            DateTime date = new DateTime(YearId, MonthId, 1).Date;
            res = date.Date >= fromDate.Date && date.Date <= toDate.Date ? true : false;

            return res;
        }
    }
}