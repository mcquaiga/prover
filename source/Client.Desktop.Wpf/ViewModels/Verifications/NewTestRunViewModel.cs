using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Dialogs;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using Prover.Application.FileLoader;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.Settings;
using Prover.Infrastructure.SampleData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DialogResult = Prover.Application.Interfaces.DialogResult;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class NewTestRunViewModel : DialogViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public NewTestRunViewModel(ILogger<NewTestRunViewModel> logger,
            IScreenManager screenManager,
            ITestManagerFactory verificationManagerFactory,
            IDeviceSessionManager commDeviceManager,
            //FileDeviceSessionManager fileDeviceManager,
            IDeviceRepository deviceRepository)
        {
            deviceRepository.All.Connect()
                .Filter(d => !d.IsHidden)
                .Sort(SortExpressionComparer<DeviceType>.Ascending(p => p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var deviceTypes)
                .Filter(d => d.Id == ApplicationSettings.Local.LastDeviceTypeUsed)
                .Do(d => SelectedDeviceType = d.FirstOrDefault().Current)
                .Subscribe().DisposeWith(_cleanup);
            DeviceTypes = deviceTypes;

            SerialPort.GetPortNames().AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var ports)
                .Subscribe().DisposeWith(_cleanup);
            CommPorts = ports;

            SerialPort.BaudRates.AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var baudRates)
                .Subscribe().DisposeWith(_cleanup);
            BaudRates = baudRates;

            //SelectedBaudRate = Selected.InstrumentBaudRate;
            //SelectedCommPort = Selected.InstrumentCommPort;
            //SelectedTachCommPort = Selected.TachCommPort;

            StartTestCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                SetLastUsedSettings();
                return Observable.StartAsync(async () => await verificationManagerFactory.StartNew(commDeviceManager, SelectedDeviceType));
            }).DisposeWith(_cleanup);

            //var changeScreens = ReactiveCommand.CreateFromTask<ITestManager>(async vm => await screenManager.ChangeView(vm));
            StartTestCommand
                .InvokeCommand(ReactiveCommand.CreateFromTask<IRoutableViewModel>(async vm =>
                {
                    await screenManager.ChangeView(vm);
                }));

            ApplicationSettings.Local.VerificationFilePath = "";
            LoadFromFile = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (fileDialog.CheckFileExists)
                    {
                        ApplicationSettings.Local.VerificationFilePath = fileDialog.FileName;
                        await ApplicationSettings.Instance.SaveSettings();
                    }
                    
                }
                
            });
        }

        public LocalSettings Selected => ApplicationSettings.Local;
        public ReactiveCommand<Unit, Unit> LoadFromFile { get; protected set; }
        public ReactiveCommand<Unit, ITestManager> StartTestCommand { get; set; }
        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }
        public ReadOnlyObservableCollection<int> BaudRates { get; set; }
        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        private void SetLastUsedSettings()
        {
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;
            //ApplicationSettings.Local.InstrumentCommPort = SelectedCommPort;
            //ApplicationSettings.Local.InstrumentBaudRate = SelectedBaudRate;
            //ApplicationSettings.Local.TachCommPort = SelectedTachCommPort;
            ApplicationSettings.Instance.SaveSettings();
        }
    }
}