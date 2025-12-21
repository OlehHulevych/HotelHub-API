using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTheRoomTypeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PricePerNight",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RoomTypeId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Photos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomTypeId",
                table: "Photos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Detail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Norishment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Spa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    View = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    RoomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Detail_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomTypeId",
                table: "Reservations",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_RoomTypeId",
                table: "Photos",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Detail_RoomTypeId",
                table: "Detail",
                column: "RoomTypeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_RoomTypes_RoomTypeId",
                table: "Photos",
                column: "RoomTypeId",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_RoomTypes_RoomTypeId",
                table: "Reservations",
                column: "RoomTypeId",
                principalTable: "RoomTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_RoomTypes_RoomTypeId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_RoomTypes_RoomTypeId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Detail");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomTypeId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Photos_RoomTypeId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "Photos");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Photos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
