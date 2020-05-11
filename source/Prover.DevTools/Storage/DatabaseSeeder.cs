using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Prover.Application.Extensions;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels.Factories;
using Prover.Modules.DevTools.SampleData;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.DevTools.Storage
{
	public class DatabaseSeeder
	{

		private readonly IServiceProvider _provider;

		public DatabaseSeeder(IServiceProvider provider)
		{
			_provider = provider;
		}


		public Task SeedDatabase(IRepository<EvcVerificationTest> repository, int records = 1)
		{
			Debug.WriteLine($"Seeding data...");
			var watch = Stopwatch.StartNew();

			var results = CreateTests(records);

			//_ = Task.Run(() => results.ForEach(t => repository.Update(t)));

			watch.Stop();
			Debug.WriteLine($"Seeding completed in {watch.ElapsedMilliseconds} ms");
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<EvcVerificationTest>> CreateTests(int records = 1)
		{

			var results = new List<EvcVerificationTest>();
			var random = new Random(DateTime.Now.Millisecond);


			var deviceType = DeviceRepository.Instance.GetByName("Mini-Max");


			var serialNumberItem = deviceType.GetItemMetadata(62);

			for (int i = 0; i < records; i++)
			{
				var device = deviceType.CreateInstance(SampleItemFiles.MiniMaxItemFile);
				DeviceInstanceEx.SetItemValue(device, serialNumberItem, random.Next(10000, 999999).ToString());
				DeviceInstanceEx.SetItemValue(device, 201, random.Next(10000, 999999).ToString());

				var testVm = VerificationViewModelFactory.Create(device);
				testVm.TestDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(4, 30)));
				testVm.SubmittedDateTime = testVm.TestDateTime.AddSeconds(random.Next(180, 720));

				testVm.SubmittedDateTime = testVm.TestDateTime.AddSeconds(random.Next(180, 720));

				testVm.SetItems<TemperatureItems>(device, 0, SampleItemFiles.TempLowItems);
				testVm.SetItems<TemperatureItems>(device, 1, SampleItemFiles.TempMidItems);
				testVm.SetItems<TemperatureItems>(device, 2, SampleItemFiles.TempHighItems);

				testVm.SetItems<PressureItems>(device, 0, SampleItemFiles.PressureTest(0));
				testVm.SetItems<PressureItems>(device, 1, SampleItemFiles.PressureTest(1));
				testVm.SetItems<PressureItems>(device, 2, SampleItemFiles.PressureTest(2));

				results.Add(testVm.ToModel());

			}
			Debug.WriteLine($"Created {records} verification tests.");
			return results;
		}

	}
}