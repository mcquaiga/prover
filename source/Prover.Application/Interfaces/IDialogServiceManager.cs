using System.Threading.Tasks;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IDialogServiceManager
    {
        IViewFor DialogContent { get; }

        Task Close();
        Task Show<TView>(TView dialogView) where TView : IViewFor;

        Task Show<T>()
            where T : class, IDialogViewModel;

        Task ShowMessage(string message, string title);
        Task<bool> ShowQuestion(string question);
    }
}