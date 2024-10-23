using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DeleteClassSubjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curriculums_ClassGroup_ClassGroupId",
                table: "Curriculums");

            migrationBuilder.DropTable(
                name: "StudentClassInGroups");

            migrationBuilder.DropTable(
                name: "ClassGroup");

            migrationBuilder.DropIndex(
                name: "IX_Curriculums_ClassGroupId",
                table: "Curriculums");

            migrationBuilder.DropColumn(
                name: "SubjectGroupType",
                table: "SubjectGroups");

            migrationBuilder.AddColumn<int>(
                name: "PeriodCount",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupId",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Curriculums",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_SubjectGroupId",
                table: "StudentClasses",
                column: "SubjectGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SubjectGroups_SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "PeriodCount",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "SubjectGroupId",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Curriculums");

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupType",
                table: "SubjectGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: true),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: true),
                    SubjectGroupId = table.Column<int>(type: "integer", nullable: true),
                    ClassGroupCode = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassGroup_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentClassInGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassGroupId = table.Column<int>(type: "integer", nullable: false),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassInGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentClassInGroups_ClassGroup_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClassInGroups_StudentClasses_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_ClassGroupId",
                table: "Curriculums",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SchoolId",
                table: "ClassGroup",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SchoolYearId",
                table: "ClassGroup",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SubjectGroupId",
                table: "ClassGroup",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassInGroups_ClassGroupId",
                table: "StudentClassInGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassInGroups_StudentClassId",
                table: "StudentClassInGroups",
                column: "StudentClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curriculums_ClassGroup_ClassGroupId",
                table: "Curriculums",
                column: "ClassGroupId",
                principalTable: "ClassGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
