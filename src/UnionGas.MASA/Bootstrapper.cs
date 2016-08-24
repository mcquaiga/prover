using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.ExternalIntegrations;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly IUnityContainer _container;

        public Bootstrapper()
        {
            
        }

        public Bootstrapper(IUnityContainer container)
        {
            Initialize();
        }
    }
}
