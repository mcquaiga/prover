using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public class UncorrectedVolumeTestViewModel : VolumeTestRunViewModelBase,IDeviceStartAndEndValues<VolumeItems>
    {
        private const decimal Tolerance = Tolerances.UNCOR_ERROR_THRESHOLD;

        public UncorrectedVolumeTestViewModel(IVolumeInputType driveType, VolumeItems startValues, VolumeItems endValues) 
            : base(Tolerance, startValues, endValues)
        {
            this.WhenAnyValue(x => x.StartReading, x => x.EndReading, 
                    (start, end) => VolumeCalculator.TotalVolume(start, end, startValues.UncorrectedMultiplier))
                .ToPropertyEx(this, x => x.ActualValue).DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.AppliedInput)
                .Select(driveType.UnCorrectedInputVolume)
                .ToPropertyEx(this, x => x.ExpectedValue).DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.ExpectedValue)
                .ToPropertyEx(this, x => x.UncorrectedInputVolume).DisposeWith(Cleanup);


            this.WhenAnyValue(x => x.StartValues)
                .Subscribe(v => StartReading = v.UncorrectedReading);

            this.WhenAnyValue(x => x.EndValues)
                .Subscribe(v => EndReading = v.UncorrectedReading);

            this.WhenAnyValue(x => x.StartReading)
                .Subscribe(startReading => StartValues.UncorrectedReading = startReading);

            this.WhenAnyValue(x => x.EndReading)
                .Subscribe(endReading => EndValues.UncorrectedReading = endReading);

            Multiplier = startValues.UncorrectedMultiplier;
        }

        [Reactive] public decimal AppliedInput { get; set; }

        public extern decimal UncorrectedInputVolume { [ObservableAsProperty] get; }
        
        protected override void Disposing()
        {
        }
    }
}