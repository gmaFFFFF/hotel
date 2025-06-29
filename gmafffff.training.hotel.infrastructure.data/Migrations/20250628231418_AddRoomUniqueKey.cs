using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmafffff.training.hotel.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomUniqueKey : Migration {
        public const string ConstraintName = "IX_Room_HotelBlockId_RoomDetails_Number";
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateIndex(
                name: ConstraintName,
                table: "Rooms",
                columns: ["HotelBlockId", "RoomDetails_Number"],
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(ConstraintName, "Rooms");
        }
    }
}
