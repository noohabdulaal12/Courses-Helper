using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdjustDemoArabSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "instructor-role", "instructor-user4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user10" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user11" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user12" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user8" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user9" });

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user4");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user10");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user11");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user12");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user8");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user10");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user11");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user12");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user9");

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user2",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Fatima", "Al-Harbi" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user3",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Khalid", "Al-Mansour" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user3",
                column: "LastName",
                value: "Al-Qahtani");

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user4",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Mohammed", "Al-Farsi" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user5",
                column: "LastName",
                value: "Al-Najjar");

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user6",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Youssef", "Al-Hassan" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user7",
                column: "LastName",
                value: "Al-Saleh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "instructor-user4", 0, "INSTRUCTOR_STAMP4", "teacher4@uni.edu", true, true, null, "TEACHER4@UNI.EDU", "TEACHER4@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_INST4", false, "teacher4@uni.edu", 2 },
                    { "trainee-user10", 0, "TRAINEE_STAMP10", "student10@uni.edu", true, true, null, "STUDENT10@UNI.EDU", "STUDENT10@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE10", false, "student10@uni.edu", 1 },
                    { "trainee-user11", 0, "TRAINEE_STAMP11", "student11@uni.edu", true, true, null, "STUDENT11@UNI.EDU", "STUDENT11@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE11", false, "student11@uni.edu", 1 },
                    { "trainee-user12", 0, "TRAINEE_STAMP12", "student12@uni.edu", true, true, null, "STUDENT12@UNI.EDU", "STUDENT12@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE12", false, "student12@uni.edu", 1 },
                    { "trainee-user8", 0, "TRAINEE_STAMP8", "student8@uni.edu", true, true, null, "STUDENT8@UNI.EDU", "STUDENT8@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE8", false, "student8@uni.edu", 1 },
                    { "trainee-user9", 0, "TRAINEE_STAMP9", "student9@uni.edu", true, true, null, "STUDENT9@UNI.EDU", "STUDENT9@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE9", false, "student9@uni.edu", 1 }
                });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user2",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Maya", "Patel" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user3",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Omar", "Haddad" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user3",
                column: "LastName",
                value: "Khan");

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user4",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Daniel", "Brown" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user5",
                column: "LastName",
                value: "Al-Farsi");

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user6",
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "James", "Miller" });

            migrationBuilder.UpdateData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user7",
                column: "LastName",
                value: "Saleh");

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "instructor-role", "instructor-user4" },
                    { "trainee-role", "trainee-user10" },
                    { "trainee-role", "trainee-user11" },
                    { "trainee-role", "trainee-user12" },
                    { "trainee-role", "trainee-user8" },
                    { "trainee-role", "trainee-user9" }
                });

            migrationBuilder.InsertData(
                table: "UserInfos",
                columns: new[] { "UserId", "DateOfBirth", "Description", "FirstName", "LastName" },
                values: new object[,]
                {
                    { "instructor-user4", new DateOnly(1988, 11, 24), null, "Sofia", "Williams" },
                    { "trainee-user10", new DateOnly(2000, 10, 2), null, "Lucas", "Wilson" },
                    { "trainee-user11", new DateOnly(2002, 12, 16), null, "Hana", "Yousef" },
                    { "trainee-user12", new DateOnly(1999, 3, 27), null, "Noah", "Taylor" },
                    { "trainee-user8", new DateOnly(1998, 1, 29), null, "Ethan", "Clark" },
                    { "trainee-user9", new DateOnly(2003, 5, 11), null, "Sara", "Ahmed" }
                });
        }
    }
}
