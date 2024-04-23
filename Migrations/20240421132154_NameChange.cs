using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bretts_services.Migrations
{
    /// <inheritdoc />
    public partial class NameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_RolesUserID",
                table: "RoleUser");

            migrationBuilder.RenameColumn(
                name: "RolesUserID",
                table: "RoleUser",
                newName: "UsersUserID");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_RolesUserID",
                table: "RoleUser",
                newName: "IX_RoleUser_UsersUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_UsersUserID",
                table: "RoleUser",
                column: "UsersUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Users_UsersUserID",
                table: "RoleUser");

            migrationBuilder.RenameColumn(
                name: "UsersUserID",
                table: "RoleUser",
                newName: "RolesUserID");

            migrationBuilder.RenameIndex(
                name: "IX_RoleUser_UsersUserID",
                table: "RoleUser",
                newName: "IX_RoleUser_RolesUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Users_RolesUserID",
                table: "RoleUser",
                column: "RolesUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
