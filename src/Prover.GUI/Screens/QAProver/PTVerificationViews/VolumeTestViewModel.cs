using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        private readonly IQaRunTestManager _testRunManager;

        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest testRun, IQaRunTestManager testRunManager = null) : base(screenManager, eventAggregator, testRun)
        {
            _testRunManager = testRunManager;
            Volume = testRun;

            if (Volume?.DriveType is MechanicalDrive)
                EnergyTestItem =
                    new EnergyTestViewModel(EventAggregator, (MechanicalDrive)Volume.DriveType);

            if (Volume?.DriveType is RotaryDrive)
                MeterDisplacementItem =
                    new RotaryMeterTestViewModel(
                        (RotaryDrive)Volume.DriveType);

            if (Volume?.VerificationTest.FrequencyTest != null)
            {
                FrequencyTestItem = new FrequencyTestViewModel(ScreenManager, EventAggregator, Volume.VerificationTest.FrequencyTest);
                
                PostVolumeTestCommand = ReactiveCommand.CreateFromTask(RunPostVolumeTest);
            }
        }

        public ReactiveCommand PostVolumeTestCommand { get; set; }
        public bool ShowPostTestButton => PostVolumeTestCommand != null && _testRunManager != null;

        public QaRunTestManager InstrumentManager { get; set; }
        public Instrument Instrument => Volume.Instrument;

        public Core.Models.Instruments.VolumeTest Volume { get; }

        public decimal AppliedInput
        {
            get { return Volume.AppliedInput; }
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChangeEvents();
            }
        }

        public EnergyTestViewModel EnergyTestItem { get; set; }
        public FrequencyTestViewModel FrequencyTestItem { get; set; }
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

        private async Task RunPostVolumeTest()
        {        
            try
            {
                await _testRunManager.DownloadPostVolumeTest();
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(Volume.VerificationTest));
            }
        }

        protected override void RaisePropertyChangeEvents()
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