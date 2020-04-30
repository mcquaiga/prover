using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.UI.Desktop.Startup
{
    public interface IConfigureModule
    {
        void Configure(HostBuilderContext builder, IServiceCollection services);
    }
}