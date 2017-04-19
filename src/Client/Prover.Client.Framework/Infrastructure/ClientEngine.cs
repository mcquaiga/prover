//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Autofac;
//using AutoMapper;
//using Microsoft.Extensions.Configuration;
//using Prover.Client.Framework.Configuration;
//using Prover.Client.Framework.Screens.Shell;
//using Prover.Client.Framework.Settings;
//using Prover.Shared.Infrastructure;
//using Prover.Shared.Infrastructure.DependencyManagement;
//using Prover.Shared.Infrastructure.Mapper;
//using ReactiveUI.Autofac;

//namespace Prover.Client.Framework.Infrastructure
//{
//    /// <summary>
//    /// Engine
//    /// </summary>
//    public class ClientEngine : IEngine
//    {
//        #region Fields

//        private ContainerManager _containerManager;

//        #endregion

//        #region Utilities

//        /// <summary>
//        /// Run startup tasks
//        /// </summary>
//        protected virtual void RunStartupTasks()
//        {
//            var typeFinder = _containerManager.Resolve<ITypeFinder>();
//            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
//            var startUpTasks = new List<IStartupTask>();
//            foreach (var startUpTaskType in startUpTaskTypes)
//                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
//            //sort
//            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
//            foreach (var startUpTask in startUpTasks)
//                startUpTask.Execute();
//        }

//        /// <summary>
//        /// Register dependencies
//        /// </summary>
//        /// <param name="config">Config</param>
//        protected virtual void RegisterDependencies(ClientConfig config)
//        {
//            if (config == null) config = new ClientConfig();

//            var builder = new ContainerBuilder();

//            //dependencies
//            var typeFinder = new AppDomainTypeFinder();

//            builder.RegisterInstance(config).As<ClientConfig>().SingleInstance();
//            builder.RegisterInstance(this).As<IEngine<ClientConfig>>().SingleInstance();
//            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

//            var mainMenuItems = typeFinder.FindClassesOfType<IAppMainMenuItem>();
//            builder.RegisterTypes(mainMenuItems.ToArray()).As<IAppMainMenuItem>();

//            CallAssemblyDependencyRegistrars(config, typeFinder, builder);

//            builder.RegisterForReactiveUI(typeFinder.GetAssemblies().ToArray());

//            var container = builder.Build();
//            _containerManager = new ContainerManager(container);

//            //set dependency resolver
//            RxAppAutofacExtension.UseAutofacDependencyResolver(container);            
//        }

//        protected static void CallAssemblyDependencyRegistrars(ClientConfig config, ITypeFinder typeFinder,
//            ContainerBuilder builder)
//        {
//            //register dependencies provided by other assemblies
//            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
//            var drInstances = new List<IDependencyRegistrar>();
//            foreach (var drType in drTypes)
//                drInstances.Add((IDependencyRegistrar) Activator.CreateInstance(drType));
//            //sort
//            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
//            foreach (var dependencyRegistrar in drInstances)
//                dependencyRegistrar.Register(builder, typeFinder, config);
//        }

//        /// <summary>
//        /// Register mapping
//        /// </summary>
//        /// <param name="config">Config</param>
//        protected virtual void RegisterMapperConfiguration(ClientConfig config)
//        {
//            //dependencies
//            var typeFinder = ContainerManager.Resolve<ITypeFinder>();

//            //register mapper configurations provided by other assemblies
//            var mcTypes = typeFinder.FindClassesOfType<IMapperConfiguration>();
//            var mcInstances = new List<IMapperConfiguration>();
//            foreach (var mcType in mcTypes)
//                mcInstances.Add((IMapperConfiguration)Activator.CreateInstance(mcType));
//            //sort
//            mcInstances = mcInstances.AsQueryable().OrderBy(t => t.Order).ToList();
//            //get configurations
//            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
//            foreach (var mc in mcInstances)
//                configurationActions.Add(mc.GetConfiguration());
//            //register
//            AutoMapperConfiguration.Init(configurationActions);
//        }
//        #endregion

//        #region Methods

//        /// <summary>
//        /// Initialize components and plugins in the nop environment.
//        /// </summary>
//        /// <param name="config">Config</param>
//        public void Initialize()
//        {
//            var configBuilder = new ConfigurationBuilder()
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

//            //register dependencies
//            RegisterDependencies(config);

//            //register mapper configurations
//            RegisterMapperConfiguration(config);

//            //startup tasks
//            if (!config.IgnoreStartupTasks)
//            {
//                RunStartupTasks();
//            }

//        }

//        /// <summary>
//        /// Resolve dependency
//        /// </summary>
//        /// <typeparam name="T">T</typeparam>
//        /// <returns></returns>
//        public T Resolve<T>() where T : class
//        {
//            return ContainerManager.Resolve<T>();
//        }

//        /// <summary>
//        ///  Resolve dependency
//        /// </summary>
//        /// <param name="type">Type</param>
//        /// <returns></returns>
//        public object Resolve(Type type)
//        {
//            return ContainerManager.Resolve(type, string.Empty);
//        }

//        /// <summary>
//        /// Resolve dependencies
//        /// </summary>
//        /// <typeparam name="T">T</typeparam>
//        /// <returns></returns>
//        public T[] ResolveAll<T>()
//        {
//            return ContainerManager.ResolveAll<T>();
//        }

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Container manager
//        /// </summary>
//        public virtual ContainerManager ContainerManager
//        {
//            get { return _containerManager; }
//        }

//        #endregion
//    }
//}