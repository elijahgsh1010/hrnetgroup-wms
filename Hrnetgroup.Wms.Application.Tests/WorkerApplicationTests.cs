using Hrnetgroup.Wms.Application.Contracts.Workers;
using Hrnetgroup.Wms.EntityframeworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Hrnetgroup.Wms.Application.Tests;

public class DbFixture
{
    public ServiceProvider ServiceProvider { get; private set; }
    
    public DbFixture()
    {
        var serviceCollection = new ServiceCollection();
        var connection = CreateDatabaseAndGetConnection();
        serviceCollection
            .AddDbContext<WmsDbContext>(options => options.UseSqlite(connection),
                ServiceLifetime.Singleton);

        serviceCollection.AddScoped<IWorkerAppService, WorkerAppService>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
    
    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new WmsDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }
}

public class WorkerApplicationTests : IClassFixture<DbFixture>
{
    private ServiceProvider _serviceProvider;
    private IWorkerAppService _workerAppService;
    
    public WorkerApplicationTests(DbFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        _workerAppService = fixture.ServiceProvider.GetService<IWorkerAppService>();
    }
    
    [Fact]
    public async Task Should_Create_Worker()
    {
        
    }
}