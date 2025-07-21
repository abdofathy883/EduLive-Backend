using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HotLineFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CertificateTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HTMLFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhatsappALert",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaseUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsappALert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsappALert_AspNetUsers_BaseUserId",
                        column: x => x.BaseUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CertificateIssueds",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateIssueds", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_CertificateIssueds_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateIssueds_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateIssueds_CertificateTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CertificateTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateIssueds_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InstructorId",
                table: "Payments",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateIssueds_CourseId",
                table: "CertificateIssueds",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateIssueds_InstructorId",
                table: "CertificateIssueds",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateIssueds_StudentId",
                table: "CertificateIssueds",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateIssueds_TemplateId",
                table: "CertificateIssueds",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappALert_BaseUserId",
                table: "WhatsappALert",
                column: "BaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_InstructorId",
                table: "Payments",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_InstructorId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "CertificateIssueds");

            migrationBuilder.DropTable(
                name: "WhatsappALert");

            migrationBuilder.DropTable(
                name: "CertificateTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Payments_InstructorId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35);

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    TemplatePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualSentTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaseUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchudledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_BaseUserId",
                        column: x => x.BaseUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CourseId",
                table: "Certificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_InstructorId",
                table: "Certificates",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_StudentId",
                table: "Certificates",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BaseUserId",
                table: "Notification",
                column: "BaseUserId");
        }
    }
}
