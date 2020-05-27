using System;
using System.IO;
using System.Linq;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Foundatio.AsyncEx;
using Prover.Application.Settings;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Services {
	public class ApplicationSettings {
		private static ISettingsService _instance;

		public static LocalSettings Local => Instance.Local;
		public static SharedSettings Shared => Instance.Shared;

		public static ISettingsService Instance
		{
			get {
				if (_instance == null) {
					if (!string.IsNullOrEmpty(SettingsFilePath) && KeyValueStore != null)
						return Initialize(SettingsFilePath, KeyValueStore);

					throw new InvalidOperationException("Call Initialize method before accessing Instance.");
				}

				if (!_instance.HasInitialized)
					_instance.RefreshSettings();

				return _instance;
			}
		}

		public static ISettingsService Initialize(string settingsFilePath,
			IKeyValueStore keyValueStore) {
			SettingsFilePath = settingsFilePath;
			KeyValueStore = keyValueStore;

			if (_instance == null)
				_instance = new SettingsService(settingsFilePath, keyValueStore);

			return _instance;
		}

		public static string SettingsFilePath { get; set; }

		public static IKeyValueStore KeyValueStore { get; set; }
	}

	/// <summary>
	///     Defines the <see cref="SettingsService" />
	/// </summary>
	internal class SettingsService : ISettingsService {
		private readonly IKeyValueStore _keyValueStore;
		private AsyncLock _lock = new AsyncLock();
		/// <summary>
		///     Initializes a new instance of the <see cref="SettingsService" /> class.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="localSettingsPath"></param>
		/// <param name="keyValueStore"></param>
		/// <param name="keyValueRepository"></param>
		internal SettingsService(string localSettingsPath, IKeyValueStore keyValueStore) {
			SettingsPath = localSettingsPath;

			_keyValueStore = keyValueStore;

			Local = new LocalSettings();
			Shared = new SharedSettings();

			HasInitialized = false;
		}

		/// <summary>
		///     Gets a value indicating whether HasInitialized
		/// </summary>
		public bool HasInitialized { get; private set; }

		#region ISettingsService Members


		/// <summary>
		///     Gets the CertificateSettings
		/// </summary>
		public CertificateSettings CertificateSettings => Shared.CertificateSettings;

		/// <summary>
		///     Gets the TestSettings
		/// </summary>
		public TestSettings TestSettings => Shared.TestSettings;

		/// <summary>
		///     Gets the Local
		/// </summary>
		public LocalSettings Local { get; private set; }

		/// <summary>
		///     Gets the Shared
		/// </summary>
		public SharedSettings Shared { get; private set; }

		/// <summary>
		///     The RefreshSettings
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
		public Task RefreshSettings() {
			Task.Run(() => {
				HasInitialized = false;

				//Local = await LoadLocalSettings(SettingsPath);
				Local = _keyValueStore.GetAll<LocalSettings>().FirstOrDefault() ?? new LocalSettings();
				Shared = _keyValueStore.GetAll<SharedSettings>().FirstOrDefault() ?? new SharedSettings();

				HasInitialized = true;
			});
			return Task.CompletedTask;
		}

		/// <summary>
		///     The SaveSettings
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
		public async Task SaveSettings() {
			_keyValueStore.AddOrUpdate(Shared.Key, Shared);
			_keyValueStore.AddOrUpdate(Local.Key, Local);

			await SaveLocalSettings(SettingsPath);

			//await EventAggregator.PublishOnUIThreadAsync(new SettingsChangeEvent());
		}

		public string SettingsPath { get; set; }

		#endregion

		/// <summary>
		///     The LoadLocalSettings
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="filePath">The filePath<see cref="string" /></param>
		/// <returns>The <see cref="System.Threading.Tasks.Task" /></returns>
		private async Task<LocalSettings> LoadLocalSettings(string filePath) {
			var dirPath = Path.GetDirectoryName(filePath);

			if (!File.Exists(filePath) || !Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			using (var sourceStream = File.Open(filePath, FileMode.Open)) {
				var result = new byte[sourceStream.Length];
				await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);

				var jsonText = Encoding.UTF8.GetString(result);

				if (string.IsNullOrEmpty(jsonText))
					return new LocalSettings();

				return JsonSerializer.Deserialize<LocalSettings>(jsonText);
			}
		}


		/// <summary>
		///     The SaveLocalSettings
		/// </summary>
		/// <param name="filePath">The filePath<see cref="string" /></param>
		private async Task SaveLocalSettings(string filePath) {

			using (await _lock.LockAsync()) {
				var dir = Path.GetDirectoryName(filePath);

				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				var jsonText = JsonSerializer.Serialize(Local);

				var result = Encoding.UTF8.GetBytes(jsonText);

				using (var sourceStream = File.Open(filePath, FileMode.Create)) {
					sourceStream.Seek(0, SeekOrigin.Begin);
					await sourceStream.WriteAsync(result, 0, result.Length);
				}
			}


			//FileStream.WriteAllText(path, jsonText);
		}
	}
}