using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoInstructorAndTraineeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "instructor-user2", 0, "INSTRUCTOR_STAMP2", "teacher2@uni.edu", true, true, null, "TEACHER2@UNI.EDU", "TEACHER2@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_INST2", false, "teacher2@uni.edu", 2 },
                    { "instructor-user3", 0, "INSTRUCTOR_STAMP3", "teacher3@uni.edu", true, true, null, "TEACHER3@UNI.EDU", "TEACHER3@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_INST3", false, "teacher3@uni.edu", 2 },
                    { "instructor-user4", 0, "INSTRUCTOR_STAMP4", "teacher4@uni.edu", true, true, null, "TEACHER4@UNI.EDU", "TEACHER4@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_INST4", false, "teacher4@uni.edu", 2 },
                    { "trainee-user10", 0, "TRAINEE_STAMP10", "student10@uni.edu", true, true, null, "STUDENT10@UNI.EDU", "STUDENT10@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE10", false, "student10@uni.edu", 1 },
                    { "trainee-user11", 0, "TRAINEE_STAMP11", "student11@uni.edu", true, true, null, "STUDENT11@UNI.EDU", "STUDENT11@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE11", false, "student11@uni.edu", 1 },
                    { "trainee-user12", 0, "TRAINEE_STAMP12", "student12@uni.edu", true, true, null, "STUDENT12@UNI.EDU", "STUDENT12@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE12", false, "student12@uni.edu", 1 },
                    { "trainee-user3", 0, "TRAINEE_STAMP3", "student3@uni.edu", true, true, null, "STUDENT3@UNI.EDU", "STUDENT3@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE3", false, "student3@uni.edu", 1 },
                    { "trainee-user4", 0, "TRAINEE_STAMP4", "student4@uni.edu", true, true, null, "STUDENT4@UNI.EDU", "STUDENT4@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE4", false, "student4@uni.edu", 1 },
                    { "trainee-user5", 0, "TRAINEE_STAMP5", "student5@uni.edu", true, true, null, "STUDENT5@UNI.EDU", "STUDENT5@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE5", false, "student5@uni.edu", 1 },
                    { "trainee-user6", 0, "TRAINEE_STAMP6", "student6@uni.edu", true, true, null, "STUDENT6@UNI.EDU", "STUDENT6@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE6", false, "student6@uni.edu", 1 },
                    { "trainee-user7", 0, "TRAINEE_STAMP7", "student7@uni.edu", true, true, null, "STUDENT7@UNI.EDU", "STUDENT7@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE7", false, "student7@uni.edu", 1 },
                    { "trainee-user8", 0, "TRAINEE_STAMP8", "student8@uni.edu", true, true, null, "STUDENT8@UNI.EDU", "STUDENT8@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE8", false, "student8@uni.edu", 1 },
                    { "trainee-user9", 0, "TRAINEE_STAMP9", "student9@uni.edu", true, true, null, "STUDENT9@UNI.EDU", "STUDENT9@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE9", false, "student9@uni.edu", 1 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "instructor-role", "instructor-user2" },
                    { "instructor-role", "instructor-user3" },
                    { "instructor-role", "instructor-user4" },
                    { "trainee-role", "trainee-user10" },
                    { "trainee-role", "trainee-user11" },
                    { "trainee-role", "trainee-user12" },
                    { "trainee-role", "trainee-user3" },
                    { "trainee-role", "trainee-user4" },
                    { "trainee-role", "trainee-user5" },
                    { "trainee-role", "trainee-user6" },
                    { "trainee-role", "trainee-user7" },
                    { "trainee-role", "trainee-user8" },
                    { "trainee-role", "trainee-user9" }
                });

            migrationBuilder.InsertData(
                table: "UserInfos",
                columns: new[] { "UserId", "DateOfBirth", "Description", "FirstName", "LastName" },
                values: new object[,]
                {
                    { "instructor-user2", new DateOnly(1981, 3, 12), null, "Maya", "Patel" },
                    { "instructor-user3", new DateOnly(1978, 7, 9), null, "Omar", "Haddad" },
                    { "instructor-user4", new DateOnly(1988, 11, 24), null, "Sofia", "Williams" },
                    { "trainee-user10", new DateOnly(2000, 10, 2), null, "Lucas", "Wilson" },
                    { "trainee-user11", new DateOnly(2002, 12, 16), null, "Hana", "Yousef" },
                    { "trainee-user12", new DateOnly(1999, 3, 27), null, "Noah", "Taylor" },
                    { "trainee-user3", new DateOnly(2001, 2, 14), null, "Aisha", "Khan" },
                    { "trainee-user4", new DateOnly(1999, 8, 3), null, "Daniel", "Brown" },
                    { "trainee-user5", new DateOnly(2002, 4, 18), null, "Layla", "Al-Farsi" },
                    { "trainee-user6", new DateOnly(2000, 6, 21), null, "James", "Miller" },
                    { "trainee-user7", new DateOnly(2001, 9, 7), null, "Noura", "Saleh" },
                    { "trainee-user8", new DateOnly(1998, 1, 29), null, "Ethan", "Clark" },
                    { "trainee-user9", new DateOnly(2003, 5, 11), null, "Sara", "Ahmed" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "instructor-role", "instructor-user2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "instructor-role", "instructor-user3" });

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
                keyValues: new object[] { "trainee-role", "trainee-user3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "trainee-role", "trainee-user7" });

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
                keyValue: "instructor-user2");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "instructor-user3");

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
                keyValue: "trainee-user3");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user4");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user5");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user6");

            migrationBuilder.DeleteData(
                table: "UserInfos",
                keyColumn: "UserId",
                keyValue: "trainee-user7");

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
                keyValue: "instructor-user2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user3");

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
                keyValue: "trainee-user3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user9");
        }
    }
}
