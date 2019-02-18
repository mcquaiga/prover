using Caliburn.Micro;
using Prover.GUI.Screens;
using System.Collections.Generic;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Dialogs.MeterDTODialog
{
    public class MeterDtoListDialogViewModel : ViewModelBase
    {
        public MeterDtoListDialogViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, IList<MeterDTO> meterDtoList) : base(screenManager, eventAggregator)
        {
            MeterDtoList = meterDtoList;
        }

        public IList<MeterDTO> MeterDtoList { get; }
    }
}
