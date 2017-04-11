using AutoMapper;
using Prover.Domain.Instrument;
using Prover.Domain.Verification.TestPoints;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.SuperFactor;
using Prover.Domain.Verification.TestPoints.Temperature;
using Prover.Domain.Verification.TestPoints.Volume;
using Prover.Domain.Verification.TestRun;
using Prover.Shared.Domain;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain
{
    public static class MappingConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<InstrumentMappingProfile>();
            cfg.AddProfile<TestRunMappingProfile>();
            cfg.AddProfile<TestPointMappingProfile>();
            cfg.AddProfile<PressureTestMappingProfile>();
            cfg.AddProfile<TemperatureTestMappingProfile>();
            cfg.AddProfile<SuperFactorTestMappingProfile>();
            cfg.AddProfile<VolumeTestMappingProfile>();
        }
    }

    //public static class Convertor
    //{
    //    private static bool _isConfigured;

    //    public static TDest ToDomain<TDest>(object dto)
    //    {
    //        ConfigureMapper();

    //        return Mapper.Map<TDest>(dto);
    //    }

    //    public static TDest ToDto<TDest>(object domain)
    //    {
    //        ConfigureMapper();

    //        return Mapper.Map<TDest>(domain);
    //    }

    //    private static void ConfigureMapper()
    //    {
    //        if (!_isConfigured)
    //        {
    //            Mapper.Initialize(MappingConfiguration.Configure);
    //            _isConfigured = true;
    //        }
    //    }
    //}
}