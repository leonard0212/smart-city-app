using SmartCity.Database.Stores;
using SmartCity.Interfaces.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartCity.Database.Extensions
{
    public static class DbExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlServer(config["database:connection"]);
                })
              .AddDbContext<LogDatabaseContext>(options =>
               {
                   options.UseSqlServer(config["database:logConnection"]);
               });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddTransient(typeof(IGenericRepositorySimpleLong<>), typeof(GenericRepositorySimpleLong<>))
                 .AddTransient(typeof(IGenericRepositoryUniqueIdentifier<>), typeof(GenericRepositoryUniqueIdentifier<>))
                 .AddTransient(typeof(IGenericRepositorySimpleUniqueIdentifier<>), typeof(GenericRepositorySimpleUniqueIdentifier<>))
                 .AddTransient<IDetectionRepository, DetectionRepository>(); ;
        }

        public static async Task<IList<T>> ToListReadUncommittedAsync<T>(this IQueryable<T> query, DbContext db)
        {
            db.Database.OpenConnection();
            db.Database.ExecuteSqlRaw(
                "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");

            var data = await query.ToListAsync();

            db.Database.ExecuteSqlRaw(
                "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");

            return data;
        }

        public static IdentityBuilder AddEfStores(this IdentityBuilder builder)
        {
            builder.Services.AddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), typeof(AppUserStore));
            builder.Services.AddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType), typeof(AppRoleStore));

            return builder;
        }
    }
}
