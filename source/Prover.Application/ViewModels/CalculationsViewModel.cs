using System;
using System.Reactive.Linq;
using Core.GasCalculations;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class CalculationsViewModel : ReactiveObject, ICalculateTrueCorrectedFactor
    {
        public VerificationTestPointViewModel TestPoint { get; }

        protected CalculationsViewModel(VerificationTestPointViewModel testPoint)
        {
            //TestPoint = testPoint;

            //Pressure = testPoint.Pressure;
            //Temperature = testPoint.Temperature;
            //SuperFactor = testPoint.SuperFactor;
        }

        public CalculationsViewModel(TemperatureFactorViewModel temperature)
        {
            Temperature = temperature;

            TrueCorrectedObservable = this.WhenAnyValue(x => x.Temperature.ExpectedValue)
                .Select(temp => Calculators.TotalCorrectionFactor(temp, null, null));

            Setup();
        }

        public CalculationsViewModel(PressureFactorViewModel pressure)
        {
            Pressure = pressure;

            TrueCorrectedObservable = this.WhenAnyValue(x => x.Pressure.ExpectedValue)
                .Select(p => Calculators.TotalCorrectionFactor(null, p, null));
            Setup();
        }

        public CalculationsViewModel(PressureFactorViewModel pressure, TemperatureFactorViewModel temperature, SuperFactorViewModel super)
        {
            Pressure = pressure;
            Temperature = temperature;
            SuperFactor = super;

            TrueCorrectedObservable = this.WhenAnyValue(x => x.Pressure.ExpectedValue, x => x.Temperature.ExpectedValue, x => x.SuperFactor.SquaredFactor,
                (p, t, s) => Calculators.TotalCorrectionFactor(t, p, s));

            Setup();
        }

        private void Setup()
        {
            TrueCorrectedObservable
                .ToPropertyEx(this, x => x.TotalCorrectionFactor);
        }

        public SuperFactorViewModel SuperFactor { get; }

        public TemperatureFactorViewModel Temperature { get; }

        public PressureFactorViewModel Pressure { get; }

        public IObservable<decimal> TrueCorrectedObservable { get; }

        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }
}