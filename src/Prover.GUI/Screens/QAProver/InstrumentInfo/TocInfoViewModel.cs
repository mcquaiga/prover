using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.Screens.QAProver.InstrumentInfo
{
    public class TocInfoViewModel
    {
        public TocInfoViewModel(Instrument instrument)
        {
            Instrument = instrument;
        }

        public Instrument Instrument { get; }

        public string MeterSerialNumber => Instrument.Items.GetItem(863)?.RawValue;
        public string TurbineSize => Instrument.Items.GetItem(864)?.RawValue;
        public decimal Kmo => decimal.Round(Instrument.Items.GetItem(868).NumericValue, 2);
        public decimal Km => decimal.Round(Instrument.Items.GetItem(865).NumericValue, 2);
        public decimal Ks => decimal.Round(Instrument.Items.GetItem(866).NumericValue, 2);
        public decimal ABar => decimal.Round(Instrument.Items.GetItem(867).NumericValue, 2);
    }
}
