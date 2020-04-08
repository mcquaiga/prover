using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Mappers;
using Prover.Application.Services;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.ViewModels.Volume.Factories
{
    public interface IVerificationViewModelFactory
    {
        EvcVerificationViewModel CreateViewModel(EvcVerificationTest evcTest);
    }

    public class VerificationViewModelFactory : IVerificationViewModelFactory
    {
        private readonly ICollection<CorrectionTestDefinition> _testDefinitions = new List<CorrectionTestDefinition>
        {
            new CorrectionTestDefinition
            {
                Level = 0, TemperatureGauge = 32, PressureGaugePercent = 80, IsVolumeTest = true
            },
            new CorrectionTestDefinition
            {
                Level = 1, TemperatureGauge = 60, PressureGaugePercent = 50, IsVolumeTest = false
            },
            new CorrectionTestDefinition
            {
                Level = 2, TemperatureGauge = 90, PressureGaugePercent = 20, IsVolumeTest = false
            }
        };

        public VerificationViewModelFactory(ICollection<CorrectionTestDefinition> testDefinitions = null) =>
            _testDefinitions = testDefinitions ?? _testDefinitions;

        public EvcVerificationViewModel CreateViewModel(EvcVerificationTest evcTest)
        {
            var viewModel = evcTest.ToViewModel();

            var testPoints = _testDefinitions
                .Select(td => BuildTestPointViewModel(evcTest.Device, viewModel, td))
                .ToList();
                
                //.SelectMany(tests => viewModel.Initialize(tests));
            viewModel.Initialize(testPoints);
            return viewModel;
        }

        private VerificationViewModel BuildTestPointViewModel(DeviceInstance device,
            EvcVerificationViewModel viewModel,
            CorrectionTestDefinition definition)
        {
            var tpViewModel = new VerificationTestPointViewModel
            {
                TestNumber = definition.Level
            };

            if (device.HasLivePressure())
                tpViewModel.VerificationTests.Add(
                    MakePressureVieWModel(device.CreateItemGroup<PressureItems>(), definition.PressureGaugePercent));

            if (device.HasLiveTemperature())
                tpViewModel.VerificationTests.Add(
                    MakeTemperatureViewModel(device.CreateItemGroup<TemperatureItems>(), definition.TemperatureGauge));

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

        private PressureFactorViewModel MakePressureVieWModel(PressureItems items, decimal gauge, decimal? atmPressure = null)
        {

            if (items != null)
                return new PressureFactorViewModel(items, gauge, atmPressure ?? items.AtmosphericPressure);

            return null;
        }

        private SuperFactorViewModel MakeSuperFactorViewModel(SuperFactorItems items,
            TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
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