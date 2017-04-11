using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.TestPoint
{
    static class TestPointMapping
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TestPointDto, TestPointDao>();
            cfg.CreateMap<TestPointDao, TestPointDto>();
        }
    }
}
