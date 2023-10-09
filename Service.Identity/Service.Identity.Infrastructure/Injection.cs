using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Identity.Domain.Configuration;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Infrastructure;

public static class Injection
{
    public static IServiceCollection InfrastructureInjection(this IServiceCollection services,
        IConfiguration configuration, IServiceCollection serviceCollection, IWebHostEnvironment environment)
    {
        services.AddSqlServerDatabase(configuration);
        services.AddHttpClient();
        services.AddServices();
        services.AddUnitOfWork();

        return services;
    }

    internal static IServiceCollection BindConfiguration<T>(this IServiceCollection services,
        IConfiguration configuration, string configurationName = null) where T : class
    {
        services.Configure<T>(config => configuration.GetSection(configurationName ?? typeof(T).Name).Bind(config));

        return services;
    }
    private static IServiceCollection AddSqlServerDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? configuration["ConnectionStrings:SqlConnection"]
            : Environment.GetEnvironmentVariable("SQL_CONNECTION");

        services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString, x =>
            x.MigrationsAssembly
                ("Service.Identity.Infrastructure")));

        return services;
    }

    private static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetModules())
            .Where(h => h.Name.Contains("Service") && h.Name.Contains("Infrastructure"))
            .ToList();

        assemblies.ForEach((asm) =>
        {
            var rd = asm.GetTypes().ToList();
            var servicesToInject = asm.GetTypes()
                .Where(h => h.IsClass && h.Name.Contains("Repository"))
                .ToList();

            foreach (var svc in servicesToInject)
            {
                var Isvc = svc.GetInterfaces().FirstOrDefault(h => h.Name.Contains(svc.Name));
                if (Isvc != null)
                    services.AddTransient(Isvc, svc);
            }
        });

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }
}