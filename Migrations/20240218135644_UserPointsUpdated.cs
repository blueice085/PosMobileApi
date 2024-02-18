using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosMobileApi.Migrations
{
    /// <inheritdoc />
    public partial class UserPointsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPoints_Users_UserId1",
                table: "UserPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPoints",
                table: "UserPoints");

            migrationBuilder.DropIndex(
                name: "IX_UserPoints_UserId1",
                table: "UserPoints");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserPoints");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "UserPoints",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPoints",
                table: "UserPoints",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPoints",
                table: "UserPoints");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserPoints",
                newName: "UserId1");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserPoints",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPoints",
                table: "UserPoints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPoints_UserId1",
                table: "UserPoints",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPoints_Users_UserId1",
                table: "UserPoints",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
