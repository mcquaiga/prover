using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class NewTestRunViewModel : DialogViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public NewTestRunViewModel(ILogger<NewTestRunViewModel> logger,
            IScreenManager screenManager,
            IVerificationManagerFactory verificationManagerFactory,
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
            
            ApplicationSettings.Local.VerificationFilePath = "";

            LoadFromFile = ReactiveCommand.CreateFromObservable(() =>
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK || !fileDialog.CheckFileExists)
                    return Observable.Empty<ITestManager>();

                ApplicationSettings.Local.VerificationFilePath = fileDialog.FileName;
                
                return Observable.StartAsync(async () =>
                {
                    var itemFile = await ItemLoader.LoadFromFile(deviceRepository, fileDialog.FileName);
                    return await verificationManagerFactory.StartNew(itemFile.Device.DeviceType);
                });
            });
            

            StartTestCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                SetLastUsedSettings();
                return Observable.StartAsync(async () => await verificationManagerFactory.StartNew(SelectedDeviceType));
            }).DisposeWith(_cleanup);

            StartTestCommand.Merge(LoadFromFile)
                            .InvokeCommand(ReactiveCommand.CreateFromTask<IRoutableViewModel>(screenManager.ChangeView));          
        }

        public LocalSettings Selected => ApplicationSettings.Local;
        public ReactiveCommand<Unit, ITestManager> LoadFromFile { get; protected set; }
        public ReactiveCommand<Unit, ITestManager> StartTestCommand { get; set; }
        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }
        public ReadOnlyObservableCollection<int> BaudRates { get; set; }
        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }
        [Reactive] public string TestDefinitionFilePath { get; set; }

        private void SetLastUsedSettings()
        {            
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;        
            ApplicationSettings.Instance.SaveSettings();
        }
    }
}