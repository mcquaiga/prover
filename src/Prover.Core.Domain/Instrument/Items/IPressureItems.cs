using System.Collections.Generic;
using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface IPressureItems : IItemsGroup
    {
        PressureUnits Units { get; set; }
        double AtmPressure { get; set; }
        double Base { get; set; }
        double Factor { get; set; }
        double GasPressure { get; set; }

        int Range { get; set; }
        string TransducerType { get; set; }
        double UnsqrFactor { get; set; }
    }
}