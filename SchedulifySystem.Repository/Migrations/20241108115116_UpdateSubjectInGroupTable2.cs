using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectInGroupTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "MainMinimumCouple",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "SubMinimumCouple",
                table: "SubjectInGroups");
        }
    }
}
