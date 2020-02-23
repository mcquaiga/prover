using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume
{
    public class UncorrectedVolumeTestViewModel : VolumeTestRunViewModelBase
    {
        private const decimal Tolerance = Global.UNCOR_ERROR_THRESHOLD;

        public UncorrectedVolumeTestViewModel(IVolumeInputType driveType, VolumeItems startValues, VolumeItems endValues) : base(Tolerance, driveType, startValues, endValues)
        {
            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, (s, e) =>
                    VolumeCalculator.TotalVolume(s.UncorrectedReading, e.UncorrectedReading))
                .ToPropertyEx(this, x => x.ActualValue);

            this.WhenAnyValue(x => x.AppliedInput)
                .Select(a => DriveType.UnCorrectedInputVolume(a))
                .ToPropertyEx(this, x => x.ExpectedValue);

            this.WhenAnyValue(x => x.ExpectedValue)
                .ToPropertyEx(this, x => x.UncorrectedInputVolume);
        }

        [Reactive] public decimal AppliedInput { get; set; }

        public extern decimal UncorrectedInputVolume { [ObservableAsProperty] get; }
    }
}