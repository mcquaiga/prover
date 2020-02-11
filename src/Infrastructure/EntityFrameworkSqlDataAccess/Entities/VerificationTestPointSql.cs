using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Devices.Core;
using Devices.Core.Repository;
using Domain.EvcVerifications;
using Domain.EvcVerifications.CorrectionTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain;
using ItemValue = Devices.Core.Items.ItemValue;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Entities
{
    //public class VerificationTestPointMapping : IEntityTypeConfiguration<VerificationTestPointSql>
    //{
    //    private readonly DeviceRepository _deviceRepository;

    //    public VerificationTestPointMapping()
    //    {
    //        _deviceRepository = Devices.Devices.Repository().Result;
    //    }

    //    public VerificationTestPointMapping(DeviceRepository deviceRepository)
    //    {
    //        _deviceRepository = deviceRepository;
    //    }

    //    public void Configure(EntityTypeBuilder<VerificationTestPointSql> builder)
    //    {
    //        var options = new JsonSerializerOptions
    //        {
    //            IgnoreReadOnlyProperties = true,
    //            WriteIndented = true
    //        };

    //        builder.Property(c => c.Id)
    //            .HasColumnName("Id");

    //        builder.Property(c => c.StartData)
    //            .HasColumnName("StartData")
    //            .HasConversion(
    //                d => JsonSerializer.Serialize(d, options),
    //                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, options));

    //        builder.Property(c => c.EndData)
    //            .HasColumnName("EndData")
    //            .HasConversion(
    //                d => JsonSerializer.Serialize(d, options),
    //                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, options));

    //        builder.Property(c => c.AppliedInput)
    //            .HasMaxLength(50);
            
          
         
    //    }
    //}

    public class VerificationTestFactorJson : BaseEntity
    {
        public virtual VerificationTestPointJson VerificationTestPoint { get; set; }

        public virtual string JsonData { get; set; }
    }

    public class VerificationTestPointJson : AggregateRoot
    {
        public virtual EvcVerificationDto EvcVerification { get; set; }

        public virtual ICollection<VerificationTestFactorJson> TestFactors { get; set; }

        public virtual string JsonData { get; set; }
    }

    public class VerificationTestPointSql : VerificationTestPoint
    {
        private VerificationTestPointSql(){}


        public virtual EvcVerificationSql EvcVerification { get; set; }

        

        [NotMapped]
        public override IEnumerable<ItemValue> BeforeValues { get; set; }

        public virtual Dictionary<string, string> StartData { get; set; }

        //public virtual ICollection<BaseVerificationTestSql> TestsList { get; set; }

        public override decimal? AppliedInput { get; set; }

        [NotMapped]
        public override IEnumerable<ItemValue> AfterValues { get; set; }
        public virtual Dictionary<string, string> EndData { get; set; }
    }
}