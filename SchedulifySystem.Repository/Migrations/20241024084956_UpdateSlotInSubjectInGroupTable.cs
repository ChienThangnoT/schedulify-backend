using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSlotInSubjectInGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MoringSlotPerWeek",
                table: "SubjectInGroups",
                newName: "SubSlotPerWeek");

            migrationBuilder.RenameColumn(
                name: "AfternoonSlotPerWeek",
                table: "SubjectInGroups",
                newName: "MainSlotPerWeek");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubSlotPerWeek",
                table: "SubjectInGroups",
                newName: "MoringSlotPerWeek");

            migrationBuilder.RenameColumn(
                name: "MainSlotPerWeek",
                table: "SubjectInGroups",
                newName: "AfternoonSlotPerWeek");
        }
    }
}
