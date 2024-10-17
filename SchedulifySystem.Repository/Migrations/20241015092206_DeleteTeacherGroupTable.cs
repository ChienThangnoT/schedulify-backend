using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTeacherGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_TeacherGroups_TeacherGroupId",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "TeacherGroups");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherGroupId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TeacherGroupId",
                table: "Teachers");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "Departments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "TeacherGroupId",
                table: "Teachers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TeacherGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherGroups_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherGroupId",
                table: "Teachers",
                column: "TeacherGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherGroups_SchoolId",
                table: "TeacherGroups",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_TeacherGroups_TeacherGroupId",
                table: "Teachers",
                column: "TeacherGroupId",
                principalTable: "TeacherGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
