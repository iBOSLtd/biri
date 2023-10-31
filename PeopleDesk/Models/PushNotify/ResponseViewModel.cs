using Newtonsoft.Json;

namespace PeopleDesk.Models.PushNotify
{
    public class ResponseViewModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
