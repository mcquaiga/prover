using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.ViewModels
{
    public class SiteInformationViewModel : ReactiveScreen, IHandle<Instrument>
    {
        private IUnityContainer _container;
        public SiteInformationViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public Instrument Instrument { get; set; }
        public void Handle(Instrument message)
        {
            MessageBox.Show("Hey Adam!");
        }
    }
}
