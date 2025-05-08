using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class Modifyingtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Room_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Villas_VillaId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_VillaFacility_Facility_FacilityId",
                table: "VillaFacility");

            migrationBuilder.DropForeignKey(
                name: "FK_VillaFacility_Villas_VillaId",
                table: "VillaFacility");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VillaFacility",
                table: "VillaFacility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facility",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "Amenity",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "GuestType",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "VillaFacility",
                newName: "VillaFacilities");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Facility",
                newName: "Facilities");

            migrationBuilder.RenameIndex(
                name: "IX_VillaFacility_FacilityId",
                table: "VillaFacilities",
                newName: "IX_VillaFacilities_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Room_VillaId",
                table: "Rooms",
                newName: "IX_Rooms_VillaId");

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "Villas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuestTypeId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VillaFacilities",
                table: "VillaFacilities",
                columns: new[] { "VillaId", "FacilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Villas_DestinationId",
                table: "Villas",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_GuestTypeId",
                table: "Rooms",
                column: "GuestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_GuestTypes_GuestTypeId",
                table: "Rooms",
                column: "GuestTypeId",
                principalTable: "GuestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Villas_VillaId",
                table: "Rooms",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VillaFacilities_Facilities_FacilityId",
                table: "VillaFacilities",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VillaFacilities_Villas_VillaId",
                table: "VillaFacilities",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Villas_Destinations_DestinationId",
                table: "Villas",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_GuestTypes_GuestTypeId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Villas_VillaId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_VillaFacilities_Facilities_FacilityId",
                table: "VillaFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_VillaFacilities_Villas_VillaId",
                table: "VillaFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Villas_Destinations_DestinationId",
                table: "Villas");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "GuestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Villas_DestinationId",
                table: "Villas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VillaFacilities",
                table: "VillaFacilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_GuestTypeId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facilities",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "GuestTypeId",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "VillaFacilities",
                newName: "VillaFacility");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "Facilities",
                newName: "Facility");

            migrationBuilder.RenameIndex(
                name: "IX_VillaFacilities_FacilityId",
                table: "VillaFacility",
                newName: "IX_VillaFacility_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_VillaId",
                table: "Room",
                newName: "IX_Room_VillaId");

            migrationBuilder.AddColumn<string>(
                name: "Amenity",
                table: "Villas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Villas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Villas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "Villas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "GuestType",
                table: "Room",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VillaFacility",
                table: "VillaFacility",
                columns: new[] { "VillaId", "FacilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facility",
                table: "Facility",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Room_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Villas_VillaId",
                table: "Room",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VillaFacility_Facility_FacilityId",
                table: "VillaFacility",
                column: "FacilityId",
                principalTable: "Facility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VillaFacility_Villas_VillaId",
                table: "VillaFacility",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
