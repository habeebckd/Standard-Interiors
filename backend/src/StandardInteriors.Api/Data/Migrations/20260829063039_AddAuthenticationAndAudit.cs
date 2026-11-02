using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StandardInteriors.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoidedByUserId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CanVoidPayments = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedByUserId",
                table: "Payments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UpdatedByUserId",
                table: "Payments",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VoidedByUserId",
                table: "Payments",
                column: "VoidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_CreatedByUserId",
                table: "Estimates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_UpdatedByUserId",
                table: "Estimates",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CreatedByUserId",
                table: "Customers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UpdatedByUserId",
                table: "Customers",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_CreatedByUserId",
                table: "Customers",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UpdatedByUserId",
                table: "Customers",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_Users_CreatedByUserId",
                table: "Estimates",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_Users_UpdatedByUserId",
                table: "Estimates",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_CreatedByUserId",
                table: "Payments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UpdatedByUserId",
                table: "Payments",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_VoidedByUserId",
                table: "Payments",
                column: "VoidedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_CreatedByUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UpdatedByUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_Users_CreatedByUserId",
                table: "Estimates");

            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_Users_UpdatedByUserId",
                table: "Estimates");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_VoidedByUserId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VoidedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Estimates_CreatedByUserId",
                table: "Estimates");

            migrationBuilder.DropIndex(
                name: "IX_Estimates_UpdatedByUserId",
                table: "Estimates");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CreatedByUserId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UpdatedByUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VoidedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "CompanySettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(2021), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(2021) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1979), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1983) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1985), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1986) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1987), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1987) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1989), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1989) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1990), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1990) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1992), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1992) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1993), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1993) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1994), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1994) });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1996), new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1996) });
        }
    }
}
