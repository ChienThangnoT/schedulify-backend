using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchoolSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolSchedules_Subjects_SubjectId",
                table: "SchoolSchedules");

            migrationBuilder.DropIndex(
                name: "IX_SchoolSchedules_SubjectId",
                table: "SchoolSchedules");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "SchoolSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "SchoolSchedules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSchedules_SubjectId",
                table: "SchoolSchedules",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolSchedules_Subjects_SubjectId",
                table: "SchoolSchedules",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
