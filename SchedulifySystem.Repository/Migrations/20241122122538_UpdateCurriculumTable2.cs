using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCurriculumTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurriculumCode",
                table: "Curriculums",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurriculumCode",
                table: "Curriculums");
        }
    }
}
