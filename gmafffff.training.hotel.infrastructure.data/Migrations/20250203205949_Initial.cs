using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmafffff.training.hotel.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccommodationReports",
                columns: table => new
                {
                    AccommodationReportId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Visitors = table.Column<string>(type: "TEXT", nullable: false),
                    ArrivalDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DepartureDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    RoomDetails_Capacity = table.Column<byte>(type: "INTEGER", nullable: false),
                    RoomDetails_Number = table.Column<string>(type: "TEXT", nullable: false),
                    RoomDetails_Type = table.Column<string>(type: "TEXT", nullable: false),
                    TariffDetails_Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TariffDetails_Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationReports", x => x.AccommodationReportId);
                });

            migrationBuilder.CreateTable(
                name: "HotelRooms",
                columns: table => new
                {
                    HotelBlockId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelRooms", x => x.HotelBlockId);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName_FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    FullName_Patronymic = table.Column<string>(type: "TEXT", nullable: true),
                    FullName_SurName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HotelBlockId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomDetails_Capacity = table.Column<byte>(type: "INTEGER", nullable: false),
                    RoomDetails_Number = table.Column<string>(type: "TEXT", nullable: false),
                    RoomDetails_Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Rooms_HotelRooms_HotelBlockId",
                        column: x => x.HotelBlockId,
                        principalTable: "HotelRooms",
                        principalColumn: "HotelBlockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tariff",
                columns: table => new
                {
                    TariffId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HotelBlockId = table.Column<int>(type: "INTEGER", nullable: false),
                    TariffDetails_Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TariffDetails_Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariff", x => x.TariffId);
                    table.ForeignKey(
                        name: "FK_Tariff_HotelRooms_HotelBlockId",
                        column: x => x.HotelBlockId,
                        principalTable: "HotelRooms",
                        principalColumn: "HotelBlockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomVisit",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Visitors = table.Column<string>(type: "TEXT", nullable: false),
                    ArrivalDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DepartureDatePlanned = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    TariffDetails_Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TariffDetails_Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    rowVersion = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomVisit", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_RoomVisit_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelBlockId",
                table: "Rooms",
                column: "HotelBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_HotelBlockId",
                table: "Tariff",
                column: "HotelBlockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationReports");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "RoomVisit");

            migrationBuilder.DropTable(
                name: "Tariff");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "HotelRooms");
        }
    }
}
