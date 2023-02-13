using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.NotificationService.Data.Migrations
{
    public partial class UpdateMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Entities",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Entities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUtc",
                table: "Entities",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUtc",
                table: "Entities");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Entities",
                newName: "id");
        }
    }
}
