using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using JsonConfig;
using NLog;
using Prover.Client.Framework.Infrastructure;
using Prover.Shared.Data;
using Prover.Shared.Infrastructure;
using Prover.Shared.Infrastructure.DependencyManagement;
using Prover.Storage.EF;
using Prover.Storage.EF.Repositories;
using Prover.Storage.EF.Storage;
using ReactiveUI;
using ReactiveUI.Autofac;
using IScreen = ReactiveUI.IScreen;
using LogManager = NLog.LogManager;

namespace Prover.Client.Framework
{
    public abstract class ClientBootstrapperBase : BootstrapperBase, IScreen
    {
        protected readonly string ModuleFilePath = $"{Environment.CurrentDirectory}\\modules.json";

        protected ContainerManager ContainerManager;

        protected ClientBootstrapperBase()
        {
            TypeFinder = new AppTypeFinder {EnsureBinFolderAssembliesLoaded = true};

            Initialize();
        }

        protected AppTypeFinder TypeFinder { get; set; }

        protected sealed override void Configure()
        {
            base.Configure();

            var builder = new ContainerBuilder();

            var config = Config.Default;

            //dependencies
            builder.RegisterInstance(TypeFinder).As<ITypeFinder>().SingleInstance();

            builder.RegisterInstance(LogManager.GetCurrentClassLogger()).As<ILogger>();

            if (config.Database != null)
            {
                builder.Register(context => DataContextFactory.CreateWithSqlite(config.Database.ConnectionString))
                    .As<EfProverContext>()
                    .InstancePerLifetimeScope();
                builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            }

            RegisterDependencies(builder, TypeFinder);
            CallAssemblyDependencyRegistrars(config, TypeFinder, builder);

            builder.RegisterForReactiveUI(TypeFinder.GetAssemblies().ToArray());

            var container = builder.Build();
            ContainerManager = new ContainerManager(container);

            //set dependency resolver
            RxAppAutofacExtension.UseAutofacDependencyResolver(container);

            if (config.Database != null)
            {
                var dataProvider = ContainerManager.Resolve<EfProverContext>();
                dataProvider.Database.EnsureCreated();
            }
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return TypeFinder.GetAssemblies();
        }

        protected sealed override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            RunStartupTasks();
            DisplayStartupView();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return ContainerManager.ResolveAll(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return ContainerManager.Resolve(service, key);
        }

        protected override void BuildUp(object instance)
        {
            ContainerManager.Container.InjectProperties(instance);
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder, ITypeFinder typeFinder);

        protected abstract void DisplayStartupView();

        protected virtual void RunStartupTasks()
        {
            var typeFinder = ContainerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask) Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        protected static void CallAssemblyDependencyRegistrars(dynamic config, ITypeFinder typeFinder,
            ContainerBuilder builder)
        {
            //register dependencies provided by other assemblies
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar) Activator.CreateInstance(drType));
            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder, config);
        }

        public RoutingState Router { get; }
    }
}