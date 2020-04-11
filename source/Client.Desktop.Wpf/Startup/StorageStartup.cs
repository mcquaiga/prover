using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Dashboard;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;
using Prover.Shared.Extensions;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    public partial class StorageStartup : IStartupTask
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
            await StartCaches(cancellationToken);

            //await _seeder.SeedDatabase(5);
        }

        #endregion

        private async Task StartCaches(CancellationToken cancellationToken)
        {
            await Observable.StartAsync(() => _provider.GetService<IDeviceRepository>()
                                                       .Load(new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance}))
                            .Select(_ => Unit.Default)
                            .Concat(
                                    _provider.GetServices<ICacheManager>()
                                             .Select(c => Observable.StartAsync(c.LoadAsync))
                                             .Merge())
                            .RunAsync(cancellationToken);
        }
    }

    public partial class StorageStartup
    {
        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var config = host.Configuration;

            services.AddStartTask<StorageStartup>();

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

            services.AddSingleton<DashboardViewModel>();

            services.AddValueDashboardItem("Verified Today", 
                    v => v.TestDateTime.IsToday() && v.Verified);

            services.AddValueDashboardItem("Failed Today",  
                    v => v.TestDateTime.IsToday() && !v.Verified);

            services.AddValueDashboardItem("Verified This Week", 
                    v => v.TestDateTime.BetweenThenAndNow(oneWeekAgo) && v.Verified);

            services.AddValueDashboardItem("Failed This Week",  
                    v => v.TestDateTime.BetweenThenAndNow(oneWeekAgo) && !v.Verified);
       
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

    public static class DashboardServiceEx
    {
        public static DateTime ThisWeek = DateTime.Now.Subtract(TimeSpan.FromDays(7));

        public static Func<EvcVerificationTest, bool> Today => (v => v.TestDateTime.IsToday());
        public static Func<EvcVerificationTest, bool> Verified => (v => v.Verified);

      

        public static void AddValueDashboardItem(this IServiceCollection services, string title, Func<EvcVerificationTest, bool> predicate)
        {
            services.AddSingleton<IDashboardValueViewModel>(c => 
                    new ValueDashboardViewModel(c.GetRequiredService<IEntityDataCache<EvcVerificationTest>>(), title, predicate));
        }
    }
}