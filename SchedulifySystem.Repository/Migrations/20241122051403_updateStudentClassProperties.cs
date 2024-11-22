using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateStudentClassProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses",
                column: "HomeroomTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses",
                column: "HomeroomTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                table: "TeacherAssignments",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
