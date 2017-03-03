using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Prover.Core.DAL;

namespace Prover.Core.DAL.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.PressureDal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("qa_pressure");
                });

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.TestPointDal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("PressureId");

                    b.Property<Guid>("QaTestRunId");

                    b.Property<Guid?>("TemperatureId");

                    b.Property<Guid?>("VolumeId");

                    b.HasKey("Id");

                    b.HasIndex("PressureId")
                        .IsUnique();

                    b.HasIndex("QaTestRunId");

                    b.HasIndex("TemperatureId")
                        .IsUnique();

                    b.HasIndex("VolumeId")
                        .IsUnique();

                    b.ToTable("qa_testpoints");
                });

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.QaTestRunDal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ArchivedDateTime");

                    b.Property<bool?>("CommPortsPassed");

                    b.Property<string>("EmployeeId");

                    b.Property<bool?>("EventLogPassed");

                    b.Property<DateTime?>("ExportedDateTime");

                    b.Property<string>("ItemValues");

                    b.Property<string>("JobId");

                    b.Property<DateTime>("TestDateTime");

                    b.HasKey("Id");

                    b.ToTable("qa_testruns");
                });

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.TemperatureTestPointDataAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("qa_temperature");
                });

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.VolumeTestPointDataAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("qa_volume");
                });

            modelBuilder.Entity("Prover.Core.DAL.DataAccess.QaTestRuns.TestPointDal", b =>
                {
                    b.HasOne("Prover.Core.DAL.DataAccess.QaTestRuns.PressureDal", "Pressure")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Core.DAL.DataAccess.QaTestRuns.TestPointDal", "PressureId");

                    b.HasOne("Prover.Core.DAL.DataAccess.QaTestRuns.QaTestRunDal", "QaTestRun")
                        .WithMany("TestPoints")
                        .HasForeignKey("QaTestRunId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Prover.Core.DAL.DataAccess.QaTestRuns.TemperatureTestPointDataAccess", "Temperature")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Core.DAL.DataAccess.QaTestRuns.TestPointDal", "TemperatureId");

                    b.HasOne("Prover.Core.DAL.DataAccess.QaTestRuns.VolumeTestPointDataAccess", "Volume")
                        .WithOne("TestPoint")
                        .HasForeignKey("Prover.Core.DAL.DataAccess.QaTestRuns.TestPointDal", "VolumeId");
                });
        }
    }
}
