using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bretts_services.Migrations
{
    /// <inheritdoc />
    public partial class AddLogGuidAndStronglyTypedLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LogGuid",
                table: "Logs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_LogGuid",
                table: "Logs",
                column: "LogGuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logs_LogGuid",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "LogGuid",
                table: "Logs");
        }
    }
}
