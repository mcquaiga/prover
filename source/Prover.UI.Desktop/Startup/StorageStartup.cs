using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Dashboard;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Shared.Domain;
using Prover.Storage;
using Prover.Storage.LiteDb;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using Prover.UI.Desktop.Extensions;

namespace Prover.UI.Desktop.Startup
{
    public partial class StorageStartup : IHostedService
    {
        private const string KeyValueStoreConnectionString = "LiteDb";
        private readonly IServiceProvider _provider;
        private readonly DatabaseSeeder _seeder;

        public StorageStartup(IServiceProvider provider, DatabaseSeeder seeder = null)
        {
            _provider = provider;
            _seeder = seeder ?? new DatabaseSeeder(provider);
        }
        
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var devices = DeviceRepository.Instance;

            //_ = _seeder.SeedDatabase(250);
            
            _provider.GetServices<ICacheManager>()
                     .ToObservable()
                     .ObserveOn(ThreadPoolScheduler.Instance)
                     .ForEachAsync(c => c.LoadAsync(), cancellationToken);

            //RxApp.TaskpoolScheduler.Schedule(() => _seeder.SeedDatabase(200));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }

    public partial class StorageStartup
    {
        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var config = host.Configuration;

            services.AddHostedService<StorageStartup>();

            if (config.IsLiteDb())
                AddLiteDb(services, host);

            //services.AddSingleton<EvcVerificationTestService>();
            services.AddSingleton<VerificationTestService>();
            services.AddSingleton<IVerificationTestService>(c => c.GetRequiredService<VerificationTestService>());
            services.AddSingleton<IEntityDataCache<EvcVerificationTest>>(c => c.GetRequiredService<VerificationTestService>());
            services.AddSingleton<ICacheManager>(c => c.GetRequiredService<VerificationTestService>());

            AddDashboard(services, host);
        }

        private static void AddDashboard(IServiceCollection services, HostBuilderContext host)
        {
            var oneWeekAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

            services.AddSingleton<DashboardFactory>();
            services.AddSingleton<DashboardViewModel>();
            //services.AddValueDashboardItem("Verified Today", 
            //        v => v.TestDateTime.IsToday() && v.Verified);

            //services.AddValueDashboardItem("Failed Today",  
            //        v => v.TestDateTime.IsToday() && !v.Verified);

            //services.AddValueDashboardItem("Verified This Week", 
            //        v => v.TestDateTime.BetweenThenAndNow(oneWeekAgo) && v.Verified);

            //services.AddValueDashboardItem("Failed This Week",  
            //        v => v.TestDateTime.BetweenThenAndNow(oneWeekAgo) && !v.Verified);
       
        }

        private static void AddLiteDb(IServiceCollection services, HostBuilderContext host)
        {
            var db = StorageDefaults.CreateLiteDb(host.Configuration.LiteDbPath());
            services.AddSingleton(c => db);

            var deviceRepo = DeviceRepository.Instance;
            services.AddSingleton<IDeviceRepository>(deviceRepo);

            //services.AddSingleton(typeof(IRepository<>), typeof(IRepository<AggregateRoot>));

            services.AddSingleton<IRepository<DeviceType>>(c =>
                    new LiteDbRepository<DeviceType>(db));

            services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c =>
                    new VerificationsLiteDbRepository(db, c.GetRequiredService<IDeviceRepository>()));

            services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();
        }
    }

    public static class DashboardServiceEx
    {
        public static DateTime ThisWeek = DateTime.Now.Subtract(TimeSpan.FromDays(7));

        public static Func<EvcVerificationTest, bool> Today => (v => v.TestDateTime.IsToday());
        public static Func<EvcVerificationTest, bool> Verified => (v => v.Verified);

      

        public static void AddValueDashboardItem(this IServiceCollection services, string title, Func<EvcVerificationTest, bool> predicate)
        {
            //services.AddSingleton<IDashboardValueViewModel>(c => 
            //        new ValueDashboardViewModel(c.GetRequiredService<IEntityDataCache<EvcVerificationTest>>(), title, predicate));
        }
    }
}