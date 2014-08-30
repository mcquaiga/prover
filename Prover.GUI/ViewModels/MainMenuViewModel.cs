using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;

namespace Prover.GUI.ViewModels
{
    public class MainMenuViewModel : Conductor<object>.Collection.OneActive
    {
        private readonly IUnityContainer _container;

        public MainMenuViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public void NewTestButton()
        {
            ActivateItem(new NewTestViewModel(_container));
        }

        public void CreateCertificateButton()
        {
            
        }
    }
}
