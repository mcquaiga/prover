using Autofac;
using System.Reflection;
using Prover.Core.ExternalIntegrations;
using Prover.Modules.Certificates.Exporter;

namespace Prover.ExternalIntegrations.CRWall
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