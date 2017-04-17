using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;

namespace UnionGas.MASA.Dialogs.LoginDialog
{
    public class LoginDialogViewModel : ViewModelBase
    {
        public LoginDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            EmployeeId = string.Empty;
        }

        public string EmployeeId { get; set; }

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