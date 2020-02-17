using System;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Infrastructure.KeyValueStore;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;

namespace Client.Wpf.Startup
{
    public class Storage : IHostedService
    {
        private readonly IServiceProvider _provider;

        public Storage(IServiceProvider provider)
        {
            _provider = provider;
        }

        private const string KeyValueStoreConnectionString = "KeyValueStore";

        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService(c => new Storage(c));

            services.AddSingleton<ILiteDatabase>(c => new LiteDatabase(c.GetService<IConfiguration>().GetConnectionString(KeyValueStoreConnectionString)));
            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           
        }
    }


}