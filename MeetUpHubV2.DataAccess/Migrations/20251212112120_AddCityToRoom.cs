using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUpHubV2.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCityToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Rooms");
        }
    }
}
