using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Wpf.Startup
{
    public interface IConfigureModule
    {
        void Configure(HostBuilderContext builder, IServiceCollection services);
    }
}