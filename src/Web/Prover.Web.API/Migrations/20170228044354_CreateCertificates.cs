using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prover.Web.API.Migrations
{
    public partial class CreateCertificates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    Number = table.Column<long>(nullable: false),
                    TestedBy = table.Column<string>(nullable: true),
                    VerificationType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Instruments",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CertificateId",
                table: "Instruments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_CertificateId",
                table: "Instruments",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instruments_Certificates_CertificateId",
                table: "Instruments",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instruments_Certificates_CertificateId",
                table: "Instruments");

            migrationBuilder.DropIndex(
                name: "IX_Instruments_CertificateId",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Instruments");

            migrationBuilder.DropTable(
                name: "Certificates");
        }
    }
}
