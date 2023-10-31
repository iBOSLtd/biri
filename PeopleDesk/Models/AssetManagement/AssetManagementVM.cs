namespace PeopleDesk.Models.AssetManagement
{
    public class AssetManagementVM
    {
    }

    public class ItemCategoryVM
    {
        public long ItemCategoryId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string ItemCategory { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
    }

    public class ItemUomVM
    {
        public long ItemUomId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string ItemUom { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
    }

    public class ItemVM
    {
        public long ItemId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string ItemCode { get; set; }
        public bool IsAutoCode { get; set; }
        public string ItemName { get; set; }
        public long ItemCategoryId { get; set; }
        public string ItemCategory { get; set; }
        public long ItemUomId { get; set; }
        public string ItemUom { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class AssetVM
    {
        public long AssetId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public string AssetCode { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUom { get; set; }
        public string ItemCategory { get; set; }
        public string SupplierName { get; set; }
        public string SupplierMobileNo { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public long? AcquisitionValue { get; set; }
        public long? InvoiceValue { get; set; }
        public long? DepreciationValue { get; set; }
        public DateTime? DepreciationDate { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class CommonDDLVM
    {
        public long Value { get; set; }
        public string Label { get; set; }
    }

    public class AssetTransferDDLVM
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public long Quantity { get; set; }
    }

    public class AssetDirectAssignVM
    {
        public long AssetDirectAssignId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkPlaceGroupId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUom { get; set; }
        public long ItemQuantity { get; set; }
        public DateTime AssignDate { get; set; }
        public bool Active { get; set; }
        public DateTime CreateAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public string Status { get; set; }
    }

    public class AssetRequisitionVM
    {
        public long AssetRequisitionId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long WorkPlaceGroupId { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUom { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string ItemCategory { get; set; }
        public long ReqisitionQuantity { get; set; }
        public DateTime ReqisitionDate { get; set; }
        public string Remarks { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PipelineHeaderId { get; set; }
        public long CurrentStage { get; set; }
        public long NextStage { get; set; }
        public string Status { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public long? RejectedBy { get; set; }
        public bool? IsDenied { get; set; }
    }

    public class AssetTransferVM
    {
        public long AssetTransferId { get; set; }
        public long AccountId { get; set; }
        public long BusinessUnitId { get; set; }
        public long? FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUom { get; set; }
        public long? TransferQuantity { get; set; }
        public long? ToEmployeeId { get; set; }
        public string ToEmployeeName { get; set; }
        public string ItemCategory { get; set; }
        public DateTime? TransferDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PipelineHeaderId { get; set; }
        public long CurrentStage { get; set; }
        public long NextStage { get; set; }
        public string Status { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public long? RejectedBy { get; set; }
    }

    public class AssetListVM
    {
        public long Id { get; set; }
        public string EmployeeName { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public long Quantity { get; set; }
        public string Uom { get; set; }
        public string SourceType { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
