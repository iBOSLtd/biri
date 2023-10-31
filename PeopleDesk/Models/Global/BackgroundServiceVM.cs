namespace PeopleDesk.Models.Global
{
    public class BackgroundServiceVM
    {
    }
    public class BackgrounNotifyVM
    {
        public long? AccountId { get; set; }
        public long? EmployeeId { get; set; }
        public long? IntReciverId { get; set; }
        public string? StrReceiverEmail { get; set; }
        public long? IntFeatureTableAutoId { get; set; } = 0;
        public string? NotifyTitle { get; set; }
        public string? NotifyDetails { get; set; }
    }

    public class BackgroundNotifyCommonVM
    {
        public string StrCategoryName { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntEmployeeId { get; set; }
        public long? IntReciverId { get; set; }
        public long IntModuleId { get; set; }
        public string StrModule { get; set; }
        public string StrFeature { get; set; }
        public string? StrReceiverEmail { get; set; }
        public long? IntFeatureTableAutoId { get; set; } = 0;
        public string? NotifyTitle { get; set; }
        public string? NotifyDetails { get; set; }
        public bool IsCommon { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
    }
}