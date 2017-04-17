using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Modules.Certificates.Exporter;
using Prover.Modules.Certificates.Models;
using Prover.Modules.Certificates.Storage;

namespace Prover.Modules.Certificates
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterType<ExportToCertificateServerManager>().As<IExportTestRun>();
        }
    }
}