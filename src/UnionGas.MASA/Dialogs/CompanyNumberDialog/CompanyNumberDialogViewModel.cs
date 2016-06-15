using Caliburn.Micro.ReactiveUI;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA.Dialogs.CompanyNumberDialog
{
    public class CompanyNumberDialogViewModel : ReactiveScreen
    {
        public CompanyNumberDialogViewModel(CompanyNumberVerifier verifier)
        {
            CompanyNumber = string.Empty;
            this.Verifier = verifier;
        }

        public CompanyNumberVerifier Verifier { get; set; }

        public string CompanyNumber { get; set; }

        public void Update()
        {
            Verifier.Update(CompanyNumber);
            this.TryClose(true);
        }

        public void Cancel()
        {
            this.TryClose(false);
        }
    }
}
