using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeDealerMgtAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIdentityTablesForDealers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Inventory",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageCapacity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inventory",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StorageCapacity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "GSTNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
