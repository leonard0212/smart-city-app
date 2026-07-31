


using Microsoft.AspNetCore.Identity;
using SmartCity.Core.Extensions;
using SmartCity.Core.Loggers;
using SmartCity.Database.Extensions;
using SmartCity.Domain.Models.Users;
using SmartCity.MessageBroker.Extensions;
using SmartCity.Core.Middlewares;

var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false)
           .Build();
var nLogConfig = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");
Utils.ConfigureNLogConnectionString(nLogConfig.Configuration, config["database:logConnection"]);
var logger = nLogConfig.GetCurrentClassLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);
    var _configuration = builder.Configuration;
    builder.Services.AddControllers();

    builder.Services.AddMemoryCache();
    builder.Services.AddEntityFrameworkSqlServer().AddDbContexts(_configuration);
    builder.Services.AddIdentity<User, Role>().AddEfStores().AddDefaultTokenProviders();

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddRepositories();
    builder.Services.AddServices();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCustomLogging();

    var assemblyes = AppDomain.CurrentDomain.GetAssemblies();
    builder.Services.AddAutoMapper(assemblyes);

    builder.Services.AddServiceBusServices(_configuration);



    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseExceptionCatcher();

    app.UseSetupCorrelationID();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

