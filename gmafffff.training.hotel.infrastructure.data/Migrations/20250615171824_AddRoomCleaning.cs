using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmafffff.training.hotel.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomCleaning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomCleanings",
                columns: table => new
                {
                    RoomCleaningId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    PutInLine = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CleaningCompleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsClean = table.Column<bool>(type: "INTEGER", nullable: false, computedColumnSql: "CleaningCompleted IS NOT NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomCleanings", x => x.RoomCleaningId);
                    table.ForeignKey(
                        name: "FK_RoomCleanings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomCleanings_RoomId",
                table: "RoomCleanings",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomCleanings");
        }
    }
}
