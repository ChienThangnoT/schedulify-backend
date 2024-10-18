using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_TeachableSubjects_TeachableSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentClasses");

            migrationBuilder.RenameColumn(
                name: "TeachableSubjectId",
                table: "TeacherAssignments",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_TeachableSubjectId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_SubjectId");

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "TeacherAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Subjects_SubjectId",
                table: "TeacherAssignments",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Subjects_SubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "TeacherAssignments",
                newName: "TeachableSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_SubjectId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_TeachableSubjectId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_TeachableSubjects_TeachableSubjectId",
                table: "TeacherAssignments",
                column: "TeachableSubjectId",
                principalTable: "TeachableSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
