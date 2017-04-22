using System.Collections.Generic;
using System.Linq;
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

        public string HasPassed => Instrument.HasPassed ? "PASS" : "FAIL";

        public string DateTimePretty => $"{Instrument.TestDateTime:M/dd/yyyy}";

        public List<VerificationTest> VerificationTests => 
            Instrument.VerificationTests.OrderBy(v => v.TestNumber).ToList(); 

        public bool ShowTemperature
            => Instrument.CompositionType == CorrectorType.PTZ || Instrument.CompositionType == CorrectorType.T;

        public bool ShowPressure
            => Instrument.CompositionType == CorrectorType.PTZ || Instrument.CompositionType == CorrectorType.P;

        public bool ShowSuperFactor
            => Instrument.CompositionType == CorrectorType.PTZ;

        public VolumeTest VolumeTest => 
            VerificationTests.FirstOrDefault(v => v.VolumeTest != null)?.VolumeTest;
    }
}