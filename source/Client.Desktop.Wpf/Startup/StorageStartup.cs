using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    public class StorageStartup : IStartupTask
    {
        private const string KeyValueStoreConnectionString = "LiteDb";
        private readonly IServiceProvider _provider;
        private readonly DatabaseSeeder _seeder;

        public StorageStartup(IServiceProvider provider, DatabaseSeeder seeder = null)
        {
            _provider = provider;
            _seeder = seeder ?? new DatabaseSeeder(provider);
        }

        #region IStartupTask Members

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var repo = _provider.GetService<IDeviceRepository>();
            await repo.Load(new[] { MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance });

            await _seeder.SeedDatabase(5);
        }

        #endregion

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var config = host.Configuration;

            services.AddStartTask<StorageStartup>();
            
            if (config.IsLiteDb())
                AddLiteDb(services, host);

            //services.AddSingleton<EvcVerificationTestService>();
            services.AddSingleton<IVerificationTestService, VerificationTestService>();
        }

        private static void AddLiteDb(IServiceCollection services, HostBuilderContext host)
        {
            var db = StorageDefaults.CreateLiteDb(host.Configuration.LiteDbPath());
            services.AddSingleton(c => db);

            services.AddSingleton<IDeviceRepository, DeviceRepository>();
            services.AddSingleton<IRepository<DeviceType>>(c =>
                new LiteDbRepository<DeviceType>(db));

            services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c =>
                new VerificationsLiteDbRepository(db, c.GetRequiredService<IDeviceRepository>()));

            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }

      

     
    }
}