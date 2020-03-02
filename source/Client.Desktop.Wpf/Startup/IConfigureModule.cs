using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Desktop.Wpf.Startup
{
    public interface IConfigureModule
    {
        void Configure(HostBuilderContext builder, IServiceCollection services);
    }
}