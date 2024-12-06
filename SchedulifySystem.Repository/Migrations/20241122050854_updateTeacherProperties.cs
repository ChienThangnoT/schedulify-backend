using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateTeacherProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments",
                column: "StudentClassRoomSubjectId",
                principalTable: "StudentClassRoomSubject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments",
                column: "StudentClassRoomSubjectId",
                principalTable: "StudentClassRoomSubject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments",
                column: "StudentClassId",
                principalTable: "StudentClasses",
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
    }
}
