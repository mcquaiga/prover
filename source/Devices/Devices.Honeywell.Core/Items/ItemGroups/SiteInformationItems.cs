using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class SiteInformationItemsHoneywell : SiteInformationItems
    {
        public override CompositionType CompositionType
        {
            get
            {
                if (LivePressureFactor == CorrectionFactorType.Live && LiveTemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.PTZ;

                if (LivePressureFactor == CorrectionFactorType.Live)
                    return CompositionType.P;

                if (LiveTemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.T;

                return CompositionType.Fixed;
            }
        }

        [ItemInfo(122)]
        public override string FirmwareVersion { get; set; }

        [ItemInfo(109)]
        public override CorrectionFactorType LivePressureFactor { get; set; }

        [ItemInfo(62)]
        public override string SerialNumber { get; set; }

        [ItemInfo(200)]
        public override string SiteId1 { get; set; }

        [ItemInfo(201)]
        public override string SiteId2 { get; set; }

        [ItemInfo(110)]
        public override CorrectionFactorType LiveSuperFactor { get; set; }

        [ItemInfo(111)]
        public override CorrectionFactorType LiveTemperatureFactor { get; set; }

    }

    /*
     *
     *     {
            get
            {
                if (PressureFactorLive == CorrectionFactorType.Live &&
                    TemperatureFactorLive == CorrectionFactorType.Live &&
                    SuperFactorLive == CorrectionFactorType.Live)
                    return CompositionType.PTZ;

                if (PressureFactorLive == CorrectionFactorType.Live)
                    return CompositionType.P;

                if (TemperatureFactorLive == CorrectionFactorType.Live)
                    return CompositionType.T;

                return CompositionType.Fixed;
            }
        }
     *
     */
}