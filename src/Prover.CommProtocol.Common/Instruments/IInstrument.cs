using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.Common.Instruments
{
    public interface IInstrument
    {
        int Id { get; }
        string Name { get; }
        
        IEnumerable<ItemMetadata> ItemDefinitions { get; }
        Dictionary<string, string> ItemData { get; }
        EvcCorrectorType CorrectorType();
        DriveTypeDescripter DriveType { get; }

        ISiteInformationItems SiteInformationItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IPressureItems PressureItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        IVolumeItems VolumeItems { get; }
        IEnergyItems EnergyItems { get; }
        IRotaryMeterItems RotaryItems { get; }

        Task<ITemperatureItems> DownloadTemperatureItems();
        Task<IPressureItems> DownloadPressureItems();
        Task<IVolumeItems> DownloadVolumeItems();
    }

    public interface IVolumeItems
    {
        decimal UncorrectedReading { get; }
        decimal UncorrectedMultiplier { get; }
        string UncorrectedUnits { get; }
        decimal CorrectedReading { get; }
        decimal CorrectedMultiplier { get; }
        string CorrectedUnits { get; }
        decimal DriveRate { get; }
        decimal UnCorrectedInputVolume(decimal appliedInput);

        void Update(IVolumeItems volumeItems);
    }

    public interface IEnergyItems
    {
        string EnergyUnits { get; }
        decimal Energy { get; }
        decimal EnergyGasValue { get; }
    }

    public interface IRotaryMeterItems
    {
        string MeterModel { get; }
        decimal MeterDisplacement { get; }
    }

    public interface IPulseOutputItems
    {
        string PulserAUnits { get; }
        decimal PulserAScaling { get; }

        string PulserBUnits { get; }      
        decimal PulseBScaling { get; }
    }

    public interface ISuperFactorItems
    {
        decimal SpecGr { get; }
        decimal Co2 { get; }
        decimal N2 { get; }
    }

    public interface IPressureItems
    {
        int Range { get; }
        string TransducerType { get; }
        decimal Base { get; }
        decimal GasPressure { get; }
        decimal AtmPressure { get; }
        decimal Factor { get; }

        void Update(IPressureItems pressureItems);
    }

    public interface ITemperatureItems
    {
        decimal Base { get; }
        TemperatureUnits Units { get; }
        decimal GasTemperature { get;  }
        decimal Factor { get; }

        void Update(ITemperatureItems temperatureItems);
    }

    public interface ISiteInformationItems
    {
        string SerialNumber { get; }
        string CompanyNumber { get; }
        string FirmwareVersion { get; }
    }
}