using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items;
using Domain.EvcVerifications;
using Domain.EvcVerifications.DriveTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Entities
{
    public class EvcVerificationMapping : IEntityTypeConfiguration<EvcVerificationSql>
    {
        private readonly DeviceRepository _deviceRepository;

        public EvcVerificationMapping()
        {
            _deviceRepository = Devices.Devices.Repository().Result;
        }

        public EvcVerificationMapping(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public void Configure(EntityTypeBuilder<EvcVerificationSql> builder)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            };

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.DeviceType)
                .HasColumnName("DeviceTypeId")
                .HasConversion(
                    dt => dt.Id, 
                    t => _deviceRepository.GetById(t)
                );

            builder.Property(c => c.DeviceValues)
                .HasColumnName("DeviceData")
                .HasConversion(
                    d => JsonSerializer.Serialize(d, options),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, options));

            builder.Property(c => c.InputDriveType)
                .HasConversion(
                    v => v.ToString(),
                    v => (VolumeInputType)Enum.Parse(typeof(VolumeInputType), v));
        }
    }

    public class EvcVerificationSql : EvcVerificationTest
    {
        private VolumeInputType _volumeInputType;
        private IVolumeInputType _driveType;

        private EvcVerificationSql() : base()
        {

        }

        [NotMapped] public override DeviceType DeviceType { get; protected set; }

        public virtual Dictionary<string, string> DeviceValues { get; set; }

        [NotMapped] public override DeviceInstance Device { get; protected set; }

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

        public virtual ICollection<BaseVerificationTestSql> ChildTests { get; set; }

        public virtual VolumeInputType InputDriveType { get; set; }


        public override void OnInitializing()
        {

            var values = DeviceType.ToItemValuesEnumerable(DeviceValues);
            Device = DeviceType.Factory.CreateInstance(values);
        }
    }
}
