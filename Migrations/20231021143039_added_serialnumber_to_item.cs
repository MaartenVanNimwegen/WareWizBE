using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WareWiz.Migrations
{
    /// <inheritdoc />
    public partial class added_serialnumber_to_item : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "serialNumber",
                table: "Items",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "serialNumber",
                table: "Items");
        }
    }
}
