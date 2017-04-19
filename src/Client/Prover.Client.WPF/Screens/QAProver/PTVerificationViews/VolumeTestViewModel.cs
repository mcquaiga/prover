namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest testRun) : base(screenManager, eventAggregator, testRun)
        {
            Volume = testRun;

            if (Volume?.DriveType is MechanicalDrive)
                EnergyTestItem =
                    new EnergyTestViewModel(EventAggregator, (MechanicalDrive) Volume.DriveType);

            if (Volume?.DriveType is RotaryDrive)
                MeterDisplacementItem =
                    new RotaryMeterTestViewModel(
                        (RotaryDrive) Volume.DriveType);
        }

        public decimal AppliedInput
        {
            get { return Volume.AppliedInput; }
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChanges();
            }
        }

        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();

        public Brush CorrectedPercentColour
            =>
                Volume?.CorrectedHasPassed == true
                    ? Brushes.White
                    : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public int CorrectedPulseCount => Volume.CorPulseCount;

        public string DriveRateDescription => Instrument.DriveRateDescription();
        public decimal? EndCorrected => Volume.AfterTestItems.Corrected();
        public decimal? EndUncorrected => Volume.AfterTestItems.Uncorrected();

        public EnergyTestViewModel EnergyTestItem { get; set; }
        public decimal? EvcCorrected => Volume.EvcCorrected;
        public decimal? EvcUncorrected => Volume.EvcUncorrected;
        public Instrument Instrument => Volume.Instrument;

        public QaRunTestManager InstrumentManager { get; set; }
        public RotaryMeterTestViewModel MeterDisplacementItem { get; set; }

        public Brush MeterDisplacementPercentColour
        {
            get
            {
                var rotaryDrive = Volume?.DriveType as RotaryDrive;
                return rotaryDrive?.Meter.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;
            }
        }

        public decimal? StartCorrected => Volume.Items?.Corrected();

        public decimal? StartUncorrected => Volume.Items?.Uncorrected();

        public decimal? TrueCorrected
        {
            get
            {
                if (Volume.TrueCorrected != null) return decimal.Round(Volume.TrueCorrected.Value, 4);

                return null;
            }
        }

        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);
        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();

        public Brush UnCorrectedPercentColour
            =>
                Volume?.UnCorrectedHasPassed == true
                    ? Brushes.White
                    : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public int UncorrectedPulseCount => Volume.UncPulseCount;

        public Core.Models.Instruments.VolumeTest Volume { get; }

        public override void Handle(VerificationTestEvent message)
        {
            RaisePropertyChanges();
        }

        private void RaisePropertyChanges()
        {
            NotifyOfPropertyChange(() => AppliedInput);
            NotifyOfPropertyChange(() => TrueCorrected);
            NotifyOfPropertyChange(() => TrueUncorrected);
            NotifyOfPropertyChange(() => StartUncorrected);
            NotifyOfPropertyChange(() => EndUncorrected);
            NotifyOfPropertyChange(() => StartCorrected);
            NotifyOfPropertyChange(() => EndCorrected);
            NotifyOfPropertyChange(() => EvcUncorrected);
            NotifyOfPropertyChange(() => EvcCorrected);
            NotifyOfPropertyChange(() => StartCorrected);
            NotifyOfPropertyChange(() => UncorrectedPulseCount);
            NotifyOfPropertyChange(() => CorrectedPulseCount);
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
            NotifyOfPropertyChange(() => Volume);
        }
    }
}