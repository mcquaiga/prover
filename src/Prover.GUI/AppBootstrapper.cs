using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;
using Prover.Core.Startup;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Reports;
using Prover.GUI.Screens.QAProver;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Shell;
using ReactiveUI;
using ReactiveUI.Autofac;
using Splat;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly List<string> _moduleFileNames = new List<string>
        {
            "UnionGas.MASA.dll",
            "Prover.GUI.Common.dll"
        };

        public ContainerBuilder Builder { get; private set; }

        public AppBootstrapper()
        {
            var coreBootstrap = new CoreBootstrapper();
            Builder = coreBootstrap.Builder;

            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();

            var assembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();
            
            //Register Types with Unity
            builder.RegisterType<WindowManager>().As<IWindowManager>();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>();

            builder.RegisterViews(assembly);
            builder.RegisterViewModels(assembly);
            builder.RegisterScreen(assembly);
            
            builder.RegisterType<ScreenManager>();

            //Register Apps
            builder.RegisterType<IAppMainMenu>().Named<QaTestRunApp>("QaTestRunApp");
            GetAppModules();

            RxAppAutofacExtension.UseAutofacDependencyResolver(builder.Build());
        }

        //private void RegisterViewModels()
        //{
        //    _container.RegisterType<MainMenuViewModel>();
        //    _container.RegisterType<SettingsViewModel>();

        //    _container.RegisterType<InstrumentInfoViewModel>();
        //    _container.RegisterType<NewQaTestRunViewModel>();
        //    _container.RegisterType<QaTestRunViewModel>();

        //    
        //    _container.RegisterType<InstrumentReportViewModel>();
        //}

        private void GetAppModules()
        {
            var assembly = Assembly.LoadFrom("UnionGas.MASA.dll");
            var type = assembly.GetType("UnionGas.MASA.Startup");
            type.GetMethod("Initialize").Invoke(null, new object[] {Container});
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
        
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.IsRegistered(serviceType))
                    return Container.Resolve(serviceType);
            }
            else
            {
                if (Container.IsRegisteredWithName(key, serviceType))
                    return Container.ResolveNamed(key, serviceType);
            }

            throw new Exception($"Could not locate any instances of contract {key ?? serviceType.Name}.");
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IViewFor<ShellViewModel>>();

            var screenManager = (ScreenManager)Locator.CurrentMutable.GetService(typeof(ScreenManager));
            Task.Run(() => screenManager.GoHome());
        }
    }
}