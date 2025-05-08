using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class RoomGuestTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomGuestType_GuestTypes_GuestTypeId",
                table: "RoomGuestType");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomGuestType_Rooms_RoomId",
                table: "RoomGuestType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomGuestType",
                table: "RoomGuestType");

            migrationBuilder.RenameTable(
                name: "RoomGuestType",
                newName: "RoomGuestTypes");

            migrationBuilder.RenameIndex(
                name: "IX_RoomGuestType_GuestTypeId",
                table: "RoomGuestTypes",
                newName: "IX_RoomGuestTypes_GuestTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomGuestTypes",
                table: "RoomGuestTypes",
                columns: new[] { "RoomId", "GuestTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomGuestTypes_GuestTypes_GuestTypeId",
                table: "RoomGuestTypes",
                column: "GuestTypeId",
                principalTable: "GuestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomGuestTypes_Rooms_RoomId",
                table: "RoomGuestTypes",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomGuestTypes_GuestTypes_GuestTypeId",
                table: "RoomGuestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomGuestTypes_Rooms_RoomId",
                table: "RoomGuestTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomGuestTypes",
                table: "RoomGuestTypes");

            migrationBuilder.RenameTable(
                name: "RoomGuestTypes",
                newName: "RoomGuestType");

            migrationBuilder.RenameIndex(
                name: "IX_RoomGuestTypes_GuestTypeId",
                table: "RoomGuestType",
                newName: "IX_RoomGuestType_GuestTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomGuestType",
                table: "RoomGuestType",
                columns: new[] { "RoomId", "GuestTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomGuestType_GuestTypes_GuestTypeId",
                table: "RoomGuestType",
                column: "GuestTypeId",
                principalTable: "GuestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomGuestType_Rooms_RoomId",
                table: "RoomGuestType",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
