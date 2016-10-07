using Caliburn.Micro.ReactiveUI;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA.Dialogs.CompanyNumberDialog
{
    public class CompanyNumberDialogViewModel : ReactiveScreen
    {
        public CompanyNumberDialogViewModel()
        {
            CompanyNumber = string.Empty;
        }

        public string CompanyNumber { get; set; }

        public void Close()
        {
            this.TryClose(true);
        }

        public void Cancel()
        {
            this.TryClose(false);
        }
    }
}
