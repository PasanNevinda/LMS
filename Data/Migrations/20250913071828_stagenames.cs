using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class stagenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentItems_Teachers_TeacherId",
                table: "ContentItems");

            migrationBuilder.DropTable(
                name: "ModuleContentItems");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ContentItems",
                newName: "StageName");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "ContentItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "ContentItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderNo",
                table: "ContentItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "ContentItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_ModuleId",
                table: "ContentItems",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentItems_Modules_ModuleId",
                table: "ContentItems",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentItems_Teachers_TeacherId",
                table: "ContentItems",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentItems_Modules_ModuleId",
                table: "ContentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentItems_Teachers_TeacherId",
                table: "ContentItems");

            migrationBuilder.DropIndex(
                name: "IX_ContentItems_ModuleId",
                table: "ContentItems");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "ContentItems");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "ContentItems");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "ContentItems");

            migrationBuilder.RenameColumn(
                name: "StageName",
                table: "ContentItems",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "ContentItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ModuleContentItems",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    ContentItemId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleContentItems", x => new { x.ModuleId, x.ContentItemId });
                    table.ForeignKey(
                        name: "FK_ModuleContentItems_ContentItems_ContentItemId",
                        column: x => x.ContentItemId,
                        principalTable: "ContentItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModuleContentItems_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleContentItems_ContentItemId",
                table: "ModuleContentItems",
                column: "ContentItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentItems_Teachers_TeacherId",
                table: "ContentItems",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
