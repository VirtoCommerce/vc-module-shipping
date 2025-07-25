// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.ShippingModule.Data.Repositories;

#nullable disable

namespace VirtoCommerce.ShippingModule.Data.MySql.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    [Migration("20250624093641_ShippingDeliveryRange")]
    partial class ShippingDeliveryRange
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("VirtoCommerce.ShippingModule.Data.Model.PickupFulfillmentRelationEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("FulfillmentCenterId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("PickupLocationId")
                        .IsRequired()
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("PickupLocationId");

                    b.ToTable("PickupFulfillmentRelation", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.ShippingModule.Data.Model.PickupLocationEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("City")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ContactEmail")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("ContactPhone")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("CountryCode")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("CountryName")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("FulfillmentCenterId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("GeoLocation")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Line1")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("Line2")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("OuterId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int?>("StorageDays")
                        .HasColumnType("int");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.Property<int?>("DeliveryDays")
                        .HasColumnType("int");

                    b.Property<string>("RegionId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("RegionName")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("StoreId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("WorkingHours")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("PickupLocation", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.ShippingModule.Data.Model.StoreShippingMethodEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LogoUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("varchar(2048)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("StoreId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("TaxType")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TypeName")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("TypeName", "StoreId")
                        .IsUnique()
                        .HasDatabaseName("IX_StoreShippingMethodEntity_TypeName_StoreId");

                    b.ToTable("StoreShippingMethod", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.ShippingModule.Data.Model.PickupFulfillmentRelationEntity", b =>
                {
                    b.HasOne("VirtoCommerce.ShippingModule.Data.Model.PickupLocationEntity", "PickupLocation")
                        .WithMany("TransferFulfillmentCenters")
                        .HasForeignKey("PickupLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PickupLocation");
                });

            modelBuilder.Entity("VirtoCommerce.ShippingModule.Data.Model.PickupLocationEntity", b =>
                {
                    b.Navigation("TransferFulfillmentCenters");
                });
#pragma warning restore 612, 618
        }
    }
}
