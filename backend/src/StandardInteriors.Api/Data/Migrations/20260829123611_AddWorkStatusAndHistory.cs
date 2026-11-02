using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StandardInteriors.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkStatusAndHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedEndDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedStartDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgressPercentage",
                table: "Estimates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResumeDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeNote",
                table: "Estimates",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StopDate",
                table: "Estimates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StopReason",
                table: "Estimates",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkStatus",
                table: "Estimates",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "WorkStatusDescription",
                table: "Estimates",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StopReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StopDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResumeDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkStatusHistories_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_WorkStatus",
                table: "Estimates",
                column: "WorkStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusHistories_ChangedByUserId",
                table: "WorkStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusHistories_EstimateId_ChangedAt",
                table: "WorkStatusHistories",
                columns: new[] { "EstimateId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_Estimates_WorkStatus",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ActualEndDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ActualStartDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "PlannedEndDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ProgressPercentage",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ResumeDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "ResumeNote",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "StopDate",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "StopReason",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "WorkStatus",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "WorkStatusDescription",
                table: "Estimates");

            migrationBuilder.UpdateData(
                table: "CompanySettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5380), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5263), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5274) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5283), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5283) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5287), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5287) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5290), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5291) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5293), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5294) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5296), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5297) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5300), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5301) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5304), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5304) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5307), new DateTime(2026, 8, 29, 6, 30, 37, 415, DateTimeKind.Utc).AddTicks(5308) });
        }
    }
}
