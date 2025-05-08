using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingAndRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadharNumber",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VillaName",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "NumberOfGuests",
                table: "Bookings",
                newName: "VillaId");

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

            migrationBuilder.AddColumn<int>(
                name: "Adults",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                table: "Bookings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomType = table.Column<string>(type: "text", nullable: false),
                    MaxGuests = table.Column<int>(type: "integer", nullable: false),
                    GuestType = table.Column<string>(type: "text", nullable: false),
                    WeekdayPrice = table.Column<double>(type: "double precision", nullable: false),
                    WeekendPrice = table.Column<double>(type: "double precision", nullable: false),
                    VillaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VillaFacility",
                columns: table => new
                {
                    VillaId = table.Column<int>(type: "integer", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillaFacility", x => new { x.VillaId, x.FacilityId });
                    table.ForeignKey(
                        name: "FK_VillaFacility_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VillaFacility_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VillaId",
                table: "Bookings",
                column: "VillaId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_VillaId",
                table: "Room",
                column: "VillaId");

            migrationBuilder.CreateIndex(
                name: "IX_VillaFacility_FacilityId",
                table: "VillaFacility",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Room_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Villas_VillaId",
                table: "Bookings",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Room_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Villas_VillaId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "VillaFacility");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VillaId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "WholeVillaWeekdayPrice",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "WholeVillaWeekendPrice",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Adults",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "VillaId",
                table: "Bookings",
                newName: "NumberOfGuests");

            migrationBuilder.AddColumn<string>(
                name: "AadharNumber",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VillaName",
                table: "Bookings",
                type: "text",
                nullable: true);
        }
    }
}
