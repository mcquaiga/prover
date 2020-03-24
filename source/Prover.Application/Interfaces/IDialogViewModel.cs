using System.Reactive;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IDialogViewModel
    {
        //ReactiveCommand<Unit, bool> CancelCommand { get; set; }
        //ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        bool IsDialogOpen { get; }
    }
}