using AutoMapper;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core
{
    public class MessageBrokerContext : IMessageBrokerContext
    {
        public IMapper Mapper { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public IApplicationLogger ApplicationLogger { get; }
        public IMessageBrokerDataLogger MessageBrokerDataLogger { get; }
        public MessageBrokerContext(IMapper mapper
            , IWebHostEnvironment hostingEnvironment
            , IApplicationLogger applicationLogger
            , IMessageBrokerDataLogger messageBrokerDataLogger)
        {
            Mapper = mapper;
            HostingEnvironment = hostingEnvironment;
            ApplicationLogger = applicationLogger;
            MessageBrokerDataLogger = messageBrokerDataLogger;
        }
    }
}
