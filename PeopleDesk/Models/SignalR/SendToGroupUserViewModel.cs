namespace PeopleDesk.Models.SignalR
{
    public class SendToGroupUserViewModel
    {
        public long AccountId { get; set; }
        public List<long> EmployeeIdList { get; set; }
    }
}
