using Autofac;
using AutoMapper;
using Prover.Data.SQL.Models;
using Prover.Data.SQL.Storage;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Data.SQL
{
    public class Startup
    {
        public Startup(ContainerBuilder container)
        {
            container.RegisterType<SqlRepository<TestRunDto, TestRunDao>>().As<IRepository<TestRunDto>>();

            Mapper.Initialize(cfg => cfg.CreateMap<TestRunDao, TestRunDto>());
            Mapper.Initialize(cfg => cfg.CreateMap<TestPointDao, TestPointDto>());
            Mapper.Initialize(cfg => cfg.CreateMap<VolumeTestDao, VolumeTestDto>());
            Mapper.Initialize(cfg => cfg.CreateMap<TemperatureTestDao, TemperatureTestDto>());
            Mapper.Initialize(cfg => cfg.CreateMap<PressureTestDao, PressureTestDto>());
        }
    }
}
