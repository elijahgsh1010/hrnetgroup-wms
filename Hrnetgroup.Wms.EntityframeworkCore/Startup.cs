using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrnetgroup.Wms.EntityframeworkCore;

public static class Startup
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        return services.AddDbContext<WmsDbContext>((sp, options) =>
        {
            options.UseSqlServer(config.GetSection("ConnectionStrings:Default").Value);
        });
    }
}