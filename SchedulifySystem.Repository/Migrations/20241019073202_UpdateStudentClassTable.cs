using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "SubjectGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectGroupId",
                table: "StudentClasses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "SubjectGroups");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectGroupId",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
