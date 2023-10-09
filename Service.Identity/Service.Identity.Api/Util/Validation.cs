using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Common;

namespace Service.Identity.Api.Util;
public class GenericResult<T> : IActionResult
    where T : class
{
    private readonly Task<Response<T>>? _accepted;
    private readonly Task<Response<ConsumerRejected>> _rejected;

    public GenericResult(Task<Response<T>>? accepted, Task<Response<ConsumerRejected>> consumerRejected)
    {
        _accepted = accepted;
        _rejected = consumerRejected;
    }


    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (_accepted != null)
        {
            if (_accepted.IsCompletedSuccessfully)
            {
                var response = await _accepted;
                var objectResult = new ObjectResult(response.Message);
                await objectResult.ExecuteResultAsync(context);
            }
            else
            {
                var response = await _rejected;
                var obj = new
                {
                    Reason = response.Message.Reason,
                    Errors = response.Message.Errors
                };
                ObjectResult objectResult = response.Message.StatusCode switch
                {
                    ConsumerStatusCode.Success => new OkObjectResult(response.Message.Reason),
                    ConsumerStatusCode.UnAuthorized => new UnauthorizedObjectResult(obj),
                    ConsumerStatusCode.Conflict => new ConflictObjectResult(obj),
                    ConsumerStatusCode.NotFound => new NotFoundObjectResult(obj),
                    ConsumerStatusCode.AccessRestrict => new ObjectResult(obj) { StatusCode = 403 },
                    _ => new BadRequestObjectResult(obj)
                };

                await objectResult.ExecuteResultAsync(context);
            }
        }
    }
}