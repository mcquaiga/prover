using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Prover.Core.Startup;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
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

        public AppBootstrapper()
        {
            var coreBootstrap = new CoreBootstrapper();
            Builder = coreBootstrap.Builder;

            Initialize();
        }

        public ContainerBuilder Builder { get; }
        public IContainer Container { get; private set; }

        protected override void Configure()
        {
            base.Configure();

            var assembly = Assembly.GetExecutingAssembly();

            //Register Types with Unity
            Builder.RegisterType<WindowManager>().As<IWindowManager>();
            Builder.RegisterType<EventAggregator>().As<IEventAggregator>();

            Builder.RegisterViews(assembly);
            Builder.RegisterViewModels(assembly);
            Builder.RegisterScreen(assembly);

            Builder.RegisterType<ScreenManager>();

            //Register Apps
            Builder.RegisterType<IAppMainMenu>().Named<QaTestRunApp>("QaTestRunApp");
            GetAppModules();

            Container = Builder.Build();
            RxAppAutofacExtension.UseAutofacDependencyResolver(Container);
        }


        private void GetAppModules()
        {
            var assembly = Assembly.LoadFrom("UnionGas.MASA.dll");
            var type = assembly.GetType("UnionGas.MASA.Startup");
            type.GetMethod("Initialize").Invoke(null, new object[] {Builder});
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

            var screenManager = (ScreenManager) Locator.CurrentMutable.GetService(typeof(ScreenManager));
            Task.Run(() => screenManager.GoHome());
        }
    }
}