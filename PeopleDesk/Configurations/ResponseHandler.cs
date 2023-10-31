using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Helper;
using System.Net;

public class ResponseHandler : ControllerBase
{
    protected new IActionResult Ok<T>(Result<T> result) => OkTResult(result);

    protected new IActionResult Created<T>(Result<T> result) => CreatedTResult(result);

    protected IActionResult NotFound() => NotFoundTResult();
    protected IActionResult NotFound(string Message) => NotFoundTResult(Message);
    protected IActionResult BadRequest() => BadRequestTResult();
    protected IActionResult BadRequest(string Message) => BadRequestTResult(Message);


    private IActionResult BadRequestTResult(string Message = "Not Found") => base.BadRequest(new MsgHelper(Message, (long)HttpStatusCode.BadRequest));
    private IActionResult NotFoundTResult(string Message = "Not Found") => base.NotFound(new MsgHelper(Message, (long)HttpStatusCode.NotFound));

    private IActionResult OkTResult<T>(Result<T> result) => result.Match<IActionResult>(OnSuccessOk<T>(), OnFail());

    private IActionResult CreatedTResult<T>(Result<T> result) => result.Match<IActionResult>(OnSuccessCreated<T>(), OnFail());

    private Func<T, IActionResult> OnSuccessOk<T>() => success => base.Ok(success);

    private Func<T, IActionResult> OnSuccessCreated<T>() => success => base.Created(nameof(T), success);

    private Func<Exception, IActionResult> OnFail() => failure =>
    {
        var error = failure?.InnerException?.Message;
        var message = failure?.Message;
        return BadRequest(new MsgHelper(message, error, (int)HttpStatusCode.BadRequest));
    };

    private IActionResult CheckResponse(object value)
    {
        var data = (MsgHelper)value;

        return data.statuscode switch
        {
            (int)HttpStatusCode.Created => base.Created(string.Empty, value),
            (int)HttpStatusCode.BadRequest => base.BadRequest(value),
            (int)HttpStatusCode.NotFound => base.NotFound(value),
            (int)HttpStatusCode.InternalServerError => base.StatusCode((int)HttpStatusCode.InternalServerError, value),
            (int)HttpStatusCode.OK => base.Ok(value),
            _ => base.StatusCode((int)HttpStatusCode.InternalServerError, value)
        };
    }
}