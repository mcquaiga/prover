using Caliburn.Micro;
using Prover.GUI.Screens;

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

        public void Close()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}