using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Microsoft.Extensions.Logging;
using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static MassTransit.Logging.DiagnosticHeaders;

namespace Service.Identity.Application.Common
{
    public class ValidationFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        private readonly IEnumerable<IValidator<TMessage>> _validators;
        private readonly ILogger<ValidationFilter<TMessage>> _logger;

        public ValidationFilter(IEnumerable<IValidator<TMessage>> validators, ILogger<ValidationFilter<TMessage>> logger)
        {
            _validators = validators;
            _logger = logger;
        }



        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var request = context.Message;
            var cancellation = context.CancellationToken;

            if (_validators.Any())
            {
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(request, cancellation)));
                var errors = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (errors.Count != 0)
                {
                    if (errors.Any())
                    {
                        var conflictErrors = errors.Where(d => d.ErrorCode == HttpStatusCode.Conflict.ToString("D"))
                            .ToList();
                        if (conflictErrors.Any())
                        {
                            throw new ConflictException(conflictErrors.AsDictionary());
                        }

                        var notFoundErrors = errors.Where(d => d.ErrorCode == HttpStatusCode.NotFound.ToString("D"))
                            .ToList();
                        if (notFoundErrors.Any())
                        {
                            throw new NotFoundException(notFoundErrors.AsDictionary());
                        }

                        var badRequestErrors = errors.Where(d => d.ErrorCode == HttpStatusCode.BadRequest.ToString("D"))
                            .ToList();
                        if (badRequestErrors.Any())
                        {
                            throw new BadRequestException(badRequestErrors.AsDictionary());
                        }

                        var forbiddenErrors = errors.Where(d => d.ErrorCode == HttpStatusCode.Forbidden.ToString("D"))
                            .ToList();
                        if (forbiddenErrors.Any())
                        {
                            throw new ForbiddenException(forbiddenErrors.AsDictionary());
                        }

                        throw new InvalidCommandException(errors.AsDictionary());
                    }

                    _logger.LogInformation("Request Not Validate: {@context}", context);
                    throw new ValidationException(errors);
                }
            }

            await next.Send(context);
        }
    }
    
}
