using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Prover.Data.SQL.Common;
using Prover.Data.SQL.Storage;

namespace Prover.Data.SQL.Migrations
{
    [DbContext(typeof(SqlDataContext))]
    [Migration("20170304010909_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Prover.Data.SQL.Models.PressureTestDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("AtmosphericGauge");

                    b.Property<decimal>("Gauge");

                    b.Property<string>("ItemData");

                    b.HasKey("Id");

                    b.ToTable("QA_PressureTests");
                });

            modelBuilder.Entity("Prover.Data.SQL.Models.TemperatureTestDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Gauge");

                    b.Property<string>("ItemData");

                    b.HasKey("Id");

                    b.ToTable("QA_TemperatureTests");
                });

            modelBuilder.Entity("Prover.Data.SQL.Models.TestPointDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<Guid?>("PressureId");

                    b.Property<Guid?>("TemperatureId");

                    b.Property<Guid>("TestRunId");

                    b.Property<Guid?>("VolumeId");

                    b.HasKey("Id");

                    b.HasIndex("PressureId")
                        .IsUnique();

                    b.HasIndex("TemperatureId")
                        .IsUnique();

                    b.HasIndex("TestRunId");

                    b.HasIndex("VolumeId")
                        .IsUnique();

                    b.ToTable("QA_TestPoints");
                });

            modelBuilder.Entity("Prover.Data.SQL.Models.TestRunDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ArchivedDateTime");

                    b.Property<bool?>("CommPortsPassed");

                    b.Property<string>("CorrectorType");

                    b.Property<string>("EmployeeId");

                    b.Property<bool?>("EventLogPassed");

                    b.Property<DateTime?>("ExportedDateTime");

                    b.Property<string>("InstrumentType");

                    b.Property<string>("ItemValues");

                    b.Property<string>("JobId");

                    b.Property<DateTime>("TestDateTime");

                    b.HasKey("Id");

                    b.ToTable("QA_TestRuns");
                });

            modelBuilder.Entity("Prover.Data.SQL.Models.VolumeTestDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AppliedInput");

                    b.Property<string>("PostTestItemData");

                    b.Property<string>("PreTestItemData");

                    b.Property<int>("PulserACount");

                    b.Property<int>("PulserBCount");

                    b.Property<int>("PulserCCount");

                    b.HasKey("Id");

                    b.ToTable("QA_VolumeTests");
                });

            modelBuilder.Entity("Prover.Data.SQL.Models.TestPointDao", b =>
                {
                    b.HasOne("Prover.Data.SQL.Models.PressureTestDao", "Pressure")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Data.SQL.Models.TestPointDao", "PressureId");

                    b.HasOne("Prover.Data.SQL.Models.TemperatureTestDao", "Temperature")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Data.SQL.Models.TestPointDao", "TemperatureId");

                    b.HasOne("Prover.Data.SQL.Models.TestRunDao", "TestRun")
                        .WithMany("TestPoints")
                        .HasForeignKey("TestRunId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Prover.Data.SQL.Models.VolumeTestDao", "Volume")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Data.SQL.Models.TestPointDao", "VolumeId");
                });
        }
    }
}
