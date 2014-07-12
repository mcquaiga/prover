using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Startup;
using Prover.GUI.ViewModels;
using Prover.GUI.Views;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private UnityContainer _container;
        public AppBootstrapper()
        {
            Initialize();

            //Start Prover.Core
            var coreBootstrap = new CoreBootstrapper();
            _container = coreBootstrap.Container;

        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
