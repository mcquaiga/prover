using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Verifications.TestRun",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchivedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    Device = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    DeviceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TestDateTime = table.Column<DateTimeOffset>(nullable: false),
                    InputDriveType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications.TestRun", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verifications.TestRunTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Verified = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    EvcVerificationSqlId = table.Column<Guid>(nullable: true),
                    VerificationTestPointId = table.Column<Guid>(nullable: true),
                    AppliedInput = table.Column<decimal>(type: "decimal", nullable: true),
                    TestNumber = table.Column<int>(nullable: true),
                    ExpectedValue = table.Column<decimal>(type: "decimal", nullable: true),
                    ActualValue = table.Column<decimal>(type: "decimal", nullable: true),
                    PercentError = table.Column<decimal>(type: "decimal", nullable: true),
                    Values = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    GaugePressure = table.Column<decimal>(type: "decimal", nullable: true),
                    AtmosphericGauge = table.Column<decimal>(type: "decimal", nullable: true),
                    GaugeTemperature = table.Column<decimal>(type: "decimal", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications.TestRunTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Verifications.TestRunTests_Verifications.TestRun_EvcVerificationSqlId",
                        column: x => x.EvcVerificationSqlId,
                        principalTable: "Verifications.TestRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Verifications.TestRunTests_Verifications.TestRunTests_VerificationTestPointId",
                        column: x => x.VerificationTestPointId,
                        principalTable: "Verifications.TestRunTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Verifications.TestRunTests_EvcVerificationSqlId",
                table: "Verifications.TestRunTests",
                column: "EvcVerificationSqlId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications.TestRunTests_VerificationTestPointId",
                table: "Verifications.TestRunTests",
                column: "VerificationTestPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Verifications.TestRunTests");

            migrationBuilder.DropTable(
                name: "Verifications.TestRun");
        }
    }
}
