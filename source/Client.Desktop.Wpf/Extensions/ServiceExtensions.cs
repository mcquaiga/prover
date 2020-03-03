using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ReactiveUI;

namespace Client.Desktop.Wpf.Extensions
{
    public static class ServiceExtensions
    {
        private static Assembly GetAssembly => Assembly.GetCallingAssembly();

        public static void AddDialogViews(this IServiceCollection services)
        {
            foreach (var ti in GetAssembly.DefinedTypes.Where(t => t.Name.Contains("Dialog"))
                .Where(ti =>
                    ti.BaseType?.IsGenericType == true &&
                    ti.BaseType?.GetGenericTypeDefinition() == typeof(ReactiveDialog<>).GetGenericTypeDefinition() &&
                    !ti.IsAbstract))
                services.TryAddTransient(ti, ti);
        }

        public static void AddMainMenuItems(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            var items = assembly.DefinedTypes.FirstOrDefault(t => t == typeof(MainMenuItems));

            if (items != null)
                items.DeclaredProperties
                    .ToList()
                    .ForEach(prop =>
                    {
                        services.AddSingleton(typeof(IMainMenuItem), c => prop.GetValue(items));
                    });

            //assembly.DefinedTypes
            //    .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IMainMenuItem)) && !ti.IsAbstract)
            //    .ToList()
            //    .ForEach(ti => services.AddSingleton(typeof(IMainMenuItem), ti));
        }

        public static void AddStartTask<T>(this IServiceCollection services) where T : class, IHaveStartupTask
        {
            services.AddSingleton<IHaveStartupTask, T>();
        }

        public static void AddViewsAndViewModels(this IServiceCollection services)
        {
            var assembly = GetAssembly;

            AddViews();
            AddViewModels();

            //AddReactiveObjects(services, assembly);

            void AddViews()
            {
                // for each type that implements IViewFor
                foreach (var ti in assembly.DefinedTypes
                    .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
                {
                    // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
                    var ivf = ti.ImplementedInterfaces.FirstOrDefault(t =>
                        t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));


                    // need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
                    if (ivf != null && !ivf.ContainsGenericParameters)
                    {
                        //RegisterType(services, ti, ivf);
                        if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() != null)
                        {
                            services.TryAddSingleton(ivf, ti);
                            //RegisterType(services, ti, ivf);
                        }
                        else
                        {
                            services.TryAddTransient(ivf, ti);
                        }
                    }
                }
            }

            void AddViewModels()
            {
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                    //var a = Assembly.Load(ass);
                foreach (var ti in ass.DefinedTypes
                    .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IRoutableViewModel)) && !ti.IsAbstract))
                    if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
                    {
                        services.TryAddTransient(ti, ti);
                        services.TryAddTransient(typeof(IRoutableViewModel), ti);
                    }
            }
        }

        public static void ConfigureModules(this HostBuilderContext builder, IServiceCollection services)
        {
            var modules = builder.Configuration.GetSection("Modules")?.GetChildren();

            if (modules != null)
                foreach (var mod in modules)
                {
                    var ass = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod.Value));

                    var moduleConfig =
                        ass.GetExportedTypes().FirstOrDefault(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConfigureModule)));

                    if (moduleConfig != null)
                    {
                        var instance = Activator.CreateInstance(moduleConfig);

                        moduleConfig
                            .GetMethod(nameof(IConfigureModule.Configure))?.Invoke(instance, new object[] { builder, services });
                    }
                }
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
            {
                services.TryAddSingleton(serviceType, factory);
            }
            else
            {
                services.TryAddTransient(serviceType, factory);
            }
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