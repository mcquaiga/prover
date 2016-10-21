using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Login;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Dialogs.LoginDialog
{
    public class LoginDialogViewModel : ViewModelBase
    {
        public string EmployeeId { get; set; }

        public LoginDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            EmployeeId = string.Empty;
        }

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
