using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherUnavailabilityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfWeek",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TeacherUnavailabilities");

            migrationBuilder.DropColumn(
                name: "WeekNumber",
                table: "TeacherUnavailabilities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DateOfWeek",
                table: "TeacherUnavailabilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "TeacherUnavailabilities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "TeacherUnavailabilities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WeekNumber",
                table: "TeacherUnavailabilities",
                type: "integer",
                nullable: true);
        }
    }
}
