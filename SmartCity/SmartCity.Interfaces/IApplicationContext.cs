using AutoMapper;
using SmartCity.Interfaces.Loggers;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Hosting;

namespace SmartCity.Interfaces
{
    public interface IApplicationContext<TUser>
    {
        ISecurityContext<TUser> SecurityContext { get; }

        IMapper Mapper { get; }
        IWebHostEnvironment HostingEnvironment { get; }

        IApplicationLogger ApplicationLogger { get; }

        IUnitOfWork UnitOfWork { get; }
    }
}
