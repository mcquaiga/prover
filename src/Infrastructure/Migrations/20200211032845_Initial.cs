using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvcVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TestDateTime = table.Column<DateTime>(nullable: false),
                    ArchivedDateTime = table.Column<DateTime>(nullable: false),
                    DeviceTypeId = table.Column<Guid>(nullable: false),
                    DeviceData = table.Column<string>(nullable: true),
                    InputDriveType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvcVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTestPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EvcVerificationId = table.Column<Guid>(nullable: true),
                    JsonData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTestPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationTestPoints_EvcVerifications_EvcVerificationId",
                        column: x => x.EvcVerificationId,
                        principalTable: "EvcVerifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VerificationTestPointId = table.Column<Guid>(nullable: true),
                    JsonData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationTests_VerificationTestPoints_VerificationTestPointId",
                        column: x => x.VerificationTestPointId,
                        principalTable: "VerificationTestPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTestPoints_EvcVerificationId",
                table: "VerificationTestPoints",
                column: "EvcVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTests_VerificationTestPointId",
                table: "VerificationTests",
                column: "VerificationTestPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationTests");

            migrationBuilder.DropTable(
                name: "VerificationTestPoints");

            migrationBuilder.DropTable(
                name: "EvcVerifications");
        }
    }
}
