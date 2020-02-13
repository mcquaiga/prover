using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Domain.EvcVerifications.Verifications;

namespace Application.ViewModels
{
    public class VerificationTestPointViewModel : BaseViewModel
    {
        public int TestNumber { get; set; }

        public IEnumerable<ItemValue> BeforeValues { get; set; }

        public IEnumerable<ItemValue> AfterValues { get; set; }

        //public ICollection<CorrectionTestViewModel<ItemGroup>> TestsCollection { get; set; } = new List<CorrectionTestViewModel<ItemGroup>>();
        public ICollection<BaseViewModel> TestsCollection { get; set; } = new List<BaseViewModel>();

        public PressureFactorViewModel Pressure =>
            (PressureFactorViewModel) TestsCollection.FirstOrDefault(t => t is PressureFactorViewModel);

        public TemperatureFactorViewModel Temperature => (TemperatureFactorViewModel) TestsCollection.FirstOrDefault(t => t is TemperatureFactorViewModel);

        public SuperFactorViewModel SuperFactor => (SuperFactorViewModel) TestsCollection.FirstOrDefault(t => t is SuperFactorViewModel);

        public VolumeViewModel Volume => (VolumeViewModel) TestsCollection.FirstOrDefault(t => t is VolumeViewModel);
    }
}