using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class courseupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentContents");

            migrationBuilder.DropTable(
                name: "LinkContents");

            migrationBuilder.DropTable(
                name: "VideoContents");

            migrationBuilder.RenameColumn(
                name: "GradeId",
                table: "Courses",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseImage",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Curriculum",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromotionVideo",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedById",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudiance",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "", true, "Programming" },
                    { 2, "", true, "Design" },
                    { 3, "", true, "Mathematics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ReviewedById",
                table: "Courses",
                column: "ReviewedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Admins_ReviewedById",
                table: "Courses",
                column: "ReviewedById",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Admins_ReviewedById",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ReviewedById",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseImage",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Curriculum",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PromotionVideo",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ReviewedById",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TargetAudiance",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Courses",
                newName: "GradeId");

            migrationBuilder.CreateTable(
                name: "DocumentContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentContents_ContentItems_Id",
                        column: x => x.Id,
                        principalTable: "ContentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkContents_ContentItems_Id",
                        column: x => x.Id,
                        principalTable: "ContentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoContents_ContentItems_Id",
                        column: x => x.Id,
                        principalTable: "ContentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
