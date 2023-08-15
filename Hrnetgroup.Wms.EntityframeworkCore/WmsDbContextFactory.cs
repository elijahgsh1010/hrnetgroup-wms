using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hrnetgroup.Wms.EntityframeworkCore;

public class WmsDbContextFactory : IDesignTimeDbContextFactory<WmsDbContext>
{
    public WmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WmsDbContext>();
        
        var configuration = BuildConfiguration();
            
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

        return new WmsDbContext(optionsBuilder.Options);
    }
    
    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Hrnetgroup.Wms.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}