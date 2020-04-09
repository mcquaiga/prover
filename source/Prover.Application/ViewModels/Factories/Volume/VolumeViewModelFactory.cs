using System;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.ViewModels.Volume;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Shared;

namespace Prover.Application.ViewModels.Factories.Volume
{
    public partial class VolumeViewModelFactory
    {
        private readonly VolumeItems _endVolumeItems;
        private readonly CalculationsViewModel _sharedCalculator;

        private readonly VolumeItems _startVolumeItems;

        private VolumeViewModelFactory(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            _startVolumeItems = device.CreateItemGroup<VolumeItems>();
            _endVolumeItems = device.CreateItemGroup<VolumeItems>();
            
            _sharedCalculator = GetSharedCalculator();
            

            CalculationsViewModel GetSharedCalculator()
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

        public static void Create(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var factory = new VolumeViewModelFactory(device, viewModel, testPoint);

            switch (viewModel.DriveType.InputType)
            {
                case VolumeInputType.Rotary:
                    factory.CreateRotaryVolume(device, viewModel, testPoint);
                    break;
                case VolumeInputType.Mechanical:
                    factory.MechanicalVolumeFactory(device, viewModel, testPoint);
                    break;
                case VolumeInputType.PulseInput:
                    factory.PulseInputVolumeFactory(device, viewModel, testPoint);
                    break;
            }
        }

        private CorrectedVolumeTestViewModel CreateCorrectedVolumeTest(IVolumeInputType volumeInputType, UncorrectedVolumeTestViewModel uncorrectedTest)
        {
            return new CorrectedVolumeTestViewModel(uncorrectedTest,
                _sharedCalculator,
                _startVolumeItems, _endVolumeItems);
        }

        private void CreatePulseOutputTests(DeviceInstance device, UncorrectedVolumeTestViewModel uncorModel,
            CorrectedVolumeTestViewModel corModel)
        {
            var pulseOutputs = device.ItemGroup<PulseOutputItems>();

            uncorModel.PulseOutputTest = new PulseOutputTestViewModel(
                pulseOutputs.ChannelByUnitType(PulseOutputType.UncVol),
                uncorModel);

            corModel.PulseOutputTest = new PulseOutputTestViewModel(
                pulseOutputs.ChannelByUnitType(PulseOutputType.CorVol),
                corModel);
        }

        private UncorrectedVolumeTestViewModel CreateUncorrectedVolumeTest(IVolumeInputType volumeInputType) =>
            new UncorrectedVolumeTestViewModel(volumeInputType, _startVolumeItems, _endVolumeItems);

        //private static CalculationsViewModel GetSharedCalculator(DeviceInstance device,
        //    VerificationTestPointViewModel testPoint)
        //{
        //    switch (device.ItemGroup<SiteInformationItems>().CompositionType)
        //    {
        //        case CompositionType.T:
        //            return new CalculationsViewModel(testPoint.GetTemperatureTest());
        //        case CompositionType.P:
        //            return new CalculationsViewModel(testPoint.GetPressureTest());
        //        case CompositionType.PTZ:
        //            return new CalculationsViewModel(testPoint.GetPressureTest(), testPoint.GetTemperatureTest(),
        //                testPoint.GetSuperFactorTest());
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}
    }
}