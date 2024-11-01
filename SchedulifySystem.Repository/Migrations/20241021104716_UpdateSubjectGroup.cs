using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups");

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupType",
                table: "Subjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SchoolYearId",
                table: "SubjectGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_SchoolYearId",
                table: "SubjectGroups",
                column: "SchoolYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_SchoolYears_SchoolYearId",
                table: "SubjectGroups",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SchoolYears_SchoolYearId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_SchoolYearId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "SubjectGroupType",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SchoolYearId",
                table: "SubjectGroups");

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
