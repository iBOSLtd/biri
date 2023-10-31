using Newtonsoft.Json;

namespace PeopleDesk.Models.PushNotify
{
    public class PushNotificationViewModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }
        }

        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";

        [JsonProperty("contentAvailable")]
        public bool ContentAvailable { get; set; }

        [JsonProperty("data")]
        public DataPayload Data { get; set; }

        [JsonProperty("notification")]
        public DataPayload Notification { get; set; }
    }
}