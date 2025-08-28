using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "VideoContents");

            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "DocumentContents");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ModuleContentItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "DocumentContents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ContentItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ModuleContentItems");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "DocumentContents");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ContentItems");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "VideoContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "DocumentContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
