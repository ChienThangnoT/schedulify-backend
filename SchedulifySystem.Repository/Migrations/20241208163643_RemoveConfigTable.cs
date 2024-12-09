using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassPeriod_TimeSlots_TimeSlotId",
                table: "ClassPeriod");

            migrationBuilder.DropTable(
                name: "ScheduleConfigs");

            migrationBuilder.DropTable(
                name: "SubjectConfigs");

            migrationBuilder.DropTable(
                name: "TeacherConfigs");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "ConfigAttributes");

            migrationBuilder.DropTable(
                name: "ConfigGroup");

            migrationBuilder.DropIndex(
                name: "IX_ClassPeriod_TimeSlotId",
                table: "ClassPeriod");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "ClassPeriod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "ClassPeriod",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfigGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GroupType = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigGroupId = table.Column<int>(type: "integer", nullable: false),
                    AttributeCode = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefaultValue = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsHardConfig = table.Column<int>(type: "integer", nullable: false),
                    IsRequire = table.Column<bool>(type: "boolean", nullable: false),
                    MaxValue = table.Column<int>(type: "integer", nullable: false),
                    MinValue = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigAttributes_ConfigGroup_ConfigGroupId",
                        column: x => x.ConfigGroupId,
                        principalTable: "ConfigGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigAttributeId = table.Column<int>(type: "integer", nullable: false),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleConfigs_ConfigAttributes_ConfigAttributeId",
                        column: x => x.ConfigAttributeId,
                        principalTable: "ConfigAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleConfigs_SchoolSchedules_SchoolScheduleId",
                        column: x => x.SchoolScheduleId,
                        principalTable: "SchoolSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigAttributeId = table.Column<int>(type: "integer", nullable: false),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectConfigs_ConfigAttributes_ConfigAttributeId",
                        column: x => x.ConfigAttributeId,
                        principalTable: "ConfigAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectConfigs_SchoolSchedules_SchoolScheduleId",
                        column: x => x.SchoolScheduleId,
                        principalTable: "SchoolSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectConfigs_StudentClasses_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectConfigs_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigAttributeId = table.Column<int>(type: "integer", nullable: false),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherConfigs_ConfigAttributes_ConfigAttributeId",
                        column: x => x.ConfigAttributeId,
                        principalTable: "ConfigAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherConfigs_SchoolSchedules_SchoolScheduleId",
                        column: x => x.SchoolScheduleId,
                        principalTable: "SchoolSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherConfigs_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_TimeSlotId",
                table: "ClassPeriod",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigAttributes_ConfigGroupId",
                table: "ConfigAttributes",
                column: "ConfigGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleConfigs_ConfigAttributeId",
                table: "ScheduleConfigs",
                column: "ConfigAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleConfigs_SchoolScheduleId",
                table: "ScheduleConfigs",
                column: "SchoolScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_ConfigAttributeId",
                table: "SubjectConfigs",
                column: "ConfigAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_SchoolScheduleId",
                table: "SubjectConfigs",
                column: "SchoolScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_StudentClassId",
                table: "SubjectConfigs",
                column: "StudentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_SubjectId",
                table: "SubjectConfigs",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherConfigs_ConfigAttributeId",
                table: "TeacherConfigs",
                column: "ConfigAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherConfigs_SchoolScheduleId",
                table: "TeacherConfigs",
                column: "SchoolScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherConfigs_TeacherId",
                table: "TeacherConfigs",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SchoolId",
                table: "TimeSlots",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPeriod_TimeSlots_TimeSlotId",
                table: "ClassPeriod",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
