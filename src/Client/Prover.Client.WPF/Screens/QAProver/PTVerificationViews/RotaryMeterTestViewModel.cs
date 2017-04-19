namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public class RotaryMeterTestViewModel : ReactiveScreen
    {
        public RotaryMeterTestViewModel(RotaryDrive rotaryDrive)
        {
            RotaryDriveType = rotaryDrive;
        }

        //Meter properties
        public string DriveRateDescription => RotaryDriveType.Instrument.DriveRateDescription();
        public decimal? EvcMeterDisplacement => RotaryDriveType.Meter.EvcMeterDisplacement;
        public decimal? MeterDisplacement => RotaryDriveType.Meter.MeterDisplacement;
        public bool MeterDisplacementHasPassed => RotaryDriveType.HasPassed;
        public decimal? MeterDisplacementPercentError => RotaryDriveType.Meter.MeterDisplacementPercentError;
        public string MeterTypeDescription => RotaryDriveType.Meter.MeterTypeDescription;

        public RotaryDrive RotaryDriveType { get; }
    }
}