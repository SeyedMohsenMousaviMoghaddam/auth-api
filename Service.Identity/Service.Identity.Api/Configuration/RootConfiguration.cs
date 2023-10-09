using PIS.Identity.API.Configuration;
using Service.Identity.Api.Configuration.Interfaces;

namespace Service.Identity.Api.Configuration;

public class RootConfiguration : IRootConfiguration
{
    public ApiConfiguration ApiConfiguration { get; } = new ();
}