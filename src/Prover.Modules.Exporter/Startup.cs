using System.Reflection;
using Autofac;

namespace Prover.Modules.Exporter
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
        }
    }
}
