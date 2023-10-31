namespace PeopleDesk.Models.Task
{
    public class TaskViewModel
    {
        public TskProjectViewModel TskProjectViewModel { get; set; }
        public TskGroupMemberViewModel TskGroupMemberViewModel { get; set; }
        public TskBoardViewModel TskBoardViewModel { get; set; }
        public TskTaskDetailViewModel TskTaskDetailViewModel { get; set; }
    }
    public class TskProjectViewModel : Base
    {
        public long IntProjectId { get; set; }
        public string StrProjectName { get; set; } = null!;
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntFileUrlId { get; set; }
        public bool IsActivate { get; set; }
        public long IntAccountId { get; set; }
        public string? StrStatus { get; set; }
        public string? StrDescription { get; set; }

        public List<TskBoardViewModel> TskBoardViewModelList { get; set; }
        public List<TskGroupMemberViewModel> TskGroupMemberViewModelList { get; set; }
    }
    public class TskGroupMemberViewModel : Base
    {
        public long IntGroupMemberId { get; set; }
        public long IntAutoId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; } = null!;
        public bool IsDelete { get; set; }
        public long? IntGroupMemberTypeId { get; set; }
    }
    public class TskBoardViewModel : Base
    {
        public long IntBoardId { get; set; }
        public long IntProjectId { get; set; }
        public string StrBoardName { get; set; } = null!;
        public string StrDescription { get; set; } = null!;
        public DateTime DteStartDate { get; set; }
        public DateTime DteEndDate { get; set; }
        public long IntReporterId { get; set; }
        public long IntFileUrlId { get; set; }
        public string StrPriority { get; set; } = null!;
        public string StrBackgroundColor { get; set; } = null!;
        public string StrHtmlColorCode { get; set; } = null!;
        public string StrStatus { get; set; } = null!;

        public List<TskTaskDetailViewModel> TskTaskDetailViewModelList { get; set; }
        public List<TskGroupMemberViewModel> TskGroupMemberViewModelList { get; set; }

    }
    public class TskTaskDetailViewModel : Base
    {
        public long IntTaskDetailsId { get; set; }
        public long IntProjectId { get; set; }
        public long IntBoardId { get; set; }
        public string StrTaskTitle { get; set; } = null!;
        public string StrTaskDescription { get; set; } = null!;
        public string StrStatus { get; set; } = null!;

        public List<TskGroupMemberViewModel> TskGroupMemberViewModelList { get; set; }
    }
    public class TskProjectCreateViewModel : Base
    {
        public long IntProjectId { get; set; }
        public string StrProjectName { get; set; }
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntFileUrlId { get; set; }
        public long IntAccountId { get; set; }
        public string? StrStatus { get; set; }
        public string? StrDescription { get; set; }
        public List<GroupMemberViewModel> GroupMemberIdNameList { get; set; }
    }
    public class TskBoardCreateViewModel : Base
    {
        public long IntProjectId { get; set; }
        public long IntBoardId { get; set; }
        public string StrBoardName { get; set; }
        public string StrDescription { get; set; } = null!;
        public DateTime DteStartDate { get; set; }
        public DateTime DteEndDate { get; set; }
        public string StrPriority { get; set; } = null!;
        public long IntReporterId { get; set; }
        public string StrBackgroundColor { get; set; } = null!;
        public string StrHtmlColorCode { get; set; } = null!;
        public long IntFileUrlId { get; set; }
        public string StrStatus { get; set; } = null!;
        public List<GroupMemberViewModel> GroupMemberIdNameList { get; set; }

    }
    public class TskTaskCreateViewModel : Base
    {
        public long IntTaskDetailsId { get; set; }
        public long IntProjectId { get; set; }
        public long IntBoardId { get; set; }
        public string StrTaskTitle { get; set; } = null!;
        public string StrTaskDescription { get; set; } = null!;
        public string StrStatus { get; set; } = null!;
        public List<GroupMemberViewModel> GroupMemberIdNameList { get; set; }
    }
    public class GroupMemberViewModel : Base
    {
        public long IntGroupMemberId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public bool IsDelete { get; set; }
    }


    #region  ====================  SP View Model  ====================

    public class ProjectCreateSPViewModel : Base
    {
        public string StrPartType { get; set; }
        public long IntProjectId { get; set; }
        public string StrProjectName { get; set; } = null!;
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntFileUrlId { get; set; }
        public bool IsActivate { get; set; }
        public long IntAccountId { get; set; }
        public string? StrStatus { get; set; }
        public string? StrDescription { get; set; }
    }
    public class BoardCreateSPViewModel : Base
    {
        public string StrPartType { get; set; }
        public long IntBoardId { get; set; }
        public long IntProjectId { get; set; }
        public string StrBoardName { get; set; } = null!;
        public string StrDescription { get; set; } = null!;
        public DateTime DteStartDate { get; set; }
        public DateTime DteEndDate { get; set; }
        public long IntReporterId { get; set; }
        public long IntFileUrlId { get; set; }
        public string StrPriority { get; set; } = null!;
        public string StrBackgroundColor { get; set; } = null!;
        public string StrHtmlColorCode { get; set; } = null!;
        public string StrStatus { get; set; } = null!;

    }
    public class TaskCreateSPViewModel : Base
    {
        public string StrPartType { get; set; }
        public long IntTaskDetailsId { get; set; }
        public long IntProjectId { get; set; }
        public long IntBoardId { get; set; }
        public string StrTaskTitle { get; set; } = null!;
        public string StrTaskDescription { get; set; } = null!;
        public string StrStatus { get; set; } = null!;

    }

    #endregion

}
