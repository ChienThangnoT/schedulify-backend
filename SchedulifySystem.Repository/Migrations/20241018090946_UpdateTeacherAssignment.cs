using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodCount",
                table: "TeacherAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TermId",
                table: "TeacherAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullDay",
                table: "StudentClasses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartAt",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TermId",
                table: "TeacherAssignments",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Terms_TermId",
                table: "TeacherAssignments",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Terms_TermId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TermId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "PeriodCount",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "IsFullDay",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "StartAt",
                table: "ClassPeriod");
        }
    }
}
