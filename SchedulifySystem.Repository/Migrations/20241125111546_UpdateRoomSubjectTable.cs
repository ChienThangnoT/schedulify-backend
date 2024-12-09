using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSubjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "StudentClassRoomSubject");

            migrationBuilder.RenameColumn(
                name: "StudentClassRoomSubjectId",
                table: "TeacherAssignments",
                newName: "RoomSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_StudentClassRoomSubjectId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_RoomSubjectId");

            migrationBuilder.AddColumn<int>(
                name: "Model",
                table: "RoomSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoomSubjectCode",
                table: "RoomSubjects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomSubjectName",
                table: "RoomSubjects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "RoomSubjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TermId",
                table: "RoomSubjects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomSubjects_SchoolId",
                table: "RoomSubjects",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomSubjects_TermId",
                table: "RoomSubjects",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSubjects_Schools_SchoolId",
                table: "RoomSubjects",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSubjects_Terms_TermId",
                table: "RoomSubjects",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_RoomSubjects_RoomSubjectId",
                table: "TeacherAssignments",
                column: "RoomSubjectId",
                principalTable: "RoomSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomSubjects_Schools_SchoolId",
                table: "RoomSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomSubjects_Terms_TermId",
                table: "RoomSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_RoomSubjects_RoomSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_RoomSubjects_SchoolId",
                table: "RoomSubjects");

            migrationBuilder.DropIndex(
                name: "IX_RoomSubjects_TermId",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "RoomSubjectCode",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "RoomSubjectName",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "RoomSubjects");

            migrationBuilder.RenameColumn(
                name: "RoomSubjectId",
                table: "TeacherAssignments",
                newName: "StudentClassRoomSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_RoomSubjectId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_StudentClassRoomSubjectId");

            migrationBuilder.AddColumn<int>(
                name: "Model",
                table: "StudentClassRoomSubject",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments",
                column: "StudentClassRoomSubjectId",
                principalTable: "StudentClassRoomSubject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
