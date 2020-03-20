using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Services;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.ViewModels.Volume.Factories
{
    public class VerificationViewModelTestCreator
    {
        private readonly List<CorrectionTestDefinition> _testDefinitions = new List<CorrectionTestDefinition>
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

        public virtual EvcVerificationViewModel BuildEvcVerificationViewModel(EvcVerificationTest evcTest)
        {
            var viewModel = evcTest.ToViewModel();
            _testDefinitions
                .ForEach(td =>
                    BuildTestPointViewModel(evcTest.Device, viewModel, td)
                );

            return viewModel;
        }

        protected virtual void BuildTestPointViewModel(DeviceInstance device,
            EvcVerificationViewModel viewModel,
            CorrectionTestDefinition definition)
        {
            var tpViewModel = new VerificationTestPointViewModel
            {
                TestNumber = definition.Level
            };

            viewModel.Tests.Add(tpViewModel);

            if (device.HasLivePressure())
                tpViewModel.TestsCollection.Add(
                    MakePressureVieWModel(device.CreateItemGroup<PressureItems>(), definition.PressureGaugePercent,
                        -100));

            if (device.HasLiveTemperature())
                tpViewModel.TestsCollection.Add(
                    MakeTemperatureViewModel(device.CreateItemGroup<TemperatureItems>(), definition.TemperatureGauge));

            if (device.HasLiveSuper())
                tpViewModel.TestsCollection.Add(
                    MakeSuperFactorViewModel(device.CreateItemGroup<SuperFactorItems>(),
                        tpViewModel.GetTemperatureTest(),
                        tpViewModel.GetPressureTest()));

            if (definition.IsVolumeTest)
                VolumeViewModelFactory.Create(device, viewModel, tpViewModel);
        }

        private PressureFactorViewModel MakePressureVieWModel(PressureItems items, decimal gauge, decimal atmPressure)
        {
            if (items != null)
                return new PressureFactorViewModel(items, gauge, atmPressure);

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