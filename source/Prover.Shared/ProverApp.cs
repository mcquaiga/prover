using System;
using System.Diagnostics;
using System.Reflection;

namespace Prover.Shared {
	public static class ProverApp {
		public static string GetVersionNumber() {
			return GetFileVersionInfo().FileVersion;
		}

		public static FileVersionInfo GetFileVersionInfo() {
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersionInfo;
		}

		public static Version GetVersion() {
			return Version.Parse(GetFileVersionInfo().FileVersion);
		}
	}
}