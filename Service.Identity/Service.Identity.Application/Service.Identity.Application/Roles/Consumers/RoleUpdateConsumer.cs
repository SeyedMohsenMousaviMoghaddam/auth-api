using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Common;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain.Configuration;
using Service.Identity.Domain.Roles;

namespace Service.Identity.Application.Roles.Consumers;

public class RoleUpdateConsumer : IConsumer<RoleUpdateRequestModel>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RoleUpdateConsumer(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<RoleUpdateRequestModel> context)
    {
        try
        {
            var request = context.Message;
            var cancellationToken = context.CancellationToken;

            var modelForUpdate = await _unitOfWork.Roles.Table.FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);
            if (modelForUpdate is null)
            {
                await context.RespondAsync<ConsumerRejected>(new
                {
                    StatusCode = ConsumerStatusCode.NotFound,
                    Errors = new[]
                    {
                        ConsumerMessage.NOTFOUND("Role")
                    }
                });
                return;
            }

            modelForUpdate.Name = request.Name;

            var mappedResult = _mapper.Map<RoleUpdateResponseModel>(modelForUpdate);

            await _unitOfWork.Roles.UpdateAsync(modelForUpdate, cancellationToken);
            await context.RespondAsync<ConsumerAccepted<RoleUpdateResponseModel>>(new
            {
                Data = mappedResult,
                StatusCode = ConsumerStatusCode.Success,
                Message = ConsumerMessage.UPDATE_SUCCESSFULLY("Role")
            });
        }
        catch (Exception ex)
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message
            });
        }
    }
}