using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.ExternalIntegrations;
using UnionGas.MASA.Verifiers;
using Prover.Core.Settings;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<IVerifier, CompanyNumberVerifier>();
            container.RegisterType<IExportTestRun, ExportManager>(new InjectionConstructor(SettingsManager.SettingsInstance.ExportServiceAddress));
        }
    }
}
