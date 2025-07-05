using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Chat_ReportsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoogleMeetSettings");

            migrationBuilder.DropIndex(
                name: "IX_GoogleMeetLessons_LessonId",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "Date_Time",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "GoogleMeetURL",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ZoomJoinURL",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ZoomPassword",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ZoomStartUrl",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "GoogleMeetId",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "JoinUrl",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "MeetingCode",
                table: "GoogleMeetLessons");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ZoomUserConnections",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ZoomUserConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ZoomUserConnections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ZoomMeetingId",
                table: "Lessons",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoogleMeetId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GoogleMeetLessons",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "GoogleMeetLessons",
                type: "int",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GoogleCalendarId",
                table: "GoogleMeetLessons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleEventId",
                table: "GoogleMeetLessons",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMeetURL",
                table: "GoogleMeetLessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "GoogleMeetLessons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GoogleMeetLessons",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "LessonReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemorizedContent = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    ExplainedTopics = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    DiscussedValues = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonReports_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonReports_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZoomMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoomMeetingId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", maxLength: 3, nullable: false),
                    JoinUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoomMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoomMeetings_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoogleMeetLessons_LessonId",
                table: "GoogleMeetLessons",
                column: "LessonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonReports_InstructorId",
                table: "LessonReports",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonReports_StudentId",
                table: "LessonReports",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoomMeetings_LessonId",
                table: "ZoomMeetings",
                column: "LessonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonReports");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ZoomMeetings");

            migrationBuilder.DropIndex(
                name: "IX_GoogleMeetLessons_LessonId",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ZoomUserConnections");

            migrationBuilder.DropColumn(
                name: "GoogleMeetId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "GoogleCalendarId",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "GoogleEventId",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "GoogleMeetURL",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "GoogleMeetLessons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "GoogleMeetLessons");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ZoomUserConnections",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ZoomUserConnections",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ZoomMeetingId",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Time",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMeetURL",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoomJoinURL",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoomPassword",
                table: "Lessons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoomStartUrl",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMeetId",
                table: "GoogleMeetLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JoinUrl",
                table: "GoogleMeetLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MeetingCode",
                table: "GoogleMeetLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GoogleMeetSettings",
                columns: table => new
                {
                    AllowExternalParticipants = table.Column<bool>(type: "bit", nullable: false),
                    RequireAdmission = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoogleMeetLessons_LessonId",
                table: "GoogleMeetLessons",
                column: "LessonId");
        }
    }
}
