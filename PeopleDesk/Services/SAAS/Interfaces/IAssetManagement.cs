using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Models;
using PeopleDesk.Models.AssetManagement;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IAssetManagement
    {
        #region Item Category

        public Task<MessageHelper> CreateItemCategory(ItemCategoryVM obj);

        #endregion Item Category

        #region Item Uom

        public Task<MessageHelper> CreateItemUom(ItemUomVM obj);

        #endregion Item Uom

        #region All DDL -- Asset Management Related

        public Task<List<CommonDDLVM>> ItemCategoryDDL(long accountId, long businessUnitId);

        public Task<List<CommonDDLVM>> ItemUomDDL(long accountId, long businessUnitId);

        public Task<List<CommonDDLVM>> ItemDDL(long accountId, long businessUnitId);
        public Task<List<CommonDDLVM>> AssetTransferFromEmployeeDDL(long accountId, long businessUnitId);
        public Task<List<AssetTransferDDLVM>> AssetTransferItemDDL(long accountId, long businessUnitId, long employeeId);

        #endregion All DDL -- Asset Management Related

        #region Item -- Registration

        public Task<MessageHelper> SaveItem(ItemVM obj);

        public Task<List<ItemVM>> GetItem(long accountId, long businessUnitId);

        #endregion Item -- Registration

        #region Asset -- Registration

        public Task<MessageHelper> SaveAsset(AssetVM obj);

        public Task<List<AssetVM>> GetAsset(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate);

        public Task<AssetVM> GetAssetById(long accountId, long businessUnitId, long assetId);

        #endregion Asset -- Registration

        #region Asset Direct Assign

        public Task<MessageHelper> SaveDirectAssetAssign(AssetDirectAssignVM obj);

        public Task<List<AssetDirectAssignVM>> GetDirectAssetAssign(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate);

        public Task<AssetDirectAssignVM> GetDirectAssetAssignById(long accountId, long businessUnitId, long assetId);

        #endregion Asset Direct Assign

        #region Asset Requisition

        public Task<MessageHelper> SaveAssetRequisition(AssetRequisitionVM obj);
        public Task<MessageHelper> SendForApprovalAssetRequisition(List<AssetRequisitionVM> obj);

        public Task<List<AssetRequisitionVM>> GetAssetRequisition(long accountId, long businessUnitId, long? workplaceGroupId, long? workPlaceId, long? employeeId);
        public Task<List<AssetRequisitionVM>> GetAssetRequisitionForSelf(long accountId, long businessUnitId, long? workplaceGroupId, long? workPlaceId, long? employeeId);

        public Task<AssetRequisitionVM> GetAssetRequisitionById(long accountId, long businessUnitId, long assetRequisitionId);

        public Task<MessageHelper> AssetRequisitionDenied(AssetRequisitionVM obj);

        #endregion Asset Requisition

        #region Asset Transfer
        public Task<MessageHelper> SaveAssetTransfer(AssetTransferVM obj);
        public Task<List<AssetTransferVM>> GetAssetTransfer(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate);
        public Task<AssetTransferVM> GetAssetTransferById(long accountId, long businessUnitId, long assetTransferId);
        #endregion

        #region Asset List
        public Task<List<AssetListVM>> GetAssetList(long accountId, long businessUnitId, long employeeId);
        public Task<MessageHelper> AssetAcknowledged(AssetListVM obj);
        #endregion
    }
}