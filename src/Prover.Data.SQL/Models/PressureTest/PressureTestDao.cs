﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Prover.Data.EF.Mappers.Resolvers;
using Prover.Data.EF.Models.TestPoint;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.PressureTest
{
    internal static class PressureTestMapping
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PressureTestDto, PressureTestDao>()
                .ForMember(x => x.ItemData, map => map.ResolveUsing(new SerializeResolver<PressureTestDto, Dictionary<string, string>, PressureTestDao>(x => x.ItemData)));

            cfg.CreateMap<PressureTestDao, PressureTestDto>()
                .ForMember(x => x.ItemData, map => map.ResolveUsing(new DeserializeResolver<PressureTestDao, string, PressureTestDto, Dictionary<string, string>>(x => x.ItemData)));

        }
    }

    [Table("QA_PressureTests")]
    internal class PressureTestDao
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TestPointId { get; set; }
        public virtual TestPointDao TestPoint { get; set; }

        public decimal Gauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }
        public string ItemData { get; set; }
    }
}