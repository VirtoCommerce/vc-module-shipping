using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public class ShippingDbContext : DbContextBase
    {
        public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
        {
        }

        protected ShippingDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StoreShippingMethodEntity>().ToTable("StoreShippingMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.StoreId).HasMaxLength(IdLength);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.TypeName).HasMaxLength(128);
            modelBuilder.Entity<StoreShippingMethodEntity>().HasIndex(x => new { x.TypeName, x.StoreId })
                .HasDatabaseName("IX_StoreShippingMethodEntity_TypeName_StoreId")
                .IsUnique();

            modelBuilder.Entity<PickupLocationEntity>().ToTable("PickupLocation").HasKey(x => x.Id);
            modelBuilder.Entity<PickupLocationEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();

            modelBuilder.Entity<PickupFulfillmentRelationEntity>().ToTable("PickupFulfillmentRelation").HasKey(x => x.Id);
            modelBuilder.Entity<PickupFulfillmentRelationEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();
            modelBuilder.Entity<PickupFulfillmentRelationEntity>()
                .HasOne(x => x.PickupLocation)
                .WithMany(x => x.TransferFulfillmentCenters)
                .HasForeignKey(x => x.PickupLocationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.ShippingModule.Data.XXX project. /> 
            switch (Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ShippingModule.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ShippingModule.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ShippingModule.Data.SqlServer"));
                    break;
            }

        }
    }
}
