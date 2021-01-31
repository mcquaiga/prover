using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Shared;

namespace Prover.Modules.Certificates.Wpf {
	public class CertificatesBootstrapper : IConfigureModule {
		/// <inheritdoc />
		public void ConfigureServices(HostBuilderContext builder, IServiceCollection services) {

		}

		/// <inheritdoc />
		public void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config) {
			config.AddJsonFile("appsettings.Certificates.json");
		}
	}
}
