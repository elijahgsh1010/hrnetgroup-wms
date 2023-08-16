using Hrnetgroup.Wms.EntityframeworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Hrnetgroup.Wms.DbMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
        
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<Test>();
                services.AddSingleton<WmsDbContext>();
                var configuration = BuildConfiguration();
                services.AddDbContext<WmsDbContext>((sp, options) =>
                {
                    options.UseSqlServer(configuration.GetSection("ConnectionStrings:Default").Value);
                });
            })
            .Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<WmsDbContext>();
                context.Database.Migrate(); // apply all migrations
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
            }
        }
    }
    
    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            // .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"))
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
