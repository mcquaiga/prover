using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Calculations;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.DevTools.Importer
{
	public class DataImporter
	{
		private static SessionContext _session;
		private static List<DeviceType> _devices;
		private static ILogger<DataImporter> _logger = DevLogger.Factory.CreateLogger<DataImporter>();


		static DataImporter()
		{
			_session = new SessionContext();

			_devices = DeviceRepository.Instance.GetAll()
									   .ToList();
		}

		public static DeviceType GetDevice(int deviceId)
		{
			return _devices.FirstOrDefault(d => d.Id == Mappers.DeviceTypeMappings[deviceId]);
		}

		public static Guid GetDeviceTypeId(int deviceId)
		{
			if (_devices == null)
			{
				var repo = DeviceRepository.Instance;

				_devices = repo.GetAll()
							   .ToList();
			}

			return Mappers.DeviceTypeMappings[deviceId];
		}

		public static async Task ImportTests(IAsyncRepository<EvcVerificationTest> testService, string folderPath)
		{
			_logger.LogDebug($"Starting data import....");
			if (!Directory.Exists(folderPath))
				throw new DirectoryNotFoundException($"{folderPath}");

			var recordCount = 0;
			var totalRecords = Directory.GetFileSystemEntries(folderPath).Length;
			_logger.LogDebug($"Found {totalRecords} items at {folderPath}");

			foreach (var file in Directory.EnumerateFiles(folderPath))
				try
				{
					using (var reader = new StreamReader(file))
					{
						var json = await reader.ReadToEndAsync();

						if (!string.IsNullOrEmpty(json))
						{
							var qaTest = JsonConvert.DeserializeObject<QaTestRunDTO>(json);
							//Debug.WriteLine($"Device Id: {qaTest.Device.DeviceTypeId}");

							if (qaTest.Device != null)
							{
								var deviceType = DeviceRepository.Instance.GetById(qaTest.Device.DeviceTypeId);
								var device = deviceType?.CreateInstance(qaTest.Device.Items);
								if (device != null)
								{
									try
									{
										var builder = device.BuildVerification();

										qaTest.Tests.OrderBy(x => x.TestNumber)
											  .ForEach(vt =>
											  {
												  var testDef = VerificationTestOptions.Defaults.CorrectionTestDefinitions.First(x => x.Level == vt.TestNumber);

												  builder.AddTestPoint(tp =>
												  {
													  var gaugePressure = device.Items.Pressure != null
															  ? PressureCalculator.GetGaugePressure(device.Items.Pressure.Range, testDef.PressureGaugePercent)
															  : 0m;
													  var result = tp.Generate(gaugePressure, testDef.TemperatureGauge, vt.Values);

													  if (vt.EndValues != null && testDef.IsVolumeTest == true)
														  result.WithVolume(vt.Values, vt.EndValues, vt.AppliedInput.ToInt32(), vt.CorPulses, vt.UnCorPulses);

													  return result;
												  });
											  });

										var model = builder.Build();
										var random = new Random(DateTime.Now.Millisecond);

										var testDate = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 60)));
										model.TestDateTime = testDate.AddHours(random.Next(-12, 18));

										model.SubmittedDateTime = model.TestDateTime.AddSeconds(random.Next(180, 660));
										model.Verified = qaTest.IsPassed;
										await testService.UpsertAsync(model);
										recordCount++;
										_logger.LogDebug($"		{recordCount} of {totalRecords}");
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
							}
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError("", ex);
				}


			_logger.LogDebug($"Import complete! {recordCount} successful records.");
		}
	}
}