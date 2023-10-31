using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Models;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.SAAS;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Controllers.SAAS
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetManagementController : ControllerBase
    {
        private readonly IAssetManagement _irepository;
        private readonly IEmployeeService _employeeService;

        public AssetManagementController(IAssetManagement iRepo, IEmployeeService _employeeService)
        {
            _irepository = iRepo;
            this._employeeService = _employeeService;
        }

        #region Item Category

        [HttpPost]
        [Route("CreateItemCategory")]
        public async Task<IActionResult> CreateItemCategory(ItemCategoryVM obj)
        {
            try
            {
                MessageHelper msg = await _irepository.CreateItemCategory(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Item Category

        #region Item Uom

        [HttpPost]
        [Route("CreateItemUom")]
        public async Task<IActionResult> CreateItemUom(ItemUomVM obj)
        {
            try
            {
                MessageHelper msg = await _irepository.CreateItemUom(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Item Uom

        #region All DDL -- Asset Management Related

        [HttpGet]
        [Route("ItemCategoryDDL")]
        public async Task<IActionResult> ItemCategoryDDL(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _irepository.ItemCategoryDDL(accountId, businessUnitId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("ItemUomDDL")]
        public async Task<IActionResult> ItemUomDDL(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _irepository.ItemUomDDL(accountId, businessUnitId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("ItemDDL")]
        public async Task<IActionResult> ItemDDL(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _irepository.ItemDDL(accountId, businessUnitId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("AssetTransferFromEmployeeDDL")]
        public async Task<IActionResult> AssetTransferFromEmployeeDDL(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _irepository.AssetTransferFromEmployeeDDL(accountId, businessUnitId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("AssetTransferItemDDL")]
        public async Task<IActionResult> AssetTransferItemDDL(long accountId, long businessUnitId, long employeeId)
        {
            try
            {
                return Ok(await _irepository.AssetTransferItemDDL(accountId, businessUnitId, employeeId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion All DDL -- Asset Management Related

        #region Item -- Registration

        [HttpPost]
        [Route("SaveItem")]
        public async Task<IActionResult> SaveItem(ItemVM obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                obj.AccountId = tokenData.accountId;
                obj.BusinessUnitId = tokenData.businessUnitId;

                MessageHelper msg = await _irepository.SaveItem(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetItem")]
        public async Task<IActionResult> GetItem(long accountId, long businessUnitId)
        {
            try
            {
                return Ok(await _irepository.GetItem(accountId, businessUnitId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Item -- Registration

        #region Asset -- Registration

        [HttpPost]
        [Route("SaveAsset")]
        public async Task<IActionResult> SaveAsset(AssetVM obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                obj.AccountId = tokenData.accountId;
                obj.BusinessUnitId = tokenData.businessUnitId;

                MessageHelper msg = await _irepository.SaveAsset(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetAsset")]
        public async Task<IActionResult> GetAsset(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _irepository.GetAsset(tokenData.accountId, tokenData.businessUnitId, fromDate, toDate));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetAssetById")]
        public async Task<IActionResult> GetAssetById(long accountId, long businessUnitId, long assetId)
        {
            try
            {
                return Ok(await _irepository.GetAssetById(accountId, businessUnitId, assetId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Asset -- Registration

        #region Asset Direct Assign

        [HttpPost]
        [Route("SaveDirectAssetAssign")]
        public async Task<IActionResult> SaveDirectAssetAssign(AssetDirectAssignVM obj)
        {
            try
            {
                var tD = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenDataIfAuthorize,
                new PayloadIsAuthorizeVM { businessUnitId = obj.BusinessUnitId, workplaceGroupId = obj.WorkPlaceGroupId }, PermissionLebelCheck.WorkplaceGroup);

                if (!tD.isAuthorize)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _irepository.SaveDirectAssetAssign(obj));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetDirectAssetAssign")]
        public async Task<IActionResult> GetDirectAssetAssign(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _irepository.GetDirectAssetAssign(tokenData.accountId, tokenData.businessUnitId, fromDate, toDate));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetDirectAssetAssignById")]
        public async Task<IActionResult> GetDirectAssetAssignById(long accountId, long businessUnitId, long assetId)
        {
            try
            {
                return Ok(await _irepository.GetDirectAssetAssignById(accountId, businessUnitId, assetId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Asset Direct Assign

        #region Asset Requisition

        [HttpPost]
        [Route("SaveAssetRequisition")]
        public async Task<IActionResult> SaveAssetRequisition(AssetRequisitionVM obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                obj.AccountId = tokenData.accountId;
                obj.EmployeeId = tokenData.employeeId;
                obj.BusinessUnitId = tokenData.businessUnitId;

                return Ok(await _irepository.SaveAssetRequisition(obj));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpPost]
        [Route("SendForApprovalAssetRequisition")]
        public async Task<IActionResult> SendForApprovalAssetRequisition(List<AssetRequisitionVM> obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != obj.Select(x => x.EmployeeId).First())
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.Select(x => x.BusinessUnitId).First(), obj.Select(x => x.WorkPlaceGroupId).First(), obj.Select(x => x.EmployeeId).First(), "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }

                obj.ForEach(x => x.AccountId = tokenData.accountId);

                return Ok(await _irepository.SendForApprovalAssetRequisition(obj));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetAssetRequisition")]
        public async Task<IActionResult> GetAssetRequisition(long businessUnitId, long workplaceGroupId, long? workPlaceId, long? employeeId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != (long)employeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, businessUnitId, workplaceGroupId, employeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }

                return Ok(await _irepository.GetAssetRequisition(tokenData.accountId, businessUnitId, workplaceGroupId, workPlaceId, employeeId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetAssetRequisitionForSelf")]
        public async Task<IActionResult> GetAssetRequisitionForSelf(long accountId, long businessUnitId, long? workplaceGroupId, long? workPlaceId, long? employeeId)
        {
            try
            {
                return Ok(await _irepository.GetAssetRequisitionForSelf(accountId, businessUnitId, workplaceGroupId, workPlaceId, employeeId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpGet]
        [Route("GetAssetRequisitionById")]
        public async Task<IActionResult> GetAssetRequisitionById(long accountId, long businessUnitId, long assetRequisitionId)
        {
            try
            {
                return Ok(await _irepository.GetAssetRequisitionById(accountId, businessUnitId, assetRequisitionId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpPost]
        [Route("AssetRequisitionDenied")]
        public async Task<IActionResult> AssetRequisitionDenied(AssetRequisitionVM obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                if (tokenData.employeeId != obj.EmployeeId)
                {
                    var check = await _employeeService.GetCommonEmployeeDDL(tokenData, obj.BusinessUnitId, obj.WorkPlaceGroupId, obj.EmployeeId, "");
                    if (check.Count() <= 0)
                    {
                        return BadRequest(new MessageHelperAccessDenied());

                    }
                }

                obj.AccountId = tokenData.accountId;
                return Ok(await _irepository.AssetRequisitionDenied(obj));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        #endregion Asset Requisition

        #region Asset Transfer

        [HttpPost]
        [Route("SaveAssetTransfer")]
        public async Task<IActionResult> SaveAssetTransfer(AssetTransferVM obj)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);

                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }
                obj.AccountId= tokenData.accountId;

                MessageHelper msg = await _irepository.SaveAssetTransfer(obj);
                if (msg.StatusCode == 200)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetAssetTransfer")]
        public async Task<IActionResult> GetAssetTransfer(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _irepository.GetAssetTransfer(tokenData.accountId, tokenData.businessUnitId, fromDate, toDate));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess());
            }
        }

        [HttpGet]
        [Route("GetAssetTransferById")]
        public async Task<IActionResult> GetAssetTransferById(long assetTransferId)
        {
            try
            {
                var tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                return Ok(await _irepository.GetAssetTransferById(tokenData.accountId, tokenData.businessUnitId, assetTransferId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }
        #endregion

        #region Asset List
        [HttpGet]
        [Route("GetAssetList")]
        public async Task<IActionResult> GetAssetList(long accountId, long businessUnitId, long employeeId)
        {
            try
            {
                return Ok(await _irepository.GetAssetList(accountId, businessUnitId, employeeId));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }

        [HttpPost]
        [Route("AssetAcknowledged")]
        public async Task<IActionResult> AssetAcknowledged(AssetListVM obj)
        {
            try
            {
                return Ok(await _irepository.AssetAcknowledged(obj));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.CatchProcess()); ;
            }
        }
        #endregion
    }
}