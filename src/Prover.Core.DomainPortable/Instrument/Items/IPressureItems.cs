using System.Collections.Generic;

namespace Prover.Core.DomainPortable.Instrument.Items
{
    public interface IPressureItems
    {
        decimal AtmPressure { get; set; }
        decimal Base { get; set; }
        decimal Factor { get; set; }
        decimal GasPressure { get; set; }

        Dictionary<string, string> ItemData { get; }
        int Range { get; set; }
        string TransducerType { get; set; }
        decimal UnsqrFactor { get; set; }
    }
}