using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Service.Identity.Api.Configuration;
using Service.Identity.Api.Configuration.Interfaces;
using Service.Identity.Application.Constants;
using Service.Identity.Application.Users.Contracts.Validators;
using Service.Identity.Domain.Users;
using Service.Identity.Infrastructure.Configuration;

namespace Service.Identity.Api.Helpers;

public static class StartupHelpers
{
    public static void UseSecurityHeaders(this IApplicationBuilder app, IConfiguration configuration)
    {
        var forwardingOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        };

        forwardingOptions.KnownNetworks.Clear();
        forwardingOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardingOptions);

        app.UseReferrerPolicy(options => options.NoReferrer());
    }

    public static void AddAuthenticationServices<TIdentityDbContext, TUserIdentity, TUserIdentityRole>(this IServiceCollection services, IConfiguration configuration) where TIdentityDbContext : DbContext
        where TUserIdentity : class
        where TUserIdentityRole : class
    {
        var identityOptions = configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>();
        services.AddSingleton(identityOptions)
                .AddIdentity<TUserIdentity, TUserIdentityRole>(options => { configuration.GetSection(nameof(IdentityOptions)).Bind(options); })
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();

        services.Replace(ServiceDescriptor.Scoped<IUserValidator<User>, UserCreateUsernameValidator<User>>());
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services, IRootConfiguration rootConfiguration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationConsts.AdministrationPolicy, policy => policy.RequireRole(rootConfiguration.ApiConfiguration.AdministrationRole));
        });
    }

    public static IRootConfiguration CreateRootConfiguration(IConfiguration configuration)
    {
        var rootConfiguration = new RootConfiguration();
        
        configuration.GetSection(ConfigurationConsts.ApiConfigurationKey).Bind(rootConfiguration.ApiConfiguration);
        
        return rootConfiguration;
    }

    public static void BindConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var identityData = new IdentityData();
        
        configuration.GetSection(nameof(IdentityData)).Bind(identityData);
        
        services.AddSingleton(identityData);
    }
}