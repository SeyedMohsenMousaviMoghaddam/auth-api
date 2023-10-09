using AutoMapper;
using Service.Identity.Application.Common;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Service.Identity.Application.Roles.Contracts;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.Common;

namespace Service.Identity.Application.Roles.Consumers;

public class RoleCreateConsumer : IConsumer<RoleCreateRequestModel>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    private readonly IUserInfo _userInfo;

    public RoleCreateConsumer(RoleManager<Role> roleManager, IMapper mapper, IUserInfo userInfo)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _userInfo = userInfo;
    }

    public async Task Consume(ConsumeContext<RoleCreateRequestModel> context)
    {
        var request = context.Message;
        var createModel = _mapper.Map<Role>(request);
        createModel.CreatedById = _userInfo.UserId.Value;
        createModel.CreatedDate = DateTime.Now;

        var result = await _roleManager.CreateAsync(createModel);
        var mappedResult = _mapper.Map<RoleCreateResponseModel>(createModel);

        if (result.Succeeded)
        {
            await context.RespondAsync<ConsumerAccepted<RoleCreateResponseModel>>(new
            {
                Data = mappedResult,
                StatusCode = ConsumerStatusCode.Success,
                Message = ConsumerMessage.CREATE_SUCCESSFULLY("Role")
            });
        }
        else
        {
            await context.RespondAsync<ConsumerRejected>(new
            {
                StatusCode = ConsumerStatusCode.BadRequest,
                Errors = new[]
                {
                    "can_not_create_role"
                }
            });
        }
    }
}