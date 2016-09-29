using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalIntegrations;
using UnionGas.MASA.Verifiers;
using Prover.Core.Settings;
using UnionGas.MASA.Exporter;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<IVerifier, CompanyNumberVerifier>(new InjectionConstructor(container, SettingsManager.SettingsInstance.ExportServiceAddress));            
            container.RegisterType<IExportTestRun, ExportManager>(new InjectionConstructor(container, SettingsManager.SettingsInstance.ExportServiceAddress));
        }
    }
}
