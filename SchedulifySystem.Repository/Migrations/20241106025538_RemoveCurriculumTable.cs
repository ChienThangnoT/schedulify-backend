using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCurriculumTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectConfigs_Curriculums_CurriculumId",
                table: "SubjectConfigs");

            migrationBuilder.DropTable(
                name: "Curriculums");

            migrationBuilder.DropIndex(
                name: "IX_SubjectConfigs_CurriculumId",
                table: "SubjectConfigs");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "SubjectConfigs");

            migrationBuilder.AddColumn<int>(
                name: "PeriodCount",
                table: "Teachers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppropriateLevel",
                table: "TeachableSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "TeachableSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MainMinimumCouple",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubMinimumCouple",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodCount",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "AppropriateLevel",
                table: "TeachableSubjects");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "TeachableSubjects");

            migrationBuilder.DropColumn(
                name: "MainMinimumCouple",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "SubMinimumCouple",
                table: "SubjectInGroups");

            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "SubjectConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Curriculums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    SubjectGroupId = table.Column<int>(type: "integer", nullable: false),
                    ClassGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Curriculums_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Curriculums_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Curriculums_SubjectGroups_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_CurriculumId",
                table: "SubjectConfigs",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolId",
                table: "Curriculums",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolYearId",
                table: "Curriculums",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SubjectGroupId",
                table: "Curriculums",
                column: "SubjectGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectConfigs_Curriculums_CurriculumId",
                table: "SubjectConfigs",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
