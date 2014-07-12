using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Prover.GUI.ViewModels
{
    public class MainMenuViewModel : Conductor<object>.Collection.OneActive
    {
        public void NewTestButton()
        {
            ActivateItem(new NewTestViewModel());
        }

        public void CreateCertificateButton()
        {
            
        }
    }
}
