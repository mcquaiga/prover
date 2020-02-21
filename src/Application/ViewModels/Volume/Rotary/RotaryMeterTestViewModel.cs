using System.Reactive.Linq;
using Application.ViewModels.Corrections;
using Devices.Core.Items.DriveTypes;
using Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume.Rotary
{
    public sealed class RotaryMeterTestViewModel : ItemVarianceTestViewModel<RotaryMeterItems>
    {

        public RotaryMeterTestViewModel(RotaryMeterItems rotaryItems) : base(rotaryItems, Global.METER_DIS_ERROR_THRESHOLD)
        {
            Items = rotaryItems;

            this.WhenAnyValue(x => x.Items)
                .Where(x => x != null)
                .Select(x => x.MeterDisplacement != 0 ? x.MeterDisplacement : x.MeterType.MeterDisplacement ?? 0 )
                .ToPropertyEx(this, x => x.ActualValue, 0);

            this.WhenAnyValue(x => x.Items)
                .Where(x => x != null)
                .Select(x => x.MeterType.MeterDisplacement ?? 0)
                .ToPropertyEx(this, x => x.ExpectedValue, 0);
        }

    }
}