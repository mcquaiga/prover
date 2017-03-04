using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prover.Data.SQL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QA_PressureTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AtmosphericGauge = table.Column<decimal>(nullable: true),
                    Gauge = table.Column<decimal>(nullable: false),
                    ItemData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_PressureTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QA_TemperatureTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Gauge = table.Column<decimal>(nullable: false),
                    ItemData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_TemperatureTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QA_TestRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchivedDateTime = table.Column<DateTime>(nullable: true),
                    CommPortsPassed = table.Column<bool>(nullable: true),
                    CorrectorType = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    EventLogPassed = table.Column<bool>(nullable: true),
                    ExportedDateTime = table.Column<DateTime>(nullable: true),
                    InstrumentType = table.Column<string>(nullable: true),
                    ItemValues = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true),
                    TestDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_TestRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QA_VolumeTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppliedInput = table.Column<decimal>(nullable: false),
                    PostTestItemData = table.Column<string>(nullable: true),
                    PreTestItemData = table.Column<string>(nullable: true),
                    PulserACount = table.Column<int>(nullable: false),
                    PulserBCount = table.Column<int>(nullable: false),
                    PulserCCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_VolumeTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QA_TestPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    PressureId = table.Column<Guid>(nullable: true),
                    TemperatureId = table.Column<Guid>(nullable: true),
                    TestRunId = table.Column<Guid>(nullable: false),
                    VolumeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QA_TestPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QA_TestPoints_QA_PressureTests_PressureId",
                        column: x => x.PressureId,
                        principalTable: "QA_PressureTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QA_TestPoints_QA_TemperatureTests_TemperatureId",
                        column: x => x.TemperatureId,
                        principalTable: "QA_TemperatureTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QA_TestPoints_QA_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "QA_TestRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QA_TestPoints_QA_VolumeTests_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "QA_VolumeTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QA_TestPoints_PressureId",
                table: "QA_TestPoints",
                column: "PressureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QA_TestPoints_TemperatureId",
                table: "QA_TestPoints",
                column: "TemperatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QA_TestPoints_TestRunId",
                table: "QA_TestPoints",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_QA_TestPoints_VolumeId",
                table: "QA_TestPoints",
                column: "VolumeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QA_TestPoints");

            migrationBuilder.DropTable(
                name: "QA_PressureTests");

            migrationBuilder.DropTable(
                name: "QA_TemperatureTests");

            migrationBuilder.DropTable(
                name: "QA_TestRuns");

            migrationBuilder.DropTable(
                name: "QA_VolumeTests");
        }
    }
}
