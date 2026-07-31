using SmartCity.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartCity.Database.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> ConfigureEntityComplexLong<T>(this EntityTypeBuilder<T> entityTypeBuilder)
          where T : EntityComplex<long>
        {
            entityTypeBuilder.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById);
            entityTypeBuilder.HasOne(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedById);
            entityTypeBuilder.HasOne(x => x.DeletedBy).WithMany().HasForeignKey(x => x.DeletedById);
            entityTypeBuilder.HasQueryFilter(x => !x.IsDeleted);

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<T> ConfigureEntityComplexGuid<T>(this EntityTypeBuilder<T> entityTypeBuilder)
          where T : EntityComplex<Guid>
        {
            entityTypeBuilder.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById);
            entityTypeBuilder.HasOne(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedById);
            entityTypeBuilder.HasOne(x => x.DeletedBy).WithMany().HasForeignKey(x => x.DeletedById);
            entityTypeBuilder.HasQueryFilter(x => !x.IsDeleted);

            return entityTypeBuilder;
        }
    }
}
