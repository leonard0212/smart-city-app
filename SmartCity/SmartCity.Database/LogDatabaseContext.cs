using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using SmartCity.Domain.Models.Logs;
using SmartCity.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Database
{
    public class LogDatabaseContext : DbContext
    {
        public readonly string ConnectionString;
        public LogDatabaseContext(DbContextOptions<LogDatabaseContext> options) : base(options)
        {
            ConnectionString = ((SqlServerOptionsExtension)options.Extensions.First(x => x is SqlServerOptionsExtension)).ConnectionString;

        }


        public DbSet<MessageBrokerDataLogs> MessageBrokerDataLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
