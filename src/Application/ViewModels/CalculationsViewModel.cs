using System;
using System.Reactive.Linq;
using Application.Interfaces;
using Application.ViewModels.Corrections;
using Core.GasCalculations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public class CalculationsViewModel : ReactiveObject, ICalculateTrueCorrectedFactor
    {
        protected CalculationsViewModel(VerificationTestPointViewModel testPoint)
        {
            //Pressure = testPoint.GetPressureTest();
            //Temperature = testPoint.GetTemperatureTest();
            //SuperFactor = testPoint.GetSuperFactorTest();
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

            TrueCorrectedObservable = this.WhenAnyValue(x => x.Pressure.ExpectedValue,
                x => x.Temperature.ExpectedValue, x => x.SuperFactor.SquaredFactor,
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