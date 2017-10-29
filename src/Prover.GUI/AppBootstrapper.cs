using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Newtonsoft.Json;
using NLog;
using Prover.Core.Startup;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Common.Screens.Toolbar;
using Prover.GUI.Reports;
using Prover.GUI.Screens;
using Prover.GUI.Screens.Shell;
using ReactiveUI;
using ReactiveUI.Autofac;
using LogManager = NLog.LogManager;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly string _moduleFilePath = $"{Environment.CurrentDirectory}\\modules.json";
        private Assembly[] _assemblies;
        private Logger _log = LogManager.GetCurrentClassLogger();
        private readonly StartScreen _splashScreen = new StartScreen();

        public AppBootstrapper()
        {
            try
            {
                _log.Info("Starting EVC Prover Application...");
               
                _splashScreen.Show();                

                var coreBootstrap = new CoreBootstrapper();
                Builder = coreBootstrap.Builder;

                Initialize();

                _log.Info("Finished starting application.");
            }
            catch (Exception e)
            {
                _log.Error("Application failed to load. See exception for more details.");
                _log.Error(e);
            }
        }

        public ContainerBuilder Builder { get; }
        public IContainer Container { get; private set; }

        protected override void Configure()
        {
            base.Configure();

            Builder.Register(c => new WindowManager())
                .As<IWindowManager>()
                .SingleInstance();

            Builder.RegisterType<ShellViewModel>()
                .As<IConductor>()
                //.OnActivated(args =>
                //{
                //    args.Instance.HomeViewModel = args.Context.Resolve<HomeViewModel>();
                //    args.Instance.ToolbarItems = args.Context.Resolve<IEnumerable<IToolbarItem>>();
                //})
                .SingleInstance();

            Builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            Builder.RegisterType<ScreenManager>()
                .SingleInstance();

            Builder.RegisterType<InstrumentReportGenerator>();
            Builder.RegisterAssemblyModules(Assemblies);

            Builder.RegisterViewModels(Assemblies);
            Builder.RegisterViews(Assemblies);
            Builder.RegisterScreen(Assemblies);

            Container = Builder.Build();
            RxAppAutofacExtension.UseAutofacDependencyResolver(Container);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            if (!File.Exists(_moduleFilePath))
            {
                throw new Exception("Could not find a modules.conf file in the current directory.");
            }

            var modules = new List<string>();

            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());

            var modulesString = File.ReadAllText(_moduleFilePath);
            modules.AddRange(JsonConvert.DeserializeObject<List<string>>(modulesString));

            foreach (var module in modules)
            {
                if (File.Exists($"{module}.dll"))
                {
                    var ass = Assembly.LoadFrom($"{module}.dll");
                    assemblies.Add(ass);
                }
            }

            _assemblies = assemblies.ToArray();

            return assemblies;
        }

        public Assembly[] Assemblies => _assemblies ?? (_assemblies = SelectAssemblies().ToArray());

        private void RegisterMainMenuApps(Assembly[] assemblies)
        {
            //register main menu apps
            Builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.GetTypeInfo()
                    .ImplementedInterfaces.Any(i => i == typeof(IHaveMainMenuItem)))
                    .As<IHaveMainMenuItem>();
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

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _splashScreen.Hide();

            DisplayRootViewFor<ShellViewModel>();       

            _splashScreen.Close();
        }
    }
}