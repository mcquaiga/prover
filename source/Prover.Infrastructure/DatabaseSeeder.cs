using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure.SampleData;

namespace Prover.Infrastructure
{
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _provider;

        public DatabaseSeeder(IServiceProvider provider)
        {
            _provider = provider;
        }

        

        public async Task SeedDatabase( int records = 1)
        {
            Debug.WriteLine($"Seeding data...");
            var results = new List<EvcVerificationTest>();
            var random = new Random(10000);
            var watch = Stopwatch.StartNew();

            var deviceType = _provider.GetService<IDeviceRepository>().GetByName("Mini-Max");
            var testService = _provider.GetService<IVerificationTestService>();
            
            var serialNumberItem = deviceType.GetItemMetadata(62);

            for (int i = 0; i < records; i++)
            {
                var device = deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);
                device.SetItemValue(serialNumberItem, random.Next(10000, 999999).ToString());
                device.SetItemValue(201, random.Next(10000, 999999).ToString());
                
                var testVm = testService.NewVerification(device);

                testVm.SubmittedDateTime = testVm.TestDateTime.Value.AddSeconds(random.Next(180, 720));

                testVm.SetItems<TemperatureItems>(device, 0, ItemFiles.TempLowItems);
                testVm.SetItems<TemperatureItems>(device, 1, ItemFiles.TempMidItems);
                testVm.SetItems<TemperatureItems>(device, 2, ItemFiles.TempHighItems);
                
                testVm.SetItems<PressureItems>(device, 0, ItemFiles.PressureTest(0));
                testVm.SetItems<PressureItems>(device, 1, ItemFiles.PressureTest(1));
                testVm.SetItems<PressureItems>(device, 2, ItemFiles.PressureTest(2));

                results.Add(testService.CreateModel(testVm));

                //await testService.AddOrUpdate(testVm);

                Debug.WriteLine($"Created verification test {i} of {records}.");
            }

            await results.ToObservable().ForEachAsync(r => testService.AddOrUpdate(r));

            watch.Stop();
            Debug.WriteLine($"Seeding completed in {watch.ElapsedMilliseconds} ms");
        }
    }
}