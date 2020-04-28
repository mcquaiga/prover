using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Corrections;
using Prover.Calculations;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels.Volume.Mechanical
{
    public class EnergyVolumeTestViewModel : VarianceTestViewModel, IDeviceStartAndEndValues<EnergyItems>
    {
        /// <inheritdoc />
        public EnergyVolumeTestViewModel(EnergyItems startValues, EnergyItems endValues) : base(Tolerances.ENERGY_PASS_TOLERANCE)
        {
            StartValues = startValues;
            EndValues = endValues;
            Units = startValues.EnergyUnitType;

            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, (start, end) => EnergyCalculator.TotalEnergy(start.EnergyReading, end.EnergyReading))
                .ToPropertyEx(this,
                x => x.ActualValue).DisposeWith(Cleanup);

            //this.WhenAnyValue()
            //    .Subscribe(v => EndReading = v.CorrectedReading);
        }

        [Reactive] public EnergyUnitType Units { get; set; }

        /// <inheritdoc />
        [Reactive]
        public EnergyItems StartValues { get; set; }

        /// <inheritdoc />
        [Reactive]
        public EnergyItems EndValues { get; set; }
    }
}