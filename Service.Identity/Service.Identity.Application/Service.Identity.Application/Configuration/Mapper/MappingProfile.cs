using AutoMapper;
using Service.Identity.Application.Common;
using Service.Identity.Domain.Common;

namespace Service.Identity.Application.Configuration.Mapper;

public class MappingProfile : Profile
{
    private readonly IUserInfo _userInfo;

    public MappingProfile(IUserInfo userInfo)
    {
        _userInfo = userInfo;

        var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                .SelectMany(asm => asm.GetModules())
                                                .Where(h => h.Name.Contains("Service") && h.Name.Contains("Application"))
                                                .ToList();

        var apiAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                   .SelectMany(asm => asm.GetModules())
                                                   .Where(h => h.Name.Contains("Service") && h.Name.Contains("Api"))
                                                   .ToList();

        applicationAssemblies.ForEach((asm) =>
        {
            var contracts = asm.GetTypes()
                               .Where(h => typeof(IContract).IsAssignableFrom(h));

            foreach (var contract in contracts)
            {
                var r = contract.BaseType != null && contract.BaseType.Name.Contains("IMapping")
                      ? contract.BaseType.GenericTypeArguments[0]
                      : null;

                if (r is null) continue;

                var instance = Activator.CreateInstance(contract);
                var methodInfo = contract.GetMethod("Mapping") ?? null;

                if (methodInfo != null && methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType)
                    methodInfo?.Invoke(instance, new object[] { this, _userInfo });
                else
                    CreateMap(r, contract).ReverseMap();
            }
        });

        apiAssemblies.ForEach((asm) =>
        {
            var types = asm.GetTypes()
                           .Where(h => typeof(IMap).IsAssignableFrom(h));

            foreach (var type in types)
            {
                var param = type.GetConstructors()
                                .Single()
                                .GetParameters()
                                .Select(p => (object)null)
                                .ToArray();

                var instance = Activator.CreateInstance(type, param);
                var methodInfo = type.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this, _userInfo });
            }
        });
    }
}