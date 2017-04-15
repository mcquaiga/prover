using Prover.Shared.Enums;

namespace Prover.CommProtocol.Common.Models.Instrument.Items
{
    public interface IVolumeItems : IItemsGroup
    {
        double CorrectedMultiplier { get;  }
        double CorrectedReading { get;  }
        string CorrectedUnits { get;  }
        double DriveRate { get;  }
        string DriveRateDescription { get;  }
        DriveTypeDescripter DriveType { get; }

        double Energy { get; }
        double EnergyGasValue { get; }
        string EnergyUnits { get;  }

        double MeterDisplacement { get;  }
        string MeterModel { get;  }
        int MeterModelId { get;  }

        double UncorrectedMultiplier { get; }
        double UncorrectedReading { get;  }
        string UncorrectedUnits { get;  }
    }

    public interface IRotaryMeterItems : IItemsGroup
    {
        double MeterDisplacement { get;  }
        string MeterModel { get;  }
        int MeterModelId { get; }
    }

    public interface IEnergyItems : IItemsGroup
    {
        double EnergyGasValue { get;  }
        double EnergyReading { get; }
        string EnergyUnits { get;  }
    }
}