using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.Dialogs
{
    public enum DialogType
    {
        None,
        Question,
        Warning,
        Information,
        Error
    }

    public class Dialog<TResponse>
    {
        public DialogType DialogType { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public IEnumerable<TResponse> PossibleResponses { get; protected set; }
        public TResponse GivenResponse { get; set; }
        public bool IsResponseGiven { get; private set; }
    }

    public class BindableResponse<TResponse>
    {
        public TResponse Response { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }
    }
}
