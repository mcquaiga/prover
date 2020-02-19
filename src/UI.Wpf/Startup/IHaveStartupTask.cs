using System.Threading;
using System.Threading.Tasks;

namespace Client.Wpf.Startup
{
    public interface IHaveStartupTask
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}