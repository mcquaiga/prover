using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.GUI.Common.Screens;

namespace CrWall.Screens.Clients.Designer
{
    public class DesignTimeClientViewModel
    {
        public IEnumerable<SelectableViewModel<InstrumentType>> InstrumentTypes
        {
            get
            {
                var instruments = new List<SelectableViewModel<InstrumentType>>();
                Instruments.GetAll().ToList().ForEach(x => instruments.Add(new SelectableViewModel<InstrumentType>(x)));
                return instruments;
            }
        }
    }
}
