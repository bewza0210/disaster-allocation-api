using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisasterApi.Migrations
{
    /// <inheritdoc />
    public partial class modifyColumn1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AreaID",
                table: "Trucks",
                newName: "TruckID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TruckID",
                table: "Trucks",
                newName: "AreaID");
        }
    }
}
