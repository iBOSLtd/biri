using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PeopleDesk.Models
{
    public class MessageHelper
    {
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
        public long? AutoId { get; set; }
    }
    public class MessageHelperCreate
    {
        public string Message { get; set; } = "Created Successfully";
        public int StatusCode { get; set; } = StatusCodes.Status201Created;
        public long? AutoId { get; set; }
    }
    public class MessageHelperUpdate
    {
        public string Message { get; set; } = "Update Successfully";
        public int StatusCode { get; set; } = StatusCodes.Status202Accepted;
        public long? AutoId { get; set; }
    }
    public class MessageHelperDelete
    {
        public string Message { get; set; } = "Delete Successfully";
        public int StatusCode { get; set; } = StatusCodes.Status202Accepted;
        public long? AutoId { get; set; }
    }
    public class MessageHelperAccessDenied
    {
        public string Message { get; set; } = "Access Denied";
        public int StatusCode { get; set; } = StatusCodes.Status403Forbidden;
        public long? AutoId { get; set; }
    }
    public class MessageHelperError
    {
        public string Message { get; set; } = "Internal Server Error";
        public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public long? AutoId { get; set; }
    }
    public class MessageHelperCustom
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public long? AutoId { get; set; }
    }
    public class ExceptionMessage
    {
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
    }
    public class CustomMessageHelper
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public long AutoId { get; set; }
        public string AutoName { get; set; }
    }
    public class ApiError
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string? StackTrace { get; set; }
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class MessageHelperBulkUpload
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<ErrorList>? ListData { get; set; }
    }
    public class ErrorList
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
    }

    public class MessageHelperThrow
    {
        public string Message { get; set; } = "";
        public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public long? AutoId { get; set; } = 0;
    }
    public static class CatchEx
    {
        public static MessageHelper CatchProcess(this Exception exception)
        {
            MessageHelper msg = new()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                AutoId = 0,
                Message = exception.Message,
            };
            return msg;

        }
    }
    public class MessageHelperWithValidation
    {
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
        public dynamic? ValidationData { get; set; }
    }
}
