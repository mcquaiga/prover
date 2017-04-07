using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Newtonsoft.Json;
using Prover.Core.Startup;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Reports;
using Prover.GUI.Screens.Shell;
using ReactiveUI.Autofac;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly string _moduleFilePath = $"{Environment.CurrentDirectory}\\modules.json";

        public AppBootstrapper()
        {
            var coreBootstrap = new CoreBootstrapper();
            Builder = coreBootstrap.Builder;

            Initialize();
        }

        public ContainerBuilder Builder { get; }
        public IContainer Container { get; private set; }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override void Configure()
        {
            base.Configure();

            //Register Types with Unity
            Builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            Builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            Builder.RegisterType<ScreenManager>().SingleInstance();

            Builder.RegisterType<InstrumentReportGenerator>();

            Container = Builder.Build();
            RxAppAutofacExtension.UseAutofacDependencyResolver(Container);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
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

            return null;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();

            Task.Run(() => IoC.Get<ScreenManager>().GoHome());
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            if (!File.Exists(_moduleFilePath))
                throw new Exception("Could not find a modules.conf file in the current directory.");

            var modules = new List<string>();

            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());

            var modulesString = File.ReadAllText(_moduleFilePath);
            modules.AddRange(JsonConvert.DeserializeObject<List<string>>(modulesString));

            foreach (var module in modules)
                if (File.Exists($"{module}.dll"))
                {
                    var ass = Assembly.LoadFrom($"{module}.dll");
                    if (ass != null)
                    {
                        var type = ass.GetType($"{module}.Startup");
                        type?.GetMethod("Initialize").Invoke(null, new object[] {Builder});
                        assemblies.Add(ass);
                    }
                }

            RegisterMainMenuApps(assemblies.ToArray());
            //RegisterToolbarItems(assemblies.ToArray());

            Builder.RegisterViewModels(assemblies.ToArray());
            Builder.RegisterViews(assemblies.ToArray());
            Builder.RegisterScreen(assemblies.ToArray());

            return assemblies;
        }

        private void RegisterMainMenuApps(Assembly[] assemblies)
        {
            //register main menu apps
            Builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        i => i == typeof(IAppMainMenu)))
                .As<IAppMainMenu>();
        }
    }
}