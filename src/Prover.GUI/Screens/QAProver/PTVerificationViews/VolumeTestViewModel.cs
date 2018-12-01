namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    using Caliburn.Micro;
    using Prover.Core.DriveTypes;
    using Prover.Core.Extensions;
    using Prover.Core.Models.Instruments;
    using Prover.Core.VerificationTests;
    using Prover.Core.VerificationTests.VolumeVerification;
    using Prover.GUI.Common;
    using Prover.GUI.Common.Events;
    using ReactiveUI;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="VolumeTestViewModel" />
    /// </summary>
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        /// <summary>
        /// Defines the StandardCardViewContext
        /// </summary>
        private const string StandardCardViewContext = "Card";

        /// <summary>
        /// Defines the PulseInputCardViewContext
        /// </summary>
        private const string PulseInputCardViewContext = "PulseInputCard";

        /// <summary>
        /// Defines the _testRunManager
        /// </summary>
        private readonly ITestRunManager _testRunManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeTestViewModel"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="testRun">The testRun<see cref="Core.Models.Instruments.VolumeTest"/></param>
        /// <param name="testRunManager">The testRunManager<see cref="ITestRunManager"/></param>
        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest testRun, ITestRunManager testRunManager = null) : base(screenManager, eventAggregator, testRun)
        {
            _testRunManager = testRunManager;
            Volume = testRun;          

            ViewContext = StandardCardViewContext;

            if (Volume?.VerificationTest.FrequencyTest != null)
            {
                ViewContext = PulseInputCardViewContext;
                FrequencyTestItem = new FrequencyTestViewModel(ScreenManager, EventAggregator, Volume.VerificationTest.FrequencyTest, _testRunManager);
                PreVolumeTestCommand = ReactiveCommand.CreateFromTask(StartVolumeTest);
                PostVolumeTestCommand = ReactiveCommand.CreateFromTask(RunPostVolumeTest);
            }
            else
            {
                if (Volume?.DriveType.Energy != null)
                    EnergyTestItem = new EnergyTestViewModel(EventAggregator, Volume?.DriveType.Energy);

                if (Volume?.DriveType is RotaryDrive)
                    MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Volume.DriveType);
            }
        }

        /// <summary>
        /// Defines the _viewContext
        /// </summary>
        private string _viewContext;

        /// <summary>
        /// Gets or sets the ViewContext
        /// </summary>
        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }

        /// <summary>
        /// Gets or sets the PostVolumeTestCommand
        /// </summary>
        public ReactiveCommand PostVolumeTestCommand { get; set; }

        /// <summary>
        /// Gets or sets the PreVolumeTestCommand
        /// </summary>
        public ReactiveCommand PreVolumeTestCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether ShowPostTestButton
        /// </summary>
        public bool ShowPostTestButton => PostVolumeTestCommand != null && _testRunManager != null;

        /// <summary>
        /// Gets or sets the InstrumentManager
        /// </summary>
        public TestRunManager InstrumentManager { get; set; }

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        public Instrument Instrument => Volume.Instrument;

        /// <summary>
        /// Gets the Volume
        /// </summary>
        public Core.Models.Instruments.VolumeTest Volume { get; }

        /// <summary>
        /// Gets or sets the AppliedInput
        /// </summary>
        public decimal AppliedInput
        {
            get { return Volume.AppliedInput; }
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChangeEvents();
            }
        }

        /// <summary>
        /// Gets or sets the EnergyTestItem
        /// </summary>
        public EnergyTestViewModel EnergyTestItem { get; set; }

        /// <summary>
        /// Gets or sets the FrequencyTestItem
        /// </summary>
        public FrequencyTestViewModel FrequencyTestItem { get; set; }

        /// <summary>
        /// Gets or sets the MeterDisplacementItem
        /// </summary>
        public RotaryMeterTestViewModel MeterDisplacementItem { get; set; }

        /// <summary>
        /// Gets the DriveRateDescription
        /// </summary>
        public string DriveRateDescription => Instrument.DriveRateDescription();

        /// <summary>
        /// Gets the UnCorrectedMultiplierDescription
        /// </summary>
        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();

        /// <summary>
        /// Gets the CorrectedMultiplierDescription
        /// </summary>
        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();

        /// <summary>
        /// Gets the TrueUncorrected
        /// </summary>
        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);

        /// <summary>
        /// Gets the TrueCorrected
        /// </summary>
        public decimal? TrueCorrected
        {
            get
            {
                if (Volume.TrueCorrected != null) return decimal.Round(Volume.TrueCorrected.Value, 4);

                return null;
            }
        }

        /// <summary>
        /// Gets the StartUncorrected
        /// </summary>
        public decimal? StartUncorrected => Volume.Items?.Uncorrected();

        /// <summary>
        /// Gets the EndUncorrected
        /// </summary>
        public decimal? EndUncorrected => Volume.AfterTestItems.Uncorrected();

        /// <summary>
        /// Gets the StartCorrected
        /// </summary>
        public decimal? StartCorrected => Volume.Items?.Corrected();

        /// <summary>
        /// Gets the EndCorrected
        /// </summary>
        public decimal? EndCorrected => Volume.AfterTestItems.Corrected();

        /// <summary>
        /// Gets the EvcUncorrected
        /// </summary>
        public decimal? EvcUncorrected => Volume.EvcUncorrected;

        /// <summary>
        /// Gets the EvcCorrected
        /// </summary>
        public decimal? EvcCorrected => Volume.EvcCorrected;

        /// <summary>
        /// Gets the UncorrectedPulseCount
        /// </summary>
        public int UncorrectedPulseCount => Volume.UncPulseCount;

        /// <summary>
        /// Gets the CorrectedPulseCount
        /// </summary>
        public int CorrectedPulseCount => Volume.CorPulseCount;

        /// <summary>
        /// Gets the UnCorrectedPercentColour
        /// </summary>
        public Brush UnCorrectedPercentColour =>
            Volume?.UnCorrectedHasPassed == true
                ? Brushes.White
                : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        /// <summary>
        /// Gets the CorrectedPercentColour
        /// </summary>
        public Brush CorrectedPercentColour =>
            Volume?.CorrectedHasPassed == true
                ? Brushes.White
                : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        /// <summary>
        /// Gets the MeterDisplacementPercentColour
        /// </summary>
        public Brush MeterDisplacementPercentColour
        {
            get
            {
                var rotaryDrive = Volume?.DriveType as RotaryDrive;
                return rotaryDrive?.Meter.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;
            }
        }

        public IVolumeTestManager VolumeTestManager { get; }

        /// <summary>
        /// The StartVolumeTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task StartVolumeTest()
        {
            try
            {
                await _testRunManager.DownloadPreVolumeTest();
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

        /// <summary>
        /// The RunPostVolumeTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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

        /// <summary>
        /// The RaisePropertyChangeEvents
        /// </summary>
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
