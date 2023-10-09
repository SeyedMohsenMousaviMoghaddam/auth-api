using Service.Identity.Application.Common;
using MassTransit;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain.Configuration;

namespace Service.Identity.Application.Users.Consumers;

public class UserCheckWhiteConsumer : IConsumer<UserCheckWhiteRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public UserCheckWhiteConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserCheckWhiteRequest> context)
    {
        // try
        // {
        //     var result = await _unitOfWork.Users.IsUserWhite(context.CancellationToken);
        //     await context.RespondAsync<UserCheckWhiteResponse>(new
        //     {
        //         IsWhite = result
        //     });
        // }
        // catch (Exception ex)
        // {
        //     await context.RespondAsync<ConsumerRejected>(new
        //     {
        //         StatusCode = ConsumerStatusCode.BadRequest,
        //         Reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message
        //}
        //     });
        // }
    }
}