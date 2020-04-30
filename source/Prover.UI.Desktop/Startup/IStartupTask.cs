using System.Threading;
using System.Threading.Tasks;

namespace Prover.UI.Desktop.Startup
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}