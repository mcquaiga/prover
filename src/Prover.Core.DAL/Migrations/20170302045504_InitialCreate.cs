using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prover.Core.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "qa_pressure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_pressure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qa_testruns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchivedDateTime = table.Column<DateTime>(nullable: true),
                    CommPortsPassed = table.Column<bool>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    EventLogPassed = table.Column<bool>(nullable: true),
                    ExportedDateTime = table.Column<DateTime>(nullable: true),
                    ItemValues = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true),
                    TestDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_testruns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qa_temperature",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_temperature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qa_volume",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_volume", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qa_testpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PressureId = table.Column<Guid>(nullable: true),
                    QaTestRunId = table.Column<Guid>(nullable: false),
                    TemperatureId = table.Column<Guid>(nullable: true),
                    VolumeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_testpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_testpoints_qa_pressure_PressureId",
                        column: x => x.PressureId,
                        principalTable: "qa_pressure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_testpoints_qa_testruns_QaTestRunId",
                        column: x => x.QaTestRunId,
                        principalTable: "qa_testruns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qa_testpoints_qa_temperature_TemperatureId",
                        column: x => x.TemperatureId,
                        principalTable: "qa_temperature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_testpoints_qa_volume_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "qa_volume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_qa_testpoints_PressureId",
                table: "qa_testpoints",
                column: "PressureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_qa_testpoints_QaTestRunId",
                table: "qa_testpoints",
                column: "QaTestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_testpoints_TemperatureId",
                table: "qa_testpoints",
                column: "TemperatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_qa_testpoints_VolumeId",
                table: "qa_testpoints",
                column: "VolumeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qa_testpoints");

            migrationBuilder.DropTable(
                name: "qa_pressure");

            migrationBuilder.DropTable(
                name: "qa_testruns");

            migrationBuilder.DropTable(
                name: "qa_temperature");

            migrationBuilder.DropTable(
                name: "qa_volume");
        }
    }
}
