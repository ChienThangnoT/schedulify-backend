using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmitRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "SubmitsRequests",
                newName: "RequestDescription");

            migrationBuilder.AddColumn<string>(
                name: "ProcessNote",
                table: "SubmitsRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessNote",
                table: "SubmitsRequests");

            migrationBuilder.RenameColumn(
                name: "RequestDescription",
                table: "SubmitsRequests",
                newName: "Description");
        }
    }
}
