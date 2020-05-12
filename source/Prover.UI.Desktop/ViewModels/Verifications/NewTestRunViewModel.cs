using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.Settings;
using Prover.UI.Desktop.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.UI.Desktop.ViewModels.Verifications
{
	public class NewTestRunViewModel : DialogViewModel
	{
		private readonly CompositeDisposable _cleanup = new CompositeDisposable();

		public NewTestRunViewModel(ILogger<NewTestRunViewModel> logger, IScreenManager screenManager,
				IVerificationService verificationService,
				IDeviceRepository deviceRepository)
		{
			deviceRepository.All.Connect().Filter(d => !d.IsHidden).Sort(SortExpressionComparer<DeviceType>.Ascending(p => p.Name)).ObserveOn(RxApp.MainThreadScheduler).Bind(out var deviceTypes)
							.Filter(d => d.Id == ApplicationSettings.Local.LastDeviceTypeUsed).Do(d => SelectedDeviceType = d.FirstOrDefault().Current).Subscribe().DisposeWith(_cleanup);
			DeviceTypes = deviceTypes;
			SerialPort.GetPortNames().AsObservableChangeSet().ObserveOn(RxApp.MainThreadScheduler).Bind(out var ports).Subscribe().DisposeWith(_cleanup);
			CommPorts = ports;
			SerialPort.BaudRates.AsObservableChangeSet().ObserveOn(RxApp.MainThreadScheduler).Bind(out var baudRates).Subscribe().DisposeWith(_cleanup);
			BaudRates = baudRates;
			ApplicationSettings.Local.VerificationFilePath = "";

			StartTestCommand = ReactiveCommand.CreateFromObservable(() =>
			{
				SetLastUsedSettings();
				return Observable.StartAsync(async () => await verificationService.StartVerification(SelectedDeviceType));
			}).DisposeWith(_cleanup);

			StartTestCommand.Select(x => x as IRoutableViewModel)
							//.Do(x => x == null ? throw new NullReferenceException("Test Manager was not convertable to IRoutableViewModel"))
							.Where(x => x != null)
							.InvokeCommand(ReactiveCommand.CreateFromTask<IRoutableViewModel>(screenManager.ChangeView));
		}

		public LocalSettings Selected => ApplicationSettings.Local;
		public ReactiveCommand<Unit, IQaTestRunManager> LoadFromFile { get; protected set; }
		public ReactiveCommand<Unit, IQaTestRunManager> StartTestCommand { get; set; }
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