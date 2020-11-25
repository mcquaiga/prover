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

namespace Prover.UI.Desktop.Startup {
	public partial class StorageStartup : IHostedService {
		private const string KeyValueStoreConnectionString = "LiteDb";
		private readonly IServiceProvider _provider;
		private IEnumerable<ICachedRepository> _caches;


		public StorageStartup(IServiceProvider provider) {
			_provider = provider;
			_caches = _provider.GetServices<ICachedRepository>();
		}

		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken) {
			///_ = DeviceRepository.Instance;

			var loading = _caches.ForEach(c => Task.Run(() => c.StartAsync(cancellationToken)).ConfigureAwait(false));


			return Task.CompletedTask;

		}

		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken) {
			return Task.WhenAll(_caches.Select(c => c.StopAsync(cancellationToken)));
		}

	}

	public partial class StorageStartup {
		public static void AddServices(IServiceCollection services, HostBuilderContext host) {
			var config = host.Configuration;

			services.AddHostedService<StorageStartup>();
			services.AddSingleton<IVerificationService, VerificationService>();

			services.AddRepositories(host);
			AddDashboard(services, host);
		}

		private static void AddDashboard(IServiceCollection services, HostBuilderContext host) {
			var oneWeekAgo = DateTime.Now.Subtract(TimeSpan.FromDays(7));

			services.AddSingleton<DashboardService>();
			services.AddSingleton<DashboardViewModel>();
		}
	}

	public static class StorageServiceEx {
		public static void AddRepositories(this IServiceCollection services, HostBuilderContext host) {

			AddAzureCosmoDb(services, host);
			AddLiteDb(services, host);

			if (!host.Configuration.UseAzure()) {
				services.AddSingleton<VerificationsLiteDbRepository>();
				services.AddSingleton<Func<IAsyncRepository<EvcVerificationTest>>>(c => () => c.GetRequiredService<VerificationsLiteDbRepository>());
			}

			services.AddSingleton<VerificationCachedRepository>();
			services.AddSingleton<ICachedRepository>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IEntityCache<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IAsyncRepository<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
			services.AddSingleton<IQueryableRepository<EvcVerificationTest>>(c => c.GetRequiredService<VerificationCachedRepository>());
		}

		public static void AddCaching(this IServiceCollection services, HostBuilderContext host) {


		}

		private static void AddAzureCosmoDb(IServiceCollection services, HostBuilderContext host) {
			if (!host.Configuration.UseAzure())
				return;

			var cosmo = new CosmosDbAsyncRepository<EvcVerificationTest>();

			services.AddSingleton<CosmosDbAsyncRepository<EvcVerificationTest>>(cosmo);
			services.AddSingleton<Func<IAsyncRepository<EvcVerificationTest>>>(c => () => c.GetRequiredService<CosmosDbAsyncRepository<EvcVerificationTest>>());


		}

		private static void AddLiteDb(IServiceCollection services, HostBuilderContext host) {
			if (host.Configuration.UseLiteDb()) {
				var db = StorageDefaults.CreateLiteDb(host.Configuration.LiteDbPath());
				services.AddSingleton(c => db);

				var deviceRepo = DeviceRepository.Instance;
				services.AddSingleton<IDeviceRepository>(DeviceRepository.Instance);

				//services.AddSingleton(typeof(IRepository<>), typeof(IRepository<AggregateRoot>));

				services.AddSingleton<IRepository<DeviceType>>(c =>
						new LiteDbRepository<DeviceType>(db));

				services.AddSingleton<IKeyValueStore, LiteDbKeyValueStore>();


			}
		}
	}
}