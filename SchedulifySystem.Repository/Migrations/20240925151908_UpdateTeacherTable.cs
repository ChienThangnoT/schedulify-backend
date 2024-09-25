using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_ClassGroup_ClassGroupId",
                table: "SubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_ClassGroupId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "ClassGroupId",
                table: "SubjectGroups",
                newName: "SubjectGroupType");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherRole",
                table: "Teachers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Subjects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Subjects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlotPerWeek",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GroupDescription",
                table: "SubjectGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "SchoolSchedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FitnessPoint",
                table: "SchoolSchedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EducationDepartmentId",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DateOfWeek",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupId",
                table: "ClassGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Account",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Account",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProvinceId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationDepartments_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schools_EducationDepartmentId",
                table: "Schools",
                column: "EducationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SubjectGroupId",
                table: "ClassGroup",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDepartments_ProvinceId",
                table: "EducationDepartments",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_EducationDepartments_EducationDepartmentId",
                table: "Schools",
                column: "EducationDepartmentId",
                principalTable: "EducationDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Schools_EducationDepartments_EducationDepartmentId",
                table: "Schools");

            migrationBuilder.DropTable(
                name: "EducationDepartments");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Schools_EducationDepartmentId",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroup_SubjectGroupId",
                table: "ClassGroup");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TeacherRole",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SlotPerWeek",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "GroupDescription",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "SchoolSchedules");

            migrationBuilder.DropColumn(
                name: "FitnessPoint",
                table: "SchoolSchedules");

            migrationBuilder.DropColumn(
                name: "EducationDepartmentId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "DateOfWeek",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "SubjectGroupId",
                table: "ClassGroup");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "SubjectGroupType",
                table: "SubjectGroups",
                newName: "ClassGroupId");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Account",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_ClassGroupId",
                table: "SubjectGroups",
                column: "ClassGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_ClassGroup_ClassGroupId",
                table: "SubjectGroups",
                column: "ClassGroupId",
                principalTable: "ClassGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
