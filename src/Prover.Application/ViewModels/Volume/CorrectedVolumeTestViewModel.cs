using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
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
                    (start, end) => VolumeCalculator.TotalVolume(start.CorrectedReading, end.CorrectedReading))
                .ToPropertyEx(this, x => x.ActualValue);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume).CombineLatest(
                    this.WhenAnyValue(x => x.TotalCorrectionFactor),
                    (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue);

            trueCorrectedFactor.TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor);
        }

        public UncorrectedVolumeTestViewModel Uncorrected { get; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }

        [Reactive] public PulseOutputTestViewModel PulseOutput { get; set; }
    }

    public class PulseOutputTestViewModel : DeviationTestViewModel<PulseOutputItems.ChannelItems>
    {
        private readonly VolumeTestRunViewModelBase _volumeTest;

        public PulseOutputTestViewModel(PulseOutputItems.ChannelItems pulseChannelItems,
            VolumeTestRunViewModelBase volumeTest) : base(Global.PULSE_VARIANCE_THRESHOLD)
        {
            _volumeTest = volumeTest;
            Items = pulseChannelItems;

            this.WhenAnyValue(x => x._volumeTest.ActualValue)
                .Select(x => x.ToInt32())
                .ToPropertyEx(this, x => x.ActualValue);
        }
    }
}