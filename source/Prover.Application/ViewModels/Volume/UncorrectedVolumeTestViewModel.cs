using System.Reactive.Disposables;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Prover.Domain;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public class UncorrectedVolumeTestViewModel : VolumeTestRunViewModelBase
    {
        private const decimal Tolerance = Global.UNCOR_ERROR_THRESHOLD;

        public UncorrectedVolumeTestViewModel(IVolumeInputType driveType, VolumeItems startValues,
            VolumeItems endValues) : base(Tolerance, driveType, startValues, endValues)
        {
            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, x => x.AppliedInput, (s, e, a) =>
                    VolumeCalculator.TotalVolume(s.UncorrectedReading, e.UncorrectedReading))
                .ToPropertyEx(this, x => x.ActualValue)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.AppliedInput)
                .Select(a => DriveType.UnCorrectedInputVolume(a))
                .ToPropertyEx(this, x => x.ExpectedValue)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.ExpectedValue)
                .ToPropertyEx(this, x => x.UncorrectedInputVolume)
                .DisposeWith(Cleanup);
        }

        [Reactive] public decimal AppliedInput { get; set; }

        public extern decimal UncorrectedInputVolume { [ObservableAsProperty] get; }


        protected override void Disposing()
        {
           
        }
    }
}