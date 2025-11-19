using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaymentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableBalance",
                table: "Teachers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LifetimeEarnings",
                table: "Teachers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PaymentTransactionId",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InstructorPayouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BankAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorPayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorPayouts_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformFinances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalCommissionEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaidToInstructors = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformFinances", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PlatformFinances",
                columns: new[] { "Id", "TotalCommissionEarned", "TotalPaidToInstructors" },
                values: new object[] { 1, 0m, 0m });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_PaymentTransactionId",
                table: "Enrollments",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorPayouts_TeacherId",
                table: "InstructorPayouts",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CourseId",
                table: "PaymentTransactions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StudentId",
                table: "PaymentTransactions",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_PaymentTransactions_PaymentTransactionId",
                table: "Enrollments",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_PaymentTransactions_PaymentTransactionId",
                table: "Enrollments");

            migrationBuilder.DropTable(
                name: "InstructorPayouts");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PlatformFinances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_PaymentTransactionId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "AvailableBalance",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "LifetimeEarnings",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Admins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" });
        }
    }
}
