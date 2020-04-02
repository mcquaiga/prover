using System.Threading;
using System.Threading.Tasks;

namespace Client.Desktop.Wpf.Startup
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}