using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSubjectv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassPeriod_ClassSchedule_ClassScheduleId",
                table: "ClassPeriod");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPeriod_ClassSchedule_ClassScheduleId",
                table: "ClassPeriod",
                column: "ClassScheduleId",
                principalTable: "ClassSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassPeriod_ClassSchedule_ClassScheduleId",
                table: "ClassPeriod");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPeriod_ClassSchedule_ClassScheduleId",
                table: "ClassPeriod",
                column: "ClassScheduleId",
                principalTable: "ClassSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
