using PeopleDesk.Data.Entity;
using PeopleDesk.Models.AssetManagement;

namespace PeopleDesk.Models.Auth
{
    public class ApprovalPipelineViewModel
    {
        public GlobalPipelineHeaderVM? GlobalPipelineHeader { get; set; }
        public List<GlobalPipelineRowViewModel>? GlobalPipelineRowList { get; set; }
    }
    public class GlobalPipelineRowViewModel
    {
        public GlobalPipelineRow? GlobalPipelineRow { get; set; }
        public UserGroupHeader? UserGroupHeader { get; set; }
    }
    public class ApprovalPipelineHeaderViewModel : Base
    {
        public long? IntPipelineHeaderId { get; set; }
        public string StrPipelineName { get; set; } = null!;
        public string StrApplicationType { get; set; } = null!;
        public string? StrRemarks { get; set; }
        public long? IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; } = -1;
        public long IntWorkplaceGroupId { get; set; } = -1;
        public long IntWorkplaceId { get; set; } = -1;
        public long IntWingId { get; set; } = -1;
        public long IntSoleDepoId { get; set; } = -1;
        public long IntRegionId { get; set; } = -1;
        public long IntAreaId { get; set; } = -1;
        public long IntTerritoryId { get; set; } = -1;
        public bool? IsValidate { get; set; }
        public List<ApprovalPipelineRowViewModel>? ApprovalPipelineRowViewModelList { get; set; }
    }
    public class ApprovalPipelineRowViewModel
    {
        public long? IntPipelineRowId { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsLineManager { get; set; }
        public long? IntUserGroupHeaderId { get; set; }
        public long IntShortOrder { get; set; }
        public string StrStatusTitle { get; set; }
        public bool IsCreate { get; set; }
        public bool IsDelete { get; set; }
    }
    public class ApprovalPipelineLandingFilterVM 
    {
        public long BusinessUnitId { get; set; }
        public long WorkplaceGroupId { get; set; }
        public long? WorkplaceId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsPaginated { get; set; }
        public bool IsHeaderNeed { get; set; }
        public string? SearchTxt { get; set; }
        public List<long>? WingNameList { get; set; }
        public List<long>? SoleDepoNameList { get; set; }
        public List<long>? RegionNameList { get; set; }
        public List<long>? AreaNameList { get; set; }
        public List<long>? TerritoryNameList { get; set; }
    }

    public class ApprovalPipelineLandingFilterWithHeader : PaginationBaseVM
    {
        public dynamic Data { get; set; }
        public PipelineLandingHeader PipelineLandingHeader { get; set; }
    }

    public class PipelineLandingHeader
    {
        public IList<CommonDDLVM> WingNameList { get; set; }
        public IList<CommonDDLVM> SoleDepoNameList { get; set; }
        public IList<CommonDDLVM> RegionNameList { get; set; }
        public IList<CommonDDLVM> AreaNameList { get; set; }
        public IList<CommonDDLVM> TerritoryNameList { get; set; }
    }

    public partial class GlobalPipelineHeaderVM
    {
        public long IntPipelineHeaderId { get; set; }
        public string StrPipelineName { get; set; }
        public string StrApplicationType { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public string strWorkPlaceGroupName { get; set; }
        public long IntWorkplaceId { get; set; }
        public string strWorkPlaceName { get; set; }
        public long IntWingId { get; set; }
        public string Wing { get; set; }
        public long IntSoleDepoId { get; set; }
        public string SoleDepo { get; set; }
        public long IntRegionId { get; set; }
        public string Reagion { get; set; }
        public long IntAreaId { get; set; }
        public string Area { get; set; }
        public long IntTerritoryId { get; set; }
        public string Territory { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

}
