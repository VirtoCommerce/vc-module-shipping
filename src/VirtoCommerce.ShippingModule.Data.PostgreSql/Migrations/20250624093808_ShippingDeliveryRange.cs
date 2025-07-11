using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.ShippingModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class ShippingDeliveryRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorageDays",
                table: "PickupLocation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryDays",
                table: "PickupLocation",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageDays",
                table: "PickupLocation");

            migrationBuilder.DropColumn(
                name: "DeliveryDays",
                table: "PickupLocation");
        }
    }
}
