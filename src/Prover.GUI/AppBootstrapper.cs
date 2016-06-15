using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Startup;
using Prover.GUI.Screens.Shell;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IUnityContainer _container;

        public AppBootstrapper()
        {
            //Start Prover.Core
            var coreBootstrap = new CoreBootstrapper();
            _container = coreBootstrap.Container;

            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();

            //Register Types with Unity
            _container.RegisterType<IWindowManager, WindowManager>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());


            var ass = AssemblySource.Instance.FirstOrDefault(x => x.FullName.Contains("UnionGas.MASA"));
            var type = ass.GetType("UnionGas.MASA.Startup");
            type.GetMethod("Initialize").Invoke(null, new object[] {_container});
            //AssemblySource.Instance.Add(ass);
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

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());
            //Load new ViewModels here
            var fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory());

            foreach (var fileName in fileEntries)
            {
                if (fileName.EndsWith("UnionGas.MASA.dll"))
                {
                    var ass = Assembly.LoadFrom(fileName);
                    assemblies.Add(ass);
                }
            }

            return assemblies;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}