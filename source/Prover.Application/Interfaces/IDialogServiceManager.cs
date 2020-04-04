using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IDialogServiceManager
    {
        Task Close();

        Task Show<TView>(TView dialogView, Action onClosed = null) where TView : IViewFor;
        Task Show<T>(Action onClosed = null) where T : class, IDialogViewModel;

        Task ShowMessage(string message, string title);
        Task<bool> ShowQuestion(string question);
        Task<TResult> ShowInputDialog<TResult>(string message, string title = null);
    }
}