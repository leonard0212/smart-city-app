using Azure.Messaging.ServiceBus;
using SmartCity.Core.Extensions;
using SmartCity.Core.Filters;
using SmartCity.Core.Loggers;
using SmartCity.Core.Middlewares;
using SmartCity.Database.Extensions;
using SmartCity.Domain.Models.Users;
using SmartCity.MessageBroker;
using SmartCity.MessageBroker.Extensions;
using SmartCity.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

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

    builder.Services.AddMemoryCache();


    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
    builder.Services.AddMvc(
              setup =>
              {
                  setup.Filters.Add(typeof(RequireHttpsAttribute));
                  setup.Filters.Add(typeof(StopWatchAttribute));
              })
        .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
        .AddDataAnnotationsLocalization()
        .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    });



    builder.Services.AddEntityFrameworkSqlServer().AddDbContexts(_configuration);
    builder.Services.AddIdentity<User, Role>(options =>
    {

     


        options.Lockout.MaxFailedAccessAttempts = int.Parse(_configuration["security:maxFailedAccessAttempts"]);
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(int.Parse(_configuration["security:defaultLockoutTimeSpan"]));
        options.Lockout.AllowedForNewUsers = true;

        options.Password.RequiredLength = 0;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
        .AddEfStores()
        .AddDefaultTokenProviders()
        .AddPasswordValidator<PasswordValidator<User>>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
       

        options.LoginPath = new PathString(builder.Configuration["security:loginUrl"]);
        options.LogoutPath = new PathString(builder.Configuration["security:logoutUrl"]);
        options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(builder.Configuration["security:timeout"]));
        options.Cookie.Name = builder.Configuration["security:cookieName"];
        options.AccessDeniedPath = "/Error/Code/403";

    });
    builder.Services.AddSession();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddSettings(_configuration);
    builder.Services.AddRepositories();
    builder.Services.AddServices();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddWebUiCustomLogging();

    var assemblyes = AppDomain.CurrentDomain.GetAssemblies();
    builder.Services.AddAutoMapper(assemblyes);

      



    var app = builder.Build();
   
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
   
    //app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCookiePolicy();
 

   


    app.UseSetupCorrelationID();


    app.UseSession();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();


    app.UseExceptionCatcher();

    app.UseResponseCaching();



    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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

