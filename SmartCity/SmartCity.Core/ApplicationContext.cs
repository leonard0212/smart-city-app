using AutoMapper;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces.Loggers;
using SmartCity.Interfaces.Security;
using SmartCity.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace SmartCity.Core
{
    public class ApplicationContext : IApplicationContext<User>
    {

        public ISecurityContext<User> SecurityContext { get; }
        public IApplicationLogger ApplicationLogger { get; }

        public IWebHostEnvironment HostingEnvironment { get; }
        public IUnitOfWork UnitOfWork { get; }

        public IMapper Mapper { get; }
        public ApplicationContext(
            ISecurityContext<User> securityContext,
            IApplicationLogger applicationLogger,
            IWebHostEnvironment hostingEnvironment,
            IMapper mapper,
            IUnitOfWork unitOfWork
           )
        {
            SecurityContext = securityContext;
            ApplicationLogger = applicationLogger;
            HostingEnvironment = hostingEnvironment;
            Mapper = mapper;
            UnitOfWork = unitOfWork;
        }





    }
}
