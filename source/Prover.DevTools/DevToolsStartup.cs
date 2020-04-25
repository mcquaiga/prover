using DynamicData.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using Prover.DevTools.Hardware;
using Prover.DevTools.SampleData;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Startup;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace Prover.DevTools
{
	public static class DevLogger
	{
		public static ILoggerFactory Factory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
			builder.AddDebug();
			builder.SetMinimumLevel(LogLevel.Debug);

		});

		public static ILogger Logger { get; } = Factory.CreateLogger("DevTools");
	}

	public class DevToolsStartup : IConfigureModule
	{
		/// <inheritdoc />
		public void Configure(HostBuilderContext builder, IServiceCollection services)
		{
			services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
			services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);

			services.AddViewsAndViewModels();

			services.AddSingleton<IDevToolsMenuItem, DataGenerator>();
			services.AddSingleton<IToolbarItem, DevToolbarMenu>();
		}



		//LoadFromFile = ReactiveCommand.CreateFromObservable(() =>
		//{
		//    var fileDialog = new OpenFileDialog();

		//    if (fileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK || !fileDialog.CheckFileExists)
		//        return Observable.Empty<IDeviceQaTestManager>();

		//    ApplicationSettings.Local.VerificationFilePath = fileDialog.FileName;

		//    return Observable.StartAsync(async () =>
		//    {
		//        var itemFile = await ItemLoader.LoadFromFile(deviceRepository, fileDialog.FileName);
		//        return await verificationManagerFactory.StartNew(itemFile.Device.DeviceType);
		//    });
		//});
	}

	public interface IDevToolsMenuItem
	{
		public string Description { get; set; }

		public ICommand Command { get; set; }
	}

	public class DevToolbarMenu : ViewModelBase, IToolbarItem
	{
		public DevToolbarMenu(IEnumerable<IDevToolsMenuItem> devMenuItems = null)
		{
			MenuItems = devMenuItems.ToList();
		}

		[Reactive] public ICollection<IDevToolsMenuItem> MenuItems { get; set; }
	}

	internal class DataGenerator : ViewModelBase, IDevToolsMenuItem
	{
		private readonly IAsyncRepository<EvcVerificationTest> _repository;
		private CompositeDisposable _cleanup;

		public DataGenerator(IAsyncRepository<EvcVerificationTest> repository)
		{
			_repository = repository;
			Command = ReactiveCommand.Create(StartStopGenerator);
		}

		/// <inheritdoc />
		[Reactive] public string Description { get; set; } = "Start data generator";

		/// <inheritdoc />
		[Reactive] public ICommand Command { get; set; }

		private void StartStopGenerator()
		{
			if (_cleanup == null || _cleanup.IsDisposed)
			{
				_cleanup = new CompositeDisposable();
				StartGenerator().DisposeWith(_cleanup);
				Description = "Stop data generator";
				return;
			}

			if (_cleanup != null && !_cleanup.IsDisposed)
			{
				StopGenerator();
				Description = "Start data generator";
				return;
			}
		}

		private void StopGenerator()
		{
			_cleanup?.Dispose();
		}

		private IDisposable StartGenerator()
		{
			return ThreadPoolScheduler.Instance.ScheduleRecurringAction(TimeSpan.FromSeconds(5), async () =>
			{
				var random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);

				var device = DeviceSamples.MiniMax;
				var test = device.NewVerification();

				test.TestDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 60)));
				await _repository.UpsertAsync(test);
				VerificationEvents.OnSave.Publish(test);
			});
		}
	}

}
