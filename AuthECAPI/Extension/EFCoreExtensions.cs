using AuthECAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Extension;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDBContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DevConnection")));
        
        return services;
    }
}