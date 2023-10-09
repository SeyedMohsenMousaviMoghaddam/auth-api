using System.Reflection;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Service.Identity.Infrastructure;
using Service.Identity.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Identity.Api;
using Service.Identity.Api.Helpers;
using Service.Identity.Application.Constants;
using Service.Identity.Domain.Roles;
using Service.Identity.Domain.Users;
using Serilog;
using Serilog.Events;


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
        true).AddUserSecrets<Program>(true).AddEnvironmentVariables()
    .Build();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", environment)
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Configuration.AddConfiguration(configuration);

var isDevelopment = environment == Environments.Development;
if (isDevelopment) builder.Configuration.AddUserSecrets<Program>(true);

StartupHelpers.BindConfiguration(builder.Services, builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.ApiPresentationInjection(builder.Configuration, builder.Environment);

builder.Services.AddAuthenticationServices<IdentityContext, User, Role>(builder.Configuration);

var rootConfiguration = StartupHelpers.CreateRootConfiguration(builder.Configuration);

builder.Services.ApplicationInjection(builder.Configuration, builder.Environment);

builder.Services.InfrastructureInjection(builder.Configuration, builder.Services, builder.Environment);

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});


builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(AuthenticationSchemaConsts.DefaultSchema,
        options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(isDevelopment
                        ? builder.Configuration["JWT:Secret"]
                        : Environment.GetEnvironmentVariable("SECRET")))
            };
        })
    .AddJwtBearer(AuthenticationSchemaConsts.TemporarySchema,
        options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        isDevelopment
                            ? builder.Configuration["TempJWT:Secret"]
                            : Environment.GetEnvironmentVariable("TEMP_SECRET")
                    ))
            };
        });

builder.Services.AddAuthorizationPolicies(rootConfiguration);


var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

var scope = scopeFactory.CreateScope();

var identityContextService = scope.ServiceProvider.GetRequiredService<IdentityContext>();

#region Initializing

identityContextService.Database.Migrate();

#endregion

app.UseRouting();
app.UseStaticFiles();
if (environment != "Production")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.InjectJavascript("/SwaggerUI/custom.js");
        options.RoutePrefix = string.Empty;
    });
}


if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSecurityHeaders(app.Configuration);
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
});
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseProblemDetails();
app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
});

app.Run();