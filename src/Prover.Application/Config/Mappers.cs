using AutoMapper;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Domain.EvcVerifications;
using Prover.Domain.EvcVerifications.Verifications;
using Prover.Domain.EvcVerifications.Verifications.CorrectionFactors;
using Prover.Domain.EvcVerifications.Verifications.Volume;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary;

namespace Prover.Application.Config
{
    public static class Mappers
    {
        public static Mapper Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BaseViewModel, VerificationEntity>()
                    .Include<CorrectionTestViewModel<ItemGroup>, VerificationTestEntity<ItemGroup>>()
                    .Include<TemperatureFactorViewModel, TemperatureCorrectionTest>()
                    .Include<PressureFactorViewModel, PressureCorrectionTest>()
                    .Include<SuperFactorViewModel, SuperCorrectionTest>();
                cfg.CreateMap<CorrectionTestViewModel<ItemGroup>, VerificationTestEntity<ItemGroup>>();

                cfg.CreateMap<TemperatureFactorViewModel, TemperatureCorrectionTest>();
                cfg.CreateMap<TemperatureCorrectionTest, TemperatureFactorViewModel>();

                cfg.CreateMap<PressureFactorViewModel, PressureCorrectionTest>();
                cfg.CreateMap<PressureCorrectionTest, PressureFactorViewModel>();
                
                cfg.CreateMap<SuperFactorViewModel, SuperCorrectionTest>()
                    .ForMember(dest => dest.GaugeTemp, opt => opt.MapFrom(src => src.Temperature.Gauge))
                    .ForMember(dest => dest.GaugePressure, opt => opt.MapFrom(src => src.Pressure.Gauge));

                cfg.CreateMap<SuperCorrectionTest, SuperFactorViewModel>()
                    .ForMember(s => s.Pressure, opts => opts.Ignore())
                    .ForMember(s => s.Temperature, opts => opts.Ignore());

                cfg.CreateMap<CorrectedVolumeTestViewModel, CorrectedVolumeTestRun>();
                cfg.CreateMap<CorrectedVolumeTestRun, CorrectedVolumeTestViewModel>();

                cfg.CreateMap<UncorrectedVolumeTestRun, UncorrectedVolumeTestViewModel>();
                cfg.CreateMap<UncorrectedVolumeTestViewModel, UncorrectedVolumeTestRun>();

                cfg.CreateMap<RotaryMeterTestViewModel, RotaryMeterTest>();
                cfg.CreateMap<RotaryMeterTest, RotaryMeterTestViewModel>();

                cfg.CreateMap<VerificationTestPointViewModel, VerificationTestPoint>();
                cfg.CreateMap<VerificationTestPoint, VerificationTestPointViewModel>();

                cfg.CreateMap<EvcVerificationViewModel, EvcVerificationTest>();
                cfg.CreateMap<EvcVerificationTest, EvcVerificationViewModel>()
                    .AfterMap((model, vm, con) =>
                    {
                        vm.Initialize();
                    });
            });

            return new Mapper(config);
        }
    }

    //public static class MapExtensions
    //{
    //    public static BaseViewModel ConvertToViewModel<T>(this T ve) where T : VerificationEntity
    //    {
    //        return new TemperatureFactorViewModel(ve.);
    //    }
    //}

    //public static class ModelConverter<T> where T : VerificationEntity
    //{
    //    public static BaseViewModel GetViewModel(T verificationEntity)
    //    {
    //        var converter = Assembly.GetCallingAssembly()
    //            .GetTypes().Where(t => t.GetInterfaces().Any(c => c.))
    //    }
    //}
}