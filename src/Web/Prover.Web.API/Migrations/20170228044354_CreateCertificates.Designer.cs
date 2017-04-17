using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Prover.Web.API.Storage;

namespace Prover.Web.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20170228044354_CreateCertificates")]
    partial class CreateCertificates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Prover.Core.Models.Clients.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime?>("ArchivedDateTime");

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("Prover.Core.Models.Clients.ClientItems", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClientId");

                    b.Property<string>("InstrumentData");

                    b.Property<string>("InstrumentTypeString")
                        .HasColumnName("InstrumentType");

                    b.Property<string>("ItemFileTypeString")
                        .HasColumnName("ItemFileType");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientItems");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.Instrument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ArchivedDateTime");

                    b.Property<Guid?>("ClientId");

                    b.Property<bool?>("CommPortsPassed");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("EmployeeId");

                    b.Property<bool?>("EventLogPassed");

                    b.Property<DateTime?>("ExportedDateTime");

                    b.Property<string>("InstrumentData");

                    b.Property<string>("JobId");

                    b.Property<DateTime>("TestDateTime");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Instruments");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Instrument");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.PressureTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("AtmosphericGauge");

                    b.Property<decimal?>("GasGauge");

                    b.Property<string>("InstrumentData");

                    b.Property<Guid>("VerificationTestId");

                    b.HasKey("Id");

                    b.HasIndex("VerificationTestId")
                        .IsUnique();

                    b.ToTable("PressureTests");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.TemperatureTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Gauge");

                    b.Property<string>("InstrumentData");

                    b.Property<Guid>("VerificationTestId");

                    b.HasKey("Id");

                    b.HasIndex("VerificationTestId")
                        .IsUnique();

                    b.ToTable("TemperatureTests");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.VerificationTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("InstrumentId");

                    b.Property<int>("TestNumber");

                    b.HasKey("Id");

                    b.HasIndex("InstrumentId");

                    b.ToTable("VerificationTests");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.VolumeTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AppliedInput");

                    b.Property<string>("DriveTypeDiscriminator");

                    b.Property<string>("InstrumentData");

                    b.Property<int>("PulseACount");

                    b.Property<int>("PulseBCount");

                    b.Property<string>("TestInstrumentData");

                    b.Property<Guid>("VerificationTestId");

                    b.HasKey("Id");

                    b.HasIndex("VerificationTestId")
                        .IsUnique();

                    b.ToTable("VolumeTests");
                });

            modelBuilder.Entity("Prover.Modules.Certificates.Models.Certificate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<long>("Number");

                    b.Property<string>("TestedBy");

                    b.Property<string>("VerificationType");

                    b.HasKey("Id");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("Prover.Modules.Certificates.Models.CertificateInstrument", b =>
                {
                    b.HasBaseType("Prover.Core.Models.Instruments.Instrument");

                    b.Property<Guid?>("CertificateId");

                    b.HasIndex("CertificateId");

                    b.ToTable("CertificateInstrument");

                    b.HasDiscriminator().HasValue("CertificateInstrument");
                });

            modelBuilder.Entity("Prover.Core.Models.Clients.ClientItems", b =>
                {
                    b.HasOne("Prover.Core.Models.Clients.Client", "Client")
                        .WithMany("Items")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.Instrument", b =>
                {
                    b.HasOne("Prover.Core.Models.Clients.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.PressureTest", b =>
                {
                    b.HasOne("Prover.Core.Models.Instruments.VerificationTest", "VerificationTest")
                        .WithOne("PressureTest")
                        .HasForeignKey("Prover.Core.Models.Instruments.PressureTest", "VerificationTestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.TemperatureTest", b =>
                {
                    b.HasOne("Prover.Core.Models.Instruments.VerificationTest", "VerificationTest")
                        .WithOne("TemperatureTest")
                        .HasForeignKey("Prover.Core.Models.Instruments.TemperatureTest", "VerificationTestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.VerificationTest", b =>
                {
                    b.HasOne("Prover.Core.Models.Instruments.Instrument", "Instrument")
                        .WithMany("VerificationTests")
                        .HasForeignKey("InstrumentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Prover.Core.Models.Instruments.VolumeTest", b =>
                {
                    b.HasOne("Prover.Core.Models.Instruments.VerificationTest", "VerificationTest")
                        .WithOne("VolumeTest")
                        .HasForeignKey("Prover.Core.Models.Instruments.VolumeTest", "VerificationTestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Prover.Modules.Certificates.Models.CertificateInstrument", b =>
                {
                    b.HasOne("Prover.Modules.Certificates.Models.Certificate", "Certificate")
                        .WithMany("Instruments")
                        .HasForeignKey("CertificateId");
                });
        }
    }
}
