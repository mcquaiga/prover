using DynamicData.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.DevTools.Hardware;
using Prover.DevTools.SampleData;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Startup;
using System;
using System.Reactive.Concurrency;

namespace Prover.DevTools
{
	internal static class DevLogger
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

			//services.AddSingleton<IToolbarItem, DevToolbarMenu>();
		}

		private IDisposable DataGenerator(IVerificationTestService verificationTestService)
		{
			return ThreadPoolScheduler.Instance.ScheduleRecurringAction(TimeSpan.FromSeconds(5),
									   async () =>
									   {
										   var random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);

										   var device = DeviceSamples.MiniMax;
										   var test = device.NewVerification();

										   test.TestDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 60)));
										   await verificationTestService.Upsert(test);

									   });
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

	//public class DevToolbarMenu : IToolbarItem
	//{
	//	public DevToolbarMenu(ICollection<ViewModelBase> devMenuItems = null)
	//	{

	//	}
	//}


}
