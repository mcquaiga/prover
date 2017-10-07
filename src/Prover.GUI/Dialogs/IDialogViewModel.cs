using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
