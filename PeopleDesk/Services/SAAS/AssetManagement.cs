using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Services.SAAS
{
    public class AssetManagement : IAssetManagement
    {
        private readonly PeopleDeskContext _context;
        private readonly IApprovalPipelineService _approvalPipelineService;

        public AssetManagement(PeopleDeskContext _context, IApprovalPipelineService _approvalPipelineService)
        {
            this._context = _context;
            this._approvalPipelineService = _approvalPipelineService;
        }

        #region Item Category

        public async Task<MessageHelper> CreateItemCategory(ItemCategoryVM obj)
        {
            try
            {
                MessageHelper msg = new();
                var duplicateCheck = _context.ItemCategories.Where(x => x.IntBusinessUnitId == obj.BusinessUnitId && x.IntAccountId == obj.AccountId && x.IsActive == true
                && x.StrItemCategory.Trim().ToLower() == obj.ItemCategory.Trim().ToLower()).FirstOrDefault();

                if (duplicateCheck != null)
                {
                    msg.Message = "Already Exist Item Category!";
                    msg.StatusCode = 409;
                    return msg;
                }

                ItemCategory detalis = new ItemCategory
                {
                    IntItemCategoryId = obj.ItemCategoryId,
                    IntAccountId = obj.AccountId,
                    IntBusinessUnitId = obj.BusinessUnitId,
                    StrItemCategory = obj.ItemCategory,
                    IsActive = true,
                    DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                    IntCreatedBy = obj.CreatedBy,
                };
                await _context.ItemCategories.AddAsync(detalis);
                await _context.SaveChangesAsync();

                msg.Message = "Created Successfully";
                msg.StatusCode = 200;
                return msg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Item Category

        #region Item Uom

        public async Task<MessageHelper> CreateItemUom(ItemUomVM obj)
        {
            try
            {
                MessageHelper msg = new();
                var duplicateCheck = _context.ItemUoms.Where(x => x.IntBusinessUnitId == obj.BusinessUnitId && x.IntAccountId == obj.AccountId && x.IsActive == true
                && x.StrItemUom.Trim().ToLower() == obj.ItemUom.Trim().ToLower()).FirstOrDefault();

                if (duplicateCheck != null)
                {
                    msg.Message = "Already Exist Item Uom!";
                    msg.StatusCode = 409;
                    return msg;
                }

                ItemUom detalis = new ItemUom
                {
                    IntItemUomId = obj.ItemUomId,
                    IntAccountId = obj.AccountId,
                    IntBusinessUnitId = obj.BusinessUnitId,
                    StrItemUom = obj.ItemUom,
                    IsActive = true,
                    DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                    IntCreatedBy = obj.CreatedBy
                };
                await _context.ItemUoms.AddAsync(detalis);
                await _context.SaveChangesAsync();

                msg.Message = "Created Successfully";
                msg.StatusCode = 200;
                return msg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Item Uom

        #region All DDL -- Asset Management Related

        public async Task<List<CommonDDLVM>> ItemCategoryDDL(long accountId, long businessUnitId)
        {
            try
            {
                var data = await (from a in _context.ItemCategories
                                  where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IsActive == true
                                  orderby a.StrItemCategory ascending
                                  select new CommonDDLVM
                                  {
                                      Value = a.IntItemCategoryId,
                                      Label = a.StrItemCategory
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonDDLVM>> ItemUomDDL(long accountId, long businessUnitId)
        {
            try
            {
                var data = await (from a in _context.ItemUoms
                                  where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IsActive == true
                                  orderby a.StrItemUom ascending
                                  select new CommonDDLVM
                                  {
                                      Value = a.IntItemUomId,
                                      Label = a.StrItemUom
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonDDLVM>> ItemDDL(long accountId, long businessUnitId)
        {
            try
            {
                var data = await (from a in _context.Items
                                  where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IsActive == true
                                  orderby a.StrItemName ascending
                                  select new CommonDDLVM
                                  {
                                      Value = a.IntItemId,
                                      Label = a.StrItemName
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonDDLVM>> AssetTransferFromEmployeeDDL(long accountId, long businessUnitId)
        {
            try
            {
                List<CommonDDLVM> dataList = new();

                List<CommonDDLVM> assetRequisition = await (from a in _context.AssetRequisitions
                                                            join emp in _context.EmpEmployeeBasicInfos on a.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                            where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId
                                                            && a.IsActive == true && emp.IsActive == true && a.IsPipelineClosed == true && a.IsReject == false
                                                            orderby emp.StrEmployeeName ascending
                                                            select new CommonDDLVM
                                                            {
                                                                Value = a.IntEmployeeId,
                                                                Label = emp.StrEmployeeName
                                                            }).Distinct().ToListAsync();
                dataList.AddRange(assetRequisition);

                List<CommonDDLVM> assetTransfer = await (from a in _context.AssetTransfers
                                                         join emp in _context.EmpEmployeeBasicInfos on a.IntToEmployeeId equals emp.IntEmployeeBasicInfoId
                                                         where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId
                                                         && a.IsActive == true && emp.IsActive == true && a.IsPipelineClosed == true && a.IsReject == false
                                                         orderby emp.StrEmployeeName ascending
                                                         select new CommonDDLVM
                                                         {
                                                             Value = a.IntToEmployeeId.Value,
                                                             Label = emp.StrEmployeeName
                                                         }).Distinct().ToListAsync();
                dataList.AddRange(assetTransfer);

                List<CommonDDLVM> assetDirect = await (from a in _context.AssetDirectAssigns
                                                       join emp in _context.EmpEmployeeBasicInfos on a.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                                                       where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId
                                                       && a.IsActive == true && emp.IsActive == true
                                                       orderby emp.StrEmployeeName ascending
                                                       select new CommonDDLVM
                                                       {
                                                           Value = a.IntEmployeeId,
                                                           Label = emp.StrEmployeeName
                                                       }).Distinct().ToListAsync();
                dataList.AddRange(assetDirect);

                return dataList.GroupBy(x => x.Value).Select(x => x.First()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AssetTransferDDLVM>> AssetTransferItemDDL(long accountId, long businessUnitId, long employeeId)
        {
            try
            {
                List<AssetTransferDDLVM> dataList = new();

                List<AssetTransferDDLVM> assetRequisition = await (from a in _context.AssetRequisitions
                                                                   join i in _context.Items on a.IntItemId equals i.IntItemId
                                                                   where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IntEmployeeId == employeeId
                                                                   && a.IsActive == true && i.IsActive == true && a.IsPipelineClosed == true && a.IsReject == false
                                                                   orderby i.StrItemName ascending
                                                                   select new AssetTransferDDLVM
                                                                   {
                                                                       Value = a.IntItemId,
                                                                       Label = i.StrItemName,
                                                                       Quantity = a.IntReqisitionQuantity
                                                                   }).ToListAsync();
                dataList.AddRange(assetRequisition);

                List<AssetTransferDDLVM> assetTransfer = await (from a in _context.AssetTransfers
                                                                join i in _context.Items on a.IntItemId equals i.IntItemId
                                                                where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IntToEmployeeId == employeeId
                                                                && a.IsActive == true && i.IsActive == true && a.IsPipelineClosed == true && a.IsReject == false
                                                                orderby i.StrItemName ascending
                                                                select new AssetTransferDDLVM
                                                                {
                                                                    Value = a.IntItemId,
                                                                    Label = i.StrItemName,
                                                                    Quantity = a.IntTransferQuantity.Value
                                                                }).ToListAsync();
                dataList.AddRange(assetTransfer);

                List<AssetTransferDDLVM> assetDirect = await (from a in _context.AssetDirectAssigns
                                                              join i in _context.Items on a.IntItemId equals i.IntItemId
                                                              where a.IntAccountId == accountId && a.IntBusinessUnitId == businessUnitId && a.IntEmployeeId == employeeId
                                                              && a.IsActive == true && i.IsActive == true
                                                              orderby i.StrItemName ascending
                                                              select new AssetTransferDDLVM
                                                              {
                                                                  Value = a.IntItemId,
                                                                  Label = i.StrItemName,
                                                                  Quantity = a.IntItemQuantity
                                                              }).ToListAsync();
                dataList.AddRange(assetDirect);

                var results = dataList.GroupBy(x => x.Value).Select(g => new AssetTransferDDLVM
                {
                    Value = g.Key,
                    Label = dataList.FirstOrDefault(d => d.Value == g.Key).Label,
                    Quantity = g.Sum(x => x.Quantity)
                });
                return results.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion All DDL -- Asset Management Related

        #region Item -- Registration

        public async Task<MessageHelper> SaveItem(ItemVM obj)
        {
            try
            {
                MessageHelper msg = new();
                var duplicateCheck = await _context.Items.Where(x => x.StrItemName.Trim().ToLower() == obj.ItemName.Trim().ToLower()
                                    && x.IntAccountId == obj.AccountId && x.IntBusinessUnitId == obj.BusinessUnitId && x.IntItemId != obj.ItemId && x.IsActive == true).FirstOrDefaultAsync();

                var codeDuplicateCheck = await _context.Items.Where(x => x.StrItemCode.Trim().ToLower() == obj.ItemCode.Trim().ToLower() && x.IntAccountId == obj.AccountId && x.IntItemId != obj.ItemId && x.IsActive == true).FirstOrDefaultAsync();

                if (duplicateCheck != null)
                {
                    msg.StatusCode = 409;
                    msg.Message = "Already Exist Item Name.";
                    return msg;
                }
                else if (codeDuplicateCheck != null)
                {
                    msg.StatusCode = 409;
                    msg.Message = "Already Exist Item Code.";
                    return msg;
                }

                var counter = await _context.Items.CountAsync() + 1;
                Item dataParent = await _context.Items.FirstOrDefaultAsync(x => x.IntItemId == obj.ItemId && x.IsActive == true);

                if (dataParent == null)
                {
                    Item data = new Item
                    {
                        IntItemId = obj.ItemId,
                        IntAccountId = obj.AccountId,
                        IntBusinessUnitId = obj.BusinessUnitId,
                        StrItemCode = obj.IsAutoCode == false ? "Item - " + (counter.ToString().LeftPad("0", 5)).ToString() : obj.ItemCode,
                        IsAutoCode = obj.IsAutoCode,
                        StrItemName = obj.ItemName,
                        IntItemCategoryId = obj.ItemCategoryId,
                        IntItemUomId = obj.ItemUomId,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                        IntCreatedBy = obj.CreatedBy,
                    };
                    await _context.Items.AddAsync(data);
                    await _context.SaveChangesAsync();

                    msg.Message = "Created Successfully";
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    dataParent.StrItemName = obj.ItemName;
                    dataParent.IntItemCategoryId = obj.ItemCategoryId;
                    dataParent.IntItemUomId = obj.ItemUomId;
                    dataParent.StrItemCode = obj.ItemCode;
                    dataParent.IsAutoCode = obj.IsAutoCode;
                    dataParent.IsActive = obj.Active;
                    dataParent.DteCreatedAt = obj.CreatedAt;
                    dataParent.IntCreatedBy = obj.CreatedBy;
                    dataParent.DteUpdatedAt = DateTime.Now.GetCurrentDateTimeBD();
                    dataParent.IntUpdatedBy = obj.UpdatedBy;

                    _context.Items.Update(dataParent);
                    await _context.SaveChangesAsync();

                    if (obj.ItemId > 0 && obj.Active == false)
                    {
                        msg.Message = "Deleted Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        msg.Message = "Updated Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ItemVM>> GetItem(long accountId, long businessUnitId)
        {
            var data = await (from e in _context.Items
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && e.IsActive == true
                              orderby e.IntItemId descending
                              select new ItemVM
                              {
                                  ItemId = e.IntItemId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  ItemCode = e.StrItemCode,
                                  IsAutoCode = e.IsAutoCode,
                                  ItemName = e.StrItemName,
                                  ItemCategoryId = e.IntItemCategoryId,
                                  ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == e.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                  ItemUomId = e.IntItemUomId,
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == e.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  Active = e.IsActive,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy,
                              }).ToListAsync();
            return data;
        }

        #endregion Item -- Registration

        #region Asset -- Registration

        public async Task<MessageHelper> SaveAsset(AssetVM obj)
        {
            try
            {
                MessageHelper msg = new();
                var duplicateCheck = await _context.Assets.Where(x => x.IntItemId == obj.ItemId
                                    && x.IntAccountId == obj.AccountId && x.IntBusinessUnitId == obj.BusinessUnitId && x.IntAssetId != obj.AssetId && x.IsActive == true).FirstOrDefaultAsync();
                if (duplicateCheck != null)
                {
                    msg.StatusCode = 409;
                    msg.Message = "Already Exist this Asset.";
                    return msg;
                }

                var counter = await _context.Assets.CountAsync() + 1;
                Asset dataParent = await _context.Assets.FirstOrDefaultAsync(x => x.IntAssetId == obj.AssetId && x.IsActive == true);

                if (dataParent == null)
                {
                    Asset data = new Asset
                    {
                        IntAssetId = obj.AssetId,
                        IntAccountId = obj.AccountId,
                        IntBusinessUnitId = obj.BusinessUnitId,
                        StrAssetCode = "ASS - " + (counter.ToString().LeftPad("0", 5)).ToString(),
                        IntItemId = obj.ItemId,
                        StrItemName = obj.ItemName,
                        StrSupplierName = obj.SupplierName,
                        StrSupplierMobileNo = obj.SupplierMobileNo,
                        DteAcquisitionDate = obj.AcquisitionDate,
                        IntAcquisitionValue = obj.AcquisitionValue,
                        IntInvoiceValue = obj.InvoiceValue,
                        IntDepreciationValue = obj.DepreciationValue,
                        DteDepreciationDate = obj.DepreciationDate,
                        DteWarrantyDate = obj.WarrantyDate,
                        StrDescription = obj.Description,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                        IntCreatedBy = obj.CreatedBy,
                    };
                    await _context.Assets.AddAsync(data);
                    await _context.SaveChangesAsync();

                    msg.Message = "Created Successfully";
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    dataParent.StrSupplierName = obj.SupplierName;
                    dataParent.StrSupplierMobileNo = obj.SupplierMobileNo;
                    dataParent.DteAcquisitionDate = obj.AcquisitionDate;
                    dataParent.IntAcquisitionValue = obj.AcquisitionValue;
                    dataParent.IntInvoiceValue = obj.InvoiceValue;
                    dataParent.IntDepreciationValue = obj.DepreciationValue;
                    dataParent.DteDepreciationDate = obj.DepreciationDate;
                    dataParent.DteWarrantyDate = obj.WarrantyDate;
                    dataParent.StrDescription = obj.Description;
                    dataParent.IsActive = obj.Active;
                    dataParent.DteUpdatedAt = DateTime.Now.GetCurrentDateTimeBD();
                    dataParent.IntUpdatedBy = obj.UpdatedBy;

                    _context.Assets.Update(dataParent);
                    await _context.SaveChangesAsync();

                    if (obj.AssetId > 0 && obj.Active == false)
                    {
                        msg.Message = "Deleted Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        msg.Message = "Updated Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AssetVM>> GetAsset(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await (from e in _context.Assets
                              join i in _context.Items on e.IntItemId equals i.IntItemId
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && (!fromDate.HasValue || e.DteAcquisitionDate.Date >= fromDate.Value.Date)
                              && (!toDate.HasValue || e.DteAcquisitionDate.Date <= toDate.Value.Date)
                              && e.IsActive == true && i.IsActive == true
                              orderby e.IntAssetId descending
                              select new AssetVM
                              {
                                  AssetId = e.IntAssetId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  AssetCode = e.StrAssetCode,
                                  ItemId = e.IntItemId,
                                  ItemName = e.StrItemName,
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                  SupplierName = e.StrSupplierName,
                                  SupplierMobileNo = e.StrSupplierMobileNo,
                                  AcquisitionDate = e.DteAcquisitionDate,
                                  AcquisitionValue = e.IntAcquisitionValue,
                                  InvoiceValue = e.IntInvoiceValue,
                                  DepreciationValue = e.IntDepreciationValue,
                                  DepreciationDate = e.DteDepreciationDate,
                                  WarrantyDate = e.DteWarrantyDate,
                                  Description = e.StrDescription,
                                  Active = e.IsActive,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy,
                              }).ToListAsync();
            return data;
        }

        public async Task<AssetVM> GetAssetById(long accountId, long businessUnitId, long assetId)
        {
            var data = await (from e in _context.Assets
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && e.IntAssetId == assetId && e.IsActive == true
                              select new AssetVM
                              {
                                  AssetId = e.IntAssetId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  AssetCode = e.StrAssetCode,
                                  ItemId = e.IntItemId,
                                  ItemName = e.StrItemName,
                                  SupplierName = e.StrSupplierName,
                                  SupplierMobileNo = e.StrSupplierMobileNo,
                                  AcquisitionDate = e.DteAcquisitionDate,
                                  AcquisitionValue = e.IntAcquisitionValue,
                                  InvoiceValue = e.IntInvoiceValue,
                                  DepreciationValue = e.IntDepreciationValue,
                                  DepreciationDate = e.DteDepreciationDate,
                                  WarrantyDate = e.DteWarrantyDate,
                                  Description = e.StrDescription,
                                  Active = e.IsActive,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy,
                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion Asset -- Registration

        #region Asset Direct Assign

        public async Task<MessageHelper> SaveDirectAssetAssign(AssetDirectAssignVM obj)
        {
            try
            {
                AssetDirectAssign dataParent = await _context.AssetDirectAssigns.FirstOrDefaultAsync(x => x.IntAssetDirectAssignId == obj.AssetDirectAssignId && x.IsActive == true);
                var msg = new MessageHelper();

                if (dataParent == null)
                {
                    AssetDirectAssign data = new AssetDirectAssign
                    {
                        IntAssetDirectAssignId = obj.AssetDirectAssignId,
                        IntAccountId = obj.AccountId,
                        IntBusinessUnitId = obj.BusinessUnitId,
                        IntEmployeeId = obj.EmployeeId,
                        IntItemId = obj.ItemId,
                        IntItemQuantity = obj.ItemQuantity,
                        DteAssignDate = obj.AssignDate,
                        IsActive = true,
                        DteCreateAt = DateTime.Now.GetCurrentDateTimeBD(),
                        IntCreatedBy = obj.CreatedBy,
                    };
                    await _context.AssetDirectAssigns.AddAsync(data);
                    await _context.SaveChangesAsync();

                    msg.Message = "Created Successfully";
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    dataParent.IntItemId = obj.ItemId;
                    dataParent.IntItemQuantity = obj.ItemQuantity;
                    dataParent.DteAssignDate = obj.AssignDate;
                    dataParent.IsActive = obj.Active;
                    dataParent.DteUpdatedAt = DateTime.Now.GetCurrentDateTimeBD();
                    dataParent.IntUpdatedBy = obj.UpdatedBy;

                    _context.AssetDirectAssigns.Update(dataParent);
                    await _context.SaveChangesAsync();

                    if (obj.AssetDirectAssignId > 0 && obj.Active == false)
                    {
                        msg.Message = "Deleted Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        msg.Message = "Updated Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AssetDirectAssignVM>> GetDirectAssetAssign(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await (from e in _context.AssetDirectAssigns
                              join i in _context.Items on e.IntItemId equals i.IntItemId
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && (!fromDate.HasValue || e.DteAssignDate.Date >= fromDate.Value.Date)
                              && (!toDate.HasValue || e.DteAssignDate.Date <= toDate.Value.Date)
                              && e.IsActive == true && i.IsActive == true && e.IntItemQuantity > 0
                              orderby e.IntAssetDirectAssignId descending
                              select new AssetDirectAssignVM
                              {
                                  AssetDirectAssignId = e.IntAssetDirectAssignId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  EmployeeId = e.IntEmployeeId,
                                  EmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  EmployeeCode = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntEmployeeId).Select(x => x.StrEmployeeCode).FirstOrDefault(),
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  ItemQuantity = e.IntItemQuantity,
                                  AssignDate = e.DteAssignDate,
                                  Active = e.IsActive,
                                  CreateAt = e.DteCreateAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy,
                                  Status = e.StrStatus == "Acknowledged" ? "Acknowledged" : "-"
                              }).ToListAsync();
            return data;
        }

        public async Task<AssetDirectAssignVM> GetDirectAssetAssignById(long accountId, long businessUnitId, long assetId)
        {
            var data = await (from e in _context.AssetDirectAssigns
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && e.IntAssetDirectAssignId == assetId && e.IsActive == true
                              select new AssetDirectAssignVM
                              {
                                  AssetDirectAssignId = e.IntAssetDirectAssignId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  EmployeeId = e.IntEmployeeId,
                                  EmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  ItemQuantity = e.IntItemQuantity,
                                  AssignDate = e.DteAssignDate,
                                  Active = e.IsActive,
                                  CreateAt = e.DteCreateAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy,
                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion Asset Direct Assign

        #region Asset Requisition

        public async Task<MessageHelper> SaveAssetRequisition(AssetRequisitionVM obj)
        {
            try
            {
                MessageHelper msg = new();
                AssetRequisition dataParent = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == obj.AssetRequisitionId && x.IsActive == true);

                if (dataParent == null)
                {
                    AssetRequisition data = new AssetRequisition
                    {
                        IntAccountId = obj.AccountId,
                        IntBusinessUnitId = obj.BusinessUnitId,
                        IntItemId = obj.ItemId,
                        IntEmployeeId = obj.EmployeeId,
                        IntReqisitionQuantity = obj.ReqisitionQuantity,
                        DteReqisitionDate = obj.ReqisitionDate,
                        StrRemarks = obj.Remarks,
                        IsActive = true,
                        DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                        IntCreatedBy = obj.CreatedBy,
                        IntPipelineHeaderId = 0,
                        IntCurrentStage = 0,
                        IntNextStage = 0,
                        StrStatus = "Pending",
                        IsPipelineClosed = false,
                        IsReject = false,
                        IsDenied = false
                    };
                    await _context.AssetRequisitions.AddAsync(data);
                    await _context.SaveChangesAsync();

                    msg.Message = "Created Successfully";
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    dataParent.IntItemId = obj.ItemId;
                    dataParent.IntReqisitionQuantity = obj.ReqisitionQuantity;
                    dataParent.DteReqisitionDate = obj.ReqisitionDate;
                    dataParent.StrRemarks = obj.Remarks;
                    dataParent.IsActive = obj.Active;
                    dataParent.DteUpdatedAt = obj.UpdatedAt;
                    dataParent.IntUpdatedBy = obj.UpdatedBy;

                    _context.AssetRequisitions.Update(dataParent);
                    await _context.SaveChangesAsync();

                    if (obj.AssetRequisitionId > 0 && obj.Active == false)
                    {
                        msg.Message = "Deleted Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        msg.Message = "Updated Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<MessageHelper> SendForApprovalAssetRequisition(List<AssetRequisitionVM> obj)
        {
            try
            {
                MessageHelper msg = new();
                foreach (var item in obj)
                {
                    AssetRequisition dataParent = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == item.AssetRequisitionId && x.IsActive == true);

                    // PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(item.AccountId, "assetApproval");
                    var stage = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType(item.EmployeeId, "assetApproval");

                    if (stage.HeaderId <= 0 || stage.CurrentStageId <= 0 || stage.NextStageId <= 0)
                    {
                        msg.StatusCode = 500;
                        msg.Message = "Pipeline was not set";
                        return msg;
                    }

                    if (dataParent == null)
                    {
                        msg.Message = "No Data Found";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        dataParent.IntReqisitionQuantity = item.ReqisitionQuantity;
                        dataParent.IntPipelineHeaderId = stage.HeaderId;
                        dataParent.IntCurrentStage = stage.CurrentStageId;
                        dataParent.IntNextStage = stage.NextStageId;

                        _context.AssetRequisitions.Update(dataParent);
                        await _context.SaveChangesAsync();
                    }
                }

                msg.Message = "Approval Send Successfully";
                msg.StatusCode = 200;
                return msg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AssetRequisitionVM>> GetAssetRequisition(long accountId, long businessUnitId, long? workplaceGroupId, long? workPlaceId, long? employeeId)
        {
            var data = await (from e in _context.AssetRequisitions
                              join emp in _context.EmpEmployeeBasicInfos on e.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                              join i in _context.Items on e.IntItemId equals i.IntItemId
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && emp.IsActive == true
                              && e.IsActive == true && i.IsActive == true && (e.IsDenied == null || e.IsDenied == false) && e.IntPipelineHeaderId == 0
                              && (workplaceGroupId == 0 || emp.IntWorkplaceGroupId == workplaceGroupId)
                              && (workPlaceId == 0 || emp.IntWorkplaceId == workPlaceId)
                              && (employeeId == 0 || e.IntEmployeeId == employeeId)
                              orderby e.IntAssetRequisitionId descending
                              select new AssetRequisitionVM
                              {
                                  AssetRequisitionId = e.IntAssetRequisitionId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  WorkPlaceGroupId = (long)emp.IntWorkplaceGroupId,
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  EmployeeId = e.IntEmployeeId,
                                  EmployeeName = emp.StrEmployeeName,
                                  EmployeeCode = emp.StrEmployeeCode,
                                  Designation = _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                  Department = _context.MasterDepartments.Where(x => x.IntDepartmentId == emp.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                  ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                  ReqisitionQuantity = e.IntReqisitionQuantity,
                                  ReqisitionDate = e.DteReqisitionDate,
                                  Remarks = e.StrRemarks,
                                  Active = e.IsActive,
                                  Status = e.StrStatus,
                                  PipelineHeaderId = e.IntPipelineHeaderId,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy
                              }).ToListAsync();
            return data;
        }

        public async Task<List<AssetRequisitionVM>> GetAssetRequisitionForSelf(long accountId, long businessUnitId, long? workplaceGroupId, long? workPlaceId, long? employeeId)
        {
            var data = await (from e in _context.AssetRequisitions
                              join emp in _context.EmpEmployeeBasicInfos on e.IntEmployeeId equals emp.IntEmployeeBasicInfoId
                              join i in _context.Items on e.IntItemId equals i.IntItemId
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && emp.IsActive == true
                              && e.IsActive == true && i.IsActive == true && e.IntReqisitionQuantity > 0
                              && (workplaceGroupId == 0 || emp.IntWorkplaceGroupId == workplaceGroupId)
                              && (workPlaceId == 0 || emp.IntWorkplaceId == workPlaceId)
                              && (employeeId == 0 || e.IntEmployeeId == employeeId)
                              orderby e.IntAssetRequisitionId descending
                              select new AssetRequisitionVM
                              {
                                  AssetRequisitionId = e.IntAssetRequisitionId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  EmployeeId = e.IntEmployeeId,
                                  EmployeeName = emp.StrEmployeeName,
                                  EmployeeCode = emp.StrEmployeeCode,
                                  Designation = _context.MasterDesignations.Where(x => x.IntDesignationId == emp.IntDesignationId).Select(x => x.StrDesignation).FirstOrDefault(),
                                  Department = _context.MasterDepartments.Where(x => x.IntDepartmentId == emp.IntDepartmentId).Select(x => x.StrDepartment).FirstOrDefault(),
                                  ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                  ReqisitionQuantity = e.IntReqisitionQuantity,
                                  ReqisitionDate = e.DteReqisitionDate,
                                  Remarks = e.StrRemarks,
                                  Active = e.IsActive,
                                  Status = e.StrStatus == "Approved By Admin" ? "Approved" : e.StrStatus == "Reject By Admin" ? "Rejected" : e.StrStatus,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy
                              }).ToListAsync();
            return data;
        }

        public async Task<AssetRequisitionVM> GetAssetRequisitionById(long accountId, long businessUnitId, long assetRequisitionId)
        {
            var data = await (from e in _context.AssetRequisitions
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && e.IntAssetRequisitionId == assetRequisitionId && e.IsActive == true
                              select new AssetRequisitionVM
                              {
                                  AssetRequisitionId = e.IntAssetRequisitionId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  EmployeeId = e.IntEmployeeId,
                                  ReqisitionQuantity = e.IntReqisitionQuantity,
                                  ReqisitionDate = e.DteReqisitionDate,
                                  Remarks = e.StrRemarks,
                                  Active = e.IsActive,
                                  Status = e.StrStatus,
                                  CreatedAt = e.DteCreatedAt,
                                  CreatedBy = e.IntCreatedBy,
                                  UpdatedAt = e.DteUpdatedAt,
                                  UpdatedBy = e.IntUpdatedBy
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<MessageHelper> AssetRequisitionDenied(AssetRequisitionVM obj)
        {
            try
            {
                MessageHelper msg = new();
                AssetRequisition dataParent = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == obj.AssetRequisitionId && x.IsActive == true);


                if (dataParent == null)
                {
                    msg.Message = "No Data Found";
                    msg.StatusCode = 200;
                    return msg;
                }
                else
                {
                    dataParent.IsDenied = true;
                    dataParent.StrStatus = "Denied";

                    _context.AssetRequisitions.Update(dataParent);
                    await _context.SaveChangesAsync();

                    msg.Message = "Asset Requisition Denied Successfully";
                    msg.StatusCode = 200;
                    return msg;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Asset Requisition

        #region Asset Transfer
        public async Task<MessageHelper> SaveAssetTransfer(AssetTransferVM obj)
        {
            try
            {
                MessageHelper msg = new();
                AssetTransfer dataParent = await _context.AssetTransfers.FirstOrDefaultAsync(x => x.IntAssetTransferId == obj.AssetTransferId && x.IsActive == true);
                //PipelineStageInfoVM stage = await _approvalPipelineService.GetCurrentNextStageAndPipelineHeaderId(obj.AccountId, "assetTransferApproval");
                var stage = await _approvalPipelineService.GetPipelineDetailsByEmloyeeIdNType((long)obj.ToEmployeeId, "assetTransferApproval");
                if (stage.HeaderId <= 0 || stage.CurrentStageId <= 0 || stage.NextStageId <= 0)
                {
                    msg.StatusCode = 400;
                    msg.Message = "Pipeline was not set";
                    return msg;
                }

                if (dataParent == null)
                {
                    AssetTransfer checkApproval = await _context.AssetTransfers.FirstOrDefaultAsync(x => x.IntFromEmployeeId == obj.FromEmployeeId && x.StrStatus == "Pending"
                    && x.IsPipelineClosed == false && x.IsActive == true);

                    if (checkApproval != null)
                    {
                        msg.Message = "From This Employee One Transfer Request Already Pending On Approval!";
                        msg.StatusCode = 409;
                        return msg;
                    }

                    else
                    {
                        AssetTransfer data = new AssetTransfer
                        {
                            IntAccountId = obj.AccountId,
                            IntBusinessUnitId = obj.BusinessUnitId,
                            IntFromEmployeeId = obj.FromEmployeeId,
                            IntItemId = obj.ItemId,
                            IntTransferQuantity = obj.TransferQuantity,
                            IntToEmployeeId = obj.ToEmployeeId,
                            DteTransferDate = obj.TransferDate,
                            StrRemarks = obj.Remarks,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now.GetCurrentDateTimeBD(),
                            IntCreatedBy = obj.CreatedBy,
                            IntPipelineHeaderId = stage.HeaderId,
                            IntCurrentStage = stage.CurrentStageId,
                            IntNextStage = stage.NextStageId,
                            StrStatus = "Pending",
                            IsPipelineClosed = false,
                            IsReject = false
                        };
                        await _context.AssetTransfers.AddAsync(data);
                        await _context.SaveChangesAsync();

                        msg.Message = "Created Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
                else
                {
                    dataParent.IntItemId = obj.ItemId;
                    dataParent.IntTransferQuantity = obj.TransferQuantity;
                    dataParent.IntToEmployeeId = obj.ToEmployeeId;
                    dataParent.DteTransferDate = obj.TransferDate;
                    dataParent.StrRemarks = obj.Remarks;
                    dataParent.IsActive = obj.IsActive;
                    dataParent.DteUpdatedAt = obj.UpdatedAt;
                    dataParent.IntUpdatedBy = obj.UpdatedBy;

                    _context.AssetTransfers.Update(dataParent);
                    await _context.SaveChangesAsync();

                    if (obj.AssetTransferId > 0 && obj.IsActive == false)
                    {
                        msg.Message = "Deleted Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                    else
                    {
                        msg.Message = "Updated Successfully";
                        msg.StatusCode = 200;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AssetTransferVM>> GetAssetTransfer(long accountId, long businessUnitId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await (from e in _context.AssetTransfers
                              join i in _context.Items on e.IntItemId equals i.IntItemId
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && (!fromDate.HasValue || e.DteTransferDate >= fromDate.Value.Date)
                              && (!toDate.HasValue || e.DteTransferDate <= toDate.Value.Date)
                              && e.IsActive == true && i.IsActive == true && e.IntTransferQuantity > 0
                              orderby e.IntAssetTransferId descending
                              select new AssetTransferVM
                              {
                                  AssetTransferId = e.IntAssetTransferId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  ItemUom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                  FromEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntFromEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  ToEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntToEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                  TransferQuantity = e.IntTransferQuantity,
                                  TransferDate = e.DteTransferDate,
                                  Remarks = e.StrRemarks,
                                  IsActive = e.IsActive,
                                  Status = e.StrStatus == "Approved By Admin" ? "Approved" : e.StrStatus == "Reject By Admin" ? "Rejected" : e.StrStatus
                              }).ToListAsync();
            return data;
        }

        public async Task<AssetTransferVM> GetAssetTransferById(long accountId, long businessUnitId, long assetTransferId)
        {
            var data = await (from e in _context.AssetTransfers
                              where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId
                              && e.IntAssetTransferId == assetTransferId && e.IsActive == true
                              select new AssetTransferVM
                              {
                                  AssetTransferId = e.IntAssetTransferId,
                                  AccountId = e.IntAccountId,
                                  BusinessUnitId = e.IntBusinessUnitId,
                                  FromEmployeeId = e.IntFromEmployeeId,
                                  FromEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntFromEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  ItemId = e.IntItemId,
                                  ItemName = _context.Items.Where(x => x.IntItemId == e.IntItemId).Select(x => x.StrItemName).FirstOrDefault(),
                                  TransferQuantity = e.IntTransferQuantity,
                                  ToEmployeeId = e.IntToEmployeeId,
                                  ToEmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntToEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                  TransferDate = e.DteTransferDate,
                                  Remarks = e.StrRemarks,
                                  IsActive = e.IsActive
                              }).FirstOrDefaultAsync();
            return data;
        }
        #endregion

        #region Asset List
        public async Task<List<AssetListVM>> GetAssetList(long accountId, long businessUnitId, long employeeId)
        {
            List<AssetListVM> assetList = new();

            List<AssetListVM> assetRequisition = await (from e in _context.AssetRequisitions
                                                        join i in _context.Items on e.IntItemId equals i.IntItemId
                                                        where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && e.IntEmployeeId == employeeId
                                                        && e.IsPipelineClosed == true && e.IsReject == false
                                                        && e.IsActive == true && i.IsActive == true
                                                        && e.IntReqisitionQuantity > 0
                                                        orderby e.IntAssetRequisitionId descending
                                                        select new AssetListVM
                                                        {
                                                            Id = e.IntAssetRequisitionId,
                                                            EmployeeName = "Own",
                                                            ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                                            ItemName = i.StrItemName,
                                                            Quantity = e.IntReqisitionQuantity,
                                                            Uom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                                            SourceType = "Requisition",
                                                            Date = e.DteReqisitionDate,
                                                            Status = e.StrStatus == "Approved By Admin" ? "Approved" : e.StrStatus
                                                        }).ToListAsync();
            assetList.AddRange(assetRequisition);

            List<AssetListVM> assetTransfer = await (from e in _context.AssetTransfers
                                                     join i in _context.Items on e.IntItemId equals i.IntItemId
                                                     where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && e.IntToEmployeeId == employeeId
                                                     && e.IsPipelineClosed == true && e.IsReject == false
                                                     && e.IsActive == true && i.IsActive == true
                                                     && e.IntTransferQuantity > 0
                                                     orderby e.IntAssetTransferId descending
                                                     select new AssetListVM
                                                     {
                                                         Id = e.IntAssetTransferId,
                                                         EmployeeName = _context.EmpEmployeeBasicInfos.Where(x => x.IntEmployeeBasicInfoId == e.IntFromEmployeeId).Select(x => x.StrEmployeeName).FirstOrDefault(),
                                                         ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                                         ItemName = i.StrItemName,
                                                         Quantity = e.IntTransferQuantity.Value,
                                                         Uom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                                         SourceType = "Transfer",
                                                         Date = e.DteTransferDate.Value,
                                                         Status = e.StrStatus == "Approved By Admin" ? "Approved" : e.StrStatus
                                                     }).ToListAsync();
            assetList.AddRange(assetTransfer);

            List<AssetListVM> assetDirect = await (from e in _context.AssetDirectAssigns
                                                   join i in _context.Items on e.IntItemId equals i.IntItemId
                                                   where e.IntAccountId == accountId && e.IntBusinessUnitId == businessUnitId && e.IntEmployeeId == employeeId
                                                   && e.IsActive == true && i.IsActive == true
                                                   && e.IntItemQuantity > 0
                                                   orderby e.IntAssetDirectAssignId descending
                                                   select new AssetListVM
                                                   {
                                                       Id = e.IntAssetDirectAssignId,
                                                       EmployeeName = "Admin",
                                                       ItemCategory = _context.ItemCategories.Where(x => x.IntItemCategoryId == i.IntItemCategoryId).Select(x => x.StrItemCategory).FirstOrDefault(),
                                                       ItemName = i.StrItemName,
                                                       Quantity = e.IntItemQuantity,
                                                       Uom = _context.ItemUoms.Where(x => x.IntItemUomId == i.IntItemUomId).Select(x => x.StrItemUom).FirstOrDefault(),
                                                       SourceType = "Direct",
                                                       Date = e.DteAssignDate,
                                                       Status = e.StrStatus ?? "Approved"
                                                   }).ToListAsync();
            assetList.AddRange(assetDirect);

            return assetList.OrderByDescending(x => x.Date).ToList();
        }

        public async Task<MessageHelper> AssetAcknowledged(AssetListVM obj)
        {
            try
            {
                MessageHelper msg = new();

                AssetRequisition assetRequisition = await _context.AssetRequisitions.FirstOrDefaultAsync(x => x.IntAssetRequisitionId == obj.Id && obj.SourceType == "Requisition" && x.IsActive == true);
                AssetTransfer assetTransfer = await _context.AssetTransfers.FirstOrDefaultAsync(x => x.IntAssetTransferId == obj.Id && obj.SourceType == "Transfer" && x.IsActive == true);
                AssetDirectAssign assetDirect = await _context.AssetDirectAssigns.FirstOrDefaultAsync(x => x.IntAssetDirectAssignId == obj.Id && obj.SourceType == "Direct" && x.IsActive == true);

                if (assetRequisition != null)
                {
                    assetRequisition.IsAcknowledged = true;
                    assetRequisition.StrStatus = "Acknowledged";

                    _context.AssetRequisitions.Update(assetRequisition);
                    await _context.SaveChangesAsync();
                }
                else if (assetTransfer != null)
                {
                    assetTransfer.IsAcknowledged = true;
                    assetTransfer.StrStatus = "Acknowledged";

                    _context.AssetTransfers.Update(assetTransfer);
                    await _context.SaveChangesAsync();
                }
                else if (assetDirect != null)
                {
                    assetDirect.IsAcknowledged = true;
                    assetDirect.StrStatus = "Acknowledged";

                    _context.AssetDirectAssigns.Update(assetDirect);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    msg.Message = "Data Not Found";
                    msg.StatusCode = 200;
                    return msg;
                }
                msg.Message = "Acknowledged Successfully";
                msg.StatusCode = 200;
                return msg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}