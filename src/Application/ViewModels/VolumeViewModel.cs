using Devices.Core.Items.ItemGroups;

namespace Application.ViewModels
{
    public class VolumeViewModel : BaseViewModel
    {
        public VolumeViewModel()
        {

        }

        public VolumeItems StartItems { get; set; }
        public VolumeItems EndItems { get; set; }
        public decimal AppliedInput { get; set; }

    }
}