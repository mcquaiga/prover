using System.Reactive.Linq;
using Application.ViewModels.Corrections;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume
{
    public class CorrectedVolumeTestViewModel : VolumeTestRunViewModel
    {
        private const decimal Tolerance = Global.COR_ERROR_THRESHOLD;

        public CorrectedVolumeTestViewModel(IVolumeInputType driveType, UncorrectedVolumeTestViewModel uncorrected, VerificationTestPointViewModel testPoint, VolumeItems startValues, VolumeItems endValues) 
            : base(Tolerance, driveType, startValues, endValues)
        {
            Uncorrected = uncorrected;
            this.WhenAnyValue(x => x.StartValues, x => x.EndValues, (s, e) => VolumeCalculator.TotalVolume(s.CorrectedReading, e.CorrectedReading))
               .ToPropertyEx(this, x => x.ActualValue);

            this.WhenAnyValue(x => x.Uncorrected.UncorrectedInputVolume).CombineLatest(this.WhenAnyValue(x => x.TotalCorrectionFactor),
                    (input, factor) => VolumeCalculator.TrueCorrected(factor, input))
                .ToPropertyEx(this, x => x.ExpectedValue);

            Pressure = testPoint.Pressure;
            Temperature = testPoint.Temperature;
            SuperFactor = testPoint.SuperFactor;

            this.WhenAnyValue(x => x.Pressure.ExpectedValue, x => x.Temperature.ExpectedValue, x => x.SuperFactor.SquaredFactor,
                    (p, t, s) => Calculators.TotalCorrectionFactor(t, p, s))
                .ToPropertyEx(this, x => x.TotalCorrectionFactor, 0);
        }
        

        public SuperFactorViewModel SuperFactor { get; private set; }

        public TemperatureFactorViewModel Temperature { get; private set; }

        public PressureFactorViewModel Pressure { get; private set; }

        public UncorrectedVolumeTestViewModel Uncorrected { get; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }

}