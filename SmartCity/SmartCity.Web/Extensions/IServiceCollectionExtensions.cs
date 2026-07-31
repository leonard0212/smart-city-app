using SmartCity.Core.Extensions;
using SmartCity.Core.Loggers;
using SmartCity.Domain.Settings;
using SmartCity.Interfaces.Loggers;

namespace SmartCity.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton(configuration);
            services.Configure<LoggingSettings>(configuration.GetSection("logging"));



            return services;
        }

        public static IServiceCollection AddWebUiCustomLogging(this IServiceCollection services)
        {
            services.AddCustomLogging();
            services.AddTransient<IWebLogger, WebLogger>();

            return services;
        }
    }
}
