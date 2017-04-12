using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Data.EF.Models.PressureTest;
using Prover.Data.EF.Models.TestPoint;
using Prover.Data.EF.Models.TestRun;

namespace Prover.Data.EF.Mappers
{
    public static class EfMapperConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<TestRunMappingProfile>();
            cfg.AddProfile<TestPointMappingProfile>();
            cfg.AddProfile<PressureTestMappingProfile>();
            cfg.AddProfile<TemperatureTestMappingProfile>();
            cfg.AddProfile<VolumeTestMappingProfile>();
        }
    }
}
