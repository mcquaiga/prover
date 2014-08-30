using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.GUI.Interfaces;
using Prover.GUI.Views;
using ReactiveUI;

namespace Prover.GUI.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell
    {

        private readonly IUnityContainer _container;

        public ShellViewModel(IUnityContainer container)
        {
            _container = container;
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            ActivateItem(new MainMenuViewModel(_container));
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
