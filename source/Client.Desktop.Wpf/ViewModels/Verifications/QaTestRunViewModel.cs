using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class QaTestRunViewModel : RoutableViewModelBase, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public QaTestRunViewModel(
            ILogger<QaTestRunViewModel> logger, 
            IScreenManager screenManager,
            IVerificationTestService verificationService) : base(screenManager, "VerificationTests")
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () => {
                logger.LogDebug("Saving test...");

                var updated = await verificationService.AddOrUpdate(TestManager.TestViewModel);
                if (updated != null)
                {
                    logger.LogDebug("Saved test successfully");
                    await NotificationInteractions.SnackBarMessage.Handle("SAVED");
                    return true;
                }
                return false;
            }).DisposeWith(_cleanup);
            SaveCommand.ThrownExceptions.Subscribe();

            var canSubmit = this.WhenAnyValue(x => x.TestManager.TestViewModel.Verified);
            SubmitTest = ReactiveCommand.CreateFromTask(async () => {

                if (true)
                {
                    TestManager.TestViewModel.TestDateTime = DateTime.Now;

                    await SaveCommand.Execute();
                    (TestManager as IDisposable)?.Dispose();
                    await ScreenManager.GoHome();
                }
            }, canSubmit).DisposeWith(_cleanup);

            PrintTestReport = ReactiveCommand.CreateFromObservable(() => MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented.")).DisposeWith(_cleanup);
            
            this.WhenAnyObservable(x => x.TestManager.TestViewModel.VerifiedObservable)
                .Where(v => v)
                .Do(async x => await NotificationInteractions.SnackBarMessage.Handle("Verification Complete"))
                .Subscribe()
                .DisposeWith(Cleanup);

        }

        public extern ITestManager TestManager { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

        public ReactiveCommand<Unit, Unit> LoadFromFile { get; protected set; }
        public ReactiveCommand<Unit, ITestManager> StartTestCommand { get; set; }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }
        public ReadOnlyObservableCollection<int> BaudRates { get; set; }
        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

     

        //public ReactiveCommand<Unit, bool> CloseCommand { get; set; }

        //[Reactive] public bool IsDialogOpen { get; private set; }

        public void Dispose()
        { 
            (TestManager as IDisposable)?.Dispose();
            _cleanup?.Dispose();
        }

        private void SetLastUsedSettings()
        {
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;
            ApplicationSettings.Local.InstrumentCommPort = SelectedCommPort;
            ApplicationSettings.Local.InstrumentBaudRate = SelectedBaudRate;
            ApplicationSettings.Local.TachCommPort = SelectedTachCommPort;
            ApplicationSettings.Instance.SaveSettings();
        }
    }
}