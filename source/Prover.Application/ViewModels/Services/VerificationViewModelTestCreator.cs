using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Services;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Domain.EvcVerifications;
using Prover.Shared;

namespace Prover.Application.ViewModels.Services
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

            var compType = device.ItemGroup<SiteInformationItems>().CompositionType;

            if (compType == CompositionType.P || compType == CompositionType.PTZ)

                tpViewModel.TestsCollection.Add(
                    MakePressureVieWModel(device.CreateItemGroup<PressureItems>(), definition.PressureGaugePercent, 0)
                );

            if (compType == CompositionType.T || compType == CompositionType.PTZ)

                tpViewModel.TestsCollection.Add(
                    MakeTemperatureViewModel(device.CreateItemGroup<TemperatureItems>(), definition.TemperatureGauge)
                );

            if (compType == CompositionType.PTZ)

                tpViewModel.TestsCollection.Add(
                    MakeSuperFactorViewModel(device.CreateItemGroup<SuperFactorItems>(),
                        tpViewModel.GetTemperatureTest(),
                        tpViewModel.GetPressureTest())
                );


            if (definition.IsVolumeTest)
                VolumeViewModelFactory.Create(device, viewModel, tpViewModel);
            //MakeVolumeViewModel(device, viewModel, tpViewModel);
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

    public class VolumeViewModelFactory
    {
        protected static CalculationsViewModel SharedCalculator;

        //private static readonly Dictionary<VolumeInputType, Func<>> _volumeInputTypeBuilders =
        //    new Dictionary<VolumeInputType, Func<VolumeInputBuilder>>
        //    {
        //        {VolumeInputType.Rotary, () => new RotaryVolumeInputBuilder()},
        //        {VolumeInputType.Mechanical, () => new MechanicalVolumeInputBuilder()},
        //        {VolumeInputType.PulseInput, () => new PulseInputVolumeBuilder()}
        //    };
        protected VolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems)
        {
            StartVolumeItems = startVolumeItems;
            EndVolumeItems = endVolumeItems;
        }

        public VolumeItems StartVolumeItems { get; }
        public VolumeItems EndVolumeItems { get; }

        public static void Create(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var startVolumeItems = device.CreateItemGroup<VolumeItems>();
            var endVolumeItems = device.CreateItemGroup<VolumeItems>();

            VolumeViewModelFactory factory = null;

            SharedCalculator = GetSharedCalculator(device, testPoint);

            switch (viewModel.DriveType.InputType)
            {
                case VolumeInputType.Rotary:
                    factory = new RotaryVolumeViewModelFactory(startVolumeItems, endVolumeItems);
                    break;
                case VolumeInputType.Mechanical:
                    factory = new MechanicalVolumeViewModelFactory(startVolumeItems, endVolumeItems);
                    break;
                default:
                    factory = new PulseInputVolumeViewModelFactory(startVolumeItems, endVolumeItems);
                    break;
            }

            factory.CreateSpecificTests(device, viewModel, testPoint);
        }

        protected void CreateCorrectedUncorrectedDefault(DeviceInstance device,
            EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var uncorModel = new UncorrectedVolumeTestViewModel(viewModel.DriveType, StartVolumeItems, EndVolumeItems);
            
            var corModel = new CorrectedVolumeTestViewModel(viewModel.DriveType, uncorModel, SharedCalculator,
                StartVolumeItems, EndVolumeItems);

            CreatePulseOutputTests(device, uncorModel, corModel);

            testPoint.GetVolumeTest().Corrected = corModel;
            testPoint.GetVolumeTest().Uncorrected = uncorModel;
        }

        private static void CreatePulseOutputTests(DeviceInstance device, UncorrectedVolumeTestViewModel uncorModel,
            CorrectedVolumeTestViewModel corModel)
        {
            var pulseOutputs = device.ItemGroup<PulseOutputItems>();

            uncorModel.PulseOutputTest = new PulseOutputTestViewModel(
                pulseOutputs.ChannelByUnitType(PulseOutputUnitType.UncVol),
                uncorModel);

            corModel.PulseOutputTest = new PulseOutputTestViewModel(
                pulseOutputs.ChannelByUnitType(PulseOutputUnitType.CorVol),
                corModel);
        }


        protected virtual void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
        }

        protected static CalculationsViewModel GetSharedCalculator(DeviceInstance device,
            VerificationTestPointViewModel testPoint)
        {
            switch (device.ItemGroup<SiteInformationItems>().CompositionType)
            {
                case CompositionType.T:
                    return new CalculationsViewModel(testPoint.GetTemperatureTest());
                case CompositionType.P:
                    return new CalculationsViewModel(testPoint.GetPressureTest());
                case CompositionType.PTZ:
                    return new CalculationsViewModel(testPoint.GetPressureTest(), testPoint.GetTemperatureTest(),
                        testPoint.GetSuperFactorTest());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal class RotaryVolumeViewModelFactory : VolumeViewModelFactory
    {
        public RotaryVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems) : base(
            startVolumeItems, endVolumeItems)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new RotaryVolumeViewModel(StartVolumeItems, EndVolumeItems);
            var rotary = new RotaryMeterTestViewModel(device.ItemGroup<RotaryMeterItems>());

            vm.RotaryMeterTest = rotary;
            testPoint.TestsCollection.Add(vm);

            CreateCorrectedUncorrectedDefault(device, viewModel, testPoint);
        }
    }

    internal class MechanicalVolumeViewModelFactory : VolumeViewModelFactory
    {
        public MechanicalVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems) : base(
            startVolumeItems, endVolumeItems)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            base.CreateSpecificTests(device, viewModel, testPoint);
        }
    }

    internal class PulseInputVolumeViewModelFactory : VolumeViewModelFactory
    {
        public PulseInputVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems) : base(
            startVolumeItems, endVolumeItems)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            base.CreateSpecificTests(device, viewModel, testPoint);
        }
    }
}