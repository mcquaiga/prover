using System.Collections.Generic;
using System.Linq;
using Prover.GUI.Common.Screens;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;

namespace Prover.Modules.Clients.Screens.Clients.Designer
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
