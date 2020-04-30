using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.Shared
{
    public interface IConfigureModule
    {
        void Configure(HostBuilderContext builder, IServiceCollection services);
    }
}