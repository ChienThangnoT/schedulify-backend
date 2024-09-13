using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableHoliday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "TeacherConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holidays",
                type: "character varying(70)",
                maxLength: 70,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Holidays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_SchoolId",
                table: "Holidays",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Holidays_Schools_SchoolId",
                table: "Holidays",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Holidays_Schools_SchoolId",
                table: "Holidays");

            migrationBuilder.DropIndex(
                name: "IX_Holidays_SchoolId",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Holidays");

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "TeacherConfigs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holidays",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(70)",
                oldMaxLength: 70,
                oldNullable: true);
        }
    }
}
