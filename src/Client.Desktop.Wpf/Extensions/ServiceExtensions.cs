using System;
using System.Linq;
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
                services.AddTransient(ti, ti);
        }

        public static void AddMainMenuItems(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            var items = assembly.DefinedTypes.FirstOrDefault(t => t == typeof(MainMenuItems));

            if (items != null)
                items.DeclaredMethods
                    .ToList()
                    .ForEach(m =>
                    {
                        services.AddSingleton(typeof(IMainMenuItem),
                            c => m.Invoke(null, new[] {c.GetService<IScreenManager>()}));
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

            AddViews(services, assembly);

            AddViewModels(services, assembly);

            //AddReactiveObjects(services, assembly);
        }

        public static void ConfigureModules(this HostBuilderContext builder, IServiceCollection services)
        {
            return;
            var modules = builder.Configuration.GetSection("Modules")?.GetChildren();

            if (modules != null)
                foreach (var mod in modules)
                {
                    var ass = Assembly.LoadFrom(mod.Value);

                    var moduleConfig =
                        ass.GetTypes().FirstOrDefault(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConfigureModule)));

                    if (moduleConfig != null)
                    {
                        var instance = Activator.CreateInstance(moduleConfig);

                        moduleConfig
                            .GetMethod(nameof(IConfigureModule.Configure))?.Invoke(instance, new object[] {builder, services});
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

        private static void AddViewModels(IServiceCollection services, Assembly assembly)
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                //var a = Assembly.Load(ass);
            foreach (var ti in ass.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IRoutableViewModel)) && !ti.IsAbstract))
                if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
                {
                    services.AddTransient(ti, ti);
                    services.AddTransient(typeof(IRoutableViewModel), ti);
                }
        }

        private static void AddViews(IServiceCollection services, Assembly assembly)
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
                    services.AddTransient(ivf, ti);
            }
        }
    }
}