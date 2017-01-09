using System.Windows.Media;
using Caliburn.Micro;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest testRun) : base(screenManager, eventAggregator, testRun)
        {
            Volume = testRun;

            if (Volume?.DriveType is MechanicalDrive)
                EnergyTestItem =
                    new EnergyTestViewModel(EventAggregator, (MechanicalDrive)Volume.DriveType);

            if (Volume?.DriveType is RotaryDrive)
                MeterDisplacementItem =
                    new RotaryMeterTestViewModel(
                        (RotaryDrive)Volume.DriveType);
        }

        public QaRunTestManager InstrumentManager { get; set; }
        public Instrument Instrument => Volume.Instrument;

        public Core.Models.Instruments.VolumeTest Volume { get; }

        public decimal AppliedInput
        {
            get { return Volume.AppliedInput; }
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChanges();
            }
        }

        public EnergyTestViewModel EnergyTestItem { get; set; }
        public RotaryMeterTestViewModel MeterDisplacementItem { get; set; }

        public string DriveRateDescription => Instrument.DriveRateDescription();
        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();
        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();

        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);

        public decimal? TrueCorrected
        {
            get
            {
                if (Volume.TrueCorrected != null) return decimal.Round(Volume.TrueCorrected.Value, 4);

                return null;
            }
        }

        public decimal? StartUncorrected => Volume.Items?.Uncorrected();
        public decimal? EndUncorrected => Volume.AfterTestItems.Uncorrected();
        public decimal? StartCorrected => Volume.Items?.Corrected();
        public decimal? EndCorrected => Volume.AfterTestItems.Corrected();
        public decimal? EvcUncorrected => Volume.EvcUncorrected;
        public decimal? EvcCorrected => Volume.EvcCorrected;

        public int UncorrectedPulseCount => Volume.UncPulseCount;
        public int CorrectedPulseCount => Volume.CorPulseCount;

        public Brush UnCorrectedPercentColour
            =>
            Volume?.UnCorrectedHasPassed == true
                ? Brushes.White
                : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public Brush CorrectedPercentColour
            =>
            Volume?.CorrectedHasPassed == true
                ? Brushes.White
                : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public Brush MeterDisplacementPercentColour
        {
            get
            {
                var rotaryDrive = Volume?.DriveType as RotaryDrive;
                return rotaryDrive?.Meter.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;
            }
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

        public override void Handle(VerificationTestEvent message)
        {
            RaisePropertyChanges();
        }
    }
}