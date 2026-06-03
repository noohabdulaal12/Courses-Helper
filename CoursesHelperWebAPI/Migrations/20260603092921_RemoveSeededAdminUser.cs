using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeededAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "admin-role", "admin-user" });

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "admin-user");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "admin-role", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[] { "admin-user", 0, "ADMIN_STAMP", "admin@uni.edu", true, true, null, "ADMIN@UNI.EDU", "ADMIN@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_ADMIN", false, "admin@uni.edu", 4 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "admin-role", "admin-user" });

            migrationBuilder.InsertData(
                table: "UserInfos",
                columns: new[] { "UserId", "DateOfBirth", "Description", "FirstName", "LastName" },
                values: new object[] { "admin-user", new DateOnly(1980, 1, 1), null, "Example", "Admin" });
        }
    }
}
