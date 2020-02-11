﻿// <auto-generated />
using System;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ProverDbContext))]
    [Migration("20200211032845_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Infrastructure.EntityFrameworkSqlDataAccess.Entities.EvcVerificationDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ArchivedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeviceData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("DeviceTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InputDriveType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TestDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("EvcVerifications");
                });

            modelBuilder.Entity("Infrastructure.EntityFrameworkSqlDataAccess.Entities.VerificationTestFactorJson", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("JsonData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("VerificationTestPointId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VerificationTestPointId");

                    b.ToTable("VerificationTests");
                });

            modelBuilder.Entity("Infrastructure.EntityFrameworkSqlDataAccess.Entities.VerificationTestPointJson", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("EvcVerificationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("JsonData")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EvcVerificationId");

                    b.ToTable("VerificationTestPoints");
                });

            modelBuilder.Entity("Infrastructure.EntityFrameworkSqlDataAccess.Entities.VerificationTestFactorJson", b =>
                {
                    b.HasOne("Infrastructure.EntityFrameworkSqlDataAccess.Entities.VerificationTestPointJson", "VerificationTestPoint")
                        .WithMany("TestFactors")
                        .HasForeignKey("VerificationTestPointId");
                });

            modelBuilder.Entity("Infrastructure.EntityFrameworkSqlDataAccess.Entities.VerificationTestPointJson", b =>
                {
                    b.HasOne("Infrastructure.EntityFrameworkSqlDataAccess.Entities.EvcVerificationDto", "EvcVerification")
                        .WithMany("TestPoints")
                        .HasForeignKey("EvcVerificationId");
                });
#pragma warning restore 612, 618
        }
    }
}
