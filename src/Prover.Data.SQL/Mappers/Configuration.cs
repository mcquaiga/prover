using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Data.SQL.Models;
using Prover.Shared.DTO.Instrument;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Data.SQL.Mappers
{
    public static class MappingConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<EvcCorrectorType, string>().ConvertUsing(src => src.ToString());
            cfg.CreateMap<string, EvcCorrectorType>().ConvertUsing(src => (EvcCorrectorType)Enum.Parse(typeof(EvcCorrectorType), src));
            cfg.CreateMap<InstrumentTypeDto, string>().ConstructUsing(src => src.Id.ToString());
            cfg.CreateMap<string, InstrumentTypeDto>()
                .ConstructUsing(src => new InstrumentTypeDto() {Id = int.Parse(src)});

            cfg.CreateMap<TestRunDto, TestRunDao>();
            cfg.CreateMap<TestRunDao, TestRunDto>();
        }
    }
}
