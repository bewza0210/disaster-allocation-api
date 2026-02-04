using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisasterApi.Migrations
{
    /// <inheritdoc />
    public partial class modifyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaID",
                table: "Trucks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AreaID",
                table: "Areas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "Areas");
        }
    }
}
