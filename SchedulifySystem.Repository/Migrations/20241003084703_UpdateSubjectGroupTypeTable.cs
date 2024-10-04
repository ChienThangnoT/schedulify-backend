using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectGroupTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectGroupType",
                table: "SubjectGroups",
                newName: "SubjectGroupTypeId");

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SlotPerTerm",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TermId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupId",
                table: "Curriculums",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubjectGroupType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefaultSlotPerTerm = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroupType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SchoolId",
                table: "Subjects",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_TermId",
                table: "SubjectInGroups",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_SubjectGroupTypeId",
                table: "SubjectGroups",
                column: "SubjectGroupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SubjectGroupId",
                table: "Curriculums",
                column: "SubjectGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curriculums_SubjectGroups_SubjectGroupId",
                table: "Curriculums",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_SubjectGroupType_SubjectGroupTypeId",
                table: "SubjectGroups",
                column: "SubjectGroupTypeId",
                principalTable: "SubjectGroupType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curriculums_SubjectGroups_SubjectGroupId",
                table: "Curriculums");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SubjectGroupType_SubjectGroupTypeId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Terms_TermId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectGroupType");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SchoolId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_SubjectInGroups_TermId",
                table: "SubjectInGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_SubjectGroupTypeId",
                table: "SubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_Curriculums_SubjectGroupId",
                table: "Curriculums");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SlotPerTerm",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "SubjectGroupId",
                table: "Curriculums");

            migrationBuilder.RenameColumn(
                name: "SubjectGroupTypeId",
                table: "SubjectGroups",
                newName: "SubjectGroupType");
        }
    }
}
