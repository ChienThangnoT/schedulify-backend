using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulifySystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCodeNumberToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Districts_DistrictId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsMainSession",
                table: "SubjectConfigs");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Schools",
                newName: "ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_DistrictId",
                table: "Schools",
                newName: "IX_Schools_ProvinceId");

            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoomTypeCode",
                table: "RoomTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomCode",
                table: "Rooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "Districts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingCode",
                table: "Building",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_Provinces_ProvinceId",
                table: "Schools",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Provinces_ProvinceId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "RoomTypeCode",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "RoomCode",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "BuildingCode",
                table: "Building");

            migrationBuilder.RenameColumn(
                name: "ProvinceId",
                table: "Schools",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Schools_ProvinceId",
                table: "Schools",
                newName: "IX_Schools_DistrictId");

            migrationBuilder.AddColumn<bool>(
                name: "IsMainSession",
                table: "SubjectConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_Districts_DistrictId",
                table: "Schools",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
