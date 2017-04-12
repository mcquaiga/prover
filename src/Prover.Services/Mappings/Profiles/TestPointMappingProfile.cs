namespace Prover.Services.Mappings.Profiles
{
    public class TestPointMappingProfile : Profile
    {
        public TestPointMappingProfile()
        {
            CreateMap<TestPoint, TestPointDto>();

            CreateMap<TestPointDto, TestPoint>()
                .AfterMap((dto, domain) =>
                {
                    var pressureItems = domain.Instrument.GetPressureItems(dto.Pressure.ItemData);
                    var temperatureItems = domain.Instrument.GetTemperatureItems(dto.Temperature.ItemData);

                    var volumePreTestItems = domain.Instrument.GetVolumeItems(dto.Volume.PreTestItemData);
                    var volumePostTestItems = domain.Instrument.GetVolumeItems(dto.Volume.PostTestItemData);
                    domain.Update(pressureItems, temperatureItems, volumePreTestItems, volumePostTestItems);
                });
        }
    }
}