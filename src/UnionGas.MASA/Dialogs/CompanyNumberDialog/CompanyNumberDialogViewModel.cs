using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;

namespace UnionGas.MASA.Dialogs.CompanyNumberDialog
{
    public class CompanyNumberDialogViewModel : ViewModelBase
    {
        public CompanyNumberDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            CompanyNumber = string.Empty;
        }

        public string CompanyNumber { get; set; }

        public void Cancel()
        {
            TryClose(false);
        }

        public void Close()
        {
            TryClose(true);
        }
    }
}