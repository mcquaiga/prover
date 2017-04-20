using Prover.Core.Models.Instruments;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Modules.Certificates.Common
{
    public class VerificationViewModel : ViewModelBase
    {
        public Instrument Instrument { get; set; }

        public int? RowNumber { get; set; }

        public VerificationViewModel(Instrument instrument)
        {
            Instrument = instrument;
        }

        public VerificationViewModel(Instrument instrument, int rowNumber)
            : this(instrument)
        {
            RowNumber = rowNumber;
        }

        public string HasPassed
        {
            get { return Instrument.HasPassed ? "PASS" : "FAIL"; }
        }

        public bool IsSelected { get; set; }

        public string DateTimePretty => $"{Instrument.TestDateTime:M/dd/yyyy h:mm tt}";

        public bool ShowTemperature
            => Instrument.CompositionType == CorrectorType.PTZ || Instrument.CompositionType == CorrectorType.T;

        public bool ShowPressure
            => Instrument.CompositionType == CorrectorType.PTZ || Instrument.CompositionType == CorrectorType.P;
    }
}