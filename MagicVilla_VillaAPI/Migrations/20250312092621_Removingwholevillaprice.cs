using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class Removingwholevillaprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WholeVillaWeekdayPrice",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "WholeVillaWeekendPrice",
                table: "Villas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WholeVillaWeekdayPrice",
                table: "Villas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WholeVillaWeekendPrice",
                table: "Villas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
