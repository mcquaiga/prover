using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels.Services;
using Client.Wpf.Extensions;
using Devices.Core.Repository;
using Domain.EvcVerifications;
using Infrastructure.KeyValueStore;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;

namespace Client.Wpf.Startup
{
    public class Storage : IHaveStartupTask
    {
        private const string KeyValueStoreConnectionString = "KeyValueStore";
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

            //LiteDb
            services.AddSingleton<ILiteDatabase>(c =>
                new LiteDatabase(c.GetService<IConfiguration>().GetConnectionString(KeyValueStoreConnectionString)));

            services.AddScoped<EvcVerificationTestService>();
            services.AddScoped<VerificationViewModelService>();

            services.AddScoped<IAsyncRepository<EvcVerificationTest>>(c 
                => new LiteDbRepository<EvcVerificationTest>(c.GetService<ILiteDatabase>(), c.GetService<DeviceRepository>()));

            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }
    }
}