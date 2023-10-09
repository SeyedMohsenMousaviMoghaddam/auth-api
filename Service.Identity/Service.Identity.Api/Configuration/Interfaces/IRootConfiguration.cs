using PIS.Identity.API.Configuration;

namespace Service.Identity.Api.Configuration.Interfaces;

public interface IRootConfiguration
{
    ApiConfiguration ApiConfiguration { get; }
}