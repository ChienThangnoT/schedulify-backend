using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSubjectGroupTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_Schools_SchoolId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SubjectGroupType_SubjectGroupTypeId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectGroupType");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_SubjectGroupTypeId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "SubjectGroupTypeId",
                table: "SubjectGroups");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Subjects",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SlotSpecialized",
                table: "Subjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalSlotInYear",
                table: "Subjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialized",
                table: "SubjectInGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "SubjectGroups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupType",
                table: "SubjectGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentClassId",
                table: "ClassSchedule",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentClassName",
                table: "ClassSchedule",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimeSlotId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DateOfWeek",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClassScheduleId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "RoomCode",
                table: "ClassPeriod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectAbbreviation",
                table: "ClassPeriod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherAbbreviation",
                table: "ClassPeriod",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherAssignmentId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectGroupId",
                table: "ClassGroup",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolYearId",
                table: "ClassGroup",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "ClassGroup",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "RoomSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomSubjects_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedule_StudentClassId",
                table: "ClassSchedule",
                column: "StudentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_TeacherAssignmentId",
                table: "ClassPeriod",
                column: "TeacherAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomSubjects_RoomId",
                table: "RoomSubjects",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomSubjects_SubjectId",
                table: "RoomSubjects",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                table: "ClassGroup",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_Schools_SchoolId",
                table: "ClassGroup",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPeriod_TeacherAssignments_TeacherAssignmentId",
                table: "ClassPeriod",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedule_StudentClasses_StudentClassId",
                table: "ClassSchedule",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_Schools_SchoolId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPeriod_TeacherAssignments_TeacherAssignmentId",
                table: "ClassPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedule_StudentClasses_StudentClassId",
                table: "ClassSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "RoomSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSchedule_StudentClassId",
                table: "ClassSchedule");

            migrationBuilder.DropIndex(
                name: "IX_ClassPeriod_TeacherAssignmentId",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "SlotSpecialized",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "TotalSlotInYear",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsSpecialized",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "SubjectGroupType",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "StudentClassId",
                table: "ClassSchedule");

            migrationBuilder.DropColumn(
                name: "StudentClassName",
                table: "ClassSchedule");

            migrationBuilder.DropColumn(
                name: "RoomCode",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "SubjectAbbreviation",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "TeacherAbbreviation",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "TeacherAssignmentId",
                table: "ClassPeriod");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "SubjectGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupTypeId",
                table: "SubjectGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TimeSlotId",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DateOfWeek",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassScheduleId",
                table: "ClassPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectGroupId",
                table: "ClassGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolYearId",
                table: "ClassGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "ClassGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectGroupType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefaultSlotPerTerm = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroupType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_SubjectGroupTypeId",
                table: "SubjectGroups",
                column: "SubjectGroupTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                table: "ClassGroup",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_Schools_SchoolId",
                table: "ClassGroup",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroup_SubjectGroups_SubjectGroupId",
                table: "ClassGroup",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Schools_SchoolId",
                table: "SubjectGroups",
                column: "SchoolId",
                principalTable: "Schools",
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
                name: "FK_Subjects_Schools_SchoolId",
                table: "Subjects",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
