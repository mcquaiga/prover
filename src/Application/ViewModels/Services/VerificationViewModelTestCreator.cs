using System;
using System.Collections.Generic;
using Application.Services;
using Application.ViewModels.Corrections;
using Application.ViewModels.Volume;
using Application.ViewModels.Volume.Rotary;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Application.ViewModels.Services
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
                    MakeSuperFactorViewModel(device.CreateItemGroup<SuperFactorItems>(), tpViewModel.Temperature,
                        tpViewModel.Pressure)
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

        private void MakeVolumeViewModel(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            //var uncorModel = new UncorrectedVolumeTestViewModel(viewModel.DriveType,
            //    device.CreateItemGroup<VolumeItems>(), device.CreateItemGroup<VolumeItems>());

            //var vm = new VolumeViewModel(device.CreateItemGroup<VolumeItems>(), device.CreateItemGroup<VolumeItems>())
            //{
            //    AppliedInput = 0,
            //    Uncorrected = uncorModel,
            //    Corrected = new CorrectedVolumeTestViewModel(viewModel.DriveType, uncorModel, testPoint,
            //        device.CreateItemGroup<VolumeItems>(), device.CreateItemGroup<VolumeItems>())
            //};

            //return vm;
        }
    }

    public class VolumeViewModelFactory
    {
        public VolumeItems StartVolumeItems { get; }
        public VolumeItems EndVolumeItems { get; }

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

        public static void Create(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var startVolumeItems = device.CreateItemGroup<VolumeItems>();
            var endVolumeItems = device.CreateItemGroup<VolumeItems>();

            VolumeViewModelFactory factory = null;

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

        protected virtual void CreateCorrectedUncorrectedDefault(DeviceInstance device,
            EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var uncorModel = new UncorrectedVolumeTestViewModel(viewModel.DriveType, StartVolumeItems, EndVolumeItems);
            var corModel = new CorrectedVolumeTestViewModel(viewModel.DriveType, uncorModel, testPoint,
                StartVolumeItems, EndVolumeItems);
            testPoint.Volume.Corrected = corModel;
            testPoint.Volume.Uncorrected = uncorModel;
        }

        protected virtual void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
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
        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            base.CreateSpecificTests(device, viewModel, testPoint);
        }

        public MechanicalVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems) : base(
            startVolumeItems, endVolumeItems)
        {
        }
    }

    internal class PulseInputVolumeViewModelFactory : VolumeViewModelFactory
    {
        protected override void CreateSpecificTests(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            base.CreateSpecificTests(device, viewModel, testPoint);
        }

        public PulseInputVolumeViewModelFactory(VolumeItems startVolumeItems, VolumeItems endVolumeItems) : base(
            startVolumeItems, endVolumeItems)
        {
        }
    }
}