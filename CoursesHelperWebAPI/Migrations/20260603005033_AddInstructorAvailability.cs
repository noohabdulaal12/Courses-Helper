using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartingTime = table.Column<TimeSpan>(type: "Time", nullable: false),
                    EndingTime = table.Column<TimeSpan>(type: "Time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorAvailabilities_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAvailabilities_InstructorId_DayOfWeek_StartingTime_EndingTime",
                table: "InstructorAvailabilities",
                columns: new[] { "InstructorId", "DayOfWeek", "StartingTime", "EndingTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorAvailabilities");
        }
    }
}
