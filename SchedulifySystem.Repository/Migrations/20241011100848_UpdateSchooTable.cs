using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchooTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Schools_SchoolId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PrincipalName",
                table: "Schools");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Account",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Schools_SchoolId",
                table: "Account",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Schools_SchoolId",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "PrincipalName",
                table: "Schools",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Schools_SchoolId",
                table: "Account",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
