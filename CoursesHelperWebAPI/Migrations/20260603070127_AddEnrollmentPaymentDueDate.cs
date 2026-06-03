using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentPaymentDueDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PaymentDueDate",
                table: "TraineeSessions",
                type: "date",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TraineeSessions",
                keyColumns: new[] { "SessionId", "TraineeId" },
                keyValues: new object[] { 1, "trainee-user" },
                column: "PaymentDueDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "TraineeSessions",
                keyColumns: new[] { "SessionId", "TraineeId" },
                keyValues: new object[] { 2, "trainee-user" },
                column: "PaymentDueDate",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "TraineeSessions");
        }
    }
}
