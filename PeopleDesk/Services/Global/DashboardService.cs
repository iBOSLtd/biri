using LanguageExt;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Services.Global
{
    public class DashboardService : IDashboardService
    {
        private readonly PeopleDeskContext _context;
        private readonly IQRYDataForReportService _iQRYDataForReportService;
        private readonly ILeaveMovementService _leaveMovementService;
        private readonly IEmployeeService _employeeService;
        private readonly IApprovalPipelineService _approvalPipelineService;

        public DashboardService(IApprovalPipelineService _approvalPipelineService, PeopleDeskContext _context, IQRYDataForReportService _iQRYDataForReportService, ILeaveMovementService _leaveMovementService, IEmployeeService employeeService)
        {
            this._context = _context;
            this._iQRYDataForReportService = _iQRYDataForReportService;
            this._leaveMovementService = _leaveMovementService;
            _employeeService = employeeService;
            this._approvalPipelineService = _approvalPipelineService;
        }

        public async Task<EmployeeDashboardViewModel> EmployeeDashboard(long EmployeeId, long? BusinessUnitId)
        {
            User userInfo = await _context.Users.AsNoTracking().AsQueryable().Where(x => x.IntRefferenceId == EmployeeId && x.IsActive == true).FirstOrDefaultAsync();

            EmployeeQryProfileAllViewModel employeeQryProfile = await _iQRYDataForReportService.EmployeeQryProfileAll(EmployeeId);

            if (employeeQryProfile == null)
            {
                throw new Exception("Invalid Employee");
            }

            TimeAttendanceDailySummary attendanceDailySummary = await _context.TimeAttendanceDailySummaries.Where(x => x.DteAttendanceDate.Value.Date == DateTime.Now.Date && x.IntEmployeeId == EmployeeId).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            string workingPeriod = String.Empty;
            string serviceLength = String.Empty;

            if (employeeQryProfile.EmployeeBasicInfo.DteJoiningDate != null)
            {
                LocalDate joiningDate = new LocalDate(employeeQryProfile.EmployeeBasicInfo.DteJoiningDate.Value.Date.Year, employeeQryProfile.EmployeeBasicInfo.DteJoiningDate.Value.Date.Month, employeeQryProfile.EmployeeBasicInfo.DteJoiningDate.Value.Date.Day);
                LocalDate currentDate = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                serviceLength += period.Years > 0 ? period.Years + (period.Years > 1 ? " years " : " year ") : "";
                serviceLength += period.Months > 0 ? period.Months + (period.Months > 1 ? " months " : " month ") : "";
                serviceLength += period.Days > 0 ? period.Days + (period.Days > 1 ? " days" : " day") : "";
            }
            string attendanceStatus = "Absent";

            if (attendanceDailySummary != null)
            {
                if (attendanceDailySummary.StrWorkingHours != null || attendanceDailySummary.StrWorkingHours != "")
                {
                    //DateTime utcTime = DateTime.UtcNow;
                    //TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
                    //DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);

                    //LocalTime startTime = new LocalTime(attendanceDailySummary.TmeInTime.Value.Hours, attendanceDailySummary.TmeInTime.Value.Minutes);
                    //LocalTime tmeLastOutTime = new LocalTime(attendanceDailySummary.TmeLastOutTime == null ? DateTime.Now.Hour : attendanceDailySummary.TmeLastOutTime.Value.Hours, attendanceDailySummary.TmeLastOutTime == null ? DateTime.Now.Minute : attendanceDailySummary.TmeLastOutTime.Value.Minutes);

                    //LocalTime endTime = new LocalTime(tmeLastOutTime == null ? localDateTime.Hour : tmeLastOutTime.Hour, tmeLastOutTime == null ? localDateTime.Minute : tmeLastOutTime.Minute);

                    //Period period2 = Period.Between(startTime, endTime);

                    //workingPeriod += period2.Hours > 0 ? period2.Hours + (period2.Hours > 1 ? " hours " : " hour ") : "0 hour ";
                    //workingPeriod += period2.Minutes > 0 ? period2.Minutes + (period2.Minutes > 1 ? " minutes" : " minute") : " 0 minute";

                    workingPeriod = attendanceDailySummary.StrWorkingHours;
                }
                else
                {
                    workingPeriod = "Please check-in";
                }

                attendanceStatus = attendanceDailySummary.IsPresent == true ? "Present"
                                        : attendanceDailySummary.IsMovement == true ? "Movement"
                                        : attendanceDailySummary.IsLate == true ? "Late In"
                                        : attendanceDailySummary.IsEarlyLeave == true ? "Early Out"
                                        : attendanceDailySummary.IsLeave == true ? "Leave"
                                        : attendanceDailySummary.DteAttendanceDate.Value.Date == DateTime.Now.Date ? "" : "Absent";

                if (attendanceStatus == "Absent")
                {
                    EmpManualAttendanceSummary ManualAttendance = await _context.EmpManualAttendanceSummaries.AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.IntAttendanceSummaryId == attendanceDailySummary.IntAutoId);
                    attendanceStatus = ManualAttendance != null ? (ManualAttendance.IsPipelineClosed == true && ManualAttendance.IsReject == false) ? "Present" : attendanceStatus : attendanceStatus;
                }
            }

            DataTable dataTable = await _leaveMovementService.GetEmployeeLeaveBalanceAndHistory(EmployeeId, "LeaveBalance", null, null, null, null);

            int isValid = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (Convert.ToInt32(row["BalanceDays"].ToString()) > 0)
                {
                    isValid = 1;
                    break;
                }
            }

            List<LeaveBalanceHistory> LeaveBalanceHistoryList = (dataTable.Rows.Count > 0 && isValid > 0) ? ConvertDataTableToList.Convert<LeaveBalanceHistory>(dataTable) : new List<LeaveBalanceHistory>();

            TimeCalender timeCalender = new TimeCalender();
            if (attendanceDailySummary != null)
            {
                timeCalender = await _context.TimeCalenders.AsNoTracking().AsQueryable().Where(x => x.IsActive == true && x.IntCalenderId == attendanceDailySummary.IntCalendarId).FirstOrDefaultAsync();
            }

            AttendanceSummaryViewModel AttendanceSummaryViewModel = await GetAttendanceSummaryCalenderReport(EmployeeId, DateTime.Now.Month, DateTime.Now.Year);

            EmpIsSupNLMORUGMemberViewModel isSupNLMORUGMemberViewModel = await _approvalPipelineService.EmployeeIsSupervisorNLineManagerORUserGroupMember((long)(employeeQryProfile?.EmployeeBasicInfo?.IntAccountId), EmployeeId);
            List<DashboardRole> dashboardRole = new List<DashboardRole>
            {
                new DashboardRole
                {
                    Value = 1,
                    Label = "Employee"
                }
            };
            if (isSupNLMORUGMemberViewModel.IsSupervisor || isSupNLMORUGMemberViewModel.IsLineManager)
            {
                dashboardRole.Add(new DashboardRole
                {
                    Value = 2,
                    Label = "Supervisor"
                });
            }
            if (await _context.ManagementDashboardPermissions.Where(x => x.IntEmployeeId == EmployeeId && x.IsActive == true).CountAsync() > 0)
            {
                dashboardRole.Add(new DashboardRole
                {
                    Value = 3,
                    Label = "Management"
                });
            }

            //string workingHours = AttendanceSummaryViewModel.timeAttendanceDailySummaries.Where(x => x.DteAttendanceDate.Value.Date == DateTime.Now.Date).Select(x => x.StrWorkingHours).FirstOrDefault();

            EmployeeDashboardViewModel model = new EmployeeDashboardViewModel
            {
                EmployeeId = EmployeeId,
                EmployeeName = employeeQryProfile?.EmployeeBasicInfo?.StrEmployeeName,
                EmployeeCode = employeeQryProfile?.EmployeeBasicInfo?.StrEmployeeCode,
                DepartmentName = employeeQryProfile?.DepartmentName,
                DesignationName = employeeQryProfile?.DesignationName,
                EmploymentType = employeeQryProfile?.EmployeeBasicInfo?.StrEmploymentType,
                EmployeeProfileUrlId = _context.EmpEmployeePhotoIdentities.AsNoTracking().AsQueryable().Where(x => x.IsActive == true && x.IntEmployeeBasicInfoId == EmployeeId).Select(x => x.IntProfilePicFileUrlId).FirstOrDefault(),
                JoiningDate = employeeQryProfile?.EmployeeBasicInfo?.DteJoiningDate,
                ConfirmationDate = employeeQryProfile?.EmployeeBasicInfo?.DteConfirmationDate,
                AttendanceStatus = attendanceStatus,
                //WorkingPeriod = (workingHours == null || workingHours == "") ? workingPeriod : workingHours,
                WorkingPeriod = workingPeriod,
                CheckIn = AttendanceSummaryViewModel.timeAttendanceDailySummaries.Where(x => x.DteAttendanceDate.Value.Date == DateTime.Now.Date).Select(x => x.TmeInTime).FirstOrDefault(),
                CheckOut = AttendanceSummaryViewModel.timeAttendanceDailySummaries.Where(x => x.DteAttendanceDate.Value.Date == DateTime.Now.Date).Select(x => x.TmeLastOutTime).FirstOrDefault(),
                ServiceLength = serviceLength,
                Supervisor = employeeQryProfile?.SupervisorName,
                DottedSupervisor = employeeQryProfile?.DottedSupervisorName,
                LineManager = employeeQryProfile?.LineManagerName,
                IntDottedSupervisorImageUrlId = employeeQryProfile?.IntDottedSupervisorImageUrlId,
                IntSupervisorImageUrlId = employeeQryProfile?.IntSupervisorImageUrlId,
                IntLineManagerImageUrlId = employeeQryProfile?.IntLineManagerImageUrlId,
                CalendarName = timeCalender.StrCalenderName,
                CalendarStartTime = timeCalender.DteStartTime,
                CalendarEndTime = timeCalender.DteEndTime,
                MonthName = DateTime.Now.ToString("MMMM"),
                LeaveBalanceHistoryList = LeaveBalanceHistoryList.OrderByDescending(x => x.BalanceDays).ToList(),
                BalanceMaxValue = LeaveBalanceHistoryList.Max(x => x.BalanceDays),
                BalanceMinValue = LeaveBalanceHistoryList.Min(x => x.BalanceDays),
                TakenMaxValue = LeaveBalanceHistoryList.Min(x => x.LeaveTakenDays),
                TakenMinValue = LeaveBalanceHistoryList.Min(x => x.LeaveTakenDays),
                AttendanceSummaryViewModel = AttendanceSummaryViewModel,
                applicationPendingViewModels = await EmployeeApplicationPendingList(EmployeeId),
                userRole = "",
                dashboardRoles = dashboardRole
            };

            return model;
        }

        public async Task<MidLevelDashboardViewModel> MidLevelDashboard(long EmployeeId, long AccountId)
        {
            DateTime yesterday = DateTime.Now.Date.AddDays(-1);
            DateTime Tommorrow = DateTime.Now.Date.AddDays(1);

            List<long> empIdList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == AccountId && x.IsActive == true && (x.IntDottedSupervisorId == EmployeeId || x.IntSupervisorId == EmployeeId || x.IntLineManagerId == EmployeeId)).Select(x => x.IntEmployeeBasicInfoId).ToListAsync();
            List<TimeAttendanceDailySummary> attDailySummary = _context.TimeAttendanceDailySummaries.Where(x => x.DteAttendanceDate == DateTime.Now.Date && empIdList.Contains(x.IntEmployeeId)).ToList();

            List<EmployeeQryProfileAllViewModel> employeeQryProfileAllList = await _iQRYDataForReportService.EmployeeQryProfileAllListBySupervisorORLineManagerId(EmployeeId);

            MidLevelDashboardViewModel model = new MidLevelDashboardViewModel
            {
                EmployeeQryProfileAllList = employeeQryProfileAllList,

                TodayPresent = attDailySummary.Where(x => x.IsPresent == true).Count(),
                TodayLate = attDailySummary.Where(x => x.IsLate == true).Count() <= 0 ? 0 : attDailySummary.Where(x => x.IsLate == true).Count(),
                TodayAbsent = attDailySummary.Where(x => x.IsAbsent == true).Count() <= 0 ? 0 : attDailySummary.Where(x => x.IsAbsent == true).Count(),

                //TodayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteFromDate.Date >= DateTime.Now.Date || x.DteToDate.Date <= DateTime.Now.Date)).Count(),
                //YesterdayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteFromDate.Date >= yesterday.Date || x.DteToDate.Date <= yesterday.Date)).Count(),
                //TommorrowMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteFromDate.Date >= Tommorrow.Date || x.DteToDate.Date <= Tommorrow.Date)).Count(),

                TodayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteCreatedAt.Date == DateTime.Now.Date)).Count(),
                YesterdayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteCreatedAt.Date == yesterday.Date)).Count(),
                TommorrowMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && (x.IsReject == false && x.IsPipelineClosed == false) && (x.DteCreatedAt.Date == Tommorrow.Date)).Count(),

                TommorrowLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && x.DteApplicationDate.Date == Tommorrow.Date && (x.IsReject == false && x.IsPipelineClosed == false)).Count(),
                TodayLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && x.DteApplicationDate.Date == DateTime.Now.Date && (x.IsReject == false && x.IsPipelineClosed == false)).Count(),
                YesterdayLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && empIdList.Contains(x.IntEmployeeId) && x.DteApplicationDate.Date == yesterday.Date && (x.IsReject == false && x.IsPipelineClosed == false)).Count(),
                employeeAttandanceListViewModels = await EmployeeTodayAttandanceList(AccountId, EmployeeId),
            };

            return model;
        }

        public async Task<TopLevelDashboardViewModel> TopLevelDashboard(long? EmployeeId, long AccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            TopLevelDashboardViewModel model = new TopLevelDashboardViewModel();

            //model.todayAttendanceViewModel = await AttendanceGraphData(AccountId, 1);
            model.leaveStatusViewModel = await LeaveGraphData(AccountId, BusinessUnitId, WorkplaceGroupId);
            model.movementStatusViewModel = await MovementGraphData(AccountId, BusinessUnitId, WorkplaceGroupId);
            model.upcomingBirthdayEmployeeList = await UpcomingBirthday(AccountId, BusinessUnitId, WorkplaceGroupId);
            model.departmentWiseEmployeeSalaryCount = await DepartmentWiseEmployeeSalary(AccountId, BusinessUnitId, WorkplaceGroupId);

            return model;
        }

        public async Task<List<ApplicationPendingViewModel>> EmployeeApplicationPendingList(long IntEmployeeId)
        {
            List<ApplicationPendingViewModel> pendingApplicaiton = new List<ApplicationPendingViewModel>();
            List<ApplicationPendingViewModel> pendingApp = new List<ApplicationPendingViewModel>();

            try
            {
                pendingApp = await _context.LveLeaveApplications.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Leave",
                        ApplicationDate = x.DteCreatedAt.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.LveMovementApplications.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Movement",
                        ApplicationDate = x.DteCreatedAt.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.PyrIouapplications.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "IOU",
                        ApplicationDate = x.DteCreatedAt.Value.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.EmpLoanApplications.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Loan",
                        ApplicationDate = x.DteCreatedAt.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.TimeEmpOverTimes.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Over Time",
                        ApplicationDate = x.DteCreatedAt.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.PyrEmpSalaryAdditionNdeductions.
                    Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Salary Addition & Deduction",
                        ApplicationDate = x.DteCreatedAt.Value.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.TimeEmployeeAttendances.
                    Where(x => (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                    .Select(x => new ApplicationPendingViewModel
                    {
                        ApplicationType = "Remote Attendance",
                        ApplicationDate = x.DteAttendanceDate.Value.Date,
                        ApprovalStatus = x.StrStatus
                    }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApp = await _context.EmpManualAttendanceSummaries.
                   Where(x => x.IsActive == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                   .Select(x => new ApplicationPendingViewModel
                   {
                       ApplicationType = "Manual Attendance",
                       ApplicationDate = x.DteCreatedAt.Date,
                       ApprovalStatus = x.StrStatus
                   }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                //Location Register
                pendingApp = await _context.TimeRemoteAttendanceRegistrations.
                   Where(x => x.IsActive == true && x.IsLocationRegister == true && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                   .Select(x => new ApplicationPendingViewModel
                   {
                       ApplicationType = "Location Registration",
                       ApplicationDate = x.DteInsertDate.Date,
                       ApprovalStatus = x.StrStatus
                   }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                //Device Register
                pendingApp = await _context.TimeRemoteAttendanceRegistrations.
                   Where(x => x.IsActive == true && x.IsLocationRegister == false && (x.IsReject == false && x.IsPipelineClosed == false && x.StrStatus.ToLower() == "Pending".ToLower()) && x.IntEmployeeId == IntEmployeeId)
                   .Select(x => new ApplicationPendingViewModel
                   {
                       ApplicationType = "Device Registration",
                       ApplicationDate = x.DteInsertDate.Date,
                       ApprovalStatus = x.StrStatus
                   }).ToListAsync();
                pendingApplicaiton.AddRange(pendingApp);

                pendingApplicaiton = pendingApplicaiton.OrderBy(x => x.ApplicationType).ThenByDescending(x => x.ApplicationDate).ToList();

                return pendingApplicaiton;
            }
            catch (Exception)
            {
                return new List<ApplicationPendingViewModel>();
            }
        }

        //public async Task<List<DashboardComponent>> DashboardComponentViewlanding(long? IntAccountId, long? IntEmployeeId)
        //{
        //    List<DashboardComponent> AccountComponents = new List<DashboardComponent>();

        //    AccountComponents = await (from dashB in _context.MasterDashboardComponents
        //                                where dashB.IsActive == true
        //                                join p in _context.MasterDashboardComponentPermissions on dashB.IntId equals p.IntDashboardId
        //                                where p.IsActive == true && p.IntForAccountOremp == 1 && p.IntAccountId == IntAccountId
        //                                select new DashboardComponent
        //                                {
        //                                    IntPermissionId = p.IntId,
        //                                    IntDashboardId = dashB.IntId,
        //                                    StrDisplayName = dashB.StrDisplayName,
        //                                    StrHashCode = p.StrHashCode,
        //                                    IntAccountId = p.IntAccountId,
        //                                    IntEmployeeId = p.IntEmployeeId,
        //                                    IsActive = p.IsActive
        //                                }).ToListAsync();

        //    List<DashboardComponent> components = new List<DashboardComponent>();
        //    components = await (from dashB in _context.MasterDashboardComponents
        //                        where dashB.IsActive == true
        //                        join p in _context.MasterDashboardComponentPermissions on dashB.IntId equals p.IntDashboardId
        //                        where p.IsActive == false && p.IntForAccountOremp == 2 && p.IntEmployeeId == IntEmployeeId
        //                        select new DashboardComponent
        //                        {
        //                            IntPermissionId = p.IntId,
        //                            IntDashboardId = dashB.IntId,
        //                            StrDisplayName = dashB.StrDisplayName,
        //                            StrHashCode = p.StrHashCode,
        //                            IntAccountId = p.IntAccountId,
        //                            IntEmployeeId = p.IntEmployeeId,
        //                            IsActive = p.IsActive
        //                        }).ToListAsync();

        //    List<DashboardComponent> result = AccountComponents.Where(p => components.All(p2 => p2.StrHashCode != p.StrHashCode)).ToList();

        //    return result;
        //}
        //public async Task<MessageHelper> DashboardComponentShowHide(DashboardComponent component)
        //{
        //    MessageHelper msg = new MessageHelper();
        //    if (component != null)
        //    {
        //        if (_context.MasterDashboardComponentPermissions.Where(x => x.IntEmployeeId == component.IntEmployeeId && x.StrHashCode == component.StrHashCode).Count() > 0)
        //        {
        //            //Update
        //            MasterDashboardComponentPermission componentPermission = new MasterDashboardComponentPermission()
        //            {
        //                IntId = (long)component.IntPermissionId,
        //                IntDashboardId = (long)component.IntDashboardId,
        //                StrHashCode = component.StrHashCode,
        //                IntForAccountOremp = 2,
        //                IntEmployeeId = component.IntEmployeeId,
        //                IsActive = component.IsActive,
        //                DteUpdatedAt = DateTime.Now,
        //                IntUpdatedBy = (long)component.IntEmployeeId
        //            };
        //            _context.MasterDashboardComponentPermissions.Update(componentPermission);
        //            await _context.SaveChangesAsync();

        //            if (component.IsActive == true)
        //            {
        //                msg.Message = "Component Show Successfully";
        //                msg.StatusCode = 200;
        //            }
        //            else
        //            {
        //                msg.Message = "Component Hide Successfully";
        //                msg.StatusCode = 201;
        //            }

        //        }
        //        else
        //        {
        //            //Insert
        //            MasterDashboardComponentPermission componentPermission = new MasterDashboardComponentPermission()
        //            {
        //                IntDashboardId = (long)component.IntDashboardId,
        //                StrHashCode = component.StrHashCode,
        //                IntForAccountOremp = 2,
        //                IntEmployeeId = component.IntEmployeeId,
        //                IsActive = component.IsActive,
        //                DteCreatedAt = DateTime.Now,
        //                IntCreatedBy = (long)component.IntEmployeeId
        //            };
        //            _context.MasterDashboardComponentPermissions.Add(componentPermission);
        //            await _context.SaveChangesAsync();

        //            msg.Message = "Component Hide Successfully";
        //            msg.StatusCode = 203;

        //        }
        //    }

        //    return msg;
        //}

        public async Task<DataTable> ConvertTimeToHours(DateTime? date, long IntAccountId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprConvertTimeToHours";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@Date", date);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", IntAccountId);

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

        public async Task<List<EmployeeAttandanceListViewModel>> EmployeeTodayAttandanceList(long IntAccountId, long EmployeeId)
        {

            List<EmployeeAttandanceListViewModel> employeeAttandanceList = new List<EmployeeAttandanceListViewModel>();

            employeeAttandanceList = await (from emp in _context.EmpEmployeeBasicInfos
                                            join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into desi
                                            from desig in desi.DefaultIfEmpty()
                                            join dep in _context.MasterDepartments on emp.IntDepartmentId equals dep.IntDepartmentId into depa
                                            from depar in depa.DefaultIfEmpty()
                                            join attend in _context.TimeAttendanceDailySummaries on emp.IntEmployeeBasicInfoId equals attend.IntEmployeeId into att
                                            from attendS in att.DefaultIfEmpty()
                                            where emp.IsActive == true && emp.IntAccountId == IntAccountId
                                            && (attendS.IntMonthId == null || attendS.IntMonthId == DateTime.Now.Month)
                                            && (attendS.IntYear == null || attendS.IntYear == DateTime.Now.Year)
                                            && (attendS.IntDayId == null || attendS.IntDayId == DateTime.Now.Day)
                                            && (emp.IntSupervisorId == EmployeeId || emp.IntDottedSupervisorId == EmployeeId || emp.IntLineManagerId == EmployeeId)
                                            select new EmployeeAttandanceListViewModel
                                            {
                                                EmployeeId = emp.IntEmployeeBasicInfoId,
                                                EmployeeName = emp.StrEmployeeName,
                                                EmployeeCode = emp.StrEmployeeCode,
                                                EmployeeProfileUrlId = _context.EmpEmployeePhotoIdentities.Where(x => x.IsActive == true && x.IntEmployeeBasicInfoId == emp.IntEmployeeBasicInfoId).Select(x => x.IntProfilePicFileUrlId).FirstOrDefault(),
                                                DepartmentId = emp.IntDepartmentId,
                                                Departmant = depar == null ? "" : depar.StrDepartment,
                                                DesignationId = emp.IntDesignationId,
                                                Designation = desig == null ? "" : desig.StrDesignation,
                                                StrWorkingHours = attendS == null ? null : attendS.StrWorkingHours,
                                                InTime = attendS == null ? null : attendS.TmeInTime,
                                                OutTime = attendS == null ? null : attendS.TmeLastOutTime,
                                                Status = attendS == null ? "" : attendS.IsLate == true ? "Late" : attendS.IsPresent == true ? "Present" : attendS.IsAbsent == true ? "Absent" : attendS.IsLeave == true ? "Leave" : attendS.IsMovement == true ? "Movement" : attendS.IsHoliday == true ? "Holiday" : attendS.IsOffday == true ? "Offday" : "Absent"
                                            }).AsNoTracking().OrderBy(x => x.EmployeeId).ToListAsync();

            return employeeAttandanceList;
        }

        public async Task<EmployeeStatusViewModel> EmployeeStatusGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<EmpEmployeeBasicInfo> empEmployees = new List<EmpEmployeeBasicInfo>();

            if (IntAccountId > 0)
            {
                if (IntYear == DateTime.Now.Year)
                {
                    //empEmployees = await _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId && x.DteJoiningDate.Value.Date <= DateTime.UtcNow.Date).ToListAsync();

                    empEmployees = await (from e in _context.EmpEmployeeBasicInfos
                                          where e.IsActive == true && e.IntAccountId == IntAccountId && e.IntBusinessUnitId == BusinessUnitId && e.IntWorkplaceGroupId == WorkplaceGroupId
                                          join ed in _context.EmpEmployeeBasicInfoDetails on e.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                          from edtl in edetails.DefaultIfEmpty()
                                          where e.DteJoiningDate.Value.Date <= DateTime.UtcNow.Date
                                          && (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                          select e).ToListAsync();
                }
                else
                {
                    //empEmployees = await _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId && x.DteJoiningDate.Value.Year <= IntYear).ToListAsync();

                    empEmployees = await (from e in _context.EmpEmployeeBasicInfos
                                          where e.IsActive == true && e.IntAccountId == IntAccountId && e.IntBusinessUnitId == BusinessUnitId && e.IntWorkplaceGroupId == WorkplaceGroupId
                                          join ed in _context.EmpEmployeeBasicInfoDetails on e.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                          from edtl in edetails.DefaultIfEmpty()
                                          where e.DteJoiningDate.Value.Year <= IntYear
                                          && (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                          select e).ToListAsync();
                }

                List<EmployeeStatusGraphViewModel> employeeStatusGraph = empEmployees.GroupBy(x => x.StrEmploymentType)
                    .Select(y => new EmployeeStatusGraphViewModel { GraphText = y.Key, GraphValue = y.Count() }).ToList();

                long? totalEmployee = empEmployees.Count();
                long? male = empEmployees.Where(x => x.IntGenderId == 1).Count();
                long? female = empEmployees.Where(x => x.IntGenderId == 2).Count();

                EmployeeStatusViewModel employeeStatus = new EmployeeStatusViewModel()
                {
                    TotalEmployee = totalEmployee,
                    TotalMale = male,
                    TotalFemale = female,
                    MalePercentage = Math.Round((decimal)male * 100 / (decimal)totalEmployee, 2),
                    FemalePercentage = Math.Round((decimal)female * 100 / (decimal)totalEmployee, 2),
                    employeeStatusGraphs = employeeStatusGraph
                };
                return employeeStatus;
            }
            else
            {
                EmployeeStatusViewModel employeeStatus = new EmployeeStatusViewModel();
                return employeeStatus;
            }
        }

        public async Task<AttendanceSummaryViewModel> GetAttendanceSummaryCalenderReport(long EmployeeId, long Month, long Year)
        {
            try
            {
                AttendanceSummaryViewModel AttSummaryObject = new AttendanceSummaryViewModel();
                List<TimeAttendanceDailySummary> timeAttendances = await _context.TimeAttendanceDailySummaries.AsNoTracking().AsQueryable().Where(x => x.DteAttendanceDate.Value.Date.Year == Year && x.DteAttendanceDate.Value.Date.Month == Month && x.IntEmployeeId == EmployeeId).OrderBy(x => x.IntDayId).ToListAsync();

                if (DateTime.UtcNow.Month == Month && DateTime.UtcNow.Year == Year)
                {
                    AttSummaryObject.timeAttendanceDailySummaries = timeAttendances.Where(x => x.DteAttendanceDate.Value.Day <= DateTime.UtcNow.Day).OrderByDescending(s => s.DteAttendanceDate).ToList();
                }
                else
                {
                    AttSummaryObject.timeAttendanceDailySummaries = timeAttendances;
                }

                if (AttSummaryObject?.timeAttendanceDailySummaries.Count() > 0)
                {
                    List<AttendanceDailySummaryViewModel> objList =
                    timeAttendances.Select(x => new AttendanceDailySummaryViewModel
                    {
                        dteDate = x.DteAttendanceDate.Value.Date,
                        DayName = x.DteAttendanceDate.Value.DayOfWeek.ToString(),
                        DayNumber = (int)x.IntDayId,
                        presentStatus = DateTime.UtcNow.Date < new DateTime((int)Year, (int)Month, (int)x.IntDayId).Date ? ""
                        : ((x.IsManual == true && x.IsManualPresent == true) || (x.IsPresent == true && (x.IsManual == false || x.IsManual == null))) ? "Present"
                        : ((x.IsManual == true && x.IsManualLate == true) || (x.IsLate == true && (x.IsManual == false || x.IsManual == null))) ? "Late"
                        : ((x.IsManual == true && x.IsManualAbsent == true) || (x.IsAbsent == true && (x.IsManual == false || x.IsManual == null))) ? "Absent"
                        : ((x.IsManual == true && x.IsManualLeave == true) || (x.IsLeave == true && (x.IsManual == false || x.IsManual == null))) ? "Leave"
                        : x.IsMovement == true ? "Movement"
                        : x.IsHoliday == true ? "Holiday"
                        : x.IsOffday == true ? "Offday"
                        : x.DteAttendanceDate.Value.Date == DateTime.Now.Date && x.IsLate == false && x.IsPresent == false && x.IsAbsent == false && x.IsLeave == false && x.IsMovement == false ? ""
                        : "Absent"
                    }).ToList();

                    var minDate = objList.OrderBy(x => x.dteDate).Select(x => x.dteDate).FirstOrDefault();
                    if (minDate != null)
                    {
                        for (int index = minDate.Value.Day - 1; index > 0; index--)
                        {
                            objList.Add(new AttendanceDailySummaryViewModel
                            {
                                dteDate = new DateTime(minDate.Value.Year, minDate.Value.Month, index),
                                DayName = new DateTime(minDate.Value.Year, minDate.Value.Month, index).DayOfWeek.ToString(),
                                DayNumber = index,
                                presentStatus = ""
                            });
                        }
                        objList = objList.OrderBy(x => x.dteDate).ToList();
                    }

                    AttSummaryObject.attendanceDailySummaryViewModel = objList;
                    AttSummaryObject.WorkingDays = AttSummaryObject.timeAttendanceDailySummaries.Where(x => x.IsWorkingDayCal == 1 && x.DteAttendanceDate.Value.Date <= DateTime.Now.Date).Count();
                    AttSummaryObject.PresentDays = AttSummaryObject.timeAttendanceDailySummaries.Where(x => (x.IsManual == true && x.IsManualPresent == true) || (x.IsPresent == true && (x.IsManual == false || x.IsManual == null))).Count();
                    AttSummaryObject.LateDays = AttSummaryObject.timeAttendanceDailySummaries.Where(x => (x.IsManual == true && x.IsManualLate == true) || (x.IsLate == true && (x.IsManual == false || x.IsManual == null))).Count();
                    AttSummaryObject.AbsentDays = AttSummaryObject.timeAttendanceDailySummaries.Where(y => (y.IsManual == true && y.IsManualAbsent == true) || (y.IsAbsent == true && (y.IsManual == false || y.IsManual == null))).Count();
                    AttSummaryObject.MovementDays = AttSummaryObject.timeAttendanceDailySummaries.Where(x => x.IsMovement == true && x.IsPresent == false).Count();
                    AttSummaryObject.LeaveDays = AttSummaryObject.timeAttendanceDailySummaries.Where(x => (x.IsManual == true && x.IsManualLeave == true) || (x.IsLeave == true && (x.IsManual == false || x.IsManual == null))).Count();
                }

                return AttSummaryObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DepartmentWiseEmployeeSalaryCountViewModel>> DepartmentWiseEmployeeSalary(long IntAccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                List<DepartmentWiseEmployeeSalaryCountViewModel> employeeSalaryCount = new List<DepartmentWiseEmployeeSalaryCountViewModel>();

                List<EmployeeWithSalaryList> EmployeeWithSalaryList = await (from emp in _context.EmpEmployeeBasicInfos
                                                                             join salary in _context.PyrEmployeeSalaryElementAssignHeaders on emp.IntEmployeeBasicInfoId equals salary.IntEmployeeId into sal1
                                                                             from sal in sal1.DefaultIfEmpty()
                                                                             where emp.IsActive == true && emp.IntAccountId == IntAccountId && sal.IsActive == true && emp.IntDepartmentId > 0
                                                                             && emp.IntWorkplaceGroupId == WorkplaceGroupId && emp.IntBusinessUnitId == BusinessUnitId
                                                                             select new EmployeeWithSalaryList
                                                                             {
                                                                                 Emp = emp,
                                                                                 Salary = sal
                                                                             }).ToListAsync();

                List<MasterDepartment> departmentList = await _context.MasterDepartments.Where(x => x.IntAccountId == IntAccountId && x.IsActive == true).ToListAsync();

                departmentList.ForEach(d =>
                {
                    employeeSalaryCount.Add(new DepartmentWiseEmployeeSalaryCountViewModel
                    {
                        DepartmentId = (long)d.IntDepartmentId,
                        Department = d.StrDepartment,
                        EmployeeCount = EmployeeWithSalaryList.Where(x => (long)x.Emp.IntDepartmentId == d.IntDepartmentId).Count(),
                        Salary = EmployeeWithSalaryList.Where(x => (long)x.Emp.IntDepartmentId == d.IntDepartmentId).Sum(x => (decimal)x?.Salary?.NumGrossSalary)
                    });
                });

                return employeeSalaryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MonthWiseLeaveTakenViewModel>> MonthWiseLeaveTakenGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<MonthWiseLeaveTakenViewModel> monthWiseLeaveTaken = new List<MonthWiseLeaveTakenViewModel>();

            var monthlyLeave = await (from la in _context.LveLeaveApplications
                                      join detail in _context.EmpEmployeeBasicInfos
                                      on la.IntEmployeeId equals detail.IntEmployeeBasicInfoId
                                      into DetailGroup
                                      from empLeave in DetailGroup.DefaultIfEmpty()
                                      where la.IsActive == true && la.IsReject == false && la.IsPipelineClosed == true
                                      && la.IntAccountId == IntAccountId && la.DteApplicationDate.Year == IntYear
                                      && la.IntBusinessUnitId == BusinessUnitId && empLeave.IntWorkplaceGroupId == WorkplaceGroupId
                                      group la by la.DteApplicationDate.Month into g
                                      select new
                                      {
                                          month = g.Key,
                                          leavecount = g.Count()
                                      }).AsNoTracking().ToListAsync();


            for (int i = 1; i <= 12; i++)
            {
                monthWiseLeaveTaken.Add(new MonthWiseLeaveTakenViewModel
                {
                    MonthId = i,
                    LeaveCount = 0
                });
            }

            foreach (var ml in monthWiseLeaveTaken)
            {
                var leave = monthlyLeave.FirstOrDefault(d => d.month == ml.MonthId);

                if (leave != null)
                {
                    ml.LeaveCount = leave.leavecount;
                }
            }

            return monthWiseLeaveTaken;
        }

        public async Task<List<MonthWiseIOUViewModel>> MonthWiseIOUGraph(long IntYear, long IntAccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<MonthWiseIOUViewModel> monthWiseIOU = new List<MonthWiseIOUViewModel>();

            List<long> empList = await _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId && x.IntWorkplaceGroupId == WorkplaceGroupId && x.IntBusinessUnitId == BusinessUnitId).Select(x => x.IntEmployeeBasicInfoId).ToListAsync();
            var monthlyIOU = await _context.PyrIouapplications.Where(x => x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true && x.DteApplicationDate.Year == IntYear && empList.Contains(x.IntEmployeeId))
                .GroupBy(x => x.DteApplicationDate.Month).Select(s => new { MonthId = s.Key, Iou = s.Sum(x => x.NumIouamount) }).ToListAsync();

            for (int i = 1; i <= 12; i++)
            {
                monthWiseIOU.Add(new MonthWiseIOUViewModel
                {
                    MonthId = i,
                    IOU = 0
                });
            }

            foreach (var iou in monthWiseIOU)
            {
                var IouAmount = monthlyIOU.FirstOrDefault(d => d.MonthId == iou.MonthId);

                if (IouAmount != null)
                {
                    iou.IOU = (decimal?)IouAmount.Iou;
                }
            }

            return monthWiseIOU;
        }

        public async Task<DepartmentWiseEmployeeTurnoverRatioViewModel> EmployeeTurnOverRatio(long IntAccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                DepartmentWiseEmployeeTurnoverRatioViewModel EmployeeTurnoverRatioViewModels = new DepartmentWiseEmployeeTurnoverRatioViewModel();

                List<MasterDepartment> departmentList = await _context.MasterDepartments.Where(x => x.IntAccountId == IntAccountId && x.IsActive == true).ToListAsync();

                List<DepartmentWiseTurnoverRateViewModel> departmentWiseTurnoverRate = new List<DepartmentWiseTurnoverRateViewModel>();
                decimal currentEmp = 0, LastYearEmp = 0;

                departmentList.ForEach(d =>
                {
                    var employeeRatio = LastYearEmployeeTrunOverRatio(IntAccountId, d.IntDepartmentId, BusinessUnitId, WorkplaceGroupId);

                    departmentWiseTurnoverRate.Add(new DepartmentWiseTurnoverRateViewModel
                    {
                        DepartmentId = d.IntDepartmentId,
                        DepartmentName = d.StrDepartment,
                        LastYearEmployee = (long)employeeRatio.Result.Item1,
                        CurrentYearEmployee = (long)employeeRatio.Result.Item2,
                        TurnoverRatio = (employeeRatio.Result.Item1 == 0 && employeeRatio.Result.Item2 == 0 && employeeRatio.Result.Item3 == 0) ? 0 : Math.Round((employeeRatio.Result.Item3 / ((employeeRatio.Result.Item1 + employeeRatio.Result.Item2) / 2)) * 100, 2)
                    });

                    LastYearEmp += employeeRatio.Result.Item1;
                    currentEmp += employeeRatio.Result.Item2;

                    EmployeeTurnoverRatioViewModels.TotalEmployee += (long)employeeRatio.Result.Item2;
                    EmployeeTurnoverRatioViewModels.TotalLeft += (long)employeeRatio.Result.Item3;
                    //EmployeeTurnoverRatioViewModels.TurnoverRate += LastYearEmp == currentEmp ? 0 : (((employeeRatio.Result.Item1 - employeeRatio.Result.Item2) / ((employeeRatio.Result.Item1 + employeeRatio.Result.Item2) / 2)) * 100);
                });

                EmployeeTurnoverRatioViewModels.TurnoverRate = Math.Round((EmployeeTurnoverRatioViewModels.TotalLeft / ((LastYearEmp + currentEmp) / 2)) * 100, 2);

                EmployeeTurnoverRatioViewModels.departmentWiseTurnoverRateViewModel = departmentWiseTurnoverRate;

                return EmployeeTurnoverRatioViewModels;
            }
            catch (Exception ex)
            {
                return new DepartmentWiseEmployeeTurnoverRatioViewModel();
            }
        }

        public async Task<List<LastFiveYearsTurnoverRatioViewModel>> LastFiveYearsTurnoverRatio(long AccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                List<LastFiveYearsTurnoverRatioViewModel> turnoverRatioViewModel = new List<LastFiveYearsTurnoverRatioViewModel>();
                for (int y = 4; y >= 0; y--)
                {
                    decimal openingValue = 0;
                    decimal currentEmployeeValue = 0;
                    decimal trunOverValue = 0;
                    DateTime periodYear = DateTime.Now.AddYears(-y);

                    DateTime FirstDateOfTheYear = new DateTime(periodYear.Year, 1, 1);
                    DateTime LastDateOfTheYear = new DateTime(periodYear.Year, 12, 31);

                    openingValue = (from emp in _context.EmpEmployeeBasicInfos
                                    where emp.IntAccountId == AccountId && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                    join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                    from edtl in edetails.DefaultIfEmpty()
                                    where (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                    && emp.DteJoiningDate.Value.Date <= FirstDateOfTheYear.Date
                                    && ((emp.DteLastWorkingDate == null && emp.IsActive == true)
                                        || emp.DteLastWorkingDate.Value.Date > FirstDateOfTheYear.Date)
                                    select emp).Count();

                    currentEmployeeValue = (from emp in _context.EmpEmployeeBasicInfos
                                            where emp.IntAccountId == AccountId && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                            join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                            from edtl in edetails.DefaultIfEmpty()
                                            where (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                            && emp.DteJoiningDate.Value.Date <= LastDateOfTheYear.Date
                                            && emp.DteJoiningDate.Value.Date >= FirstDateOfTheYear.Date
                                            && ((emp.DteLastWorkingDate == null && emp.IsActive == true)
                                                || emp.DteLastWorkingDate.Value.Date > LastDateOfTheYear.Date)
                                            select emp).Count();

                    trunOverValue = (from emp in _context.EmpEmployeeBasicInfos
                                     where emp.IntAccountId == AccountId && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                     join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                     from edtl in edetails.DefaultIfEmpty()
                                     where ((emp.DteLastWorkingDate.Value.Year == periodYear.Year || emp.IsActive == false)
                                     || (edtl.DteUpdatedAt.Value.Year == periodYear.Year
                                     && (edtl.IntEmployeeStatusId == 2 || edtl.IntEmployeeStatusId == 3)))
                                     select emp).Count();

                    turnoverRatioViewModel.Add(new LastFiveYearsTurnoverRatioViewModel
                    {
                        Years = periodYear.Year,
                        YearlyTurnover = (openingValue == 0 && currentEmployeeValue == 0 && trunOverValue == 0) ? 0 : Math.Round((trunOverValue / ((openingValue + currentEmployeeValue) / 2)) * 100, 2)
                    });
                }
                return turnoverRatioViewModel;
            }
            catch (Exception ex)
            {
                return new List<LastFiveYearsTurnoverRatioViewModel>();
            }
        }

        public async Task<(decimal, decimal, decimal)> LastYearEmployeeTrunOverRatio(long accountId, long deptId, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                decimal openingValue = 0;
                decimal currentEmployeeValue = 0;
                decimal trunOverValue = 0;

                DateTime FirstDateOfTheYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
                DateTime LastDateOfTheYear = new DateTime(DateTime.UtcNow.Year, 12, 31);

                //openingValue = (from emp in _context.EmpEmployeeBasicInfos
                //                where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId && emp.DteJoiningDate.Value.Year < DateTime.Now.Year
                //                && ((emp.DteLastWorkingDate == null && emp.IsActive == true)
                //                    || emp.DteLastWorkingDate.Value.Year == DateTime.Now.Year)
                //                select emp).Count();

                openingValue = (from emp in _context.EmpEmployeeBasicInfos
                                where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId
                                && emp.IntBusinessUnitId == BusinessUnitId
                                && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                from edtl in edetails.DefaultIfEmpty()
                                where (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                && emp.DteJoiningDate.Value.Date <= FirstDateOfTheYear.Date
                                && ((emp.DteLastWorkingDate == null && emp.IsActive == true)
                                    || emp.DteLastWorkingDate.Value.Date > FirstDateOfTheYear.Date)                   
                                select emp).Count();

                //currentEmployeeValue = (from emp in _context.EmpEmployeeBasicInfos
                //                        where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId
                //                        && emp.DteLastWorkingDate == null && emp.IsActive == true
                //                        select emp).Count();

                currentEmployeeValue = (from emp in _context.EmpEmployeeBasicInfos
                                        where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId
                                        && emp.IntBusinessUnitId == BusinessUnitId
                                        && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                        join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                        from edtl in edetails.DefaultIfEmpty()
                                        where (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                        && emp.DteLastWorkingDate == null && emp.IsActive == true
                                        select emp).Count();

                //trunOverValue = (from emp in _context.EmpEmployeeBasicInfos
                //                 where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId
                //                 && (emp.DteLastWorkingDate.Value.Year == DateTime.Now.Year || emp.IsActive == false)
                //                 select emp).Count();

                trunOverValue = (from emp in _context.EmpEmployeeBasicInfos
                                 where emp.IntAccountId == accountId && emp.IntDepartmentId == deptId
                                 && emp.IntBusinessUnitId == BusinessUnitId
                                 && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                 join ed in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                 from edtl in edetails.DefaultIfEmpty()
                                 where ((emp.DteLastWorkingDate.Value.Year == DateTime.Now.Year || emp.IsActive == false)
                                 || (edtl.DteUpdatedAt.Value.Year == DateTime.Now.Year
                                 && (edtl.IntEmployeeStatusId == 2 || edtl.IntEmployeeStatusId == 3)))
                                 select emp).Count();

                if (openingValue == 0 && currentEmployeeValue == 0)
                {
                    trunOverValue = 0;
                }

                return (openingValue, currentEmployeeValue, trunOverValue);
            }
            catch (Exception)
            {
                return (0, 0, 0);
            }
        }

        public async Task<SalaryRangeViewModel> EmployeeCountBySalaryRange(long AccountId, long? MinSalary, long? MaxSalary, long BusinessUnitId, long WorkplaceGroupId)
        {
            // List<PyrEmployeeSalaryElementAssignHeader> salaryElementAssignHeader = await _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IsActive == true && x.IntAccountId == AccountId).ToListAsync();

            List<PyrEmployeeSalaryElementAssignHeader> salaryElementAssignHeader = await (from sal in _context.PyrEmployeeSalaryElementAssignHeaders
                                                                                          join emp in _context.EmpEmployeeBasicInfos on sal.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                                          where sal.IsActive == true && sal.IntAccountId == AccountId && sal.IntBusinessUnitId == BusinessUnitId
                                                                                          && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                                                                          select new PyrEmployeeSalaryElementAssignHeader
                                                                                          {
                                                                                              NumGrossSalary = sal.NumGrossSalary,
                                                                                              IntEmployeeId = sal.IntEmployeeId
                                                                                          }).ToListAsync();


            SalaryRangeViewModel salaryRangeViewModel = new SalaryRangeViewModel();

            if (MinSalary != null && MaxSalary != null)
            {
                salaryRangeViewModel.FixedMinimumSalary = salaryElementAssignHeader.Min(x => x.NumGrossSalary);
                salaryRangeViewModel.FixedMaximumSalary = salaryElementAssignHeader.Max(x => x.NumGrossSalary);
                salaryRangeViewModel.MinimumSalary = (decimal)MinSalary;
                salaryRangeViewModel.MaximumSalary = (decimal)MaxSalary;
                salaryRangeViewModel.NumberOfEmployee = salaryElementAssignHeader.Where(s => s.NumGrossSalary >= MinSalary && s.NumGrossSalary <= MaxSalary).Count();
            }
            else if (MaxSalary == null && MinSalary == null)
            {
                salaryRangeViewModel.FixedMinimumSalary = salaryElementAssignHeader.Min(x => x.NumGrossSalary);
                salaryRangeViewModel.FixedMaximumSalary = salaryElementAssignHeader.Max(x => x.NumGrossSalary);
                salaryRangeViewModel.MinimumSalary = salaryElementAssignHeader.Min(x => x.NumGrossSalary);
                salaryRangeViewModel.MaximumSalary = salaryElementAssignHeader.Max(x => x.NumGrossSalary);
                salaryRangeViewModel.NumberOfEmployee = salaryElementAssignHeader.Select(s => s.IntEmployeeId).Count();
            }

            return salaryRangeViewModel;
        }

        public async Task<List<UpcomingBirthdayEmployeeViewModel>> UpcomingBirthday(long AccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<UpcomingBirthdayEmployeeViewModel> UpcomingBirthday = _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteDateOfBirth.Value.Month == DateTime.Now.Month && x.DteDateOfBirth.Value.Day >= DateTime.Now.Day
                                                                                                            && x.IntBusinessUnitId == BusinessUnitId && x.IntWorkplaceGroupId == WorkplaceGroupId)
            .Select(y => new UpcomingBirthdayEmployeeViewModel
            {
                EmployeeId = y.IntEmployeeBasicInfoId,
                EmployeeName = y.StrEmployeeName,
                DateOfBirth = y.DteDateOfBirth,
                ProfileUrl = _context.EmpEmployeePhotoIdentities.Where(x => x.IsActive == true && x.IntEmployeeBasicInfoId == y.IntEmployeeBasicInfoId).Select(x => x.IntProfilePicFileUrlId).FirstOrDefault(),
                Department = _context.MasterDepartments.Where(x => x.IsActive == true && x.IntDepartmentId == y.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                Designation = _context.MasterDesignations.Where(x => x.IsActive == true && x.IntDesignationId == y.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault()
            }).OrderBy(z => z.DateOfBirth.Value.Day).ToList();

            return UpcomingBirthday;
        }

        #region ================== GRAPH DATA ALL ============================

        public async Task<DepartmentWiseSalaryGraphViewModel> DepartmentWiseSalaryGraph(long? BusinessUnitId)
        {
            List<DepartmentWiseSalaryGraphDataViewModel> DepartmentWiseSalaryGraphDataViewModelList = new List<DepartmentWiseSalaryGraphDataViewModel>();
            List<EmpEmployeeBasicInfo> EmployeeBasicInfoList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true).ToListAsync();
            List<MasterDepartment> TblEmpDepartmentList = _context.MasterDepartments.Where(x => x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true).ToList();

            List<CatagoriesViewModel> caltegoriesList = new List<CatagoriesViewModel>();
            List<DatasViewModel> dataList = new List<DatasViewModel>();

            foreach (var dept in TblEmpDepartmentList)
            {
                DepartmentWiseSalaryGraphDataViewModel newObj = new DepartmentWiseSalaryGraphDataViewModel
                {
                    DepartmentName = dept.StrDepartment,
                    SalaryRangeList = (from emp in EmployeeBasicInfoList
                                       where emp.IntDepartmentId == dept.IntDepartmentId
                                       join defaultSalary in _context.PyrEmployeeSalaryDefaults on emp.IntEmployeeBasicInfoId equals defaultSalary.IntEmployeeId
                                       group defaultSalary by defaultSalary.NumGrossSalary into grossSalary
                                       select grossSalary.Key).ToList()
                };

                decimal? deptBasedTotalSalary = (from emp in EmployeeBasicInfoList
                                                 where emp.IntDepartmentId == dept.IntDepartmentId
                                                 join defaultSalary in _context.PyrEmployeeSalaryDefaults on emp.IntEmployeeBasicInfoId equals defaultSalary.IntEmployeeId
                                                 select defaultSalary.NumGrossSalary).Sum();

                newObj.DeptBasedNumberOfEmployeeCount = EmployeeBasicInfoList.Where(x => x.IntDepartmentId == dept.IntDepartmentId).Count();
                newObj.DeptBasedAvarageSalary = (deptBasedTotalSalary > 0 && newObj.DeptBasedNumberOfEmployeeCount > 0) ? (deptBasedTotalSalary / newObj.DeptBasedNumberOfEmployeeCount) : 0;

                newObj.DepartmentBaseMaxSalary = newObj.SalaryRangeList.Count() > 0 ? newObj.SalaryRangeList.Max(x => x.Value) : 0;
                newObj.DepartmentBaseMinSalary = newObj.SalaryRangeList.Count() > 0 ? newObj.SalaryRangeList.Min(x => x.Value) : 0;
                DepartmentWiseSalaryGraphDataViewModelList.Add(newObj);

                CatagoriesViewModel catagoriesViewModel = new CatagoriesViewModel
                {
                    index = Convert.ToInt64(newObj.DeptBasedAvarageSalary),
                    name = dept.StrDepartment
                };
                DatasViewModel datasViewModel = new DatasViewModel
                {
                    index = Convert.ToInt64(newObj.DeptBasedAvarageSalary),
                    value = newObj.DeptBasedAvarageSalary,
                };

                caltegoriesList.Add(catagoriesViewModel);
                dataList.Add(datasViewModel);
            }

            // ========================== custom data set for graph ====================================

            int lastIndex = DepartmentWiseSalaryGraphDataViewModelList.Max(x => x.SalaryRangeList.Count());
            List<CustomGraphDataSet> CustomGraphDataSetList = new List<CustomGraphDataSet>();

            for (int i = 0; i < lastIndex; i++)
            {
                CustomGraphDataSet obj = new CustomGraphDataSet();

                obj.title = "Salary";
                List<decimal?> datas = new List<decimal?>();

                foreach (var dept in TblEmpDepartmentList)
                {
                    int indexIsExists = DepartmentWiseSalaryGraphDataViewModelList.FirstOrDefault(x => x.DepartmentName == dept.StrDepartment).SalaryRangeList.Count();
                    if (indexIsExists > i)
                    {
                        decimal? value = DepartmentWiseSalaryGraphDataViewModelList.FirstOrDefault(x => x.DepartmentName == dept.StrDepartment).SalaryRangeList[i].Value;
                        datas.Add(value);
                    }
                    else
                    {
                        datas.Add(0);
                    }
                }
                obj.datas = datas;
                CustomGraphDataSetList.Add(obj);
            }

            // ========================== END ====================================

            DepartmentWiseSalaryGraphViewModel departmentWiseSalaryGraphViewModel = new DepartmentWiseSalaryGraphViewModel
            {
                SalaryRangeFrom = await _context.PyrEmployeeSalaryDefaults.MaxAsync(x => x.NumGrossSalary),
                SalaryRangeTo = await _context.PyrEmployeeSalaryDefaults.MinAsync(x => x.NumGrossSalary),
                DepartmentWiseSalaryGraphDataList = DepartmentWiseSalaryGraphDataViewModelList.OrderByDescending(x => x.DepartmentBaseMaxSalary).ToList(),
                DepartmentList = TblEmpDepartmentList,
                CustomGraphDataSetList = CustomGraphDataSetList.OrderByDescending(x => x.datas.Count()).ToList()
            };

            departmentWiseSalaryGraphViewModel.SalaryRangeFrom = departmentWiseSalaryGraphViewModel.SalaryRangeFrom > 0 ? departmentWiseSalaryGraphViewModel.SalaryRangeFrom : 0;
            departmentWiseSalaryGraphViewModel.SalaryRangeTo = departmentWiseSalaryGraphViewModel.SalaryRangeTo > 0 ? departmentWiseSalaryGraphViewModel.SalaryRangeTo : 0;

            // --------------------------------- case two update at 13-01-2022 (imran vai and wahid vai requirement)-------------------------------------------
            departmentWiseSalaryGraphViewModel.catagoriesViewModels = caltegoriesList.OrderByDescending(x => x.index).ToList();
            departmentWiseSalaryGraphViewModel.datas = dataList.OrderByDescending(x => x.index).Select(x => x.value).ToList();
            // ----------------------------------case two end --------------------------------

            return departmentWiseSalaryGraphViewModel;
        }

        public async Task<DepartmentWiseAgeGraphViewModel> DepartmentWiseAgeGraph(long? BusinessUnitId)
        {
            List<DepartmentWiseAgeGraphDataViewModel> DepartmentWiseAgeGraphDataViewModelList = new List<DepartmentWiseAgeGraphDataViewModel>();
            List<MasterDepartment> TblEmpDepartmentList = _context.MasterDepartments.Where(x => x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true).ToList();
            List<EmpEmployeeBasicInfo> EmployeeBasicInfoList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true).ToListAsync();

            List<CatagoriesViewModel> caltegoriesList = new List<CatagoriesViewModel>();
            List<DatasViewModel> dataList = new List<DatasViewModel>();

            foreach (var dept in TblEmpDepartmentList)
            {
                DepartmentWiseAgeGraphDataViewModel newObj = new DepartmentWiseAgeGraphDataViewModel
                {
                    DepartmentName = dept.StrDepartment,
                    AgeRangeList = (from emp in EmployeeBasicInfoList
                                    where emp.IntDepartmentId == dept.IntDepartmentId
                                    group emp by CalculateAge(emp.DteDateOfBirth.Value.Date, DateTime.Now) into age
                                    select age.Key).ToList()
                };

                decimal? deptBasedTotalAge = (from emp in EmployeeBasicInfoList
                                              where emp.IntDepartmentId == dept.IntDepartmentId
                                              select CalculateAge(emp.DteDateOfBirth.Value.Date, DateTime.Now)).Sum();

                newObj.DeptBasedNumberOfEmployeeCount = EmployeeBasicInfoList.Where(x => x.IntDepartmentId == dept.IntDepartmentId).Count();
                newObj.DepartmentBaseAvgAge = (deptBasedTotalAge > 0 && newObj.DeptBasedNumberOfEmployeeCount > 0) ? (decimal)(deptBasedTotalAge / newObj.DeptBasedNumberOfEmployeeCount) : 0;

                newObj.DepartmentBaseMaxAge = newObj.AgeRangeList.Count() > 0 ? newObj.AgeRangeList.Max(x => x) : 0;
                newObj.DepartmentBaseMinAge = newObj.AgeRangeList.Count() > 0 ? newObj.AgeRangeList.Min(x => x) : 0;
                DepartmentWiseAgeGraphDataViewModelList.Add(newObj);

                CatagoriesViewModel catagoriesViewModel = new CatagoriesViewModel
                {
                    index = Convert.ToInt64(newObj.DepartmentBaseAvgAge),
                    name = dept.StrDepartment
                };
                DatasViewModel datasViewModel = new DatasViewModel
                {
                    index = Convert.ToInt64(newObj.DepartmentBaseAvgAge),
                    value = newObj.DepartmentBaseAvgAge,
                };

                caltegoriesList.Add(catagoriesViewModel);
                dataList.Add(datasViewModel);
            }

            // ========================== custom data set for graph ====================================

            int lastIndex = DepartmentWiseAgeGraphDataViewModelList.Max(x => x.AgeRangeList.Count());
            List<CustomGraphDataSet> CustomGraphDataSetList = new List<CustomGraphDataSet>();

            for (int i = 0; i < lastIndex; i++)
            {
                CustomGraphDataSet obj = new CustomGraphDataSet();

                obj.title = "Age";
                List<decimal?> datas = new List<decimal?>();

                foreach (var dept in TblEmpDepartmentList)
                {
                    int indexIsExists = DepartmentWiseAgeGraphDataViewModelList.FirstOrDefault(x => x.DepartmentName == dept.StrDepartment).AgeRangeList.Count();
                    if (indexIsExists > i)
                    {
                        decimal? value = DepartmentWiseAgeGraphDataViewModelList.FirstOrDefault(x => x.DepartmentName == dept.StrDepartment).AgeRangeList[i];
                        datas.Add(value);
                    }
                    else
                    {
                        datas.Add(0);
                    }
                }
                obj.datas = datas;
                CustomGraphDataSetList.Add(obj);
            }

            // =========================================== end  ==============================================

            DepartmentWiseAgeGraphViewModel departmentWiseSalaryGraphViewModel = new DepartmentWiseAgeGraphViewModel
            {
                SalaryRangeFrom = await _context.PyrEmployeeSalaryDefaults.MaxAsync(x => x.NumGrossSalary),
                SalaryRangeTo = await _context.PyrEmployeeSalaryDefaults.MinAsync(x => x.NumGrossSalary),
                DepartmentWiseAgeGraphDataList = DepartmentWiseAgeGraphDataViewModelList.OrderByDescending(x => x.DepartmentBaseMaxAge).ToList(),
                DepartmentList = TblEmpDepartmentList,
                CustomGraphDataSetList = CustomGraphDataSetList
            };
            // --------------------------------- case two update at 13-01-2022 (imran vai and wahid vai requirement)-------------------------------------------
            departmentWiseSalaryGraphViewModel.catagoriesViewModels = caltegoriesList.OrderByDescending(x => x.index).ToList();
            departmentWiseSalaryGraphViewModel.datas = dataList.OrderByDescending(x => x.index).Select(x => x.value).ToList();

            return departmentWiseSalaryGraphViewModel;
        }

        // this mathod also use for year base data
        public async Task<MonthOfYearWiseSeparationGraphViewModel> MonthOfYearWiseSeparationGraph(long? BusinessUnitId, int? Year)
        {
            List<MonthOfYearWiseSeparationGraphDataViewModel> MonthOfYearWiseSeparationGraphDataViewModelList = new List<MonthOfYearWiseSeparationGraphDataViewModel>();

            List<MonthViewModel> MonthList = new List<MonthViewModel>
            {
                new MonthViewModel {MonthId = 1, MonthName = "January" },
                new MonthViewModel {MonthId = 2, MonthName = "February" },
                new MonthViewModel {MonthId = 3, MonthName = "March" },
                new MonthViewModel {MonthId = 4, MonthName = "April" },
                new MonthViewModel {MonthId = 5, MonthName = "May" },
                new MonthViewModel {MonthId = 6, MonthName = "June" },
                new MonthViewModel {MonthId = 7, MonthName = "July" },
                new MonthViewModel {MonthId = 8, MonthName = "August" },
                new MonthViewModel {MonthId = 9, MonthName = "September" },
                new MonthViewModel {MonthId = 10, MonthName = "October" },
                new MonthViewModel {MonthId = 11, MonthName = "November" },
                new MonthViewModel {MonthId = 12, MonthName = "December" },
            };

            //List<TblEmployeeSeparation> EmployeeSeparationList = await _context.TblEmployeeSeparations.Where(x => x.DteInsertDate.Value.Year == Year).ToListAsync();

            //foreach (var month in MonthList)
            //{
            //    MonthOfYearWiseSeparationGraphDataViewModel newObj = new MonthOfYearWiseSeparationGraphDataViewModel
            //    {
            //        MonthId = month.MonthId,
            //        MonthName = month.MonthName,
            //        NumberOfEmployee = EmployeeSeparationList.Where(x => x.DteInsertDate.Value.Year == DateTime.Now.Year && x.DteInsertDate.Value.Month == month.MonthId).Count()
            //    };

            //    MonthOfYearWiseSeparationGraphDataViewModelList.Add(newObj);
            //}

            MonthOfYearWiseSeparationGraphViewModel monthOfYearWiseSeparationGraphViewModel = new MonthOfYearWiseSeparationGraphViewModel
            {
                MaximumSeparation = MonthOfYearWiseSeparationGraphDataViewModelList.Count() > 0 ? MonthOfYearWiseSeparationGraphDataViewModelList.Max(x => x.NumberOfEmployee) : 0,
                MinimumSeparation = MonthOfYearWiseSeparationGraphDataViewModelList.Count() > 0 ? MonthOfYearWiseSeparationGraphDataViewModelList.Min(x => x.NumberOfEmployee) : 0,
                MonthOfYearWiseSeparationGraphDataList = MonthOfYearWiseSeparationGraphDataViewModelList.OrderBy(x => x.MonthId).ToList()
            };

            return monthOfYearWiseSeparationGraphViewModel;
        }

        public async Task<SalaryAndOvertimeGraphViewModel> SalaryAndOvertimeGraph(long? BusinessUnitId, int? Year)
        {
            List<CatagoriesViewModel> categoriesListForSalary = new List<CatagoriesViewModel>();
            List<DatasViewModel> dataListForSalary = new List<DatasViewModel>();
            List<DatasViewModel> dataListForOvertime = new List<DatasViewModel>();

            List<PyrSalaryGenerateHeader> tblSalaryGenerateHeadersList = await _context.PyrSalaryGenerateHeaders.Where(x => x.IntYearId == DateTime.Now.Year).AsNoTracking().ToListAsync();

            for (int i = 1; i <= 12; i++)
            {
                CatagoriesViewModel obj = new CatagoriesViewModel
                {
                    index = i,
                    name = new DateTime(2020, i, 1).ToString("MMM")
                };
                DatasViewModel datasViewModel = new DatasViewModel
                {
                    index = i,
                    //value = tblSalaryGenerateHeadersList.Where(x => x.IntMonthId == i).Sum(x => x.NumNetPayableAmountCal)
                };
                DatasViewModel datasViewModel2 = new DatasViewModel
                {
                    index = i,
                    //value = tblSalaryGenerateHeadersList.Where(x => x.IntMonthId == i).Sum(x => x.NumOverTimeAmount)
                };

                categoriesListForSalary.Add(obj);
                dataListForSalary.Add(datasViewModel);
                dataListForOvertime.Add(datasViewModel2);
            }

            SalaryAndOvertimeGraphViewModel retObj = new SalaryAndOvertimeGraphViewModel
            {
                SalaryRangeFrom = dataListForSalary.Max(x => x.value),
                SalaryRangeTo = dataListForSalary.Min(x => x.value),
                catagoriesViewModels = categoriesListForSalary.OrderBy(x => x.index).ToList(),
                datas = dataListForSalary.OrderBy(x => x.index).Select(x => x.value).ToList(),
                datas2 = dataListForOvertime.OrderBy(x => x.index).Select(x => x.value).ToList()
            };

            return retObj;
        }

        public int CalculateAge(DateTime FromDateTime, DateTime ToDateTime)
        {
            LocalDate fromDate = new LocalDate(FromDateTime.Year, FromDateTime.Month, FromDateTime.Day);
            LocalDate toDate = new LocalDate(ToDateTime.Year, ToDateTime.Month, ToDateTime.Day);
            Period period = Period.Between(fromDate, toDate.PlusDays(1));

            int age = period.Years;

            return age;
        }

        public async Task<List<CustomGraphDataSet>> CustomGraphDataSet(string titleName)
        {
            return new List<CustomGraphDataSet>();
        }

        public async Task<TodayAttendanceViewModel> AttendanceGraphData(long AccountId, int intDay, long WorkplaceGroupId)
        {
            TodayAttendanceViewModel attendance = new TodayAttendanceViewModel();
            long TotalEmployee = 0, TodayPresent = 0, TodayLate = 0, TodayAbsent = 0;
            int initial = intDay == 1 ? 0 : 1;
            intDay = intDay == 1 ? 0 : intDay == 2 ? 1 : 7;

            //List<long> empId = _context.EmpEmployeeBasicInfoDetails.Where(s => s.IsActive == true && (s.IntEmployeeStatusId == 1 || s.IntEmployeeStatusId == 4)).Select(s => s.IntEmployeeId).ToList();

            //int ttlEmp = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == AccountId && x.IsActive == true
            //&& x.DteJoiningDate.Value.Date <= DateTime.Now.Date && empId.Contains(x.IntEmployeeBasicInfoId)).CountAsync();

            int totalCurrentEmployee = await (from e in _context.EmpEmployeeBasicInfos
                                              where e.IsActive == true && e.IntAccountId == AccountId && e.IntWorkplaceGroupId == WorkplaceGroupId
                                              join ed in _context.EmpEmployeeBasicInfoDetails on e.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                              from edtl in edetails.DefaultIfEmpty()
                                              where e.DteJoiningDate.Value.Date <= DateTime.UtcNow.Date
                                              && (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                              select e).CountAsync();

            for (int i = initial; i <= intDay; i++)
            {
                DateTime dateTime = DateTime.Now.AddDays(-i);

                //List<long> empIdList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == AccountId && x.IsActive == true
                //&& x.DteJoiningDate.Value.Date <= dateTime.Date).Select(x => x.IntEmployeeBasicInfoId).ToListAsync();

                List<long> empIdList = await (from e in _context.EmpEmployeeBasicInfos
                                              where e.IsActive == true && e.IntAccountId == AccountId && e.IntWorkplaceGroupId == WorkplaceGroupId
                                              join ed in _context.EmpEmployeeBasicInfoDetails on e.IntEmployeeBasicInfoId equals ed.IntEmployeeId into edetails
                                              from edtl in edetails.DefaultIfEmpty()
                                              where e.DteJoiningDate.Value.Date <= dateTime.Date
                                              && (edtl == null || edtl.IntEmployeeStatusId == 1 || edtl.IntEmployeeStatusId == 4)
                                              select e.IntEmployeeBasicInfoId).ToListAsync();

                List<TimeAttendanceDailySummary> employeeAttendance = await (from att in _context.TimeAttendanceDailySummaries
                                                                             where empIdList.Contains(att.IntEmployeeId)
                                                                             && att.DteAttendanceDate.Value.Date == dateTime.Date
                                                                             select att).ToListAsync();

                TotalEmployee += empIdList.Count();
                TodayPresent += employeeAttendance.Where(x => x.IsPresent == true).Count();
                TodayLate += employeeAttendance.Where(x => x.IsLate == true && x.IsPresent == false && x.IsAbsent == false && x.IsHoliday == false && x.IsOffday == false && x.IsLeave == false).Count();
                TodayAbsent += employeeAttendance.Where(x => x.IsAbsent == true && x.IsLate == false && x.IsPresent == false && x.IsHoliday == false && x.IsOffday == false && x.IsLeave == false && x.IsMovement == false && x.IsLeaveWithPay == false).Count();
            }

            attendance.TodayPresentPercentage = Math.Round(((decimal)TodayPresent * 100) / (decimal)TotalEmployee, 2);
            attendance.TodayLatePercentage = Math.Round(((decimal)TodayLate * 100) / (decimal)TotalEmployee, 2);
            attendance.TodayAbsentPercentage = Math.Round(((decimal)TodayAbsent * 100) / (decimal)TotalEmployee, 2);
            attendance.TotalEmployeeCount = totalCurrentEmployee;

            List<AttendanceDonutChart> chartData = new List<AttendanceDonutChart>();

            chartData.Add(new AttendanceDonutChart
            {
                name = "Present",
                value = TodayPresent
            });
            chartData.Add(new AttendanceDonutChart
            {
                name = "Late",
                value = TodayLate
            });
            chartData.Add(new AttendanceDonutChart
            {
                name = "Absent",
                value = TodayAbsent
            });

            attendance.AttendanceDonutChartData = chartData;

            return attendance;
        }

        public async Task<LeaveStatusViewModel> LeaveGraphData(long AccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<long> empIdList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == AccountId && x.IsActive == true && x.IntBusinessUnitId == BusinessUnitId && x.IntWorkplaceGroupId == WorkplaceGroupId).Select(x => x.IntEmployeeBasicInfoId).ToListAsync();

            decimal TotalEmployee = empIdList.Count();

            LeaveStatusViewModel leave = new LeaveStatusViewModel
            {
                TodayLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.Date && x.DteToDate.Date >= DateTime.Now.Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count(),
                TommorrowLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.AddDays(1).Date && x.DteToDate.Date >= DateTime.Now.AddDays(1).Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count(),
                YesterdayLeave = _context.LveLeaveApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.AddDays(-1).Date && x.DteToDate.Date >= DateTime.Now.AddDays(-1).Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count()
            };
            leave.TodayLeavePercentage = Math.Round((Convert.ToDecimal(leave.TodayLeave) * 100) / TotalEmployee, 2);
            leave.TommorrowLeavePercentage = Math.Round((Convert.ToDecimal(leave.TommorrowLeave) * 100) / TotalEmployee, 2);
            leave.YesterdayLeavePercentage = Math.Round((Convert.ToDecimal(leave.YesterdayLeave) * 100) / TotalEmployee, 2);

            return leave;
        }

        public async Task<MovementStatusViewModel> MovementGraphData(long AccountId, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<long> empIdList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == AccountId && x.IsActive == true && x.IntBusinessUnitId == BusinessUnitId && x.IntWorkplaceGroupId == WorkplaceGroupId).Select(x => x.IntEmployeeBasicInfoId).ToListAsync();

            decimal TotalEmployee = empIdList.Count();

            MovementStatusViewModel movement = new MovementStatusViewModel
            {
                TodayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.Date && x.DteToDate.Date >= DateTime.Now.Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count(),
                TommorrowMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.AddDays(1).Date && x.DteToDate.Date >= DateTime.Now.AddDays(1).Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count(),
                YesterdayMovement = _context.LveMovementApplications.Where(x => x.IsActive == true && x.IntAccountId == AccountId && x.DteFromDate.Date <= DateTime.Now.AddDays(-1).Date && x.DteToDate.Date >= DateTime.Now.AddDays(-1).Date && x.IsPipelineClosed == true && x.IsReject == false && empIdList.Contains(x.IntEmployeeId)).Count()
            };
            movement.TodayMovementPercentage = Math.Round(Convert.ToDecimal(movement.TodayMovement * 100) / TotalEmployee);
            movement.YesterdayMovementPercentage = Math.Round(Convert.ToDecimal(movement.YesterdayMovement * 100) / TotalEmployee);
            movement.TommorrowMovementPercentage = Math.Round(Convert.ToDecimal(movement.TommorrowMovement * 100) / TotalEmployee);

            return movement;
        }

        public async Task<InternNProbationPeriodViewModel> InternNProbationPeriodGraphData(long AccountId, int? Year, long BusinessUnitId, long WorkplaceGroupId)
        {
            List<EmpEmployeeBasicInfo> employeeBasicInfo = await _context.EmpEmployeeBasicInfos.Where(x => x.IsActive == true 
                                    && x.IntAccountId == AccountId 
                                    && x.IntBusinessUnitId==BusinessUnitId
                                    && x.IntWorkplaceGroupId==WorkplaceGroupId).ToListAsync();

            //DateTime date1 = DateTime.Parse("01/01/2022");
            //DateTime date2 = DateTime.Parse("11/11/2021");

            //var d = ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month + (date1.Day >= date2.Day ? 0 : -1);

            long InternId = await (from parent in _context.MasterEmploymentTypes
                                   where parent.IntAccountId == 0 && parent.IsActive == true && parent.StrEmploymentType.ToLower() == "intern"
                                   join type in _context.MasterEmploymentTypes on parent.IntEmploymentTypeId equals type.IntParentId
                                   where type.IsActive == true && type.IntAccountId == AccountId
                                   select type.IntEmploymentTypeId).FirstOrDefaultAsync();


            long ProbationId = await (from parent in _context.MasterEmploymentTypes
                                      where parent.IntAccountId == 0 && parent.IsActive == true && parent.StrEmploymentType.ToLower() == "probationary"
                                      join type in _context.MasterEmploymentTypes on parent.IntEmploymentTypeId equals type.IntParentId
                                      where type.IsActive == true && type.IntAccountId == AccountId
                                      select type.IntEmploymentTypeId).FirstOrDefaultAsync();

            InternNProbationPeriodViewModel internNProbationPeriod = new InternNProbationPeriodViewModel
            {
                InternBellowThreeMonth = employeeBasicInfo.Where(x => x.IntEmploymentTypeId == InternId && (((DateTime.Now.Year - x.DteJoiningDate.Value.Year) * 12) + DateTime.Now.Month - x.DteJoiningDate.Value.Month + (DateTime.Now.Day >= x.DteJoiningDate.Value.Day ? 0 : -1)) <= 3).Count(),
                InternAboveThreeMonth = employeeBasicInfo.Where(x => x.IntEmploymentTypeId == InternId && (((DateTime.Now.Year - x.DteJoiningDate.Value.Year) * 12) + DateTime.Now.Month - x.DteJoiningDate.Value.Month + (DateTime.Now.Day >= x.DteJoiningDate.Value.Day ? 0 : -1)) > 3).Count(),
                ProbationBellowSixMonth = employeeBasicInfo.Where(x => x.IntEmploymentTypeId == ProbationId && (((DateTime.Now.Year - x.DteJoiningDate.Value.Year) * 12) + DateTime.Now.Month - x.DteJoiningDate.Value.Month + (DateTime.Now.Day >= x.DteJoiningDate.Value.Day ? 0 : -1)) <= 6).Count(),
                ProbationAboveSixMonth = employeeBasicInfo.Where(x => x.IntEmploymentTypeId == ProbationId && (((DateTime.Now.Year - x.DteJoiningDate.Value.Year) * 12) + DateTime.Now.Month - x.DteJoiningDate.Value.Month + (DateTime.Now.Day >= x.DteJoiningDate.Value.Day ? 0 : -1)) > 6).Count()
            };

            return internNProbationPeriod;
        }

        #endregion ================== GRAPH DATA ALL ============================

        #region ============ Management Dashboard Permission =============

        public async Task<List<ManagementDashboardViewModel>> ManagementDashboardPermission(long EmployeeId)
        {
            List<ManagementDashboardViewModel> managementDashboardList = new List<ManagementDashboardViewModel>();

            List<RoleExtensionRow> isALL = await _context.RoleExtensionRows.Where(x => x.IntOrganizationTypeId == 1 && x.IntEmployeeId == EmployeeId).ToListAsync();

            if (isALL.Where(x => x.IntOrganizationReffId == 0).Count() > 0)
            {
                EmpEmployeeBasicInfo employee = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == EmployeeId).FirstOrDefaultAsync();

                if (employee != null)
                {
                    managementDashboardList = await (from bu in _context.MasterBusinessUnits
                                                     where bu.IntAccountId == employee.IntAccountId && bu.IsActive == true
                                                     join acc in _context.Accounts on bu.IntAccountId equals acc.IntAccountId
                                                     where acc.IsActive == true
                                                     select new ManagementDashboardViewModel
                                                     {
                                                         IntAccountId = acc.IntAccountId,
                                                         AccountName = acc.StrAccountName,
                                                         IntBusinessUnitId = bu.IntBusinessUnitId,
                                                         BusinessUnitName = bu.StrBusinessUnit,
                                                         EmployeeManagementPermissionList = (from emp in _context.EmpEmployeeBasicInfos
                                                                                             where emp.IntAccountId == acc.IntAccountId && emp.IntBusinessUnitId == bu.IntBusinessUnitId
                                                                                             join per in _context.ManagementDashboardPermissions on emp.IntEmployeeBasicInfoId equals per.IntEmployeeId
                                                                                             where per.IsActive == true
                                                                                             select new EmployeeManagementPermission
                                                                                             {
                                                                                                 intEmployeeId = emp.IntEmployeeBasicInfoId,
                                                                                                 EmployeeName = emp.StrEmployeeName,
                                                                                                 EmployeeType = emp.StrEmploymentType,
                                                                                                 Designation = _context.MasterDesignations.Where(x => x.IsActive == true && x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                                                                                 Department = _context.MasterDepartments.Where(x => x.IsActive == true && x.IntDepartmentId == emp.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                                                                                 IsChecked = per == null ? false : true
                                                                                             }).ToList()
                                                     }).ToListAsync();
                    return managementDashboardList;
                }
                else
                {
                    return managementDashboardList;
                }
            }
            else if (isALL.Count() > 0)
            {
                managementDashboardList = await (from re in _context.RoleExtensionRows
                                                 where re.IntOrganizationTypeId == 1 && re.IntEmployeeId == EmployeeId
                                                 join bu in _context.MasterBusinessUnits on re.IntOrganizationReffId equals bu.IntBusinessUnitId
                                                 join acc in _context.Accounts on bu.IntAccountId equals acc.IntAccountId
                                                 where re.IsActive == true && bu.IsActive == true && acc.IsActive == true
                                                 select new ManagementDashboardViewModel
                                                 {
                                                     IntAccountId = acc.IntAccountId,
                                                     AccountName = acc.StrAccountName,
                                                     IntBusinessUnitId = bu.IntBusinessUnitId,
                                                     BusinessUnitName = bu.StrBusinessUnit,
                                                     EmployeeManagementPermissionList = (from emp in _context.EmpEmployeeBasicInfos
                                                                                         where emp.IntAccountId == acc.IntAccountId && emp.IntBusinessUnitId == bu.IntBusinessUnitId && emp.IsActive == true
                                                                                         join per in _context.ManagementDashboardPermissions on emp.IntEmployeeBasicInfoId equals per.IntEmployeeId
                                                                                         where per.IsActive == true
                                                                                         select new EmployeeManagementPermission
                                                                                         {
                                                                                             intEmployeeId = emp.IntEmployeeBasicInfoId,
                                                                                             EmployeeName = emp.StrEmployeeName,
                                                                                             EmployeeType = emp.StrEmploymentType,
                                                                                             Designation = _context.MasterDesignations.Where(x => x.IsActive == true && x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                                                                             Department = _context.MasterDepartments.Where(x => x.IsActive == true && x.IntDepartmentId == emp.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                                                                             IsChecked = per == null ? false : true
                                                                                         }).ToList()
                                                 }).ToListAsync();
                return managementDashboardList;
            }
            else
            {
                return managementDashboardList;
            }
        }

        public async Task<List<EmployeeManagementPermission>> ManagementDashboardPermissionByAccount(long IntAccountId, long IntBusinessUnitId)
        {
            List<EmployeeManagementPermission> data = await (from emp in _context.EmpEmployeeBasicInfos
                                                             where emp.IntAccountId == IntAccountId && emp.IntBusinessUnitId == IntBusinessUnitId
                                                             select new EmployeeManagementPermission
                                                             {
                                                                 intEmployeeId = emp.IntEmployeeBasicInfoId,
                                                                 EmployeeName = emp.StrEmployeeName,
                                                                 EmployeeType = emp.StrEmploymentType,
                                                                 Designation = _context.MasterDesignations.Where(x => x.IsActive == true && x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                                                 Department = _context.MasterDepartments.Where(x => x.IsActive == true && x.IntDepartmentId == emp.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                                                 IsChecked = _context.ManagementDashboardPermissions.Where(x => x.IntAccountId == IntAccountId && x.IntBusinessId == IntBusinessUnitId && x.IntEmployeeId == emp.IntEmployeeBasicInfoId && x.IsActive == true).Count() > 0 ? true : false
                                                             }).ToListAsync();

            return data;
        }

        public async Task<MessageHelper> ManagementDashboardPermissionCRUD(ManagementDashboardViewModel dashboard)
        {
            try
            {
                ManagementDashboardPermission dashboardPermission = new ManagementDashboardPermission();
                List<ManagementDashboardPermission> dashboardPermissionList = new List<ManagementDashboardPermission>();

                List<ManagementDashboardPermission> existsPermission = _context.ManagementDashboardPermissions.Where(x => x.IsActive == true && x.IntAccountId == dashboard.IntAccountId && x.IntBusinessId == dashboard.IntBusinessUnitId)
                    .Select(x => new ManagementDashboardPermission
                    {
                        IntId = x.IntId,
                        IntAccountId = x.IntAccountId,
                        IntBusinessId = x.IntBusinessId,
                        IntEmployeeId = x.IntEmployeeId,
                        IsActive = false,
                        IntCreatedBy = x.IntCreatedBy,
                        DteCreatedAt = x.DteCreatedAt,
                        IntUpdatedBy = dashboard.IntCreateBy,
                        DteUpdatedAt = DateTime.Now
                    }).ToList();
                _context.ManagementDashboardPermissions.UpdateRange(existsPermission);
                await _context.SaveChangesAsync();

                foreach (var item in dashboard.EmployeeManagementPermissionList)
                {
                    if (item.IsChecked == true)
                    {
                        dashboardPermission = new ManagementDashboardPermission
                        {
                            IntAccountId = dashboard.IntAccountId,
                            IntBusinessId = dashboard.IntBusinessUnitId,
                            IntEmployeeId = item.intEmployeeId,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = (long)dashboard.IntCreateBy
                        };
                        dashboardPermissionList.Add(dashboardPermission);
                    }
                }
                await _context.ManagementDashboardPermissions.AddRangeAsync(dashboardPermissionList);
                await _context.SaveChangesAsync();

                MessageHelper message = new MessageHelper();
                message.Message = "Save Successfully.";
                message.StatusCode = 200;
                return message;
            }
            catch (Exception ex)
            {
                MessageHelper message = new MessageHelper();
                message.Message = ex.Message;
                message.StatusCode = 500;
                return message;
            }
        }

        #endregion ============ Management Dashboard Permission =============
    }
}