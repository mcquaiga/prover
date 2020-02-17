using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain;
using ItemValue = Devices.Core.Items.ItemValue;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Entities
{

    internal static class MappingHelpers
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            IgnoreNullValues = true,
            Converters = { new JsonStringEnumConverter(), }
        };

        public static void Configure<T, TItem>(EntityTypeBuilder<T> builder) 
            where T : VerificationTestEntity
            where TItem : ItemGroup
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            };

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            //builder.Property(c => c.Values)
            //    .HasConversion(
            //        x => JsonSerializer.Serialize(x, options),
            //        x => JsonSerializer.Deserialize<TemperatureItems>(x, options))
            //    .HasColumnType("nvarchar(MAX)")
            //    .HasColumnName("Values");

            builder.Property(c => c.PercentError)
                .HasColumnName("PercentError");

            builder.Property(c => c.ExpectedValue)
                .HasColumnType("decimal")
                .HasColumnName("ExpectedValue");

            builder.Property(c => c.ActualValue)
                .HasColumnType("decimal")
                .HasColumnName("ActualValue");
        }
    }

    public class VerificationMapping : IEntityTypeConfiguration<VerificationEntity>
    {
        private readonly DeviceRepository _deviceRepository;

        public VerificationMapping()
        {
            _deviceRepository = Devices.RepositoryFactory.CreateDefault();
        }

        public VerificationMapping(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public void Configure(EntityTypeBuilder<VerificationEntity> builder)
        {

            builder.ToTable("Verifications.TestRunTests");

            builder.Property(c => c.Id)
                .HasColumnName("Id");
        }
    }

    public class VerificationTestPointMapping : IEntityTypeConfiguration<VerificationTestPoint>
    {
        private readonly DeviceRepository _deviceRepository;

        public VerificationTestPointMapping()
        {
            _deviceRepository = Devices.RepositoryFactory.Instance;
        }

        public VerificationTestPointMapping(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public void Configure(EntityTypeBuilder<VerificationTestPoint> builder)
        {
           

            builder.HasBaseType<VerificationEntity>();

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.AppliedInput)
                .HasColumnType("decimal");

            builder.HasMany(c => c.Tests)
                .WithOne();
        }
    }
    

    public class PressureCorrectionTestMapping : IEntityTypeConfiguration<PressureCorrectionTest>
    {
        public void Configure(EntityTypeBuilder<PressureCorrectionTest> builder)
        {
            //builder.ToTable("Verifications.VerificationTestPressure");

            builder.HasBaseType<VerificationEntity>();

            //MappingHelpers.Configure<PressureCorrectionTest, PressureItems>(builder);
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Items)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, MappingHelpers.JsonSerializerOptions),
                    x => JsonSerializer.Deserialize<PressureItems>(x, MappingHelpers.JsonSerializerOptions))
                .HasColumnType("nvarchar(MAX)")
                .HasColumnName("Values");

            builder.Property(c => c.PercentError)
                .HasColumnName("PercentError")
                .HasColumnType("decimal");

            builder.Property(c => c.ExpectedValue)
                .HasColumnType("decimal")
                .HasColumnName("ExpectedValue");

            builder.Property(c => c.ActualValue)
                .HasColumnType("decimal")
                .HasColumnName("ActualValue");

            builder.Property(c => c.Gauge)
                .HasColumnName("GaugePressure")
                .HasColumnType("decimal");

            builder.Property(c => c.AtmosphericGauge)
                .HasColumnName("AtmosphericGauge")
                .HasColumnType("decimal");
        }
    }

    public class TemperatureCorrectionTestMapping : IEntityTypeConfiguration<TemperatureCorrectionTest>
    {
        public void Configure(EntityTypeBuilder<TemperatureCorrectionTest> builder)
        {
        
            //builder.ToTable("Verifications.VerificationTestPressure");

            builder.HasBaseType<VerificationEntity>();

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Items)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, MappingHelpers.JsonSerializerOptions),
                    x => JsonSerializer.Deserialize<TemperatureItems>(x, MappingHelpers.JsonSerializerOptions))
                .HasColumnType("nvarchar(MAX)")
                .HasColumnName("Values");

            builder.Property(c => c.PercentError)
                .HasColumnName("PercentError")
                .HasColumnType("decimal");

            builder.Property(c => c.ExpectedValue)
                .HasColumnType("decimal")
                .HasColumnName("ExpectedValue");

            builder.Property(c => c.ActualValue)
                .HasColumnType("decimal")
                .HasColumnName("ActualValue");

            builder.Property(c => c.Gauge)
                .HasColumnName("GaugeTemperature")
                .HasColumnType("decimal");
        }
    }

    public class SuperFactorCorrectionTestMapping : IEntityTypeConfiguration<SuperCorrectionTest>
    {
        public void Configure(EntityTypeBuilder<SuperCorrectionTest> builder)
        {
            //builder.ToTable("Verifications.VerificationTestPressure");

            builder.HasBaseType<VerificationEntity>();
            builder.Property(c => c.Id)
                .HasColumnName("Id");
            //MappingHelpers.Configure<SuperCorrectionTest, SuperFactorItems>(builder);

            builder.Property(c => c.Items)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, MappingHelpers.JsonSerializerOptions),
                    x => JsonSerializer.Deserialize<SuperFactorItems>(x, MappingHelpers.JsonSerializerOptions))
                .HasColumnType("nvarchar(MAX)")
                .HasColumnName("Values");

            builder.Property(c => c.PercentError)
                .HasColumnName("PercentError")
                .HasColumnType("decimal");

            builder.Property(c => c.ExpectedValue)
                .HasColumnType("decimal")
                .HasColumnName("ExpectedValue");

            builder.Property(c => c.ActualValue)
                .HasColumnType("decimal")
                .HasColumnName("ActualValue");

            builder.Property(c => c.GaugeTemp)
                .HasColumnName("GaugeTemperature")
                .HasColumnType("decimal");

            builder.Property(c => c.GaugePressure)
                .HasColumnName("GaugePressure")
                .HasColumnType("decimal");
        }
    }

    //public class VerificationTestSql : VerificationEntity
    //{
    //    public virtual TestPointSql TestPoint { get; set; }
    //}
}

//public class VerificationTestFactorJson : BaseEntity
//{
//    public virtual VerificationTestPointJson VerificationTestPoint { get; set; }

//    public virtual string JsonData { get; set; }
//}

//public class VerificationTestPointJson : AggregateRoot
//{
//    public virtual EvcVerificationDto EvcVerification { get; set; }

//    public virtual ICollection<VerificationTestFactorJson> TestFactors { get; set; }

//    public virtual string JsonData { get; set; }
//}