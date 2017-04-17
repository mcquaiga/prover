using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prover.Web.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    ArchivedDateTime = table.Column<DateTime>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    InstrumentData = table.Column<string>(nullable: true),
                    InstrumentType = table.Column<string>(nullable: true),
                    ItemFileType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientItems_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchivedDateTime = table.Column<DateTime>(nullable: true),
                    ClientId = table.Column<Guid>(nullable: true),
                    CommPortsPassed = table.Column<bool>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    EventLogPassed = table.Column<bool>(nullable: true),
                    ExportedDateTime = table.Column<DateTime>(nullable: true),
                    InstrumentData = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true),
                    TestDateTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instruments_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InstrumentId = table.Column<Guid>(nullable: false),
                    TestNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationTests_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PressureTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AtmosphericGauge = table.Column<decimal>(nullable: true),
                    GasGauge = table.Column<decimal>(nullable: true),
                    InstrumentData = table.Column<string>(nullable: true),
                    VerificationTestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressureTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PressureTests_VerificationTests_VerificationTestId",
                        column: x => x.VerificationTestId,
                        principalTable: "VerificationTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Gauge = table.Column<double>(nullable: false),
                    InstrumentData = table.Column<string>(nullable: true),
                    VerificationTestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemperatureTests_VerificationTests_VerificationTestId",
                        column: x => x.VerificationTestId,
                        principalTable: "VerificationTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolumeTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppliedInput = table.Column<decimal>(nullable: false),
                    DriveTypeDiscriminator = table.Column<string>(nullable: true),
                    InstrumentData = table.Column<string>(nullable: true),
                    PulseACount = table.Column<int>(nullable: false),
                    PulseBCount = table.Column<int>(nullable: false),
                    TestInstrumentData = table.Column<string>(nullable: true),
                    VerificationTestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolumeTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolumeTests_VerificationTests_VerificationTestId",
                        column: x => x.VerificationTestId,
                        principalTable: "VerificationTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientItems_ClientId",
                table: "ClientItems",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_ClientId",
                table: "Instruments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PressureTests_VerificationTestId",
                table: "PressureTests",
                column: "VerificationTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureTests_VerificationTestId",
                table: "TemperatureTests",
                column: "VerificationTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTests_InstrumentId",
                table: "VerificationTests",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_VolumeTests_VerificationTestId",
                table: "VolumeTests",
                column: "VerificationTestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientItems");

            migrationBuilder.DropTable(
                name: "PressureTests");

            migrationBuilder.DropTable(
                name: "TemperatureTests");

            migrationBuilder.DropTable(
                name: "VolumeTests");

            migrationBuilder.DropTable(
                name: "VerificationTests");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
