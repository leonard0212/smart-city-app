using SmartCity.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using SmartCity.Database.Extensions;
using SmartCity.Domain.Models.Settings;
using SmartCity.Domain.Models.Common;
using SmartCity.Domain.Models.Entities;
using System;
using SmartCity.Domain.Models;

namespace SmartCity.Database
{
    public class DatabaseContext : IdentityDbContext<User, Role, Guid>
    {

        public readonly string ConnectionString;


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            ConnectionString = ((SqlServerOptionsExtension)options.Extensions.First(x => x is SqlServerOptionsExtension)).ConnectionString;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            CreateRawDetectionModels(builder);
            CreateDetectionModels(builder);
            CreateUserAndAccountsModels(builder);
            CreateSettingsModels(builder);
            CreateDetectionChatModels(builder);
            CreateAdministrativeModels(builder);
        }

        private static void CreateRawDetectionModels(ModelBuilder builder)
        {
            builder.Entity<RawDetection>().ToTable("RawDetection").HasKey(x => x.Id);
            builder.Entity<RawDetection>().HasMany(x => x.Detections).WithOne(x => x.RawDetection);
            builder.Entity<RawDetection>().HasMany(x => x.DetectionsCrop);
            builder.Entity<RawDetection>().Property(x => x.MainClass).AsEnumProperty();
            builder.Entity<RawDetection>().Property(x => x.SubClass).AsEnumProperty();
            builder.Entity<RawDetection>().Property(x => x.ProcessStatus).AsEnumProperty();


            builder.Entity<RawDetectedObject>().ToTable("RawDetectedObject").HasKey(x => x.Id);
            builder.Entity<RawDetectedObject>().HasOne(x => x.RawDetection);
            builder.Entity<RawDetectedObject>().Property(x => x.Type).AsEnumProperty();

            builder.Entity<RawDetectedCropObject>().ToTable("RawDetectedCropObject").HasKey(x => x.Id);
            builder.Entity<RawDetectedCropObject>().HasOne(x => x.RawDetection);
            builder.Entity<RawDetectedCropObject>().Property(x => x.Type).AsEnumProperty();

        }

        private static void CreateDetectionModels(ModelBuilder builder)
        {
            builder.Entity<Detection>().ToTable("Detection").HasKey(x => x.Id);
            builder.Entity<Detection>().ConfigureEntityComplexGuid();
            builder.Entity<Detection>().HasOne(x => x.Departament);
            builder.Entity<Detection>().Property(x => x.Category).AsEnumProperty();
            builder.Entity<Detection>().Property(x => x.NeedIntervention).AsEnumProperty();
            builder.Entity<Detection>().Property(x => x.ResolutionStatus).AsEnumProperty();
            builder.Entity<Detection>().Property(x => x.Severity).AsEnumProperty();

            builder.Entity<Detection>().HasMany(x => x.Files);
            builder.Entity<DetectionFile>().ToTable("DetectionFile").HasKey(x => x.Id);

            builder.Entity<DetectionFlowLog>().ToTable("DetectionFlowLog").HasKey(x => x.Id);
            builder.Entity<DetectionFlowLog>().HasOne(x => x.Detection);
            builder.Entity<DetectionFlowLog>().HasOne(x => x.User);



            builder.Entity<RoadInfrastructureDetection>().ToTable("RoadInfrastructureDetection");
            builder.Entity<RoadInfrastructureDetection>().Property(x => x.Type).AsEnumProperty();
            builder.Entity<RoadInfrastructureDetection>().Property(x => x.Dimension).AsEnumProperty();

            builder.Entity<BillboardDetection>().ToTable("BillboardDetection");
            builder.Entity<BillboardDetection>().Property(x => x.BillboardCategory).AsEnumProperty();
            builder.Entity<BillboardDetection>().Property(x => x.BillboardSize).AsEnumProperty();

            builder.Entity<TrashAssetsDetection>().ToTable("TrashAssetsDetection");
            builder.Entity<TrashAssetsDetection>().Property(x => x.AssetStatus).AsEnumProperty();
            builder.Entity<TrashAssetsDetection>().Property(x => x.TrashAssetCategory).AsEnumProperty();
            builder.Entity<TrashAssetsDetection>().Property(x => x.TrashVolumType).AsEnumProperty();


            builder.Entity<TrafficSignDetection>().ToTable("TrafficSignDetection");

        }


        private static void CreateUserAndAccountsModels(ModelBuilder builder)
        {
            builder.Entity<Account>().ToTable("Account").HasKey(x => x.Id);
            builder.Entity<Account>().ConfigureEntityComplexGuid();
            builder.Entity<Account>().HasMany(x => x.Users).WithOne(x => x.Account);
        }
        private static void CreateSettingsModels(ModelBuilder builder)
        {
            builder.Entity<SystemProperty>().ToTable("_SystemProperty");
            builder.Entity<AppFile>().ToTable("AppFile").HasKey(x => x.Id);
        }

        private static void CreateDetectionChatModels(ModelBuilder builder)
        {
            builder.Entity<DetectionChat>().ToTable("DetectionChat");
            builder.Entity<DetectionChat>().HasKey(dc => dc.Id);

            builder.Entity<DetectionChat>().Property(dc => dc.Id)
                  .HasDefaultValueSql("NEWID()");

            builder.Entity<DetectionChat>().Property(dc => dc.CreatedAt)
                  .HasColumnType("datetime");

            builder.Entity<DetectionChat>().HasOne(dc => dc.Detection)
                  .WithMany()
                  .HasForeignKey(dc => dc.DetectionId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DetectionChat>().HasOne(dc => dc.User)
                  .WithMany()
                  .HasForeignKey(dc => dc.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        }


        private static void CreateAdministrativeModels(ModelBuilder builder)
        {
            builder.Entity<Departament>().ToTable("Departament").HasKey(x => x.Id);
            builder.Entity<Departament>().HasMany(x => x.Teams).WithOne(x => x.Departament);

            builder.Entity<TeamMembership>().ToTable("TeamMembership").HasKey(x => x.Id);
            builder.Entity<TeamMembership>().HasOne(x => x.User).WithMany(x => x.TeamMemberships);
            builder.Entity<TeamMembership>().HasOne(x => x.Team).WithMany(x => x.TeamMemberships);

            builder.Entity<Team>().ToTable("Team").HasKey(x => x.Id);
            builder.Entity<Team>().HasMany(x => x.TeamMemberships).WithOne(x => x.Team);
            builder.Entity<Team>().HasOne(x => x.Departament).WithMany(x => x.Teams);
        }




    }
}
