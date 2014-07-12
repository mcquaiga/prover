using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.GUI.Interfaces;
using Prover.GUI.Views;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell
    {
        public ShellViewModel()
        {
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            ActivateItem(new MainMenuViewModel());
        }

        public void HomeButton()
        {
            ShowMainMenu();
        }

        public string Title
        {
            get { return "Prover"; }
        }
    }
}
