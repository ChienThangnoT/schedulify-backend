using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentClassTable_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Curriculums_CurriculumId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SchoolYears_SchoolYearId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Curriculums_CurriculumId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Subjects_SubjectId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectInGroups",
                table: "SubjectInGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectInGroups_StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectGroups",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.RenameTable(
                name: "SubjectInGroups",
                newName: "CurriculumDetail");

            migrationBuilder.RenameTable(
                name: "SubjectGroups",
                newName: "StudentClassGroups");

            migrationBuilder.RenameColumn(
                name: "SubjectGroupId",
                table: "StudentClasses",
                newName: "StudentClassGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClasses_SubjectGroupId",
                table: "StudentClasses",
                newName: "IX_StudentClasses_StudentClassGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectInGroups_TermId",
                table: "CurriculumDetail",
                newName: "IX_CurriculumDetail_TermId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectInGroups_SubjectId",
                table: "CurriculumDetail",
                newName: "IX_CurriculumDetail_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectInGroups_CurriculumId",
                table: "CurriculumDetail",
                newName: "IX_CurriculumDetail_CurriculumId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectGroups_SchoolYearId",
                table: "StudentClassGroups",
                newName: "IX_StudentClassGroups_SchoolYearId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectGroups_SchoolId",
                table: "StudentClassGroups",
                newName: "IX_StudentClassGroups_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectGroups_CurriculumId",
                table: "StudentClassGroups",
                newName: "IX_StudentClassGroups_CurriculumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurriculumDetail",
                table: "CurriculumDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentClassGroups",
                table: "StudentClassGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumDetail_Curriculums_CurriculumId",
                table: "CurriculumDetail",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumDetail_Subjects_SubjectId",
                table: "CurriculumDetail",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumDetail_Terms_TermId",
                table: "CurriculumDetail",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_StudentClassGroups_StudentClassGroupId",
                table: "StudentClasses",
                column: "StudentClassGroupId",
                principalTable: "StudentClassGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassGroups_Curriculums_CurriculumId",
                table: "StudentClassGroups",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassGroups_SchoolYears_SchoolYearId",
                table: "StudentClassGroups",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassGroups_Schools_SchoolId",
                table: "StudentClassGroups",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumDetail_Curriculums_CurriculumId",
                table: "CurriculumDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumDetail_Subjects_SubjectId",
                table: "CurriculumDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumDetail_Terms_TermId",
                table: "CurriculumDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_StudentClassGroups_StudentClassGroupId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassGroups_Curriculums_CurriculumId",
                table: "StudentClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassGroups_SchoolYears_SchoolYearId",
                table: "StudentClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassGroups_Schools_SchoolId",
                table: "StudentClassGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentClassGroups",
                table: "StudentClassGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurriculumDetail",
                table: "CurriculumDetail");

            migrationBuilder.RenameTable(
                name: "StudentClassGroups",
                newName: "SubjectGroups");

            migrationBuilder.RenameTable(
                name: "CurriculumDetail",
                newName: "SubjectInGroups");

            migrationBuilder.RenameColumn(
                name: "StudentClassGroupId",
                table: "StudentClasses",
                newName: "SubjectGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClasses_StudentClassGroupId",
                table: "StudentClasses",
                newName: "IX_StudentClasses_SubjectGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClassGroups_SchoolYearId",
                table: "SubjectGroups",
                newName: "IX_SubjectGroups_SchoolYearId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClassGroups_SchoolId",
                table: "SubjectGroups",
                newName: "IX_SubjectGroups_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClassGroups_CurriculumId",
                table: "SubjectGroups",
                newName: "IX_SubjectGroups_CurriculumId");

            migrationBuilder.RenameIndex(
                name: "IX_CurriculumDetail_TermId",
                table: "SubjectInGroups",
                newName: "IX_SubjectInGroups_TermId");

            migrationBuilder.RenameIndex(
                name: "IX_CurriculumDetail_SubjectId",
                table: "SubjectInGroups",
                newName: "IX_SubjectInGroups_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_CurriculumDetail_CurriculumId",
                table: "SubjectInGroups",
                newName: "IX_SubjectInGroups_CurriculumId");

            migrationBuilder.AddColumn<int>(
                name: "StudentClassGroupId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectGroups",
                table: "SubjectGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectInGroups",
                table: "SubjectInGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_StudentClassGroupId",
                table: "SubjectInGroups",
                column: "StudentClassGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Curriculums_CurriculumId",
                table: "SubjectGroups",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_SchoolYears_SchoolYearId",
                table: "SubjectGroups",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Curriculums_CurriculumId",
                table: "SubjectInGroups",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_StudentClassGroupId",
                table: "SubjectInGroups",
                column: "StudentClassGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Subjects_SubjectId",
                table: "SubjectInGroups",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");
        }
    }
}
