using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoursesHelperWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomsTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TakenTogethers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TakenTogethers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraEmail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraEmail_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraPhone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraPhone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraPhone_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInfos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeCertifications",
                columns: table => new
                {
                    TraineeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CertificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeCertifications", x => new { x.TraineeId, x.CertificationId });
                    table.ForeignKey(
                        name: "FK_TraineeCertifications_AspNetUsers_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeCertifications_Certifications_CertificationId",
                        column: x => x.CertificationId,
                        principalTable: "Certifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classrooms_ClassroomsTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ClassroomsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    NumberOfSessions = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CertificationCourses",
                columns: table => new
                {
                    CertificationId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationCourses", x => new { x.CertificationId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CertificationCourses_Certifications_CertificationId",
                        column: x => x.CertificationId,
                        principalTable: "Certifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificationCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => new { x.CourseId, x.PrerequisiteId });
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    MaxSeats = table.Column<int>(type: "int", nullable: false),
                    StartingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartingTime = table.Column<TimeSpan>(type: "Time", nullable: false),
                    EndingTime = table.Column<TimeSpan>(type: "Time", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSessions_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "instructorQualifications",
                columns: table => new
                {
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructorQualifications", x => new { x.InstructorId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_instructorQualifications_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_instructorQualifications_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TakenTogetherCourses",
                columns: table => new
                {
                    TakenTogetherId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TakenTogetherCourses", x => new { x.TakenTogetherId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_TakenTogetherCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TakenTogetherCourses_TakenTogethers_TakenTogetherId",
                        column: x => x.TakenTogetherId,
                        principalTable: "TakenTogethers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeQualifications",
                columns: table => new
                {
                    TraineeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeQualifications", x => new { x.TraineeId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_TraineeQualifications_AspNetUsers_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeQualifications_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeSessions",
                columns: table => new
                {
                    TraineeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeSessions", x => new { x.TraineeId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_TraineeSessions_AspNetUsers_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeSessions_CourseSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CourseSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "admin-role", null, "Admin", "ADMIN" },
                    { "coordinator-role", null, "Coordinator", "COORDINATOR" },
                    { "instructor-role", null, "Instructor", "INSTRUCTOR" },
                    { "trainee-role", null, "Trainee", "TRAINEE" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "admin-user", 0, "ADMIN_STAMP", "admin@uni.edu", true, true, null, "ADMIN@UNI.EDU", "ADMIN@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_ADMIN", false, "admin@uni.edu", 4 },
                    { "coordinator-user", 0, "COORDINATOR_STAMP", "coord@uni.edu", true, true, null, "COORD@UNI.EDU", "COORD@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_COORD", false, "coord@uni.edu", 3 },
                    { "instructor-user", 0, "INSTRUCTOR_STAMP", "teacher@uni.edu", true, true, null, "TEACHER@UNI.EDU", "TEACHER@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_INST", false, "teacher@uni.edu", 2 },
                    { "trainee-user", 0, "TRAINEE_STAMP", "student@uni.edu", true, true, null, "STUDENT@UNI.EDU", "STUDENT@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE", false, "student@uni.edu", 1 },
                    { "trainee-user2", 0, "TRAINEE_STAMP2", "student2@uni.edu", true, true, null, "STUDENT2@UNI.EDU", "STUDENT2@UNI.EDU", "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==", null, false, "STAMP_TRAINEE2", false, "student2@uni.edu", 1 }
                });

            migrationBuilder.InsertData(
                table: "Certifications",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "A certification in Information Technology.", "IT Cert" },
                    { 2, "A certification in programming.", "Programming Cert" }
                });

            migrationBuilder.InsertData(
                table: "ClassroomsTypes",
                columns: new[] { "Id", "Capacity", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 20, "A room with 20+ computers and a smart board.", "Computer lab" },
                    { 2, 20, "A room wtih engineering equipment for 20+ students.", "Engineering room" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Information Technology: A subject focusing on hardware configuration and repair.", "IT" },
                    { 2, "Programming: A subject focusing on software development.", "Programming" },
                    { 3, "Business: A subject focusing on managing companies and finances.", "Business" },
                    { 4, "Mechanical Engineering: A subject focusing on moving hardware systems", "Mechanical Engineering" }
                });

            migrationBuilder.InsertData(
                table: "TakenTogethers",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Two IT courses that must be taken together after completing the introduction.", "IT Program" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "admin-role", "admin-user" },
                    { "coordinator-role", "coordinator-user" },
                    { "instructor-role", "instructor-user" },
                    { "trainee-role", "trainee-user" },
                    { "trainee-role", "trainee-user2" }
                });

            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Description", "TypeId" },
                values: new object[,]
                {
                    { 1, "Computer lab 19.101", 1 },
                    { 2, "Computer lab 19.102", 1 },
                    { 3, "Computer lab 19.103", 1 },
                    { 4, "Engineering room 2.1", 2 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Description", "Name", "NumberOfSessions", "Price", "SubjectId" },
                values: new object[,]
                {
                    { 1, "Introduction to Information Technology (IT).", "IT 1", 10, 100m, 1 },
                    { 2, "Introduction to Programming.", "Programming 1", 10, 100m, 2 },
                    { 3, "Introduction to Business.", "Business 1", 12, 120m, 3 },
                    { 4, "Introduction to Mechanical Engineering.", "Mechanical Engineering 1", 15, 150m, 4 },
                    { 5, "Continuation to Information Technology (IT).", "IT 2", 10, 100m, 1 },
                    { 6, "Continuation to Information Technology (IT).", "IT 3", 10, 100m, 1 }
                });

            migrationBuilder.InsertData(
                table: "ExtraEmail",
                columns: new[] { "Id", "Email", "UserId" },
                values: new object[,]
                {
                    { 1, "stu1extra@uni.edu", "trainee-user" },
                    { 2, "stu2extra@uni.edu", "trainee-user2" }
                });

            migrationBuilder.InsertData(
                table: "ExtraPhone",
                columns: new[] { "Id", "Phone", "UserId" },
                values: new object[,]
                {
                    { 1, "9310134141242141", "trainee-user" },
                    { 2, "901204124014021", "trainee-user2" }
                });

            migrationBuilder.InsertData(
                table: "TraineeCertifications",
                columns: new[] { "CertificationId", "TraineeId" },
                values: new object[] { 2, "trainee-user2" });

            migrationBuilder.InsertData(
                table: "UserInfos",
                columns: new[] { "UserId", "DateOfBirth", "Description", "FirstName", "LastName" },
                values: new object[,]
                {
                    { "admin-user", new DateOnly(1980, 1, 1), null, "Example", "Admin" },
                    { "coordinator-user", new DateOnly(1985, 5, 15), null, "Example", "Coordinator" },
                    { "instructor-user", new DateOnly(1975, 10, 20), null, "Example", "Instructor" },
                    { "trainee-user", new DateOnly(2000, 12, 5), null, "Example", "Trainee" },
                    { "trainee-user2", new DateOnly(2000, 12, 5), null, "Example", "Trainee2" }
                });

            migrationBuilder.InsertData(
                table: "CertificationCourses",
                columns: new[] { "CertificationId", "CourseId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 5 },
                    { 1, 6 },
                    { 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "CoursePrerequisites",
                columns: new[] { "CourseId", "PrerequisiteId" },
                values: new object[,]
                {
                    { 5, 1 },
                    { 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "CourseSessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "DayOfWeek", "EndingTime", "InstructorId", "MaxSeats", "StartingDate", "StartingTime" },
                values: new object[,]
                {
                    { 1, 1, 5, 1, new TimeSpan(0, 16, 0, 0, 0), "instructor-user", 20, new DateOnly(2026, 1, 1), new TimeSpan(0, 14, 0, 0, 0) },
                    { 2, 1, 5, 1, new TimeSpan(0, 16, 0, 0, 0), "instructor-user", 20, new DateOnly(2026, 1, 1), new TimeSpan(0, 14, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TakenTogetherCourses",
                columns: new[] { "CourseId", "TakenTogetherId" },
                values: new object[,]
                {
                    { 5, 1 },
                    { 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "TraineeQualifications",
                columns: new[] { "CourseId", "TraineeId" },
                values: new object[,]
                {
                    { 1, "trainee-user" },
                    { 2, "trainee-user2" }
                });

            migrationBuilder.InsertData(
                table: "instructorQualifications",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[,]
                {
                    { 5, "instructor-user" },
                    { 6, "instructor-user" }
                });

            migrationBuilder.InsertData(
                table: "TraineeSessions",
                columns: new[] { "SessionId", "TraineeId", "AmountPaid", "Status" },
                values: new object[,]
                {
                    { 1, "trainee-user", 100m, 1 },
                    { 2, "trainee-user", 100m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationCourses_CourseId",
                table: "CertificationCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_TypeId",
                table: "Classrooms",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_PrerequisiteId",
                table: "CoursePrerequisites",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectId",
                table: "Courses",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_ClassroomId",
                table: "CourseSessions",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_CourseId",
                table: "CourseSessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_InstructorId",
                table: "CourseSessions",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraEmail_UserId_Email",
                table: "ExtraEmail",
                columns: new[] { "UserId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPhone_UserId_Phone",
                table: "ExtraPhone",
                columns: new[] { "UserId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_instructorQualifications_CourseId",
                table: "instructorQualifications",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TakenTogetherCourses_CourseId",
                table: "TakenTogetherCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertifications_CertificationId",
                table: "TraineeCertifications",
                column: "CertificationId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeQualifications_CourseId",
                table: "TraineeQualifications",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeSessions_SessionId",
                table: "TraineeSessions",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CertificationCourses");

            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "ExtraEmail");

            migrationBuilder.DropTable(
                name: "ExtraPhone");

            migrationBuilder.DropTable(
                name: "instructorQualifications");

            migrationBuilder.DropTable(
                name: "TakenTogetherCourses");

            migrationBuilder.DropTable(
                name: "TraineeCertifications");

            migrationBuilder.DropTable(
                name: "TraineeQualifications");

            migrationBuilder.DropTable(
                name: "TraineeSessions");

            migrationBuilder.DropTable(
                name: "UserInfos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TakenTogethers");

            migrationBuilder.DropTable(
                name: "Certifications");

            migrationBuilder.DropTable(
                name: "CourseSessions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "ClassroomsTypes");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
