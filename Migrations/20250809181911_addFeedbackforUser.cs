using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kos.Migrations
{
    /// <inheritdoc />
    public partial class addFeedbackforUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddressed",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsAddressed",
                table: "Reviews");
        }
    }
}
