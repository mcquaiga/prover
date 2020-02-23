using System.Reactive.Linq;
using Application.Interfaces;
using Application.ViewModels.Corrections;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume
{
    public class CorrectedVolumeTestViewModel : VolumeTestRunViewModelBase
    {
        private const decimal Tolerance = Global.COR_ERROR_THRESHOLD;

        public CorrectedVolumeTestViewModel(IVolumeInputType driveType, UncorrectedVolumeTestViewModel uncorrected,
            ICalculateTrueCorrectedFactor trueCorrectedFactor, VolumeItems startValues, VolumeItems endValues)
            : base(Tolerance, driveType, startValues, endValues)
        {
            Uncorrected = uncorrected;
            this.WhenAnyValue(x => x.StartValues, x => x.EndValues,
                    (s, e) => VolumeCalculator.TotalVolume(s.CorrectedReading, e.CorrectedReading))
                .ToPropertyEx(this, x => x.ActualValue);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume).CombineLatest(this.WhenAnyValue(x => x.TotalCorrectionFactor),
                    (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue);

            trueCorrectedFactor.TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor);
        }

        public UncorrectedVolumeTestViewModel Uncorrected { get; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }

    public class PulseOutputTestViewModel : VarianceTestViewModel
    {
    }
}