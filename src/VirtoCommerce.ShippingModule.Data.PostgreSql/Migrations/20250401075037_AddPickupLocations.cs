using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.ShippingModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickupLocation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    FulfillmentCenterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WorkingHours = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Line1 = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Line2 = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CountryName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    RegionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RegionName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    GeoLocation = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OuterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickupFulfillmentRelation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FulfillmentCenterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PickupLocationId = table.Column<string>(type: "character varying(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupFulfillmentRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupFulfillmentRelation_PickupLocation_PickupLocationId",
                        column: x => x.PickupLocationId,
                        principalTable: "PickupLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupFulfillmentRelation_PickupLocationId",
                table: "PickupFulfillmentRelation",
                column: "PickupLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupFulfillmentRelation");

            migrationBuilder.DropTable(
                name: "PickupLocation");
        }
    }
}
