using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PeopleDesk.Models.SignalR
{
    [AllowAnonymous]
    public class NotificationHub : Hub
    {
    }
}
