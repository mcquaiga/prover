using Devices.Core.Items.ItemGroups;
using Prover.Application.Interfaces;
using Prover.Calculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels.Volume
{
    public class CorrectedVolumeTestViewModel : VolumeTestRunViewModelBase
    {
        private const decimal Tolerance = Tolerances.COR_ERROR_THRESHOLD;

        public CorrectedVolumeTestViewModel(IUncorrectedVolumeTestViewModel uncorrected,
            ICalculateTrueCorrectedFactor trueCorrectedFactor, VolumeItems startValues, VolumeItems endValues)
            : base(Tolerance, startValues, endValues)
        {
            Uncorrected = uncorrected;

            this.WhenAnyValue(x => x.StartReading, x => x.EndReading,
                    (start, end) => VolumeCalculator.TotalVolume(start, end, startValues.CorrectedMultiplier))
                .ToPropertyEx(this, x => x.ActualValue)
                .DisposeWith(Cleanup);

            trueCorrectedFactor.TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor, trueCorrectedFactor.TotalCorrectionFactor, true)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume, x => x.TotalCorrectionFactor,
                    (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.StartValues)
                .Subscribe(v => StartReading = v.CorrectedReading);

            this.WhenAnyValue(x => x.EndValues)
                .Subscribe(v => EndReading = v.CorrectedReading);

            this.WhenAnyValue(x => x.StartReading)
                .Subscribe(startReading => StartValues.CorrectedReading = startReading);

            this.WhenAnyValue(x => x.EndReading)
                .Subscribe(endReading => EndValues.CorrectedReading = endReading);

            Multiplier = startValues.CorrectedMultiplier;
        }

        [Reactive] public IUncorrectedVolumeTestViewModel Uncorrected { get; protected set; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }
}