using ManagementSystem.Api.Repositories.Implementation;
using ManagementSystem.Api.Repositories.Interfaces;
using ManagementSystem.Api.Services.Implementation;
using ManagementSystem.Api.Services.Interfaces;

namespace ManagementSystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserServices, UserService>();

        return services;
    }
}
