using System;
using System.Linq;
using System.Reactive.Linq;
using Core.GasCalculations;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class CalculationsViewModel : ReactiveObject, ICalculateTrueCorrectedFactor
    {
        public VerificationTestPointViewModel TestPoint { get; }

        protected CalculationsViewModel()
        {
            //TestPoint = testPoint;

            //Pressure = testPoint.Pressure;
            //Temperature = testPoint.Temperature;
            //SuperFactor = testPoint.SuperFactor;
        }

        public CalculationsViewModel(TemperatureFactorViewModel temperature)
            
        {
            Temperature = temperature;

            TrueCorrectedObservable = 
                this.WhenAnyValue(x => x.Temperature.ExpectedValue)
                    .Select(temp => Calculators.TotalCorrectionFactor(temp, null, null));

            Setup();
        }

        public CalculationsViewModel(PressureFactorViewModel pressure)
        {
            Pressure = pressure;

            TrueCorrectedObservable = 
                this.WhenAnyValue(x => x.Pressure.ExpectedValue)
                    .Select(p => Calculators.TotalCorrectionFactor(null, p, null));
            Setup();
        }

        public CalculationsViewModel(PressureFactorViewModel pressure, TemperatureFactorViewModel temperature, SuperFactorViewModel super)
        {
            Pressure = pressure;
            Temperature = temperature;
            SuperFactor = super;

            TrueCorrectedObservable = 
                this.WhenAnyValue(x => x.Pressure.ExpectedValue, x => x.Temperature.ExpectedValue, x => x.SuperFactor.SquaredFactor,
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
        public VolumeViewModelBase VolumeViewModel { get; private set; }
        public PressureFactorViewModel Pressure { get; }
        public IObservable<decimal> TrueCorrectedObservable { get; }
        public extern decimal TotalCorrectionFactor { [ObservableAsProperty] get; }
    }

    public class VolumeCalculationsViewModel : CalculationsViewModel
    {
        public VolumeCalculationsViewModel(TemperatureFactorViewModel temperature, VolumeViewModelBase volumeViewModel) : base(temperature)
        {
            Uncorrected = volumeViewModel.AllTests().OfType<UncorrectedVolumeTestViewModel>().FirstOrDefault();
            Corrected = volumeViewModel.AllTests().OfType<CorrectedVolumeTestViewModel>().FirstOrDefault();
        }
        public UncorrectedVolumeTestViewModel Uncorrected { get; private set; }
        public CorrectedVolumeTestViewModel Corrected { get; private set; }

        public void SetupVolumeCalculations(VolumeViewModelBase volumeViewModel)
        {

        }
    }
}