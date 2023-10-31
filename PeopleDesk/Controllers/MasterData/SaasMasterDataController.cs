using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Master.Interfaces;

namespace PeopleDesk.Controllers.MasterData
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaasMasterDataController : ControllerBase
    {
        private readonly ISaasMasterService _saasMasterService;
        private readonly PeopleDeskContext _context;

        public SaasMasterDataController(ISaasMasterService saasMasterService, PeopleDeskContext _context)
        {
            _saasMasterService = saasMasterService;
            this._context = _context;
        }

        #region ================== Employee BusinessUnit =====================

        [HttpPost]
        [Route("SaveBusinessUnit")]
        public async Task<IActionResult> SaveBusinessUnit(MasterBusinessUnit model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            MessageHelperUpdate resUpdate = new MessageHelperUpdate();

            try
            {
                if (await _context.MasterBusinessUnits.Where(x => x.StrBusinessUnit.ToLower() == model.StrBusinessUnit.ToLower()
                    && x.IntBusinessUnitId != model.IntBusinessUnitId
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    MasterBusinessUnit ret = await _saasMasterService.SaveBusinessUnit(model);
                    if (model.IntBusinessUnitId > 0)
                    {
                        resUpdate.AutoId = ret.IntBusinessUnitId;
                        return Ok(resUpdate);
                    }
                    else
                    {
                        res.AutoId = ret.IntBusinessUnitId;
                        return Ok(res);
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
        [Route("GetAllBusinessUnit")]
        public async Task<IActionResult> GetAllBusinessUnit(long accountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllBusinessUnit(accountId));
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
        [Route("GetBusinessUnitById")]
        public async Task<IActionResult> GetBusinessUnitById(long Id)
        {
            try
            {
                return Ok(await _saasMasterService.GetBusinessUnitById(Id));
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
        [Route("GetBusinessDetailsByBusinessUnitId")]
        public async Task<IActionResult> GetBusinessDetailsByBusinessUnitIdAsync(long businessUnitId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.Authorization, new PayloadIsAuthorizeVM { businessUnitId = businessUnitId }, PermissionLebelCheck.BusinessUnit);

                if(tokenData.isAuthorize)
                {
                    MasterBusinessUnitDWithAccount masterBusinessUnitDetails = await _saasMasterService.GetBusinessDetailsByBusinessUnitIdAsync(businessUnitId);

                    return masterBusinessUnitDetails != null ? StatusCode(200, masterBusinessUnitDetails) : StatusCode(404, "No data Found");
                }
                else
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
        }   

        [HttpPut]
        [Route("DeleteBusinessUnit")]
        public async Task<IActionResult> DeleteBusinessUnit(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeleteBusinessUnit(id);
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

        #endregion ================== Employee BusinessUnit =====================

        #region ====================== Employee Designation =====================

        [HttpPost]
        [Route("SaveDesignation")]
        public async Task<IActionResult> SaveDesignation(MasterDesignation model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.MasterDesignations.Where(x => x.StrDesignation.ToLower() == model.StrDesignation.ToLower()
                    && x.StrDesignationCode.ToLower() == model.StrDesignationCode.ToLower() && x.IntDesignationId != model.IntDesignationId
                    && x.IntAccountId == model.IntAccountId && x.IntBusinessUnitId == model.IntBusinessUnitId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    if (model.IntDesignationId > 0)
                    {
                        res.Message = "Updated Successfully";
                    }

                    MasterDesignation ret = await _saasMasterService.SaveDesignation(model);
                    res.AutoId = ret.IntDesignationId;

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

        [HttpPost]
        [Route("SaveDesignationV2")]
        public async Task<IActionResult> SaveDesignationV2(DesignationVM model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                List<long> designationListForRoleBridge = new List<long>();

                foreach (long businessUnitId in model.IntBusinessUnitIdList)
                {
                    if (businessUnitId == 0)
                    {
                        MasterDesignation existingDesignation = await _context.MasterDesignations.Where(x => x.IntAccountId == model.IntAccountId
                        && x.StrDesignation.ToLower() == model.StrDesignation.ToLower() && x.StrDesignationCode.ToLower() == model.StrDesignationCode.ToLower()
                        && x.IsActive == true).FirstOrDefaultAsync();

                        if (existingDesignation != null)
                        {
                            existingDesignation.IntBusinessUnitId = businessUnitId;
                            existingDesignation.IntPayscaleGradeId = model.IntPayscaleGradeId;
                            existingDesignation.DteUpdatedAt = DateTime.Now;
                            existingDesignation.IntUpdatedBy = model.IntUpdatedBy;

                            await _saasMasterService.SaveDesignation(existingDesignation);

                            res.Message = "Updated Successfully";
                            designationListForRoleBridge.Add(existingDesignation.IntDesignationId);
                        }
                        else
                        {
                            MasterDesignation newObj = new MasterDesignation
                            {
                                IntDesignationId = 0,
                                StrDesignation = model.StrDesignation,
                                StrDesignationCode = model.StrDesignationCode,
                                IntPositionId = model.IntPositionId,
                                IsActive = true,
                                IsDeleted = false,
                                IntAccountId = model.IntAccountId,
                                IntBusinessUnitId = businessUnitId,
                                DteCreatedAt = DateTime.Now,
                                IntCreatedBy = model.IntCreatedBy,
                                IntPayscaleGradeId = model.IntPayscaleGradeId
                            };
                            await _saasMasterService.SaveDesignation(newObj);

                            res.Message = "Created Successfully";
                            designationListForRoleBridge.Add(newObj.IntDesignationId);
                        }
                    }
                    else if (businessUnitId > 0 && !(await _context.MasterDesignations.Where(x => x.StrDesignation.ToLower() == model.StrDesignation.ToLower()
                            && x.StrDesignationCode.ToLower() == model.StrDesignationCode.ToLower() && x.IntDesignationId != model.IntDesignationId
                            && x.IntAccountId == model.IntAccountId && x.IntBusinessUnitId == businessUnitId && x.IsActive == true).CountAsync() > 0))
                    {
                        MasterDesignation newObj = new MasterDesignation
                        {
                            IntDesignationId = model.IntDesignationId,
                            StrDesignation = model.StrDesignation,
                            StrDesignationCode = model.StrDesignationCode,
                            IntPositionId = model.IntPositionId,
                            IsActive = true,
                            IsDeleted = false,
                            IntAccountId = model.IntAccountId,
                            IntBusinessUnitId = businessUnitId,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = model.IntCreatedBy,
                            IntPayscaleGradeId = model.IntPayscaleGradeId
                        };
                        await _saasMasterService.SaveDesignation(newObj);

                        res.Message = model.IntDesignationId > 0 ? "Updated Successfully" : "Created Successfully";
                        designationListForRoleBridge.Add(newObj.IntDesignationId);
                    }
                    else if (businessUnitId > 0)
                    {
                        designationListForRoleBridge.Add(businessUnitId);
                    }
                }

                List<RoleBridgeWithDesignation> RoleBridgeWithDesignationList = new List<RoleBridgeWithDesignation>();
                List<RoleBridgeWithDesignation> ExistingRoleBridgeWithDesignationList = new List<RoleBridgeWithDesignation>();

                if (model.IntUserRoleIdList.Count() == 1 && model.IntUserRoleIdList.FirstOrDefault().Value == 0)
                {
                    model.IntUserRoleIdList = await _context.UserRoles.Where(x => x.IntAccountId == model.IntAccountId && x.IsActive == true).Select(x => (long?)x.IntRoleId).ToArrayAsync();
                }

                foreach (long designationId in designationListForRoleBridge)
                {
                    var deleteList = await _context.RoleBridgeWithDesignations
                       .Where(x => x.IntAccountId == model.IntAccountId && x.IntDesignationOrEmployeeId == designationId
                                            && !model.IntUserRoleIdList.Contains(x.IntRoleId) && x.IsActive == true).ToListAsync();

                    ExistingRoleBridgeWithDesignationList.AddRange(deleteList);

                    foreach (long roleId in model.IntUserRoleIdList)
                    {
                        RoleBridgeWithDesignation isExists = await _context.RoleBridgeWithDesignations
                       .FirstOrDefaultAsync(x => x.IntAccountId == model.IntAccountId && x.IntDesignationOrEmployeeId == designationId
                                            && x.IntRoleId == roleId && x.IsActive == true);

                        if (isExists == null)
                        {
                            RoleBridgeWithDesignationList.Add(new RoleBridgeWithDesignation
                            {
                                IntId = 0,
                                IntAccountId = model.IntAccountId,
                                StrIsFor = "Designation",
                                IntDesignationOrEmployeeId = designationId,
                                IntRoleId = roleId,
                                IntCreatedBy = (long)model.IntCreatedBy,
                                DteCreatedDateTime = DateTime.Now,
                                IsActive = true
                            });
                        }
                    }
                }

                if (RoleBridgeWithDesignationList.Count() > 0)
                {
                    await _context.RoleBridgeWithDesignations.AddRangeAsync(RoleBridgeWithDesignationList);
                    await _context.SaveChangesAsync();
                }
                if (ExistingRoleBridgeWithDesignationList.Count() > 0)
                {
                    ExistingRoleBridgeWithDesignationList.ForEach(item =>
                    {
                        item.IsActive = false;
                    });
                    _context.RoleBridgeWithDesignations.UpdateRange(ExistingRoleBridgeWithDesignationList);
                    await _context.SaveChangesAsync();
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllDesignation")]
        public async Task<IActionResult> GetAllDesignation(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllDesignation(accountId, businessUnitId));
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
        [Route("GetDesignationById")]
        public async Task<IActionResult> GetDesignationById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetDesignationById(id));
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

        [HttpPut]
        [Route("DeleteDesignation")]
        public async Task<IActionResult> DeleteDesignation(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeleteDesignation(id);
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

        #endregion ====================== Employee Designation =====================

        #region =============== Separation Type ===================

        [HttpPost]
        [Route("SaveSeparationType")]
        public async Task<IActionResult> SaveSeparationType(EmpSeparationType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.EmpSeparationTypes.Where(x => x.StrSeparationType.ToLower() == model.StrSeparationType.ToLower()
                    && x.IntAccountId == model.IntAccountId && x.IntSeparationTypeId != model.IntSeparationTypeId).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    bool response = await _saasMasterService.SaveSeparationType(model);

                    if (response)
                    {
                        if (model.IsActive == false && model.IntSeparationTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (model.IntSeparationTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }

                    if (res.StatusCode == 500)
                    {
                        return BadRequest(res);
                    }
                    else { return Ok(res); }
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
        [Route("GetAllSeparationType")]
        public async Task<IActionResult> GetAllSeparationType(long accountId)
        {
            try
            {
                if (accountId > 0)
                {
                    return Ok(await _saasMasterService.GetAllSeparationType(accountId));
                }
                else
                {
                    return Ok(new List<MasterEmploymentType>());
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
        [Route("GetSeparationTypeById")]
        public async Task<IActionResult> GetSeparationTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetSeparationTypeById(id));
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

        #endregion =============== Separation Type ===================

        #region =============== User Role ===================

        [HttpPost]
        [Route("SaveUserRole")]
        public async Task<IActionResult> SaveUserRole(UserRole model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            long id = model.IntRoleId;
            try
            {
                if (await _context.UserRoles.Where(x => x.StrRoleName.ToLower() == model.StrRoleName.ToLower()
                    && x.IntAccountId == model.IntAccountId && x.IntRoleId != model.IntRoleId).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    bool response = await _saasMasterService.SaveUserRole(model);

                    if (response)
                    {
                        if (model.IsActive == false && model.IntRoleId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (id > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }

                    if (res.StatusCode == 500)
                    {
                        return BadRequest(res);
                    }
                    else { return Ok(res); }
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
        [Route("GetAllUserRole")]
        public async Task<IActionResult> GetAllUserRole()
        {
            try
            {
                var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tD.accountId > 0)
                {
                    return Ok(await _saasMasterService.GetAllUserRole(tD.accountId));
                }
                else
                {
                    return Ok(new List<MasterEmploymentType>());
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
        [Route("GetUserRoleById")]
        public async Task<IActionResult> GetUserRoleById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetUserRoleById(id));
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

        #endregion =============== User Role ===================

        #region ====================== Employee Employment Type =====================

        [HttpPost]
        [Route("SaveEmploymentType")]
        public async Task<IActionResult> SaveEmploymentType(MasterEmploymentType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.MasterEmploymentTypes.Where(x => x.StrEmploymentType.ToLower() == model.StrEmploymentType.ToLower()
                    && x.IntAccountId == model.IntAccountId && x.IntEmploymentTypeId != model.IntEmploymentTypeId).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    MasterEmploymentType employmentType = await _context.MasterEmploymentTypes.FirstOrDefaultAsync(x => x.StrEmploymentType.ToLower() == model.StrEmploymentType.ToLower() && x.IntAccountId == 0);

                    model.IntParentId = employmentType != null ? employmentType.IntEmploymentTypeId : 0;

                    bool isCreate = model.IntEmploymentTypeId > 0 ? false : true;
                    bool response = await _saasMasterService.SaveEmploymentType(model);

                    if (response)
                    {
                        if (model.IsActive == false && model.IntEmploymentTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (model.IntEmploymentTypeId > 0 && isCreate == false)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";

                            if (model.IntEmploymentTypeId > 0 && model.IsActive == true)
                            {
                                List<EmpEmployeeBasicInfo> existingEmployeeList = await _context.EmpEmployeeBasicInfos.Where(x => x.IntAccountId == model.IntAccountId && x.IntEmploymentTypeId == model.IntEmploymentTypeId && x.IsActive == true).ToListAsync();
                                if (existingEmployeeList.Count() > 0)
                                {
                                    existingEmployeeList.ForEach(emp =>
                                    {
                                        emp.StrEmploymentType = model.StrEmploymentType;
                                    });

                                    _context.EmpEmployeeBasicInfos.UpdateRange(existingEmployeeList);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }

                    if (res.StatusCode == 500)
                    {
                        return BadRequest(res);
                    }
                    else { return Ok(res); }
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
        [Route("GetAllEmploymentType")]
        public async Task<IActionResult> GetAllloymentType(long accountId)
        {
            try
            {
                if (accountId > 0)
                {
                    return Ok(await _saasMasterService.GetAllEmploymentType(accountId));
                }
                else
                {
                    return Ok(new List<MasterEmploymentType>());
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
        [Route("GetAllEmploymentTypeForWorkline")]
        public async Task<IActionResult> GetAllEmploymentTypeForWorkline(long accountId)
        {
            try
            {
                if (accountId > 0)
                {
                    IEnumerable<EmploymentTypeVM> empType = await _saasMasterService.GetAllEmploymentType(accountId);
                    empType = empType.Where(x => x.IntParentId == 1 || x.IntParentId == 3).ToList();
                    return Ok(empType);
                }
                else
                {
                    return Ok(new List<MasterEmploymentType>());
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
        [Route("GetEmpEmploymentTypeById")]
        public async Task<IActionResult> GetEmploymentTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetEmploymentTypeById(id));
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

        [HttpPut]
        [Route("DeleteEmploymentType")]
        public async Task<IActionResult> DeleteEmploymentType(long id)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();
                res = await _saasMasterService.DeleteEmploymentType(id);

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

        #endregion ====================== Employee Employment Type =====================

        #region ============ Loan Type ==============

        [HttpPost]
        [Route("SaveEmpLoanType")]
        public async Task<IActionResult> SaveEmpLoanType(EmpLoanType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.EmpLoanTypes.Where(x => x.StrLoanType.ToLower() == model.StrLoanType.ToLower()
                    && x.IntAccountId == model.IntAccountId && model.IntLoanTypeId == 0).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    bool response = false;
                    if (model.IntLoanTypeId > 0 && model.IsActive == false && _context.EmpLoanApplications.Where(x => x.IntLoanTypeId == model.IntLoanTypeId && x.IsActive == true).Count() > 0)
                    {
                        res.StatusCode = 500;
                        res.Message = "This type of Loan already exists in Loan Application.";
                        return BadRequest(res);
                    }
                    else
                    {
                        response = await _saasMasterService.SaveLoanType(model);
                    }

                    if (response)
                    {
                        if (model.IsActive == false && model.IntLoanTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (model.IntLoanTypeId > 0 && model.IsActive == true)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }

                    if (res.StatusCode == 500)
                    {
                        return BadRequest(res);
                    }
                    else { return Ok(res); }
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
        [Route("GetAllEmpLoanType")]
        public async Task<IActionResult> GetAllEmpLoanType()
        {
            try
            {
                return Ok(await _saasMasterService.GetAllLoanType());
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
        [Route("GetEmpLoanTypeById")]
        public async Task<IActionResult> GetEmpLoanTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetLoanTypeById(id));
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

        [HttpPut]
        [Route("DeleteLoanType")]
        public async Task<IActionResult> DeleteLoanType(long id)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();

                await _saasMasterService.DeleteLoanType(id);
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

        #endregion ============ Loan Type ==============

        #region ================== Employee Position =====================

        [HttpPost]
        [Route("SavePosition")]
        public async Task<IActionResult> SavePosition(MasterPosition model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            MessageHelperUpdate resUpdate = new MessageHelperUpdate();

            try
            {
                if (await _context.MasterPositions.Where(x => x.StrPosition.ToLower() == model.StrPosition.ToLower()
                    && x.StrPositionCode.ToLower() == model.StrPositionCode.ToLower() && x.IntPositionId != model.IntPositionId
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    MasterPosition ret = await _saasMasterService.SavePosition(model);
                    if (model.IntPositionId > 0)
                    {
                        res.AutoId = ret.IntPositionId;
                        return Ok(resUpdate);
                    }
                    else
                    {
                        res.AutoId = ret.IntPositionId;
                        return Ok(res);
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
        [Route("GetAllPosition")]
        public async Task<IActionResult> GetAllPosition(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllPosition(accountId, businessUnitId));
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
        [Route("GetPositionById")]
        public async Task<IActionResult> GetPositionById(long Id)
        {
            try
            {
                return Ok(await _saasMasterService.GetPositionById(Id));
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

        [HttpPut]
        [Route("DeletePosition")]
        public async Task<IActionResult> DeletePosition(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeletePosition(id);
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

        #endregion ================== Employee Position =====================

        #region ================== Employee Workplace =====================

        [HttpPost]
        [Route("SaveWorkplace")]
        public async Task<IActionResult> SaveWorkplace(MasterWorkplace model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.MasterWorkplaces.Where(x => x.StrWorkplace.ToLower() == model.StrWorkplace.ToLower() && x.IntBusinessUnitId == model.IntBusinessUnitId
                    && x.StrWorkplaceCode.ToLower() == model.StrWorkplaceCode.ToLower() && x.IntWorkplaceId != model.IntWorkplaceId
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    if (model.IntWorkplaceId > 0)
                    {
                        res.Message = "Updated Successfully";
                    }

                    MasterWorkplace ret = await _saasMasterService.SaveWorkplace(model);

                    res.AutoId = ret.IntWorkplaceId;

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

        [HttpGet]
        [Route("GetAllWorkplace")]
        public async Task<IActionResult> GetAllWorkplace(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllWorkplace(accountId, businessUnitId));
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
        [Route("GetWorkplaceById")]
        public async Task<IActionResult> GetWorkplaceById(long Id)
        {
            try
            {
                return Ok(await _saasMasterService.GetWorkplaceById(Id));
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

        [HttpPut]
        [Route("DeleteWorkplace")]
        public async Task<IActionResult> DeleteWorkplace(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeleteWorkplace(id);
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

        #endregion ================== Employee Workplace =====================

        #region ================== Employee WorkplaceGroup =====================

        [HttpPost]
        [Route("SaveWorkplaceGroup")]
        public async Task<IActionResult> SaveWorkplaceGroup(MasterWorkplaceGroup model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.MasterWorkplaceGroups.Where(x => x.StrWorkplaceGroup.ToLower() == model.StrWorkplaceGroup.ToLower()
                && x.IntWorkplaceGroupId != model.IntWorkplaceGroupId
                && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    MasterWorkplaceGroup ret = await _saasMasterService.SaveWorkplaceGroup(model);
                    res.AutoId = ret.IntWorkplaceGroupId;
                    if (model.IntWorkplaceGroupId > 0)
                    {
                        res.Message = "Updated Successfully";
                    }
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

        [HttpGet]
        [Route("GetAllWorkplaceGroup")]
        public async Task<IActionResult> GetAllWorkplaceGroup(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllWorkplaceGroup(accountId, businessUnitId));
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
        [Route("GetWorkplaceGroupById")]
        public async Task<IActionResult> GetWorkplaceGroupById(long Id)
        {
            try
            {
                return Ok(await _saasMasterService.GetWorkplaceGroupById(Id));
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

        [HttpPut]
        [Route("DeleteWorkplaceGroup")]
        public async Task<IActionResult> DeleteWorkplaceGroup(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeleteWorkplaceGroup(id);
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

        #endregion ================== Employee WorkplaceGroup =====================

        #region ================== Employee BankWallet =====================

        [HttpPost]
        [Route("SaveBankWallet")]
        public async Task<IActionResult> SaveBankWallet(BankWallet model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.BankWallets.Where(x => x.StrBankWalletName.ToLower() == model.StrBankWalletName.ToLower()
                && x.StrCode.ToLower() == model.StrCode.ToLower() && x.IntBankWalletId != model.IntBankWalletId
                && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    BankWallet ret = await _saasMasterService.SaveBankWallet(model);
                    res.AutoId = ret.IntBankWalletId;
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

        [HttpGet]
        [Route("GetAllBankWallet")]
        public async Task<IActionResult> GetAllBankWallet()
        {
            try
            {
                return Ok(await _saasMasterService.GetAllBankWallet());
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
        [Route("GetBankWalletById")]
        public async Task<IActionResult> GetBankWalletById(long Id)
        {
            try
            {
                return Ok(await _saasMasterService.GetBankWalletById(Id));
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

        [HttpPut]
        [Route("DeleteBankWallet")]
        public async Task<IActionResult> DeleteBankWallet(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully"
                };
                await _saasMasterService.DeleteBankWalletById(id);
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

        #endregion ================== Employee BankWallet =====================

        #region ========== GlobalDocumentType ==================

        [HttpPost]
        [Route("SaveGlobalDocumentType")]
        public async Task<IActionResult> SaveGlobalDocumentType(GlobalDocumentType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                if (await _context.GlobalDocumentTypes.Where(x => x.StrDocumentType.ToLower() == model.StrDocumentType.ToLower()
                && x.IntDocumentTypeId != model.IntDocumentTypeId && x.IntAccountId == model.IntAccountId
                && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    if (model.IntDocumentTypeId > 0)
                    {
                        res.Message = "Updated Successfully";
                    }
                    await _saasMasterService.SaveGlobalDocumentType(model);

                    res.AutoId = model.IntDocumentTypeId;
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

        [HttpGet]
        [Route("GetAllGlobalDocumentType")]
        public async Task<IActionResult> GetAllGlobalDocumentType(long IntAccountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllGlobalDocumentType(IntAccountId));
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
        [Route("GetGlobalDocumentTypeById")]
        public async Task<IActionResult> GetUserTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetGlobalDocumentTypeById(id));
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
        [Route("DeleteGlobalDocumentTypeById")]
        public async Task<IActionResult> DeleteUserTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.DeleteGlobalDocumentTypeById(id));
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

        #endregion ========== GlobalDocumentType ==================

        #region ========== GlobalFileUrl ==================

        [HttpPost]
        [Route("SaveGlobalFileUrl")]
        public async Task<IActionResult> SaveGlobalFileUrl(GlobalFileUrl model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _saasMasterService.SaveGlobalFileUrl(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllGlobalFileUrl")]
        public async Task<IActionResult> GetAllGlobalFileUrl()
        {
            try
            {
                return Ok(await _saasMasterService.GetAllGlobalFileUrl());
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
        [Route("GetGlobalFileUrlById")]
        public async Task<IActionResult> GetGlobalFileUrlById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetGlobalFileUrlById(id));
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

        #endregion ========== GlobalFileUrl ==================

        #region ========== GlobalInstitute ==================

        [HttpPost]
        [Route("SaveGlobalInstitute")]
        public async Task<IActionResult> SaveGlobalInstitute(GlobalInstitute model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.GlobalInstitutes.Where(x => x.StrInstituteName.ToLower() == model.StrInstituteName.ToLower()
                && x.IntInstituteId != model.IntInstituteId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    await _saasMasterService.SaveGlobalInstitute(model);

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

        [HttpGet]
        [Route("GetAllGlobalInstitute")]
        public async Task<IActionResult> GetAllGlobalInstitute()
        {
            try
            {
                return Ok(await _saasMasterService.GetAllGlobalInstitute());
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
        [Route("GetGlobalInstituteById")]
        public async Task<IActionResult> GetGlobalInstituteById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetGlobalInstituteById(id));
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
        [Route("DeleteGlobalInstituteById")]
        public async Task<IActionResult> DeleteGlobalInstituteById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.DeleteGlobalInstituteById(id));
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

        #endregion ========== GlobalInstitute ==================

        #region ========== Workline Config =============

        [HttpPost]
        [Route("CRUDWorklineConfig")]
        public async Task<IActionResult> CRUDWorklineConfig(EmpWorklineConfigViewModel obj)
        {
            try
            {
                MessageHelper message = await _saasMasterService.CRUDWorklineConfig(obj);
                if (message.StatusCode == 200)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllWorklineConfig")]
        public async Task<IActionResult> GetAllWorklineConfig(long AccountId)
        {
            try
            {
                IEnumerable<EmpWorklineConfigViewModel> empWorklinesList = await _saasMasterService.GetAllWorklineConfig(AccountId);
                return Ok(empWorklinesList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetWorklineConfigById")]
        public async Task<IActionResult> GetWorklineConfigById(long IntWorklineId)
        {
            try
            {
                EmpWorklineConfigViewModel empWorklinesList = await _saasMasterService.GetWorklineConfigById(IntWorklineId);
                return Ok(empWorklinesList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion ========== Workline Config =============

        #region ========== LveLeaveType ==================

        [HttpPost]
        [Route("SaveLveLeaveType")]
        public async Task<IActionResult> SaveLveLeaveType(LveLeaveType model)
        {
            BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
            if(tokenData.accountId == -1 || tokenData.accountId != model.IntAccountId)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }

            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.LveLeaveTypes.Where(x => x.StrLeaveType.ToLower() == model.StrLeaveType.ToLower()
                    && x.StrLeaveTypeCode.ToLower() == model.StrLeaveTypeCode.ToLower() && x.IntLeaveTypeId != model.IntLeaveTypeId
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    LveLeaveType leaveType = await _context.LveLeaveTypes.FirstOrDefaultAsync(x => x.StrLeaveType.ToLower() == model.StrLeaveType.ToLower() && x.IntAccountId == 0);

                    model.IntParentId = leaveType != null ? leaveType.IntLeaveTypeId : 0;

                    bool isCreate = model.IntLeaveTypeId > 0 ? false : true;
                    bool response = await _saasMasterService.SaveLveLeaveType(model);

                    if (response)
                    {
                        if (model.IsActive == false && model.IntLeaveTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (model.IntLeaveTypeId > 0 && isCreate == false)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }
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

        [HttpGet]
        [Route("GetAllLveLeaveType")]
        public async Task<IActionResult> GetAllLveLeaveType()
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                return Ok(await _saasMasterService.GetAllLveLeaveType(tokenData.accountId));
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
        [Route("GetLveLeaveTypeById")]
        public async Task<IActionResult> GetLveLeaveTypeById(long id)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                return Ok(await _saasMasterService.GetLveLeaveTypeById(id));
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
        [Route("DeleteLveLeaveTypeById")]
        public async Task<IActionResult> DeleteLveLeaveTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.DeleteLveLeaveTypeById(id));
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

        #endregion ========== LveLeaveType ==================

        #region ========== LveMovementType ==================

        [HttpPost]
        [Route("SaveLveMovementType")]
        public async Task<IActionResult> SaveLveMovementType(LveMovementType model)
        {
            BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
            if (tokenData.accountId == -1 || tokenData.accountId != model.IntAccountId)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }

            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                if (await _context.LveMovementTypes.Where(x => x.StrMovementType.ToLower() == model.StrMovementType.ToLower()
                && x.StrMovementTypeCode.ToLower() == model.StrMovementTypeCode.ToLower() && x.IntMovementTypeId != model.IntMovementTypeId
                && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name & Code !!!";
                    return BadRequest(res);
                }
                else
                {
                    await _saasMasterService.SaveLveMovementType(model);
                    if (model.IntMovementTypeId > 0)
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
        [Route("GetAllLveMovementType")]
        public async Task<IActionResult> GetAllLveMovementType()
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                return Ok(await _saasMasterService.GetAllLveMovementType(tokenData.accountId));
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
        [Route("GetLveMovementTypeById")]
        public async Task<IActionResult> GetLveMovementTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetLveMovementTypeById(id));
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
        [Route("DeleteLveMovementTypeById")]
        public async Task<IActionResult> DeleteLveMovementTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.DeleteLveMovementTypeById(id));
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

        #endregion ========== LveMovementType ==================

        #region ===> Employment Typewise Leave Balance <===

        [HttpPost]
        [Route("CRUDEmploymentTypeWiseLeaveBalance")]
        public async Task<IActionResult> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel obj)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                //BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId }, PermissionLebelCheck.BusinessUnit);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var response = await _saasMasterService.CRUDEmploymentTypeAndWorkplaceWiseLeaveBalanceViewModel(obj, tokenData);

                if (response.StatusCode != 200)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }


                //FOR SP

                //PartId = 1 -> Create
                //PartId = 2 -> Edit
                //PartId = 3 -> Delete

                //CRUDEmploymentTypeWiseLeaveBalanceViewModel obj
                //BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId }, PermissionLebelCheck.BusinessUnit);

                //if(tokenData.accountId == -1 || !tokenData.isAuthorize)
                //{
                //    return BadRequest(new MessageHelperAccessDenied());
                //}

                //var response = await _saasMasterService.CRUDEmploymentTypeWiseLeaveBalance(obj);

                //if (response.Message == "500")
                //{
                //    response.Message = "Already exist";
                //    response.StatusCode = 500;
                //    return BadRequest(response);
                //}
                //else
                //{
                //    return Ok(response);
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        #endregion ===> Employment Typewise Leave Balance <===

        #region ========== Poliocy ==========

        [HttpPost]
        [Route("CRUDPolicyCategory")]
        public async Task<IActionResult> CRUDPolicyCategory(CRUDPolicyCategoryViewModel obj)
        {
            try
            {
                var res = await _saasMasterService.CRUDPolicyCategory(obj);
                return Ok(res);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("GetPolicyCategoryDDL")]
        public async Task<IActionResult> GetPolicyCategoryDDL(long AccountId)
        {
            try
            {
                var res = await _saasMasterService.GetPolicyCategoryDDL(AccountId);
                return Ok(res);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("GetPolicyAreaTypes")]
        public async Task<IActionResult> GetPolicyAreaTypes()
        {
            try
            {
                var res = await _saasMasterService.GetPolicyAreaTypes();
                return Ok(res);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpPost]
        [Route("CreatePolicy")]
        public async Task<IActionResult> CreatePolicy(PolicyCommonViewModel obj)
        {
            try
            {
                var res = await _saasMasterService.CreatePolicy(obj);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetPolicyLanding")]
        public async Task<IActionResult> GetPolicyLanding(long AccountId, long BusinessUnitId, long CategoryId, string? Search)
        {
            try
            {
                var res = await _saasMasterService.GetPolicyLanding(AccountId, BusinessUnitId, CategoryId, Search);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetUploadedPolicyById")]
        public async Task<IActionResult> GetUploadedPolicyById(long AccountId, long BusinessUnitId, long PolicyId)
        {
            try
            {
                PolicyHeader policyHeader = await _context.PolicyHeaders.Where(x => x.IntAccountId == AccountId && x.IntBusinessUnitId == BusinessUnitId && x.IntPolicyId == PolicyId).FirstOrDefaultAsync();

                return Ok(policyHeader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("DeletePolicy")]
        public async Task<IActionResult> DeletePolicy(long PolicyId)
        {
            try
            {
                var res = await _saasMasterService.DeletePolicy(PolicyId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetPolicyOnEmployeeInbox")]
        public async Task<IActionResult> GetPolicyOnEmployeeInbox()
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _saasMasterService.GetPolicyOnEmployeeInbox(tokenData.employeeId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelperError { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("PolicyAcknowledge")]
        public async Task<IActionResult> PolicyAcknowledge(long PolicyId, long EmployeeId)
        {
            try
            {
                var res = await _saasMasterService.PolicyAcknowledge(PolicyId, EmployeeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ========== Poliocy ==========

        #region ========== Payroll   Management ==========

        [HttpPost]
        [Route("CRUDPayrollManagement")]
        public async Task<IActionResult> CRUDPayrollManagement(CRUDPayrolManagementViewModel obj)
        {
            try
            {
                MessageHelper msg = await _saasMasterService.CRUDPayrollManagement(obj);

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

        #endregion ========== Payroll   Management ==========

        #region ========== Bank Branch ==========

        [HttpGet]
        [Route("BankBranchLanding")]
        public async Task<IActionResult> BankBranchLanding(long? bankId, long? bankBranchId, long? accountId, string? search)
        {
            try
            {
                var res = await _saasMasterService.BankBranchLanding(bankId, bankBranchId, accountId, search);
                return Ok(res);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("BankBranchLandingById")]
        public async Task<IActionResult> BankBranchLandingById(long IntBankBranchId)
        {
            try
            {
                BankBranchViewModel bankBranch = await _saasMasterService.BankBranchLandingById(IntBankBranchId);
                return Ok(bankBranch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion ========== Bank Branch ==========

        #region ========== Master dashboard component ===========

        [HttpPost]
        [Route("DashboardComponent")]
        public async Task<IActionResult> DashboardComponent(MasterDashboardComponent obj)
        {
            GeneratedHashCode hashCodeObj = _context.GeneratedHashCodes.OrderBy(x => x.IntHashId).FirstOrDefault(x => x.IsLocked == false);
            obj.StrHashCode = hashCodeObj.StrHashCode;

            MessageHelperCreate res = await _saasMasterService.SaveDashboardComponent(obj);
            if (res.StatusCode == 200)
            {
                hashCodeObj.IsLocked = true;
                _context.GeneratedHashCodes.Update(hashCodeObj);
                await _context.SaveChangesAsync();
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("DashboardComponentLanding")]
        public async Task<IActionResult> DashboardComponentLanding()
        {
            return Ok(await _saasMasterService.GetAllDashboardComponent());
        }

        [HttpGet]
        [Route("DashboardComponentById")]
        public async Task<IActionResult> DashboardComponentById(long id)
        {
            return Ok(await _saasMasterService.GetDashboardComponentById(id));
        }

        [HttpDelete]
        [Route("DashboardComponent")]
        public async Task<IActionResult> DashboardComponent(long id)
        {
            return Ok(await _saasMasterService.DeleteDashboardComponent(id));
        }

        [HttpGet]
        [Route("DashboardComponentPermissionLanding")]
        public async Task<IActionResult> DashboardComponentPermissionLanding(long accountId)
        {
            List<MasterComponentVM> list = await (from c in _context.MasterDashboardComponents
                                                  join p in _context.MasterDashboardComponentPermissions on c.IntId equals p.IntDashboardComponentId into p2
                                                  from pp in p2.DefaultIfEmpty()
                                                  select new MasterComponentVM
                                                  {
                                                      Obj = c,
                                                      IntPermissionId = pp != null ? pp.IntId : 0,
                                                      IsPermission = pp != null ? ((pp.IntAccountId == accountId && pp.IsActive == true) ? true : false) : false
                                                  }).ToListAsync();
            return Ok(list);
        }

        #endregion ========== Master dashboard component ===========

        #region ========== Master dashboard component permission ==========

        [HttpPost]
        [Route("DashboardComponentPermission")]
        public async Task<IActionResult> DashboardComponentPermission(List<DashboardComponentPermissionVM> model)
        {
            try
            {
                MessageHelper response = new MessageHelper();
                if (model.Count() > 0)
                {
                    List<MasterDashboardComponentPermission> removePermission = new List<MasterDashboardComponentPermission>();
                    List<MasterDashboardComponentPermission> addPermission = new List<MasterDashboardComponentPermission>();

                    model.Where(x => x.IsDelete == true).ToList().ForEach(x =>
                    {
                        MasterDashboardComponentPermission obj = _context.MasterDashboardComponentPermissions
                                                                        .FirstOrDefault(y => y.IntId == x.IntPermissionId);

                        if (obj != null)
                        {
                            obj.IsActive = false;
                            obj.DteUpdatedAt = DateTime.Now;
                            obj.IntUpdatedBy = x.ActionById;
                            removePermission.Add(obj);
                        }
                    });

                    addPermission = model.Where(x => x.IsCreate == true).Select(x => new MasterDashboardComponentPermission
                    {
                        IntDashboardComponentId = x.IntDashboardId,
                        StrHashCode = x.StrHashCode,
                        IntAccountId = x.IntAccountId,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = x.ActionById
                    }).ToList();

                    _context.MasterDashboardComponentPermissions.RemoveRange(removePermission);
                    await _context.MasterDashboardComponentPermissions.AddRangeAsync(addPermission);
                    await _context.SaveChangesAsync();

                    response.Message = "Permission Update Successfully";
                    response.StatusCode = 200;
                    return Ok(response);
                }
                else
                {
                    response.Message = "invalid data";
                    response.StatusCode = 400;
                    return BadRequest(response);
                }

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("DashboardComponentPermissionById")]
        public async Task<IActionResult> GetDashboardComponentPermissionById(long id)
        {
            return Ok(await _saasMasterService.GetDashboardComponentPermissionById(id));
        }

        #endregion ========== Master dashboard component permission ==========

        #region Notification Category

        [HttpGet]
        [Route("NotificaitonPermission")]
        public async Task<IActionResult> NotificaitonPermission(long IntAccountId)
        {
            List<NotificationCategoryViewModel> datas = new List<NotificationCategoryViewModel>();

            List<NotificationCategory> NotificationCategoryList = await _context.NotificationCategories.Where(x => x.IsActive == true).ToListAsync();
            List<NotificationPermissionMaster> NotificationPermissionMastersList = await _context.NotificationPermissionMasters.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId).ToListAsync();
            List<NotificationCategoriesType> NotificationCategoriesTypesList = await _context.NotificationCategoriesTypes.Where(x => x.IsActive == true).ToListAsync();
            List<NotificationPermissionDetail> NotificationPermissionDetailsList = await _context.NotificationPermissionDetails.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId).ToListAsync();

            NotificationCategoryList.ForEach(c =>
            {
                NotificationPermissionMaster master = NotificationPermissionMastersList.Where(x => x.IntNcategoryId == c.IntId).FirstOrDefault();
                List<NotificationPermissionDetail> detailsList = NotificationPermissionDetailsList.Where(x => x.IntAccountId == IntAccountId && master.IntPermissionId == x.IntPermissionId).ToList();

                datas.Add(new NotificationCategoryViewModel
                {
                    CategoryId = c.IntId,
                    PermissionId = master == null ? 0 : master.IntPermissionId,
                    CategoryName = c.StrCategoriesName,
                    IsChecked = master == null ? false : true,
                    NotificationCategoryTypes = (from cateType in NotificationCategoriesTypesList
                                                 join npd1 in detailsList on cateType.IntId equals npd1.IntNcategoryTypeId into npd2
                                                 from npd in npd2.DefaultIfEmpty()
                                                 where cateType.IsActive == true
                                                 select new NotificationCategoryTypeViewModel
                                                 {
                                                     PermissionId = npd == null ? 0 : npd.IntPermissionId,
                                                     CategoryTypeId = cateType.IntId,
                                                     TypeName = cateType.StrTypeName,
                                                     IsChecked = npd == null ? false : true
                                                 }).ToList()
                });
            });

            return Ok(datas);
        }

        [HttpPost]
        [Route("NotificationPermissionCRUD")]
        public async Task<IActionResult> NotificationPermissionCRUD(NotificationPermissionViewModel notifiyPermission)
        {
            try
            {
                NotificationPermissionMaster permissionMaster = new NotificationPermissionMaster();
                NotificationPermissionDetail permissionDetails = new NotificationPermissionDetail();

                List<NotificationPermissionMaster> existsPermissionMaster = await _context.NotificationPermissionMasters
                    .Where(x => x.IntAccountId == notifiyPermission.IntAccountId && x.IsActive == true)
                    .Select(y => new NotificationPermissionMaster
                    {
                        IntPermissionId = y.IntPermissionId,
                        IntAccountId = y.IntAccountId,
                        IntNcategoryId = y.IntNcategoryId,
                        StrNcategoryName = y.StrNcategoryName,
                        IsActive = false,
                        IntCreatedBy = y.IntCreatedBy,
                        DteCreatedAt = y.DteCreatedAt,
                        IntUpdatedBy = notifiyPermission.intCreateBy,
                        DteUpdatedAt = DateTime.Now
                    }).ToListAsync();

                List<NotificationPermissionDetail> existsPermissionDetails = await _context.NotificationPermissionDetails
                    .Where(x => x.IntAccountId == notifiyPermission.IntAccountId && x.IsActive == true)
                    .Select(y => new NotificationPermissionDetail
                    {
                        IntPermissionDetailsId = y.IntPermissionDetailsId,
                        IntPermissionId = y.IntPermissionId,
                        IntNcategoryTypeId = y.IntNcategoryTypeId,
                        SteNcategoryTypeName = y.SteNcategoryTypeName,
                        IntAccountId = y.IntAccountId,
                        IsActive = false,
                        IntCreatedBy = y.IntCreatedBy,
                        DteCreatedAt = y.DteCreatedAt,
                        IntUpdatedBy = notifiyPermission.intCreateBy,
                        DteUpdatedAt = DateTime.Now
                    }).ToListAsync();

                _context.NotificationPermissionMasters.UpdateRange(existsPermissionMaster);
                _context.NotificationPermissionDetails.UpdateRange(existsPermissionDetails);
                await _context.SaveChangesAsync();

                foreach (var item in notifiyPermission.notificationCategoriesViewModel)
                {
                    permissionMaster = new NotificationPermissionMaster
                    {
                        IntAccountId = notifiyPermission.IntAccountId,
                        IntNcategoryId = item.CategoryId,
                        StrNcategoryName = item.CategoryName,
                        IsActive = true,
                        IntCreatedBy = notifiyPermission.intCreateBy,
                        DteCreatedAt = DateTime.Now
                    };

                    await _context.NotificationPermissionMasters.AddAsync(permissionMaster);
                    await _context.SaveChangesAsync();

                    List<NotificationPermissionDetail> permissionDetailsList = new List<NotificationPermissionDetail>();

                    foreach (var NotifyDetail in item.notificationCategoriesViewModel)
                    {
                        permissionDetails = new NotificationPermissionDetail
                        {
                            IntPermissionId = permissionMaster.IntPermissionId,
                            IntNcategoryTypeId = NotifyDetail.CategoryTypeId,
                            SteNcategoryTypeName = NotifyDetail.CategoryTypeName,
                            IntAccountId = notifiyPermission.IntAccountId,
                            IsActive = true,
                            IntCreatedBy = notifiyPermission.intCreateBy,
                            DteCreatedAt = DateTime.Now,
                        };
                        permissionDetailsList.Add(permissionDetails);
                    }
                    await _context.NotificationPermissionDetails.AddRangeAsync(permissionDetailsList);
                    await _context.SaveChangesAsync();
                }

                MessageHelper message = new MessageHelper();
                message.Message = "Save Successfully.";
                message.StatusCode = 200;
                return Ok(message);
            }
            catch (Exception ex)
            {
                MessageHelper message = new MessageHelper();
                message.Message = ex.Message;
                message.StatusCode = 500;
                return Ok(message);
            }
        }

        [HttpGet]
        [Route("NotificationPermissionLanding")]
        public async Task<IActionResult> NotificationPermissionLanding()
        {
            List<AccountNotificationViewModel> accountNotification = new List<AccountNotificationViewModel>();

            accountNotification = await (from acc in _context.Accounts
                                         where acc.IsActive == true
                                         select new AccountNotificationViewModel
                                         {
                                             CompanyId = acc.IntAccountId,
                                             companyName = acc.StrAccountName,
                                             notificationCategoryViewModel = (from nc in _context.NotificationCategories
                                                                              join npm in _context.NotificationPermissionMasters on nc.IntId equals npm.IntNcategoryId
                                                                              where npm.IntAccountId == acc.IntAccountId && npm.IsActive == true && nc.IsActive == true
                                                                              select new NotificationCategoryViewModel
                                                                              {
                                                                                  CategoryId = nc.IntId,
                                                                                  PermissionId = npm == null ? 0 : npm.IntPermissionId,
                                                                                  CategoryName = nc.StrCategoriesName,
                                                                                  IsChecked = npm == null ? false : true,
                                                                                  NotificationCategoryTypes = (from cateType in _context.NotificationCategoriesTypes
                                                                                                               join npd in _context.NotificationPermissionDetails on cateType.IntId equals npd.IntNcategoryTypeId
                                                                                                               where npd.IntPermissionId == npm.IntPermissionId && npd.IntAccountId == acc.IntAccountId && cateType.IsActive == true && npd.IsActive == true
                                                                                                               select new NotificationCategoryTypeViewModel
                                                                                                               {
                                                                                                                   PermissionId = npd == null ? 0 : npd.IntPermissionId,
                                                                                                                   CategoryTypeId = cateType.IntId,
                                                                                                                   TypeName = cateType.StrTypeName,
                                                                                                                   IsChecked = npd == null ? false : true
                                                                                                               }).ToList()
                                                                              }).ToList()
                                         }).ToListAsync();

            return Ok(accountNotification);
        }

        #endregion Notification Category

        #region Dashboard Permission

        [HttpGet]
        [Route("DashComponentPermissionLanding")]
        public async Task<IActionResult> DashComponentPermissionLanding()
        {
            List<ManagementDashboardPermissionViewModel> DashboardPermissions = new List<ManagementDashboardPermissionViewModel>();

            DashboardPermissions = await (from acc in _context.Accounts
                                          where acc.IsActive == true
                                          select new ManagementDashboardPermissionViewModel
                                          {
                                              IntAccountid = acc.IntAccountId,
                                              AccountName = acc.StrAccountName,
                                              dashboardComponentViewModels = (from com in _context.MasterDashboardComponents
                                                                              join cp in _context.MasterDashboardComponentPermissions on com.IntId equals cp.IntDashboardComponentId
                                                                              where cp.IntAccountId == acc.IntAccountId && com.IsActive == true && cp.IsActive == true
                                                                              select new DashboardComponentViewModel
                                                                              {
                                                                                  intDashboardComponentId = cp.IntDashboardComponentId,
                                                                                  DashboardComponentName = com.StrName,
                                                                                  strHashCode = cp.StrHashCode,
                                                                                  IsChecked = cp == null ? false : true
                                                                              }).ToList()
                                          }).ToListAsync();

            return Ok(DashboardPermissions);
        }

        [HttpGet]
        [Route("DashComponentPermissionByAccount")]
        public async Task<IActionResult> DashComponentPermissionByAccount(long IntAccountId)
        {
            List<DashboardComponentViewModel> DashboardPermission = new List<DashboardComponentViewModel>();

            List<MasterDashboardComponent> dashboardComponentList = await _context.MasterDashboardComponents.Where(x => x.IsActive == true).ToListAsync();
            List<MasterDashboardComponentPermission> dashboardComponentPermissionList = await _context.MasterDashboardComponentPermissions.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId).ToListAsync();

            dashboardComponentList.ForEach(x =>
            {
                MasterDashboardComponentPermission componentPermission = dashboardComponentPermissionList.Where(s => s.IntDashboardComponentId == x.IntId).FirstOrDefault();

                DashboardPermission.Add(new DashboardComponentViewModel
                {
                    intDashboardComponentId = x.IntId,
                    DashboardComponentName = x.StrName,
                    strHashCode = x.StrHashCode,
                    IsChecked = componentPermission == null ? false : true
                });
            });

            return Ok(DashboardPermission);
        }

        [HttpPost]
        [Route("DashComponentPermissionCRUD")]
        public async Task<IActionResult> DashComponentPermissionCRUD(ManagementDashboardPermissionViewModel dashboardPermission)
        {
            try
            {
                MasterDashboardComponentPermission dashboardComponentPermission = new MasterDashboardComponentPermission();
                List<MasterDashboardComponentPermission> dashboardComponentPermissionList = new List<MasterDashboardComponentPermission>();

                List<MasterDashboardComponentPermission> existsPermission = _context.MasterDashboardComponentPermissions.Where(x => x.IsActive == true && x.IntAccountId == dashboardPermission.IntAccountid)
                    .Select(x => new MasterDashboardComponentPermission
                    {
                        IntId = x.IntId,
                        IntAccountId = x.IntAccountId,
                        StrAccountName = dashboardPermission.AccountName,
                        IntDashboardComponentId = x.IntDashboardComponentId,
                        StrHashCode = x.StrHashCode,
                        IsActive = false,
                        IntCreatedBy = x.IntCreatedBy,
                        DteCreatedAt = x.DteCreatedAt,
                        IntUpdatedBy = dashboardPermission.IntCreateBy,
                        DteUpdatedAt = DateTime.Now
                    }).ToList();
                _context.MasterDashboardComponentPermissions.UpdateRange(existsPermission);
                await _context.SaveChangesAsync();

                foreach (var item in dashboardPermission.dashboardComponentViewModels)
                {
                    if (item.IsChecked == true)
                    {
                        dashboardComponentPermission = new MasterDashboardComponentPermission
                        {
                            IntAccountId = dashboardPermission.IntAccountid,
                            StrAccountName = dashboardPermission.AccountName,
                            IntDashboardComponentId = item.intDashboardComponentId,
                            StrHashCode = item.strHashCode,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = (long)dashboardPermission.IntCreateBy
                        };
                        dashboardComponentPermissionList.Add(dashboardComponentPermission);
                    }
                }
                await _context.MasterDashboardComponentPermissions.AddRangeAsync(dashboardComponentPermissionList);
                await _context.SaveChangesAsync();

                MessageHelper message = new MessageHelper();
                message.Message = "Save Successfully.";
                message.StatusCode = 200;
                return Ok(message);
            }
            catch (Exception ex)
            {
                MessageHelper message = new MessageHelper();
                message.Message = ex.Message;
                message.StatusCode = 500;
                return Ok(message);
            }
        }

        #endregion Dashboard Permission

        #region Account Bank Details

        [HttpGet]
        [Route("AccountBankDetailsLanding")]
        public async Task<IActionResult> AccountBankDetailsLanding(long IntAccountId, long? IntBusinessUnitId)
        {
            try
            {
                //List<AccountBankDetail> CompanyBank = await _context.AccountBankDetails.Where(x => x.IsActive == true && x.IntAccountId == IntAccountId && (IntBusinessUnitId == null || x.IntBusinessUnitId == IntBusinessUnitId)).ToListAsync();

                IEnumerable<AccountBankDetailViewModel> CompanyAccount = await _context.AccountBankDetails.Where(x => x.IsActive == true
                && x.IntAccountId == IntAccountId && (IntBusinessUnitId == null || x.IntBusinessUnitId == 0 || x.IntBusinessUnitId == IntBusinessUnitId))
                    .Select(x => new AccountBankDetailViewModel
                    {
                        IntAccountBankDetailsId = x.IntAccountBankDetailsId,
                        IntAccountId = x.IntAccountId,
                        intBusinessUnitId = x.IntBusinessUnitId,
                        StrBusinessUnitName = x.IntBusinessUnitId == 0 ? "All" : _context.MasterBusinessUnits.Where(s => s.IntBusinessUnitId == x.IntBusinessUnitId).Select(s => s.StrBusinessUnit).FirstOrDefault(),
                        IntBankOrWalletType = x.IntBankOrWalletType,
                        IntBankWalletId = x.IntBankWalletId,
                        StrBankWalletName = x.StrBankWalletName,
                        StrDistrict = x.StrDistrict,
                        IntBankBranchId = x.IntBankBranchId,
                        StrBranchName = x.StrBranchName,
                        StrRoutingNo = x.StrRoutingNo,
                        StrSwiftCode = x.StrSwiftCode,
                        StrAccountName = x.StrAccountName,
                        StrAccountNo = x.StrAccountNo,
                        IsActive = x.IsActive,
                    }).OrderByDescending(x => x.IntAccountBankDetailsId).AsNoTracking().AsQueryable().ToListAsync();

                return Ok(CompanyAccount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("AccountBankDetailsById")]
        public async Task<IActionResult> AccountBankDetailsById(long intAccountBankDetailsId)
        {
            try
            {
                AccountBankDetail CompanyBank = await _context.AccountBankDetails.Where(x => x.IsActive == true && x.IntAccountBankDetailsId == intAccountBankDetailsId).FirstOrDefaultAsync();
                return Ok(CompanyBank);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("AccountBankDetailsCRUD")]
        public async Task<IActionResult> AccountBankDetailsCRUD(AccountBankDetailViewModel model)
        {
            try
            {
                MessageHelper message = new MessageHelper();

                if (model.IntAccountBankDetailsId == 0)
                {
                    if (_context.AccountBankDetails.Where(x => x.IsActive == true && x.IntAccountId == model.IntAccountId
                    && x.IntBankWalletId == model.IntBankWalletId && x.StrAccountNo == model.StrAccountNo).Count() > 0)
                    {
                        message.Message = "Already Exist this Account No";
                        message.StatusCode = 400;
                    }
                    else
                    {
                        if(model.intBusinessUnitId==0)
                        {
                            var blist = _context.MasterBusinessUnits.Where( x => x.IsActive == true).Select(y=>y.IntBusinessUnitId).ToList();
                            foreach(var item in blist)
                            {
                                AccountBankDetail bankDetail = new AccountBankDetail
                                {
                                    IntAccountId = model.IntAccountId,
                                    IntBusinessUnitId = item,
                                    IntBankOrWalletType = (long)model.IntBankOrWalletType,
                                    IntBankWalletId = (long)model.IntBankWalletId,
                                    StrBankWalletName = model.StrBankWalletName,
                                    StrDistrict = model.StrDistrict,
                                    IntBankBranchId = model.IntBankBranchId,
                                    StrBranchName = model.StrBranchName,
                                    StrRoutingNo = model.StrRoutingNo,
                                    StrSwiftCode = model.StrSwiftCode,
                                    StrAccountName = model.StrAccountName,
                                    StrAccountNo = model.StrAccountNo,
                                    IsActive = true,
                                    DteCreatedAt = DateTime.Now,
                                    IntCreatedBy = model.IntCreatedBy,
                                };
                                await _context.AccountBankDetails.AddAsync(bankDetail);
                            }
                           
                        }
                        else
                        {
                            AccountBankDetail bankDetail = new AccountBankDetail
                            {
                                IntAccountId = model.IntAccountId,
                                IntBusinessUnitId = model.intBusinessUnitId,
                                IntBankOrWalletType = (long)model.IntBankOrWalletType,
                                IntBankWalletId = (long)model.IntBankWalletId,
                                StrBankWalletName = model.StrBankWalletName,
                                StrDistrict = model.StrDistrict,
                                IntBankBranchId = model.IntBankBranchId,
                                StrBranchName = model.StrBranchName,
                                StrRoutingNo = model.StrRoutingNo,
                                StrSwiftCode = model.StrSwiftCode,
                                StrAccountName = model.StrAccountName,
                                StrAccountNo = model.StrAccountNo,
                                IsActive = true,
                                DteCreatedAt = DateTime.Now,
                                IntCreatedBy = model.IntCreatedBy,
                            };
                            await _context.AccountBankDetails.AddAsync(bankDetail);
                        }
                      
                        await _context.SaveChangesAsync();

                        message.Message = "CREATE SUCCESSFULLY";
                        message.StatusCode = 200;
                    }
                }
                else
                {
                    AccountBankDetail accBank = _context.AccountBankDetails.Where(x => x.IntAccountBankDetailsId == model.IntAccountBankDetailsId).FirstOrDefault();

                    accBank.IntAccountBankDetailsId = model.IntAccountBankDetailsId;
                    accBank.IntAccountId = model.IntAccountId;
                    accBank.IntBusinessUnitId = model.intBusinessUnitId;
                    accBank.IntBankOrWalletType = (long)model.IntBankOrWalletType;
                    accBank.IntBankWalletId = (long)model.IntBankWalletId;
                    accBank.StrBankWalletName = model.StrBankWalletName;
                    accBank.StrDistrict = model.StrDistrict;
                    accBank.IntBankBranchId = model.IntBankBranchId;
                    accBank.StrBranchName = model.StrBranchName;
                    accBank.StrRoutingNo = model.StrRoutingNo;
                    accBank.StrSwiftCode = model.StrSwiftCode;
                    accBank.StrAccountName = model.StrAccountName;
                    accBank.StrAccountNo = model.StrAccountNo;
                    accBank.IsActive = model.IsActive;
                    accBank.DteCreatedAt = accBank.DteCreatedAt;
                    accBank.IntCreatedBy = accBank.IntCreatedBy;
                    accBank.DteUpdatedAt = DateTime.Now;
                    accBank.IntUpdatedBy = model.IntCreatedBy;

                    _context.AccountBankDetails.Update(accBank);
                    await _context.SaveChangesAsync();

                    //_context.Entry(Accountbank).State = EntityState.Modified;
                    //_context.SaveChanges();

                    message.Message = "UPDATE SUCCESSFULLY";
                    message.StatusCode = 200;
                }

                return Ok(message);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion Account Bank Details

        #region ============ EmpExpense Type ==============

        [HttpPost]
        [Route("SaveEmpExpenseType")]
        public async Task<IActionResult> SaveEmpExpenseType(EmpExpenseType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                if (await _context.EmpExpenseTypes.Where(x => x.StrExpenseType.ToLower() == model.StrExpenseType.ToLower()
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true && model.IntExpenseTypeId == 0).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    EmpExpenseType expenseType = await _context.EmpExpenseTypes.FirstOrDefaultAsync(x => x.StrExpenseType.ToLower() == model.StrExpenseType.ToLower() && x.IntAccountId == model.IntAccountId);

                    bool isCreate = model.IntExpenseTypeId > 0 ? false : true;

                    bool response = false;

                    if (model.IntExpenseTypeId > 0 && model.IsActive == false && _context.EmpExpenseApplications.Where(x => x.IntExpenseTypeId == model.IntExpenseTypeId && x.IntAccountId == model.IntAccountId && x.IsActive == true).Count() > 0)
                    {
                        res.StatusCode = 500;
                        res.Message = "This type of Expense type already exists in Expense Application.";
                        return BadRequest(res);
                    }
                    else
                    {
                        response = await _saasMasterService.SaveExpenseType(model);
                    }

                    if (response)
                    {
                        if (model.IsActive == false && model.IntExpenseTypeId > 0)
                        {
                            res.StatusCode = 200;
                            res.Message = "Deleted Successfully !!!";
                        }
                        else if (model.IntExpenseTypeId > 0 && isCreate == false)
                        {
                            res.StatusCode = 200;
                            res.Message = "Updated Successfully !!!";
                        }
                        else
                        {
                            res.StatusCode = 200;
                            res.Message = "Created Successfully !!!";
                        }
                    }
                    else
                    {
                        res.StatusCode = 500;
                        res.Message = "Internal Server Error !!!";
                    }

                    if (res.StatusCode == 500)
                    {
                        return BadRequest(res);
                    }
                    else { return Ok(res); }
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
        [Route("GetAllEmpExpenseType")]
        public async Task<IActionResult> GetAllEmpExpenseType(long IntAccountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllEmpExpenseType(IntAccountId));
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
        [Route("GetEmpExpenseTypeById")]
        public async Task<IActionResult> GetEmpExpenseTypeById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetEmpExpenseTypeById(id));
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

        [HttpPut]
        [Route("DeleteEmpExpense")]
        public async Task<IActionResult> DeleteEmpExpense(long id)
        {
            try
            {
                MessageHelperCreate res = new MessageHelperCreate();

                await _saasMasterService.DeleteEmpExpense(id);
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

        #endregion ============ EmpExpense Type ==============

        #region ============ Tax Challan Config ==============
        [HttpPost("SaveTaxChallanConfig")]
        public async Task<IActionResult> SaveTaxChallanConfigData(MasterTaxChallanConfig obj)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _saasMasterService.SaveMasterTaxChallanConfig(obj);
                return Ok(res);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet("GetTaxchallanConfigById")]
        public async Task<IActionResult> GetTaxchallanConfigById(long id)
        {
            try
            {
                return Ok(await _saasMasterService.GetTaxChallanConfigById(id));
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("GetAllTaxchallanConfig")]
        public async Task<IActionResult> GetAllMasterTaxchallanConfig(long intAccountId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllMasterTaxchallanConfig(intAccountId));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region ====== Territory Type ========

        [HttpPost]
        [Route("SaveTerritoryType")]
        public async Task<IActionResult> SaveTerritoryType(MasterTerritoryType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            MessageHelperUpdate resUpdate = new MessageHelperUpdate();

            try
            {
                if (await _context.MasterTerritoryTypes.Where(x => x.StrTerritoryType.ToLower() == model.StrTerritoryType.ToLower()
                    && x.IntTerritoryTypeId != model.IntTerritoryTypeId
                    && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId
                    && x.IntAccountId == model.IntAccountId && x.IsActive == true).CountAsync() > 0)
                {
                    res.StatusCode = 401;
                    res.Message = "Already Exists This Name !!!";
                    return BadRequest(res);
                }
                else
                {
                    MasterTerritoryType ret = await _saasMasterService.SaveTerritoryType(model);
                    if (model.IntTerritoryTypeId > 0)
                    {
                        resUpdate.AutoId = ret.IntTerritoryTypeId;
                        return Ok(resUpdate);
                    }
                    else
                    {
                        res.AutoId = ret.IntTerritoryTypeId;
                        return Ok(res);
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
        [Route("GetAllTerritoryType")]
        public async Task<IActionResult> GetAllTerritoryType(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _saasMasterService.GetAllTerritoryType(accountId, businessUnitId));
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

        //[HttpGet]
        //[Route("GetTerritoryTypeById")]
        //public async Task<IActionResult> GetTerritoryTypeById(long id)
        //{
        //    try
        //    {
        //        return Ok(await _saasMasterService.GetTerritoryTypeById(id));
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

        #endregion

        #region ====== Territory Setup ========

        [HttpPost]
        [Route("SaveTerritorySetup")]
        public async Task<IActionResult> SaveTerritorySetup(TerritorySetupViewModel model)
        {
            MessageHelper message = new MessageHelper();
            try
            {
                if (model.TerritoryId > 0)
                {
                    TerritorySetup territorySetup = await _context.TerritorySetups.Where(x => x.IntTerritoryId == model.TerritoryId).FirstOrDefaultAsync();

                    territorySetup.IntTerritoryTypeId = model.TerritoryTypeId;
                    territorySetup.StrTerritoryAddress = model.TerritoryAddress;
                    territorySetup.StrTerritoryName = model.TerritoryName;
                    territorySetup.StrTerritoryCode = model.TerritoryCode;
                    territorySetup.IntParentTerritoryId = model.ParentTerritoryId;
                    territorySetup.IntWorkplaceGroupId = model.WorkplaceGroupId;
                    territorySetup.IntHrPositionId = model.HrPositionId;
                    territorySetup.IntAccountId = model.AccountId;
                    territorySetup.IntBusinessUnitId = model.BusinessUnitId;
                    territorySetup.IntUpdateBy = model.CreatedBy;
                    territorySetup.DteUpdateBy = DateTime.Now;
                    territorySetup.IsActive = model.IsActive;
                    territorySetup.StrRemarks = model.Remarks;

                    _context.TerritorySetups.Update(territorySetup);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Territory Setup Successfully updated.";
                }
                else
                {
                    TerritorySetup territorySetup = new TerritorySetup()
                    {
                        IntTerritoryTypeId = model.TerritoryTypeId,
                        StrTerritoryAddress = model.TerritoryAddress,
                        StrTerritoryName = model.TerritoryName,
                        StrTerritoryCode = model.TerritoryCode,
                        IntParentTerritoryId = model.ParentTerritoryId,
                        IntWorkplaceGroupId = model.WorkplaceGroupId,
                        IntHrPositionId = model.HrPositionId,
                        IntAccountId = model.AccountId,
                        IntBusinessUnitId = model.BusinessUnitId,
                        IntCreatedBy = model.CreatedBy,
                        DteCreatedAt = DateTime.Now,
                        IsActive = true,
                        StrRemarks = model.Remarks
                    };
                    _context.TerritorySetups.Add(territorySetup);
                    await _context.SaveChangesAsync();

                    message.StatusCode = 200;
                    message.Message = "Territory Setup Successfully save.";
                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetTerritorySetup")]
        public async Task<IActionResult> GetTerritorySetup(long AccountId, long BusinessUnitId)
        {
            try
            {
                List<TerritorySetupViewModel> territorySetupList = await (from ts in _context.TerritorySetups
                                                                          where ts.IsActive == true
                                                                          && ts.IntAccountId == AccountId
                                                                          && ts.IntBusinessUnitId == BusinessUnitId
                                                                          select new TerritorySetupViewModel()
                                                                          {
                                                                              TerritoryId = ts.IntTerritoryId,
                                                                              TerritoryName = ts.StrTerritoryName,
                                                                              TerritoryCode = ts.StrTerritoryCode,
                                                                              TerritoryAddress = ts.StrTerritoryAddress,
                                                                              ParentTerritoryId = ts.IntParentTerritoryId,
                                                                              TerritoryTypeId = ts.IntTerritoryTypeId,
                                                                              WorkplaceGroupId = (long)ts.IntWorkplaceGroupId,
                                                                              AccountId = ts.IntAccountId,
                                                                              BusinessUnitId = ts.IntBusinessUnitId
                                                                          }).ToListAsync();

                return Ok(territorySetupList);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetTerritoryLandingTableView")]
        public async Task<IActionResult> GetTerritoryLandingTableView(long AccountId, long BusinessUnitId, long? WorkplaceGroupId, long? ParentTerritoryId, long? TerritoryTypeId)
        {
            try
            {
                List<TerritorySetupTableViewModel> record = await (from territory in _context.TerritorySetups
                                                                   join terryType in _context.OrganizationTypes on territory.IntTerritoryTypeId equals terryType.IntOrganizationTypeId

                                                                   join parentTerritory in _context.TerritorySetups on territory.IntParentTerritoryId equals parentTerritory.IntTerritoryId into parentTerritory1
                                                                   from parentTerry in parentTerritory1.DefaultIfEmpty()
                                                                   join parentTerryType1 in _context.OrganizationTypes on parentTerry.IntTerritoryTypeId equals parentTerryType1.IntOrganizationTypeId into parentTerryType2
                                                                   from parentTerryType in parentTerryType2.DefaultIfEmpty()

                                                                   where territory.IntAccountId == AccountId && territory.IntBusinessUnitId == BusinessUnitId && territory.IsActive == true
                                                                   && (WorkplaceGroupId == null || WorkplaceGroupId == 0 || territory.IntWorkplaceGroupId == WorkplaceGroupId)
                                                                   && (TerritoryTypeId == null || TerritoryTypeId == 0 || terryType.IntOrganizationTypeId == TerritoryTypeId)
                                                                   && (ParentTerritoryId == null || ParentTerritoryId == 0 || parentTerry.IntTerritoryId == ParentTerritoryId)
                                                                   select new TerritorySetupTableViewModel
                                                                   {
                                                                       TerritoryId = territory.IntTerritoryId,
                                                                       TerritoryName = territory.StrTerritoryName,
                                                                       AccountId = territory.IntAccountId,
                                                                       BusinessUnitId = territory.IntBusinessUnitId,
                                                                       WorkplaceGroupId = territory.IntWorkplaceGroupId,
                                                                       HrPositionId = territory.IntHrPositionId,
                                                                       TerritoryCode = territory.StrTerritoryCode,
                                                                       TerritoryAddress = territory.StrTerritoryAddress,
                                                                       TerritoryTypeId = territory.IntTerritoryTypeId,
                                                                       TerritoryType = terryType != null ? terryType.StrOrganizationTypeName : "",
                                                                       ParentTerritoryId = parentTerry != null ? parentTerry.IntTerritoryId : 0,
                                                                       ParentTerritory = parentTerry != null ? parentTerry.StrTerritoryName : "",
                                                                       ParentTerritoryTypeId = parentTerry != null ? parentTerry.IntTerritoryTypeId : 0,
                                                                       ParentTerritoryType = parentTerryType != null ? parentTerryType.StrOrganizationTypeName : "",
                                                                       Remarks = territory.StrRemarks
                                                                   }).ToListAsync();
                return Ok(record);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion


    }
}