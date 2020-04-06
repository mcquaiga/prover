using System.Reactive;
using System.Threading;
using ReactiveUI;

namespace Prover.Application.Interfaces
{

    public enum DialogResult
    {
        Undetermined,
        Cancelled,
        Accepted
    }
    public interface IDialogViewModel
    {
        CancellationToken Cancelled { get; }
        ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
        DialogResult Response { get; }
        //bool IsDialogOpen { get; }
    }
}