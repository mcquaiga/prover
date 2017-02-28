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

            builder.RegisterType<CertificateContext>();
            builder.RegisterType<CertificateStore>().As<ICertificateStore<Certificate>>();
            builder.RegisterType<CertificateInstrumentStore>().As<ICertificateStore<CertificateInstrument>>();

            builder.RegisterType<ExportToCertificateServerManager>().As<IExportTestRun>();
        }
    }
}