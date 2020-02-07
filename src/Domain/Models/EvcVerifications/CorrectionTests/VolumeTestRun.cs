using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Interfaces.Items;
using Domain.Interfaces;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    public class MechanicalVolumeTest : VolumeTest
    {
        public MechanicalVolumeTest(IVolumeItems items, List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalDriveTestLimits) : base(items)
        {
            _mechanicalDriveTestLimits = mechanicalDriveTestLimits;
        }

        private readonly List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> _mechanicalDriveTestLimits;
    }

    public class VolumeTest : IAssertPassFail
    {
        public bool Passed { get; }

        public VolumeTest(IVolumeItems items)
        {
            _items = items;
        }

        private readonly IVolumeItems _items;
    }
}