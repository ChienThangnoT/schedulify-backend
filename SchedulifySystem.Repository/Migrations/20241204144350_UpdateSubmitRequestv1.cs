using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmitRequestv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolYearId",
                table: "SubmitsRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmitsRequests_SchoolYearId",
                table: "SubmitsRequests",
                column: "SchoolYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmitsRequests_SchoolYears_SchoolYearId",
                table: "SubmitsRequests",
                column: "SchoolYearId",
                principalTable: "SchoolYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmitsRequests_SchoolYears_SchoolYearId",
                table: "SubmitsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SubmitsRequests_SchoolYearId",
                table: "SubmitsRequests");

            migrationBuilder.DropColumn(
                name: "SchoolYearId",
                table: "SubmitsRequests");
        }
    }
}
