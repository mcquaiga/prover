using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Startup;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Screens.QAProver;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Shell;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IUnityContainer _container;

        private readonly List<string> _moduleFileNames = new List<string>()
        {
            "UnionGas.MASA.dll",
            "Prover.GUI.Common.dll"
        }; 

        public AppBootstrapper()
        {
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

            _container.RegisterType<ScreenManager>(new ContainerControlledLifetimeManager());

            //Register Apps
            _container.RegisterType<IAppMainMenu, QaTestRunApp>("QaTestRunApp", new ContainerControlledLifetimeManager());

            RegisterViewModels();
            GetAppModules();
        }

        private void RegisterViewModels()
        {
            _container.RegisterType<MainMenuViewModel>();
            _container.RegisterType<SettingsViewModel>();

            _container.RegisterType<InstrumentInfoViewModel>();
            _container.RegisterType<NewQaTestRunViewModel>();
            _container.RegisterType<QaTestRunViewModel>();
        }

        private void GetAppModules()
        {
            foreach(var ass in AssemblySource.Instance.Where(x => x.FullName.Contains("UnionGas.MASA")))
            {
                var type = ass.GetType("UnionGas.MASA.Startup");
                type.GetMethod("Initialize").Invoke(null, new object[] { _container });
            }
            
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

            var fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory());

            foreach (var fileName in fileEntries.Where(x => _moduleFileNames.Any(y => x.EndsWith(y))))
            {
                var ass = Assembly.LoadFrom(fileName);
                assemblies.Add(ass);
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

            var screenManager = _container.Resolve<ScreenManager>();
            Task.Run(() => screenManager.GoHome());
        }
    }
}