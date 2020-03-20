using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Interfaces;

namespace Prover.Application.ViewModels.Volume.Factories
{
    public partial class VolumeViewModelFactory
    {
        private void MechanicalVolumeFactory(DeviceInstance device, EvcVerificationViewModel viewModel,
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
}
