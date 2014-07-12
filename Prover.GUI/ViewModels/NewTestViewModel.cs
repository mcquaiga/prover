using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : Screen
    {
        public Instrument Instrument { get; set; }


        public List<String> BaudRates
        {
            get { return Enum.GetNames(typeof(BaudRateEnum)).ToList(); }
        }

        public List<string> CommPorts
        {
            get { return InstrumentCommunication.GetCommPortList(); }
        }
    }
}
