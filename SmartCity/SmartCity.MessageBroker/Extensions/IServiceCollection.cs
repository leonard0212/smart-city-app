using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.MessageBroker.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBusServices(this IServiceCollection services, IConfiguration config)
        {
            var queues = config["ServiceBus:Queues"]?.Split(",").ToList();
            var connectionString = config["ServiceBus:ConnectionString"];

            services.AddSingleton(new ServiceBusClient(connectionString));

            foreach (var q in queues)
            {
                services.AddSingleton<IHostedService>(sp =>
                {
                    var serviceBusClient = sp.GetRequiredService<ServiceBusClient>();
                    return new ServiceBusListenerService(q, serviceBusClient, sp);
                });
            }

            return services;
        }

    }
}
