using Microsoft.Practices.Unity;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Settings;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<DCRWebServiceSoap, DCRWebServiceSoapClient>();

            container.RegisterType<VerifierUpdaterBase, CompanyNumberVerifierUpdater>();
            container.RegisterType<IExportTestRun, ExportManager>(new InjectionConstructor(container, SettingsManager.SettingsInstance.ExportServiceAddress));
        }
    }
}