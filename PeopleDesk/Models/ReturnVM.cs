namespace PeopleDesk.Models
{
    public class ReturnVM
    {
        public string Message { get; set; } = "Invalid Request";
        public int StatusCode { get; set; } = StatusCodes.Status403Forbidden;
        public long? KeyId { get; set; } = 0;
        public dynamic? Data { get; set; } = null;
    }
}