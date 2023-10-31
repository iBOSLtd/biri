namespace PeopleDesk.Models.Asset
{
    public class AssetServiceViewModel
    {
    }

    public class GetAssetDDLViewModel
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public string Code { get; set; }
    }

    public class AssetViewModel
    {
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; } = null!;
        public string? StrAssetCode { get; set; }
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class AssetTypeViewModel
    {
        public long IntAssetTypeId { get; set; }
        public string StrAssetTypeName { get; set; } = null!;
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class UomViewModel
    {
        public long IntUomId { get; set; }
        public string StrUomName { get; set; } = null!;
        public long IntAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class AssetRegisterViewModel
    {
        public long IntRegisterId { get; set; }
        public string StrRegisterCode { get; set; } = null!;
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; } = null!;
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long? IntUomId { get; set; }
        public string? StrUomName { get; set; }
        public long? IntAssetTypeId { get; set; }
        public string? StrAssetTypeName { get; set; }
        public string StrSpecification { get; set; } = null!;
        public DateTime DteAcquisitionDate { get; set; }
        public DateTime DteWarrantyDate { get; set; }
        public bool IsActive { get; set; }
        public long? IntAssignEmployeeId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }

        public string? StrAssetCode { get; set; }
        public int Quantity { get; set; }

    }
    public class AssetRequisitionViewModel
    {
        public long Sl { get; set; }
        public long IntId { get; set; }
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; } = null!;
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public long IntRequestQuantity { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteRequestDate { get; set; }
        public string StrRemarks { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsAprroved { get; set; }
        public long? IntApprovedQuantity { get; set; }
        public bool IsRejected { get; set; }
        public string Status { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class AssetRequisitionLandingPaginationViewModel
    {
        public List<AssetRequisitionViewModel> Data { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
    }
    public class AssetRequisitionApprovalViewModel
    {
        public long Id { get; set; }
        public bool IsApprove { get; set; }
        public long ApprovedQty { get; set; }
        public List<AssetAssignViewModel> AssignList { get; set; }
    }
    public class AssetAssignViewModel
    {
        public long Sl { get; set; }
        public long IntId { get; set; }
        public long IntAssetId { get; set; }
        public string StrAssetName { get; set; } = null!;
        public long IntAssetRegisterId { get; set; }
        public string StrRegisterCode { get; set; } = null!;
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public DateTime DteAssignDate { get; set; }
        public string StrRemarks { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? DteWithdrawDate { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
    public class AssetAssignLandingPaginationViewModel
    {
        public List<AssetAssignViewModel> Data { get; set; }
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
    }
}
