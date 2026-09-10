using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bretts_services.Migrations
{
    /// <inheritdoc />
    public partial class RestrictInUseRoleDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleID",
                table: "RoleUser");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleID",
                table: "RoleUser",
                column: "RolesRoleID",
                principalTable: "Roles",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleID",
                table: "RoleUser");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Roles_RolesRoleID",
                table: "RoleUser",
                column: "RolesRoleID",
                principalTable: "Roles",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
