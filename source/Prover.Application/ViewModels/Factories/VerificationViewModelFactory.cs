using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Mappers;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Factories.Volume;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Application.ViewModels.Factories
{
    public interface IVerificationViewModelFactory
    {
        EvcVerificationViewModel CreateViewModel(EvcVerificationTest evcTest);
    }

    public class VerificationViewModelFactory : IVerificationViewModelFactory
    {
        private readonly ILoginService _loginService;

        private readonly ICollection<CorrectionTestDefinition> _testDefinitions = new List<CorrectionTestDefinition>
        {
                new CorrectionTestDefinition
                {
                        Level = 0,
                        TemperatureGauge = 32,
                        PressureGaugePercent = 80,
                        IsVolumeTest = true
                },
                new CorrectionTestDefinition
                {
                        Level = 1,
                        TemperatureGauge = 60,
                        PressureGaugePercent = 50,
                        IsVolumeTest = false
                },
                new CorrectionTestDefinition
                {
                        Level = 2,
                        TemperatureGauge = 90,
                        PressureGaugePercent = 20,
                        IsVolumeTest = false
                }
        };

        public VerificationViewModelFactory(ILoginService loginService, ICollection<CorrectionTestDefinition> testDefinitions = null)
        {
            _loginService = loginService;
            _testDefinitions = testDefinitions ?? _testDefinitions;
        }

        public EvcVerificationViewModel CreateViewModel(EvcVerificationTest evcTest)
        {
            var viewModel = evcTest.ToViewModel();

            var testPoints = BuildTestPointViewModel(evcTest.Device, viewModel, _testDefinitions);

            viewModel.Initialize(testPoints.ToList(), _loginService);

            return viewModel;
        }

        private IEnumerable<VerificationViewModel> BuildTestPointViewModel(DeviceInstance device, EvcVerificationViewModel viewModel, ICollection<CorrectionTestDefinition>
                testDefinitions)
        {
            return testDefinitions.Select(td => BuildTestPointViewModel(device, viewModel, td));
        }

        private VerificationViewModel BuildTestPointViewModel(DeviceInstance device, EvcVerificationViewModel viewModel,
                CorrectionTestDefinition definition)
        {
            var tpViewModel = new VerificationTestPointViewModel
            {
                    TestNumber = definition.Level
            };

            if (device.HasLivePressure())
                tpViewModel.VerificationTests.Add(
                        MakePressureVieWModel(device.CreateItemGroup<PressureItems>(),
                                definition.PressureGaugePercent));

            if (device.HasLiveTemperature())
                tpViewModel.VerificationTests.Add(
                        MakeTemperatureViewModel(device.CreateItemGroup<TemperatureItems>(),
                                definition.TemperatureGauge));

            if (device.HasLiveSuper())
                tpViewModel.VerificationTests.Add(
                        MakeSuperFactorViewModel(device.CreateItemGroup<SuperFactorItems>(),
                                tpViewModel.GetTemperatureTest(),
                                tpViewModel.GetPressureTest()));

            if (definition.IsVolumeTest)
                VolumeViewModelFactory.Create(device, viewModel, tpViewModel);

            tpViewModel.Initialize();
            return tpViewModel;
        }

        private PressureFactorViewModel MakePressureVieWModel(PressureItems items, decimal gauge,
                decimal? atmPressure = null)
        {
            if (items != null)
                return new PressureFactorViewModel(items, gauge, atmPressure ?? items.AtmosphericPressure);

            return null;
        }

        private SuperFactorViewModel MakeSuperFactorViewModel(SuperFactorItems items,
                TemperatureFactorViewModel temperature,
                PressureFactorViewModel pressure)
        {
            if (items != null && pressure != null && temperature != null)
                return new SuperFactorViewModel(items, temperature, pressure);

            return null;
        }

        private TemperatureFactorViewModel MakeTemperatureViewModel(TemperatureItems items, decimal gauge)
        {
            if (items != null)
                return new TemperatureFactorViewModel(items, gauge);

            return null;
        }
    }
}