using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Devices.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    public class Storage : IHaveStartupTask
    {
        private const string KeyValueStoreConnectionString = "LocalData";
        private readonly IServiceProvider _provider;

        public Storage(IServiceProvider provider) => _provider = provider;

        #region IHaveStartupTask Members

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //var dbInitializer = _provider.GetService<LiteDbInitializer>();

            //dbInitializer.Initialize();
        }

        #endregion

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddStartTask<Storage>();

            var connectionString = Environment.ExpandEnvironmentVariables(
                host.Configuration.GetConnectionString(KeyValueStoreConnectionString));

            //LiteDb
            var db = StorageDefaults.CreateDatabase(connectionString);
            services.AddSingleton(c => db);

            services.AddScoped<EvcVerificationTestService>();
            services.AddScoped<VerificationViewModelService>();

            services.AddScoped<IAsyncRepository<EvcVerificationTest>>(c 
                => new VerificationsLiteDbRepository(StorageDefaults.Database, c.GetService<DeviceRepository>()));

            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }
    }
}