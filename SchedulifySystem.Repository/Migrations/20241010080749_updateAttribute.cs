using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Building");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SubjectGroupType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "SubjectGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolYearCode",
                table: "SchoolYears",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassGroupCode",
                table: "ClassGroup",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SubjectGroupType");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "SchoolYearCode",
                table: "SchoolYears");

            migrationBuilder.DropColumn(
                name: "ClassGroupCode",
                table: "ClassGroup");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Building",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
