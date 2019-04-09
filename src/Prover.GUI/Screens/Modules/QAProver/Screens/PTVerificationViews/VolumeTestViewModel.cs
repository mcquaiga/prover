using Caliburn.Micro;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.VolumeVerification;
using Prover.GUI.Screens.Dialogs;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        /// <summary>
        /// Defines the StandardCardViewContext
        /// </summary>
        private const string StandardCardViewContext = "CardNew";

        /// <summary>
        /// Defines the PulseInputCardViewContext
        /// </summary>
        private const string PulseInputCardViewContext = "PulseInputCard";

        #region Public Constructors

        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest volumeTest, IQaRunTestManager qaRunTestManager = null, ISubject<VerificationTest> changeObservable = null)
            : base(screenManager, eventAggregator, volumeTest, changeObservable)
        {
            ViewContext = StandardCardViewContext;
            Volume = volumeTest;
            TestManager = qaRunTestManager;

            AppliedInput = (long)Volume.AppliedInput;
            UncorrectedPulseCount = Volume.UncPulseCount;
            CorrectedPulseCount = Volume.CorPulseCount;

            var canRunTestCommand = this.WhenAny(x => x.TestManager, tm => tm != null);
            canRunTestCommand.ToProperty(this, model => model.DisplayButtons, out _displayButtons);

            CreateDriveSpecificViews();

            if (TestManager != null)
            {
                if (TestManager?.VolumeTestManager is ManualVolumeTestManager
                    || TestManager?.VolumeTestManager is FrequencyVolumeTestManager)
                {
                    var canRunPreTest = this.WhenAnyValue(x => x.ManualVolumeTestStep)
                        .Select(x => x == TestStep.PreTest);
                    PreVolumeTestCommand = DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator,
                        "Starting Volume Test...", RunPreVolumeTest, canRunPreTest);

                    var canRunPostTest = this.WhenAnyValue(x => x.ManualVolumeTestStep)
                        .Select(x => x == TestStep.PostTest || x == TestStep.PreTest);
                    PostVolumeTestCommand = DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator,
                        "Finishing Volume Test...", RunPostVolumeTest, canRunPostTest);

                    ManualVolumeTestStep = TestStep.PreTest;
                }

                if (TestManager?.VolumeTestManager is AutoVolumeTestManager)
                {
                    RunVolumeTestCommand = DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator,
                        "Running Volume Test...", RunTest, canRunTestCommand);
                }

                this.WhenAnyValue(x => x.AppliedInput, x => x.UncorrectedPulseCount, x => x.CorrectedPulseCount)
                    .Subscribe(_ =>
                    {
                        Volume.AppliedInput = _.Item1;
                        Volume.UncPulseCount = _.Item2;
                        Volume.CorPulseCount = _.Item3;
                        ChangedEvent.OnNext(TestRun.VerificationTest);
                    });
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public bool DisplayButtons => _displayButtons.Value;

        public ReactiveCommand PostVolumeTestCommand { get; set; }

        public ReactiveCommand PreVolumeTestCommand { get; set; }

        public ReactiveCommand RunVolumeTestCommand { get; set; }

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
        #endregion Public Properties

        #region Internal Enums

        internal enum TestStep
        {
            PreTest,
            PostTest
        }

        #endregion Internal Enums

        #region Methods

        public async Task RunTest(IObserver<string> status, CancellationToken ct)
        {
            try
            {
                TestManager.VolumeTestManager.StatusMessage.Subscribe(status);
                await TestManager.RunVolumeTest(ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {
                AppliedInput = (long)Volume.AppliedInput;
                UncorrectedPulseCount = Volume.UncPulseCount;
                CorrectedPulseCount = Volume.CorPulseCount;
            }
        }

        private void CreateDriveSpecificViews()
        {
            if (Volume?.DriveType is MechanicalDrive)
            {
                EnergyTestItem = new EnergyTestViewModel(EventAggregator, ((MechanicalDrive)Volume.DriveType).Energy);
            }
            else if (Volume?.DriveType is RotaryDrive)
            {
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive)Volume.DriveType);
            }

            if (Volume?.VerificationTest.FrequencyTest != null)
            {
                ViewContext = PulseInputCardViewContext;
                FrequencyTestItem = new FrequencyTestViewModel(ScreenManager, EventAggregator, Volume.VerificationTest.FrequencyTest, ChangedEvent, TestManager);
            }
        }

        private async Task RunPostVolumeTest(IObserver<string> status, CancellationToken ct)
        {
            try
            {
                TestManager.VolumeTestManager.StatusMessage
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(status);

                await TestManager.DownloadPostVolumeTest(ct);
                ManualVolumeTestStep = TestStep.PreTest;
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {
                ChangedEvent.OnNext(TestRun.VerificationTest);
            }
        }

        private async Task RunPreVolumeTest(IObserver<string> status, CancellationToken ct)
        {
            TestManager.VolumeTestManager.StatusMessage.Subscribe(status);
            await TestManager.DownloadPreVolumeTest(ct);
            ManualVolumeTestStep = TestStep.PostTest;
            ChangedEvent.OnNext(TestRun.VerificationTest);
        }
        #endregion

        #region Properties

        public long AppliedInput
        {
            get => _appliedInput;
            set => this.RaiseAndSetIfChanged(ref _appliedInput, value);
        }

        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();
        public Brush CorrectedPercentColour =>
                Volume?.CorrectedHasPassed == true
                    ? Brushes.White
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        public int CorrectedPulseCount
        {
            get => _correctedPulseCount;
            set => this.RaiseAndSetIfChanged(ref _correctedPulseCount, value);
        }

        public string DriveRateDescription => Instrument.DriveRateDescription();
        public decimal? EndCorrected => Volume.AfterTestItems.Corrected();
        public decimal? EndUncorrected => Volume.AfterTestItems.Uncorrected();
        public EnergyTestViewModel EnergyTestItem { get; set; }
        public decimal? EvcCorrected => Volume.EvcCorrected;
        public decimal? EvcUncorrected => Volume.EvcUncorrected;
        public FrequencyTestViewModel FrequencyTestItem { get; set; }
        public Instrument Instrument => Volume.Instrument;
        public bool IsAutoVolumeTest => TestManager?.VolumeTestManager is AutoVolumeTestManager;
        public bool IsManualVolumeTest => TestManager?.VolumeTestManager is ManualVolumeTestManager;
        public RotaryMeterTestViewModel MeterDisplacementItem { get; set; }
        public Brush MeterDisplacementPercentColour
        {
            get
            {
                RotaryDrive rotaryDrive = Volume?.DriveType as RotaryDrive;
                return rotaryDrive?.Meter.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;
            }
        }

        public decimal? StartCorrected => Volume.Items?.Corrected();
        public decimal? StartUncorrected => Volume.Items?.Uncorrected();
        public IQaRunTestManager TestManager { get; set; }
        public decimal? TrueCorrected
        {
            get
            {
                if (Volume.TrueCorrected != null)
                {
                    return decimal.Round(Volume.TrueCorrected.Value, 4);
                }

                return null;
            }
        }

        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);
        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();
        public Brush UnCorrectedPercentColour
            =>
                Volume?.UnCorrectedHasPassed == true
                    ? Brushes.White
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        public int UncorrectedPulseCount
        {
            get => _uncorrectedPulseCount;
            set => this.RaiseAndSetIfChanged(ref _uncorrectedPulseCount, value);
        }

        public Core.Models.Instruments.VolumeTest Volume { get; }
        internal TestStep ManualVolumeTestStep
        {
            get => _manualVolumeTestStep;
            set => this.RaiseAndSetIfChanged(ref _manualVolumeTestStep, value);
        }

        private long _appliedInput;
        private int _correctedPulseCount;
        private TestStep _manualVolumeTestStep;
        private int _uncorrectedPulseCount;
        #endregion

        #region Protected Methods

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TrueCorrected);
            NotifyOfPropertyChange(() => TrueUncorrected);
            NotifyOfPropertyChange(() => StartUncorrected);
            NotifyOfPropertyChange(() => EndUncorrected);
            NotifyOfPropertyChange(() => StartCorrected);
            NotifyOfPropertyChange(() => EndCorrected);
            NotifyOfPropertyChange(() => EvcUncorrected);
            NotifyOfPropertyChange(() => EvcCorrected);
            NotifyOfPropertyChange(() => StartCorrected);
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
            NotifyOfPropertyChange(() => Volume);
        }

        #endregion Protected Methods

        #region Private Fields

        private readonly ObservableAsPropertyHelper<bool> _displayButtons;

        #endregion Private Fields
    }
}