using Elasticsearch.Net;
using Hrnetgroup.Wms;
using Hrnetgroup.Wms.Domain;
using Hrnetgroup.Wms.Domain.ApplicationUsers;
using Hrnetgroup.Wms.Domain.Workers;
using Hrnetgroup.Wms.EntityframeworkCore;
using Microsoft.AspNetCore.Identity;
using Nest;
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
builder.Services.AddScoped(typeof(Hrnetgroup.Wms.Application.Contracts.IRepository<>), typeof(EfRepository<>));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddServices();

var elasticClient = new ElasticClient(new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
    .BasicAuthentication("elastic", "123qwe")
    .CertificateFingerprint("27dd077b79abbfbdd29b43fe36077f25541b92ff59ee0557d04d31009c145a3f")
    .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
    .DefaultMappingFor<Worker>(i => i.IndexName("workers"))
    .EnableApiVersioningHeader()
    .DefaultIndex("workers"));


builder.Services.AddSingleton(elasticClient);
builder.Services.AddSingleton<IElasticClient, ElasticClient>();
builder.Services.AddScoped<IElasticManager, ElasticManager>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<WmsDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

app.UseCors("Default");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();