using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Bankinfoupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "InstructorPayouts");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNo",
                table: "InstructorPayouts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountNo",
                table: "InstructorPayouts");

            migrationBuilder.AddColumn<int>(
                name: "BankAccountId",
                table: "InstructorPayouts",
                type: "int",
                nullable: true);
        }
    }
}
