using Caliburn.Micro;

namespace Prover.GUI.Dialogs
{
    public interface IDialogViewModel<TResponse>
    {
        bool IsClosed { get; set; }
        Dialog<TResponse> Dialog { get; set; }
        IObservableCollection<BindableResponse<TResponse>> Responses { get; }
        void Respond(BindableResponse<TResponse> bindableResponse);
    }
}