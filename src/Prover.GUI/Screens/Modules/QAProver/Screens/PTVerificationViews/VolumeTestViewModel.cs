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
        public long AppliedInput
        {
            get => _appliedInput;
            set => this.RaiseAndSetIfChanged(ref _appliedInput, value);
        }

        public Brush CorrectedPercentColour =>
                        Volume?.CorrectedHasPassed == true
                            ? Brushes.White
                            : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        public int CorrectedPulseCount
        {
            get => _correctedPulseCount;
            set => this.RaiseAndSetIfChanged(ref _correctedPulseCount, value);
        }

        public bool DisplayButtons => _displayButtons.Value;

        public decimal? EndCorrected => Volume.AfterTestItems.Corrected();

        public decimal? EndUncorrected => Volume.AfterTestItems.Uncorrected();

        public EnergyTestViewModel EnergyTestItem { get; set; }

        public FrequencyTestViewModel FrequencyTestItem { get; set; }

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

        public ReactiveCommand PostVolumeTestCommand { get; set; }

        public ReactiveCommand PreVolumeTestCommand { get; set; }

        public ReactiveCommand RunVolumeTestCommand { get; set; }

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

        /// <summary>
        /// Gets or sets the ViewContext
        /// </summary>
        public string ViewContext
        {
            get { return _viewContext; }
            set { this.RaiseAndSetIfChanged(ref _viewContext, value); }
        }

        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();

        public string DriveRateDescription => Instrument.DriveRateDescription();

        public decimal? EvcCorrected => Volume.EvcCorrected;

        public decimal? EvcUncorrected => Volume.EvcUncorrected;

        public Instrument Instrument => Volume.Instrument;

        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);

        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();

        public Core.Models.Instruments.VolumeTest Volume { get; }

        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
                    Core.Models.Instruments.VolumeTest volumeTest, IQaRunTestManager qaRunTestManager = null, ISubject<VerificationTest> changeObservable = null)
                    : base(screenManager, eventAggregator, volumeTest, changeObservable)
        {
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

                    RunVolumeTestCommand.ThrownExceptions
                        .Subscribe(ex => Log.Error(ex));
                }

                this.WhenAnyValue(x => x.AppliedInput)
                    .Subscribe(i =>
                    {
                        Volume.AppliedInput = i;
                        ChangedEvent.OnNext(TestRun.VerificationTest);
                    });

                this.WhenAnyValue(x => x.UncorrectedPulseCount)
                    .Subscribe(i =>
                    {
                        Volume.UncPulseCount = i;

                        ChangedEvent.OnNext(TestRun.VerificationTest);
                    });

                this.WhenAnyValue(x => x.CorrectedPulseCount)
                    .Subscribe(i =>
                    {
                        Volume.CorPulseCount = i;
                        ChangedEvent.OnNext(TestRun.VerificationTest);
                    });
            }
        }

        public async Task RunTest(IObserver<string> status, CancellationToken ct)
        {
            try
            {
                //TestManager.VolumeTestManager.StatusMessage.Subscribe(status);
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

        internal enum TestStep
        {
            PreTest,
            PostTest
        }

        internal TestStep ManualVolumeTestStep
        {
            get => _manualVolumeTestStep;
            set => this.RaiseAndSetIfChanged(ref _manualVolumeTestStep, value);
        }

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

        private const string PulseInputCardViewContext = "PulseInputCard";

        private const string StandardCardViewContext = "CardNew";

        private readonly ObservableAsPropertyHelper<bool> _displayButtons;

        private long _appliedInput;

        private int _correctedPulseCount;

        private TestStep _manualVolumeTestStep;

        private int _uncorrectedPulseCount;

        /// <summary>
        /// Defines the _viewContext
        /// </summary>
        private string _viewContext;

        private void CreateDriveSpecificViews()
        {
            ViewContext = StandardCardViewContext;

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
            await TestManager.DownloadPreVolumeTest(ct);
            ManualVolumeTestStep = TestStep.PostTest;
            ChangedEvent.OnNext(TestRun.VerificationTest);
        }
    }
}