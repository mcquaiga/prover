using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Startup;
using Prover.GUI.ViewModels;
using Prover.GUI.Views;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly IUnityContainer _container;

        public AppBootstrapper()
        {
            Initialize();

            //Start Prover.Core
            var coreBootstrap = new CoreBootstrapper();
            _container = coreBootstrap.Container;

            //Register Types with Unity
            _container.RegisterType<IWindowManager, WindowManager>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEventAggregator, EventAggregator>();

        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return _container.Resolve(service);
            }
            
            if (!string.IsNullOrEmpty(key))
            {
                return _container.Resolve(Type.GetType(key));
            }

            return null;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
