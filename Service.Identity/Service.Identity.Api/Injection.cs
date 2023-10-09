using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using Service.Identity.Api.Helpers;
using Service.Identity.Api.Util;
using Service.Identity.Api.Validation;
using Service.Identity.Application.Common;
using Service.Identity.Application.Configuration.Token;
using Service.Identity.Domain.Common;

namespace Service.Identity.Api;

public static class PresentationDependencyInjection
{
    public static IServiceCollection ApiPresentationInjection(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddProblems(configuration, env);
        services.AddDependantServices();
        if (env.EnvironmentName != "Production")
        {
            services.AddSwagger();
        }
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(
            c =>
            {
                c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        return serviceCollection;
    }

    private static IServiceCollection AddDependantServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        serviceCollection.AddScoped<IUserInfo, UserInfo>();
        return serviceCollection;
    }

    private static IServiceCollection AddProblems(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        bool activeExceptionDetails = Convert.ToBoolean(configuration["ActiveExceptionDetails"]);

        services.AddProblemDetails(x =>
        {
            x.IncludeExceptionDetails = (ctx, exp) => activeExceptionDetails || env.IsStaging();
            x.Map<MassTransit.RequestException>(ex => new InvalidCommandProblemDetails(ex, activeExceptionDetails));
            x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex, activeExceptionDetails));
            x.Map<ConflictException>(ex => new ConflictExceptionProblemDetails(ex));
            x.Map<NotFoundException>(ex => new NotFoundExceptionProblemDetails(ex));
            x.Map<BadRequestException>(ex => new BadRequestExceptionProblemDetails(ex));
            x.Map<UnprocessableEntityException>(ex => new UnprocessableEntityExceptionProblemDetails(ex));
            x.Map<ForbiddenException>(ex => new ForbiddenExceptionProblemDetails(ex));
        });

        return services;
    }
}