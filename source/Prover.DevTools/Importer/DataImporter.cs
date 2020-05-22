using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prover.Application.Interactions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.ViewModels;
using Prover.Calculations;
using Prover.Shared.Storage.Interfaces;
using ReactiveUI;

namespace Prover.Modules.DevTools.Importer
{
	public class DataImporter : ViewModelBase, IDevToolsMenuItem
	{
		private static SessionContext _session;
		private static List<DeviceType> _devices;
		private static readonly ILogger<DataImporter> _logger = DevLogger.Factory.CreateLogger<DataImporter>();
		private readonly IAsyncRepository<EvcVerificationTest> _repository;

		private static readonly Subject<int> _totalItemSubject = new Subject<int>();
		public static IObservable<int> TotalItemsCount => _totalItemSubject;

		static DataImporter()
		{
			_session = new SessionContext();
			_devices = DeviceRepository.Instance.GetAll().ToList();
		}

		public DataImporter(IAsyncRepository<EvcVerificationTest> repository)
		{
			_repository = repository;

			var import = ReactiveCommand.CreateFromTask(async () =>
			{
				var folderBrowser = new FolderBrowserDialog();

				if (folderBrowser.ShowDialog() != DialogResult.OK)
					return;

				var importer = Observable.Create<ImportStatusUpdate>(async obs =>
				{
					await ImportTests(_repository, folderBrowser.SelectedPath, obs);
					return new CompositeDisposable();
				}).Publish();

				await Notifications.SnackBarUpdates.Handle(importer.Select(x => x.ToString()));

				importer.Connect();
				//Task.Run(async () => await );

			}, outputScheduler: RxApp.MainThreadScheduler);

			Command = import;

			//var msgUpdates = this.WhenAnyObservable(x => x.RecordCount, x => x.RecordTotal).Select((current, total) => $"IMPORTED {current} OF {total}");
			//import.Subscribe(async _ => await Notifications.SnackBarUpdates.Handle("DATA IMPORT COMPLETE"));
		}



		/// <inheritdoc />
		public string Description { get; set; } = "Import Tests...";

		/// <inheritdoc />
		public ICommand Command { get; set; }

		public static DeviceType GetDevice(int deviceId)
		{
			return _devices.FirstOrDefault(d => d.Id == Mappers.DeviceTypeMappings[deviceId]);
		}

		public static Guid GetDeviceTypeId(int deviceId)
		{
			if (_devices == null)
			{
				var repo = DeviceRepository.Instance;
				_devices = repo.GetAll().ToList();
			}

			return Mappers.DeviceTypeMappings[deviceId];
		}

		public static async Task ImportTests(IAsyncRepository<EvcVerificationTest> testService, string folderPath, IObserver<ImportStatusUpdate> updates = null)
		{
			_logger.LogDebug("Starting import....");

			var totalRecords = CheckFolderPath(folderPath);
			var record = 0;
			foreach (var file in Directory.EnumerateFiles(folderPath))
			{
				try
				{
					var qaTest = await GetObjectFromFile(file);
					var device = GetDeviceInstance(qaTest);

					var model = GetVerificationModel(device, qaTest);
					await testService.UpsertAsync(model);
					record++;
					updates?.OnNext(new ImportStatusUpdate() { Count = record, Total = totalRecords, Test = model });
				}
				catch (AggregateException aggregateException)
				{
					aggregateException.Flatten().InnerExceptions.ForEach(x => _logger.LogError(x.Message));
				}

				catch (Exception ex)
				{
					_logger.LogError("", ex);
				}
			}

			updates?.OnCompleted();

			_logger.LogDebug($"Import complete!");
		}

		private static void GenerateTestData(EvcVerificationTest model, QaTestRunDTO qaTest)
		{
			var random = new Random(DateTime.Now.Millisecond);
			var testDate = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 30)));
			model.TestDateTime = testDate.AddHours(random.Next(-12, 18));
			model.SubmittedDateTime = model.TestDateTime.AddSeconds(random.Next(180, 660));
			model.Verified = qaTest.IsPassed;
		}

		private static EvcVerificationTest GetVerificationModel(DeviceInstance device, QaTestRunDTO qaTest)
		{
			var builder = device.BuildVerification();

			qaTest.Tests.OrderBy(x => x.TestNumber)
				  .ForEach(vt =>
				  {
					  var testDef = VerificationDefaults.VerificationOptions.CorrectionTestDefinitions.First(x => x.Level == vt.TestNumber);

					  builder.AddTestPoint(tp =>
					  {
						  var gaugePressure = device.Items.Pressure != null ? PressureCalculator.GetGaugePressure(device.Items.Pressure.Range, testDef.PressureGaugePercent) : 0m;
						  var result = tp.Generate(gaugePressure, testDef.TemperatureGauge, vt.Values);

						  if (vt.EndValues != null && testDef.IsVolumeTest)
							  result.WithVolume(vt.Values, vt.EndValues, vt.AppliedInput.ToInt32(), vt.CorPulses, vt.UnCorPulses);
						  return result;
					  });
				  });
			var model = builder.Build();

			GenerateTestData(model, qaTest);

			return model;
		}

		private static DeviceInstance GetDeviceInstance(QaTestRunDTO qaTest)
		{
			if (qaTest.Device == null)
				throw new NullReferenceException(nameof(qaTest.Device));

			var deviceType = DeviceRepository.Instance.GetById(qaTest.Device.DeviceTypeId);
			var device = deviceType?.CreateInstance(qaTest.Device.Items);
			return device;
		}

		private static int CheckFolderPath(string folderPath)
		{
			if (!Directory.Exists(folderPath))
				throw new DirectoryNotFoundException($"{folderPath}");

			var totalRecords = Directory.GetFileSystemEntries(folderPath).Length;
			_logger.LogDebug($"Found {totalRecords} items at {folderPath}");
			_totalItemSubject.OnNext(totalRecords);
			return totalRecords;
		}

		private static async Task<QaTestRunDTO> GetObjectFromFile(string file)
		{
			using var reader = new StreamReader(file);

			var json = await reader.ReadToEndAsync();

			if (string.IsNullOrEmpty(json))
				throw new FileLoadException($"File {file} could not be loaded");

			return JsonConvert.DeserializeObject<QaTestRunDTO>(json);
		}

		public class ImportStatusUpdate
		{
			public int Count { get; set; }
			public int Total { get; set; }
			public EvcVerificationTest Test { get; set; }

			/// <inheritdoc />
			public override string ToString() => $"Record {Count} of {Total}.";
		}
	}
}