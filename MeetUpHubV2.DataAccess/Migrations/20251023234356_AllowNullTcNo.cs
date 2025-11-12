using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUpHubV2.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullTcNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TcNo",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TcNo",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }
    }
}
