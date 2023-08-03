using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.NotificationService.Data.Migrations
{
    public partial class updatedEmailClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "Emails",
                newName: "ToEmail");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "Emails",
                newName: "FromEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToEmail",
                table: "Emails",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "FromEmail",
                table: "Emails",
                newName: "From");
        }
    }
}
