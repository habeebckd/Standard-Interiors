using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StandardInteriors.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialStandardInteriorsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatePrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentPrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DefaultEstimateNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlternatePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EstimateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PricingMode = table.Column<int>(type: "int", nullable: false),
                    ManualOverallAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EstimateNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    RevisedFromEstimateId = table.Column<int>(type: "int", nullable: true),
                    CalculatedTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculatedReceivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculatedBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estimates_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimateSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    SectionNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricingMode = table.Column<int>(type: "int", nullable: true),
                    ManualSectionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateSections_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimateStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateStatusHistory_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EstimateId = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VoidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateSectionId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ManualAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsCustomItem = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateItems_EstimateSections_EstimateSectionId",
                        column: x => x.EstimateSectionId,
                        principalTable: "EstimateSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstimateItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "CompanySettings",
                columns: new[] { "Id", "Address", "CompanyName", "CreatedAt", "Currency", "DefaultEstimateNote", "Email", "EstimatePrefix", "LogoPath", "PaymentPrefix", "Phone", "UpdatedAt" },
                values: new object[] { 1, null, "STANDARD INTERIORS", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(2021), "INR", "This estimate is prepared for the listed interior works. Final cost may vary based on site measurements, material/brand selection, design changes and additional work approved during execution.", null, "EST", "..\\..\\..\\..\\..\\..\\assets\\logo\\standard-interiors-logo.png", "PAY", null, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(2021) });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "Id", "Code", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "CASH", 1, true, "Cash" },
                    { 2, "GPAY", 2, true, "GPay" },
                    { 3, "PHONEPE", 3, true, "PhonePe" },
                    { 4, "UPI", 4, true, "UPI" },
                    { 5, "BANK", 5, true, "Bank Transfer" },
                    { 6, "CHEQUE", 6, true, "Cheque" },
                    { 7, "CARD", 7, true, "Card" },
                    { 8, "OTHER", 8, true, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "DefaultPrice", "DefaultUnit", "Description", "DisplayOrder", "IsActive", "ServiceCode", "ServiceName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1979), null, null, null, 1, true, "SRV-001", "Ceiling", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1983) },
                    { 2, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1985), null, null, null, 2, true, "SRV-002", "Electrical Work", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1986) },
                    { 3, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1987), null, null, null, 3, true, "SRV-003", "Putty", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1987) },
                    { 4, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1989), null, null, null, 4, true, "SRV-004", "Painting", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1989) },
                    { 5, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1990), null, null, null, 5, true, "SRV-005", "Lights", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1990) },
                    { 6, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1992), null, null, null, 6, true, "SRV-006", "Curtains", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1992) },
                    { 7, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1993), null, null, null, 7, true, "SRV-007", "Wardrobe & Dressing Room", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1993) },
                    { 8, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1994), null, null, null, 8, true, "SRV-008", "Bed Set", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1994) },
                    { 9, new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1996), null, null, null, 9, true, "SRV-009", "Glass Partition", new DateTime(2026, 8, 29, 5, 28, 55, 460, DateTimeKind.Utc).AddTicks(1996) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Customers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_EstimateSectionId",
                table: "EstimateItems",
                column: "EstimateSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_ServiceId",
                table: "EstimateItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_CustomerId",
                table: "Estimates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_EstimateDate",
                table: "Estimates",
                column: "EstimateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_EstimateNumber",
                table: "Estimates",
                column: "EstimateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstimateSections_EstimateId",
                table: "EstimateSections",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateStatusHistory_EstimateId",
                table: "EstimateStatusHistory",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Code",
                table: "PaymentMethods",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EstimateId",
                table: "Payments",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentNumber",
                table: "Payments",
                column: "PaymentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCode",
                table: "Services",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceName",
                table: "Services",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySettings");

            migrationBuilder.DropTable(
                name: "EstimateItems");

            migrationBuilder.DropTable(
                name: "EstimateStatusHistory");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "EstimateSections");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Estimates");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
