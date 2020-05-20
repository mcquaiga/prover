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
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Prover.UI.Desktop.Startup
{
	public partial class StorageStartup : IHostedService
	{
		private const string KeyValueStoreConnectionString = "LiteDb";
		private readonly IServiceProvider _provider;
		private IEnumerable<ICachedRepository> _caches;


		public StorageStartup(IServiceProvider provider)
		{
			_provider = provider;
			_caches = _provider.GetServices<ICachedRepository>();
		}

		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken)
		{
			///_ = DeviceRepository.Instance;

			var loading = _caches.ForEach(c => Task.Run(() => c.StartAsync(cancellationToken)).ConfigureAwait(false));


			return Task.CompletedTask;
			//return Task.CompletedTask;
		}

		/// <inheritdoc />
		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await Task.WhenAll(_caches.Select(c => c.StopAsync(cancellationToken)));
		}

	}

	public partial class StorageStartup
	{
		public static void AddServices(IServiceCollection services, HostBuilderContext host)
		{
			var config = host.Configuration;

			services.AddHostedService<StorageStartup>();
			services.AddSingleton<IVerificationService, VerificationService>();

			services.AddRepositories(host);
			AddDashboard(services, host);
		}

		private static void AddDashboard(IServiceCollection services, HostBuilderContext host)
		{
			var oneWeekAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

			services.AddSingleton<DashboardService>();
			services.AddSingleton<DashboardViewModel>();
		}
	}

	public static class StorageServiceEx
	{
		public static void AddRepositories(this IServiceCollection services, HostBuilderContext host)
		{
			var repo = AddLiteDb(services, host);

			if (host.Configuration.UseAzure())
				repo = AddMongoDb(services, host);

			//services.AddCaching(host);

			services.AddSingleton<VerificationCachedRepository>(c => ActivatorUtilities.CreateInstance<VerificationCachedRepository>(c, repo));
			services.AddSingleton<ICachedRepository>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IEntityCache<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IQueryableRepository<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
		}

		public static void AddCaching(this IServiceCollection services, HostBuilderContext host)
		{
			//services.AddAllTypes<ICachedRepository>(lifetime: ServiceLifetime.Singleton);

			//services.AddSingleton<EntityCache<EvcVerificationTest>>();
			//services.AddSingleton<IEntityDataCache<EvcVerificationTest>>(c => c.GetRequiredService<EntityCache<EvcVerificationTest>>());
			//services.AddSingleton<ICacheAggregateRoot<EvcVerificationTest>>(c => c.GetRequiredService<EntityCache<EvcVerificationTest>>());


			//var repoDesc = new ServiceDescriptor(typeof(IAsyncRepository<EvcVerificationTest>), c => c.GetRequiredService<VerificationCachedRepository>(), ServiceLifetime.Singleton);
			//services.Replace(repoDesc);


		}

		private static IAsyncRepository<EvcVerificationTest> AddMongoDb(IServiceCollection services, HostBuilderContext host)
		{
			var cosmo = new CosmosDbAsyncRepository<EvcVerificationTest>();

			//Task.Run(() => cosmo.Initialize());

			services.AddSingleton<CosmosDbAsyncRepository<EvcVerificationTest>>(cosmo);
			//services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c => c.GetRequiredService<CosmosDbAsyncRepository<EvcVerificationTest>>());

			return cosmo;
		}

		private static IAsyncRepository<EvcVerificationTest> AddLiteDb(IServiceCollection services, HostBuilderContext host)
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
				{
					var repo = new VerificationsLiteDbRepository(db);
					services.AddSingleton<VerificationsLiteDbRepository>(repo);
					return repo;
				}
			}

			return null;
		}
	}
}