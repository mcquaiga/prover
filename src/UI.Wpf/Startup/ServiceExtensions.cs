using System.Linq;
using System.Reflection;
using Client.Wpf.Screens;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Client.Wpf.Startup
{
    public static class ServiceExtensions
    {
        public static void AddViewsAndViewModels(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
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


            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IRoutableViewModel)) && !ti.IsAbstract))
                if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
                {
                    services.AddTransient(ti, ti);
                    services.AddTransient(typeof(IRoutableViewModel), ti);
                }

            // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
        }

        public static void AddMainMenuItems(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IMainMenuItem)) && !ti.IsAbstract)
                .ToList()
                .ForEach(ti => services.AddSingleton(typeof(IMainMenuItem), ti));
        }
    }
}