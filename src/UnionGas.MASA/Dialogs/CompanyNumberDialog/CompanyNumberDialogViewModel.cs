using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA.Dialogs.CompanyNumberDialog
{
    public class CompanyNumberDialogViewModel : ViewModelBase
    {
        public CompanyNumberDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
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
