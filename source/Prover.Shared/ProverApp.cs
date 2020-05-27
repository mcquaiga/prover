using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.Shared {
	public static class ProverApp {
		static ProverApp() {
			VersionNumber = ProverApp.GetVersion().ToString(3);
		}

		public static void Initialize(IHostEnvironment host) {
			AppEnvironment = host.EnvironmentName;
		}

		public static void Initialize(IServiceProvider provider) {
			Initialize(provider.GetService<IHostEnvironment>());
		}

		public static string GetVersionNumber() {
			return GetFileVersionInfo().FileVersion;
		}

		public static FileVersionInfo GetFileVersionInfo() {
			var assembly = Assembly.GetEntryAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersionInfo;
		}

		public static Version GetVersion() {
			return Version.Parse(GetFileVersionInfo().FileVersion);
		}

		public static string Title => "EVC Prover" + $" - v{VersionNumber}" + $" - {AppEnvironment}";
		public static string VersionNumber { get; private set; }//=> ProverApp.GetVersionNumber();
		public static string AppEnvironment { get; set; }
	}
}