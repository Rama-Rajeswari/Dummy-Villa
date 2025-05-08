using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingRoomGuesttypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_GuestTypes_GuestTypeId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_GuestTypeId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "GuestTypeId",
                table: "Rooms");

            migrationBuilder.CreateTable(
                name: "RoomGuestType",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    GuestTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomGuestType", x => new { x.RoomId, x.GuestTypeId });
                    table.ForeignKey(
                        name: "FK_RoomGuestType_GuestTypes_GuestTypeId",
                        column: x => x.GuestTypeId,
                        principalTable: "GuestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomGuestType_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomGuestType_GuestTypeId",
                table: "RoomGuestType",
                column: "GuestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomGuestType");

            migrationBuilder.AddColumn<int>(
                name: "GuestTypeId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_GuestTypeId",
                table: "Rooms",
                column: "GuestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_GuestTypes_GuestTypeId",
                table: "Rooms",
                column: "GuestTypeId",
                principalTable: "GuestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
