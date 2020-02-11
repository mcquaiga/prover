using Devices.Core.Items.ItemGroups;

namespace Application.ViewModels
{
    public class VolumeViewModel
    {
        public IVolumeItems StartItems { get; set; }
        public IVolumeItems EndItems { get; set; }
        public decimal AppliedInput { get; set; }

    }
}