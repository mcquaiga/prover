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
using Prover.GUI.Reports;
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
            "UnionGas.MASA",
            "Prover.GUI.Common"
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

            //Register Types with Unity
            Builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            Builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            Builder.RegisterType<ScreenManager>().SingleInstance();

            //Register Apps
            Builder.RegisterType<QaTestRunApp>().As<IAppMainMenu>();

            Builder.RegisterType<InstrumentReportGenerator>();

            Container = Builder.Build();
            RxAppAutofacExtension.UseAutofacDependencyResolver(Container);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());

            foreach (var module in _moduleFileNames)
            {
                var ass = Assembly.LoadFrom($"{module}.dll");
                if (ass != null)
                {
                    var type = ass.GetType($"{module}.Startup");
                    type?.GetMethod("Initialize").Invoke(null, new object[] { Builder });
                    assemblies.Add(ass);
                }
            }

            RegisterMainMenuApps(assemblies.ToArray());

            Builder.RegisterViewModels(assemblies.ToArray());
            Builder.RegisterViews(assemblies.ToArray());
            Builder.RegisterScreen(assemblies.ToArray());

            return assemblies;
        }

        private void RegisterMainMenuApps(Assembly[] assemblies)
        {
            //register main menu apps
            Builder.RegisterAssemblyTypes(assemblies.ToArray())
                    .Where(t => t.IsAssignableFrom(typeof(IAppMainMenu)))
                    .AsImplementedInterfaces();
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
            DisplayRootViewFor<ShellViewModel>();

            Task.Run(() => IoC.Get<ScreenManager>().GoHome());
        }
    }
}