using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kos.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleGalleries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Galleries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Galleries");
        }
    }
}
