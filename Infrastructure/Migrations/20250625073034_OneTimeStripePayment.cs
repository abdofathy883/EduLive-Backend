using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OneTimeStripePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AspNetUsers_StudentUserId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Courses_CourseId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Subscription_SubscriptionId",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "InstructorPayOut");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "PaymentPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_SubscriptionId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "HasCompletedStripeOnboarding",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeConnectAccountId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeChargeId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameColumn(
                name: "StudentUserId",
                table: "Payments",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "StripePaymentIntentId",
                table: "Payments",
                newName: "InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_StudentUserId",
                table: "Payments",
                newName: "IX_Payments_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_CourseId",
                table: "Payments",
                newName: "IX_Payments_CourseId");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_StudentId",
                table: "Payments",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_StudentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Payment",
                newName: "StudentUserId");

            migrationBuilder.RenameColumn(
                name: "InstructorId",
                table: "Payment",
                newName: "StripePaymentIntentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_StudentId",
                table: "Payment",
                newName: "IX_Payment_StudentUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CourseId",
                table: "Payment",
                newName: "IX_Payment_CourseId");

            migrationBuilder.AddColumn<bool>(
                name: "HasCompletedStripeOnboarding",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeConnectAccountId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "StripeChargeId",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Payment",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InstructorPayOut",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripeTransferId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorPayOut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorPayOut_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillingInterval = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StripePriceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripeProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentPlanId = table.Column<int>(type: "int", nullable: false),
                    StudentUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_PaymentPlan_PaymentPlanId",
                        column: x => x.PaymentPlanId,
                        principalTable: "PaymentPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SubscriptionId",
                table: "Payment",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorPayOut_InstructorUserId",
                table: "InstructorPayOut",
                column: "InstructorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PaymentPlanId",
                table: "Subscription",
                column: "PaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_StudentUserId",
                table: "Subscription",
                column: "StudentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AspNetUsers_StudentUserId",
                table: "Payment",
                column: "StudentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Courses_CourseId",
                table: "Payment",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Subscription_SubscriptionId",
                table: "Payment",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");
        }
    }
}
