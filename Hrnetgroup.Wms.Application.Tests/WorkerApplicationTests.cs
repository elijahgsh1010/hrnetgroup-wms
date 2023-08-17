using Hrnetgroup.Wms.Application.Contracts;
using Hrnetgroup.Wms.Application.Contracts.Workers;
using Hrnetgroup.Wms.Application.Contracts.Workers.Dtos;
using Hrnetgroup.Wms.Domain;
using Hrnetgroup.Wms.Domain.Holidays;
using Hrnetgroup.Wms.Domain.Leaves;
using Hrnetgroup.Wms.Domain.Workers;
using Hrnetgroup.Wms.EntityframeworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

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
        serviceCollection.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
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
    private IRepository<Worker> _workerRepository;
    private WmsDbContext _dbContext;

    public WorkerApplicationTests(DbFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        _workerAppService = fixture.ServiceProvider.GetService<IWorkerAppService>();
        _workerRepository = fixture.ServiceProvider.GetService<IRepository<Worker>>();
        _dbContext = fixture.ServiceProvider.GetService<WmsDbContext>();
    }
    
    [Fact]
    public async Task Should_Get_Correct_Worker_Information()
    {
        // arrange
        await _workerAppService.CreateWorker(new CreateWorkerInput()
        {
            WorkingDays = new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday },
            AmountPerHour = 9.5M,
            Name = "Freya",
            ContractStartDate = new DateTime(2021, 12, 18),
            NumOfHourPerDay = 8,
            TotalNumOfWorkingDays = 14
        });
        var workers = _dbContext.Workers.ToList();
        _dbContext.Leaves.Add(new Leave()
        {
            WorkerId = workers[0].Id,
            DateFrom = new DateTime(2021, 12, 27),
            DateTo = new DateTime(2021, 12, 30)
        });
        _dbContext.Leaves.Add(new Leave()
        {
            WorkerId = workers[0].Id,
            DateFrom = new DateTime(2022, 1, 1),
            DateTo = new DateTime(2022, 1, 1)
        });

        _dbContext.Holidays.Add(new Holiday()
        {
            Date = new DateTime(2021, 12, 25),
            Name = "Christmas"
        });
        
        _dbContext.Holidays.Add(new Holiday()
        {
            Date = new DateTime(2022, 1, 1),
            Name = "New Year"
        });
        
        
        _dbContext.SaveChanges();

        // act
        var result = await _workerAppService.GetWorkerInformation(workers[0].Id);

        // assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Freya");
        result.ExpectedEndDate.Date.ShouldBe(new DateTime(2022, 1, 22));
        result.TotalSalary.ShouldBe(1292);
        result.WorkingDays.ShouldContain("Wednesday");
        result.WorkingDays.ShouldContain("Friday");
        result.WorkingDays.ShouldContain("Saturday");
    }

    [Fact]
    public async Task Should_Not_Allow_To_Apply_Leave()
    {
        // arrange
        await _workerAppService.CreateWorker(new CreateWorkerInput()
        {
            WorkingDays = new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday },
            AmountPerHour = 9.5M,
            Name = "Freya",
            ContractStartDate = new DateTime(2021, 12, 18),
            NumOfHourPerDay = 8,
            TotalNumOfWorkingDays = 14
        });
        var workers = _dbContext.Workers.ToList();
        _dbContext.Leaves.Add(new Leave()
        {
            WorkerId = workers[0].Id,
            DateFrom = new DateTime(2021, 12, 25),
            DateTo = new DateTime(2021, 12, 30)
        });
        await _dbContext.SaveChangesAsync();
        
        // act
        Should.Throw<UserFriendlyException>(async () =>
        {
            await _workerAppService.ApplyLeave(new ApplyLeaveInput()
            {
                WorkerId = 1,
                DateFrom = new DateTime(2021, 12, 24),
                DateTo = new DateTime(2021, 12, 28)
            });
        });
        
        Should.Throw<UserFriendlyException>(async () =>
        {
            await _workerAppService.ApplyLeave(new ApplyLeaveInput()
            {
                WorkerId = 1,
                DateFrom = new DateTime(2021, 12, 27),
                DateTo = new DateTime(2021, 12, 28)
            });
        });
        
        Should.Throw<UserFriendlyException>(async () =>
        {
            await _workerAppService.ApplyLeave(new ApplyLeaveInput()
            {
                WorkerId = 1,
                DateFrom = new DateTime(2021, 12, 30),
                DateTo = new DateTime(2021, 12, 31)
            });
        });

        // clear
        var leaves = _dbContext.Leaves.ToList();
        _dbContext.Leaves.RemoveRange(leaves);
    }
}