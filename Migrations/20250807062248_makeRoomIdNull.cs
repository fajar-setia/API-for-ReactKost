using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kos.Migrations
{
    /// <inheritdoc />
    public partial class makeRoomIdNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Rooms_RoomId",
                table: "Galleries");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Galleries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Rooms_RoomId",
                table: "Galleries",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Rooms_RoomId",
                table: "Galleries");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Galleries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Rooms_RoomId",
                table: "Galleries",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
