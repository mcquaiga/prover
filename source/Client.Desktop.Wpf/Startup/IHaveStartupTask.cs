using System.Threading;
using System.Threading.Tasks;

namespace Client.Desktop.Wpf.Startup
{
    public interface IHaveStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}