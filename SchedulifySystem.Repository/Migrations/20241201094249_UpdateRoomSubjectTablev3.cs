using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSubjectTablev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "RoomSubjects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomSubjects_TeacherId",
                table: "RoomSubjects",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSubjects_Teachers_TeacherId",
                table: "RoomSubjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomSubjects_Teachers_TeacherId",
                table: "RoomSubjects");

            migrationBuilder.DropIndex(
                name: "IX_RoomSubjects_TeacherId",
                table: "RoomSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "RoomSubjects");
        }
    }
}
