using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public CorrectedVolumeTestViewModel(UncorrectedVolumeTestViewModel uncorrected,
            ICalculateTrueCorrectedFactor trueCorrectedFactor, VolumeItems startValues, VolumeItems endValues)
            : base(Tolerance, startValues, endValues)
        {
            Uncorrected = uncorrected;
            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, x => x.ExpectedValue,
                    (start, end, expected) => VolumeCalculator.TotalVolume(start.CorrectedReading, end.CorrectedReading))
                .ToPropertyEx(this, x => x.ActualValue)
                .DisposeWith(Cleanup);

            trueCorrectedFactor.TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor, trueCorrectedFactor.TotalCorrectionFactor, deferSubscription: true)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume, x => x.TotalCorrectionFactor, (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.StartValues)
                .Subscribe(v => StartCorrectedReading = v.CorrectedReading);

            this.WhenAnyValue(x => x.EndValues)
                .Subscribe(v => EndCorrectedReading = v.CorrectedReading);
        }

        public UncorrectedVolumeTestViewModel Uncorrected { get; }

        [Reactive] public decimal StartCorrectedReading { get; set; }
        [Reactive] public decimal EndCorrectedReading { get; set; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }
}