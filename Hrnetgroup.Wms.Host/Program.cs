using Hrnetgroup.Wms;
using Hrnetgroup.Wms.Application;
using Hrnetgroup.Wms.Application.Contracts;
using Hrnetgroup.Wms.Application.Contracts.Holidays;
using Hrnetgroup.Wms.Application.Contracts.Workers;
using Hrnetgroup.Wms.Domain.ApplicationUsers;
using Hrnetgroup.Wms.EntityframeworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddAuthAndSwagger(config);

// builder.Services.AddTransient<IWorkerAppService, WorkerAppService>();
// builder.Services.AddTransient<IHolidayAppService, HolidayAppService>();

builder.Services.AddDbContext(config);
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddServices();


// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<WmsDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();