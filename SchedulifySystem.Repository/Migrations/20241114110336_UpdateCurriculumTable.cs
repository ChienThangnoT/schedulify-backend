using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCurriculumTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SubjectInGroups_CurriculumDetailId",
                table: "SubjectGroups");

            migrationBuilder.RenameColumn(
                name: "CurriculumDetailId",
                table: "SubjectGroups",
                newName: "CurriculumId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectGroups_CurriculumDetailId",
                table: "SubjectGroups",
                newName: "IX_SubjectGroups_CurriculumId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Curriculums_CurriculumId",
                table: "SubjectGroups",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Curriculums_CurriculumId",
                table: "SubjectGroups");

            migrationBuilder.RenameColumn(
                name: "CurriculumId",
                table: "SubjectGroups",
                newName: "CurriculumDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectGroups_CurriculumId",
                table: "SubjectGroups",
                newName: "IX_SubjectGroups_CurriculumDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_SubjectInGroups_CurriculumDetailId",
                table: "SubjectGroups",
                column: "CurriculumDetailId",
                principalTable: "SubjectInGroups",
                principalColumn: "Id");
        }
    }
}
