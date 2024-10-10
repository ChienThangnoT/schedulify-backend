using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAcountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_EducationDepartments_EducationDepartmentId",
                table: "Schools");

            migrationBuilder.DropTable(
                name: "EducationDepartments");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Building");

            migrationBuilder.RenameColumn(
                name: "EducationDepartmentId",
                table: "Schools",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_EducationDepartmentId",
                table: "Schools",
                newName: "IX_Schools_DistrictId");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SubjectGroupType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "SubjectGroups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolYearCode",
                table: "SchoolYears",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassGroupCode",
                table: "ClassGroup",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Districts",
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
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OTP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTP_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_AccountId",
                table: "OTP",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_Districts_DistrictId",
                table: "Schools",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Districts_DistrictId",
                table: "Schools");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "OTP");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SubjectGroupType");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "SchoolYearCode",
                table: "SchoolYears");

            migrationBuilder.DropColumn(
                name: "ClassGroupCode",
                table: "ClassGroup");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Schools",
                newName: "EducationDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_DistrictId",
                table: "Schools",
                newName: "IX_Schools_EducationDepartmentId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Building",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducationDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProvinceId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_EducationDepartments_ProvinceId",
                table: "EducationDepartments",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_EducationDepartments_EducationDepartmentId",
                table: "Schools",
                column: "EducationDepartmentId",
                principalTable: "EducationDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
