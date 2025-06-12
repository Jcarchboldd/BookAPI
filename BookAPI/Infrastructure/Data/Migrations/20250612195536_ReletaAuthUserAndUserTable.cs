using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAPI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReletaAuthUserAndUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthUserId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthUserId",
                table: "Users",
                column: "AuthUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AuthUsers_AuthUserId",
                table: "Users",
                column: "AuthUserId",
                principalTable: "AuthUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AuthUsers_AuthUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AuthUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthUserId",
                table: "Users");
        }
    }
}
