using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOTPTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OTP_Account_AccountId",
                table: "OTP");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OTP",
                table: "OTP");

            migrationBuilder.RenameTable(
                name: "OTP",
                newName: "OTPs");

            migrationBuilder.RenameIndex(
                name: "IX_OTP_AccountId",
                table: "OTPs",
                newName: "IX_OTPs_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OTPs",
                table: "OTPs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OTPs_Account_AccountId",
                table: "OTPs",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OTPs_Account_AccountId",
                table: "OTPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OTPs",
                table: "OTPs");

            migrationBuilder.RenameTable(
                name: "OTPs",
                newName: "OTP");

            migrationBuilder.RenameIndex(
                name: "IX_OTPs_AccountId",
                table: "OTP",
                newName: "IX_OTP_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OTP",
                table: "OTP",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OTP_Account_AccountId",
                table: "OTP",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
