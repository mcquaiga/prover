using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Application.ViewModels;
using AutoMapper;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.CorrectionFactors;

namespace Application.Services
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
                //.Include<CorrectionTestViewModel<TemperatureItems>, VerificationTestEntity<TemperatureItems>>();

                cfg.CreateMap<TemperatureFactorViewModel, TemperatureCorrectionTest>();
                cfg.CreateMap<TemperatureCorrectionTest, TemperatureFactorViewModel>();

                cfg.CreateMap<PressureFactorViewModel, PressureCorrectionTest>();
                cfg.CreateMap<PressureCorrectionTest, PressureFactorViewModel>();
                
                cfg.CreateMap<SuperFactorViewModel, SuperCorrectionTest>()
                    .ForMember(dest => dest.GaugeTemp, opt => opt.MapFrom(src => src.Temperature.Gauge))
                    .ForMember(dest => dest.GaugePressure, opt => opt.MapFrom(src => src.Pressure.Gauge));

                cfg.CreateMap<SuperCorrectionTest, SuperFactorViewModel>();
                    //.ForMember(dest => dest.Pressure, opt =>
                    //.ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Pressure.Gauge));

                cfg.CreateMap<VerificationTestPointViewModel, VerificationTestPoint>();

                cfg.CreateMap<VerificationTestPoint, VerificationTestPointViewModel>()
                    .AfterMap((point, model, con) =>
                    {
                        // con.Mapper.Map<ICollection<VerificationEntity>, >>()

                        point.Tests.ToList().ForEach(v =>
                        {
                            //var m = con;
                            //con.Mapper.Map(v.GetType(), )
                            //model.TestsCollection.Add();

                        });


                        model.TestsCollection.Add(
                            new SuperFactorViewModel(point.GetTest<SuperCorrectionTest>().Items, model.Temperature,
                                model.Pressure)
                        );
                    });


                cfg.CreateMap<EvcVerificationViewModel, EvcVerificationTest>();
                cfg.CreateMap<EvcVerificationTest,EvcVerificationViewModel>();
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