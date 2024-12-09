using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectTablev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terms_Schools_SchoolId",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_Terms_SchoolId",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "MainMinimumCouple",
                table: "SubjectInGroups");

            migrationBuilder.RenameColumn(
                name: "SchoolId",
                table: "Terms",
                newName: "StartWeek");

            migrationBuilder.RenameColumn(
                name: "SubMinimumCouple",
                table: "SubjectInGroups",
                newName: "SubjectInGroupType");

            migrationBuilder.AddColumn<int>(
                name: "EndWeek",
                table: "Terms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolYearId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SchoolYearId",
                table: "Subjects",
                column: "SchoolYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SchoolYears_SchoolYearId",
                table: "Subjects",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SchoolYears_SchoolYearId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SchoolYearId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "EndWeek",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "SchoolYearId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "StartWeek",
                table: "Terms",
                newName: "SchoolId");

            migrationBuilder.RenameColumn(
                name: "SubjectInGroupType",
                table: "SubjectInGroups",
                newName: "SubMinimumCouple");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Terms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Terms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MainMinimumCouple",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_SchoolId",
                table: "Terms",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terms_Schools_SchoolId",
                table: "Terms",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
