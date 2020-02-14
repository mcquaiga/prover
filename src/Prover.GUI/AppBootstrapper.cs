using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Prover.Core;
using Prover.Core.Settings;
using Prover.GUI.Reports;
using Prover.GUI.Screens;
using Prover.GUI.Screens.Shell;
using ReactiveUI.Autofac;
using LogManager = NLog.LogManager;
using StartScreen = Prover.GUI.Screens.Startup.StartScreen;

namespace Prover.GUI
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly string _moduleFilePath = $"{Environment.CurrentDirectory}\\modules.json";
        private Assembly[] _assemblies;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly StartScreen _splashScreen = new StartScreen();

        public AppBootstrapper()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _log.Info("Starting EVC Prover Application.");

                _splashScreen.Show();

                Initialize();

                _log.Info($"Finished starting application in {sw.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                _log.Error(ex + Environment.NewLine + "Shutting down unexpectedly.");
                MessageBox.Show(ex.Message + Environment.NewLine + "Shutting down unexpectedly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            finally
            {
                sw.Stop();
            }
        }

        public ContainerBuilder Builder { get; private set; }
        public IContainer Container { get; private set; }

        protected override void Configure()
        {
            try
            {
                base.Configure();

                Builder = new ContainerBuilder();
                CoreBootstrapper.RegisterServices(Builder);

                Builder.RegisterType<WindowManager>()
                    .As<IWindowManager>()
                    .SingleInstance();

                Builder.RegisterType<ShellViewModel>()
                    .As<IConductor>()
                    .SingleInstance();

                Builder.RegisterType<EventAggregator>()
                    .As<IEventAggregator>()
                    .SingleInstance();

                Builder.RegisterType<ScreenManager>()
                    .SingleInstance();

                Builder.RegisterType<InstrumentReportGenerator>()
                    .SingleInstance();
                Builder.RegisterAssemblyModules(Assemblies);

                Builder.RegisterViewModels(Assemblies);
                Builder.RegisterViews(Assemblies);
                Builder.RegisterScreen(Assemblies);

                Container = Builder.Build();
               
                RxAppAutofacExtension.UseAutofacDependencyResolver(Container);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(base.SelectAssemblies());

            if (_moduleFilePath == null || !File.Exists(_moduleFilePath))
                throw new Exception("Could not find a modules.json in the current directory.");

            var modulesString = File.ReadAllText(_moduleFilePath);
            var mods = (JsonConvert.DeserializeObject(modulesString) as JObject)?["modules"];

            if (mods == null)
                throw new Exception("Incorrectly formatted modules.json.");

            assemblies.AddRange(from module in mods.Children()
                                where File.Exists($"{module}.dll")
                                select Assembly.LoadFrom($"{module}.dll"));

            _assemblies = assemblies.ToArray();

            return assemblies;
        }

        public Assembly[] Assemblies => _assemblies ?? (_assemblies = SelectAssemblies().ToArray());

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
            
            throw new Exception($"Could not locate instance for type {serviceType} with key {key}.");
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                _splashScreen.Hide();
                DisplayRootViewFor<ShellViewModel>();

                Container.Resolve<IEnumerable<IDisplayOnStartup>>().ToObservable()
                    .ForEachAsync(d => d.Show());
                
                _splashScreen.Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show(ex.Message + Environment.NewLine + "Shutting down application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Container.Resolve<ISettingsService>().SaveSettings();
            base.OnExit(sender, e);
        }
    }
}