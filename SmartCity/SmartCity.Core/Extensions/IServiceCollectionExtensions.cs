using SmartCity.Core.Loggers;
using SmartCity.Core.Security;
using SmartCity.Core.Services;
using SmartCity.Core.ValidatorsServices;
using SmartCity.Database;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Loggers;
using SmartCity.Interfaces.Security;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.ValidatorsServices;
using Microsoft.Extensions.DependencyInjection;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
using SmartCity.Core.Services.Common;
using SmartCity.Interfaces.Services.Common;

namespace SmartCity.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services

                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<ISecurityContext<User>, SecurityContext>()
                .AddScoped<IApplicationContext<User>, ApplicationContext>()
                .AddScoped<IMessageBrokerContext, MessageBrokerContext>()


                 //services
                 .AddScoped<IRawDetectionService, RawDetectionService>()
                 .AddScoped<IDetectionService, DetectionService>()
                 .AddScoped<IRoadInfrastructureService, RoadInfrastructureService>()
                 .AddScoped<ITrashService, TrashService>()
                 .AddScoped<ITrafficSignService, TrafficSignService>()
                 .AddScoped<IBillBoardService, BillBoardService>()
                 .AddScoped<IDetectionChatService, DetectionChatService>()
                 .AddScoped<IDetectionFlowLogService, DetectionFlowLogService>()

                 //managementServices
                 .AddTransient<IUserService, UserService>()
                 .AddScoped<IFileService, FileService>()
                 .AddScoped<ISystemProperyService, SystemProperyService>()
                 .AddScoped<ICacheService, CacheService>()
                 .AddScoped<IHttpClientService, HttpClientService>()
                 .AddTransient<INomenclatureService, NomenclatureService>()
                 .AddTransient<IDepartamentService, DepartamentService>()
                  .AddTransient<ILogsService, LogsService>()



                //validators
                .AddTransient<IUserServiceValidator, UserServiceValidator>()
                .AddTransient<IFileServiceValidator, FileServiceValidator>()


                .AddTransient<IAzureStorageService, AzureStorageService>()


                .AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
                });

            return services;
        }


        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            return services.AddTransient<IApplicationLogger, ApplicationLogger>()
                           .AddTransient<IDiagnosticsLogger, DiagnosticsLogger>()
                            .AddTransient<IOutRequestDataLogger, OutRequestDataLogger>()
                          .AddTransient<IMessageBrokerDataLogger, MessageBrokerDataLogger>();
        }
    }
}
