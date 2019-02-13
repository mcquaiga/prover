using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
