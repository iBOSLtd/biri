using DocumentFormat.OpenXml.Wordprocessing;
using LanguageExt.UnitsOfMeasure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Auth;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.Jwt;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalPipelineController : ControllerBase
    {
        private readonly PeopleDeskContext _context;
        private readonly IApprovalPipelineService _approvalPipelineService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly IEmployeeService _employeeService;

        public ApprovalPipelineController(IAuthService _authService, PeopleDeskContext context, IApprovalPipelineService _approvalPipelineService, ITokenService _tokenService, IEmployeeService employeeService)
        {
            _context = context;
            this._approvalPipelineService = _approvalPipelineService;
            this._tokenService = _tokenService;
            this._authService = _authService;
            _employeeService = employeeService;
        }

        #region ========== P I P E L I N E === C O N G I G U R A T I O N ========

        [HttpGet]
        [Route("PipelineHistoryByApplicationId")]
        public async Task<IActionResult> PipelineHistoryByApplicationId(long? pipelineHeaderId, string? applictionType, long? currentStage, long? nextStage)
        {
            try
            {
                if ((pipelineHeaderId <= 0 || pipelineHeaderId is null) && !string.IsNullOrEmpty(applictionType))
                {
                    pipelineHeaderId = _context.GlobalPipelineHeaders.FirstOrDefault(x => x.StrApplicationType.ToLower() == applictionType.ToLower()).IntPipelineHeaderId;
                }

                List<ApprovalPipelineHeaderViewModel> headerData = await (from h in _context.GlobalPipelineHeaders
                                                                          where h.IntPipelineHeaderId == pipelineHeaderId
                                                                          select new ApprovalPipelineHeaderViewModel
                                                                          {
                                                                              IntPipelineHeaderId = pipelineHeaderId,
                                                                              StrPipelineName = h.StrPipelineName,
                                                                              StrApplicationType = h.StrApplicationType,
                                                                              StrRemarks = h.StrRemarks,
                                                                              IntAccountId = h.IntAccountId,
                                                                              ApprovalPipelineRowViewModelList = (from r in _context.GlobalPipelineRows
                                                                                                                  where r.IntPipelineHeaderId == h.IntPipelineHeaderId
                                                                                                                  orderby r.IntPipelineHeaderId
                                                                                                                  select new ApprovalPipelineRowViewModel
                                                                                                                  {
                                                                                                                      IntPipelineHeaderId = r.IntPipelineHeaderId,
                                                                                                                      IntPipelineRowId = r.IntPipelineRowId,
                                                                                                                      IsSupervisor = r.IsSupervisor,
                                                                                                                      IsLineManager = r.IsLineManager,
                                                                                                                      IntUserGroupHeaderId = r.IntUserGroupHeaderId,
                                                                                                                      IntShortOrder = r.IntShortOrder,
                                                                                                                      StrStatusTitle = r.StrStatusTitle
                                                                                                                  }).ToList()
                                                                          }).ToListAsync();

                return Ok(headerData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("ApprovalPipelineCreateNUpdate")]
        public async Task<IActionResult> ApprovalPipelineCreateNUpdate(ApprovalPipelineHeaderViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                MessageHelper messageHelper = new MessageHelper();

                ////GlobalPipelineHeader isExists = await _context.GlobalPipelineHeaders.Where(x => x.IsActive == true
                ////&& x.IntAccountId == tokenData.accountId
                ////&& (x.IntBusinessUnitId == 0 ? true : (x.IntBusinessUnitId == model.IntBusinessUnitId && (x.IntWorkplaceGroupId == 0 ? true : x.IntWorkplaceGroupId == model.IntWorkplaceGroupId
                ////&& (x.IntWorkplaceId == 0 ? true : x.IntWorkplaceId == model.IntWorkplaceId
                ////&& (x.IntWingId == 0 ? true : x.IntWingId == model.IntWingId
                ////&& (x.IntSoleDepoId == 0 ? true : x.IntSoleDepoId == model.IntSoleDepoId
                ////&& (x.IntRegionId == 0 ? true : x.IntRegionId == model.IntRegionId
                ////&& (x.IntAreaId == 0 ? true : x.IntAreaId == model.IntAreaId
                ////&& (x.IntTerritoryId == 0 ? true : x.IntTerritoryId == model.IntTerritoryId)))))))))
                ////&& x.IntPipelineHeaderId != model.IntPipelineHeaderId
                ////&& x.StrPipelineName.ToLower().Trim() == model.StrPipelineName.ToLower().Trim()).FirstOrDefaultAsync();
                ///


                var isExists = await (from item in _context.GlobalPipelineHeaders
                                      where item.IntAccountId == tokenData.accountId && item.IsActive == true
                                      && (item.IntBusinessUnitId == 0 ? true
                                      : item.IntBusinessUnitId == model.IntBusinessUnitId && item.IntBusinessUnitId != -1 && (item.IntWorkplaceGroupId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntBusinessUnitId != model.IntBusinessUnitId && x.IntWingId > -1 || x.IntSoleDepoId > -1 || x.IntRegionId > -1 || x.IntAreaId > -1 || x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntWorkplaceGroupId == model.IntWorkplaceGroupId && item.IntWorkplaceGroupId != -1 && (item.IntWingId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntWorkplaceGroupId != model.IntWorkplaceGroupId && x.IntSoleDepoId > -1 || x.IntRegionId > -1 || x.IntAreaId > -1 || x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntWingId == model.IntWingId && item.IntWingId != -1 && (item.IntSoleDepoId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntWingId != model.IntWingId && x.IntRegionId > -1 || x.IntAreaId > -1 || x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntSoleDepoId == model.IntSoleDepoId && item.IntSoleDepoId != -1 && (item.IntRegionId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntSoleDepoId != model.IntSoleDepoId && x.IntAreaId > -1 || x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntRegionId == model.IntRegionId && item.IntTerritoryId != -1 && (item.IntAreaId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntRegionId != model.IntRegionId && x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntAreaId == model.IntAreaId && item.IntRegionId != -1 && (item.IntTerritoryId == 0 && _context.GlobalPipelineHeaders.Where(x => x.IntAreaId != model.IntAreaId && x.IntTerritoryId > -1).Count() > 0 ? false : true) ? true
                                      : item.IntTerritoryId == model.IntTerritoryId && item.IntTerritoryId != -1 ? true
                                      : false)
                                      && item.IntPipelineHeaderId != model.IntPipelineHeaderId
                                      && item.StrPipelineName.ToLower().Trim() == model.StrPipelineName.ToLower().Trim()
                                      && (item.IntBusinessUnitId == 0 ? true

                                      : item.IntBusinessUnitId == model.IntBusinessUnitId && item.IntBusinessUnitId != -1 && (model.IntWorkplaceGroupId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId > -1 && (x.IntWingId > -1 || x.IntSoleDepoId > -1 || x.IntRegionId > -1
                                      || x.IntAreaId > -1 || x.IntTerritoryId > -1)).Count() > 0 ? true : false) ? true

                                      : item.IntWorkplaceGroupId == model.IntWorkplaceGroupId && item.IntWorkplaceGroupId != -1 && (model.IntWingId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && (x.IntWingId > -1 && x.IntSoleDepoId > -1
                                      || x.IntRegionId > -1 || x.IntAreaId > -1 || x.IntTerritoryId > -1)).Count() > 0 ? true : false) ? true

                                      : item.IntWingId == model.IntWingId && item.IntWingId != -1 && (model.IntSoleDepoId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && x.IntWingId == model.IntWingId
                                      && (x.IntSoleDepoId > -1 || x.IntAreaId > -1 || x.IntTerritoryId > -1)).Count() > 0 ? true : false) ? true

                                      : item.IntSoleDepoId == model.IntSoleDepoId && item.IntSoleDepoId != -1 && (model.IntRegionId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && x.IntWingId == model.IntWingId
                                      && x.IntSoleDepoId == model.IntSoleDepoId && (x.IntRegionId > -1 && x.IntAreaId > -1 || x.IntTerritoryId > -1)).Count() > 0 ? true : false) ? true

                                      : item.IntRegionId == model.IntRegionId && item.IntRegionId != -1 && (model.IntAreaId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && x.IntWingId == model.IntWingId
                                      && x.IntSoleDepoId == model.IntSoleDepoId && x.IntRegionId == model.IntRegionId && (x.IntAreaId > -1 && x.IntTerritoryId > -1)).Count() > 0 ? true : false) ? true

                                      : item.IntAreaId == model.IntAreaId && item.IntAreaId != -1 && (model.IntTerritoryId == 0 && _context.GlobalPipelineHeaders
                                      .Where(x => x.IntBusinessUnitId == model.IntBusinessUnitId && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && x.IntWingId == model.IntWingId
                                      && x.IntSoleDepoId == model.IntSoleDepoId && x.IntRegionId == model.IntRegionId && x.IntAreaId == model.IntAreaId && x.IntTerritoryId > -1).Count() > 0 ? true : false) ? true

                                      : item.IntTerritoryId == model.IntTerritoryId && item.IntTerritoryId != -1 ? true
                                      : false)

                                      select item).AsNoTracking().FirstOrDefaultAsync();


                var actualExists = await _context.GlobalPipelineHeaders.FirstOrDefaultAsync(
                                                         x => x.IntPipelineHeaderId != model.IntPipelineHeaderId && x.IntAccountId == tokenData.accountId && x.IntBusinessUnitId == model.IntBusinessUnitId
                                                         && x.IntWorkplaceGroupId == model.IntWorkplaceGroupId && x.IntWingId == model.IntWingId
                                                         && x.IntSoleDepoId == model.IntSoleDepoId && x.IntRegionId == model.IntRegionId
                                                         && x.StrApplicationType == model.StrApplicationType
                                                         && x.IntAreaId == model.IntAreaId && x.IntTerritoryId == model.IntTerritoryId);

                if ((!(model.StrApplicationType == "salaryGenerateRequest" 
                    || model.StrApplicationType == "bonusGenerate" 
                    || model.StrApplicationType == "arearSalaryGenerateRequest") && (isExists != null))
                    || actualExists != null)
                {
                    messageHelper.Message = "Pipeline Name Already Exists";
                    return BadRequest(messageHelper);
                }


                #region======= is validate part===========
                //if (model.IsValidate == true)
                //{
                //    List<GlobalPipelineRow> globalPipelineRowList = isExists == null ? new List<GlobalPipelineRow>()
                //        : await _context.GlobalPipelineRows.Where(x => x.IntPipelineHeaderId == isExists.IntPipelineHeaderId).ToListAsync();

                //    List<PipelineTypeDdl> pipelineddl = await _context.PipelineTypeDdls.ToListAsync();

                //    foreach (var p in pipelineddl)
                //    {
                //        if (p.StrApplicationType == "leave")
                //        {
                //            List<LveLeaveApplication> leaveapplications = (from item in _context.LveLeaveApplications
                //                                                           where globalPipelineRowList
                //                                                                 .Where(i => (i.IntPipelineRowId == item.IntCurrentStage
                //                                                                 || i.IntPipelineRowId == item.IntNextStage) && item.IsPipelineClosed == false).Count() > 0
                //                                                           select item).ToList();

                //            if (leaveapplications.Count > 0)
                //            {
                //                return BadRequest(leaveapplications);
                //            }
                //        }
                //        else if (p.StrApplicationType == "movement")
                //        {
                //            List<LveMovementApplication> movementapplications = (from item in _context.LveMovementApplications
                //                                                                 where globalPipelineRowList
                //                                                                       .Where(i => (i.IntPipelineRowId == item.IntCurrentStage
                //                                                                       || i.IntPipelineRowId == item.IntNextStage) && item.IsPipelineClosed == false).Count() > 0
                //                                                                 select item).ToList();

                //            if (movementapplications.Count > 0)
                //            {
                //                return BadRequest(movementapplications);
                //            }
                //        }
                //    }
                //}
                #endregion

                if (model.IntPipelineHeaderId > 0 && !string.IsNullOrEmpty(model.StrApplicationType))
                {
                    GlobalPipelineHeader pipelineHeader = await _context.GlobalPipelineHeaders.FirstOrDefaultAsync(x => x.IntPipelineHeaderId == model.IntPipelineHeaderId);
                    if (pipelineHeader == null)
                    {
                        messageHelper.StatusCode = 500;
                        messageHelper.Message = "Pipeline Not Found !!!";
                        return BadRequest(messageHelper);
                    }
                    else
                    {
                        List<GlobalPipelineRow> removeGlobalPipelineRow = new List<GlobalPipelineRow>();

                        model.ApprovalPipelineRowViewModelList.Where(x => x.IsDelete == true).ToList().ForEach(x =>
                        {
                            GlobalPipelineRow row = _context.GlobalPipelineRows.FirstOrDefault(y => y.IntPipelineRowId == x.IntPipelineRowId);
                            row.IsActive = false;
                            removeGlobalPipelineRow.Add(row);
                        });

                        List<GlobalPipelineRow> createGlobalPipelineRow = model.ApprovalPipelineRowViewModelList.Where(x => x.IsCreate == true)
                                                                            .Select(y => new GlobalPipelineRow
                                                                            {
                                                                                IntPipelineHeaderId = pipelineHeader.IntPipelineHeaderId,
                                                                                IsSupervisor = y.IsSupervisor,
                                                                                IsLineManager = y.IsLineManager,
                                                                                StrStatusTitle = y.StrStatusTitle,
                                                                                IntUserGroupHeaderId = y.IntUserGroupHeaderId,
                                                                                IntShortOrder = y.IntShortOrder,
                                                                                IsActive = true,
                                                                                IntCreatedBy = model.IntCreatedBy,
                                                                                DteCreatedAt = DateTime.Now
                                                                            }).ToList();

                        //pipelineHeader.StrApplicationType = model.StrApplicationType;
                        pipelineHeader.StrRemarks = model.StrRemarks;
                        pipelineHeader.DteUpdatedAt = DateTime.Now;
                        pipelineHeader.IntUpdatedBy = model.IntUpdatedBy;

                        _context.GlobalPipelineHeaders.Update(pipelineHeader);
                        _context.GlobalPipelineRows.UpdateRange(removeGlobalPipelineRow);
                        await _context.GlobalPipelineRows.AddRangeAsync(createGlobalPipelineRow);
                        await _context.SaveChangesAsync();

                        messageHelper.StatusCode = 200;
                        messageHelper.Message = "Update Successfully";

                        return Ok(messageHelper);
                    }
                }
                else if (!string.IsNullOrEmpty(model.StrApplicationType))
                {
                    GlobalPipelineHeader pipelineHeader = new GlobalPipelineHeader
                    {
                        StrPipelineName = model.StrPipelineName,
                        StrApplicationType = model.StrApplicationType,
                        StrRemarks = model.StrRemarks,
                        IntAccountId = (long)model.IntAccountId,
                        IntBusinessUnitId = model.IntBusinessUnitId,
                        IntWorkplaceGroupId = model.IntWorkplaceGroupId,
                        IntWorkplaceId = model.IntWorkplaceId,
                        IntWingId = model.IntWingId,
                        IntSoleDepoId = model.IntSoleDepoId,
                        IntRegionId = model.IntRegionId,
                        IntAreaId = model.IntAreaId,
                        IntTerritoryId = model.IntTerritoryId,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = model.IntCreatedBy
                    };
                    await _context.GlobalPipelineHeaders.AddAsync(pipelineHeader);
                    await _context.SaveChangesAsync();

                    List<GlobalPipelineRow> pipelineRowRows = model.ApprovalPipelineRowViewModelList.Where(x => x.IsCreate == true)
                                                                           .Select(y => new GlobalPipelineRow
                                                                           {
                                                                               IntPipelineHeaderId = pipelineHeader.IntPipelineHeaderId,
                                                                               IsSupervisor = y.IsSupervisor,
                                                                               IsLineManager = y.IsLineManager,
                                                                               IntUserGroupHeaderId = y.IntUserGroupHeaderId,
                                                                               IntShortOrder = y.IntShortOrder,
                                                                               StrStatusTitle = y.StrStatusTitle,
                                                                               IsActive = true,
                                                                               IntCreatedBy = model.IntCreatedBy,
                                                                               DteCreatedAt = DateTime.Now
                                                                           }).ToList();
                    await _context.GlobalPipelineRows.AddRangeAsync(pipelineRowRows);
                    await _context.SaveChangesAsync();

                    messageHelper.StatusCode = 200;
                    messageHelper.Message = "Create Successfully";

                    return Ok(messageHelper);
                }
                else
                {
                    messageHelper.StatusCode = 401;
                    messageHelper.Message = "Application Type Is Required !!!";
                    return BadRequest(messageHelper);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("ApprovalPipelineHeaderLanding")]
        public async Task<IActionResult> ApprovalPipelineHeaderLanding(ApprovalPipelineLandingFilterVM model)
        {
            try
            {
                var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tD.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId(tD.employeeId);



                if (userRoleList.Count() > 0)
                {
                    model.SearchTxt = !string.IsNullOrEmpty(model.SearchTxt) ? model.SearchTxt.ToLower() : model.SearchTxt;

                    var data = (from header in _context.GlobalPipelineHeaders
                                join ddl in _context.PipelineTypeDdls on header.StrApplicationType equals ddl.StrApplicationType
                                join menu in _context.Menus on ddl.StrHashCode equals menu.StrHashCode
                                join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId

                                join tr in _context.TerritorySetups on header.IntTerritoryId equals tr.IntTerritoryId into tr2
                                from terr in tr2.DefaultIfEmpty()

                                join win1 in _context.TerritorySetups on header.IntWingId equals win1.IntTerritoryId into win2
                                from win in win2.DefaultIfEmpty()

                                join sole1 in _context.TerritorySetups on header.IntSoleDepoId equals sole1.IntTerritoryId into sole2
                                from sole in sole2.DefaultIfEmpty()

                                join reg1 in _context.TerritorySetups on header.IntRegionId equals reg1.IntTerritoryId into reg2
                                from reg in reg2.DefaultIfEmpty()

                                join are1 in _context.TerritorySetups on header.IntAreaId equals are1.IntTerritoryId into are2
                                from are in are2.DefaultIfEmpty()

                                where header.IntAccountId == tD.accountId
                                && (header.IntBusinessUnitId == -1 ? false : (header.IntBusinessUnitId > 0 ? header.IntBusinessUnitId == model.BusinessUnitId : true))
                                && (header.IntWorkplaceGroupId == -1 ? false : (header.IntWorkplaceGroupId > 0 ? header.IntWorkplaceGroupId == model.WorkplaceGroupId : true))
                                && menu.IsActive == true && header.IsActive == true
                                && mpermission.IsActive == true && menu.IsHasApproval == true && (mpermission.IsForWeb == true || mpermission.IsForApps == true)
                                && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
                                    || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && mpermission.IntEmployeeOrRoleId == tD.employeeId))

                                && (!string.IsNullOrEmpty(model.SearchTxt) ? (header.StrPipelineName.ToLower().Contains(model.SearchTxt)
                                     || win.StrTerritoryName.ToLower().Contains(model.SearchTxt)
                                     || sole.StrTerritoryName.ToLower().Contains(model.SearchTxt)
                                     || reg.StrTerritoryName.ToLower().Contains(model.SearchTxt)
                                     || are.StrTerritoryName.ToLower().Contains(model.SearchTxt)
                                     || terr.StrTerritoryName.ToLower().Contains(model.SearchTxt))
                                     : true)

                                && ((model.WingNameList.Count > 0 ? model.WingNameList.Contains((long)header.IntWingId) : true)
                                    && (model.SoleDepoNameList.Count > 0 ? model.SoleDepoNameList.Contains((long)header.IntSoleDepoId) : true)
                                    && (model.RegionNameList.Count > 0 ? model.RegionNameList.Contains((long)header.IntRegionId) : true)
                                    && (model.AreaNameList.Count > 0 ? model.AreaNameList.Contains((long)header.IntAreaId) : true)
                                    && (model.TerritoryNameList.Count > 0 ? model.TerritoryNameList.Contains((long)header.IntTerritoryId) : true))

                                orderby header.IntPipelineHeaderId descending
                                select new
                                {
                                    IntPipelineHeaderId = header.IntPipelineHeaderId,
                                    StrPipelineName = header.StrPipelineName,
                                    StrApplicationType = header.StrApplicationType,
                                    StrRemarks = header.StrRemarks,
                                    IntAccountId = header.IntAccountId,
                                    IsActive = header.IsActive,
                                    DteCreatedAt = header.DteCreatedAt,
                                    IntCreatedBy = header.IntCreatedBy,
                                    DteUpdatedAt = header.DteUpdatedAt,
                                    IntUpdatedBy = header.IntUpdatedBy,
                                    TerritoryId = header.IntTerritoryId,
                                    TerritoryName = header.IntTerritoryId == -1 ? "None" : (header.IntTerritoryId == 0 ? "All " : (terr != null ? terr.StrTerritoryName : "")),
                                    WingId = header.IntWingId,
                                    WingName = header.IntWingId == -1 ? "None" : (header.IntWingId == 0 ? "All " : (win != null ? win.StrTerritoryName : "")),
                                    SoleDepoId = header.IntSoleDepoId,
                                    SoleDepoName = header.IntSoleDepoId == -1 ? "None" : (header.IntSoleDepoId == 0 ? "All " : (sole != null ? sole.StrTerritoryName : "")),
                                    AreaId = header.IntAreaId,
                                    AreaName = header.IntAreaId == -1 ? "None" : (header.IntAreaId == 0 ? "All " : (are != null ? are.StrTerritoryName : "")),
                                    RegionId = header.IntRegionId,
                                    RegionName = header.IntRegionId == -1 ? "None" : (header.IntRegionId == 0 ? "All " : (reg != null ? reg.StrTerritoryName : ""))
                                }).AsNoTracking().AsQueryable();

                    ApprovalPipelineLandingFilterWithHeader approvalPipeline = new();

                    if (model.IsHeaderNeed)
                    {
                        PipelineLandingHeader eh = new();

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

                        approvalPipeline.PipelineLandingHeader = eh;
                    }

                    if (!model.IsPaginated)
                    {
                        approvalPipeline.TotalCount = await data.CountAsync();
                        approvalPipeline.Data = await data.ToListAsync();
                    }
                    else
                    {
                        int maxSize = 1000;
                        model.PageSize = model.PageSize > maxSize ? maxSize : model.PageSize;
                        model.PageNo = model.PageNo < 1 ? 1 : model.PageNo;

                        approvalPipeline.TotalCount = await data.CountAsync();
                        approvalPipeline.Data = await data.Skip(model.PageSize * (model.PageNo - 1)).Take(model.PageSize).ToListAsync();
                        approvalPipeline.PageSize = model.PageSize;
                        approvalPipeline.CurrentPage = model.PageNo;
                    }
                    return Ok(approvalPipeline);
                }
                return BadRequest(new MessageHelper() { StatusCode = 400, Message = "User Role is not permission this employee."});
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageHelper() { StatusCode = 500, Message = ex.Message });
            }

            
        }

        [HttpGet]
        [Route("ApprovalPipelineHeaderDetailsById")]
        public async Task<IActionResult> ApprovalPipelineHeaderDetailsById(long headerId, long intBusinessUnitId, long intWorkplaceGroupId)
        {
            var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                new PayloadIsAuthorizeVM { businessUnitId = intBusinessUnitId, workplaceGroupId = intWorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

            if (!tD.isAuthorize)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }
            List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId(tD.employeeId);

            List<GlobalPipelineRowViewModel> rowList = await (from item in _context.GlobalPipelineRows
                                                              join ug1 in _context.UserGroupHeaders on item.IntUserGroupHeaderId equals ug1.IntUserGroupHeaderId into ug2
                                                              from ug in ug2.DefaultIfEmpty()
                                                              where item.IntPipelineHeaderId == headerId && item.IsActive == true
                                                              select new GlobalPipelineRowViewModel
                                                              {
                                                                  GlobalPipelineRow = item,
                                                                  UserGroupHeader = ug != null ? ug : new UserGroupHeader()
                                                              }).ToListAsync();

            var HeaderVm = await (from header in _context.GlobalPipelineHeaders
                                  join ddl in _context.PipelineTypeDdls on header.StrApplicationType equals ddl.StrApplicationType
                                  join menu in _context.Menus on ddl.StrHashCode equals menu.StrHashCode
                                  join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId

                                  join tr in _context.TerritorySetups on header.IntTerritoryId equals tr.IntTerritoryId into tr2
                                  from terr in tr2.DefaultIfEmpty()

                                  join win1 in _context.TerritorySetups on header.IntWingId equals win1.IntTerritoryId into win2
                                  from win in win2.DefaultIfEmpty()

                                  join sole1 in _context.TerritorySetups on header.IntSoleDepoId equals sole1.IntTerritoryId into sole2
                                  from sole in sole2.DefaultIfEmpty()

                                  join reg1 in _context.TerritorySetups on header.IntRegionId equals reg1.IntTerritoryId into reg2
                                  from reg in reg2.DefaultIfEmpty()

                                  join are1 in _context.TerritorySetups on header.IntAreaId equals are1.IntTerritoryId into are2
                                  from are in are2.DefaultIfEmpty()

                                  join wg1 in _context.MasterWorkplaceGroups on header.IntWorkplaceGroupId equals wg1.IntWorkplaceGroupId into wg2
                                  from wg in wg2.DefaultIfEmpty()

                                  join wp1 in _context.MasterWorkplaces on header.IntWorkplaceId equals wp1.IntWorkplaceId into wp2
                                  from wp in wp2.DefaultIfEmpty()

                                  where header.IntAccountId == tD.accountId && header.IntPipelineHeaderId == headerId
                                  && (header.IntBusinessUnitId == -1 ? false : (header.IntBusinessUnitId > 0 ? header.IntBusinessUnitId == intBusinessUnitId : true))
                                  && (header.IntWorkplaceGroupId == -1 ? false : (header.IntWorkplaceGroupId > 0 ? header.IntWorkplaceGroupId == intWorkplaceGroupId : true))
                                  && menu.IsActive == true && header.IsActive == true
                                  && mpermission.IsActive == true && menu.IsHasApproval == true && (mpermission.IsForWeb == true || mpermission.IsForApps == true)
                                  && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
                                      || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && mpermission.IntEmployeeOrRoleId == tD.employeeId))
                                  select new GlobalPipelineHeaderVM
                                  {
                                      IntPipelineHeaderId = header.IntPipelineHeaderId,
                                      StrPipelineName = header.StrPipelineName,
                                      StrApplicationType = header.StrApplicationType,
                                      StrRemarks = header.StrRemarks,
                                      IntAccountId = header.IntAccountId,
                                      IntBusinessUnitId = header.IntBusinessUnitId,
                                      IntWorkplaceGroupId = header.IntWorkplaceGroupId,
                                      IntWorkplaceId = header.IntWorkplaceId,
                                      IntWingId = header.IntWingId,
                                      IntSoleDepoId = header.IntSoleDepoId,
                                      IntRegionId = header.IntRegionId,
                                      IntAreaId = header.IntAreaId,
                                      IntTerritoryId = header.IntTerritoryId,
                                      IsActive = header.IsActive,
                                      DteCreatedAt = header.DteCreatedAt,
                                      IntCreatedBy = header.IntCreatedBy,
                                      DteUpdatedAt = header.DteUpdatedAt,
                                      IntUpdatedBy = header.IntUpdatedBy,
                                      strWorkPlaceGroupName = header.IntWorkplaceGroupId == -1 ? "None" : (header.IntWorkplaceGroupId == 0 ? "All " : (wg != null ? wg.StrWorkplaceGroup : "")),
                                      strWorkPlaceName = header.IntWorkplaceId == -1 ? "None" : (header.IntWorkplaceId == 0 ? "All " : (wp != null ? wp.StrWorkplace : "")),
                                      Territory = header.IntTerritoryId == -1 ? "None" : (header.IntTerritoryId == 0 ? "All " : (terr != null ? terr.StrTerritoryName : "")),
                                      Wing = header.IntWingId == -1 ? "None" : (header.IntWingId == 0 ? "All " : (win != null ? win.StrTerritoryName : "")),
                                      SoleDepo = header.IntSoleDepoId == -1 ? "None" : (header.IntSoleDepoId == 0 ? "All " : (sole != null ? sole.StrTerritoryName : "")),
                                      Area = header.IntAreaId == -1 ? "None" : (header.IntAreaId == 0 ? "All " : (are != null ? are.StrTerritoryName : "")),
                                      Reagion = header.IntRegionId == -1 ? "None" : (header.IntRegionId == 0 ? "All " : (reg != null ? reg.StrTerritoryName : ""))

                                  }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            ApprovalPipelineViewModel data = new ApprovalPipelineViewModel
            {
                GlobalPipelineHeader = HeaderVm,
                GlobalPipelineRowList = rowList
            };

            return Ok(data);
        }
        [HttpGet]
        [Route("ApprovalPipelineDDL")]
        public async Task<IActionResult> ApprovalPipelineDDL(long employeeId)
        {
            List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId(employeeId);
            List<PipelineTypeDdl> filteredData = new List<PipelineTypeDdl>();

            if (userRoleList.Count() > 0)
            {
                List<PipelineTypeDdl> data = await (from ddl in _context.PipelineTypeDdls
                                                    join menu in _context.Menus on ddl.StrHashCode equals menu.StrHashCode
                                                    join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId
                                                    where menu.IsActive == true && mpermission.IsActive == true && (mpermission.IsForWeb == true || mpermission.IsForApps == true)
                                                    && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
                                                        || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && mpermission.IntEmployeeOrRoleId == employeeId))
                                                    select ddl).Distinct().ToListAsync();

                data.ForEach(item =>
                {
                    if (!filteredData.Select(x => x.IntId).Contains(item.IntId))
                    {
                        filteredData.Add(item);
                    }
                });

                return Ok(filteredData.OrderBy(x => x.StrDisplayName).ToList());
            }
            else
            {
                return Ok(filteredData);
            }
        }

        //[HttpGet]
        //[Route("ApprovalDashboardLanding")]
        //public async Task<IActionResult> ApprovalDashboardLanding(long employeeId)
        //{
        //    List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId(employeeId);
        //    List<Menu> filteredData = new List<Menu>();

        //    if (userRoleList.Count() > 0)
        //    {
        //        List<Menu> data = await (from menu in _context.Menus
        //                                 join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId
        //                                 where menu.IsHasApproval == true && menu.IsActive == true && mpermission.IsForWeb == true
        //                                 && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
        //                                        || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && mpermission.IntEmployeeOrRoleId == employeeId))
        //                                 select menu).OrderBy(x => x.StrMenuName).Distinct().ToListAsync();

        //        data.ForEach(item =>
        //        {
        //            if (!filteredData.Select(x => x.IntMenuId).Contains(item.IntMenuId))
        //            {
        //                filteredData.Add(item);
        //            }
        //        });

        //        return Ok(filteredData.OrderBy(x => x.StrMenuName).ToList());
        //    }
        //    else
        //    {
        //        return Ok(filteredData);
        //    }
        //}

        [HttpGet]
        [Route("ApprovalDashboardLandingForApps")]
        public async Task<IActionResult> ApprovalDashboardLandingForApps(long accountId, long employeeId)
        {
            var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
            if (tokenData.accountId == -1)
            {
                return BadRequest(new MessageHelperAccessDenied());
            }
            Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.IntAccountId == accountId);
            if (account == null)
            {
                return BadRequest("invalid account");
            }

            Data.Entity.User parentUser = await _context.Users.FirstOrDefaultAsync(x => x.IntAccountId == accountId && x.IntRefferenceId == account.IntOwnerEmployeeId && x.IsActive == true);

            if (account == null || parentUser == null)
            {
                return BadRequest("invalid parent user");
            }
            else
            {
                List<RoleValuLabelVM> userRoleList = await _authService.GetAllAssignedRoleByEmployeeId((long)parentUser.IntRefferenceId);
                List<Menu> filteredData = new List<Menu>();

                if (userRoleList.Count() > 0)
                {
                    List<Menu> data = await (from menu in _context.Menus
                                             join mpermission in _context.MenuPermissions on menu.IntMenuId equals mpermission.IntMenuId
                                             where menu.IsHasApproval == true && menu.IsActive == true && mpermission.IsForApps == true
                                             && ((mpermission.StrIsFor.ToLower() == "Role".ToLower() && userRoleList.Select(x => x.Value).Contains(mpermission.IntEmployeeOrRoleId))
                                                    || (mpermission.StrIsFor.ToLower() == "Employee".ToLower() && mpermission.IntEmployeeOrRoleId == (long)parentUser.IntRefferenceId))
                                             select menu).Distinct().ToListAsync();

                    data.ForEach(item =>
                    {
                        if (!filteredData.Select(x => x.IntMenuId).Contains(item.IntMenuId))
                        {
                            filteredData.Add(item);
                        }
                    });
                    return Ok(filteredData.OrderBy(x => x.StrMenuName).ToList());
                }
                else
                {
                    return Ok(filteredData);
                }
            }
        }

        #endregion ========== P I P E L I N E === C O N G I G U R A T I O N ========

        #region ============== A P L I C A T I O N ===================

        //[HttpPost]
        //[Route("LeaveApplicationLanding")]
        //public async Task<IActionResult> LeaveApplicationLanding(LeaveApplicationLandingViewModel model)
        //{
        //    try
        //    {
        //        var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
        //        if (tokenData.accountId == -1)
        //        {
        //            return BadRequest(new MessageHelperAccessDenied());
        //        }
        //        model.AccountId = tokenData.accountId;
        //        model.IsAdmin = tokenData.isOfficeAdmin;
        //        model.ApproverId = tokenData.employeeId;

        //        LeaveApprovalResponse response = await _approvalPipelineService.LeaveLandingEngine(model);

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}


        [HttpPost]
        [Route("LeaveApplicationLanding")]
        public async Task<IActionResult> LeaveApplicationLanding(LeaveApplicationLandingRequestVM model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.LeaveLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("LeaveApplicationApproval")]
        public async Task<IActionResult> LeaveApplicationApproval(List<LeaveAndMovementApprovedDTO> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string response = "Error";

                foreach (LeaveAndMovementApprovedDTO item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    LveLeaveApplication application = await _context.LveLeaveApplications.AsNoTracking().FirstOrDefaultAsync(x => x.IntApplicationId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            LeaveApprovalResponseVM result = new LeaveApprovalResponseVM();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.LeaveApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                    await _approvalPipelineService.LeaveBalanceAndAttendanceUpdateAfterLeaveApproved(application);
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.LveLeaveApplications.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                response = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                response = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }
                return Ok(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("MovementApplicationLanding")]
        public async Task<IActionResult> MovementApplicationLanding(MovementApplicationLandingRequestVM model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }


                return Ok(await _approvalPipelineService.MovementLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex.Message);
            }
        }

        [HttpPost]
        [Route("MovementApplicationApproval")]
        public async Task<IActionResult> MovementApplicationApproval(List<LeaveAndMovementApprovedDTO> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (LeaveAndMovementApprovedDTO item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    LveMovementApplication application = await _context.LveMovementApplications.FirstOrDefaultAsync(x => x.IntApplicationId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            MovementApprovalResponse result = new MovementApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.MovementApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                    await _approvalPipelineService.AttendanceSummaryUpdateAfterMovementApproved(application);
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }

                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.LveMovementApplications.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Failed";
                            }
                        }
                    }
                }
                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("RemoteAttendanceLocationNDeviceLanding")]
        public async Task<IActionResult> RemoteAttendanceLocationNDeviceLanding(RemoteAttendanceLocationNDeviceLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.RemoteAttendanceLocationNDeviceLandingEngine(model, tokenData));

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("RemoteAttendanceLocationNDeviceApproval")]
        public async Task<IActionResult> RemoteAttendanceLocationNDeviceApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TimeRemoteAttendanceRegistration application = await _context.TimeRemoteAttendanceRegistrations.FirstOrDefaultAsync(x => x.IntAttendanceRegId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            RemoteAttendanceLocationNDeviceApprovalResponse result = new RemoteAttendanceLocationNDeviceApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.RemoteAttendanceLocationNDeviceApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.TimeRemoteAttendanceRegistrations.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpPost]
        [Route("RemoteAttendanceLanding")]
        public async Task<IActionResult> RemoteAttendanceLanding(RemoteAttendanceLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)model.BusinessUnitId, workplaceGroupId = (long)model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                bool IsMarket = false;
                ApplicationLandingVM response = await _approvalPipelineService.RemoteAttendanceLandingEngine(model, tokenData, IsMarket);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        //[HttpPost]
        //[Route("RemoteAttendanceLanding")]
        //public async Task<IActionResult> RemoteAttendanceLanding(RemoteAttendanceLandingViewModel model)
        //{
        //    try
        //    {
        //        var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
        //        if (tokenData.accountId == -1)
        //        {
        //            return BadRequest(new MessageHelperAccessDenied());
        //        }
        //        model.AccountId = tokenData.accountId;
        //        model.IsAdmin = tokenData.isOfficeAdmin;
        //        model.ApproverId = tokenData.employeeId;

        //        bool IsMarket = false;
        //        RemoteAttendanceApprovalResponse response = await _approvalPipelineService.RemoteAttendanceLandingEngine(model, IsMarket);

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.CatchProcess());
        //    }
        //}

        [HttpPost]
        [Route("RemoteAttendanceApproval")]
        public async Task<IActionResult> RemoteAttendanceApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TimeEmployeeAttendance application = await _context.TimeEmployeeAttendances.FirstOrDefaultAsync(x => x.IntRemoteAttendanceId == item.ApplicationId);

                    if (application.IsPipelineClosed == false)
                    {
                        if (application != null)
                        {
                            RemoteAttendanceApprovalResponse result = new RemoteAttendanceApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.RemoteAttendanceApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId, false);
                            }
                            else
                            {
                                if (item.IsReject)
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                else
                                {
                                    application.StrStatus = "Approved By Admin";
                                }

                                application.IsPipelineClosed = true;

                                _context.TimeEmployeeAttendances.Update(application);
                                await _context.SaveChangesAsync();

                                await _approvalPipelineService.AttendanceSummaryUpdateAfterRemoteAttendanceApproved(application);

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("MarketAttendanceLanding")]
        public async Task<IActionResult> MarketAttendanceLanding(RemoteAttendanceLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)model.BusinessUnitId, workplaceGroupId = (long)model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                bool IsMarket = true;
                ApplicationLandingVM response = await _approvalPipelineService.RemoteAttendanceLandingEngine(model, tokenData, IsMarket);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("MarketAttendanceApproval")]
        public async Task<IActionResult> MarketAttendanceApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TimeEmployeeAttendance application = await _context.TimeEmployeeAttendances.FirstOrDefaultAsync(x => x.IntRemoteAttendanceId == item.ApplicationId);

                    if (application.IsPipelineClosed == false)
                    {
                        if (application != null)
                        {
                            RemoteAttendanceApprovalResponse result = new RemoteAttendanceApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.RemoteAttendanceApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId, true);
                            }
                            else
                            {
                                if (item.IsReject)
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                else
                                {
                                    application.StrStatus = "Approved By Admin";
                                }

                                application.IsPipelineClosed = true;

                                _context.TimeEmployeeAttendances.Update(application);
                                await _context.SaveChangesAsync();

                                await _approvalPipelineService.AttendanceSummaryUpdateAfterRemoteAttendanceApproved(application);

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("SalaryAdditionNDeductionLanding")]
        public async Task<IActionResult> SalaryAdditionNDeductionLanding(SalaryAdditionNDeductionLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.SalaryAdditionNDeductionLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("SalaryAdditionNDeductionApproval")]
        public async Task<IActionResult> SalaryAdditionNDeductionApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrEmpSalaryAdditionNdeduction application = await _context.PyrEmpSalaryAdditionNdeductions.FirstOrDefaultAsync(x => x.IntSalaryAdditionAndDeductionId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            SalaryAdditionNDeductionApprovalResponse result = new SalaryAdditionNDeductionApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.SalaryAdditionNDeductionApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.PyrEmpSalaryAdditionNdeductions.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("IOUApplicationLanding")]
        public async Task<IActionResult> IOUApplicationLanding(IOULandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.IOUApplicationLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("IOUApplicationApproval")]
        public async Task<IActionResult> IOUApplicationApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrIouapplication application = await _context.PyrIouapplications.FirstOrDefaultAsync(x => x.IntIouid == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            IOUApprovalResponse result = new IOUApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.IOUApplicationApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.PyrIouapplications.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("IOUAdjustmentLanding")]
        public async Task<IActionResult> IOUAdjustmentLanding(IOUAdjustmentLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.IOUAdjustmentLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("IOUAdjustmentApproval")]
        public async Task<IActionResult> IOUAdjustmentApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrIouadjustmentHistory application = await _context.PyrIouadjustmentHistories.FirstOrDefaultAsync(x => x.IntIouadjustmentId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            IOUAdjustmentApprovalResponse result = new IOUAdjustmentApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.IOUAdjustmentApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.PyrIouadjustmentHistories.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("LoanApplicationLanding")]
        public async Task<IActionResult> LoanApplicationLanding(LoanApplicationLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.LoanLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("LoanApplicationApproval")]
        public async Task<IActionResult> LoanApplicationApproval(List<LoanApprovelViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (LoanApprovelViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpLoanApplication application = await _context.EmpLoanApplications.FirstOrDefaultAsync(x => x.IntLoanApplicationId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            LoanApprovalResponse result = new LoanApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.LoanApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                    application.IntApproveLoanAmount = application.IntLoanAmount;
                                    application.IntApproveNumberOfInstallment = application.IntNumberOfInstallment;
                                    application.IntApproveNumberOfInstallmentAmount = application.IntNumberOfInstallmentAmount;
                                    application.NumRemainingBalance = application.IntLoanAmount;
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpLoanApplications.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }


        [HttpPost]
        [Route("SalaryGenerateRequestLandingEngine")]
        public async Task<IActionResult> SalaryGenerateRequestLandingEngine(SalaryGenerateRequestLandingRequestVM model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.SalaryGenerateRequestLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("SalaryGenerateRequestApproval")]
        public async Task<IActionResult> SalaryGenerateRequestApproval(List<PipelineApprovalViewModel> model)
        {
            using var transction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrPayrollSalaryGenerateRequest application = await _context.PyrPayrollSalaryGenerateRequests.FirstOrDefaultAsync(x => x.IntSalaryGenerateRequestId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            SalaryGenerateRequestApprovalResponse result = new SalaryGenerateRequestApprovalResponse();
                            bool rollBackIsComplete = true;

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.SalaryGenerateRequestApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;

                                    // Rollback Loan Balance
                                    rollBackIsComplete = await _approvalPipelineService.LoanBalanceRollBackAfterGeneratedSalaryRejectd(application);

                                }
                                if (rollBackIsComplete)
                                {
                                    application.IsPipelineClosed = true;
                                    _context.PyrPayrollSalaryGenerateRequests.Update(application);
                                    await _context.SaveChangesAsync();

                                    result.ResponseStatus = "success";
                                }
                                else
                                {
                                    result.ResponseStatus = "failed";
                                }

                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                                await transction.CommitAsync();
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                                await transction.RollbackAsync();
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                await transction.RollbackAsync();
                return BadRequest(ex.CatchProcess());
            }


        }

        [HttpPost]
        [Route("OverTimeLanding")]
        public async Task<IActionResult> OverTimeLanding(OverTimeLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.OverTimeLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("OverTimeApproval")]
        public async Task<IActionResult> OverTimeApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TimeEmpOverTime application = await _context.TimeEmpOverTimes.FirstOrDefaultAsync(x => x.IntOverTimeId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            OverTimeApprovalResponse result = new OverTimeApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.OverTimeApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.TimeEmpOverTimes.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }
        [HttpPost]
        [Route("ManualAttendanceLandingEngine")]
        public async Task<IActionResult> ManualAttendanceLandingEngine(ManualAttendanceSummaryLandingVM model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.ManualAttendanceLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("ManualAttendanceApprovalEngine")]
        public async Task<IActionResult> ManualAttendanceApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpManualAttendanceSummary application = await _context.EmpManualAttendanceSummaries.FirstOrDefaultAsync(x => x.IntId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            ManualAttendanceSummaryApprovalResponse result = new ManualAttendanceSummaryApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.ManualAttendanceApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (item.IsReject)
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                else
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                application.IsPipelineClosed = true;

                                _context.EmpManualAttendanceSummaries.Update(application);
                                await _context.SaveChangesAsync();

                                if (application.IsPipelineClosed == true && application.IsReject == false)
                                {
                                    TimeAttendanceDailySummary attendance = await _context.TimeAttendanceDailySummaries.Where(x => x.IntEmployeeId == application.IntEmployeeId && x.DteAttendanceDate.Value.Date == application.DteAttendanceDate.Value.Date).FirstOrDefaultAsync();
                                    attendance.IsManual = true;
                                    attendance.IsManualPresent = application.StrRequestStatus.ToLower() == "Present".ToLower() ? true : false;
                                    attendance.IsManualLate = application.StrRequestStatus.ToLower() == "Late".ToLower() ? true : false;
                                    attendance.IsManualAbsent = application.StrRequestStatus.ToLower() == "Absent".ToLower() ? true : false;
                                    attendance.IntManualAttendanceBy = item.ApproverEmployeeId;
                                    attendance.DteManualAttendanceDate = DateTime.Now;

                                    _context.TimeAttendanceDailySummaries.Update(attendance);
                                    await _context.SaveChangesAsync();
                                }

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("EmployeeSeparationLandingEngine")]
        public async Task<IActionResult> EmployeeSeparationLandingEngine(EmployeeSeparationLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.EmployeeSeparationLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("EmployeeSeparationApprovalEngine")]
        public async Task<IActionResult> EmployeeSeparationApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpEmployeeSeparation application = await _context.EmpEmployeeSeparations.FirstOrDefaultAsync(x => x.IntSeparationId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            EmployeeSeparationApprovalResponse result = new EmployeeSeparationApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.EmployeeSeparationApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);

                                //if (application.IsPipelineClosed == true && application.IsReject == false)
                                //{
                                //    MessageHelper message = await _employeeService.EmployeeInactive(application.IntEmployeeId);
                                //}
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpEmployeeSeparations.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";

                                //MessageHelper message = await _employeeService.EmployeeInactive(application.IntEmployeeId);
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("TransferNPromotionLandingEngine")]
        public async Task<IActionResult> TransferNPromotionLandingEngine(TransferNPromotionLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.TransferNPromotionLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("TransferNPromotionApprovalEngine")]
        public async Task<IActionResult> TransferNPromotionApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpTransferNpromotion application = await _context.EmpTransferNpromotions.FirstOrDefaultAsync(x => x.IntTransferNpromotionId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            TransferNPromotionApprovalResponse result = new TransferNPromotionApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.TransferNPromotionApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpTransferNpromotions.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("EmployeeIncrementLandingEngine")]
        public async Task<IActionResult> EmployeeIncrementLandingEngine(EmployeeIncrementLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.EmployeeIncrementLandingEngine(model, tokenData));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("EmployeeIncrementApprovalEngine")]
        public async Task<IActionResult> EmployeeIncrementApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpEmployeeIncrement application = await _context.EmpEmployeeIncrements.FirstOrDefaultAsync(x => x.IntIncrementId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            EmployeeIncrementApprovalResponse result = new EmployeeIncrementApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.EmployeeIncrementApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpEmployeeIncrements.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("BonusGenerateHeaderLandingEngine")]
        public async Task<IActionResult> BonusGenerateHeaderLandingEngine(BonusGenerateHeaderLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.BonusGenerateHeaderLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("BonusGenerateHeaderApprovalEngine")]
        public async Task<IActionResult> BonusGenerateHeaderApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrBonusGenerateHeader application = await _context.PyrBonusGenerateHeaders.FirstOrDefaultAsync(x => x.IntBonusHeaderId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            BonusGenerateHeaderApprovalResponse result = new BonusGenerateHeaderApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.BonusGenerateHeaderApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.PyrBonusGenerateHeaders.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("PFWithdrawLandingEngine")]
        public async Task<IActionResult> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = (long)model.BusinessUnitId, workplaceGroupId = (long)model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.PFWithdrawLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        //[HttpPost]
        //[Route("PFWithdrawLandingEngine")]
        //public async Task<IActionResult> PFWithdrawLandingEngine(PFWithdrawLandingViewModel model)
        //{
        //    try
        //    {
        //        var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
        //        if (tokenData.accountId == -1)
        //        {
        //            return BadRequest(new MessageHelperAccessDenied());
        //        }
        //        model.AccountId = tokenData.accountId;
        //        model.IsAdmin = tokenData.isOfficeAdmin;
        //        model.ApproverId = tokenData.employeeId;

        //        PFWithdrawApprovalResponse response = await _approvalPipelineService.PFWithdrawLandingEngine(model);

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(400, ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("PFWithdrawApprovalEngine")]
        public async Task<IActionResult> PFWithdrawApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpPfwithdraw application = await _context.EmpPfwithdraws.FirstOrDefaultAsync(x => x.IntPfwithdrawId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            PFWithdrawApprovalResponse result = new PFWithdrawApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.PFWithdrawApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpPfwithdraws.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("ArearSalaryGenerateRequestLanding")]
        public async Task<IActionResult> ArearSalaryGenerateRequestLanding(ArearSalaryGenerateRequestLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.ArearSalaryGenerateRequestLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("ArearSalaryGenerateRequestApproval")]
        public async Task<IActionResult> ArearSalaryGenerateRequestApproval(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    PyrArearSalaryGenerateRequest application = await _context.PyrArearSalaryGenerateRequests.FirstOrDefaultAsync(x => x.IntArearSalaryGenerateRequestId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            ArearSalaryGenerateRequestApprovalResponse result = new ArearSalaryGenerateRequestApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.ArearSalaryGenerateRequestApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.PyrArearSalaryGenerateRequests.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("ExpenseLandingEngine")]
        public async Task<IActionResult> ExpenseLandingEngine(ExpenseApplicationLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.ExpenseLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("ExpenseApprovalEngine")]
        public async Task<IActionResult> ExpenseApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpExpenseApplication application = await _context.EmpExpenseApplications.FirstOrDefaultAsync(x => x.IntExpenseId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            ExpenseApprovalResponse result = new ExpenseApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.ExpenseApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                //application.DteUpdatedAt = DateTime.Now;
                                //application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpExpenseApplications.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("SalaryCertificateRequestLandingEngine")]
        public async Task<IActionResult> SalaryCertificateRequestLandingEngine(SalaryCertificateRequestLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.SalaryCertificateRequestLandingEngine(model, tokenData));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("SalaryCertificateRequestApprovalEngine")]
        public async Task<IActionResult> SalaryCertificateRequestApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    EmpSalaryCertificateRequest application = await _context.EmpSalaryCertificateRequests.FirstOrDefaultAsync(x => x.IntSalaryCertificateRequestId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            SalaryCertificateRequestApprovalResponse result = new SalaryCertificateRequestApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.SalaryCertificateRequestApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.EmpSalaryCertificateRequests.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }


        // Asset Management
        [HttpPost]
        [Route("AssetRequisitionLandingEngine")]
        public async Task<IActionResult> AssetRequisitionLandingEngine(AssetRequisitionLandingRequestVM model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _approvalPipelineService.AssetRequisitionLandingEngine(model, tokenData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("AssetRequisitionApprovalEngine")]
        public async Task<IActionResult> AssetRequisitionApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    AssetRequisition application = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            AssetRequisitionApprovalResponse result = new AssetRequisitionApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.AssetRequisitionApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.AssetRequisitions.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost]
        [Route("AssetTransferLandingEngine")]
        public async Task<IActionResult> AssetTransferLandingEngine(AssetTransferLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                model.AccountId = tokenData.accountId;
                model.IsAdmin = tokenData.isOfficeAdmin;
                model.ApproverId = tokenData.employeeId;

                AssetTransferApprovalResponse response = await _approvalPipelineService.AssetTransferLandingEngine(model);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("AssetTransferApprovalEngine")]
        public async Task<IActionResult> AssetTransferApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    AssetTransfer application = await _context.AssetTransfers.FirstOrDefaultAsync(x => x.IntAssetTransferId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            AssetTransferApprovalResponse result = new AssetTransferApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.AssetTransferApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

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

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        //Training Schedule
        [HttpPost]
        [Route("TrainingScheduleLandingEngine")]
        public async Task<IActionResult> TrainingScheduleLandingEngine(TrainingScheduleLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                model.AccountId = tokenData.accountId;
                model.IsAdmin = tokenData.isOfficeAdmin;
                model.ApproverId = tokenData.employeeId;

                TrainingScheduleApprovalResponse response = await _approvalPipelineService.TrainingScheduleLandingEngine(model);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        [HttpPost]
        [Route("TrainingScheduleApprovalEngine")]
        public async Task<IActionResult> TrainingScheduleApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TrainingSchedule application = await _context.TrainingSchedules.FirstOrDefaultAsync(x => x.IntScheduleId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            TrainingScheduleApprovalResponse result = new TrainingScheduleApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.TrainingScheduleApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteActionDate = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.TrainingSchedules.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        //Training Requisition
        [HttpPost]
        [Route("TrainingRequisitionLandingEngine")]
        public async Task<IActionResult> TrainingRequisitionLandingEngine(TrainingRequisitionLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize, new PayloadIsAuthorizeVM { businessUnitId = model.BusinessUnitId, workplaceGroupId = model.WorkplaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (tokenData.accountId == -1 || !tokenData.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                ApplicationLandingVM response = await _approvalPipelineService.TrainingRequisitionLandingEngine(model, tokenData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost]
        [Route("TrainingRequisitionApprovalEngine")]
        public async Task<IActionResult> TrainingRequisitionApprovalEngine(List<PipelineApprovalViewModel> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string res = string.Empty;
                foreach (PipelineApprovalViewModel item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    TrainingRequisition application = await _context.TrainingRequisitions.FirstOrDefaultAsync(x => x.IntRequisitionId == item.ApplicationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            TrainingRequisitionApprovalResponse result = new TrainingRequisitionApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.TrainingRequisitionApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";
                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteActionDate = DateTime.Now;
                                application.IntApprovedBy = item.ApproverEmployeeId;

                                _context.TrainingRequisitions.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                res = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                res = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }

                return Ok(JsonSerializer.Serialize(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }

        [HttpPost("MasterLocationAssaignLandingEngine")]
        public async Task<IActionResult> MastrLocationLocationLandingEngine(MasterLocationLandingViewModel model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                model.AccountId = tokenData.accountId;
                model.IsAdmin = tokenData.isOfficeAdmin;
                model.ApproverId = tokenData.employeeId;

                return Ok(await _approvalPipelineService.MastrerLocaLandingEngine(model));
            }
            catch (Exception ex)
            {

                MessageHelper msg = new MessageHelper();
                msg.StatusCode = 500;
                msg.AutoId = 0;
                msg.Message = ex.Message;
                return BadRequest(msg);

            }
        }

        [HttpPost]
        [Route("MasterLocationAssaignApprovalEngine")]
        public async Task<IActionResult> MasterLocationAssaignApproval(List<MasterLocationAssaignApprovedDTO> model)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                string response = "Error";

                foreach (MasterLocationAssaignApprovedDTO item in model)
                {
                    item.AccountId = tokenData.accountId;
                    item.IsAdmin = tokenData.isOfficeAdmin;
                    item.ApproverEmployeeId = tokenData.employeeId;

                    MasterLocationRegister application = await _context.MasterLocationRegisters.FirstOrDefaultAsync(x => x.IntMasterLocationId == item.LocationId);

                    if (application.IsPipelineClosed == false && application.IsActive == true)
                    {
                        if (application != null)
                        {
                            MasterLocationApprovalResponse result = new MasterLocationApprovalResponse();

                            if (!item.IsAdmin)
                            {
                                result = await _approvalPipelineService.MasterLocationApprovalEngine(application, item.IsReject, (long)item.ApproverEmployeeId, item.AccountId);
                            }
                            else
                            {
                                if (!item.IsReject)
                                {
                                    application.StrStatus = "Approved By Admin";

                                }
                                else
                                {
                                    application.StrStatus = "Reject By Admin";
                                    application.IsReject = true;
                                }
                                application.IsPipelineClosed = true;
                                application.DteUpdatedAt = DateTime.Now;
                                application.IntUpdatedBy = item.ApproverEmployeeId;

                                _context.MasterLocationRegisters.Update(application);
                                await _context.SaveChangesAsync();

                                result.ResponseStatus = "success";
                            }

                            if (result.ResponseStatus == "success")
                            {
                                response = (item.IsReject ? "Rejected" : "Approved") + " Successfully";
                            }
                            else
                            {
                                response = (item.IsReject ? "Rejected" : "Approve") + " Failed";
                            }
                        }
                    }
                }
                return Ok(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.CatchProcess());
            }
        }



        #endregion ============== A P L I C A T I O N ===================
    }
}