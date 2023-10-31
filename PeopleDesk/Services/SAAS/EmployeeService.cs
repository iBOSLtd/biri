using Azure.Storage.Blobs.Models;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
//using Elastic.Apm.Api;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.UnitsOfMeasure;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Attendance;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.Helper;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;
using Connection = PeopleDesk.Helper.Connection;

namespace PeopleDesk.Services.SAAS
{
    public class EmployeeService : IEmployeeService
    {
        private readonly PeopleDeskContext _context;
        private readonly IAuthService _authService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private DataTable dt = new DataTable();

        #region ================ Employee Department ===================

        public EmployeeService(IApprovalPipelineService _approvalPipelineService, PeopleDeskContext _context, IAuthService _authService)
        {
            this._context = _context;
            this._authService = _authService;
            this._approvalPipelineService = _approvalPipelineService;
        }

        public async Task<long> SaveEmpDepartment(MasterDepartment obj)
        {
            if (obj.IntDepartmentId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.MasterDepartments.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntDepartmentId;
        }

        public async Task<List<DepartmentViewModel>> GetAllEmpDepartment(long accountId, long businessUnitId)
        {
            List<DepartmentViewModel> departmentList = await (from dept in _context.MasterDepartments
                                                              join p1 in _context.MasterDepartments on dept.IntParentDepId equals p1.IntDepartmentId into p2
                                                              from parent in p2.DefaultIfEmpty()
                                                              join bu in _context.MasterBusinessUnits on dept.IntBusinessUnitId equals bu.IntBusinessUnitId into bus
                                                              from busi in bus.DefaultIfEmpty()
                                                              where dept.IsActive == true && dept.IntAccountId == accountId
                                                              && (dept.IntBusinessUnitId == 0 || busi.IntBusinessUnitId == businessUnitId)
                                                              select new DepartmentViewModel
                                                              {
                                                                  IntDepartmentId = dept.IntDepartmentId,
                                                                  StrDepartment = dept.StrDepartment,
                                                                  StrDepartmentCode = dept.StrDepartmentCode,
                                                                  IntParentDepId = parent == null ? 0 : parent.IntDepartmentId,
                                                                  StrParentDepName = parent == null ? "" : parent.StrDepartment,
                                                                  IsActive = dept.IsActive,
                                                                  IsDeleted = dept.IsDeleted,
                                                                  IntBusinessUnitId = dept.IntBusinessUnitId,
                                                                  StrBusinessUnit = busi == null ? "ALL" : busi.StrBusinessUnit,
                                                                  IntAccountId = dept.IntAccountId,
                                                                  IntCreatedBy = dept.IntCreatedBy,
                                                                  DteCreatedAt = dept.DteCreatedAt,
                                                                  IntUpdatedBy = dept.IntUpdatedBy,
                                                                  DteUpdatedAt = dept.DteUpdatedAt
                                                              }).OrderByDescending(x => x.IntDepartmentId).AsNoTracking().AsQueryable().ToListAsync();
            return departmentList;
        }

        public async Task<MasterDepartment> GetEmpDepartmentById(long Id)
        {
            return await _context.MasterDepartments.FirstAsync(x => x.IntDepartmentId == Id);
        }

        public async Task<bool> DeleteEmpDepartment(long id)
        {
            try
            {
                MasterDepartment obj = await _context.MasterDepartments.FirstAsync(x => x.IntDepartmentId == id);
                obj.IsActive = false;
                _context.MasterDepartments.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================ Employee Department ===================

        #region =========== Employee Bulk Upload ================

        public async Task<MessageHelperBulkUpload> SaveEmployeeBulkUpload(List<EmployeeBulkUploadViewModel> model)
        {
            MessageHelperBulkUpload message = new MessageHelperBulkUpload();

            try
            {
                long accountId = model.First().IntAccountId;

                IEnumerable<EmpEmployeeBasicInfo> employeeList = await _context.EmpEmployeeBasicInfos
                    .Where(x => x.IntAccountId == model.First().IntAccountId && x.IsActive == true).ToListAsync();

                List<ErrorList> exisList = (from existEmp in employeeList
                                            join bulkEmp in model on existEmp.StrEmployeeCode.ToLower() equals bulkEmp.StrEmployeeCode.ToLower()
                                            where existEmp.IsActive == true && bulkEmp.IntAccountId == existEmp.IntAccountId
                                            select new ErrorList
                                            {
                                                Title = bulkEmp.StrEmployeeName + " [" + bulkEmp.StrEmployeeCode + "]",
                                                Body = "Employee Code [" + bulkEmp.StrEmployeeCode + "] already exist."
                                            }).AsQueryable().ToList();

                if (exisList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = exisList;
                    return message;
                }

                #region Validation

                List<string> deptList = await _context.MasterDepartments.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrDepartment).ToListAsync();

                var invalidDeptList = model.Where(x => !deptList.Contains(x.StrDepartment)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Department Name [" + x.StrDepartment + "] is not valid !!"
                }).ToList();

                if (invalidDeptList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidDeptList;
                    return message;
                }

                List<string> desigList = await _context.MasterDesignations.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrDesignation).ToListAsync();

                var invalidDesigList = model.Where(x => !desigList.Contains(x.StrDesignation)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Designation Name [" + x.StrDesignation + "] is not valid !!"
                }).ToList();

                if (invalidDesigList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidDesigList;
                    return message;
                }

                List<string> empTypeList = await _context.MasterEmploymentTypes.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrEmploymentType).ToListAsync();

                var invalidEmpTypeList = model.Where(x => !empTypeList.Contains(x.StrEmploymentType)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Employment Type [" + x.StrEmploymentType + "] is not valid !!"
                }).ToList();

                if (invalidEmpTypeList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidEmpTypeList;
                    return message;
                }

                List<string> genderList = await _context.Genders.Where(x => x.IsActive == true).Select(x => x.StrGender).ToListAsync();

                var invalidGenderList = model.Where(x => !genderList.Contains(x.StrGender)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Gender [" + x.StrGender + "] is not valid !!"
                }).ToList();

                if (invalidGenderList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidGenderList;
                    return message;
                }

                List<string> religionList = await _context.Religions.Where(x => x.IsActive == true).Select(x => x.StrReligion).ToListAsync();

                var invalidReligionList = model.Where(x => !genderList.Contains(x.StrGender)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Religion [" + x.StrReligionName + "] is not valid !!"
                }).ToList();

                if (invalidReligionList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidReligionList;
                    return message;
                }

                List<string> userTypeList = await _context.UserTypes.Where(x => x.IsActive == true).Select(x => x.StrUserType).ToListAsync();

                var invalidUserTypeList = model.Where(x => !userTypeList.Contains(x.StrUserType)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "User Type [" + x.StrUserType + "] is not valid !!"
                }).ToList();

                if (invalidUserTypeList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidUserTypeList;
                    return message;
                }

                List<string> buList = await _context.MasterBusinessUnits.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrBusinessUnit).ToListAsync();

                var invalidBuList = model.Where(x => !buList.Contains(x.StrBusinessUnit)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Business Unit [" + x.StrUserType + "] is not valid !!"
                }).ToList();

                if (invalidBuList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidBuList;
                    return message;
                }

                List<string> wgList = await _context.MasterWorkplaceGroups.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrWorkplaceGroup).ToListAsync();

                var invalidWgList = model.Where(x => !wgList.Contains(x.StrWorkplaceGroup)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Workplace Group [" + x.StrWorkplaceGroup + "] is not valid !!"
                }).ToList();

                if (invalidWgList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidWgList;
                    return message;
                }

                List<string> wpList = await _context.MasterWorkplaces.Where(x => x.IntAccountId == accountId && x.IsActive == true).Select(x => x.StrWorkplace).ToListAsync();

                var invalidWpList = model.Where(x => !wpList.Contains(x.StrWorkplace)).Select(x => new ErrorList
                {
                    Title = "For Employee Name [" + x.StrEmployeeName + "]",
                    Body = "Workplace [" + x.StrWorkplace + "] is not valid !!"
                }).ToList();

                if (invalidWpList.Count() > 0)
                {
                    message.StatusCode = 500;
                    message.Message = "Existing Error.";
                    message.ListData = invalidWpList;
                    return message;
                }

                #endregion Validation
                else
                {
                    List<EmployeeBulkUpload> employeeBulkList = new List<EmployeeBulkUpload>();

                    foreach (EmployeeBulkUploadViewModel employee in model)
                    {
                        EmployeeBulkUpload employeeInfo = new EmployeeBulkUpload
                        {
                            IntEmpBulkUploadId = 0,
                            IntAccountId = employee.IntAccountId,
                            IntUrlId = employee.IntUrlId,
                            IntSlid = employee.IntIdentitySlid,
                            StrBusinessUnit = employee.StrBusinessUnit,
                            StrWorkplaceGroup = employee.StrWorkplaceGroup,
                            StrWorkplace = employee.StrWorkplace,
                            StrDepartment = employee.StrDepartment,
                            StrDesignation = employee.StrDesignation,
                            StrHrPosition = employee.strHrPosition,
                            StrEmploymentType = employee.StrEmploymentType,
                            StrEmployeeName = employee.StrEmployeeName,
                            StrEmployeeCode = employee.StrEmployeeCode,
                            StrCardNumber = employee.StrCardNumber,
                            StrGender = employee.StrGender,
                            IsSalaryHold = employee.IsSalaryHold,
                            StrReligionName = employee.StrReligionName,
                            DteDateOfBirth = employee.DteDateOfBirth,
                            DteJoiningDate = employee.DteJoiningDate,
                            DteInternCloseDate = employee.DteInternCloseDate,
                            DteProbationaryCloseDate = employee.DteProbationaryCloseDate,
                            DteContactFromDate = employee.DteContactFromDate,
                            DteContactToDate = employee.DteContactToDate,
                            StrSupervisorCode = employee.strSupervisorCode,
                            StrDottedSupervisorCode = employee.strDottedSupervisorCode,
                            StrLineManagerCode = employee.strLineManagerCode,
                            StrLoginId = employee.StrLoginId,
                            StrPassword = employee.StrPassword,
                            StrEmailAddress = employee.StrEmailAddress,
                            StrPhoneNumber = employee.StrPhoneNumber,
                            StrDisplayName = employee.StrDisplayName,
                            StrUserType = employee.StrUserType,
                            IsProcess = false,
                            IsActive = true,
                            StrWingName = employee.StrWingName,
                            StrSoleDepoName = employee.StrSoleDepoName,
                            StrRegionName = employee.StrRegionName,
                            StrAreaName = employee.StrAreaName,
                            StrTerritoryName = employee.StrTerritoryName,
                            IntCreateBy = employee.IntCreateBy,
                            DteCreateAt = employee.DteCreateAt
                        };

                        var Password = Encoding.UTF8.GetBytes(employeeInfo.StrPassword);
                        string encryptedPassword = Convert.ToBase64String(Password);
                        employeeInfo.StrPassword = encryptedPassword;

                        employeeBulkList.Add(employeeInfo);
                    }

                    await _context.EmployeeBulkUploads.AddRangeAsync(employeeBulkList);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Employee Bulk Upload Successfully.";
                    return message;
                }
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return message;
            }
        }

        #endregion =========== Employee Bulk Upload ================

        #region ================ Employee Basic Info ==================

        public async Task<long> SaveEmpBasicInfo(EmployeeBasicViewModel obj)
        {
            MessageHelper res = new MessageHelper();

            EmpEmployeeBasicInfo emp = new EmpEmployeeBasicInfo
            {
                StrEmployeeCode = obj.StrEmployeeCode,
                StrCardNumber = obj.StrCardNumber,
                StrEmployeeName = obj.StrEmployeeName,
                IntGenderId = obj.IntGenderId,
                StrGender = obj.StrGender,
                IntReligionId = obj.IntReligionId,
                StrMaritalStatus = obj.StrMaritalStatus,
                StrBloodGroup = obj.StrBloodGroup,
                IntDepartmentId = obj.IntDepartmentId,
                IntDesignationId = obj.IntDesignationId,
                DteDateOfBirth = obj.DteDateOfBirth,
                DteJoiningDate = obj.DteJoiningDate,
                DteConfirmationDate = obj.DteConfirmationDate,
                DteLastWorkingDate = obj.DteLastWorkingDate,
                IntSupervisorId = obj.IntSupervisorId,
                IntLineManagerId = obj.IntLineManagerId,
                IntDottedSupervisorId = obj.IntDottedSupervisorId,
                IsSalaryHold = obj.IsSalaryHold,
                IsActive = obj.IsActive,
                IsUserInactive = obj.IsUserInactive,
                IsRemoteAttendance = obj.IsRemoteAttendance,
                IntWorkplaceId = obj.IntWorkplaceId,
                IntAccountId = obj.IntAccountId,
                IntBusinessUnitId = obj.IntBusinessUnitId,
                DteCreatedAt = obj.DteCreatedAt,
                IntCreatedBy = obj.IntCreatedBy
            };

            if (obj.IntEmployeeBasicInfoId > 0)
            {
                emp.IntUpdatedBy = obj.IntUpdatedBy;
                emp.DteUpdatedAt = DateTime.Now;

                _context.Entry(emp).State = EntityState.Modified;
                _context.Entry(emp).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(emp).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();

                res.StatusCode = 200;
                res.Message = "UPDATE SUCCESSFULLY";
            }
            else
            {
                emp.IntCreatedBy = obj.IntCreatedBy;
                emp.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeeBasicInfos.AddAsync(emp);
                await _context.SaveChangesAsync();

                res.StatusCode = 200;
                res.Message = "CREATE SUCCESSFULLY";
            }

            return obj.IntEmployeeBasicInfoId;
        }

        public async Task<IEnumerable<EmpEmployeeBasicInfo>> GetAllEmpBasicInfo(long accountId)
        {
            return await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == accountId && x.IsActive == true).OrderByDescending(x => x.IntEmployeeBasicInfoId).ToListAsync();
        }

        public async Task<dynamic> GetEmpBasicInfoById(long id)
        {
            try
            {
                var data = await (from emp in _context.EmpEmployeeBasicInfos
                                  where emp.IntEmployeeBasicInfoId == id
                                  join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dd
                                  from dept in dd.DefaultIfEmpty()
                                  join de in _context.MasterDesignations on emp.IntDesignationId equals de.IntDesignationId into dess
                                  from des in dess.DefaultIfEmpty()
                                  join s in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals s.IntEmployeeBasicInfoId into ss
                                  from sup in ss.DefaultIfEmpty()
                                  join dotteds in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dotteds.IntEmployeeBasicInfoId into dottedss
                                  from dottedSup in dottedss.DefaultIfEmpty()
                                  join l in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals l.IntEmployeeBasicInfoId into ll
                                  from man in ll.DefaultIfEmpty()
                                  join et in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals et.IntEmploymentTypeId into ett
                                  from empT in ett.DefaultIfEmpty()
                                  join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wg2
                                  from wg in wg2.DefaultIfEmpty()
                                  join photo in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo.IntEmployeeBasicInfoId into photo2
                                  from photo in photo2.DefaultIfEmpty()
                                  join att in _context.TimeAttendanceDailySummaries on new { EmployeeId = emp.IntEmployeeBasicInfoId, DayId = DateTime.Now.Day, MonthId = DateTime.Now.Month } equals new { EmployeeId = att.IntEmployeeId, DayId = att.DteAttendanceDate.Value.Day, MonthId = att.DteAttendanceDate.Value.Month } into att2
                                  from att in att2.DefaultIfEmpty()

                                  select new
                                  {
                                      WorkplaceGroupId = wg.IntWorkplaceGroupId,
                                      EmployeeId = emp.IntEmployeeBasicInfoId,
                                      EmployeeCode = emp.StrEmployeeCode,
                                      CardNumber = emp.StrCardNumber,
                                      EmployeeName = emp.StrEmployeeName,
                                      att.StrCalendarName,
                                      ServiceLength = YearMonthDayCalculate.YearMonthDayLongFormCal((DateTime)emp.DteJoiningDate, DateTime.Now.Date),
                                      Gender = emp.StrGender,
                                      MaritalStatus = emp.StrMaritalStatus,
                                      BloodGroup = emp.StrBloodGroup,
                                      DateOfBirth = emp.DteDateOfBirth,
                                      JoiningDate = emp.DteJoiningDate,
                                      EmploymentType = emp.StrEmploymentType,
                                      dept.StrDepartment,
                                      des.StrDesignation,
                                      supervisor = sup.StrEmployeeName ?? "",
                                      dotterSupervisor = dottedSup.StrEmployeeName ?? "",
                                      lineManager = man.StrEmployeeName ?? "",
                                      ImageUrlId = photo.IntProfilePicFileUrlId ?? 0
                                  }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEmpBasicInfo(long id)
        {
            try
            {
                EmpEmployeeBasicInfo obj = await _context.EmpEmployeeBasicInfos.FirstAsync(x => x.IntEmployeeBasicInfoId == id);
                obj.IsActive = false;
                _context.EmpEmployeeBasicInfos.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task<MessageHelper> CRUDTblEmployeeBasicInfo(EmployeeBasicViewModel obj)
        //{
        //    try
        //    {
        //        List<EmpEmployeeBasicInfo> managementList = new List<EmpEmployeeBasicInfo>();
        //        EmpEmployeeBasicInfo emp = new EmpEmployeeBasicInfo();
        //        long oldEmploymentTypeId = 0;

        //        if (obj.StrPart.ToLower() == "EditEmployee".ToLower() && obj.IntEmployeeBasicInfoId > 0)
        //        {
        //            oldEmploymentTypeId = _context.EmpEmployeeBasicInfos.FirstOrDefault(x => x.IntEmployeeBasicInfoId == obj.IntEmployeeBasicInfoId).IntEmploymentTypeId;
        //        }

        //        if (obj.StrPart.ToLower() == "CreateEmployee".ToLower() || obj.StrPart.ToLower() == "EditEmployee".ToLower())
        //        {
        //            emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == obj.IntSupervisorId);
        //            if (emp != null && emp.IntEmployeeBasicInfoId > 0)
        //            {
        //                managementList.Add(emp);
        //                emp = new EmpEmployeeBasicInfo();
        //            }
        //            emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == obj.IntDottedSupervisorId);
        //            if (emp != null && emp.IntEmployeeBasicInfoId > 0)
        //            {
        //                managementList.Add(emp);
        //                emp = new EmpEmployeeBasicInfo();
        //            }
        //            emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == obj.IntLineManagerId);
        //            if (emp != null && emp.IntEmployeeBasicInfoId > 0)
        //            {
        //                managementList.Add(emp);
        //                emp = new EmpEmployeeBasicInfo();
        //            }
        //        }

        //        var connection = new SqlConnection(Connection.iPEOPLE_HCM);
        //        string sql = "saas.sprEmployeeBasicInfo";
        //        SqlCommand sqlCmd = new SqlCommand(sql, connection);
        //        sqlCmd.CommandType = CommandType.StoredProcedure;

        //        sqlCmd.Parameters.AddWithValue("@StrPart", obj.StrPart);
        //        sqlCmd.Parameters.AddWithValue("@IntEmployeeBasicInfoId", obj.IntEmployeeBasicInfoId);
        //        sqlCmd.Parameters.AddWithValue("@StrEmployeeCode", obj.StrEmployeeCode);
        //        sqlCmd.Parameters.AddWithValue("@StrCardNumber", obj.StrCardNumber);
        //        sqlCmd.Parameters.AddWithValue("@StrEmployeeName", obj.StrEmployeeName);
        //        sqlCmd.Parameters.AddWithValue("@IntGenderId", obj.IntGenderId);
        //        sqlCmd.Parameters.AddWithValue("@StrGender", obj.StrGender);
        //        sqlCmd.Parameters.AddWithValue("@IntReligionId", obj.IntReligionId);
        //        sqlCmd.Parameters.AddWithValue("@StrReligion", obj.StrReligion);
        //        sqlCmd.Parameters.AddWithValue("@StrMaritalStatus", obj.StrMaritalStatus);
        //        sqlCmd.Parameters.AddWithValue("@StrBloodGroup", obj.StrBloodGroup);
        //        sqlCmd.Parameters.AddWithValue("@IntDepartmentId", obj.IntDepartmentId);
        //        sqlCmd.Parameters.AddWithValue("@IntDesignationId", obj.IntDesignationId);
        //        sqlCmd.Parameters.AddWithValue("@DteDateOfBirth", obj.DteDateOfBirth);
        //        sqlCmd.Parameters.AddWithValue("@DteJoiningDate", obj.DteJoiningDate);
        //        sqlCmd.Parameters.AddWithValue("@dteInternCloseDate", obj.DteInternCloseDate);
        //        sqlCmd.Parameters.AddWithValue("@dteProbationaryCloseDate", obj.DteProbationaryCloseDate);
        //        sqlCmd.Parameters.AddWithValue("@DteConfirmationDate", obj.DteConfirmationDate);
        //        sqlCmd.Parameters.AddWithValue("@DteContactFromDate", obj.DteContactFromDate);
        //        sqlCmd.Parameters.AddWithValue("@DteContactToDate", obj.DteContactToDate);
        //        sqlCmd.Parameters.AddWithValue("@DteLastWorkingDate", obj.DteLastWorkingDate);
        //        sqlCmd.Parameters.AddWithValue("@IntSupervisorId", obj.IntSupervisorId);
        //        sqlCmd.Parameters.AddWithValue("@IntLineManagerId", obj.IntLineManagerId);
        //        sqlCmd.Parameters.AddWithValue("@IntDottedSupervisorId", obj.IntDottedSupervisorId);
        //        sqlCmd.Parameters.AddWithValue("@IsSalaryHold", obj.IsSalaryHold);
        //        sqlCmd.Parameters.AddWithValue("@IsActive", obj.IsActive);
        //        sqlCmd.Parameters.AddWithValue("@IsUserInactive", obj.IsUserInactive);
        //        sqlCmd.Parameters.AddWithValue("@IsRemoteAttendance", obj.IsRemoteAttendance);
        //        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", obj.IntWorkplaceGroupId);
        //        sqlCmd.Parameters.AddWithValue("@IntWorkplaceId", obj.IntWorkplaceId);
        //        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", obj.IntBusinessUnitId);
        //        sqlCmd.Parameters.AddWithValue("@IntEmploymentTypeId", obj.IntEmploymentTypeId);
        //        sqlCmd.Parameters.AddWithValue("@StrEmploymentType", obj.StrEmploymentType);
        //        sqlCmd.Parameters.AddWithValue("@IntAccountId", obj.IntAccountId);
        //        sqlCmd.Parameters.AddWithValue("@DteCreatedAt", obj.DteCreatedAt);
        //        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", obj.IntCreatedBy);
        //        sqlCmd.Parameters.AddWithValue("@DteUpdatedAt", obj.DteUpdatedAt);
        //        sqlCmd.Parameters.AddWithValue("@IntUpdatedBy", obj.IntUpdatedBy);
        //        sqlCmd.Parameters.AddWithValue("@IntPayrollGroupId", obj.IntPayrollGroupId);
        //        sqlCmd.Parameters.AddWithValue("@StrPayrollGroup", obj.StrPayrollGroupName);
        //        sqlCmd.Parameters.AddWithValue("@IntPascalGradeId", obj.IntPayscaleGradeId);
        //        sqlCmd.Parameters.AddWithValue("@StrPascalGrade", obj.StrPayscaleGradeName);
        //        sqlCmd.Parameters.AddWithValue("@IntCalenderId", obj.IntCalenderId);
        //        sqlCmd.Parameters.AddWithValue("@StrCalender", obj.StrCalenderName);
        //        sqlCmd.Parameters.AddWithValue("@IntPositionId", obj.IntHrpositionId);
        //        sqlCmd.Parameters.AddWithValue("@StrPosition", obj.StrHrpostionName);
        //        sqlCmd.Parameters.AddWithValue("@IntEmployeeStatusId", obj.IntEmployeeStatusId);
        //        sqlCmd.Parameters.AddWithValue("@StrEmployeeStatus", obj.StrEmployeeStatus);
        //        sqlCmd.Parameters.AddWithValue("@StrPersonalMail", obj.StrPersonalMail);
        //        sqlCmd.Parameters.AddWithValue("@StrOfficeMail", obj.StrOfficeMail);
        //        sqlCmd.Parameters.AddWithValue("@StrPersonalMobile", obj.StrPersonalMobile);
        //        sqlCmd.Parameters.AddWithValue("@StrOfficeMobile", obj.StrOfficeMobile);
        //        sqlCmd.Parameters.AddWithValue("@isTakeHomePay", obj.IsTakeHomePay);
        //        sqlCmd.Parameters.AddWithValue("@IntWingId", obj.WingId);
        //        sqlCmd.Parameters.AddWithValue("@IntSoleDepoId", obj.SoleDepoId);
        //        sqlCmd.Parameters.AddWithValue("@IntRegionId", obj.RegionId);
        //        sqlCmd.Parameters.AddWithValue("@IntAreaId", obj.AreaId);
        //        sqlCmd.Parameters.AddWithValue("@IntTerritoryId", obj.TerritoryId);
        //        sqlCmd.Parameters.AddWithValue("@strPinNo", obj.PinNo);

        //        connection.Open();
        //        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
        //        {
        //            sqlAdapter.Fill(dt);
        //        }
        //        connection.Close();

        //        var msg = new MessageHelper();
        //        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
        //        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

        //        if (msg.StatusCode == 500)
        //        {
        //            return msg;
        //        }
        //        else
        //        {
        //            msg.AutoId = Convert.ToInt64(dt.Rows[0]["autoId"]);

        //            #region User Create

        //            if (obj.IsCreateUser == true && (obj.StrPart.ToLower() == "CreateEmployee".ToLower() || obj.StrPart.ToLower() == "EditEmployee".ToLower()) && msg.AutoId > 0 && obj.UserViewModel != null)
        //            {
        //                if (!string.IsNullOrEmpty(obj.UserViewModel.StrLoginId) && !string.IsNullOrEmpty(obj.UserViewModel.StrPassword)
        //                    && obj.UserViewModel.IntUrlId > 0)
        //                {
        //                    long isExists = await _context.Users.Where(x => x.IntAccountId == obj.IntAccountId && x.StrLoginId == obj.UserViewModel.StrLoginId && x.IntUrlId == obj.UserViewModel.IntUrlId).CountAsync();
        //                    long globalUserExists = await _context.GlobalUserUrls.Where(x => x.StrLoginId == obj.UserViewModel.StrLoginId && x.IsActive == true).CountAsync();

        //                    if (isExists <= 0 && globalUserExists <= 0)
        //                    {
        //                        var Password = Encoding.UTF8.GetBytes(obj.UserViewModel.StrPassword);
        //                        string encryptedPassword = Convert.ToBase64String(Password);

        //                        User user = new User
        //                        {
        //                            StrLoginId = obj.UserViewModel.StrLoginId,
        //                            StrPassword = encryptedPassword,
        //                            //StrOldPassword = model.StrOldPassword,
        //                            StrDisplayName = obj.StrEmployeeName,
        //                            IntUserTypeId = (long)obj.UserViewModel.IntUserTypeId,
        //                            IntRefferenceId = msg.AutoId,
        //                            IsOfficeAdmin = obj.UserViewModel.IsOfficeAdmin,
        //                            IsSuperuser = obj.UserViewModel.IsSuperuser,
        //                            //DteLastLogin = model.DteLastLogin
        //                            IsActive = obj.IsActive,
        //                            IntUrlId = (long)obj.UserViewModel.IntUrlId,
        //                            IntAccountId = (long)obj.IntAccountId,
        //                            DteCreatedAt = DateTime.Now,
        //                            IntCreatedBy = obj.IntCreatedBy
        //                        };

        //                        //save into global url
        //                        GlobalUserUrl globalUserUrl = new()
        //                        {
        //                            StrLoginId = obj.UserViewModel.StrLoginId,
        //                            IntUrlId = (long)obj.UserViewModel.IntUrlId,
        //                            IsActive = true
        //                        };
        //                        await _context.GlobalUserUrls.AddAsync(globalUserUrl);

        //                        User CreatedUser = await _authService.SaveUser(user);

        //                        EmpEmployeeBasicInfoDetail employeeBasicInfoDetail = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == (long)msg.AutoId);
        //                        if (employeeBasicInfoDetail != null)
        //                        {
        //                            employeeBasicInfoDetail.StrOfficeMail = obj.UserViewModel.IntOfficeMail;
        //                            employeeBasicInfoDetail.StrPersonalMobile = obj.UserViewModel.StrContactNo;

        //                            _context.EmpEmployeeBasicInfoDetails.Update(employeeBasicInfoDetail);
        //                            await _context.SaveChangesAsync();
        //                        }
        //                        else
        //                        {
        //                            EmpEmployeeBasicInfoDetail empDetails = new EmpEmployeeBasicInfoDetail
        //                            {
        //                                IntEmployeeId = (long)msg.AutoId,
        //                                StrOfficeMail = obj.UserViewModel.IntOfficeMail,
        //                                StrPersonalMobile = obj.UserViewModel.StrContactNo,
        //                                DteCreatedAt = DateTime.Now,
        //                                IntCreatedBy = obj.IntCreatedBy
        //                            };
        //                            await _context.EmpEmployeeBasicInfoDetails.AddAsync(empDetails);
        //                            await _context.SaveChangesAsync();
        //                        }
        //                    }
        //                }
        //            }

        //            #endregion User Create

        //            #region Calendar Assign

        //            if (obj.StrPart.ToLower() == "CreateEmployee".ToLower())
        //            {
        //                if (obj.DteJoiningDate.Value.Year < DateTime.Now.Year)
        //                {
        //                    obj.calendarAssignViewModel.GenerateStartDate = new DateTime(DateTime.Now.Year, 1, 1);
        //                }

        //                string sql3 = "saas.sprRosterGenerate";
        //                SqlCommand sqlCmd3 = new SqlCommand(sql3, connection);
        //                sqlCmd3.CommandType = CommandType.StoredProcedure;
        //                sqlCmd3.Parameters.AddWithValue("@intEmployeeId", msg.AutoId);
        //                sqlCmd3.Parameters.AddWithValue("@dteGenerateStartDate", obj.calendarAssignViewModel.GenerateStartDate);
        //                sqlCmd3.Parameters.AddWithValue("@intRunningCalendarId", obj.calendarAssignViewModel.RunningCalendarId);
        //                sqlCmd3.Parameters.AddWithValue("@strCalendarType", obj.calendarAssignViewModel.CalendarType);
        //                sqlCmd3.Parameters.AddWithValue("@intRosterGroupId", obj.calendarAssignViewModel.RosterGroupId);
        //                sqlCmd3.Parameters.AddWithValue("@dteNextChangeDate", obj.calendarAssignViewModel.NextChangeDate);
        //                sqlCmd3.Parameters.AddWithValue("@dteGenerateEndDate", obj.calendarAssignViewModel.GenerateEndDate);
        //                sqlCmd3.Parameters.AddWithValue("@intCreatedBy", obj.IntCreatedBy);
        //                sqlCmd3.Parameters.AddWithValue("@isAutoGenerate", false);

        //                connection.Open();
        //                sqlCmd3.ExecuteNonQuery();
        //                connection.Close();
        //            }

        //            #endregion Calendar Assign

        //            if (obj.StrPart.ToLower() == "CreateEmployee".ToLower() || obj.StrPart.ToLower() == "EditEmployee".ToLower() || obj.StrPart.ToLower() == "Confirmation".ToLower())
        //            {
        //                if (msg.AutoId > 0)
        //                {
        //                    #region Leave Balance Generate

        //                    string sql2 = "saas.sprLeaveBalanceGenerate";
        //                    SqlCommand sqlCmd2 = new SqlCommand(sql2, connection);
        //                    sqlCmd2.CommandType = CommandType.StoredProcedure;
        //                    sqlCmd2.Parameters.AddWithValue("@intEmployeeId", msg.AutoId);
        //                    sqlCmd2.Parameters.AddWithValue("@isAutoGenerate", 0);
        //                    sqlCmd2.Parameters.AddWithValue("@intCreatedBy", msg.AutoId);
        //                    sqlCmd2.Parameters.AddWithValue("@oldEmploymentTypeId", oldEmploymentTypeId);
        //                    connection.Open();
        //                    sqlCmd2.ExecuteNonQuery();
        //                    connection.Close();

        //                    #endregion Leave Balance Generate

        //                    if (obj.StrPart.ToLower() != "Confirmation".ToLower())
        //                    {
        //                        EmpEmployeeBasicInfo empInfo = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == (long)msg.AutoId && x.IsActive == true).FirstOrDefault();

        //                        if (empInfo.IntSupervisorId == empInfo.IntDottedSupervisorId && empInfo.IntSupervisorId == empInfo.IntLineManagerId)
        //                        {
        //                            bool IsMenuPermission = await LeaveNMovementMenuPermission((long)empInfo.IntSupervisorId, (long)obj.IntCreatedBy);
        //                        }
        //                        else if (empInfo.IntSupervisorId == empInfo.IntDottedSupervisorId && empInfo.IntSupervisorId != empInfo.IntLineManagerId)
        //                        {
        //                            bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)empInfo.IntSupervisorId, (long)obj.IntCreatedBy);
        //                            bool IsMenuPermissionLM = await LeaveNMovementMenuPermission((long)empInfo.IntLineManagerId, (long)obj.IntCreatedBy);
        //                        }
        //                        else if (empInfo.IntSupervisorId == empInfo.IntLineManagerId && empInfo.IntSupervisorId != empInfo.IntDottedSupervisorId)
        //                        {
        //                            bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)empInfo.IntSupervisorId, (long)obj.IntCreatedBy);
        //                            bool IsMenuPermissionDSup = await LeaveNMovementMenuPermission((long)empInfo.IntDottedSupervisorId, (long)obj.IntCreatedBy);
        //                        }
        //                        else
        //                        {
        //                            bool IsMenPermissionDSup = await LeaveNMovementMenuPermission((long)empInfo.IntDottedSupervisorId, (long)obj.IntCreatedBy);
        //                            bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)empInfo.IntSupervisorId, (long)obj.IntCreatedBy);
        //                            bool IsMenuPermissionLM = await LeaveNMovementMenuPermission((long)empInfo.IntLineManagerId, (long)obj.IntCreatedBy);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    msg.StatusCode = 500;
        //                    msg.Message = "Invalid EmployeeId After SP Execution";
        //                }
        //            }

        //            return msg;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<bool> LeaveNMovementMenuPermission(long IntEmployeeId, long IntCreatedBy)
        {
            try
            {
                EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == IntEmployeeId && x.IsActive == true).FirstOrDefaultAsync();

                List<MenuUserPermissionViewModel> menuPermission = await _authService.GetMenuUserPermission(IntEmployeeId, "Employee".ToLower());

                EmpIsSupNLMORUGMemberViewModel empIsSupNLMORUG = await _approvalPipelineService.EmployeeIsSupervisorNLineManagerORUserGroupMember(emp.IntAccountId, IntEmployeeId);

                EmpIsSupNLMORUGMemberViewModel leaveApplication = await _approvalPipelineService.PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(emp.IntAccountId, "leave".ToLower(), IntEmployeeId);
                EmpIsSupNLMORUGMemberViewModel movementApplication = await _approvalPipelineService.PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(emp.IntAccountId, "movement".ToLower(), IntEmployeeId);
                EmpIsSupNLMORUGMemberViewModel manualAttendance = await _approvalPipelineService.PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(emp.IntAccountId, "manualAttendance".ToLower(), IntEmployeeId);
                EmpIsSupNLMORUGMemberViewModel remoteAttendance = await _approvalPipelineService.PipelineRowDetailsByApplicationTypeForIsSupNLMORUG(emp.IntAccountId, "remoteAttendance".ToLower(), IntEmployeeId);

                // IntmenuId = 98 (Leave Approval)
                if (menuPermission.Where(x => x.IntMenuId == 98).Count() == 0)
                {
                    if ((empIsSupNLMORUG.IsSupervisor == true && leaveApplication.IsSupervisor == true)
                        || (empIsSupNLMORUG.IsLineManager == true && leaveApplication.IsLineManager == true))
                    {
                        MenuPermission menuPer = new MenuPermission
                        {
                            IntModuleId = 4,
                            StrModuleName = "Approval",
                            IntMenuId = 98,
                            StrMenuName = "Leave Approval",
                            StrIsFor = "Employee",
                            IntEmployeeOrRoleId = IntEmployeeId,
                            StrEmployeeName = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "], " + _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                            IntAccountId = emp.IntAccountId,
                            IsView = true,
                            IsCreate = true,
                            IsEdit = true,
                            IsDelete = true,
                            IsActive = true,
                            IsForWeb = true,
                            IsForApps = true,
                            IntCreatedBy = IntCreatedBy,
                            DteCreatedAt = DateTime.UtcNow
                        };

                        await _context.MenuPermissions.AddAsync(menuPer);
                        await _context.SaveChangesAsync();
                    }
                }

                // IntmenuId = 104 (Movement Approval)
                if (menuPermission.Where(x => x.IntMenuId == 104).Count() == 0)
                {
                    if ((empIsSupNLMORUG.IsSupervisor == true && movementApplication.IsSupervisor == true)
                        || (empIsSupNLMORUG.IsLineManager == true && movementApplication.IsLineManager == true))
                    {
                        MenuPermission menuPer = new MenuPermission
                        {
                            IntModuleId = 4,
                            StrModuleName = "Approval",
                            IntMenuId = 104,
                            StrMenuName = "Movement Approval",
                            StrIsFor = "Employee",
                            IntEmployeeOrRoleId = IntEmployeeId,
                            StrEmployeeName = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "], " + _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                            IntAccountId = emp.IntAccountId,
                            IsView = true,
                            IsCreate = true,
                            IsEdit = true,
                            IsDelete = true,
                            IsActive = true,
                            IsForWeb = true,
                            IsForApps = true,
                            IntCreatedBy = IntCreatedBy,
                            DteCreatedAt = DateTime.UtcNow
                        };

                        await _context.MenuPermissions.AddAsync(menuPer);
                        await _context.SaveChangesAsync();
                    }
                }
                // IntmenuId = 105 (Attendance Approval / Manual Attendance)
                if (menuPermission.Where(x => x.IntMenuId == 105).Count() == 0)
                {
                    if ((empIsSupNLMORUG.IsSupervisor == true && manualAttendance.IsSupervisor == true)
                        || (empIsSupNLMORUG.IsLineManager == true && manualAttendance.IsLineManager == true))
                    {
                        MenuPermission menuPer = new MenuPermission
                        {
                            IntModuleId = 4,
                            StrModuleName = "Approval",
                            IntMenuId = 105,
                            StrMenuName = "Attendance Approval",
                            StrIsFor = "Employee",
                            IntEmployeeOrRoleId = IntEmployeeId,
                            StrEmployeeName = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "], " + _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                            IntAccountId = emp.IntAccountId,
                            IsView = true,
                            IsCreate = true,
                            IsEdit = true,
                            IsDelete = true,
                            IsActive = true,
                            IsForWeb = true,
                            IsForApps = true,
                            IntCreatedBy = IntCreatedBy,
                            DteCreatedAt = DateTime.UtcNow
                        };

                        await _context.MenuPermissions.AddAsync(menuPer);
                        await _context.SaveChangesAsync();
                    }
                }
                // IntMenuId = 30319 (Remote Attendance)
                if (menuPermission.Where(x => x.IntMenuId == 30319).Count() == 0)
                {
                    if ((empIsSupNLMORUG.IsSupervisor == true && remoteAttendance.IsSupervisor == true)
                        || (empIsSupNLMORUG.IsLineManager == true && remoteAttendance.IsLineManager == true))
                    {
                        MenuPermission menuPer = new MenuPermission
                        {
                            IntModuleId = 4,
                            StrModuleName = "Approval",
                            IntMenuId = 30319,
                            StrMenuName = "Remote Attendance",
                            StrIsFor = "Employee",
                            IntEmployeeOrRoleId = IntEmployeeId,
                            StrEmployeeName = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "], " + _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                            IntAccountId = emp.IntAccountId,
                            IsView = true,
                            IsCreate = true,
                            IsEdit = true,
                            IsDelete = true,
                            IsActive = true,
                            IsForWeb = true,
                            IsForApps = true,
                            IntCreatedBy = IntCreatedBy,
                            DteCreatedAt = DateTime.UtcNow
                        };

                        await _context.MenuPermissions.AddAsync(menuPer);
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<EmploymentTypeVM> GetEmploymentParentType(long IntAccountId, long? subEmploymentTypeId)
        {
            EmploymentTypeVM employmentTypeVM = await (from sub in _context.MasterEmploymentTypes
                                                       join parent in _context.MasterEmploymentTypes on sub.IntParentId equals parent.IntEmploymentTypeId
                                                       where sub.IsActive == true && sub.IntAccountId == IntAccountId
                                                       && (subEmploymentTypeId == 0 || sub.IntEmploymentTypeId == subEmploymentTypeId)
                                                       select new EmploymentTypeVM
                                                       {
                                                           IntEmploymentTypeId = sub.IntEmploymentTypeId,
                                                           StrEmploymentType = sub.StrEmploymentType,
                                                           IntParentId = parent.IntEmploymentTypeId,
                                                           StrParentName = parent.StrEmploymentType,
                                                           IntAccountId = sub.IntAccountId,
                                                           IsManual = (parent.IntEmploymentTypeId == 1 && _context.Accounts.Where(x => x.IntAccountId == sub.IntAccountId).Select(x => x.IsProbationaryCloseMenual).FirstOrDefault() == true) ? 1
                                                           : (parent.IntEmploymentTypeId == 3 && _context.Accounts.Where(x => x.IntAccountId == sub.IntAccountId).Select(x => x.IsInternCloseMenual).FirstOrDefault() == true) ? 1 : 0
                                                       }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            return employmentTypeVM;
        }
        public async Task<MessageHelper> CreateNUpdateEmployeeBasicInfo(EmployeeBasicCreateNUpdateVM model, long AccountId, long CreateBy)
        {
            MessageHelper msg = new MessageHelper();

            try
            {
                if (_context.EmpEmployeeBasicInfos.Where(n => n.StrEmployeeCode == model.StrEmployeeCode && n.IntAccountId == AccountId && n.IntEmployeeBasicInfoId != model.IntEmployeeBasicInfoId).Count() > 0)
                {
                    msg.StatusCode = StatusCodes.Status406NotAcceptable;
                    msg.Message = "Employee Code Already Exists!";
                    return msg;
                }


                MasterEmploymentType? parentEmploymentType = await (from sub in _context.MasterEmploymentTypes
                                                                    join parent in _context.MasterEmploymentTypes on sub.IntParentId equals parent.IntEmploymentTypeId
                                                                    where sub.IsActive == true && parent.IsActive == true && sub.IntAccountId == AccountId
                                                                    && sub.IntEmploymentTypeId == model.IntEmploymentTypeId
                                                                    select parent).FirstOrDefaultAsync();

                EmpWorklineConfig? worklineConfig = await _context.EmpWorklineConfigs.FirstOrDefaultAsync(n => n.IntEmploymentTypeId == model.IntEmploymentTypeId && n.IntAccountId == AccountId && n.IsActive == true);

                if (parentEmploymentType?.IntEmploymentTypeId == 3 && model.DteInternCloseDate == null)
                {
                    model.DteInternCloseDate = model.DteJoiningDate.Value.Date.AddDays((long)worklineConfig.IntServiceLengthInDays).Date;
                    model.DteProbationaryCloseDate = null;
                }
                else if (parentEmploymentType?.IntEmploymentTypeId == 1 && model.DteProbationaryCloseDate == null)
                {
                    model.DteProbationaryCloseDate = model.DteJoiningDate.Value.Date.AddDays((long)worklineConfig.IntServiceLengthInDays).Date;
                    model.DteInternCloseDate = null;
                }

                EmpEmployeeBasicInfo employeeBasicInfoCNU = new();

                if (model.IntEmployeeBasicInfoId > 0) /*UPDATE EMPLOYEE*/
                {
                    EmpEmployeeBasicInfo updateEmployee = await _context.EmpEmployeeBasicInfos.AsNoTracking().FirstOrDefaultAsync(n => n.IntEmployeeBasicInfoId == model.IntEmployeeBasicInfoId);

                    if (updateEmployee != null)
                    {

                        updateEmployee = await EmployeeBasicInfoCreateNUpdateMethod(model, updateEmployee, AccountId, CreateBy);

                        if (updateEmployee != null)
                        {
                            model.IntEmployeeBasicInfoId = updateEmployee.IntEmployeeBasicInfoId;
                        }
                        else
                        {
                            msg.StatusCode = StatusCodes.Status400BadRequest;
                            msg.Message = "Failed to update this employee";
                            return msg;
                        }

                        EmpEmployeeBasicInfoDetail basicInfoDetail = await EmployeeBasicInfoDetailsCreateNUpdateMethod(model, CreateBy); /*UPDATE EMPLOYEE DETAILS*/

                        if (basicInfoDetail != null && (model.IntEmployeeStatusId == 2 || model.IntEmployeeStatusId == 3))
                        {
                            User user = await _context.Users.Where(n => n.IntRefferenceId == model.IntEmployeeBasicInfoId).AsNoTracking().FirstOrDefaultAsync();
                            if (user != null)
                            {
                                user.IsActive = false;
                                user.IntUpdatedBy = CreateBy;
                                user.DteUpdatedAt = DateTimeExtend.BD;
                                _context.Users.Update(user);
                                _context.SaveChanges();
                            }
                        }

                        LeaveBalanceGenerate(model.IntEmployeeBasicInfoId, CreateBy, null); /*LeaveBalanceGenerate*/

                        await RoleExtensionCreateNUpdate(model, CreateBy, false); /*ROLE EXTENSION*/

                        employeeBasicInfoCNU = updateEmployee;

                        msg.StatusCode = 200;
                        msg.Message = "Update Successfully!";
                    }

                }
                else
                {
                    var createEmp = await EmployeeBasicInfoCreateNUpdateMethod(model, null, AccountId, CreateBy);

                    if (createEmp != null)
                    {
                        model.IntEmployeeBasicInfoId = createEmp.IntEmployeeBasicInfoId;
                    }
                    else
                    {
                        msg.StatusCode = StatusCodes.Status400BadRequest;
                        msg.Message = "Failed to create new employee";
                        return msg;
                    }

                    await EmployeeBasicInfoDetailsCreateNUpdateMethod(model, CreateBy);

                    if (model.IsCreateUser == true)
                    {
                        model.UserViewModel.IntRefferenceId = createEmp.IntEmployeeBasicInfoId;
                        await UserCreation(model.UserViewModel, AccountId, CreateBy); /*USER CREATE*/
                    }
                    model.calendarAssignViewModel.EmployeeId = createEmp.IntEmployeeBasicInfoId;
                    model.calendarAssignViewModel.JoiningDate = model.DteJoiningDate;

                    CalendarAssignWhenEmployeeCreation(model.calendarAssignViewModel, AccountId, CreateBy); /*CALENDAR ASSIGN*/

                    LeaveBalanceGenerate(createEmp.IntEmployeeBasicInfoId, CreateBy, null); /*LEAVE BALANCE GENERATE*/

                    await RoleExtensionCreateNUpdate(model, CreateBy, true); /*ROLE EXTENSION*/

                    employeeBasicInfoCNU = createEmp;

                    msg.StatusCode = 200;
                    msg.Message = "Create Successfully!";
                }


                if (employeeBasicInfoCNU.IntSupervisorId == employeeBasicInfoCNU.IntDottedSupervisorId && employeeBasicInfoCNU.IntSupervisorId == employeeBasicInfoCNU.IntLineManagerId)
                {
                    bool IsMenuPermission = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntSupervisorId, (long)CreateBy);
                }
                else if (employeeBasicInfoCNU.IntSupervisorId == employeeBasicInfoCNU.IntDottedSupervisorId && employeeBasicInfoCNU.IntSupervisorId != employeeBasicInfoCNU.IntLineManagerId)
                {
                    bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntSupervisorId, (long)CreateBy);
                    bool IsMenuPermissionLM = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntLineManagerId, (long)CreateBy);
                }
                else if (employeeBasicInfoCNU.IntSupervisorId == employeeBasicInfoCNU.IntLineManagerId && employeeBasicInfoCNU.IntSupervisorId != employeeBasicInfoCNU.IntDottedSupervisorId)
                {
                    bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntSupervisorId, (long)CreateBy);
                    bool IsMenuPermissionDSup = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntDottedSupervisorId, (long)CreateBy);
                }
                else
                {
                    bool IsMenPermissionDSup = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntDottedSupervisorId, (long)CreateBy);
                    bool IsMenuPermissionSup = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntSupervisorId, (long)CreateBy);
                    bool IsMenuPermissionLM = await LeaveNMovementMenuPermission((long)employeeBasicInfoCNU.IntLineManagerId, (long)CreateBy);
                }
            }
            catch (Exception ex)
            {
                msg.StatusCode = StatusCodes.Status500InternalServerError;
                msg.Message = ex.Message;
            }

            return msg;
        }

        public async Task<MessageHelper> ConfirmationEmployee(ConfirmationEmployeeVM model, long AccountId, long EmployeeId)
        {
            MessageHelper msg = new();

            long empTypeId = await _context.MasterEmploymentTypes.Where(n => n.IsActive == true && n.IntAccountId == AccountId && n.StrEmploymentType.ToLower() == "Permanent".ToLower()).Select(n => n.IntEmploymentTypeId).FirstOrDefaultAsync();

            if (empTypeId > 0)
            {
                long oldEmploymentTypeId = 0;

                EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(n => n.IntEmployeeBasicInfoId == model.EmployeeId);
                oldEmploymentTypeId = emp.IntEmploymentTypeId;

                emp.IntEmploymentTypeId = empTypeId;
                emp.StrEmploymentType = "Permanent";
                emp.DteConfirmationDate = model.ConfirmationDate;
                emp.IntDesignationId = model.DesignationId;
                emp.IntUpdatedBy = EmployeeId;
                emp.DteUpdatedAt = DateTimeExtend.BD;

                _context.EmpEmployeeBasicInfos.Update(emp);
                await _context.SaveChangesAsync();

                EmpEmployeeBasicInfoDetail empDetails = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(n => n.IntEmployeeId == model.EmployeeId);
                empDetails.StrPinNo = model.PinNo;
                empDetails.IntHrpositionId = (_context.MasterDesignations.Where(n => n.IntDesignationId == model.DesignationId).Select(n => n.IntPositionId).FirstOrDefault());
                empDetails.IntUpdatedBy = EmployeeId;
                empDetails.DteUpdatedAt = DateTimeExtend.BD;

                _context.EmpEmployeeBasicInfoDetails.Update(empDetails);
                await _context.SaveChangesAsync();

                #region Leave Balance Generate
                MessageHelper message = await LeaveBalanceGenerate(model.EmployeeId, EmployeeId, oldEmploymentTypeId);
                if (message.StatusCode != 200)
                {
                    return message;
                }
                #endregion

                msg.StatusCode = 200;
                msg.Message = "Confirm Successfully!";
            }
            else
            {
                msg.StatusCode = 400;
                msg.Message = "Please Create OR Active Permanent Employment Type!";
            }
            return msg;
        }
        public async Task<MessageHelper> CalendarAssignWhenEmployeeCreation(CalendarAssignViewModel model, long AccountId, long CreatedBy)
        {
            MessageHelper messageHelper = new MessageHelper();
            try
            {
                if (model.JoiningDate.Value.Year < DateTime.Now.Year)
                {
                    model.GenerateStartDate = new DateTime(DateTime.Now.Year, 1, 1);
                }
                var connection = new SqlConnection(Connection.iPEOPLE_HCM);
                string sql3 = "saas.sprRosterGenerate";

                SqlCommand sqlCmd3 = new SqlCommand(sql3, connection);
                sqlCmd3.CommandType = CommandType.StoredProcedure;
                sqlCmd3.Parameters.AddWithValue("@intEmployeeId", model.EmployeeId);
                sqlCmd3.Parameters.AddWithValue("@dteGenerateStartDate", model.GenerateStartDate);
                sqlCmd3.Parameters.AddWithValue("@intRunningCalendarId", model.RunningCalendarId);
                sqlCmd3.Parameters.AddWithValue("@strCalendarType", model.CalendarType);
                sqlCmd3.Parameters.AddWithValue("@intRosterGroupId", model.RosterGroupId);
                sqlCmd3.Parameters.AddWithValue("@dteNextChangeDate", model.NextChangeDate);
                sqlCmd3.Parameters.AddWithValue("@dteGenerateEndDate", model.GenerateEndDate);
                sqlCmd3.Parameters.AddWithValue("@intCreatedBy", CreatedBy);
                sqlCmd3.Parameters.AddWithValue("@isAutoGenerate", false);

                connection.Open();
                sqlCmd3.ExecuteNonQuery();
                connection.Close();

                messageHelper.StatusCode = 200;
                messageHelper.Message = "Calendar Assign Successfully!";
            }
            catch (Exception ex)
            {
                messageHelper.StatusCode = 500;
                messageHelper.Message = ex.Message;
            }
            return messageHelper;
        }
        public async Task<MessageHelper> LeaveBalanceGenerate(long? EmployeeId, long CreateBy, long? OldEmploymentType)
        {
            MessageHelper msg = new MessageHelper();
            try
            {
                var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql2 = "saas.sprLeaveBalanceGenerate";
                SqlCommand sqlCmd2 = new SqlCommand(sql2, connection);
                sqlCmd2.CommandType = CommandType.StoredProcedure;
                sqlCmd2.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                sqlCmd2.Parameters.AddWithValue("@isAutoGenerate", 0);
                sqlCmd2.Parameters.AddWithValue("@intCreatedBy", CreateBy);
                sqlCmd2.Parameters.AddWithValue("@oldEmploymentTypeId", OldEmploymentType);
                connection.Open();
                sqlCmd2.ExecuteNonQuery();
                connection.Close();

                msg.StatusCode = 200;
                msg.Message = "Leave Balance Generate Successful!";
            }
            catch (Exception ex)
            {
                msg.StatusCode = 500;
                msg.Message = ex.Message;
            }
            return msg;
        }
        public async Task<MessageHelper> UserCreation(CreateUserViewModel userModel, long AccountId, long CreateBy)
        {
            MessageHelper msg = new MessageHelper();


            if (!string.IsNullOrEmpty(userModel.StrLoginId) && !string.IsNullOrEmpty(userModel.StrPassword) && userModel.IntUrlId > 0)
            {
                long isExists = await _context.Users.Where(x => x.IntAccountId == AccountId && x.StrLoginId == userModel.StrLoginId && x.IntUrlId == userModel.IntUrlId).CountAsync();
                long globalUserExists = await _context.GlobalUserUrls.Where(x => x.StrLoginId == userModel.StrLoginId && x.IsActive == true).CountAsync();

                if (isExists <= 0 && globalUserExists <= 0)
                {
                    var Password = Encoding.UTF8.GetBytes(userModel.StrPassword);
                    string encryptedPassword = Convert.ToBase64String(Password);

                    User user = new User
                    {
                        StrLoginId = userModel.StrLoginId,
                        StrPassword = encryptedPassword,
                        //StrOldPassword = model.StrOldPassword,
                        StrDisplayName = _context.EmpEmployeeBasicInfos.Where(n => n.IntEmployeeBasicInfoId == userModel.IntRefferenceId).Select(n => n.StrEmployeeName).FirstOrDefault(),
                        IntUserTypeId = (long)userModel.IntUserTypeId,
                        IntRefferenceId = userModel.IntRefferenceId,
                        IsOfficeAdmin = userModel.IsOfficeAdmin,
                        IsSuperuser = userModel.IsSuperuser,
                        //DteLastLogin = model.DteLastLogin
                        IsActive = true,
                        IntUrlId = (long)userModel.IntUrlId,
                        IntAccountId = AccountId,
                        DteCreatedAt = DateTimeExtend.BD,
                        IntCreatedBy = CreateBy
                    };

                    //save into global url
                    GlobalUserUrl globalUserUrl = new()
                    {
                        StrLoginId = userModel.StrLoginId,
                        IntUrlId = (long)userModel.IntUrlId,
                        IsActive = true
                    };
                    await _context.GlobalUserUrls.AddAsync(globalUserUrl);

                    User CreatedUser = await _authService.SaveUser(user);

                    msg.StatusCode = 200;
                    msg.Message = "User Created successfully!";
                    msg.AutoId = CreatedUser.IntUserId;
                }
                else
                {
                    msg.StatusCode = 400;
                    msg.Message = "User already exists!";
                }
            }
            return msg;
        }

        #endregion ================ Employee Basic Info ==================

        #region ================ Employee Address ===============

        public async Task<MessageHelper> SaveEmployeeAddress(EmployeeAddressViewModel obj)
        {
            try
            {
                MessageHelper Res = new MessageHelper();

                EmpEmployeeAddress address = new EmpEmployeeAddress
                {
                    IntEmployeeAddressId = obj.IntEmployeeAddressId,
                    IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                    IntAddressTypeId = obj.IntAddressTypeId,
                    StrAddressType = obj.StrAddressType,
                    IntCountryId = obj.IntCountryId,
                    StrCountry = obj.StrCountry,
                    StrDivision = obj.StrDivision,
                    StrDistrictOrState = obj.StrDistrictOrState,
                    StrAddressDetails = obj.StrAddressDetails,
                    StrZipOrPostCode = obj.StrZipOrPostCode,
                    IsActive = true,
                    //IntWorkplaceId = obj.IntWorkplaceId,
                    //IntAccountId = obj.IntAccountId,
                    //IntBusinessUnitId = obj.IntBusinessUnitId,
                    IntCreatedBy = obj.IntCreatedBy,
                    DteCreatedAt = obj.DteCreatedAt
                };

                if (address.IntEmployeeAddressId > 0)
                {
                    address.IntUpdatedBy = obj.IntUpdatedBy;
                    address.DteUpdatedAt = DateTime.Now;

                    _context.Entry(address).State = EntityState.Modified;
                    _context.Entry(address).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(address).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();

                    Res.StatusCode = 200;
                    Res.Message = "UPDATE SUCCESSFULLY";
                }
                else
                {
                    address.IntCreatedBy = obj.IntCreatedBy;
                    address.DteCreatedAt = DateTime.Now;

                    await _context.EmpEmployeeAddresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    Res.StatusCode = 200;
                    Res.Message = "CREATE SUCCESSFULLY";
                }
                return Res;
            }
            catch (Exception ex)
            {
                return new MessageHelper { Message = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<List<EmployeeAddressViewModel>> GetAllEmployeeAddress(long employeeId)
        {
            try
            {
                var obj = await (from empadd in _context.EmpEmployeeAddresses
                                 where empadd.IsActive == true && empadd.IntEmployeeBasicInfoId == employeeId
                                 select new EmployeeAddressViewModel
                                 {
                                     IntEmployeeAddressId = empadd.IntEmployeeAddressId,
                                     IntEmployeeBasicInfoId = empadd.IntEmployeeBasicInfoId,
                                     IntAddressTypeId = empadd.IntAddressTypeId,
                                     StrAddressType = empadd.StrAddressType,
                                     IntCountryId = empadd.IntCountryId,
                                     StrCountry = empadd.StrCountry,
                                     StrDivision = empadd.StrDivision,
                                     StrDistrictOrState = empadd.StrDistrictOrState,
                                     StrAddressDetails = empadd.StrAddressDetails,
                                     StrZipOrPostCode = empadd.StrZipOrPostCode,
                                     IsActive = empadd.IsActive,
                                     //IntWorkplaceId = empadd.IntWorkplaceId,
                                     //IntBusinessUnitId = empadd.IntBusinessUnitId,
                                     //IntAccountId = empadd.IntAccountId,
                                     IntCreatedBy = empadd.IntCreatedBy,
                                     DteCreatedAt = empadd.DteCreatedAt,
                                     IntUpdatedBy = empadd.IntUpdatedBy,
                                     DteUpdatedAt = empadd.DteUpdatedAt
                                 }).OrderByDescending(x => x.IntEmployeeAddressId).ToListAsync();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeAddressViewModel> GetEmployeeAddressById(long id)
        {
            try
            {
                EmployeeAddressViewModel obj = await (from empadd in _context.EmpEmployeeAddresses
                                                      where empadd.IntEmployeeAddressId == id && empadd.IsActive == true
                                                      select new EmployeeAddressViewModel
                                                      {
                                                          IntEmployeeAddressId = empadd.IntEmployeeAddressId,
                                                          IntEmployeeBasicInfoId = empadd.IntEmployeeBasicInfoId,
                                                          IntAddressTypeId = empadd.IntAddressTypeId,
                                                          StrAddressType = empadd.StrAddressType,
                                                          IntCountryId = empadd.IntCountryId,
                                                          StrCountry = empadd.StrCountry,
                                                          StrDivision = empadd.StrDivision,
                                                          StrDistrictOrState = empadd.StrDistrictOrState,
                                                          StrAddressDetails = empadd.StrAddressDetails,
                                                          StrZipOrPostCode = empadd.StrZipOrPostCode,
                                                          IsActive = empadd.IsActive,
                                                          //IntWorkplaceId = empadd.IntWorkplaceId,
                                                          //IntBusinessUnitId = empadd.IntBusinessUnitId,
                                                          //IntAccountId = empadd.IntAccountId,
                                                          IntCreatedBy = empadd.IntCreatedBy,
                                                          DteCreatedAt = empadd.DteCreatedAt,
                                                          IntUpdatedBy = empadd.IntUpdatedBy,
                                                          DteUpdatedAt = empadd.DteUpdatedAt
                                                      }).FirstOrDefaultAsync();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MessageHelper> DeleteEmployeeAddress(long id)
        {
            try
            {
                MessageHelper Res = new MessageHelper();

                EmpEmployeeAddress obj = await (from empadd in _context.EmpEmployeeAddresses
                                                where empadd.IntEmployeeAddressId == id && empadd.IsActive == true
                                                select empadd).FirstOrDefaultAsync();
                if (obj == null)
                {
                    throw new Exception("Data Not Found");
                }
                else
                {
                    obj.IsActive = false;
                    _context.EmpEmployeeAddresses.Update(obj);
                    await _context.SaveChangesAsync();

                    Res.Message = "Delete Successfully";
                    Res.StatusCode = 200;
                }
                return Res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion ================ Employee Address ===============

        #region ============== Employee Bank Details ================

        public async Task<long> SaveEmpBankDetails(EmployeeBankDetailsViewModel obj)
        {
            EmpEmployeeBankDetail data = new EmpEmployeeBankDetail
            {
                IntEmployeeBankDetailsId = obj.IntEmployeeBankDetailsId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                IntBankOrWalletType = obj.IntBankOrWalletType,
                IntBankWalletId = obj.IntBankWalletId,
                StrBankWalletName = obj.StrBankWalletName,
                StrDistrict = obj.StrDistrict,
                StrBranchName = obj.StrBranchName,
                StrRoutingNo = obj.StrRoutingNo,
                StrSwiftCode = obj.StrSwiftCode,
                StrAccountName = obj.StrAccountName,
                StrAccountNo = obj.StrAccountNo,
                IsPrimarySalaryAccount = obj.IsPrimarySalaryAccount,
                IsActive = true,
                IntWorkplaceId = obj.IntWorkplaceId,
                IntAccountId = obj.IntAccountId,
                IntBusinessUnitId = obj.IntBusinessUnitId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = obj.DteCreatedAt
            };

            if (obj.IntEmployeeBasicInfoId != data.IntEmployeeBasicInfoId)
            {
                if (obj.IntEmployeeBankDetailsId > 0)
                {
                    data.IntUpdatedBy = obj.IntUpdatedBy;
                    data.DteUpdatedAt = DateTime.Now;

                    _context.Entry(data).State = EntityState.Modified;
                    _context.Entry(data).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(data).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    data.IntCreatedBy = obj.IntCreatedBy;
                    data.DteCreatedAt = DateTime.Now;

                    await _context.EmpEmployeeBankDetails.AddAsync(data);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception("Already Exists");
            }
            return obj.IntEmployeeBankDetailsId;
        }

        public async Task<MessageHelper> CreateBankBranch(GlobalBankBranchViewModel obj)
        {
            GlobalBankBranch data = new GlobalBankBranch
            {
                IntBankBranchId = obj.IntBankBranchId,
                StrBankBranchCode = obj.StrBankBranchCode,
                StrBankBranchName = obj.StrBankBranchName,
                StrBankBranchAddress = obj.StrBankBranchAddress,
                IntAccountId = obj.IntAccountId,
                IntDistrictId = obj.IntDistrictId,
                IntCountryId = obj.IntCountryId,
                IntBankId = obj.IntBankId,
                StrBankName = obj.StrBankName,
                StrBankShortName = obj.StrBankShortName,
                StrBankCode = obj.StrBankCode,
                StrRoutingNo = obj.StrRoutingNo,
                IsActive = obj.IsActive,
                DteCreatedAt = DateTime.Now,
                IntCreatedBy = obj.IntCreatedBy,
                DteUpdatedAt = obj.DteUpdatedAt,
                IntUpdatedBy = obj.IntUpdatedBy,
            };

            if (obj.IntBankBranchId > 0)
            {
                _context.GlobalBankBranches.Update(data);
                await _context.SaveChangesAsync();

                return new MessageHelper()
                {
                    Message = "Bank Branch Update Successfull",
                    StatusCode = 200
                };
            }
            else
            {
                List<GlobalBankBranch> isExists = await _context.GlobalBankBranches.
                Where(x => x.StrBankBranchName.Trim().ToLower() == obj.StrBankBranchName.Trim().ToLower()
                && (x.IntAccountId == 0 || x.IntAccountId == obj.IntAccountId)).ToListAsync();

                if (isExists.Count() <= 0)
                {
                    await _context.GlobalBankBranches.AddAsync(data);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Already Exist this Branch!");
                }
                return new MessageHelper()
                {
                    Message = "Bank Branch Create Successfull",
                    StatusCode = 200
                };
            }
        }

        public async Task<List<FeatureCommontDDL>> BankBranchDDL(long BankId, long AccountID, long DistrictId)
        {
            try
            {
                var data = await (from gbb in _context.GlobalBankBranches
                                  where gbb.IsActive == true && gbb.IntBankId == BankId
                                  && (gbb.IntAccountId == null || gbb.IntAccountId == 0 || gbb.IntAccountId == AccountID)
                                  && (DistrictId == null || DistrictId == 0 || gbb.IntDistrictId == DistrictId || gbb.IntDistrictId == null || gbb.IntDistrictId == 0)
                                  select new FeatureCommontDDL
                                  {
                                      Value = gbb.IntBankBranchId,
                                      Label = gbb.StrBankBranchName,
                                      Name = gbb.StrRoutingNo,
                                  }).OrderByDescending(x => x.Value).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeBankDetailsViewModel> GetEmpBankDetailsById(long id)
        {
            EmployeeBankDetailsViewModel obj = await (from bd in _context.EmpEmployeeBankDetails
                                                      where bd.IntEmployeeBankDetailsId == id
                                                      select new EmployeeBankDetailsViewModel
                                                      {
                                                          IntEmployeeBankDetailsId = bd.IntEmployeeBankDetailsId,
                                                          IntEmployeeBasicInfoId = bd.IntEmployeeBasicInfoId,
                                                          IntBankWalletId = bd.IntBankWalletId,
                                                          StrBankWalletName = bd.StrBankWalletName,
                                                          IntBankOrWalletType = bd.IntBankOrWalletType,
                                                          StrDistrict = bd.StrDistrict,
                                                          StrBranchName = bd.StrBranchName,
                                                          StrRoutingNo = bd.StrRoutingNo,
                                                          StrSwiftCode = bd.StrSwiftCode,
                                                          StrAccountName = bd.StrAccountName,
                                                          StrAccountNo = bd.StrAccountNo,
                                                          IsPrimarySalaryAccount = bd.IsPrimarySalaryAccount,
                                                          IsActive = bd.IsActive,
                                                          IntWorkplaceId = bd.IntWorkplaceId,
                                                          IntAccountId = bd.IntAccountId,
                                                          IntBusinessUnitId = bd.IntBusinessUnitId,
                                                          IntCreatedBy = bd.IntCreatedBy,
                                                          DteCreatedAt = bd.DteCreatedAt,
                                                          IntUpdatedBy = bd.IntUpdatedBy,
                                                          DteUpdatedAt = bd.DteUpdatedAt
                                                      }).FirstOrDefaultAsync();

            if (obj == null)
            {
                throw new Exception("Data Not Found");
            }
            return obj;
        }

        public async Task<bool> DeleteEmpBankDetails(long id)
        {
            try
            {
                EmpEmployeeBankDetail obj = await _context.EmpEmployeeBankDetails.FirstAsync(x => x.IntEmployeeBankDetailsId == id);
                obj.IsActive = false;
                _context.EmpEmployeeBankDetails.Update(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== Employee Bank Details ================

        #region ================= Employee Education =================

        public async Task<long> SaveEmpEducation(EmployeeEducationViewModel obj)
        {
            EmpEmployeeEducation edu = new EmpEmployeeEducation
            {
                IntEmployeeEducationId = obj.IntEmployeeEducationId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                IntInstituteId = obj.IntInstituteId,
                StrInstituteName = obj.StrInstituteName,
                IsForeign = obj.IsForeign,
                IntCountryId = obj.IntCountryId,
                StrCountry = obj.StrCountry,
                IntEducationDegreeId = obj.IntEducationDegreeId,
                StrEducationDegree = obj.StrEducationDegree,
                IntEducationFieldOfStudyId = obj.IntEducationFieldOfStudyId,
                StrEducationFieldOfStudy = obj.StrEducationFieldOfStudy,
                StrCgpa = obj.StrCgpa,
                StrOutOf = obj.StrOutOf,
                DteStartDate = obj.DteStartDate,
                DteEndDate = obj.DteEndDate,
                IntCertificateFileUrlId = obj.IntCertificateFileUrlId,
                IsActive = obj.IsActive,
                //IntWorkplaceId = obj.IntWorkplaceId,
                //IntAccountId = obj.IntAccountId,
                //IntBusinessUnitId = obj.IntBusinessUnitId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now
            };

            if (obj.IntEmployeeEducationId > 0)
            {
                edu.IntUpdatedBy = obj.IntUpdatedBy;
                edu.DteUpdatedAt = DateTime.Now;

                _context.Entry(edu).State = EntityState.Modified;
                _context.Entry(edu).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(edu).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                edu.IntCreatedBy = obj.IntCreatedBy;
                edu.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeeEducations.AddAsync(edu);
                await _context.SaveChangesAsync();
            }
            return obj.IntEmployeeEducationId;
        }

        public async Task<List<EmployeeEducationViewModel>> GetEmpEducationById(long id)
        {
            List<EmployeeEducationViewModel> edu = await (from ed in _context.EmpEmployeeEducations
                                                          where ed.IsActive == true && ed.IntEmployeeEducationId == id
                                                          select new EmployeeEducationViewModel
                                                          {
                                                              IntEmployeeEducationId = ed.IntEmployeeEducationId,
                                                              IntEmployeeBasicInfoId = ed.IntEmployeeBasicInfoId,
                                                              IntInstituteId = ed.IntInstituteId,
                                                              StrInstituteName = ed.StrInstituteName,
                                                              IsForeign = ed.IsForeign,
                                                              IntCountryId = ed.IntCountryId,
                                                              StrCountry = ed.StrCountry,
                                                              IntEducationDegreeId = ed.IntEducationDegreeId,
                                                              StrEducationDegree = ed.StrEducationDegree,
                                                              IntEducationFieldOfStudyId = ed.IntEducationFieldOfStudyId,
                                                              StrEducationFieldOfStudy = ed.StrEducationFieldOfStudy,
                                                              StrCgpa = ed.StrCgpa,
                                                              StrOutOf = ed.StrOutOf,
                                                              DteStartDate = ed.DteStartDate,
                                                              DteEndDate = ed.DteEndDate,
                                                              IntCertificateFileUrlId = ed.IntCertificateFileUrlId,
                                                              IsActive = ed.IsActive,
                                                              //IntWorkplaceId = ed.IntWorkplaceId,
                                                              //IntAccountId = ed.IntAccountId,
                                                              //IntBusinessUnitId = ed.IntBusinessUnitId,
                                                              IntCreatedBy = ed.IntCreatedBy,
                                                              DteCreatedAt = ed.DteCreatedAt,
                                                              IntUpdatedBy = ed.IntUpdatedBy,
                                                              DteUpdatedAt = ed.DteUpdatedAt
                                                          }).OrderByDescending(x => x.IntEmployeeEducationId).ToListAsync();
            if (edu == null)
            {
                throw new Exception("Data Not Found");
            }
            return edu;
        }

        public async Task<bool> DeleteEmpEducation(long id)
        {
            try
            {
                EmpEmployeeEducation edu = await _context.EmpEmployeeEducations.FirstAsync(x => x.IntEmployeeEducationId == id);
                edu.IsActive = false;
                _context.EmpEmployeeEducations.Update(edu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================= Employee Education =================

        #region ================ Employee Relatives ==================

        public async Task<long> SaveEmpRelativesContact(EmployeeRelativesContactViewModel obj)
        {
            EmpEmployeeRelativesContact relative = new EmpEmployeeRelativesContact
            {
                IntEmployeeRelativesContactId = obj.IntEmployeeRelativesContactId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                StrRelativesName = obj.StrRelativesName,
                StrRelationship = obj.StrRelationship,
                StrPhone = obj.StrPhone,
                StrEmail = obj.StrEmail,
                StrAddress = obj.StrAddress,
                IsEmergencyContact = obj.IsEmergencyContact,
                //StrGrantorNomineeType = obj.StrGrantorNomineeType,
                StrNid = obj.StrNid,
                StrBirthId = obj.StrBirthId,
                DteDateOfBirth = obj.DteDateOfBirth,
                StrRemarks = obj.StrRemarks,
                IntPictureFileUrlId = obj.IntPictureFileUrlId,
                IsActive = obj.IsActive,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now
            };

            if (obj.IntEmployeeRelativesContactId > 0)
            {
                relative.IntUpdatedBy = obj.IntUpdatedBy;
                relative.DteUpdatedAt = DateTime.Now;

                _context.Entry(relative).State = EntityState.Modified;
                _context.Entry(relative).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(relative).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                relative.IntCreatedBy = obj.IntCreatedBy;
                relative.DteCreatedAt = obj.DteCreatedAt;

                await _context.EmpEmployeeRelativesContacts.AddAsync(relative);
                await _context.SaveChangesAsync();
            }
            return obj.IntEmployeeRelativesContactId;
        }

        public async Task<List<EmployeeRelativesContactViewModel>> GetEmpRelativesContactById(long id)
        {
            List<EmployeeRelativesContactViewModel> relative = await (from rel in _context.EmpEmployeeRelativesContacts
                                                                      where rel.IsActive == true && rel.IntEmployeeRelativesContactId == id
                                                                      select new EmployeeRelativesContactViewModel
                                                                      {
                                                                          IntEmployeeRelativesContactId = rel.IntEmployeeRelativesContactId,
                                                                          IntEmployeeBasicInfoId = rel.IntEmployeeBasicInfoId,
                                                                          StrRelativesName = rel.StrRelativesName,
                                                                          StrRelationship = rel.StrRelationship,
                                                                          StrAddress = rel.StrAddress,
                                                                          StrEmail = rel.StrEmail,
                                                                          StrPhone = rel.StrPhone,
                                                                          IsEmergencyContact = rel.IsEmergencyContact,
                                                                          //StrGrantorNomineeType = rel.StrGrantorNomineeType,
                                                                          StrNid = rel.StrNid,
                                                                          StrBirthId = rel.StrBirthId,
                                                                          DteDateOfBirth = rel.DteDateOfBirth,
                                                                          StrRemarks = rel.StrRemarks,
                                                                          IntPictureFileUrlId = rel.IntPictureFileUrlId,
                                                                          IsActive = rel.IsActive,
                                                                          IntCreatedBy = rel.IntCreatedBy,
                                                                          DteCreatedAt = rel.DteCreatedAt,
                                                                          IntUpdatedBy = rel.IntUpdatedBy,
                                                                          DteUpdatedAt = rel.DteUpdatedAt
                                                                      }).OrderByDescending(x => x.IntEmployeeRelativesContactId).ToListAsync();
            if (relative == null)
            {
                throw new Exception("Data Not Found");
            }
            return relative;
        }

        public async Task<bool> DeleteEmpRelativesContact(long id)
        {
            try
            {
                EmpEmployeeRelativesContact relative = await _context.EmpEmployeeRelativesContacts.FirstAsync(x => x.IntEmployeeRelativesContactId == id);
                relative.IsActive = false;
                _context.EmpEmployeeRelativesContacts.Update(relative);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================ Employee Relatives ==================

        #region ====================== Employee Job History ==================

        public async Task<long> SaveEmpJobHistory(EmployeeJobHistoryViewModel obj)
        {
            EmpEmployeeJobHistory history = new EmpEmployeeJobHistory
            {
                IntJobExperienceId = obj.IntJobExperienceId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                StrCompanyName = obj.StrCompanyName,
                StrJobTitle = obj.StrJobTitle,
                StrLocation = obj.StrLocation,
                DteFromDate = obj.DteFromDate,
                DteToDate = obj.DteToDate,
                StrRemarks = obj.StrRemarks,
                IntNocfileUrlId = obj.IntNocfileUrlId,
                IsActive = obj.IsActive,
                IntWorkplaceId = obj.IntWorkplaceId,
                IntAccountId = obj.IntAccountId,
                IntBusinessUnitId = obj.IntBusinessUnitId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now
            };

            if (obj.IntJobExperienceId > 0)
            {
                history.IntUpdatedBy = obj.IntUpdatedBy;
                history.DteUpdatedAt = DateTime.Now;

                _context.Entry(history).State = EntityState.Modified;
                _context.Entry(history).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(history).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                history.IntCreatedBy = obj.IntCreatedBy;
                history.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeeJobHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            return obj.IntJobExperienceId;
        }

        public async Task<List<EmployeeJobHistoryViewModel>> GetEmpJobHistorytById(long id)
        {
            List<EmployeeJobHistoryViewModel> history = await (from a in _context.EmpEmployeeJobHistories
                                                               where a.IsActive == true && a.IntJobExperienceId == id
                                                               select new EmployeeJobHistoryViewModel
                                                               {
                                                                   IntJobExperienceId = a.IntJobExperienceId,
                                                                   IntEmployeeBasicInfoId = a.IntEmployeeBasicInfoId,
                                                                   StrCompanyName = a.StrCompanyName,
                                                                   StrJobTitle = a.StrJobTitle,
                                                                   StrLocation = a.StrLocation,
                                                                   DteFromDate = a.DteFromDate,
                                                                   DteToDate = a.DteToDate,
                                                                   StrRemarks = a.StrRemarks,
                                                                   IntNocfileUrlId = a.IntNocfileUrlId,
                                                                   IsActive = a.IsActive,
                                                                   IntWorkplaceId = a.IntWorkplaceId,
                                                                   IntAccountId = a.IntAccountId,
                                                                   IntBusinessUnitId = a.IntBusinessUnitId,
                                                                   IntCreatedBy = a.IntCreatedBy,
                                                                   DteCreatedAt = a.DteCreatedAt,
                                                                   IntUpdatedBy = a.IntUpdatedBy,
                                                                   DteUpdatedAt = a.DteUpdatedAt
                                                               }).OrderByDescending(x => x.IntJobExperienceId).ToListAsync();
            if (history == null)
            {
                throw new Exception("Data Not Found");
            }
            return history;
        }

        public async Task<bool> DeleteEmpJobHistory(long id)
        {
            try
            {
                EmpEmployeeJobHistory history = await _context.EmpEmployeeJobHistories.FirstAsync(x => x.IntJobExperienceId == id);
                history.IsActive = false;
                _context.EmpEmployeeJobHistories.Update(history);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ====================== Employee Job History ==================

        #region ================== Employee Training ======================

        public async Task<long> SaveEmpTraining(EmployeeTrainingViewModel obj)
        {
            EmpEmployeeTraining training = new EmpEmployeeTraining
            {
                IntTrainingId = obj.IntTrainingId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                StrTrainingTitle = obj.StrTrainingTitle,
                IntInstituteId = obj.IntInstituteId,
                StrInstituteName = obj.StrInstituteName,
                IsForeign = obj.IsForeign,
                IntCountryId = obj.IntCountryId,
                StrCountry = obj.StrCountry,
                DteStartDate = obj.DteStartDate,
                DteEndDate = obj.DteEndDate,
                DteExpiryDate = obj.DteExpiryDate,
                IntTrainingFileUrlId = obj.IntTrainingFileUrlId,
                IsActive = true,
                //IntWorkplaceId = obj.IntWorkplaceId,
                //IntBusinessUnitId = obj.IntBusinessUnitId,
                //IntAccountId = obj.IntAccountId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now
            };

            if (obj.IntTrainingId > 0)
            {
                training.IntUpdatedBy = obj.IntUpdatedBy;
                training.DteUpdatedAt = DateTime.Now;

                _context.Entry(training).State = EntityState.Modified;
                _context.Entry(training).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(training).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                training.IntCreatedBy = obj.IntCreatedBy;
                training.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeeTrainings.AddAsync(training);
                await _context.SaveChangesAsync();
            }
            return obj.IntTrainingId;
        }

        public async Task<List<EmployeeTrainingViewModel>> GetEmpTrainingById(long id)
        {
            List<EmployeeTrainingViewModel> training = await (from tr in _context.EmpEmployeeTrainings
                                                              where tr.IsActive == true && tr.IntTrainingId == id
                                                              select new EmployeeTrainingViewModel
                                                              {
                                                                  IntTrainingId = tr.IntTrainingId,
                                                                  IntEmployeeBasicInfoId = tr.IntEmployeeBasicInfoId,
                                                                  StrTrainingTitle = tr.StrTrainingTitle,
                                                                  IntInstituteId = tr.IntInstituteId,
                                                                  StrInstituteName = tr.StrInstituteName,
                                                                  IsForeign = tr.IsForeign,
                                                                  IntCountryId = tr.IntCountryId,
                                                                  StrCountry = tr.StrCountry,
                                                                  DteStartDate = tr.DteStartDate,
                                                                  DteEndDate = tr.DteEndDate,
                                                                  DteExpiryDate = tr.DteExpiryDate,
                                                                  IntTrainingFileUrlId = tr.IntTrainingFileUrlId,
                                                                  IsActive = tr.IsActive,
                                                                  //IntWorkplaceId = tr.IntWorkplaceId,
                                                                  //IntBusinessUnitId = tr.IntBusinessUnitId,
                                                                  //IntAccountId = tr.IntAccountId,
                                                                  IntCreatedBy = tr.IntCreatedBy,
                                                                  DteCreatedAt = tr.DteCreatedAt,
                                                                  IntUpdatedBy = tr.IntUpdatedBy,
                                                                  DteUpdatedAt = tr.DteUpdatedAt
                                                              }).OrderByDescending(x => x.IntTrainingId).ToListAsync();
            if (training == null)
            {
                throw new Exception("Data Not Found");
            }
            return training;
        }

        public async Task<bool> DeleteEmpTraining(long id)
        {
            try
            {
                EmpEmployeeTraining training = await _context.EmpEmployeeTrainings.FirstAsync(x => x.IntTrainingId == id);
                training.IsActive = false;
                _context.EmpEmployeeTrainings.Update(training);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================== Employee Training ======================

        #region ===================== Employee File ===================

        public async Task<long> SaveEmpFile(EmployeeFileViewModel obj)
        {
            EmpEmployeeFile file = new EmpEmployeeFile
            {
                IntEmployeeFileId = obj.IntEmployeeFileId,
                IntDocumentTypeId = obj.IntDocumentTypeId,
                StrFileTitle = obj.StrFileTitle,
                IntEmployeeFileUrlId = obj.IntEmployeeFileUrlId,
                StrTags = obj.StrTags,
                IsActive = true,
                IntWorkplaceId = obj.IntWorkplaceId,
                IntAccountId = obj.IntAccountId,
                IntBusinessUnitId = obj.IntBusinessUnitId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now,
            };

            if (obj.IntEmployeeFileId > 0)
            {
                file.IntUpdatedBy = obj.IntUpdatedBy;
                file.DteUpdatedAt = DateTime.Now;

                _context.Entry(file).State = EntityState.Modified;
                _context.Entry(file).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(file).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                file.IntCreatedBy = obj.IntCreatedBy;
                file.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeeFiles.AddAsync(file);
                await _context.SaveChangesAsync();
            }
            return obj.IntEmployeeFileId;
        }

        public async Task<List<EmployeeFileViewModel>> GetEmpFileById(long id)
        {
            List<EmployeeFileViewModel> file = await (from f in _context.EmpEmployeeFiles
                                                      where f.IntEmployeeFileId == id && f.IsActive == true
                                                      select new EmployeeFileViewModel
                                                      {
                                                          IntEmployeeFileId = f.IntEmployeeFileId,
                                                          IntDocumentTypeId = f.IntDocumentTypeId,
                                                          StrFileTitle = f.StrFileTitle,
                                                          IntEmployeeFileUrlId = f.IntEmployeeFileUrlId,
                                                          StrTags = f.StrTags,
                                                          IsActive = f.IsActive,
                                                          IntWorkplaceId = f.IntWorkplaceId,
                                                          IntAccountId = f.IntAccountId,
                                                          IntBusinessUnitId = f.IntBusinessUnitId,
                                                          IntCreatedBy = f.IntCreatedBy,
                                                          DteCreatedAt = f.DteCreatedAt,
                                                          IntUpdatedBy = f.IntUpdatedBy,
                                                          DteUpdatedAt = f.DteUpdatedAt
                                                      }).OrderByDescending(x => x.IntEmployeeFileId).ToListAsync();
            if (file == null)
            {
                throw new Exception("Data Not Found");
            }
            return file;
        }

        public async Task<bool> DeleteEmpFile(long id)
        {
            try
            {
                EmpEmployeeFile file = await _context.EmpEmployeeFiles.FirstAsync(x => x.IntEmployeeFileId == id);
                file.IsActive = false;
                _context.EmpEmployeeFiles.Update(file);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ===================== Employee File ===================

        #region ================ Employee Photo Identity ===================

        public async Task<long> SaveEmpPhotoIdentity(EmployeePhotoIdentityViewModel obj)
        {
            EmpEmployeePhotoIdentity photo = new EmpEmployeePhotoIdentity
            {
                IntEmployeePhotoIdentityId = obj.IntEmployeePhotoIdentityId,
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                IntProfilePicFileUrlId = obj.IntProfilePicFileUrlId,
                IntProfilePicFormalFileUrlId = obj.IntProfilePicFormalFileUrlId,
                IntSignatureFileUrlId = obj.IntSignatureFileUrlId,
                StrNid = obj.StrNid,
                IntNidfileUrlId = obj.IntNidfileUrlId,
                IntBirthIdfileUrlId = obj.IntBirthIdfileUrlId,
                StrBirthId = obj.StrBirthId,
                IntPassportFileUrlId = obj.IntPassportFileUrlId,
                StrPassport = obj.StrPassport,
                StrBiography = obj.StrBiography,
                StrNationality = obj.StrNationality,
                StrHobbies = obj.StrHobbies,
                IsActive = true,
                //IntWorkplaceId = obj.IntWorkplaceId,
                //IntAccountId = obj.IntAccountId,
                //IntBusinessUnitId = obj.IntBusinessUnitId,
                IntCreatedBy = obj.IntCreatedBy,
                DteCreatedAt = DateTime.Now
            };

            if (obj.IntEmployeePhotoIdentityId > 0)
            {
                photo.IntUpdatedBy = obj.IntUpdatedBy;
                photo.DteUpdatedAt = DateTime.Now;

                _context.Entry(photo).State = EntityState.Modified;
                _context.Entry(photo).Property(x => x.IntCreatedBy).IsModified = false;
                _context.Entry(photo).Property(x => x.DteCreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                photo.IntCreatedBy = obj.IntCreatedBy;
                photo.DteCreatedAt = DateTime.Now;

                await _context.EmpEmployeePhotoIdentities.AddAsync(photo);
                await _context.SaveChangesAsync();
            }
            return obj.IntEmployeePhotoIdentityId;
        }

        public async Task<EmployeePhotoIdentityViewModel> GetEmpPhotoIdentityById(long id)
        {
            EmployeePhotoIdentityViewModel photo = await (from p in _context.EmpEmployeePhotoIdentities
                                                          where p.IntEmployeePhotoIdentityId == id && p.IsActive == true
                                                          select new EmployeePhotoIdentityViewModel
                                                          {
                                                              IntEmployeePhotoIdentityId = p.IntEmployeePhotoIdentityId,
                                                              IntEmployeeBasicInfoId = p.IntEmployeeBasicInfoId,
                                                              IntProfilePicFileUrlId = p.IntProfilePicFileUrlId,
                                                              IntProfilePicFormalFileUrlId = p.IntProfilePicFormalFileUrlId,
                                                              IntSignatureFileUrlId = p.IntSignatureFileUrlId,
                                                              StrNid = p.StrNid,
                                                              IntNidfileUrlId = p.IntNidfileUrlId,
                                                              StrBirthId = p.StrBirthId,
                                                              IntBirthIdfileUrlId = p.IntBirthIdfileUrlId,
                                                              StrPassport = p.StrPassport,
                                                              IntPassportFileUrlId = p.IntPassportFileUrlId,
                                                              StrNationality = p.StrNationality,
                                                              StrBiography = p.StrBiography,
                                                              StrHobbies = p.StrHobbies,
                                                              IsActive = p.IsActive,
                                                              //IntWorkplaceId = p.IntWorkplaceId,
                                                              //IntAccountId = p.IntAccountId,
                                                              //IntBusinessUnitId = p.IntBusinessUnitId,
                                                              IntCreatedBy = p.IntCreatedBy,
                                                              DteCreatedAt = p.DteCreatedAt,
                                                              IntUpdatedBy = p.IntUpdatedBy,
                                                              DteUpdatedAt = p.DteUpdatedAt
                                                          }).FirstOrDefaultAsync();
            if (photo == null)
            {
                throw new Exception("Data Not Found");
            }
            return photo;
        }

        public async Task<bool> DeleteEmpPhotoIdentity(long id)
        {
            try
            {
                EmpEmployeePhotoIdentity photo = await _context.EmpEmployeePhotoIdentities.FirstAsync(x => x.IntEmployeePhotoIdentityId == id);
                photo.IsActive = false;
                _context.EmpEmployeePhotoIdentities.Update(photo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ================ Employee Photo Identity ===================

        #region ============== Employee Document Management ===============

        public async Task<MessageHelper> SaveEmployeeDocumentManagement(EmployeeDocumentManagementViewModel obj)
        {
            MessageHelper msg = new MessageHelper();
            try
            {
                EmpEmployeeDocumentManagement empDocument = new EmpEmployeeDocumentManagement
                {
                    IntDocumentManagementId = obj.IntDocumentManagementId,
                    IntAccountId = obj.IntAccountId,
                    IntEmployeeId = obj.IntEmployeeId,
                    StrEmployeeName = obj.StrEmployeeName,
                    IntDocumentTypeId = obj.IntDocumentTypeId,
                    StrDocumentType = obj.StrDocumentType,
                    IntFileUrlId = obj.IntFileUrlId,
                    IsActive = true,
                };

                if (obj.IntDocumentManagementId > 0)
                {
                    empDocument.DteUpdatedAt = DateTime.Now;
                    empDocument.IntUpdatedBy = obj.IntUpdatedBy;

                    _context.Entry(empDocument).State = EntityState.Modified;
                    _context.Entry(empDocument).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(empDocument).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();

                    msg.StatusCode = 200;
                    msg.Message = "Update Successfully";
                }
                else
                {
                    empDocument.IntCreatedBy = obj.IntCreatedBy;
                    empDocument.DteCreatedAt = DateTime.Now;

                    await _context.EmpEmployeeDocumentManagements.AddAsync(empDocument);
                    await _context.SaveChangesAsync();

                    msg.StatusCode = 200;
                    msg.Message = "Create Successfully";
                }

                return msg;
            }
            catch (Exception)
            {
                msg.StatusCode = 500;
                msg.Message = "";
                return msg;
            }
        }

        public async Task<List<EmpEmployeeDocumentManagement>> GetAllEmployeeDocumentManagement(long IntAccountId, long IntEmployeeId)
        {
            List<EmpEmployeeDocumentManagement> empDocument = await _context.EmpEmployeeDocumentManagements.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId && x.IntEmployeeId == IntEmployeeId).ToListAsync();

            return empDocument;
        }

        public async Task<EmpEmployeeDocumentManagement> GetEmployeeDocumentById(long id)
        {
            EmpEmployeeDocumentManagement empEmployeeDocument = await _context.EmpEmployeeDocumentManagements.FirstOrDefaultAsync(x => x.IntDocumentManagementId == id);

            return empEmployeeDocument;
        }

        public async Task<bool> DeleteEmpDocumentManagement(long id)
        {
            try
            {
                EmpEmployeeDocumentManagement documentManagement = await _context.EmpEmployeeDocumentManagements.FirstOrDefaultAsync(x => x.IntDocumentManagementId == id);
                documentManagement.IsActive = false;
                _context.EmpEmployeeDocumentManagements.Update(documentManagement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ============== Employee Document Management ===============

        #region ================ Elezible for job config =================

        //public async Task<MessageHelper> SaveElezibleJobConfig(List<ElezableForJobConfigViewModel> objList)
        //{
        //    MessageHelper msg = new MessageHelper();
        //    try
        //    {
        //        foreach (var obj in objList)
        //        {
        //            ElezableForJobConfig JobConfig = new ElezableForJobConfig
        //            {
        //                IntJobConfigId = obj.IntJobConfigId,
        //                IntAccountId = obj.IntAccountId,
        //                IntFromEmploymentTypeId = obj.IntFromEmploymentTypeId,
        //                IntToEmploymentTypeId = obj.IntToEmploymentTypeId,
        //                IntServiceLengthDays = obj.IntServiceLengthDays,
        //                IsActive = true,
        //                IntCreatedBy = obj.IntCreatedBy,
        //                DteCreatedAt = DateTime.Now
        //            };

        //            if (obj.IsDelete == false)
        //            {
        //                if (obj.IntJobConfigId > 0)
        //                {
        //                    JobConfig.DteUpdatedAt = DateTime.Now;
        //                    JobConfig.IntUpdatedBy = obj.IntCreatedBy;

        //                    _context.Entry(JobConfig).State = EntityState.Modified;
        //                    _context.Entry(JobConfig).Property(x => x.IntCreatedBy).IsModified = false;
        //                    _context.Entry(JobConfig).Property(x => x.DteCreatedAt).IsModified = false;
        //                    await _context.SaveChangesAsync();

        //                    msg.StatusCode = 200;
        //                    msg.Message = "Update Successfully";
        //                }
        //                else
        //                {
        //                    JobConfig.IntCreatedBy = obj.IntCreatedBy;
        //                    JobConfig.DteCreatedAt = DateTime.Now;

        //                    await _context.ElezableForJobConfigs.AddAsync(JobConfig);
        //                    await _context.SaveChangesAsync();

        //                    msg.StatusCode = 200;
        //                    msg.Message = "Create Successfully";
        //                }
        //            }
        //            else
        //            {
        //                if (obj.IntJobConfigId > 0)
        //                {
        //                    JobConfig.IsActive = false;
        //                    JobConfig.DteUpdatedAt = DateTime.Now;
        //                    JobConfig.IntUpdatedBy = obj.IntCreatedBy;

        //                    _context.Entry(JobConfig).State = EntityState.Modified;
        //                    _context.Entry(JobConfig).Property(x => x.IntCreatedBy).IsModified = false;
        //                    _context.Entry(JobConfig).Property(x => x.DteCreatedAt).IsModified = false;
        //                    await _context.SaveChangesAsync();

        //                    msg.StatusCode = 200;
        //                    msg.Message = "Delete Successfully";
        //                }
        //                else
        //                {
        //                    msg.StatusCode = 500;
        //                    msg.Message = "Delete Unsuccessfully";
        //                }
        //            }
        //        }
        //        return msg;
        //    }
        //    catch (Exception e)
        //    {
        //        msg.StatusCode = 500;
        //        msg.Message = e.Message;
        //        return msg;
        //    }

        //}
        //public async Task<List<ElezableForJobConfigViewModel>> GetAllJobConfig(long IntAccountId)
        //{
        //    List<ElezableForJobConfigViewModel> jobConfigs = await _context.ElezableForJobConfigs.Where(x => x.IntAccountId == IntAccountId)
        //        .OrderByDescending(x => x.IntJobConfigId)
        //        .Select(x => new ElezableForJobConfigViewModel
        //        {
        //            IntJobConfigId = x.IntJobConfigId,
        //            IntAccountId = x.IntAccountId,
        //            IntFromEmploymentTypeId = x.IntFromEmploymentTypeId,
        //            FromEmploymentType = _context.MasterEmploymentTypes.Where(s => s.IntEmploymentTypeId == x.IntFromEmploymentTypeId).Select(s => s.StrEmploymentType).FirstOrDefault(),
        //            IntToEmploymentTypeId = x.IntToEmploymentTypeId,
        //            ToEmploymentType = _context.MasterEmploymentTypes.Where(s => s.IntEmploymentTypeId == x.IntToEmploymentTypeId).Select(s => s.StrEmploymentType).FirstOrDefault(),
        //            IntServiceLengthDays = x.IntServiceLengthDays,
        //            IsActive = x.IsActive,
        //            IntCreatedBy = x.IntCreatedBy,
        //            DteCreatedAt = x.DteCreatedAt
        //        }).ToListAsync();

        //    return jobConfigs;
        //}
        //public async Task<ElezableForJobConfigViewModel> GetJobConfigById(long id)
        //{
        //    ElezableForJobConfigViewModel jobConfigs = await _context.ElezableForJobConfigs.Where(x => x.IntJobConfigId == id)
        //        .Select(x => new ElezableForJobConfigViewModel
        //        {
        //            IntJobConfigId = x.IntJobConfigId,
        //            IntAccountId = x.IntAccountId,
        //            IntFromEmploymentTypeId = x.IntFromEmploymentTypeId,
        //            FromEmploymentType = _context.MasterEmploymentTypes.Where(s => s.IntEmploymentTypeId == x.IntFromEmploymentTypeId).Select(s => s.StrEmploymentType).FirstOrDefault(),
        //            IntToEmploymentTypeId = x.IntToEmploymentTypeId,
        //            ToEmploymentType = _context.MasterEmploymentTypes.Where(s => s.IntEmploymentTypeId == x.IntToEmploymentTypeId).Select(s => s.StrEmploymentType).FirstOrDefault(),
        //            IntServiceLengthDays = x.IntServiceLengthDays,
        //            IsActive = x.IsActive,
        //            IntCreatedBy = x.IntCreatedBy,
        //            DteCreatedAt = x.DteCreatedAt
        //        }).FirstOrDefaultAsync();

        //    return jobConfigs;
        //}

        #endregion ================ Elezible for job config =================

        #region ========== Time Sheet ==============

        public async Task<DataTable> TimeSheetAllLanding(string? PartType, long? BuninessUnitId, long? intId, long? intYear, long? intMonth)
        {
            try
            {
                //ViewType = 1 for pending 2 for approved 3 for rejected
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprTimeSheetAllLanding";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPart", PartType);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BuninessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intId", intId);
                        sqlCmd.Parameters.AddWithValue("@intYear", intYear);
                        sqlCmd.Parameters.AddWithValue("@intMonth", intMonth);
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", "");
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

        public async Task<DataTable> AttendanceAdjustmentFilter(AttendanceAdjustmentFilterViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprTimeSheetAllLanding";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        string jsonString = JsonSerializer.Serialize(obj);

                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPart", "TodayAllEmployeeAttendanceSummaryByBusinessUnitId");
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", 0);
                        sqlCmd.Parameters.AddWithValue("@intId", 0);
                        sqlCmd.Parameters.AddWithValue("@intYear", 0);
                        sqlCmd.Parameters.AddWithValue("@intMonth", 0);
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

        public async Task<MessageHelper> CreateExtraSideDuty(ExtraSideDutyViewModel obj)
        {
            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.sprExtraSideDuty";
                using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                    sqlCmd.Parameters.AddWithValue("@intExtraSideDutyId", obj.ExtraSideDutyId);
                    sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                    sqlCmd.Parameters.AddWithValue("@strEmployeeName", obj.EmployeeName);
                    sqlCmd.Parameters.AddWithValue("@intDutyCount", obj.DutyCount);
                    sqlCmd.Parameters.AddWithValue("@dteDutyDate", obj.DutyDate);
                    sqlCmd.Parameters.AddWithValue("@strRemarks", obj.Remarks);
                    sqlCmd.Parameters.AddWithValue("@strinsertBy", obj.InsertBy);
                    sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                    sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);

                    connection.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(dt);
                    }
                    connection.Close();

                    var msg = new MessageHelper();
                    msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                    msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                    return msg;
                }
            }
        }

        public async Task<IEnumerable<dynamic>> OverTimeFilter(OverTimeFilterViewModel obj, dynamic tokenData)
        {
            try
            {
                //using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                //{
                //    string sql = "saas.sprTimeSheetAllLanding";
                //    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                //    {
                //        string jsonString = JsonSerializer.Serialize(obj);

                //        sqlCmd.CommandType = CommandType.StoredProcedure;
                //        sqlCmd.Parameters.AddWithValue("@strPart", obj.StrPartName);
                //        sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
                //        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                //        sqlCmd.Parameters.AddWithValue("@intId", obj.IntOverTimeId);
                //        sqlCmd.Parameters.AddWithValue("@intYear", 0);
                //        sqlCmd.Parameters.AddWithValue("@intMonth", 0);
                //        sqlCmd.Parameters.AddWithValue("@FormDate", obj.FormDate);
                //        sqlCmd.Parameters.AddWithValue("@ToDate", obj.ToDate);
                //        connection.Open();

                //        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                //        {
                //            sqlAdapter.Fill(dt);
                //        }
                //        connection.Close();
                //    }
                //}
                //return dt;



                using var connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprTimeSheetAllLanding";
                string jsonString = JsonSerializer.Serialize(obj);
                string jsonTKData = JsonSerializer.Serialize(tokenData.Result);
                var values = new
                {
                    strPart = obj.StrPartName,
                    jsonFilter = jsonString,
                    jsonTokenData = jsonTKData,
                    intBusinessUnitId = obj.BusinessUnitId,
                    intWorkplaceGroupId = obj.WorkplaceGroupId,
                    intId = obj.IntOverTimeId,
                    intYear = 0,
                    intMonth = 0,
                    FormDate = obj.FormDate,
                    ToDate = obj.ToDate
                };
                var res = await connection.QueryAsync(sql, values, commandType: CommandType.StoredProcedure);

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion ========== Time Sheet ==============

        #region ====================== Employee Information ==================

        public async Task<MessageHelper> UpdateEmployeeProfile(EmployeeProfileUpdateViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeProfileUpdate";
                    SqlCommand sqlCmd = new SqlCommand(sql, connection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@strPart", obj.PartType);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                    sqlCmd.Parameters.AddWithValue("@intAutoId", obj.AutoId);
                    sqlCmd.Parameters.AddWithValue("@strValue", obj.Value);
                    sqlCmd.Parameters.AddWithValue("@intInsertByEmpId", obj.InsertByEmpId);
                    sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);

                    sqlCmd.Parameters.AddWithValue("@intBankId", obj.BankId);
                    sqlCmd.Parameters.AddWithValue("@strBankName", obj.BankName);
                    sqlCmd.Parameters.AddWithValue("@strBranchName", obj.BranchName);
                    sqlCmd.Parameters.AddWithValue("@strRoutingNo", obj.RoutingNo);
                    sqlCmd.Parameters.AddWithValue("@strSwiftCode", obj.SwiftCode);
                    sqlCmd.Parameters.AddWithValue("@strAccountName", obj.AccountName);
                    sqlCmd.Parameters.AddWithValue("@strAccountNo", obj.AccountNo);

                    sqlCmd.Parameters.AddWithValue("@strPaymentGateway", obj.PaymentGateway);
                    sqlCmd.Parameters.AddWithValue("@strDigitalBankingName", obj.DigitalBankingName);
                    sqlCmd.Parameters.AddWithValue("@strDigitalBankingNo", obj.DigitalBankingNo);

                    sqlCmd.Parameters.AddWithValue("@intAddressTypeId", obj.AddressTypeId);
                    sqlCmd.Parameters.AddWithValue("@intCountryId", obj.CountryId);
                    sqlCmd.Parameters.AddWithValue("@strCountryName", obj.CountryName);
                    sqlCmd.Parameters.AddWithValue("@intDivisionId", obj.DivisionId);
                    sqlCmd.Parameters.AddWithValue("@strDivisionName", obj.DivisionName);
                    sqlCmd.Parameters.AddWithValue("@intDistrictId", obj.DistrictId);
                    sqlCmd.Parameters.AddWithValue("@strDistrictName", obj.DistrictName);
                    sqlCmd.Parameters.AddWithValue("@intThanaId", obj.ThanaId);
                    sqlCmd.Parameters.AddWithValue("@strThana", obj.ThanaName);
                    sqlCmd.Parameters.AddWithValue("@intPostOfficeId", obj.PostOfficeId);
                    sqlCmd.Parameters.AddWithValue("@strPostOfficeName", obj.PostOfficeName);
                    sqlCmd.Parameters.AddWithValue("@strPostCode", obj.PostCode);
                    sqlCmd.Parameters.AddWithValue("@strAddressDetails", obj.AddressDetails);

                    sqlCmd.Parameters.AddWithValue("@strCompanyName", obj.CompanyName);
                    sqlCmd.Parameters.AddWithValue("@strJobTitle", obj.JobTitle);
                    sqlCmd.Parameters.AddWithValue("@strLocation", obj.Location);
                    sqlCmd.Parameters.AddWithValue("@dteFromDate", obj.FromDate);
                    sqlCmd.Parameters.AddWithValue("@dteToDate", obj.ToDate);

                    sqlCmd.Parameters.AddWithValue("@strDescription", obj.Description);

                    sqlCmd.Parameters.AddWithValue("@isForeign", obj.isForeign);
                    sqlCmd.Parameters.AddWithValue("@strInstituteName", obj.InstituteName);
                    sqlCmd.Parameters.AddWithValue("@strDegree", obj.Degree);
                    sqlCmd.Parameters.AddWithValue("@intDegreeId", obj.DegreeId);
                    sqlCmd.Parameters.AddWithValue("@strFieldOfStudy", obj.FieldOfStudy);
                    sqlCmd.Parameters.AddWithValue("@strCGPA", obj.CGPA);
                    sqlCmd.Parameters.AddWithValue("@strOutOf", obj.OutOf);
                    sqlCmd.Parameters.AddWithValue("@dteStartDate", obj.StartDate);
                    sqlCmd.Parameters.AddWithValue("@dteEndDate", obj.EndDate);
                    sqlCmd.Parameters.AddWithValue("@intFileUrlId", obj.FileUrlId);

                    sqlCmd.Parameters.AddWithValue("@strName", obj.Name);
                    sqlCmd.Parameters.AddWithValue("@intRelationId", obj.RelationId);
                    sqlCmd.Parameters.AddWithValue("@strRelationName", obj.RelationName);
                    sqlCmd.Parameters.AddWithValue("@strPhone", obj.Phone);
                    sqlCmd.Parameters.AddWithValue("@strEmail", obj.Email);
                    sqlCmd.Parameters.AddWithValue("@strNID", obj.NID);
                    sqlCmd.Parameters.AddWithValue("@strBirthId", obj.BirthId);
                    sqlCmd.Parameters.AddWithValue("@dteDateOfBirth", obj.DateOfBirth);
                    sqlCmd.Parameters.AddWithValue("@strRemarks", obj.Remarks);
                    sqlCmd.Parameters.AddWithValue("@isEmergencyContact", obj.IsEmergencyContact);
                    sqlCmd.Parameters.AddWithValue("@intSpecialContactTypeId", obj.SpecialContactTypeId);
                    sqlCmd.Parameters.AddWithValue("@strSpecialContactTypeName", obj.SpecialContactTypeName);

                    sqlCmd.Parameters.AddWithValue("@strTrainingName", obj.TrainingName);
                    sqlCmd.Parameters.AddWithValue("@strOrganizationName", obj.OrganizationName);
                    sqlCmd.Parameters.AddWithValue("@dteExpirationDate", obj.ExpirationDate);

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@msg";
                    outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outputParameter.Size = int.MaxValue;
                    outputParameter.Direction = System.Data.ParameterDirection.Output;
                    sqlCmd.Parameters.Add(outputParameter);
                    connection.Open();
                    sqlCmd.ExecuteNonQuery();
                    string output = outputParameter.Value.ToString();
                    connection.Close();

                    var msg = new MessageHelper();
                    msg.StatusCode = 200;
                    msg.Message = output;
                    return msg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CRUDEmployeeSeparation(BaseVM tokenData, EmployeeSeparationViewModel obj)
        {
            try
            {

                PipelineStageInfoVM res = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(obj.IntEmployeeId, "separation");


                string strDocumentId = String.Join(",", obj.StrDocumentId);

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeSeparation";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        //string jsonString = JsonSerializer.Serialize(obj.StrDocumentId);
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@intSeparationId", obj.IntSeparationId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.IntEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@strEmployeeName", obj.StrEmployeeName);
                        sqlCmd.Parameters.AddWithValue("@strEmployeeCode", obj.StrEmployeeCode);
                        sqlCmd.Parameters.AddWithValue("@intSeparationTypeId", obj.IntSeparationTypeId);
                        sqlCmd.Parameters.AddWithValue("@strSeparationTypeName", obj.StrSeparationTypeName);

                        sqlCmd.Parameters.AddWithValue("@dteSeparationDate", obj.DteSeparationDate);
                        sqlCmd.Parameters.AddWithValue("@dteLastWorkingDate", obj.DteLastWorkingDate);
                        sqlCmd.Parameters.AddWithValue("@strReason", obj.StrReason);
                        sqlCmd.Parameters.AddWithValue("@strDocumentId", strDocumentId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);

                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedId", tokenData.employeeId);
                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", res.HeaderId);
                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", res.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", res.NextStageId);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ====================== Employee Information ==================

        #region ====================== Employee Query Data ==================

        public async Task<EmployeeProfileLandingPaginationViewModel> EmployeeProfileLandingPagination(long accountId, long businessUnitId, long WorkplaceGroupId, string? searchTxt, long? EmployeeId, int PageNo, int PageSize, bool IsForXl)
        {
            IQueryable<EmployeeProfileLandingView> data = (from emp in _context.EmpEmployeeBasicInfos
                                                           where emp.IntAccountId == accountId && (businessUnitId == 0 || emp.IntBusinessUnitId == businessUnitId) && emp.IsActive == true
                                                           && (WorkplaceGroupId == 0 || emp.IntWorkplaceGroupId == WorkplaceGroupId)
                                                           join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                           from desig in desig2.DefaultIfEmpty()
                                                           join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                           from info in info2.DefaultIfEmpty()

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

                                                           join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                           from dept in dpt2.DefaultIfEmpty()
                                                           join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                           from sup in sup2.DefaultIfEmpty()
                                                           join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                                           from dsup in dsup2.DefaultIfEmpty()
                                                           join man1 in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals man1.IntEmployeeBasicInfoId into man2
                                                           from man in man2.DefaultIfEmpty()
                                                           join wrk1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wrk1.IntWorkplaceId into wrk2
                                                           from wrk in wrk2.DefaultIfEmpty()
                                                           join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                           from bus in bus2.DefaultIfEmpty()
                                                           join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                           from acc in acc2.DefaultIfEmpty()
                                                           join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                           from photo in photo2.DefaultIfEmpty()
                                                           where (info.IntEmployeeStatusId == 1 || info.IntEmployeeStatusId == 4)
                                                           orderby emp.IntEmployeeBasicInfoId descending
                                                           select new EmployeeProfileLandingView
                                                           {
                                                               IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                               StrEmployeeName = emp.StrEmployeeName,
                                                               StrEmployeeCode = emp.StrEmployeeCode,
                                                               StrPersonalMobile = info == null ? "" : info.StrPersonalMobile,
                                                               StrPersonalMail = info == null ? "" : info.StrPersonalMail,
                                                               StrOfficeMobile = info == null ? "" : info.StrOfficeMobile,
                                                               StrOfficeMail = info == null ? "" : info.StrOfficeMail,
                                                               PinNo = info.StrPinNo,
                                                               StrReferenceId = emp.StrReferenceId,
                                                               IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                               IntDesignationId = emp.IntDesignationId,
                                                               StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                               IntDepartmentId = emp.IntDepartmentId,
                                                               StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                               IntSupervisorId = emp.IntSupervisorId,
                                                               StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                                               IntLineManagerId = emp.IntLineManagerId,
                                                               StrLinemanager = man == null ? "" : man.StrEmployeeName,
                                                               StrCardNumber = emp.StrCardNumber,
                                                               IntGenderId = emp.IntGenderId,
                                                               StrGender = emp.StrGender,
                                                               IntReligionId = emp.IntReligionId,
                                                               StrReligion = emp.StrReligion,
                                                               StrMaritalStatus = emp.StrMaritalStatus,
                                                               StrBloodGroup = emp.StrBloodGroup,
                                                               DteDateOfBirth = emp.DteDateOfBirth,
                                                               DteJoiningDate = emp.DteJoiningDate,
                                                               DteConfirmationDate = emp.DteConfirmationDate,
                                                               DteLastWorkingDate = emp.DteLastWorkingDate,
                                                               IntDottedSupervisorId = emp.IntDottedSupervisorId,
                                                               StrDottedSupervisorName = dsup == null ? "" : dsup.StrEmployeeName,
                                                               IsSalaryHold = emp.IsSalaryHold,
                                                               IsActive = emp.IsActive,
                                                               IsUserInactive = emp.IsUserInactive,
                                                               IsRemoteAttendance = emp.IsRemoteAttendance,
                                                               IntWorkplaceId = emp.IntWorkplaceId,
                                                               StrWorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                                               IntBusinessUnitId = emp.IntBusinessUnitId,
                                                               StrBusinessUnitName = bus == null ? "" : bus.StrBusinessUnit,
                                                               IntAccountId = emp.IntAccountId,
                                                               StrAccountName = acc == null ? "" : acc.StrAccountName,

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

                                                               DteCreatedAt = emp.DteCreatedAt,
                                                               IntCreatedBy = emp.IntCreatedBy,
                                                               DteUpdatedAt = emp.DteUpdatedAt,
                                                               IntUpdatedBy = emp.IntUpdatedBy,
                                                               IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                               StrEmploymentType = emp.StrEmploymentType,
                                                               IntEmployeeStatusId = info.IntEmployeeStatusId,
                                                               StrEmployeeStatus = info.StrEmployeeStatus,
                                                           }).AsNoTracking().AsQueryable();

            EmployeeProfileLandingPaginationViewModel retObj = new EmployeeProfileLandingPaginationViewModel();

            if (IsForXl)
            {
                retObj.Data = await data.ToListAsync();
            }
            else
            {
                int maxSize = 1000;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                    data = data.Where(x => x.StrEmployeeName.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrEmployeeCode.ToLower().Contains(searchTxt)
                   || x.StrReferenceId.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrDepartment.ToLower().Contains(searchTxt) || x.WingName.ToLower().Contains(searchTxt)
                   || x.SoleDepoName.ToLower().Contains(searchTxt) || x.RegionName.ToLower().Contains(searchTxt) || x.AreaName.ToLower().Contains(searchTxt) || x.TerritoryName.ToLower().Contains(searchTxt));
                }


                retObj.TotalCount = await data.CountAsync();
                retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;
            }
            return retObj;
        }
        public async Task<dynamic> EmployeeProfileLandingPaginationWithMasterFilter(BaseVM tokenData, long businessUnitId, long WorkplaceGroupId, string? searchTxt, int PageNo, int PageSize, bool IsPaginated, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors, List<long> employeementType, List<long> LineManagers, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            IQueryable<EmployeeProfileLandingVM> data = (from emp in _context.EmpEmployeeBasicInfos
                                                         join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                         from desig in desig2.DefaultIfEmpty()
                                                         join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                         from info in info2.DefaultIfEmpty()
                                                         join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                         from dept in dpt2.DefaultIfEmpty()
                                                         join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                         from sup in sup2.DefaultIfEmpty()
                                                         join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                                         from dsup in dsup2.DefaultIfEmpty()
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
                                                         join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                         from bus in bus2.DefaultIfEmpty()
                                                         join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                         from photo in photo2.DefaultIfEmpty()

                                                         where emp.IntAccountId == tokenData.accountId && emp.IsActive == true && (info.IntEmployeeStatusId == 1 || info.IntEmployeeStatusId == 4)

                                                         //&& (workplaceGroupId == 0 || emp.IntWorkplaceGroupId == workplaceGroupId)

                                                         && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                                                         && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(WorkplaceGroupId)) && emp.IntWorkplaceGroupId == WorkplaceGroupId

                                                         && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                         : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                         : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId)
                                                         : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo)
                                                         : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId)
                                                         : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId) && tokenData.areaList.Contains(info.IntAreaId)
                                                         : tokenData.territoryList.Contains(info.IntTerritoryId))
                                                         || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                                                         && (!string.IsNullOrEmpty(searchTxt) ? (emp.StrEmployeeName.ToLower().Contains(searchTxt) || emp.StrEmployeeCode.ToLower().Contains(searchTxt)
                                                         || emp.StrReferenceId.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                                                         || info.StrPinNo.ToLower().Contains(searchTxt) || info.StrPersonalMobile.ToLower().Contains(searchTxt)) : true)

                                                         && ((departments.Count > 0 ? departments.Contains((long)emp.IntDepartmentId) : true)
                                                         && (designations.Count > 0 ? designations.Contains((long)emp.IntDesignationId) : true)
                                                         && (supervisors.Count > 0 ? supervisors.Contains((long)emp.IntSupervisorId) : true)
                                                         && (LineManagers.Count > 0 ? LineManagers.Contains((long)emp.IntLineManagerId) : true)
                                                         && (employeementType.Count > 0 ? employeementType.Contains((long)emp.IntEmploymentTypeId) : true)

                                                         && (WingList.Count > 0 ? WingList.Contains((long)info.IntWingId) : true)
                                                         && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)info.IntSoleDepo) : true)
                                                         && (RegionList.Count > 0 ? RegionList.Contains((long)info.IntRegionId) : true)
                                                         && (AreaList.Count > 0 ? AreaList.Contains((long)info.IntAreaId) : true)
                                                         && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)info.IntTerritoryId) : true))


                                                         orderby emp.IntEmployeeBasicInfoId descending
                                                         select new EmployeeProfileLandingVM
                                                         {
                                                             IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                             StrEmployeeName = emp.StrEmployeeName,
                                                             StrEmployeeCode = emp.StrEmployeeCode,
                                                             StrReferenceId = emp.StrReferenceId,
                                                             IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                             IntDesignationId = emp.IntDesignationId,
                                                             StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                             IntDepartmentId = emp.IntDepartmentId,
                                                             StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                             IntSupervisorId = emp.IntSupervisorId,
                                                             StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                                             IntLineManagerId = emp.IntLineManagerId,
                                                             StrLinemanager = man == null ? "" : man.StrEmployeeName,
                                                             StrCardNumber = emp.StrCardNumber,
                                                             DteDateOfBirth = emp.DteDateOfBirth,
                                                             DteJoiningDate = emp.DteJoiningDate,
                                                             IntDottedSupervisorId = emp.IntDottedSupervisorId,
                                                             StrDottedSupervisorName = dsup == null ? "" : dsup.StrEmployeeName,
                                                             IntWorkplaceId = emp.IntWorkplaceId,
                                                             StrWorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                                             IntWorkplaceGroupId = emp.IntWorkplaceGroupId,
                                                             StrWorkplaceGroupName = wg == null ? "" : wg.StrWorkplaceGroup,
                                                             IntBusinessUnitId = emp.IntBusinessUnitId,
                                                             StrBusinessUnitName = bus == null ? "" : bus.StrBusinessUnit,
                                                             IntAccountId = emp.IntAccountId,
                                                             IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                             StrEmploymentType = emp.StrEmploymentType,
                                                             PinNo = info.StrPinNo,
                                                             ContactNo = info.StrPersonalMobile,
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
                                                             IntEmployeeStatusId = info.IntEmployeeStatusId,
                                                             StrEmployeeStatus = info.StrEmployeeStatus,
                                                         }).OrderByDescending(x => x.IntEmployeeBasicInfoId).AsNoTracking().AsQueryable();

            EmployeeProfileLandingPaginationViewModelWithHeader retObj = new();

            if (IsHeaderNeed)
            {
                EmployeeHeader eh = new();

                //eh.WingNameList = data.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).Distinct().ToList();
                //eh.SoleDepoNameList = data.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).Distinct().ToList();
                //eh.RegionNameList = data.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).Distinct().ToList();
                //eh.AreaNameList = data.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).Distinct().ToList();
                //eh.TerritoryNameList = data.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).Distinct().ToList();
                //eh.StrDepartmentList = data.Where(x => !string.IsNullOrEmpty(x.StrDepartment)).Select(x => new CommonDDLVM { Value = (long)x.IntDepartmentId, Label = (string)x.StrDepartment }).Distinct().ToList();
                //eh.StrDesignationList = data.Where(x => !string.IsNullOrEmpty(x.StrDesignation)).Select(x => new CommonDDLVM { Value = (long)x.IntDesignationId, Label = (string)x.StrDesignation }).Distinct().ToList();
                //eh.StrSupervisorNameList = data.Where(x => !string.IsNullOrEmpty(x.StrSupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.IntSupervisorId, Label = (string)x.StrSupervisorName }).Distinct().ToList();
                //eh.StrLinemanagerList = data.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).Distinct().ToList();
                //eh.StrEmploymentTypeList = data.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).Distinct().ToList();

                var datas = data.Select(x => new
                {
                    WingId = x.WingId,
                    WingName = x.WingName,
                    SoleDepoId = x.SoleDepoId,
                    SoleDepoName = x.SoleDepoName,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    AreaId = x.AreaId,
                    AreaName = x.AreaName,
                    TerritoryId = x.TerritoryId,
                    TerritoryName = x.TerritoryName,
                    IntDepartmentId = x.IntDepartmentId,
                    StrDepartment = x.StrDepartment,
                    IntDesignationId = x.IntDesignationId,
                    StrDesignation = x.StrDesignation,
                    IntSupervisorId = x.IntSupervisorId,
                    StrSupervisorName = x.StrSupervisorName,
                    IntLineManagerId = x.IntLineManagerId,
                    StrLinemanager = x.StrLinemanager,
                    IntEmploymentTypeId = x.IntEmploymentTypeId,
                    StrEmploymentType = x.StrEmploymentType
                }).Distinct().ToList();

                eh.WingNameList = datas.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).DistinctBy(x => x.Value).ToList();
                eh.SoleDepoNameList = datas.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).DistinctBy(x => x.Value).ToList();
                eh.RegionNameList = datas.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).DistinctBy(x => x.Value).ToList();
                eh.AreaNameList = datas.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).DistinctBy(x => x.Value).ToList();
                eh.TerritoryNameList = datas.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).DistinctBy(x => x.Value).ToList();
                eh.StrDepartmentList = datas.Where(x => !string.IsNullOrEmpty(x.StrDepartment)).Select(x => new CommonDDLVM { Value = (long)x.IntDepartmentId, Label = (string)x.StrDepartment }).DistinctBy(x => x.Value).ToList();
                eh.StrDesignationList = datas.Where(x => !string.IsNullOrEmpty(x.StrDesignation)).Select(x => new CommonDDLVM { Value = (long)x.IntDesignationId, Label = (string)x.StrDesignation }).DistinctBy(x => x.Value).ToList();
                eh.StrSupervisorNameList = datas.Where(x => !string.IsNullOrEmpty(x.StrSupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.IntSupervisorId, Label = (string)x.StrSupervisorName }).DistinctBy(x => x.Value).ToList();
                eh.StrLinemanagerList = datas.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).DistinctBy(x => x.Value).ToList();
                eh.StrEmploymentTypeList = datas.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).DistinctBy(x => x.Value).ToList();

                retObj.EmployeeHeader = eh;
            }


            if (IsPaginated == false)
            {
                retObj.Data = await data.ToListAsync();
            }
            else
            {
                int maxSize = 1000;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                retObj.TotalCount = await data.CountAsync();
                retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;
            }

            return retObj;
        }

        public async Task<EmployeeProfileLandingView> EmployeeProfileViewData(long employeeId)
        {
            EmployeeProfileLandingView data = await (from emp in _context.EmpEmployeeBasicInfos
                                                     where emp.IntEmployeeBasicInfoId == employeeId
                                                     join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                     from desig in desig2.DefaultIfEmpty()
                                                     join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                     from info in info2.DefaultIfEmpty()

                                                     join wing1 in _context.TerritorySetups on info.IntWingId equals wing1.IntTerritoryId into wing2
                                                     from wing in wing2.DefaultIfEmpty()
                                                     join soleDepo1 in _context.TerritorySetups on info.IntSoleDepo equals soleDepo1.IntTerritoryId into soleDepo2
                                                     from soleDepo in soleDepo2.DefaultIfEmpty()
                                                     join region1 in _context.TerritorySetups on info.IntRegionId equals region1.IntTerritoryId into region2
                                                     from region in region2.DefaultIfEmpty()
                                                     join area1 in _context.TerritorySetups on info.IntAreaId equals area1.IntTerritoryId into area2
                                                     from area in area2.DefaultIfEmpty()
                                                     join territory1 in _context.TerritorySetups on info.IntTerritoryId equals territory1.IntTerritoryId into territory2
                                                     from territory in territory2.DefaultIfEmpty()

                                                     join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                     from dept in dpt2.DefaultIfEmpty()
                                                     join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                     from sup in sup2.DefaultIfEmpty()
                                                     join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                                     from dsup in dsup2.DefaultIfEmpty()
                                                     join man1 in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals man1.IntEmployeeBasicInfoId into man2
                                                     from man in man2.DefaultIfEmpty()
                                                     join wrk1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wrk1.IntWorkplaceId into wrk2
                                                     from wrk in wrk2.DefaultIfEmpty()
                                                     join wg1 in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                                     from wg in wg2.DefaultIfEmpty()
                                                     join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                     from bus in bus2.DefaultIfEmpty()
                                                     join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                     from acc in acc2.DefaultIfEmpty()
                                                     join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                     from photo in photo2.DefaultIfEmpty()
                                                     join supPhoto1 in _context.EmpEmployeePhotoIdentities on emp.IntSupervisorId equals supPhoto1.IntEmployeeBasicInfoId into supPhoto2
                                                     from supPhoto in supPhoto2.DefaultIfEmpty()
                                                     join manPhoto1 in _context.EmpEmployeePhotoIdentities on emp.IntLineManagerId equals manPhoto1.IntEmployeeBasicInfoId into manPhoto2
                                                     from manPhoto in manPhoto2.DefaultIfEmpty()
                                                     select new EmployeeProfileLandingView
                                                     {
                                                         IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                         StrEmployeeName = emp.StrEmployeeName,
                                                         StrEmployeeCode = emp.StrEmployeeCode,
                                                         StrReferenceId = emp.StrReferenceId,
                                                         PinNo = info.StrPinNo,
                                                         IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                         IntDesignationId = emp.IntDesignationId,
                                                         StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                         IntDepartmentId = emp.IntDepartmentId,
                                                         StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                         IntSupervisorId = emp.IntSupervisorId,
                                                         StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                                         IntSupervisorImageUrlId = supPhoto == null ? 0 : supPhoto.IntProfilePicFileUrlId,
                                                         IntLineManagerId = emp.IntLineManagerId,
                                                         StrLinemanager = man == null ? "" : man.StrEmployeeName,
                                                         IntLinemanagerImageUrlId = manPhoto == null ? 0 : manPhoto.IntProfilePicFileUrlId,
                                                         StrCardNumber = emp.StrCardNumber,
                                                         IntGenderId = emp.IntGenderId,
                                                         StrGender = emp.StrGender,
                                                         IntReligionId = emp.IntReligionId,
                                                         StrReligion = emp.StrReligion,
                                                         StrMaritalStatus = emp.StrMaritalStatus,
                                                         StrBloodGroup = emp.StrBloodGroup,
                                                         DteDateOfBirth = emp.DteDateOfBirth,
                                                         DteJoiningDate = emp.DteJoiningDate,
                                                         DteInternCloseDate = emp.DteInternCloseDate,
                                                         DteProbationaryCloseDate = emp.DteProbationaryCloseDate,
                                                         DteConfirmationDate = emp.DteConfirmationDate,
                                                         DteContractFromDate = emp.DteContactFromDate,
                                                         DteContractToDate = emp.DteContactToDate,
                                                         DteLastWorkingDate = emp.DteLastWorkingDate,
                                                         IntDottedSupervisorId = emp.IntDottedSupervisorId,
                                                         StrDottedSupervisorName = dsup == null ? "" : dsup.StrEmployeeName,
                                                         StrServiceLength = emp.DteJoiningDate != null && emp.DteLastWorkingDate != null ? YearMonthDayCalculate.YearMonthDayShortFormCal(emp.DteJoiningDate.Value.Date, emp.DteLastWorkingDate.Value.Date)
                                                               : emp.DteJoiningDate != null ? YearMonthDayCalculate.YearMonthDayShortFormCal(emp.DteJoiningDate.Value.Date, DateTime.Now.Date) : " 0 days",
                                                         IsSalaryHold = emp.IsSalaryHold,
                                                         IsActive = emp.IsActive,
                                                         IsUserInactive = emp.IsUserInactive,
                                                         IsCreateUser = _context.Users.Where(x => x.IntRefferenceId == employeeId && x.IsActive == true).Count() > 0 ? true : false,
                                                         IsRemoteAttendance = emp.IsRemoteAttendance,
                                                         IntWorkplaceId = emp.IntWorkplaceId,
                                                         StrWorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                                         IntWorkplaceGroupId = wg != null ? wg.IntWorkplaceGroupId : 0,
                                                         StrWorkplaceGroupName = wg == null ? "" : wg.StrWorkplaceGroup,
                                                         IntBusinessUnitId = emp.IntBusinessUnitId,
                                                         StrBusinessUnitName = bus == null ? "" : bus.StrBusinessUnit,
                                                         IntAccountId = emp.IntAccountId,
                                                         StrAccountName = acc == null ? "" : acc.StrAccountName,
                                                         DteCreatedAt = emp.DteCreatedAt,
                                                         IntCreatedBy = emp.IntCreatedBy,
                                                         DteUpdatedAt = emp.DteUpdatedAt,
                                                         IntUpdatedBy = emp.IntUpdatedBy,
                                                         IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                         StrEmploymentType = emp.StrEmploymentType,
                                                         //employmentType = GetEmploymentParentType(emp.IntAccountId, emp.IntEmploymentTypeId),
                                                         IntParentId = null,
                                                         IsManual = null,
                                                         IntDetailsId = info.IntDetailsId,
                                                         StrOfficeMail = info.StrOfficeMail,
                                                         StrPersonalMail = info.StrPersonalMail,
                                                         StrPersonalMobile = info.StrPersonalMobile,
                                                         StrOfficeMobile = info.StrOfficeMobile,
                                                         IntPayrollGroupId = info.IntPayrollGroupId,
                                                         StrPayrollGroupName = info.StrPayrollGroupName,
                                                         IntPayscaleGradeId = info.IntPayscaleGradeId,
                                                         StrPayscaleGradeName = info.StrPayscaleGradeName,
                                                         IntHrpositionId = info.IntHrpositionId,
                                                         StrHrpostionName = info.StrHrpostionName,
                                                         IntEmployeeStatusId = info.IntEmployeeStatusId,
                                                         StrEmployeeStatus = info.StrEmployeeStatus,
                                                         VehicleNo = info.StrVehicleNo,
                                                         Remarks = info.StrRemarks,
                                                         DrivingLicenseNo = info.StrDrivingLicenseNo,
                                                         IsTakeHomePay = info.IsTakeHomePay,
                                                         WingId = info.IntWingId,
                                                         WingName = wing != null ? wing.StrTerritoryName : "",
                                                         SoleDepoId = info.IntSoleDepo,
                                                         SoleDepoName = soleDepo != null ? soleDepo.StrTerritoryName : "",
                                                         RegionId = info.IntRegionId,
                                                         RegionName = region != null ? region.StrTerritoryName : "",
                                                         AreaId = info.IntAreaId,
                                                         AreaName = area != null ? area.StrTerritoryName : "",
                                                         TerritoryId = info.IntTerritoryId,
                                                         TerritoryName = territory != null ? territory.StrTerritoryName : ""
                                                     }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            EmploymentTypeVM type = await GetEmploymentParentType((long)data?.IntAccountId, (long)data?.IntEmploymentTypeId);

            TimeAttendanceDailySummary calender = await _context.TimeAttendanceDailySummaries.Where(x => x.IntEmployeeId == data.IntEmployeeBasicInfoId && x.DteAttendanceDate.Value.Date == DateTime.Now.Date).FirstOrDefaultAsync();

            data.IntCalenderTypeId = calender == null ? 0 : calender.IntCalendarTypeId;
            data.StrCalenderType = calender == null ? "" : calender.StrCalendarType;
            data.IntCalenderId = calender == null ? 0 : calender.IntCalendarId;
            data.StrCalenderName = calender == null ? "" : calender.StrCalendarName;

            data.IntParentId = type != null ? type.IntParentId : 0;
            data.IsManual = type != null ? type.IsManual : 0;

            return data;
        }

        public async Task<EmployeeProfileLandingPaginationViewModel> EmployeeListForUserLandingPagination(long accountId, long businessUnitId, long workplaceGroupId, string? searchTxt, int? isUser, int PageNo, int PageSize, bool IsForXl)
        {
            IQueryable<EmployeeProfileLandingView> data = (from emp in _context.EmpEmployeeBasicInfos
                                                           where emp.IntBusinessUnitId == businessUnitId && emp.IntAccountId == accountId && emp.IsActive == true && emp.IntWorkplaceGroupId == workplaceGroupId
                                                           join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                           from desig in desig2.DefaultIfEmpty()
                                                           join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                           from info in info2.DefaultIfEmpty()
                                                           join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                           from dept in dpt2.DefaultIfEmpty()
                                                           join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                           from sup in sup2.DefaultIfEmpty()
                                                           join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                                           from dsup in dsup2.DefaultIfEmpty()
                                                           join man1 in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals man1.IntEmployeeBasicInfoId into man2
                                                           from man in man2.DefaultIfEmpty()
                                                           join wrk1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wrk1.IntWorkplaceId into wrk2
                                                           from wrk in wrk2.DefaultIfEmpty()
                                                           join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                           from bus in bus2.DefaultIfEmpty()
                                                           join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                           from acc in acc2.DefaultIfEmpty()
                                                           join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                           from photo in photo2.DefaultIfEmpty()
                                                           join supPhoto1 in _context.EmpEmployeePhotoIdentities on emp.IntSupervisorId equals supPhoto1.IntEmployeeBasicInfoId into supPhoto2
                                                           from supPhoto in supPhoto2.DefaultIfEmpty()
                                                           join manPhoto1 in _context.EmpEmployeePhotoIdentities on emp.IntLineManagerId equals manPhoto1.IntEmployeeBasicInfoId into manPhoto2
                                                           from manPhoto in manPhoto2.DefaultIfEmpty()
                                                           join user1 in _context.Users on emp.IntEmployeeBasicInfoId equals user1.IntRefferenceId into user2
                                                           from user in user2.DefaultIfEmpty()
                                                           join usertype1 in _context.UserTypes on user.IntUserTypeId equals usertype1.IntUserTypeId into usertype2
                                                           from usertype in usertype2.DefaultIfEmpty()
                                                           join details1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals details1.IntEmployeeId into details2
                                                           from details in details2.DefaultIfEmpty()
                                                           where (isUser == 0 ? true : isUser == 1 ? (user != null && user.IsActive == true) : isUser == 2 ? (user == null || user.IsActive == false) : false)
                                                           && (info.IntEmployeeStatusId != 2 && info.IntEmployeeStatusId != 3)
                                                           select new EmployeeProfileLandingView
                                                           {
                                                               IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                               StrEmployeeName = emp.StrEmployeeName,
                                                               StrEmployeeCode = emp.StrEmployeeCode,
                                                               StrReferenceId = emp.StrReferenceId,
                                                               PinNo = info.StrPinNo,
                                                               IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                               IntDesignationId = emp.IntDesignationId,
                                                               StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                               IntDepartmentId = emp.IntDepartmentId,
                                                               StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                               IntSupervisorId = emp.IntSupervisorId,
                                                               StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                                               IntSupervisorImageUrlId = supPhoto == null ? 0 : supPhoto.IntProfilePicFileUrlId,
                                                               IntLineManagerId = emp.IntLineManagerId,
                                                               StrLinemanager = man == null ? "" : man.StrEmployeeName,
                                                               IntLinemanagerImageUrlId = manPhoto == null ? 0 : manPhoto.IntProfilePicFileUrlId,
                                                               StrCardNumber = emp.StrCardNumber,
                                                               IntGenderId = emp.IntGenderId,
                                                               StrGender = emp.StrGender,
                                                               IntReligionId = emp.IntReligionId,
                                                               StrReligion = emp.StrReligion,
                                                               StrMaritalStatus = emp.StrMaritalStatus,
                                                               StrBloodGroup = emp.StrBloodGroup,
                                                               DteDateOfBirth = emp.DteDateOfBirth,
                                                               DteJoiningDate = emp.DteJoiningDate,
                                                               DteConfirmationDate = emp.DteConfirmationDate,
                                                               DteLastWorkingDate = emp.DteLastWorkingDate,
                                                               IntDottedSupervisorId = emp.IntDottedSupervisorId,
                                                               StrDottedSupervisorName = dsup == null ? "" : dsup.StrEmployeeName,
                                                               StrServiceLength = emp.DteJoiningDate != null && emp.DteLastWorkingDate != null ? YearMonthDayCalculate.YearMonthDayShortFormCal(emp.DteJoiningDate.Value.Date, emp.DteLastWorkingDate.Value.Date)
                                                               : emp.DteJoiningDate != null ? YearMonthDayCalculate.YearMonthDayShortFormCal(emp.DteJoiningDate.Value.Date, DateTime.Now.Date) : " 0 days",
                                                               IsSalaryHold = emp.IsSalaryHold,
                                                               IsActive = emp.IsActive,
                                                               IsUserInactive = emp.IsUserInactive,
                                                               IsRemoteAttendance = emp.IsRemoteAttendance,
                                                               IntWorkplaceId = emp.IntWorkplaceId,
                                                               StrWorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                                               IntBusinessUnitId = emp.IntBusinessUnitId,
                                                               StrBusinessUnitName = bus == null ? "" : bus.StrBusinessUnit,
                                                               IntAccountId = emp.IntAccountId,
                                                               StrAccountName = acc == null ? "" : acc.StrAccountName,
                                                               DteCreatedAt = emp.DteCreatedAt,
                                                               IntCreatedBy = emp.IntCreatedBy,
                                                               DteUpdatedAt = emp.DteUpdatedAt,
                                                               IntUpdatedBy = emp.IntUpdatedBy,
                                                               IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                               StrEmploymentType = emp.StrEmploymentType,
                                                               StrLoginId = user.StrLoginId,
                                                               //IntCountryId = address == null ? 0 : address.IntCountryId,
                                                               //StrCountry = address == null ? "" : address.StrCountry,
                                                               IntUserTypeId = user.IntUserTypeId,
                                                               IntUserId = user != null ? user.IntUserId : 0,
                                                               StrUserType = usertype.StrUserType,
                                                               StrPassword = user != null ? (!string.IsNullOrEmpty(user.StrPassword) ? _authService.DeCoding(user.StrPassword) : "") : "",
                                                               StrPersonalEmail = details.StrPersonalMail,
                                                               StrOfficeMail = details.StrOfficeMail,
                                                               StrOfficeMobile = details.StrOfficeMobile,
                                                               StrPersonalMobile = details.StrPersonalMobile,
                                                               UserStatus = user != null ? user.IsActive : false,
                                                               IntEmployeeStatusId = info.IntEmployeeStatusId,
                                                               StrEmployeeStatus = info.StrEmployeeStatus,
                                                           }).AsQueryable().AsNoTracking();

            EmployeeProfileLandingPaginationViewModel retObj = new EmployeeProfileLandingPaginationViewModel();

            if (IsForXl)
            {
                retObj.Data = await data.ToListAsync();
            }
            else
            {
                int maxSize = 100;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                    data = data.Where(x => x.StrEmployeeName.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrEmployeeCode.ToLower().Contains(searchTxt)
                   || x.StrReferenceId.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrDepartment.ToLower().Contains(searchTxt));
                }

                retObj.TotalCount = await data.CountAsync();
                retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;
            }
            return retObj;
        }

        public async Task<List<EmployeeListDDLViewModel>> EmployeeListBySupervisorORLineManagerNOfficeadmin(long EmployeeId, long? WorkplaceGroupId)
        {
            User officeAdmin = _context.Users.Where(x => x.IsActive == true && x.IntRefferenceId == EmployeeId).FirstOrDefault();

            List<EmployeeListDDLViewModel> data = await (from emp in _context.EmpEmployeeBasicInfos
                                                         join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                         from info in info2.DefaultIfEmpty()
                                                         join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                                         from photo in photo2.DefaultIfEmpty()
                                                         let address = _context.EmpEmployeeAddresses.Where(x => x.IntAddressTypeId == 1).FirstOrDefault()
                                                         where emp.IntAccountId == officeAdmin.IntAccountId && emp.IsActive == true && info.IsActive == true && (info.IntEmployeeStatusId == 1 || info.IntEmployeeStatusId == 4)
                                                         && (officeAdmin.IsOfficeAdmin == true || emp.IntLineManagerId == EmployeeId || emp.IntSupervisorId == EmployeeId || emp.IntDottedSupervisorId == EmployeeId)
                                                         && (WorkplaceGroupId == null || WorkplaceGroupId == 0 || emp.IntWorkplaceGroupId == WorkplaceGroupId)
                                                         select new EmployeeListDDLViewModel
                                                         {
                                                             IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                             StrEmployeeName = emp.StrEmployeeName,
                                                             StrEmployeeCode = emp.StrEmployeeCode,
                                                             StrReferenceId = emp.StrReferenceId,
                                                             IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                                             IntAccountId = emp.IntAccountId,
                                                             StrOfficeMail = info.StrOfficeMail,
                                                             StrOfficeMobile = info.StrOfficeMobile,
                                                             StrPersonalMobile = info.StrPersonalMobile,
                                                             StrPersonalMail = info.StrPersonalMail
                                                         }).OrderBy(x => x.StrEmployeeName).AsQueryable().ToListAsync();

            return data;
        }

        public async Task<EmployeeProfileView> EmployeeProfileView(long employeeId)
        {
            EmployeeProfileView data = new EmployeeProfileView();

            data.EmployeeProfileLandingView = await EmployeeProfileViewData(employeeId);

            data.userVM = await (from u in _context.Users
                                 where u.IntRefferenceId == employeeId
                                 join ed in _context.EmpEmployeeBasicInfoDetails on u.IntRefferenceId equals ed.IntEmployeeId into empDetails
                                 from empd in empDetails.DefaultIfEmpty()
                                 join ut in _context.UserTypes on u.IntUserTypeId equals ut.IntUserTypeId into ut1
                                 from userType in ut1.DefaultIfEmpty()
                                 where u.IsActive == true && empd.IsActive == true
                                 select new UserVM
                                 {
                                     LoginId = u.StrLoginId,
                                     StrPassword = u.StrPassword,
                                     OfficeMail = empd.StrOfficeMail,
                                     StrPersonalMobile = empd.StrPersonalMobile,
                                     UserTypeId = u.IntUserTypeId,
                                     StrUserType = userType.StrUserType,
                                     UserStatus = u.IsActive
                                 }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            data.EmpEmployeeAddress = await _context.EmpEmployeeAddresses.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpEmployeeBankDetail = await _context.EmpEmployeeBankDetails.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).FirstOrDefaultAsync();
            data.EmpEmployeeEducation = await _context.EmpEmployeeEducations.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpEmployeeRelativesContact = await _context.EmpEmployeeRelativesContacts.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpEmployeeJobHistory = await _context.EmpEmployeeJobHistories.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpEmployeeTraining = await _context.EmpEmployeeTrainings.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpEmployeePhotoIdentity = await _context.EmpEmployeePhotoIdentities.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).FirstOrDefaultAsync();
            data.EmpJobExperience = await _context.EmpJobExperiences.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            data.EmpSocialMedia = await _context.EmpSocialMedia.AsNoTracking().AsQueryable().Where(x => x.IntEmployeeBasicInfoId == employeeId && x.IsActive == true).ToListAsync();
            return data;
        }

        #endregion ====================== Employee Query Data ==================

        #region ====================== Employee Bank Info ==================

        public async Task<MessageHelper> CRUDEmployeeBankDetails(EmployeeBankDetailsViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBankingDetails";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intPart", obj.PartId);
                        sqlCmd.Parameters.AddWithValue("@IntEmployeeBankDetailsId", obj.IntEmployeeBankDetailsId);
                        sqlCmd.Parameters.AddWithValue("@IntEmployeeBasicInfoId", obj.IntEmployeeBasicInfoId);
                        sqlCmd.Parameters.AddWithValue("@IntBankOrWalletType", obj.IntBankOrWalletType);
                        sqlCmd.Parameters.AddWithValue("@IntBankWalletId", obj.IntBankWalletId);
                        sqlCmd.Parameters.AddWithValue("@StrBankWalletName", obj.StrBankWalletName);
                        sqlCmd.Parameters.AddWithValue("@StrDistrict", obj.StrDistrict);
                        sqlCmd.Parameters.AddWithValue("@StrBranchName", obj.StrBranchName);
                        sqlCmd.Parameters.AddWithValue("@StrRoutingNo", obj.StrRoutingNo);
                        sqlCmd.Parameters.AddWithValue("@StrSwiftCode", obj.StrSwiftCode);
                        sqlCmd.Parameters.AddWithValue("@StrAccountName", obj.StrAccountName);
                        sqlCmd.Parameters.AddWithValue("@StrAccountNo", obj.StrAccountNo);
                        sqlCmd.Parameters.AddWithValue("@IsPrimarySalaryAccount", obj.IsPrimarySalaryAccount);
                        sqlCmd.Parameters.AddWithValue("@IsActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceId", obj.IntWorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", obj.IntBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", obj.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@DteCreatedAt", obj.DteCreatedAt);
                        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", obj.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@DteUpdatedAt", obj.DteUpdatedAt);
                        sqlCmd.Parameters.AddWithValue("@IntUpdatedBy", obj.IntUpdatedBy);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ====================== Employee Bank Info ==================

        #region ======== Bulk Tax Assign =========

        public async Task<MessageHelperBulkUpload> SaveTaxBulkUpload(List<TaxBulkUploadViewModel> model)
        {
            MessageHelperBulkUpload message = new MessageHelperBulkUpload();

            try
            {
                List<EmpTax> newBulkList = new List<EmpTax>(model.Count);
                List<EmpTax> editBulkList = new List<EmpTax>(model.Count);

                foreach (TaxBulkUploadViewModel employee in model)
                {
                    var emp = await _context.EmpEmployeeBasicInfos.Where(x => x.StrEmployeeCode == employee.StrEmployeeCode
                    && x.IsActive == true).FirstOrDefaultAsync();

                    if (emp == null)
                    {
                        throw new Exception($"Employee Code {employee.StrEmployeeCode} is not valid");
                    }

                    var exist = await _context.EmpTaxes.Where(x => x.IntEmployeeId == emp.IntEmployeeBasicInfoId && x.IsActive == true).FirstOrDefaultAsync();

                    if (exist == null)
                    {
                        EmpTax employeeInfo = new EmpTax();

                        employeeInfo.IntEmployeeId = emp.IntEmployeeBasicInfoId;
                        employeeInfo.NumTaxAmount = employee.NumTaxAmount;
                        employeeInfo.IntAccountId = emp.IntAccountId;
                        employeeInfo.IsActive = true;
                        employeeInfo.DteCreatedAt = DateTime.UtcNow;
                        employeeInfo.IntCreatedBy = emp.IntCreatedBy;

                        newBulkList.Add(employeeInfo);
                    }
                    else
                    {
                        exist.IntEmployeeId = emp.IntEmployeeBasicInfoId;
                        exist.NumTaxAmount = employee.NumTaxAmount;
                        exist.IntAccountId = emp.IntAccountId;
                        exist.IsActive = true;
                        exist.DteUpdatedAt = DateTime.UtcNow;
                        exist.IntUpdatedBy = emp.IntUpdatedBy;

                        editBulkList.Add(exist);
                    }
                }

                if (newBulkList.Count > 0)
                {
                    await _context.EmpTaxes.AddRangeAsync(newBulkList);
                    await _context.SaveChangesAsync();
                }

                if (editBulkList.Count > 0)
                {
                    _context.EmpTaxes.UpdateRange(editBulkList);
                    await _context.SaveChangesAsync();
                }

                message.StatusCode = 200;
                message.Message = "Income Tax Bulk Upload Successfully.";
                return message;
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return message;
            }
        }

        #endregion ======== Bulk Tax Assign =========

        #region ======== Tax Assign =========

        public async Task<MessageHelper> EmployeeTaxAssign(List<EmployeeTaxAssignViewModel> model)
        {
            MessageHelper message = new MessageHelper();

            try
            {
                List<long> empIdList = model.Select(a => (long)a.IntEmployeeId).ToList();
                var buID = model.Select(x => x.IntBusinessUnitId).First();
                var wgID = model.Select(x => x.IntWorkplaceGroupId).First();
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)buID, workplaceGroupId = (long)wgID }, PermissionLebelCheck.WorkplaceGroup);

                //IEnumerable<CommonEmployeeDDL> empIdCheck = await GetCommonEmployeeDDL(tokenData, item.IntBusinessUnitId, item.IntWorkplaceGroupId, item.IntEmployeeId, "");

                IEnumerable<CommonEmployeeDDL> empIdCheck = await PermissionCheckFromEmployeeListByEnvetFireEmployee(tokenData, (long)buID, (long)wgID, empIdList, "");

                if (empIdCheck.Count() != empIdList.Count())
                {
                    message.StatusCode = 401;
                    message.Message = "Employee Id Not Authenticate!";
                    return message;
                }

                foreach (var item in model)
                {
                    if (item.IntTaxId > 0)
                    {
                        EmpTax empTax = _context.EmpTaxes.Where(x => x.IntTaxId == item.IntTaxId && x.IntEmployeeId == item.IntEmployeeId && x.IntAccountId == item.IntAccountId && x.IsActive == true).FirstOrDefault();

                        if (empTax != null)
                        {
                            empTax.NumTaxAmount = (decimal)item.NumTaxAmount;
                            empTax.DteUpdatedAt = DateTime.UtcNow;
                            empTax.IntUpdatedBy = item.IntCreatedBy;

                            _context.EmpTaxes.Update(empTax);
                            await _context.SaveChangesAsync();

                            message.StatusCode = 200;
                            message.Message = "Tax Assign Successfully done.";
                        }
                        else
                        {
                            message.StatusCode = 500;
                            message.Message = "Tax not exists";
                        }
                    }
                    else
                    {
                        EmpTax empTax = new EmpTax()
                        {
                            IntEmployeeId = (long)item.IntEmployeeId,
                            NumTaxAmount = (long)item.NumTaxAmount,
                            IsActive = true,
                            IntAccountId = item.IntAccountId,
                            DteCreatedAt = DateTime.UtcNow,
                            IntCreatedBy = item.IntCreatedBy
                        };
                        _context.EmpTaxes.Add(empTax);
                        await _context.SaveChangesAsync();

                        message.StatusCode = 200;
                        message.Message = "Tax Assign Successfully done.";
                    }
                }
                return message;
            }
            catch (Exception e)
            {
                message.StatusCode = 500;
                message.Message = e.Message;
                return message;
            }
        }


        public async Task<ActiveEmployeeTaxAssignLanding> GetAllActiveEmployeeForTaxAssign(long IntAccountId, long? IntBusinessUnitId,
            long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntEmployeeId, string? searchTxt, int PageNo, int PageSize, int? intAssignStatus)
        {

            IQueryable<ActiveEmployeeForTaxAssignViewModel> data = (from emp in _context.EmpEmployeeBasicInfos
                                                                    join dtl1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals dtl1.IntEmployeeId into dtls
                                                                    from dtl in dtls.DefaultIfEmpty()
                                                                    join tax1 in _context.EmpTaxes on emp.IntEmployeeBasicInfoId equals tax1.IntEmployeeId into tax2
                                                                    from tax in tax2.DefaultIfEmpty()
                                                                    join dep1 in _context.MasterDepartments on emp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                                                    from dep in dep2.DefaultIfEmpty()
                                                                    join des1 in _context.MasterDesignations on emp.IntDesignationId equals des1.IntDesignationId into des2
                                                                    from des in des2.DefaultIfEmpty()
                                                                        //join salary1 in _context.PyrEmployeeSalaryElementAssignHeaders on emp.IntEmployeeBasicInfoId equals salary1.IntEmployeeId into salary2
                                                                        //from salary in salary2.DefaultIfEmpty()
                                                                    let salary = _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == emp.IntEmployeeBasicInfoId && x.IsActive == true).OrderBy(x => x.IntEmpSalaryElementAssignHeaderId).LastOrDefault()
                                                                    where emp.IntAccountId == IntAccountId
                                                                    && (emp.DteLastWorkingDate == null && emp.IsActive == true)
                                                                    && (dtl == null || dtl.IntEmployeeStatusId == 1 || dtl.IntEmployeeStatusId == 4)
                                                                    && emp.IntBusinessUnitId == IntBusinessUnitId
                                                                    && (IntWorkplaceGroupId == null || IntWorkplaceGroupId == 0 || emp.IntWorkplaceGroupId == IntWorkplaceGroupId)
                                                                    && (IntWorkplaceId == null || IntWorkplaceId == 0 || emp.IntWorkplaceId == IntWorkplaceId)
                                                                    && (IntEmployeeId == null || IntEmployeeId == 0 || emp.IntEmployeeBasicInfoId == IntEmployeeId)
                                                                    && (intAssignStatus == 1 ? tax.NumTaxAmount > 0 : intAssignStatus == 2 ? (tax.NumTaxAmount <= 0 || tax.NumTaxAmount == null) : intAssignStatus == null && intAssignStatus == 0 ? true : true)
                                                                    select new ActiveEmployeeForTaxAssignViewModel
                                                                    {
                                                                        IntAccountId = emp.IntAccountId,
                                                                        IntBusinessUnitId = emp.IntBusinessUnitId,
                                                                        IntEmployeeId = tax == null ? emp.IntEmployeeBasicInfoId : tax.IntEmployeeId,
                                                                        EmployeeName = emp.StrEmployeeName,
                                                                        EmployeeCode = emp.StrEmployeeCode,
                                                                        intDepartmentId = emp.IntDepartmentId,
                                                                        StrDepartment = dep.StrDepartment,
                                                                        IntDesignationId = emp.IntDesignationId,
                                                                        StrDesignation = des.StrDesignation,
                                                                        IntTaxId = tax == null ? 0 : tax.IntTaxId,
                                                                        IsTakeHomePay = dtl.IsTakeHomePay == null ? false : dtl.IsTakeHomePay,
                                                                        NumTaxAmount = tax == null ? 0 : tax.NumTaxAmount,
                                                                        NumGrossSalary = salary == null ? 0 : salary.NumNetGrossSalary,
                                                                        Status = (tax == null || tax.NumTaxAmount <= 0) ? "Not Assign" : "Assign"
                                                                    }).AsNoTracking().AsQueryable();

            ActiveEmployeeTaxAssignLanding retObj = new ActiveEmployeeTaxAssignLanding();
            int maxSize = 1000;
            PageSize = PageSize > maxSize ? maxSize : PageSize;
            PageNo = PageNo < 1 ? 1 : PageNo;

            if (!string.IsNullOrEmpty(searchTxt))
            {
                searchTxt = searchTxt.ToLower();
                data = data.Where(x => x.EmployeeName.ToLower().Contains(searchTxt) || x.EmployeeCode.ToLower().Contains(searchTxt)
                || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrDepartment.ToLower().Contains(searchTxt));
            }

            retObj.TotalCount = await data.CountAsync();
            retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
            retObj.PageSize = PageSize;
            retObj.CurrentPage = PageNo;

            return retObj;
        }


        #endregion ======== Tax Assign =========

        #region ========== Time Sheet CRUD ==============

        public async Task<CustomMessageHelper> TimeSheetCRUD(TimeSheetViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprTimeSheetCRUD";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPart", obj.PartType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intAutoId", obj.AutoId);

                        sqlCmd.Parameters.AddWithValue("@strInsertUserId", obj.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);

                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", obj.WorkplaceGroupId);

                        sqlCmd.Parameters.AddWithValue("@strHolidayGroupName", obj.HolidayGroupName);
                        sqlCmd.Parameters.AddWithValue("@intYear", obj.Year);

                        sqlCmd.Parameters.AddWithValue("@intHolidayGroupId", obj.HolidayGroupId);
                        sqlCmd.Parameters.AddWithValue("@strHolidayName", obj.HolidayName);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", obj.FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", obj.ToDate);
                        sqlCmd.Parameters.AddWithValue("@numTotalDays", obj.TotalDays);

                        sqlCmd.Parameters.AddWithValue("@strCalenderCode", obj.CalenderCode);
                        sqlCmd.Parameters.AddWithValue("@strCalenderName", obj.CalendarName);
                        sqlCmd.Parameters.AddWithValue("@dteOfficeStartTime", obj.OfficeStartTime);
                        sqlCmd.Parameters.AddWithValue("@dteStartTime", obj.StartTime);
                        sqlCmd.Parameters.AddWithValue("@dteExtendedStartTime", obj.ExtendedStartTime);
                        sqlCmd.Parameters.AddWithValue("@dteLastStartTime", obj.LastStartTime);
                        sqlCmd.Parameters.AddWithValue("@dteOfficeCloseTime", obj.OfficeCloseTime);
                        sqlCmd.Parameters.AddWithValue("@dteEndTime", obj.EndTime);
                        sqlCmd.Parameters.AddWithValue("@numMinWorkHour", obj.MinWorkHour);
                        sqlCmd.Parameters.AddWithValue("@isNightShift", obj.isNightShift);

                        sqlCmd.Parameters.AddWithValue("@strExceptionOffdayName", obj.ExceptionOffdayName);
                        sqlCmd.Parameters.AddWithValue("@isAlternativeDay", obj.IsAlternativeDay);

                        sqlCmd.Parameters.AddWithValue("@intExceptionOffdayGroupId", obj.ExceptionOffdayGroupId);
                        sqlCmd.Parameters.AddWithValue("@strWeekOfMonth", obj.WeekOfMonth);
                        sqlCmd.Parameters.AddWithValue("@intWeekOfMonthId", obj.WeekOfMonthId);
                        sqlCmd.Parameters.AddWithValue("@strDaysOfWeek", obj.DaysOfWeek);
                        sqlCmd.Parameters.AddWithValue("@intDaysOfWeekId", obj.DaysOfWeekId);
                        sqlCmd.Parameters.AddWithValue("@strRemarks", obj.Remarks);

                        sqlCmd.Parameters.AddWithValue("@strRosterGroupName", obj.RosterGroupName);
                        sqlCmd.Parameters.AddWithValue("@strOffdayGroupName", obj.OffdayGroupName);

                        sqlCmd.Parameters.AddWithValue("@isConfirm", obj.IsConfirm);

                        //Roster
                        sqlCmd.Parameters.AddWithValue("@jsonRoster", obj.timeSheetRosterJsons == null ? "[]" : JsonSerializer.Serialize(obj.timeSheetRosterJsons));
                        //Offday
                        sqlCmd.Parameters.AddWithValue("@jsonOffday", obj.timeSheetOffdayJsons == null ? "[]" : JsonSerializer.Serialize(obj.timeSheetOffdayJsons));
                        ///
                        sqlCmd.Parameters.AddWithValue("@jsonObjList", (obj.objOvertimeBulkUpload == null || obj.objOvertimeBulkUpload.Count == 0) ? "[]" : JsonSerializer.Serialize(obj.objOvertimeBulkUpload));

                        //DateTime startTime = string.IsNullOrEmpty(obj.StartTime) ? DateTime.Now : DateTime.Parse(obj.StartTime);
                        //DateTime endTime = string.IsNullOrEmpty(obj.EndTime) ? DateTime.Now : DateTime.Parse(obj.EndTime);

                        //double overtimeHours = (endTime - startTime).TotalHours;

                        sqlCmd.Parameters.AddWithValue("@intWorkplaceId", obj.WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@dteOvertimeDate", obj.OvertimeDate);
                        sqlCmd.Parameters.AddWithValue("@timeStartTime", string.IsNullOrEmpty(obj.StartTime) ? DateTime.Now.TimeOfDay : TimeSpan.Parse(obj.StartTime));
                        sqlCmd.Parameters.AddWithValue("@timeEndTime", string.IsNullOrEmpty(obj.EndTime) ? DateTime.Now.TimeOfDay : TimeSpan.Parse(obj.EndTime));
                        sqlCmd.Parameters.AddWithValue("@numOvertimeHour", obj.OvertimeHour);
                        sqlCmd.Parameters.AddWithValue("@strReason", obj.Reason);
                        sqlCmd.Parameters.AddWithValue("@dteBreakStartTime", obj.BreakStartTime);
                        sqlCmd.Parameters.AddWithValue("@dteBreakEndTime", obj.BreakEndTime);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new CustomMessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        msg.AutoId = string.IsNullOrEmpty(dt.Rows[0]["AutoId"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[0]["AutoId"]);
                        msg.AutoName = string.IsNullOrEmpty(dt.Rows[0]["AutoName"].ToString()) ? "" : Convert.ToString(dt.Rows[0]["AutoName"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<HolidayAssignLandingPaginationViewModelWithHeader> HolidayNExceptionFilter(BaseVM tokenData, long businessUnitId, long WorkplaceGroupId, string? searchTxt, int PageNo, int PageSize, bool? IsNotAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> departments, List<long> designations, List<long> supervisors, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                searchTxt = string.IsNullOrEmpty(searchTxt) ? searchTxt : searchTxt.ToLower();

                IQueryable<HolidayAssignViewModel> data = (from eb in _context.EmpEmployeeBasicInfos
                                                           join ed in _context.EmpEmployeeBasicInfoDetails on eb.IntEmployeeBasicInfoId equals ed.IntEmployeeId
                                                           join H in _context.TimeEmployeeHolidays on eb.IntEmployeeBasicInfoId equals H.IntEmployeeId into HJoin
                                                           from H in HJoin.DefaultIfEmpty()
                                                           join EOFF in _context.TimeEmployeeExcOffdays on eb.IntEmployeeBasicInfoId equals EOFF.IntEmployeeId into EOFFJoin
                                                           from EOFF in EOFFJoin.DefaultIfEmpty()
                                                           join EOFFG in _context.TimeExceptionOffdayGroups on EOFF.IntExceptionOffdayGroupId equals EOFFG.IntExceptionOffdayGroupId into EOFFGJoin
                                                           from EOFFG in EOFFGJoin.DefaultIfEmpty()
                                                           join dept in _context.MasterDepartments on eb.IntDepartmentId equals dept.IntDepartmentId into deptJoin
                                                           from dept in deptJoin.DefaultIfEmpty()
                                                           join desig in _context.MasterDesignations on eb.IntDesignationId equals desig.IntDesignationId into desigJoin
                                                           from desig in desigJoin.DefaultIfEmpty()
                                                           join super in _context.EmpEmployeeBasicInfos on eb.IntSupervisorId equals super.IntEmployeeBasicInfoId into superJoin
                                                           from supervisor in superJoin.DefaultIfEmpty()
                                                           join wg in _context.MasterWorkplaceGroups on eb.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wgJoin
                                                           from wg in wgJoin.DefaultIfEmpty()
                                                           join phi in _context.EmpEmployeePhotoIdentities on eb.IntEmployeeBasicInfoId equals phi.IntEmployeeBasicInfoId into phiJoin
                                                           from phi in phiJoin.DefaultIfEmpty()

                                                           join wi in _context.TerritorySetups on ed.IntWingId equals wi.IntTerritoryId into wi1
                                                           from wing in wi1.DefaultIfEmpty()
                                                           join soleD in _context.TerritorySetups on ed.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                           from soleDp in soleD1.DefaultIfEmpty()
                                                           join regn in _context.TerritorySetups on ed.IntRegionId equals regn.IntTerritoryId into regn1
                                                           from region in regn1.DefaultIfEmpty()
                                                           join area1 in _context.TerritorySetups on ed.IntAreaId equals area1.IntTerritoryId into area2
                                                           from area in area2.DefaultIfEmpty()
                                                           join terrty in _context.TerritorySetups on ed.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                           from Territory in terrty1.DefaultIfEmpty()

                                                           where eb.IsActive == true && ed.IsActive == true && eb.IntAccountId == tokenData.accountId
                                                              && (ed.IntEmployeeStatusId == 1 || ed.IntEmployeeStatusId == 4)

                                                              && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && eb.IntBusinessUnitId == businessUnitId
                                                              && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(WorkplaceGroupId)) && eb.IntWorkplaceGroupId == WorkplaceGroupId

                                                             && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                             : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId)
                                                             : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId)
                                                             : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo)
                                                             : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId)
                                                             : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId) && tokenData.areaList.Contains(ed.IntAreaId)
                                                             : tokenData.territoryList.Contains(ed.IntTerritoryId))
                                                             || eb.IntDottedSupervisorId == tokenData.employeeId || eb.IntSupervisorId == tokenData.employeeId || eb.IntLineManagerId == tokenData.employeeId)

                                                              && (!string.IsNullOrEmpty(searchTxt) ? (eb.StrEmployeeName.ToLower().Contains(searchTxt) || eb.StrEmployeeCode.ToLower().Contains(searchTxt)
                                                                  || eb.StrReferenceId.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                                                                  || ed.StrPinNo.ToLower().Contains(searchTxt) || ed.StrPersonalMobile.ToLower().Contains(searchTxt)) : true)

                                                              && (IsPaginated == true ? ((departments.Count > 0 ? departments.Contains((long)eb.IntDepartmentId) : true)
                                                                  && (designations.Count > 0 ? designations.Contains((long)eb.IntDesignationId) : true)
                                                                  && (supervisors.Count > 0 ? supervisors.Contains((long)eb.IntSupervisorId) : true)
                                                                  && (WingList.Count > 0 ? WingList.Contains((long)ed.IntWingId) : true)
                                                                  && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)ed.IntSoleDepo) : true)
                                                                  && (RegionList.Count > 0 ? RegionList.Contains((long)ed.IntRegionId) : true)
                                                                  && (AreaList.Count > 0 ? AreaList.Contains((long)ed.IntAreaId) : true)
                                                                  && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)ed.IntTerritoryId) : true)) : true)

                                                              && (IsNotAssign == null ? true : IsNotAssign == true ? (H.IntHolidayGroupId == null) : H.IntHolidayGroupId != null)

                                                           //&& (IsNotAssign == null || (att != null && IsNotAssign == false) || (att == null && IsNotAssign == true))

                                                           orderby eb.IntEmployeeBasicInfoId ascending
                                                           select new HolidayAssignViewModel
                                                           {
                                                               EmployeeId = eb.IntEmployeeBasicInfoId,
                                                               EmployeeName = eb.StrEmployeeName,
                                                               EmployeeCode = eb.StrEmployeeCode,
                                                               PinNo = ed.StrPinNo,
                                                               DepartmentId = eb.IntDepartmentId,
                                                               Department = dept.StrDepartment,
                                                               DesignationId = eb.IntDesignationId,
                                                               Designation = desig.StrDesignation,
                                                               SupervisorId = eb.IntSupervisorId,
                                                               SupervisorName = supervisor.StrEmployeeName,
                                                               WorkplaceGroupId = wg.IntWorkplaceGroupId,
                                                               WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                               BusinessUnitId = eb.IntBusinessUnitId,
                                                               EmploymentStatusId = ed.IntEmployeeStatusId,
                                                               EmployeeStatus = ed.StrEmployeeStatus,
                                                               ProfileImageUrl = phi.IntProfilePicFileUrlId,
                                                               EmployeeHolidayAssignId = H.IntEmployeeHolidayAssignId,
                                                               HolidayGroupId = H.IntHolidayGroupId,
                                                               HolidayGroupName = H.StrHolidayGroupName,
                                                               ExceptionOffdayGroupId = EOFF.IntExceptionOffdayGroupId,
                                                               ExceptionOffdayGroupName = EOFFG.StrExceptionOffdayName,
                                                               ExceptionEffectiveDate = EOFF.DteEffectiveDate,
                                                               HolidayEffectiveDate = H.DteEffectiveDate,
                                                               IsAlternativeDay = EOFFG.IsAlternativeDay,
                                                               WingId = ed.IntWingId,
                                                               WingName = wing == null ? "" : wing.StrTerritoryName,
                                                               SoleDepoId = ed.IntSoleDepo,
                                                               SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                               RegionId = ed.IntRegionId,
                                                               RegionName = region == null ? "" : region.StrTerritoryName,
                                                               AreaId = ed.IntAreaId,
                                                               AreaName = area == null ? "" : area.StrTerritoryName,
                                                               TerritoryId = ed.IntTerritoryId,
                                                               TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                                           }).AsNoTracking().AsQueryable();



                HolidayAssignLandingPaginationViewModelWithHeader holidayAssign = new();

                if (data.Count() <= 0)
                {
                    return holidayAssign;
                }

                if (IsHeaderNeed)
                {
                    HolidayAssignHeader eh = new();

                    eh.WingNameList = data.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).Distinct().ToList();
                    eh.SoleDepoNameList = data.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).Distinct().ToList();
                    eh.RegionNameList = data.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).Distinct().ToList();
                    eh.AreaNameList = data.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).Distinct().ToList();
                    eh.TerritoryNameList = data.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).Distinct().ToList();
                    eh.DepartmentList = data.Where(x => !string.IsNullOrEmpty(x.Department)).Select(x => new CommonDDLVM { Value = (long)x.DepartmentId, Label = (string)x.Department }).Distinct().ToList();
                    eh.DesignationList = data.Where(x => !string.IsNullOrEmpty(x.Designation)).Select(x => new CommonDDLVM { Value = (long)x.DesignationId, Label = (string)x.Designation }).Distinct().ToList();
                    eh.SupervisorNameList = data.Where(x => !string.IsNullOrEmpty(x.SupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.SupervisorId, Label = (string)x.SupervisorName }).Distinct().ToList();
                    //eh.StrLinemanagerList = data.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).Distinct().ToList();
                    //eh.StrEmploymentTypeList = data.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).Distinct().ToList();

                    holidayAssign.holidayAssignHeader = eh;
                }

                if (IsPaginated == false)
                {
                    holidayAssign.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    holidayAssign.TotalCount = await data.CountAsync();
                    holidayAssign.employeeList = holidayAssign.TotalCount > 0 ? string.Join(",", await data.Select(x => x.EmployeeId).ToListAsync()) : "";
                    holidayAssign.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                    holidayAssign.PageSize = PageSize;
                    holidayAssign.CurrentPage = PageNo;
                }

                return holidayAssign;
                //using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                //{
                //    string sql = "saas.sprTimeSheetAllLanding";
                //    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                //    {
                //        string jsonString = JsonSerializer.Serialize(obj);

                //        sqlCmd.CommandType = CommandType.StoredProcedure;
                //        sqlCmd.Parameters.AddWithValue("@strPart", "holidayNexception");
                //        sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
                //        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", 0);
                //        sqlCmd.Parameters.AddWithValue("@intId", 0);
                //        sqlCmd.Parameters.AddWithValue("@intYear", 0);
                //        sqlCmd.Parameters.AddWithValue("@intMonth", 0);
                //        sqlCmd.Parameters.AddWithValue("@intPageNo", obj.PageNo);
                //        sqlCmd.Parameters.AddWithValue("@intPageSize", obj.PageSize);
                //        sqlCmd.Parameters.AddWithValue("@strSearchText", obj.SearchText);
                //        connection.Open();

                //        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                //        {
                //            sqlAdapter.Fill(dt);
                //        }
                //        connection.Close();
                //    }
                //}
                //return query;
            }
            catch (Exception ex)
            {
                return new HolidayAssignLandingPaginationViewModelWithHeader();
            }
        }

        public async Task<MessageHelper> HolidayAndExceptionOffdayAssign(HolidayAssignVm obj)
        {
            var msg = new MessageHelper();
            msg.StatusCode = 200;

            try
            {
                // ====================== Holiday ================================

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {

                    string sql = "saas.sprEmployeeHolidayAssign_Bulk";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@employeeList", obj.EmployeeList);
                        sqlCmd.Parameters.AddWithValue("@HolydayGroupId", obj.HolidayGroupId);
                        sqlCmd.Parameters.AddWithValue("@strHolidayGroupName", obj.HolidayGroupName);
                        sqlCmd.Parameters.AddWithValue("@effectiveDate", obj.EffectiveDate);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", obj.WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@actionBy", obj.ActionBy);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        msg.Message = "Holiday " + output;
                    }
                }

                // ========================= EXCEPTION ===================================
                /*
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string jsonString = JsonSerializer.Serialize(obj.ExceptionOffdayAssignDTOList);

                        string sql = "saas.sprEmployeeExcOffdayAssign";
                        using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;

                            sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                            SqlParameter outputParameter = new SqlParameter();
                            outputParameter.ParameterName = "@msg";
                            outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                            outputParameter.Size = int.MaxValue;
                            outputParameter.Direction = System.Data.ParameterDirection.Output;
                            sqlCmd.Parameters.Add(outputParameter);
                            connection.Open();
                            sqlCmd.ExecuteNonQuery();
                            string output = outputParameter.Value.ToString();
                            connection.Close();

                        msg.Message = msg.Message + " And " + output;
                    }
                }
                */
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CalendarAssignLandingPaginationViewModelWithHeader> CalendarAssignFilter(BaseVM tokenData, long BusinessUnitId, long WorkplaceGroupId, string? SearchTxt, int PageNo, int PageSize, bool? IsNotAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> Departments, List<long> Designations, List<long> Supervisors, List<long> EmploymentTypeList, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                SearchTxt = string.IsNullOrEmpty(SearchTxt) ? SearchTxt : SearchTxt.ToLower();

                IQueryable<CalendarAssignLandingViewModel> data = (from eb in _context.EmpEmployeeBasicInfos
                                                                   join ed in _context.EmpEmployeeBasicInfoDetails on eb.IntEmployeeBasicInfoId equals ed.IntEmployeeId
                                                                   join supervisor in _context.EmpEmployeeBasicInfos on eb.IntSupervisorId equals supervisor.IntEmployeeBasicInfoId into supervisorGroup
                                                                   from supervisor in supervisorGroup.DefaultIfEmpty()
                                                                   join att in _context.TimeAttendanceDailySummaries on new { EmployeeId = eb.IntEmployeeBasicInfoId, AttendanceDate = DateTime.Now.Date } equals new { EmployeeId = att.IntEmployeeId, AttendanceDate = (DateTime)att.DteAttendanceDate.Value.Date } into attGroup
                                                                   from att in attGroup.DefaultIfEmpty()
                                                                   join dept in _context.MasterDepartments on eb.IntDepartmentId equals dept.IntDepartmentId into deptGroup
                                                                   from dept in deptGroup.DefaultIfEmpty()
                                                                   join desig in _context.MasterDesignations on eb.IntDesignationId equals desig.IntDesignationId into desigGroup
                                                                   from desig in desigGroup.DefaultIfEmpty()

                                                                   join wi in _context.TerritorySetups on ed.IntWingId equals wi.IntTerritoryId into wi1
                                                                   from wing in wi1.DefaultIfEmpty()
                                                                   join soleD in _context.TerritorySetups on ed.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                                   from soleDp in soleD1.DefaultIfEmpty()
                                                                   join regn in _context.TerritorySetups on ed.IntRegionId equals regn.IntTerritoryId into regn1
                                                                   from region in regn1.DefaultIfEmpty()
                                                                   join area1 in _context.TerritorySetups on ed.IntAreaId equals area1.IntTerritoryId into area2
                                                                   from area in area2.DefaultIfEmpty()
                                                                   join terrty in _context.TerritorySetups on ed.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                                   from Territory in terrty1.DefaultIfEmpty()

                                                                   join wg in _context.MasterWorkplaceGroups on eb.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wgGroup
                                                                   from wg in wgGroup.DefaultIfEmpty()
                                                                   join phi in _context.EmpEmployeePhotoIdentities on eb.IntEmployeeBasicInfoId equals phi.IntEmployeeBasicInfoId into phiGroup
                                                                   from phi in phiGroup.DefaultIfEmpty()
                                                                   where eb.IsActive == true && ed.IsActive == true && eb.IntAccountId == tokenData.accountId
                                                                   && (ed.IntEmployeeStatusId == 1 || ed.IntEmployeeStatusId == 4)

                                                                   && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(BusinessUnitId)) && eb.IntBusinessUnitId == BusinessUnitId
                                                                   && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(WorkplaceGroupId)) && eb.IntWorkplaceGroupId == WorkplaceGroupId

                                                                     && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                     : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId)
                                                                     : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId)
                                                                     : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo)
                                                                     : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId)
                                                                     : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId) && tokenData.areaList.Contains(ed.IntAreaId)
                                                                     : tokenData.territoryList.Contains(ed.IntTerritoryId))
                                                                     || eb.IntDottedSupervisorId == tokenData.employeeId || eb.IntSupervisorId == tokenData.employeeId || eb.IntLineManagerId == tokenData.employeeId)

                                                                   && (!string.IsNullOrEmpty(SearchTxt) ? (eb.StrEmployeeName.ToLower().Contains(SearchTxt) || eb.StrEmployeeCode.ToLower().Contains(SearchTxt)
                                                                       || eb.StrReferenceId.ToLower().Contains(SearchTxt) || desig.StrDesignation.ToLower().Contains(SearchTxt) || dept.StrDepartment.ToLower().Contains(SearchTxt)
                                                                       || ed.StrPinNo.ToLower().Contains(SearchTxt) || ed.StrPersonalMobile.ToLower().Contains(SearchTxt)) : true)

                                                                   && (IsPaginated == true ? ((Departments.Count > 0 ? Departments.Contains((long)eb.IntDepartmentId) : true)
                                                                       && (Designations.Count > 0 ? Designations.Contains((long)eb.IntDesignationId) : true)
                                                                       && (Supervisors.Count > 0 ? Supervisors.Contains((long)eb.IntSupervisorId) : true)
                                                                       && (EmploymentTypeList.Count > 0 ? EmploymentTypeList.Contains((long)eb.IntEmploymentTypeId) : true)
                                                                       && (WingList.Count > 0 ? WingList.Contains((long)ed.IntWingId) : true)
                                                                       && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)ed.IntSoleDepo) : true)
                                                                       && (RegionList.Count > 0 ? RegionList.Contains((long)ed.IntRegionId) : true)
                                                                       && (AreaList.Count > 0 ? AreaList.Contains((long)ed.IntAreaId) : true)
                                                                       && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)ed.IntTerritoryId) : true)) : true)

                                                                   && (IsNotAssign == null || (att != null && IsNotAssign == false) || (att == null && IsNotAssign == true))

                                                                   orderby eb.IntEmployeeBasicInfoId descending
                                                                   select new CalendarAssignLandingViewModel
                                                                   {
                                                                       CalendarAssignId = att.IntCalendarId,
                                                                       EmployeeId = eb.IntEmployeeBasicInfoId,
                                                                       EmployeeName = eb.StrEmployeeName,
                                                                       EmployeeCode = eb.StrEmployeeCode,
                                                                       DepartmentId = eb.IntDepartmentId,
                                                                       Department = dept.StrDepartment,
                                                                       DesignationId = eb.IntDesignationId,
                                                                       Designation = desig.StrDesignation,
                                                                       SupervisorId = eb.IntSupervisorId,
                                                                       SupervisorName = supervisor.StrEmployeeName,
                                                                       WorkplaceGroupId = wg.IntWorkplaceGroupId,
                                                                       WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                                       EmploymentStatusId = ed.IntEmployeeStatusId,
                                                                       EmployeeStatus = ed.StrEmployeeStatus,
                                                                       EmploymentTypeId = eb.IntEmploymentTypeId,
                                                                       EmploymentType = eb.StrEmploymentType,
                                                                       ProfileImageUrl = phi.IntProfilePicFileUrlId,
                                                                       RosterGroupId = att.IntRosterGroupId,
                                                                       RosterGroupName = att.StrRosterGroupName,
                                                                       CalendarType = att.StrCalendarType,
                                                                       CalendarName = att.StrCalendarName ?? "N/A",
                                                                       GenerateDate = att.DteGenerateDate,
                                                                       JoiningDate = eb.DteJoiningDate,
                                                                       WingId = ed.IntWingId,
                                                                       AccountId = eb.IntAccountId,
                                                                       BusinessUnitId = eb.IntBusinessUnitId,
                                                                       WingName = wing == null ? "" : wing.StrTerritoryName,
                                                                       SoleDepoId = ed.IntSoleDepo,
                                                                       SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                                       RegionId = ed.IntRegionId,
                                                                       RegionName = region == null ? "" : region.StrTerritoryName,
                                                                       AreaId = ed.IntAreaId,
                                                                       AreaName = area == null ? "" : area.StrTerritoryName,
                                                                       TerritoryId = ed.IntTerritoryId,
                                                                       TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                                                   }).AsNoTracking().AsQueryable();

                CalendarAssignLandingPaginationViewModelWithHeader calendarLanding = new();

                if (data.Count() <= 0)
                {
                    return calendarLanding;
                }

                if (IsHeaderNeed)
                {
                    CalendarAssignHeader eh = new();

                    eh.WingNameList = data.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).Distinct().ToList();
                    eh.SoleDepoNameList = data.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).Distinct().ToList();
                    eh.RegionNameList = data.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).Distinct().ToList();
                    eh.AreaNameList = data.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).Distinct().ToList();
                    eh.TerritoryNameList = data.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).Distinct().ToList();
                    eh.DepartmentList = data.Where(x => !string.IsNullOrEmpty(x.Department)).Select(x => new CommonDDLVM { Value = (long)x.DepartmentId, Label = (string)x.Department }).Distinct().ToList();
                    eh.DesignationList = data.Where(x => !string.IsNullOrEmpty(x.Designation)).Select(x => new CommonDDLVM { Value = (long)x.DesignationId, Label = (string)x.Designation }).Distinct().ToList();
                    eh.SupervisorNameList = data.Where(x => !string.IsNullOrEmpty(x.SupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.SupervisorId, Label = (string)x.SupervisorName }).Distinct().ToList();
                    eh.EmploymentTypeList = data.Where(x => !string.IsNullOrEmpty(x.EmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.EmploymentTypeId, Label = (string)x.EmploymentType }).Distinct().ToList();

                    calendarLanding.calendarAssignHeader = eh;
                }

                if (IsPaginated == false)
                {
                    calendarLanding.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    calendarLanding.TotalCount = await data.CountAsync();
                    calendarLanding.employeeIdList = calendarLanding.TotalCount > 0 ? string.Join(",", await data.Select(x => x.EmployeeId).ToListAsync()) : "";
                    calendarLanding.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                    calendarLanding.PageSize = PageSize;
                    calendarLanding.CurrentPage = PageNo;
                }

                return calendarLanding;
            }
            catch (Exception ex)
            {
                return new CalendarAssignLandingPaginationViewModelWithHeader();
            }
        }

        public async Task<dynamic> TimeAttendanceSummaryForRoasterAsync(long IntEmployeeId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
                string sql = "saas.sprTimeAttendanceSummaryForRoaster";
                var values = new
                {
                    intEmployeeId = IntEmployeeId,
                    dteGenerateStartDate = FromDate,
                    dteGenerateEndDate = ToDate
                };
                return await connection.QueryFirstOrDefaultAsync<MessageHelper>(sql, values, commandType: CommandType.StoredProcedure, commandTimeout: 0);

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<MessageHelper> RosterGenerateList(RosterGenerateViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprRosterGenerate";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        //var jsonString = JsonSerializer.Serialize(obj);

                        sqlCmd.Parameters.AddWithValue("@employeeIdList", obj.EmployeeList);
                        sqlCmd.Parameters.AddWithValue("@generateStartDate", obj.GenerateStartDate);
                        sqlCmd.Parameters.AddWithValue("@generateEndDate", obj.GenerateEndDate);
                        sqlCmd.Parameters.AddWithValue("@runningCalendarId", obj.RunningCalendarId);
                        sqlCmd.Parameters.AddWithValue("@clendarType", obj.CalendarType);
                        sqlCmd.Parameters.AddWithValue("@rosterGroupId", obj.RosterGroupId);
                        sqlCmd.Parameters.AddWithValue("@isAutoGenerate", obj.IsAutoGenerate);
                        sqlCmd.Parameters.AddWithValue("@nextChangeDate", obj.NextChangeDate);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", obj.IntCreatedBy);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);

                        sqlCmd.CommandTimeout = 1800;
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OffdayAssignLandingPaginationViewModelWithHeader> OffdayLandingFilter(BaseVM tokenData, long BusinessUnitId, long WorkplaceGroupId, string? SearchTxt, int PageNo, int PageSize, bool? IsAssign, bool IsPaginated, bool IsHeaderNeed,
            List<long> Departments, List<long> Designations, List<long> Supervisors, List<long> EmploymentTypeList, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                IQueryable<OffdayAssignLandingViewModel> data = (from eb in _context.EmpEmployeeBasicInfos
                                                                 join ed in _context.EmpEmployeeBasicInfoDetails on eb.IntEmployeeBasicInfoId equals ed.IntEmployeeId
                                                                 join dept in _context.MasterDepartments on eb.IntDepartmentId equals dept.IntDepartmentId into deptGroup
                                                                 from dept in deptGroup.DefaultIfEmpty()
                                                                 join desig in _context.MasterDesignations on eb.IntDesignationId equals desig.IntDesignationId into desigGroup
                                                                 from desig in desigGroup.DefaultIfEmpty()
                                                                 join wg in _context.MasterWorkplaceGroups on eb.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wgGroup
                                                                 from wg in wgGroup.DefaultIfEmpty()

                                                                 join wi in _context.TerritorySetups on ed.IntWingId equals wi.IntTerritoryId into wi1
                                                                 from wing in wi1.DefaultIfEmpty()
                                                                 join soleD in _context.TerritorySetups on ed.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                                 from soleDp in soleD1.DefaultIfEmpty()
                                                                 join regn in _context.TerritorySetups on ed.IntRegionId equals regn.IntTerritoryId into regn1
                                                                 from region in regn1.DefaultIfEmpty()
                                                                 join area1 in _context.TerritorySetups on ed.IntAreaId equals area1.IntTerritoryId into area2
                                                                 from area in area2.DefaultIfEmpty()
                                                                 join terrty in _context.TerritorySetups on ed.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                                 from Territory in terrty1.DefaultIfEmpty()

                                                                 join phi in _context.EmpEmployeePhotoIdentities on eb.IntEmployeeBasicInfoId equals phi.IntEmployeeBasicInfoId into phiGroup
                                                                 from phi in phiGroup.DefaultIfEmpty()
                                                                 join OD in _context.TimeEmployeeOffdays on eb.IntEmployeeBasicInfoId equals OD.IntEmployeeId into ODGroup
                                                                 from OD in ODGroup.DefaultIfEmpty()
                                                                 where eb.IsActive == true && ed.IsActive == true && eb.IntAccountId == tokenData.accountId
                                                                 && (ed.IntEmployeeStatusId == 1 || ed.IntEmployeeStatusId == 4)

                                                                 && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(BusinessUnitId)) && eb.IntBusinessUnitId == BusinessUnitId
                                                                 && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(WorkplaceGroupId)) && eb.IntWorkplaceGroupId == WorkplaceGroupId

                                                                 && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                 : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId)
                                                                 : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId)
                                                                 : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo)
                                                                 : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId)
                                                                 : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(eb.IntWorkplaceGroupId) && tokenData.wingList.Contains(ed.IntWingId) && tokenData.soleDepoList.Contains(ed.IntSoleDepo) && tokenData.regionList.Contains(ed.IntRegionId) && tokenData.areaList.Contains(ed.IntAreaId)
                                                                 : tokenData.territoryList.Contains(ed.IntTerritoryId))
                                                                 || eb.IntDottedSupervisorId == tokenData.employeeId || eb.IntSupervisorId == tokenData.employeeId || eb.IntLineManagerId == tokenData.employeeId)

                                                                    && (!string.IsNullOrEmpty(SearchTxt) ? (eb.StrEmployeeName.ToLower().Contains(SearchTxt) || eb.StrEmployeeCode.ToLower().Contains(SearchTxt)
                                                                        || eb.StrReferenceId.ToLower().Contains(SearchTxt) || desig.StrDesignation.ToLower().Contains(SearchTxt) || dept.StrDepartment.ToLower().Contains(SearchTxt)
                                                                        || ed.StrPinNo.ToLower().Contains(SearchTxt) || ed.StrPersonalMobile.ToLower().Contains(SearchTxt)) : true)

                                                                    && (IsPaginated == true ? ((Departments.Count > 0 ? Departments.Contains((long)eb.IntDepartmentId) : true)
                                                                        && (Designations.Count > 0 ? Designations.Contains((long)eb.IntDesignationId) : true)
                                                                        && (Supervisors.Count > 0 ? Supervisors.Contains((long)eb.IntSupervisorId) : true)
                                                                        && (EmploymentTypeList.Count > 0 ? EmploymentTypeList.Contains((long)eb.IntEmploymentTypeId) : true)
                                                                        && (WingList.Count > 0 ? WingList.Contains((long)ed.IntWingId) : true)
                                                                        && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)ed.IntSoleDepo) : true)
                                                                        && (RegionList.Count > 0 ? RegionList.Contains((long)ed.IntRegionId) : true)
                                                                        && (AreaList.Count > 0 ? AreaList.Contains((long)ed.IntAreaId) : true)
                                                                        && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)ed.IntTerritoryId) : true)) : true)

                                                                    && (IsAssign == null || (IsAssign == false ? ((OD.IsSaturday == false || OD.IsSaturday == null) && (OD.IsSunday == false || OD.IsSunday == null) && (OD.IsMonday == false || OD.IsMonday == null) && (OD.IsTuesday == false || OD.IsTuesday == null) && (OD.IsWednesday == false || OD.IsWednesday == null) && (OD.IsThursday == false || OD.IsThursday == null) && (OD.IsFriday == false || OD.IsFriday == null))
                                                                            : (OD.IsSaturday == true || OD.IsSunday == true || OD.IsMonday == true || OD.IsTuesday == true || OD.IsWednesday == true || OD.IsThursday == true || OD.IsFriday == true)))

                                                                 orderby eb.IntEmployeeBasicInfoId ascending
                                                                 select new OffdayAssignLandingViewModel
                                                                 {
                                                                     EmployeeOffdayAssignId = OD.IntEmployeeOffdayAssignId,
                                                                     EmployeeId = eb.IntEmployeeBasicInfoId,
                                                                     EmployeeName = eb.StrEmployeeName,
                                                                     EmployeeCode = eb.StrEmployeeCode,
                                                                     DepartmentId = eb.IntDepartmentId,
                                                                     Department = dept.StrDepartment,
                                                                     DesignationId = eb.IntDesignationId,
                                                                     Designation = desig.StrDesignation,
                                                                     SupervisorId = eb.IntEmployeeBasicInfoId,
                                                                     SupervisorName = eb.StrEmployeeName,
                                                                     WorkplaceGroupId = eb.IntWorkplaceGroupId,
                                                                     WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                                     EmploymentTypeId = eb.IntEmploymentTypeId,
                                                                     EmploymentType = eb.StrEmploymentType,
                                                                     EmploymentStatusId = ed.IntEmployeeStatusId,
                                                                     EmployeeStatus = ed.StrEmployeeStatus,
                                                                     ProfileImageUrl = phi.IntProfilePicFileUrlId,
                                                                     EffectiveDate = OD.DteEffectiveDate,
                                                                     IsSaturday = OD.IsSaturday,
                                                                     IsSunday = OD.IsSunday,
                                                                     IsMonday = OD.IsMonday,
                                                                     IsTuesday = OD.IsTuesday,
                                                                     IsWednesday = OD.IsWednesday,
                                                                     IsThursday = OD.IsThursday,
                                                                     IsFriday = OD.IsFriday,
                                                                     WingId = ed.IntWingId,
                                                                     AccountId = eb.IntAccountId,
                                                                     BusinessUnitId = eb.IntBusinessUnitId,
                                                                     WingName = wing == null ? "" : wing.StrTerritoryName,
                                                                     SoleDepoId = ed.IntSoleDepo,
                                                                     SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                                     RegionId = ed.IntRegionId,
                                                                     RegionName = region == null ? "" : region.StrTerritoryName,
                                                                     AreaId = ed.IntAreaId,
                                                                     AreaName = area == null ? "" : area.StrTerritoryName,
                                                                     TerritoryId = ed.IntTerritoryId,
                                                                     TerritoryName = Territory == null ? "" : Territory.StrTerritoryName
                                                                 }).AsNoTracking().AsQueryable();



                OffdayAssignLandingPaginationViewModelWithHeader offdayAssign = new();

                if (data.Count() <= 0)
                {
                    return offdayAssign;
                }

                if (IsHeaderNeed)
                {
                    OffdayAssignHeader eh = new();

                    eh.WingNameList = data.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).Distinct().ToList();
                    eh.SoleDepoNameList = data.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).Distinct().ToList();
                    eh.RegionNameList = data.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).Distinct().ToList();
                    eh.AreaNameList = data.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).Distinct().ToList();
                    eh.TerritoryNameList = data.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).Distinct().ToList();
                    eh.DepartmentList = data.Where(x => !string.IsNullOrEmpty(x.Department)).Select(x => new CommonDDLVM { Value = (long)x.DepartmentId, Label = (string)x.Department }).Distinct().ToList();
                    eh.DesignationList = data.Where(x => !string.IsNullOrEmpty(x.Designation)).Select(x => new CommonDDLVM { Value = (long)x.DesignationId, Label = (string)x.Designation }).Distinct().ToList();
                    eh.SupervisorNameList = data.Where(x => !string.IsNullOrEmpty(x.SupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.SupervisorId, Label = (string)x.SupervisorName }).Distinct().ToList();
                    eh.EmploymentTypeList = data.Where(x => !string.IsNullOrEmpty(x.EmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.EmploymentTypeId, Label = (string)x.EmploymentType }).Distinct().ToList();

                    offdayAssign.offdayAssignHeader = eh;
                }

                if (IsPaginated == false)
                {
                    offdayAssign.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    offdayAssign.TotalCount = await data.CountAsync();
                    offdayAssign.employeeList = offdayAssign.TotalCount > 0 ? string.Join(",", await data.Select(x => x.EmployeeId).ToListAsync()) : "";
                    offdayAssign.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                    offdayAssign.PageSize = PageSize;
                    offdayAssign.CurrentPage = PageNo;
                }

                //using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                //{
                //    string sql = "saas.sprTimeSheetAllLanding";
                //    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                //    {
                //        string jsonString = JsonSerializer.Serialize(obj);

                //        sqlCmd.CommandType = CommandType.StoredProcedure;
                //        sqlCmd.Parameters.AddWithValue("@strPart", "offday");
                //        sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
                //        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", 0);
                //        sqlCmd.Parameters.AddWithValue("@intId", 0);
                //        sqlCmd.Parameters.AddWithValue("@intYear", 0);
                //        sqlCmd.Parameters.AddWithValue("@intMonth", 0);
                //        sqlCmd.Parameters.AddWithValue("@intPageNo", obj.PageNo);
                //        sqlCmd.Parameters.AddWithValue("@intPageSize", obj.PageSize);
                //        sqlCmd.Parameters.AddWithValue("@strSearchText", obj.SearchText);
                //        connection.Open();

                //        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                //        {
                //            sqlAdapter.Fill(dt);
                //        }
                //        connection.Close();
                //    }
                //}
                return offdayAssign;
            }
            catch (Exception ex)
            {
                return new OffdayAssignLandingPaginationViewModelWithHeader();
            }
        }

        public async Task<MessageHelper> OffdayAssign(OffdayAssignViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprEmployeeOffdayAssign";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@employeeList", obj.EmployeeList);
                        sqlCmd.Parameters.AddWithValue("@effectiveDate", obj.EffectiveDate);
                        sqlCmd.Parameters.AddWithValue("@saturday", obj.IsSaturday);
                        sqlCmd.Parameters.AddWithValue("@sunday", obj.IsSunday);
                        sqlCmd.Parameters.AddWithValue("@monday", obj.IsMonday);
                        sqlCmd.Parameters.AddWithValue("@tuesday", obj.IsTuesday);
                        sqlCmd.Parameters.AddWithValue("@wednesnesday", obj.IsWednesday);
                        sqlCmd.Parameters.AddWithValue("@thursday", obj.IsThursday);
                        sqlCmd.Parameters.AddWithValue("@friday", obj.IsFriday);
                        sqlCmd.Parameters.AddWithValue("@account", obj.AccountId);
                        sqlCmd.Parameters.AddWithValue("@businessUnit", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@workplaceGroup", obj.WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@active", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@actionBy", obj.ActionBy);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EmpOverTimeUploadDTO>> CreateOvertimeUpload(List<EmpOverTimeUploadDTO> objList)
        {
            var createList = new List<TimeEmpOverTimeUpload>();
            var responseList = new List<EmpOverTimeUploadDTO>();

            foreach (var obj in objList)
            {
                bool isValid = true;
                DateTime OvertimeDate = DateTime.Now;
                string FromTime = "";
                string ToTime = "";

                var EmpInfo = await (from emp in _context.EmpEmployeeBasicInfos
                                     join ds in _context.MasterDesignations on emp.IntDesignationId equals ds.IntDesignationId into dsg
                                     from desig in dsg.DefaultIfEmpty()
                                     where emp.StrEmployeeCode.ToLower() == obj.EmployeeCode.ToLower()
                                     && emp.IsActive == true
                                     select new { emp, desig }).FirstOrDefaultAsync();

                if (EmpInfo != null
                    && 2000 <= obj.Year && obj.Year <= 3000
                    && 1 <= obj.Month && obj.Month <= 12
                    && 1 <= obj.Day && obj.Day <= DateTime.DaysInMonth((int)obj.Year, (int)obj.Month)
                    && 1 <= obj.FromHour && obj.FromHour <= 12
                    && 0 <= obj.FromMinute && obj.FromMinute <= 60
                    && (obj.FromAmPm.ToUpper() == "AM" || obj.FromAmPm.ToUpper() == "PM")
                    && 1 <= obj.ToHour && obj.ToHour <= 12
                    && 0 <= obj.ToMinute && obj.ToMinute <= 60
                    && (obj.ToAmPm.ToUpper() == "AM" || obj.ToAmPm.ToUpper() == "PM"))
                {
                    OvertimeDate = Convert.ToDateTime(obj.Year.ToString() + "-" + obj.Month + "-" + obj.Day);
                    FromTime = obj.FromHour + ":" + obj.FromMinute + " " + obj.FromAmPm;
                    ToTime = obj.ToHour + ":" + obj.ToMinute + " " + obj.ToAmPm;
                }
                else
                {
                    isValid = false;
                }

                createList.Add(new TimeEmpOverTimeUpload()
                {
                    IntAutoId = 0,
                    IntEmployeeId = EmpInfo == null ? 0 : EmpInfo.emp.IntEmployeeBasicInfoId,
                    StrEmployeeName = EmpInfo == null ? "" : EmpInfo.emp.StrEmployeeName,
                    StrEmployeeCode = obj.EmployeeCode,
                    IntBusinessUnitId = EmpInfo == null ? (long)obj.BusinessUnitId : EmpInfo.emp.IntBusinessUnitId,
                    IntDesignationId = EmpInfo == null ? 0 : EmpInfo.desig.IntDesignationId,
                    StrDesignationName = EmpInfo == null ? "" : EmpInfo.desig.StrDesignation,
                    IntYear = (long)obj.Year,
                    IntMonth = (long)obj.Month,
                    IntDay = (long)obj.Day,
                    IntFromHour = (long)obj.FromHour,
                    IntFromMinute = (long)obj.FromMinute,
                    StrFromAmPm = obj.FromAmPm,
                    IntToHour = (long)obj.ToHour,
                    IntToMinute = (long)obj.ToMinute,
                    StrToAmPm = obj.ToAmPm,
                    IsSubmitted = false,
                    IsValid = isValid,
                    DteCreatedAt = DateTime.Now,
                    StrInsertBy = obj.InsertBy,
                    StrRemarks = obj.Remarks,
                    DteOvertimeDate = OvertimeDate,
                    StrFromTime = FromTime,
                    StrToTime = ToTime
                });
            }

            await _context.TimeEmpOverTimeUploads.AddRangeAsync(createList);
            await _context.SaveChangesAsync();

            foreach (var obj in createList)
            {
                responseList.Add(new EmpOverTimeUploadDTO()
                {
                    AutoId = obj.IntAutoId,
                    EmployeeId = obj.IntEmployeeId,
                    EmployeeCode = obj.StrEmployeeCode,
                    EmployeeName = obj.StrEmployeeName,
                    EmployeeDesignationId = obj.IntDesignationId,
                    EmployeeDesignationName = obj.StrDesignationName,
                    BusinessUnitId = obj.IntBusinessUnitId,
                    Year = obj.IntYear,
                    Month = obj.IntMonth,
                    Day = obj.IntDay,
                    OnlyDate = obj.DteOvertimeDate,
                    FromHour = obj.IntFromHour,
                    FromMinute = obj.IntFromMinute,
                    FromAmPm = obj.StrFromAmPm,
                    FromTime = obj.StrFromTime,
                    ToHour = obj.IntToHour,
                    ToMinute = obj.IntToMinute,
                    ToAmPm = obj.StrToAmPm,
                    ToTime = obj.StrToTime,
                    IsSubmitted = false,
                    IsValid = obj.IsValid,
                    InsertDateTime = obj.DteCreatedAt,
                    InsertBy = obj.StrInsertBy,
                    Remarks = obj.StrRemarks
                });
            }

            return responseList;
        }

        public async Task<MessageHelperBulkUpload> CreateOvertime(List<TimeEmpOverTimeVM> objList)
        {
            MessageHelperBulkUpload msg = new MessageHelperBulkUpload();
            var list = new List<ErrorList>();

            try
            {
                var createList = new List<TimeEmpOverTime>();
                var updateList = new List<TimeEmpOverTime>();

                foreach (var item in objList)
                {
                    var EmpInfo = await (from emp in _context.EmpEmployeeBasicInfos
                                         join ds in _context.MasterDesignations on emp.IntDesignationId equals ds.IntDesignationId into dsg
                                         from desig in dsg.DefaultIfEmpty()
                                         where emp.IntEmployeeBasicInfoId == item.IntEmployeeId
                                         && emp.IsActive == true
                                         select new { emp, desig }).FirstOrDefaultAsync();


                    if (EmpInfo is not null)
                    {
                        if (item.StrDailyOrMonthly == "Daily")
                        {
                            TimeEmpOverTime getOTData = await _context.TimeEmpOverTimes.Where(x => x.IntEmployeeId == EmpInfo.emp.IntEmployeeBasicInfoId && x.IntAccountId == EmpInfo.emp.IntAccountId && x.DteOverTimeDate == item.DteOverTimeDate && x.StrDailyOrMonthly == "Daily" && x.IsActive == true).FirstOrDefaultAsync();

                            var getMonthlyOTData = await _context.TimeEmpOverTimes.Where(x => x.IntEmployeeId == EmpInfo.emp.IntEmployeeBasicInfoId && x.IntAccountId == EmpInfo.emp.IntAccountId && x.IntMonth == item.DteOverTimeDate.Value.Month && x.IntYear == item.DteOverTimeDate.Value.Year && x.StrDailyOrMonthly == "Monthly" && x.IsActive == true).ToListAsync();

                            if (getMonthlyOTData != null)
                            {
                                foreach (var items in getMonthlyOTData)
                                {
                                    items.IsActive = false;
                                    items.IntUpdatedBy = item.IntUpdatedBy;
                                    items.DteUpdatedAt = DateTime.Now;

                                    _context.TimeEmpOverTimes.Update(items);
                                    await _context.SaveChangesAsync();
                                }
                                item.IsUpdate = false;
                            }

                            if (getOTData != null)
                            {
                                getOTData.DteOverTimeDate = item.DteOverTimeDate;
                                getOTData.NumOverTimeHour = item.NumOverTimeHour;
                                getOTData.StrReason = item.StrReason;
                                getOTData.IntUpdatedBy = item.IntUpdatedBy;
                                getOTData.DteUpdatedAt = DateTime.Now;

                                _context.TimeEmpOverTimes.Update(getOTData);
                                await _context.SaveChangesAsync();

                                item.IsUpdate = true;
                            }

                            if (item.IsUpdate == false)
                            {
                                if (EmpInfo != null && item.DteOverTimeDate != null)
                                {
                                    PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId((long)item.IntAccountId, "Overtime");
                                    if (stage.HeaderId <= 0)
                                    {
                                        msg.StatusCode = 500;
                                        msg.Message = "Pipeline was not set";
                                        return msg;
                                    }

                                    createList.Add(new TimeEmpOverTime
                                    {
                                        IntEmployeeId = EmpInfo.emp.IntEmployeeBasicInfoId,
                                        IntAccountId = EmpInfo != null ? EmpInfo.emp.IntAccountId : 0,
                                        IntBusinessUnitId = EmpInfo.emp.IntBusinessUnitId,
                                        IntWorkplaceGroupId = EmpInfo.emp.IntWorkplaceGroupId,
                                        IntWorkplaceId = EmpInfo.emp.IntWorkplaceId,
                                        DteOverTimeDate = item.DteOverTimeDate,
                                        NumOverTimeHour = item.NumOverTimeHour,
                                        IntYear = item.DteOverTimeDate.Value.Year,
                                        IntMonth = item.DteOverTimeDate.Value.Month,
                                        StrReason = item.StrReason,
                                        StrDailyOrMonthly = "Daily",
                                        IsActive = item.IsActive,
                                        IntCreatedBy = item.IntCreatedBy,
                                        DteCreatedAt = DateTime.Now,
                                        IntCurrentStage = stage.CurrentStageId,
                                        IntNextStage = stage.NextStageId,
                                        StrStatus = "Pending"
                                    });
                                }
                                else
                                {
                                    list.Add(new ErrorList
                                    {
                                        Title = "UnUploadList",
                                        Body = item.EmployeeCode,
                                    });

                                    continue;
                                }
                            }
                        }
                        else if (item.StrDailyOrMonthly == "Monthly")
                        {
                            var getDailyOTData = await _context.TimeEmpOverTimes.Where(x => x.IntEmployeeId == EmpInfo.emp.IntEmployeeBasicInfoId && x.IntAccountId == EmpInfo.emp.IntAccountId && x.IntMonth == item.DteOverTimeDate.Value.Month && x.IntYear == item.DteOverTimeDate.Value.Year && x.StrDailyOrMonthly == "Daily" && x.IsActive == true).ToListAsync();

                            TimeEmpOverTime getMonthlyOTData = await _context.TimeEmpOverTimes.Where(x => x.IntEmployeeId == EmpInfo.emp.IntEmployeeBasicInfoId && x.IntAccountId == EmpInfo.emp.IntAccountId && x.IntMonth == item.DteOverTimeDate.Value.Month && x.IntYear == item.DteOverTimeDate.Value.Year && x.StrDailyOrMonthly == "Monthly" && x.IsActive == true).FirstOrDefaultAsync();

                            if (getDailyOTData != null)
                            {
                                foreach (var items in getDailyOTData)
                                {
                                    items.IsActive = false;
                                    items.IntUpdatedBy = item.IntUpdatedBy;
                                    items.DteUpdatedAt = DateTime.Now;

                                    _context.TimeEmpOverTimes.Update(items);
                                    await _context.SaveChangesAsync();

                                }
                                item.IsUpdate = false;
                            }
                            if (getMonthlyOTData != null)
                            {
                                getMonthlyOTData.NumOverTimeHour = item.NumOverTimeHour;
                                getMonthlyOTData.StrReason = item.StrReason;
                                getMonthlyOTData.IntUpdatedBy = item.IntUpdatedBy;
                                getMonthlyOTData.DteUpdatedAt = DateTime.Now;

                                _context.TimeEmpOverTimes.Update(getMonthlyOTData);
                                await _context.SaveChangesAsync();

                                item.IsUpdate = true;
                            }
                            if (item.IsUpdate == false)
                            {
                                if (EmpInfo != null)
                                {
                                    PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId((long)item.IntAccountId, "Overtime");
                                    if (stage.HeaderId <= 0)
                                    {
                                        msg.StatusCode = 500;
                                        msg.Message = "Pipeline was not set";
                                        return msg;
                                    }
                                    createList.Add(new TimeEmpOverTime
                                    {
                                        IntEmployeeId = EmpInfo.emp.IntEmployeeBasicInfoId,
                                        IntAccountId = EmpInfo != null ? EmpInfo.emp.IntAccountId : 0,
                                        IntBusinessUnitId = EmpInfo.emp.IntBusinessUnitId,
                                        IntWorkplaceGroupId = EmpInfo.emp.IntWorkplaceGroupId,
                                        IntWorkplaceId = EmpInfo.emp.IntWorkplaceId,
                                        IntYear = item.DteOverTimeDate.Value.Year,
                                        IntMonth = item.DteOverTimeDate.Value.Month,
                                        DteOverTimeDate = item.DteOverTimeDate,
                                        TmeStartTime = item.TmeStartTime,
                                        TmeEndTime = item.TmeEndTime,
                                        NumOverTimeHour = item.NumOverTimeHour,
                                        StrReason = item.StrReason,
                                        StrDailyOrMonthly = "Monthly",
                                        IsActive = item.IsActive,
                                        IntCreatedBy = item.IntCreatedBy,
                                        DteCreatedAt = DateTime.Now,
                                        IntCurrentStage = stage.CurrentStageId,
                                        IntNextStage = stage.NextStageId,
                                        StrStatus = "Pending"
                                    });
                                }
                                else
                                {
                                    list.Add(new ErrorList
                                    {
                                        Title = "UnUploadList",
                                        Body = item.EmployeeCode,
                                    });

                                    continue;
                                }
                            }
                        }
                    }
                }
                if (createList.Count > 0)
                {
                    await _context.TimeEmpOverTimes.AddRangeAsync(createList);
                    await _context.SaveChangesAsync();

                    msg.StatusCode = 200;
                    msg.Message = "Successfully Create!";
                }
                else
                {
                    msg.StatusCode = 200;
                    msg.Message = "Successfully Updated!";
                }
                if (list.Count > 0)
                {
                    msg.ListData = list;
                }

                return msg;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<CustomMessageHelper> SubmitOvertimeUpload(List<EmpOverTimeUploadDTO> objList)
        {
            var updateTblList = new List<TimeEmpOverTimeUpload>();
            var insertTblList = new List<EmpOverTimeUploadDTO>();

            foreach (var obj in objList)
            {
                var updateItem = await (_context.TimeEmpOverTimeUploads.Where(x => x.IntAutoId == obj.AutoId && x.IsValid == true).FirstOrDefaultAsync());
                if (updateItem != null)
                {
                    updateItem.IsSubmitted = true;
                    updateTblList.Add(updateItem);

                    insertTblList.Add(new EmpOverTimeUploadDTO()
                    {
                        AutoId = obj.AutoId,
                        EmployeeId = obj.EmployeeId,
                        EmployeeCode = obj.EmployeeCode,
                        EmployeeName = obj.EmployeeName,
                        EmployeeDesignationId = obj.EmployeeDesignationId,
                        EmployeeDesignationName = obj.EmployeeDesignationName,
                        BusinessUnitId = obj.BusinessUnitId,
                        Year = obj.Year,
                        Month = obj.Month,
                        Day = obj.Day,
                        OnlyDate = obj.OnlyDate,
                        FromHour = obj.FromHour,
                        FromMinute = obj.FromMinute,
                        FromAmPm = obj.FromAmPm,
                        FromTime = obj.FromTime,
                        ToHour = obj.ToHour,
                        ToMinute = obj.ToMinute,
                        ToAmPm = obj.ToAmPm,
                        ToTime = obj.ToTime,
                        OvertimeHour = obj.OvertimeHour,
                        IsSubmitted = obj.IsSubmitted,
                        IsValid = obj.IsValid,
                        InsertDateTime = obj.InsertDateTime,
                        InsertBy = obj.InsertBy,
                        Remarks = obj.Remarks
                    });
                }
            }
            if (updateTblList.Count > 0)
            {
                _context.TimeEmpOverTimeUploads.UpdateRange(updateTblList);
                await _context.SaveChangesAsync();
            }

            var objNewCreate = new TimeSheetViewModel()
            {
                PartType = "OvertimeBulkEntry",
                objOvertimeBulkUpload = insertTblList
            };
            var res = await TimeSheetCRUD(objNewCreate);

            return res;
        }

        #endregion ========== Time Sheet CRUD ==============

        #region ========== Report ==========

        public async Task<List<AttendanceDailySummaryViewModel>> GetAttendanceSummaryCalenderViewReport(long EmployeeId, long Month, long Year)
        {
            try
            {
                List<AttendanceDailySummaryViewModel> objList = new List<AttendanceDailySummaryViewModel>();
                objList = await _context.TimeAttendanceDailySummaries
                            .Where(x => x.IntEmployeeId == EmployeeId && x.IntMonthId == Month && x.IntYear == Year)
                            .Select(x => new AttendanceDailySummaryViewModel
                            {
                                DayName = DateTime.Parse($"{x.IntDayId}/{Month}/{Year}").DayOfWeek.ToString(),
                                DayNumber = (int)x.IntDayId,
                                presentStatus = DateTime.UtcNow.Date < DateTime.Parse($"{x.IntDayId}/{Month}/{Year}").Date ? "-" : x.IsPresent == true ? "Present" : x.IsLate == true ? "Late" : x.IsAbsent == true ? "Absent" : x.IsLeave == true ? "Leave" : x.IsMovement == true ? "Movement" : x.IsHoliday == true ? "Holiday" : x.IsOffday == true ? "Offday" : "Absent"
                            }).ToListAsync();

                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<EmployeeDaylyAttendanceReportLanding> GetDateWiseAttendanceReport(long IntAccountId, long IntBusinessUnitId, long? IntWorkplaceGroupId, DateTime attendanceDate, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                IQueryable<EmpAttendanceSummaryListVM> data = (from emp in _context.EmpEmployeeBasicInfos
                                                               join att in _context.TimeAttendanceDailySummaries on emp.IntEmployeeBasicInfoId equals att.IntEmployeeId into attGroup
                                                               from att in attGroup.DefaultIfEmpty()
                                                               join empDtl in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empDtl.IntEmployeeId
                                                               join desi in _context.MasterDesignations on emp.IntDesignationId equals desi.IntDesignationId into desig2
                                                               from desi in desig2.DefaultIfEmpty()
                                                               join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                                               from dept in dept2.DefaultIfEmpty()
                                                               where emp.IntAccountId == IntAccountId && emp.IsActive == true
                                                               && (att.DteAttendanceDate.Value.Date == attendanceDate.Date)
                                                               && (empDtl.IntEmployeeStatusId == 1 || empDtl.IntEmployeeStatusId == 4)
                                                               && (emp.IntBusinessUnitId == IntBusinessUnitId) && (emp.IntWorkplaceGroupId == IntWorkplaceGroupId)
                                                               select new EmpAttendanceSummaryListVM
                                                               {
                                                                   EmployeeId = emp.IntEmployeeBasicInfoId,
                                                                   EmployeeCode = emp.StrEmployeeCode,
                                                                   EmployeeName = emp.StrEmployeeName,
                                                                   Department = dept.StrDepartment ?? "",
                                                                   Designation = desi.StrDesignation ?? "",
                                                                   EmploymentType = emp.StrEmploymentType,
                                                                   DepartmentId = emp.IntDepartmentId,
                                                                   DutyHours = att.StrWorkingHours ?? "",
                                                                   InTime = att.TmeInTime,
                                                                   OutTime = att.TmeLastOutTime,
                                                                   ActualStatus = att.IsPresent == true ? "Present" : att.IsLate == true ? "Late" :
                                                                                   att.IsAbsent == true ? "Absent" :
                                                                                   att.IsMovement == true ? "Movement" :
                                                                                   att.IsLeave == true ? "Leave" :
                                                                                   att.IsHoliday == true ? "Holiday" :
                                                                                   att.IsOffday == true ? "Offday" :
                                                                                   att.IsLeaveWithPay == true ? "Leave with pay" :
                                                                                   "Absent",
                                                                   ManualStatus = att.IsManual == true && att.IsManualPresent == true ? "Present" :
                                                                                     att.IsManual == true && att.IsManualLate == true ? "Late" :
                                                                                     att.IsManual == true && att.IsManualAbsent == true ? "Absent" :
                                                                                     att.IsManual == true && att.IsManualLeave == true ? "Leave" :
                                                                                     "N/A",
                                                                   Present = att.IsPresent == true ? true : (att.IsManual == true && att.IsManualPresent == true) ? true : false,
                                                                   Late = att.IsLate == true ? true : (att.IsManual == true && att.IsManualLate == true) ? true : false,
                                                                   Absent = att.IsAbsent == true ? true : (att.IsManual == true && att.IsManualAbsent == true) ? true : (att.IsPresent == false && att.IsLate == false && att.IsLeave == false && att.IsMovement == false && att.IsHoliday == false && att.IsOffday == false) ? true : false,
                                                                   Leave = att.IsLeave == true ? true : (att.IsManual == true && att.IsManualLeave == true) ? true : false,
                                                                   Movement = att.IsMovement ?? false,
                                                                   Holiday = att.IsHoliday ?? false,
                                                                   Weekend = att.IsOffday ?? false,
                                                                   CalendarName = att.StrCalendarName ?? "N/A",
                                                                   Remarks = att.IsManual == true ?
                                                                               (from manAtt in _context.EmpManualAttendanceSummaries
                                                                                where manAtt.DteAttendanceDate.Value.Date == att.DteAttendanceDate.Value.Date &&
                                                                                                 manAtt.IntEmployeeId == emp.IntEmployeeBasicInfoId &&
                                                                                                 manAtt.IsActive == true &&
                                                                                                 manAtt.IsPipelineClosed == true &&
                                                                                                 manAtt.IsReject == false
                                                                                select manAtt.StrRemarks).FirstOrDefault() :
                                                                               "N/A",
                                                               }).AsNoTracking()
                                                               .OrderBy(a=> a.DepartmentId).ThenBy(a=>a.EmployeeName).AsQueryable();

                var attCountList = await data.ToListAsync();
                IList<DepartmentVM> deptList = new List<DepartmentVM>();
                deptList = data
                    .GroupBy(x => new { x.DepartmentId, x.Department })
                    .Select(d => new DepartmentVM { deptId = d.Key.DepartmentId, deptName = d.Key.Department }).OrderBy(a => a.deptId).ToList();

                EmployeeDaylyAttendanceReportLanding retObj = new EmployeeDaylyAttendanceReportLanding();
                retObj.AttendanceDate = attendanceDate.Date.ToString("dd MMM, yyyy");
                retObj.departmentVM = deptList;
                retObj.TotalEmployee = data.Count();
                retObj.PresentCount = attCountList.Where(a => a.Present == true).Count();
                retObj.AbsentCount = attCountList.Where(a => a.Absent == true).Count();
                retObj.LateCount = attCountList.Where(a => a.Late == true).Count();
                retObj.LeaveCount = attCountList.Where(a => a.Leave == true).Count();
                retObj.MovementCount = attCountList.Where(a => a.Movement == true).Count();
                retObj.WeekendCount = attCountList.Where(a => a.Weekend == true).Count();
                retObj.HolidayCount = attCountList.Where(a => a.Holiday == true).Count();

                if (IsXls)
                {
                    retObj.Data = attCountList;
                }
                else
                {
                    if(PageNo == 0 &&  PageSize == 0)
                    {
                        MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == IntBusinessUnitId && x.IntAccountId == IntAccountId && x.IsActive == true).FirstOrDefaultAsync();
                        retObj.WorkplaceGroup= await _context.MasterWorkplaceGroups.Where(x => x.IntWorkplaceGroupId == IntWorkplaceGroupId && x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => x.StrWorkplaceGroup).FirstOrDefaultAsync();
                        //retObj.Workplace= await _context.MasterWorkplaces.Where(x => x.IntWorkplaceId == IntWorkplaceId && x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => x.StrWorkplace).FirstOrDefaultAsync();
                        retObj.Data = attCountList;
                        retObj.TotalCount = await data.CountAsync();
                        if (businessUnit != null)
                        {
                            retObj.CompanyAddress = businessUnit.StrAddress == null ? "" : businessUnit.StrAddress;
                            retObj.CompanyLogoUrlId = businessUnit.StrLogoUrlId;
                            retObj.BusinessUnitName = businessUnit.StrBusinessUnit;
                        }
                       
                        retObj.BusinessUnitId = IntBusinessUnitId;
                        return retObj;
                    }
                    else
                    {
                        int maxSize = 1000;
                        PageSize = PageSize > maxSize ? maxSize : PageSize;
                        PageNo = PageNo < 1 ? 1 : PageNo;

                        if (!string.IsNullOrEmpty(searchTxt))
                        {
                            searchTxt = searchTxt.ToLower();
                            data = data.Where(x => x.EmployeeName.ToLower().Contains(searchTxt) || x.Designation.ToLower().Contains(searchTxt) || x.EmployeeCode.ToLower().Contains(searchTxt)
                            || x.Department.ToLower().Contains(searchTxt));
                        }

                        retObj.TotalCount = await data.CountAsync();
                        retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                        retObj.PageSize = (long)PageSize;
                        retObj.CurrentPage = (long)PageNo;

                    }
                   
                }

                return retObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DailyAttendanceReportVM> DailyAttendanceReport(long IntAccountId, DateTime AttendanceDate, long IntBusinessUnitId, long? IntWorkplaceGroupId, long? IntWorkplaceId, long? IntDepartmentId, string? EmployeeIdList, string? SearchTxt, int? PageNo, int? PageSize, bool IsPaginated)
        {
            try
            {
                DataTable dt = new DataTable();

                string? EmpIdList = EmployeeIdList == null ? null : EmployeeIdList.Replace("[", "").Replace("]", "");

                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprDailyAttendanceReport";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@IntAccountId", IntAccountId);
                sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", IntBusinessUnitId);
                sqlCmd.Parameters.AddWithValue("@AttendanceDate", AttendanceDate);
                sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", IntWorkplaceGroupId);
                sqlCmd.Parameters.AddWithValue("@IntWorkplaceId", IntWorkplaceId);
                sqlCmd.Parameters.AddWithValue("@IntDepartmentId", IntDepartmentId);
                sqlCmd.Parameters.AddWithValue("@EmployeeIdList", EmpIdList);
                sqlCmd.Parameters.AddWithValue("@strSearchTxt", SearchTxt);
                sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
                sqlCmd.Parameters.AddWithValue("@isPaginated", IsPaginated);
                connection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                sqlAdapter.Fill(dt);
                connection.Close();


                List<EmployeeAttendanceSummaryViewModel> EmployeeAttendanceSummaryList = new List<EmployeeAttendanceSummaryViewModel>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        EmployeeAttendanceSummaryViewModel EmployeeAttendanceSummary = new EmployeeAttendanceSummaryViewModel();

                        EmployeeAttendanceSummary.EmployeeId = row["intEmployeeBasicInfoId"].ToString();
                        EmployeeAttendanceSummary.EmployeeName = row["strEmployeeName"].ToString();
                        EmployeeAttendanceSummary.EmployeeCode = row["strEmployeeCode"].ToString();
                        EmployeeAttendanceSummary.Department = row["strDepartment"].ToString();
                        EmployeeAttendanceSummary.Designation = row["strDesignation"].ToString();
                        EmployeeAttendanceSummary.EmploymentType = row["strEmploymentType"].ToString();
                        EmployeeAttendanceSummary.InTime = row["inTime"].ToString();
                        EmployeeAttendanceSummary.OutTime = row["outTime"].ToString();
                        EmployeeAttendanceSummary.DutyHours = row["DutyHours"].ToString();
                        EmployeeAttendanceSummary.ActualStatus = row["ActualAttendanceStatus"].ToString();
                        EmployeeAttendanceSummary.ManualStatus = row["ManualAttendanceStatus"].ToString();
                        EmployeeAttendanceSummary.Present = Convert.ToBoolean(row["isPresent"]);
                        EmployeeAttendanceSummary.Late = Convert.ToBoolean(row["isLate"]);
                        EmployeeAttendanceSummary.Absent = Convert.ToBoolean(row["isAbsent"]);
                        EmployeeAttendanceSummary.Leave = Convert.ToBoolean(row["isLeave"]);
                        EmployeeAttendanceSummary.Movement = Convert.ToBoolean(row["isMovement"]);
                        EmployeeAttendanceSummary.Holiday = Convert.ToBoolean(row["isHoliday"]);
                        EmployeeAttendanceSummary.Location = row["strLocation"].ToString();
                        EmployeeAttendanceSummary.CalendarName = row["CalendarName"].ToString();
                        EmployeeAttendanceSummary.Remarks = row["strRemarks"].ToString();
                        EmployeeAttendanceSummary.TotalCount = Convert.ToInt64(row["totalCount"]);

                        EmployeeAttendanceSummaryList.Add(EmployeeAttendanceSummary);
                    }
                }

                MasterBusinessUnit businessUnit = await _context.MasterBusinessUnits.Where(x => x.IntBusinessUnitId == IntBusinessUnitId && x.IntAccountId == IntAccountId && x.IsActive == true).FirstOrDefaultAsync();

                DailyAttendanceReportVM dailyAttendanceReport = new DailyAttendanceReportVM();
                dailyAttendanceReport.EmployeeAttendanceSummaryVM = EmployeeAttendanceSummaryList;
                dailyAttendanceReport.AttendanceDate = AttendanceDate;
                dailyAttendanceReport.BusinessUnitId = IntBusinessUnitId;
                dailyAttendanceReport.CompanyAddress = businessUnit.StrAddress;
                dailyAttendanceReport.CompanyLogoUrlId = businessUnit.StrLogoUrlId;
                dailyAttendanceReport.BusinessUnit = businessUnit.StrBusinessUnit;
                dailyAttendanceReport.WorkplaceGroup = await _context.MasterWorkplaceGroups.Where(x => x.IntWorkplaceGroupId == IntWorkplaceGroupId && x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => x.StrWorkplaceGroup).FirstOrDefaultAsync();
                dailyAttendanceReport.Workplace = await _context.MasterWorkplaces.Where(x => x.IntWorkplaceId == IntWorkplaceId && x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => x.StrWorkplace).FirstOrDefaultAsync();
                dailyAttendanceReport.Department = await _context.MasterDepartments.Where(x => x.IntDepartmentId == IntDepartmentId && x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => x.StrDepartment).FirstOrDefaultAsync();
                dailyAttendanceReport.TotalEmployee = EmployeeAttendanceSummaryList.Select(x => x.EmployeeName).Count();
                dailyAttendanceReport.PresentCount = EmployeeAttendanceSummaryList.Where(x => x.Present == true).Count();
                dailyAttendanceReport.ManualPresentCount = EmployeeAttendanceSummaryList.Where(x => x.ManualStatus.ToLower() == "Present".ToLower()).Count();
                dailyAttendanceReport.LateCount = EmployeeAttendanceSummaryList.Where(x => x.Late == true).Count();
                dailyAttendanceReport.AbsentCount = EmployeeAttendanceSummaryList.Where(x => x.Absent == true).Count();
                dailyAttendanceReport.MovementCount = EmployeeAttendanceSummaryList.Where(x => x.Movement == true).Count();
                dailyAttendanceReport.LeaveCount = EmployeeAttendanceSummaryList.Where(x => x.Leave == true).Count();
                dailyAttendanceReport.HolidayCount = EmployeeAttendanceSummaryList.Where(x => x.Holiday == true).Count();
                dailyAttendanceReport.WeekendCount = EmployeeAttendanceSummaryList.Where(x => x.ActualStatus.ToLower() == "Offday".ToLower()).Count();

                return dailyAttendanceReport;
            }
            catch (Exception ex)
            {
                return new DailyAttendanceReportVM();
            }
        }

        public async Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId,
            DateTime? FromDate, DateTime? ToDate, long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount,
            long? MovementTypeId, DateTime? ApplicationDate, long? LoggedEmployeeId, int? MonthId, long? YearId, long? WorkplaceGroupId, string? SearchTxt, int? PageNo, int? PageSize)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprAllLandingPageData";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strTableName", TableName);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intId", intId);
                        sqlCmd.Parameters.AddWithValue("@intStatusId", intStatusId);
                        sqlCmd.Parameters.AddWithValue("@jsonFilter", "");
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate);
                        sqlCmd.Parameters.AddWithValue("@LoanTypeId", LoanTypeId);
                        sqlCmd.Parameters.AddWithValue("@DeptId", DeptId);
                        sqlCmd.Parameters.AddWithValue("@DesigId", DesigId);
                        sqlCmd.Parameters.AddWithValue("@EmpId", EmpId);
                        sqlCmd.Parameters.AddWithValue("@MinimumAmount", MinimumAmount);
                        sqlCmd.Parameters.AddWithValue("@MaximumAmount", MaximumAmount);
                        sqlCmd.Parameters.AddWithValue("@MovementTypeId", MovementTypeId);
                        sqlCmd.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@intLoggedEmployeeId", LoggedEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@IntMonthId", MonthId);
                        sqlCmd.Parameters.AddWithValue("@IntYearId", YearId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", SearchTxt);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", PageSize);
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

        public async Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllList(long AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? EmployeeId)
        {
            List<EmployeeQryProfileAllViewModel> data = await (from emp in _context.EmpEmployeeBasicInfos
                                                               join d in _context.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dd
                                                               from dept in dd.DefaultIfEmpty()
                                                               join de in _context.MasterDesignations on emp.IntDesignationId equals de.IntDesignationId into dess
                                                               from des in dess.DefaultIfEmpty()
                                                               join s in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals s.IntEmployeeBasicInfoId into ss
                                                               from sup in ss.DefaultIfEmpty()
                                                               join l in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals l.IntEmployeeBasicInfoId into ll
                                                               from man in ll.DefaultIfEmpty()
                                                               join et in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals et.IntEmploymentTypeId into ett
                                                               from empT in ett.DefaultIfEmpty()
                                                               join b in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals b.IntBusinessUnitId into bb
                                                               from bus in bb.DefaultIfEmpty()
                                                               join w in _context.MasterWorkplaceGroups on emp.IntWorkplaceId equals w.IntWorkplaceGroupId into ww
                                                               from wpg in ww.DefaultIfEmpty()
                                                               where emp.IsActive == true &&
                                                               emp.IntAccountId == AccountId &&
                                                               (BusinessUnitId > 0 ? emp.IntBusinessUnitId == BusinessUnitId : true) &&
                                                               (WorkplaceGroupId > 0 ? emp.IntWorkplaceId == WorkplaceGroupId : true) &&
                                                               (DeptId > 0 ? emp.IntDepartmentId == DeptId : true) &&
                                                               (DesigId > 0 ? emp.IntDesignationId == DesigId : true) &&
                                                               (EmployeeId > 0 ? emp.IntEmployeeBasicInfoId == EmployeeId : true)
                                                               select new EmployeeQryProfileAllViewModel
                                                               {
                                                                   EmployeeBasicInfo = emp,
                                                                   BusinessUnit = bus,
                                                                   DepartmentId = dept.IntDepartmentId,
                                                                   DepartmentName = dept.StrDepartment,
                                                                   DesignationId = des.IntDesignationId,
                                                                   DesignationName = des.StrDesignation,
                                                                   SupervisorId = sup.IntEmployeeBasicInfoId,
                                                                   SupervisorName = sup.StrEmployeeName,
                                                                   LineManagerId = man.IntEmployeeBasicInfoId,
                                                                   LineManagerName = man.StrEmployeeName,
                                                                   EmploymentTypeId = emp.IntEmploymentTypeId,
                                                                   EmploymentTypeName = empT.StrEmploymentType,
                                                                   WorkplaceGroupId = wpg.IntWorkplaceGroupId,
                                                                   WorkplaceGroupName = wpg.StrWorkplaceGroup
                                                               }).OrderByDescending(x => x.EmployeeBasicInfo.IntEmployeeBasicInfoId).AsNoTracking().ToListAsync();

            return data;
        }

        //public async Task<DataTable> AllEmployeeListWithFilter(AllEmployeeListWithFilterViewModel objFilter)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
        //        {
        //            string sql = "saas.sprAllLandingPageData";
        //            string jsonString = JsonSerializer.Serialize(objFilter);
        //            using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
        //            {
        //                sqlCmd.CommandType = CommandType.StoredProcedure;
        //                sqlCmd.Parameters.AddWithValue("@strTableName", objFilter.PartName);
        //                sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);

        //                sqlCmd.Parameters.AddWithValue("@intAccountId", null);
        //                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", null);
        //                sqlCmd.Parameters.AddWithValue("@intId", null);
        //                sqlCmd.Parameters.AddWithValue("@intStatusId", null);
        //                sqlCmd.Parameters.AddWithValue("@dteFromDate", null);
        //                sqlCmd.Parameters.AddWithValue("@dteToDate", null);
        //                sqlCmd.Parameters.AddWithValue("@LoanTypeId", null);
        //                sqlCmd.Parameters.AddWithValue("@DeptId", null);
        //                sqlCmd.Parameters.AddWithValue("@DesigId", null);
        //                sqlCmd.Parameters.AddWithValue("@EmpId", null);
        //                sqlCmd.Parameters.AddWithValue("@MinimumAmount", null);
        //                sqlCmd.Parameters.AddWithValue("@MaximumAmount", null);
        //                sqlCmd.Parameters.AddWithValue("@MovementTypeId", null);
        //                sqlCmd.Parameters.AddWithValue("@ApplicationDate", null);
        //                //sqlCmd.Parameters.AddWithValue("@StatusId", null);
        //                sqlCmd.Parameters.AddWithValue("@intLoggedEmployeeId", null);
        //                connection.Open();

        //                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
        //                {
        //                    sqlAdapter.Fill(dt);
        //                }
        //                connection.Close();
        //            }
        //        }

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataTable();
        //    }
        //}

        public async Task<dynamic> EmployeeReportWithFilter(BaseVM tokenData, long businessUnitId, long workplaceGroupId, bool IsPaginated, string? searchTxt, int pageSize, int pageNo, bool IsHeaderNeed,
                    List<long> departments, List<long> designations, List<long> supervisors, List<long> employeementType, List<long> LineManagers, List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                searchTxt = !string.IsNullOrEmpty(searchTxt) ? searchTxt.ToLower() : searchTxt;

                var query = (from emp in _context.EmpEmployeeBasicInfos
                             join empD in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD.IntEmployeeId into empD2
                             from empD in empD2.DefaultIfEmpty()
                             join deptt in _context.MasterDepartments on emp.IntDepartmentId equals deptt.IntDepartmentId into deptt2
                             from dept in deptt2.DefaultIfEmpty()
                             join dess in _context.MasterDesignations on emp.IntDesignationId equals dess.IntDesignationId into dess2
                             from des in dess2.DefaultIfEmpty()
                             join bnk in _context.EmpEmployeeBankDetails on new { empId = emp.IntEmployeeBasicInfoId, isPrimarySalAcc = true, isActive = true } equals new { empId = bnk.IntEmployeeBasicInfoId, isPrimarySalAcc = bnk.IsPrimarySalaryAccount, isActive = (bool)bnk.IsActive } into empBnk
                             from bank in empBnk.DefaultIfEmpty()
                             join sal in _context.PyrEmployeeSalaryElementAssignHeaders on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = sal.IntEmployeeId, isActive = (bool)sal.IsActive } into empSal
                             from salary in empSal.DefaultIfEmpty()
                             join supervisor in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals supervisor.IntEmployeeBasicInfoId into empSupervisor
                             from sup in empSupervisor.DefaultIfEmpty()
                             join lm in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals lm.IntEmployeeBasicInfoId into lm2
                             from lm in lm2.DefaultIfEmpty()

                             join bus in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus.IntBusinessUnitId into empBusinessUnit
                             from businessUnit in empBusinessUnit.DefaultIfEmpty()
                             join w in _context.MasterWorkplaces on emp.IntWorkplaceId equals w.IntWorkplaceId into empWorkplace
                             from workplace in empWorkplace.DefaultIfEmpty()
                             join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into empWorkplaceGroup
                             from workplaceGroup in empWorkplaceGroup.DefaultIfEmpty()
                             join father in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId,  relId = (long)1, nomId = (long)3, IsActive = true } equals new { empId = father.IntEmployeeBasicInfoId, relId = father.IntRelationShipId, nomId = (long)father.IntGrantorNomineeTypeId, IsActive = (bool)father.IsActive } into empFather
                             from fatherContact in empFather.DefaultIfEmpty()
                             join mother in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId, relId = (long)2, nomId = (long)3, IsActive = true } equals new { empId = mother.IntEmployeeBasicInfoId, relId = mother.IntRelationShipId, nomId = (long)mother.IntGrantorNomineeTypeId, IsActive = (bool)mother.IsActive } into empMother
                             from motherContact in empMother.DefaultIfEmpty()
                             join presAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)1, IsActive = true } equals new { empId = presAdd.IntEmployeeBasicInfoId, aType = presAdd.IntAddressTypeId, IsActive = (bool)presAdd.IsActive } into empPresentAddress
                             from presentAddress in empPresentAddress.DefaultIfEmpty()
                             join permAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)2, IsActive = true } equals new { empId = permAdd.IntEmployeeBasicInfoId, aType = permAdd.IntAddressTypeId, IsActive = (bool)permAdd.IsActive } into empPermanentAddress
                             from permanentAddress in empPermanentAddress.DefaultIfEmpty()
                             join oth in _context.EmpEmployeePhotoIdentities on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = oth.IntEmployeeBasicInfoId, isActive = (bool)oth.IsActive } into empOther
                             from other in empOther.DefaultIfEmpty()

                             join wi in _context.TerritorySetups on empD.IntWingId equals wi.IntTerritoryId into wi1
                             from wing in wi1.DefaultIfEmpty()
                             join soleD in _context.TerritorySetups on empD.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                             from soleDp in soleD1.DefaultIfEmpty()
                             join regn in _context.TerritorySetups on empD.IntRegionId equals regn.IntTerritoryId into regn1
                             from region in regn1.DefaultIfEmpty()
                             join area1 in _context.TerritorySetups on empD.IntAreaId equals area1.IntTerritoryId into area2
                             from area in area2.DefaultIfEmpty()
                             join terrty in _context.TerritorySetups on empD.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                             from Territory in terrty1.DefaultIfEmpty()

                             where emp.IntAccountId == tokenData.accountId && (empD.IntEmployeeStatusId == 1 || empD.IntEmployeeStatusId == 4) && emp.IsActive == true
                             && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                             && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                            && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                            : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                            : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                            : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                            : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                            : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                            : tokenData.territoryList.Contains(empD.IntTerritoryId))
                            || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                             && (!string.IsNullOrEmpty(searchTxt) ? (emp.StrEmployeeName.ToLower().Contains(searchTxt) || emp.StrEmployeeCode.ToLower().Contains(searchTxt)
                                 || emp.StrReferenceId.ToLower().Contains(searchTxt) || des.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                                 || empD.StrPinNo.ToLower().Contains(searchTxt) || empD.StrPersonalMobile.ToLower().Contains(searchTxt)
                                 || fatherContact.StrRelativesName.ToLower().Contains(searchTxt) || wing.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || soleDp.StrTerritoryName.ToLower().Contains(searchTxt) || region.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || area.StrTerritoryName.ToLower().Contains(searchTxt) || Territory.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || motherContact.StrRelativesName.ToLower().Contains(searchTxt) || empD.StrPersonalMobile.ToLower().Contains(searchTxt)
                                 || other.StrNid.ToLower().Contains(searchTxt) || bank.StrAccountNo.ToLower().Contains(searchTxt)
                                 || bank.StrRoutingNo.ToLower().Contains(searchTxt) || emp.StrReligion.ToLower().Contains(searchTxt))
                                 : true)

                             && ((departments.Count > 0 ? departments.Contains((long)emp.IntDepartmentId) : true)
                                && (designations.Count > 0 ? designations.Contains((long)emp.IntDesignationId) : true)
                                && (supervisors.Count > 0 ? supervisors.Contains((long)emp.IntSupervisorId) : true)
                                && (LineManagers.Count > 0 ? LineManagers.Contains((long)emp.IntLineManagerId) : true)
                                && (employeementType.Count > 0 ? employeementType.Contains((long)emp.IntEmploymentTypeId) : true)
                                && (WingList.Count > 0 ? WingList.Contains((long)empD.IntWingId) : true)
                                && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)empD.IntSoleDepo) : true)
                                && (RegionList.Count > 0 ? RegionList.Contains((long)empD.IntRegionId) : true)
                                && (AreaList.Count > 0 ? AreaList.Contains((long)empD.IntAreaId) : true)
                                && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)empD.IntTerritoryId) : true))


                             orderby emp.IntEmployeeBasicInfoId ascending
                             select new
                             {
                                 IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                 EmployeeName = emp.StrEmployeeName,
                                 intBusinessUnitId = emp.IntBusinessUnitId,
                                 strBusinessUnit = businessUnit == null ? "" : businessUnit.StrBusinessUnit,
                                 intWorkplaceGroupId = emp.IntWorkplaceGroupId,
                                 strWorkplaceGroup = workplaceGroup == null ? "" : workplaceGroup.StrWorkplaceGroup,
                                 //intWorkplaceId = workplace.IntWorkplaceId,
                                 //strWorkplace = workplace.StrWorkplace,
                                 IntDesignationId = emp.IntDesignationId,
                                 StrDesignation = des.StrDesignation,
                                 IntDepartmentId = emp.IntDepartmentId,
                                 StrDepartment = dept.StrDepartment,
                                 DateOfJoining = emp.DteJoiningDate,
                                 PayrollGroup = salary == null ? "" : salary.StrSalaryBreakdownHeaderTitle,
                                 DateOfConfirmation = emp.DteConfirmationDate,
                                 FatherName = fatherContact == null ? "" : fatherContact.StrRelativesName,
                                 MotherName = motherContact == null ? "" : motherContact.StrRelativesName,
                                 presentAddress = (presentAddress.StrAddressDetails==null?"":"Village: "+ presentAddress.StrAddressDetails + ", ")+
                                                  (presentAddress.StrPostOffice == null ? "" : "Post Office: " + presentAddress.StrPostOffice + (presentAddress.StrZipOrPostCode == null ? "": "-"+ presentAddress.StrZipOrPostCode) + ", ")+ //(presentAddress.StrZipOrPostCode == null ? "" : "Post Code: " + presentAddress.StrZipOrPostCode + ", ")+ 
                                                  (presentAddress.StrThana==null?"":"Thana: "+ presentAddress.StrThana + ", ")+
                                                  (presentAddress.StrDistrictOrState==null?"":"District: "+ presentAddress.StrDistrictOrState),

                                 //PresentAddress = presentAddress == null ? "" : presentAddress.StrAddressDetails,
                                 PermanentAddress = (permanentAddress.StrAddressDetails == null ? "" : "Village: "+ permanentAddress.StrAddressDetails + ", ")+
                                                    (permanentAddress.StrPostOffice == null ? "" : "Post Office: " + permanentAddress.StrPostOffice + (permanentAddress.StrZipOrPostCode == null ? "" : "-" + permanentAddress.StrZipOrPostCode) + ", ") +                                                    
                                                    (permanentAddress.StrThana == null ? "" : "Thana: " + permanentAddress.StrThana + ", ")+
                                                    (permanentAddress.StrDistrictOrState == null ? "" : "District: " + permanentAddress.StrDistrictOrState),
                                 //PermanentAddress = permanentAddress == null ? "" : permanentAddress.StrAddressDetails,

                                 DateOfBirth = emp.DteDateOfBirth,
                                 Religion = emp.StrReligion,
                                 Gender = emp.StrGender,
                                 MaritialStatus = emp.StrMaritalStatus,
                                 BloodGroup = emp.StrBloodGroup,
                                 MobileNo = empD.StrOfficeMobile,
                                 NID = other == null ? "" : other.StrNid,
                                 BirthID = other == null ? "" : other.StrBirthId,
                                 StrVehicleNo = empD.StrVehicleNo,
                                 StrRemarks = empD.StrRemarks,
                                 StrPersonalMobile = empD.StrPersonalMobile,
                                 WingId = empD.IntWingId,
                                 WingName = wing == null ? "" : wing.StrTerritoryName,
                                 SoleDepoId = empD.IntSoleDepo,
                                 SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                 RegionId = empD.IntRegionId,
                                 RegionName = region == null ? "" : region.StrTerritoryName,
                                 AreaId = empD.IntAreaId,
                                 AreaName = area == null ? "" : area.StrTerritoryName,
                                 TerritoryId = empD.IntTerritoryId,
                                 TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                 GrossSalary = salary == null ? 0 : salary.NumGrossSalary,
                                 TotalSalary = salary == null ? 0 : salary.NumGrossSalary,
                                 EmpStatus = emp.IsActive == true ? "Active" : "Inactive",
                                 EmployeeCode = emp.StrEmployeeCode,
                                 intEmploymentTypeId = emp.IntEmploymentTypeId,
                                 StrEmploymentType = emp.StrEmploymentType,
                                 Email = empD.StrPersonalMail,
                                 OfficeEmail = empD.StrOfficeMail,
                                 IntSupervisorId = emp.IntSupervisorId,
                                 StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                 IntLineManagerId = emp.IntLineManagerId,
                                 StrLinemanager = lm == null ? "" : lm.StrEmployeeName,
                                 BankName = bank == null ? "" : bank.StrBankWalletName,
                                 BranchName = bank == null ? "" : bank.StrBranchName,
                                 AccountNo = bank == null ? "" : bank.StrAccountNo,
                                 RoutingNo = bank == null ? "" : bank.StrRoutingNo,
                                 StrPinNo = empD.StrPinNo
                             });

                EmployeeProfileLandingPaginationViewModelWithHeader employeeReport = new();

                if (IsHeaderNeed)
                {
                    EmployeeHeader eh = new();

                    var datas = query.Select(x => new
                    {
                        WingId = x.WingId,
                        WingName = x.WingName,
                        SoleDepoId = x.SoleDepoId,
                        SoleDepoName = x.SoleDepoName,
                        RegionId = x.RegionId,
                        RegionName = x.RegionName,
                        AreaId = x.AreaId,
                        AreaName = x.AreaName,
                        TerritoryId = x.TerritoryId,
                        TerritoryName = x.TerritoryName,
                        IntDepartmentId = x.IntDepartmentId,
                        StrDepartment = x.StrDepartment,
                        IntDesignationId = x.IntDesignationId,
                        StrDesignation = x.StrDesignation,
                        IntSupervisorId = x.IntSupervisorId,
                        StrSupervisorName = x.StrSupervisorName,
                        IntLineManagerId = x.IntLineManagerId,
                        StrLinemanager = x.StrLinemanager,
                        IntEmploymentTypeId = x.intEmploymentTypeId,
                        StrEmploymentType = x.StrEmploymentType
                    }).Distinct().ToList();

                    eh.WingNameList = datas.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).DistinctBy(x => x.Value).ToList();
                    eh.SoleDepoNameList = datas.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).DistinctBy(x => x.Value).ToList();
                    eh.RegionNameList = datas.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).DistinctBy(x => x.Value).ToList();
                    eh.AreaNameList = datas.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).DistinctBy(x => x.Value).ToList();
                    eh.TerritoryNameList = datas.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).DistinctBy(x => x.Value).ToList();
                    eh.StrDepartmentList = datas.Where(x => !string.IsNullOrEmpty(x.StrDepartment)).Select(x => new CommonDDLVM { Value = (long)x.IntDepartmentId, Label = (string)x.StrDepartment }).DistinctBy(x => x.Value).ToList();
                    eh.StrDesignationList = datas.Where(x => !string.IsNullOrEmpty(x.StrDesignation)).Select(x => new CommonDDLVM { Value = (long)x.IntDesignationId, Label = (string)x.StrDesignation }).DistinctBy(x => x.Value).ToList();
                    eh.StrSupervisorNameList = datas.Where(x => !string.IsNullOrEmpty(x.StrSupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.IntSupervisorId, Label = (string)x.StrSupervisorName }).DistinctBy(x => x.Value).ToList();
                    eh.StrLinemanagerList = datas.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).DistinctBy(x => x.Value).ToList();
                    eh.StrEmploymentTypeList = datas.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).DistinctBy(x => x.Value).ToList();

                    employeeReport.EmployeeHeader = eh;
                }

                if (IsPaginated == false)
                {
                    dynamic data = new
                    {
                        TotalCount = await query.CountAsync(),
                        Data = await query.ToListAsync(),
                        pageSize = pageSize,
                        CurrentPage = pageNo,
                    };
                    return data;
                }
                else
                {
                    int maxSize = 1000;
                    pageSize = pageSize > maxSize ? maxSize : pageSize;
                    pageNo = pageNo < 1 ? 1 : pageNo;

                    dynamic data = new
                    {
                        TotalCount = await query.CountAsync(),
                        Data = await query.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToListAsync(),
                        EmployeeHeader = employeeReport.EmployeeHeader,
                        pageSize = pageSize,
                        CurrentPage = pageNo,
                    };
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EmployeeSeaprationLanding> EmployeeSeparationListFilter(long AccountId, long BusinessUnitId, long WorkplaceGroupId, long? EmployeeId, DateTime? FromDate, DateTime? ToDate, bool IsForXl, int PageNo, int PageSize, string? searchTxt, BaseVM tokenData)
        {
            try
            {
                IQueryable<EmployeeSeparationLandingVM> data = (from sep in _context.EmpEmployeeSeparations
                                                                join emp in _context.EmpEmployeeBasicInfos on sep.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp2
                                                                from emp in emp2.DefaultIfEmpty()
                                                                join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                                                                from info in info2.DefaultIfEmpty()
                                                                join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                                                from dept in dept2.DefaultIfEmpty()
                                                                join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                                                from desig in desig2.DefaultIfEmpty()
                                                                where sep.IsActive == true
                                                                && sep.IntAccountId == AccountId
                                                                && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                                                && (FromDate == null || ToDate == null || (FromDate <= sep.DteSeparationDate && sep.DteSeparationDate <= ToDate))
                                                                && (EmployeeId == null || EmployeeId == 0 || sep.IntEmployeeId == EmployeeId)

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

                                                                orderby sep.IntSeparationId descending
                                                                select new EmployeeSeparationLandingVM
                                                                {
                                                                    SeparationId = sep.IntSeparationId,
                                                                    intEmployeeId = sep.IntEmployeeId,
                                                                    strEmployeeName = emp.StrEmployeeName ?? "",
                                                                    strEmployeeCode = emp.StrEmployeeCode ?? "",
                                                                    intSeparationTypeId = sep.IntSeparationTypeId,
                                                                    strSeparationTypeName = sep.StrSeparationTypeName,
                                                                    dteSeparationDate = sep.DteSeparationDate,
                                                                    dteLastWorkingDate = sep.DteLastWorkingDate,
                                                                    strReason = sep.StrReason,
                                                                    IsReleased = sep.IsReleased,
                                                                    IsRejoin = sep.IsRejoin,
                                                                    IsReject = sep.IsReject,
                                                                    isActive = sep.IsActive,
                                                                    dteCreatedAt = sep.DteCreatedAt,
                                                                    dteUpdatedAt = sep.DteUpdatedAt,
                                                                    intCreatedBy = sep.IntCreatedBy,
                                                                    ApprovalStatus = sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false && sep.IsRejoin == true ? "Rejoined" :
                                                                                     sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false ? "Approved" :
                                                                                     sep.IsActive == true && sep.IsPipelineClosed == false && sep.IsReject == false ? "Pending" :
                                                                                     sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == true ? "Rejected" : "N/A",
                                                                    strEmploymentType = emp.StrEmploymentType,
                                                                    strDepartment = dept.StrDepartment ?? "",
                                                                    strDesignation = desig.StrDesignation ?? "",
                                                                    StrDocumentId = sep.StrDocumentId
                                                                }).AsNoTracking().AsQueryable();


                EmployeeSeaprationLanding retObj = new EmployeeSeaprationLanding();
                if (IsForXl)
                {
                    retObj.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    if (!string.IsNullOrEmpty(searchTxt))
                    {
                        searchTxt = searchTxt.ToLower();
                        data = data.Where(x => x.strEmployeeName.ToLower().Contains(searchTxt) || x.strDesignation.ToLower().Contains(searchTxt) || x.strEmployeeCode.ToLower().Contains(searchTxt)
                        || x.strDepartment.ToLower().Contains(searchTxt));
                    }

                    retObj.TotalCount = await data.CountAsync();
                    retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                    retObj.PageSize = PageSize;
                    retObj.CurrentPage = PageNo;
                }
                return retObj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<EmployeeSeparationLandingVM> EmployeeSeparationById(long SeparationId)
        {
            try
            {
                EmployeeSeparationLandingVM data = await (from sep in _context.EmpEmployeeSeparations
                                                          join emp in _context.EmpEmployeeBasicInfos on sep.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp2
                                                          from emp in emp2.DefaultIfEmpty()
                                                          join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                                          from dept in dept2.DefaultIfEmpty()
                                                          join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                                          from desig in desig2.DefaultIfEmpty()
                                                          where sep.IsActive == true
                                                          && sep.IntSeparationId == SeparationId
                                                          select new EmployeeSeparationLandingVM
                                                          {
                                                              SeparationId = sep.IntSeparationId,
                                                              intEmployeeId = sep.IntEmployeeId,
                                                              strEmployeeName = emp.StrEmployeeName ?? "",
                                                              strEmployeeCode = emp.StrEmployeeCode ?? "",
                                                              intSeparationTypeId = sep.IntSeparationTypeId,
                                                              strSeparationTypeName = sep.StrSeparationTypeName,
                                                              dteSeparationDate = sep.DteSeparationDate,
                                                              dteLastWorkingDate = sep.DteLastWorkingDate,
                                                              strReason = sep.StrReason,
                                                              IsReject = sep.IsReject,
                                                              isActive = sep.IsActive,
                                                              dteCreatedAt = sep.DteCreatedAt,
                                                              dteUpdatedAt = sep.DteUpdatedAt,
                                                              intCreatedBy = sep.IntCreatedBy,
                                                              ApprovalStatus = sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false && sep.IsRejoin == true ? "Rejoin" :
                                                                                 sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false ? "Approved" :
                                                                                 sep.IsActive == true && sep.IsPipelineClosed == false && sep.IsReject == false ? "Pending" :
                                                                                 sep.IsActive == true && sep.IsPipelineClosed == false && sep.IsReject == true ? "Rejected" : "",
                                                              strEmploymentType = emp.StrEmploymentType,
                                                              strDepartment = dept.StrDepartment ?? "",
                                                              strDesignation = desig.StrDesignation ?? "",
                                                              StrDocumentId = sep.StrDocumentId
                                                          }).AsNoTracking().FirstOrDefaultAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<MessageHelper> EmployeeRejoinFromSeparation(EmployeeRejoinFromSeparationVM model, long EmployeeId)
        {
            try
            {
                EmpEmployeeSeparation separation = await _context.EmpEmployeeSeparations.FirstOrDefaultAsync(n => n.IsActive == true && n.IntSeparationId == model.SeparationId && n.IsPipelineClosed == true && n.IsReject == false);

                if (!(bool)separation.IsReleased)
                {
                    return new MessageHelper { Message = "This separated employee is not released", StatusCode = 400 };
                }

                separation.IsRejoin = true;
                separation.StrStatus = "Rejoin";
                separation.DteUpdatedAt = DateTime.Now;
                separation.IntUpdatedBy = EmployeeId;

                _context.EmpEmployeeSeparations.Update(separation);
                await _context.SaveChangesAsync();

                EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == separation.IntEmployeeId);
                EmpEmployeeBasicInfoDetail details = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == separation.IntEmployeeId);
                User user = await _context.Users.FirstOrDefaultAsync(x => (long)x.IntRefferenceId == separation.IntEmployeeId);

                if (emp is not null)
                {
                    emp.IsActive = true;
                    emp.DteUpdatedAt = DateTime.Now;
                    emp.IntUpdatedBy = EmployeeId;
                    _context.EmpEmployeeBasicInfos.Update(emp);
                }

                if (details is not null)
                {
                    details.IntEmployeeStatusId = 1;
                    details.StrEmployeeStatus = "Active";
                    details.DteUpdatedAt = DateTime.Now;
                    details.IsActive = true;
                    details.IntUpdatedBy = EmployeeId;

                    _context.EmpEmployeeBasicInfoDetails.Update(details);
                }

                if (user is not null)
                {
                    user.IsActive = true;
                    user.IntUpdatedBy = EmployeeId;
                    user.DteUpdatedAt = DateTime.Now;
                    _context.Users.Update(user);
                }
                await _context.SaveChangesAsync();

                return new MessageHelper { Message = "This employee successfully rejoin", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new MessageHelper { Message = ex.Message, StatusCode = 400 };
            }
        }

        public async Task<EmployeeSeparationReportLanding> EmployeeSeparationReportFilter(BaseVM tokenData, long businessUnitId, long workplaceGroupId, DateTime? FromDate, DateTime? ToDate, int pageNo, int pageSize, string? searchTxt,
            bool IsPaginated, bool IsHeaderNeed, List<long> departments, List<long> designations, List<long> supervisors, List<long> LineManagers, List<long> employeementType, List<long> WingList, List<long> SoledepoList, 
            List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                var data = (from sep in _context.EmpEmployeeSeparations
                            join emp in _context.EmpEmployeeBasicInfos on sep.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp2
                            from emp in emp2.DefaultIfEmpty()
                            join info1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info1.IntEmployeeId into info2
                            from info in info2.DefaultIfEmpty()
                            join bus in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus.IntBusinessUnitId into empBusinessUnit
                            from businessUnit in empBusinessUnit.DefaultIfEmpty()
                            join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into empWorkplaceGroup
                            from workplaceGroup in empWorkplaceGroup.DefaultIfEmpty()
                            join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                            from dept in dept2.DefaultIfEmpty()
                            join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                            from desig in desig2.DefaultIfEmpty()
                            join sal in _context.PyrEmployeeSalaryElementAssignHeaders on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = sal.IntEmployeeId, isActive = (bool)sal.IsActive } into empSal
                            from salary in empSal.DefaultIfEmpty()

                            join bnk in _context.EmpEmployeeBankDetails on new { empId = emp.IntEmployeeBasicInfoId, isPrimarySalAcc = true, isActive = true } equals new { empId = bnk.IntEmployeeBasicInfoId, isPrimarySalAcc = bnk.IsPrimarySalaryAccount, isActive = (bool)bnk.IsActive } into empBnk
                            from bank in empBnk.DefaultIfEmpty()
                            join oth in _context.EmpEmployeePhotoIdentities on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = oth.IntEmployeeBasicInfoId, isActive = (bool)oth.IsActive } into empOther
                            from other in empOther.DefaultIfEmpty()
                            join father in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId, relId = (long)1, IsActive = true } equals new { empId = father.IntEmployeeBasicInfoId, relId = father.IntRelationShipId, IsActive = (bool)father.IsActive } into empFather
                            from fatherContact in empFather.DefaultIfEmpty()
                            join mother in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId, relId = (long)2, IsActive = true } equals new { empId = mother.IntEmployeeBasicInfoId, relId = mother.IntRelationShipId, IsActive = (bool)mother.IsActive } into empMother
                            from motherContact in empMother.DefaultIfEmpty()
                            join supervisor in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals supervisor.IntEmployeeBasicInfoId into empSupervisor
                            from sup in empSupervisor.DefaultIfEmpty()
                            join lm in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals lm.IntEmployeeBasicInfoId into lm2
                            from lm in lm2.DefaultIfEmpty()
                            join presAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)1, IsActive = true } equals new { empId = presAdd.IntEmployeeBasicInfoId, aType = presAdd.IntAddressTypeId, IsActive = (bool)presAdd.IsActive } into empPresentAddress
                            from presentAddress in empPresentAddress.DefaultIfEmpty()
                            join permAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)2, IsActive = true } equals new { empId = permAdd.IntEmployeeBasicInfoId, aType = permAdd.IntAddressTypeId, IsActive = (bool)permAdd.IsActive } into empPermanentAddress
                            from permanentAddress in empPermanentAddress.DefaultIfEmpty()

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

                            where sep.IsActive == true && sep.IntAccountId == tokenData.accountId
                            && emp.IntBusinessUnitId == businessUnitId && emp.IntWorkplaceGroupId == workplaceGroupId
                            && (FromDate == null || ToDate == null || (FromDate <= sep.DteSeparationDate && sep.DteSeparationDate <= ToDate))
                             //&& (EmployeeId == null || EmployeeId == 0 || sep.IntEmployeeId == EmployeeId)

                             && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                             && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                             && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                             : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                             : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId)
                             : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo)
                             : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId)
                             : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(info.IntWingId) && tokenData.soleDepoList.Contains(info.IntSoleDepo) && tokenData.regionList.Contains(info.IntRegionId) && tokenData.areaList.Contains(info.IntAreaId)
                             : tokenData.territoryList.Contains(info.IntTerritoryId))
                             || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                             && (!string.IsNullOrEmpty(searchTxt) ? (emp.StrEmployeeName.ToLower().Contains(searchTxt) || emp.StrEmployeeCode.ToLower().Contains(searchTxt)
                                 || emp.StrReferenceId.ToLower().Contains(searchTxt) || desig.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                                 || info.StrPinNo.ToLower().Contains(searchTxt) || info.StrPersonalMobile.ToLower().Contains(searchTxt)
                                 || fatherContact.StrRelativesName.ToLower().Contains(searchTxt) || wing.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || soleDp.StrTerritoryName.ToLower().Contains(searchTxt) || region.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || area.StrTerritoryName.ToLower().Contains(searchTxt) || Territory.StrTerritoryName.ToLower().Contains(searchTxt)
                                 || motherContact.StrRelativesName.ToLower().Contains(searchTxt) || info.StrPersonalMobile.ToLower().Contains(searchTxt)
                                 || other.StrNid.ToLower().Contains(searchTxt) || bank.StrAccountNo.ToLower().Contains(searchTxt)
                                 || bank.StrRoutingNo.ToLower().Contains(searchTxt) || emp.StrReligion.ToLower().Contains(searchTxt)
                                 )
                                 : true)

                             && ((departments.Count > 0 ? departments.Contains((long)emp.IntDepartmentId) : true)
                                && (designations.Count > 0 ? designations.Contains((long)emp.IntDesignationId) : true)
                                && (supervisors.Count > 0 ? supervisors.Contains((long)emp.IntSupervisorId) : true)
                                && (LineManagers.Count > 0 ? LineManagers.Contains((long)emp.IntLineManagerId) : true)
                                && (employeementType.Count > 0 ? employeementType.Contains((long)emp.IntEmploymentTypeId) : true)
                                && (WingList.Count > 0 ? WingList.Contains((long)info.IntWingId) : true)
                                && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)info.IntSoleDepo) : true)
                                && (RegionList.Count > 0 ? RegionList.Contains((long)info.IntRegionId) : true)
                                && (AreaList.Count > 0 ? AreaList.Contains((long)info.IntAreaId) : true)
                                && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)info.IntTerritoryId) : true))

                            orderby sep.IntSeparationId descending
                            select new
                            {
                                SeparationId = sep.IntSeparationId,
                                intBusinessUnitId = emp.IntBusinessUnitId,
                                strBusinessUnit = businessUnit == null ? "" : businessUnit.StrBusinessUnit,
                                intWorkplaceGroupId = emp.IntWorkplaceGroupId,
                                strWorkplaceGroup = workplaceGroup == null ? "" : workplaceGroup.StrWorkplaceGroup,
                                intEmployeeId = sep.IntEmployeeId,
                                strEmployeeName = emp.StrEmployeeName ?? "",
                                strEmployeeCode = emp.StrEmployeeCode ?? "",
                                intSeparationTypeId = (long)sep.IntSeparationTypeId,
                                strSeparationTypeName = sep.StrSeparationTypeName,
                                dteSeparationDate = sep.DteSeparationDate,
                                dteLastWorkingDate = sep.DteLastWorkingDate,
                                strReason = sep.StrReason,
                                IsReleased = sep.IsReleased,
                                IsRejoin = sep.IsRejoin,
                                IsReject = sep.IsReject,
                                isActive = sep.IsActive,
                                dteCreatedAt = sep.DteCreatedAt,
                                dteUpdatedAt = sep.DteUpdatedAt,
                                intCreatedBy = sep.IntCreatedBy,
                                ApprovalStatus = sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false && sep.IsRejoin == true ? "Rejoined" :
                                                 sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false ? "Approved" :
                                                 sep.IsActive == true && sep.IsPipelineClosed == false && sep.IsReject == false ? "Pending" :
                                                 sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == true ? "Rejected" : "N/A",
                                strEmploymentType = emp.StrEmploymentType,
                                IntDepartmentId = emp.IntDepartmentId,
                                StrDepartment = dept == null ? "" : dept.StrDepartment,                                
                                IntDesignationId = emp.IntDesignationId,
                                StrDesignation = desig == null? "" : desig.StrDesignation,
                                StrDocumentId = sep.StrDocumentId,

                                DateOfJoining = emp.DteJoiningDate,
                                PayrollGroup = salary == null ? "" : salary.StrSalaryBreakdownHeaderTitle,
                                GrossSalary = salary == null ? 0 : salary.NumGrossSalary,
                                TotalSalary = salary == null ? 0 : salary.NumGrossSalary,
                                DateOfConfirmation = emp.DteConfirmationDate,
                                FatherName = fatherContact == null ? "" : fatherContact.StrRelativesName,
                                MotherName = motherContact == null ? "" : motherContact.StrRelativesName,

                                presentAddress = (presentAddress.StrAddressDetails == null ? "" : "Village: " + presentAddress.StrAddressDetails + ", ") +
                                                  (presentAddress.StrPostOffice == null ? "" : "Post Office: " + presentAddress.StrPostOffice + (presentAddress.StrZipOrPostCode == null ? "" : "-" + presentAddress.StrZipOrPostCode) + ", ") +
                                                  (presentAddress.StrThana == null ? "" : "Thana: " + presentAddress.StrThana + ", ") +
                                                  (presentAddress.StrDistrictOrState == null ? "" : "District: " + presentAddress.StrDistrictOrState),

                                PermanentAddress = (permanentAddress.StrAddressDetails == null ? "" : "Village: " + permanentAddress.StrAddressDetails + ", ") +
                                                    (permanentAddress.StrPostOffice == null ? "" : "Post Office: " + permanentAddress.StrPostOffice + (permanentAddress.StrZipOrPostCode == null ? "" : "-" + permanentAddress.StrZipOrPostCode) + ", ") +
                                                    (permanentAddress.StrThana == null ? "" : "Thana: " + permanentAddress.StrThana + ", ") +
                                                    (permanentAddress.StrDistrictOrState == null ? "" : "District: " + permanentAddress.StrDistrictOrState),
                                DateOfBirth = emp.DteDateOfBirth,
                                Religion = emp.StrReligion,
                                Gender = emp.StrGender,
                                MaritialStatus = emp.StrMaritalStatus,
                                BloodGroup = emp.StrBloodGroup,
                                MobileNo = info.StrOfficeMobile,
                                NID = other == null ? "" : other.StrNid,
                                BirthID = other == null ? "" : other.StrBirthId,
                                StrVehicleNo = info.StrVehicleNo,
                                StrRemarks = info.StrRemarks,
                                StrPersonalMobile = info.StrPersonalMobile,
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
                                intEmploymentTypeId = emp.IntEmploymentTypeId,
                                Email = info.StrPersonalMail,
                                OfficeEmail = info.StrOfficeMail,
                                IntSupervisorId = emp.IntSupervisorId,
                                StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                IntLineManagerId = emp.IntLineManagerId,
                                StrLinemanager = lm == null ? "" : lm.StrEmployeeName,
                                BankName = bank == null ? "" : bank.StrBankWalletName,
                                BranchName = bank == null ? "" : bank.StrBranchName,
                                AccountNo = bank == null ? "" : bank.StrAccountNo,
                                RoutingNo = bank == null ? "" : bank.StrRoutingNo,
                                StrPinNo = info.StrPinNo
                            });

                EmployeeSeparationReportLanding separatedReport = new();

                if (IsHeaderNeed)
                {
                    EmployeeHeader eh = new();

                    var datas = data.Select(x => new
                    {
                        WingId = x.WingId,
                        WingName = x.WingName,
                        SoleDepoId = x.SoleDepoId,
                        SoleDepoName = x.SoleDepoName,
                        RegionId = x.RegionId,
                        RegionName = x.RegionName,
                        AreaId = x.AreaId,
                        AreaName = x.AreaName,
                        TerritoryId = x.TerritoryId,
                        TerritoryName = x.TerritoryName,
                        IntDepartmentId = x.IntDepartmentId,
                        StrDepartment = x.StrDepartment,
                        IntDesignationId = x.IntDesignationId,
                        StrDesignation = x.StrDesignation,
                        IntSupervisorId = x.IntSupervisorId,
                        StrSupervisorName = x.StrSupervisorName,
                        IntLineManagerId = x.IntLineManagerId,
                        StrLinemanager = x.StrLinemanager,
                        IntEmploymentTypeId = x.intEmploymentTypeId,
                        StrEmploymentType = x.strEmploymentType
                    }).Distinct().ToList();

                    eh.WingNameList = datas.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).DistinctBy(x => x.Value).ToList();
                    eh.SoleDepoNameList = datas.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).DistinctBy(x => x.Value).ToList();
                    eh.RegionNameList = datas.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).DistinctBy(x => x.Value).ToList();
                    eh.AreaNameList = datas.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).DistinctBy(x => x.Value).ToList();
                    eh.TerritoryNameList = datas.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).DistinctBy(x => x.Value).ToList();
                    eh.StrDepartmentList = datas.Where(x => !string.IsNullOrEmpty(x.StrDepartment)).Select(x => new CommonDDLVM { Value = (long)x.IntDepartmentId, Label = (string)x.StrDepartment }).DistinctBy(x => x.Value).ToList();
                    eh.StrDesignationList = datas.Where(x => !string.IsNullOrEmpty(x.StrDesignation)).Select(x => new CommonDDLVM { Value = (long)x.IntDesignationId, Label = (string)x.StrDesignation }).DistinctBy(x => x.Value).ToList();
                    eh.StrSupervisorNameList = datas.Where(x => !string.IsNullOrEmpty(x.StrSupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.IntSupervisorId, Label = (string)x.StrSupervisorName }).DistinctBy(x => x.Value).ToList();
                    eh.StrLinemanagerList = datas.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).DistinctBy(x => x.Value).ToList();
                    eh.StrEmploymentTypeList = datas.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).DistinctBy(x => x.Value).ToList();

                    separatedReport.EmployeeHeader = eh;
                }

                if (IsPaginated == false)
                {
                    separatedReport.TotalCount = await data.CountAsync();
                    separatedReport.Data = await data.CountAsync();
                   
                    return separatedReport;
                }
                else
                {
                    int maxSize = 1000;
                    pageSize = pageSize > maxSize ? maxSize : pageSize;
                    pageNo = pageNo < 1 ? 1 : pageNo;

                    separatedReport.TotalCount = await data.CountAsync();
                    separatedReport.Data = await data.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToListAsync();
                    separatedReport.PageSize = pageSize;
                    separatedReport.CurrentPage = pageNo;
                    
                    return separatedReport;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<DataTable> EmployeeSeparationListFilter(EmployeeSeparationListFilterViewModel obj)
        //{
        //    try
        //    {
        //        var jsonString = JsonSerializer.Serialize(obj);
        //        using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
        //        {
        //            string sql = "saas.sprAllLandingPageData";
        //            using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
        //            {
        //                sqlCmd.CommandType = CommandType.StoredProcedure;
        //                sqlCmd.Parameters.AddWithValue("@strTableName", obj.TableName);
        //                sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
        //                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
        //                sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", obj.WorkplaceGroupId);
        //                sqlCmd.Parameters.AddWithValue("@intId", obj.IntSeparationId);
        //                sqlCmd.Parameters.AddWithValue("@intStatusId", obj.Status);
        //                sqlCmd.Parameters.AddWithValue("@jsonFilter", jsonString);
        //                sqlCmd.Parameters.AddWithValue("@dteFromDate", obj.ApplicationFromDate);
        //                sqlCmd.Parameters.AddWithValue("@dteToDate", obj.ApplicationToDate);

        //                connection.Open();

        //                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
        //                {
        //                    sqlAdapter.Fill(dt);
        //                }
        //                connection.Close();
        //            }
        //        }

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataTable();
        //    }
        //}

        public async Task<EmployeeDetailsViewModel> GetEmployeeDetailsByEmployeeId(long? EmployeeId)
        {
            try
            {
                EmployeeDetailsViewModel data = await (from e in _context.EmpEmployeeBasicInfos
                                                       where e.IntEmployeeBasicInfoId == EmployeeId || EmployeeId == 0
                                                       join emptt in _context.MasterEmploymentTypes on e.IntEmploymentTypeId equals emptt.IntEmploymentTypeId into EMPT
                                                       from empt in EMPT.DefaultIfEmpty()
                                                       join supp in _context.EmpEmployeeBasicInfos on e.IntSupervisorId equals supp.IntEmployeeBasicInfoId into SUP
                                                       from sup in SUP.DefaultIfEmpty()
                                                       join mann in _context.EmpEmployeeBasicInfos on e.IntLineManagerId equals mann.IntEmployeeBasicInfoId into MAN
                                                       from man in MAN.DefaultIfEmpty()
                                                       join deptt in _context.MasterDepartments on e.IntDepartmentId equals deptt.IntDepartmentId into DEPT
                                                       from dept in DEPT.DefaultIfEmpty()
                                                       join dess in _context.MasterDesignations on e.IntDesignationId equals dess.IntDesignationId into DES
                                                       from des in DES.DefaultIfEmpty()
                                                       join mw in _context.MasterWorkplaces on e.IntWorkplaceId equals mw.IntWorkplaceId into MW
                                                       from mww in MW.DefaultIfEmpty()
                                                       join wpg in _context.MasterWorkplaceGroups on mww.IntWorkplaceGroupId equals wpg.IntWorkplaceGroupId into wpgp
                                                       from wgroup in wpgp.DefaultIfEmpty()
                                                       join ed in _context.EmpEmployeeBasicInfoDetails on e.IntEmployeeBasicInfoId equals ed.IntEmployeeId into ED
                                                       from edd in ED.DefaultIfEmpty()
                                                       join psgg in _context.PyrPayscaleGrades on edd.IntPayscaleGradeId equals psgg.IntPayscaleGradeId into psgg2
                                                       from psg in psgg2.DefaultIfEmpty()
                                                       select new EmployeeDetailsViewModel
                                                       {
                                                           EmployeeInfoObj = e,
                                                           EmployeeTypeObj = empt,
                                                           SupervisorObj = sup,
                                                           LineManagerObj = man,
                                                           DepartmentObj = dept,
                                                           DesignationObj = des,
                                                           WorkplaceGroupObj = wgroup,
                                                           PayscaleGradeObj = psg
                                                       }).AsNoTracking().FirstOrDefaultAsync();

                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<LeaveApplicationReportViewModel>> GetAllApprovedLeaveApplicationListByEmployee(long EmployeeId, string? fromDate, string? toDate)
        {
            List<LeaveApplicationReportViewModel> data = await (from l in _context.LveLeaveApplications
                                                                where l.IntEmployeeId == EmployeeId
                                                                join ltt in _context.LveLeaveTypes on l.IntLeaveTypeId equals ltt.IntLeaveTypeId into ltt2
                                                                from lt in ltt2.DefaultIfEmpty()
                                                                where
                                                                   (!string.IsNullOrEmpty(fromDate) ? l.DteApplicationDate.Date >= Convert.ToDateTime(fromDate).Date : true)
                                                                   && (!string.IsNullOrEmpty(toDate) ? l.DteApplicationDate.Date <= Convert.ToDateTime(toDate).Date : true)
                                                                   && (l.IsReject == false && l.IsPipelineClosed == true)
                                                                   && l.IsActive == true
                                                                select new LeaveApplicationReportViewModel
                                                                {
                                                                    LeaveApplication = l,
                                                                    LeaveType = lt,
                                                                }).OrderByDescending(x => x.LeaveApplication.IntApplicationId).AsNoTracking().ToListAsync();

            return data;
        }

        public async Task<EmployeeSeparationReportViewModel> SeperationReportByEmployeeId(long? EmployeeId)
        {
            try
            {
                EmployeeSeparationReportViewModel data = await (from s in _context.EmpEmployeeSeparations
                                                                join stt in _context.EmpSeparationTypes on s.IntSeparationTypeId equals stt.IntSeparationTypeId into ST
                                                                from st in ST.DefaultIfEmpty()
                                                                join ee in _context.EmpEmployeeBasicInfos on s.IntEmployeeId equals ee.IntEmployeeBasicInfoId into E
                                                                from e in E.DefaultIfEmpty()
                                                                join emptt in _context.MasterEmploymentTypes on e.IntEmploymentTypeId equals emptt.IntEmploymentTypeId into EMPT
                                                                from empt in EMPT.DefaultIfEmpty()
                                                                join supp in _context.EmpEmployeeBasicInfos on e.IntSupervisorId equals supp.IntEmployeeBasicInfoId into SUP
                                                                from sup in SUP.DefaultIfEmpty()
                                                                join mann in _context.EmpEmployeeBasicInfos on e.IntLineManagerId equals mann.IntEmployeeBasicInfoId into MAN
                                                                from man in MAN.DefaultIfEmpty()
                                                                join deptt in _context.MasterDepartments on e.IntDepartmentId equals deptt.IntDepartmentId into DEPT
                                                                from dept in DEPT.DefaultIfEmpty()
                                                                join dess in _context.MasterDesignations on e.IntDesignationId equals dess.IntDesignationId into DES
                                                                from des in DES.DefaultIfEmpty()
                                                                where e.IntEmployeeBasicInfoId == EmployeeId
                                                                select new EmployeeSeparationReportViewModel
                                                                {
                                                                    EmployeeInfoObj = e,
                                                                    EmployeeTypeObj = empt,
                                                                    SupervisorObj = sup,
                                                                    LineManagerObj = man,
                                                                    DepartmentObj = dept,
                                                                    DesignationObj = des,
                                                                    SeperationObj = s,
                                                                    SeparationTypeObj = st
                                                                }).AsNoTracking().FirstOrDefaultAsync();

                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<SalaryTaxCertificateViewModel> SalaryTaxCertificate(long FiscalYearId, string FiscalYear, long EmployeeId)
        {
            //SalaryTaxCertificateViewModel salaryTaxCertificateView = new SalaryTaxCertificateViewModel();

            FiscalYear fiscalYear = await _context.FiscalYears.Where(x => x.IntAutoId == FiscalYearId && x.IsActive == true).FirstOrDefaultAsync();

            MasterTaxChallanConfig masterTaxChallan = await _context.MasterTaxChallanConfigs.Where(x => x.IntYear == fiscalYear.IntYearId && x.DteFiscalFromDate.Date == fiscalYear.DteFiscalFromDate.Date
                                                        && x.DteFiscalToDate.Date == fiscalYear.DteFiscalToDate.Date).FirstOrDefaultAsync();

            SalaryTaxCertificateViewModel salaryTaxCertificateView = await (from emp in _context.EmpEmployeeBasicInfos
                                                                            where emp.IsActive == true && emp.IntEmployeeBasicInfoId == EmployeeId
                                                                            join des in _context.MasterDesignations on emp.IntDesignationId equals des.IntDesignationId into des1
                                                                            from desig in des1.DefaultIfEmpty()
                                                                            join dep in _context.MasterDepartments on emp.IntDepartmentId equals dep.IntDepartmentId into dep1
                                                                            from depart in dep1.DefaultIfEmpty()
                                                                            join bus in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus.IntBusinessUnitId into bus1
                                                                            from busiUnit in bus1.DefaultIfEmpty()
                                                                                //where (SH.IntMonthId >= fiscalYear.DteFiscalFromDate.Month && SH.IntYearId == fiscalYear.DteFiscalFromDate.Year)
                                                                                //|| (SH.IntMonthId <= fiscalYear.DteFiscalToDate.Month && SH.IntYearId == fiscalYear.DteFiscalToDate.Year)
                                                                            select new SalaryTaxCertificateViewModel
                                                                            {
                                                                                EmployeeName = emp.StrEmployeeName,
                                                                                EmployeeCode = emp.StrEmployeeCode,
                                                                                EmploymentType = emp.StrEmploymentType,
                                                                                BusinessUnit = busiUnit.StrBusinessUnit,
                                                                                Designation = desig.StrDesignation,
                                                                                Department = depart.StrDepartment,
                                                                                FiscalyearName = FiscalYear,
                                                                                TaxAmount = Math.Round((decimal)(from SH in _context.PyrSalaryGenerateHeaders
                                                                                                                 where SH.IsActive == true && SH.IntEmployeeId == EmployeeId
                                                                                                                 && ((SH.IntMonthId >= fiscalYear.DteFiscalFromDate.Month && SH.IntYearId == fiscalYear.DteFiscalFromDate.Year)
                                                                                                                 || (SH.IntMonthId <= fiscalYear.DteFiscalToDate.Month && SH.IntYearId == fiscalYear.DteFiscalToDate.Year))
                                                                                                                 select SH.NumTaxAmount).Sum(), 2),
                                                                                ConfirmationDate = emp.DteJoiningDate,
                                                                                Circle = masterTaxChallan == null ? "" : masterTaxChallan.StrCircle,
                                                                                ZoneName = masterTaxChallan == null ? "" : masterTaxChallan.StrZone,
                                                                                BankId = masterTaxChallan == null ? 0 : masterTaxChallan.IntBankId,
                                                                                BankName = masterTaxChallan == null ? "" : masterTaxChallan.StrBankName,
                                                                                ChallanNo = masterTaxChallan == null ? "" : masterTaxChallan.StrChallanNo,
                                                                                TaxPaidDate = masterTaxChallan == null ? null : masterTaxChallan.DteChallanDate,
                                                                                Fiscalyear = masterTaxChallan == null ? null : masterTaxChallan.DteFiscalFromDate.ToString("MMM yyyy") + " to " + masterTaxChallan.DteFiscalToDate.ToString("MMM yyyy"),
                                                                                payrollElementVMs = (from SR in _context.PyrSalaryGenerateRows
                                                                                                     join SH in _context.PyrSalaryGenerateHeaders on SR.IntSalaryGenerateHeaderId equals SH.IntSalaryGenerateHeaderId
                                                                                                     where SR.IsActive == true && SH.IsActive == true && SR.IntEmployeeId == EmployeeId
                                                                                                     && ((SH.IntMonthId >= fiscalYear.DteFiscalFromDate.Month && SH.IntYearId == fiscalYear.DteFiscalFromDate.Year)
                                                                                                     || (SH.IntMonthId <= fiscalYear.DteFiscalToDate.Month && SH.IntYearId == fiscalYear.DteFiscalToDate.Year))

                                                                                                     group SR by SR.IntPayrollElementId into RowElement
                                                                                                     //select RowElement
                                                                                                     select new PayrollElementVM
                                                                                                     {
                                                                                                         PayrollElementId = RowElement.Select(x => x.IntPayrollElementId).FirstOrDefault(),
                                                                                                         PayrollElement = RowElement.Select(x => x.StrPayrollElement).FirstOrDefault(),
                                                                                                         NumAmount = Math.Round(RowElement.Select(x => x.NumAmount).Sum(), 2)
                                                                                                     }).ToList()
                                                                            }).FirstOrDefaultAsync();

            return salaryTaxCertificateView;
        }
        public async Task<EmpJobConfirmationLanding> EmpJobConfirmation(long AccountId, long BusinessUnitId, long WorkplaceGroupId, long Month, long Year, bool IsXls, int PageNo, int PageSize, string? searchTxt)
        {
            try
            {
                var arr = new[] { 1, 4 };
                IQueryable<EmpJobConfirmationLandingVM> data = (from emp in _context.EmpEmployeeBasicInfos
                                                                join supervisor in _context.EmpEmployeeBasicInfos on emp.IntEmployeeBasicInfoId equals supervisor.IntEmployeeBasicInfoId into supervisor2
                                                                from supervisor in supervisor2.DefaultIfEmpty()
                                                                join details in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals details.IntEmployeeId into details2
                                                                from details in details2.DefaultIfEmpty()
                                                                join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                                                from dept in dept2.DefaultIfEmpty()
                                                                join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                                                from desig in desig2.DefaultIfEmpty()
                                                                join wp in _context.MasterWorkplaces on emp.IntWorkplaceId equals wp.IntWorkplaceId into wp2
                                                                from wp in wp2.DefaultIfEmpty()
                                                                join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wg2
                                                                from wg in wg2.DefaultIfEmpty()
                                                                    //join psc in _context.PyrPayscaleGrades on details.IntPayscaleGradeId equals psc.IntPayscaleGradeId
                                                                where //arr.Contains(details.IntEmployeeStatusId) &&
                                                                (details.IntEmployeeStatusId == 1 || details.IntEmployeeStatusId == 4) &&
                                                                 emp.IntAccountId == AccountId && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                                                && emp.DteJoiningDate.Value.Year == Year && emp.DteJoiningDate.Value.Month == Month
                                                                && emp.IsActive == true
                                                                select new EmpJobConfirmationLandingVM
                                                                {
                                                                    EmployeeId = emp.IntEmployeeBasicInfoId,
                                                                    EmployeeName = emp.StrEmployeeName,
                                                                    EmployeeCode = emp.StrEmployeeCode,
                                                                    DepartmentId = emp.IntDepartmentId,
                                                                    DepartmentName = dept.StrDepartment,
                                                                    ConfirmationDateRaw = emp.DteConfirmationDate,
                                                                    DesignationId = emp.IntDesignationId,
                                                                    DesignationName = desig.StrDesignation,
                                                                    JoiningDate = emp.DteJoiningDate,
                                                                    SupervisorId = supervisor.IntEmployeeBasicInfoId,
                                                                    SupervisorName = supervisor.StrEmployeeName,
                                                                    WorkplaceId = emp.IntWorkplaceId,
                                                                    WorkplaceName = wp.StrWorkplace,
                                                                    WorkplaceGroupId = emp.IntWorkplaceGroupId,
                                                                    WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                                    ConfirmationDate = emp.DteConfirmationDate,
                                                                    ProbationaryCloseDate = emp.DteProbationaryCloseDate,
                                                                    InternCloseDate = emp.DteInternCloseDate,
                                                                    ServiceLength = emp.IsActive == true ? YearMonthDayCalculate.YearMonthDayLongFormCal((DateTime)emp.DteJoiningDate, DateTime.Now.Date) : YearMonthDayCalculate.YearMonthDayLongFormCal((DateTime)emp.DteJoiningDate, (DateTime)emp.DteLastWorkingDate),
                                                                }).AsNoTracking().AsQueryable();

                EmpJobConfirmationLanding retObj = new EmpJobConfirmationLanding();
                if (IsXls)
                {
                    retObj.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    PageSize = PageSize > maxSize ? maxSize : PageSize;
                    PageNo = PageNo < 1 ? 1 : PageNo;

                    if (!string.IsNullOrEmpty(searchTxt))
                    {
                        searchTxt = searchTxt.ToLower();
                        data = data.Where(x => x.EmployeeName.ToLower().Contains(searchTxt) || x.DesignationName.ToLower().Contains(searchTxt) || x.EmployeeCode.ToLower().Contains(searchTxt)
                        || x.DepartmentName.ToLower().Contains(searchTxt));
                    }

                    retObj.TotalCount = await data.CountAsync();
                    retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
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
        #endregion ========== Report ==========

        #region ======= Salary Management =======

        public async Task<DataTable> EmployeeSalaryManagement(EmployeeSalaryManagementDTO obj)
        {
            try
            {
                DataTable dt = new DataTable();

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprSalaryAssign";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPartType", obj.PartType);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", obj.WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intDepartmentId", obj.DepartmentId);
                        sqlCmd.Parameters.AddWithValue("@intDesignationId", obj.DesignationId);
                        sqlCmd.Parameters.AddWithValue("@intSupervisorId", obj.SupervisorId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@strStatus", obj.StrStatus);
                        sqlCmd.Parameters.AddWithValue("@strSearchTxt", obj.StrSearchTxt);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", obj.PageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", obj.PageSize);
                        sqlCmd.Parameters.AddWithValue("@isPaginated", obj.IsPaginated);
                        sqlCmd.Parameters.AddWithValue("@intBreakdownHeaderId", obj.IntBreakdownHeaderId);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
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

        public async Task<MessageHelper> SalaryGenerateRequest(SalaryGenerateRequestDTO obj)
        {
            try
            {
                ///ReportType:
                //'Request For Generate'
                //'Requested Successfully'
                //'Approved Successfully'

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "pyr.sprSalaryGenerateRequest";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@strReportType", obj.ReportType);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", obj.BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@strBusinessUnit", obj.BusinessUnit);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", obj.WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@strWorkplaceGroup", obj.WorkplaceGroup);
                        sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", obj.PayrollGroupId);
                        sqlCmd.Parameters.AddWithValue("@strPayrollGroup", obj.PayrollGroup);
                        sqlCmd.Parameters.AddWithValue("@intMonthId", obj.MonthId);
                        sqlCmd.Parameters.AddWithValue("@intYearId", obj.YearId);
                        sqlCmd.Parameters.AddWithValue("@strGenerateByUserId", obj.GenerateByUserId);

                        SqlParameter outputParameter = new SqlParameter();
                        outputParameter.ParameterName = "@msg";
                        outputParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                        outputParameter.Size = int.MaxValue;
                        outputParameter.Direction = System.Data.ParameterDirection.Output;
                        sqlCmd.Parameters.Add(outputParameter);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        string output = outputParameter.Value.ToString();
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = 200;
                        msg.Message = output;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> GetSalaryGenerateRequestReport(long BusinessUnitId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "timesheet.sprSalaryGenerateRequestReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SalaryCertificateLanding> SalaryCertificateApplication(long? intSalaryCertificateRequestId, long AccountId, long BusinessUnitId, long WorkplaceGroupId, long? IntEmployeeId, long? MonthId, long? YearId, string? searchTxt, int PageNo, int PageSize)
        {
            try
            {
                IQueryable<SalaryCertificateVM> data = (from sal in _context.EmpSalaryCertificateRequests
                                                        join emp in _context.EmpEmployeeBasicInfos on sal.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                        join dsg in _context.MasterDesignations on emp.IntDesignationId equals dsg.IntDesignationId
                                                        join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId
                                                        where sal.IsActive == true && emp.IntAccountId == AccountId &&
                                                              (sal.IntEmployeeId == IntEmployeeId || IntEmployeeId == 0) &&
                                                              (sal.IntPayRollMonth == MonthId || MonthId == 0) &&
                                                              (sal.IntPayRollYear == YearId || YearId == 0)
                                                        select new SalaryCertificateVM
                                                        {
                                                            intEmployeeId = sal.IntEmployeeId,
                                                            intSalaryCertificateRequestId = sal.IntSalaryCertificateRequestId,
                                                            intEmploymentTypeId = emp.IntEmploymentTypeId,
                                                            strEmploymentType = emp.StrEmploymentType,
                                                            intPayRollMonth = sal.IntPayRollMonth,
                                                            intPayRollYear = sal.IntPayRollYear,
                                                            employeeCode = emp.StrEmployeeCode ?? "",
                                                            EmployeeName = emp.StrEmployeeName ?? "",
                                                            strDesignation = dsg.StrDesignation ?? "",
                                                            strDepartment = dept.StrDepartment ?? "",
                                                            isActive = sal.IsActive,
                                                            strStatus = sal.StrStatus,
                                                            Status = sal.IsReject == false && sal.IsPipelineClosed == true && sal.IsActive == true ? "Approved" :
                                                                         sal.IsReject == false && sal.IsPipelineClosed == false && sal.IsActive == true ? "Pending" :
                                                                         sal.IsReject == true ? "Rejected" : "",
                                                            RejectedBy = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == sal.IntRejectedBy).Select(a => a.StrEmployeeName).FirstOrDefault()
                                                        }).AsNoTracking().AsQueryable();

                SalaryCertificateLanding retObj = new SalaryCertificateLanding();
                int maxSize = 1000;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                    data = data.Where(x => x.EmployeeName.ToLower().Contains(searchTxt) || x.strDesignation.ToLower().Contains(searchTxt) || x.employeeCode.ToLower().Contains(searchTxt)
                    || x.strDepartment.ToLower().Contains(searchTxt));
                }

                retObj.TotalCount = await data.CountAsync();
                retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;

                return retObj;

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion ======= Salary Management =======

        #region ======= BonusManagement =======

        public async Task<MessageHelper> CRUDBonusSetup(CRUDBonusSetupViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBonusSetup";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@StrPartName", obj.strPartName);
                        sqlCmd.Parameters.AddWithValue("@IntBonusSetupId", obj.IntBonusSetupId);
                        sqlCmd.Parameters.AddWithValue("@IntBonusId", obj.IntBonusId);
                        sqlCmd.Parameters.AddWithValue("@StrBonusName", obj.StrBonusName);
                        sqlCmd.Parameters.AddWithValue("@StrBonusDescription", obj.StrBonusDescription);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", obj.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", obj.IntBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntReligion", obj.IntReligion);
                        sqlCmd.Parameters.AddWithValue("@StrReligionName", obj.StrReligionName);
                        sqlCmd.Parameters.AddWithValue("@IntEmploymentTypeId", obj.IntEmploymentTypeId);
                        sqlCmd.Parameters.AddWithValue("@StrEmploymentType", obj.StrEmploymentType);
                        sqlCmd.Parameters.AddWithValue("@IntMinimumServiceLengthMonth", obj.IntMinimumServiceLengthMonth);
                        sqlCmd.Parameters.AddWithValue("@MaximumServiceLengthMonth", obj.IntMaximumServiceLengthMonth);
                        sqlCmd.Parameters.AddWithValue("@StrBonusPercentageOn", obj.StrBonusPercentageOn);
                        sqlCmd.Parameters.AddWithValue("@NumBonusPercentage", obj.NumBonusPercentage);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", obj.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@IsActive", obj.IsActive);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> EligbleEmployeeForBonusGenerateLanding(string StrPartName, long IntAccountId, long IntBusinessUnitId, long WorkplaceGroupId, long? WingId, long? SoleDepoId,
            long? RegionId, long? AreaId, long? TerritoryId, long? IntBonusHeaderId, long? IntBonusId, DateTime? DteEffectedDate, long? IntCreatedBy, long? IntPageNo, long? IntPageSize, string? searchText)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBonusGenerateSelectQueryAll";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@StrPartName", StrPartName);
                        sqlCmd.Parameters.AddWithValue("@IntBonusHeaderId", IntBonusHeaderId);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", IntBusinessUnitId);

                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@IntWingId", WingId);
                        sqlCmd.Parameters.AddWithValue("@IntSoleDepoId", SoleDepoId);
                        sqlCmd.Parameters.AddWithValue("@IntRegionId", RegionId);
                        sqlCmd.Parameters.AddWithValue("@IntAreaId", AreaId);
                        sqlCmd.Parameters.AddWithValue("@IntTerritoryId", TerritoryId);

                        sqlCmd.Parameters.AddWithValue("@IntBonusId", IntBonusId);
                        sqlCmd.Parameters.AddWithValue("@DteEffectedDate", DteEffectedDate);
                        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", IntPageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", IntPageSize);
                        sqlCmd.Parameters.AddWithValue("@strSearchText", searchText);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> BonusAllLanding(BonusAllLandingViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBonusAllLanding";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@StrPartName", obj.StrPartName);
                        sqlCmd.Parameters.AddWithValue("@IntBonusHeaderId", obj.IntBonusHeaderId);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", obj.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", obj.IntBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntBonusId", obj.IntBonusId);
                        sqlCmd.Parameters.AddWithValue("@IntPayrollGroupId", obj.IntPayrollGroupId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", obj.IntWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceId", obj.IntWorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@IntReligionId", obj.IntReligionId);
                        sqlCmd.Parameters.AddWithValue("@DteEffectedDate", obj.DteEffectedDate);
                        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", obj.IntCreatedBy);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", obj.dteFromDate);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", obj.dteToDate);

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageHelper> CRUDBonusGenerate(CRUDBonusGenerateViewModel obj)
        {
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprBonusGenerate";
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(obj.BonusGenerateRowVM);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@StrPartName", obj.StrPartName);
                        sqlCmd.Parameters.AddWithValue("@IntBonusHeaderId", obj.IntBonusHeaderId);
                        sqlCmd.Parameters.AddWithValue("@IntAccountId", obj.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@IntBusinessUnitId", obj.IntBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@IntWorkplaceGroupId", obj.WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@IntWingId", obj.WingId);
                        sqlCmd.Parameters.AddWithValue("@IntSoleDepoId", obj.SoleDepoId);
                        sqlCmd.Parameters.AddWithValue("@IntRegionId", obj.RegionId);
                        sqlCmd.Parameters.AddWithValue("@IntAreaId", obj.AreaId);
                        sqlCmd.Parameters.AddWithValue("@IntTerritoryId", obj.TerritoryId);
                        sqlCmd.Parameters.AddWithValue("@IntBonusId", obj.IntBonusId);
                        sqlCmd.Parameters.AddWithValue("@DteEffectedDate", obj.DteEffectedDateTime);
                        sqlCmd.Parameters.AddWithValue("@NumBonusAmount", obj.NumBonusAmount);
                        sqlCmd.Parameters.AddWithValue("@IsArrearBonus", obj.IsArrearBonus);
                        sqlCmd.Parameters.AddWithValue("@IntArrearBonusReferenceId", obj.IntArrearBonusReferenceId);
                        sqlCmd.Parameters.AddWithValue("@IntCreatedBy", obj.IntCreatedBy);

                        if (obj.BonusGenerateRowVM != null)
                        {
                            sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);
                        }

                        connection.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();

                        var msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ======= BonusManagement =======

        #region ==============  Loan  =================

        public async Task<MessageHelper> LoanCRUD(LoanViewModel obj)
        {
            try
            {
                var pipe = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType((long)obj.EmployeeId, "loan");

                DataTable dt = new DataTable();
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprLoanCRUD";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPartType", obj.PartType);
                        sqlCmd.Parameters.AddWithValue("@intLoanApplicationId", obj.LoanApplicationId);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intLoanTypeId", obj.LoanTypeId);
                        sqlCmd.Parameters.AddWithValue("@intLoanAmount", obj.LoanAmount);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", obj.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@intNumberOfInstallment", obj.NumberOfInstallment);
                        sqlCmd.Parameters.AddWithValue("@intNumberOfInstallmentAmount", obj.NumberOfInstallmentAmount);
                        sqlCmd.Parameters.AddWithValue("@intApproveLoanAmount", obj.IntApproveLoanAmount);
                        sqlCmd.Parameters.AddWithValue("@intApproveNumberOfInstallment", obj.IntApproveNumberOfInstallment);
                        sqlCmd.Parameters.AddWithValue("@intApproveNumberOfInstallmentAmount", obj.IntApproveNumberOfInstallmentAmount);
                        sqlCmd.Parameters.AddWithValue("@dteApplicationDate", obj.ApplicationDate);
                        sqlCmd.Parameters.AddWithValue("@numRemainingBalance", obj.RemainingBalance);
                        sqlCmd.Parameters.AddWithValue("@dteEffectiveDate", obj.EffectiveDate);
                        sqlCmd.Parameters.AddWithValue("@strDescription", obj.Description);
                        sqlCmd.Parameters.AddWithValue("@intFileUrlId", obj.FileUrl);
                        sqlCmd.Parameters.AddWithValue("@strReferenceNo", obj.ReferenceNo);
                        sqlCmd.Parameters.AddWithValue("@isActive", obj.IsActive);
                        sqlCmd.Parameters.AddWithValue("@intCreatedBy", obj.CreatedBy);
                        sqlCmd.Parameters.AddWithValue("@dteInsertDateTime", obj.InsertDateTime);
                        sqlCmd.Parameters.AddWithValue("@intUpdatedBy", obj.UpdatedBy);
                        sqlCmd.Parameters.AddWithValue("@dteUpdateDateTime", obj.UpdateDateTime);

                        sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipe.CurrentStageId);
                        sqlCmd.Parameters.AddWithValue("@NextStageId", pipe.NextStageId);
                        sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipe.HeaderId);

                        connection.Open();

                        SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                        dataAdapter.Fill(dt);
                        connection.Close();

                        MessageHelper msg = new MessageHelper();
                        msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                        msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoanLandingPagination> GetLoanApplicationByAdvanceFilter(LoanApplicationByAdvanceFilterViewModel obj)
        {
            try
            {
                var data = (from loan in _context.EmpLoanApplications
                            join empp in _context.EmpEmployeeBasicInfos on loan.IntEmployeeId equals empp.IntEmployeeBasicInfoId into empp2
                            from emp in empp2.DefaultIfEmpty()
                            join lt in _context.EmpLoanTypes on loan.IntLoanTypeId equals lt.IntLoanTypeId into ltype
                            from ltp in ltype.DefaultIfEmpty()
                            join deptt in _context.MasterDepartments on emp.IntDepartmentId equals deptt.IntDepartmentId into deptt2
                            from dept in deptt2.DefaultIfEmpty()
                            join desigg in _context.MasterDesignations on emp.IntDesignationId equals desigg.IntDesignationId into desigg2
                            from desig in desigg2.DefaultIfEmpty()
                            join pp in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals pp.IntEmployeeId into pp2
                            from p in pp2.DefaultIfEmpty()
                            where emp.IntAccountId == obj.AccountId
                            && obj.BusinessUnitId == emp.IntBusinessUnitId
                            //(obj.LoanTypeId > 0 ? obj.LoanTypeId == loan.IntLoanTypeId : true)
                            && (obj.WorkplaceGroupId > 0 ? obj.WorkplaceGroupId == emp.IntWorkplaceGroupId : true)
                            //&& (obj.DesignationId > 0 ? obj.DesignationId == emp.IntDesignationId : true)
                            //&& (obj.EmployeeId > 0 ? obj.EmployeeId == emp.IntEmployeeBasicInfoId : true)

                            && ((!string.IsNullOrEmpty(obj.FromDate) && !string.IsNullOrEmpty(obj.ToDate))
                                  ? Convert.ToDateTime(obj.FromDate).Date <= loan.DteApplicationDate.Date
                                  && loan.DteApplicationDate.Date <= Convert.ToDateTime(obj.ToDate).Date : false)
                            //&& ((string.IsNullOrEmpty(obj.FromDate) && !string.IsNullOrEmpty(obj.ToDate))
                            //      ? loan.DteApplicationDate.Date <= Convert.ToDateTime(obj.ToDate).Date : true)
                            //&& ((!string.IsNullOrEmpty(obj.FromDate) && string.IsNullOrEmpty(obj.ToDate))
                            //      ? Convert.ToDateTime(obj.FromDate).Date <= loan.DteApplicationDate.Date : true)

                            //&& (obj.MinimumAmount > 0 && obj.MaximumAmount > 0 ? obj.MinimumAmount <= loan.IntLoanAmount && loan.IntLoanAmount <= obj.MaximumAmount : true)
                            //&& (obj.MinimumAmount <= 0 && obj.MaximumAmount > 0 ? loan.IntLoanAmount <= obj.MaximumAmount : true)
                            //&& (obj.MinimumAmount > 0 && obj.MaximumAmount <= 0 ? obj.MinimumAmount <= loan.IntLoanAmount : true)

                            //&& (string.IsNullOrEmpty(obj.ApplicationStatus) ? (loan.IsActive == true)
                            //: obj.ApplicationStatus.ToLower() == "running" ? (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false)
                            //: obj.ApplicationStatus.ToLower() == "pending" ? (loan.IsActive == true && loan.IsPipelineClosed == false && loan.IsReject == false)
                            ////: obj.ApplicationStatus.ToLower() == "approved" ? (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false)
                            //: obj.ApplicationStatus.ToLower() == "rejected" ? (loan.IsActive == true && loan.IsReject == true)
                            //: true)

                            //&& (string.IsNullOrEmpty(obj.InstallmentStatus) ? true
                            //: (obj.InstallmentStatus.ToLower() == "not started") ? (loan.IsActive == true && loan.IsPipelineClosed == false && (loan.IsReject == false || loan.IsReject == true))
                            //: (obj.InstallmentStatus.ToLower() == "running") ? (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false)
                            //: (obj.InstallmentStatus.ToLower() == "hold") ? (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false && loan.IsHold == true)
                            //: (obj.InstallmentStatus.ToLower() == "completed") ? (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false && loan.NumRemainingBalance <= 0)
                            //: true)

                            select new LoanReportByAdvanceFilterViewModel
                            {
                                LoanApplicationId = loan.IntLoanApplicationId,
                                EmployeeId = loan.IntEmployeeId,
                                StrEmployeeName = emp.StrEmployeeName,
                                StrDesignation = desig.StrDesignation,
                                StrReferenceId = emp.StrReferenceId,
                                StrDepartment = dept.StrDepartment,
                                StrEmployeeCode = emp.StrEmployeeCode,
                                LoanTypeId = loan.IntLoanTypeId,
                                LoanAmount = loan.IntLoanAmount,
                                NumberOfInstallment = loan.IntNumberOfInstallment,
                                NumberOfInstallmentAmount = loan.IntNumberOfInstallmentAmount,
                                ApplicationDate = loan.DteCreatedAt,
                                IsApprove = (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false) ? true : false,
                                ApproveBy = loan.IntUpdatedBy.ToString(),
                                ApproveDate = loan.DteUpdatedAt,
                                ApproveLoanAmount = loan.IntApproveLoanAmount,
                                ApproveNumberOfInstallment = loan.IntApproveNumberOfInstallment,
                                RemainingBalance = loan.NumRemainingBalance,
                                EffectiveDate = loan.DteEffectiveDate,
                                Description = loan.StrDescription,
                                FileUrl = loan.IntFileUrlId,
                                // StrReferenceId = loan.StrReferenceNo,
                                isActive = loan.IsActive,
                                isReject = loan.IsReject,
                                RejectBy = loan.IntUpdatedBy.ToString(),
                                RejectDate = loan.DteUpdatedAt,
                                CreatedByUserId = loan.IntCreatedBy,
                                CreatedatDateTime = loan.DteCreatedAt,
                                UpdateByUserId = loan.IntUpdatedBy,
                                UpdateDateTime = loan.DteUpdatedAt,


                                PositionName = p.StrHrpostionName,
                                LoanType = ltp.StrLoanType,
                                ApplicationStatus = (loan.IsActive == true && loan.IsPipelineClosed == false && loan.IsReject == false) ? "Pending"
                                : (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false) ? "Approved"
                                : (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == true) ? "Rejected"
                                : (loan.IsActive == false) ? "Deleted" : "N/A",
                                InstallmentStatus = (loan.IsActive == true && loan.IsPipelineClosed == false && loan.IsReject == false) ? "Not Started"
                                : (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false && loan.IsHold == true) ? "Hold"
                                : (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false && loan.NumRemainingBalance <= 0) ? "Completed"
                                : (loan.IsActive == true && loan.IsPipelineClosed == true && loan.IsReject == false) ? "Running"
                                : (loan.IsActive == false) ? "Deleted" : "N/A"
                            }).OrderByDescending(x => x.LoanApplicationId).AsNoTracking().AsQueryable();

                LoanLandingPagination retObj = new();

                if (!string.IsNullOrEmpty(obj.SearchText))
                {
                    string searchTxt = obj.SearchText;
                    data = data.Where(x => x.StrEmployeeName.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrEmployeeCode.ToLower().Contains(searchTxt)
                   || x.StrReferenceId.ToLower().Contains(searchTxt) || x.StrDepartment.ToLower().Contains(searchTxt));
                }
                if (obj.Ispaginated == false)
                {
                    retObj.Data = await data.ToListAsync();
                }
                else if (obj.Ispaginated)
                {
                    retObj.TotalCount = await data.CountAsync();
                    retObj.Data = await data.Skip(obj.PageSize * (obj.PageNo - 1)).Take(obj.PageSize).ToListAsync();
                    retObj.PageSize = (long)obj.PageSize;
                    retObj.CurrentPage = (long)obj.PageNo;
                }

                return retObj;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion ==============  Loan  =================

        #region ========= Remote Attendance ===========

        public async Task<MessageHelper> RemoteAttendanceRegistration(RemoteAttendanceRegistrationViewModel obj)
        {
            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.sprRemoteAttendanceRegistration";
                using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                    sqlCmd.Parameters.AddWithValue("@intAttendanceRegId", obj.AttendanceRegId);
                    sqlCmd.Parameters.AddWithValue("@intAccountId", obj.IntAccountId);
                    sqlCmd.Parameters.AddWithValue("@isLocationRegister", obj.IsLocationRegister);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                    sqlCmd.Parameters.AddWithValue("@strEmployeeName", obj.EmployeeName);
                    sqlCmd.Parameters.AddWithValue("@isHomeOffice", obj.IsHomeOffice);
                    sqlCmd.Parameters.AddWithValue("@strLongitude", obj.Longitude);
                    sqlCmd.Parameters.AddWithValue("@strLatitude", obj.Latitude);
                    sqlCmd.Parameters.AddWithValue("@strPlaceName", obj.PlaceName);
                    sqlCmd.Parameters.AddWithValue("@strAddress", obj.Address);
                    sqlCmd.Parameters.AddWithValue("@strDeviceId", obj.DeviceId);
                    sqlCmd.Parameters.AddWithValue("@strDeviceName", obj.DeviceName);
                    sqlCmd.Parameters.AddWithValue("@intInsertBy", obj.InsertBy);

                    connection.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(dt);
                    }
                    connection.Close();

                    MessageHelper msg = new MessageHelper();
                    msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                    msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                    return msg;
                }
            }
        }

        public async Task<MessageHelper> RemoteAttendance(RemoteAttendanceViewModel obj)
        {
            using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
            {
                string sql = "saas.sprRemoteAttendance";
                using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@intPartId", obj.PartId);
                    sqlCmd.Parameters.AddWithValue("@intAccountId", obj.AccountId);
                    sqlCmd.Parameters.AddWithValue("@intEmployeeId", obj.EmployeeId);
                    sqlCmd.Parameters.AddWithValue("@strLongitude", obj.Longitude);
                    sqlCmd.Parameters.AddWithValue("@strLatitude", obj.Latitude);
                    sqlCmd.Parameters.AddWithValue("@intRealTimeImage", obj.RealTimeImageId);
                    sqlCmd.Parameters.AddWithValue("@strDeviceId", obj.DeviceId);
                    sqlCmd.Parameters.AddWithValue("@strDeviceName", obj.DeviceName);
                    sqlCmd.Parameters.AddWithValue("@isMarket", obj.isMarket);
                    sqlCmd.Parameters.AddWithValue("@strVisitingCompany", obj.VisitingCompany);
                    sqlCmd.Parameters.AddWithValue("@strVisitingLocation", obj.VisitingLocation);
                    sqlCmd.Parameters.AddWithValue("@strRemarks", obj.Remarks);

                    connection.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(dt);
                    }
                    connection.Close();

                    var msg = new MessageHelper();
                    msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                    msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);
                    return msg;
                }
            }
        }

        #endregion ========= Remote Attendance ===========

        #region ================ Transfer & Promotion ===================

        public async Task<MessageHelperCreate> CRUDEmpTransferNpromotion(TransferNpromotionVM obj)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                PipelineStageInfoVM pipelineStageInfo = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(obj.IntEmployeeId, "transferNPromotion");
                //PipelineStageInfoVM pipelineStageInfo = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(model.IntAccountId, "transferNPromotion");
                EmpEmployeeIncrement employeeIncrement = new EmpEmployeeIncrement();

                if (pipelineStageInfo == null || pipelineStageInfo.HeaderId <= 0)
                {
                    res.StatusCode = 500;
                    res.Message = "Pipeline was not setup";
                    return res;
                }
                else if (await _context.EmpTransferNpromotions.Where(x => x.IntEmployeeId == obj.IntEmployeeId && x.IsActive == true && x.IntTransferNpromotionId != obj.IntTransferNpromotionId && x.IsPipelineClosed == false && x.IsReject == false && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Already one Transfer & Promotion Application has been Exist";
                    return res;
                }
                else
                {
                    // on edit
                    if (obj.IntTransferNpromotionId > 0)
                    {
                        employeeIncrement = await _context.EmpEmployeeIncrements.FirstOrDefaultAsync(x => x.IntTransferNpromotionReferenceId == obj.IntTransferNpromotionId && x.IsActive == true);
                        await DeleteEmpTransferNpromotion(obj.IntTransferNpromotionId, (long)obj.IntCreatedBy);
                    }
                    // edit closed

                    EmpEmployeeBasicInfo empEmployeeBasic = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == obj.IntEmployeeId);
                    EmpEmployeeBasicInfoDetail empEmployeeDetails = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == obj.IntEmployeeId);

                    EmpTransferNpromotion empTransfer = new EmpTransferNpromotion
                    {
                        IntTransferNpromotionId = 0,
                        IntEmployeeId = obj.IntEmployeeId,
                        StrEmployeeName = obj.StrEmployeeName,
                        StrTransferNpromotionType = obj.StrTransferNpromotionType,
                        IntAccountId = obj.IntAccountId,

                        IntBusinessUnitIdFrom = empEmployeeBasic.IntBusinessUnitId,
                        IntWorkplaceGroupIdFrom = empEmployeeBasic.IntWorkplaceGroupId,
                        IntWorkplaceIdFrom = empEmployeeBasic.IntWorkplaceId,
                        IntDepartmentIdFrom = empEmployeeBasic.IntDepartmentId,
                        IntDesignationIdFrom = empEmployeeBasic.IntDesignationId,
                        IntSupervisorIdFrom = empEmployeeBasic.IntSupervisorId,
                        IntLineManagerIdFrom = empEmployeeBasic.IntLineManagerId,
                        IntDottedSupervisorIdFrom = empEmployeeBasic.IntDottedSupervisorId,
                        IntWingIdFrom = empEmployeeDetails.IntWingId,
                        IntSoldDepoIdFrom = empEmployeeDetails.IntSoleDepo,
                        IntRegionIdFrom = empEmployeeDetails.IntRegionId,
                        IntAreaIdFrom = empEmployeeDetails.IntAreaId,
                        IntTerritoryIdFrom = empEmployeeDetails.IntTerritoryId,

                        IntBusinessUnitId = obj.IntBusinessUnitId,
                        IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                        IntWorkplaceId = obj.IntWorkplaceId,
                        IntDepartmentId = obj.IntDepartmentId,
                        IntDesignationId = obj.IntDesignationId,
                        IntSupervisorId = obj.IntSupervisorId,
                        IntLineManagerId = obj.IntLineManagerId,
                        IntDottedSupervisorId = obj.IntDottedSupervisorId,
                        IntWingId = obj.IntWingId,
                        IntSoldDepoId = obj.IntSoldDepoId,
                        IntRegionId = obj.IntRegionId,
                        IntAreaId = obj.IntAreaId,
                        IntTerritoryId = obj.IntTerritoryId,

                        DteEffectiveDate = obj.DteEffectiveDate,
                        DteReleaseDate = obj.DteReleaseDate,
                        IntAttachementId = obj.IntAttachementId,
                        StrRemarks = obj.StrRemarks,
                        IntPipelineHeaderId = pipelineStageInfo.HeaderId,
                        IntCurrentStage = pipelineStageInfo.CurrentStageId,
                        IntNextStage = pipelineStageInfo.NextStageId,
                        StrStatus = "Pending",
                        IsPipelineClosed = false,
                        IsReject = false,
                        IsJoined = false,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = obj.IntCreatedBy,
                        IsActive = true
                    };

                    long transferNPromotionId = await SaveEmpTransferNpromotion(empTransfer);

                    if (employeeIncrement != null && employeeIncrement.IntIncrementId > 0)
                    {
                        employeeIncrement.IntTransferNpromotionReferenceId = transferNPromotionId;
                        _context.Update(employeeIncrement);
                        await _context.SaveChangesAsync();
                    }

                    List<EmpTransferNpromotionUserRole> empTransferNpromotionUserRoleList = new List<EmpTransferNpromotionUserRole>();
                    List<EmpTransferNpromotionRoleExtension> empTransferNpromotionRoleExtensionList = new List<EmpTransferNpromotionRoleExtension>();

                    foreach (EmpTransferNpromotionUserRoleVM item in obj.EmpTransferNpromotionUserRoleVMList)
                    {
                        empTransferNpromotionUserRoleList.Add(new EmpTransferNpromotionUserRole
                        {
                            IntTransferNpromotionId = transferNPromotionId,
                            IntUserRoleId = (long)item.IntUserRoleId,
                            StrUserRoleName = item.StrUserRoleName,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = obj.IntCreatedBy,
                            IsActive = true
                        });
                    }

                    foreach (EmpTransferNpromotionRoleExtensionVM item in obj.EmpTransferNpromotionRoleExtensionVMList)
                    {
                        empTransferNpromotionRoleExtensionList.Add(new EmpTransferNpromotionRoleExtension
                        {
                            IntTransferNpromotionId = transferNPromotionId,
                            IntEmployeeId = (long)item.IntEmployeeId,
                            IntOrganizationTypeId = (long)item.IntOrganizationTypeId,
                            StrOrganizationTypeName = item.StrOrganizationTypeName,
                            IntOrganizationReffId = (long)item.IntOrganizationReffId,
                            StrOrganizationReffName = item.StrOrganizationReffName,
                            IntCreatedBy = (long)obj.IntCreatedBy,
                            DteCreatedDateTime = DateTime.Now,
                            IsActive = true
                        });
                    }

                    if (empTransferNpromotionUserRoleList.Any())
                    {
                        await _context.EmpTransferNpromotionUserRoles.AddRangeAsync(empTransferNpromotionUserRoleList);
                        await _context.SaveChangesAsync();
                    }
                    if (empTransferNpromotionRoleExtensionList.Any())
                    {
                        await _context.EmpTransferNpromotionRoleExtensions.AddRangeAsync(empTransferNpromotionRoleExtensionList);
                        await _context.SaveChangesAsync();
                    }

                    res.StatusCode = 200;
                    res.Message = obj.IntTransferNpromotionId > 0 ? "Updated Successfully" : "Created Successfully";
                    res.AutoId = transferNPromotionId;

                    return res;
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<long> SaveEmpTransferNpromotion(EmpTransferNpromotion obj)
        {
            if (obj.IntTransferNpromotionId > 0)
            {
                _context.Entry(obj).State = EntityState.Modified;
                _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;
                _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;

                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.EmpTransferNpromotions.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            return obj.IntTransferNpromotionId;
        }

        public async Task<TransferNPromotionPaginationVM> GetAllEmpTransferNpromotion(BaseVM tokenData, long businessUnitId, long workplaceGroupId, string landingType, DateTime dteFromDate, DateTime dteToDate, string? SearchTxt, int PageNo, int PageSize)
        {
            IQueryable<TransferNpromotionVM> listData = (from trns in _context.EmpTransferNpromotions
                                                         where trns.IntAccountId == tokenData.accountId && trns.IsActive == true
                                                         && (trns.DteEffectiveDate >= dteFromDate && trns.DteEffectiveDate <= dteToDate)

                                                         join bus1 in _context.MasterBusinessUnits on trns.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                         from bus in bus2.DefaultIfEmpty()
                                                         join wg1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                                         from wg in wg2.DefaultIfEmpty()
                                                         join w1 in _context.MasterWorkplaces on trns.IntWorkplaceId equals w1.IntWorkplaceId into w2
                                                         from w in w2.DefaultIfEmpty()
                                                         join dept1 in _context.MasterDepartments on trns.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                                         from dept in dept2.DefaultIfEmpty()
                                                         join desig1 in _context.MasterDesignations on trns.IntDesignationId equals desig1.IntDesignationId into desig2
                                                         from desig in desig2.DefaultIfEmpty()
                                                         join sup1 in _context.EmpEmployeeBasicInfos on trns.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                         from sup in sup2.DefaultIfEmpty()
                                                         join lin1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerId equals lin1.IntEmployeeBasicInfoId into lin2
                                                         from lin in lin2.DefaultIfEmpty()

                                                         join wi in _context.TerritorySetups on trns.IntWingId equals wi.IntTerritoryId into wi1
                                                         from wing in wi1.DefaultIfEmpty()
                                                         join soleD in _context.TerritorySetups on trns.IntSoldDepoId equals soleD.IntTerritoryId into soleD1
                                                         from soleDp in soleD1.DefaultIfEmpty()
                                                         join regn in _context.TerritorySetups on trns.IntRegionId equals regn.IntTerritoryId into regn1
                                                         from region in regn1.DefaultIfEmpty()
                                                         join area1 in _context.TerritorySetups on trns.IntAreaId equals area1.IntTerritoryId into area2
                                                         from area in area2.DefaultIfEmpty()
                                                         join terrty in _context.TerritorySetups on trns.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                         from Territory in terrty1.DefaultIfEmpty()

                                                         join busFrom1 in _context.MasterBusinessUnits on trns.IntBusinessUnitIdFrom equals busFrom1.IntBusinessUnitId into busFrom2
                                                         from busFrom in busFrom2.DefaultIfEmpty()
                                                         join wgFrom1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupIdFrom equals wgFrom1.IntWorkplaceGroupId into wgFrom2
                                                         from wgFrom in wgFrom2.DefaultIfEmpty()
                                                         join wFrom1 in _context.MasterWorkplaces on trns.IntWorkplaceIdFrom equals wFrom1.IntWorkplaceId into wFrom2
                                                         from wFrom in wFrom2.DefaultIfEmpty()
                                                         join deptFrom1 in _context.MasterDepartments on trns.IntDepartmentIdFrom equals deptFrom1.IntDepartmentId into deptFrom2
                                                         from deptFrom in deptFrom2.DefaultIfEmpty()
                                                         join desigFrom1 in _context.MasterDesignations on trns.IntDesignationIdFrom equals desigFrom1.IntDesignationId into desigFrom2
                                                         from desigFrom in desigFrom2.DefaultIfEmpty()
                                                         join supFrom1 in _context.EmpEmployeeBasicInfos on trns.IntDottedSupervisorIdFrom equals supFrom1.IntEmployeeBasicInfoId into supFrom2
                                                         from supFrom in supFrom2.DefaultIfEmpty()
                                                         join linFrom1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerIdFrom equals linFrom1.IntEmployeeBasicInfoId into linFrom2
                                                         from linFrom in linFrom2.DefaultIfEmpty()

                                                         join wiFrom in _context.TerritorySetups on trns.IntWingIdFrom equals wiFrom.IntTerritoryId into wiFrom1
                                                         from wingFrom in wiFrom1.DefaultIfEmpty()
                                                         join soleDFrom in _context.TerritorySetups on trns.IntSoldDepoIdFrom equals soleDFrom.IntTerritoryId into soleDFrom1
                                                         from soleDpFrom in soleDFrom1.DefaultIfEmpty()
                                                         join regnFrom in _context.TerritorySetups on trns.IntRegionIdFrom equals regnFrom.IntTerritoryId into regnFrom1
                                                         from regionFrom in regnFrom1.DefaultIfEmpty()
                                                         join areaFrom1 in _context.TerritorySetups on trns.IntAreaIdFrom equals areaFrom1.IntTerritoryId into areaFrom2
                                                         from areaFrom in areaFrom2.DefaultIfEmpty()
                                                         join terrtyFrom in _context.TerritorySetups on trns.IntTerritoryIdFrom equals terrtyFrom.IntTerritoryId into terrtyFrom1
                                                         from TerritoryFrom in terrtyFrom1.DefaultIfEmpty()

                                                         where ((trns.IsJoined == false ? ((tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && trns.IntBusinessUnitId == businessUnitId
                                                                                     && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && trns.IntWorkplaceGroupId == workplaceGroupId
                                                                                     && ((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                                     : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupId)
                                                                                     : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupId) && tokenData.wingList.Contains(trns.IntWingId)
                                                                                     : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupId) && tokenData.wingList.Contains(trns.IntWingId) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId)
                                                                                     : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupId) && tokenData.wingList.Contains(trns.IntWingId) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId) && tokenData.regionList.Contains(trns.IntRegionId)
                                                                                     : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupId) && tokenData.wingList.Contains(trns.IntWingId) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId) && tokenData.regionList.Contains(trns.IntRegionId) && tokenData.areaList.Contains(trns.IntAreaId)
                                                                                     : tokenData.territoryList.Contains(trns.IntTerritoryId)))
                                                                                        : true)
                                                            || (trns.IsJoined == true ? ((tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && trns.IntBusinessUnitIdFrom == businessUnitId
                                                                                     && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && trns.IntWorkplaceGroupIdFrom == workplaceGroupId

                                                                                     && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && trns.IntWorkplaceGroupIdFrom == workplaceGroupId
                                                                                     && ((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                                                     : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupIdFrom)
                                                                                     : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupIdFrom) && tokenData.wingList.Contains(trns.IntWingIdFrom)
                                                                                     : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupIdFrom) && tokenData.wingList.Contains(trns.IntWingIdFrom) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId)
                                                                                     : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupIdFrom) && tokenData.wingList.Contains(trns.IntWingIdFrom) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId) && tokenData.regionList.Contains(trns.IntRegionIdFrom)
                                                                                     : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(trns.IntWorkplaceGroupIdFrom) && tokenData.wingList.Contains(trns.IntWingIdFrom) && tokenData.soleDepoList.Contains(trns.IntSoldDepoId) && tokenData.regionList.Contains(trns.IntRegionIdFrom) && tokenData.areaList.Contains(trns.IntAreaIdFrom)
                                                                                     : tokenData.territoryList.Contains(trns.IntTerritoryIdFrom)))
                                                                                        : true))
                                                            && (!string.IsNullOrEmpty(SearchTxt) ? (trns.StrEmployeeName.ToLower().Contains(SearchTxt.ToLower()) || trns.StrTransferNpromotionType.ToLower().Contains(SearchTxt.ToLower())) : true)

                                                         select new TransferNpromotionVM
                                                         {
                                                             IntTransferNpromotionId = trns.IntTransferNpromotionId,
                                                             IntEmployeeId = trns.IntEmployeeId,
                                                             StrEmployeeName = trns.StrEmployeeName,
                                                             StrTransferNpromotionType = trns.StrTransferNpromotionType,
                                                             IntAccountId = trns.IntAccountId,
                                                             //AccountName = account.StrAccountName,
                                                             IntBusinessUnitId = trns.IntBusinessUnitId,
                                                             BusinessUnitName = bus.StrBusinessUnit,
                                                             IntWorkplaceGroupId = trns.IntWorkplaceGroupId,
                                                             WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                             IntWorkplaceId = trns.IntWorkplaceId,
                                                             WorkplaceName = w.StrWorkplaceGroup,
                                                             IntDepartmentId = trns.IntDepartmentId,
                                                             DepartmentName = dept.StrDepartment,
                                                             IntDesignationId = trns.IntDesignationId,
                                                             DesignationName = desig.StrDesignation,
                                                             IntSupervisorId = trns.IntSupervisorId,
                                                             SupervisorName = sup.StrEmployeeName,
                                                             IntLineManagerId = trns.IntLineManagerId,
                                                             LineManagerName = lin.StrEmployeeName,
                                                             IntDottedSupervisorId = trns.IntDottedSupervisorId,
                                                             //DottedSupervisorName = trns.DottedSupervisorName,

                                                             IntWingId = trns.IntWingId,
                                                             WingName = wing == null ? "" : wing.StrTerritoryName,
                                                             IntSoldDepoId = trns.IntSoldDepoId,
                                                             SoldDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                             IntRegionId = trns.IntRegionId,
                                                             RegionName = region == null ? "" : region.StrTerritoryName,
                                                             IntAreaId = trns.IntAreaId,
                                                             AreaName = area == null ? "" : area.StrTerritoryName,
                                                             IntTerritoryId = trns.IntTerritoryId,
                                                             TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,

                                                             DteEffectiveDate = trns.DteEffectiveDate,
                                                             DteReleaseDate = trns.DteReleaseDate,
                                                             IntAttachementId = trns.IntAttachementId,
                                                             StrRemarks = trns.StrRemarks,
                                                             StrStatus = trns.IsJoined == true ? "Joined" : trns.DteReleaseDate != null ? "Released" : (trns.IsPipelineClosed == true && trns.IsReject == false) ? "Approved" : (trns.IsPipelineClosed == true && trns.IsReject == true) ? "Rejected" : "Pending",
                                                             IsReject = trns.IsReject,
                                                             DteRejectDateTime = trns.DteRejectDateTime,
                                                             IntRejectedBy = trns.IntRejectedBy,
                                                             DteCreatedAt = trns.DteCreatedAt,
                                                             IntCreatedBy = trns.IntCreatedBy,
                                                             DteUpdatedAt = trns.DteUpdatedAt,
                                                             IntUpdatedBy = trns.IntUpdatedBy,
                                                             IsActive = trns.IsActive,
                                                             IsJoined = trns.IsJoined,

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
                                                             LineManagerNameFrom = linFrom != null ? linFrom.StrEmployeeName : "",

                                                             IntWingIdFrom = trns.IntWingIdFrom,
                                                             WingNameFrom = wingFrom == null ? "" : wingFrom.StrTerritoryName,
                                                             IntSoldDepoIdFrom = trns.IntSoldDepoIdFrom,
                                                             SoldDepoNameFrom = soleDpFrom == null ? "" : soleDpFrom.StrTerritoryName,
                                                             IntRegionIdFrom = trns.IntRegionIdFrom,
                                                             RegionNameFrom = regionFrom == null ? "" : regionFrom.StrTerritoryName,
                                                             IntAreaIdFrom = trns.IntAreaIdFrom,
                                                             AreaNameFrom = areaFrom == null ? "" : areaFrom.StrTerritoryName,
                                                             IntTerritoryIdFrom = trns.IntTerritoryIdFrom,
                                                             TerritoryNameFrom = TerritoryFrom == null ? "" : TerritoryFrom.StrTerritoryName
                                                         }).AsNoTracking().OrderByDescending(n => n.DteEffectiveDate.Date).AsQueryable();

            TransferNPromotionPaginationVM transferNPromotion = new();

            int maxSize = 1000;
            PageSize = PageSize > maxSize ? maxSize : PageSize;
            PageNo = PageNo < 1 ? 1 : PageNo;

            transferNPromotion.TotalCount = await listData.CountAsync();
            transferNPromotion.Data = await listData.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
            transferNPromotion.PageSize = PageSize;
            transferNPromotion.CurrentPage = PageNo;

            return transferNPromotion;
        }

        public async Task<List<TransferNpromotionVM>> GetEmpTransferNpromotionHistoryByEmployeeId(long accountId, long employeeId)
        {
            List<TransferNpromotionVM> listData = await (from trns in _context.EmpTransferNpromotions
                                                         where trns.IntAccountId == accountId && trns.IsActive == true && trns.IntEmployeeId == employeeId
                                                         && trns.IsPipelineClosed == true && trns.IsReject == false && trns.IsActive == true

                                                         join account in _context.Accounts on trns.IntAccountId equals account.IntAccountId
                                                         join bus1 in _context.MasterBusinessUnits on trns.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                         from bus in bus2.DefaultIfEmpty()
                                                         join wg1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                                         from wg in wg2.DefaultIfEmpty()
                                                         join w1 in _context.MasterWorkplaces on trns.IntWorkplaceId equals w1.IntWorkplaceId into w2
                                                         from w in w2.DefaultIfEmpty()
                                                         join dept1 in _context.MasterDepartments on trns.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                                         from dept in dept2.DefaultIfEmpty()
                                                         join desig1 in _context.MasterDesignations on trns.IntDesignationId equals desig1.IntDesignationId into desig2
                                                         from desig in desig2.DefaultIfEmpty()
                                                         join sup1 in _context.EmpEmployeeBasicInfos on trns.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                                         from sup in sup2.DefaultIfEmpty()
                                                         join lin1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerId equals lin1.IntEmployeeBasicInfoId into lin2
                                                         from lin in lin2.DefaultIfEmpty()
                                                         join wi in _context.TerritorySetups on trns.IntWingId equals wi.IntTerritoryId into wi1
                                                         from wing in wi1.DefaultIfEmpty()
                                                         join soleD in _context.TerritorySetups on trns.IntSoldDepoId equals soleD.IntTerritoryId into soleD1
                                                         from soleDp in soleD1.DefaultIfEmpty()
                                                         join regn in _context.TerritorySetups on trns.IntRegionId equals regn.IntTerritoryId into regn1
                                                         from region in regn1.DefaultIfEmpty()
                                                         join area1 in _context.TerritorySetups on trns.IntAreaId equals area1.IntTerritoryId into area2
                                                         from area in area2.DefaultIfEmpty()
                                                         join terrty in _context.TerritorySetups on trns.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                         from Territory in terrty1.DefaultIfEmpty()

                                                         join busFrom1 in _context.MasterBusinessUnits on trns.IntBusinessUnitIdFrom equals busFrom1.IntBusinessUnitId into busFrom2
                                                         from busFrom in busFrom2.DefaultIfEmpty()
                                                         join wgFrom1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupIdFrom equals wgFrom1.IntWorkplaceGroupId into wgFrom2
                                                         from wgFrom in wgFrom2.DefaultIfEmpty()
                                                         join wFrom1 in _context.MasterWorkplaces on trns.IntWorkplaceIdFrom equals wFrom1.IntWorkplaceId into wFrom2
                                                         from wFrom in wFrom2.DefaultIfEmpty()
                                                         join deptFrom1 in _context.MasterDepartments on trns.IntDepartmentIdFrom equals deptFrom1.IntDepartmentId into deptFrom2
                                                         from deptFrom in deptFrom2.DefaultIfEmpty()
                                                         join desigFrom1 in _context.MasterDesignations on trns.IntDesignationIdFrom equals desigFrom1.IntDesignationId into desigFrom2
                                                         from desigFrom in desigFrom2.DefaultIfEmpty()
                                                         join supFrom1 in _context.EmpEmployeeBasicInfos on trns.IntDottedSupervisorIdFrom equals supFrom1.IntEmployeeBasicInfoId into supFrom2
                                                         from supFrom in supFrom2.DefaultIfEmpty()
                                                         join linFrom1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerIdFrom equals linFrom1.IntEmployeeBasicInfoId into linFrom2
                                                         from linFrom in linFrom2.DefaultIfEmpty()
                                                         join wiFrom in _context.TerritorySetups on trns.IntWingIdFrom equals wiFrom.IntTerritoryId into wiFrom1
                                                         from wingFrom in wiFrom1.DefaultIfEmpty()
                                                         join soleDFrom in _context.TerritorySetups on trns.IntSoldDepoIdFrom equals soleDFrom.IntTerritoryId into soleDFrom1
                                                         from soleDpFrom in soleDFrom1.DefaultIfEmpty()
                                                         join regnFrom in _context.TerritorySetups on trns.IntRegionIdFrom equals regnFrom.IntTerritoryId into regnFrom1
                                                         from regionFrom in regnFrom1.DefaultIfEmpty()
                                                         join areaFrom1 in _context.TerritorySetups on trns.IntAreaIdFrom equals areaFrom1.IntTerritoryId into areaFrom2
                                                         from areaFrom in areaFrom2.DefaultIfEmpty()
                                                         join terrtyFrom in _context.TerritorySetups on trns.IntTerritoryIdFrom equals terrtyFrom.IntTerritoryId into terrtyFrom1
                                                         from TerritoryFrom in terrtyFrom1.DefaultIfEmpty()

                                                         select new TransferNpromotionVM
                                                         {
                                                             IntTransferNpromotionId = trns.IntTransferNpromotionId,
                                                             IntEmployeeId = trns.IntEmployeeId,
                                                             StrEmployeeName = trns.StrEmployeeName,
                                                             StrTransferNpromotionType = trns.StrTransferNpromotionType,
                                                             IntAccountId = trns.IntAccountId,
                                                             AccountName = account.StrAccountName,
                                                             IntBusinessUnitId = trns.IntBusinessUnitId,
                                                             BusinessUnitName = bus.StrBusinessUnit,
                                                             IntWorkplaceGroupId = trns.IntWorkplaceGroupId,
                                                             WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                             IntWorkplaceId = trns.IntWorkplaceId,
                                                             WorkplaceName = w.StrWorkplaceGroup,
                                                             IntDepartmentId = trns.IntDepartmentId,
                                                             DepartmentName = dept.StrDepartment,
                                                             IntDesignationId = trns.IntDesignationId,
                                                             DesignationName = desig.StrDesignation,
                                                             IntSupervisorId = trns.IntSupervisorId,
                                                             SupervisorName = sup.StrEmployeeName,
                                                             IntLineManagerId = trns.IntLineManagerId,
                                                             LineManagerName = lin.StrEmployeeName,
                                                             IntDottedSupervisorId = trns.IntDottedSupervisorId,
                                                             //DottedSupervisorName = trns.DottedSupervisorName,

                                                             IntWingId = trns.IntWingId,
                                                             WingName = wing == null ? "" : wing.StrTerritoryName,
                                                             IntSoldDepoId = trns.IntSoldDepoId,
                                                             SoldDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                             IntRegionId = trns.IntRegionId,
                                                             RegionName = region == null ? "" : region.StrTerritoryName,
                                                             IntAreaId = trns.IntAreaId,
                                                             AreaName = area == null ? "" : area.StrTerritoryName,
                                                             IntTerritoryId = trns.IntTerritoryId,
                                                             TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,

                                                             DteEffectiveDate = trns.DteEffectiveDate,
                                                             DteReleaseDate = trns.DteReleaseDate,
                                                             IntAttachementId = trns.IntAttachementId,
                                                             StrRemarks = trns.StrRemarks,
                                                             StrStatus = trns.IsPipelineClosed == true && trns.IsReject == false ? "Release" : trns.IsPipelineClosed == true && trns.IsReject == true ? "Rejected" : "Pending",
                                                             IsReject = trns.IsReject,
                                                             DteRejectDateTime = trns.DteRejectDateTime,
                                                             IntRejectedBy = trns.IntRejectedBy,
                                                             DteCreatedAt = trns.DteCreatedAt,
                                                             IntCreatedBy = trns.IntCreatedBy,
                                                             DteUpdatedAt = trns.DteUpdatedAt,
                                                             IntUpdatedBy = trns.IntUpdatedBy,
                                                             IsActive = trns.IsActive,
                                                             IsJoined = trns.IsJoined,

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
                                                             LineManagerNameFrom = linFrom != null ? linFrom.StrEmployeeName : "",

                                                             IntWingIdFrom = trns.IntWingIdFrom,
                                                             WingNameFrom = wingFrom == null ? "" : wingFrom.StrTerritoryName,
                                                             IntSoldDepoIdFrom = trns.IntSoldDepoIdFrom,
                                                             SoldDepoNameFrom = soleDpFrom == null ? "" : soleDpFrom.StrTerritoryName,
                                                             IntRegionIdFrom = trns.IntRegionIdFrom,
                                                             RegionNameFrom = regionFrom == null ? "" : regionFrom.StrTerritoryName,
                                                             IntAreaIdFrom = trns.IntAreaIdFrom,
                                                             AreaNameFrom = areaFrom == null ? "" : areaFrom.StrTerritoryName,
                                                             IntTerritoryIdFrom = trns.IntTerritoryIdFrom,
                                                             TerritoryNameFrom = TerritoryFrom == null ? "" : TerritoryFrom.StrTerritoryName,

                                                             EmpTransferNpromotionUserRoleVMList = _context.EmpTransferNpromotionUserRoles
                                                                      .Where(ur => ur.IntTransferNpromotionId == trns.IntTransferNpromotionId && ur.IsActive == true)
                                                                      .Select(nobj => new EmpTransferNpromotionUserRoleVM
                                                                      {
                                                                          IntTransferNpromotionUserRoleId = nobj.IntTransferNpromotionId,
                                                                          IntTransferNpromotionId = nobj.IntTransferNpromotionId,
                                                                          IntUserRoleId = nobj.IntUserRoleId,
                                                                          StrUserRoleName = nobj.StrUserRoleName
                                                                      }).ToList(),

                                                             EmpTransferNpromotionRoleExtensionVMList = _context.EmpTransferNpromotionRoleExtensions
                                                                           .Where(ur => ur.IntTransferNpromotionId == trns.IntTransferNpromotionId && ur.IsActive == true)
                                                                           .Select(nobj => new EmpTransferNpromotionRoleExtensionVM
                                                                           {
                                                                               IntRoleExtensionRowId = nobj.IntRoleExtensionRowId,
                                                                               IntTransferNpromotionId = nobj.IntTransferNpromotionId,
                                                                               IntEmployeeId = nobj.IntEmployeeId,
                                                                               IntOrganizationTypeId = nobj.IntOrganizationTypeId,
                                                                               StrOrganizationTypeName = nobj.StrOrganizationTypeName,
                                                                               IntOrganizationReffId = nobj.IntOrganizationReffId,
                                                                               StrOrganizationReffName = nobj.StrOrganizationReffName,
                                                                           }).ToList()
                                                         }).OrderByDescending(x => x.DteCreatedAt).ToListAsync();
            return listData;
        }

        public async Task<TransferNpromotionVM> GetEmpTransferNpromotionById(long Id)
        {
            TransferNpromotionVM data = await (from trns in _context.EmpTransferNpromotions
                                               where trns.IntTransferNpromotionId == Id

                                               join account in _context.Accounts on trns.IntAccountId equals account.IntAccountId
                                               join bus1 in _context.MasterBusinessUnits on trns.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                               from bus in bus2.DefaultIfEmpty()
                                               join wg1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                               from wg in wg2.DefaultIfEmpty()
                                               join w1 in _context.MasterWorkplaces on trns.IntWorkplaceId equals w1.IntWorkplaceId into w2
                                               from w in w2.DefaultIfEmpty()
                                               join dept1 in _context.MasterDepartments on trns.IntDepartmentId equals dept1.IntDepartmentId into dept2
                                               from dept in dept2.DefaultIfEmpty()
                                               join desig1 in _context.MasterDesignations on trns.IntDesignationId equals desig1.IntDesignationId into desig2
                                               from desig in desig2.DefaultIfEmpty()
                                               join sup1 in _context.EmpEmployeeBasicInfos on trns.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                               from sup in sup2.DefaultIfEmpty()
                                               join lin1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerId equals lin1.IntEmployeeBasicInfoId into lin2
                                               from lin in lin2.DefaultIfEmpty()

                                               join wi in _context.TerritorySetups on trns.IntWingId equals wi.IntTerritoryId into wi1
                                               from wing in wi1.DefaultIfEmpty()
                                               join soleD in _context.TerritorySetups on trns.IntSoldDepoId equals soleD.IntTerritoryId into soleD1
                                               from soleDp in soleD1.DefaultIfEmpty()
                                               join regn in _context.TerritorySetups on trns.IntRegionId equals regn.IntTerritoryId into regn1
                                               from region in regn1.DefaultIfEmpty()
                                               join area1 in _context.TerritorySetups on trns.IntAreaId equals area1.IntTerritoryId into area2
                                               from area in area2.DefaultIfEmpty()
                                               join terrty in _context.TerritorySetups on trns.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                               from Territory in terrty1.DefaultIfEmpty()

                                               join busFrom1 in _context.MasterBusinessUnits on trns.IntBusinessUnitIdFrom equals busFrom1.IntBusinessUnitId into busFrom2
                                               from busFrom in busFrom2.DefaultIfEmpty()
                                               join wgFrom1 in _context.MasterWorkplaceGroups on trns.IntWorkplaceGroupIdFrom equals wgFrom1.IntWorkplaceGroupId into wgFrom2
                                               from wgFrom in wgFrom2.DefaultIfEmpty()
                                               join wFrom1 in _context.MasterWorkplaces on trns.IntWorkplaceIdFrom equals wFrom1.IntWorkplaceId into wFrom2
                                               from wFrom in wFrom2.DefaultIfEmpty()
                                               join deptFrom1 in _context.MasterDepartments on trns.IntDepartmentIdFrom equals deptFrom1.IntDepartmentId into deptFrom2
                                               from deptFrom in deptFrom2.DefaultIfEmpty()
                                               join desigFrom1 in _context.MasterDesignations on trns.IntDesignationIdFrom equals desigFrom1.IntDesignationId into desigFrom2
                                               from desigFrom in desigFrom2.DefaultIfEmpty()
                                               join supFrom1 in _context.EmpEmployeeBasicInfos on trns.IntDottedSupervisorIdFrom equals supFrom1.IntEmployeeBasicInfoId into supFrom2
                                               from supFrom in supFrom2.DefaultIfEmpty()
                                               join linFrom1 in _context.EmpEmployeeBasicInfos on trns.IntLineManagerIdFrom equals linFrom1.IntEmployeeBasicInfoId into linFrom2
                                               from linFrom in linFrom2.DefaultIfEmpty()

                                               join wiFrom in _context.TerritorySetups on trns.IntWingIdFrom equals wiFrom.IntTerritoryId into wiFrom1
                                               from wingFrom in wiFrom1.DefaultIfEmpty()
                                               join soleDFrom in _context.TerritorySetups on trns.IntSoldDepoIdFrom equals soleDFrom.IntTerritoryId into soleDFrom1
                                               from soleDpFrom in soleDFrom1.DefaultIfEmpty()
                                               join regnFrom in _context.TerritorySetups on trns.IntRegionIdFrom equals regnFrom.IntTerritoryId into regnFrom1
                                               from regionFrom in regnFrom1.DefaultIfEmpty()
                                               join areaFrom1 in _context.TerritorySetups on trns.IntAreaIdFrom equals areaFrom1.IntTerritoryId into areaFrom2
                                               from areaFrom in areaFrom2.DefaultIfEmpty()
                                               join terrtyFrom in _context.TerritorySetups on trns.IntTerritoryIdFrom equals terrtyFrom.IntTerritoryId into terrtyFrom1
                                               from TerritoryFrom in terrtyFrom1.DefaultIfEmpty()

                                               select new TransferNpromotionVM
                                               {
                                                   IntTransferNpromotionId = trns.IntTransferNpromotionId,
                                                   IntEmployeeId = trns.IntEmployeeId,
                                                   StrEmployeeName = trns.StrEmployeeName,
                                                   StrTransferNpromotionType = trns.StrTransferNpromotionType,
                                                   IntAccountId = trns.IntAccountId,
                                                   AccountName = account.StrAccountName,
                                                   IntBusinessUnitId = trns.IntBusinessUnitId,
                                                   BusinessUnitName = bus.StrBusinessUnit,
                                                   IntWorkplaceGroupId = trns.IntWorkplaceGroupId,
                                                   WorkplaceGroupName = wg.StrWorkplaceGroup,
                                                   IntWorkplaceId = trns.IntWorkplaceId,
                                                   WorkplaceName = w.StrWorkplaceGroup,
                                                   IntDepartmentId = trns.IntDepartmentId,
                                                   DepartmentName = dept.StrDepartment,
                                                   IntDesignationId = trns.IntDesignationId,
                                                   DesignationName = desig.StrDesignation,
                                                   IntSupervisorId = trns.IntSupervisorId,
                                                   SupervisorName = sup.StrEmployeeName,
                                                   IntLineManagerId = trns.IntLineManagerId,
                                                   LineManagerName = lin.StrEmployeeName,
                                                   IntDottedSupervisorId = trns.IntDottedSupervisorId,
                                                   //DottedSupervisorName = trns.DottedSupervisorName,

                                                   IntWingId = trns.IntWingId,
                                                   WingName = wing == null ? "" : wing.StrTerritoryName,
                                                   IntSoldDepoId = trns.IntSoldDepoId,
                                                   SoldDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                   IntRegionId = trns.IntRegionId,
                                                   RegionName = region == null ? "" : region.StrTerritoryName,
                                                   IntAreaId = trns.IntAreaId,
                                                   AreaName = area == null ? "" : area.StrTerritoryName,
                                                   IntTerritoryId = trns.IntTerritoryId,
                                                   TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,

                                                   DteEffectiveDate = trns.DteEffectiveDate,
                                                   DteReleaseDate = trns.DteReleaseDate,
                                                   IntAttachementId = trns.IntAttachementId,
                                                   StrRemarks = trns.StrRemarks,
                                                   StrStatus = trns.IsJoined == true ? "Joined" : trns.DteReleaseDate != null ? "Released" : (trns.IsPipelineClosed == true && trns.IsReject == false) ? "Approved" : (trns.IsPipelineClosed == true && trns.IsReject == true) ? "Rejected" : "Pending",
                                                   IsReject = trns.IsReject,
                                                   DteRejectDateTime = trns.DteRejectDateTime,
                                                   IntRejectedBy = trns.IntRejectedBy,
                                                   DteCreatedAt = trns.DteCreatedAt,
                                                   IntCreatedBy = trns.IntCreatedBy,
                                                   DteUpdatedAt = trns.DteUpdatedAt,
                                                   IntUpdatedBy = trns.IntUpdatedBy,
                                                   IsActive = trns.IsActive,
                                                   IsJoined = trns.IsJoined,

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
                                                   LineManagerNameFrom = linFrom != null ? linFrom.StrEmployeeName : "",

                                                   IntWingIdFrom = trns.IntWingIdFrom,
                                                   WingNameFrom = wingFrom == null ? "" : wingFrom.StrTerritoryName,
                                                   IntSoldDepoIdFrom = trns.IntSoldDepoIdFrom,
                                                   SoldDepoNameFrom = soleDpFrom == null ? "" : soleDpFrom.StrTerritoryName,
                                                   IntRegionIdFrom = trns.IntRegionIdFrom,
                                                   RegionNameFrom = regionFrom == null ? "" : regionFrom.StrTerritoryName,
                                                   IntAreaIdFrom = trns.IntAreaIdFrom,
                                                   AreaNameFrom = areaFrom == null ? "" : areaFrom.StrTerritoryName,
                                                   IntTerritoryIdFrom = trns.IntTerritoryIdFrom,
                                                   TerritoryNameFrom = TerritoryFrom == null ? "" : TerritoryFrom.StrTerritoryName,

                                                   EmpTransferNpromotionUserRoleVMList = _context.EmpTransferNpromotionUserRoles
                                                                   .Where(ur => ur.IntTransferNpromotionId == Id && ur.IsActive == true)
                                                                   .Select(nobj => new EmpTransferNpromotionUserRoleVM
                                                                   {
                                                                       IntTransferNpromotionUserRoleId = nobj.IntTransferNpromotionId,
                                                                       IntTransferNpromotionId = nobj.IntTransferNpromotionId,
                                                                       IntUserRoleId = nobj.IntUserRoleId,
                                                                       StrUserRoleName = nobj.StrUserRoleName
                                                                   }).ToList(),

                                                   EmpTransferNpromotionRoleExtensionVMList = _context.EmpTransferNpromotionRoleExtensions
                                                                        .Where(ur => ur.IntTransferNpromotionId == Id && ur.IsActive == true)
                                                                        .Select(nobj => new EmpTransferNpromotionRoleExtensionVM
                                                                        {
                                                                            IntRoleExtensionRowId = nobj.IntRoleExtensionRowId,
                                                                            IntTransferNpromotionId = nobj.IntTransferNpromotionId,
                                                                            IntEmployeeId = nobj.IntEmployeeId,
                                                                            IntOrganizationTypeId = nobj.IntOrganizationTypeId,
                                                                            StrOrganizationTypeName = nobj.StrOrganizationTypeName,
                                                                            IntOrganizationReffId = nobj.IntOrganizationReffId,
                                                                            StrOrganizationReffName = nobj.StrOrganizationReffName,
                                                                        }).ToList()
                                               }).OrderByDescending(x => x.DteCreatedAt).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> DeleteEmpTransferNpromotion(long id, long actionBy)
        {
            try
            {
                EmpTransferNpromotion obj = await _context.EmpTransferNpromotions.FirstAsync(x => x.IntTransferNpromotionId == id);
                obj.IsActive = false;
                _context.EmpTransferNpromotions.Update(obj);
                await _context.SaveChangesAsync();

                List<EmpTransferNpromotionUserRole> roleList = await _context.EmpTransferNpromotionUserRoles.Where(x => x.IntTransferNpromotionId == id).ToListAsync();
                List<EmpTransferNpromotionRoleExtension> extensionList = await _context.EmpTransferNpromotionRoleExtensions.Where(x => x.IntTransferNpromotionId == id).ToListAsync();

                roleList.ForEach(r =>
                {
                    r.IsActive = false;
                    r.DteUpdatedAt = DateTime.Now;
                    r.IntUpdatedBy = actionBy;
                });

                extensionList.ForEach(r =>
                {
                    r.IsActive = false;
                });

                _context.EmpTransferNpromotionUserRoles.UpdateRange(roleList);
                _context.EmpTransferNpromotionRoleExtensions.UpdateRange(extensionList);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsPromotionEligibleThroughIncrement(CRUDIncrementPromotionTransferVM objCrud)
        {
            try
            {
                var item = objCrud.incrementList.FirstOrDefault();

                var empInfo = await (from emp in _context.EmpEmployeeBasicInfos
                                     join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId
                                     join payscale in _context.PyrPayscaleGrades on desig.IntPayscaleGradeId equals payscale.IntPayscaleGradeId
                                     where emp.IntEmployeeBasicInfoId == item.IntEmployeeId && emp.IsActive == true
                                     select new { emp, desig, payscale }).FirstOrDefaultAsync();

                var salaryInfo = await _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IsActive == true).FirstOrDefaultAsync();

                if (empInfo == null) return false;
                if (salaryInfo == null) return false;

                decimal? NumIncrementAmount = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? item.NumIncrementPercentageOrAmount :
                                            item.StrIncrementDependOn.ToLower() == "Gross".ToLower() ? salaryInfo.NumGrossSalary * (item.NumIncrementPercentageOrAmount / 100) :
                                            item.StrIncrementDependOn.ToLower() == "Basic".ToLower() ? salaryInfo.NumBasicOrgross * (item.NumIncrementPercentageOrAmount / 100) : 0;

                if (salaryInfo.NumGrossSalary + NumIncrementAmount > empInfo.payscale.NumMaxSalary)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<MessageHelperCreate> CreateEmployeeIncrement(CRUDIncrementPromotionTransferVM objCrud)
        {
            try
            {
                var res = new MessageHelperCreate();

                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = objCrud.BusinessUnitId, workplaceGroupId = objCrud.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    res = new()
                    {
                        StatusCode = 403,
                        Message = "Acess Denied",
                        AutoId = 0
                    };
                    return res;
                }

                PipelineStageInfoVM incrementPipeline = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(objCrud.incrementList.FirstOrDefault().IntAccountId, "increment");
                PipelineStageInfoVM promotionPipeline = new();
                if (objCrud.isPromotion)
                {
                    promotionPipeline = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(objCrud.incrementList.FirstOrDefault().IntAccountId, "transferNPromotion");
                }

                if ((objCrud.isPromotion == true && objCrud.incrementList.Count > 1) || (objCrud.incrementList.Count == 0))
                {
                    res.StatusCode = 500;
                    res.Message = "Invalid data";
                    return res;
                }
                else if (incrementPipeline.HeaderId == 0)
                {
                    res.StatusCode = 500;
                    res.Message = "Increment pipeline was not setup";
                    return res;
                }
                else if (objCrud.isPromotion == true && promotionPipeline.HeaderId == 0)
                {
                    res.StatusCode = 500;
                    res.Message = "Promotion pipeline was not setup";
                    return res;
                }
                else
                {
                    long transferNpromotionReferenceId = 0;

                    long intIncrementId = objCrud.incrementList.Count() == 1 ? objCrud.incrementList.FirstOrDefault().IntIncrementId : 0;

                    if (intIncrementId == 0) /// CREATE
                    {
                        if (objCrud.isPromotion == true)
                        {
                            var obj = objCrud.transferPromotionObj;
                            IEnumerable<CommonEmployeeDDL> empIdCheck = await GetCommonEmployeeDDL(tokenData, objCrud.BusinessUnitId, objCrud.WorkplaceGroupId, obj.IntEmployeeId, "");
                            if (empIdCheck.Count() > 0)
                            {
                                MessageHelperCreate promObj = await CRUDEmpTransferNpromotion(objCrud.transferPromotionObj);
                                if (promObj.StatusCode != 200) throw new Exception(promObj.Message);
                                transferNpromotionReferenceId = (long)promObj.AutoId;
                            }
                        }
                        var createList = new List<EmpEmployeeIncrement>();

                        List<long> empIdList = objCrud.incrementList.Select(a => (long)a.IntEmployeeId).ToList();
                        IEnumerable<CommonEmployeeDDL> empIdListCheck = await PermissionCheckFromEmployeeListByEnvetFireEmployee(tokenData, objCrud.BusinessUnitId, objCrud.WorkplaceGroupId, empIdList, "");
                        if (empIdListCheck.Count() != empIdList.Count())
                        {
                            res.StatusCode = 400;
                            res.Message = "Employee Id Not Valid!";
                            return res;
                        }
                        foreach (var item in objCrud.incrementList)
                        {
                            var empInfo = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == item.IntEmployeeId).FirstOrDefaultAsync();
                            var salaryInfo = await _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IsActive == true).FirstOrDefaultAsync();

                            if (empInfo == null) throw new Exception("Employee Info Not Found");
                            if (salaryInfo == null) throw new Exception($"{empInfo.StrEmployeeName}'s Salary Info Not Found");

                            var incInfo = await _context.EmpEmployeeIncrements.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IsActive == true && x.IntPipelineHeaderId > 0
                                                                                        && x.IsPipelineClosed == false && x.IsReject == false).FirstOrDefaultAsync();
                            if (incInfo != null) throw new Exception($"there is already a pending application for {empInfo.StrEmployeeName}");

                            createList.Add(new EmpEmployeeIncrement()
                            {
                                IntIncrementId = item.IntIncrementId,
                                IntEmployeeId = item.IntEmployeeId,
                                StrEmployeeName = item.StrEmployeeName,
                                IntEmploymentTypeId = empInfo.IntEmploymentTypeId,
                                IntDesignationId = empInfo.IntDesignationId,
                                IntDepartmentId = empInfo.IntDepartmentId,
                                IntAccountId = item.IntAccountId,
                                IntBusinessUnitId = item.IntBusinessUnitId,
                                NumOldGrossAmount = salaryInfo.NumGrossSalary,
                                StrIncrementDependOn = item.StrIncrementDependOn,
                                NumIncrementDependOn = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? item.NumIncrementPercentageOrAmount :
                                                       item.StrIncrementDependOn.ToLower() == "Gross".ToLower() ? salaryInfo.NumGrossSalary :
                                                       item.StrIncrementDependOn.ToLower() == "Basic".ToLower() ? salaryInfo.NumBasicOrgross : 0,
                                NumIncrementPercentage = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? 0 : item.NumIncrementPercentageOrAmount,
                                NumIncrementAmount = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? item.NumIncrementPercentageOrAmount :
                                                       item.StrIncrementDependOn.ToLower() == "Gross".ToLower() ? salaryInfo.NumGrossSalary * (item.NumIncrementPercentageOrAmount / 100) :
                                                       item.StrIncrementDependOn.ToLower() == "Basic".ToLower() ? salaryInfo.NumBasicOrgross * (item.NumIncrementPercentageOrAmount / 100) : 0,
                                DteEffectiveDate = item.DteEffectiveDate,
                                IntTransferNpromotionReferenceId = transferNpromotionReferenceId,
                                IsActive = true,
                                DteCreatedAt = DateTime.Now,
                                IntCreatedBy = item.IntCreatedBy,
                                DteUpdatedAt = null,
                                IntUpdatedBy = null,
                                IntPipelineHeaderId = incrementPipeline.HeaderId,
                                IntCurrentStage = incrementPipeline.CurrentStageId,
                                IntNextStage = incrementPipeline.NextStageId,
                                IsProcess = false,
                                StrStatus = "Pending",
                                IsPipelineClosed = false,
                                IsReject = false
                            });
                        }

                        await _context.EmpEmployeeIncrements.AddRangeAsync(createList);
                        await _context.SaveChangesAsync();

                        return new MessageHelperCreate();
                    }
                    else /// EDIT DELETE
                    {
                        var incrementData = await _context.EmpEmployeeIncrements.Where(x => x.IntIncrementId == intIncrementId).FirstOrDefaultAsync();

                        if (incrementData == null)
                        {
                            throw new Exception("increment data not found to edit");
                        }

                        if (incrementData.StrStatus.ToLower() == "Pending".ToLower())
                        {
                            var item = objCrud.incrementList.First();

                            if (objCrud.isPromotion == true) // Edit + Create
                            {
                                MessageHelperCreate promObj = await CRUDEmpTransferNpromotion(objCrud.transferPromotionObj);
                                if (promObj.StatusCode != 200) throw new Exception(promObj.Message);
                                transferNpromotionReferenceId = (long)promObj.AutoId;
                            }
                            else if ((objCrud.isPromotion == false && incrementData.IntTransferNpromotionReferenceId > 0) || (item.IsActive == false)) // Delete
                            {
                                bool flag = await DeleteEmpTransferNpromotion((long)incrementData.IntTransferNpromotionReferenceId, (long)item.IntCreatedBy);
                                transferNpromotionReferenceId = 0;
                            }

                            if (item.IsActive == false)
                            {
                                incrementData.IsActive = item.IsActive;
                                incrementData.DteUpdatedAt = DateTime.Now;
                                incrementData.IntUpdatedBy = item.IntCreatedBy;
                            }
                            else
                            {
                                var empInfo = await _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == item.IntEmployeeId).FirstOrDefaultAsync();
                                var salaryInfo = await _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IsActive == true).FirstOrDefaultAsync();

                                incrementData.IntEmploymentTypeId = empInfo.IntEmploymentTypeId;
                                incrementData.IntDesignationId = empInfo.IntDesignationId;
                                incrementData.IntDepartmentId = empInfo.IntDepartmentId;
                                incrementData.NumOldGrossAmount = salaryInfo.NumGrossSalary;
                                incrementData.StrIncrementDependOn = item.StrIncrementDependOn;
                                incrementData.NumIncrementDependOn = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? item.NumIncrementPercentageOrAmount :
                                                       item.StrIncrementDependOn.ToLower() == "Gross".ToLower() ? salaryInfo.NumGrossSalary :
                                                       item.StrIncrementDependOn.ToLower() == "Basic".ToLower() ? salaryInfo.NumBasicOrgross : 0;
                                incrementData.NumIncrementPercentage = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? 0 : item.NumIncrementPercentageOrAmount;
                                incrementData.NumIncrementAmount = item.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? item.NumIncrementPercentageOrAmount :
                                                       item.StrIncrementDependOn.ToLower() == "Gross".ToLower() ? salaryInfo.NumGrossSalary * (item.NumIncrementPercentageOrAmount / 100) :
                                                       item.StrIncrementDependOn.ToLower() == "Basic".ToLower() ? salaryInfo.NumBasicOrgross * (item.NumIncrementPercentageOrAmount / 100) : 0;
                                incrementData.DteEffectiveDate = item.DteEffectiveDate;
                                incrementData.IntTransferNpromotionReferenceId = transferNpromotionReferenceId;
                                incrementData.IsActive = item.IsActive;
                                incrementData.DteUpdatedAt = DateTime.Now;
                                incrementData.IntUpdatedBy = item.IntCreatedBy;
                                incrementData.IntPipelineHeaderId = incrementPipeline.HeaderId;
                                incrementData.IntCurrentStage = incrementPipeline.CurrentStageId;
                                incrementData.IntNextStage = incrementPipeline.NextStageId;
                                incrementData.StrStatus = "Pending";
                                incrementData.IsPipelineClosed = false;
                                incrementData.IsReject = false;
                            }

                            _context.EmpEmployeeIncrements.Update(incrementData);
                            await _context.SaveChangesAsync();

                            return new MessageHelperCreate()
                            {
                                StatusCode = 200,
                                Message = "Updated successfully"
                            };
                        }
                        else
                        {
                            return new MessageHelperCreate()
                            {
                                StatusCode = 500,
                                Message = "Request is already in approval pipeline"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIncrementPaginationVM> GetEmployeeIncrementLanding(long accountId, long businessUnitId, long? workplaceGroupId, DateTime? dteFromDate, DateTime? dteToDate, int PageNo, int PageSize, string? searchTxt)
        {
            if (dteFromDate == null) dteFromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (dteToDate == null) dteToDate = DateTime.Now.Date;

            IQueryable<CRUDEmployeeIncrementVM> data = (from emp in _context.EmpEmployeeIncrements
                                                        join employee in _context.EmpEmployeeBasicInfos on emp.IntEmployeeId equals employee.IntEmployeeBasicInfoId
                                                        join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                        from desig in desig2.DefaultIfEmpty()
                                                        join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                        from dept in dpt2.DefaultIfEmpty()
                                                        join tpe1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals tpe1.IntEmploymentTypeId into tpe2
                                                        from tpe in tpe2.DefaultIfEmpty()
                                                        join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                        from bus in bus2.DefaultIfEmpty()
                                                        join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                        from acc in acc2.DefaultIfEmpty()
                                                        where emp.IntAccountId == accountId && emp.IntBusinessUnitId == businessUnitId && emp.IsActive == true
                                                        && (emp.DteEffectiveDate >= dteFromDate && emp.DteEffectiveDate <= dteToDate)
                                                        && (workplaceGroupId == null || workplaceGroupId <= 0 || employee.IntWorkplaceGroupId == workplaceGroupId)
                                                        orderby emp.DteCreatedAt descending
                                                        select new CRUDEmployeeIncrementVM
                                                        {
                                                            IntIncrementId = emp.IntIncrementId,
                                                            IntEmployeeId = emp.IntEmployeeId,
                                                            StrEmployeeName = emp.StrEmployeeName,
                                                            IntAccountId = (long)emp.IntAccountId,
                                                            IntBusinessUnitId = (long)emp.IntBusinessUnitId,
                                                            StrIncrementDependOn = emp.StrIncrementDependOn,
                                                            NumIncrementPercentageOrAmount = emp.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? emp.NumIncrementAmount : emp.NumIncrementPercentage,
                                                            DteEffectiveDate = emp.DteEffectiveDate,
                                                            IsActive = emp.IsActive,
                                                            IntCreatedBy = emp.IntCreatedBy,
                                                            IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                            StrEmploymentType = tpe.StrEmploymentType,
                                                            IntDesignationId = emp.IntDesignationId,
                                                            StrDesignation = desig.StrDesignation,
                                                            IntDepartmentId = emp.IntDepartmentId,
                                                            StrDepartment = dept.StrDepartment,
                                                            StrEmployeeCode = employee.StrEmployeeCode,
                                                            StrStatus = emp.StrStatus,
                                                            IsPromotion = (emp.IntTransferNpromotionReferenceId == 0 || emp.IntTransferNpromotionReferenceId == null) ? false : true,
                                                            IntTransferNpromotionReferenceId = emp.IntTransferNpromotionReferenceId,
                                                            NumIncrementAmount = emp.NumIncrementAmount
                                                        }).OrderByDescending(x => x.IntIncrementId).AsQueryable().AsNoTracking();

            GetIncrementPaginationVM retObj = new GetIncrementPaginationVM();
            int maxSize = 1000;
            PageSize = PageSize > maxSize ? maxSize : PageSize;
            PageNo = PageNo < 1 ? 1 : PageNo;

            if (!string.IsNullOrEmpty(searchTxt))
            {
                searchTxt = searchTxt.ToLower();
                data = data.Where(x => x.StrEmployeeName.ToLower().Contains(searchTxt) || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrEmployeeCode.ToLower().Contains(searchTxt)
                || x.StrDesignation.ToLower().Contains(searchTxt) || x.StrDepartment.ToLower().Contains(searchTxt));
            }

            retObj.TotalCount = await data.CountAsync();
            retObj.Data = await data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToListAsync();
            retObj.PageSize = PageSize;
            retObj.CurrentPage = PageNo;

            return retObj;

        }

        public async Task<CRUDIncrementPromotionTransferVM> GetEmployeeIncrementById(long autoId)
        {
            CRUDEmployeeIncrementVM data = await (from emp in _context.EmpEmployeeIncrements
                                                  join employee in _context.EmpEmployeeBasicInfos on emp.IntEmployeeId equals employee.IntEmployeeBasicInfoId
                                                  join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                  from desig in desig2.DefaultIfEmpty()
                                                  join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                  from dept in dpt2.DefaultIfEmpty()
                                                  join tpe1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals tpe1.IntEmploymentTypeId into tpe2
                                                  from tpe in tpe2.DefaultIfEmpty()
                                                  join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                  from bus in bus2.DefaultIfEmpty()
                                                  join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                  from acc in acc2.DefaultIfEmpty()
                                                  where emp.IntIncrementId == autoId
                                                  select new CRUDEmployeeIncrementVM
                                                  {
                                                      IntIncrementId = emp.IntIncrementId,
                                                      IntEmployeeId = emp.IntEmployeeId,
                                                      StrEmployeeName = emp.StrEmployeeName,
                                                      IntAccountId = (long)emp.IntAccountId,
                                                      IntBusinessUnitId = (long)emp.IntBusinessUnitId,
                                                      IntWorkplaceGroupId = (long)employee.IntWorkplaceGroupId,
                                                      StrIncrementDependOn = emp.StrIncrementDependOn,
                                                      NumIncrementPercentageOrAmount = emp.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? emp.NumIncrementAmount : emp.NumIncrementPercentage,
                                                      DteEffectiveDate = emp.DteEffectiveDate,
                                                      IsActive = emp.IsActive,
                                                      IntCreatedBy = emp.IntCreatedBy,
                                                      IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                      StrEmploymentType = tpe.StrEmploymentType,
                                                      IntDesignationId = emp.IntDesignationId,
                                                      StrDesignation = desig.StrDesignation,
                                                      IntDepartmentId = emp.IntDepartmentId,
                                                      StrDepartment = dept.StrDepartment,
                                                      StrEmployeeCode = employee.StrEmployeeCode,
                                                      StrStatus = emp.StrStatus,
                                                      IntTransferNpromotionReferenceId = emp.IntTransferNpromotionReferenceId,
                                                      NumIncrementAmount = emp.NumIncrementAmount
                                                  }).FirstOrDefaultAsync();

            if (data == null)
            {
                return new CRUDIncrementPromotionTransferVM();
            }

            var promTrans = await GetEmpTransferNpromotionById((long)data.IntTransferNpromotionReferenceId);

            return new CRUDIncrementPromotionTransferVM()
            {
                isPromotion = data.IntTransferNpromotionReferenceId > 0 ? true : false,
                incrementList = new List<CRUDEmployeeIncrementVM>() { data },
                transferPromotionObj = promTrans
            };
        }

        public async Task UpdateSalaryBreakDownDueToIncrement(long employeeId, decimal incrementedAmount)
        {
            try
            {
                decimal newGrossAmount = 0;

                /// R O W
                List<PyrEmployeeSalaryElementAssignRow> salaryElementRow = await (from head in _context.PyrEmployeeSalaryElementAssignHeaders
                                                                                  join row in _context.PyrEmployeeSalaryElementAssignRows on head.IntEmpSalaryElementAssignHeaderId equals row.IntEmpSalaryElementAssignHeaderId
                                                                                  where head.IntEmployeeId == employeeId && head.IsActive == true && row.IsActive == true && head.IsPerdaySalary == false
                                                                                  select row).ToListAsync();

                newGrossAmount = salaryElementRow.Select(x => x.NumAmount).Sum() + incrementedAmount;

                salaryElementRow.ForEach(item =>
                {
                    item.NumAmount = newGrossAmount * (decimal)(item.NumNumberOfPercent / 100);
                });

                _context.PyrEmployeeSalaryElementAssignRows.UpdateRange(salaryElementRow);

                /// H E A D E R
                PyrEmployeeSalaryElementAssignHeader salaryElementHead = await (from head in _context.PyrEmployeeSalaryElementAssignHeaders
                                                                                where head.IntEmployeeId == employeeId && head.IsActive == true
                                                                                select head).FirstOrDefaultAsync();

                string dependOn = await (from head in _context.PyrSalaryBreakdownHeaders
                                         where head.IntSalaryBreakdownHeaderId == salaryElementHead.IntSalaryBreakdownHeaderId && head.IsActive == true
                                         select head.StrDependOn).FirstOrDefaultAsync();

                salaryElementHead.NumGrossSalary = newGrossAmount;
                salaryElementHead.NumBasicOrgross = dependOn.ToLower().Contains("basic") ? salaryElementRow.Where(x => x.StrSalaryElement.ToLower().Contains("basic")).Select(x => x.NumAmount).FirstOrDefault() : newGrossAmount;

                _context.PyrEmployeeSalaryElementAssignHeaders.UpdateRange(salaryElementHead);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CRUDEmployeeIncrementVM>> GetEmployeeIncrementByEmoloyeeId(long IntAccountId, long IntEmployeeId)
        {
            try
            {
                List<CRUDEmployeeIncrementVM> data = await (from emp in _context.EmpEmployeeIncrements
                                                            join employee in _context.EmpEmployeeBasicInfos on emp.IntEmployeeId equals employee.IntEmployeeBasicInfoId
                                                            join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                            from desig in desig2.DefaultIfEmpty()
                                                            join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                            from dept in dpt2.DefaultIfEmpty()
                                                            join tpe1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals tpe1.IntEmploymentTypeId into tpe2
                                                            from tpe in tpe2.DefaultIfEmpty()
                                                            join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                                            from bus in bus2.DefaultIfEmpty()
                                                                //join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                                                //from acc in acc2.DefaultIfEmpty()
                                                            where emp.IntAccountId == IntAccountId && emp.IntEmployeeId == IntEmployeeId
                                                            && emp.IsActive == true
                                                            select new CRUDEmployeeIncrementVM
                                                            {
                                                                IntIncrementId = emp.IntIncrementId,
                                                                IntEmployeeId = emp.IntEmployeeId,
                                                                StrEmployeeName = emp.StrEmployeeName,
                                                                IntAccountId = (long)emp.IntAccountId,
                                                                IntBusinessUnitId = (long)emp.IntBusinessUnitId,
                                                                NumOldGrossAmount = emp.NumOldGrossAmount,
                                                                NumCurrentGrossAmount = emp.NumOldGrossAmount + emp.NumIncrementAmount,
                                                                StrIncrementDependOn = emp.StrIncrementDependOn,
                                                                NumIncrementPercentageOrAmount = emp.StrIncrementDependOn.ToLower() == "Amount".ToLower() ? emp.NumIncrementAmount : emp.NumIncrementPercentage,
                                                                DteEffectiveDate = emp.DteEffectiveDate,
                                                                IsActive = emp.IsActive,
                                                                IntCreatedBy = emp.IntCreatedBy,
                                                                IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                                                StrEmploymentType = tpe.StrEmploymentType,
                                                                IntDesignationId = emp.IntDesignationId,
                                                                StrDesignation = desig.StrDesignation,
                                                                IntDepartmentId = emp.IntDepartmentId,
                                                                StrDepartment = dept.StrDepartment,
                                                                StrEmployeeCode = employee.StrEmployeeCode,
                                                                StrStatus = emp.StrStatus,
                                                                IntTransferNpromotionReferenceId = emp.IntTransferNpromotionReferenceId,
                                                                NumIncrementAmount = emp.NumIncrementAmount
                                                            }).ToListAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion ================ Transfer & Promotion ===================

        #region PF Gratuity

        public async Task<MessageHelperCreate> CRUDEmpPfngratuity(CRUDEmpPfngratuityVM obj)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();

                if (obj.IntPfngratuityId > 0) // update
                {
                    EmpPfngratuity update = await _context.EmpPfngratuities.Where(x => x.IntPfngratuityId == obj.IntPfngratuityId && x.IsActive == true).FirstOrDefaultAsync();

                    if (update != null)
                    {
                        update.IntPfngratuityId = obj.IntPfngratuityId;
                        update.IsHasPfpolicy = obj.IsHasPfpolicy;
                        update.IntNumOfEligibleYearForBenifit = obj.IntNumOfEligibleYearForBenifit;
                        update.NumEmployeeContributionOfBasic = obj.NumEmployeeContributionOfBasic;
                        update.NumEmployerContributionOfBasic = obj.NumEmployerContributionOfBasic;
                        update.IntNumOfEligibleMonthForPfinvestment = obj.IntNumOfEligibleMonthForPfinvestment;
                        update.IsHasGratuityPolicy = obj.IsHasGratuityPolicy;
                        update.IntNumOfEligibleYearForGratuity = obj.IntNumOfEligibleYearForGratuity;
                        update.DteUpdatedAt = DateTime.Now;
                        update.IntUpdatedBy = obj.IntCreatedBy;

                        _context.EmpPfngratuities.Update(update);
                        await _context.SaveChangesAsync();

                        res.StatusCode = 200;
                        res.Message = "Updated successfully";
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Data not found for update";
                    }
                }
                else // create
                {
                    if (_context.EmpPfngratuities.Where(x => x.IntAccountId == obj.IntAccountId && x.IsActive == true).Count() > 0)
                    {
                        res.StatusCode = 500;
                        res.Message = "PF & Gratuity already exist in this Account.";
                    }
                    else
                    {
                        EmpPfngratuity create = new EmpPfngratuity()
                        {
                            IntAccountId = obj.IntAccountId,
                            IsHasPfpolicy = obj.IsHasPfpolicy,
                            IntNumOfEligibleYearForBenifit = obj.IntNumOfEligibleYearForBenifit,
                            NumEmployeeContributionOfBasic = obj.NumEmployeeContributionOfBasic,
                            NumEmployerContributionOfBasic = obj.NumEmployerContributionOfBasic,
                            IntNumOfEligibleMonthForPfinvestment = obj.IntNumOfEligibleMonthForPfinvestment,
                            IsHasGratuityPolicy = obj.IsHasGratuityPolicy,
                            IntNumOfEligibleYearForGratuity = obj.IntNumOfEligibleYearForGratuity,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = obj.IntCreatedBy
                        };
                        await _context.EmpPfngratuities.AddAsync(create);
                        await _context.SaveChangesAsync();

                        res.StatusCode = 200;
                        res.Message = "Created successfully";
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CRUDEmpPfngratuityVM> GetEmpPfngratuity(long AccountId)
        {
            try
            {
                return await _context.EmpPfngratuities.Where(x => x.IntAccountId == AccountId && x.IsActive == true)
                                                    .Select(x => new CRUDEmpPfngratuityVM()
                                                    {
                                                        IntPfngratuityId = x.IntPfngratuityId,
                                                        IntAccountId = x.IntAccountId,
                                                        IsHasPfpolicy = x.IsHasPfpolicy,
                                                        IntNumOfEligibleYearForBenifit = x.IntNumOfEligibleYearForBenifit,
                                                        NumEmployeeContributionOfBasic = x.NumEmployeeContributionOfBasic,
                                                        NumEmployerContributionOfBasic = x.NumEmployerContributionOfBasic,
                                                        IntNumOfEligibleMonthForPfinvestment = x.IntNumOfEligibleMonthForPfinvestment,
                                                        IsHasGratuityPolicy = x.IsHasGratuityPolicy,
                                                        IntNumOfEligibleYearForGratuity = x.IntNumOfEligibleYearForGratuity,
                                                        IntCreatedBy = x.IntCreatedBy,
                                                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PFInvestmentPagination> GetPFInvestmentLanding(long accountId, long businessUnitId, string? searchTxt, int? pageNo, int? pageSize, DateTime dteFromDate, DateTime dteToDate)
        {
            try
            {
                IQueryable<PFInvestmentHeaderVM> data = (from head in _context.EmpPfInvestmentHeaders
                                                         where head.IntAccountId == accountId && head.IntBusinessUnitId == businessUnitId && head.IsActive == true
                                                         where head.DteInvestmentDate.Date >= dteFromDate && head.DteInvestmentDate.Date <= dteToDate
                                                         orderby head.DteInvestmentDate descending
                                                         select new PFInvestmentHeaderVM
                                                         {
                                                             IntInvenstmentHeaderId = head.IntInvenstmentHeaderId,
                                                             IntAccountId = head.IntAccountId,
                                                             IntBusinessUnitId = head.IntBusinessUnitId,
                                                             DtePfPeriodFromMonthYear = head.DtePfPeriodFromMonthYear,
                                                             DtePfPeriodToMonthYear = head.DtePfPeriodToMonthYear,
                                                             StrInvestmentCode = head.StrInvestmentCode,
                                                             StrInvestmentReffNo = head.StrInvestmentReffNo,
                                                             DteInvestmentDate = head.DteInvestmentDate,
                                                             DteMatureDate = head.DteMatureDate,
                                                             NumInterestRate = head.NumInterestRate,
                                                             IntBankId = head.IntBankId,
                                                             StrBankName = head.StrBankName,
                                                             IntBankBranchId = head.IntBankBranchId,
                                                             StrBankBranchName = head.StrBankBranchName,
                                                             StrRoutingNo = head.StrRoutingNo,
                                                             StrAccountName = head.StrAccountName,
                                                             StrAccountNumber = head.StrAccountNumber,
                                                             IsActive = head.IsActive,
                                                             IntCreatedBy = head.IntCreatedBy,
                                                             NumInvestmentAmount = _context.EmpPfInvestmentRows.Where(x => x.IntInvenstmentHeaderId == head.IntInvenstmentHeaderId && x.IsActive == true).Select(x => x.NumTotalAmount).Sum(),
                                                             NumInterestAmount = _context.EmpPfInvestmentRows.Where(x => x.IntInvenstmentHeaderId == head.IntInvenstmentHeaderId && x.IsActive == true).Select(x => x.NumTotalAmount).Sum() * head.NumInterestRate,

                                                             StrStatus = DateTime.Now.Date < head.DteMatureDate.Value.Date ? "Active" : "Completed"
                                                         }).AsNoTracking().AsQueryable();
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    searchTxt = searchTxt.ToLower();
                    data = data.Where(x => x.StrInvestmentCode.ToLower().Contains(searchTxt) || x.StrInvestmentReffNo.ToLower().Contains(searchTxt));
                }

                PFInvestmentPagination retObj = new PFInvestmentPagination
                {
                    Data = await data.ToListAsync()
                };

                return retObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PFInvestmentRowVM>> GetPFInvestmentById(long HeaderId)
        {
            try
            {
                EmpPfInvestmentHeader headerTable = await _context.EmpPfInvestmentHeaders.Where(x => x.IntInvenstmentHeaderId == HeaderId).FirstOrDefaultAsync();

                List<PFInvestmentRowVM> data = await (from rrw in _context.EmpPfInvestmentRows
                                                      join emp1 in _context.EmpEmployeeBasicInfos on rrw.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                                                      from emp in emp2.DefaultIfEmpty()
                                                      join desig1 in _context.MasterDesignations on rrw.IntDesignationId equals desig1.IntDesignationId into desig2
                                                      from desig in desig2.DefaultIfEmpty()
                                                      join dpt1 in _context.MasterDepartments on rrw.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                      from dept in dpt2.DefaultIfEmpty()
                                                      join tpe1 in _context.MasterEmploymentTypes on rrw.IntEmploymentTypeId equals tpe1.IntEmploymentTypeId into tpe2
                                                      from tpe in tpe2.DefaultIfEmpty()
                                                      where rrw.IntInvenstmentHeaderId == HeaderId && rrw.IsActive == true
                                                      select new PFInvestmentRowVM
                                                      {
                                                          IntRowId = rrw.IntRowId,
                                                          IntInvenstmentHeaderId = rrw.IntInvenstmentHeaderId,
                                                          IntEmployeeId = rrw.IntEmployeeId,
                                                          StrEmployeeName = rrw.StrEmployeeName,
                                                          StrEmployeeCode = emp.StrEmployeeCode,
                                                          IntDesignationId = rrw.IntDesignationId,
                                                          StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                          IntDepartmentId = rrw.IntDepartmentId,
                                                          StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                          IntEmploymentTypeId = rrw.IntEmploymentTypeId,
                                                          StrEmploymentType = tpe == null ? "" : tpe.StrEmploymentType,
                                                          StrServiceLength = rrw.StrServiceLength,
                                                          NumEmployeeContribution = rrw.NumEmployeeContribution,
                                                          NumEmployerContribution = rrw.NumEmployerContribution,
                                                          NumTotalAmount = rrw.NumTotalAmount,
                                                          IsActive = rrw.IsActive,
                                                          IntCreatedBy = rrw.IntCreatedBy,

                                                          NumInterestRate = headerTable.NumInterestRate,
                                                          NumInterestAmount = rrw.NumTotalAmount * headerTable.NumInterestRate,
                                                          NumGrandTotalAmount = rrw.NumTotalAmount + (rrw.NumTotalAmount * headerTable.NumInterestRate),
                                                      }).OrderByDescending(x => x.IntRowId).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PFInvestmentHeaderVM> GetValidPFInvestmentPeriod(long AccountId, long BusinessUnitId)
        {
            try
            {
                PFInvestmentHeaderVM res = new PFInvestmentHeaderVM();
                DateTime investmentPeriodStart;

                var pfConfig = await _context.EmpPfngratuities.Where(x => x.IntAccountId == AccountId && x.IsActive == true).FirstOrDefaultAsync();
                if (pfConfig == null) throw new Exception("Please Configure Company PF");
                int eligibleMonth = (int)pfConfig.IntNumOfEligibleMonthForPfinvestment - 1;

                var lastInvestment = await _context.EmpPfInvestmentHeaders.Where(x => x.IntAccountId == AccountId && x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true)
                                                                          .OrderByDescending(x => x.DtePfPeriodToMonthYear).FirstOrDefaultAsync();
                if (lastInvestment == null)
                {
                    var firstSalaryDate = await _context.PyrPayrollSalaryGenerateRequests
                                                        .Where(x => x.IntAccountId == AccountId && x.IntBusinessUnitId == BusinessUnitId && x.IsActive == true && x.IsPipelineClosed == true && x.IsReject == false)
                                                        .OrderBy(x => x.IntYear).ThenBy(x => x.IntMonth)
                                                        .Select(x => new { date = new DateTime((int)x.IntYear, (int)x.IntMonth, 1) })
                                                        .FirstOrDefaultAsync();

                    if (firstSalaryDate == null) throw new Exception("Salary not found to invest");

                    investmentPeriodStart = firstSalaryDate.date;
                }
                else
                {
                    investmentPeriodStart = lastInvestment.DtePfPeriodToMonthYear.Date.AddMonths(1);
                }

                res.DtePfPeriodFromMonthYear = DateTimeExtension.GetFirstDayOfMonth(investmentPeriodStart);
                res.DtePfPeriodToMonthYear = DateTimeExtension.GetLastDayOfMonth(investmentPeriodStart.AddMonths(eligibleMonth));
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PFInvestmentRowVM>> GetEmployeeDataForPFInvestment(long accountId, long businessUnitId, DateTime fromMonthYear, DateTime toMonthYear)
        {
            fromMonthYear = DateTimeExtension.GetFirstDayOfMonth(fromMonthYear);
            toMonthYear = DateTimeExtension.GetLastDayOfMonth(toMonthYear);

            var pfConfig = await _context.EmpPfngratuities.Where(x => x.IntAccountId == accountId && x.IsActive == true).FirstOrDefaultAsync();
            if (pfConfig == null) throw new Exception("Please Configure Company PF");

            var totalInvestmentMonth = DateTimeExtension.GetMonthDifference(fromMonthYear, toMonthYear) + 1;

            var salaryDataV1 = await (from sal in _context.PyrPayrollSalaryGenerateRequests
                                      where sal.IntAccountId == accountId && sal.IntBusinessUnitId == businessUnitId
                                      && sal.IsActive == true && sal.IsPipelineClosed == true && sal.IsReject == false
                                      select new
                                      {
                                          accountId = sal.IntAccountId,
                                          businessUnitId = sal.IntBusinessUnitId,
                                          salaryDate = new DateTime((int)sal.IntYear, (int)sal.IntMonth, 1)
                                      }).ToListAsync();

            var salaryData = salaryDataV1.Where(sal => fromMonthYear.Date <= sal.salaryDate.Date && sal.salaryDate.Date <= toMonthYear.Date)
                                    .GroupBy(grp => new
                                    {
                                        accountId = grp.accountId,
                                        businessUnitId = grp.businessUnitId,
                                        salaryDate = grp.salaryDate,
                                    })
                                    .Select(grp => new
                                    {
                                        accountId = grp.Key.accountId,
                                        businessUnitId = grp.Key.businessUnitId,
                                        salaryDate = grp.Key.salaryDate,
                                    }).ToList();

            if ((pfConfig.IntNumOfEligibleMonthForPfinvestment > salaryData.Count())
                || (pfConfig.IntNumOfEligibleMonthForPfinvestment > totalInvestmentMonth)
                || (totalInvestmentMonth > salaryData.Count()))
                throw new Exception("Generated salary don't meet minimum required month to invest");

            List<PFInvestmentRowVM> empSalaryList = await (
                                        from sal in _context.PyrPayrollSalaryGenerateRequests
                                        join head in _context.PyrSalaryGenerateHeaders on sal.IntSalaryGenerateRequestId equals head.IntSalaryGenerateRequestId
                                        where sal.IntAccountId == accountId && sal.IntBusinessUnitId == businessUnitId
                                        && sal.IsActive == true && sal.IsPipelineClosed == true && sal.IsReject == false
                                        && head.IsActive == true
                                        select new PFInvestmentRowVM
                                        {
                                            IntEmployeeId = (long)head.IntEmployeeId,
                                            StrEmployeeName = head.StrEmployeeName,
                                            StrEmployeeCode = head.StrEmployeeCode,
                                            IntDesignationId = head.IntDesignationId,
                                            StrDesignation = head.StrDesignation,
                                            IntDepartmentId = head.IntDepartmentId,
                                            StrDepartment = head.StrDepartment,
                                            IntEmploymentTypeId = head.IntEmploymentTypeId,
                                            StrEmploymentType = head.StrEmploymentType,
                                            StrServiceLength = head.StrServiceLength,
                                            NumEmployeeContribution = head.NumPfamount == null ? 0 : (decimal)head.NumPfamount,
                                            NumEmployerContribution = head.NumPfcompany == null ? 0 : (decimal)head.NumPfcompany,
                                            NumTotalAmount = head.NumPfamount == null ? 0 : (decimal)head.NumPfamount + head.NumPfcompany == null ? 0 : (decimal)head.NumPfcompany,
                                            //for condition check
                                            salaryDate = new DateTime((int)sal.IntYear, (int)sal.IntMonth, 1)
                                        })
                                        .Where(sal => fromMonthYear.Date <= sal.salaryDate.Date && sal.salaryDate.Date <= toMonthYear.Date)
                                        .ToListAsync();

            return empSalaryList;
        }

        public async Task<MessageHelperCreate> CreatePFInvestment(CRUDPFInvestmentVM obj)
        {
            if (obj.rowList.Count() == 0) throw new Exception("There is no employee salary to invest");

            var createHeader = new EmpPfInvestmentHeader()
            {
                IntAccountId = obj.header.IntAccountId,
                IntBusinessUnitId = obj.header.IntBusinessUnitId,
                DtePfPeriodFromMonthYear = obj.header.DtePfPeriodFromMonthYear,
                DtePfPeriodToMonthYear = obj.header.DtePfPeriodToMonthYear,
                StrInvestmentCode = obj.header.StrInvestmentCode,
                StrInvestmentReffNo = obj.header.StrInvestmentReffNo,
                DteInvestmentDate = obj.header.DteInvestmentDate,
                DteMatureDate = obj.header.DteMatureDate,
                NumInterestRate = obj.header.NumInterestRate,
                IntBankId = obj.header.IntBankId,
                StrBankName = obj.header.StrBankName,
                IntBankBranchId = obj.header.IntBankBranchId,
                StrBankBranchName = obj.header.StrBankBranchName,
                StrRoutingNo = obj.header.StrRoutingNo,
                StrAccountName = obj.header.StrAccountName,
                StrAccountNumber = obj.header.StrAccountNumber,
                IsActive = true,
                DteCreatedAt = DateTime.Now,
                IntCreatedBy = obj.header.IntCreatedBy,
            };
            await _context.EmpPfInvestmentHeaders.AddAsync(createHeader);
            await _context.SaveChangesAsync();

            var createRow = new List<EmpPfInvestmentRow>();

            createRow.AddRange(obj.rowList.Select(x => new EmpPfInvestmentRow()
            {
                IntInvenstmentHeaderId = createHeader.IntInvenstmentHeaderId,
                IntEmployeeId = x.IntEmployeeId,
                StrEmployeeName = x.StrEmployeeName,
                IntDesignationId = x.IntDesignationId,
                IntDepartmentId = x.IntDepartmentId,
                IntEmploymentTypeId = x.IntEmploymentTypeId,
                StrServiceLength = x.StrServiceLength,
                NumEmployeeContribution = x.NumEmployeeContribution,
                NumEmployerContribution = x.NumEmployerContribution,
                NumTotalAmount = x.NumEmployeeContribution + x.NumEmployerContribution,
                IsActive = true,
                DteCreatedAt = DateTime.Now,
                IntCreatedBy = x.IntCreatedBy,
            }));

            await _context.EmpPfInvestmentRows.AddRangeAsync(createRow);
            await _context.SaveChangesAsync();

            return new MessageHelperCreate()
            {
                Message = "Created successfully",
                StatusCode = 200
            };
        }

        public async Task<PFNGratuityViewModel> PfNGratuityLanding(long IntAccountId, long IntEmployeeId)
        {
            try
            {
                PFNGratuityViewModel pFNGratuityViewModel = new PFNGratuityViewModel();

                EmpEmployeeBasicInfo employeeBasicInfo = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == IntEmployeeId && x.IntAccountId == IntAccountId && x.IsActive == true && x.DteLastWorkingDate == null).FirstOrDefault();

                if (employeeBasicInfo != null)
                {
                    pFNGratuityViewModel.PfAccountViewModel = await PfNGratuityCount(employeeBasicInfo);

                    List<FiscalYearWisePFInfoViewModel> fiscalYearWisePFInfo = (from y in _context.FiscalYears
                                                                                where y.IsActive == true
                                                                                select new FiscalYearWisePFInfoViewModel
                                                                                {
                                                                                    intYearId = y.IntYearId,
                                                                                    strFiscalYear = y.StrFiscalYear,
                                                                                    dteFiscalFromDate = y.DteFiscalFromDate,
                                                                                    dteFiscalToDate = y.DteFiscalToDate,
                                                                                    PFInformationViewModel = (from sr in _context.PyrPayrollSalaryGenerateRequests
                                                                                                              join pf in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals pf.IntSalaryGenerateRequestId
                                                                                                              where pf.IsActive == true && sr.IsActive == true && sr.IntAccountId == IntAccountId
                                                                                                              && sr.IsPipelineClosed == true && sr.IsReject == false
                                                                                                              && pf.IntEmployeeId == IntEmployeeId
                                                                                                              && pf.DteSalaryGenerateFor.Value.Date >= y.DteFiscalFromDate.Date
                                                                                                              && pf.DteSalaryGenerateFor.Value.Date <= y.DteFiscalToDate.Date
                                                                                                              select new PFInformationViewModel
                                                                                                              {
                                                                                                                  //Month = pf.DteSalaryGenerateFor.Value.Month,
                                                                                                                  Month = pf.DteSalaryGenerateFor.Value.ToString("MMMM"),
                                                                                                                  EmployeeContribution = pf.NumPfamount,
                                                                                                                  OrgContribution = pf.NumPfcompany,
                                                                                                                  TotalPfAmount = pf.NumPfamount - pf.NumPfcompany
                                                                                                              }).ToList()
                                                                                }).ToList();

                    pFNGratuityViewModel.FiscalYearWisePFInfoViewModel = fiscalYearWisePFInfo;
                }
                pFNGratuityViewModel.PFWithdrawViewModel = await PfWithdrawLanding(IntAccountId, IntEmployeeId);

                return pFNGratuityViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PfAccountViewModel> PfNGratuityCount(EmpEmployeeBasicInfo employeeBasicInfo)
        {
            PfAccountViewModel pfAccount = new PfAccountViewModel();

            int LengthYears = (DateTime.Now.Year - employeeBasicInfo.DteJoiningDate.Value.Year - 1) + (((DateTime.Now.Month > employeeBasicInfo.DteJoiningDate.Value.Month) || ((DateTime.Now.Month == employeeBasicInfo.DteJoiningDate.Value.Month) && (DateTime.Now.Day >= employeeBasicInfo.DteJoiningDate.Value.Day))) ? 1 : 0);

            decimal basicSalary = await _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == employeeBasicInfo.IntEmployeeBasicInfoId && x.IsActive == true
            && x.IntAccountId == employeeBasicInfo.IntAccountId).Select(x => x.NumBasicOrgross).FirstOrDefaultAsync();

            EmpPfngratuity empPfngratuity = _context.EmpPfngratuities.Where(x => x.IsActive == true && x.IntAccountId == employeeBasicInfo.IntAccountId && x.IsHasGratuityPolicy == true).FirstOrDefault();

            List<PyrSalaryGenerateHeader> pyrSalaryGenerate = (from sr in _context.PyrPayrollSalaryGenerateRequests
                                                               join sg in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals sg.IntSalaryGenerateRequestId
                                                               where sr.IsActive == true && sg.IsActive == true && sr.IntAccountId == employeeBasicInfo.IntAccountId
                                                               && sr.IsPipelineClosed == true && sr.IsReject == false
                                                               && sg.IntEmployeeId == employeeBasicInfo.IntEmployeeBasicInfoId
                                                               select sg).ToList();

            List<EmpPfwithdraw> pfwithdraw = _context.EmpPfwithdraws.Where(x => x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true && x.IntEmployeeId == employeeBasicInfo.IntEmployeeBasicInfoId && x.IntAccountId == employeeBasicInfo.IntAccountId).ToList();

            if (empPfngratuity != null && LengthYears >= empPfngratuity.IntNumOfEligibleYearForGratuity)
            {
                pfAccount = new PfAccountViewModel
                {
                    Gratuity = basicSalary * empPfngratuity.IntNumOfEligibleYearForGratuity,
                    EmployeePFContribution = pyrSalaryGenerate.Sum(x => x.NumPfamount),
                    EmployerPFContribution = pyrSalaryGenerate.Sum(x => x.NumPfcompany),
                    TotalPFNGratuity = (basicSalary * empPfngratuity.IntNumOfEligibleYearForGratuity) + pyrSalaryGenerate.Sum(x => x.NumPfamount) + pyrSalaryGenerate.Sum(x => x.NumPfcompany),
                    TotalPFWithdraw = pfwithdraw.Sum(x => x.NumWithdrawAmount),
                    TotalAvailablePFNGratuity = ((basicSalary * empPfngratuity.IntNumOfEligibleYearForGratuity) + pyrSalaryGenerate.Sum(x => x.NumPfamount) + pyrSalaryGenerate.Sum(x => x.NumPfcompany)) - pfwithdraw.Sum(x => x.NumWithdrawAmount)
                };
            }
            return pfAccount;
        }

        public async Task<List<PFWithdrawViewModel>> PfWithdrawLanding(long IntAccountId, long IntEmployeeId)
        {
            List<PFWithdrawViewModel> pFWithdrawView = await (from pw in _context.EmpPfwithdraws
                                                              join emp in _context.EmpEmployeeBasicInfos on pw.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp1
                                                              from empi in emp1.DefaultIfEmpty()
                                                              join dep1 in _context.MasterDepartments on empi.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                                              from dep in dep2.DefaultIfEmpty()
                                                              join deg1 in _context.MasterDesignations on empi.IntDesignationId equals deg1.IntDesignationId into deg2
                                                              from desg in deg2.DefaultIfEmpty()
                                                              where pw.IsActive == true && pw.IntAccountId == IntAccountId
                                                              && dep.IsActive == true && desg.IsActive == true
                                                              && (pw.IntEmployeeId == IntEmployeeId || empi.IntSupervisorId == IntEmployeeId
                                                              || empi.IntDottedSupervisorId == IntEmployeeId || empi.IntLineManagerId == IntEmployeeId)
                                                              select new PFWithdrawViewModel
                                                              {
                                                                  intPFWithdrawId = pw.IntPfwithdrawId,
                                                                  intAccountId = pw.IntAccountId,
                                                                  intEmployeeId = pw.IntEmployeeId,
                                                                  strEmployee = empi.StrEmployeeName,
                                                                  strDepartment = dep.StrDepartment,
                                                                  strDesignation = desg.StrDesignation,
                                                                  dteApplicationDate = pw.DteApplicationDate,
                                                                  numWithdrawAmount = pw.NumWithdrawAmount,
                                                                  strReason = pw.StrReason,
                                                                  dteCreatedAt = pw.DteCreatedAt,
                                                                  intCreatedBy = pw.IntCreatedBy,
                                                                  isActive = pw.IsActive,
                                                                  strStatus = (pw.IsReject == false && pw.IsActive == true && pw.IsPipelineClosed == false) ? "Pending"
                                                                  : (pw.IsReject == false && pw.IsActive == true && pw.IsPipelineClosed == true) ? "Approved" : pw.IsReject == true ? "Reject"
                                                                  : (pw.IsPipelineClosed == false && pw.IsReject == false && pw.StrStatus.ToLower() != "Pending") ? "Process" : "",
                                                              }).ToListAsync();

            return pFWithdrawView;
        }

        public async Task<MessageHelper> CRUDPFWithdraw(PFWithdrawViewModel obj)
        {
            MessageHelper msg = new MessageHelper();
            try
            {
                if (obj.intPFWithdrawId > 0)
                {
                    EmpPfwithdraw pfwithdraw = await _context.EmpPfwithdraws.FirstOrDefaultAsync(x => x.IntPfwithdrawId == obj.intPFWithdrawId && x.IsActive == true);
                    if (pfwithdraw != null)
                    {
                        pfwithdraw.IntEmployeeId = obj.intEmployeeId;
                        pfwithdraw.StrEmployee = obj.strEmployee;
                        pfwithdraw.IntAccountId = obj.intAccountId;
                        pfwithdraw.DteApplicationDate = (DateTime)obj.dteApplicationDate;
                        pfwithdraw.NumWithdrawAmount = obj.numWithdrawAmount;
                        pfwithdraw.StrReason = obj.strReason;
                        pfwithdraw.IsActive = (bool)obj.isActive;
                        pfwithdraw.DteUpdatedAt = DateTime.Now;
                        pfwithdraw.IntUpdatedBy = obj.intCreatedBy;

                        _context.EmpPfwithdraws.Update(pfwithdraw);
                        await _context.SaveChangesAsync();

                        msg.StatusCode = 200;
                        msg.Message = obj.isActive == true ? "Update Successfully" : "Delete Successfully";
                    }
                    else
                    {
                        msg.StatusCode = 500;
                        msg.Message = "Data Not Found";
                    }
                }
                else
                {
                    if (_context.EmpPfwithdraws.Where(x => x.DteApplicationDate.Year == obj.dteApplicationDate.Value.Year
                    && x.DteApplicationDate.Month == obj.dteApplicationDate.Value.Month && x.IsActive == true && x.IsPipelineClosed == false && x.IsReject == false).Count() > 0)
                    {
                        msg.StatusCode = 500;
                        msg.Message = "PF Withdraw already exist in this month.";
                    }
                    else
                    {
                        PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId((long)obj.intAccountId, "pfWithdraw");

                        EmpPfwithdraw empPfwithdraw = new EmpPfwithdraw
                        {
                            IntEmployeeId = obj.intEmployeeId,
                            StrEmployee = obj.strEmployee,
                            IntAccountId = obj.intAccountId,
                            DteApplicationDate = (DateTime)obj.dteApplicationDate,
                            NumWithdrawAmount = obj.numWithdrawAmount,
                            StrReason = obj.strReason,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = (long)obj.intCreatedBy,
                            StrStatus = "Pending",
                            IntPipelineHeaderId = stage.HeaderId,
                            IntCurrentStage = stage.CurrentStageId,
                            IntNextStage = stage.NextStageId
                        };

                        await _context.EmpPfwithdraws.AddAsync(empPfwithdraw);
                        await _context.SaveChangesAsync();

                        msg.StatusCode = 200;
                        msg.Message = "Create Successfully";
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                msg.StatusCode = 500;
                msg.Message = ex.Message;

                return msg;
            }
        }

        #endregion PF Gratuity

        public long? EmployeeLeaveTaken(long intEmployeeId, DateTime FromDate, DateTime ToDate, long intLeaveTypeId)
        {
            try
            {
                var empLeaveTaken = (from lv in _context.LveLeaveApplications
                                     where lv.IsActive == true && lv.IsPipelineClosed == true && lv.IsReject == false
                                     && lv.IntEmployeeId == intEmployeeId && lv.IntLeaveTypeId == intLeaveTypeId
                                     && ((lv.DteFromDate >= FromDate && lv.DteFromDate <= ToDate)
                                        || (lv.DteToDate >= FromDate && lv.DteToDate <= ToDate)
                                        || (FromDate >= lv.DteFromDate && FromDate <= lv.DteToDate)
                                        || (ToDate >= lv.DteFromDate && ToDate <= lv.DteToDate))
                                     select new
                                     {
                                         days = YearMonthDayCalculate.CalculateDaysBetweenTwoDate(lv.DteFromDate, lv.DteToDate)
                                     }).AsNoTracking().ToList();


                return empLeaveTaken.Sum(x => x.days);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PromotionReportViewModel> PromotionReportForPdf(long IntTransferNPromotionId, long AccountId)
        {
            try
            {
                PromotionReportViewModel promotionReport = await (from pro in _context.EmpTransferNpromotions
                                                                  where pro.IntTransferNpromotionId == IntTransferNPromotionId
                                                                  join fromDep1 in _context.MasterDepartments on pro.IntDepartmentIdFrom equals fromDep1.IntDepartmentId into fromDep2
                                                                  from fromDep in fromDep2.DefaultIfEmpty()
                                                                  join fromDesig1 in _context.MasterDesignations on pro.IntDesignationIdFrom equals fromDesig1.IntDesignationId into fromDesig2
                                                                  from fromDesig in fromDesig2.DefaultIfEmpty()
                                                                  join fromBusi1 in _context.MasterBusinessUnits on pro.IntBusinessUnitIdFrom equals fromBusi1.IntBusinessUnitId into fromBusi2
                                                                  from fromBusi in fromBusi2.DefaultIfEmpty()
                                                                  join toDep1 in _context.MasterDepartments on pro.IntDepartmentId equals toDep1.IntDepartmentId into toDep2
                                                                  from toDep in toDep2.DefaultIfEmpty()
                                                                  join toDesig1 in _context.MasterDesignations on pro.IntDesignationIdFrom equals toDesig1.IntDesignationId into toDesig2
                                                                  from toDesig in toDesig2.DefaultIfEmpty()
                                                                  join toBusi1 in _context.MasterBusinessUnits on pro.IntBusinessUnitIdFrom equals toBusi1.IntBusinessUnitId into toBusi2
                                                                  from toBusi in toBusi2.DefaultIfEmpty()
                                                                  join approver in _context.EmpEmployeeBasicInfos on pro.IntUpdatedBy equals approver.IntEmployeeBasicInfoId into appv
                                                                  from approverEmp in appv.DefaultIfEmpty()
                                                                  join appvBusi1 in _context.MasterBusinessUnits on approverEmp.IntBusinessUnitId equals appvBusi1.IntBusinessUnitId into appvBu
                                                                  from appvBus in appvBu.DefaultIfEmpty()
                                                                  join appvDep1 in _context.MasterDepartments on approverEmp.IntDepartmentId equals appvDep1.IntDepartmentId into appvDp
                                                                  from appvDep in appvDp.DefaultIfEmpty()
                                                                  join appvDesg1 in _context.MasterDesignations on approverEmp.IntDesignationId equals appvDesg1.IntDesignationId into appvDsg
                                                                  from appvDesg in appvDsg.DefaultIfEmpty()
                                                                  where pro.IsActive == true && pro.IntAccountId == AccountId
                                                                  select new PromotionReportViewModel()
                                                                  {
                                                                      EmployeeId = pro.IntEmployeeId,
                                                                      EmployeeName = pro.StrEmployeeName,
                                                                      FromBusinessUnit = fromBusi != null ? fromBusi.StrBusinessUnit : "",
                                                                      FromDepartment = fromDep != null ? fromDep.StrDepartment : "",
                                                                      FromDesignation = fromDesig != null ? fromDesig.StrDesignation : "",
                                                                      ToBusinessUnit = toBusi != null ? toBusi.StrBusinessUnit : "",
                                                                      ToDepartment = toDep != null ? toDep.StrDepartment : "",
                                                                      ToDesignation = toDesig != null ? toDesig.StrDesignation : "",
                                                                      ReleaseDate = pro.DteReleaseDate,
                                                                      EffectiveDate = pro.DteEffectiveDate,
                                                                      ApproverName = approverEmp != null ? approverEmp.StrEmployeeName : "",
                                                                      ApproverDepartment = appvDep != null ? appvDep.StrDepartment : "",
                                                                      ApproverBusinessUnit = appvBus != null ? appvBus.StrBusinessUnit : "",
                                                                      ApproverDesignation = appvDesg != null ? appvDesg.StrDesignation : ""
                                                                  }).FirstOrDefaultAsync();
                return promotionReport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IncrementLetterViewModel> IncrementLetterForPdf(long IntIncrementLetterId, long AccountId)
        {
            try
            {
                IncrementLetterViewModel empIncrement = await (from inc in _context.EmpEmployeeIncrements
                                                               where inc.IntIncrementId == IntIncrementLetterId && inc.IntAccountId == AccountId
                                                               join empType1 in _context.MasterEmploymentTypes on inc.IntEmploymentTypeId equals empType1.IntEmploymentTypeId into empType2
                                                               from empType in empType2.DefaultIfEmpty()
                                                               join desig1 in _context.MasterDesignations on inc.IntDesignationId equals desig1.IntDesignationId into desig2
                                                               from desig in desig2.DefaultIfEmpty()
                                                               join dep1 in _context.MasterDepartments on inc.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                                               from dep in dep2.DefaultIfEmpty()
                                                               join busi1 in _context.MasterBusinessUnits on inc.IntBusinessUnitId equals busi1.IntBusinessUnitId into busi2
                                                               from busi in busi2.DefaultIfEmpty()
                                                               join approver in _context.EmpEmployeeBasicInfos on inc.IntUpdatedBy equals approver.IntEmployeeBasicInfoId into appv
                                                               from approverEmp in appv.DefaultIfEmpty()
                                                               join appvBusi1 in _context.MasterBusinessUnits on approverEmp.IntBusinessUnitId equals appvBusi1.IntBusinessUnitId into appvBu
                                                               from appvBus in appvBu.DefaultIfEmpty()
                                                               join appvDep1 in _context.MasterDepartments on approverEmp.IntDepartmentId equals appvDep1.IntDepartmentId into appvDp
                                                               from appvDep in appvDp.DefaultIfEmpty()
                                                               join appvDesg1 in _context.MasterDesignations on approverEmp.IntDesignationId equals appvDesg1.IntDesignationId into appvDsg
                                                               from appvDesg in appvDsg.DefaultIfEmpty()
                                                               where inc.IsActive == true && inc.IsPipelineClosed == true && inc.IsReject == false
                                                               select new IncrementLetterViewModel()
                                                               {
                                                                   IncrementLetterId = inc.IntIncrementId,
                                                                   EmployeeId = inc.IntEmployeeId,
                                                                   EmployeeName = inc.StrEmployeeName,
                                                                   EmploymentType = empType.StrEmploymentType,
                                                                   Designation = desig.StrDesignation,
                                                                   Department = dep.StrDepartment,
                                                                   BusinessUnit = busi.StrBusinessUnit,
                                                                   IncrementSalaryDate = inc.DteEffectiveDate,
                                                                   ApprovarName = approverEmp != null ? approverEmp.StrEmployeeName : "",
                                                                   ApprovarBusinessUnit = appvBus != null ? appvBus.StrBusinessUnit : "",
                                                                   ApprovarDepartment = appvDep != null ? appvDep.StrDepartment : "",
                                                                   ApprovarDesignation = appvDesg != null ? appvDesg.StrDesignation : "",

                                                                   existingSalaryViewModels = (from salaryHeader in _context.PyrEmployeeSalaryElementAssignHeaders
                                                                                               join salaryRow in _context.PyrEmployeeSalaryElementAssignRows on salaryHeader.IntEmpSalaryElementAssignHeaderId equals salaryRow.IntEmpSalaryElementAssignHeaderId
                                                                                               where salaryHeader.IntEmpSalaryElementAssignHeaderId == inc.IntOldSalaryElementAssignHeaderId
                                                                                               select new ExistingSalaryViewModel
                                                                                               {
                                                                                                   IntEmpSalaryElementAssignHeaderId = salaryRow.IntEmpSalaryElementAssignHeaderId,
                                                                                                   SalaryElementId = salaryRow.IntSalaryElementId,
                                                                                                   SalaryElement = salaryRow.StrSalaryElement,
                                                                                                   numAmount = (double)salaryRow.NumAmount
                                                                                               }).ToList(),

                                                                   incrementSalaryViewModels = (from salaryHeader in _context.PyrEmployeeSalaryElementAssignHeaders
                                                                                                join salaryRow in _context.PyrEmployeeSalaryElementAssignRows on salaryHeader.IntEmpSalaryElementAssignHeaderId equals salaryRow.IntEmpSalaryElementAssignHeaderId
                                                                                                where salaryHeader.IntEmpSalaryElementAssignHeaderId == inc.IntNewSalaryElementAssignHeaderId
                                                                                                && salaryHeader.IsActive == true && salaryRow.IsActive == true
                                                                                                select new IncrementSalaryViewModel()
                                                                                                {
                                                                                                    IntEmpSalaryElementAssignHeaderId = salaryRow.IntEmpSalaryElementAssignHeaderId,
                                                                                                    SalaryElementId = salaryRow.IntSalaryElementId,
                                                                                                    SalaryElement = salaryRow.StrSalaryElement,
                                                                                                    numAmount = (double)salaryRow.NumAmount
                                                                                                }).ToList()
                                                               }).FirstOrDefaultAsync();

                return empIncrement;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TransferAndPromotionReportPDFViewModel> TransferAndPromotionReportForPdf(long IntTransferAndPromotionId, long AccountId)
        {
            TransferAndPromotionReportPDFViewModel transferAndPromotionReportPDF = await (from tp in _context.EmpTransferNpromotions
                                                                                          where tp.IntTransferNpromotionId == IntTransferAndPromotionId && tp.IntAccountId == AccountId
                                                                                          join desig1 in _context.MasterDesignations on tp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                                                          from desig in desig2.DefaultIfEmpty()
                                                                                          join OldDesig1 in _context.MasterDesignations on tp.IntDesignationIdFrom equals OldDesig1.IntDesignationId into OldDesig2
                                                                                          from OldDesig in OldDesig2.DefaultIfEmpty()

                                                                                          join OldDep1 in _context.MasterDepartments on tp.IntDepartmentIdFrom equals OldDep1.IntDepartmentId into OldDep2
                                                                                          from OldDep in OldDep2.DefaultIfEmpty()

                                                                                          join dep1 in _context.MasterDepartments on tp.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                                                                          from dep in dep2.DefaultIfEmpty()

                                                                                          join busi1 in _context.MasterBusinessUnits on tp.IntBusinessUnitId equals busi1.IntBusinessUnitId into busi2
                                                                                          from busi in busi2.DefaultIfEmpty()

                                                                                          join OldBusi1 in _context.MasterBusinessUnits on tp.IntBusinessUnitId equals OldBusi1.IntBusinessUnitId into OldBusi2
                                                                                          from OldBusi in OldBusi2.DefaultIfEmpty()

                                                                                          join approver in _context.EmpEmployeeBasicInfos on tp.IntUpdatedBy equals approver.IntEmployeeBasicInfoId into appv
                                                                                          from approverEmp in appv.DefaultIfEmpty()
                                                                                          join appvBusi1 in _context.MasterBusinessUnits on approverEmp.IntBusinessUnitId equals appvBusi1.IntBusinessUnitId into appvBu
                                                                                          from appvBus in appvBu.DefaultIfEmpty()
                                                                                          join appvDep1 in _context.MasterDepartments on approverEmp.IntDepartmentId equals appvDep1.IntDepartmentId into appvDp
                                                                                          from appvDep in appvDp.DefaultIfEmpty()
                                                                                          join appvDesg1 in _context.MasterDesignations on approverEmp.IntDesignationId equals appvDesg1.IntDesignationId into appvDsg
                                                                                          from appvDesg in appvDsg.DefaultIfEmpty()
                                                                                          where tp.IsActive == true
                                                                                          select new TransferAndPromotionReportPDFViewModel()
                                                                                          {
                                                                                              TransferAndPromotionId = tp.IntTransferNpromotionId,
                                                                                              EmployeeName = tp.StrEmployeeName,
                                                                                              Designation = OldDesig != null ? OldDesig.StrDesignation : "",
                                                                                              Department = OldDep != null ? OldDep.StrDepartment : "",
                                                                                              BusinessUnit = OldBusi != null ? OldBusi.StrBusinessUnit : "",
                                                                                              PromotedDate = tp.DteEffectiveDate,
                                                                                              PreviousDesignation = OldDesig != null ? OldDesig.StrDesignation : "",
                                                                                              PromotedBusinessUnit = busi != null ? busi.StrBusinessUnit : "",
                                                                                              PromotedDesignation = desig != null ? desig.StrDesignation : "",
                                                                                              ApprovarName = approverEmp != null ? approverEmp.StrEmployeeName : "",
                                                                                              ApprovarBusinessUnit = appvBus != null ? appvBus.StrBusinessUnit : "",
                                                                                              ApprovarDepartment = appvDep != null ? appvDep.StrDepartment : "",
                                                                                              ApprovarDesignation = appvDesg != null ? appvDesg.StrDesignation : "",
                                                                                          }).FirstOrDefaultAsync();
            return transferAndPromotionReportPDF;
        }

        public async Task<TransferReportViewModel> TransferReport(long IntTransferId, long AccountId)
        {
            TransferReportViewModel transferReportViewModel = await (from trans in _context.EmpTransferNpromotions
                                                                     where trans.IntTransferNpromotionId == IntTransferId && trans.IntAccountId == AccountId
                                                                     join desig1 in _context.MasterDesignations on trans.IntDesignationId equals desig1.IntDesignationId into desig2
                                                                     from desig in desig2.DefaultIfEmpty()
                                                                     join OldDesig1 in _context.MasterDesignations on trans.IntDesignationIdFrom equals OldDesig1.IntDesignationId into OldDesig2
                                                                     from FromDesig in OldDesig2.DefaultIfEmpty()

                                                                     join dept1 in _context.MasterDepartments on trans.IntDepartmentIdFrom equals dept1.IntDepartmentId into dept2
                                                                     from fromDept in dept2.DefaultIfEmpty()
                                                                     join dep1 in _context.MasterDepartments on trans.IntDepartmentId equals dep1.IntDepartmentId into dep2
                                                                     from toDep in dep2.DefaultIfEmpty()
                                                                     join emp1 in _context.EmpEmployeeBasicInfos on trans.IntEmployeeId equals emp1.IntEmployeeBasicInfoId into emp2
                                                                     from emp in emp2.DefaultIfEmpty()

                                                                     join busi1 in _context.MasterBusinessUnits on trans.IntBusinessUnitId equals busi1.IntBusinessUnitId into busi2
                                                                     from toBusi in busi2.DefaultIfEmpty()
                                                                     join frombusi1 in _context.MasterBusinessUnits on trans.IntBusinessUnitIdFrom equals frombusi1.IntBusinessUnitId into frombusi2
                                                                     from fromBusi in frombusi2.DefaultIfEmpty()
                                                                     join approver in _context.EmpEmployeeBasicInfos on trans.IntUpdatedBy equals approver.IntEmployeeBasicInfoId into appv
                                                                     from approverEmp in appv.DefaultIfEmpty()
                                                                     join appvBusi1 in _context.MasterBusinessUnits on approverEmp.IntBusinessUnitId equals appvBusi1.IntBusinessUnitId into appvBu
                                                                     from appvBus in appvBu.DefaultIfEmpty()
                                                                     join appvDep1 in _context.MasterDepartments on approverEmp.IntDepartmentId equals appvDep1.IntDepartmentId into appvDp
                                                                     from appvDep in appvDp.DefaultIfEmpty()
                                                                     join appvDesg1 in _context.MasterDesignations on approverEmp.IntDesignationId equals appvDesg1.IntDesignationId into appvDsg
                                                                     from appvDesg in appvDsg.DefaultIfEmpty()
                                                                     where trans.IsActive == true

                                                                     select new TransferReportViewModel()
                                                                     {
                                                                         EmployeeName = trans.StrEmployeeName,
                                                                         FromDesignation = FromDesig.StrDesignation,
                                                                         ToDesignation = desig.StrDesignation,
                                                                         FromDepartment = fromDept.StrDepartment,
                                                                         ToDepartment = toDep.StrDepartment,
                                                                         JoiningDate = emp.DteJoiningDate,
                                                                         FromBusinessUnit = fromBusi.StrBusinessUnit,
                                                                         ToBusinessUnit = toBusi.StrBusinessUnit,
                                                                         EffectiveDate = trans.DteReleaseDate,
                                                                         PresenetSalary = _context.PyrEmployeeSalaryElementAssignHeaders.Where(x => x.IntEmployeeId == trans.IntEmployeeId && x.IsActive == true).Select(s => s.NumGrossSalary).FirstOrDefault(),
                                                                         Approvarname = approverEmp.StrEmployeeName,
                                                                         ApprovarDesignation = appvDesg.StrDesignation,
                                                                         ApprovarBusinessUnit = appvBus.StrBusinessUnit,
                                                                     }).FirstOrDefaultAsync();
            return transferReportViewModel;
        }

        public async Task<BankAdviceReportForIBBLViewModel> BankAdviceReportForIBBL(long MonthId, long SalaryGenerateRequestId, long YearId, long AccountId, long intBusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                IQueryable<BankAdvice> query = (from sr in _context.PyrPayrollSalaryGenerateRequests
                                                join sh in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals sh.IntSalaryGenerateRequestId into shJoin
                                                from sh in shJoin.DefaultIfEmpty()

                                                join des in _context.MasterDesignations on sh.IntDesignationId equals des.IntDesignationId into RankJoin
                                                from des in RankJoin.DefaultIfEmpty()

                                                join empD in _context.EmpEmployeeBasicInfoDetails on sh.IntEmployeeId equals empD.IntEmployeeId into empJoin
                                                from empD in empJoin.DefaultIfEmpty()
                                                join wi in _context.TerritorySetups on empD.IntWingId equals wi.IntTerritoryId into wi1
                                                from wing in wi1.DefaultIfEmpty()
                                                join soleD in _context.TerritorySetups on empD.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                from soleDp in soleD1.DefaultIfEmpty()
                                                join regn in _context.TerritorySetups on empD.IntRegionId equals regn.IntTerritoryId into regn1
                                                from region in regn1.DefaultIfEmpty()
                                                join area1 in _context.TerritorySetups on empD.IntAreaId equals area1.IntTerritoryId into area2
                                                from area in area2.DefaultIfEmpty()
                                                join terrty in _context.TerritorySetups on empD.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                from Territory in terrty1.DefaultIfEmpty()
                                                where sr.IsActive == true && sh.IsActive == true && sr.IntSalaryGenerateRequestId == SalaryGenerateRequestId
                                                    && sr.IntAccountId == AccountId && sr.IntYear == YearId && sr.IntMonth == MonthId

                                                orderby sh.StrEmployeeName ascending
                                                select new BankAdvice
                                                {
                                                    EmployeeCode = sh.StrEmployeeCode,
                                                    EmployeeName = sh.StrEmployeeName,                                                   
                                                    Designation = sh.StrDesignation == null ? "" : sh.StrDesignation,
                                                    DesignationRank=des.IntRankingId,
                                                    Territory = Territory.StrTerritoryName == null ? "" : Territory.StrTerritoryName,
                                                    Area = area.StrTerritoryName == null ? "" : area.StrTerritoryName,
                                                    Region = region.StrTerritoryName == null ? "" : region.StrTerritoryName,
                                                    MobileNumber = empD.StrPersonalMobile == null ? "" : empD.StrPersonalMobile,
                                                    AccountNo = sh.StrAccountNo == null ? "" : sh.StrAccountNo,
                                                    NetSalary = sh.NumNetPayableSalary,
                                                    Remarks = "",
                                                    Wing = wing.StrTerritoryName == null ? "" : wing.StrTerritoryName,
                                                    SoleDepo = soleDp.StrTerritoryName == null ? "" : soleDp.StrTerritoryName
                                                }).OrderBy(x => x.DesignationRank).AsNoTracking().AsQueryable();

                var gList =await query.GroupBy(x => new { x.SoleDepo, x.Wing }).Select(x => new BankAdviceHeaderVM { SoleDepoName = x.Key.SoleDepo, WingName = x.Key.Wing }).ToListAsync();
                var data =await query.ToListAsync();

                BankAdviceReportForIBBLViewModel forIBBLViewModel = await (from bank in _context.AccountBankDetails
                                                                           where bank.IntAccountId == AccountId && bank.IntBusinessUnitId == intBusinessUnitId
                                                                           && bank.IsActive == true
                                                                           join busi1 in _context.MasterBusinessUnits on bank.IntBusinessUnitId equals busi1.IntBusinessUnitId into busi2
                                                                           from busi in busi2.DefaultIfEmpty()
                                                                           where busi.IsActive == true
                                                                           select new BankAdviceReportForIBBLViewModel()
                                                                           {
                                                                               BusinessUnit = busi.StrBusinessUnit,
                                                                               CompanyAddress = busi.StrAddress,
                                                                               ReportGenerateDate = new DateTime((int)YearId, (int)MonthId, 1),
                                                                               CompanyBankName = bank.StrBankWalletName,
                                                                               CompanyBranchName = bank.StrBranchName,
                                                                               CompanyAccountNo = bank.StrAccountNo,
                                                                               intLogoUrlId = busi.StrLogoUrlId,
                                                                               BankAdviceVM = gList ,
                                                                               Data = data

                                                                               //employeePaymentInfoViewModels = (from salaryHeader in _context.PyrSalaryGenerateHeaders
                                                                               //                                 where salaryHeader.IntAccountId == AccountId
                                                                               //                                 && salaryHeader.IntBusinessUnitId == intBusinessUnitId
                                                                               //                                 && salaryHeader.IntSalaryGenerateRequestId == SalaryGenerateRequestId
                                                                               //                                 && salaryHeader.IsActive == true
                                                                               //                                 && salaryHeader.IntWorkplaceGroupId == WorkplaceGroupId
                                                                               //                                 join emp in _context.EmpEmployeeBasicInfos on salaryHeader.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                               //                                 join empdtlbank1 in _context.EmpEmployeeBankDetails on emp.IntEmployeeBasicInfoId equals empdtlbank1.IntEmployeeBasicInfoId into empdtl2
                                                                               //                                 from empdtlbank in empdtl2.DefaultIfEmpty()
                                                                               //                                 where salaryHeader.IntMonthId == MonthId && salaryHeader.IntYearId == YearId
                                                                               //                                 select new EmployeePaymentInfoViewModel
                                                                               //                                 {
                                                                               //                                     BankAccountNo = salaryHeader != null ? salaryHeader.StrAccountNo : "N/A",
                                                                               //                                     AccountName = empdtlbank != null ? empdtlbank.StrAccountName : "N/A",
                                                                               //                                     NetAmount = salaryHeader != null ? salaryHeader.NumNetPayableSalary : 0,
                                                                               //                                     EmployeCode = emp != null ? emp.StrEmployeeCode : "N/A",
                                                                               //                                     BranchName = salaryHeader != null ? salaryHeader.StrBankBranchName : "N/A"
                                                                               //                                 }).ToList()
                                                                           }).FirstOrDefaultAsync();

                return forIBBLViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BankAdviceReportForBEFTNViewModel> BankAdviceReportForBEFTN(long MonthId, long SalaryGenerateRequestId, long YearId, long AccountId, long? intBusinessUnitId, long WorkplaceGroupId)
        {
            try
            {
                IQueryable<BankAdvice> query = (from sr in _context.PyrPayrollSalaryGenerateRequests
                                                join sh in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals sh.IntSalaryGenerateRequestId into shJoin
                                                from sh in shJoin.DefaultIfEmpty()
                                                join des in _context.MasterDesignations on sh.IntDesignationId equals des.IntDesignationId into RankJoin
                                                from des in RankJoin.DefaultIfEmpty()
                                                join empD in _context.EmpEmployeeBasicInfoDetails on sh.IntEmployeeId equals empD.IntEmployeeId into empJoin
                                                from empD in empJoin.DefaultIfEmpty()
                                                join wi in _context.TerritorySetups on empD.IntWingId equals wi.IntTerritoryId into wi1
                                                from wing in wi1.DefaultIfEmpty()
                                                join soleD in _context.TerritorySetups on empD.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                                from soleDp in soleD1.DefaultIfEmpty()
                                                join regn in _context.TerritorySetups on empD.IntRegionId equals regn.IntTerritoryId into regn1
                                                from region in regn1.DefaultIfEmpty()
                                                join area1 in _context.TerritorySetups on empD.IntAreaId equals area1.IntTerritoryId into area2
                                                from area in area2.DefaultIfEmpty()
                                                join terrty in _context.TerritorySetups on empD.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                                from Territory in terrty1.DefaultIfEmpty()
                                                where sr.IsActive == true && sh.IsActive == true && sr.IntSalaryGenerateRequestId == SalaryGenerateRequestId
                                                    && sr.IntAccountId == AccountId && sr.IntYear == YearId && sr.IntMonth == MonthId

                                                orderby sh.StrEmployeeName ascending
                                                select new BankAdvice
                                                {
                                                    EmployeeCode = sh.StrEmployeeCode,
                                                    EmployeeName = sh.StrEmployeeName,
                                                    Designation = sh.StrDesignation == null ? "" : sh.StrDesignation,
                                                    DesignationRank=des.IntRankingId,
                                                    Territory = Territory.StrTerritoryName == null ? "" : Territory.StrTerritoryName,
                                                    Area = area.StrTerritoryName == null ? "" : area.StrTerritoryName,
                                                    Region = region.StrTerritoryName == null ? "" : region.StrTerritoryName,
                                                    MobileNumber = empD.StrPersonalMobile == null ? "" : empD.StrPersonalMobile,
                                                    AccountNo = sh.StrAccountNo == null ? "" : sh.StrAccountNo,
                                                    NetSalary = sh.NumNetPayableSalary,
                                                    Remarks = "",                                                  
                                                    Wing = wing.StrTerritoryName == null ? "" : wing.StrTerritoryName,
                                                    SoleDepo = soleDp.StrTerritoryName == null ? "" : soleDp.StrTerritoryName
                                                }).OrderBy(x => x.DesignationRank).AsNoTracking().AsQueryable();

                var gList = await query.GroupBy(x => new { x.SoleDepo, x.Wing }).Select(x => new BankAdviceHeaderVM { SoleDepoName = x.Key.SoleDepo, WingName = x.Key.Wing }).ToListAsync();
                var data = await query.ToListAsync();

                DateTime dte = new DateTime(2022, 8, 1);
                string paymentInfo = "Salary For " + dte.ToString("MMMM-yyyy");

                BankAdviceReportForBEFTNViewModel forBEFTNViewModel = await (from bank in _context.AccountBankDetails
                                                                             where bank.IntAccountId == AccountId && bank.IntBusinessUnitId == intBusinessUnitId
                                                                             && bank.IsActive == true
                                                                             join busi1 in _context.MasterBusinessUnits on bank.IntBusinessUnitId equals busi1.IntBusinessUnitId into busi2
                                                                             from busi in busi2.DefaultIfEmpty()
                                                                             where busi.IsActive == true
                                                                             select new BankAdviceReportForBEFTNViewModel()
                                                                             {
                                                                                 BusinessUnit = busi.StrBusinessUnit,
                                                                                 CompanyAddress = busi.StrAddress,
                                                                                 CompanyBankName = bank.StrBankWalletName,
                                                                                 CompanyBranchName = bank.StrBranchName,
                                                                                 ReportGenerateDate = new DateTime((int)YearId, (int)MonthId, 1),
                                                                                 CompanyAccountNo = bank.StrAccountNo,
                                                                                 intLogoUrlId = busi.StrLogoUrlId,
                                                                                 BankAdviceVM = gList,
                                                                                 Data = data
                                                                                 //employeePaymentInfoBEFTNViewModels = (from salaryHeader in _context.PyrSalaryGenerateHeaders
                                                                                 //                                      where salaryHeader.IntAccountId == AccountId
                                                                                 //                                      && salaryHeader.IntBusinessUnitId == intBusinessUnitId
                                                                                 //                                      && salaryHeader.IntSalaryGenerateRequestId == SalaryGenerateRequestId
                                                                                 //                                      && salaryHeader.IsActive == true
                                                                                 //                                       && salaryHeader.IntWorkplaceGroupId == WorkplaceGroupId
                                                                                 //                                      join emp in _context.EmpEmployeeBasicInfos on salaryHeader.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                                 //                                      join empdtlbank1 in _context.EmpEmployeeBankDetails on emp.IntEmployeeBasicInfoId equals empdtlbank1.IntEmployeeBasicInfoId into empdtl2
                                                                                 //                                      from empdtlbank in empdtl2.DefaultIfEmpty()
                                                                                 //                                      where salaryHeader.IntMonthId == MonthId && salaryHeader.IntYearId == YearId

                                                                                 //                                      select new EmployeePaymentInfoBEFTNViewModel
                                                                                 //                                      {
                                                                                 //                                          AccountName = salaryHeader.StrAccountName,
                                                                                 //                                          EmployeeCode = emp != null ? emp.StrEmployeeCode : "N/A",
                                                                                 //                                          BankName = salaryHeader.StrFinancialInstitution,
                                                                                 //                                          Branch = salaryHeader.StrBankBranchName,
                                                                                 //                                          AccountType = "Savings",
                                                                                 //                                          AccountNo = empdtlbank != null ? empdtlbank.StrAccountNo : "N/A",
                                                                                 //                                          Amount = salaryHeader != null ? salaryHeader.NumNetPayableSalary : 0,
                                                                                 //                                          RoutingNo = empdtlbank.StrRoutingNo
                                                                                 //                                      }).ToList()
                                                                             }).FirstOrDefaultAsync();

                return forBEFTNViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region===== master location register============

        public async Task<MessageHelper> MasterLocationRegistrationAsync(MasterLocationRegisterDTO model)
        {
            MessageHelper msg = new();
            try
            {
                MasterLocationRegister fromDb = new();

                if (model.IntMasterLocationId > 0)
                {
                    fromDb = await _context.MasterLocationRegisters.Where(x => x.IntAccountId == model.IntAccountId && x.IntMasterLocationId == model.IntMasterLocationId).AsNoTracking().FirstOrDefaultAsync();
                    if (fromDb == null)
                    {
                        msg.AutoId = 0;
                        msg.Message = "No data found from data";
                        msg.StatusCode = 500;
                        return msg;
                    }
                }

                PipelineStageInfoVM pipelineInfo = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(model.IntAccountId, "masterLocationRegistration");
                if (pipelineInfo.HeaderId == 0)
                {
                    msg.AutoId = 0;
                    msg.Message = "pipeline was not set";
                    msg.StatusCode = 500;
                    return msg;
                }

                MasterLocationRegister newEntity = new()
                {
                    IntMasterLocationId = model.IntMasterLocationId,
                    IntAccountId = model.IntAccountId,
                    IntBusinessId = model.IntBusinessId,
                    StrLocationCode = model.StrLocationCode,
                    StrLongitude = model.StrLongitude,
                    StrLatitude = model.StrLatitude,
                    StrPlaceName = model.StrPlaceName,
                    StrAddress = model.StrAddress,
                    IsActive = model.IsActive,
                    IntPipelineHeaderId = pipelineInfo.HeaderId,
                    IntCurrentStage = pipelineInfo.CurrentStageId,
                    IntNextStage = pipelineInfo.NextStageId,
                    StrStatus = "pending",
                    IsPipelineClosed = false,
                    IsReject = false,
                    DteCreatedAt = DateTime.Now,
                    IntCreatedBy = model.ActionBy
                };
                if (fromDb != null && model.IntMasterLocationId > 0)
                {
                    newEntity.IntUpdatedBy = model.ActionBy;
                    newEntity.DteUpdatedAt = DateTime.Now;
                    _context.Entry(newEntity).State = EntityState.Modified;
                    _context.Entry(newEntity).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(newEntity).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.MasterLocationRegisters.Update(newEntity);

                    msg.Message = "update successful";
                }
                else
                {
                    await _context.MasterLocationRegisters.AddAsync(newEntity);
                    msg.Message = "Create successful";
                }

                int res = await _context.SaveChangesAsync();

                if (res > 0)
                {
                    msg.AutoId = newEntity.IntMasterLocationId;
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    msg.AutoId = 0;
                    msg.StatusCode = 500;
                    return msg;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GetMasterLocationRegisterDTO>> GetMasterLocationByAccountIdAsync(long AcccountId, long BusinessUnitId)
        {
            try
            {
                List<GetMasterLocationRegisterDTO> listLocation = new();

                listLocation = await (from ml in _context.MasterLocationRegisters
                                      where ml.IsActive == true && ml.IntAccountId == AcccountId && (BusinessUnitId > 0 ? ml.IntBusinessId == BusinessUnitId : true)
                                      select new GetMasterLocationRegisterDTO
                                      {
                                          IntMasterLocationId = ml.IntMasterLocationId,
                                          IntAccountId = ml.IntAccountId,
                                          IntBusinessId = ml.IntBusinessId,
                                          StrLongitude = ml.StrLongitude,
                                          StrLatitude = ml.StrLatitude,
                                          StrLocationCode = ml.StrLocationCode,
                                          StrPlaceName = ml.StrPlaceName,
                                          StrAddress = ml.StrAddress,
                                          IsActive = ml.IsActive,
                                          IntPipelineHeaderId = ml.IntPipelineHeaderId,
                                          IntCurrentStage = ml.IntCurrentStage,
                                          IntNextStage = ml.IntNextStage,
                                          StrStatus = ml.StrStatus == "pending" ? "pending" : (ml.IsReject == true ? "rejected" : ((ml.IntCurrentStage != ml.IntNextStage && ml.IsPipelineClosed == false) ? "process" : "approved")),
                                          IsPipelineClosed = ml.IsPipelineClosed,
                                          IsReject = ml.IsReject,
                                          DteRejectDateTime = ml.DteRejectDateTime,
                                          IntRejectedBy = ml.IntRejectedBy,
                                          DteCreatedAt = ml.DteCreatedAt,
                                          IntCreatedBy = ml.IntCreatedBy,
                                          DteUpdatedAt = ml.DteUpdatedAt,
                                          IntUpdatedBy = ml.IntUpdatedBy
                                      }).AsQueryable().AsQueryable().ToListAsync();

                return listLocation;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Location assaign

        public async Task<List<MasterLoctionRegistrationDdl>> MasterLocationRegistrationDDL(long AccountId, long BussinessUnit)
        {
            try
            {
                List<MasterLoctionRegistrationDdl> masterddl = new();

                masterddl = await (from ml in _context.MasterLocationRegisters
                                   where ml.IsActive == true && ml.IntAccountId == AccountId && (ml.IntBusinessId > 0 ? ml.IntBusinessId == BussinessUnit : true)
                                   && ml.IsPipelineClosed == true && ml.IsReject == false
                                   select new MasterLoctionRegistrationDdl
                                   {
                                       Value = ml.IntMasterLocationId,
                                       Label = ml.StrPlaceName,
                                       IntMasterLocationId = ml.IntMasterLocationId,
                                       IntAccountId = ml.IntAccountId,
                                       IntBusinessId = ml.IntBusinessId,
                                       StrLongitude = ml.StrLongitude,
                                       StrLatitude = ml.StrLatitude,
                                       StrPlaceName = ml.StrPlaceName,
                                       StrAddress = ml.StrAddress,
                                       StrLocationCOde = ml.StrLocationCode,
                                       LocationLog = ml.StrAddress + " (" + ml.StrLatitude + " | " + ml.StrLongitude + ")",
                                       Count = _context.TimeRemoteAttendanceRegistrations.Where(x => x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true && x.IntMasterLocationId == ml.IntMasterLocationId).Count(),
                                       StrStatus = "Approved"
                                   }).AsQueryable().AsNoTracking().ToListAsync();

                return masterddl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> EmployeeWiseLocationAsync(long AccountId, long EmployeeId)
        {
            try
            {
                var data = await (from emp in _context.EmpEmployeeBasicInfos
                                  where emp.IntEmployeeBasicInfoId == EmployeeId && emp.IntAccountId == AccountId && emp.IsActive == true
                                  join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                  from desig in desig2.DefaultIfEmpty()
                                  join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                  from dept in dpt2.DefaultIfEmpty()
                                  join sup1 in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals sup1.IntEmployeeBasicInfoId into sup2
                                  from sup in sup2.DefaultIfEmpty()
                                  join dsup1 in _context.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dsup1.IntEmployeeBasicInfoId into dsup2
                                  from dsup in dsup2.DefaultIfEmpty()
                                  join man1 in _context.EmpEmployeeBasicInfos on emp.IntLineManagerId equals man1.IntEmployeeBasicInfoId into man2
                                  from man in man2.DefaultIfEmpty()
                                  join wrk1 in _context.MasterWorkplaces on emp.IntWorkplaceId equals wrk1.IntWorkplaceId into wrk2
                                  from wrk in wrk2.DefaultIfEmpty()
                                  join wg1 in _context.MasterWorkplaceGroups on wrk.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                  from wg in wg2.DefaultIfEmpty()
                                  join bus1 in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
                                  from bus in bus2.DefaultIfEmpty()
                                  join acc1 in _context.Accounts on emp.IntAccountId equals acc1.IntAccountId into acc2
                                  from acc in acc2.DefaultIfEmpty()
                                  join photo1 in _context.EmpEmployeePhotoIdentities on emp.IntEmployeeBasicInfoId equals photo1.IntEmployeeBasicInfoId into photo2
                                  from photo in photo2.DefaultIfEmpty()
                                  select new
                                  {
                                      IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                      StrEmployeeName = emp.StrEmployeeName,
                                      StrEmployeeCode = emp.StrEmployeeCode,
                                      StrReferenceId = emp.StrReferenceId,
                                      IntEmployeeImageUrlId = photo == null ? 0 : photo.IntProfilePicFileUrlId,
                                      IntDesignationId = emp.IntDesignationId,
                                      StrDesignation = desig == null ? "" : desig.StrDesignation,
                                      IntDepartmentId = emp.IntDepartmentId,
                                      StrDepartment = dept == null ? "" : dept.StrDepartment,
                                      IntSupervisorId = emp.IntSupervisorId,
                                      StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                      IntLineManagerId = emp.IntLineManagerId,
                                      StrLinemanager = man == null ? "" : man.StrEmployeeName,
                                      StrCardNumber = emp.StrCardNumber,
                                      IntDottedSupervisorId = emp.IntDottedSupervisorId,
                                      StrDottedSupervisorName = dsup == null ? "" : dsup.StrEmployeeName,
                                      IsActive = emp.IsActive,
                                      IsUserInactive = emp.IsUserInactive,
                                      IntWorkplaceId = emp.IntWorkplaceId,
                                      StrWorkplaceName = wrk == null ? "" : wrk.StrWorkplace,
                                      IntWorkplaceGroupId = wg != null ? wg.IntWorkplaceGroupId : 0,
                                      StrWorkplaceGroupName = wg == null ? "" : wg.StrWorkplaceGroup,
                                      IntBusinessUnitId = emp.IntBusinessUnitId,
                                      StrBusinessUnitName = bus == null ? "" : bus.StrBusinessUnit,
                                      IntAccountId = emp.IntAccountId,
                                      StrAccountName = acc == null ? "" : acc.StrAccountName,
                                      IntEmploymentTypeId = emp.IntEmploymentTypeId,
                                      StrEmploymentType = emp.StrEmploymentType,
                                  }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

                var role = await (from emb in _context.EmpEmployeeBasicInfos
                                  where emb.IntAccountId == AccountId && emb.IntEmployeeBasicInfoId == EmployeeId && emb.IsActive == true
                                  join rbw in _context.RoleBridgeWithDesignations on emb.IntEmployeeBasicInfoId equals rbw.IntDesignationOrEmployeeId
                                  join ur1 in _context.UserRoles on rbw.IntRoleId equals ur1.IntRoleId
                                  where rbw.IntAccountId == emb.IntAccountId && ((rbw.StrIsFor == "Employee" && rbw.IntDesignationOrEmployeeId == emb.IntEmployeeBasicInfoId)
                                  || (rbw.StrIsFor == "Designation" && rbw.IntDesignationOrEmployeeId == emb.IntDesignationId))
                                  select new
                                  {
                                      Role = ur1.StrRoleName
                                  }).AsNoTracking().AsQueryable().ToListAsync();

                string fullRole = "";

                int len = role.Count;
                if (len > 0)
                {
                    foreach (var r in role)
                    {
                        fullRole = fullRole + r.Role + (len > 1 ? "," : "");
                        len -= 1;
                    }
                }

                var employeeInfo = new
                {
                    IntEmployeeBasicInfoId = data.IntEmployeeBasicInfoId,
                    StrEmployeeName = data.StrEmployeeName,
                    StrEmployeeCode = data.StrEmployeeCode,
                    StrReferenceId = data.StrReferenceId,
                    IntEmployeeImageUrlId = data.IntEmployeeImageUrlId,
                    IntDesignationId = data.IntDesignationId,
                    StrDesignation = data.StrDesignation,
                    IntDepartmentId = data.IntDepartmentId,
                    StrDepartment = data.StrDepartment,
                    IntSupervisorId = data.IntSupervisorId,
                    StrSupervisorName = data.StrSupervisorName,
                    IntLineManagerId = data.IntLineManagerId,
                    StrLinemanager = data.StrLinemanager,
                    StrCardNumber = data.StrCardNumber,
                    IntDottedSupervisorId = data.IntDottedSupervisorId,
                    StrDottedSupervisorName = data.StrDottedSupervisorName,
                    IsActive = data.IsActive,
                    IsUserInactive = data.IsUserInactive,
                    IntWorkplaceId = data.IntWorkplaceId,
                    StrWorkplaceName = data.StrWorkplaceName,
                    IntWorkplaceGroupId = data.IntWorkplaceGroupId,
                    StrWorkplaceGroupName = data.StrWorkplaceGroupName,
                    IntBusinessUnitId = data.IntBusinessUnitId,
                    StrBusinessUnitName = data.StrBusinessUnitName,
                    IntAccountId = data.IntAccountId,
                    StrAccountName = data.StrAccountName,
                    IntEmploymentTypeId = data.IntEmploymentTypeId,
                    StrEmploymentType = data.StrEmploymentType,
                    Role = fullRole
                };

                List<MasterLocationRegister> locationList = await _context.MasterLocationRegisters
                    .Where(x => x.IntAccountId == AccountId && x.IsActive == true
                    && (x.IsPipelineClosed == true && x.IsReject == false)).AsNoTracking().ToListAsync();

                List<TimeRemoteAttendanceRegistration> registerList = await _context.TimeRemoteAttendanceRegistrations
                    .Where(x => x.IntAccountId == AccountId && x.IsActive == true
                    && !(x.IsPipelineClosed == true && x.IsReject == true) && x.IntEmployeeId == EmployeeId).AsNoTracking().ToListAsync();

                var resultList = (from loc in locationList
                                  join reg1 in registerList on loc.IntMasterLocationId equals reg1.IntMasterLocationId into reg2
                                  from reg in reg2.DefaultIfEmpty()
                                  select new
                                  {
                                      IntMasterLocationId = loc.IntMasterLocationId,
                                      StrLocationCode = loc.StrLocationCode,
                                      IsChecked = reg != null ? ((reg.IsReject == false && reg.IsPipelineClosed == true) ? true : false) : false,
                                      StrAddress = loc.StrAddress,
                                      LocationName = loc.StrPlaceName,
                                      StrLatitude = loc.StrLatitude,
                                      StrLongitude = loc.StrLongitude,
                                      LocationLog = loc.StrAddress + " (" + loc.StrLatitude + " | " + loc.StrLongitude + ")",
                                      StrStatus = reg != null ? (reg.StrStatus == "pending" ? "pending" : (reg.IsReject == true ? "rejected" : ((reg.IsPipelineClosed == true) ? "approved" : "process"))) : ""
                                  }).ToList();

                var finalList = new { employeeInfo, resultList };

                return finalList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MessageHelper> CreateNUpdateEmployeeWiseLocationAssaignAsync(CreateNUpdateMasterLocationEployeeWise model)
        {
            MessageHelper msg = new();
            try
            {
                List<TimeRemoteAttendanceRegistration> newList = new();
                if (model.ListLocations.Count > 0)
                {
                    PipelineStageInfoVM pipelineInfo = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(model.IntAccountId, "locationNDevice");

                    foreach (var item in model.ListLocations)
                    {
                        if (item.IsCreate && ((await _context.TimeRemoteAttendanceRegistrations.Where(x => x.IntEmployeeId == model.IntEmployeeId && x.IntAccountId == model.IntAccountId && x.IsActive == true && !(x.IsPipelineClosed == true && x.IsReject == true) && x.IntMasterLocationId == item.MasterLocationId).FirstOrDefaultAsync()) == null))
                        {
                            MasterLocationRegister locationRegister = _context.MasterLocationRegisters.Where(x => x.IntMasterLocationId == item.MasterLocationId).FirstOrDefault();

                            TimeRemoteAttendanceRegistration timeRemote = new()
                            {
                                IntAttendanceRegId = 0,
                                IntAccountId = model.IntAccountId,
                                IntEmployeeId = model.IntEmployeeId,
                                IntMasterLocationId = item.MasterLocationId,
                                StrLatitude = locationRegister.StrLatitude,
                                StrLongitude = locationRegister.StrLongitude,
                                StrPlaceName = locationRegister.StrPlaceName,
                                StrAddress = locationRegister.StrAddress,
                                IsLocationRegister = true,
                                StrEmployeeName = model.strEmployeeName,
                                DteInsertDate = DateTime.Now,
                                IntInsertBy = model.IntActionBy,
                                IntPipelineHeaderId = pipelineInfo.HeaderId,
                                IntCurrentStage = pipelineInfo.CurrentStageId,
                                IntNextStage = pipelineInfo.NextStageId,
                                StrStatus = "pending",
                                IsActive = true,
                                IsPipelineClosed = false,
                                IsReject = false
                            };
                            newList.Add(timeRemote);
                        }
                        else if (item.IsCreate == false)
                        {
                            await _context.Database.ExecuteSqlRawAsync($"UPDATE saas.timeRemoteAttendanceRegistration SET isActive=0 WHERE intMasterLocationId={item.MasterLocationId} AND intEmployeeId={model.IntEmployeeId} AND intAccountId={model.IntAccountId}");
                        }
                    }
                }
                if (newList.Count() > 0)
                {
                    await _context.TimeRemoteAttendanceRegistrations.AddRangeAsync(newList);
                    await _context.SaveChangesAsync();
                }
                msg.StatusCode = 200;
                msg.Message = "Success";

                return msg;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> GetLocationWiseEmployeeAsync(long LocationMasterId, long AccountId, long BusineesUint)
        {
            try
            {
                List<LocationWiseEmployeeList> employeeBasic = await (from emp in _context.EmpEmployeeBasicInfos
                                                                      where emp.IntAccountId == AccountId && emp.IsActive == true
                                                                      join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                                                      from desig in desig2.DefaultIfEmpty()
                                                                      join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                                                      from dept in dpt2.DefaultIfEmpty()
                                                                      select new LocationWiseEmployeeList()
                                                                      {
                                                                          IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                                          StrEmployeeName = emp.StrEmployeeName,
                                                                          StrEmployeeCode = emp.StrEmployeeCode,
                                                                          IntDesignationId = emp.IntDesignationId,
                                                                          StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                                          IntDepartmentId = emp.IntDepartmentId,
                                                                          StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                                          IsChecked = false,
                                                                          StrStatus = ""
                                                                      }).AsNoTracking().AsQueryable().ToListAsync();

                List<LocationWiseEmployeeList> checkedUnchekedEmployee = (from emp in employeeBasic
                                                                          join rar in _context.TimeRemoteAttendanceRegistrations on emp.IntEmployeeBasicInfoId equals rar.IntEmployeeId //into rar2
                                                                                                                                                                                        //from rar in rar2.DefaultIfEmpty()
                                                                          where rar.IsActive == true && rar.IsReject == false
                                                                          && rar.IntMasterLocationId == LocationMasterId
                                                                          select new LocationWiseEmployeeList()
                                                                          {
                                                                              IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                                              StrEmployeeName = emp.StrEmployeeName,
                                                                              StrEmployeeCode = emp.StrEmployeeCode,
                                                                              IntDesignationId = emp.IntDesignationId,
                                                                              StrDesignation = emp.StrDesignation,
                                                                              IntDepartmentId = emp.IntDepartmentId,
                                                                              StrDepartment = emp.StrDepartment,
                                                                              IsChecked = rar != null ? ((rar.IsReject == false && rar.IsPipelineClosed == true) ? true : false) : false,
                                                                              StrStatus = rar != null ? (rar.StrStatus == "pending" ? "pending" : (rar.IsReject == true ? "rejected" : ((rar.IsPipelineClosed == true) ? "approved" : "process"))) : ""
                                                                          }).ToList();

                employeeBasic.ForEach(x =>
                {
                    checkedUnchekedEmployee.ForEach(i =>
                    {
                        if (x.IntEmployeeBasicInfoId == i.IntEmployeeBasicInfoId)
                        {
                            x.IsChecked = i.IsChecked;
                            x.StrStatus = i.StrStatus;
                        }
                    });
                });

                return employeeBasic;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> CreateNUpdateLocationWiseEmployeeAsync(CreateNUpdateMasterLocationWise model)
        {
            try
            {
                MessageHelper msg = new();
                List<TimeRemoteAttendanceRegistration> newList = new();
                if (model.ListEmployee.Count > 0)
                {
                    PipelineStageInfoVM pipelineInfo = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(model.IntAccountId, "locationNDevice");

                    foreach (var item in model.ListEmployee)
                    {
                        if (item.IsCreate && ((await _context.TimeRemoteAttendanceRegistrations.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntAccountId == model.IntAccountId && x.IsActive == true && !(x.IsPipelineClosed == true && x.IsReject == true) && x.IntMasterLocationId == model.MasterLocationId).FirstOrDefaultAsync()) == null))
                        {
                            TimeRemoteAttendanceRegistration timeRemote = new()
                            {
                                IntAttendanceRegId = 0,
                                IntAccountId = model.IntAccountId,
                                IntEmployeeId = item.IntEmployeeId,
                                IntMasterLocationId = model.MasterLocationId,
                                IsLocationRegister = true,
                                StrEmployeeName = item.strEmployeeName,
                                DteInsertDate = DateTime.Now,
                                IntInsertBy = model.IntActionBy,
                                IntPipelineHeaderId = pipelineInfo.HeaderId,
                                IntCurrentStage = pipelineInfo.CurrentStageId,
                                IntNextStage = pipelineInfo.NextStageId,
                                StrStatus = "pending",
                                IsActive = true,
                                IsPipelineClosed = false,
                                IsReject = false
                            };
                            newList.Add(timeRemote);
                        }
                        else if (item.IsCreate == false)
                        {
                            await _context.Database.ExecuteSqlRawAsync($"UPDATE saas.timeRemoteAttendanceRegistration SET isActive=0 WHERE intMasterLocationId={model.MasterLocationId} AND intEmployeeId={item.IntEmployeeId} AND intAccountId={model.IntAccountId}");
                        }
                    }
                }
                if (newList.Count() > 0)
                {
                    await _context.TimeRemoteAttendanceRegistrations.AddRangeAsync(newList);
                    await _context.SaveChangesAsync();
                }
                msg.StatusCode = 200;
                msg.Message = "Success";

                return msg;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> GetLocationDashBoardByAccountId(long IntAccountId, long IntBusinessUnitId)
        {
            try
            {
                var locationDashBoard = await (from ml in _context.MasterLocationRegisters
                                               where ml.IsActive == true && ml.IntAccountId == IntAccountId && (ml.IntBusinessId > 0 ? ml.IntBusinessId == IntBusinessUnitId : true)

                                               select new
                                               {
                                                   IntMasterLocationId = ml.IntMasterLocationId,
                                                   IntAccountId = ml.IntAccountId,
                                                   IntBusinessId = ml.IntBusinessId,
                                                   StrLongitude = ml.StrLongitude,
                                                   StrLatitude = ml.StrLatitude,
                                                   StrPlaceName = ml.StrPlaceName,
                                                   StrAddress = ml.StrAddress,
                                                   StrLocationCOde = ml.StrLocationCode,
                                                   LocationLog = ml.StrAddress + " (" + ml.StrLatitude + " | " + ml.StrLongitude + ")",
                                                   Count = _context.TimeRemoteAttendanceRegistrations.Where(x => x.IsActive == true && x.IsReject == false && x.IsPipelineClosed == true && x.IntMasterLocationId == ml.IntMasterLocationId).Count(),
                                                   StrStatus = ml.StrStatus == "pending" ? "pending" : (ml.IsReject == true ? "rejected" : ((ml.IsPipelineClosed == true) ? "approved" : "process"))
                                               }).AsQueryable().AsNoTracking().ToListAsync();

                return locationDashBoard;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> GetEmployeeListLocationBased(long IntAccountId, long IntBusinessUnitId)
        {
            try
            {
                var locationDashBoard = await (from emp in _context.EmpEmployeeBasicInfos
                                               where emp.IsActive == true && emp.IntAccountId == IntAccountId && (IntBusinessUnitId == 0 || emp.IntBusinessUnitId == IntBusinessUnitId)
                                               join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                                               from desig in desig2.DefaultIfEmpty()
                                               join dpt1 in _context.MasterDepartments on emp.IntDepartmentId equals dpt1.IntDepartmentId into dpt2
                                               from dept in dpt2.DefaultIfEmpty()
                                               where (_context.TimeRemoteAttendanceRegistrations.Where(x => x.IsActive == true && x.IntEmployeeId == emp.IntEmployeeBasicInfoId && x.IntMasterLocationId > 0 && !(x.IsPipelineClosed == true && x.IsReject == true)).Count() > 0)
                                               select new
                                               {
                                                   IntEmployeeBasicInfoId = emp.IntEmployeeBasicInfoId,
                                                   StrEmployeeName = emp.StrEmployeeName,
                                                   StrEmployeeCode = emp.StrEmployeeCode,
                                                   IntDesignationId = emp.IntDesignationId,
                                                   StrDesignation = desig == null ? "" : desig.StrDesignation,
                                                   IntDepartmentId = emp.IntDepartmentId,
                                                   StrDepartment = dept == null ? "" : dept.StrDepartment,
                                                   LocationList = (from reg in _context.TimeRemoteAttendanceRegistrations
                                                                   where reg.IsActive == true && reg.IntEmployeeId == emp.IntEmployeeBasicInfoId
                                                                   && !(reg.IsPipelineClosed == true && reg.IsReject == true)
                                                                   join ml in _context.MasterLocationRegisters on reg.IntMasterLocationId equals ml.IntMasterLocationId
                                                                   select new
                                                                   {
                                                                       IntMasterLocationId = reg.IntMasterLocationId,
                                                                       strPlaceName = ml.StrPlaceName,
                                                                       StrStatus = ml.StrStatus == "pending" ? "pending" : (ml.IsReject == true ? "rejected" : ((ml.IsPipelineClosed == true) ? "approved" : "process"))
                                                                   }).ToList()
                                               }).AsNoTracking().AsQueryable().ToListAsync();

                return locationDashBoard;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region ==========Shift Management ===========
        public async Task<dynamic> GetEmployeeShiftInfoAsync(long intEmployeeId, long intYear, long intMonth)
        {
            try
            {
                var employeeShiftAssign = await (from att in _context.TimeAttendanceDailySummaries
                                                 where att.IntMonthId == intMonth && att.IntYear == intYear && att.IntEmployeeId == intEmployeeId
                                                 select new
                                                 {
                                                     IntEmployeeId = att.IntEmployeeId,
                                                     DteAttendanceDate = att.DteAttendanceDate,
                                                     DayName = att.DteAttendanceDate.Value.DayOfWeek.ToString(),
                                                     IntDayId = att.IntDayId,
                                                     StrCalendarName = att.StrCalendarName,
                                                     StrCalendarType = att.StrCalendarType,
                                                     DteNextChangeDate = att.DteNextChangeDate
                                                 }).AsNoTracking().AsQueryable().OrderBy(x => x.DteAttendanceDate.Value.Date).ToListAsync();

                return employeeShiftAssign;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<dynamic> GetCalenderDdlAsync(long intAccountId, long intBusinessUnitId)
        {
            try
            {
                var calendarDdl = await (from cal in _context.TimeCalenders
                                         where cal.IntAccountId == intAccountId && cal.IntBusinessUnitId == intBusinessUnitId && cal.IsActive == true
                                         select new
                                         {
                                             IntCalenderId = cal.IntCalenderId,
                                             StrCalenderCode = cal.StrCalenderCode,
                                             StrCalenderName = cal.StrCalenderName,
                                             DteStartTime = cal.DteStartTime,
                                             dDteExtendedStartTime = cal.DteExtendedStartTime,
                                             DteLastStartTime = cal.DteLastStartTime,
                                             DteEndTime = cal.DteEndTime,
                                             dte = cal.DteOfficeStartTime,
                                             NumMinWorkHour = cal.NumMinWorkHour,
                                             IsNightShif = cal.IsNightShift,
                                             DteOfficeStartTime = cal.DteOfficeStartTime,
                                             DteOfficeCloseTime = cal.DteOfficeCloseTime,
                                             DteBreakStartTime = cal.DteBreakStartTime,
                                             DteBreakEndTime = cal.DteBreakEndTime
                                         }).AsNoTracking().AsQueryable().ToListAsync();
                return calendarDdl;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<dynamic> PostCalendarAssignAsync(CalendarAssignList model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                string employee = "";
                int count = model.IntEmployeeId.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        employee = employee + Convert.ToString(model.IntEmployeeId[i]);
                        employee = employee + (i < count - 1 ? "," : "");
                    }
                }
                if (model.Shifts.Count > 0 && count > 0)
                {
                    foreach (var shift in model.Shifts)
                    {
                        var fromDate = shift.Fromdate.ToString("yyyy-MM-dd");
                        var toDate = shift.Todate.ToString("yyyy-MM-dd");
                        var isNightShift = shift.IsNightShift != null && shift.IsNightShift == true ? "1" : "0";

                        var log = $@"INSERT INTO saas.LogTimeAttendanceHistory (intDayId, intMonthId, intYear, intEmployeeId, intCalendarTypeId, strCalendarType ,
                                    intCalendarId, strCalendarName, dteNextChangeDate, dteStartTime, dteExtendedStartTime, dteLastStartTime, dteEndTime, numMinWorkHour,
                                    isNightShift,dteOfficeOpeningTime,dteOfficeClosingTime, isActive ,intCreatedBy, dteCreatedAt,dteAttendanceDate)
                                    SELECT intDayId, intMonthId, intYear, intEmployeeId, intCalendarTypeId, strCalendarType , intCalendarId, strCalendarName, dteNextChangeDate, 
                                    dteStartTime, dteExtendedStartTime, dteLastStartTime, dteEndTime, numMinWorkHour,ISNULL(isNightShift,0),dteOfficeOpeningTime,dteOfficeClosingTime, 1, {model.IntActionBy},GETDATE(),dteAttendanceDate
                                    FROM saas.timeAttendanceDailySummary WHERE intEmployeeId IN ({employee}) AND dteAttendanceDate BETWEEN '{fromDate}' AND '{toDate}'";

                        if (await _context.Database.ExecuteSqlRawAsync(log) > 0)
                        {
                            string sql = @$"UPDATE saas.timeAttendanceDailySummary 
                                        SET intCalendarTypeId = 2, strCalendarType = 'Calendar General',intCalendarId = {shift.IntCalenderId},
                                        strCalendarName = '{shift.StrCalenderName}',dteNextChangeDate = '{shift.Todate.AddDays(1).Date.ToString("yyyy-MM-dd")}',
                                        dteStartTime = '{shift.DteOfficeStartTime}',dteExtendedStartTime = '{shift.DteExtendedStartTime}',
                                        dteLastStartTime = '{shift.DteLastStartTime}',dteEndTime = '{shift.DteEndTime}',numMinWorkHour = {shift.NumMinWorkHour},
                                        intRosterGroupId = 0, strRosterGroupName = 'N/A',isNightShift = {isNightShift},dteOfficeOpeningTime = '{shift.DteOfficeStartTime}',
                                        dteOfficeClosingTime = '{shift.DteOfficeCloseTime}'
                                        WHERE intEmployeeId IN ({employee}) AND dteAttendanceDate BETWEEN '{fromDate}' AND '{toDate}'";

                            await _context.Database.ExecuteSqlRawAsync(sql);
                        }

                    }
                }
                await transaction.CommitAsync();
                MessageHelper msg = new()
                {
                    AutoId = 0,
                    StatusCode = 200,
                    Message = "Success"
                };
                return msg;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
        public async Task<dynamic> LogAttendanceOfChangedCalendarAsync(long EmployeeId, DateTime FromDate, DateTime Todate)
        {
            try
            {
                if (FromDate.Month != Todate.Month || FromDate.Year != Todate.Year)
                {
                    throw new Exception("Month must be same");
                }

                var data = await (from log in _context.LogTimeAttendanceHistories
                                  join emp1 in _context.EmpEmployeeBasicInfos on log.IntCreatedBy equals emp1.IntEmployeeBasicInfoId into emp2
                                  from emp in emp2.DefaultIfEmpty()
                                  where log.IntEmployeeId == EmployeeId
                                  && log.DteAttendanceDate >= FromDate && log.DteAttendanceDate <= Todate
                                  && log.IsActive == true
                                  orderby log.DteCreatedAt
                                  select new
                                  {
                                      IntAutoId = log.IntAutoId,
                                      IntDayId = log.IntDayId,
                                      IntMonthId = log.IntMonthId,
                                      IntYear = log.IntYear,
                                      DteDate = log.DteAttendanceDate,
                                      IntEmployeeId = log.IntEmployeeId,
                                      IntCalendarTypeId = log.IntCalendarTypeId,
                                      StrCalendarType = log.StrCalendarType,
                                      IntCalendarId = log.IntCalendarId,
                                      StrCalendarName = log.StrCalendarName,
                                      DteNextChangeDate = log.DteNextChangeDate,
                                      DteStartTime = log.DteStartTime,
                                      DteExtendedStartTime = log.DteExtendedStartTime,
                                      DteLastStartTime = log.DteLastStartTime,
                                      DteEndTime = log.DteEndTime,
                                      NumMinWorkHour = log.NumMinWorkHour,
                                      IsNightShift = log.IsNightShift,
                                      DteOfficeOpeningTime = log.DteOfficeOpeningTime,
                                      DteOfficeClosingTime = log.DteOfficeClosingTime,
                                      IntCreatedBy = log.IntCreatedBy,
                                      StrCreatorName = emp == null ? "" : emp.StrEmployeeName,
                                      StrEmployeeCode = emp.StrEmployeeCode,
                                      DteCreatedAt = log.DteCreatedAt,
                                  }).AsQueryable().AsNoTracking().ToListAsync();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region====Off day reassign====
        public async Task<dynamic> EmployeeOffDayReassignAsync(EmployeeOffDayReassignList model)
        {
            try
            {
                if (model.IntEmployeeId.Count > 0 && model.Offdays.Count > 0)
                {
                    foreach (var emp in model.IntEmployeeId)
                    {
                        foreach (var offday in model.Offdays)
                        {
                            TimeEmployeeOffdayReassign offdayModel = new()
                            {
                                IntEmployeeId = emp,
                                IsOffday = offday.IsOffDay,
                                IsActive = offday.IsActive,
                                DteDate = offday.Date,
                            };
                            var exist = await _context.TimeEmployeeOffdayReassigns.Where(x => x.IntEmployeeId == emp && x.DteDate == offday.Date).FirstOrDefaultAsync();
                            if (exist != null)
                            {
                                exist.IsOffday = offdayModel.IsOffday;
                                exist.IsActive = offdayModel.IsActive;
                                exist.DteCreatedAt = exist.DteCreatedAt;
                                exist.IntCreatedBy = exist.IntCreatedBy;
                                exist.DteUpdatedAt = DateTimeExtend.BD;
                                exist.IntUpdatedBy = model.IntActionBy;

                                _context.TimeEmployeeOffdayReassigns.Update(exist);
                            }
                            else
                            {
                                offdayModel.DteCreatedAt = DateTimeExtend.BD;
                                offdayModel.IntCreatedBy = model.IntActionBy;

                                await _context.TimeEmployeeOffdayReassigns.AddAsync(offdayModel);
                            }
                        }

                    }
                }
                int res = await _context.SaveChangesAsync();

                MessageHelper msg = new();
                msg.AutoId = 0;
                msg.StatusCode = res > 0 ? 200 : 400;
                msg.Message = res > 0 ? "Success" : "Failed";
                return msg;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GetEmployeeOffDayReassignLandingAsync
        public async Task<dynamic> GetEmployeeOffDayReassignLandingAsync(int IntMonthId, int IntYearId, long EmployeeId)
        {
            try
            {
                var lastDay = DateTime.DaysInMonth(IntYearId, IntMonthId);

                var fromDate = new DateTime(IntYearId, IntMonthId, 1);
                var toDate = new DateTime(IntYearId, IntMonthId, lastDay);

                var OffdayList = await (from off in _context.TimeEmployeeOffdayReassigns
                                        where off.IntEmployeeId == EmployeeId && off.IsActive == true
                                        && (off.DteDate >= fromDate && off.DteDate <= toDate)
                                        select new
                                        {
                                            OffDayDate = off.DteDate,
                                            OffayIsOffday = off.IsOffday
                                        }).AsQueryable().AsNoTracking().ToListAsync();


                var weekendDays = await (from week in _context.TimeEmployeeOffdays
                                         where week.IntEmployeeId == EmployeeId && week.IsActive == true
                                         select week).AsQueryable().AsNoTracking().FirstOrDefaultAsync();

                if (OffdayList.Count <= 0 && weekendDays == null)
                {
                    return Array.Empty<string>();
                }

                List<OffdayStatus> offdayStatuses = new();
                var startDate = fromDate;
                for (int i = 1; i <= lastDay; i++)
                {
                    OffdayStatus status = new();
                    status.DteDate = startDate;
                    status.DayName = startDate.DayOfWeek.ToString();
                    status.DayId = i;
                    status.IsOffday = false;
                    int weekDay = (int)startDate.DayOfWeek;

                    var dateFromOffdayTable = OffdayList.FirstOrDefault(x => x.OffDayDate == startDate);
                    if (dateFromOffdayTable != null)
                    {
                        status.IsOffday = dateFromOffdayTable.OffayIsOffday;
                    }
                    else if (weekendDays != null)
                    {
                        if (weekDay == 0)
                        {
                            status.IsOffday = weekendDays.IsSunday != null ? (bool)weekendDays.IsSunday : false;
                        }
                        if (weekDay == 1)
                        {
                            status.IsOffday = weekendDays.IsMonday != null ? (bool)weekendDays.IsMonday : false;
                        }
                        if (weekDay == 2)
                        {
                            status.IsOffday = weekendDays.IsTuesday != null ? (bool)weekendDays.IsTuesday : false;
                        }
                        if (weekDay == 3)
                        {
                            status.IsOffday = weekendDays.IsWednesday != null ? (bool)weekendDays.IsWednesday : false;
                        }
                        if (weekDay == 4)
                        {
                            status.IsOffday = weekendDays.IsThursday != null ? (bool)weekendDays.IsThursday : false;
                        }
                        if (weekDay == 5)
                        {
                            status.IsOffday = weekendDays.IsFriday != null ? (bool)weekendDays.IsFriday : false;
                        }
                        if (weekDay == 6)
                        {
                            status.IsOffday = weekendDays.IsSaturday != null ? (bool)weekendDays.IsSaturday : false;
                        }

                    }
                    offdayStatuses.Add(status);
                    startDate = startDate.AddDays(1);

                }
                return offdayStatuses;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region=== Master Fixed Roaster===
        public async Task<dynamic> GetFixedRoasterMasterByIdAsync(long intAccountId, long intBusinessId)
        {
            try
            {
                var data = await (from mas in _context.TimeFixedRoasterSetupMasters
                                  where mas.IntAccountId == intAccountId
                                 && mas.IntBusinessUnit == intBusinessId && mas.IsActive == true
                                  select new
                                  {
                                      IntId = mas.IntId,
                                      IntAccountId = mas.IntAccountId,
                                      IntBusinessUnit = mas.IntBusinessUnit,
                                      StrRoasterName = mas.StrRoasterName
                                  }).AsQueryable().AsNoTracking().ToListAsync();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<dynamic> GetFixedRoasterDetaisByIdAsync(long intFixedMasterId)
        {
            try
            {
                var data = await (from ros in _context.TimeFixedRoasterSetupDetails
                                  where ros.IntFixedRoasterMasterId == intFixedMasterId
                                  && ros.IsActive == true
                                  select new
                                  {
                                      IntId = ros.IntId,
                                      IntDay = ros.IntDay,
                                      IntCalendarId = ros.IntCalendarId,
                                      StrCalendarName = ros.StrCalendarName,
                                      IsOffDay = ros.IsOffDay,
                                      IsHoliday = ros.IsHoliday,
                                      IntFixedRoasterMasterId = ros.IntFixedRoasterMasterId
                                  }).AsQueryable().AsNoTracking().ToListAsync();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<dynamic> CreateNUpdateFixedRoasterMasterAsync(FixedMasterRoaster model)
        {
            try
            {
                int res = 0;
                if (model.IntId > 0)
                {
                    var exist = await _context.TimeFixedRoasterSetupMasters.FirstOrDefaultAsync(x => x.IntId == model.IntId && x.IsActive == true);
                    if (exist != null)
                    {
                        exist.IntBusinessUnit = model.IntBusinessUnit;
                        exist.IntAccountId = model.IntAccountId;
                        exist.StrRoasterName = model.StrRoasterName;
                        exist.IntUpdatedBy = model.IntActionBy;
                        exist.DteUpdatedAt = DateTime.Now;
                        exist.IsActive = model.IsActive;

                        _context.Update(exist);
                        res = await _context.SaveChangesAsync();

                        if (res > 0 && model.IsActive == false)
                        {
                            await _context.Database.ExecuteSqlRawAsync($"UPDATE saas.timeFixedRoasterSetupDetails SET isActive=0 WHERE intFixedRoasterMasterId={model.IntId}");
                        }
                        return exist.IntId;
                    }
                }
                else
                {
                    TimeFixedRoasterSetupMaster newEntity = new()
                    {
                        IntAccountId = model.IntAccountId,
                        IntBusinessUnit = model.IntBusinessUnit,
                        StrRoasterName = model.StrRoasterName,
                        IsActive = model.IsActive,
                        IntCreatedBy = model.IntActionBy,
                        DteCreatedAt = DateTime.Now

                    };
                    await _context.TimeFixedRoasterSetupMasters.AddAsync(newEntity);
                    res = await _context.SaveChangesAsync();
                    return newEntity.IntId;
                }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<dynamic> CreateNUpdateFixedRoasterDeatailsAsync(List<FixedRoasterVm> model)
        {
            try
            {
                int count = model.Count;
                if (count > 0)
                {
                    foreach (var item in model)
                    {
                        if (item.IntDay >= 1 && item.IntDay <= 31)
                        {
                            var exist = await _context.TimeFixedRoasterSetupDetails.FirstOrDefaultAsync(x => (x.IntId == item.IntId || (x.IntFixedRoasterMasterId == item.MasterId && x.IntDay == item.IntDay)) && x.IsActive == true);
                            if (exist != null)
                            {
                                exist.IntDay = item.IntDay;
                                exist.IsActive = item.IsActive;
                                exist.IntFixedRoasterMasterId = item.MasterId;
                                exist.IntUpdatedBy = item.IntActionBy;
                                exist.IntCalendarId = item.IntCalendarId;
                                exist.IsHoliday = item.IsHoliday;
                                exist.IsOffDay = item.IsOffDay;
                                exist.StrCalendarName = item.StrCalendarName;
                                exist.DteUpdatedAt = DateTime.Now;
                                _context.TimeFixedRoasterSetupDetails.Update(exist);
                            }
                            else
                            {
                                TimeFixedRoasterSetupDetail newEntity = new()
                                {
                                    IntId = item.IntId,
                                    IntDay = item.IntDay,
                                    IntFixedRoasterMasterId = item.MasterId,
                                    IntCalendarId = item.IntCalendarId,
                                    StrCalendarName = item.StrCalendarName,
                                    IsHoliday = item.IsHoliday,
                                    IsOffDay = item.IsOffDay,
                                    IntCreatedBy = item.IntActionBy,
                                    DteCreatedAt = DateTime.Now,
                                    IsActive = item.IsActive
                                };
                                await _context.TimeFixedRoasterSetupDetails.AddAsync(newEntity);
                            }
                        }

                    }
                }
                int res = await _context.SaveChangesAsync();

                MessageHelper msg = new();
                msg.StatusCode = res > 0 ? 200 : 400;
                msg.Message = res > 0 ? "Success" : "Failed";

                return msg;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region === Active current inactive Employee===
        public async Task<MessageHelper> EmployeeInactive(long EmployeeId)
        {
            
            EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == EmployeeId && x.IsActive == true);
            EmpEmployeeBasicInfoDetail details = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == EmployeeId && x.IsActive == true);
            User user = await _context.Users.FirstOrDefaultAsync(x => (long)x.IntRefferenceId == EmployeeId && x.IsActive == true);

            if (emp is not null)
            {
                emp.IsActive = false;
                _context.EmpEmployeeBasicInfos.Update(emp);
            }

            if (details is not null)
            {
                details.IntEmployeeStatusId = 2;
                details.StrEmployeeStatus = "Inactive";

                _context.EmpEmployeeBasicInfoDetails.Update(details);
            }

            if (user is not null)
            {
                user.IsActive = false;
                _context.Users.Update(user);
            }

            if (user is not null || emp is not null || details is not null)
            {
                await _context.SaveChangesAsync();
            }

            return new MessageHelper { StatusCode = 200, Message = "Employee Inactive Successfully" };
        }
        public async Task<dynamic> PostAcitveCurrentInactiveEmployeeAsync(long IntEmployeeId)
        {
            try
            {
                MessageHelperUpdate msg = new();
                var existEmployee = await _context.EmpEmployeeBasicInfoDetails.FirstOrDefaultAsync(x => x.IntEmployeeId == IntEmployeeId);
                existEmployee.IntEmployeeStatusId = 1;
                existEmployee.StrEmployeeStatus = "Active";
                existEmployee.DteUpdatedAt = DateTime.Now;

                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.IntRefferenceId == IntEmployeeId && x.IsActive == false);
                if (existingUser != null)
                {
                    existingUser.IsActive = true;
                    existingUser.DteUpdatedAt = DateTime.Now;
                    _context.Users.Update(existingUser);
                }

                _context.EmpEmployeeBasicInfoDetails.Update(existEmployee);

                await _context.SaveChangesAsync();

                return msg;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<InactiveEmployeeListLanding> GetInactiveEmployeeList(BaseVM tokenData, long businessUnitId, long workplaceGroupId, DateTime? FromDate, DateTime? ToDate, int pageNo, int pageSize, string? searchTxt, bool IsPaginated,
            bool IsHeaderNeed, List<long> departments, List<long> designations, List<long> supervisors, List<long> LineManagers, List<long> employeementType, List<long> WingList, List<long> SoledepoList, List<long> RegionList,
            List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                searchTxt = !string.IsNullOrEmpty(searchTxt) ? searchTxt.ToLower() : searchTxt;

                var data = (from emp in _context.EmpEmployeeBasicInfos
                            join details1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals details1.IntEmployeeId into details2
                            from empD in details2.DefaultIfEmpty()
                            join desig1 in _context.MasterDesignations on emp.IntDesignationId equals desig1.IntDesignationId into desig2
                            from des in desig2.DefaultIfEmpty()
                            join dept1 in _context.MasterDepartments on emp.IntDepartmentId equals dept1.IntDepartmentId into dpt2
                            from dept in dpt2.DefaultIfEmpty()
                            join sep1 in _context.EmpEmployeeSeparations on emp.IntEmployeeBasicInfoId equals sep1.IntEmployeeId into sep2
                            from sep in sep2.DefaultIfEmpty()
                            join sal in _context.PyrEmployeeSalaryElementAssignHeaders on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = sal.IntEmployeeId, isActive = (bool)sal.IsActive } into empSal
                            from salary in empSal.DefaultIfEmpty()
                            join supervisor in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals supervisor.IntEmployeeBasicInfoId into empSupervisor
                            from sup in empSupervisor.DefaultIfEmpty()
                            join lm in _context.EmpEmployeeBasicInfos on emp.IntSupervisorId equals lm.IntEmployeeBasicInfoId into lm2
                            from lm in lm2.DefaultIfEmpty()

                            join father in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId, relId = (long)1, IsActive = true } equals new { empId = father.IntEmployeeBasicInfoId, relId = father.IntRelationShipId, IsActive = (bool)father.IsActive } into empFather
                            from fatherContact in empFather.DefaultIfEmpty()
                            join mother in _context.EmpEmployeeRelativesContacts on new { empId = emp.IntEmployeeBasicInfoId, relId = (long)2, IsActive = true } equals new { empId = mother.IntEmployeeBasicInfoId, relId = mother.IntRelationShipId, IsActive = (bool)mother.IsActive } into empMother
                            from motherContact in empMother.DefaultIfEmpty()
                            join bnk in _context.EmpEmployeeBankDetails on new { empId = emp.IntEmployeeBasicInfoId, isPrimarySalAcc = true, isActive = true } equals new { empId = bnk.IntEmployeeBasicInfoId, isPrimarySalAcc = bnk.IsPrimarySalaryAccount, isActive = (bool)bnk.IsActive } into empBnk
                            from bank in empBnk.DefaultIfEmpty()
                            join oth in _context.EmpEmployeePhotoIdentities on new { empId = emp.IntEmployeeBasicInfoId, isActive = true } equals new { empId = oth.IntEmployeeBasicInfoId, isActive = (bool)oth.IsActive } into empOther
                            from other in empOther.DefaultIfEmpty()
                            join bus in _context.MasterBusinessUnits on emp.IntBusinessUnitId equals bus.IntBusinessUnitId into empBusinessUnit
                            from businessUnit in empBusinessUnit.DefaultIfEmpty()
                            join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into empWorkplaceGroup
                            from workplaceGroup in empWorkplaceGroup.DefaultIfEmpty()
                            join presAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)1, IsActive = true } equals new { empId = presAdd.IntEmployeeBasicInfoId, aType = presAdd.IntAddressTypeId, IsActive = (bool)presAdd.IsActive } into empPresentAddress
                            from presentAddress in empPresentAddress.DefaultIfEmpty()
                            join permAdd in _context.EmpEmployeeAddresses on new { empId = emp.IntEmployeeBasicInfoId, aType = (long)2, IsActive = true } equals new { empId = permAdd.IntEmployeeBasicInfoId, aType = permAdd.IntAddressTypeId, IsActive = (bool)permAdd.IsActive } into empPermanentAddress
                            from permanentAddress in empPermanentAddress.DefaultIfEmpty()

                            join wi in _context.TerritorySetups on empD.IntWingId equals wi.IntTerritoryId into wi1
                            from wing in wi1.DefaultIfEmpty()
                            join soleD in _context.TerritorySetups on empD.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                            from soleDp in soleD1.DefaultIfEmpty()
                            join regn in _context.TerritorySetups on empD.IntRegionId equals regn.IntTerritoryId into regn1
                            from region in regn1.DefaultIfEmpty()
                            join area1 in _context.TerritorySetups on empD.IntAreaId equals area1.IntTerritoryId into area2
                            from area in area2.DefaultIfEmpty()
                            join terrty in _context.TerritorySetups on empD.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                            from Territory in terrty1.DefaultIfEmpty()

                            where emp.IntAccountId == tokenData.accountId && (emp.IsActive == false || empD.IntEmployeeStatusId == 2)
                            && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                            && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                            //&& (tokenData.workplaceList.Contains(0) || tokenData.workplaceList.Contains(workplaceId)) && emp.IntWorkplaceId == workplaceId
                            //&& emp.IntBusinessUnitId == businessUnitId && emp.IntWorkplaceGroupId == workplaceGroupId

                            && (emp.DteJoiningDate.Value.Date >= FromDate.Value.Date && emp.DteJoiningDate.Value.Date <= ToDate.Value.Date)
                            && !(sep.IsActive == true && sep.IsPipelineClosed == true && sep.IsReject == false && sep.IsReleased == true)

                            && (!string.IsNullOrEmpty(searchTxt) ? (emp.StrEmployeeName.ToLower().Contains(searchTxt) || emp.StrEmployeeCode.ToLower().Contains(searchTxt)
                               || emp.StrReferenceId.ToLower().Contains(searchTxt) || des.StrDesignation.ToLower().Contains(searchTxt) || dept.StrDepartment.ToLower().Contains(searchTxt)
                               || empD.StrPinNo.ToLower().Contains(searchTxt) || empD.StrPersonalMobile.ToLower().Contains(searchTxt)
                               || fatherContact.StrRelativesName.ToLower().Contains(searchTxt) || wing.StrTerritoryName.ToLower().Contains(searchTxt)
                               || soleDp.StrTerritoryName.ToLower().Contains(searchTxt) || region.StrTerritoryName.ToLower().Contains(searchTxt)
                               || area.StrTerritoryName.ToLower().Contains(searchTxt) || Territory.StrTerritoryName.ToLower().Contains(searchTxt)
                               || motherContact.StrRelativesName.ToLower().Contains(searchTxt) || empD.StrPersonalMobile.ToLower().Contains(searchTxt)
                               || other.StrNid.ToLower().Contains(searchTxt) || bank.StrAccountNo.ToLower().Contains(searchTxt)
                               || bank.StrRoutingNo.ToLower().Contains(searchTxt) || emp.StrReligion.ToLower().Contains(searchTxt))
                               : true)

                           && ((departments.Count > 0 ? departments.Contains((long)emp.IntDepartmentId) : true)
                              && (designations.Count > 0 ? designations.Contains((long)emp.IntDesignationId) : true)
                              && (supervisors.Count > 0 ? supervisors.Contains((long)emp.IntSupervisorId) : true)
                              && (LineManagers.Count > 0 ? LineManagers.Contains((long)emp.IntLineManagerId) : true)
                              && (employeementType.Count > 0 ? employeementType.Contains((long)emp.IntEmploymentTypeId) : true)
                              && (WingList.Count > 0 ? WingList.Contains((long)empD.IntWingId) : true)
                              && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)empD.IntSoleDepo) : true)
                              && (RegionList.Count > 0 ? RegionList.Contains((long)empD.IntRegionId) : true)
                              && (AreaList.Count > 0 ? AreaList.Contains((long)empD.IntAreaId) : true)
                              && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)empD.IntTerritoryId) : true))

                            orderby emp.IntEmployeeBasicInfoId ascending
                            select new
                            {
                                intEmployeeId = emp.IntEmployeeBasicInfoId,
                                intBusinessUnitId = emp.IntBusinessUnitId,
                                strBusinessUnit = businessUnit == null ? "" : businessUnit.StrBusinessUnit,
                                intWorkplaceGroupId = emp.IntWorkplaceGroupId,
                                strWorkplaceGroup = workplaceGroup == null ? "" : workplaceGroup.StrWorkplaceGroup,
                                StrEmployeeCode = emp.StrEmployeeCode,
                                StrEmployeeName = emp.StrEmployeeName,
                                IntDesignationId = emp.IntDesignationId,
                                StrDesignation = des == null ? "" : des.StrDesignation,
                                IntDepartmentId = emp.IntDepartmentId,
                                StrDepartment = dept == null ? "" : dept.StrDepartment,
                                DteJoiningDate = Convert.ToDateTime(emp.DteJoiningDate).ToString("dd MMM, yyyy"),
                                ServiceLength = YearMonthDayCalculate.YearMonthDayLongFormCal(emp.DteJoiningDate.Value.Date, (emp.DteLastWorkingDate.Value.Date == null ? DateTime.Now.Date : emp.DteLastWorkingDate.Value.Date)),
                                PayrollGroup = salary == null ? "" : salary.StrSalaryBreakdownHeaderTitle,
                                DateOfConfirmation = emp.DteConfirmationDate,
                                FatherName = fatherContact == null ? "" : fatherContact.StrRelativesName,
                                MotherName = motherContact == null ? "" : motherContact.StrRelativesName,
                                //PresentAddress = presentAddress == null ? "" : presentAddress.StrAddressDetails,
                                //PermanentAddress = permanentAddress == null ? "" : permanentAddress.StrAddressDetails,

                                presentAddress = (presentAddress.StrAddressDetails == null ? "" : "Village: " + presentAddress.StrAddressDetails + ", ") +
                                                  (presentAddress.StrPostOffice == null ? "" : "Post Office: " + presentAddress.StrPostOffice + (presentAddress.StrZipOrPostCode == null ? "" : "-" + presentAddress.StrZipOrPostCode) + ", ") +
                                                  (presentAddress.StrThana == null ? "" : "Thana: " + presentAddress.StrThana + ", ") +
                                                  (presentAddress.StrDistrictOrState == null ? "" : "District: " + presentAddress.StrDistrictOrState),

                                PermanentAddress = (permanentAddress.StrAddressDetails == null ? "" : "Village: " + permanentAddress.StrAddressDetails + ", ") +
                                                    (permanentAddress.StrPostOffice == null ? "" : "Post Office: " + permanentAddress.StrPostOffice + (permanentAddress.StrZipOrPostCode == null ? "" : "-" + permanentAddress.StrZipOrPostCode) + ", ") +
                                                    (permanentAddress.StrThana == null ? "" : "Thana: " + permanentAddress.StrThana + ", ") +
                                                    (permanentAddress.StrDistrictOrState == null ? "" : "District: " + permanentAddress.StrDistrictOrState),
                                DateOfBirth = emp.DteDateOfBirth,
                                Religion = emp.StrReligion,
                                Gender = emp.StrGender,
                                MaritialStatus = emp.StrMaritalStatus,
                                BloodGroup = emp.StrBloodGroup,
                                MobileNo = empD.StrOfficeMobile,
                                NID = other == null ? "" : other.StrNid,
                                BirthID = other == null ? "" : other.StrBirthId,
                                StrVehicleNo = empD.StrVehicleNo,
                                StrRemarks = empD.StrRemarks,
                                StrPersonalMobile = empD.StrPersonalMobile,
                                WingId = empD.IntWingId,
                                WingName = wing == null ? "" : wing.StrTerritoryName,
                                SoleDepoId = empD.IntSoleDepo,
                                SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                RegionId = empD.IntRegionId,
                                RegionName = region == null ? "" : region.StrTerritoryName,
                                AreaId = empD.IntAreaId,
                                AreaName = area == null ? "" : area.StrTerritoryName,
                                TerritoryId = empD.IntTerritoryId,
                                TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                GrossSalary = salary == null ? 0 : salary.NumGrossSalary,
                                TotalSalary = salary == null ? 0 : salary.NumGrossSalary,
                                intEmploymentTypeId = emp.IntEmploymentTypeId,
                                StrEmploymentType = emp.StrEmploymentType,
                                Email = empD.StrPersonalMail,
                                OfficeEmail = empD.StrOfficeMail,
                                IntSupervisorId = emp.IntSupervisorId,
                                StrSupervisorName = sup == null ? "" : sup.StrEmployeeName,
                                IntLineManagerId = emp.IntLineManagerId,
                                StrLinemanager = lm == null ? "" : lm.StrEmployeeName,
                                BankName = bank == null ? "" : bank.StrBankWalletName,
                                BranchName = bank == null ? "" : bank.StrBranchName,
                                AccountNo = bank == null ? "" : bank.StrAccountNo,
                                RoutingNo = bank == null ? "" : bank.StrRoutingNo,
                                StrPinNo = empD.StrPinNo,
                                StrStatus = "Inactive"
                            }).AsNoTracking().AsQueryable();

                InactiveEmployeeListLanding retObj = new InactiveEmployeeListLanding();

                if (IsHeaderNeed)
                {
                    EmployeeHeader eh = new();

                    var datas = data.Select(x => new
                    {
                        WingId = x.WingId,
                        WingName = x.WingName,
                        SoleDepoId = x.SoleDepoId,
                        SoleDepoName = x.SoleDepoName,
                        RegionId = x.RegionId,
                        RegionName = x.RegionName,
                        AreaId = x.AreaId,
                        AreaName = x.AreaName,
                        TerritoryId = x.TerritoryId,
                        TerritoryName = x.TerritoryName,
                        IntDepartmentId = x.IntDepartmentId,
                        StrDepartment = x.StrDepartment,
                        IntDesignationId = x.IntDesignationId,
                        StrDesignation = x.StrDesignation,
                        IntSupervisorId = x.IntSupervisorId,
                        StrSupervisorName = x.StrSupervisorName,
                        IntLineManagerId = x.IntLineManagerId,
                        StrLinemanager = x.StrLinemanager,
                        IntEmploymentTypeId = x.intEmploymentTypeId,
                        StrEmploymentType = x.StrEmploymentType
                    }).Distinct().ToList();

                    eh.WingNameList = datas.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).DistinctBy(x => x.Value).ToList();
                    eh.SoleDepoNameList = datas.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).DistinctBy(x => x.Value).ToList();
                    eh.RegionNameList = datas.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).DistinctBy(x => x.Value).ToList();
                    eh.AreaNameList = datas.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).DistinctBy(x => x.Value).ToList();
                    eh.TerritoryNameList = datas.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).DistinctBy(x => x.Value).ToList();
                    eh.StrDepartmentList = datas.Where(x => !string.IsNullOrEmpty(x.StrDepartment)).Select(x => new CommonDDLVM { Value = (long)x.IntDepartmentId, Label = (string)x.StrDepartment }).DistinctBy(x => x.Value).ToList();
                    eh.StrDesignationList = datas.Where(x => !string.IsNullOrEmpty(x.StrDesignation)).Select(x => new CommonDDLVM { Value = (long)x.IntDesignationId, Label = (string)x.StrDesignation }).DistinctBy(x => x.Value).ToList();
                    eh.StrSupervisorNameList = datas.Where(x => !string.IsNullOrEmpty(x.StrSupervisorName)).Select(x => new CommonDDLVM { Value = (long)x.IntSupervisorId, Label = (string)x.StrSupervisorName }).DistinctBy(x => x.Value).ToList();
                    eh.StrLinemanagerList = datas.Where(x => !string.IsNullOrEmpty(x.StrLinemanager)).Select(x => new CommonDDLVM { Value = (long)x.IntLineManagerId, Label = (string)x.StrLinemanager }).DistinctBy(x => x.Value).ToList();
                    eh.StrEmploymentTypeList = datas.Where(x => !string.IsNullOrEmpty(x.StrEmploymentType)).Select(x => new CommonDDLVM { Value = (long)x.IntEmploymentTypeId, Label = (string)x.StrEmploymentType }).DistinctBy(x => x.Value).ToList();

                    retObj.EmployeeHeader = eh;
                }

                if (IsPaginated == false)
                {
                    retObj.Data = await data.ToListAsync();
                }
                else
                {
                    int maxSize = 1000;
                    pageSize = pageSize > maxSize ? maxSize : pageSize;
                    pageNo = pageNo < 1 ? 1 : pageNo;

                    retObj.TotalCount = await data.CountAsync();
                    retObj.Data = await data.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToListAsync();
                    retObj.PageSize = pageSize;
                    retObj.CurrentPage = pageNo;
                }

                return retObj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion


        #region =============================ALL METHOD====================================
        public async Task<EmpEmployeeBasicInfo> EmployeeBasicInfoCreateNUpdateMethod(EmployeeBasicCreateNUpdateVM model, EmpEmployeeBasicInfo? updateEmployee, long accountId, long CreateBy)
        {
            if (updateEmployee != null && model.IntEmployeeBasicInfoId > 0)
            {
                if (_context.Accounts.Where(n => n.IntAccountId == accountId && n.IsEmployeeCodeAutoGenerated == true).Count() <= 0)
                {
                    updateEmployee.StrEmployeeCode = model.StrEmployeeCode;
                }

                updateEmployee.IntWorkplaceId = model.IntWorkplaceId;
                updateEmployee.StrCardNumber = model.StrCardNumber;
                updateEmployee.StrEmployeeName = model.StrEmployeeName;
                updateEmployee.IntGenderId = model.IntGenderId;
                updateEmployee.StrGender = model.StrGender;
                updateEmployee.IntReligionId = model.IntReligionId;
                updateEmployee.StrReligion = model.StrReligion;
                updateEmployee.IntDepartmentId = model.IntDepartmentId;
                updateEmployee.IntDesignationId = model.IntDesignationId;
                updateEmployee.DteDateOfBirth = model.DteDateOfBirth;
                updateEmployee.DteJoiningDate = model.DteJoiningDate;
                updateEmployee.DteInternCloseDate = model.DteInternCloseDate;
                updateEmployee.DteProbationaryCloseDate = model.DteProbationaryCloseDate;
                updateEmployee.DteConfirmationDate = model.DteConfirmationDate;
                updateEmployee.DteContactFromDate = model.DteContactFromDate;
                updateEmployee.DteContactToDate = model.DteContactToDate;
                updateEmployee.IntSupervisorId = model.IntSupervisorId;
                updateEmployee.IntLineManagerId = model.IntLineManagerId;
                updateEmployee.IntDottedSupervisorId = model.IntDottedSupervisorId;
                updateEmployee.IsSalaryHold = model.IsSalaryHold;
                updateEmployee.IsActive = model.IsActive;
                updateEmployee.IsUserInactive = model.IsUserInactive;
                updateEmployee.IsRemoteAttendance = model.IsRemoteAttendance;
                updateEmployee.StrMaritalStatus = model.StrMaritalStatus;
                updateEmployee.StrBloodGroup = model.StrBloodGroup;
                updateEmployee.IntEmploymentTypeId = model.IntEmploymentTypeId;
                updateEmployee.StrEmploymentType = model.StrEmploymentType;
                updateEmployee.DteUpdatedAt = DateTimeExtend.BD;
                updateEmployee.IntUpdatedBy = CreateBy;

                _context.EmpEmployeeBasicInfos.Update(updateEmployee);
                await _context.SaveChangesAsync();

                return updateEmployee;
            }
            else
            {
                long employeeCode = 0;

                if (_context.Accounts.Where(n => n.IntAccountId == accountId && n.IsEmployeeCodeAutoGenerated == true).Count() > 0)
                {
                    employeeCode = await _context.EmpEmployeeBasicInfos
                                    .Where(e => e.IntAccountId == accountId
                                    && Convert.ToInt64(e.StrEmployeeCode) >= (model.IntWorkplaceGroupId == 1 ? 30000000 : model.IntWorkplaceGroupId == 2 ? 10000000 : model.IntWorkplaceGroupId == 3 ? 70000000 : -1)
                                    && (model.IntWorkplaceGroupId == 3 ? true : Convert.ToInt64(e.StrEmployeeCode) < (model.IntWorkplaceGroupId == 1 ? 70000000 : model.IntWorkplaceGroupId == 2 ? 30000000 : -1)))
                                    .MaxAsync(e => Convert.ToInt64(e.StrEmployeeCode)) + 1;

                    model.StrEmployeeCode = Convert.ToString(employeeCode);
                }

                EmpEmployeeBasicInfo createEmp = new EmpEmployeeBasicInfo
                {
                    StrEmployeeCode = model.StrEmployeeCode,
                    StrCardNumber = model.StrCardNumber,
                    StrEmployeeName = model.StrEmployeeName,
                    IntBusinessUnitId = model.IntBusinessUnitId,
                    IntWorkplaceGroupId = model.IntWorkplaceGroupId,
                    IntWorkplaceId = model.IntWorkplaceId,
                    IntEmploymentTypeId = model.IntEmploymentTypeId,
                    StrEmploymentType = model.StrEmploymentType,
                    DteConfirmationDate = model.DteConfirmationDate,
                    StrReferenceId = model.StrEmployeeCode,
                    IntGenderId = model.IntGenderId,
                    StrGender = model.StrGender,
                    IntReligionId = model.IntReligionId,
                    StrReligion = model.StrReligion,
                    StrMaritalStatus = model.StrMaritalStatus,
                    StrBloodGroup = model.StrBloodGroup,
                    IntDepartmentId = model.IntDepartmentId,
                    IntDesignationId = model.IntDesignationId,
                    DteDateOfBirth = model.DteDateOfBirth,
                    DteJoiningDate = model.DteJoiningDate,
                    DteInternCloseDate = model.DteInternCloseDate,
                    DteProbationaryCloseDate = model.DteProbationaryCloseDate,
                    IntSupervisorId = model.IntSupervisorId,
                    IntLineManagerId = model.IntLineManagerId,
                    IntDottedSupervisorId = model.IntDottedSupervisorId,
                    DteContactFromDate = model.DteContactFromDate,
                    DteContactToDate = model.DteContactToDate,
                    IsSalaryHold = model.IsSalaryHold,
                    IsActive = true,
                    IsUserInactive = model.IsUserInactive,
                    IsRemoteAttendance = model.IsRemoteAttendance,
                    IntAccountId = accountId,
                    IntCreatedBy = CreateBy,
                    DteCreatedAt = DateTimeExtend.BD
                };

                await _context.EmpEmployeeBasicInfos.AddAsync(createEmp);
                await _context.SaveChangesAsync();

                return createEmp;
            }

        }
        public async Task<EmpEmployeeBasicInfoDetail> EmployeeBasicInfoDetailsCreateNUpdateMethod(EmployeeBasicCreateNUpdateVM model, long CreateBy)
        {

            EmpEmployeeBasicInfoDetail? empDetails = await _context.EmpEmployeeBasicInfoDetails.Where(n => n.IntEmployeeId == model.IntEmployeeBasicInfoId).AsNoTracking().FirstOrDefaultAsync();

            if (empDetails != null)
            {
                empDetails.StrOfficeMail = model.StrOfficeMail != null && model.StrOfficeMail != "" ? model.StrOfficeMail : empDetails.StrOfficeMail;
                empDetails.StrPersonalMail = model.StrPersonalMail != null && model.StrPersonalMail != "" ? model.StrPersonalMail : empDetails.StrPersonalMail;
                empDetails.StrPersonalMobile = model.StrPersonalMobile != null && model.StrPersonalMobile != "" ? model.StrPersonalMobile : empDetails.StrPersonalMobile;
                empDetails.StrOfficeMobile = model.StrOfficeMobile != null && model.StrOfficeMobile != "" ? model.StrOfficeMobile : empDetails.StrOfficeMobile;
                empDetails.IntCalenderId = model.IntCalenderId;
                empDetails.StrCalenderName = model.StrCalenderName;
                empDetails.IntHrpositionId = model.IntHrpositionId;
                empDetails.StrHrpostionName = model.StrHrpostionName;
                empDetails.IntEmployeeStatusId = model.IntEmployeeStatusId;
                empDetails.StrEmployeeStatus = model.StrEmployeeStatus;
                empDetails.IsTakeHomePay = model.IsTakeHomePay;
                empDetails.IntWingId = model.WingId;
                empDetails.IntSoleDepo = model.SoleDepoId;
                empDetails.IntRegionId = model.RegionId;
                empDetails.IntAreaId = model.AreaId;
                empDetails.IntTerritoryId = model.TerritoryId;
                empDetails.IsActive = model.IsActive;
                empDetails.IntUpdatedBy = CreateBy;
                empDetails.DteUpdatedAt = DateTimeExtend.BD;

                _context.EmpEmployeeBasicInfoDetails.Update(empDetails);
                await _context.SaveChangesAsync();

                return empDetails;
            }
            else
            {
                EmpEmployeeBasicInfoDetail employeeBasicInfoDetail = new EmpEmployeeBasicInfoDetail
                {
                    IntEmployeeId = model.IntEmployeeBasicInfoId,
                    IntEmployeeStatusId = 1,
                    StrEmployeeStatus = "Active",
                    IntHrpositionId = model.IntHrpositionId,
                    StrHrpostionName = model.StrHrpostionName,
                    IntWingId = model.WingId,
                    IntSoleDepo = model.SoleDepoId,
                    IntRegionId = model.RegionId,
                    IntAreaId = model.AreaId,
                    IntTerritoryId = model.TerritoryId,
                    IsTakeHomePay = model.IsTakeHomePay,
                    IsActive = true,
                    StrOfficeMail = (bool)model.IsCreateUser == true ? model.UserViewModel.IntOfficeMail : model?.StrOfficeMail,
                    StrPersonalMobile = (bool)model.IsCreateUser == true ? model.UserViewModel.StrContactNo : model?.StrPersonalMobile,
                    DteCreatedAt = DateTimeExtend.BD,
                    IntCreatedBy = CreateBy
                };
                await _context.EmpEmployeeBasicInfoDetails.AddAsync(employeeBasicInfoDetail);
                await _context.SaveChangesAsync();

                return employeeBasicInfoDetail;
            }
        }

        public async Task<bool> RoleExtensionCreateNUpdate(EmployeeBasicCreateNUpdateVM model, long CreateBy, bool isCreate)
        {
            try
            {
                long roleExtensionHeaderId = 0;
                if (isCreate)
                {
                    RoleExtensionHeader roleExtension = new RoleExtensionHeader
                    {
                        IntEmployeeId = model.IntEmployeeBasicInfoId,
                        IntCreatedBy = (long)CreateBy,
                        DteCreatedDateTime = DateTimeExtend.BD,
                        IsActive = true
                    };

                    await _context.RoleExtensionHeaders.AddAsync(roleExtension);
                    await _context.SaveChangesAsync();

                    roleExtensionHeaderId = roleExtension.IntRoleExtensionHeaderId;
                }
                else
                {
                    roleExtensionHeaderId = await _context.RoleExtensionHeaders.Where(n => n.IntEmployeeId == model.IntEmployeeBasicInfoId && n.IsActive == true).Select(n => n.IntRoleExtensionHeaderId).FirstOrDefaultAsync();
                }


                List<RoleExtensionRow> roleRowList = new();
                int roleCount = 0;

                if (!isCreate)
                {
                    roleCount = (from hd in _context.RoleExtensionHeaders
                                 join er in _context.RoleExtensionRows on hd.IntRoleExtensionHeaderId equals er.IntRoleExtensionHeaderId
                                 where er.IntEmployeeId == model.IntEmployeeBasicInfoId && er.IntOrganizationTypeId == 1 && er.IntOrganizationReffId == 0 && er.IsActive == true
                                 select er).Count();
                }

                if (isCreate == true || (roleCount == 0 && (_context.RoleExtensionRows.Where(n => n.IntEmployeeId == model.IntEmployeeBasicInfoId && n.IntOrganizationTypeId == 1 && n.IntOrganizationReffId == model.IntBusinessUnitId).Count() <= 0)))
                {
                    RoleExtensionRow roleRowBu = new RoleExtensionRow
                    {
                        IntRoleExtensionHeaderId = roleExtensionHeaderId,
                        IntEmployeeId = model.IntEmployeeBasicInfoId,
                        IntOrganizationTypeId = 1,
                        StrOrganizationTypeName = "Business Unit",
                        IntOrganizationReffId = model.IntBusinessUnitId,
                        StrOrganizationReffName = _context.MasterBusinessUnits.Where(n => n.IntBusinessUnitId == model.IntBusinessUnitId).Select(n => n.StrBusinessUnit).FirstOrDefault(),
                        IntCreatedBy = (long)CreateBy,
                        DteCreatedDateTime = DateTimeExtend.BD,
                        IsActive = true
                    };
                    roleRowList.Add(roleRowBu);
                }
                roleCount = 0;

                if (!isCreate)
                {
                    roleCount = (from hd in _context.RoleExtensionHeaders
                                 join er in _context.RoleExtensionRows on hd.IntRoleExtensionHeaderId equals er.IntRoleExtensionHeaderId
                                 where er.IntEmployeeId == model.IntEmployeeBasicInfoId && er.IntOrganizationTypeId == 2 && er.IntOrganizationReffId == 0 && er.IsActive == true
                                 select er).Count();
                }

                if (isCreate == true || (roleCount == 0 && (_context.RoleExtensionRows.Where(n => n.IntEmployeeId == model.IntEmployeeBasicInfoId && n.IntOrganizationTypeId == 2 && n.IntOrganizationReffId == model.IntWorkplaceGroupId).Count() <= 0)))
                {
                    RoleExtensionRow roleRowWg = new RoleExtensionRow
                    {
                        IntRoleExtensionHeaderId = roleExtensionHeaderId,
                        IntEmployeeId = model.IntEmployeeBasicInfoId,
                        IntOrganizationTypeId = 2,
                        StrOrganizationTypeName = "Workplace Group",
                        IntOrganizationReffId = model.IntWorkplaceGroupId,
                        StrOrganizationReffName = _context.MasterWorkplaceGroups.Where(n => n.IntWorkplaceGroupId == model.IntWorkplaceGroupId).Select(n => n.StrWorkplaceGroup).FirstOrDefault(),
                        IntCreatedBy = (long)CreateBy,
                        DteCreatedDateTime = DateTimeExtend.BD,
                        IsActive = true
                    };
                    roleRowList.Add(roleRowWg);
                }
                roleCount = 0;

                if (!isCreate)
                {
                    roleCount = (from hd in _context.RoleExtensionHeaders
                                 join er in _context.RoleExtensionRows on hd.IntRoleExtensionHeaderId equals er.IntRoleExtensionHeaderId
                                 where er.IntEmployeeId == model.IntEmployeeBasicInfoId && er.IntOrganizationTypeId == 3 && er.IntOrganizationReffId == 0 && er.IsActive == true
                                 select er).Count();
                }

                if (isCreate == true || (roleCount == 0 && (_context.RoleExtensionRows.Where(n => n.IntEmployeeId == model.IntEmployeeBasicInfoId && n.IntOrganizationTypeId == 3 && n.IntOrganizationReffId == model.IntWorkplaceId).Count() <= 0)))
                {
                    RoleExtensionRow roleRowW = new RoleExtensionRow
                    {
                        IntRoleExtensionHeaderId = roleExtensionHeaderId,
                        IntEmployeeId = model.IntEmployeeBasicInfoId,
                        IntOrganizationTypeId = 3,
                        StrOrganizationTypeName = "Workplace",
                        IntOrganizationReffId = model.IntWorkplaceId,
                        StrOrganizationReffName = _context.MasterWorkplaces.Where(n => n.IntWorkplaceId == model.IntWorkplaceId).Select(n => n.StrWorkplace).FirstOrDefault(),
                        IntCreatedBy = (long)CreateBy,
                        DteCreatedDateTime = DateTimeExtend.BD,
                        IsActive = true
                    };
                    roleRowList.Add(roleRowW);
                }

                if (roleRowList.Count() > 0)
                {
                    _context.RoleExtensionRows.AddRange(roleRowList);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion =============================ALL METHOD====================================

        #region =============================Get Employee All Role And Extensions=============================
        public async Task<RoleExtensionsList> GetEmployeeRoleExtensions(long IntEmployeeId, long IntAccountId)
        {
            try
            {
                var data = await (from rh in _context.RoleExtensionHeaders
                                  join rr in _context.RoleExtensionRows on new { a = rh.IntRoleExtensionHeaderId, b = (bool)rh.IsActive, c = true }
                                  equals new { a = rr.IntRoleExtensionHeaderId, b = true, c = (bool)rr.IsActive }
                                  where rh.IntEmployeeId == IntEmployeeId
                                  select rr).ToListAsync();

                RoleExtensionsList roleExtensionsList = new();

                roleExtensionsList.BusinessUnitList = data.Where(x => x.IntOrganizationTypeId == 1).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.WorkGroupList = data.Where(x => x.IntOrganizationTypeId == 2).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.WorkPlaceList = data.Where(x => x.IntOrganizationTypeId == 3).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.WingList = data.Where(x => x.IntOrganizationTypeId == 4).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.SoleDepoList = data.Where(x => x.IntOrganizationTypeId == 5).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.RegiontList = data.Where(x => x.IntOrganizationTypeId == 6).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.AreaList = data.Where(x => x.IntOrganizationTypeId == 7).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();
                roleExtensionsList.TeritoryList = data.Where(x => x.IntOrganizationTypeId == 8).Select(x => new CommonDDLVM { Label = x.StrOrganizationReffName, Value = x.IntOrganizationReffId }).ToList();

                //if (roleExtensionsList.BusinessUnitList.Count == 1)
                //{
                //    long id = roleExtensionsList.BusinessUnitList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.BusinessUnitList = _context.MasterBusinessUnits.Where(x => x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntBusinessUnitId, Label = x.StrBusinessUnit }).ToList();
                //    }

                //}
                //if (roleExtensionsList.WorkGroupList.Count == 1)
                //{
                //    long id = roleExtensionsList.WorkGroupList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.WorkGroupList = _context.MasterWorkplaceGroups.Where(x => x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntWorkplaceGroupId, Label = x.StrWorkplaceGroup }).ToList();
                //    }

                //}
                //if (roleExtensionsList.WorkPlaceList.Count == 1)
                //{
                //    long id = roleExtensionsList.WorkPlaceList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.WorkPlaceList = _context.MasterWorkplaces.Where(x => x.IntAccountId == IntAccountId && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntWorkplaceId, Label = x.StrWorkplace }).ToList();
                //    }

                //}
                //if (roleExtensionsList.WingList.Count == 1)
                //{
                //    long id = roleExtensionsList.WingList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.WingList = _context.TerritorySetups.Where(x => x.IntAccountId == IntAccountId && x.IntTerritoryTypeId == 1 && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntTerritoryId, Label = x.StrTerritoryName }).ToList();
                //    }

                //}
                //if (roleExtensionsList.SoleDepoList.Count == 1)
                //{
                //    long id = roleExtensionsList.SoleDepoList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.SoleDepoList = _context.TerritorySetups.Where(x => x.IntAccountId == IntAccountId && x.IntTerritoryTypeId == 2 && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntTerritoryId, Label = x.StrTerritoryName }).ToList();
                //    }

                //}
                //if (roleExtensionsList.RegiontList.Count == 1)
                //{
                //    long id = roleExtensionsList.RegiontList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.RegiontList = _context.TerritorySetups.Where(x => x.IntAccountId == IntAccountId && x.IntTerritoryTypeId == 3 && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntTerritoryId, Label = x.StrTerritoryName }).ToList();
                //    }

                //}
                //if (roleExtensionsList.AreaList.Count == 1)
                //{
                //    long id = roleExtensionsList.AreaList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.AreaList = _context.TerritorySetups.Where(x => x.IntAccountId == IntAccountId && x.IntTerritoryTypeId == 4 && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntTerritoryId, Label = x.StrTerritoryName }).ToList();
                //    }

                //}
                //if (roleExtensionsList.TeritoryList.Count == 1)
                //{
                //    long id = roleExtensionsList.TeritoryList.Select(x => x.Value).First();

                //    if (id == 0)
                //    {
                //        roleExtensionsList.TeritoryList = _context.TerritorySetups.Where(x => x.IntAccountId == IntAccountId && x.IntTerritoryTypeId == 5 && x.IsActive == true).Select(x => new CommonDDLVM { Value = x.IntTerritoryId, Label = x.StrTerritoryName }).ToList();
                //    }

                //}

                return roleExtensionsList;
            }
            catch (Exception ex)
            {
                return new RoleExtensionsList();
            }
        }

        #endregion
        #region ===== Attendance Process ====
        public async Task<MessageHelper> TimeAttendanceProcess(TimeAttendanceProcessViewModel obj)
        {
            MessageHelper msg = new MessageHelper();
            try
            {
                List<TimeAttendanceProcessRequestRow> rowDataList = new List<TimeAttendanceProcessRequestRow>();
                var model = new TimeAttendanceProcessRequestHeader
                {
                    IntAccountId = obj.IntAccountId,
                    IntBusinessUnitId = obj.IntBusinessUnitId,
                    IntTotalEmployee = obj.IntTotalEmployee,
                    DteProcessDate = DateTime.UtcNow,
                    DteFromDate = obj.DteFromDate,
                    DteToDate = obj.DteToDate,
                    Status = obj.Status,
                    IsAll = obj.IsAll,
                    IsActive = true,
                    DteCreateDate = DateTime.UtcNow,
                    IntCreateBy = obj.IntCreateBy
                };

                await _context.TimeAttendanceProcessRequestHeaders.AddAsync(model);
                await _context.SaveChangesAsync();

                long id = model.IntHeaderRequestId;

                if (obj != null)
                {
                    foreach (var row in obj.processRequestRowVMs)
                    {
                        var rowModel = new TimeAttendanceProcessRequestRow
                        {
                            IntHeaderRequestId = id,
                            IntEmployeeId = row.IntEmployeeId,
                            IsActive = true,
                            DteCreateDate = DateTime.UtcNow,
                            IntCreateBy = obj.IntCreateBy,
                        };

                        rowDataList.Add(rowModel);
                    }
                }

                if (rowDataList.Count() > 0)
                {
                    await _context.TimeAttendanceProcessRequestRows.AddRangeAsync(rowDataList);
                    await _context.SaveChangesAsync();

                    model.IntTotalEmployee = rowDataList.Count();

                    _context.TimeAttendanceProcessRequestHeaders.Update(model);
                    await _context.SaveChangesAsync();
                }

                msg.Message = "Successfully Saved!";
                msg.StatusCode = 200;

                return msg;
            }
            catch (Exception)
            {
                throw new Exception("Error!");
            }
        }

        public async Task<List<TimeAttendanceProcessResponseVM>> GetTimeAttendanceProcess(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                List<TimeAttendanceProcessResponseVM> dataList = await (from header in _context.TimeAttendanceProcessRequestHeaders
                                                                        where header.DteProcessDate >= FromDate && header.DteProcessDate <= ToDate
                                                                        select new TimeAttendanceProcessResponseVM
                                                                        {
                                                                            IntHeaderRequestId = header.IntHeaderRequestId,
                                                                            IntAccountId = header.IntAccountId,
                                                                            IntBusinessUnitId = header.IntBusinessUnitId,
                                                                            DteFromDate = header.DteFromDate,
                                                                            DteToDate = header.DteToDate,
                                                                            IntTotalEmployee = _context.TimeAttendanceProcessRequestRows.Where(x => x.IntHeaderRequestId == header.IntHeaderRequestId && x.IsActive == true).Count(),
                                                                            Status = header.Status,
                                                                            IsActive = header.IsActive,
                                                                            IsAll = header.IsAll,

                                                                            ProcessEmployees = (from rows in _context.TimeAttendanceProcessRequestRows
                                                                                                join emp in _context.EmpEmployeeBasicInfos on rows.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                                                join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId
                                                                                                join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId
                                                                                                where rows.IntHeaderRequestId == header.IntHeaderRequestId && rows.IsActive == true
                                                                                                select new ProcessEmployee
                                                                                                {
                                                                                                    IntEmployeeId = rows.IntEmployeeId,
                                                                                                    StrEmployeeName = emp.StrEmployeeName,
                                                                                                    StrDepartment = dept.StrDepartment,
                                                                                                    StrDesignation = desig.StrDesignation
                                                                                                }).ToList()
                                                                        }).AsNoTracking().ToListAsync();

                return dataList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public async Task<IEnumerable<CommonEmployeeDDL>> GetCommonEmployeeDDL(BaseVM tokenData, long businessUnitId, long workplaceGroupId, long? employeeId, string? searchText)
        {
            
            try
            {
                IEnumerable<CommonEmployeeDDL> data = await (from emp in _context.EmpEmployeeBasicInfos
                                                             join empT1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals empT1.IntEmploymentTypeId into empT2
                                                             from empT in empT2.DefaultIfEmpty()
                                                             join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                                             from empD in empD2.DefaultIfEmpty()
                                                             join des1 in _context.MasterDesignations on emp.IntDesignationId equals des1.IntDesignationId into des2
                                                             from des in des2.DefaultIfEmpty()
                                                             where emp.IntAccountId == tokenData.accountId && (employeeId > 0 ? emp.IntEmployeeBasicInfoId == employeeId : true)
                                                             && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                                                             && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                                                             && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                             : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                             : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                                                             : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                                                             : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                                                             : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                                                             : tokenData.territoryList.Contains(empD.IntTerritoryId))
                                                             || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)
                                                            && (searchText == null || emp.StrEmployeeName.ToLower().Contains(searchText.ToLower()) || emp.StrEmployeeCode.ToLower().Contains(searchText.ToLower()))
                                                             select new CommonEmployeeDDL
                                                             {
                                                                 EmployeeId = emp.IntEmployeeBasicInfoId,
                                                                 EmployeeName = emp.StrEmployeeName,
                                                                 EmployeeCode = emp.StrEmployeeCode,
                                                                 EmployeeNameWithCode = emp.StrEmployeeName + " [" + emp.StrEmployeeCode + "]" + ", " + des.StrDesignation,
                                                                 EmploymentTypeId = empT != null ? empT.IntEmploymentTypeId : 0,
                                                                 EmploymentType = empT != null ? empT.StrEmploymentType : "",
                                                                 Designation = des.IntDesignationId,
                                                                 DesignationName = des.StrDesignation
                                                             }).AsNoTracking().ToListAsync();

                return data;
            }
            catch (Exception)
            {
                return new List<CommonEmployeeDDL>();
            }


        }

        public async Task<IEnumerable<CommonEmployeeDDL>> PermissionCheckFromEmployeeListByEnvetFireEmployee(BaseVM tokenData, long businessUnitId, long workplaceGroupId, List<long> employeeIdList, string? searchText)
        {

            try
            {
                IEnumerable<CommonEmployeeDDL> data = await (from emp in _context.EmpEmployeeBasicInfos
                                                             join empT1 in _context.MasterEmploymentTypes on emp.IntEmploymentTypeId equals empT1.IntEmploymentTypeId into empT2
                                                             from empT in empT2.DefaultIfEmpty()
                                                             join empD1 in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals empD1.IntEmployeeId into empD2
                                                             from empD in empD2.DefaultIfEmpty()
                                                             where emp.IntAccountId == tokenData.accountId && (employeeIdList.Count() > 0 ? employeeIdList.Contains(emp.IntEmployeeBasicInfoId) : true)
                                                             && (tokenData.businessUnitList.Contains(0) || tokenData.businessUnitList.Contains(businessUnitId)) && emp.IntBusinessUnitId == businessUnitId
                                                             && (tokenData.workplaceGroupList.Contains(0) || tokenData.workplaceGroupList.Contains(workplaceGroupId)) && emp.IntWorkplaceGroupId == workplaceGroupId

                                                             && (((tokenData.isOfficeAdmin == true || tokenData.workplaceGroupList.Contains(0)) ? true
                                                             : tokenData.wingList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId)
                                                             : tokenData.soleDepoList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId)
                                                             : tokenData.regionList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo)
                                                             : tokenData.areaList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId)
                                                             : tokenData.territoryList.Contains(0) ? tokenData.workplaceGroupList.Contains(emp.IntWorkplaceGroupId) && tokenData.wingList.Contains(empD.IntWingId) && tokenData.soleDepoList.Contains(empD.IntSoleDepo) && tokenData.regionList.Contains(empD.IntRegionId) && tokenData.areaList.Contains(empD.IntAreaId)
                                                             : tokenData.territoryList.Contains(empD.IntTerritoryId))
                                                             || emp.IntDottedSupervisorId == tokenData.employeeId || emp.IntSupervisorId == tokenData.employeeId || emp.IntLineManagerId == tokenData.employeeId)

                                                            && (searchText == null || emp.StrEmployeeName.ToLower().Contains(searchText.ToLower()) || emp.StrEmployeeCode.ToLower().Contains(searchText.ToLower()))
                                                             select new CommonEmployeeDDL
                                                             {
                                                                 EmployeeId = emp.IntEmployeeBasicInfoId,
                                                                 EmployeeName = emp.StrEmployeeName,
                                                                 EmployeeCode = emp.StrEmployeeCode,
                                                                 EmployeeNameWithCode = emp.StrEmployeeName + " (" + emp.StrEmployeeCode + ")",
                                                                 EmploymentTypeId = empT != null ? empT.IntEmploymentTypeId : 0,
                                                                 EmploymentType = empT != null ? empT.StrEmploymentType : ""
                                                             }).AsNoTracking().ToListAsync();

                return data;
            }
            catch (Exception)
            {
                return new List<CommonEmployeeDDL>();
            }


        }

        #region === Addition N Deduction ===
        public async Task<MessageHelperWithValidation> BulkSalaryAdditionNDeduction(BulkSalaryAdditionNDeductionViewModel model)
        {
            try
            {
                List<BulkSalaryAdditionNDeductionVM> models = model.bulkSalaryAdditionNDeductions;

                var valuss = models.Select(s => s.strAdditionNDeduction.ToLower()).Distinct();
                var adn = models.Select(s => s.EmployeeCode).Distinct();

                var additionNDeduction = await _context.PyrPayrollElementTypes.Where(n => n.IsPrimarySalary == false && n.IsActive == true && valuss.Contains(n.StrPayrollElementName.ToLower())).ToListAsync();

                var Emp = await _context.EmpEmployeeBasicInfos.Where(n => adn.Contains(n.StrEmployeeCode) && n.IsActive == true).ToListAsync();

                List<PyrEmpSalaryAdditionNdeduction> finalData = (from et in models
                                                                  join ad in additionNDeduction on et.strAdditionNDeduction equals ad.StrPayrollElementName
                                                                  join e in Emp on et.EmployeeCode equals e.StrEmployeeCode
                                                                  select new PyrEmpSalaryAdditionNdeduction
                                                                  {
                                                                      IntAccountId = e.IntAccountId,
                                                                      IntBusinessUnitId = e.IntBusinessUnitId,
                                                                      IntEmployeeId = e.IntEmployeeBasicInfoId,
                                                                      IsAutoRenew = et.isAutoRenew,
                                                                      IntYear = et.intYear,
                                                                      IntMonth = et.intMonth,
                                                                      StrMonth = et.strMonth,
                                                                      IntToYear = et.intToYear,
                                                                      IntToMonth = et.intToMonth,
                                                                      StrToMonth = et.strToMonth,
                                                                      IsAddition = et.isAddition,
                                                                      StrAdditionNdeduction = et.strAdditionNDeduction,
                                                                      IntAdditionNdeductionTypeId = ad.IntPayrollElementTypeId,
                                                                      IntAmountWillBeId = et.strAmountWillBe == "Percentage Of Basic" ? 1 : et.strAmountWillBe == "Percentage Of Gross" ? 2 : et.strAmountWillBe == "Fixed Amount" ? 3 : 0,
                                                                      StrAmountWillBe = et.strAmountWillBe,
                                                                      NumAmount = et.numAmount,
                                                                      StrCreatedBy = et.intActionBy.ToString(),
                                                                      DteCreatedAt = DateTime.Now,
                                                                      StrStatus = "Pending",
                                                                      IsActive = true,
                                                                      IsReject = false,
                                                                      IntPipelineHeaderId = _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(e.IntEmployeeBasicInfoId, "salaryAdditionNDeduction").Result.HeaderId, //await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(e.IntEmployeeBasicInfoId, "salaryAdditionNDeduction")
                                                                      IntCurrentStage = _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(e.IntEmployeeBasicInfoId, "salaryAdditionNDeduction").Result.CurrentStageId,
                                                                      IntNextStage = _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(e.IntEmployeeBasicInfoId, "salaryAdditionNDeduction").Result.NextStageId
                                                                  }).ToList();

                List<PyrEmpSalaryAdditionNdeduction> EmpSalaryAdditionNdeductions = await _context.PyrEmpSalaryAdditionNdeductions.Where(n => n.IsActive == true && n.IsReject == false).AsNoTracking().ToListAsync();

                ////  ad = existing data
                ////  fd = new data

                List<PyrEmpSalaryAdditionNdeduction> existAllowNDedu = (from ad in EmpSalaryAdditionNdeductions
                                                                       join fd in finalData on ad.IntEmployeeId equals fd.IntEmployeeId
                                                                       where fd.IntAdditionNdeductionTypeId == ad.IntAdditionNdeductionTypeId
                                                                       && (((ad.IntYear < fd.IntYear && ad.IsAutoRenew == true) || (ad.IntYear == fd.IntYear && ad.IntMonth <= fd.IntMonth && ad.IsAutoRenew == true))
                                                                        || ((ad.IntYear < fd.IntToYear || (ad.IntYear == fd.IntToYear && ad.IntMonth <= fd.IntToMonth)) &&
                                                                            (ad.IntToYear > fd.IntYear || (ad.IntToYear == fd.IntYear && ad.IntToMonth >= fd.IntMonth))))
                                                                       select new PyrEmpSalaryAdditionNdeduction
                                                                       {
                                                                           IntSalaryAdditionAndDeductionId = ad.IntSalaryAdditionAndDeductionId,
                                                                           IntAccountId = ad.IntAccountId,
                                                                           IntBusinessUnitId = ad.IntBusinessUnitId,
                                                                           IntEmployeeId = ad.IntEmployeeId,
                                                                           IsAutoRenew = ad.IsAutoRenew,
                                                                           IntYear = ad.IntYear,
                                                                           IntMonth = ad.IntMonth,
                                                                           StrMonth = ad.StrMonth,
                                                                           IntToYear = ad.IntToYear,
                                                                           IntToMonth = ad.IntToMonth,
                                                                           StrToMonth = ad.StrToMonth,
                                                                           IsAddition = ad.IsAddition,
                                                                           StrAdditionNdeduction = ad.StrAdditionNdeduction,
                                                                           IntAdditionNdeductionTypeId = ad.IntAdditionNdeductionTypeId,
                                                                           IntAmountWillBeId = fd.IntAmountWillBeId,
                                                                           StrAmountWillBe = fd.StrAmountWillBe,
                                                                           NumAmount = fd.NumAmount,
                                                                           StrCreatedBy = fd.StrCreatedBy,
                                                                           DteCreatedAt = DateTime.Now,
                                                                           StrStatus = "Pending",
                                                                           IsActive = true,
                                                                           IsReject = false,
                                                                           IntPipelineHeaderId = fd.IntPipelineHeaderId,
                                                                           IntCurrentStage = fd.IntCurrentStage,
                                                                           IntNextStage = fd.IntNextStage,
                                                                           IsPipelineClosed = false
                                                                       }).ToList();

                MessageHelperWithValidation message = new();

                if (existAllowNDedu.Count > 0 && model.IsForceAssign == false && model.IsSkipNAssign == false)
                {
                    List<BulkSalaryAdditionNDeductionVM> existAllowNDeduEmp = existAllowNDedu.Select(n => new BulkSalaryAdditionNDeductionVM
                    {
                        intSalaryAdditionAndDeductionId = n.IntAdditionNdeductionTypeId,
                        intAccountId = n.IntAccountId,
                        intBusinessUnitId = n.IntBusinessUnitId,
                        IntEmployeeId = n.IntEmployeeId,
                        EmployeeCode = _context.EmpEmployeeBasicInfos.FirstOrDefault(i => i.IntEmployeeBasicInfoId == n.IntEmployeeId).StrEmployeeCode,
                        EmployeeName = _context.EmpEmployeeBasicInfos.FirstOrDefault(i => i.IntEmployeeBasicInfoId == n.IntEmployeeId).StrEmployeeName,
                        isAutoRenew = n.IsAutoRenew,
                        intYear = n.IntYear,
                        intMonth = n.IntMonth,
                        strMonth = n.StrMonth,
                        isAddition = n.IsAddition,
                        strAdditionNDeduction = n.StrAdditionNdeduction,
                        intAmountWillBeId = n.IntAmountWillBeId,
                        strAmountWillBe = n.StrAmountWillBe,
                        numAmount = n.NumAmount,
                        intToYear = n.IntToYear,
                        intToMonth = n.IntToMonth,
                        strToMonth = n.StrToMonth
                    }).ToList();

                    message.StatusCode = 400;
                    message.Message = "Exists";
                    message.ValidationData = existAllowNDeduEmp;
                    return message;
                }

                if (model.IsForceAssign == true)
                {
                    _context.PyrEmpSalaryAdditionNdeductions.UpdateRange(existAllowNDedu);

                    List<PyrEmpSalaryAdditionNdeduction> newAllNDedu = finalData.Where(n => !existAllowNDedu.Any(i => n.IntEmployeeId == i.IntEmployeeId && n.IntAdditionNdeductionTypeId == i.IntAdditionNdeductionTypeId)).ToList();

                    await _context.PyrEmpSalaryAdditionNdeductions.AddRangeAsync(newAllNDedu);
                }
                else if (model.IsSkipNAssign == true)
                {
                    List<PyrEmpSalaryAdditionNdeduction> newAllNDedu = finalData.Where(n => !existAllowNDedu.Any(i => n.IntEmployeeId == i.IntEmployeeId && n.IntAdditionNdeductionTypeId == i.IntAdditionNdeductionTypeId)).ToList();

                    await _context.PyrEmpSalaryAdditionNdeductions.AddRangeAsync(newAllNDedu);
                }
                else
                {
                    await _context.PyrEmpSalaryAdditionNdeductions.AddRangeAsync(finalData);
                }
                await _context.SaveChangesAsync();

                message.StatusCode = 200;
                message.Message = "Allowance and deduction assign successfully.";

                return message;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<EmpSalaryAdditionNDeductionLanding> SalaryAdditionDeductionLanding(long AccountId, long IntMonth, long IntYear, long BusinessUnitId, long WorkplaceGroupId, int PageNo, int PageSize, string? searchTxt, bool IsHeaderNeed,
             List<long> WingList, List<long> SoledepoList, List<long> RegionList, List<long> AreaList, List<long> TerritoryList)
        {
            try
            {
                searchTxt = !string.IsNullOrEmpty(searchTxt) ? searchTxt.ToLower() : "";

                List<EmpSalaryAdditionNDeductionLandingVM> dataSets = await (from addi in _context.PyrEmpSalaryAdditionNdeductions
                                                                         join emp in _context.EmpEmployeeBasicInfos on addi.IntEmployeeId equals emp.IntEmployeeBasicInfoId into emp2
                                                                         from emp in emp2.DefaultIfEmpty()
                                                                         join info in _context.EmpEmployeeBasicInfoDetails on emp.IntEmployeeBasicInfoId equals info.IntEmployeeId into info2
                                                                         from info in info2.DefaultIfEmpty()
                                                                         join desig in _context.MasterDesignations on emp.IntDesignationId equals desig.IntDesignationId into desig2
                                                                         from desig in desig2.DefaultIfEmpty()
                                                                         join dept in _context.MasterDepartments on emp.IntDepartmentId equals dept.IntDepartmentId into dept2
                                                                         from dept in dept2.DefaultIfEmpty()
                                                                         join wp in _context.MasterWorkplaces on emp.IntWorkplaceId equals wp.IntWorkplaceId into wp2
                                                                         from wp in wp2.DefaultIfEmpty()
                                                                         join wg in _context.MasterWorkplaceGroups on emp.IntWorkplaceGroupId equals wg.IntWorkplaceGroupId into wg2
                                                                         from wg in wg2.DefaultIfEmpty()
                                                                         join bu in _context.MasterBusinessUnits on addi.IntBusinessUnitId equals bu.IntBusinessUnitId into bu2
                                                                         from bu in bu2.DefaultIfEmpty()
                                                                         where addi.IntAccountId == AccountId && addi.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                                                         && addi.IsActive == true && emp.IsActive == true
                                                                         && (!string.IsNullOrEmpty(searchTxt) ? emp.StrEmployeeName.ToLower().Contains(searchTxt)
                                                                                    || emp.StrEmployeeCode.ToLower().Contains(searchTxt)
                                                                                    || desig.StrDesignation.ToLower().Contains(searchTxt)
                                                                                    || wg.StrWorkplaceGroup.ToLower().Contains(searchTxt)
                                                                                    || dept.StrDepartment.ToLower().Contains(searchTxt) : true)
                                                                         group new
                                                                         {
                                                                             addi.IntSalaryAdditionAndDeductionId,
                                                                             addi.IntYear,
                                                                             addi.IntMonth,
                                                                             addi.IntEmployeeId,
                                                                             addi.StrMonth,
                                                                             emp.StrEmployeeName,
                                                                             emp.StrEmployeeCode,
                                                                             desig.StrDesignation,
                                                                             dept.StrDepartment,
                                                                             emp.IntWorkplaceGroupId,
                                                                             wg.StrWorkplaceGroup,
                                                                             wp.StrWorkplace,
                                                                             emp.IntBusinessUnitId,
                                                                             bu.StrBusinessUnit,
                                                                             addi.IntToYear,
                                                                             addi.IntToMonth,
                                                                             addi.IsActive,
                                                                             addi.IsAddition,
                                                                             addi.IsAutoRenew,
                                                                             info.IntWingId,
                                                                             info.IntSoleDepo,
                                                                             info.IntRegionId,
                                                                             info.IntAreaId,
                                                                             info.IntTerritoryId
                                                                         } by new
                                                                         {
                                                                             addi.IntSalaryAdditionAndDeductionId,
                                                                             addi.IntYear,
                                                                             addi.IntMonth,
                                                                             addi.IntEmployeeId,
                                                                             addi.StrMonth,
                                                                             emp.StrEmployeeName,
                                                                             emp.StrEmployeeCode,
                                                                             desig.StrDesignation,
                                                                             dept.StrDepartment,
                                                                             emp.IntWorkplaceGroupId,
                                                                             wg.StrWorkplaceGroup,
                                                                             wp.StrWorkplace,
                                                                             emp.IntBusinessUnitId,
                                                                             bu.StrBusinessUnit,
                                                                             addi.IntToYear,
                                                                             addi.IntToMonth,
                                                                             addi.IsActive,
                                                                             addi.IsAddition,
                                                                             addi.IsAutoRenew,
                                                                             info.IntWingId,
                                                                             info.IntSoleDepo,
                                                                             info.IntRegionId,
                                                                             info.IntAreaId,
                                                                             info.IntTerritoryId
                                                                         }
                                                                         into gp
                                                                         orderby gp.Key.StrEmployeeName ascending
                                                                         select new EmpSalaryAdditionNDeductionLandingVM
                                                                         {
                                                                             intSalaryAdditionAndDeductionId = gp.Key.IntSalaryAdditionAndDeductionId,
                                                                             Year = gp.Key.IntYear,
                                                                             Month = gp.Key.IntMonth,
                                                                             intEmployeeId = gp.Key.IntEmployeeId,
                                                                             MonthName = gp.Key.StrMonth,
                                                                             StrEmployeeName = gp.Key.StrEmployeeName,
                                                                             StrEmployeeCode = gp.Key.StrEmployeeCode,
                                                                             StrDesignation = gp.Key.StrDesignation,
                                                                             StrDepartment = gp.Key.StrDepartment,
                                                                             intWorkplaceGroupId = gp.Key.IntWorkplaceGroupId,
                                                                             StrWorkplaceGroup = gp.Key.StrWorkplaceGroup,
                                                                             StrWorkplace = gp.Key.StrWorkplace,
                                                                             intBusinessUnitId = gp.Key.IntBusinessUnitId,
                                                                             BusinessUnit = gp.Key.StrBusinessUnit,
                                                                             ToYear = gp.Key.IntToYear,
                                                                             ToMonth = gp.Key.IntToMonth,
                                                                             isActive = gp.Key.IsActive,
                                                                             isAddition = gp.Key.IsAddition,
                                                                             isAutoRenew = gp.Key.IsAutoRenew,
                                                                             FromDate = new DateTime((int)gp.Key.IntYear, (int)gp.Key.IntMonth, 1),
                                                                             WingId = gp.Key.IntWingId,
                                                                             SoleDepoId = gp.Key.IntSoleDepo,
                                                                             RegionId = gp.Key.IntRegionId,
                                                                             AreaId = gp.Key.IntAreaId,
                                                                             TerritoryId = gp.Key.IntTerritoryId
                                                                         }).AsNoTracking().ToListAsync();

                List<EmpSalaryAdditionNDeductionLandingVM> data = (from info in dataSets
                                                                   join wi in _context.TerritorySetups on info.WingId equals wi.IntTerritoryId into wi1
                                                                   from wing in wi1.DefaultIfEmpty()
                                                                   join soleD in _context.TerritorySetups on info.SoleDepoId equals soleD.IntTerritoryId into soleD1
                                                                   from soleDp in soleD1.DefaultIfEmpty()
                                                                   join regn in _context.TerritorySetups on info.RegionId equals regn.IntTerritoryId into regn1
                                                                   from region in regn1.DefaultIfEmpty()
                                                                   join area1 in _context.TerritorySetups on info.AreaId equals area1.IntTerritoryId into area2
                                                                   from area in area2.DefaultIfEmpty()
                                                                   join terrty in _context.TerritorySetups on info.TerritoryId equals terrty.IntTerritoryId into terrty1
                                                                   from Territory in terrty1.DefaultIfEmpty()
                                                                   where (WingList.Count > 0 ? WingList.Contains((long)info.WingId) : true)
                                                                     && (SoledepoList.Count > 0 ? SoledepoList.Contains((long)info.SoleDepoId) : true)
                                                                     && (RegionList.Count > 0 ? RegionList.Contains((long)info.RegionId) : true)
                                                                     && (AreaList.Count > 0 ? AreaList.Contains((long)info.AreaId) : true)
                                                                     && (TerritoryList.Count > 0 ? TerritoryList.Contains((long)info.TerritoryId) : true)


                                                                   select new EmpSalaryAdditionNDeductionLandingVM
                                                                   {
                                                                       intSalaryAdditionAndDeductionId = info.intSalaryAdditionAndDeductionId,
                                                                       Year = info.Year,
                                                                       Month = info.Month,
                                                                       ToMonth = info.ToMonth,
                                                                       ToYear = info.ToYear,
                                                                       intEmployeeId = info.intEmployeeId,
                                                                       MonthName = info.MonthName,
                                                                       StrEmployeeName = info.StrEmployeeName + " [" + info.StrEmployeeCode + "]" ?? "",
                                                                       StrDesignation = info.StrDesignation,
                                                                       StrDepartment = info.StrDepartment,
                                                                       intWorkplaceGroupId = info.intWorkplaceGroupId,
                                                                       StrWorkplaceGroup = info.StrWorkplaceGroup,
                                                                       StrWorkplace = info.StrWorkplace,
                                                                       intBusinessUnitId = info.intBusinessUnitId,
                                                                       BusinessUnit = info.BusinessUnit,
                                                                       isActive = info.isActive,
                                                                       isAddition = info.isAddition,
                                                                       isAutoRenew = info.isAutoRenew,
                                                                       FromDate = new DateTime((int)info.Year, (int)info.Month, 1),
                                                                       ToDate = info.ToYear == null || info.ToMonth == null? null : new DateTime((int)info.ToYear, (int)info.ToMonth, new DateTime((int)info.ToYear, (int)info.ToMonth, 1).AddMonths(1).AddDays(-1).Day),
                                                                       WingId = info.WingId,
                                                                       WingName = wing == null ? "" : wing.StrTerritoryName,
                                                                       SoleDepoId = info.SoleDepoId,
                                                                       SoleDepoName = soleDp == null ? "" : soleDp.StrTerritoryName,
                                                                       RegionId = info.RegionId,
                                                                       RegionName = region == null ? "" : region.StrTerritoryName,
                                                                       AreaId = info.AreaId,
                                                                       AreaName = area == null ? "" : area.StrTerritoryName,
                                                                       TerritoryId = info.TerritoryId,
                                                                       TerritoryName = Territory == null ? "" : Territory.StrTerritoryName,
                                                                   }).ToList();
                
                EmpSalaryAdditionNDeductionLanding retObj = new EmpSalaryAdditionNDeductionLanding();

                if (IsHeaderNeed && WorkplaceGroupId == 3)
                {
                    EmployeeHeader eh = new();

                    var datas = data.Select(x => new
                    {
                        WingId = x.WingId,
                        WingName = x.WingName,
                        SoleDepoId = x.SoleDepoId,
                        SoleDepoName = x.SoleDepoName,
                        RegionId = x.RegionId,
                        RegionName = x.RegionName,
                        AreaId = x.AreaId,
                        AreaName = x.AreaName,
                        TerritoryId = x.TerritoryId,
                        TerritoryName = x.TerritoryName
                    }).Distinct().ToList();

                    eh.WingNameList = datas.Where(x => !string.IsNullOrEmpty(x.WingName)).Select(x => new CommonDDLVM { Value = (long)x.WingId, Label = (string)x.WingName }).DistinctBy(x => x.Value).ToList();
                    eh.SoleDepoNameList = datas.Where(x => !string.IsNullOrEmpty(x.SoleDepoName)).Select(x => new CommonDDLVM { Value = (long)x.SoleDepoId, Label = (string)x.SoleDepoName }).DistinctBy(x => x.Value).ToList();
                    eh.RegionNameList = datas.Where(x => !string.IsNullOrEmpty(x.RegionName)).Select(x => new CommonDDLVM { Value = (long)x.RegionId, Label = (string)x.RegionName }).DistinctBy(x => x.Value).ToList();
                    eh.AreaNameList = datas.Where(x => !string.IsNullOrEmpty(x.AreaName)).Select(x => new CommonDDLVM { Value = (long)x.AreaId, Label = (string)x.AreaName }).DistinctBy(x => x.Value).ToList();
                    eh.TerritoryNameList = datas.Where(x => !string.IsNullOrEmpty(x.TerritoryName)).Select(x => new CommonDDLVM { Value = (long)x.TerritoryId, Label = (string)x.TerritoryName }).DistinctBy(x => x.Value).ToList();

                    retObj.EmployeeHeader = eh;
                }
                else
                {
                    retObj.EmployeeHeader = new EmployeeHeader();
                }

                int maxSize = 1000;
                PageSize = PageSize > maxSize ? maxSize : PageSize;
                PageNo = PageNo < 1 ? 1 : PageNo;

                DateTime? paramFromDate = new DateTime((int)IntYear, (int)IntMonth, 1);
                DateTime? paramToDate = new DateTime((int)IntYear, (int)IntMonth, new DateTime((int)IntYear, (int)IntMonth, 1).AddMonths(1).AddDays(-1).Day);

                retObj.Data = data.Where(x => ((x.isAutoRenew == true && x.Year < IntYear) || (x.isAutoRenew == true && x.Year == IntYear && x.Month <= IntMonth)) || 
                    (x.isAutoRenew == false && x.FromDate.Value.Date <= paramToDate.Value.Date && x.ToDate.Value.Date >= paramFromDate.Value.Date)).ToList();

                retObj.TotalCount = retObj.Data.Count();
                retObj.Data = retObj.Data.Skip(PageSize * (PageNo - 1)).Take(PageSize).ToList();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;

                return retObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public async Task<EmployeeMovementPaginationVM> EmployeeMovementReportAll(long BusinessUnitId, long WorkplaceGroupId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, string SearchText, bool IsPaginated, BaseVM tokenData)
        {
            try
            {
                List<MovementApplicationViewModel> movements = (from ml in _context.LveMovementApplications
                                                                join emp in _context.EmpEmployeeBasicInfos on ml.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                                where ml.IntAccountId == tokenData.accountId && ml.IsActive == true && ml.IsReject == false
                                                                && emp.IntBusinessUnitId == BusinessUnitId && emp.IntWorkplaceGroupId == WorkplaceGroupId
                                                                && ((ml.DteFromDate.Date >= FromDate.Date && ml.DteFromDate.Date <= ToDate.Date)
                                                                        || (ml.DteToDate.Date >= FromDate.Date && ml.DteToDate.Date <= ToDate.Date)
                                                                        || (FromDate.Date >= ml.DteFromDate.Date && FromDate.Date <= ml.DteToDate.Date)
                                                                        || (ToDate.Date >= ml.DteFromDate.Date && ToDate.Date <= ml.DteToDate.Date))
                                                                select new MovementApplicationViewModel()
                                                                {
                                                                    EmployeeBasicInfoId = ml.IntEmployeeId,
                                                                    EmployeeName = emp.StrEmployeeName,
                                                                    EmployeeCode = emp.StrEmployeeCode,
                                                                    DepartmentId = (long)emp.IntDepartmentId,
                                                                    DesignationId = (long)emp.IntDesignationId,
                                                                    EmploymentTypeId = emp.IntEmploymentTypeId,
                                                                    EmploymentType = emp.StrEmploymentType,
                                                                    RawFromDate = ml.DteFromDate,
                                                                    RawToDate = ml.DteToDate,
                                                                    RawDuration = 0
                                                                }).ToList();

                movements.ForEach(n =>
                {
                    if (n.RawFromDate.Value.Date >= FromDate.Date && n.RawFromDate.Value.Date <= ToDate.Date && n.RawToDate.Value.Date >= FromDate.Date && n.RawToDate.Value.Date <= ToDate.Date)
                    {
                        TimeSpan days = n.RawToDate.Value.Date - n.RawFromDate.Value.Date;
                        n.RawDuration = days.Days + 1;
                    }
                    else if ((n.RawFromDate.Value.Date < FromDate.Date || n.RawFromDate.Value.Date > ToDate.Date) && (n.RawToDate.Value.Date >= FromDate.Date && n.RawToDate.Value.Date <= ToDate.Date))
                    {
                        TimeSpan days = n.RawToDate.Value.Date - FromDate.Date;
                        n.RawDuration = days.Days + 1;
                    }
                    else if ((n.RawFromDate.Value.Date >= FromDate.Date && n.RawFromDate.Value.Date <= ToDate.Date) && (n.RawToDate.Value.Date < FromDate.Date || n.RawToDate.Value.Date > ToDate.Date))
                    {
                        TimeSpan days = ToDate.Date - n.RawFromDate.Value.Date;
                        n.RawDuration = days.Days + 1;
                    }
                    else if ((FromDate.Date >= n.RawFromDate.Value.Date && FromDate.Date <= n.RawToDate.Value.Date) && (ToDate.Date >= n.RawFromDate.Value.Date && ToDate.Date <= n.RawToDate.Value.Date))
                    {
                        TimeSpan days = ToDate.Date - FromDate.Date;
                        n.RawDuration = days.Days + 1;
                    }
                });

                var MovementReport = (from mnt in movements
                                      join dep in _context.MasterDepartments on mnt.DepartmentId equals dep.IntDepartmentId into deps
                                      from dep in deps.DefaultIfEmpty()
                                      join desi in _context.MasterDesignations on mnt.DesignationId equals desi.IntDesignationId into desis
                                      from desi in desis.DefaultIfEmpty()
                                      group mnt by new
                                      {
                                          mnt.EmployeeBasicInfoId,
                                          mnt.EmployeeName,
                                          mnt.EmployeeCode,
                                          mnt.DepartmentId,
                                          dep.StrDepartment,
                                          mnt.DesignationId,
                                          desi.StrDesignation,
                                          mnt.EmploymentTypeId,
                                          mnt.EmploymentType
                                      } into g
                                      select new MovementApplicationViewModel()
                                      {
                                          EmployeeBasicInfoId = g.Key.EmployeeBasicInfoId,
                                          EmployeeName = g.Key.EmployeeName,
                                          EmployeeCode = g.Key.EmployeeCode,
                                          DepartmentId = g.Key.DepartmentId,
                                          DepartmentName = g.Key.StrDepartment,
                                          DesignationId = g.Key.DesignationId,
                                          DesignationName = g.Key.StrDesignation,
                                          EmploymentTypeId = g.Key.EmploymentTypeId,
                                          EmploymentType = g.Key.EmploymentType,
                                          RawDuration = g.Sum(r => r.RawDuration)
                                      }).AsEnumerable();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = SearchText.ToLower();
                    MovementReport = MovementReport.Where(x => x.EmployeeName.ToLower().Contains(SearchText) ||
                                                               x.EmployeeCode.ToLower().Contains(SearchText) ||
                                                               x.DepartmentName.ToLower().Contains(SearchText) ||
                                                               x.DesignationName.ToLower().Contains(SearchText));


                }

                EmployeeMovementPaginationVM retObj = new();

                retObj.TotalCount = MovementReport.Count();
                retObj.PageSize = PageSize;
                retObj.CurrentPage = PageNo;
                retObj.Data = IsPaginated == true ? MovementReport.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList() : MovementReport.ToList();
                return retObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}