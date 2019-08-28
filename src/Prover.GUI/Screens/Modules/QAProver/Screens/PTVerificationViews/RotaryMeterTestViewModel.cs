using Caliburn.Micro.ReactiveUI;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments.DriveTypes;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class RotaryMeterTestViewModel : ReactiveScreen
    {
        public bool MeterDisplacementHasPassed => RotaryDriveType.HasPassed;

        public RotaryDrive RotaryDriveType { get; }

        //Meter properties
        public string DriveRateDescription => RotaryDriveType.Instrument.DriveRateDescription();

        public string EvcMeterDisplacement => RotaryDriveType.Meter.EvcMeterDisplacement.Value.ToString("0.####");

        public string MeterDisplacement => RotaryDriveType.Meter.MeterDisplacement.ToString("0.####");

        public decimal? MeterDisplacementPercentError => RotaryDriveType.Meter.MeterDisplacementPercentError;

        public string MeterTypeDescription => RotaryDriveType.Meter.MeterTypeDescription;

        public RotaryMeterTestViewModel(RotaryDrive rotaryDrive)
        {
            RotaryDriveType = rotaryDrive;
        }
    }
}