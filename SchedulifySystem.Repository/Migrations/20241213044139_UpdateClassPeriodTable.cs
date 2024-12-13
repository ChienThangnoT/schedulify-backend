using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassPeriodTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassPeriod_TeacherAssignments_TeacherAssignmentId",
                table: "ClassPeriod");

            migrationBuilder.DropIndex(
                name: "IX_ClassPeriod_TeacherAssignmentId",
                table: "ClassPeriod");

            migrationBuilder.AlterColumn<int>(
                name: "Week",
                table: "PeriodChange",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Week",
                table: "PeriodChange",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_TeacherAssignmentId",
                table: "ClassPeriod",
                column: "TeacherAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPeriod_TeacherAssignments_TeacherAssignmentId",
                table: "ClassPeriod",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
