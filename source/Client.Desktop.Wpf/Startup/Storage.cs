using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;
using Prover.Infrastructure.SampleData;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    public class Storage : IStartupTask
    {
        private const string KeyValueStoreConnectionString = "LiteDb";
        private readonly IServiceProvider _provider;

        public Storage(IServiceProvider provider) => _provider = provider;

        #region IHaveStartupTask Members

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var repo = _provider.GetService<IDeviceRepository>();
            await repo.Load(new[] { MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance });

            //await SeedDatabase(50);
        }

        #endregion

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var config = host.Configuration;

            services.AddStartTask<Storage>();
            
            if (config.IsLiteDb())
                AddLiteDb(services, host);

            services.AddSingleton<EvcVerificationTestService>();
            services.AddSingleton<IVerificationTestService, VerificationTestService>();
        }

        private static void AddLiteDb(IServiceCollection services, HostBuilderContext host)
        {
            var db = StorageDefaults.CreateLiteDb(host.Configuration.LiteDbPath());
            services.AddSingleton(c => db);

            services.AddSingleton<IRepository<DeviceType>>(c =>
                new LiteDbRepository<DeviceType>(db));

            services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c =>
                new VerificationsLiteDbRepository(db, c.GetService<DeviceRepository>()));

            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }

        private async Task SeedDatabase(int records = 1)
        {
            var watch = Stopwatch.StartNew();
            Debug.WriteLine($"Seeding data...");
            var deviceType = _provider.GetService<IDeviceRepository>().GetByName("Mini-Max");
            var device = deviceType.CreateInstance(ItemFiles.MiniMaxItemFile);

            var testService = _provider.GetService<IVerificationTestService>();
            var evcService = _provider.GetService<EvcVerificationTestService>();
            for (int i = 0; i < records; i++)
            {
                var testVm = testService.NewTest(device);
                var tempItems = deviceType.ToItemValues(ItemFiles.TempLowItems);
                var firstTest = testVm.VerificationTests.OfType<VerificationTestPointViewModel>().First(v => v.TestNumber == 0);
                firstTest.Temperature.Items = deviceType.GetGroupValues<TemperatureItems>(tempItems);
                
                await testService.AddOrUpdate(testVm);
                
                Debug.WriteLine($"Created verification test {i} of {records}.");
            }
            watch.Stop();
            Debug.WriteLine($"Seeding completed in {watch.ElapsedMilliseconds} ms");
        }

        private void SetItems<T>(DeviceType deviceType, EvcVerificationViewModel testVm, int testNumber, Dictionary<string, string> items)
            where T : ItemGroup
        {
            var tempItems = deviceType.ToItemValues(items);
            var firstTest = testVm.VerificationTests.OfType<VerificationTestPointViewModel>().First(v => v.TestNumber == testNumber);
            firstTest.VerificationTests.OfType<CorrectionTestViewModel<T>>().First().Items = deviceType.GetGroupValues<T>(tempItems);
        }

     
    }
}