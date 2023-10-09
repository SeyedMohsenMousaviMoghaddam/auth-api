using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Service.Identity.Application.Configuration.Mapper;
using Service.Identity.Domain;
using Service.Identity.Application.Common;
using Service.Identity.Domain.Common;

namespace Service.Identity.Application;

public static class Injection
{
    public static IServiceCollection ApplicationInjection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddMassTransit(configuration,env);
        services.AddFluentValidators(configuration, env);
        services.AddMappingProfile();
        return services;
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services,IConfiguration configuration,IWebHostEnvironment env)
    {
        var IntegrateRequestClients = new List<Type>();
        var intervals = new[] { 2, 5, 10, 30, 60, 60 * 3, 60 * 5, 60 * 12, 60 * 24, 60 * 24 * 2 }.Select(s => TimeSpan.FromMinutes(s))
                                                                                                 .ToArray();

        const string domainCommandName = nameof(IContract);
        var assemblies = Assembly.GetExecutingAssembly();
        var integratedAssemblyName = (Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                                                                     .FirstOrDefault(f => f.Name != null && f.Name.Contains("Identity.Application.Integration")));

        if (integratedAssemblyName != null)
        {
            var integrationAssembly = Assembly.Load(integratedAssemblyName);
            IntegrateRequestClients = integrationAssembly.GetTypes()
                                                         .Where(t => t.GetInterface(domainCommandName) is not null && t.Name.Contains(domainCommandName) is false)
                                                         .Distinct()
                                                         .ToList();
        }
        
        var requestClients = assemblies.GetTypes()
                                       .Where(t => t.GetInterface(domainCommandName) is not null &&t.Name.Contains(domainCommandName) is false)
                                       .Distinct()
                                       .ToList();

        var integrationConsumers = Assembly.GetExecutingAssembly()
                                           .GetTypes()
                                           .Where(h => h.GetInterface("IIntegrateConsumer`1") is not null)
                                           .ToArray();

        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(Assembly.GetExecutingAssembly());
            requestClients.ForEach(message => { cfg.AddRequestClient(message); });
            cfg.ConfigureMediator((context, cfg) => { cfg.UseConsumeFilter(typeof(ValidationFilter<>), context); });
        });
        
        
        services.AddMassTransitHostedService(true);

        return services;
    }

    internal static IServiceCollection AddMappingProfile(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddSingleton(provider => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile(services.BuildServiceProvider().GetService<IUserInfo>()));
        }).CreateMapper());

        return services;
    }

    internal static IServiceCollection AddFluentValidators(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddFluentValidation((h => h.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly())));
        return services;
    }
}