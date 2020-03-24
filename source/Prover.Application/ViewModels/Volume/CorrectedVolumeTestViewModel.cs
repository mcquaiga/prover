using System.Reactive.Disposables;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Interfaces;
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
                .ToPropertyEx(this, x => x.ActualValue)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume).CombineLatest(
                    this.WhenAnyValue(x => x.TotalCorrectionFactor),
                    (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue)
                .DisposeWith(Cleanup);

            trueCorrectedFactor.TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor)
                .DisposeWith(Cleanup);
        }

        public UncorrectedVolumeTestViewModel Uncorrected { get; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }

        
    }
}