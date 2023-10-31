using PeopleDesk.Models;
using System.Net;

namespace PeopleDesk.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                ApiError response = new ApiError();
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

                string message;
                var exceptiopnType = ex.GetType();

                if (exceptiopnType == typeof(Exception))
                {
                    message = ex.Message;
                }
                else
                {
                    message = "Something went wrong.";
                }

                if (_env.IsDevelopment())
                {
                    response.Message = ex.Message;
                    response.StatusCode = (int)httpStatusCode;
                    response.StackTrace = ex.StackTrace;
                }
                else
                {
                    response.Message = message;
                    response.StatusCode = (int)httpStatusCode;
                    response.StackTrace = ex.Message + " " + ex.StackTrace;
                }

                httpContext.Response.StatusCode = (int)httpStatusCode;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(response.ToJson());
            }
        }
    }
}