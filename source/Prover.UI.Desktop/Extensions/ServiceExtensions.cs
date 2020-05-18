using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Prover.Shared;
using Prover.UI.Desktop.Startup;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.UI.Desktop.Extensions
{
	public static class ServiceExtensions
	{
		public static void AddAllTypes<T>(this IServiceCollection services, Assembly[] assemblies = null,
										  ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			assemblies ??= new[] { Assembly.GetCallingAssembly() };

			if (typeof(T).IsInterface)
			{
				var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
				foreach (var type in typesFromAssemblies)
					services.TryAdd(new ServiceDescriptor(typeof(T), type, lifetime));
			}
			else
			{
				var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.BaseType == typeof(T) && !x.IsAbstract));
				foreach (var type in typesFromAssemblies)
					services.TryAdd(new ServiceDescriptor(type, type, lifetime));
			}

		}


		public static void AddMainMenuItems(this IServiceCollection services)
		{
			var assembly = Assembly.GetCallingAssembly();
			var items =
					assembly
						   .DefinedTypes
						   .Where(t => t.ImplementedInterfaces.Contains(typeof(IToolbarItem)) && !t.IsAbstract);

			foreach (var item in items)
				services.AddSingleton(typeof(IToolbarItem), item);
		}

		public static void AddStartTask<T>(this IServiceCollection services) where T : class, IStartupTask
		{
			services.AddSingleton<IStartupTask, T>();
		}

		public static void AddViewsAndViewModels(this IServiceCollection services, IEnumerable<Assembly> ass = null, ServiceLifetime defaultLifetime = ServiceLifetime.Transient)
		{

			var assemblies = ass?.ToList() ?? AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("Prover.")).ToList(); //.GetEntryAssembly().Get().ToList();//.CurrentDomain.GetAssemblies();// new[] { Assembly.GetCallingAssembly() };

			//var assembly = Assembly.GetCallingAssembly();

			AddViews();
			//AddViewModels();

			void AddViews()
			{
				// for each type that implements IViewFor
				foreach (var ti in assemblies.SelectMany(assembly => assembly.DefinedTypes
										   .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract)))
				{
					// grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
					var ivf = ti.ImplementedInterfaces.FirstOrDefault(t =>
																			  t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

					// need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
					if (ivf != null && !ivf.ContainsGenericParameters)
					{
						var lifetime = ti.GetCustomAttribute<SingleInstanceViewAttribute>() != null ? ServiceLifetime.Singleton : defaultLifetime;
						services.TryAdd(new ServiceDescriptor(ivf, ti, lifetime));

						var viewModel = ivf.GenericTypeArguments.FirstOrDefault(t => !t.IsAbstract);
						if (viewModel != null && viewModel.GetInterfaces().Contains(typeof(IRoutableViewModel)))
							services.TryAdd(new ServiceDescriptor(viewModel, viewModel, lifetime));
					}
				}
			}

			void AddViewModels()
			{
				//foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
				//var a = Assembly.Load(ass);
				foreach (var ti in assemblies.SelectMany(assembly => assembly.DefinedTypes
																			 .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IRoutableViewModel)) && !ti.IsAbstract)))
					if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
					{
						services.TryAddScoped(ti, ti);
						services.TryAddScoped(typeof(IRoutableViewModel), ti);
					}

				//services.AddAllTypes<ReactiveObject>(assemblies.ToArray(), ServiceLifetime.Scoped);
				services.AddAllTypes<ViewModelBase>(assemblies.ToArray(), ServiceLifetime.Scoped);

				//foreach (var ti in assemblies.SelectMany(assembly => assembly.DefinedTypes
				//															 .Where(ti => ti.IsTypeOrInheritsOf(typeof(ReactiveObject)))))
				//{
				//	if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
				//	{
				//		services.TryAddScoped(ti, ti);
				//		services.TryAddScoped(typeof(ReactiveObject), ti);
				//	}
				//}
			}
		}


		public static void DiscoverModules(this HostBuilderContext host)
		{
			var names = host.Configuration.GetSection("Modules")?.GetChildren().Select(c => c.Value).ToList();
			var modules = LoadConfigModules(names);

			host.Properties.Add("Modules", modules);
		}

		public static ICollection<IConfigureModule> LoadConfigModules(ICollection<string> moduleNames)
		{
			var results = new List<IConfigureModule>();
			var basePath = Directory.GetParent(Assembly.GetEntryAssembly()?.Location).FullName;
			foreach (var module in moduleNames)
			{
				try
				{
					var ass = Assembly.LoadFile(Path.Combine(basePath, module));

					var moduleConfig =
							ass.GetExportedTypes().FirstOrDefault(t =>
									t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConfigureModule)));

					if (moduleConfig != null)
					{
						var instance = Activator.CreateInstance(moduleConfig);

						results.Add(instance as IConfigureModule);

						//moduleConfig
						//        .GetMethod(nameof(IConfigureModule.ConfigureServices))
						//        ?.Invoke(instance, new object[] { builder, services });
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"An error occured loading module {module}. {Environment.NewLine} Exception: {ex.Message}.");
				}
			}

			return results;
		}

		public static void AddModuleConfigurations(this HostBuilderContext builder, IConfigurationBuilder config)
		{
			if (builder.Properties.TryGetValue("Modules", out var property))
			{
				if (property is List<IConfigureModule> modules)
				{
					modules.ForEach(m => m.ConfigureAppConfiguration(builder, config));
				}
			}
		}

		public static void AddModuleServices(this HostBuilderContext builder, IServiceCollection services)
		{
			if (builder.Properties.TryGetValue("Modules", out var property))
			{
				if (property is List<IConfigureModule> modules)
				{
					modules.ForEach(m => m.ConfigureServices(builder, services));
				}
			}

			//builder.DiscoverModules();
			//if (!builder.Properties.TryGetValue("Modules", out var modules) || string.IsNullOrEmpty(basePath))
			//    return;

			//foreach (var module in ((List<string>)modules))
			//{
			//    try
			//    {
			//        var ass = Assembly.LoadFile(Path.Combine(basePath, module));

			//        var moduleConfig =
			//                ass.GetExportedTypes().FirstOrDefault(t =>
			//                        t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConfigureModule)));

			//        if (moduleConfig != null)
			//        {
			//            var instance = Activator.CreateInstance(moduleConfig);

			//            moduleConfig
			//                   .GetMethod(nameof(IConfigureModule.ConfigureServices))
			//                  ?.Invoke(instance, new object[] { builder, services });
			//        }
			//    }
			//    catch (Exception ex)
			//    {
			//        Debug.WriteLine($"An error occured loading module {module}. {Environment.NewLine} Exception: {ex.Message}.");
			//    }
			//}



			//return builder;


		}

		private static void AddReactiveObjects(IServiceCollection services, Assembly assembly)
		{
			foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
				foreach (var ti in ass.DefinedTypes
									  .Where(ti => ti.BaseType == typeof(ReactiveObject) && !ti.IsAbstract))

					if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
						services.TryAddTransient(ti, ti);
		}

		private static void RegisterType(IServiceCollection services, TypeInfo ti, Type serviceType)
		{
			var factory = TypeFactory(ti);
			if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() != null)
				services.TryAddSingleton(serviceType, factory);
			else
				services.TryAddTransient(serviceType, factory);
		}

		[SuppressMessage("Redundancy", "CA1801: Redundant parameter", Justification = "Used on some platforms")]
		private static Func<IServiceProvider, object> TypeFactory(TypeInfo typeInfo)
		{
			var exp = Expression.Lambda<Func<object>>(Expression.New(
															  typeInfo.DeclaredConstructors.First(ci => ci.IsPublic && !ci.GetParameters().Any()))).Compile();

			return provider => exp;
			//return Expression.Lambda<Func<IServiceProvider, object>>
			//(Expression.New(
			//    typeInfo.DeclaredConstructors.First(ci => ci.IsPublic && !ci.GetParameters().Any()))).Compile();
		}
	}
}