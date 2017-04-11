//using System;
//using Prover.Core.DomainPortable.Instrument;
//using Prover.Domain.Verification.TestPoints;

//namespace Prover.Domain.Verification.TestRun
//{
//    public static class TestRunMappingConfiguration
//    {
//        public static void Configure(IMapperConfigurationExpression cfg)
//        {
//            cfg.CreateMap<IInstrument, string>()
//                .ConstructUsing(src => src.GetType().AssemblyQualifiedName);

//            cfg.CreateMap<string, IInstrument>()
//                .ConstructUsing(src =>
//                {
//                    var type = Type.GetType(src);
//                    if (type.IsAssignableFrom(typeof(IInstrument)))
//                        return Activator.CreateInstance(type) as IInstrument;

//                    return null;
//                });

//            cfg.CreateMap<TestRun, TestRunDto>();
//            cfg.CreateMap<TestRunDto, TestRun>();

//            cfg.CreateMap<TestPoint, TestPointDto>();
//            cfg.CreateMap<TestPointDto, TestPoint>();
//        }
//    }
//}