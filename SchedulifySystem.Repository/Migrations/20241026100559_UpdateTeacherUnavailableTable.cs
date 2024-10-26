using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherUnavailableTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TeacherUnavailabilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "TeacherUnavailabilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartAt",
                table: "TeacherUnavailabilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeetingDay",
                table: "Departments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "StartAt",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "MeetingDay",
                table: "Departments");
        }
    }
}
