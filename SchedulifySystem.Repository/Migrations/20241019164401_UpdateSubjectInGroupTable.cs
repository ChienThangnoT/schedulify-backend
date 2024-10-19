using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubjectInGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlotPerWeek",
                table: "SubjectInGroups",
                newName: "MoringSlotPerWeek");

            migrationBuilder.AddColumn<int>(
                name: "AfternoonSlotPerWeek",
                table: "SubjectInGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoublePeriod",
                table: "SubjectInGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfternoonSlotPerWeek",
                table: "SubjectInGroups");

            migrationBuilder.DropColumn(
                name: "IsDoublePeriod",
                table: "SubjectInGroups");

            migrationBuilder.RenameColumn(
                name: "MoringSlotPerWeek",
                table: "SubjectInGroups",
                newName: "SlotPerWeek");
        }
    }
}
