using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Caching;
using Prover.Application.Dashboard;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Shared.Storage.Interfaces;
using Prover.Storage.LiteDb;
using Prover.Storage.MongoDb;
using Prover.UI.Desktop.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.UI.Desktop.Startup
{
	public partial class StorageStartup : IHostedService
	{
		private const string KeyValueStoreConnectionString = "LiteDb";
		private readonly IServiceProvider _provider;


		public StorageStartup(IServiceProvider provider)
		{
			_provider = provider;
			//_seeder = seeder ?? new DatabaseSeeder(provider);
		}

		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken)
		{
			_ = DeviceRepository.Instance;
			//var cache = _provider.GetRequiredService<IEntityDataCache<EvcVerificationTest>>();
			//var task = Task.Run(() => _provider.GetRequiredService<CosmosDbAsyncRepository<EvcVerificationTest>>().Initialize());
			//var cache = _provider.GetRequiredService<CosmosDbAsyncRepository<EvcVerificationTest>>();
			//cache.Update();

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_provider.GetRequiredService<IEntityDataCache<EvcVerificationTest>>().Dispose();
			await Task.CompletedTask;
		}

	}

	public partial class StorageStartup
	{
		public static void AddServices(IServiceCollection services, HostBuilderContext host)
		{
			var config = host.Configuration;

			services.AddHostedService<StorageStartup>();

			AddLiteDb(services, host);
			AddMongoDb(services, host);

			services.AddSingleton<IVerificationService, VerificationService>();
			services.AddSingleton<IEntityDataCache<EvcVerificationTest>, VerificationCache>();

			AddDashboard(services, host);
		}

		private static void AddDashboard(IServiceCollection services, HostBuilderContext host)
		{
			var oneWeekAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

			services.AddSingleton<DashboardService>();
			services.AddSingleton<DashboardViewModel>();

		}

		private static void AddMongoDb(IServiceCollection services, HostBuilderContext host)
		{
			if (host.Configuration.UseAzure())
			{
				var cosmo = new CosmosDbAsyncRepository<EvcVerificationTest>();

				Task.Run(() => cosmo.Initialize());

				services.AddSingleton<CosmosDbAsyncRepository<EvcVerificationTest>>(cosmo);
				services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c => c.GetRequiredService<CosmosDbAsyncRepository<EvcVerificationTest>>());

			}

			//services.AddSingleton<IEventsSubscriber>(c =>
			//{
			//    var cosmo = cosmoTask.Result;
			//    var logger = c.GetService<ILogger<CosmosDbAsyncRepository<EvcVerificationTest>>>();
			//    VerificationEvents.OnSave.Subscribe(async (context) =>
			//    {
			//        try
			//        {
			//            logger.LogDebug($"Saving to Azure CosmoDb - Id: {context.Input.Id}");
			//            await cosmo.UpsertAsync(context.Input);
			//        }
			//        catch (Exception ex)
			//        {
			//            logger.LogError(ex, $"Error saving to Azure MongoDb");
			//        }
			//    });
			//    return cosmo;
			//});


		}

		private static void AddLiteDb(IServiceCollection services, HostBuilderContext host)
		{
			if (host.Configuration.UseLiteDb())
			{
				var db = StorageDefaults.CreateLiteDb(host.Configuration.LiteDbPath());
				services.AddSingleton(c => db);

				var deviceRepo = DeviceRepository.Instance;
				services.AddSingleton<IDeviceRepository>(DeviceRepository.Instance);

				//services.AddSingleton(typeof(IRepository<>), typeof(IRepository<AggregateRoot>));

				services.AddSingleton<IRepository<DeviceType>>(c =>
						new LiteDbRepository<DeviceType>(db));

				services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();

				if (!host.Configuration.UseAzure())
					services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c => new VerificationsLiteDbRepository(db, c.GetRequiredService<IDeviceRepository>()));

			}
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