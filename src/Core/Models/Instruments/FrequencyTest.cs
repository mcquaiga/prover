using System;
using System.Reflection;

namespace Prover.Core.Models.Instruments
{
    public class FrequencyTest : EntityWithId, IHaveVerificationTest, IHavePercentError
    {
        public FrequencyTest() { }

        public FrequencyTest(VerificationTest verificationTest)
        {
            VerificationTest = verificationTest;
            VerificationTestId = verificationTest.Id;
        }       

        [Required]
        public virtual VerificationTest VerificationTest { get; set; }
        public Guid VerificationTestId { get; set; }

        public long MainRotorPulseCount { get; set; }
        public long SenseRotorPulseCount { get; set; }
        public long MechanicalOutputFactor { get; set; }

        private string _itemsType;
        public string ItemsType
        {
            get => PreTestItemValues?.GetType().AssemblyQualifiedName;
            set => _itemsType = value;
        }

        [NotMapped]
        public IFrequencyTestItems PreTestItemValues { get; set; }

        private string _preTestItemData;
        public string PreTestItemData
        {
            get => JsonConvert.SerializeObject(PreTestItemValues);
            set => _preTestItemData = value;
        }

        [NotMapped]
        public IFrequencyTestItems PostTestItemValues { get; set; }

        private string _postTestItemData;
        public string PostTestItemData
        {
            get => JsonConvert.SerializeObject(PostTestItemValues);
            set => _postTestItemData = value;
        }

        public decimal? AdjustedVolumePercentError
        {
            get
            {
                if (AdjustedVolume() == 0) return null;

                var result = (EvcAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
                return result.HasValue ? decimal.Round(result.Value, 2) : default(decimal?);
            }
        }

        public decimal? UnadjustedVolumePercentError
        {
            get
            {
                if (UnadjustedVolume() == 0) return null;

                var result = (EvcUnadjustedVolume() - UnadjustedVolume()) / UnadjustedVolume() * 100;
                return result.HasValue ? decimal.Round(result.Value, 2) : default(decimal?);
            }
        }

        public decimal? PercentError { get; }

        [NotMapped]
        public bool HasPassed => 
            (AdjustedVolumePercentError.HasValue && AdjustedVolumePercentError < 1 && AdjustedVolumePercentError > -1)
        && (UnadjustedVolumePercentError.HasValue && UnadjustedVolumePercentError < 1 && UnadjustedVolumePercentError > -1);

        public decimal AdjustedVolume()
        {
            var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            return decimal.Round(mainAdjVol - senseAdjVol, 4);
        }

        public long RoundedAdjustedVolume()
        {
            var indexRate = (long) VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            var result = (long) (AdjustedVolume() / indexRate);
            return result * indexRate;
        }

        public decimal UnadjustedVolume()
        {
            if (MainRotorPulseCount == 0 || MechanicalOutputFactor == 0) return 0m;

            return decimal.Round((decimal) MainRotorPulseCount / MechanicalOutputFactor, 4);
        }

        public decimal? EvcAdjustedVolume()
        {
            var result = (PostTestItemValues?.AdjustedVolumeReading - PreTestItemValues?.AdjustedVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;

            return result != null ? decimal.Round(result.Value, 4) : default(decimal?);
        }

        public decimal? EvcUnadjustedVolume()
        {
            var result = (PostTestItemValues?.UnadjustVolumeReading - PreTestItemValues?.UnadjustVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            //
            return result != null ? decimal.Round(result.Value, 4) : default(decimal?);
        }
        
        public override void OnInitializing()
        {
            base.OnInitializing();

            if (string.IsNullOrEmpty(_itemsType)) return;

            var type = Type.GetType(_itemsType, name => Assembly.Load(name), (assembly, s, arg3) => assembly.GetType(s)); 
            if (type != null && !string.IsNullOrEmpty(_preTestItemData) && !string.IsNullOrEmpty(_postTestItemData))
            {
                PreTestItemValues = (IFrequencyTestItems) JsonConvert.DeserializeObject(_preTestItemData, type);
                PostTestItemValues = (IFrequencyTestItems) JsonConvert.DeserializeObject(_postTestItemData, type);
            }
        }
    }
}
