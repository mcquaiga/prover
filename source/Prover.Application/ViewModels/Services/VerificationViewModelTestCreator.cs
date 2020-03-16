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
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
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
        protected VolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems,
            VolumeInputType driveTypeInputType)
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
                    factory = new RotaryVolumeViewModelFactory(startVolumeItems, endVolumeItems, viewModel.DriveType.InputType);
                    break;
                case VolumeInputType.Mechanical:
                    factory = new MechanicalVolumeViewModelFactory(startVolumeItems, endVolumeItems, viewModel.DriveType.InputType);
                    break;
                default:
                    factory = new PulseInputVolumeViewModelFactory(startVolumeItems, endVolumeItems, viewModel.DriveType.InputType);
                    break;
            }

            factory.CreateSpecificTests(device, viewModel, testPoint);
        }

        protected UncorrectedVolumeTestViewModel CreateUncorrectedVolumeTest(IVolumeInputType volumeInputType)
        {
            return new UncorrectedVolumeTestViewModel(volumeInputType, StartVolumeItems, EndVolumeItems);
        }

        protected CorrectedVolumeTestViewModel CreateCorrectedVolumeTest(IVolumeInputType volumeInputType,
            UncorrectedVolumeTestViewModel uncorrectedTest)
        {
            return new CorrectedVolumeTestViewModel(volumeInputType, uncorrectedTest, SharedCalculator,
                StartVolumeItems, EndVolumeItems);
        }

        protected void CreatePulseOutputTests(DeviceInstance device, UncorrectedVolumeTestViewModel uncorModel,
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
        public RotaryVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems,
            VolumeInputType driveTypeInputType) : base(
            startVolumeItems, endVolumeItems, driveTypeInputType)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new RotaryVolumeViewModel(StartVolumeItems, EndVolumeItems)
            {
                Uncorrected = CreateUncorrectedVolumeTest(viewModel.DriveType)
            };
            vm.Corrected = CreateCorrectedVolumeTest(viewModel.DriveType, vm.Uncorrected);

            CreatePulseOutputTests(device, vm.Uncorrected, vm.Corrected);

            var rotary = new RotaryMeterTestViewModel(device.ItemGroup<RotaryMeterItems>());
            vm.RotaryMeterTest = rotary;
            testPoint.TestsCollection.Add(vm);
        }
    }

    internal class MechanicalVolumeViewModelFactory : VolumeViewModelFactory
    {
        public MechanicalVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems,
            VolumeInputType driveTypeInputType) : base(
            startVolumeItems, endVolumeItems, driveTypeInputType)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            //var vm = new MechanicalVolumeViewModel(StartVolumeItems, EndVolumeItems)
            //{
            //    Uncorrected = CreateUncorrectedVolumeTest(viewModel.DriveType)
            //};

            //vm.Corrected = CreateCorrectedVolumeTest(viewModel.DriveType, vm.Uncorrected);
            //CreatePulseOutputTests(device, vm.Uncorrected, vm.Corrected);

           
            //testPoint.TestsCollection.Add(vm);
        }
    }

    internal class PulseInputVolumeViewModelFactory : VolumeViewModelFactory
    {
        public PulseInputVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems,
            VolumeInputType driveTypeInputType) : base(
            startVolumeItems, endVolumeItems, driveTypeInputType)
        {
        }

        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            base.CreateSpecificTests(device, viewModel, testPoint);
        }
    }
}