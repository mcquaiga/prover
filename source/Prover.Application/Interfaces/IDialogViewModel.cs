using System.Reactive;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IDialogViewModel
    {
        ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
        //bool IsDialogOpen { get; }
    }
}