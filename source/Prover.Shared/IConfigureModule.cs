using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.Shared
{
    public interface IConfigureModule
    {
        void ConfigureServices(HostBuilderContext builder, IServiceCollection services);
        void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config);
    }
}