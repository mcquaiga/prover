using AutoMapper;
using Prover.Services.Mappings.Profiles;

namespace Prover.Services.Mappings
{
    public static class MappingConfiguration
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            //cfg.AddProfile<InstrumentMappingProfile>();
            //cfg.AddProfile<TestRunMappingProfile>();
            //cfg.AddProfile<TestPointMappingProfile>();
            //cfg.AddProfile<PressureTestMappingProfile>();
            //cfg.AddProfile<TemperatureTestMappingProfile>();
            //cfg.AddProfile<SuperFactorTestMappingProfile>();
            //cfg.AddProfile<VolumeTestMappingProfile>();
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