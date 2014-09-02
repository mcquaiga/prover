using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Events
{
    public class ScreenChangeEvent
    {
        public ReactiveScreen ViewModel;
        public ScreenChangeEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }

    }
}
