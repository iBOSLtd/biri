using PeopleDesk.Data.Entity;

namespace PeopleDesk.Models.SignalR
{
    public class NotificationViewModel
    {
        public long? Id { get; set; }
        public long? BusinessUnitId { get; set; }
        public string? NotifyTitle { get; set; }
        public string? NotifyDetails { get; set; }
        public long? ModuleId { get; set; }
        public string? Module { get; set; }
        public string? Feature { get; set; }
        public bool? IsCommon { get; set; }
        public string? Receiver { get; set; }
        public bool? IsSeen { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsDelete { get; set; }
        public string? TimeDifference { get; set; }
        public NotificationMaster NotificationMaster { get; set; }
    }
}
