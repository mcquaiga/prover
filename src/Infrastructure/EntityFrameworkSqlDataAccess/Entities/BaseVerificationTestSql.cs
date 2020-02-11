using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Devices.Core.Repository;
using Domain.EvcVerifications.CorrectionTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Entities
{

    public class BaseVerificationTestMapping : IEntityTypeConfiguration<BaseVerificationTestSql>
    {
        private readonly DeviceRepository _deviceRepository;

        public BaseVerificationTestMapping()
        {
            _deviceRepository = Devices.Devices.Repository().Result;
        }

      

        public void Configure(EntityTypeBuilder<BaseVerificationTestSql> builder)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            };

            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.ParentId).IsRequired();

        }
    }

    public class BaseVerificationTestSql : BaseVerificationTest
    {

        public virtual Guid ParentId { get; set; }
    }
}