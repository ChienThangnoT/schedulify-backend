using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChangeSUbjectGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_SubjectGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropIndex(
                name: "IX_SubjectInGroups_SubjectGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "SubjectGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "SchoolSchedules");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "SchoolSchedules");

            migrationBuilder.RenameColumn(
                name: "GroupCode",
                table: "SubjectGroups",
                newName: "StudentClassGroupCode");

            migrationBuilder.RenameColumn(
                name: "WeeklyRange",
                table: "SchoolSchedules",
                newName: "StartWeek");

            migrationBuilder.RenameColumn(
                name: "MainSession",
                table: "SchoolSchedules",
                newName: "EndWeek");

            migrationBuilder.AddColumn<int>(
                name: "StudentClassRoomSubjectId",
                table: "TeacherAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentClassGroupId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurriculumDetailId",
                table: "SubjectGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HomeroomTeacherId",
                table: "StudentClasses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Curriculums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurriculumName = table.Column<string>(type: "text", nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "StudentClassRoomSubject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    RoomSubjectId = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassRoomSubject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentClassRoomSubject_RoomSubjects_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "RoomSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClassRoomSubject_StudentClasses_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_StudentClassRoomSubjectId",
                table: "TeacherAssignments",
                column: "StudentClassRoomSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_CurriculumId",
                table: "SubjectInGroups",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_StudentClassGroupId",
                table: "SubjectInGroups",
                column: "StudentClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_CurriculumDetailId",
                table: "SubjectGroups",
                column: "CurriculumDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_RoomId",
                table: "StudentClasses",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolId",
                table: "Curriculums",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolYearId",
                table: "Curriculums",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassRoomSubject_StudentClassId",
                table: "StudentClassRoomSubject",
                column: "StudentClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses",
                column: "HomeroomTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_SubjectInGroups_CurriculumDetailId",
                table: "SubjectGroups",
                column: "CurriculumDetailId",
                principalTable: "SubjectInGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_Curriculums_CurriculumId",
                table: "SubjectInGroups",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_StudentClassGroupId",
                table: "SubjectInGroups",
                column: "StudentClassGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments",
                column: "StudentClassRoomSubjectId",
                principalTable: "StudentClassRoomSubject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Rooms_RoomId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_SubjectInGroups_CurriculumDetailId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_Curriculums_CurriculumId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_StudentClassRoomSubject_StudentClassRoom~",
                table: "TeacherAssignments");

            migrationBuilder.DropTable(
                name: "Curriculums");

            migrationBuilder.DropTable(
                name: "StudentClassRoomSubject");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_StudentClassRoomSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SubjectInGroups_CurriculumId",
                table: "SubjectInGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectInGroups_StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_CurriculumDetailId",
                table: "SubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_RoomId",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "StudentClassRoomSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "StudentClassGroupId",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "CurriculumDetailId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "StudentClasses");

            migrationBuilder.RenameColumn(
                name: "StudentClassGroupCode",
                table: "SubjectGroups",
                newName: "GroupCode");

            migrationBuilder.RenameColumn(
                name: "StartWeek",
                table: "SchoolSchedules",
                newName: "WeeklyRange");

            migrationBuilder.RenameColumn(
                name: "EndWeek",
                table: "SchoolSchedules",
                newName: "MainSession");

            migrationBuilder.AddColumn<int>(
                name: "SubjectGroupId",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HomeroomTeacherId",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "SchoolSchedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "SchoolSchedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HolidayType = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holidays_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_SubjectGroupId",
                table: "SubjectInGroups",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_SchoolId",
                table: "Holidays",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                table: "StudentClasses",
                column: "HomeroomTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectInGroups_SubjectGroups_SubjectGroupId",
                table: "SubjectInGroups",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
