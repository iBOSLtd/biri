using Newtonsoft.Json;

namespace PeopleDesk.Helper
{
    public class MsgHelper
    {

        public MsgHelper() { }
        public MsgHelper(string message, long status)
        {
            Message = message; statuscode = status;
        }
        public MsgHelper(string message, string error, long status) : this(message, status)
        {
            Error = error;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public long statuscode { get; set; }
        public string code { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
