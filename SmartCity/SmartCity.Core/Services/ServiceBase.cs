using AutoMapper;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces.Loggers;
using SmartCity.Interfaces.Security;
using SmartCity.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace SmartCity.Core.Services
{
    public abstract class ServiceBase
    {
        protected ServiceBase(IApplicationContext<User> applicationContext)
        {
            ApplicationContext = applicationContext;
            ApplicationLogger = applicationContext.ApplicationLogger;
        }

        private IApplicationContext<User> ApplicationContext { get; }

        public IApplicationLogger ApplicationLogger { get; }
        protected ISecurityContext<User> SecurityContext => ApplicationContext.SecurityContext;
        protected IMapper Mapper => ApplicationContext.Mapper;

        protected IWebHostEnvironment HostingEnvironment => ApplicationContext.HostingEnvironment;

        protected IUnitOfWork UnitOfWork => ApplicationContext.UnitOfWork;

    }
}
