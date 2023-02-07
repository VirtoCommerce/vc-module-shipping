﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.ShippingModule.Data.Repositories;

#nullable disable

namespace VirtoCommerce.ShippingModule.Data.MySql.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    partial class ShippingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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
#pragma warning restore 612, 618
        }
    }
}
