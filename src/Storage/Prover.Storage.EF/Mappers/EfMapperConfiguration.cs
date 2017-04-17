using AutoMapper;

namespace Prover.Storage.EF.Mappers
{
    public static class EfMapperConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            //cfg.AddProfile<TestRunMappingProfile>();
            //cfg.AddProfile<TestPointMappingProfile>();
            //cfg.AddProfile<PressureTestMappingProfile>();
            //cfg.AddProfile<TemperatureTestMappingProfile>();
            //cfg.AddProfile<VolumeTestMappingProfile>();
        }
    }
}