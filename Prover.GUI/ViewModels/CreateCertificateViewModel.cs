using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;

namespace Prover.GUI.ViewModels
{
    public class CreateCertificateViewModel: ReactiveScreen
    {
        private IUnityContainer _container;
        public CreateCertificateViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public InstrumentsListViewModel InstrumentsListViewModel
        {
            get { return new InstrumentsListViewModel(_container); }
        }
    }
}
