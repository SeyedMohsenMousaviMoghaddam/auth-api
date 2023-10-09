using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.Identity.Api.Util;
using Service.Identity.Application.UserPolcies.Contracts;
using Service.Identity.Application.Users.Contracts;
using Service.Identity.Domain;
using Service.Identity.Domain.Common;
using Service.Identity.Domain.Enums;

namespace Service.Pharmacy.Api.Filters;

public class PolicyAttribute : TypeFilterAttribute
{
    public PolicyAttribute(string[] policies, AuthOperator authOperator = AuthOperator.AND) : base(
        typeof(OnAuthorization))
    {
        Arguments = new object[]
        {
            policies,
            authOperator
        };
    }

    private class OnAuthorization : IAsyncActionFilter
    {
        private readonly string[] _policies;
        private readonly AuthOperator _authOperator;
        private readonly IUserInfo _userInfo;
        private readonly IMediator _mediator;

        public OnAuthorization(string[] policies, AuthOperator authOperator, IUserInfo userInfo, IMediator mediator)
        {
            _policies = policies;
            _authOperator = authOperator;
            _userInfo = userInfo;
            _mediator = mediator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // var userCheckWhiteRequestClient = _mediator.CreateRequestClient<UserCheckWhiteRequest>();
            // var model = new UserCheckWhiteRequest();
            //
            // var userWhiteResponse = await userCheckWhiteRequestClient.GetResponse<UserCheckWhiteResponse>(model);
            //
            // if (userWhiteResponse is null || !userWhiteResponse.Message.IsWhite)
            // {
            //     context.Result = new UnauthorizedResult();
            //     return;
            // }

            var requestClient = _mediator.CreateRequestClient<UserHasPolicyRequestModel>();
            var query = new UserHasPolicyRequestModel()
            {
                UserId = _userInfo.UserId.Value,
                Policies = _policies,
                Condition = _authOperator.Equals(AuthOperator.AND) ? true : false,
            };

            var response = await requestClient.GetResponse<UserHasPolicyResponseModel>(query);
            if (!response.Message.UserHasPolicy)
            {
                context.Result = new AccessRestrictedResult();
                return;
            }

            await next();
        }
    }
}