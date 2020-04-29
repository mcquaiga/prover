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
        public EnergyVolumeTestViewModel(EnergyItems startValues, EnergyItems endValues, CorrectedVolumeTestViewModel correctedVolume) : base(Tolerances.ENERGY_PASS_TOLERANCE)
        {
            StartValues = startValues;
            EndValues = endValues;
            CorrectedVolume = correctedVolume;
            Units = startValues.EnergyUnitType;

            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, (start, end) => EnergyCalculator.TotalEnergy(start.EnergyReading, end.EnergyReading))
                .ToPropertyEx(this, x => x.ActualValue, deferSubscription: true, scheduler: RxApp.MainThreadScheduler)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.CorrectedVolume.ExpectedValue, x => x.EndValues, (corrected, end) => EnergyCalculator.Calculated(StartValues.EnergyUnitType, corrected, end.EnergyGasValue))
                .ToPropertyEx(this, x => x.ExpectedValue, deferSubscription: true, scheduler: RxApp.MainThreadScheduler).DisposeWith(Cleanup);

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

        [Reactive] public CorrectedVolumeTestViewModel CorrectedVolume { get; set; }
    }
}