using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDemoUserNameBasedEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user2",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "fatima.alharbi@uni.edu", "FATIMA.ALHARBI@UNI.EDU", "FATIMA.ALHARBI@UNI.EDU", "fatima.alharbi@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user3",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "khalid.almansour@uni.edu", "KHALID.ALMANSOUR@UNI.EDU", "KHALID.ALMANSOUR@UNI.EDU", "khalid.almansour@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user3",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "aisha.alqahtani@uni.edu", "AISHA.ALQAHTANI@UNI.EDU", "AISHA.ALQAHTANI@UNI.EDU", "aisha.alqahtani@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user4",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "mohammed.alfarsi@uni.edu", "MOHAMMED.ALFARSI@UNI.EDU", "MOHAMMED.ALFARSI@UNI.EDU", "mohammed.alfarsi@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user5",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "layla.alnajjar@uni.edu", "LAYLA.ALNAJJAR@UNI.EDU", "LAYLA.ALNAJJAR@UNI.EDU", "layla.alnajjar@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user6",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "youssef.alhassan@uni.edu", "YOUSSEF.ALHASSAN@UNI.EDU", "YOUSSEF.ALHASSAN@UNI.EDU", "youssef.alhassan@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user7",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "noura.alsaleh@uni.edu", "NOURA.ALSALEH@UNI.EDU", "NOURA.ALSALEH@UNI.EDU", "noura.alsaleh@uni.edu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user2",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "teacher2@uni.edu", "TEACHER2@UNI.EDU", "TEACHER2@UNI.EDU", "teacher2@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user3",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "teacher3@uni.edu", "TEACHER3@UNI.EDU", "TEACHER3@UNI.EDU", "teacher3@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user3",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "student3@uni.edu", "STUDENT3@UNI.EDU", "STUDENT3@UNI.EDU", "student3@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user4",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "student4@uni.edu", "STUDENT4@UNI.EDU", "STUDENT4@UNI.EDU", "student4@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user5",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "student5@uni.edu", "STUDENT5@UNI.EDU", "STUDENT5@UNI.EDU", "student5@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user6",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "student6@uni.edu", "STUDENT6@UNI.EDU", "STUDENT6@UNI.EDU", "student6@uni.edu" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "trainee-user7",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "student7@uni.edu", "STUDENT7@UNI.EDU", "STUDENT7@UNI.EDU", "student7@uni.edu" });
        }
    }
}
