using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSubjectTablev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassRoomSubject_RoomSubjects_StudentClassId",
                table: "StudentClassRoomSubject");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassRoomSubject_RoomSubjectId",
                table: "StudentClassRoomSubject",
                column: "RoomSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassRoomSubject_RoomSubjects_RoomSubjectId",
                table: "StudentClassRoomSubject",
                column: "RoomSubjectId",
                principalTable: "RoomSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClassRoomSubject_RoomSubjects_RoomSubjectId",
                table: "StudentClassRoomSubject");

            migrationBuilder.DropIndex(
                name: "IX_StudentClassRoomSubject_RoomSubjectId",
                table: "StudentClassRoomSubject");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClassRoomSubject_RoomSubjects_StudentClassId",
                table: "StudentClassRoomSubject",
                column: "StudentClassId",
                principalTable: "RoomSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
