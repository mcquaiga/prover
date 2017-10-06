using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.VolumeVerification;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens.Dialogs;
using Prover.GUI.Modules.QAProver.Screens.PTVerificationViews.VolumeTest.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class VolumeTestViewModel : TestRunViewModelBase<Core.Models.Instruments.VolumeTest>
    {
        public VolumeTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.VolumeTest volumeTest, IQaRunTestManager qaRunTestManager = null)
            : base(screenManager, eventAggregator, volumeTest)
        {
            Volume = volumeTest;
            TestManager = qaRunTestManager;

            var canRunTestCommand = this.WhenAny(x => x.TestManager, tm => tm != null);
            canRunTestCommand.ToProperty(this, model => model.ShowRunButton, out _showRunButton);

            CreateDriveSpecificViews();

            if (TestManager != null)
            {
                PreVolumeTestCommand = (ReactiveCommand<Unit, Unit>) DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator,
                    "Initializing Volume Test...", RunPreVolumeTest);

                if (TestManager?.VolumeTestManager is ManualVolumeTestManager)
                {
                    RunVolumeTestCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var vm = new ManualVolumeTestDialogViewModel(TestManager.VolumeTestManager as ManualVolumeTestManager, eventAggregator, RunTest);
                        var result = await ScreenManager.ShowModalDialog(vm);

                        //var message =
                        //    new DialogDisplayEvent(;
                        //await eventAggregator.PublishOnUIThreadAsync(message);
                    }, canRunTestCommand);
                }
                else if (TestManager?.VolumeTestManager is AutoVolumeTestManager)
                {
                    RunVolumeTestCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        //var message =
                        //    new DialogDisplayEvent(new ManualVolumeTestDialogViewModel(TestManager.VolumeTestManager, eventAggregator, RunTest));
                        //await eventAggregator.PublishOnUIThreadAsync(message);
                    }, canRunTestCommand);
                }

                PostVolumeTestCommand = (ReactiveCommand<Unit, Unit>) DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator,
                    "Completing Volume Test...", RunPostVolumeTest);

                ExecuteVolumeTestCommand = ReactiveCommand.CreateCombined(new[] { PreVolumeTestCommand },
                    canRunTestCommand);

                ExecuteVolumeTestCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(async () =>
                {
                    await PreVolumeTestCommand.Execute();
                }));

                TestManager.VolumeTestManager.TestStepsObservable
                    .Select(x =>
                    {
                        switch (x)
                        {
                            case VolumeTestSteps.PreTest:
                                return RunVolumeTestCommand.Execute();
                            case VolumeTestSteps.ExecutingTest:
                                return PostVolumeTestCommand.Execute();

                            default:
                                return null;
                        }
                    })
                    .Subscribe();               

                //TestManager.VolumeTestManager.TestStepsObservable
                //    .Where(x => x == VolumeTestSteps.ExecutingTest)                    
                //    .InvokeCommand(this, x => x.PostVolumeTestCommand);
            }
        }

        public ReactiveCommand<Unit, Unit> PreVolumeTestCommand { get; set; }
        public ReactiveCommand ExecuteVolumeTestCommand { get; set; }
        public ReactiveCommand<Unit, Unit> PostVolumeTestCommand { get; set; }

        private async Task RunPreVolumeTest(IObserver<string> status, CancellationToken ct)
        {
            TestManager.VolumeTestManager.StatusMessage.Subscribe(status);

            if (SettingsManager.SettingsInstance.TestSettings.RunVolumeSyncTest)
                await TestManager.VolumeTestManager.ExecuteSyncTest(ct);

            await TestManager.VolumeTestManager.PreTest(ct);
        }

        private async Task RunPostVolumeTest(IObserver<string> status, CancellationToken ct)
        {
            try
            {
                TestManager.VolumeTestManager.StatusMessage.Subscribe(status);
                await TestManager.VolumeTestManager.PostTest(ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

      

        public async Task RunTest(CancellationToken ct)
        {
            try
            {
                await TestManager.VolumeTestManager.ExecutingTest(ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
        }

        private void CreateDriveSpecificViews()
        {
            if (Volume?.DriveType is MechanicalDrive)
                EnergyTestItem = new EnergyTestViewModel(EventAggregator, (MechanicalDrive) Volume.DriveType);
            else if (Volume?.DriveType is RotaryDrive)
                MeterDisplacementItem = new RotaryMeterTestViewModel((RotaryDrive) Volume.DriveType);
        }

        private ReactiveCommand<Unit, Unit> _runVolumeTestCommand;

        public ReactiveCommand<Unit, Unit> RunVolumeTestCommand
        {
            get => _runVolumeTestCommand;
            set => this.RaiseAndSetIfChanged(ref _runVolumeTestCommand, value);
        }

        public IQaRunTestManager TestManager { get; set; }
        public Instrument Instrument => Volume.Instrument;
        public Core.Models.Instruments.VolumeTest Volume { get; }

        public decimal AppliedInput
        {
            get => Volume.AppliedInput;
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChangeEvents();
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

        public int UncorrectedPulseCount
        {
            get => Volume.UncPulseCount;
            set
            {
                Volume.UncPulseCount = value;
                RaisePropertyChangeEvents();
            }
        }

        public int CorrectedPulseCount
        {
            get => Volume.CorPulseCount;
            set
            {
                Volume.CorPulseCount = value;
                RaisePropertyChangeEvents();
            }
        }

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

        private readonly ObservableAsPropertyHelper<bool> _showRunButton;
        public bool ShowRunButton => _showRunButton.Value;

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