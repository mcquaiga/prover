using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Modules.Clients.Validators;

namespace Prover.Modules.Clients
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterType<ItemVerificationManager>().As<IValidator>();
        }
    }
}