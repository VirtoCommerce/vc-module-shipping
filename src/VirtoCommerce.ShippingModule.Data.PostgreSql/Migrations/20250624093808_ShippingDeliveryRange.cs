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
                name: "PickupDeadline",
                table: "PickupLocation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadyForPickup",
                table: "PickupLocation",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupDeadline",
                table: "PickupLocation");

            migrationBuilder.DropColumn(
                name: "ReadyForPickup",
                table: "PickupLocation");
        }
    }
}
