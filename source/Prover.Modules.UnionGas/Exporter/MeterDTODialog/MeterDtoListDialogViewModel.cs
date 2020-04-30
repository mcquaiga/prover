using System.Collections.Generic;
using Prover.UI.Desktop.Dialogs;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.Exporter.MeterDTODialog
{
    public class MeterDtoListDialogViewModel : DialogViewModel
    {
        public MeterDtoListDialogViewModel(IList<MeterDTO> meterDtoList)
        {
            MeterDtoList = meterDtoList;
        }

        public IList<MeterDTO> MeterDtoList { get; }
    }
}
