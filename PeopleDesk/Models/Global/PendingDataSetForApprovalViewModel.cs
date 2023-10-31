namespace PeopleDesk.Models.Global
{
    public class PendingDataSetForApprovalViewModel
    {
        public long MenuId { get; set; }
        public string MenuName { get; set; }
        public string PipelineCode { get; set; }
        public long TotalCount { get; set; }
        public string? RouteUrl { get; set; }
    }

}
