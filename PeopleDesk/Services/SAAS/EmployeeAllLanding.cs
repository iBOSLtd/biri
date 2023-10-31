using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace PeopleDesk.Services.SAAS
{
    public class EmployeeAllLanding : IEmployeeAllLanding
    {
        private readonly PeopleDeskContext _context;
        private readonly IAuthService _authService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private DataTable dt = new DataTable();

        public EmployeeAllLanding(IApprovalPipelineService _approvalPipelineService, PeopleDeskContext _context, IAuthService _authService)
        {
            this._context = _context;
            this._authService = _authService;
            this._approvalPipelineService = _approvalPipelineService;
        }
        public async Task<ConfirmationEmployeeLandingPaginationViewModelWithHeader> EmployeeBasicForConfirmation(BaseVM tokenData, long? BusinessUnitId, long? WorkplaceGroupId, DateTime? FromDate, DateTime? ToDate, string? SearchTxt, int PageNo, int PageSize)
        {
            IQueryable<EmployeeBasicForConfirmationVM> data = (from emp in _context.EmpEmployeeBasicInfos
                                                         where emp.IntAccountId == tokenData.accountId && emp.IsActive == true
                                                         join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                         from desig in desig2.DefaultIfEmpty()
                                                         join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                         from info in info2.DefaultIfEmpty()
                                                         join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                         from dept in dpt2.DefaultIfEmpty()
                                                         join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                         from sup in sup2.DefaultIfEmpty()
                                                         //join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                                         //from dsup in dsup2.DefaultIfEmpty()
                                                         join man1 in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals man1.IntEmployeeBasicInfoId into man2
                                                         from man in man2.DefaultIfEmpty()

                                                         join wi in _context.TerritorySetups on info.IntWingId equals wi.IntTerritoryId into wi1
                                                         from wing in wi1.DefaultIfEmpty()
                                                         join soleD in _context.TerritorySetups on info.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                         from soleDp in soleD1.DefaultIfEmpty()
                                                         join regn in _context.TerritorySetups on info.IntRegionId equals regn.IntTerritoryId into regn1
                                                         from region in regn1.DefaultIfEmpty()
                                                         join area1 in _context.TerritorySetups on info.IntAreaId equals area1.IntTerritoryId into area2
                                                         from area in area2.DefaultIfEmpty()
                                                         join terrty in _context.TerritorySetups on info.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                         from Territory in terrty1.DefaultIfEmpty()

                                                         join wg1 in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                                         from wg in wg2.DefaultIfEmpty()
                                                         join wrk1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wrk1.IntWorkplaceId into wrk2
                                                         from wrk in wrk2.DefaultIfEmpty()
                                                         //join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                         //from bus in bus2.DefaultIfEmpty()
                                                         join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                         from photo in photo2.DefaultIfEmpty()

                                                         join empType1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals empType1.IntEmploymentTypeId into emptype2
                                                         from empType in emptype2.DefaultIfEmpty()
                                                         join parent in _context.MasterEmploymentTypes on empType.IntParentId equals parent.IntEmploymentTypeId into parent2
                                                         from parentEmpType in parent2.DefaultIfEmpty()
                                                       

                                                         where (info.IntEmployeeStatusId == 1 || info.IntEmployeeStatusId == 4)

                                                         && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(BusinessUnitId)) && emp.IntBusinessUnitId == BusinessUnitId
                                                         && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(WorkplaceGroupId)) && emp.IntWorkplaceGroupId == WorkplaceGroupId

                                                        && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                        : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                        : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId)
                                                        : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo)
                                                        : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId)
                                                        : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId) && tokenData.areaList.Contains(info.IntAreaId)
                                                        : tokenData.territoryList.Contains(info.IntTerritoryId))
                                                        || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                                                         && (FromDate == null || ToDate == null || (emp.DteJoiningDate.Value.Date >= FromDate && emp.DteJoiningDate.Value.Date <= ToDate))
                                                         orderby emp.IntEmployeeBasicInfoId descending
                                                         select new EmployeeBasicForConfirmationVM
                                                         {
                                                             EmployeeId = emp.IntEmployeeBasicInfoId,
                                                             EmployeeName = emp.StrEmployeeName,
                                                             EmployeeCode = emp.StrEmployeeCode,
                                                             ConfirmationDateRaw = emp.DteConfirmationDate.Value.Date,
                                                             ConfirmationDate = emp.DteConfirmationDate.Value.Date.ToString("dd MMM, yyyy"),
                                                             JoiningDateFormated = emp.DteJoiningDate.Value.Date.ToString("dd MMM, yyyy"),
                                                             ProfilePicUrl = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                             DesignationId = emp.IntDesignationId,
                                                             DesignationName = desig == null ? "" : desig.StrDesignation,
                                                             DepartmentId = emp.IntDepartmentId,
                                                             DepartmentName = dept == null ? "" : dept.StrDepartment,
                                                             SupervisorId = emp.IntSupervisorId,
                                                             SupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                                             LineManagerId = emp.IntLineManagerId,
                                                             LineManager = man == null ? "" : man.StrEmployeeName,                                                             
                                                             DateOfBirth = emp.DteDateOfBirth,
                                                             JoiningDate = emp.DteJoiningDate,
                                                             WorkplaceId = emp.IntWorkplaceId,
                                                             WorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                                             WorkplaceGroupId = emp.IntWorkplaceGroupId,
                                                             WorkplaceGroupName = wg == null ? "" : wg.StrWorkplaceGroup,
                                                             BusinessUnitId = emp.IntBusinessUnitId,
                                                             AccountId = emp.IntAccountId,
                                                             EmploymentTypeId = emp.IntEmploymentTypeId,
                                                             EmploymentType = emp.StrEmploymentType,
                                                             PinNo = info.StrPinNo,
                                                             WingId = info.IntWingId,
                                                             WingName = wing == null ? "" : wing.StrTerritoryName,
                                                             SoleDepoId = info.IntSoleDepo,
                                                             SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                             RegionId = info.IntRegionId,
                                                             RegionName = region == null ? "" : region.StrTerritoryName,
                                                             AreaId = info.IntAreaId,
                                                             AreaName = area == null ? "" : area.StrTerritoryName,
                                                             TerritoryId = info.IntTerritoryId,
                                                             TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                                             ConfirmationStatus = (parentEmpType.StrEmploymentType.ToLower() == "Permanent".ToLower()) ? "Confirm" : "NotConfirm",
                                                         }).OrderByDescending(x => x.EmployeeId).AsNoTracking().AsQueryable();


            ConfirmationEmployeeLandingPaginationViewModelWithHeader confirmationEmployeeLanding = new ConfirmationEmployeeLandingPaginationViewModelWithHeader();
            
            int maxSize = 1000;
            PageSize = PageSize > maxSize ? maxSize : PageSize;
            PageNo = PageNo < 1 ? 1 : PageNo;
            

            if (!string.IsNullOrEmpty(SearchTxt))
            {
                SearchTxt = SearchTxt.ToLower();
                data = data.Where(x => x.EmployeeName.ToLower().Contains(SearchTxt) || x.EmployeeCode.ToLower().Contains(SearchTxt) || x.DesignationName.ToLower().Contains(SearchTxt) 
                || x.DepartmentName.ToLower().Contains(SearchTxt) || x.WingName.ToLower().Contains(SearchTxt) || x.SoleDepoName.ToLower().Contains(SearchTxt) || x.RegionName.ToLower().Contains(SearchTxt)
                || x.AreaName.ToLower().Contains(SearchTxt) || x.TerritoryName.ToLower().Contains(SearchTxt) || x.PinNo.ToLower().Contains(SearchTxt));
            }


            confirmationEmployeeLanding.TotalCount = await data.CountAsync();
            confirmationEmployeeLanding.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
            confirmationEmployeeLanding.PageSize = PageSize;
            confirmationEmployeeLanding.CurrentPage = PageNo;
            

            return confirmationEmployeeLanding;

        }

        public async Task<TimeSheetEmpAttenReportLanding> GetEmpAttendanceReport(long AccountId, long IntBusinessUnitId, long IntWorkplaceGroupId, DateTime FromDate, DateTime ToDate, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                //var attendanceSummary = await _context.TimeAttendanceDailySummaries
                //                                .Where(a => a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate)
                //                                .Select(a => new
                //                                {
                //                                    IntEmployeeId = a.IntEmployeeId,
                //                                    IsPresent = a.IsPresent == null ? false : a.IsPresent,
                //                                    IsAbsent = a.IsAbsent == null ? false : a.IsAbsent,
                //                                    IsLate = a.IsLate == null ? false : a.IsLate,
                //                                    IsLeave = a.IsLeave == null ? false : a.IsLeave,
                //                                    IsLeaveWithPay = a.IsLeaveWithPay == null ? false : a.IsLeaveWithPay,
                //                                    IsMovement = a.IsMovement == null ? false : a.IsMovement,
                //                                    IsHoliday = a.IsHoliday == null ? false : a.IsHoliday,
                //                                    IsOffday = a.IsOffday == null ? false : a.IsOffday,
                //                                    IsEarlyLeave = a.IsEarlyLeave == null ? false : a.IsEarlyLeave,
                //                                    IsManual = a.IsManual == null ? false : a.IsManual,
                //                                    IsManualPresent = a.IsManualPresent == null ? false : a.IsManualPresent,
                //                                    IsManualAbsent = a.IsManualAbsent == null ? false : a.IsManualAbsent,
                //                                    IsManualLeave = a.IsManualLeave == null ? false : a.IsManualLeave,
                //                                    IsManualLate = a.IsManualLate == null ? false : a.IsManualLate,
                //                                    a.IsWorkingDayCal
                //                                }).AsNoTracking().ToListAsync();

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                }

                var attendanceSummary = (from a in _context.TimeAttendanceDailySummaries
                                        join emp in _context.EmpEmployeeBasicInfos on a.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                        //join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                        //from dept in dept2.DefaultIfEmpty()
                                        //join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                        //from desig in desig2.DefaultIfEmpty()
                                        where a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate
                                        && emp.IntAccountId == AccountId && emp.IntBusinessUnitId == IntBusinessUnitId &&
                                        emp.IsActive == true && emp.IntWorkplaceGroupId == IntWorkplaceGroupId
                                        //&& (!string.IsNullOrEmpty(searchTxt)? true : emp.StrEmployeeName.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt))
                                        select new
                                            {
                                                IntEmployeeId = a.IntEmployeeId,
                                                IsPresent = a.IsPresent == null ? false : (bool)a.IsPresent,
                                                IsAbsent = a.IsAbsent == null ? false : (bool)a.IsAbsent,
                                                IsLate = a.IsLate == null ? false : (bool)a.IsLate,
                                                IsLeave = a.IsLeave == null ? false : (bool)a.IsLeave,
                                                IsLeaveWithPay = a.IsLeaveWithPay == null ? false : (bool)a.IsLeaveWithPay,
                                                IsMovement = a.IsMovement == null ? false : (bool)a.IsMovement,
                                                IsHoliday = a.IsHoliday == null ? false : (bool)a.IsHoliday,
                                                IsOffday = a.IsOffday == null ? false : (bool)a.IsOffday,
                                                IsEarlyLeave = a.IsEarlyLeave == null ? false : (bool)a.IsEarlyLeave,
                                                IsManual = a.IsManual == null ? false : (bool)a.IsManual,
                                                IsManualPresent = a.IsManualPresent == null ? false : (bool)a.IsManualPresent,
                                                IsManualAbsent = a.IsManualAbsent == null ? false : (bool)a.IsManualAbsent,
                                                IsManualLeave = a.IsManualLeave == null ? false : (bool)a.IsManualLeave,
                                                IsManualLate = a.IsManualLate == null ? false : (bool)a.IsManualLate,
                                                IsWorkingDayCal = a.IsWorkingDayCal
                                            }).AsNoTracking().AsQueryable();

                var empList = (from emp in _context.EmpEmployeeBasicInfos
                               join details in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals details.IntEmployeeId
                               join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                               from dept in dept2.DefaultIfEmpty()
                               join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                               from desig in desig2.DefaultIfEmpty()
                               where emp.IntAccountId == AccountId && emp.IntBusinessUnitId == IntBusinessUnitId &&
                               emp.IsActive == true && emp.IntWorkplaceGroupId == IntWorkplaceGroupId
                               && (details.IntEmployeeStatusId == 1 || details.IntEmployeeStatusId == 4)
                               && (!string.IsNullOrEmpty(searchTxt) ? emp.StrEmployeeName.ToLower().Contains(searchTxt) || emp.StrEmployeeCode.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt) : true)
                               select new
                               {
                                   EmployeeId = emp.IntEmployeeBasicInfoId,
                                   EmployeeCode = emp.StrEmployeeCode,
                                   EmployeeName = emp.StrEmployeeName,
                                   Department = dept != null ? dept.StrDepartment : "",
                                   Designation = desig != null ? desig.StrDesignation : "",
                                   EmploymentType = emp.StrEmploymentType ?? "",
                                   WorkingDays = (int)(ToDate - FromDate).TotalDays + 1,
                                   //Present = _context.TimeAttendanceDailySummaries.Where(a => a.IntEmployeeId == emp.IntEmployeeBasicInfoId && (a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate) && ((a.IsManual == true && (a.IsManualPresent != null ? a.IsManualPresent : false) == true) || (a.IsPresent == true && a.IsManual == false))).Count(),
                                   SalaryStatus = details.IntEmployeeStatusId == 4 ? "Hold" : "Not Hold"
                               }).AsNoTracking().AsQueryable();

                IQueryable<TimeSheetEmpAttenListVM> data = (from emp in empList
                                                            select new TimeSheetEmpAttenListVM
                                                            {
                                                                EmployeeId = emp.EmployeeId,
                                                                EmployeeCode = emp.EmployeeCode,
                                                                EmployeeName = emp.EmployeeName,
                                                                Department = emp.Department,
                                                                Designation = emp.Designation,
                                                                EmploymentType = emp.EmploymentType,
                                                                WorkingDays = emp.WorkingDays,
                                                                //Present = _context.TimeAttendanceDailySummaries.Where(a => a.IntEmployeeId == emp.EmployeeId && (a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate) && ((a.IsManual == true && (a.IsManualPresent != null ? a.IsManualPresent : false) == true) || (a.IsPresent == true && a.IsManual == false))).Count(), //attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && ((a.IsManual == true && a.IsManualPresent == true) || (a.IsPresent == true && a.IsManual == false))).Count(),
                                                                //Absent = _context.TimeAttendanceDailySummaries.Where(a => a.IntEmployeeId == emp.EmployeeId && (a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate) && ((a.IsManual == true && (a.IsManualPresent != null ? a.IsManualPresent : false) == true) || (a.IsPresent == true && a.IsManual == false))).Count(), //attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && ((a.IsManual == true && a.IsManualAbsent == true) || (a.IsAbsent == true && a.IsManual == false))).Count(),
                                                                Present = attendanceSummary.Where(x => x.IntEmployeeId == emp.EmployeeId && ((x.IsPresent == true && x.IsManual == false) || (x.IsManual == true && x.IsManualPresent == true))).Count(),
                                                                Absent = attendanceSummary.Where(x => x.IntEmployeeId == emp.EmployeeId && ((x.IsAbsent == true && x.IsManual == false) || (x.IsManual == true && x.IsManualAbsent == true))).Count(),
                                                                Movement = attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && a.IsMovement == true && a.IsManual == false).Count(),
                                                                Holiday = attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && a.IsHoliday == true && a.IsManual == false).Count(),
                                                                Weekend = attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && a.IsOffday == true && a.IsManual == false).Count(),
                                                                Late = attendanceSummary.Where(a => a.IntEmployeeId == emp.EmployeeId && ((a.IsManual == true && a.IsManualLate == true) || (a.IsLate == true && a.IsManual == false))).Count(),
                                                                SalaryStatus = emp.SalaryStatus
                                                            }).AsQueryable();

                //IQueryable<TimeSheetEmpAttenListVM> dataold = (from emp in _context.EmpEmployeeBasicInfos
                //                                                       join details in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals details.IntEmployeeId
                //                                                       join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                //                                                       from dept in dept2.DefaultIfEmpty()
                //                                                       join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                //                                                       from desig in desig2.DefaultIfEmpty()
                //                                                       where emp.IntAccountId == AccountId && emp.IntBusinessUnitId == IntBusinessUnitId &&
                //                                                       emp.IsActive == true && emp.IntWorkplaceGroupId == IntWorkplaceGroupId
                //                                                       && (details.IntEmployeeStatusId == 1 || details.IntEmployeeStatusId == 4)
                //                                                       select new TimeSheetEmpAttenListVM
                //                                                       {
                //                                                           EmployeeId = emp.IntEmployeeBasicInfoId,
                //                                                           EmployeeCode = emp.StrEmployeeCode,
                //                                                           EmployeeName = emp.StrEmployeeName,
                //                                                           Department = dept != null ? dept.StrDepartment : "",
                //                                                           Designation = desig != null ? desig.StrDesignation : "",
                //                                                           EmploymentType = emp.StrEmploymentType ?? "",
                //                                                           WorkingDays = (int)(ToDate - FromDate).TotalDays + 1,
                //                                                           Present = _context.TimeAttendanceDailySummaries.Where(a => a.IntEmployeeId == emp.IntEmployeeBasicInfoId && (a.DteAttendanceDate >= FromDate && a.DteAttendanceDate <= ToDate) && ((a.IsManual == true && (a.IsManualPresent != null ? a.IsManualPresent : false) == true) || (a.IsPresent == true && a.IsManual == false))).Count(),
                //                                                           //Absent = attendanceSummary.Where(a =>
                //                                                           //    a.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                //                                                           //    ((a.IsManual == (bool)true && a.IsManualAbsent == (bool)true) || (a.IsAbsent == (bool)true && a.IsManual == (bool)false))).Count(),
                //                                                           //Movement = attendanceSummary.Where(a =>
                //                                                           //    a.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                //                                                           //    a.IsMovement == (bool)true && a.IsManual == (bool)false).Count(),
                //                                                           //Holiday = attendanceSummary.Where(a =>
                //                                                           //    a.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                //                                                           //    a.IsHoliday == (bool)true && a.IsManual == (bool)false).Count(),
                //                                                           //Weekend = attendanceSummary.Where(a =>
                //                                                           //    a.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                //                                                           //    a.IsOffday == (bool)true && a.IsManual == (bool)false).Count(),
                //                                                           //Late = attendanceSummary.Where(a =>
                //                                                           //    a.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                //                                                           //    ((a.IsManual == (bool)true && a.IsManualLate == (bool)true) || (a.IsLate == (bool)true && a.IsManual == (bool)false))).Count(),
                //                                                           SalaryStatus = details.IntEmployeeStatusId == 4 ? "Hold" : "Not Hold"
                //                                                       }).AsNoTracking().AsQueryable();

                var attData = data.ToList();

                TimeSheetEmpAttenReportLanding retObj = new TimeSheetEmpAttenReportLanding();
                retObj.PresentCount = attData.Sum(a => a.Present);
                retObj.AbsentCount = attData.Sum(a => a.Absent);
                retObj.LateCount = attData.Sum(a => a.Late);

                if (IsXls)
                {
                    retObj.Data = attData;
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    //if (!string.IsNullOrEmpty(searchTxt))
                    //{
                    //    searchTxt = searchTxt.ToLower();
                    //    data = data.Where(x => x.EmployeeName.ToLower().Contains(searchTxt)
                    //    || x.Designation.ToLower().Contains(searchTxt)
                    //    || x.Department.ToLower().Contains(searchTxt));
                    //}


                    retObj.TotalCount = data.Count();
                    retObj.Data = data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToList();
                    retObj.PageSize = PageSize;
                    retObj.CurrentPage = PageNo;
                }

                return retObj;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
