using Azure.Messaging.ServiceBus;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCity.Interfaces.Services;

using SmartCity.Domain.Models.Entities;
using SmartCity.Interfaces.Repository;

namespace SmartCity.MessageBroker
{
    public class ServiceBusListenerService : BackgroundService
    {
        private readonly string _queueName;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IMessageBrokerContext _messageBrokerContext;
        private readonly IMessageBrokerDataLogger _messageBrokerDataLogger;
        private readonly IApplicationLogger _applicationLogger;
        private readonly IServiceProvider _serviceProvider;
        public ServiceBusListenerService(string queueName, ServiceBusClient serviceBusClient, IServiceProvider serviceProvider)
        {
            _queueName = queueName;
            _serviceBusClient = serviceBusClient;
            _serviceProvider = serviceProvider;
            using (var scope = serviceProvider.CreateScope())
            {
                _messageBrokerContext = scope.ServiceProvider.GetRequiredService<IMessageBrokerContext>();
                _messageBrokerDataLogger = _messageBrokerContext.MessageBrokerDataLogger;
                _applicationLogger = _messageBrokerContext.ApplicationLogger;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _applicationLogger.LogInfo($"Starting listener for queue: {_queueName}");
            var processor = _serviceBusClient.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += async args =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var rawDetectionService = scope.ServiceProvider.GetRequiredService<IRawDetectionService>();
                        _messageBrokerDataLogger.LogRequest(args.Message, _queueName);

                        if (_queueName == "test-queue")
                            await rawDetectionService.CreateRawDetectionAsync(args.Message.Body.ToString(), args?.Message?.MessageId);
                        await args.CompleteMessageAsync(args.Message);
                    }
                }
                catch (Exception ex)
                {
                    _applicationLogger.LogError(ex, $"Error processing message from queue {_queueName}");
                }
            };

            processor.ProcessErrorAsync += args =>
            {
                _applicationLogger.LogError(args.Exception, $"Error in Service Bus Processor for queue {_queueName}");
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);

            await processor.StopProcessingAsync(stoppingToken);
            _applicationLogger.LogInfo($"Stopped listener for queue: {_queueName}");
        }
    }

}
