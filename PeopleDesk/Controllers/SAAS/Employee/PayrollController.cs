using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Master.Interfaces;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Data;

namespace PeopleDesk.Controllers.SAAS.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : Controller
    {
        private readonly PeopleDeskContext _context;
        private readonly ISaasMasterService _saasMasterService;
        private readonly IEmployeeService _employeeService;
        private readonly IPayrollService _payrollService;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private DataTable dt = new DataTable();

        public PayrollController(IEmployeeService _employeeService, ISaasMasterService _saasMasterService, IPayrollService payrollService, IApprovalPipelineService approvalPipelineService, PeopleDeskContext _context)
        {
            this._context = _context;
            this._saasMasterService = _saasMasterService;
            this._employeeService = _employeeService;
            this._payrollService = payrollService;
            this._approvalPipelineService = approvalPipelineService;
        }

        #region ========== Paryroll Configuration ==================

        [HttpPost]
        [Route("SavePayrollElementType")]
        public async Task<IActionResult> SavePayrollElementType(PyrPayrollElementType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                if (await _context.PyrPayrollElementTypes.Where(x => x.StrPayrollElementName.ToLower() == model.StrPayrollElementName.ToLower()
                && x.IntAccountId == model.IntAccountId && x.IntPayrollElementTypeId != model.IntPayrollElementTypeId
                && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else if (model.IsBasicSalary == true && await _context.PyrPayrollElementTypes.Where(x => x.IntAccountId == model.IntAccountId && x.IntPayrollElementTypeId != model.IntPayrollElementTypeId
                && x.IsActive == true && x.IsBasicSalary == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Basic Salary Element Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    if (model.IntPayrollElementTypeId > 0)
                    {
                        res.Message = "Created Successfully";
                    }
                    else
                    {
                        res.Message = "Updated Successfully";
                    }

                    await _saasMasterService.SavePayrollElementType(model);

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }
        [HttpGet("IsSalaryElementById")]
        public async Task<IActionResult> IsSalaryElement(long accountId, long bussinessUnitId, long typeId)
        {
            try
            {
                var salaryAdditionNdeduction = await (from add in _context.PyrEmpSalaryAdditionNdeductions
                                                      where add.IntAccountId == accountId && add.IntBusinessUnitId == bussinessUnitId && add.IsActive == true
                                                      && add.IntAdditionNdeductionTypeId == typeId
                                                      select add).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

                var salaryElementAssignRow = await (from sHeader in _context.PyrEmployeeSalaryElementAssignHeaders
                                                    where sHeader.IntAccountId == accountId && sHeader.IntBusinessUnitId == bussinessUnitId && sHeader.IsActive == true
                                                    join sRow in _context.PyrEmployeeSalaryElementAssignRows on sHeader.IntEmpSalaryElementAssignHeaderId equals sRow.IntEmpSalaryElementAssignHeaderId
                                                    where sRow.IsActive == true && sRow.IntSalaryElementId == typeId
                                                    join element in _context.PyrPayrollElementTypes on sRow.IntSalaryElementId equals element.IntPayrollElementTypeId
                                                    where element.IsActive == true
                                                    select element).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

                var data = new
                {
                    IsAllowance = salaryAdditionNdeduction == null ? false : true,
                    IsSalary = salaryElementAssignRow == null ? false : true,
                    IsAllowanceAddition = salaryAdditionNdeduction != null ? salaryAdditionNdeduction.IsAddition : false,
                    IsSalaryAddition = salaryElementAssignRow != null ? salaryElementAssignRow.IsAddition : false

                };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetAllPayrollElementType")]
        public async Task<IActionResult> GetAllPayrollElementType(long accountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllPayrollElementType(accountId));
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

        [HttpGet]
        [Route("GetPayrollElementTypeById")]
        public async Task<IActionResult> GetPayrollElementTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetPayrollElementTypeById(id));
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

        [HttpGet]
        [Route("DeletePayrollElementTypeById")]
        public async Task<IActionResult> DeletePayrollElementTypeById(long id)
        {
            try
            {
                MessageHelper response = new MessageHelper
                {
                    StatusCode = 401,
                    Message = "Something went wrong please try again!!!"
                };
                bool res = await _saasMasterService.DeletePayrollElementTypeById(id);
                if (res)
                {
                    response.Message = "Deleted Successfully";
                }
                return Ok(response);
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

        [HttpGet]
        [Route("GetAllSalaryElementByAccountIdDDL")]
        public async Task<IActionResult> GetAllSalaryElementByAccountIdDDL(long accountId)
        {
            try
            {
                List<SalaryBreakdownElementDDLViewModel> listData = await (from obj in _context.PyrPayrollElementTypes
                                                                           where obj.IsActive == true && obj.IsPrimarySalary == true && obj.IntAccountId == accountId
                                                                           select new SalaryBreakdownElementDDLViewModel
                                                                           {
                                                                               Label = obj.StrPayrollElementName,
                                                                               Value = obj.IntPayrollElementTypeId,
                                                                               IsBasic = obj.IsBasicSalary
                                                                           }).OrderBy(x => x.Label).ToListAsync();

                return Ok(listData);
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

        [HttpPost]
        [Route("SalaryBreakdownCreateNApply")]
        public async Task<IActionResult> SalaryBreakdownCreateNApply(PyrSalaryBreakdownHeaderViewModel model)
        {
            try
            {
                MessageHelper message = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Apply Successfully !!!"
                };

                PyrSalaryBreakdownHeader isExists = await _context.PyrSalaryBreakdownHeaders
                    .FirstOrDefaultAsync(x => x.IntAccountId == model.IntAccountId && x.StrSalaryBreakdownTitle.ToLower() == model.StrSalaryBreakdownTitle.ToLower()
                    && x.IntSalaryBreakdownHeaderId != model.IntSalaryBreakdownHeaderId && x.IsActive == true);

                if (isExists != null)
                {
                    message.StatusCode = 401;
                    message.Message = "Already Exists This Breakdow With This Title";
                    return BadRequest(message);
                }
                else if (model.IsDefault == true && (await _context.PyrSalaryBreakdownHeaders.Where(s => s.IntAccountId == model.IntAccountId && s.IntSalaryBreakdownHeaderId != model.IntSalaryBreakdownHeaderId && s.IsDefault == true && s.IsActive == true).CountAsync() > 0))
                {
                    message.StatusCode = 401;
                    message.Message = "Default Payroll Group Already Exists";
                    return BadRequest(message);
                }
                else
                {
                    // ========================= IS OLD/UPDATE ==========================
                    if (model.IntSalaryBreakdownHeaderId > 0)
                    {
                        PyrSalaryBreakdownHeader header = await _context.PyrSalaryBreakdownHeaders.FirstOrDefaultAsync(x => x.IntSalaryBreakdownHeaderId == model.IntSalaryBreakdownHeaderId);
                        List<PyrSalaryBreakdownRow> updateList = await _context.PyrSalaryBreakdownRows.Where(x => x.IntSalaryBreakdownHeaderId == header.IntSalaryBreakdownHeaderId).ToListAsync();

                        updateList.ForEach(u =>
                        {
                            u.IsActive = false;
                            u.DteUpdatedAt = DateTime.Now;
                            u.IntUpdatedBy = model.IntCreatedBy;
                        });


                        header.IsActive = false;
                        header.DteUpdatedAt = DateTime.Now;
                        header.IntUpdatedBy = model.IntCreatedBy;

                        _context.PyrSalaryBreakdownHeaders.Update(header);
                        _context.PyrSalaryBreakdownRows.UpdateRange(updateList);
                        await _context.SaveChangesAsync();
                    }
                    // ========================= END =============================

                    List<PyrSalaryBreakdownRow> createList = new List<PyrSalaryBreakdownRow>();

                    PyrSalaryBreakdownHeader headerObj = new PyrSalaryBreakdownHeader
                    {
                        StrSalaryBreakdownTitle = model.StrSalaryBreakdownTitle,
                        IntAccountId = model.IntAccountId,
                        IntHrPositionId = model.IntHrPositonId,
                        IntWorkplaceGroupId = model.IntWorkplaceGroupId,
                        IntSalaryPolicyId = model.IntSalaryPolicyId,
                        IsPerday = model.IsPerday,
                        StrDependOn = model.StrDependOn,
                        IsDefault = model.IsDefault,
                        IsActive = model.IsActive,
                        IntCreatedBy = model.IntCreatedBy,
                        DteCreatedAt = DateTime.Now
                    };

                    await _context.PyrSalaryBreakdownHeaders.AddAsync(headerObj);
                    await _context.SaveChangesAsync();

                    model.pyrSalaryBreakdowRowList.ForEach(c =>
                    {
                        createList.Add(new PyrSalaryBreakdownRow
                        {
                            IntSalaryBreakdownHeaderId = headerObj.IntSalaryBreakdownHeaderId,
                            IntPayrollElementTypeId = c.IntPayrollElementTypeId,
                            StrPayrollElementName = c.StrPayrollElementName,
                            StrBasedOn = c.StrBasedOn,
                            StrDependOn = c.StrDependOn,
                            NumAmount = c.NumAmount,
                            NumNumberOfPercent = c.NumNumberOfPercent,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = c.IntCreatedBy
                        });
                    });

                    await _context.PyrSalaryBreakdownRows.AddRangeAsync(createList);
                    await _context.SaveChangesAsync();

                    // ===================== CALL SP FOR UPDATE EMPLOYEE =======================

                    //====================== END ==========================

                    return Ok(message);
                }
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

        [HttpGet]
        [Route("GetAllAppliedSalaryBreakdownElement")]
        public async Task<IActionResult> GetAllAppliedSalaryBreakdownElement(long SalaryBreakdownHeaderId)
        {
            try
            {
                List<SalaryBreakdownRowViewModel> elementList = await _context.PyrSalaryBreakdownRows.Where(x => x.IntSalaryBreakdownHeaderId == SalaryBreakdownHeaderId
                                                                && x.IsActive == true).Select(x => new SalaryBreakdownRowViewModel
                                                                {
                                                                    IntSalaryBreakdownHeaderId = x.IntSalaryBreakdownHeaderId,
                                                                    IntSalaryBreakdownRowId = x.IntSalaryBreakdownRowId,
                                                                    IntPayrollElementTypeId = x.IntPayrollElementTypeId,
                                                                    StrPayrollElementName = x.StrPayrollElementName,
                                                                    StrBasedOn = x.StrBasedOn,
                                                                    StrDependOn = x.StrDependOn,
                                                                    IsBasic = _context.PyrPayrollElementTypes.Where(s => s.IntPayrollElementTypeId == x.IntPayrollElementTypeId && s.IsActive == true).Select(s => s.IsBasicSalary).FirstOrDefault(),
                                                                    NumNumberOfPercent = x.NumNumberOfPercent,
                                                                    NumAmount = x.NumAmount,
                                                                    IsActive = x.IsActive,
                                                                    DteCreatedAt = x.DteCreatedAt,
                                                                    IntCreatedBy = x.IntCreatedBy,
                                                                    DteUpdatedAt = x.DteUpdatedAt,
                                                                    IntUpdatedBy = x.IntUpdatedBy
                                                                }).ToListAsync();
                return Ok(elementList);
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

        [HttpGet]
        [Route("GetAllSalaryBreakdownLanding")]
        public async Task<IActionResult> GetAllSalaryBreakdownLanding(long accountId, long EmployeeId)
        {
            try
            {
                IEnumerable<PyrSalaryBreakdowViewModel> listData = await (from obj in _context.PyrSalaryBreakdownHeaders
                                                                          join po in _context.PyrSalaryPolicies on obj.IntSalaryPolicyId equals po.IntPolicyId into pol
                                                                          from policy in pol.DefaultIfEmpty()
                                                                          join hrPo in _context.MasterWorkplaceGroups on obj.IntWorkplaceGroupId equals hrPo.IntWorkplaceGroupId into hrPo1
                                                                          from wg in hrPo1.DefaultIfEmpty()
                                                                          where (obj.IntAccountId == accountId || obj.IntCreatedBy == EmployeeId)
                                                                          && obj.IsActive == true
                                                                          select new PyrSalaryBreakdowViewModel
                                                                          {
                                                                              IntSalaryBreakdownHeaderId = obj.IntSalaryBreakdownHeaderId,
                                                                              StrSalaryBreakdownTitle = obj.StrSalaryBreakdownTitle,
                                                                              IntAccountId = obj.IntAccountId,
                                                                              IntWorkplaceGroupId = obj.IntWorkplaceGroupId,
                                                                              WorkplaceGroup = wg == null ? "" : wg.StrWorkplaceGroup,
                                                                              StrDependOn = obj.StrDependOn,
                                                                              IsPerday = obj.IsPerday,
                                                                              IntSalaryPolicyId = obj.IntSalaryPolicyId,
                                                                              StrSalaryPolicy = policy == null ? "" : policy.StrPolicyName,
                                                                              IsDefault = obj.IsDefault,
                                                                              IsActive = obj.IsActive,
                                                                              DteCreatedAt = obj.DteCreatedAt,
                                                                              IntCreatedBy = obj.IntCreatedBy,
                                                                              DteUpdatedAt = obj.DteUpdatedAt,
                                                                              IntUpdatedBy = obj.IntUpdatedBy
                                                                          }).OrderByDescending(x => x.DteCreatedAt).AsNoTracking().AsQueryable().ToListAsync();
                return Ok(listData);
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

        #endregion ========== Paryroll Configuration ==================

        #region ============= Payroll Policy ======================

        [HttpPost]
        [Route("SaveSalaryPolicy")]
        public async Task<IActionResult> SaveSalaryPolicy(PyrSalaryPolicy model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                if (await _context.PyrSalaryPolicies.Where(x => x.StrPolicyName.ToLower() == model.StrPolicyName.ToLower()
                && x.IntAccountId == model.IntAccountId && x.IntPolicyId != model.IntPolicyId && x.IsActive == true).CountAsync() > 0)
                //if (await _context.PyrSalaryPolicies.Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This policy!!!";
                    return BadRequest(res);
                }
                else
                {
                    await _saasMasterService.SaveSalaryPolicy(model);
                    if (model.IntPolicyId > 0)
                    {
                        return Ok(new MessageHelperUpdate());
                    }
                    else
                    {
                        return Ok(new MessageHelperCreate());
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllSalaryPolicy")]
        public async Task<IActionResult> GetAllSalaryPolicy(long accountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllSalaryPolicy(accountId));
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

        [HttpGet]
        [Route("GetSalaryPolicyById")]
        public async Task<IActionResult> GetSalaryPolicyById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetSalaryPolicyById(id));
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

        [HttpGet]
        [Route("DeleteSalaryPolicyById")]
        public async Task<IActionResult> DeleteSalaryPolicyById(long id)
        {
            try
            {
                MessageHelper response = new MessageHelper
                {
                    StatusCode = 401,
                    Message = "Something went wrong please try again!!!"
                };
                bool res = await _saasMasterService.DeleteSalaryPolicyById(id);
                if (res)
                {
                    response.Message = "Deleted Successfully";
                }
                return Ok(response);
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

        //[HttpPost]
        //[Route("SalaryPolicyApply")]
        //public async Task<IActionResult> SalaryPolicyApply(PyrSalaryPolicyApplyViewModel model)
        //{
        //    try
        //    {
        //        MessageHelper message = new MessageHelper
        //        {
        //            StatusCode = 200,
        //            Message = "Policy Apply Successfully !!!"
        //        };

        //        if (await _context.PyrSalaryPolicyAssigns.Where(x => x.IntPolicyAssignId == model.IntSalaryPolicyId
        //            && x.IntAccountId == model.IntAccountId && x.IntBusinessUnitId == model.IntBusinessUnitId
        //            && x.IntPolicyAssignId != model.IntPolicyAssignId
        //            && x.IsActive == true).CountAsync() > 0)
        //        {
        //            message.StatusCode = 401;
        //            message.Message = "Already Applied This Policy !!!";
        //            return BadRequest(message);
        //        }
        //        else
        //        {
        //            if (model.IntPolicyAssignId > 0)
        //            {
        //                PyrSalaryPolicyAssign pyrSalaryPolicy = await _context.PyrSalaryPolicyAssigns.FirstOrDefaultAsync(x => x.IntPolicyAssignId == model.IntPolicyAssignId);
        //                pyrSalaryPolicy.IsActive = false;
        //                pyrSalaryPolicy.DteUpdatedAt = DateTime.Now;
        //                pyrSalaryPolicy.IntUpdatedBy = model.IntCreatedBy;

        //                _context.PyrSalaryPolicyAssigns.Update(pyrSalaryPolicy);
        //                await _context.SaveChangesAsync();
        //            }
        //            PyrSalaryPolicyAssign policy = new PyrSalaryPolicyAssign
        //            {
        //                IntAccountId = model.IntAccountId,
        //                IntBusinessUnitId = model.IntBusinessUnitId,
        //                IntPolicyId = model.IntSalaryPolicyId,
        //                StrPolicyName = model.StrSalaryPolicyName,
        //                IsActive = true,
        //                DteCreatedAt = DateTime.Now,
        //                IntCreatedBy = model.IntCreatedBy
        //            };

        //            await _context.PyrSalaryPolicyAssigns.AddAsync(policy);
        //            await _context.SaveChangesAsync();
        //        }

        //        return Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageHelper res = new MessageHelper
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //        return BadRequest(res);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllAppliedSalaryPolicyLanding")]
        //public async Task<IActionResult> GetAllAppliedSalaryPolicyLanding(long accountId, long businessUnitId, long workplaceGroupId, long workplaceId, long departmentId, long designationId, long employmentTypeId)
        //{
        //    try
        //    {
        //        List<PyrSalaryBreakdowViewModel> listData = await (from obj in _context.PyrSalaryPolicyAssigns
        //                                                           where obj.IsActive == true && obj.IntAccountId == accountId
        //                                                           join bus1 in _context.MasterBusinessUnits on obj.IntBusinessUnitId equals bus1.IntBusinessUnitId into bus2
        //                                                           from bus in bus2.DefaultIfEmpty()

        //                                                           where (obj.IntBusinessUnitId == businessUnitId || businessUnitId == 0)
        //                                                           select new PyrSalaryBreakdowViewModel
        //                                                           {
        //                                                               IntAccountId = obj.IntAccountId,
        //                                                               IsActive = obj.IsActive,
        //                                                               DteCreatedAt = obj.DteCreatedAt,
        //                                                               IntCreatedBy = obj.IntCreatedBy,
        //                                                               DteUpdatedAt = obj.DteUpdatedAt,
        //                                                               IntUpdatedBy = obj.IntUpdatedBy
        //                                                           }).OrderByDescending(x => x.DteCreatedAt).ToListAsync();
        //        return Ok(listData);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageHelper res = new MessageHelper
        //        {
        //            StatusCode = 500,
        //            Message = ex.Message
        //        };
        //        return BadRequest(res);
        //    }
        //}

        #endregion ============= Payroll Policy ======================

        #region ============ Policy Assign ===================

        [HttpPost]
        [Route("PolicyAssign")]
        public async Task<IActionResult> PolicyAssign(PolicyAssignViewModel policyAssignViewModel)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPolicyAssign";

                    string jsonString = System.Text.Json.JsonSerializer.Serialize(policyAssignViewModel.EmpPolicyIdViewModelList);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strEntryType", policyAssignViewModel.strEntryType);
                        sqlCmd.Parameters.AddWithValue("@intPolicyAssignId", policyAssignViewModel.intPolicyAssignId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", policyAssignViewModel.intAccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", policyAssignViewModel.intBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intActionBy", policyAssignViewModel.intActionBy);
                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("BreakdownPolicyAssign")]
        public async Task<IActionResult> BreakdownPolicyAssign(SalaryBreakdownPolicyViewModel salaryBreakdownPolicy)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprSalaryBreakdownPolicyAssign";

                    string jsonString = System.Text.Json.JsonSerializer.Serialize(salaryBreakdownPolicy.breakdownPolicieList);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strEntryType", salaryBreakdownPolicy.strEntryType);
                        sqlCmd.Parameters.AddWithValue("@intSalaryBreakdownAssignId", salaryBreakdownPolicy.intSalaryBreakdownAssignId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", salaryBreakdownPolicy.intAccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", salaryBreakdownPolicy.intBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intActionBy", salaryBreakdownPolicy.intActionBy);
                        sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ============ Policy Assign ===================

        #region =========== Employee Salary Assign ===============

        [HttpGet]
        [Route("BreakdownNPolicyForSalaryAssign")]
        public async Task<IActionResult> BreakdownNPolicyForSalaryAssign(string StrReportType, long? IntEmployeeId, long? IntAccountId, long? IntSalaryBreakdownHeaderId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPyrEmployeeSalaryElementAssignAllSelectQuery";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strReportType", StrReportType);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", IntEmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@intSalaryBreakdownHeaderId", IntSalaryBreakdownHeaderId);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("EmployeeSalaryAssign")]
        public async Task<IActionResult> EmployeeSalaryAssign(EmployeeSalaryAssignViewModel employeeSalaryAssignView)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string jsonString = String.Empty;
                    string jsonEmployeeIdList = String.Empty;

                    string sql = "saas.sprPyrEmployeeSalaryElementAssign";
                    if (employeeSalaryAssignView.BreakdownElements != null)
                    {
                        jsonString = System.Text.Json.JsonSerializer.Serialize(employeeSalaryAssignView.BreakdownElements);
                    }
                    if (employeeSalaryAssignView.IntEmployeeIdList != null)
                    {
                        jsonEmployeeIdList = System.Text.Json.JsonSerializer.Serialize(employeeSalaryAssignView.IntEmployeeIdList);
                    }

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@jsonEmployeeIdList", jsonEmployeeIdList);
                        sqlCmd.Parameters.AddWithValue("@intSalaryBreakdownHeaderId", employeeSalaryAssignView.IntSalaryBreakdownHeaderId);
                        sqlCmd.Parameters.AddWithValue("@strSalaryBreakdownHeaderTitle", employeeSalaryAssignView.StrSalaryBreakdownHeaderTitle);
                        sqlCmd.Parameters.AddWithValue("@numBasicORGross", employeeSalaryAssignView.NumBasicORGross);
                        sqlCmd.Parameters.AddWithValue("@numGrossAmount", employeeSalaryAssignView.NumGrossAmount);
                        sqlCmd.Parameters.AddWithValue("@intEffectiveYear", employeeSalaryAssignView.EffectiveYear);
                        sqlCmd.Parameters.AddWithValue("@intEffectiveMonth", employeeSalaryAssignView.EffectiveMonth);
                        sqlCmd.Parameters.AddWithValue("@intCreateBy", employeeSalaryAssignView.IntCreateBy);
                        sqlCmd.Parameters.AddWithValue("@isPerdaySalary", employeeSalaryAssignView.IsPerdaySalary);
                        sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", employeeSalaryAssignView.IntPayrollGroupId);
                        sqlCmd.Parameters.AddWithValue("@strPayrollGroup", employeeSalaryAssignView.StrPayrollGroupName);
                        sqlCmd.Parameters.AddWithValue("@numNetGrossSalary", employeeSalaryAssignView.NumNetGrossSalary);

                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);
                        }

                        connection.Open();
                        using (SqlDataAdapter sqlData = new SqlDataAdapter(sqlCmd))
                        {
                            sqlData.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                MessageHelper msg = new MessageHelper();
                msg.StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);
                msg.Message = Convert.ToString(dt.Rows[0]["returnMessage"]);

                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion =========== Employee Salary Assign ===============

        #region ======= SalaryManagement =======

        [HttpPost]
        [Route("EmployeeSalaryManagement")]
        public async Task<IActionResult> EmployeeSalaryManagement(EmployeeSalaryManagementDTO obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)obj.BusinessUnitId, workplaceGroupId = (long)obj.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.isAuthorize)
                {
                    return Ok(JsonConvert.SerializeObject(await _employeeService.EmployeeSalaryManagement(obj)));
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("SalaryIsHold")]
        public async Task<IActionResult> SalaryIsHold(bool? isHold, long? EmployeeId)
        {
            try
            {
                EmpEmployeeBasicInfoDetail empBasic = await _context.EmpEmployeeBasicInfoDetails.Where(x => x.IntEmployeeId == EmployeeId).AsNoTracking().FirstOrDefaultAsync();
                if (empBasic != null && isHold != null)
                {
                    empBasic.IntEmployeeStatusId = isHold == true ? 4 : 1;
                    _context.EmpEmployeeBasicInfoDetails.Update(empBasic);
                    await _context.SaveChangesAsync();
                }

                return Ok("Update Successfull");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //[Route("EmployeeSalaryAssign")]
        //public async Task<IActionResult> EmployeeSalaryAssign(SalaryAssignDTO model)
        //{
        //    try
        //    {
        //        if (model.EmployeeId > 0 && model.EffectiveYear >= DateTime.Now.Year && model.EffectiveMonth >= DateTime.Now.Month)
        //        {
        //            //
        //            PyrEmployeeSalaryDefault salaryDefault = await _context.PyrEmployeeSalaryDefaults.Where(x => x.IntEmployeeId == model.EmployeeId).AsNoTracking().FirstOrDefaultAsync();

        //            #region   ===================== LOG =================================
        //            if (salaryDefault != null)
        //            {
        //                LogPyrEmployeeSalaryDefault logObj = new LogPyrEmployeeSalaryDefault
        //                {
        //                    IntEmployeeId = salaryDefault.IntEmployeeId,
        //                    IntEmploymentTypeId = salaryDefault.IntEmploymentTypeId,
        //                    //IntPayrollGroupId = salaryDefault.IntPayrollGroupId,
        //                    //IntPayscaleGradeId = salaryDefault.IntPayscaleGradeId,
        //                    //IntPayscaleGradeAdditionId = salaryDefault.IntPayscaleGradeAdditionId,
        //                    NumBasic = salaryDefault.NumBasic,
        //                    NumHouseAllowance = salaryDefault.NumHouseAllowance,
        //                    NumMedicalAllowance = salaryDefault.NumMedicalAllowance,
        //                    NumConveyanceAllowance = salaryDefault.NumConveyanceAllowance,
        //                    NumWashingAllowance = salaryDefault.NumWashingAllowance,
        //                    NumCbadeduction = salaryDefault.NumCbadeduction,
        //                    NumSpecialAllowance = salaryDefault.NumSpecialAllowance,
        //                    NumGrossSalary = salaryDefault.NumGrossSalary,
        //                    NumTotalSalary = salaryDefault.NumTotalSalary,
        //                    IntEffectiveMonth = salaryDefault.IntEffectiveMonth,
        //                    IntEffectiveYear = salaryDefault.IntEffectiveYear,
        //                    ReqNumBasic = salaryDefault.ReqNumBasic,
        //                    ReqNumHouseAllowance = salaryDefault.ReqNumHouseAllowance,
        //                    ReqNumMedicalAllowance = salaryDefault.ReqNumMedicalAllowance,
        //                    ReqNumConveyanceAllowance = salaryDefault.ReqNumConveyanceAllowance,
        //                    ReqNumWashingAllowance = salaryDefault.ReqNumWashingAllowance,
        //                    ReqNumCbadeduction = salaryDefault.ReqNumCbadeduction,
        //                    ReqNumSpecialAllowance = salaryDefault.ReqNumSpecialAllowance,
        //                    ReqNumGrossSalary = salaryDefault.ReqNumGrossSalary,
        //                    ReqNumTotalSalary = salaryDefault.ReqNumTotalSalary,
        //                    IsActive = salaryDefault.IsActive,
        //                    IntCreatedBy = salaryDefault.IntCreatedBy,
        //                    DteCreatedAt = salaryDefault.DteCreatedAt,
        //                    IntUpdatedBy = salaryDefault.IntUpdatedBy,
        //                    DteUpdatedAt = salaryDefault.DteUpdatedAt,
        //                    IntLogCreatedByUserId = model.IntLogCreatedByUserId,
        //                    DteLogInsertCreatedAt = DateTime.Now
        //                };

        //                await _context.LogPyrEmployeeSalaryDefaults.AddAsync(logObj);
        //                await _context.SaveChangesAsync();
        //            }
        //            #endregion ==================== END LOG =================================

        //            //long othersSalaryCalculate = (long)((model.OthersSalaryList.Where(x=>x.ActionTypeId == 1 && x.PayrollElementTypeCode == "+1").Sum(x=>x.reqAmount))
        //            //    //+ (model.OthersSalaryList.Where(x => x.ActionTypeId == 2 && x.PayrollElementTypeCode == "+1").Sum(x => x.reqAmount))
        //            //    - (model.OthersSalaryList.Where(x => x.ActionTypeId == 3 && x.PayrollElementTypeCode == "+1").Sum(x => x.reqAmount))
        //            //    - (model.OthersSalaryList.Where(x => x.ActionTypeId == 1 && x.PayrollElementTypeCode == "-1").Sum(x => x.reqAmount))
        //            //    - (model.OthersSalaryList.Where(x => x.ActionTypeId == 2 && x.PayrollElementTypeCode == "-1").Sum(x => x.reqAmount))
        //            //    + (model.OthersSalaryList.Where(x => x.ActionTypeId == 3 && x.PayrollElementTypeCode == "-1").Sum(x => x.reqAmount))
        //            //    - (model.reqCBADeduction));

        //            PyrEmployeeSalaryDefault newSalaryDefaultObj = new PyrEmployeeSalaryDefault
        //            {
        //                IntEmployeeSalaryDefaultId = salaryDefault != null ? salaryDefault.IntEmployeeSalaryDefaultId : 0,
        //                IntEmployeeId = (int)model.EmployeeId,
        //                NumBasic = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqBasic : salaryDefault != null ? salaryDefault.NumBasic : 0,
        //                NumHouseAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqHouseAllowance : salaryDefault != null ? salaryDefault.NumHouseAllowance : 0,
        //                NumMedicalAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqMedicalAllowance : salaryDefault != null ? salaryDefault.NumMedicalAllowance : 0,
        //                NumConveyanceAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqConveyanceAllowance : salaryDefault != null ? salaryDefault.NumConveyanceAllowance : 0,
        //                NumWashingAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqWashingAllowance : salaryDefault != null ? salaryDefault.NumWashingAllowance : 0,
        //                NumCbadeduction = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqCBADeduction : model.CBADeduction,
        //                NumSpecialAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqSpecialAllowance : salaryDefault != null ? salaryDefault.NumSpecialAllowance : 0,
        //                NumGrossSalary = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqGrossSalary : salaryDefault != null ? salaryDefault.NumGrossSalary : 0,
        //                NumTotalSalary = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? model.reqTotalSalary : salaryDefault != null ? salaryDefault.NumTotalSalary : 0,
        //                //IntEffectiveYear = (int?)((model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.EffectiveYear),
        //                //IntEffectiveMonth = (int?)((model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.EffectiveMonth),
        //                IntEffectiveYear = (int)model.EffectiveYear,
        //                IntEffectiveMonth = (int?)model.EffectiveMonth,
        //                ReqNumBasic = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqBasic,
        //                ReqNumHouseAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqHouseAllowance,
        //                ReqNumMedicalAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqMedicalAllowance,
        //                ReqNumConveyanceAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqConveyanceAllowance,
        //                ReqNumWashingAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqWashingAllowance,
        //                ReqNumCbadeduction = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqCBADeduction,
        //                ReqNumSpecialAllowance = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqSpecialAllowance,
        //                ReqNumGrossSalary = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqGrossSalary,
        //                ReqNumTotalSalary = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : model.reqTotalSalary,
        //                IsActive = model.isActive,
        //                IntCreatedBy = salaryDefault == null ? model.IntCreatedBy : salaryDefault.IntCreatedBy,
        //                DteCreatedAt = salaryDefault == null ? DateTime.Now : salaryDefault.DteCreatedAt,
        //                IntUpdatedBy = salaryDefault != null ? model.IntUpdatedBy : null,
        //                DteUpdatedAt = salaryDefault != null ? DateTime.Now : null
        //            };

        //            if (salaryDefault == null)
        //            {
        //                await _context.AddAsync(newSalaryDefaultObj);
        //                await _context.SaveChangesAsync();
        //            }
        //            else if (salaryDefault != null)
        //            {
        //                _context.Update(newSalaryDefaultObj);
        //                await _context.SaveChangesAsync();
        //            }

        //            // Others Salary
        //            List<PyrEmployeeSalaryOther> createList = new List<PyrEmployeeSalaryOther>();
        //            List<PyrEmployeeSalaryOther> editList = new List<PyrEmployeeSalaryOther>();
        //            List<PyrEmployeeSalaryOther> deleteList = new List<PyrEmployeeSalaryOther>();
        //            List<LogPyrEmployeeSalaryOther> logList = new List<LogPyrEmployeeSalaryOther>();

        //            foreach (var salary in model.OthersSalaryList)
        //            {
        //                if (salary.ActionTypeId == 1) // ============= create ===============
        //                {
        //                    PyrEmployeeSalaryOther obj = new PyrEmployeeSalaryOther
        //                    {
        //                        IntEmployeeId = (int)salary.EmployeeId,
        //                        IntPayrollElementId = (int)salary.PayrollElementId,
        //                        StrPayrollElementTypeCode = salary.PayrollElementTypeCode,
        //                        NumAmount = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? salary.reqAmount : 0,
        //                        ReqNumAmount = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : salary.reqAmount,
        //                    };
        //                    createList.Add(obj);
        //                }
        //                // ============== update disable for front end complexity in future need then just comment out above code

        //                //else if (salary.ActionTypeId == 2)  // ============= edit ===============
        //                //{
        //                //    #region ============== log =================
        //                //    TblEmployeeSalaryOther otherSalary = await _context.TblEmployeeSalaryOthers.Where(x=>x.IntEmployeeSalaryOtherId == salary.EmployeeSalaryOtherId).FirstOrDefaultAsync();
        //                //       if(otherSalary != null)
        //                //       {
        //                //            LogTblEmployeeSalaryOther otherSalaryLogObj = new LogTblEmployeeSalaryOther
        //                //            {
        //                //                IntEmployeeId = otherSalary.IntEmployeeId,
        //                //                IntPayrollElementId = otherSalary.IntPayrollElementId,
        //                //                StrPayrollElementTypeCode = otherSalary.StrPayrollElementTypeCode,
        //                //                NumAmount = otherSalary.NumAmount,
        //                //                ReqNumAmount = otherSalary.ReqNumAmount,
        //                //                IntEffectiveMonth = salaryDefault.IntEffectiveMonth != null ? salaryDefault.IntEffectiveMonth : 0,
        //                //                IntEffectiveYear = salaryDefault.IntEffectiveYear != null ? salaryDefault.IntEffectiveYear : 0,
        //                //                StrLogInsertByUserId = model.InsertByUserId,
        //                //                DtelogInsertByDateTime = DateTime.Now
        //                //            };
        //                //            logList.Add(otherSalaryLogObj);
        //                //       }
        //                //    #endregion ============== end =================
        //                //    TblEmployeeSalaryOther obj = new TblEmployeeSalaryOther
        //                //        {
        //                //            IntEmployeeSalaryOtherId = (int)salary.EmployeeSalaryOtherId,
        //                //            IntEmployeeId = (int)salary.EmployeeId,
        //                //            IntPayrollElementId = (int)salary.PayrollElementId,
        //                //            StrPayrollElementTypeCode = salary.PayrollElementTypeCode,
        //                //            NumAmount = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? salary.reqAmount : 0,
        //                //            ReqNumAmount = (model.EffectiveYear == DateTime.Now.Year && model.EffectiveMonth == DateTime.Now.Month) ? 0 : salary.reqAmount,
        //                //        };
        //                //        editList.Add(obj);
        //                //}
        //                else if (salary.ActionTypeId == 3)  // ============= delete ===============
        //                {
        //                    var tempId = salary.EmployeeSalaryOtherId == null ? 0 : salary.EmployeeSalaryOtherId;
        //                    PyrEmployeeSalaryOther obj = await _context.PyrEmployeeSalaryOthers.Where(x => x.IntEmployeeSalaryOtherId == tempId).FirstOrDefaultAsync();
        //                    if (obj != null)
        //                    {
        //                        #region ============== log =================
        //                        LogPyrEmployeeSalaryOther otherSalaryLogObj = new LogPyrEmployeeSalaryOther
        //                        {
        //                            IntEmployeeId = obj.IntEmployeeId,
        //                            IntPayrollElementId = obj.IntPayrollElementId,
        //                            StrPayrollElementTypeCode = obj.StrPayrollElementTypeCode,
        //                            NumAmount = obj.NumAmount,
        //                            ReqNumAmount = obj.ReqNumAmount,
        //                            IntEffectiveMonth = salaryDefault.IntEffectiveMonth != null ? salaryDefault.IntEffectiveMonth : 0,
        //                            IntEffectiveYear = salaryDefault.IntEffectiveYear != null ? salaryDefault.IntEffectiveYear : 0,
        //                            IntLogCreatedByUserId = model.IntLogCreatedByUserId,
        //                            DtelogCreatedByDateTime = DateTime.Now
        //                        };
        //                        #endregion ============== log =================
        //                        logList.Add(otherSalaryLogObj);
        //                        deleteList.Add(obj);
        //                    }
        //                }
        //            }

        //            await _context.LogPyrEmployeeSalaryOthers.AddRangeAsync(logList);
        //            await _context.PyrEmployeeSalaryOthers.AddRangeAsync(createList);
        //            _context.PyrEmployeeSalaryOthers.UpdateRange(editList);
        //            _context.PyrEmployeeSalaryOthers.RemoveRange(deleteList);

        //            _context.SaveChangesAsync();

        //            return Ok("Salary Assigned Successfull");
        //        }
        //        else
        //        {
        //            return NotFound(model.EmployeeId <= 0 ? "Invalid Employee" : "Previous month OR previous year is not possible");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        [HttpPost]
        [Route("SalaryGenerateRequest")]
        public async Task<IActionResult> SalaryGenerateRequest(SalaryGenerateRequestDTO obj)
        {
            try
            {
                MessageHelper msg = await _employeeService.SalaryGenerateRequest(obj);

                if (msg.StatusCode == 500)
                {
                    return BadRequest(msg);
                }
                else
                {
                    return Ok(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetSalaryGenerateRequestReport")]
        public async Task<IActionResult> GetSalaryGenerateRequestReport(long BusinessUnitId)
        {
            try
            {
                var dt = await _employeeService.GetSalaryGenerateRequestReport(BusinessUnitId);

                var res = JsonConvert.SerializeObject(dt);

                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ======= SalaryManagement =======

        #region ===== Dynamic Salary Report =====

        [HttpGet]
        [Route("DynamicSalaryReport")]
        public async Task<IActionResult> DynamicSalaryReport()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Clear();

                dt.TableName = "Employee Info";

                dt.Columns.Add("Name");
                dt.Columns.Add("Address");
                dt.Columns.Add("Designation");
                dt.Columns.Add("Department");
                dt.Columns.Add("Salary");
                dt.Columns.Add("Transport Bill");
                dt.Columns.Add("House Rent");
                dt.Columns.Add("Medical awareness");
                dt.Columns.Add("Late Present");
                dt.Columns.Add("Leave without pay");
                dt.Columns.Add("Net Salary");

                for (int i = 1; i <= 1560; i++)
                {
                    DataRow Emp1 = dt.NewRow();

                    Emp1["Name"] = "Devid Loren_" + i;
                    Emp1["Address"] = "House-4, Road-4, Block-B, New York";
                    Emp1["Designation"] = "Software Engineer";
                    Emp1["Department"] = "Development";
                    Emp1["Salary"] = 30000 + (6560 - i);
                    Emp1["Transport Bill"] = 5000 + (6560 - i);
                    Emp1["House Rent"] = 20000 + (6560 - i);
                    Emp1["Medical awareness"] = 10000 + (6560 - i);
                    Emp1["Late Present"] = 1000 + (6560 - i);
                    Emp1["Leave without pay"] = 2000 + (6560 - i);
                    Emp1["Net Salary"] = 62000 + (6560 - i);

                    dt.Rows.Add(Emp1);
                }

                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ===== Dynamic Salary Report =====

        #region ======= Salary Generate =========

        [HttpGet]
        [Route("SalarySelectQueryAll")]
        public async Task<IActionResult> SalarySelectQueryAll(string partName, long? intBusinessUnitId,
            long intMonthId, long intYearId, long? intWorkPlaceId, long? intWorkplaceGroupId, long? intPayrollGroupId,
            long? intSalaryGenerateRequestId, int? intBankOrWalletType, long? IntEmployeeId, long? WingId, long? SoleDepoId, long? RegionId, long? AreaId, string? TerritoryId,
            DateTime? GenerateFromDate, DateTime? GenerateToDate, long? IntPageNo, long? IntPageSize, string? searchText, string? AreaListFromDDL)
        {
            DataTable dt = new DataTable();
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)intBusinessUnitId, workplaceGroupId = (long)intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                    {
                        string sql = "saas.sprPyrSalarySelectQueryAll";

                        using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@strPartName", partName);
                            sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                            sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                            sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                            sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                            sqlCmd.Parameters.AddWithValue("@intWorkplaceId", intWorkPlaceId);
                            sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                            sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", intPayrollGroupId);
                            sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", intSalaryGenerateRequestId);
                            sqlCmd.Parameters.AddWithValue("@intBankOrWalletType", intBankOrWalletType);

                            sqlCmd.Parameters.AddWithValue("@intWingId", WingId);
                            sqlCmd.Parameters.AddWithValue("@intSoleDepoId", SoleDepoId);
                            sqlCmd.Parameters.AddWithValue("@intRegionId", RegionId);
                            sqlCmd.Parameters.AddWithValue("@intAreaId", AreaId);
                            sqlCmd.Parameters.AddWithValue("@intTerritoryIds", TerritoryId);

                            sqlCmd.Parameters.AddWithValue("@dteFromDate", GenerateFromDate);
                            sqlCmd.Parameters.AddWithValue("@dteToDate", GenerateToDate);
                            sqlCmd.Parameters.AddWithValue("@intEmployeeId", tokenData.employeeId);
                            sqlCmd.Parameters.AddWithValue("@intPageNo", IntPageNo);
                            sqlCmd.Parameters.AddWithValue("@intPageSize", IntPageSize);
                            sqlCmd.Parameters.AddWithValue("@strSearchText", searchText);
                            sqlCmd.Parameters.AddWithValue("@AreaListFromDDL", AreaListFromDDL);
                            sqlCmd.Parameters.AddWithValue("@isOfficeAdmin", tokenData.isOfficeAdmin);

                            connection.Open();

                            using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                            {
                                sqlAdapter.Fill(dt);
                            }
                            connection.Close();
                        }
                    }
                    return Ok(JsonConvert.SerializeObject(dt));
                }

            }
            catch (Exception EX)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("AllowanceNDeductionUpdateForSalaryGenerate")]
        public async Task<IActionResult> AllowanceNDeductionUpdateForSalaryGenerate(List<AllowanceNDeductionForSalaryGenerate> allowanceNDeduction)
        {
            try
            {
                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprPyrAllowanceNDeductionUpdateForSalaryGenerate";
                string jsonString = System.Text.Json.JsonSerializer.Serialize(allowanceNDeduction);

                SqlCommand sqlCmd = new SqlCommand(sql, connection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                connection.Open();
                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                {
                    sqlAdapter.Fill(dt);
                }
                connection.Close();

                return Ok(new MessageHelperUpdate());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EmployeeTakeHomePayNotAssignForTax")]
        public async Task<IActionResult> EmployeeTakeHomePayNotAssignForTax(EmployeeNotAssignForTaxViewModel employeeNotAssignForTax)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPyrSalarySelectQueryAll";
                   // string jsonString = System.Text.Json.JsonSerializer.Serialize(employeeNotAssignForTax.listOfEmployeeId);

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@strPartName", employeeNotAssignForTax.PartName);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", employeeNotAssignForTax.IntAccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", employeeNotAssignForTax.IntBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@strEmpIdList", employeeNotAssignForTax.listOfEmployeeId);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("ArearSalarySelectQueryAll")]
        public async Task<IActionResult> ArearSalarySelectQueryAll(string partName, long intAccountId, long? intBusinessUnitId,
        long? intWorkplaceGroupId, long intWorkPlaceId, long? intPayrollGroupId, long intMonthId, long intYearId,
        long? intArearSalaryGenerateRequestId, string? strBankAccountNumber, string? strAdviceType, DateTime? dteEffectiveFrom,
        DateTime? dteEffectiveTo, int? intBankOrWalletType, long? IntEmployeeId, long? IntPageNo, long? IntPageSize, string? searchText)
        {
            DataTable dt = new DataTable();
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)intBusinessUnitId, workplaceGroupId = (long)intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                    {
                        string sql = "saas.sprPyrArearSalarySelectQueryAll";

                        using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@strPartName", partName);
                            sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                            sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                            sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                            sqlCmd.Parameters.AddWithValue("@intWorkplaceId", intWorkPlaceId);
                            sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", intPayrollGroupId);
                            sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                            sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                            sqlCmd.Parameters.AddWithValue("@intArearSalaryGenerateRequestId", intArearSalaryGenerateRequestId);
                            sqlCmd.Parameters.AddWithValue("@strBankAccountNumber", strBankAccountNumber);
                            sqlCmd.Parameters.AddWithValue("@strAdviceType", strAdviceType);
                            sqlCmd.Parameters.AddWithValue("@dteEffectiveFrom", dteEffectiveFrom);
                            sqlCmd.Parameters.AddWithValue("@dteEffectiveTo", dteEffectiveTo);
                            sqlCmd.Parameters.AddWithValue("@intBankOrWalletType", intBankOrWalletType);
                            sqlCmd.Parameters.AddWithValue("@intEmployeeId", tokenData.employeeId);
                            sqlCmd.Parameters.AddWithValue("@intPageNo", IntPageNo);
                            sqlCmd.Parameters.AddWithValue("@intPageSize", IntPageSize);
                            sqlCmd.Parameters.AddWithValue("@strSearchText", searchText);

                            connection.Open();

                            using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                            {
                                sqlAdapter.Fill(dt);
                            }
                            connection.Close();
                        }
                    }
                    return Ok(JsonConvert.SerializeObject(dt));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SalaryCRUD")]
        public async Task<IActionResult> SalaryCRUD(SalaryGenerateViewModel model)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.intBusinessUnitId, workplaceGroupId = (long)model.intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData == null || !tokenData.isAuthorize)
                {
                    return Ok(new MessageHelperAccessDenied());
                }

                var pipeline = new PipelineStageInfoVM();
                if (model.strPartName.ToLower() == "GeneratedSalarySendForApproval".ToLower())
                {
                    var requestHeader = await _context.PyrPayrollSalaryGenerateRequests.Where(x => x.IntSalaryGenerateRequestId == model.intSalaryGenerateRequestId && x.IsActive == true).FirstOrDefaultAsync();
                    if (requestHeader == null)
                    {
                        throw new Exception("Header Not Found");
                    }
                    pipeline = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(tokenData.accountId, requestHeader.IntBusinessUnitId, (long)requestHeader.IntWorkplaceGroupId, (long)requestHeader.IntSoleDepoId, (long)requestHeader.IntAreaId, "salaryGenerateRequest");

                }

                DataTable dt = new DataTable();
                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string sql = "saas.sprPyrSalaryCRUD";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                //string jsonString = System.Text.Json.JsonSerializer.Serialize(model.generateRequestRows);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@strPartName", model.strPartName);
                sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", model.intSalaryGenerateRequestId);
                sqlCmd.Parameters.AddWithValue("@strSalaryCode", model.strSalaryCode);
                sqlCmd.Parameters.AddWithValue("@intAccountId", tokenData.accountId);
                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", model.intBusinessUnitId);
                sqlCmd.Parameters.AddWithValue("@strBusinessUnit", model.strBusinessUnit);

                sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", model.intWorkplaceGroupId);
                sqlCmd.Parameters.AddWithValue("@strWorkplaceGroup", model.strWorkplaceGroup);

                sqlCmd.Parameters.AddWithValue("@intWingId", model.intWingId);
                sqlCmd.Parameters.AddWithValue("@intSoleDepoId", model.intSoleDepoId);
                sqlCmd.Parameters.AddWithValue("@intRegionId", model.intRegionId);
                sqlCmd.Parameters.AddWithValue("@intAreaId", model.intAreaId);
                sqlCmd.Parameters.AddWithValue("@territoryIdList", model.territoryIdList);
                sqlCmd.Parameters.AddWithValue("@territoryNameList", model.territoryNameList);

                sqlCmd.Parameters.AddWithValue("@intMonthId", model.intMonthId);
                sqlCmd.Parameters.AddWithValue("@intYearId", model.intYearId);
                sqlCmd.Parameters.AddWithValue("@strDescription", model.strDescription);
                sqlCmd.Parameters.AddWithValue("@intCreatedBy", model.intCreatedBy);
                sqlCmd.Parameters.AddWithValue("@strSalaryType", model.strSalryType);
                sqlCmd.Parameters.AddWithValue("@dteFromDate", model.dteFromDate);
                sqlCmd.Parameters.AddWithValue("@dteToDate", model.dteToDate);

                sqlCmd.Parameters.AddWithValue("@CurrentStageId", pipeline.CurrentStageId);
                sqlCmd.Parameters.AddWithValue("@NextStageId", pipeline.NextStageId);
                sqlCmd.Parameters.AddWithValue("@PipelineHeaderId", pipeline.HeaderId);
                sqlCmd.Parameters.AddWithValue("@strEmpIdList", model.strEmpIdList);
                //sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                connection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                sqlAdapter.Fill(dt);
                connection.Close();

                int StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);

                if (StatusCode == 200)
                {
                    return Ok(JsonConvert.SerializeObject(dt));
                }
                else
                {
                    return BadRequest(JsonConvert.SerializeObject(dt));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError() { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ArearSalaryCRUD")]
        public async Task<IActionResult> ArearSalaryCRUD(ArearSalaryGenerateVM model)
        {
            try
            {
                DataTable dt = new DataTable();

                SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM);

                string jsonString = System.Text.Json.JsonSerializer.Serialize(model.EmployeeIdList);

                string sql = "saas.sprPyrArearSalaryCRUD";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@strPartName", model.strPartName);
                sqlCmd.Parameters.AddWithValue("@intArearSalaryGenerateRequestId", model.intArearSalaryGenerateRequestId);
                sqlCmd.Parameters.AddWithValue("@intAccountId", model.intAccountId);
                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", model.intBusinessUnitId);
                sqlCmd.Parameters.AddWithValue("@strBusinessUnit", model.strBusinessUnit);
                sqlCmd.Parameters.AddWithValue("@dteEffectiveFrom", model.dteEffectiveFrom);
                sqlCmd.Parameters.AddWithValue("@dteEffectiveTo", model.dteEffectiveTo);
                sqlCmd.Parameters.AddWithValue("@strDescription", model.strDescription);
                sqlCmd.Parameters.AddWithValue("@intCreatedBy", model.intCreatedBy);
                sqlCmd.Parameters.AddWithValue("@intSalaryPolicyId", model.intSalaryPolicyId);
                sqlCmd.Parameters.AddWithValue("@numPercentOfGross", model.numPercentOfGross);
                sqlCmd.Parameters.AddWithValue("@jsonString", jsonString);

                connection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
                sqlAdapter.Fill(dt);
                connection.Close();

                int StatusCode = Convert.ToInt32(dt.Rows[0]["returnStatus"]);

                if (StatusCode == 200)
                {
                    return Ok(JsonConvert.SerializeObject(dt));
                }
                else
                {
                    return BadRequest(JsonConvert.SerializeObject(dt));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("SalaryDetailsReport")]
        public async Task<IActionResult> SalaryDetailsReport(long intAccountId, long? intBusinessUnitId, long? intWorkplaceGroupId, long intMonthId, long intYearId, string? strSalaryCode)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprPyrSalaryDetailsReport";

                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@intAccountId", intAccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
                        sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
                        sqlCmd.Parameters.AddWithValue("@strSalaryCode", strSalaryCode);

                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }
                }
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("BankAdvaiceReport")]
        public async Task<IActionResult> BankAdvaiceReport(BankAdviceHeader obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)obj.intBusinessUnitId, workplaceGroupId = (long)obj.intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var result = await _payrollService.BankAdvices(obj, tokenData.accountId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        //[HttpGet]
        //[Route("BankAdvaiceReport")]
        //public async Task<IActionResult> BankAdvaiceReport(string partName, long intAccountId, long? intBusinessUnitId, long intMonthId, long intYearId,
        //    long? intWorkPlaceId, long? intWorkplaceGroupId, long? intPayrollGroupId, long? intSalaryGenerateRequestId, string? BankAccountNo, int? intBankOrWalletType, long? intEmployeeId, string? strAdviceType)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        using (SqlConnection connection = new SqlConnection(Connection.iPEOPLE_HCM))
        //        {
        //            string sql = "saas.sprPyrSalarySelectQueryAll";

        //            using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
        //            {
        //                sqlCmd.CommandType = CommandType.StoredProcedure;
        //                sqlCmd.Parameters.AddWithValue("@strPartName", partName);
        //                sqlCmd.Parameters.AddWithValue("@intAccountId", intAccountId);
        //                sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", intBusinessUnitId);
        //                sqlCmd.Parameters.AddWithValue("@intMonthId", intMonthId);
        //                sqlCmd.Parameters.AddWithValue("@intYearId", intYearId);
        //                sqlCmd.Parameters.AddWithValue("@intWorkplaceId", intWorkPlaceId);
        //                sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", intWorkplaceGroupId);
        //                sqlCmd.Parameters.AddWithValue("@intPayrollGroupId", intPayrollGroupId);
        //                sqlCmd.Parameters.AddWithValue("@intSalaryGenerateRequestId", intSalaryGenerateRequestId);
        //                sqlCmd.Parameters.AddWithValue("@BankAccountNumber", BankAccountNo);
        //                sqlCmd.Parameters.AddWithValue("@intBankOrWalletType", intBankOrWalletType);
        //                sqlCmd.Parameters.AddWithValue("@intEmployeeId", intEmployeeId);
        //                sqlCmd.Parameters.AddWithValue("@strAdviceType", strAdviceType);

        //                connection.Open();

        //                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
        //                {
        //                    sqlAdapter.Fill(dt);
        //                }
        //                connection.Close();
        //            }
        //        }
        //        return Ok(JsonConvert.SerializeObject(dt));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion ======= Salary Generate =========

        #region ======== Payscale Grade ==========

        [HttpPost]
        [Route("CRUDPayScaleGrade")]
        public async Task<IActionResult> CRUDPayScaleGrade(PayScaleGradeViewModel model)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (model.IntPayscaleGradeId > 0)
                {
                    PyrPayscaleGrade payscaleGrade = await _context.PyrPayscaleGrades.Where(x => x.IntPayscaleGradeId == model.IntPayscaleGradeId).FirstOrDefaultAsync();
                    payscaleGrade.StrPayscaleGradeName = model.StrPayscaleGradeName;
                    payscaleGrade.StrPayscaleGradeCode = model.StrPayscaleGradeCode;
                    payscaleGrade.IntShortOrder = model.IntShortOrder;
                    payscaleGrade.NumMaxSalary = model.NumMaxSalary;
                    payscaleGrade.NumMinSalary = model.NumMinSalary;
                    //payscaleGrade.IntDesignationId = model.IntDesignationId;
                    payscaleGrade.StrDepentOn = model.StrDepentOn;
                    payscaleGrade.IntAccountId = model.IntAccountId;
                    payscaleGrade.IsActive = model.IsActive;
                    payscaleGrade.DteUpdatedAt = DateTime.Now;
                    payscaleGrade.IntUpdatedBy = model.IntCreatedBy;

                    _context.PyrPayscaleGrades.Update(payscaleGrade);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Updated Successfully.";
                }
                else
                {
                    PyrPayscaleGrade payscaleGrade = new PyrPayscaleGrade()
                    {
                        StrPayscaleGradeName = model.StrPayscaleGradeName,
                        StrPayscaleGradeCode = model.StrPayscaleGradeCode,
                        IntShortOrder = model.IntShortOrder,
                        NumMaxSalary = model.NumMaxSalary,
                        NumMinSalary = model.NumMinSalary,
                        //IntDesignationId = model.IntDesignationId,
                        StrDepentOn = model.StrDepentOn,
                        IntAccountId = model.IntAccountId,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = model.IntCreatedBy
                    };
                    _context.PyrPayscaleGrades.Add(payscaleGrade);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Saved Successfully.";
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                message.StatusCode = 500;
                message.Message = ex.Message;

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("GetAllScaleGrade")]
        public async Task<IActionResult> GetAllScaleGrade(long IntAccountId, long? IntPayscaleGradeId)
        {
            try
            {
                IEnumerable<PayScaleGradeViewModel> payScaleGradeList = await (from psg in _context.PyrPayscaleGrades
                                                                               where psg.IntAccountId == IntAccountId
                                                                               //join des in _context.MasterDesignations on psg.IntDesignationId equals des.IntDesignationId into des1
                                                                               //from desig in des1.DefaultIfEmpty()
                                                                               join bu in _context.MasterBusinessUnits on psg.IntBusinessUnitId equals bu.IntBusinessUnitId into bu1
                                                                               from busi in bu1.DefaultIfEmpty()
                                                                               where (IntPayscaleGradeId == null || IntPayscaleGradeId == 0 || psg.IntPayscaleGradeId == IntPayscaleGradeId)
                                                                               select new PayScaleGradeViewModel
                                                                               {
                                                                                   IntPayscaleGradeId = psg.IntPayscaleGradeId,
                                                                                   StrPayscaleGradeCode = psg.StrPayscaleGradeCode,
                                                                                   StrPayscaleGradeName = psg.StrPayscaleGradeName,
                                                                                   IntShortOrder = psg.IntShortOrder,
                                                                                   StrDepentOn = psg.StrDepentOn,
                                                                                   IntAccountId = psg.IntAccountId,
                                                                                   NumMaxSalary = psg.NumMaxSalary,
                                                                                   NumMinSalary = psg.NumMinSalary,
                                                                                   //IntDesignationId = psg.IntDesignationId,
                                                                                   StrDesignationName = psg.StrPayscaleGradeName,
                                                                                   IsActive = psg.IsActive,
                                                                                   IntBusinessUnitId = psg.IntBusinessUnitId,
                                                                                   StrBusinessUnitName = busi.StrBusinessUnit
                                                                               }).AsNoTracking().AsQueryable().ToListAsync();
                return Ok(payScaleGradeList);
            }
            catch (Exception ex)
            {
                MessageHelper msg = new MessageHelper();
                msg.StatusCode = 500;
                msg.Message = ex.Message;
                return BadRequest(msg);
            }
        }

        #endregion ======== Payscale Grade ==========

        #region ======== OverTime Configuration ==========
        [HttpGet]
        [Route("GetOverTimeConfig")]
        public async Task<IActionResult> GetOverTimeConfig(int accountId)
        {
            try
            {
                GetOverTimeConfigurationVM data = await _saasMasterService.GetOverTimeConfiguration(accountId);
                List<string> empty = new();
                return Ok(data != null ? data : empty);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveOverTimeConfig")]
        public async Task<IActionResult> SaveOverTimeConfig(GetOverTimeConfigurationVM model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _saasMasterService.SaveOTConfiguration(model);
                return Ok(new MessageHelperCreate());
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }
        [HttpGet]
        [Route("GetOverTimeConfigById")]
        public async Task<IActionResult> GetOverTimeConfigById(long intAccountId)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                return Ok(await _saasMasterService.GetOverTimeConfigById(intAccountId));
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }
        [HttpPost]
        [Route("UpdateOverTimeHours")]
        public async Task<IActionResult> UpdateAttenDailySummery(List<TimeAttendanceDailySummeryVM> objDetails)
        {
            MessageHelperUpdate res = new MessageHelperUpdate();
            try
            {
                if (objDetails.Count > 0)
                {
                    BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                    //var data = await _employeeService.PermissionCheckFromEmployeeListByEnvetFireEmployee(tokenData, )

                    await _saasMasterService.UpdateTimeAttenSummery(objDetails);
                    return Ok(res.Message);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }
        [HttpGet("GetOverTimeListData")]
        public async Task<IActionResult> GetOverTimeListData(string? PartType, long AccountId, long BusinessUnitId, long? WorkplaceGroupId, long? WorkplaceId, long? DepartmentId, long? DesignationId, long? EmployeeId, DateTime FromDate, DateTime ToDate, long? IntPageNo, long? IntPageSize, string? searchText)
        {
            try
            {
                OvertimeDetails otDetails = new OvertimeDetails();
                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "saas.sprOvertimeReport";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@partType", PartType);
                        sqlCmd.Parameters.AddWithValue("@intAccountId", AccountId);
                        sqlCmd.Parameters.AddWithValue("@intBusinessUnitId", BusinessUnitId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceGroupId", WorkplaceGroupId);
                        sqlCmd.Parameters.AddWithValue("@intWorkplaceId", WorkplaceId);
                        sqlCmd.Parameters.AddWithValue("@intDepartmentId", DepartmentId);
                        sqlCmd.Parameters.AddWithValue("@intDesignationId", DesignationId);
                        sqlCmd.Parameters.AddWithValue("@dteFromDate", FromDate.Date);
                        sqlCmd.Parameters.AddWithValue("@dteToDate", ToDate.Date);
                        sqlCmd.Parameters.AddWithValue("@intEmployeeId", EmployeeId);
                        sqlCmd.Parameters.AddWithValue("@intPageNo", IntPageNo);
                        sqlCmd.Parameters.AddWithValue("@intPageSize", IntPageSize);
                        sqlCmd.Parameters.AddWithValue("@strSearchText", searchText);


                        connection.Open();

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                        connection.Close();
                    }

                    string sql1 = "select TOP(1) * from saas.OverTimeConfiguration where intAccountId='" + AccountId + "' and isActive=1";
                    using (SqlCommand sqlCmd = new SqlCommand(sql1, connection))
                    {
                        connection.Open();

                        var reader = sqlCmd.ExecuteReader();

                        while (reader.Read())
                        {
                            otDetails.intMaxOverTimeDaily = (Convert.ToInt64(reader["intMaxOverTimeDaily"])) / 60;
                            otDetails.intMaxOverTimeMonthly = (Convert.ToInt64(reader["intMaxOverTimeMonthly"])) / 60;
                            otDetails.intOtAmountShouldBe = Convert.ToInt64(reader["intOtAmountShouldBe"]);
                            otDetails.intOtbenefitsHour = Convert.ToInt64(reader["intOtbenefitsHour"]);
                            //intOtbenefitsHour
                            //intOtAmountShouldBe
                        }
                        reader.Close();
                        connection.Close();
                    }
                }
                //list overtime
                List<OvertimeReport> dataList = new List<OvertimeReport>();
                foreach (DataRow row in dt.Rows)
                {
                    OvertimeReport overtime = new OvertimeReport();

                    overtime.intAutoId = Convert.ToInt64(row["intAutoId"]);
                    overtime.intEmployeeId = Convert.ToInt64(row["intEmployeeId"]);
                    overtime.strEmployeeCode = row["strEmployeeCode"].ToString();
                    overtime.strEmployeeName = row["strEmployeeName"].ToString();
                    overtime.strDesignationName = row["strDesignation"].ToString();
                    overtime.strDepartmentName = row["strDepartment"].ToString();
                    overtime.EmployementType = row["EmployementType"].ToString();
                    overtime.dteAttendanceDate = (DateTime)(row["dteAttendanceDate"]);
                    overtime.timeStartTime = (TimeSpan)(row["tmeStartTime"]);
                    overtime.timeEndTime = (TimeSpan)(row["tmeEndTime"]);
                    overtime.numHours = Convert.ToDecimal(row["numMinutes"]);
                    overtime.numTotalAmount = Convert.ToDecimal(row["totalAmount"]);
                    overtime.numPerMinunitRate = Convert.ToDecimal(row["perMinunits"]);
                    overtime.TotalCount = Convert.ToInt64(row["totalCount"]);
                    dataList.Add(overtime);
                }
                otDetails.overtimeReports = dataList;

                return Ok(otDetails);
            }
            catch (Exception ex)
            {
                return NotFound("Invalid Data");
            }
        }

        [HttpGet("GetPerMinuteAmountByID")]
        public async Task<IActionResult> GetPerminuteAmountById(long EmployeeId, long AccountId)
        {
            using var connection = new SqlConnection(Connection.iPEOPLE_HCM);
            string sql = "saas.sprOvertimeReport";

            var values = new
            {
                @partType = "OvertimePerMinuteAmount",
                @intAccountId = AccountId,
                @intEmployeeId = EmployeeId,
                @intBusinessUnitId = 0,
                @dteFromDate = "2022-01-01",
                @dteToDate = "2022-01-01"

            };

            var res = await connection.QueryFirstOrDefaultAsync(sql, values, commandType: CommandType.StoredProcedure);
            return Ok(res);
        }

        //[HttpPost]
        //[Route("SaveOverTimeConfigDetails")]
        //public async Task<IActionResult> SaveOverTimeConfigDetails(OverTimeConfigurationDetail model)
        //{
        //    MessageHelperCreate res = new MessageHelperCreate();
        //    try
        //    {
        //        return Ok(await _saasMasterService.SaveOTConfigDetails(model));
        //    }
        //    catch (Exception ex)
        //    {
        //        res.StatusCode = 500;
        //        res.Message = ex.Message;
        //        return BadRequest(res);
        //    }
        //}

        #endregion

        #region ========= Payroll Basic Percentage ========
        [HttpPost]
        [Route("CRUDGrossWiseBasicConfig")]
        public async Task<IActionResult> CRUDGrossWiseBasicConfig(PyrGrossWiseBasicConfig model)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (model.IntGrossWiseBasicId > 0)
                {
                    PyrGrossWiseBasicConfig grossWiseBasic = await _context.PyrGrossWiseBasicConfigs.Where(x => x.IntGrossWiseBasicId == model.IntGrossWiseBasicId).FirstOrDefaultAsync();
                    grossWiseBasic.IntBusinessUnitId = model.IntBusinessUnitId;
                    grossWiseBasic.IntAccountId = model.IntAccountId;
                    grossWiseBasic.NumMinGross = model.NumMinGross;
                    grossWiseBasic.NumMaxGross = model.NumMaxGross;
                    grossWiseBasic.NumPercentageOfBasic = model.NumPercentageOfBasic;
                    grossWiseBasic.IsActive = model.IsActive;
                    grossWiseBasic.IntCreateBy = model.IntCreateBy;
                    grossWiseBasic.DteCreateDate = DateTime.Now;

                    _context.PyrGrossWiseBasicConfigs.Update(grossWiseBasic);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Updated Successfully.";
                }
                else
                {
                    PyrGrossWiseBasicConfig GrossWiseBasicConfig = new PyrGrossWiseBasicConfig()
                    {
                        IntBusinessUnitId = model.IntBusinessUnitId,
                        IntAccountId = model.IntAccountId,
                        NumMinGross = model.NumMinGross,
                        NumMaxGross = model.NumMaxGross,
                        NumPercentageOfBasic = model.NumPercentageOfBasic,
                        IsActive = model.IsActive,
                        IntCreateBy = model.IntCreateBy,
                        DteCreateDate = DateTime.Now
                    };

                    _context.PyrGrossWiseBasicConfigs.Add(GrossWiseBasicConfig);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Saved Successfully.";
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                message.StatusCode = 500;
                message.Message = ex.Message;

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("GetAllGrossWiseBasic")]
        public async Task<IActionResult> GetAllGrossWiseBasic(long IntAccountId, long? IntBusinessUnitId, long? IntGrossWiseBasicId)
        {
            try
            {
                IEnumerable<GrossWiseBasicViewModel> payScaleGradeList = await (from psg in _context.PyrGrossWiseBasicConfigs
                                                                                where psg.IntAccountId == IntAccountId && psg.IntBusinessUnitId == IntBusinessUnitId
                                                                                join bu in _context.MasterBusinessUnits on psg.IntBusinessUnitId equals bu.IntBusinessUnitId into bu1
                                                                                from busi in bu1.DefaultIfEmpty()
                                                                                where (IntGrossWiseBasicId == null || IntGrossWiseBasicId == 0 || psg.IntGrossWiseBasicId == IntGrossWiseBasicId)
                                                                                select new GrossWiseBasicViewModel
                                                                                {
                                                                                    IntGrossWiseBasicId = psg.IntGrossWiseBasicId,
                                                                                    IntBusinessUnitId = psg.IntBusinessUnitId,
                                                                                    BusinessUnit = busi != null ? busi.StrBusinessUnit : "",
                                                                                    IntAccountId = psg.IntAccountId,
                                                                                    NumMinGross = psg.NumMinGross,
                                                                                    NumMaxGross = psg.NumMaxGross,
                                                                                    NumPercentageOfBasic = psg.NumPercentageOfBasic,
                                                                                    IsActive = psg.IsActive,
                                                                                    IntCreateBy = psg.IntCreateBy,
                                                                                    DteCreateDate = DateTime.Now
                                                                                }).AsNoTracking().AsQueryable().ToListAsync();
                return Ok(payScaleGradeList);
            }
            catch (Exception ex)
            {
                MessageHelper msg = new MessageHelper();
                msg.StatusCode = 500;
                msg.Message = ex.Message;
                return BadRequest(msg);
            }
        }

        #endregion

        #region ===== Gross Wise Basic Amount and Percentage =====
        [HttpGet]
        [Route("GetGrossWiseBasicAmountNPercentage")]
        public async Task<IActionResult> GetGrossWiseBasicAmountNPercentage(long BreakDownHeaderId, decimal GrossAmount)
        {
            try
            {
                PyrSalaryBreakdownHeader salaryBreakdownHeader = await _context.PyrSalaryBreakdownHeaders.Where(x => x.IntSalaryBreakdownHeaderId == BreakDownHeaderId && x.IsActive == true).FirstOrDefaultAsync();

                GrossWiseBasicPercentageViewModel basicPercentageViewModel = new GrossWiseBasicPercentageViewModel();

                if (salaryBreakdownHeader != null && (salaryBreakdownHeader.IntWorkplaceGroupId == 2 || salaryBreakdownHeader.IntWorkplaceGroupId == 3))
                {
                    PyrGrossWiseBasicConfig grossWiseBasicConfig = await _context.PyrGrossWiseBasicConfigs.Where(x => x.IsActive == true && x.NumMinGross <= GrossAmount && x.NumMaxGross >= GrossAmount).FirstOrDefaultAsync();

                    if (grossWiseBasicConfig != null && GrossAmount > 0)
                    {
                        basicPercentageViewModel.BasicSalary = GrossAmount * grossWiseBasicConfig.NumPercentageOfBasic * (decimal)0.01;
                        basicPercentageViewModel.NumPercentageOfGross = grossWiseBasicConfig.NumPercentageOfBasic;
                    }
                }

                return Ok(basicPercentageViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion

        #region ==== sole to area ddl ====
        [HttpGet]
        [Route("GetSoledepoDdl")]
        public async Task<IActionResult> GetSoledepoDdlByWorkplaceGroup(long businessUnitId, long WorkplaceGroupId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _payrollService.GetSoledepoDdlByWorkplaceGroup(tokenData.accountId, tokenData.businessUnitId, WorkplaceGroupId, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }


        [HttpGet]
        [Route("GetAreaBySoleDepoDdl")]
        public async Task<IActionResult> GetAreaBySoleDepoId(long businessUnitId, long WorkplaceGroupId, long soleDepoId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _payrollService.GetAreaBySoleDepoId(tokenData.accountId, tokenData.businessUnitId, WorkplaceGroupId, soleDepoId, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetAreaDDLByEmployeeId")]
        public async Task<IActionResult> GetAreaDDLByEmployeeId(long businessUnitId, long workplaceGroupId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId, workplaceGroupId = workplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);
                if (!tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                List<AreaPermissionVM> areaList = new();
                if (tokenData.isOfficeAdmin)
                {
                    areaList = (from ts in _context.TerritorySetups
                                where ts.IsActive == true && ts.IntTerritoryTypeId == 7
                                select new AreaPermissionVM()
                                {
                                    AreaId = ts.IntTerritoryId,
                                    AreaName = ts.StrTerritoryName
                                }).ToList();

                }
                else
                {
                    areaList = (from hd in _context.RoleExtensionHeaders
                                join rw in _context.RoleExtensionRows on hd.IntRoleExtensionHeaderId equals rw.IntRoleExtensionHeaderId
                                where hd.IsActive == true && rw.IsActive == true
                                && (tokenData.isOfficeAdmin == true ? true : rw.IntEmployeeId == tokenData.employeeId)
                                && rw.IntOrganizationTypeId == 7
                                select new AreaPermissionVM()
                                {
                                    AreaId = rw.IntOrganizationReffId,
                                    AreaName = rw.StrOrganizationReffName
                                }).ToList();
                }


                return Ok(areaList);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}