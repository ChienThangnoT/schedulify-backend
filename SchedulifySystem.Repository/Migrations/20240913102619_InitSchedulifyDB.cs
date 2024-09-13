using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitSchedulifyDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GroupType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HolidayType = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PrincipalName = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartYear = table.Column<string>(type: "text", nullable: true),
                    EndYear = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttributeCode = table.Column<string>(type: "text", nullable: true),
                    ConfigGroupId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsHardConfig = table.Column<int>(type: "integer", nullable: false),
                    IsRequire = table.Column<bool>(type: "boolean", nullable: false),
                    MinValue = table.Column<int>(type: "integer", nullable: false),
                    MaxValue = table.Column<int>(type: "integer", nullable: false),
                    DefaultValue = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Account_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Building_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTypes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "ClassGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassGroup_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassGroup_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terms_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Terms_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RoomTypeId = table.Column<int>(type: "integer", nullable: false),
                    MaxClassPerTime = table.Column<int>(type: "integer", nullable: false),
                    BuildingId = table.Column<int>(type: "integer", nullable: false),
                    AvailabilityeStatus = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    TeacherGroupId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teachers_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teachers_TeacherGroups_TeacherGroupId",
                        column: x => x.TeacherGroupId,
                        principalTable: "TeacherGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Curriculums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    ClassGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Curriculums_ClassGroup_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "SubjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClassGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectGroups_ClassGroup_ClassGroupId",
                        column: x => x.ClassGroupId,
                        principalTable: "ClassGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    TermId = table.Column<int>(type: "integer", nullable: false),
                    ApplyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WeeklyRange = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ScheduleType = table.Column<int>(type: "integer", nullable: false),
                    MainSession = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSchedules_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolSchedules_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolSchedules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolSchedules_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HomeroomTeacherId = table.Column<int>(type: "integer", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    SchoolYearId = table.Column<int>(type: "integer", nullable: false),
                    MainSession = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentClasses_SchoolYears_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClasses_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClasses_Teachers_HomeroomTeacherId",
                        column: x => x.HomeroomTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeachableSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachableSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeachableSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeachableSubjects_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherUnavailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    DateOfWeek = table.Column<int>(type: "integer", nullable: true),
                    WeekNumber = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherUnavailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherUnavailabilities_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectInGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SubjectGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectInGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectInGroups_SubjectGroups_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectInGroups_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedule_SchoolSchedules_SchoolScheduleId",
                        column: x => x.SchoolScheduleId,
                        principalTable: "SchoolSchedules",
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
                    Value = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "TeacherConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigAttributeId = table.Column<int>(type: "integer", nullable: false),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "StudentClassInGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    ClassGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SubjectConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    CurriculumId = table.Column<int>(type: "integer", nullable: false),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    SchoolScheduleId = table.Column<int>(type: "integer", nullable: false),
                    IsMainSession = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    ConfigAttributeId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                        name: "FK_SubjectConfigs_Curriculums_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curriculums",
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
                name: "TeacherAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeachableSubjectId = table.Column<int>(type: "integer", nullable: false),
                    StudentClassId = table.Column<int>(type: "integer", nullable: false),
                    AssignmentType = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAssignments_StudentClasses_StudentClassId",
                        column: x => x.StudentClassId,
                        principalTable: "StudentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherAssignments_TeachableSubjects_TeachableSubjectId",
                        column: x => x.TeachableSubjectId,
                        principalTable: "TeachableSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassScheduleId = table.Column<int>(type: "integer", nullable: false),
                    TimeSlotId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassPeriod_ClassSchedule_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPeriod_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPeriod_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPeriod_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassPeriod_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_SchoolId",
                table: "Account",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Building_SchoolId",
                table: "Building",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SchoolId",
                table: "ClassGroup",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroup_SchoolYearId",
                table: "ClassGroup",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_ClassScheduleId",
                table: "ClassPeriod",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_RoomId",
                table: "ClassPeriod",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_SubjectId",
                table: "ClassPeriod",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_TeacherId",
                table: "ClassPeriod",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPeriod_TimeSlotId",
                table: "ClassPeriod",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedule_SchoolScheduleId",
                table: "ClassSchedule",
                column: "SchoolScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigAttributes_ConfigGroupId",
                table: "ConfigAttributes",
                column: "ConfigGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_ClassGroupId",
                table: "Curriculums",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolId",
                table: "Curriculums",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_SchoolYearId",
                table: "Curriculums",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_SchoolId",
                table: "Departments",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_SchoolId",
                table: "RoomTypes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleConfigs_ConfigAttributeId",
                table: "ScheduleConfigs",
                column: "ConfigAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleConfigs_SchoolScheduleId",
                table: "ScheduleConfigs",
                column: "SchoolScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSchedules_SchoolId",
                table: "SchoolSchedules",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSchedules_SchoolYearId",
                table: "SchoolSchedules",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSchedules_SubjectId",
                table: "SchoolSchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSchedules_TermId",
                table: "SchoolSchedules",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_HomeroomTeacherId",
                table: "StudentClasses",
                column: "HomeroomTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_SchoolId",
                table: "StudentClasses",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_SchoolYearId",
                table: "StudentClasses",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassInGroups_ClassGroupId",
                table: "StudentClassInGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassInGroups_StudentClassId",
                table: "StudentClassInGroups",
                column: "StudentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_ConfigAttributeId",
                table: "SubjectConfigs",
                column: "ConfigAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectConfigs_CurriculumId",
                table: "SubjectConfigs",
                column: "CurriculumId");

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
                name: "IX_SubjectGroups_ClassGroupId",
                table: "SubjectGroups",
                column: "ClassGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_SubjectGroupId",
                table: "SubjectInGroups",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInGroups_SubjectId",
                table: "SubjectInGroups",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachableSubjects_SubjectId",
                table: "TeachableSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachableSubjects_TeacherId",
                table: "TeachableSubjects",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_StudentClassId",
                table: "TeacherAssignments",
                column: "StudentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeachableSubjectId",
                table: "TeacherAssignments",
                column: "TeachableSubjectId");

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
                name: "IX_TeacherGroups_SchoolId",
                table: "TeacherGroups",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_DepartmentId",
                table: "Teachers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_SchoolId",
                table: "Teachers",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherGroupId",
                table: "Teachers",
                column: "TeacherGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherUnavailabilities_TeacherId",
                table: "TeacherUnavailabilities",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Terms_SchoolId",
                table: "Terms",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Terms_SchoolYearId",
                table: "Terms",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SchoolId",
                table: "TimeSlots",
                column: "SchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "ClassPeriod");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "ScheduleConfigs");

            migrationBuilder.DropTable(
                name: "StudentClassInGroups");

            migrationBuilder.DropTable(
                name: "SubjectConfigs");

            migrationBuilder.DropTable(
                name: "SubjectInGroups");

            migrationBuilder.DropTable(
                name: "TeacherAssignments");

            migrationBuilder.DropTable(
                name: "TeacherConfigs");

            migrationBuilder.DropTable(
                name: "TeacherUnavailabilities");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "ClassSchedule");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "Curriculums");

            migrationBuilder.DropTable(
                name: "SubjectGroups");

            migrationBuilder.DropTable(
                name: "StudentClasses");

            migrationBuilder.DropTable(
                name: "TeachableSubjects");

            migrationBuilder.DropTable(
                name: "ConfigAttributes");

            migrationBuilder.DropTable(
                name: "SchoolSchedules");

            migrationBuilder.DropTable(
                name: "Building");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "ClassGroup");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "ConfigGroup");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Terms");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "TeacherGroups");

            migrationBuilder.DropTable(
                name: "SchoolYears");

            migrationBuilder.DropTable(
                name: "Schools");
        }
    }
}
