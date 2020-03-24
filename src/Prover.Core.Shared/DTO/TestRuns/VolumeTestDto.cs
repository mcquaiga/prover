#region

using System;
using System.Collections.Generic;

#endregion

namespace Prover.Core.Shared.DTO.TestRuns
{
    public class VolumeTestDto : TestDtoBase
    {
        public VolumeTestDto()
        {
        }

        public VolumeTestDto(Guid id, Dictionary<string, string> preTestItemData,
            Dictionary<string, string> postTestItemData, double appliedInput)
            : base(id, null)
        {
            PreTestItemData = preTestItemData;
            PostTestItemData = postTestItemData;
            AppliedInput = appliedInput;
        }

        public double AppliedInput { get; set; }
        public Dictionary<string, string> PostTestItemData { get; }
        public Dictionary<string, string> PreTestItemData { get; }
    }
}