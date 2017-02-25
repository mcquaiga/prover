using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.VerificationTests.TestActions;
using Prover.Modules.Clients.TestActions;

namespace Prover.Modules.Clients
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterType<ItemVerificationManager>().As<PreTestValidationBase>();
            builder.RegisterType<ClientPostTestResetManager>().As<PostTestResetBase>();
        }
    }
}