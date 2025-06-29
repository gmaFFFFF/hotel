using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmafffff.training.hotel.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class AddvisitorHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "History_Count",
                table: "Persons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "History_Duration",
                table: "Persons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "History_Count",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "History_Duration",
                table: "Persons");
        }
    }
}
