using AutoMapper;
using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces
{
    public interface IMessageBrokerContext
    {
        IMapper Mapper { get; }
        IWebHostEnvironment HostingEnvironment { get; }
        IApplicationLogger ApplicationLogger { get; }
        IMessageBrokerDataLogger MessageBrokerDataLogger { get; }
    }
}
