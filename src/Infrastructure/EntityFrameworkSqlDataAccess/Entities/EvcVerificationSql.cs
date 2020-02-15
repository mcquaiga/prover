using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Entities
{
    public class EvcVerificationMapping : IEntityTypeConfiguration<EvcVerificationSql>
    {
        private readonly DeviceRepository _deviceRepository;

        public EvcVerificationMapping()
        {
            _deviceRepository = Devices.Repository.Get();
        }

        public EvcVerificationMapping(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        #region Public Methods

        public void Configure(EntityTypeBuilder<EvcVerificationSql> builder)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                Converters = {new JsonDeviceInstanceConverter(_deviceRepository)}
            };

            builder.ToTable("Verifications.TestRun");

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.DeviceType)
                .HasColumnName("DeviceTypeId")
                .HasConversion(
                    dt => dt.Id,
                    t => _deviceRepository.GetById(t)
                ).HasColumnType("uniqueidentifier");

            builder.Property(c => c.Device)
                .HasColumnName("Device")
                .HasConversion(
                    d => JsonSerializer.Serialize(d, options),
                    v => JsonSerializer.Deserialize<DeviceInstance>(v, options))
                .HasColumnType("nvarchar(MAX)");

            builder.Property(c => c.InputDriveType)
                .HasConversion(
                    v => v.ToString(),
                    v => (VolumeInputType) Enum.Parse(typeof(VolumeInputType), v));

            builder.HasMany(evc => evc.Tests)
                .WithOne();

            //builder.Property(c => c.ChildTests)
            //    .HasColumnName("ChildTests");
        }

        #endregion
    }

    public class EvcVerificationSql : EvcVerificationTest
    {
        private IVolumeInputType _driveType;

        public EvcVerificationSql(Guid id, DeviceInstance device) : this(device)
        {
            Id = id;
        }

        public EvcVerificationSql(DeviceInstance device) : base(device)
        {
        }

        #region Public Properties

        //public virtual ICollection<VerificationEntity> VerificationTests { get; set; }


        [NotMapped]
        public override IVolumeInputType DriveType
        {
            get => _driveType;
            set
            {
                _driveType = value;
                InputDriveType = value.InputType;
            }
        }

        public virtual VolumeInputType InputDriveType { get; set; }

        #endregion

        #region Public Methods

        public override void OnInitializing()
        {
            //var values = DeviceType.ToItemValuesEnumerable(DeviceValues);
            //Device = DeviceType.Factory.CreateInstance(values);
            DriveType = VolumeInputTypes.Create(Device);
        }

        #endregion
    }
}


//public class EvcVerificationDtoMapping : IEntityTypeConfiguration<EvcVerificationDto>
//{
//    public void Configure(EntityTypeBuilder<EvcVerificationDto> builder)
//    {
//        var options = new JsonSerializerOptions
//        {
//            IgnoreReadOnlyProperties = true,
//            WriteIndented = true
//        };

//        builder.HasMany(t => t.TestPoints).WithOne(c => c.EvcVerification);
//    }
//}

//public class EvcVerificationDto : AggregateRoot
//{
//    public DateTime TestDateTime { get; set; }
//    public DateTime ArchivedDateTime { get; set; }

//    public virtual ICollection<VerificationTestPointJson> TestPoints { get; set; }

//    public virtual Guid DeviceTypeId { get; set; }

//    public string DeviceData { get; set; }

//    public string InputDriveType { get; set; }
//}