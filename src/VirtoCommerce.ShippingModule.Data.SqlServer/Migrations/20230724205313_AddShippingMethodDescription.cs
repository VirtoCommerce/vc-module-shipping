using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.ShippingModule.Data.SqlServer.Migrations
{
    public partial class AddShippingMethodDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StoreShippingMethod",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "StoreShippingMethod");
        }
    }
}
