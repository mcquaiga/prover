using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.Mappings
{
    public static class TestRunMappingConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TestRun, TestRunDto>();
            cfg.CreateMap<TestRunDto, TestRun>();
        }
    }
}
