using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.Global.Interface;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly PeopleDeskContext _context;
        private readonly IDashboardService _dashboardService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private readonly IAuthService _authService;

        public DashboardController(IAuthService _authService, IApprovalPipelineService _approvalPipelineService, IDashboardService _dashboardService, PeopleDeskContext _context)
        {
            this._context = _context;
            this._dashboardService = _dashboardService;
            this._approvalPipelineService = _approvalPipelineService;
            this._authService = _authService;
        }

        [HttpGet]
        [Route("PendingApprovalDashboard")]
        public async Task<IActionResult> PendingApprovalDashboard(long businessUnitId, long workplaceGroupId, DateTime fromDate, DateTime toDate, bool? iAmFromWeb, bool? iAmFromApps)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                long? parentEmployeeId = 0;
                if (tokenData.isOfficeAdmin)
                {
                    Account account = await _context.Accounts.AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.IntAccountId == tokenData.accountId);
                    if (account == null)
                    {
                        return BadRequest("invalid account");
                    }

                    User parentUser = await _context.Users.AsNoTracking().AsQueryable().FirstOrDefaultAsync(x => x.IntAccountId == tokenData.accountId && x.IntRefferenceId == account.IntOwnerEmployeeId && x.IsActive == true);

                    if (account == null || parentUser == null)
                    {
                        return BadRequest("invalid parent user");
                    }
                    else
                    {
                        parentEmployeeId = (long)parentUser.IntRefferenceId;
                    }
                }

                List<PendingDataSetForApprovalViewModel> model = new List<PendingDataSetForApprovalViewModel>();
                EmpIsSupNLMORUGMemberViewModel isSupNLMNUG = new EmpIsSupNLMORUGMemberViewModel();

                List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId(tokenData.employeeId);

                if (userRoleList.Count() > 0)
                {
                    List<Menu> unData = await (from menu in _context.Menus
                                               join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId
                                               where menu.IsActive == true && menu.IsHasApproval == true
                                               && (mpermission.IsForWeb == true || mpermission.IsForApps == true)
                                               && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
                                                      || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && (mpermission.IntEmployeeOrRoleId == parentEmployeeId || mpermission.IntEmployeeOrRoleId == tokenData.employeeId)))
                                               select menu).AsNoTracking().AsQueryable().Distinct().ToListAsync();

                    List<Menu> data = new List<Menu>();

                    /*================================================*/
                    bool isAdmin = tokenData.isOfficeAdmin; long accountId = tokenData.accountId; long employeeId = tokenData.employeeId; /////////////remove this line of code after pipeline DONE
                    /*================================================*/

                    unData.ForEach(item =>
                    {
                        if (!data.Select(x => x.IntMenuId).Contains(item.IntMenuId))
                        {
                            data.Add(item);
                        }
                    });

                    foreach (Menu item in data)
                    {
                        // Leave Application
                        if (item.StrHashCode == "BABBAPBANDNQNARQ" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            LeaveApplicationLandingRequestVM leaveApplicationLandingView = new LeaveApplicationLandingRequestVM
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.LeaveLandingEngine(leaveApplicationLandingView, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/leaveApproval"
                            });
                        }

                        // Movement Application
                        if (item.StrHashCode == "BABBAPBANDNQNNAO" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            MovementApplicationLandingRequestVM movemnetApplicationLandingView = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.MovementLandingEngine(movemnetApplicationLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/movementApproval"
                            });
                        }

                        // Remote Attendance
                        if (item.StrHashCode == "BABBAPBANDNQNONP" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            RemoteAttendanceLandingViewModel remoteAttendanceLandingView = new RemoteAttendanceLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.RemoteAttendanceLandingEngine(remoteAttendanceLandingView, tokenData, false);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/remoteAttendance"
                            });
                        }

                        // Market Attendance
                        if (item.StrHashCode == "BABBAPBANDNQNOBE" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            RemoteAttendanceLandingViewModel remoteAttendanceLandingView = new RemoteAttendanceLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.RemoteAttendanceLandingEngine(remoteAttendanceLandingView, tokenData, true);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/marketVisit"
                            });
                        }

                        // Location & Device Registration
                        if (item.StrHashCode == "BABBAPBANDNQNONC" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            RemoteAttendanceLocationNDeviceLandingViewModel locationNDeviceLandingView = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.RemoteAttendanceLocationNDeviceLandingEngine(locationNDeviceLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/remoteAttendanceLocationNDevice"
                            });
                        }

                        // Salary Addition & Deduction
                        if (item.StrHashCode == "BABBAPBANDNQNBQO" && iAmFromWeb == true)
                        {
                            SalaryAdditionNDeductionLandingViewModel salaryAdditionNDeductionLandingView = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.SalaryAdditionNDeductionLandingEngine(salaryAdditionNDeductionLandingView, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/allowanceNDeduction"
                            });
                        }

                        // IOU
                        if (item.StrHashCode == "BABBAPBANDNQNBQC" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            IOULandingViewModel iOULandingView = new IOULandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.IOUApplicationLandingEngine(iOULandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/iouApplication"
                            });
                        }

                        // Loan
                        if (item.StrHashCode == "BABBAPBANDNQNNAQ" && (iAmFromWeb == true || iAmFromApps == true))
                        {

                            LoanApplicationLandingViewModel loanLandingView = new LoanApplicationLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.LoanLandingEngine(loanLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/loanApproval"
                            });
                        }

                        // Salary Generate
                        if (item.StrHashCode == "BABBAPBANDNQNNAD" && iAmFromWeb == true)
                        {

                            SalaryGenerateRequestLandingRequestVM salaryGenerateLandingRequest = new SalaryGenerateRequestLandingRequestVM
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.SalaryGenerateRequestLandingEngine(salaryGenerateLandingRequest, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/salaryApproval"
                            });
                        }

                        // Arear Salary Generate
                        if (item.StrHashCode == "BABBAPBANDNQNBQQ" && iAmFromWeb == true)
                        {
                            ArearSalaryGenerateRequestLandingViewModel salaryLandingView = new ArearSalaryGenerateRequestLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.ArearSalaryGenerateRequestLandingEngine(salaryLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/arrearSalaryApproval"
                            });
                        }

                        // Overtime
                        if (item.StrHashCode == "BABBAPBANDNQNNAP" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            OverTimeLandingViewModel overtimeLandingView = new OverTimeLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.OverTimeLandingEngine(overtimeLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/overtimeApproval"
                            });
                        }

                        // IOU Adjustment
                        if (item.StrHashCode == "BABBAPBANDNQNBQP" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            IOUAdjustmentLandingViewModel iouAdjLandingView = new IOUAdjustmentLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.IOUAdjustmentLandingEngine(iouAdjLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/iouAdjustmentApproval"
                            });
                        }

                        // Attendance Approval
                        if (item.StrHashCode == "BABBAPBANDNQNNAC" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            ManualAttendanceSummaryLandingVM ManualAttendanceApplicationLandingView = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.ManualAttendanceLandingEngine(ManualAttendanceApplicationLandingView, tokenData);


                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/attendanceApproval"
                            });
                        }

                        // Separation
                        if (item.StrHashCode == "BABBAPBANDNQNNAE" && iAmFromWeb == true)
                        {
                            EmployeeSeparationLandingViewModel separationLandingView = new EmployeeSeparationLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.EmployeeSeparationLandingEngine(separationLandingView, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/separationApproval"
                            });
                        }

                        // Transfer & Promotion
                        if (item.StrHashCode == "BABBAPBANDNQNBQB" && iAmFromWeb == true)
                        {

                            TransferNPromotionLandingViewModel transferNPromotionLanding = new TransferNPromotionLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.TransferNPromotionLandingEngine(transferNPromotionLanding, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/transferandpromotion"
                            });
                        }

                        // Employee Increment
                        if (item.StrHashCode == "BABBAPBANDNQNBQD" && iAmFromWeb == true)
                        {
                            EmployeeIncrementLandingViewModel employeeIncrementLanding = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.EmployeeIncrementLandingEngine(employeeIncrementLanding, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/incrementApproval"
                            });
                        }

                        // Bonus Generate Header
                        if (item.StrHashCode == "BABBAPBANDNQNNAR" && iAmFromWeb == true)
                        {
                            BonusGenerateHeaderLandingViewModel bonusGenerateHeaderLanding = new BonusGenerateHeaderLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.BonusGenerateHeaderLandingEngine(bonusGenerateHeaderLanding, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/bonusApproval"
                            });
                        }

                        // PF Withdraw
                        if (item.StrHashCode == "BABBAPBANDNQNBQN" && iAmFromWeb == true)
                        {
                            PFWithdrawLandingViewModel PFWithdrawLanding = new PFWithdrawLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.PFWithdrawLandingEngine(PFWithdrawLanding, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/pfWithdraw"
                            });
                        }

                        // Expense Application
                        if (item.StrHashCode == "BABBAPBANDNQNOND" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            ExpenseApplicationLandingViewModel ExpenseLanding = new ExpenseApplicationLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.ExpenseLandingEngine(ExpenseLanding, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/expenseApproval"
                            });
                        }

                        // Salary Certificate Requisition Application
                        if (item.StrHashCode == "BABBAPBANDNQNONR" && iAmFromWeb == true)
                        {
                            SalaryCertificateRequestLandingViewModel SalaryCertificateLanding = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.SalaryCertificateRequestLandingEngine(SalaryCertificateLanding, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/salaryCertificateApproval"
                            });
                        }

                        // Asset Requisition
                        if (item.StrHashCode == "BABBAPBANDNQNNCA" && iAmFromWeb == true)
                        {
                            AssetRequisitionLandingRequestVM leaveApplicationLandingView = new()
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.AssetRequisitionLandingEngine(leaveApplicationLandingView, tokenData);

                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/assetApproval"
                            });
                        }

                        // Asset Transfer
                        if (item.StrHashCode == "BABBAPBANDNQNOPO" && iAmFromWeb == true)
                        {
                            AssetTransferApprovalResponse Response = new AssetTransferApprovalResponse();
                            AssetTransferLandingViewModel AssetTransferLanding = new AssetTransferLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                IsAdmin = isAdmin,
                                IsSupOrLineManager = 0,
                                ApproverId = employeeId,
                                AccountId = accountId,
                                IsSupervisor = false,
                                IsLineManager = false,
                                IsUserGroup = false
                            };

                            Response = await _approvalPipelineService.AssetTransferLandingEngine(AssetTransferLanding);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.ListData.Count(),
                                RouteUrl = "/approval/assetTransferApproval"
                            });
                        }

                        // Training Schedule
                        if (item.StrHashCode == "BABBAPBANDNQNOPP" && iAmFromWeb == true)
                        {
                            TrainingScheduleApprovalResponse Response = new TrainingScheduleApprovalResponse();
                            TrainingScheduleLandingViewModel TrainingScheduleLanding = new TrainingScheduleLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                IsAdmin = isAdmin,
                                IsSupOrLineManager = 0,
                                ApproverId = employeeId,
                                AccountId = accountId,
                                IsSupervisor = false,
                                IsLineManager = false,
                                IsUserGroup = false
                            };

                            Response = await _approvalPipelineService.TrainingScheduleLandingEngine(TrainingScheduleLanding);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.ListData.Count(),
                                RouteUrl = "/approval/scheduleApproval"
                            });
                        }

                        // Training Requisition
                        if (item.StrHashCode == "BABBAPBANDNQNOPD" && iAmFromWeb == true)
                        {
                            TrainingRequisitionLandingViewModel TrainingRequisitionLanding = new TrainingRequisitionLandingViewModel
                            {
                                ApplicationStatus = "pending",
                                BusinessUnitId = businessUnitId,
                                WorkplaceGroupId = workplaceGroupId,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var Response = await _approvalPipelineService.TrainingRequisitionLandingEngine(TrainingRequisitionLanding, tokenData);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = Response.TotalCount,
                                RouteUrl = "/approval/trainingRequisitionApproval"
                            });
                        }

                        // Master Location Approval
                        if (item.StrHashCode == "BABBAPBANDNQNOPR" && (iAmFromWeb == true || iAmFromApps == true))
                        {
                            MasterLocationApprovalResponse response = new();
                            MasterLocationLandingViewModel masterLocationLanding = new()
                            {
                                ApplicationStatus = "pending",
                                IsAdmin = isAdmin,
                                IsSupOrLineManager = 0,
                                ApproverId = employeeId,
                                AccountId = accountId,
                                IsSupervisor = false,
                                IsLineManager = false,
                                IsUserGroup = false
                            };

                            response = await _approvalPipelineService.MastrerLocaLandingEngine(masterLocationLanding);
                            model.Add(new PendingDataSetForApprovalViewModel
                            {
                                MenuId = item.IntMenuId,
                                MenuName = item.StrMenuName,
                                PipelineCode = item.StrHashCode,
                                TotalCount = response.ListData.Count(),
                                RouteUrl = "/approval/masterLocationApproval"
                            });
                        }
                    }
                }

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("EmployeeDashboard")]
        public async Task<IActionResult> EmployeeDashboard()
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                DashboardViewModel model = new DashboardViewModel
                {
                    EmployeeDashboardViewModel = await _dashboardService.EmployeeDashboard(tokenData.employeeId, tokenData.businessUnitId)
                };

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("MidLevelDashboard")]
        public async Task<IActionResult> MidLevelDashboard()
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                DashboardViewModel model = new DashboardViewModel
                {
                    MidLevelDashboardViewModel = await _dashboardService.MidLevelDashboard(tokenData.employeeId, tokenData.accountId)
                };

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("TopLevelDashboard")]
        public async Task<IActionResult> TopLevelDashboard(long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                DashboardViewModel model = new DashboardViewModel();
                model.TopLevelDashboardViewModel = await _dashboardService.TopLevelDashboard(tokenData.employeeId, tokenData.accountId, BusinessUnitId, WorkplaceGroupId);

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("AttendanceGraphData")]
        public async Task<IActionResult> AttendanceGraphData(int intDay, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                TodayAttendanceViewModel model = await _dashboardService.AttendanceGraphData(tokenData.accountId, intDay, WorkplaceGroupId);

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("DepartmentWiseSalaryGraph")]
        public async Task<IActionResult> DepartmentWiseSalaryGraph(long BusinessUnitId)
        {
            DepartmentWiseSalaryGraphViewModel model = await _dashboardService.DepartmentWiseSalaryGraph(BusinessUnitId);
            return Ok(model);
        }

        [HttpGet]
        [Route("DepartmentWiseAgeGraph")]
        public async Task<IActionResult> DepartmentWiseAgeGraph(long BusinessUnitId)
        {
            DepartmentWiseAgeGraphViewModel model = await _dashboardService.DepartmentWiseAgeGraph(BusinessUnitId);
            return Ok(model);
        }

        [HttpGet]
        [Route("MonthOfYearWiseSeparationGraph")]
        public async Task<IActionResult> MonthOfYearWiseSeparationGraph(long? BusinessUnitId, int? Year)
        {
            MonthOfYearWiseSeparationGraphViewModel model = await _dashboardService.MonthOfYearWiseSeparationGraph(BusinessUnitId, Year);
            return Ok(model);
        }


        [HttpGet]
        [Route("MonthAndYearBasedAttendanceSummary")]
        public async Task<IActionResult> MonthAndYearBasedAttendanceSummary(long EmployeeId, int YearId, int MonthId)
        {
            try
            {
                List<TimeAttendanceDailySummary> attendanceDailySummaryList = await _context.TimeAttendanceDailySummaries
                .Where(x => x.DteAttendanceDate.Value.Date.Year == YearId
                && x.DteAttendanceDate.Value.Date.Month == MonthId && x.IntEmployeeId == EmployeeId).ToListAsync();

                DateTime makeDate = new DateTime(YearId, MonthId, 1);

                string? MonthName = makeDate.ToString("MMMM");
                int? WorkingDays = DateTime.DaysInMonth(YearId, MonthId);
                int? PresentDays = attendanceDailySummaryList.Where(x => x.IsPresent == true).Count();
                int? LateDays = attendanceDailySummaryList.Where(x => x.IsLate == true).Count();
                int? AbsentDays = attendanceDailySummaryList.Where(y => y.IsAbsent == true).Count();

                var data = new
                {
                    MonthName = MonthName,
                    WorkingDays = attendanceDailySummaryList.Where(x => x.IsWorkingDayCal == 1).Count(),
                    PresentDays = PresentDays,
                    AbsentDays = AbsentDays,
                    LateDays = LateDays
                };

                return Ok(data);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        //[HttpGet]
        //[Route("DashboardComponentViewlanding")]
        //public async Task<IActionResult> DashboardComponentViewlanding(long? IntAccountId, long? IntEmployeeId)
        //{
        //    try
        //    {
        //        List<DashboardComponent> PendingApplicationLanding = await _dashboardService.DashboardComponentViewlanding(IntAccountId,IntEmployeeId);

        //        //EmployeeStatus employeeStatus = await _dashboardService.EmployeeStatusLanding(DateTime.Now.Year);

        //        return Ok(PendingApplicationLanding);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //[HttpPost]
        //[Route("DashboardComponentShowHide")]
        //public async Task<IActionResult> DashboardComponentShowHide(DashboardComponent component)
        //{
        //    try
        //    {
        //        MessageHelper msg = await _dashboardService.DashboardComponentShowHide(component);

        //        return Ok(msg);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpGet]
        [Route("EmployeeStatusGraph")]
        public async Task<IActionResult> EmployeeStatusGraph(long IntYear, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                EmployeeStatusViewModel employeeStatus = await _dashboardService.EmployeeStatusGraph(IntYear, tokenData.accountId, BusinessUnitId, WorkplaceGroupId);

                return Ok(employeeStatus);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("MonthWiseLeaveTakenGraph")]
        public async Task<IActionResult> MonthWiseLeaveTakenGraph(long IntYear, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                    new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId },
                    PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                //if (tokenData.accountId == -1)
                //{
                //    return BadRequest(new MessageHelperAccessDenied());
                //}

                List<MonthWiseLeaveTakenViewModel> monthlyLeaveTakens = await _dashboardService.MonthWiseLeaveTakenGraph(IntYear, tokenData.accountId, BusinessUnitId, WorkplaceGroupId);
                return Ok(monthlyLeaveTakens);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("MonthWiseIOUGraph")]
        public async Task<IActionResult> MonthWiseIOUGraph(long IntYear, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                List<MonthWiseIOUViewModel> IouAmount = await _dashboardService.MonthWiseIOUGraph(IntYear, tokenData.accountId, BusinessUnitId, WorkplaceGroupId);

                return Ok(IouAmount);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("GetAttendanceSummaryCalenderViewReport")]
        public async Task<IActionResult> GetAttendanceSummaryCalenderViewReport(long Month, long Year)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                AttendanceSummaryViewModel AttendanceSummary = await _dashboardService.GetAttendanceSummaryCalenderReport(tokenData.employeeId, Month, Year);

                return Ok(AttendanceSummary);
            }
            catch (Exception e)
            {

                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("EmployeeTurnOverRatio")]
        public async Task<IActionResult> EmployeeTurnOverRatio( long BusinessUnitId, long WorkplaceGroupId)
        {
            
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                    new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId },
                    PermissionLebelCheck.WorkplaceGroup);


                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _dashboardService.EmployeeTurnOverRatio(tokenData.accountId, BusinessUnitId, WorkplaceGroupId);

                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("LastFiveYearEmployeeTurnOverRatio")]
        public async Task<IActionResult> LastFiveYearEmployeeTurnOverRatio(long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _dashboardService.LastFiveYearsTurnoverRatio(tokenData.accountId, 1, 1);

                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("EmployeeCountBySalaryRange")]
        public async Task<IActionResult> EmployeeCountBySalaryRange(long? MinSalary, long? MaxSalary, long BusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                SalaryRangeViewModel salaryRange = await _dashboardService.EmployeeCountBySalaryRange(tokenData.accountId, MinSalary, MaxSalary, BusinessUnitId, WorkplaceGroupId);
                return Ok(salaryRange);

            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("InternNProbationPeriodGraphData")]
        public async Task<IActionResult> InternNProbationPeriodGraphData(int? Year,long BusinessUnitId,long WorkplaceGroupId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                    new PayloadIsAuthorizeVM { businessUnitId = BusinessUnitId, workplaceGroupId = WorkplaceGroupId },
                    PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                //if (tokenData.accountId == -1)
                //{
                //    return BadRequest(new MessageHelperAccessDenied());
                //}

                InternNProbationPeriodViewModel internNProbationPeriodInfo = await _dashboardService.InternNProbationPeriodGraphData(tokenData.accountId, Year,  BusinessUnitId,  WorkplaceGroupId);
                return Ok(internNProbationPeriodInfo);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageHelperError { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("ManagementDashboardPermissionLanding")]
        public async Task<IActionResult> ManagementDashboardPermissionLanding(long EmployeeId)
        {
            try
            {
                List<ManagementDashboardViewModel> managementDashboardViews = await _dashboardService.ManagementDashboardPermission(EmployeeId);

                return Ok(managementDashboardViews);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("ManagementDashboardPermissionByAccount")]
        public async Task<IActionResult> ManagementDashboardPermissionByAccount(long IntAccountId, long IntBusinessUnitId)
        {
            try
            {
                List<EmployeeManagementPermission> DashboardPermission = await _dashboardService.ManagementDashboardPermissionByAccount(IntAccountId, IntBusinessUnitId);
                return Ok(DashboardPermission);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("ManagementDashboardPermissionCRUD")]
        public async Task<IActionResult> ManagementDashboardPermissionCRUD(ManagementDashboardViewModel dashboard)
        {
            try
            {
                MessageHelper message = await _dashboardService.ManagementDashboardPermissionCRUD(dashboard);
                return Ok(message);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
