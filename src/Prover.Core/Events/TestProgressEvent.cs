using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;

namespace Prover.Core.Events
{
    public class TestProgressEvent
    {      
        public TestProgressEvent(ReactiveScreen activeViewModel)
        {
            this.ActiveViewModel = activeViewModel;
        }

        public ReactiveScreen ActiveViewModel { get; set; }
    }
}
